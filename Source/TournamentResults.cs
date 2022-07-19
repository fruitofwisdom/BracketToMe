using System.Collections.Generic;
using System.ComponentModel;

namespace BracketToMe
{
	public class Result
	{
		public int Seed;
		public string Team;
		public int Score;
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
					if (i < 64)
					{
						winnerIndex = i / 2 + 64;
					}
					else if (i < 96)
					{
						winnerIndex = (i - 64) / 2 + 96;
					}
					else if (i < 112)
					{
						winnerIndex = (i - 96) / 2 + 112;
					}
					else if (i < 120)
					{
						winnerIndex = (i - 112) / 2 + 120;
					}
					else if (i < 124)
					{
						winnerIndex = (i - 120) / 2 + 124;
					}
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

		public void SimulateTournament()
		{
			// Run all the simulations up to the championship.
			for (int i = 0; i < 126; i += 2)
			{
				// Compare WestSeed1 and WestSeed16, etc. Put results in WestWinner1, etc.
				string fieldName1 = FieldNames[i];
				string fieldName2 = FieldNames[i + 1];
				Result result1 = LookUpResults(fieldName1);
				Result result2 = LookUpResults(fieldName2);
				Result finalResult = SimulateGame(result1, result2);
				string finalField = LookUpWinner(fieldName1);
				Results[finalField] = finalResult;
			}

			// Tell the UI our results have changed.
			PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LookUpResults"));
		}

		private Result SimulateGame(Result result1, Result result2)
		{
			// TODO: Calculate scores and winners. Just use the seed for now.
			Result finalResult = new Result();
			float team1Score = 0.0f;
			team1Score = (17.0f - System.Math.Min(result1.Seed, 16.0f)) / 16.0f * 100.0f;
			float team2Score = 0.0f;
			team2Score = (17.0f - System.Math.Min(result2.Seed, 16.0f)) / 16.0f * 100.0f;
			if (team1Score >= team2Score)
			{
				finalResult.Seed = result1.Seed;
				finalResult.Team = result1.Team;
				finalResult.Score = 0;
			}
			else
			{
				finalResult.Seed = result2.Seed;
				finalResult.Team = result2.Team;
				finalResult.Score = 0;
			}
			return finalResult;
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
