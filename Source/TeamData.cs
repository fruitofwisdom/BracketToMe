using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BracketToMe
{
	public class Team
	{
		public int Id;
		public string Name;
		public int Seed;
		public int BpiRank;
		public float BpiOff;
		public float BpiDef;
		public int RecordW;
		public int RecordL;
		public int ConferenceW;
		public int ConferenceL;
		public int VsTop25W;
		public int VsTop25L;
		public int Last10W;
		public int Last10L;
		public int SosRank;
		public int SorRank;
		public float Ppg;
		public float OppPgg;
		public float FieldGoalPer;
		public float ThreePointPer;
		public float FreeThrowPer;

		public Team(string[] fields)
		{
			Id = fields[0] == "" ? 0 : int.Parse(fields[0]);
			Name = fields[1].Trim();
			Seed = fields[2] == "" ? 0 : int.Parse(fields[2]);
			BpiRank = fields[3] == "" ? 0 : int.Parse(fields[3]);
			BpiOff = fields[4] == "" ? 0.0f : float.Parse(fields[4]);
			BpiDef = fields[5] == "" ? 0.0f : float.Parse(fields[5]);
			RecordW = fields[6] == "" ? 0 : int.Parse(fields[6]);
			RecordL = fields[7] == "" ? 0 : int.Parse(fields[7]);
			ConferenceW = fields[8] == "" ? 0 : int.Parse(fields[8]);
			ConferenceL = fields[9] == "" ? 0 : int.Parse(fields[9]);
			VsTop25W = fields[10] == "" ? 0 : int.Parse(fields[10]);
			VsTop25L = fields[11] == "" ? 0 : int.Parse(fields[11]);
			Last10W = fields[12] == "" ? 0 : int.Parse(fields[12]);
			Last10L = fields[13] == "" ? 0 : int.Parse(fields[13]);
			SosRank = fields[14] == "" ? 0 : int.Parse(fields[14]);
			SorRank = fields[15] == "" ? 0 : int.Parse(fields[15]);
			Ppg = fields[16] == "" ? 0.0f : float.Parse(fields[16]);
			OppPgg = fields[17] == "" ? 0.0f : float.Parse(fields[17]);
			FieldGoalPer = fields[18] == "" ? 0.0f : float.Parse(fields[18]);
			ThreePointPer = fields[19] == "" ? 0.0f : float.Parse(fields[19]);
			FreeThrowPer = fields[20] == "" ? 0.0f : float.Parse(fields[20]);
		}
	}

	public class TeamData
	{
		public List<Team> Teams = new List<Team>();

		public Team GetTeam(string teamName)
		{
			Team toReturn = null;

			foreach (Team team in Teams)
			{
				if (team.Name == teamName)
				{
					toReturn = team;
					break;
				}
			}

			return toReturn;
		}

		public async Task<bool> ReadFile(Windows.Storage.StorageFile file)
		{
			IList<string> lines = await Windows.Storage.FileIO.ReadLinesAsync(file);
			foreach (string line in lines)
			{
				string[] fields = line.Split(',');
				Teams.Add(new Team(fields));
			}

			return true;
		}
	}
}
