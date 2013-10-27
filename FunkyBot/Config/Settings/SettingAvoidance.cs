using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FunkyBot.Avoidances;
using FunkyBot.Cache;


namespace FunkyBot.Settings
{
	public class SettingAvoidance
	{
		 public bool AttemptAvoidanceMovements { get; set; }
		 public bool UseAdvancedProjectileTesting { get; set; }
         public int FailureRetryMilliseconds { get; set; }

		 [XmlArray]
		 public AvoidanceValue[] Avoidances { get { return avoidances; } set { avoidances=value; } }

		 private AvoidanceValue[] avoidances;

		 public SettingAvoidance()
		 {
			  AttemptAvoidanceMovements=true;
			  UseAdvancedProjectileTesting=false;
			  Avoidances=AvoidanceCache.AvoidancesDefault;
              FailureRetryMilliseconds = 2000;
		 }

		 private static string DefaultFilePath=Path.Combine(FolderPaths.SettingsDefaultPath,"Specific","Avoidance_Default.xml");
		 public static SettingAvoidance DeserializeFromXML()
		 {
			  XmlSerializer deserializer=new XmlSerializer(typeof(SettingAvoidance));
			  TextReader textReader=new StreamReader(DefaultFilePath);
			  SettingAvoidance settings;
			  settings=(SettingAvoidance)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
		 public static SettingAvoidance DeserializeFromXML(string Path)
		 {
			  XmlSerializer deserializer=new XmlSerializer(typeof(SettingAvoidance));
			  TextReader textReader=new StreamReader(Path);
			  SettingAvoidance settings;
			  settings=(SettingAvoidance)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
	}
}
