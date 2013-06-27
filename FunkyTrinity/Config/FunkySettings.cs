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

				 string sFunkyCharacterConfigFile=Path.Combine(FolderPaths.sDemonBuddyPath, "Settings", "FunkyTrinity", Bot.CurrentAccountName, Bot.CurrentHeroName+".cfg");

             FileStream configStream = File.Open(sFunkyCharacterConfigFile, FileMode.Create, FileAccess.Write, FileShare.Read);
             using (StreamWriter configWriter = new StreamWriter(configStream))
             {
                 configWriter.WriteLine("OOCIdentifyItems=" + Bot.SettingsFunky.OOCIdentifyItems.ToString());
                 configWriter.WriteLine("BuyPotionsDuringTownRun=" + Bot.SettingsFunky.BuyPotionsDuringTownRun.ToString());
                 configWriter.WriteLine("EnableWaitAfterContainers=" + Bot.SettingsFunky.EnableWaitAfterContainers.ToString());
                 configWriter.WriteLine("UseExtendedRangeRepChest=" + Bot.SettingsFunky.UseExtendedRangeRepChest.ToString());
                 configWriter.WriteLine("UseItemRules=" + Bot.SettingsFunky.UseItemRules.ToString());
                 configWriter.WriteLine("UseItemRulesPickup=" + Bot.SettingsFunky.UseItemRulesPickup.ToString());
                 configWriter.WriteLine("OOCIdentifyItemsMinimumRequired=" + Bot.SettingsFunky.OOCIdentifyItemsMinimumRequired.ToString());

                 configWriter.WriteLine("EnableCoffeeBreaks=" + Bot.SettingsFunky.EnableCoffeeBreaks.ToString());
                 configWriter.WriteLine("MinBreakTime=" + Bot.SettingsFunky.MinBreakTime.ToString());
                 configWriter.WriteLine("MaxBreakTime=" + Bot.SettingsFunky.MaxBreakTime.ToString());
                 configWriter.WriteLine("breakTimeHour=" + Bot.SettingsFunky.breakTimeHour.ToString());
                 configWriter.WriteLine("ShrineRange=" + Bot.SettingsFunky.ShrineRange.ToString());

                 configWriter.WriteLine("ItemRuleUseItemIDs=" + Bot.SettingsFunky.ItemRuleUseItemIDs.ToString());
                 configWriter.WriteLine("ItemRuleDebug=" + Bot.SettingsFunky.ItemRuleDebug.ToString());
                 configWriter.WriteLine("ItemRuleType=" + Bot.SettingsFunky.ItemRuleType.ToString());
                 configWriter.WriteLine("ItemRuleLogPickup=" + Bot.SettingsFunky.ItemRuleLogPickup);
                 configWriter.WriteLine("ItemRuleLogKeep=" + Bot.SettingsFunky.ItemRuleLogKeep);
                 configWriter.WriteLine("ItemRuleGilesScoring=" + Bot.SettingsFunky.ItemRuleGilesScoring.ToString());
                 configWriter.WriteLine("UseLevelingLogic=" + Bot.SettingsFunky.UseLevelingLogic.ToString());

                 configWriter.WriteLine("AttemptAvoidanceMovements=" + Bot.SettingsFunky.AttemptAvoidanceMovements.ToString());
                 configWriter.WriteLine("UseAdvancedProjectileTesting=" + Bot.SettingsFunky.UseAdvancedProjectileTesting.ToString());

                 configWriter.WriteLine("KiteDistance=" + Bot.SettingsFunky.KiteDistance.ToString());
                 configWriter.WriteLine("DestructibleRange=" + Bot.SettingsFunky.DestructibleRange.ToString());

                 configWriter.WriteLine("GlobeHealthPercent=" + Bot.SettingsFunky.GlobeHealthPercent.ToString());
                 configWriter.WriteLine("PotionHealthPercent=" + Bot.SettingsFunky.PotionHealthPercent.ToString());

                 configWriter.WriteLine("IgnoreCombatRange=" + Bot.SettingsFunky.IgnoreCombatRange.ToString());
                 configWriter.WriteLine("IgnoreLootRange=" + Bot.SettingsFunky.IgnoreLootRange.ToString());
                 configWriter.WriteLine("ItemRange=" + Bot.SettingsFunky.ItemRange.ToString());
                 configWriter.WriteLine("ContainerOpenRange=" + Bot.SettingsFunky.ContainerOpenRange.ToString());
                 configWriter.WriteLine("NonEliteCombatRange=" + Bot.SettingsFunky.NonEliteCombatRange.ToString());
                 configWriter.WriteLine("IgnoreCorpses=" + Bot.SettingsFunky.IgnoreCorpses.ToString());
                 configWriter.WriteLine("IgnoreAboveAverageMobs=" + Bot.SettingsFunky.IgnoreAboveAverageMobs.ToString());
                 configWriter.WriteLine("GoblinPriority=" + Bot.SettingsFunky.GoblinPriority.ToString());
                 configWriter.WriteLine("AfterCombatDelay=" + Bot.SettingsFunky.AfterCombatDelay.ToString());
                 configWriter.WriteLine("OutOfCombatMovement=" + Bot.SettingsFunky.OutOfCombatMovement.ToString());
                 configWriter.WriteLine("EliteCombatRange=" + Bot.SettingsFunky.EliteCombatRange.ToString());
                 configWriter.WriteLine("ExtendedCombatRange=" + Bot.SettingsFunky.ExtendedCombatRange.ToString());
                 configWriter.WriteLine("GoldRange=" + Bot.SettingsFunky.GoldRange.ToString());
                 configWriter.WriteLine("MinimumWeaponItemLevel=" + Bot.SettingsFunky.MinimumWeaponItemLevel[0].ToString() + "," + Bot.SettingsFunky.MinimumWeaponItemLevel[1].ToString());
                 configWriter.WriteLine("MinimumArmorItemLevel=" + Bot.SettingsFunky.MinimumArmorItemLevel[0].ToString() + "," + Bot.SettingsFunky.MinimumArmorItemLevel[1].ToString());
                 configWriter.WriteLine("MinimumJeweleryItemLevel=" + Bot.SettingsFunky.MinimumJeweleryItemLevel[0].ToString() + "," + Bot.SettingsFunky.MinimumJeweleryItemLevel[1].ToString());
                 configWriter.WriteLine("MinimumLegendaryItemLevel=" + Bot.SettingsFunky.MinimumLegendaryItemLevel.ToString());
                 configWriter.WriteLine("MaximumHealthPotions=" + Bot.SettingsFunky.MaximumHealthPotions.ToString());
                 configWriter.WriteLine("MinimumGoldPile=" + Bot.SettingsFunky.MinimumGoldPile.ToString());
                 configWriter.WriteLine("MinimumGemItemLevel=" + Bot.SettingsFunky.MinimumGemItemLevel.ToString());
                 configWriter.WriteLine("PickupGems=" + Bot.SettingsFunky.PickupGems[0].ToString() + "," + Bot.SettingsFunky.PickupGems[1].ToString() + "," + Bot.SettingsFunky.PickupGems[2].ToString() + "," + Bot.SettingsFunky.PickupGems[3].ToString());
                 configWriter.WriteLine("PickupCraftTomes=" + Bot.SettingsFunky.PickupCraftTomes.ToString());
                 configWriter.WriteLine("PickupCraftPlans=" + Bot.SettingsFunky.PickupCraftPlans.ToString());
                 configWriter.WriteLine("PickupFollowerItems=" + Bot.SettingsFunky.PickupFollowerItems.ToString());
                 configWriter.WriteLine("MiscItemLevel=" + Bot.SettingsFunky.MiscItemLevel.ToString());
                 configWriter.WriteLine("GilesMinimumWeaponScore=" + Bot.SettingsFunky.GilesMinimumWeaponScore.ToString());
                 configWriter.WriteLine("GilesMinimumArmorScore=" + Bot.SettingsFunky.GilesMinimumArmorScore.ToString());
                 configWriter.WriteLine("GilesMinimumJeweleryScore=" + Bot.SettingsFunky.GilesMinimumJeweleryScore.ToString());

                 configWriter.WriteLine("AvoidanceRetryMin=" + Bot.SettingsFunky.AvoidanceRecheckMinimumRate);
                 configWriter.WriteLine("AvoidanceRetryMax=" + Bot.SettingsFunky.AvoidanceRecheckMaximumRate);
                 configWriter.WriteLine("KiteRetryMin=" + Bot.SettingsFunky.KitingRecheckMinimumRate);
                 configWriter.WriteLine("KiteRetryMax=" + Bot.SettingsFunky.KitingRecheckMaximumRate);
                 configWriter.WriteLine("ItemRulesSalvaging=" + Bot.SettingsFunky.ItemRulesSalvaging);
                 configWriter.WriteLine("DebugStatusBar=" + Bot.SettingsFunky.DebugStatusBar);
                 configWriter.WriteLine("LogSafeMovementOutput=" + Bot.SettingsFunky.LogSafeMovementOutput);

                 configWriter.WriteLine("EnableClusteringTargetLogic=" + Bot.SettingsFunky.EnableClusteringTargetLogic.ToString());
                 configWriter.WriteLine("ClusterDistance=" + Bot.SettingsFunky.ClusterDistance.ToString());
                 configWriter.WriteLine("ClusterMinimumUnitCount=" + Bot.SettingsFunky.ClusterMinimumUnitCount.ToString());
                 configWriter.WriteLine("ClusterKillLowHPUnits=" + Bot.SettingsFunky.ClusterKillLowHPUnits.ToString());
                 configWriter.WriteLine("IgnoreClusteringWhenLowHP=" + Bot.SettingsFunky.IgnoreClusteringWhenLowHP.ToString());
                 configWriter.WriteLine("IgnoreClusterLowHPValue=" + Bot.SettingsFunky.IgnoreClusterLowHPValue.ToString());
                 configWriter.WriteLine("TreasureGoblinRange=" + Bot.SettingsFunky.TreasureGoblinRange.ToString());
					  configWriter.WriteLine("SkipAhead="+Bot.SettingsFunky.SkipAhead.ToString());
					  configWriter.WriteLine("GlobeRange="+Bot.SettingsFunky.GlobeRange.ToString());
					  configWriter.WriteLine("ItemRuleCustomPath="+(!String.IsNullOrEmpty(Bot.SettingsFunky.ItemRuleCustomPath)?Bot.SettingsFunky.ItemRuleCustomPath.ToString():""));
					  configWriter.WriteLine("ItemRulesUnidStashing="+Bot.SettingsFunky.ItemRulesUnidStashing.ToString());

					  //ItemRulesUnidStashing
					  switch (Bot.ActorClass)
                 {
                     case Zeta.Internals.Actors.ActorClass.Barbarian:
                         configWriter.WriteLine("bSelectiveWhirlwind=" + Bot.SettingsFunky.Class.bSelectiveWhirlwind.ToString());
                         configWriter.WriteLine("bWaitForWrath=" + Bot.SettingsFunky.Class.bWaitForWrath.ToString());
                         configWriter.WriteLine("bGoblinWrath=" + Bot.SettingsFunky.Class.bGoblinWrath.ToString());
                         configWriter.WriteLine("bFuryDumpWrath=" + Bot.SettingsFunky.Class.bFuryDumpWrath.ToString());
                         configWriter.WriteLine("bFuryDumpWrath=" + Bot.SettingsFunky.Class.bFuryDumpAlways.ToString());
                         break;
                     case Zeta.Internals.Actors.ActorClass.DemonHunter:
                         configWriter.WriteLine("iDHVaultMovementDelay=" + Bot.SettingsFunky.Class.iDHVaultMovementDelay.ToString());
                         configWriter.WriteLine("GoblinMinimumRange=" + Bot.SettingsFunky.Class.GoblinMinimumRange.ToString());
                         break;
                     case Zeta.Internals.Actors.ActorClass.Monk:
                         configWriter.WriteLine("bMonkInnaSet=" + Bot.SettingsFunky.Class.bMonkInnaSet.ToString());
								 configWriter.WriteLine("bMonkSpamMantra="+Bot.SettingsFunky.Class.bMonkSpamMantra.ToString());
								 //bMonkSpamMantra
								 break;
                     case Zeta.Internals.Actors.ActorClass.WitchDoctor:
                         configWriter.WriteLine("bEnableCriticalMass=" + Bot.SettingsFunky.Class.bEnableCriticalMass.ToString());
                         configWriter.WriteLine("GoblinMinimumRange=" + Bot.SettingsFunky.Class.GoblinMinimumRange.ToString());
                         break;
                     case Zeta.Internals.Actors.ActorClass.Wizard:
                         configWriter.WriteLine("bEnableCriticalMass=" + Bot.SettingsFunky.Class.bEnableCriticalMass.ToString());
                         configWriter.WriteLine("bWaitForArchon=" + Bot.SettingsFunky.Class.bWaitForArchon.ToString());
                         configWriter.WriteLine("bKiteOnlyArchon=" + Bot.SettingsFunky.Class.bKiteOnlyArchon.ToString());
                         configWriter.WriteLine("GoblinMinimumRange=" + Bot.SettingsFunky.Class.GoblinMinimumRange.ToString());
                         break;
                 }

					  System.Collections.Generic.Dictionary<AvoidanceType, double> currentDictionaryAvoidance=ReturnDictionaryUsingActorClass(Bot.ActorClass);
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

				 if (Bot.CurrentAccountName==null)
             {
					  Bot.UpdateCurrentAccountDetails();
             }

				 string sFunkyCharacterFolder=Path.Combine(FolderPaths.sDemonBuddyPath, "Settings", "FunkyTrinity", Bot.CurrentAccountName);
             if (!System.IO.Directory.Exists(sFunkyCharacterFolder))
                 System.IO.Directory.CreateDirectory(sFunkyCharacterFolder);

				 string sFunkyCharacterConfigFile=Path.Combine(sFunkyCharacterFolder, Bot.CurrentHeroName+".cfg");

             //Check for Config file
             if (!File.Exists(sFunkyCharacterConfigFile))
             {
                Log("No config file found, now creating a new config from defaults at: " + sFunkyCharacterConfigFile);
					 Bot.SettingsFunky=new Settings_Funky(false, false, true, true, true, true, false, 0, 0, 0, 0, false, 30, false, false, "Hard", "Rare", "Rare", true, true, 0, 10, 25, 40, 0.6d, 0.4d, false, 2, 500, false, 60, 0, 40, new int[1], new int[1], new int[1], 55, 60, 250, new bool[3], 100, false, true, true, 59, false, 70000, 30000, 27000, false, false);
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
									  AvoidanceType avoidType;
									  try
									  {
											avoidType=(AvoidanceType)Enum.Parse(typeof(AvoidanceType), avoidstr[0]);
									  } catch (Exception)
									  {
											continue;
									  }
                             

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

											switch (Bot.ActorClass)
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
                                     Bot.SettingsFunky.OOCIdentifyItems = Convert.ToBoolean(config[1]);
                                     break;
                                 case "BuyPotionsDuringTownRun":
                                     Bot.SettingsFunky.BuyPotionsDuringTownRun = Convert.ToBoolean(config[1]);
                                     break;
                                 case "EnableWaitAfterContainers":
                                     Bot.SettingsFunky.EnableWaitAfterContainers = Convert.ToBoolean(config[1]);
                                     break;
                                 case "UseItemRules":
                                     Bot.SettingsFunky.UseItemRules = Convert.ToBoolean(config[1]);
                                     break;
                                 case "UseItemRulesPickup":
                                     Bot.SettingsFunky.UseItemRulesPickup = Convert.ToBoolean(config[1]);
                                     break;
                                 case "UseExtendedRangeRepChest":
                                     Bot.SettingsFunky.UseExtendedRangeRepChest = Convert.ToBoolean(config[1]);
                                     break;
                                 case "EnableCoffeeBreaks":
                                     Bot.SettingsFunky.EnableCoffeeBreaks = Convert.ToBoolean(config[1]);
                                     break;
                                 case "MinBreakTime":
                                     Bot.SettingsFunky.MinBreakTime = Convert.ToInt32(config[1]);
                                     break;
                                 case "MaxBreakTime":
                                     Bot.SettingsFunky.MaxBreakTime = Convert.ToInt32(config[1]);
                                     break;
                                 case "OOCIdentifyItemsMinimumRequired":
                                     Bot.SettingsFunky.OOCIdentifyItemsMinimumRequired = Convert.ToInt32(config[1]);
                                     break;
                                 case "breakTimeHour":
                                     try
                                     {
                                         Bot.SettingsFunky.breakTimeHour = Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));
                                     }
                                     catch
                                     {
                                         Logging.Write("Exception converting breakTimeHour to Double");
                                     }
                                     break;
                                 case "ShrineRange":
                                     Bot.SettingsFunky.ShrineRange = Convert.ToInt16(config[1]);
                                     break;
                                 case "ItemRuleUseItemIDs":
                                     Bot.SettingsFunky.ItemRuleUseItemIDs = Convert.ToBoolean(config[1]);
                                     break;
                                 case "ItemRuleDebug":
                                     Bot.SettingsFunky.ItemRuleDebug = Convert.ToBoolean(config[1]);
                                     break;
                                 case "ItemRuleType":
                                     Bot.SettingsFunky.ItemRuleType = Convert.ToString(config[1]);
                                     break;
                                 case "ItemRuleLogKeep":
                                     Bot.SettingsFunky.ItemRuleLogKeep = Convert.ToString(config[1]);
                                     break;
                                 case "ItemRuleLogPickup":
                                     Bot.SettingsFunky.ItemRuleLogPickup = Convert.ToString(config[1]);
                                     break;
                                 case "ItemRuleGilesScoring":
                                     Bot.SettingsFunky.ItemRuleGilesScoring = Convert.ToBoolean(config[1]);
                                     break;
                                 case "AttemptAvoidanceMovements":
                                     Bot.SettingsFunky.AttemptAvoidanceMovements = Convert.ToBoolean(config[1]);
                                     break;
                                 case "KiteDistance":
                                     Bot.SettingsFunky.KiteDistance = Convert.ToInt32(config[1]);
                                     break;
                                 case "DestructibleRange":
                                     Bot.SettingsFunky.DestructibleRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "GlobeHealthPercent":
                                     try
                                     {
                                         Bot.SettingsFunky.GlobeHealthPercent = Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));
                                     }
                                     catch
                                     {
                                         Logging.Write("Exception converting GlobeHealthPercent to Double");
                                     }

                                     break;
                                 case "PotionHealthPercent":
                                     try
                                     {
                                         Bot.SettingsFunky.PotionHealthPercent = Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));

                                     }
                                     catch
                                     {
                                         Logging.Write("Exception converting PotionHealthPercent to Double");
                                     }

                                     break;
                                 case "ContainerOpenRange":
                                     Bot.SettingsFunky.ContainerOpenRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "IgnoreCombatRange":
                                     Bot.SettingsFunky.IgnoreCombatRange = Convert.ToBoolean(config[1]);
                                     break;
                                 case "IgnoreLootRange":
                                     Bot.SettingsFunky.IgnoreLootRange = Convert.ToBoolean(config[1]);
                                     break;
                                 case "NonEliteCombatRange":
                                     Bot.SettingsFunky.NonEliteCombatRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "IgnoreCorpses":
                                     Bot.SettingsFunky.IgnoreCorpses = Convert.ToBoolean(config[1]);
                                     break;
                                 case "GoblinPriority":
                                     Bot.SettingsFunky.GoblinPriority = Convert.ToInt32(config[1]);
                                     break;
                                 case "TreasureGoblinRange":
                                     Bot.SettingsFunky.TreasureGoblinRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "AfterCombatDelay":
                                     Bot.SettingsFunky.AfterCombatDelay = Convert.ToInt32(config[1]);
                                     break;
                                 case "OutOfCombatMovement":
                                     Bot.SettingsFunky.OutOfCombatMovement = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bEnableCriticalMass":
                                     Bot.SettingsFunky.Class.bEnableCriticalMass = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bSelectiveWhirlwind":
                                     Bot.SettingsFunky.Class.bSelectiveWhirlwind = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bWaitForWrath":
                                     Bot.SettingsFunky.Class.bWaitForWrath = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bGoblinWrath":
                                     Bot.SettingsFunky.Class.bGoblinWrath = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bFuryDumpWrath":
                                     Bot.SettingsFunky.Class.bFuryDumpWrath = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bFuryDumpAlways":
                                     Bot.SettingsFunky.Class.bFuryDumpAlways = Convert.ToBoolean(config[1]);
                                     break;
                                 case "iDHVaultMovementDelay":
                                     Bot.SettingsFunky.Class.iDHVaultMovementDelay = Convert.ToInt32(config[1]);
                                     break;
                                 case "bMonkInnaSet":
                                     Bot.SettingsFunky.Class.bMonkInnaSet = Convert.ToBoolean(config[1]);
                                     break;
											case "bMonkSpamMantra":
												 Bot.SettingsFunky.Class.bMonkSpamMantra=Convert.ToBoolean(config[1]);
												 break;
                                 case "bWaitForArchon":
                                     Bot.SettingsFunky.Class.bWaitForArchon = Convert.ToBoolean(config[1]);
                                     break;
                                 case "bKiteOnlyArchon":
                                     Bot.SettingsFunky.Class.bKiteOnlyArchon = Convert.ToBoolean(config[1]);
                                     break;
                                 case "EliteCombatRange":
                                     Bot.SettingsFunky.EliteCombatRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "ExtendedCombatRange":
                                     Bot.SettingsFunky.ExtendedCombatRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "GoldRange":
                                     Bot.SettingsFunky.GoldRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "MinimumWeaponItemLevel":
                                     splitValue = config[1].Split(',');
                                     Bot.SettingsFunky.MinimumWeaponItemLevel = new int[] { Convert.ToInt16(splitValue[0]), Convert.ToInt16(splitValue[1]) };
                                     break;
                                 case "MinimumArmorItemLevel":
                                     splitValue = config[1].Split(',');
                                     Bot.SettingsFunky.MinimumArmorItemLevel = new int[] { Convert.ToInt16(splitValue[0]), Convert.ToInt16(splitValue[1]) };
                                     break;
                                 case "MinimumJeweleryItemLevel":
                                     splitValue = config[1].Split(',');
                                     Bot.SettingsFunky.MinimumJeweleryItemLevel = new int[] { Convert.ToInt16(splitValue[0]), Convert.ToInt16(splitValue[1]) };
                                     break;
                                 case "MinimumLegendaryItemLevel":
                                     Bot.SettingsFunky.MinimumLegendaryItemLevel = Convert.ToInt32(config[1]);
                                     break;
                                 case "MaximumHealthPotions":
                                     Bot.SettingsFunky.MaximumHealthPotions = Convert.ToInt32(config[1]);
                                     break;
                                 case "MinimumGoldPile":
                                     Bot.SettingsFunky.MinimumGoldPile = Convert.ToInt32(config[1]);
                                     break;
                                 case "MinimumGemItemLevel":
                                     Bot.SettingsFunky.MinimumGemItemLevel = Convert.ToInt32(config[1]);
                                     break;
                                 case "PickupCraftTomes":
                                     Bot.SettingsFunky.PickupCraftTomes = Convert.ToBoolean(config[1]);
                                     break;
                                 case "PickupCraftPlans":
                                     Bot.SettingsFunky.PickupCraftPlans = Convert.ToBoolean(config[1]);
                                     break;
                                 case "PickupFollowerItems":
                                     Bot.SettingsFunky.PickupFollowerItems = Convert.ToBoolean(config[1]);
                                     break;
                                 case "MiscItemLevel":
                                     Bot.SettingsFunky.MiscItemLevel = Convert.ToInt32(config[1]);
                                     break;
                                 case "GilesMinimumWeaponScore":
                                     Bot.SettingsFunky.GilesMinimumWeaponScore = Convert.ToInt32(config[1]);
                                     break;
                                 case "GilesMinimumArmorScore":
                                     Bot.SettingsFunky.GilesMinimumArmorScore = Convert.ToInt32(config[1]);
                                     break;
                                 case "GilesMinimumJeweleryScore":
                                     Bot.SettingsFunky.GilesMinimumJeweleryScore = Convert.ToInt32(config[1]);
                                     break;
                                 case "PickupGems":
                                     splitValue = config[1].Split(',');
                                     Bot.SettingsFunky.PickupGems = new bool[] { Convert.ToBoolean(splitValue[0]), Convert.ToBoolean(splitValue[1]), Convert.ToBoolean(splitValue[2]), Convert.ToBoolean(splitValue[3]) };
                                     break;
                                 case "UseLevelingLogic":
                                     //UseLevelingLogic
                                     Bot.SettingsFunky.UseLevelingLogic = Convert.ToBoolean(config[1]);
                                     break;
                                 case "UseAdvancedProjectileTesting":
                                     Bot.SettingsFunky.UseAdvancedProjectileTesting = Convert.ToBoolean(config[1]);
                                     break;
                                 case "IgnoreAboveAverageMobs":
                                     Bot.SettingsFunky.IgnoreAboveAverageMobs = Convert.ToBoolean(config[1]);
                                     break;
                                 case "ItemRulesSalvaging":
                                     Bot.SettingsFunky.ItemRulesSalvaging = Convert.ToBoolean(config[1]);
                                     break;
                                 case "AvoidanceRetryMin":
                                     Bot.SettingsFunky.AvoidanceRecheckMinimumRate = Convert.ToInt32(config[1]);
                                     break;
                                 case "AvoidanceRetryMax":
                                     Bot.SettingsFunky.AvoidanceRecheckMaximumRate = Convert.ToInt32(config[1]);
                                     break;
                                 case "KiteRetryMin":
                                     Bot.SettingsFunky.KitingRecheckMinimumRate = Convert.ToInt32(config[1]);
                                     break;
                                 case "KiteRetryMax":
                                     Bot.SettingsFunky.KitingRecheckMaximumRate = Convert.ToInt32(config[1]);
                                     break;
                                 case "DebugStatusBar":
                                     Bot.SettingsFunky.DebugStatusBar = Convert.ToBoolean(config[1]);
                                     break;
                                 case "LogSafeMovementOutput":
                                     Bot.SettingsFunky.LogSafeMovementOutput = Convert.ToBoolean(config[1]);
                                     break;
                                 case "EnableClusteringTargetLogic":
                                     Bot.SettingsFunky.EnableClusteringTargetLogic = Convert.ToBoolean(config[1]);
                                     break;
                                 case "ClusterDistance":
                                     Bot.SettingsFunky.ClusterDistance = Convert.ToInt32(config[1]);
                                     break;
                                 case "ClusterMinimumUnitCount":
                                     Bot.SettingsFunky.ClusterMinimumUnitCount = Convert.ToInt32(config[1]);
                                     break;
                                 case "ClusterKillLowHPUnits":
                                     Bot.SettingsFunky.ClusterKillLowHPUnits = Convert.ToBoolean(config[1]);
                                     break;
                                 case "IgnoreClusteringWhenLowHP":
                                     Bot.SettingsFunky.IgnoreClusteringWhenLowHP = Convert.ToBoolean(config[1]);
                                     break;
                                 case "IgnoreClusterLowHPValue":
                                     try
                                     {
                                         Bot.SettingsFunky.IgnoreClusterLowHPValue = Convert.ToDouble(String.Format(config[1], "F2", CultureInfo.InvariantCulture));
                                     }
                                     catch
                                     {
                                         Logging.Write("Exception converting IgnoreClusterLowHPValue to Double");
                                     }

                                     break;
                                 case "ItemRange":
                                     Bot.SettingsFunky.ItemRange = Convert.ToInt32(config[1]);
                                     break;
                                 case "GoblinMinimumRange":
                                     Bot.SettingsFunky.Class.GoblinMinimumRange = Convert.ToInt32(config[1]);
                                     break;
											case "SkipAhead":
												 Bot.SettingsFunky.SkipAhead=Convert.ToBoolean(config[1]);
												 break;
											case "GlobeRange":
												 Bot.SettingsFunky.GlobeRange=Convert.ToInt32(config[1]);
												 break;
											case "ItemRuleCustomPath":
												 Bot.SettingsFunky.ItemRuleCustomPath=config[1];
												 break;
											case "ItemRulesUnidStashing":
												 Bot.SettingsFunky.ItemRulesUnidStashing=Convert.ToBoolean(config[1]);
												 break;
											//ItemRulesUnidStashing
                             }
                         }
                     }

                 }
                 //configReader.Close();
             }

             Zeta.Common.Logging.WriteDiagnostic("[Funky] Character settings loaded!");
         }

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
					  ItemRuleCustomPath="";
                 AvoidanceRecheckMaximumRate = 3500;
                 AvoidanceRecheckMinimumRate = 550;
                 KitingRecheckMaximumRate = 4500;
                 KitingRecheckMinimumRate = 1000;
                 OOCIdentifyItems = oocIDitems;
                 BuyPotionsDuringTownRun = buyPotions;
                 EnableWaitAfterContainers = WaitForContainers;
                 UseItemRulesPickup = itemRulesPickup;
                 UseItemRules = itemRules;
					  ItemRulesUnidStashing=true;
					  ItemRulesSalvaging=true;
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
                 ItemRange = 75;
                 TreasureGoblinRange = 55;
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

					  PickupGems=new bool[] { true, true, true, true };

                 MinimumGemItemLevel = minGemLevel;
					  PickupCraftTomes=true;
					  PickupCraftPlans=true;
					  PickupFollowerItems=true;
                 MiscItemLevel = miscitemlevel;
                 UseLevelingLogic = itemlevelinglogic;
                 UseAdvancedProjectileTesting = projectiletesting;
                 IgnoreAboveAverageMobs = ignoreelites;
                 DebugStatusBar = false;
                 LogSafeMovementOutput = false;


                 EnableClusteringTargetLogic = true;
                 IgnoreClusteringWhenLowHP = true;
                 ClusterKillLowHPUnits = true;
                 ClusterDistance = 9d;
                 ClusterMinimumUnitCount = 2;
                 IgnoreClusterLowHPValue = 0.55d;
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
					  public bool bMonkSpamMantra { get; set; }

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
							bMonkSpamMantra=false;
                     bWaitForArchon = false;
                     bKiteOnlyArchon = false;
                     GoblinMinimumRange = 40;
                 }
             }
         }
    }
}