using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Objects
{
	public class BountyDataCollection
	{
		public BountyActCollection ActOne { get; set; }
		public BountyActCollection ActTwo { get; set; }
		public BountyActCollection ActThree { get; set; }
		public BountyActCollection ActFour { get; set; }
		public BountyActCollection ActFive { get; set; }


		public BountyDataCollection()
		{
			ActOne = new BountyActCollection();
			ActTwo = new BountyActCollection();
			ActThree = new BountyActCollection();
			ActFour = new BountyActCollection();
			ActFive = new BountyActCollection();

			//ActOne.KillBounties.Add(new BountyQuestCacheEntry("A1_Kill_LeroicsManor", new[] { 367561, 367559 }, BountyQuestTypes.Kill, 19941, 100854, new[] { 1199, 19943 }));
			//ActOne.ClearBounties.Add(new BountyQuestCacheEntry("A1_Clear_DenOfFallen", new[] { 345488 }, BountyQuestTypes.Clear, 19954, 194232, new[] { 135237 }));
			//ActOne.CursedEventBounties.Add(new BountyQuestCacheEntry("", new[] { 369763 }, BountyQuestTypes.CursedEvent, 93632, 19940));
			//ActOne.BossBounties.Add(new BountyQuestCacheEntry("A1_Boss_SkeletonKing", new[] { 361234 }, BountyQuestTypes.Boss, 19787, 19789));

		}


		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "External", "Dictionaries", "Cache_Bounties.xml");
		public static BountyDataCollection DeserializeFromXML()
		{
			var deserializer = new XmlSerializer(typeof(BountyDataCollection));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (BountyDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		public static BountyDataCollection DeserializeFromXML(string FolderPath)
		{
			if (!Directory.Exists(FolderPath))
				return null;

			string FilePath = Path.Combine(FolderPath, "Cache_Bounties.xml");

			var deserializer = new XmlSerializer(typeof(BountyDataCollection));
			TextReader textReader = new StreamReader(FilePath);
			var settings = (BountyDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		public static void SerializeToXML(BountyDataCollection settings)
		{
			var serializer = new XmlSerializer(typeof(BountyDataCollection));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		public static void SerializeToXML(BountyDataCollection settings, string FolderPath)
		{
			string FilePath = Path.Combine(FolderPath, "Cache_Bounties.xml");
			var serializer = new XmlSerializer(typeof(BountyDataCollection));
			var textWriter = new StreamWriter(FilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}


		

		///<summary>
		///
		///</summary>
		public class BountyQuestCacheEntry
		{
			#region Properties
			public string Name { get; set; }
			///<summary>
			///The Bounty Type
			///</summary>
			public BountyTypes Type { get; set; }
			///<summary>
			///The Level Area where the Waypoint is.
			///</summary>
			public int StartingLevelAreaID { get; set; }
			///<summary>
			///The Level Area where the quest should finish.
			///</summary>
			public int EndingLevelAreaID
			{
				get
				{
					//No specific end level set.. we use start level ID
					if (_endingLevelAreaID == -1 && StartingLevelAreaID != -1)
						_endingLevelAreaID = StartingLevelAreaID;

					return _endingLevelAreaID;
				}
				set { _endingLevelAreaID = value; }
			}

			private int _endingLevelAreaID = -1;

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
				Type = BountyTypes.None;
				StartingLevelAreaID = -1;
				EndingLevelAreaID = -1;
				TotalKillCount = -1;
				QuestSNOs = null;
			}

			public BountyQuestCacheEntry(string name, int[] Questsnos, BountyTypes type, int startLevelAreaID)
			{
				Name = name;
				Type = type;
				StartingLevelAreaID = startLevelAreaID;
				EndingLevelAreaID = startLevelAreaID;
				TotalKillCount = -1;
				QuestSNOs = Questsnos;
			}
			public BountyQuestCacheEntry(string name, int[] Questsnos, BountyTypes type, int startLevelAreaID, int endLevelAreaID)
			{
				Name = name;
				Type = type;
				StartingLevelAreaID = startLevelAreaID;
				EndingLevelAreaID = endLevelAreaID;
				TotalKillCount = -1;
				QuestSNOs = Questsnos;
			}
			public BountyQuestCacheEntry(string name, int[] Questsnos, BountyTypes type, int startLevelAreaID, int endLevelAreaID, params int[] levelareaids)
			{
				Name = name;
				Type = type;
				StartingLevelAreaID = startLevelAreaID;
				EndingLevelAreaID = endLevelAreaID;
				LevelAreaIDs = new List<int>(levelareaids);
				TotalKillCount = -1;
				QuestSNOs = Questsnos;
			}
			public BountyQuestCacheEntry(string name, int[] Questsnos, BountyTypes type, int startLevelAreaID, int endLevelAreaID, int killCount)
			{
				Name = name;
				Type = type;
				StartingLevelAreaID = startLevelAreaID;
				EndingLevelAreaID = endLevelAreaID;
				TotalKillCount = killCount;
				QuestSNOs = Questsnos;
			}
			public BountyQuestCacheEntry(string name, int[] Questsnos, BountyTypes type, int startLevelAreaID, int endLevelAreaID, int killCount, params int[] levelareaids)
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
						_levelareaIDs = new List<int> { StartingLevelAreaID };

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

		///<summary>
		///
		///</summary>
		public class BountyActCollection
		{
			public HashSet<BountyQuestCacheEntry> KillBounties { get; set; }
			public HashSet<BountyQuestCacheEntry> ClearBounties { get; set; }
			public HashSet<BountyQuestCacheEntry> CursedEventBounties { get; set; }
			public HashSet<BountyQuestCacheEntry> BossBounties { get; set; }
			public HashSet<BountyQuestCacheEntry> MiscEventBounties { get; set; }

			public BountyActCollection()
			{
				KillBounties = new HashSet<BountyQuestCacheEntry>();
				ClearBounties = new HashSet<BountyQuestCacheEntry>();
				CursedEventBounties = new HashSet<BountyQuestCacheEntry>();
				BossBounties = new HashSet<BountyQuestCacheEntry>();
				MiscEventBounties = new HashSet<BountyQuestCacheEntry>();
			}

			public List<BountyQuestCacheEntry> AllBounties
			{
				get
				{
					var _allBounties = new List<BountyQuestCacheEntry>();
					_allBounties.AddRange(KillBounties);
					_allBounties.AddRange(ClearBounties);
					_allBounties.AddRange(CursedEventBounties);
					_allBounties.AddRange(BossBounties);
					_allBounties.AddRange(MiscEventBounties);

					return _allBounties;
				}
			}
		}
	}
}
