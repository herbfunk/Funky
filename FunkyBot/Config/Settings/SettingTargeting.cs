using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Settings
{
	public class SettingTargeting
	{
		 public bool IgnoreAboveAverageMobs { get; set; }
		 public bool IgnoreCorpses { get; set; }
		 public bool MissleDampeningEnforceCloseRange { get; set; }
		 public int GoblinPriority { get; set; }
		 public bool[] UseShrineTypes { get; set; }
		 public bool UseExtendedRangeRepChest { get; set; }

		 public bool UnitExceptionLowHP { get; set; }
         public int UnitExceptionLowHPMaximumDistance { get; set; }
		 public bool UnitExceptionRangedUnits { get; set; }
		 public bool UnitExceptionSpawnerUnits { get; set; }
		 public bool UnitExceptionSucideBombers { get; set; }

		 public bool PrioritizeCloseRangeUnits { get; set; }
		 public int PrioritizeCloseRangeMinimumUnits { get; set; }

		 public SettingTargeting()
		 {
			  GoblinPriority=2;
			  UseShrineTypes=new bool[6] { true, true, true, true, true, true };
			  IgnoreAboveAverageMobs=false;
			  PrioritizeCloseRangeUnits=true;
			  PrioritizeCloseRangeMinimumUnits=3;
			  IgnoreCorpses=false;
			  UseExtendedRangeRepChest=false;
			  MissleDampeningEnforceCloseRange=true;
			  UnitExceptionLowHP=true;
              UnitExceptionLowHPMaximumDistance = 20;
			  UnitExceptionRangedUnits=true;
			  UnitExceptionSpawnerUnits=true;
			  UnitExceptionSucideBombers=true;
		 }
		 private static string DefaultFilePath=Path.Combine(FolderPaths.SettingsDefaultPath, "Specific", "Targeting_Default.xml");
		 public static SettingTargeting DeserializeFromXML()
		 {
			  XmlSerializer deserializer=new XmlSerializer(typeof(SettingTargeting));
			  TextReader textReader=new StreamReader(DefaultFilePath);
			  SettingTargeting settings;
			  settings=(SettingTargeting)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
		 public static SettingTargeting DeserializeFromXML(string Path)
		 {
			  XmlSerializer deserializer=new XmlSerializer(typeof(SettingTargeting));
			  TextReader textReader=new StreamReader(Path);
			  SettingTargeting settings;
			  settings=(SettingTargeting)deserializer.Deserialize(textReader);
			  textReader.Close();
			  return settings;
		 }
	}
}
