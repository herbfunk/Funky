using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Config.Settings.Class
{
	public class SettingBarbarian
	{
		public bool bSelectiveWhirlwind { get; set; }
		public bool bWaitForWrath { get; set; }
		public bool bGoblinWrath { get; set; }
		public bool bFuryDumpWrath { get; set; }
		public bool bFuryDumpAlways { get; set; }
		public bool bBarbUseWOTBAlways { get; set; }

		public SettingBarbarian()
		{
			bBarbUseWOTBAlways = false;
			bSelectiveWhirlwind = false;
			bWaitForWrath = false;
			bGoblinWrath = false;
			bFuryDumpWrath = false;
			bFuryDumpAlways = false;
		}

		internal string DefaultFilePath
		{
			get { return Path.Combine(FolderPaths.SettingsDefaultPath, "Specific", "Demonbuddy_Default.xml"); }
		}
		public SettingBarbarian DeserializeFromXML()
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(SettingBarbarian));
			TextReader textReader = new StreamReader(DefaultFilePath);
			SettingBarbarian settings;
			settings = (SettingBarbarian)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		public SettingBarbarian DeserializeFromXML(string Path)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(SettingBarbarian));
			TextReader textReader = new StreamReader(Path);
			SettingBarbarian settings;
			settings = (SettingBarbarian)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}
