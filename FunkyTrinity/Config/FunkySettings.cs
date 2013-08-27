using System;
using Zeta;
using System.IO;
using System.Globalization;
using Zeta.Common;
using System.Xml.Serialization;
using FunkyTrinity.Enums;

namespace FunkyTrinity
{
	 public partial class Funky
	 {

   
         public class Settings_Funky
         {
             public bool DebugStatusBar { get; set; }
             public bool LogSafeMovementOutput { get; set; }
				 public bool LogGroupingOutput { get; set; }
				 public LogLevel FunkyLogFlags { get; set; }

				 public bool SkipAhead { get; set; }

				 public bool StopGameOnBotLowHealth { get; set; }
				 public double StopGameOnBotHealthPercent { get; set; }
				 public bool StopGameOnBotEnableScreenShot { get; set; }
				 public int StopGameScreenShotWindowWidth { get; set; }
				 public int StopGameScreenShotWindowHeight { get; set; }
             
             public bool LogStuckLocations { get; set; }
             public bool EnableUnstucker { get; set; }
             public bool RestartGameOnLongStucks { get; set; }
             
             public bool OOCIdentifyItems { get; set; }
             public bool BuyPotionsDuringTownRun { get; set; }
             public bool EnableWaitAfterContainers { get; set; }
             public bool UseExtendedRangeRepChest { get; set; }
             public bool EnableCoffeeBreaks { get; set; }
             public int MinBreakTime { get; set; }
             public int MaxBreakTime { get; set; }
             public int OOCIdentifyItemsMinimumRequired { get; set; }
             public double breakTimeHour { get; set; }
				 public bool[] UseShrineTypes { get; set; }

             //Character Related
             public bool AttemptAvoidanceMovements { get; set; }
             public bool UseAdvancedProjectileTesting { get; set; }

				 public bool EnableFleeingBehavior { get; set; }
				 public double FleeBotMinimumHealthPercent { get; set; }
				 public int FleeMaxMonsterDistance { get; set; }

             public int KiteDistance { get; set; }


				 public bool AttemptGroupingMovements { get; set; }
				 public double GroupingClusterRadiusDistance { get; set; }
				 public int GroupingMinimumUnitDistance { get; set; }
				 public int GroupingMaximumDistanceAllowed { get; set; }
				 public int GroupingMinimumClusterCount { get; set; }
				 public int GroupingMinimumUnitsInCluster { get; set; }

             public double GlobeHealthPercent { get; set; }
             public double PotionHealthPercent { get; set; }
             public bool IgnoreCorpses { get; set; }
             public int GoblinPriority { get; set; }
             public int AfterCombatDelay { get; set; }
             public bool IgnoreAboveAverageMobs { get; set; }
             public bool OutOfCombatMovement { get; set; }
				 public bool AllowBuffingInTown { get; set; }
				 public bool MissleDampeningEnforceCloseRange { get; set; }

             //new additions
             public bool IgnoreCombatRange { get; set; }
             public bool IgnoreLootRange { get; set; }
             public int ItemRange { get; set; }
             public int GoldRange { get; set; }
				 public int GlobeRange { get; set; }
             public int ShrineRange { get; set; }
             public int DestructibleRange { get; set; }
             public int ContainerOpenRange { get; set; }
             public int NonEliteCombatRange { get; set; }
             public int EliteCombatRange { get; set; }
             public int TreasureGoblinRange { get; set; }
				 //public int ExtendedCombatRange { get; set; }


				 public bool UseItemRulesPickup { get; set; }
				 public bool UseItemRules { get; set; }
             public bool ItemRulesSalvaging { get; set; }
				 public bool ItemRulesUnidStashing { get; set; }
             public bool ItemRuleUseItemIDs { get; set; }
             public bool ItemRuleDebug { get; set; }
             public string ItemRuleType { get; set; }
				 public string ItemRuleCustomPath { get; set; }
             public string ItemRuleLogPickup { get; set; }
             public string ItemRuleLogKeep { get; set; }
             public bool ItemRuleGilesScoring { get; set; }

             public bool UseLevelingLogic { get; set; }

             //Plugin Item Default Settings
             public int GilesMinimumWeaponScore { get; set; }
             public int GilesMinimumArmorScore { get; set; }
             public int GilesMinimumJeweleryScore { get; set; }

