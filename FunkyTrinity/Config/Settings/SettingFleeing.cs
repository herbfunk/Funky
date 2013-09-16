using System.IO;
using System.Xml.Serialization;

namespace FunkyTrinity.Settings
{
	public class SettingFleeing
	{
		public bool EnableFleeingBehavior { get; set; }
		public double FleeBotMinimumHealthPercent { get; set; }
		public int FleeMaxMonsterDistance { get; set; }

		public SettingFleeing(bool enabled=true)
		{
			EnableFleeingBehavior=enabled;
			FleeMaxMonsterDistance=6;
			FleeBotMinimumHealthPercent=0.75d;
		}
		public SettingFleeing()
		{
			EnableFleeingBehavior=false;
			FleeMaxMonsterDistance=3;
			FleeBotMinimumHealthPercent=0.75d;
		}

		private static string DefaultFilePath=Path.Combine(Funky.FolderPaths.SettingsDefaultPath, "Specific", "Fleeing_Default.xml");
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