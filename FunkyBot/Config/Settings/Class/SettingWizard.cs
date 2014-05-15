using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Config.Settings.Class
{
	public class SettingWizard
	{
		public bool bWaitForArchon { get; set; }
		public bool bKiteOnlyArchon { get; set; }
		public bool bCancelArchonRebuff { get; set; }
		public bool bTeleportIntoGrouping { get; set; }
		public bool bTeleportFleeWhenLowHP { get; set; }
		public bool SerpentSparker { get; set; }

		 public SettingWizard()
		 {
			 bTeleportIntoGrouping = false;
			 bTeleportFleeWhenLowHP = true;
			 bCancelArchonRebuff = false;
			 bWaitForArchon = false;
			 bKiteOnlyArchon = true;
			 SerpentSparker = false;
		 }

		 internal string DefaultFilePath
		 {
			 get { return Path.Combine(FolderPaths.SettingsDefaultPath, "Specific", "Demonbuddy_Default.xml"); }
		 }
		 public SettingWizard DeserializeFromXML()
		 {
			 XmlSerializer deserializer = new XmlSerializer(typeof(SettingWizard));
			  TextReader textReader=new StreamReader(DefaultFilePath);
			  SettingWizard settings;
			  settings = (SettingWizard)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
		 public SettingWizard DeserializeFromXML(string Path)
		 {
			 XmlSerializer deserializer = new XmlSerializer(typeof(SettingWizard));
			  TextReader textReader=new StreamReader(Path);
			  SettingWizard settings;
			  settings = (SettingWizard)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
	}
}