             public int[] MinimumWeaponItemLevel { get; set; }
             public int[] MinimumArmorItemLevel { get; set; }
             public int[] MinimumJeweleryItemLevel { get; set; }
             public int MinimumLegendaryItemLevel { get; set; }

             public int MaximumHealthPotions { get; set; }
             public int MinimumGoldPile { get; set; }

             //red, green, purple, yellow
             public bool[] PickupGems { get; set; }
             public int MinimumGemItemLevel { get; set; }

             public bool PickupCraftTomes { get; set; }
             public bool PickupCraftPlans { get; set; }

				 public bool PickupBlacksmithPlanSix { get; set; }
				 public bool PickupBlacksmithPlanFive { get; set; }
				 public bool PickupBlacksmithPlanFour { get; set; }
				 public bool PickupBlacksmithPlanArchonGauntlets { get; set; }
				 public bool PickupBlacksmithPlanArchonSpaulders { get; set; }
				 public bool PickupBlacksmithPlanRazorspikes { get; set; }

				 public bool PickupJewelerDesignFlawlessStar { get; set; }
				 public bool PickupJewelerDesignPerfectStar { get; set; }
				 public bool PickupJewelerDesignRadiantStar { get; set; }
				 public bool PickupJewelerDesignMarquise { get; set; }
				 public bool PickupJewelerDesignAmulet { get; set; }

             public bool PickupFollowerItems { get; set; }
				 public bool PickupInfernalKeys { get; set; }
				 public bool PickupDemonicEssence { get; set; }

             public int MiscItemLevel { get; set; }

             public int AvoidanceRecheckMaximumRate { get; set; }//=1250;
             public int AvoidanceRecheckMinimumRate { get; set; }//=500;
             public int KitingRecheckMaximumRate { get; set; }//=4000;
             public int KitingRecheckMinimumRate { get; set; }//=2000;

             public double ClusterDistance { get; set; }
             public int ClusterMinimumUnitCount { get; set; }
             public bool EnableClusteringTargetLogic { get; set; }
             public bool ClusterKillLowHPUnits { get; set; }
				 public bool ClusteringAllowRangedUnits { get; set; }
				 public bool ClusteringAllowSpawnerUnits { get; set; }
             public bool IgnoreClusteringWhenLowHP { get; set; }
             public double IgnoreClusterLowHPValue { get; set; }

