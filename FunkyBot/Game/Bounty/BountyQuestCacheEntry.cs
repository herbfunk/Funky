using System;
using System.Collections.Generic;

namespace FunkyBot.Game.Bounty
{
	///<summary>
	///A Cached Entry For A Bounty
	///</summary>
	public class BountyQuestCacheEntry
	{
		#region Properties
		public string Name { get; set; }
		///<summary>
		///The Bounty Type
		///</summary>
		public BountyQuestTypes Type { get; set; }
		///<summary>
		///The Level Area where the Waypoint is.
		///</summary>
		public int StartingLevelAreaID { get; set; }
		///<summary>
		///The Level Area where the quest should finish.
		///</summary>
		public int EndingLevelAreaID { get; set; }
		///<summary>
		///Any Connecting Areas inbetween Start and End
		///</summary>
		public List<int> LevelAreaIDs { get; set; }
		///<summary>
		///For Kill Quests Only
		///</summary>
		public int TotalKillCount { get; set; }

		public int[] QuestSNOs { get; set; }
		#endregion

		#region Constructors
		public BountyQuestCacheEntry()
		{
			Name = String.Empty;
			Type = BountyQuestTypes.None;
			StartingLevelAreaID = -1;
			EndingLevelAreaID = -1;
			TotalKillCount = -1;
			QuestSNOs = null;
		}

		public BountyQuestCacheEntry(string name, int[] Questsnos, BountyQuestTypes type, int startLevelAreaID)
		{
			Name = name;
			Type = type;
			StartingLevelAreaID = startLevelAreaID;
			EndingLevelAreaID = startLevelAreaID;
			TotalKillCount = -1;
			QuestSNOs = Questsnos;
		}
		public BountyQuestCacheEntry(string name, int[] Questsnos, BountyQuestTypes type, int startLevelAreaID, int endLevelAreaID)
		{
			Name = name;
			Type = type;
			StartingLevelAreaID = startLevelAreaID;
			EndingLevelAreaID = endLevelAreaID;
			TotalKillCount = -1;
			QuestSNOs = Questsnos;
		}
		public BountyQuestCacheEntry(string name, int[] Questsnos, BountyQuestTypes type, int startLevelAreaID, int endLevelAreaID, params int[] levelareaids)
		{
			Name = name;
			Type = type;
			StartingLevelAreaID = startLevelAreaID;
			EndingLevelAreaID = endLevelAreaID;
			LevelAreaIDs = new List<int>(levelareaids);
			TotalKillCount = -1;
			QuestSNOs = Questsnos;
		}
		public BountyQuestCacheEntry(string name, int[] Questsnos, BountyQuestTypes type, int startLevelAreaID, int endLevelAreaID, int killCount)
		{
			Name = name;
			Type = type;
			StartingLevelAreaID = startLevelAreaID;
			EndingLevelAreaID = endLevelAreaID;
			TotalKillCount = killCount;
			QuestSNOs = Questsnos;
		}
		public BountyQuestCacheEntry(string name, int[] Questsnos, BountyQuestTypes type, int startLevelAreaID, int endLevelAreaID, int killCount, params int[] levelareaids)
		{
			Name = name;
			Type = type;
			StartingLevelAreaID = startLevelAreaID;
			EndingLevelAreaID = endLevelAreaID;
			TotalKillCount = killCount;
			LevelAreaIDs = new List<int>(levelareaids);
			QuestSNOs = Questsnos;
		}

		#endregion

		private List<int> _levelareaIDs = null;
		public List<int> GetAllLevelIDs
		{
			get
			{
				if (_levelareaIDs == null)
				{
					_levelareaIDs = new List<int> {StartingLevelAreaID};

					if (StartingLevelAreaID != EndingLevelAreaID)
						_levelareaIDs.Add(EndingLevelAreaID);

					if (LevelAreaIDs != null)
						foreach (var levelID in LevelAreaIDs)
						{
							_levelareaIDs.Add(levelID);
						}
				}

				return _levelareaIDs;
			}
		}

	}
}