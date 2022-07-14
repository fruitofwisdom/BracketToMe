using System.Collections.Generic;
using System.ComponentModel;

namespace BracketToMe
{
	public class TournamentResults : INotifyPropertyChanged
	{
		// TextBoxes will bind into this dictionary, one for each seed and winner, etc. See all
		// possible field names at the bottom.
		public Dictionary<string, string> Fields = new Dictionary<string, string>();

		// This is invoked whenever the results are changed.
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public void Populate(TeamData data)
		{
			// Seed the initial team names. Fields are in the form: WestSeed1, etc.
			for (int i = 0; i < 64; ++i)
			{
				string fieldName = LookUpRegion(i);
				fieldName += "Seed";
				fieldName += LookUpSeed(i);

				Fields.Add(fieldName, data.Teams[i].Name);
			}

			PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LookUpResults"));
		}

		// Convert i (0 to 63) to a region (West, East, South and Midwest).
		private string LookUpRegion(int i)
		{
			string toReturn = "Unknown";
			if (i / 16 == 0)
			{
				toReturn = "West";
			}
			else if (i / 16 == 1)
			{
				toReturn = "East";
			}
			else if (i / 16 == 2)
			{
				toReturn = "South";
			}
			else if (i / 16 == 3)
			{
				toReturn = "Midwest";
			}
			return toReturn;
		}

		// Convert i (0 to 63) to an appropriate seed (1, 16, 8, 9, etc.).
		private string LookUpSeed(int i)
		{
			string toReturn = "0";
			if (i % 16 == 0)
			{
				toReturn = "1";
			}
			else if (i % 16 == 1)
			{
				toReturn = "16";
			}
			else if (i % 16 == 2)
			{
				toReturn = "8";
			}
			else if (i % 16 == 3)
			{
				toReturn = "9";
			}
			else if (i % 16 == 4)
			{
				toReturn = "5";
			}
			else if (i % 16 == 5)
			{
				toReturn = "12";
			}
			else if (i % 16 == 6)
			{
				toReturn = "4";
			}
			else if (i % 16 == 7)
			{
				toReturn = "13";
			}
			else if (i % 16 == 8)
			{
				toReturn = "6";
			}
			else if (i % 16 == 9)
			{
				toReturn = "11";
			}
			else if (i % 16 == 10)
			{
				toReturn = "3";
			}
			else if (i % 16 == 11)
			{
				toReturn = "14";
			}
			else if (i % 16 == 12)
			{
				toReturn = "7";
			}
			else if (i % 16 == 13)
			{
				toReturn = "10";
			}
			else if (i % 16 == 14)
			{
				toReturn = "2";
			}
			else if (i % 16 == 15)
			{
				toReturn = "15";
			}
			return toReturn;
		}

		public string LookUpResults(string field)
		{
			string toReturn = "";
			if (Fields.ContainsKey(field))
			{
				toReturn = Fields[field];
			}
			return toReturn;
		}
	}

	/*
	The full list of seeds is:
	WestSeed1
	WestSeed16
	WestSeed8
	WestSeed9
	WestSeed5
	WestSeed12
	WestSeed4
	WestSeed13
	WestSeed6
	WestSeed11
	WestSeed3
	WestSeed14
	WestSeed7
	WestSeed10
	WestSeed2
	WestSeed15
	WestWinner1
	WestWinner2
	WestWinner3
	WestWinner4
	WestWinner5
	WestWinner6
	WestWinner7
	WestWinner8
	WestSweetSixteen1
	WestSweetSixteen2
	WestSweetSixteen3
	WestSweetSixteen4
	WestEliteEight1
	WestEliteEight2
	EastSeed1
	EastSeed16
	EastSeed8
	EastSeed9
	EastSeed5
	EastSeed12
	EastSeed4
	EastSeed13
	EastSeed6
	EastSeed11
	EastSeed3
	EastSeed14
	EastSeed7
	EastSeed10
	EastSeed2
	EastSeed15
	EastWinner1
	EastWinner2
	EastWinner3
	EastWinner4
	EastWinner5
	EastWinner6
	EastWinner7
	EastWinner8
	EastSweetSixteen1
	EastSweetSixteen2
	EastSweetSixteen3
	EastSweetSixteen4
	EastEliteEight1
	EastEliteEight2
	SouthSeed1
	SouthSeed16
	SouthSeed8
	SouthSeed9
	SouthSeed5
	SouthSeed12
	SouthSeed4
	SouthSeed13
	SouthSeed6
	SouthSeed11
	SouthSeed3
	SouthSeed14
	SouthSeed7
	SouthSeed10
	SouthSeed2
	SouthSeed15
	SouthWinner1
	SouthWinner2
	SouthWinner3
	SouthWinner4
	SouthWinner5
	SouthWinner6
	SouthWinner7
	SouthWinner8
	SouthSweetSixteen1
	SouthSweetSixteen2
	SouthSweetSixteen3
	SouthSweetSixteen4
	SouthEliteEight1
	SouthEliteEight2
	MidwestSeed1
	MidwestSeed16
	MidwestSeed8
	MidwestSeed9
	MidwestSeed5
	MidwestSeed12
	MidwestSeed4
	MidwestSeed13
	MidwestSeed6
	MidwestSeed11
	MidwestSeed3
	MidwestSeed14
	MidwestSeed7
	MidwestSeed10
	MidwestSeed2
	MidwestSeed15
	MidwestWinner1
	MidwestWinner2
	MidwestWinner3
	MidwestWinner4
	MidwestWinner5
	MidwestWinner6
	MidwestWinner7
	MidwestWinner8
	MidwestSweetSixteen1
	MidwestSweetSixteen2
	MidwestSweetSixteen3
	MidwestSweetSixteen4
	MidwestEliteEight1
	MidwestEliteEight2
	WestEastFinalFour
	SouthMidwestFinalFour
	Champion
	*/
}