             //Class Settings
             public ClassSettings Class { get; set; }
             public Settings_Funky()
             {
					  DebugStatusBar=true;
					  LogSafeMovementOutput=false;
					  LogGroupingOutput=false;
					  FunkyLogFlags=LogLevel.OutOfCombat| LogLevel.OutOfGame | LogLevel.User;

					  AttemptGroupingMovements=true;
					  GroupingClusterRadiusDistance=10d;
					  GroupingMinimumUnitDistance=35;
					  GroupingMaximumDistanceAllowed=110;
					  GroupingMinimumClusterCount=1;
					  GroupingMinimumUnitsInCluster=3;
					 

					  EnableFleeingBehavior=true;
					  FleeMaxMonsterDistance=6;
					  FleeBotMinimumHealthPercent=0.75d;
					  EnableUnstucker=true;
					  RestartGameOnLongStucks=true;
					  LogStuckLocations=true;
					  UseShrineTypes=new bool[6] { true, true, true, true, true, true };
					  ItemRuleCustomPath="";
                 AvoidanceRecheckMaximumRate = 3500;
                 AvoidanceRecheckMinimumRate = 550;
                 KitingRecheckMaximumRate = 4500;
                 KitingRecheckMinimumRate = 1000;
                 OOCIdentifyItems = false;
                 BuyPotionsDuringTownRun = false;
					  EnableWaitAfterContainers=false;
                 UseItemRulesPickup = true;
					  UseItemRules=true;
					  ItemRulesUnidStashing=true;
					  ItemRulesSalvaging=true;
                 UseExtendedRangeRepChest = false;
					  EnableCoffeeBreaks=false;
                 MaxBreakTime = 4;
                 MinBreakTime = 8;
                 OOCIdentifyItemsMinimumRequired = 10;
                 breakTimeHour = 1.5d;
                 ItemRuleDebug = false;
                 ItemRuleUseItemIDs = false;
                 ItemRuleType = "hard";
                 ItemRuleLogKeep = "Rare";
					  ItemRuleLogPickup="Rare";
                 ItemRuleGilesScoring = true;
					  AttemptAvoidanceMovements=true;
                 KiteDistance = 0;
                 IgnoreCombatRange = false;
                 IgnoreLootRange = false;
                 DestructibleRange = 10;
                 ContainerOpenRange = 30;
                 NonEliteCombatRange = 45;
                 GlobeHealthPercent = 0.6d;
                 PotionHealthPercent = 0.5d;
                 IgnoreCorpses = false;
                 GoblinPriority = 2;
                 AfterCombatDelay = 500;
                 OutOfCombatMovement = false;
					  AllowBuffingInTown=false;
                 EliteCombatRange = 60;
                 GoldRange = 45;
					  GlobeRange=40;
                 ItemRange = 75;
                 TreasureGoblinRange = 55;
                 ShrineRange = 30;
                 MinimumWeaponItemLevel = new int[] { 0, 59 };
                 MinimumArmorItemLevel = new int[] { 0, 59 };
                 MinimumJeweleryItemLevel = new int[] { 0, 55 };
                 GilesMinimumWeaponScore = 75000;
                 GilesMinimumArmorScore = 30000;
                 GilesMinimumJeweleryScore = 30000;

                 MinimumLegendaryItemLevel = 59;
                 MaximumHealthPotions = 100;
                 MinimumGoldPile = 425;

					  PickupGems=new bool[] { true, true, true, true };

                 MinimumGemItemLevel = 60;
					  PickupCraftTomes=true;
					  PickupCraftPlans=true;
					  PickupBlacksmithPlanSix=true;
					  PickupBlacksmithPlanFive=false;
					  PickupBlacksmithPlanFour=false;
					  PickupBlacksmithPlanRazorspikes=false;
					  PickupBlacksmithPlanArchonGauntlets=false;
					  PickupBlacksmithPlanArchonSpaulders=false;
					  PickupJewelerDesignFlawlessStar=false;
					  PickupJewelerDesignPerfectStar=false;
					  PickupJewelerDesignRadiantStar=false;
					  PickupJewelerDesignMarquise=false;
					  PickupJewelerDesignAmulet=false;
					  PickupInfernalKeys=true;
					  PickupDemonicEssence=true;

					  PickupFollowerItems=true;
                 MiscItemLevel = 59;
                 UseLevelingLogic = false;
                 UseAdvancedProjectileTesting = false;
                 IgnoreAboveAverageMobs = false;



                 EnableClusteringTargetLogic = true;
                 IgnoreClusteringWhenLowHP = true;
                 ClusterKillLowHPUnits = true;
                 ClusterDistance = 7d;
                 ClusterMinimumUnitCount = 3;
                 IgnoreClusterLowHPValue = 0.55d;
					  ClusteringAllowRangedUnits=true;
					  ClusteringAllowSpawnerUnits=true;

					  SkipAhead=true;

					  StopGameOnBotEnableScreenShot=true;
					  StopGameOnBotLowHealth=false;
					  StopGameOnBotHealthPercent=0.10d;
					  StopGameScreenShotWindowWidth=1000;
					  StopGameScreenShotWindowHeight=800;

					  MissleDampeningEnforceCloseRange=true;

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
					  string sFunkyCharacterConfigFile=FolderPaths.sFunkySettingsCurrentPath;

					  //Check for Config file
					  if (!File.Exists(sFunkyCharacterConfigFile))
					  {
							Log("No config file found, now creating a new config from defaults at: "+sFunkyCharacterConfigFile);
							Bot.SettingsFunky=new Settings_Funky();
							Settings_Funky.SerializeToXML(Bot.SettingsFunky);
					  }

					  Bot.SettingsFunky=Settings_Funky.DeserializeFromXML();
				 }
				 public static void SerializeToXML(Settings_Funky settings)
				 {
					  XmlSerializer serializer=new XmlSerializer(typeof(Settings_Funky));
					  TextWriter textWriter=new StreamWriter(FolderPaths.sFunkySettingsCurrentPath);
					  serializer.Serialize(textWriter, settings);
					  textWriter.Close();
				 }
				 public static Settings_Funky DeserializeFromXML()
				 {
					  XmlSerializer deserializer=new XmlSerializer(typeof(Settings_Funky));
					  TextReader textReader=new StreamReader(FolderPaths.sFunkySettingsCurrentPath);
					  Settings_Funky settings;
					  settings=(Settings_Funky)deserializer.Deserialize(textReader);
					  textReader.Close();

					  return settings;
				 }
         }
    }
}