using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FunkyTrinity.Cache;
using FunkyTrinity.Enums;

namespace FunkyTrinity.Settings
{
	public class SettingAvoidance
	{
		 public bool AttemptAvoidanceMovements { get; set; }
		 public bool UseAdvancedProjectileTesting { get; set; }

		 public AvoidanceValue[] Avoidances { get; set; }

		 public SettingAvoidance()
		 {
			  AttemptAvoidanceMovements=true;
			  UseAdvancedProjectileTesting=false;
			  Avoidances=new AvoidanceValue[21]
			  {
				  new AvoidanceValue(AvoidanceType.ArcaneSentry, 1, 14), 
				  new AvoidanceValue(AvoidanceType.AzmodanBodies, 1, 47),
				  new AvoidanceValue(AvoidanceType.AzmodanFireball, 1, 16),
				  new AvoidanceValue(AvoidanceType.AzmodanPool, 1, 54),
				  new AvoidanceValue(AvoidanceType.BeeProjectile, 0.5, 2), 
				  new AvoidanceValue(AvoidanceType.BelialGround, 1, 25),
				  new AvoidanceValue(AvoidanceType.Dececrator, 1, 9),
				  new AvoidanceValue(AvoidanceType.DiabloMetor, 0.80, 28),
				  new AvoidanceValue(AvoidanceType.DiabloPrison, 1, 15),
				  new AvoidanceValue(AvoidanceType.Frozen, 1, 19), 
				  new AvoidanceValue(AvoidanceType.GrotesqueExplosion, 0.50, 20),
				  new AvoidanceValue(AvoidanceType.LacuniBomb, 0.25, 2),
				  new AvoidanceValue(AvoidanceType.MageFirePool, 1, 10), 
				  new AvoidanceValue(AvoidanceType.MoltenCore, 1, 20), 
				  new AvoidanceValue(AvoidanceType.MoltenTrail, 0.75, 6),
				  new AvoidanceValue(AvoidanceType.PlagueCloud, 0.75, 19),
				  new AvoidanceValue(AvoidanceType.PlagueHand, 1, 15),
				  new AvoidanceValue(AvoidanceType.PoisonGas, 0.5, 9), 
				  new AvoidanceValue(AvoidanceType.ShamanFireBall, 0.1, 2), 
				  new AvoidanceValue(AvoidanceType.SuccubusProjectile, 0.25, 2),
				  new AvoidanceValue(AvoidanceType.TreeSpore, 1, 13), 
			  };
		 }

		 private static string DefaultFilePath=Path.Combine(Funky.FolderPaths.sTrinityPluginPath, "Config", "Defaults", "Avoidance_Default.xml");
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
