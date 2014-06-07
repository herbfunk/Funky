using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace FunkyBot.Game.Bounty
{
	///<summary>
	///This is used to load specific bounty cache files.
	///</summary>
	public class BountyQuestActCache
	{
		public HashSet<BountyQuestCacheEntry> KillBounties { get; set; }
		public HashSet<BountyQuestCacheEntry> ClearBounties { get; set; }
		public HashSet<BountyQuestCacheEntry> CursedEventBounties { get; set; }
		public HashSet<BountyQuestCacheEntry> BossBounties { get; set; } 
		public HashSet<BountyQuestCacheEntry> MiscEventBounties { get; set; }

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

		public BountyQuestActCache()
		{
			KillBounties = new HashSet<BountyQuestCacheEntry>();
			ClearBounties = new HashSet<BountyQuestCacheEntry>();
			CursedEventBounties = new HashSet<BountyQuestCacheEntry>();
			BossBounties = new HashSet<BountyQuestCacheEntry>();
			MiscEventBounties = new HashSet<BountyQuestCacheEntry>();
		}


		private static string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Game", "Bounty", "BountyCache");
		public static void SerializeToXML(BountyQuestActCache settings)
		{
			// Type[] Settings=new Type[] {typeof(SettingCluster),typeof(SettingFleeing),typeof(SettingGrouping),typeof(SettingItemRules),typeof(SettingLoot),typeof(SettingRanges) };
			XmlSerializer serializer = new XmlSerializer(typeof(BountyQuestActCache));
			StreamWriter textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		public static BountyQuestActCache DeserializeFromXML(string fileName)
		{
			string FilePath = DefaultFilePath + @"\" + fileName;
			Logger.DBLog.DebugFormat("Loading Bounty Cache From File {0}", FilePath);

			XmlSerializer deserializer = new XmlSerializer(typeof(BountyQuestActCache));
			TextReader textReader = new StreamReader(FilePath);
			BountyQuestActCache settings;
			settings = (BountyQuestActCache)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}