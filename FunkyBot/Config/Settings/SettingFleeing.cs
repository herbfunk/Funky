using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Settings
{
	public class SettingFleeing
	{
		public bool EnableFleeingBehavior { get; set; }
        public int FailureRetryMilliseconds { get; set; }
		public double FleeBotMinimumHealthPercent { get; set; }
		public int FleeMaxMonsterDistance { get; set; }

        //Units to Flee From!
        public bool FleeUnitElectrified { get; set; }
        public bool FleeUnitRareElite { get; set; }
        public bool FleeUnitNormal { get; set; }
        public bool FleeUnitAboveAverageHitPoints { get; set; }
        public bool FleeUnitIgnoreFast { get; set; }
        public bool FleeUnitIgnoreSucideBomber { get; set; }
        public bool FleeUnitIgnoreRanged { get; set; }

		public SettingFleeing(bool enabled=true)
		{
            FleeUnitIgnoreRanged = true;
            FleeUnitIgnoreSucideBomber = true;
            FleeUnitIgnoreFast = true;
            FleeUnitElectrified = true;
            FleeUnitRareElite = true;
            FleeUnitNormal = true;
            FleeUnitAboveAverageHitPoints = true;
			EnableFleeingBehavior=enabled;
			FleeMaxMonsterDistance=6;
			FleeBotMinimumHealthPercent=0.75d;
            FailureRetryMilliseconds = 2000;
		}
		public SettingFleeing()
		{
            FleeUnitIgnoreRanged = true;
            FleeUnitIgnoreSucideBomber = true;
            FleeUnitIgnoreFast = true;
            FleeUnitElectrified = true;
            FleeUnitRareElite = true;
            FleeUnitNormal = true;
            FleeUnitAboveAverageHitPoints = true;
			EnableFleeingBehavior=false;
			FleeMaxMonsterDistance=3;
			FleeBotMinimumHealthPercent=0.75d;
            FailureRetryMilliseconds = 2000;
		}

		private static string DefaultFilePath=Path.Combine(FolderPaths.SettingsDefaultPath, "Specific", "Fleeing_Default.xml");
		public static SettingFleeing DeserializeFromXML()
		{
			 XmlSerializer deserializer=new XmlSerializer(typeof(SettingFleeing));
			 TextReader textReader=new StreamReader(DefaultFilePath);
			 SettingFleeing settings;
			 settings=(SettingFleeing)deserializer.Deserialize(textReader);
			 textReader.Close();
			 return settings;
		}
		public static SettingFleeing DeserializeFromXML(string Path)
		{
			 XmlSerializer deserializer=new XmlSerializer(typeof(SettingFleeing));
			 TextReader textReader=new StreamReader(Path);
			 SettingFleeing settings;
			 settings=(SettingFleeing)deserializer.Deserialize(textReader);
			 textReader.Close();
			 return settings;
		}
	}
}