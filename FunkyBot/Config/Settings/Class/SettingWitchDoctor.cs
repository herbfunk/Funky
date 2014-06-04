using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Config.Settings.Class
{
	public class SettingWitchDoctor
	{
		public bool ZunimassaFullSet { get; set; }
		public SettingWitchDoctor()
		 {
			 ZunimassaFullSet = false;
		 }

		internal string DefaultFilePath
		{
			get { return Path.Combine(FolderPaths.SettingsDefaultPath, "Specific", "Demonbuddy_Default.xml"); }
		}

		 public SettingWitchDoctor DeserializeFromXML()
		 {
			 XmlSerializer deserializer = new XmlSerializer(typeof(SettingWitchDoctor));
			  TextReader textReader=new StreamReader(DefaultFilePath);
			  SettingWitchDoctor settings;
			  settings = (SettingWitchDoctor)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
		 public SettingWitchDoctor DeserializeFromXML(string Path)
		 {
			 XmlSerializer deserializer = new XmlSerializer(typeof(SettingWitchDoctor));
			  TextReader textReader=new StreamReader(Path);
			  SettingWitchDoctor settings;
			  settings = (SettingWitchDoctor)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
	}
}
