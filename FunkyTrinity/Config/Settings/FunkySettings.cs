using System;
using Zeta;
using System.IO;
using System.Globalization;
using Zeta.Common;
using System.Xml.Serialization;
using FunkyTrinity.Enums;

namespace FunkyTrinity.Settings
{
	 
         public class Settings_Funky
         {
				 public SettingDemonBuddy Demonbuddy { get; set; }
				 public SettingDebug Debug { get; set; }
				 public SettingTargeting Targeting { get; set; }
				 public SettingCombat Combat { get; set; }
				 public SettingAvoidance Avoidance { get; set; }
				 public bool StopGameOnBotLowHealth { get; set; }
				 public double StopGameOnBotHealthPercent { get; set; }
				 public bool StopGameOnBotEnableScreenShot { get; set; }
				 public int StopGameScreenShotWindowWidth { get; set; }
				 public int StopGameScreenShotWindowHeight { get; set; }
             

             
             public bool OOCIdentifyItems { get; set; }
             public bool BuyPotionsDuringTownRun { get; set; }
             public bool EnableWaitAfterContainers { get; set; }
            
             public bool EnableCoffeeBreaks { get; set; }
             public int MinBreakTime { get; set; }
             public int MaxBreakTime { get; set; }
             public int OOCIdentifyItemsMinimumRequired { get; set; }
             public double breakTimeHour { get; set; }


             //Character Related


				 public SettingFleeing Fleeing { get; set; }

				 //public int FleeDistance { get; set; }


				 public SettingGrouping Grouping { get; set; }


           
            
             public int AfterCombatDelay { get; set; }


             public bool OutOfCombatMovement { get; set; }
				 public bool AllowBuffingInTown { get; set; }

				 public bool UseLevelingLogic { get; set; }
				


				 public SettingItemRules ItemRules { get; set; }
				 public SettingLoot Loot { get; set; }
				 public SettingRanges Ranges { get; set; }



            



             public int AvoidanceRecheckMaximumRate { get; set; }//=1250;
             public int AvoidanceRecheckMinimumRate { get; set; }//=500;
             public int KitingRecheckMaximumRate { get; set; }//=4000;
             public int KitingRecheckMinimumRate { get; set; }//=2000;

				 public SettingCluster Cluster { get; set; }

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
					  
					 
                 AvoidanceRecheckMaximumRate = 3500;
                 AvoidanceRecheckMinimumRate = 550;
                 KitingRecheckMaximumRate = 4500;
                 KitingRecheckMinimumRate = 1000;
                 OOCIdentifyItems = false;
                 BuyPotionsDuringTownRun = false;
					  EnableWaitAfterContainers=false;

                
					  EnableCoffeeBreaks=false;
                 MaxBreakTime = 4;
                 MinBreakTime = 8;
                 OOCIdentifyItemsMinimumRequired = 10;
                 breakTimeHour = 1.5d;



                

                 AfterCombatDelay = 500;
                 OutOfCombatMovement = false;
					  AllowBuffingInTown=false;


                 UseLevelingLogic = false;
                




					  StopGameOnBotEnableScreenShot=true;
					  StopGameOnBotLowHealth=false;
					  StopGameOnBotHealthPercent=0.10d;
					  StopGameScreenShotWindowWidth=1000;
					  StopGameScreenShotWindowHeight=800;

					 


                 Class = new ClassSettings();
             }


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

                 public ClassSettings()
                 {
							bTeleportIntoGrouping=false;
							bTeleportFleeWhenLowHP=true;
							bCancelArchonRebuff=false;
							bBarbUseWOTBAlways=false;
                     bSelectiveWhirlwind = false;
                     bWaitForWrath = false;
                     bGoblinWrath = false;
                     bFuryDumpWrath = false;
                     bFuryDumpAlways = false;
                     iDHVaultMovementDelay = 400;
                     bMonkInnaSet = false;
							bMonkSpamMantra=false;
                     bWaitForArchon = false;
                     bKiteOnlyArchon = true;
                     GoblinMinimumRange = 40;
                 }
             }


				 public static void LoadFunkyConfiguration()
				 {
					  string sFunkyCharacterConfigFile=Funky.FolderPaths.sFunkySettingsCurrentPath;

					  //Check for Config file
					  if (!File.Exists(sFunkyCharacterConfigFile))
					  {
							Funky.Log("No config file found, now creating a new config from defaults at: "+sFunkyCharacterConfigFile);


							if (Bot.CurrentLevel<60)
							{
								 bool disableBehaviors=true;
								 Bot.SettingsFunky=new Settings_Funky
								 {
									  Grouping=new SettingGrouping(disableBehaviors),
									  Cluster=new SettingCluster(disableBehaviors),
									  Fleeing=new SettingFleeing(disableBehaviors),
									  UseLevelingLogic=!disableBehaviors,
								 };
							}
							else
							{
								 if (Bot.ActorClass==Zeta.Internals.Actors.ActorClass.Barbarian||Bot.ActorClass==Zeta.Internals.Actors.ActorClass.Monk)
								 {
									  Settings_Funky settings=Settings_Funky.DeserializeFromXML(Path.Combine(Funky.FolderPaths.SettingsDefaultPath, "InfernoMelee.xml"));
								 }
								 else
								 {
									  Settings_Funky settings=Settings_Funky.DeserializeFromXML(Path.Combine(Funky.FolderPaths.SettingsDefaultPath, "InfernoRanged.xml"));
								 }
							}

							Settings_Funky.SerializeToXML(Bot.SettingsFunky);
					  }

					  Bot.SettingsFunky=Settings_Funky.DeserializeFromXML();
				 }
				 public static void SerializeToXML(Settings_Funky settings)
				 {
					 // Type[] Settings=new Type[] {typeof(SettingCluster),typeof(SettingFleeing),typeof(SettingGrouping),typeof(SettingItemRules),typeof(SettingLoot),typeof(SettingRanges) };
					  XmlSerializer serializer=new XmlSerializer(typeof(Settings_Funky));
					  TextWriter textWriter=new StreamWriter(Funky.FolderPaths.sFunkySettingsCurrentPath);
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
					  return DeserializeFromXML(Funky.FolderPaths.sFunkySettingsCurrentPath);
				 }
         }
}