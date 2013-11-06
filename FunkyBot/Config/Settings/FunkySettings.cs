using System;
using Zeta;
using System.IO;
using System.Globalization;
using Zeta.Common;
using System.Xml.Serialization;


namespace FunkyBot.Settings
{
	 
         public class Settings_Funky
         {
				 public SettingDemonBuddy Demonbuddy { get; set; }
				 public SettingDebug Debug { get; set; }
				 public SettingTargeting Targeting { get; set; }
				 public SettingCombat Combat { get; set; }
				 public SettingAvoidance Avoidance { get; set; }

            
             public bool BuyPotionsDuringTownRun { get; set; }
             public bool EnableWaitAfterContainers { get; set; }


             //Character Related


				 public SettingFleeing Fleeing { get; set; }

				 //public int FleeDistance { get; set; }


				 public SettingGrouping Grouping { get; set; }


           
            
             public int AfterCombatDelay { get; set; }


             public bool OutOfCombatMovement { get; set; }
				 public bool AllowBuffingInTown { get; set; }



				 public SettingItemRules ItemRules { get; set; }
				 public SettingLoot Loot { get; set; }
				 public SettingRanges Ranges { get; set; }



            




				 public SettingCluster Cluster { get; set; }

                 public SettingLOSMovement LOSMovement { get; set; }

				 public SettingPlugin Plugin { get; set; }

             //Class Settings
             public ClassSettings Class { get; set; }
             public Settings_Funky()
             {

					  Demonbuddy=new SettingDemonBuddy();
					  Debug=new SettingDebug();
					  Grouping=new SettingGrouping();
					  Fleeing=new SettingFleeing();
					  Ranges=new SettingRanges();
					  ItemRules=new SettingItemRules();
					  Loot=new SettingLoot();
					  Cluster=new SettingCluster();
					  Targeting=new SettingTargeting();
					  Combat=new SettingCombat();
					  Avoidance=new SettingAvoidance();
                      LOSMovement = new SettingLOSMovement();
					  Plugin=new SettingPlugin();
					 

                 BuyPotionsDuringTownRun = false;
					  EnableWaitAfterContainers=false;

                 AfterCombatDelay = 500;
                 OutOfCombatMovement = false;
					  AllowBuffingInTown=false;

                 Class = new ClassSettings();
             }

				 //TODO:: Create Abstract Base Class and Derieved Classes for each D3 Class.
             public class ClassSettings
             {
                 //barb
                 public bool bSelectiveWhirlwind { get; set; }
                 public bool bWaitForWrath { get; set; }
                 public bool bGoblinWrath { get; set; }
                 public bool bFuryDumpWrath { get; set; }
                 public bool bFuryDumpAlways { get; set; }
                 public bool bBarbUseWOTBAlways { get; set; }
                 //DH
                 public int iDHVaultMovementDelay { get; set; }

                 //Monk
                 public bool bMonkInnaSet { get; set; }
                 public bool bMonkSpamMantra { get; set; }
                 public bool bMonkComboStrike { get; set; }
                 public int iMonkComboStrikeAbilities { get; set; }
                 public bool bMonkMaintainSweepingWind { get; set; }

                 //Wiz
                 public bool bWaitForArchon { get; set; }
                 public bool bKiteOnlyArchon { get; set; }
                 public bool bCancelArchonRebuff { get; set; }
                 public bool bTeleportIntoGrouping { get; set; }
                 public bool bTeleportFleeWhenLowHP { get; set; }

                 //WD+Wiz
                 //public bool bEnableCriticalMass { get; set; }

                 //Range Class
                 public int GoblinMinimumRange { get; set; }
				 //Combat All Class
				 public bool AllowDefaultAttackAlways { get; set; }

                 public ClassSettings()
                 {
                     bTeleportIntoGrouping = false;
                     bTeleportFleeWhenLowHP = true;
                     bCancelArchonRebuff = false;
                     bBarbUseWOTBAlways = false;
                     bSelectiveWhirlwind = false;
                     bWaitForWrath = false;
                     bGoblinWrath = false;
                     bFuryDumpWrath = false;
                     bFuryDumpAlways = false;
                     iDHVaultMovementDelay = 400;
                     bMonkInnaSet = false;
                     bMonkSpamMantra = false;
                     bMonkComboStrike = false;
                     bMonkMaintainSweepingWind = false;
                     iMonkComboStrikeAbilities = 0;
                     bWaitForArchon = false;
                     bKiteOnlyArchon = true;
                     GoblinMinimumRange = 40;
					 AllowDefaultAttackAlways = false;
                 }
             }


				 public static void LoadFunkyConfiguration()
				 {
					  string sFunkyCharacterConfigFile=FolderPaths.sFunkySettingsCurrentPath;

					  //Check for Config file
					  if (!File.Exists(sFunkyCharacterConfigFile))
					  {
							Funky.Log("No config file found, now creating a new config from defaults at: "+sFunkyCharacterConfigFile);


							if (Bot.Game.CurrentLevel < 60)
							{
								 Funky.Log("Using Low Level Settings");
								 bool disableBehaviors=true;
								 Bot.Settings=new Settings_Funky
								 {
									  Grouping=new SettingGrouping(disableBehaviors),
									  Cluster=new SettingCluster(disableBehaviors),
									  Fleeing=new SettingFleeing(disableBehaviors),
								 };
							}
							else
							{
								if (Bot.Game.ActorClass == Zeta.Internals.Actors.ActorClass.Barbarian || Bot.Game.ActorClass == Zeta.Internals.Actors.ActorClass.Monk)
								 {
									  Funky.Log("Using Melee Inferno Default Settings");
									  Settings_Funky settings=Settings_Funky.DeserializeFromXML(Path.Combine(FolderPaths.SettingsDefaultPath, "InfernoMelee.xml"));
								 }
								 else
								 {
									  Funky.Log("Using Ranged Inferno Default Settings");
									  Settings_Funky settings=Settings_Funky.DeserializeFromXML(Path.Combine(FolderPaths.SettingsDefaultPath, "InfernoRanged.xml"));
								 }
							}

							Settings_Funky.SerializeToXML(Bot.Settings);
					  }

					  Bot.Settings=Settings_Funky.DeserializeFromXML();
				 }
				 public static void SerializeToXML(Settings_Funky settings)
				 {
					 // Type[] Settings=new Type[] {typeof(SettingCluster),typeof(SettingFleeing),typeof(SettingGrouping),typeof(SettingItemRules),typeof(SettingLoot),typeof(SettingRanges) };
					  XmlSerializer serializer=new XmlSerializer(typeof(Settings_Funky));
					  TextWriter textWriter=new StreamWriter(FolderPaths.sFunkySettingsCurrentPath);
					  serializer.Serialize(textWriter, settings);
					  textWriter.Close();
				 }
				 public static Settings_Funky DeserializeFromXML(string path)
				 {
					  // Type[] Settings=new Type[] { typeof(SettingCluster), typeof(SettingFleeing), typeof(SettingGrouping), typeof(SettingItemRules), typeof(SettingLoot), typeof(SettingRanges) };
					  XmlSerializer deserializer=new XmlSerializer(typeof(Settings_Funky));
					  TextReader textReader=new StreamReader(path);
					  Settings_Funky settings;
					  settings=(Settings_Funky)deserializer.Deserialize(textReader);
					  textReader.Close();

					  return settings;
				 }
				 public static Settings_Funky DeserializeFromXML()
				 {
					  return DeserializeFromXML(FolderPaths.sFunkySettingsCurrentPath);
				 }
         }
}