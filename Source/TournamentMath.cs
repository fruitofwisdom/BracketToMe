using System;

namespace BracketToMe
{

	public static class Weights
	{
		// When calculating a score, how much should each be weighted?
		public static float PpgWeight = 0.5f;
		public static float CalculatedWeight = 0.5f;
		// For the BPI factor, how much should each be weighted?
		public static float SeedWeight = 0.25f;
		public static float BpiRankWeight = 0.25f;
		public static float BpiOffWeight = 0.3f;
		public static float BpiDefWeight = 0.2f;
		// For the record factor, how much should each be weighted?
		public static float RecordWeight = 0.3f;
		public static float ConferenceWeight = 0.2f;
		public static float VsTop25Weight = 0.05f;
		public static float Last10Weight = 0.15f;
		public static float SosRankWeight = 0.1f;
		public static float SorRankWeight = 0.2f;
		// How much should the BPI and record factor into a team's performance?
		public static float BpiFactorWeight = 0.6f;
		public static float RecordFactorWeight = 0.4f;
		// How much should these weights affect a team's calculated score?
		public static float OverallFactorWeight = 0.1f;
		// How many attempts of each type per game should be considered?
		// NOTE: NCAA standard has been 50% of all points from field goals, 30% three-pointers,
		// and 20% free throws. These values give roughly that approximation.
		public static int FieldGoalAttempts = 37;
		public static int ThreePointAttempts = 22;
		public static int FreeThrowAttempts = 20;
	}

	public class TournamentMath
	{
		public static (int team1Score, int team2Score) SimulateGame(Team team1, Team team2)
		{
			// Calculate various "factors" to predict a team's performance. A team's BPI factor is
			// a combination of its conference seed, its BPI rank, and its component BPI offensive
			// and defensive score. A team's record factor is a combination of its total win/loss
			// record, its conference record, its record against top-25 teams, its record from its
			// last ten games, its strength-of-schedule rank, and its strength-of-record rank.
			// These are combined into a +/- effect on their performance when calculating a
			// predicted game score below.
			float team1BpiFactor = CalculateBpiFactor(team1);
			float team2BpiFactor = CalculateBpiFactor(team2);
			float team1RecordFactor = CalculateRecordFactor(team1);
			float team2RecordFactor = CalculateRecordFactor(team2);
			float team1WeightedFactor =
				1.0f +
				((team1BpiFactor * Weights.BpiFactorWeight + team1RecordFactor * Weights.RecordFactorWeight) * 2.0f - 100.0f) /
				(100.0f / Weights.OverallFactorWeight);
			float team2WeightedFactor =
				1.0f +
				((team2BpiFactor * Weights.BpiFactorWeight + team2RecordFactor * Weights.RecordFactorWeight) * 2.0f - 100.0f) /
				(100.0f / Weights.OverallFactorWeight);

			// Calculate the final score of the game. This is a combination of historic points-
			// per-game combined with opposing-points-per-game and a calculated score based on
			// shots attempted and performance-weighted historic percentages.  The team with the
			// highest score is, of course, the winner for the simulation.
			(int team1Score, int team2Score) = CalculateScore(team1, team2, team1WeightedFactor, team2WeightedFactor);

			// Ties aren't allowed, so prefer the team with the higher weighted factor.
			if (team1Score == team2Score)
			{
				if (team1WeightedFactor > team2WeightedFactor)
				{
					team1Score++;
				}
				else
				{
					team2Score++;
				}
			}

			return (team1Score, team2Score);
		}

		private static float CalculateBpiFactor(Team team)
		{
			return
				(17.0f - Math.Min(team.Seed, 16.0f)) / 16.0f * 100.0f * Weights.SeedWeight +
				(129.0f - Math.Min(team.BpiRank, 128.0f)) / 128.0f * 100.0f * Weights.BpiRankWeight +
				Math.Max(Math.Min(team.BpiOff + 7.0f, 28.0f), 1.0f) / 28.0f * 100.0f * Weights.BpiOffWeight +
				Math.Max(Math.Min(team.BpiDef + 7.0f, 28.0f), 1.0f) / 28.0f * 100.0f * Weights.BpiDefWeight;
		}

		private static float CalculateRecordFactor(Team team)
		{
			return
				Math.Max(Math.Min(team.RecordW - team.RecordL / 2.0f, 32.0f), 0.0f) / 32.0f * 100.0f * Weights.RecordWeight +
				Math.Max(Math.Min(team.ConferenceW - team.ConferenceL / 2.0f, 18.0f), 0.0f) / 18.0f * 100.0f * Weights.ConferenceWeight +
				Math.Max(Math.Min(team.VsTop25W - team.VsTop25L / 2.0f, 7.0f), 0.0f) / 7.0f * 100.0f * Weights.VsTop25Weight +
				Math.Max(Math.Min(team.Last10W - team.Last10L / 2.0f, 7.0f), 0.0f) / 7.0f * 100.0f * Weights.Last10Weight +
				(251.0f - Math.Min(team.SosRank, 250.0f)) / 250.0f * 50.0f * Weights.SosRankWeight +
				(251.0f - Math.Min(team.SorRank, 250.0f)) / 250.0f * 50.0f * Weights.SorRankWeight;
		}

		private static (int, int) CalculateScore(Team team1, Team team2, float team1WeightedFactor, float team2WeightedFactor)
		{
			// We'll use two methods to predict the score of a game. First, a simple average
			// between one team's points-per-game and the other team's opposing points-per-game.
			float team1PpgScore = (team1.Ppg + team2.OppPgg) / 2.0f;
			float team2PpgScore = (team2.Ppg + team1.OppPgg) / 2.0f;

			// Second, we'll calculate each team's score based on predicted successful field goal,
			// three-point, and free throw attempts.
			float team1FieldGoalPoints = Weights.FieldGoalAttempts * (team1.FieldGoalPer / 100.0f) * 2.0f;
			float team1ThreePointPoints = Weights.ThreePointAttempts * (team1.ThreePointPer / 100.0f) * 3.0f;
			float team1FreeThrowPoints = Weights.FreeThrowAttempts * (team1.FreeThrowPer / 100.0f);
			float team1CalculatedPoints = team1FieldGoalPoints + team1ThreePointPoints + team1FreeThrowPoints;
			float team2FieldGoalPoints = Weights.FieldGoalAttempts * (team2.FieldGoalPer / 100.0f) * 2.0f;
			float team2ThreePointPoints = Weights.ThreePointAttempts * (team2.ThreePointPer / 100.0f) * 3.0f;
			float team2FreeThrowPoints = Weights.FreeThrowAttempts * (team2.FreeThrowPer / 100.0f);
			float team2CalculatedPoints = team2FieldGoalPoints + team2ThreePointPoints + team2FreeThrowPoints;

			// Then we'll average the two based on their respective weights.
			float team1BlendedScore =
				team1PpgScore * Weights.PpgWeight +
				team1CalculatedPoints * Weights.CalculatedWeight;
			float team2BlendedScore =
				team2PpgScore * Weights.PpgWeight +
				team2CalculatedPoints * Weights.CalculatedWeight;

			// Lastly, we'll scale the scores based on the relative strength of the teams.
			int team1Score = (int)Math.Round(team1BlendedScore * team1WeightedFactor);
			int team2Score = (int)Math.Round(team2BlendedScore * team2WeightedFactor);

			return (team1Score, team2Score);
		}
	}
}
