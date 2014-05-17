using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Config.Settings
{
	public class SettingRanges
	{
		public bool IgnoreCombatRange { get; set; }
		public bool IgnoreLootRange { get; set; }
		public bool IgnoreProfileBlacklists { get; set; }

		public int ItemRange { get; set; }
		public int GoldRange { get; set; }
		public int GlobeRange { get; set; }
		public int PotionRange { get; set; }
		public int ShrineRange { get; set; }
		public int DoorRange { get; set; }
		public int DestructibleRange { get; set; }
		public int ContainerOpenRange { get; set; }
		public int NonEliteCombatRange { get; set; }
		public int EliteCombatRange { get; set; }
		public int TreasureGoblinRange { get; set; }

		public int PoolsOfReflectionRange { get; set; }
		public int CursedShrineRange { get; set; }
		public int CursedChestRange { get; set; }

		public SettingRanges()
		{
			IgnoreCombatRange=false;
			IgnoreLootRange=false;
			IgnoreProfileBlacklists=false;
			DestructibleRange=0;
			ContainerOpenRange=30;
			NonEliteCombatRange=45;
			EliteCombatRange=60;
			GoldRange=45;
			GlobeRange=40;
			DoorRange=30;
			PotionRange = 45;
			ItemRange=150;
			TreasureGoblinRange=55;
			ShrineRange=30;
			PoolsOfReflectionRange = 75;
			CursedShrineRange = 75;
			CursedChestRange = 75;
		}

		private static string DefaultFilePath=Path.Combine(FolderPaths.SettingsDefaultPath, "Specific", "Range_Default.xml");
		public static SettingRanges DeserializeFromXML()
		{
			 XmlSerializer deserializer=new XmlSerializer(typeof(SettingRanges));
			 TextReader textReader=new StreamReader(DefaultFilePath);
			 SettingRanges settings;
			 settings=(SettingRanges)deserializer.Deserialize(textReader);
			 textReader.Close();
			 return settings;
		}
		public static SettingRanges DeserializeFromXML(string Path)
		{
			 XmlSerializer deserializer=new XmlSerializer(typeof(SettingRanges));
			 TextReader textReader=new StreamReader(Path);
			 SettingRanges settings;
			 settings=(SettingRanges)deserializer.Deserialize(textReader);
			 textReader.Close();
			 return settings;
		}
	}
}