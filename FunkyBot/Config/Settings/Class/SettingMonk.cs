using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Config.Settings.Class
{
	public class SettingMonk
	{

		public bool bMonkInnaSet { get; set; }
		public bool bMonkSpamMantra { get; set; }
		public bool bMonkComboStrike { get; set; }
		public int iMonkComboStrikeAbilities { get; set; }
		public bool bMonkMaintainSweepingWind { get; set; }

		public SettingMonk()
		{
			bMonkInnaSet = false;
			bMonkSpamMantra = false;
			bMonkComboStrike = false;
			bMonkMaintainSweepingWind = false;
			iMonkComboStrikeAbilities = 0;
		}

		internal string DefaultFilePath
		{
			get { return Path.Combine(FolderPaths.SettingsDefaultPath, "Specific", "Demonbuddy_Default.xml"); }
		}
		public SettingMonk DeserializeFromXML()
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(SettingMonk));
			TextReader textReader = new StreamReader(DefaultFilePath);
			SettingMonk settings;
			settings = (SettingMonk)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		public SettingMonk DeserializeFromXML(string Path)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(SettingMonk));
			TextReader textReader = new StreamReader(Path);
			SettingMonk settings;
			settings = (SettingMonk)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}
