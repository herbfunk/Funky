using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Settings
{
	public class SettingDemonBuddy
	{
		 public bool EnableDemonBuddyCharacterSettings { get; set; }
		 public int MonsterPower { get; set; }
		 public int NewGameMinimumWaitTime { get; set; }
		 public int NewGameMaxmimumWaitTime { get; set; }

		 public SettingDemonBuddy()
		 {
			  EnableDemonBuddyCharacterSettings=false;
			  MonsterPower=0;
			  NewGameMinimumWaitTime=0;
			  NewGameMaxmimumWaitTime=0;
		 }

		 private static string DefaultFilePath=Path.Combine(FolderPaths.SettingsDefaultPath, "Specific", "Demonbuddy_Default.xml");
		 public static SettingDemonBuddy DeserializeFromXML()
		 {
			  XmlSerializer deserializer=new XmlSerializer(typeof(SettingDemonBuddy));
			  TextReader textReader=new StreamReader(DefaultFilePath);
			  SettingDemonBuddy settings;
			  settings=(SettingDemonBuddy)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
		 public static SettingDemonBuddy DeserializeFromXML(string Path)
		 {
			  XmlSerializer deserializer=new XmlSerializer(typeof(SettingDemonBuddy));
			  TextReader textReader=new StreamReader(Path);
			  SettingDemonBuddy settings;
			  settings=(SettingDemonBuddy)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
	}
}
