using System;
using Zeta;
using System.IO;
using System.Globalization;
using Zeta.Common;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
         private static void SaveFunkyConfiguration()
         {

             string sFunkyCharacterConfigFile = Path.Combine(FolderPaths.sDemonBuddyPath, "Settings", "FunkyTrinity", CurrentAccountName, CurrentHeroName + ".cfg");

             FileStream configStream = File.Open(sFunkyCharacterConfigFile, FileMode.Create, FileAccess.Write, FileShare.Read);
             using (StreamWriter configWriter = new StreamWriter(configStream))
             {
                 configWriter.WriteLine("OOCIdentifyItems=" + SettingsFunky.OOCIdentifyItems.ToString());
                 configWriter.WriteLine("BuyPotionsDuringTownRun=" + SettingsFunky.BuyPotionsDuringTownRun.ToString());
                 configWriter.WriteLine("EnableWaitAfterContainers=" + SettingsFunky.EnableWaitAfterContainers.ToString());
                 configWriter.WriteLine("UseExtendedRangeRepChest=" + SettingsFunky.UseExtendedRangeRepChest.ToString());
                 configWriter.WriteLine("UseItemRules=" + SettingsFunky.UseItemRules.ToString());
                 configWriter.WriteLine("UseItemRulesPickup=" + SettingsFunky.UseItemRulesPickup.ToString());
                 configWriter.WriteLine("OOCIdentifyItemsMinimumRequired=" + SettingsFunky.OOCIdentifyItemsMinimumRequired.ToString());

                 configWriter.WriteLine("EnableCoffeeBreaks=" + SettingsFunky.EnableCoffeeBreaks.ToString());
                 configWriter.WriteLine("MinBreakTime=" + SettingsFunky.MinBreakTime.ToString());
                 configWriter.WriteLine("MaxBreakTime=" + SettingsFunky.MaxBreakTime.ToString());
                 configWriter.WriteLine("breakTimeHour=" + SettingsFunky.breakTimeHour.ToString());
                 configWriter.WriteLine("ShrineRange=" + SettingsFunky.ShrineRange.ToString());

                 configWriter.WriteLine("ItemRuleUseItemIDs=" + SettingsFunky.ItemRuleUseItemIDs.ToString());
                 configWriter.WriteLine("ItemRuleDebug=" + SettingsFunky.ItemRuleDebug.ToString());
                 configWriter.WriteLine("ItemRuleType=" + SettingsFunky.ItemRuleType.ToString());
                 configWriter.WriteLine("ItemRuleLogPickup=" + SettingsFunky.ItemRuleLogPickup);
                 configWriter.WriteLine("ItemRuleLogKeep=" + SettingsFunky.ItemRuleLogKeep);
                 configWriter.WriteLine("ItemRuleGilesScoring=" + SettingsFunky.ItemRuleGilesScoring.ToString());
                 configWriter.WriteLine("UseLevelingLogic=" + SettingsFunky.UseLevelingLogic.ToString());

                 configWriter.WriteLine("AttemptAvoidanceMovements=" + SettingsFunky.AttemptAvoidanceMovements.ToString());
                 configWriter.WriteLine("UseAdvancedProjectileTesting=" + SettingsFunky.UseAdvancedProjectileTesting.ToString());

                 configWriter.WriteLine("KiteDistance=" + SettingsFunky.KiteDistance.ToString());
                 configWriter.WriteLine("DestructibleRange=" + SettingsFunky.DestructibleRange.ToString());

                 configWriter.WriteLine("GlobeHealthPercent=" + SettingsFunky.GlobeHealthPercent.ToString());
                 configWriter.WriteLine("PotionHealthPercent=" + SettingsFunky.PotionHealthPercent.ToString());

                 configWriter.WriteLine("IgnoreCombatRange=" + SettingsFunky.IgnoreCombatRange.ToString());
                 configWriter.WriteLine("IgnoreLootRange=" + SettingsFunky.IgnoreLootRange.ToString());
                 configWriter.WriteLine("ItemRange=" + SettingsFunky.ItemRange.ToString());
                 configWriter.WriteLine("ContainerOpenRange=" + SettingsFunky.ContainerOpenRange.ToString());
                 configWriter.WriteLine("NonEliteCombatRange=" + SettingsFunky.NonEliteCombatRange.ToString());
                 configWriter.WriteLine("IgnoreCorpses=" + SettingsFunky.IgnoreCorpses.ToString());
                 configWriter.WriteLine("IgnoreAboveAverageMobs=" + SettingsFunky.IgnoreAboveAverageMobs.ToString());
                 configWriter.WriteLine("GoblinPriority=" + SettingsFunky.GoblinPriority.ToString());
                 configWriter.WriteLine("AfterCombatDelay=" + SettingsFunky.AfterCombatDelay.ToString());
                 configWriter.WriteLine("OutOfCombatMovement=" + SettingsFunky.OutOfCombatMovement.ToString());
                 configWriter.WriteLine("EliteCombatRange=" + SettingsFunky.EliteCombatRange.ToString());
                 configWriter.WriteLine("ExtendedCombatRange=" + SettingsFunky.ExtendedCombatRange.ToString());
                 configWriter.WriteLine("GoldRange=" + SettingsFunky.GoldRange.ToString());
                 configWriter.WriteLine("MinimumWeaponItemLevel=" + SettingsFunky.MinimumWeaponItemLevel[0].ToString() + "," + SettingsFunky.MinimumWeaponItemLevel[1].ToString());
                 configWriter.WriteLine("MinimumArmorItemLevel=" + SettingsFunky.MinimumArmorItemLevel[0].ToString() + "," + SettingsFunky.MinimumArmorItemLevel[1].ToString());
                 configWriter.WriteLine("MinimumJeweleryItemLevel=" + SettingsFunky.MinimumJeweleryItemLevel[0].ToString() + "," + SettingsFunky.MinimumJeweleryItemLevel[1].ToString());
                 configWriter.WriteLine("MinimumLegendaryItemLevel=" + SettingsFunky.MinimumLegendaryItemLevel.ToString());
                 configWriter.WriteLine("MaximumHealthPotions=" + SettingsFunky.MaximumHealthPotions.ToString());
                 configWriter.WriteLine("MinimumGoldPile=" + SettingsFunky.MinimumGoldPile.ToString());
                 configWriter.WriteLine("MinimumGemItemLevel=" + SettingsFunky.MinimumGemItemLevel.ToString());
                 configWriter.WriteLine("PickupGems=" + SettingsFunky.PickupGems[0].ToString() + "," + SettingsFunky.PickupGems[1].ToString() + "," + SettingsFunky.PickupGems[2].ToString() + "," + SettingsFunky.PickupGems[3].ToString());
                 configWriter.WriteLine("PickupCraftTomes=" + SettingsFunky.PickupCraftTomes.ToString());
                 configWriter.WriteLine("PickupCraftPlans=" + SettingsFunky.PickupCraftPlans.ToString());
                 configWriter.WriteLine("PickupFollowerItems=" + SettingsFunky.PickupFollowerItems.ToString());
                 configWriter.WriteLine("MiscItemLevel=" + SettingsFunky.MiscItemLevel.ToString());
                 configWriter.WriteLine("GilesMinimumWeaponScore=" + SettingsFunky.GilesMinimumWeaponScore.ToString());
                 configWriter.WriteLine("GilesMinimumArmorScore=" + SettingsFunky.GilesMinimumArmorScore.ToString());
                 configWriter.WriteLine("GilesMinimumJeweleryScore=" + SettingsFunky.GilesMinimumJeweleryScore.ToString());

                 configWriter.WriteLine("AvoidanceRetryMin=" + SettingsFunky.AvoidanceRecheckMinimumRate);
                 configWriter.WriteLine("AvoidanceRetryMax=" + SettingsFunky.AvoidanceRecheckMaximumRate);
                 configWriter.WriteLine("KiteRetryMin=" + SettingsFunky.KitingRecheckMinimumRate);
                 configWriter.WriteLine("KiteRetryMax=" + SettingsFunky.KitingRecheckMaximumRate);
                 configWriter.WriteLine("ItemRulesSalvaging=" + SettingsFunky.ItemRulesSalvaging);
                 configWriter.WriteLine("DebugStatusBar=" + SettingsFunky.DebugStatusBar);
                 configWriter.WriteLine("LogSafeMovementOutput=" + SettingsFunky.LogSafeMovementOutput);

                 configWriter.WriteLine("EnableClusteringTargetLogic=" + SettingsFunky.EnableClusteringTargetLogic.ToString());
                 configWriter.WriteLine("ClusterDistance=" + SettingsFunky.ClusterDistance.ToString());
                 configWriter.WriteLine("ClusterMinimumUnitCount=" + SettingsFunky.ClusterMinimumUnitCount.ToString());
                 configWriter.WriteLine("ClusterKillLowHPUnits=" + SettingsFunky.ClusterKillLowHPUnits.ToString());
                 configWriter.WriteLine("IgnoreClusteringWhenLowHP=" + SettingsFunky.IgnoreClusteringWhenLowHP.ToString());
                 configWriter.WriteLine("IgnoreClusterLowHPValue=" + SettingsFunky.IgnoreClusterLowHPValue.ToString());
                 configWriter.WriteLine("TreasureGoblinRange=" + SettingsFunky.TreasureGoblinRange.ToString());
					  configWriter.WriteLine("SkipAhead="+SettingsFunky.SkipAhead.ToString());
					  configWriter.WriteLine("GlobeRange="+SettingsFunky.GlobeRange.ToString());

					  //GlobeRange
                 switch (ActorClass)
                 {
                     case Zeta.Internals.Actors.ActorClass.Barbarian:
                         configWriter.WriteLine("bSelectiveWhirlwind=" + SettingsFunky.Class.bSelectiveWhirlwind.ToString());
                         configWriter.WriteLine("bWaitForWrath=" + SettingsFunky.Class.bWaitForWrath.ToString());
                         configWriter.WriteLine("bGoblinWrath=" + SettingsFunky.Class.bGoblinWrath.ToString());
                         configWriter.WriteLine("bFuryDumpWrath=" + SettingsFunky.Class.bFuryDumpWrath.ToString());
                         configWriter.WriteLine("bFuryDumpWrath=" + SettingsFunky.Class.bFuryDumpAlways.ToString());
                         break;
                     case Zeta.Internals.Actors.ActorClass.DemonHunter:
                         configWriter.WriteLine("iDHVaultMovementDelay=" + SettingsFunky.Class.iDHVaultMovementDelay.ToString());
                         configWriter.WriteLine("GoblinMinimumRange=" + SettingsFunky.Class.GoblinMinimumRange.ToString());
                         break;
                     case Zeta.Internals.Actors.ActorClass.Monk:
                         configWriter.WriteLine("bMonkInnaSet=" + SettingsFunky.Class.bMonkInnaSet.ToString());
                         break;
                     case Zeta.Internals.Actors.ActorClass.WitchDoctor:
                         configWriter.WriteLine("bEnableCriticalMass=" + SettingsFunky.Class.bEnableCriticalMass.ToString());
                         configWriter.WriteLine("GoblinMinimumRange=" + SettingsFunky.Class.GoblinMinimumRange.ToString());
                         break;
                     case Zeta.Internals.Actors.ActorClass.Wizard:
                         configWriter.WriteLine("bEnableCriticalMass=" + SettingsFunky.Class.bEnableCriticalMass.ToString());
                         configWriter.WriteLine("bWaitForArchon=" + SettingsFunky.Class.bWaitForArchon.ToString());
                         configWriter.WriteLine("bKiteOnlyArchon=" + SettingsFunky.Class.bKiteOnlyArchon.ToString());
                         configWriter.WriteLine("GoblinMinimumRange=" + SettingsFunky.Class.GoblinMinimumRange.ToString());
                         break;
                 }

                 System.Collections.Generic.Dictionary<AvoidanceType, double> currentDictionaryAvoidance = ReturnDictionaryUsingActorClass(ActorClass);
                 //Avoidances..
                 foreach (AvoidanceType item in currentDictionaryAvoidance.Keys)
                 {
                     configWriter.WriteLine(item.ToString() + "_radius=" + dictAvoidanceRadius[item].ToString());
                     configWriter.WriteLine(item.ToString() + "_health=" + currentDictionaryAvoidance[item].ToString());
                 }

             }
             //configStream.Close();
         }

         private static void LoadFunkyConfiguration()
         {

             if (CurrentAccountName == null)
             {
                 UpdateCurrentAccountDetails();
             }

             string sFunkyCharacterFolder = Path.Combine(FolderPaths.sDemonBuddyPath, "Settings", "FunkyTrinity", CurrentAccountName);
             if (!System.IO.Directory.Exists(sFunkyCharacterFolder))
                 System.IO.Directory.CreateDirectory(sFunkyCharacterFolder);

             string sFunkyCharacterConfigFile = Path.Combine(sFunkyCharacterFolder, CurrentHeroName + ".cfg");

             //Check for Config file
             if (!File.Exists(sFunkyCharacterConfigFile))
             {
                 Log("No config file found, now creating a new config from defaults at: " + sFunkyCharacterConfigFile);
                 if(ActorClass == Zeta.Internals.Actors.ActorClass.Barbarian || ActorClass == Zeta.Internals.Actors.ActorClass.Monk)
                 {
                     SettingsFunky = new Settings_Funky(false, false, true, true, true, true, false, 0, 0, 0, 0, false, 30, false, false, "hard", "Rare", "Rare", true, true, 0, 10, 25, 40, 0.6d, 0.4d, false, 2, 500, false, 60, 30, 40, new int[1], new int[1], new int[1], 55, 60, 250, new bool[3], 60, false, true, true, 59, false, 70000, 30000, 27000, false, false);
                 }
                 else
                 {
                     SettingsFunky = new Settings_Funky(false, false, true, true, true, true, false, 0, 0, 0, 0, false, 30, false, false, "hard", "Rare", "Rare", true, true, 5, 10, 25, 40, 0.4d, 0.6d, false, 2, 500, false, 60, 30, 40, new int[1], new int[1], new int[1], 55, 60, 250, new bool[3], 60, false, true, true, 59, false, 70000, 30000, 27000, false, false);
                 }
                 //SettingsFunky = new Settings_Funky(false, false, false, false, false, false, false, 4, 8, 3, 1.5d, true, 20, false, false, "hard", "Rare", "Rare", true, false, 0, 5, 15, 20, 0.5d, 0.5d, false, 2, 500, false, 50, 30, 40, new int[1], new int[1], new int[1], 1, 100, 300, new bool[3], 60, true, true, true, 59, false, 70000, 16000, 15000, false, false);
                SaveFunkyConfiguration();
             }

             string[] splitValue;

             //Load File
             using (StreamReader configReader = new StreamReader(sFunkyCharacterConfigFile))
             {
                 while (!configReader.EndOfStream)
                 {
                     string[] config = configReader.ReadLine().Split('=');
                     if (config != null)
                     {
                         //Check if its an avoidance..
                         if (config[0].Contains("_"))
                         {
                             string[] avoidstr = config[0].Split('_');
                             AvoidanceType avoidType = (AvoidanceType)Enum.Parse(typeof(AvoidanceType), avoidstr[0]);
                             if (avoidstr[1].Contains("health"))
                             {
                                 double health = 0d;
                                 try
                                 {
                                     health = Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));
                                 }
                                 catch
                                 {
                                     Logging.Write("Exception converting to Double At Avoidance Health {0}", avoidType.ToString());
                                 }

                                 switch (ActorClass)
                                 {
                                     case Zeta.Internals.Actors.ActorClass.Barbarian:
                                         dictAvoidanceHealthBarb[avoidType] = health;
                                         break;
                                     case Zeta.Internals.Actors.ActorClass.DemonHunter:
                                         dictAvoidanceHealthDemon[avoidType] = health;
                                         break;
                                     case Zeta.Internals.Actors.ActorClass.Monk:
                                         dictAvoidanceHealthMonk[avoidType] = health;
                                         break;
                                     case Zeta.Internals.Actors.ActorClass.WitchDoctor:
                                         dictAvoidanceHealthWitch[avoidType] = health;
                                         break;
                                     case Zeta.Internals.Actors.ActorClass.Wizard:
                                         dictAvoidanceHealthWizard[avoidType] = health;
                                         break;
                                 }
                             }
                             else
                             {
                                 dictAvoidanceRadius[avoidType] = Convert.ToSingle(config[1]);
                             }
                         }
                         else
                         {
                             switch (config[0])
                             {
                                 case "OOCIdentifyItems":
                                     SettingsFunky.OOCIdentifyItems = Convert.ToBoolean(config[1]);
                                     break;
                                 case "BuyPotionsDuringTownRun":
                                     SettingsFunky.BuyPotionsDuringTownRun = Convert.ToBoolean(config[1]);
                                     break;
                                 case "EnableWaitAfterContainers":
                                     SettingsFunky.EnableWaitAfterContainers = Convert.ToBoolean(config[1]);
                                     break;
                                 case "UseItemRules":
                                     SettingsFunky.UseItemRules = Convert.ToBoolean(config[1]);
                                     break;
                                 case "UseItemRulesPickup":
                                     SettingsFunky.UseItemRulesPickup = Convert.ToBoolean(config[1]);
                                     break;
                                 case "UseExtendedRangeRepChest":
                                     SettingsFunky.UseExtendedRangeRepChest = Convert.ToBoolean(config[1]);
                                     break;
                                 case "EnableCoffeeBreaks":
                                     SettingsFunky.EnableCoffeeBreaks = Convert.ToBoolean(config[1]);
                                     break;
                                 case "MinBreakTime":
                                     SettingsFunky.MinBreakTime = Convert.ToInt32(config[1]);
                                     break;
                                 case "MaxBreakTime":
                                     SettingsFunky.MaxBreakTime = Convert.ToInt32(config[1]);
                                     break;
                                 case "OOCIdentifyItemsMinimumRequired":
                                     SettingsFunky.OOCIdentifyItemsMinimumRequired = Convert.ToInt32(config[1]);
                                     break;
                                 case "breakTimeHour":
                                     try
                                     {
                                         SettingsFunky.breakTimeHour = Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));
                                     }
                                     catch
                                     {
                                         Logging.Write("Exception converting breakTimeHour to Double");
                                     }
                                     break;
                                 case "ShrineRange":
                                     SettingsFunky.ShrineRange = Convert.ToInt16(config[1]);
                                     break;
                                 case "ItemRuleUseItemIDs":
                                     SettingsFunky.ItemRuleUseItemIDs = Convert.ToBoolean(config[1]);
                                     break;
                                 case "ItemRuleDebug":
                                     SettingsFunky.ItemRuleDebug = Convert.ToBoolean(config[1]);
                                     break;
                                 case "ItemRuleType":
                                     SettingsFunky.ItemRuleType = Convert.ToString(config[1]);
                                     break;
                                 case "ItemRuleLogKeep":
                                     SettingsFunky.ItemRuleLogKeep = Convert.ToString(config[1]);
                                     break;
                                 case "ItemRuleLogPickup":
                                     SettingsFunky.ItemRuleLogPickup = Convert.ToString(config[1]);
                                     break;
                                 case "ItemRuleGilesScoring":
                                     SettingsFunky.ItemRuleGilesScoring = Convert.ToBoolean(config[1]);
                                     break;
                                 case "AttemptAvoidanceMovements":
                                     SettingsFunky.AttemptAvoidanceMovements = Convert.ToBoolean(config[1]);
                                     break;
                                 case "KiteDistance":
                                     SettingsFunky.KiteDistance = Convert.ToInt32(config[1]);
                                     break;
                                 case "DestructibleRange":
                                     SettingsFunky.DestructibleRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "GlobeHealthPercent":
                                     try
                                     {
                                         SettingsFunky.GlobeHealthPercent = Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));
                                     }
                                     catch
                                     {
                                         Logging.Write("Exception converting GlobeHealthPercent to Double");
                                     }

                                     break;
                                 case "PotionHealthPercent":
                                     try
                                     {
                                         SettingsFunky.PotionHealthPercent = Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));

                                     }
                                     catch
                                     {
                                         Logging.Write("Exception converting PotionHealthPercent to Double");
                                     }

                                     break;
                                 case "ContainerOpenRange":
                                     SettingsFunky.ContainerOpenRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "IgnoreCombatRange":
                                     SettingsFunky.IgnoreCombatRange = Convert.ToBoolean(config[1]);
                                     break;
                                 case "IgnoreLootRange":
                                     SettingsFunky.IgnoreLootRange = Convert.ToBoolean(config[1]);
                                     break;
                                 case "NonEliteCombatRange":
                                     SettingsFunky.NonEliteCombatRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "IgnoreCorpses":
                                     SettingsFunky.IgnoreCorpses = Convert.ToBoolean(config[1]);
                                     break;
                                 case "GoblinPriority":
                                     SettingsFunky.GoblinPriority = Convert.ToInt32(config[1]);
                                     break;
                                 case "TreasureGoblinRange":
                                     SettingsFunky.TreasureGoblinRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "AfterCombatDelay":
                                     SettingsFunky.AfterCombatDelay = Convert.ToInt32(config[1]);
                                     break;
                                 case "OutOfCombatMovement":
                                     SettingsFunky.OutOfCombatMovement = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bEnableCriticalMass":
                                     SettingsFunky.Class.bEnableCriticalMass = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bSelectiveWhirlwind":
                                     SettingsFunky.Class.bSelectiveWhirlwind = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bWaitForWrath":
                                     SettingsFunky.Class.bWaitForWrath = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bGoblinWrath":
                                     SettingsFunky.Class.bGoblinWrath = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bFuryDumpWrath":
                                     SettingsFunky.Class.bFuryDumpWrath = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bFuryDumpAlways":
                                     SettingsFunky.Class.bFuryDumpAlways = Convert.ToBoolean(config[1]);
                                     break;
                                 case "iDHVaultMovementDelay":
                                     SettingsFunky.Class.iDHVaultMovementDelay = Convert.ToInt32(config[1]);
                                     break;
                                 case "bMonkInnaSet":
                                     SettingsFunky.Class.bMonkInnaSet = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bWaitForArchon":
                                     SettingsFunky.Class.bWaitForArchon = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bKiteOnlyArchon":
                                     SettingsFunky.Class.bKiteOnlyArchon = Convert.ToBoolean(config[1]);
                                     break;
                                 case "EliteCombatRange":
                                     SettingsFunky.EliteCombatRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "ExtendedCombatRange":
                                     SettingsFunky.ExtendedCombatRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "GoldRange":
                                     SettingsFunky.GoldRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "MinimumWeaponItemLevel":
                                     splitValue = config[1].Split(',');
                                     SettingsFunky.MinimumWeaponItemLevel = new int[] { Convert.ToInt16(splitValue[0]), Convert.ToInt16(splitValue[1]) };
                                     break;
                                 case "MinimumArmorItemLevel":
                                     splitValue = config[1].Split(',');
                                     SettingsFunky.MinimumArmorItemLevel = new int[] { Convert.ToInt16(splitValue[0]), Convert.ToInt16(splitValue[1]) };
                                     break;
                                 case "MinimumJeweleryItemLevel":
                                     splitValue = config[1].Split(',');
                                     SettingsFunky.MinimumJeweleryItemLevel = new int[] { Convert.ToInt16(splitValue[0]), Convert.ToInt16(splitValue[1]) };
                                     break;
                                 case "MinimumLegendaryItemLevel":
                                     SettingsFunky.MinimumLegendaryItemLevel = Convert.ToInt32(config[1]);
                                     break;
                                 case "MaximumHealthPotions":
                                     SettingsFunky.MaximumHealthPotions = Convert.ToInt32(config[1]);
                                     break;
                                 case "MinimumGoldPile":
                                     SettingsFunky.MinimumGoldPile = Convert.ToInt32(config[1]);
                                     break;
                                 case "MinimumGemItemLevel":
                                     SettingsFunky.MinimumGemItemLevel = Convert.ToInt32(config[1]);
                                     break;
                                 case "PickupCraftTomes":
                                     SettingsFunky.PickupCraftTomes = Convert.ToBoolean(config[1]);
                                     break;
                                 case "PickupCraftPlans":
                                     SettingsFunky.PickupCraftPlans = Convert.ToBoolean(config[1]);
                                     break;
                                 case "PickupFollowerItems":
                                     SettingsFunky.PickupFollowerItems = Convert.ToBoolean(config[1]);
                                     break;
                                 case "MiscItemLevel":
                                     SettingsFunky.MiscItemLevel = Convert.ToInt32(config[1]);
                                     break;
                                 case "GilesMinimumWeaponScore":
                                     SettingsFunky.GilesMinimumWeaponScore = Convert.ToInt32(config[1]);
                                     break;
                                 case "GilesMinimumArmorScore":
                                     SettingsFunky.GilesMinimumArmorScore = Convert.ToInt32(config[1]);
                                     break;
                                 case "GilesMinimumJeweleryScore":
                                     SettingsFunky.GilesMinimumJeweleryScore = Convert.ToInt32(config[1]);
                                     break;
                                 case "PickupGems":
                                     splitValue = config[1].Split(',');
                                     SettingsFunky.PickupGems = new bool[] { Convert.ToBoolean(splitValue[0]), Convert.ToBoolean(splitValue[1]), Convert.ToBoolean(splitValue[2]), Convert.ToBoolean(splitValue[3]) };
                                     break;
                                 case "UseLevelingLogic":
                                     //UseLevelingLogic
                                     SettingsFunky.UseLevelingLogic = Convert.ToBoolean(config[1]);
                                     break;
                                 case "UseAdvancedProjectileTesting":
                                     SettingsFunky.UseAdvancedProjectileTesting = Convert.ToBoolean(config[1]);
                                     break;
                                 case "IgnoreAboveAverageMobs":
                                     SettingsFunky.IgnoreAboveAverageMobs = Convert.ToBoolean(config[1]);
                                     break;
                                 case "ItemRulesSalvaging":
                                     SettingsFunky.ItemRulesSalvaging = Convert.ToBoolean(config[1]);
                                     break;
                                 case "AvoidanceRetryMin":
                                     SettingsFunky.AvoidanceRecheckMinimumRate = Convert.ToInt32(config[1]);
                                     break;
                                 case "AvoidanceRetryMax":
                                     SettingsFunky.AvoidanceRecheckMaximumRate = Convert.ToInt32(config[1]);
                                     break;
                                 case "KiteRetryMin":
                                     SettingsFunky.KitingRecheckMinimumRate = Convert.ToInt32(config[1]);
                                     break;
                                 case "KiteRetryMax":
                                     SettingsFunky.KitingRecheckMaximumRate = Convert.ToInt32(config[1]);
                                     break;
                                 case "DebugStatusBar":
                                     SettingsFunky.DebugStatusBar = Convert.ToBoolean(config[1]);
                                     break;
                                 case "LogSafeMovementOutput":
                                     SettingsFunky.LogSafeMovementOutput = Convert.ToBoolean(config[1]);
                                     break;
                                 case "EnableClusteringTargetLogic":
                                     SettingsFunky.EnableClusteringTargetLogic = Convert.ToBoolean(config[1]);
                                     break;
                                 case "ClusterDistance":
                                     SettingsFunky.ClusterDistance = Convert.ToInt32(config[1]);
                                     break;
                                 case "ClusterMinimumUnitCount":
                                     SettingsFunky.ClusterMinimumUnitCount = Convert.ToInt32(config[1]);
                                     break;
                                 case "ClusterKillLowHPUnits":
                                     SettingsFunky.ClusterKillLowHPUnits = Convert.ToBoolean(config[1]);
                                     break;
                                 case "IgnoreClusteringWhenLowHP":
                                     SettingsFunky.IgnoreClusteringWhenLowHP = Convert.ToBoolean(config[1]);
                                     break;
                                 case "IgnoreClusterLowHPValue":
                                     try
                                     {
                                         SettingsFunky.IgnoreClusterLowHPValue = Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));
                                     }
                                     catch
                                     {
                                         Logging.Write("Exception converting IgnoreClusterLowHPValue to Double");
                                     }

                                     break;
                                 case "ItemRange":
                                     SettingsFunky.ItemRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "GoblinMinimumRange":
                                     SettingsFunky.Class.GoblinMinimumRange = Convert.ToInt32(config[1]);
                                     break;
											case "SkipAhead":
												 SettingsFunky.SkipAhead=Convert.ToBoolean(config[1]);
												 break;
											case "GlobeRange":
												 SettingsFunky.GlobeRange=Convert.ToInt32(config[1]);
												 break;
											//GlobeRange
                             }
                         }
                     }

                 }
                 //configReader.Close();
             }

             Zeta.Common.Logging.WriteDiagnostic("[Funky] Character settings loaded!");
         }

         public static Settings_Funky SettingsFunky = new Settings_Funky(false, false, false, false, false, false, false, 4, 8, 3, 1.5d, true, 20, false, false, "hard", "Rare", "Rare", true, false, 0, 10, 30, 60, 0.6d, 0.4d, true, 2, 250, false, 60, 30, 40, new int[1], new int[1], new int[1], 1, 100, 300, new bool[3], 60, true, true, true, 59, false, 75000, 25000, 25000, false, false);

         public class Settings_Funky
         {
             public bool DebugStatusBar { get; set; }
             public bool LogSafeMovementOutput { get; set; }
				 public bool SkipAhead { get; set; }
             
             public bool LogStuckLocations { get; set; }
             public bool EnableUnstucker { get; set; }
             public bool RestartGameOnLongStucks { get; set; }
             
             public bool OOCIdentifyItems { get; set; }
             public bool BuyPotionsDuringTownRun { get; set; }
             public bool EnableWaitAfterContainers { get; set; }
             public bool UseItemRulesPickup { get; set; }
             public bool UseItemRules { get; set; }
             public bool UseExtendedRangeRepChest { get; set; }
             public bool EnableCoffeeBreaks { get; set; }
             public int MinBreakTime { get; set; }
             public int MaxBreakTime { get; set; }
             public int OOCIdentifyItemsMinimumRequired { get; set; }
             public double breakTimeHour { get; set; }

             //Character Related
             public bool AttemptAvoidanceMovements { get; set; }
             public bool UseAdvancedProjectileTesting { get; set; }
             public int KiteDistance { get; set; }
             public double GlobeHealthPercent { get; set; }
             public double PotionHealthPercent { get; set; }
             public bool IgnoreCorpses { get; set; }
             public int GoblinPriority { get; set; }
             public int AfterCombatDelay { get; set; }
             public bool IgnoreAboveAverageMobs { get; set; }
             public bool OutOfCombatMovement { get; set; }

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
             public int ExtendedCombatRange { get; set; }

             //Item Rules Additions
             public bool ItemRulesSalvaging { get; set; }
             public bool ItemRuleUseItemIDs { get; set; }
             public bool ItemRuleDebug { get; set; }
             public string ItemRuleType { get; set; }
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
             public bool PickupFollowerItems { get; set; }
             public int MiscItemLevel { get; set; }

             public int AvoidanceRecheckMaximumRate { get; set; }//=1250;
             public int AvoidanceRecheckMinimumRate { get; set; }//=500;
             public int KitingRecheckMaximumRate { get; set; }//=4000;
             public int KitingRecheckMinimumRate { get; set; }//=2000;

             public double ClusterDistance { get; set; }
             public int ClusterMinimumUnitCount { get; set; }
             public bool EnableClusteringTargetLogic { get; set; }
             public bool ClusterKillLowHPUnits { get; set; }
             public bool IgnoreClusteringWhenLowHP { get; set; }
             public double IgnoreClusterLowHPValue { get; set; }

             //Class Settings
             public ClassSettings Class { get; set; }
             public Settings_Funky(bool oocIDitems, bool buyPotions, bool WaitForContainers,
                  bool itemRulesPickup, bool itemRules, bool extendRangeRepChest,
                  bool coffeebreak, int minbreak, int maxbreak, int minOOCitems, double breakTime, bool ignoreDestructibles, int shrinerange
                  , bool itemruleIDs, bool itemruleDebug, string itemruletype, string itemrulekeeplog, string itemrulepickuplog, bool gilesscoring,
                  bool attemptavoidance, int Kite, int destructiblerange, int containerrange, int noneliterange,
                  double globehealth, double potionhealth, bool ignorecorpse, int goblinpriority, int aftercombatdelay, bool outofcombatmovement,
                  int eliterange, int extendedrange, int goldrange, int[] minweaponlevel, int[] minarmorlevel, int[] minjewelerylevel, int minlegendarylevel,
                  int maxhealthpots, int mingoldpile, bool[] gems, int minGemLevel, bool craftTomes, bool craftPlans, bool Followeritems, int miscitemlevel, bool itemlevelinglogic, int gilesWeaponScore, int gilesArmorScore, int gilesJeweleryScore, bool projectiletesting, bool ignoreelites)
             {
                 LogStuckLocations = true;
                 RestartGameOnLongStucks = true;
                 EnableUnstucker = true;

                 AvoidanceRecheckMaximumRate = 3500;
                 AvoidanceRecheckMinimumRate = 550;
                 KitingRecheckMaximumRate = 4500;
                 KitingRecheckMinimumRate = 1000;
                 ItemRulesSalvaging = true;
                 OOCIdentifyItems = oocIDitems;
                 BuyPotionsDuringTownRun = buyPotions;
                 EnableWaitAfterContainers = WaitForContainers;
                 UseItemRulesPickup = itemRulesPickup;
                 UseItemRules = itemRules;
                 UseExtendedRangeRepChest = extendRangeRepChest;
                 EnableCoffeeBreaks = coffeebreak;
                 MaxBreakTime = minbreak;
                 MinBreakTime = maxbreak;
                 OOCIdentifyItemsMinimumRequired = minOOCitems;
                 breakTimeHour = breakTime;
                 ItemRuleDebug = itemruleDebug;
                 ItemRuleUseItemIDs = itemruleIDs;
                 ItemRuleType = itemruletype;
                 ItemRuleLogKeep = itemrulekeeplog;
                 ItemRuleLogPickup = itemrulepickuplog;
                 ItemRuleGilesScoring = gilesscoring;
                 AttemptAvoidanceMovements = attemptavoidance;
                 KiteDistance = Kite;
                 IgnoreCombatRange = false;
                 IgnoreLootRange = false;
                 DestructibleRange = destructiblerange;
                 ContainerOpenRange = containerrange;
                 NonEliteCombatRange = noneliterange;
                 GlobeHealthPercent = globehealth;
                 PotionHealthPercent = potionhealth;
                 IgnoreCorpses = ignorecorpse;
                 GoblinPriority = goblinpriority;
                 AfterCombatDelay = aftercombatdelay;
                 OutOfCombatMovement = outofcombatmovement;
                 EliteCombatRange = eliterange;
                 ExtendedCombatRange = extendedrange;
                 GoldRange = goldrange;
					  GlobeRange=40;
                 ItemRange = 50;
                 TreasureGoblinRange = 40;
                 ShrineRange = shrinerange;
                 MinimumWeaponItemLevel = new int[] { 0, 59 };
                 MinimumArmorItemLevel = new int[] { 0, 59 };
                 MinimumJeweleryItemLevel = new int[] { 0, 55 };
                 GilesMinimumWeaponScore = gilesWeaponScore;
                 GilesMinimumArmorScore = gilesArmorScore;
                 GilesMinimumJeweleryScore = gilesJeweleryScore;

                 MinimumLegendaryItemLevel = minlegendarylevel;
                 MaximumHealthPotions = maxhealthpots;
                 MinimumGoldPile = mingoldpile;

                 PickupGems = new bool[] { true, true, false, false };

                 MinimumGemItemLevel = minGemLevel;
                 PickupCraftTomes = craftTomes;
                 PickupCraftPlans = craftPlans;
                 PickupFollowerItems = Followeritems;
                 MiscItemLevel = miscitemlevel;
                 UseLevelingLogic = itemlevelinglogic;
                 UseAdvancedProjectileTesting = projectiletesting;
                 IgnoreAboveAverageMobs = ignoreelites;
                 DebugStatusBar = false;
                 LogSafeMovementOutput = false;


                 EnableClusteringTargetLogic = true;
                 IgnoreClusteringWhenLowHP = true;
                 ClusterKillLowHPUnits = true;
                 ClusterDistance = 10d;
                 ClusterMinimumUnitCount = 3;
                 IgnoreClusterLowHPValue = 0.50d;
					  SkipAhead=true;

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

                 //DH
                 public int iDHVaultMovementDelay { get; set; }

                 //Monk
                 public bool bMonkInnaSet { get; set; }

                 //Wiz
                 public bool bWaitForArchon { get; set; }
                 public bool bKiteOnlyArchon { get; set; }

                 //WD+Wiz
                 public bool bEnableCriticalMass { get; set; }

                 //Range Class
                 public int GoblinMinimumRange { get; set; }

                 public ClassSettings()
                 {
                     bEnableCriticalMass = false;
                     bSelectiveWhirlwind = false;
                     bWaitForWrath = false;
                     bGoblinWrath = false;
                     bFuryDumpWrath = false;
                     bFuryDumpAlways = false;
                     iDHVaultMovementDelay = 400;
                     bMonkInnaSet = false;
                     bWaitForArchon = false;
                     bKiteOnlyArchon = false;
                     GoblinMinimumRange = 35;
                 }
             }
         }
    }
}