using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Settings
{
	public class SettingDemonHunter
	{
		public int iDHVaultMovementDelay { get; set; }

		public SettingDemonHunter()
		{
			iDHVaultMovementDelay = 400;
		}

		internal string DefaultFilePath
		{
			get { return Path.Combine(FolderPaths.SettingsDefaultPath, "Specific", "Demonbuddy_Default.xml"); }
		}
		public SettingDemonHunter DeserializeFromXML()
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(SettingDemonHunter));
			TextReader textReader = new StreamReader(DefaultFilePath);
			SettingDemonHunter settings;
			settings = (SettingDemonHunter)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		public SettingDemonHunter DeserializeFromXML(string Path)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(SettingDemonHunter));
			TextReader textReader = new StreamReader(Path);
			SettingDemonHunter settings;
			settings = (SettingDemonHunter)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}
