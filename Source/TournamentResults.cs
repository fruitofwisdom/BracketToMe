using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BracketToMe
{
	public struct Result
	{
		public int Seed;
		public string Team;
		public int Score;
	}

	public static class Weights
	{
		public static float PpgWeight = 0.5f;
		public static float CalculatedWeight = 0.5f;
		public static float SeedWeight = 0.25f;
		public static float BpiRankWeight = 0.25f;
		public static float BpiOffWeight = 0.3f;
		public static float BpiDefWeight = 0.2f;
		public static float RecordWeight = 0.3f;
		public static float ConferenceWeight = 0.2f;
		public static float VsTop25Weight = 0.05f;
		public static float Last10Weight = 0.15f;
		public static float SosRankWeight = 0.1f;
		public static float SorRankWeight = 0.2f;
		public static float BpiFactorWeight = 0.6f;
		public static float RecordFactorWeight = 0.4f;
		public static float OverallFactorWeight = 0.1f;
		public static int FieldGoalAttempts = 60;
		public static int ThreePointAttempts = 20;
		public static int FreeThrowAttempts = 20;
		public static int OtherPoints = 25;
	}

	public class TournamentResults : INotifyPropertyChanged
	{
		// ResultControls will bind into this dictionary, one for each seed and winner, etc. See
		// all possible field names at the bottom.
		public Dictionary<string, Result> Results = new Dictionary<string, Result>();

		// This is invoked whenever the results are changed.
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public void Populate(TeamData data)
		{
			Results.Clear();

			// Seed the initial team names. Fields are in the form: WestSeed1, etc.
			for (int i = 0; i < 64; ++i)
			{
				string fieldName = FieldNames[i];
				Result result = new Result
				{
					Seed = data.Teams[i].Seed,
					Team = data.Teams[i].Name,
					Score = 0
				};
				Results.Add(fieldName, result);
			}

			// Tell the UI our results have changed.
			PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LookUpResults"));
		}

		private string LookUpWinner(string fieldName1)
		{
			string toReturn = "Unknown";
			for (int i = 0; i < FieldNames.Length; ++i)
			{
				if (FieldNames[i] == fieldName1)
				{
					int winnerIndex = 0;
					// All 64 teams play in the first round.
					if (i < 64)
					{
						winnerIndex = i / 2 + 64;
					}
					// Then 32 teams play in the second round.
					else if (i < 96)
					{
						winnerIndex = (i - 64) / 2 + 96;
					}
					// Then the Sweet Sixteen.
					else if (i < 112)
					{
						winnerIndex = (i - 96) / 2 + 112;
					}
					// Then the Elite Eight.
					else if (i < 120)
					{
						winnerIndex = (i - 112) / 2 + 120;
					}
					// Then the Final Four.
					else if (i < 124)
					{
						winnerIndex = (i - 120) / 2 + 124;
					}
					// Then the championship game.
					else if (i < 126)
					{
						winnerIndex = 126;
					}
					toReturn = FieldNames[winnerIndex];
					break;
				}
			}
			return toReturn;
		}

		// Run all the simulations up to the championship.
		public void SimulateTournament(TeamData data)
		{
			// For example, compare WestSeed1 and WestSeed16 and put the results in WestWinner1.
			for (int i = 0; i < 126; i += 2)
			{
				string team1FieldName = FieldNames[i];
				string team2FieldName = FieldNames[i + 1];
				string winnerFieldName = LookUpWinner(team1FieldName);
				Result team1Result = Results[team1FieldName];
				Result team2Result = Results[team2FieldName];
				Result winnerResult = SimulateGame(ref team1Result, ref team2Result, data);
				// NOTE: Scores are calculated and kept in the "previous" results.
				Results[team1FieldName] = team1Result;
				Results[team2FieldName] = team2Result;
				Results[winnerFieldName] = winnerResult;
			}

			// Tell the UI our results have changed.
			PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LookUpResults"));
		}

		// Run a simulation of one game and return the winner.
		private Result SimulateGame(ref Result team1Result, ref Result team2Result, TeamData data)
		{
			Team team1 = data.GetTeam(team1Result.Team);
			Team team2 = data.GetTeam(team2Result.Team);

			// Calculate various "factors" to predict a team's performance. A team's BPI factor is
			// a combination of its conference seed, its BPI rank, and its component BPI offensive
			// and defensive score. A team's record factor is a combination of its total win/loss
			// record, its conference record, its record against top-25 teams, its record from its
			// last ten games, its strength-of-schedule rank, and its strength-of-record rank.
			// These are combined into a +/- effect on their performance when calculating a
			// predicted game score below.
			float team1BpiFactor =
				(17.0f - Math.Min(team1.Seed, 16.0f)) / 16.0f * 100.0f * Weights.SeedWeight +
				(129.0f - Math.Min(team1.BpiRank, 128.0f)) / 128.0f * 100.0f * Weights.BpiRankWeight +
				Math.Max(Math.Min(team1.BpiOff + 7.0f, 28.0f), 1.0f) / 28.0f * 100.0f * Weights.BpiOffWeight +
				Math.Max(Math.Min(team1.BpiDef + 7.0f, 28.0f), 1.0f) / 28.0f * 100.0f * Weights.BpiDefWeight;
			float team2BpiFactor =
				(17.0f - Math.Min(team2.Seed, 16.0f)) / 16.0f * 100.0f * Weights.SeedWeight +
				(129.0f - Math.Min(team2.BpiRank, 128.0f)) / 128.0f * 100.0f * Weights.BpiRankWeight +
				Math.Max(Math.Min(team2.BpiOff + 7.0f, 28.0f), 1.0f) / 28.0f * 100.0f * Weights.BpiOffWeight +
				Math.Max(Math.Min(team2.BpiDef + 7.0f, 28.0f), 1.0f) / 28.0f * 100.0f * Weights.BpiDefWeight;
			float team1RecordFactor =
				Math.Max(Math.Min(team1.RecordW - team1.RecordL / 2.0f, 32.0f), 0.0f) / 32.0f * 100.0f * Weights.RecordWeight +
				Math.Max(Math.Min(team1.ConferenceW - team1.ConferenceL / 2.0f, 18.0f), 0.0f) / 18.0f * 100.0f * Weights.ConferenceWeight +
				Math.Max(Math.Min(team1.VsTop25W - team1.VsTop25L / 2.0f, 7.0f), 0.0f) / 7.0f * 100.0f * Weights.VsTop25Weight +
				Math.Max(Math.Min(team1.Last10W - team1.Last10L / 2.0f, 7.0f), 0.0f) / 7.0f * 100.0f * Weights.Last10Weight +
				(251.0f - Math.Min(team1.SosRank, 250.0f)) / 250.0f * 50.0f * Weights.SosRankWeight +
				(251.0f - Math.Min(team1.SorRank, 250.0f)) / 250.0f * 50.0f * Weights.SorRankWeight;
			float team2RecordFactor =
				Math.Max(Math.Min(team2.RecordW - team2.RecordL / 2.0f, 32.0f), 0.0f) / 32.0f * 100.0f * Weights.RecordWeight +
				Math.Max(Math.Min(team2.ConferenceW - team2.ConferenceL / 2.0f, 18.0f), 0.0f) / 18.0f * 100.0f * Weights.ConferenceWeight +
				Math.Max(Math.Min(team2.VsTop25W - team2.VsTop25L / 2.0f, 7.0f), 0.0f) / 7.0f * 100.0f * Weights.VsTop25Weight +
				Math.Max(Math.Min(team2.Last10W - team2.Last10L / 2.0f, 7.0f), 0.0f) / 7.0f * 100.0f * Weights.Last10Weight +
				(251.0f - Math.Min(team2.SosRank, 250.0f)) / 250.0f * 50.0f * Weights.SosRankWeight +
				(251.0f - Math.Min(team2.SorRank, 250.0f)) / 250.0f * 50.0f * Weights.SorRankWeight;
			float team1WeightedFactor =
				1.0f + ((team1BpiFactor * Weights.BpiFactorWeight + team1RecordFactor * Weights.RecordFactorWeight) * 2.0f - 100.0f) / (100.0f / Weights.OverallFactorWeight);
			float team2WeightedFactor =
				1.0f + ((team2BpiFactor * Weights.BpiFactorWeight + team2RecordFactor * Weights.RecordFactorWeight) * 2.0f - 100.0f) / (100.0f / Weights.OverallFactorWeight);

			// Calculate the final score of the game. This is a combination of historic points-
			// per-game combined with opposing-points-per-game and a calculated score based on
			// shots attempted and performance-weighted historic percentages.  The team with the
			// highest score is, of course, the winner for the simulation.
			float team1PpgScore = (team1.Ppg + team2.OppPgg) / 2.0f;
			float team2PpgScore = (team2.Ppg + team1.OppPgg) / 2.0f;
			float team1CalculatedPoints =
				(Weights.FieldGoalAttempts * team1.FieldGoalPer * team1WeightedFactor / 100.0f) +
				(Weights.ThreePointAttempts * team1.ThreePointPer * team1WeightedFactor / 100.0f) +
				(Weights.FreeThrowAttempts * team1.FreeThrowPer * team1WeightedFactor / 100.0f) +
				Weights.OtherPoints;
			float team2CalculatedPoints =
				(Weights.FieldGoalAttempts * team2.FieldGoalPer * team2WeightedFactor / 100.0f) +
				(Weights.ThreePointAttempts * team2.ThreePointPer * team2WeightedFactor / 100.0f) +
				(Weights.FreeThrowAttempts * team2.FreeThrowPer * team2WeightedFactor / 100.0f) +
				Weights.OtherPoints;
			int team1Score = (int)Math.Round(
				team1PpgScore * Weights.PpgWeight +
				team1CalculatedPoints * Weights.CalculatedWeight);
			int team2Score = (int)Math.Round(
				team2PpgScore * Weights.PpgWeight +
				team2CalculatedPoints * Weights.CalculatedWeight);

			// Create new, updated Results to prevent binding cache issues.
			team1Result = new Result
			{
				Seed = team1Result.Seed,
				Team = team1Result.Team,
				Score = team1Score
			};
			team2Result = new Result
			{
				Seed = team2Result.Seed,
				Team = team2Result.Team,
				Score = team2Score
			};
			Result winnerResult = new Result
			{
				Seed = team1Score >= team2Score ? team1Result.Seed : team2Result.Seed,
				Team = team1Score >= team2Score ? team1Result.Team : team2Result.Team,
				Score = 0       // Will be calculated next round.
			};
			return winnerResult;
		}

		public Result LookUpResults(string fieldName)
		{
			Result toReturn = new Result();
			if (Results.ContainsKey(fieldName))
			{
				toReturn = Results[fieldName];
			}
			return toReturn;
		}

		// The full list of field names is:
		private static readonly string[] FieldNames = new string[]
		{
			"WestSeed1",
			"WestSeed16",
			"WestSeed8",
			"WestSeed9",
			"WestSeed5",
			"WestSeed12",
			"WestSeed4",
			"WestSeed13",
			"WestSeed6",
			"WestSeed11",
			"WestSeed3",
			"WestSeed14",
			"WestSeed7",
			"WestSeed10",
			"WestSeed2",
			"WestSeed15",
			"EastSeed1",
			"EastSeed16",
			"EastSeed8",
			"EastSeed9",
			"EastSeed5",
			"EastSeed12",
			"EastSeed4",
			"EastSeed13",
			"EastSeed6",
			"EastSeed11",
			"EastSeed3",
			"EastSeed14",
			"EastSeed7",
			"EastSeed10",
			"EastSeed2",
			"EastSeed15",
			"SouthSeed1",
			"SouthSeed16",
			"SouthSeed8",
			"SouthSeed9",
			"SouthSeed5",
			"SouthSeed12",
			"SouthSeed4",
			"SouthSeed13",
			"SouthSeed6",
			"SouthSeed11",
			"SouthSeed3",
			"SouthSeed14",
			"SouthSeed7",
			"SouthSeed10",
			"SouthSeed2",
			"SouthSeed15",
			"MidwestSeed1",
			"MidwestSeed16",
			"MidwestSeed8",
			"MidwestSeed9",
			"MidwestSeed5",
			"MidwestSeed12",
			"MidwestSeed4",
			"MidwestSeed13",
			"MidwestSeed6",
			"MidwestSeed11",
			"MidwestSeed3",
			"MidwestSeed14",
			"MidwestSeed7",
			"MidwestSeed10",
			"MidwestSeed2",
			"MidwestSeed15",
			"WestWinner1",
			"WestWinner2",
			"WestWinner3",
			"WestWinner4",
			"WestWinner5",
			"WestWinner6",
			"WestWinner7",
			"WestWinner8",
			"EastWinner1",
			"EastWinner2",
			"EastWinner3",
			"EastWinner4",
			"EastWinner5",
			"EastWinner6",
			"EastWinner7",
			"EastWinner8",
			"SouthWinner1",
			"SouthWinner2",
			"SouthWinner3",
			"SouthWinner4",
			"SouthWinner5",
			"SouthWinner6",
			"SouthWinner7",
			"SouthWinner8",
			"MidwestWinner1",
			"MidwestWinner2",
			"MidwestWinner3",
			"MidwestWinner4",
			"MidwestWinner5",
			"MidwestWinner6",
			"MidwestWinner7",
			"MidwestWinner8",
			"WestSweetSixteen1",
			"WestSweetSixteen2",
			"WestSweetSixteen3",
			"WestSweetSixteen4",
			"EastSweetSixteen1",
			"EastSweetSixteen2",
			"EastSweetSixteen3",
			"EastSweetSixteen4",
			"SouthSweetSixteen1",
			"SouthSweetSixteen2",
			"SouthSweetSixteen3",
			"SouthSweetSixteen4",
			"MidwestSweetSixteen1",
			"MidwestSweetSixteen2",
			"MidwestSweetSixteen3",
			"MidwestSweetSixteen4",
			"WestEliteEight1",
			"WestEliteEight2",
			"EastEliteEight1",
			"EastEliteEight2",
			"SouthEliteEight1",
			"SouthEliteEight2",
			"MidwestEliteEight1",
			"MidwestEliteEight2",
			"WestFinalFour",
			"EastFinalFour",
			"SouthFinalFour",
			"MidwestFinalFour",
			"WestEastChampion",
			"SouthMidwestChampion",
			"Champion",
		};
	}
}
