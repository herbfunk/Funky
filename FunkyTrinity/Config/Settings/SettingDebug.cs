using System.IO;
using System.Xml.Serialization;

namespace FunkyTrinity.Settings
{
	public class SettingDebug
	{
		 public bool DebugStatusBar { get; set; }
		 public bool LogGroupingOutput { get; set; }
		 public LogLevel FunkyLogFlags { get; set; }
		 public bool SkipAhead { get; set; }
		 public bool LogStuckLocations { get; set; }
		 public bool EnableUnstucker { get; set; }
		 public bool RestartGameOnLongStucks { get; set; }

		 public SettingDebug()
		 {
			  DebugStatusBar=true;
			  FunkyLogFlags=LogLevel.All;
			  EnableUnstucker=true;
			  RestartGameOnLongStucks=true;
			  LogStuckLocations=true;
			  SkipAhead=true;
		 }

		 private static string DefaultFilePath=Path.Combine(Funky.FolderPaths.SettingsDefaultPath, "Specific", "Debug_Default.xml");
		 public static SettingDebug DeserializeFromXML()
		 {
			  XmlSerializer deserializer=new XmlSerializer(typeof(SettingDebug));
			  TextReader textReader=new StreamReader(DefaultFilePath);
			  SettingDebug settings;
			  settings=(SettingDebug)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
		 public static SettingDebug DeserializeFromXML(string Path)
		 {
			  XmlSerializer deserializer=new XmlSerializer(typeof(SettingDebug));
			  TextReader textReader=new StreamReader(Path);
			  SettingDebug settings;
			  settings=(SettingDebug)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
	}
}
