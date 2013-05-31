using System;
using System.Linq;
using Zeta;
using System.Windows.Controls;
using Zeta.Common;
using System.Globalization;
using System.Collections.ObjectModel;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
         public partial class FunkyWindow
         {

             private void DebugButtonClicked(object sender, EventArgs e)
             {
                 LBDebug.Items.Clear();

                 Button btnsender = (Button)sender;
                 if (btnsender.Name == "Objects")
                 {
                     LBDebug.Items.Add(ObjectCache.Objects.DumpDebugInfo());

                     Zeta.Common.Logging.WriteVerbose("Dumping Object Cache");
                     try
                     {

                         foreach (var item in ObjectCache.Objects.Values)
                         {
                             LBDebug.Items.Add(item.DebugString);
                         }
                     }
                     catch (InvalidOperationException)
                     {
                         LBDebug.Items.Add("End of Output due to Modification Exception");
                     }

                 }
                 else if (btnsender.Name == "Obstacles")
                 {
                     LBDebug.Items.Add(ObjectCache.Obstacles.DumpDebugInfo());

                     Zeta.Common.Logging.WriteVerbose("Dumping Obstacle Cache");
                     try
                     {
                         foreach (var item in ObjectCache.Obstacles)
                         {
                             LBDebug.Items.Add(item.Value.DebugString);
                         }
                     }
                     catch (InvalidOperationException)
                     {

                         LBDebug.Items.Add("End of Output due to Modification Exception");
                     }

                 }
                 else if (btnsender.Name == "SNO")
                 {

                     LBDebug.Items.Add(ObjectCache.cacheSnoCollection.DumpDebugInfo());

                     Zeta.Common.Logging.WriteVerbose("Dumping SNO Cache");
                     try
                     {
                         foreach (var item in ObjectCache.cacheSnoCollection)
                         {
                             LBDebug.Items.Add(item.Value.DebugString);
                         }
                     }
                     catch (InvalidOperationException)
                     {

                         LBDebug.Items.Add("End of Output due to Modification Exception");
                     }

                 }
                 else if (btnsender.Name == "RESET")
                 {

                 }
                 else if (btnsender.Name == "MGP")
                 {
                     UpdateSearchGridProvider(true);
                 }
                 else if (btnsender.Name == "TEST")
                 {
                     try
                     {
                         if (Bot.Class == null)
                             return;

                         Logging.Write("Character Information");
                         Logging.Write("Hotbar Abilities");
                         foreach (Zeta.Internals.Actors.SNOPower item in Bot.Class.HotbarAbilities)
                         {
                             Logging.Write("{0} with current rune index {1}", item.ToString(), Bot.Class.RuneIndexCache.ContainsKey(item) ? Bot.Class.RuneIndexCache[item].ToString() : "none");
                         }
                         Bot.Character.UpdateAnimationState();
                         Logging.Write("State: {0} -- SNOAnim {1}", Bot.Character.CurrentAnimationState.ToString(), Bot.Character.CurrentSNOAnim.ToString());

                     }
                     catch (Exception ex)
                     {
                         Logging.WriteVerbose("Safely Handled Exception {0}", ex.Message);
                     }

                 }

                 LBDebug.Items.Refresh();
             }

             private void WeaponItemLevelSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 if (slider_sender.Name == "Magic")
                 {
                     SettingsFunky.MinimumWeaponItemLevel[0] = Value;
                     TBMinimumWeaponLevel[0].Text = Value.ToString();
                 }
                 else
                 {
                     SettingsFunky.MinimumWeaponItemLevel[1] = Value;
                     TBMinimumWeaponLevel[1].Text = Value.ToString();
                 }
             }
             private void ArmorItemLevelSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 if (slider_sender.Name == "Magic")
                 {
                     SettingsFunky.MinimumArmorItemLevel[0] = Value;
                     TBMinimumArmorLevel[0].Text = Value.ToString();
                 }
                 else
                 {
                     SettingsFunky.MinimumArmorItemLevel[1] = Value;
                     TBMinimumArmorLevel[1].Text = Value.ToString();
                 }
             }
             private void JeweleryItemLevelSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 if (slider_sender.Name == "Magic")
                 {
                     SettingsFunky.MinimumJeweleryItemLevel[0] = Value;
                     TBMinimumJeweleryLevel[0].Text = Value.ToString();
                 }
                 else
                 {
                     SettingsFunky.MinimumJeweleryItemLevel[1] = Value;
                     TBMinimumJeweleryLevel[1].Text = Value.ToString();
                 }
             }
             private void GilesWeaponScoreSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.GilesMinimumWeaponScore = Value;
                 TBGilesWeaponScore.Text = Value.ToString();
             }
             private void GilesArmorScoreSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.GilesMinimumArmorScore = Value;
                 TBGilesArmorScore.Text = Value.ToString();
             }
             private void GilesJeweleryScoreSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.GilesMinimumJeweleryScore = Value;
                 TBGilesJeweleryScore.Text = Value.ToString();
             }

             private void LegendaryItemLevelSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.MinimumLegendaryItemLevel = Value;
                 TBMinLegendaryLevel.Text = Value.ToString();
             }
             private void HealthPotionSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.MaximumHealthPotions = Value;
                 TBMaxHealthPots.Text = Value.ToString();
             }
             private void GoldAmountSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.MinimumGoldPile = Value;
                 TBMinGoldPile.Text = Value.ToString();
             }
             private void ClusterDistanceSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.ClusterDistance = Value;
                 TBClusterDistance.Text = Value.ToString();
             }
             private void ClusterMinUnitSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.ClusterMinimumUnitCount = Value;
                 TBClusterMinUnitCount.Text = Value.ToString();
             }
             private void ClusterLowHPValueSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
                 SettingsFunky.IgnoreClusterLowHPValue = Value;
                 TBClusterLowHPValue.Text = Value.ToString();
             }
             private void EnableClusteringTargetLogicChecked(object sender, EventArgs e)
             {
                 SettingsFunky.EnableClusteringTargetLogic = !SettingsFunky.EnableClusteringTargetLogic;
             }
             private void IgnoreClusteringBotLowHPisChecked(object sender, EventArgs e)
             {
                 SettingsFunky.IgnoreClusteringWhenLowHP = !SettingsFunky.IgnoreClusteringWhenLowHP;
             }
             private void ClusteringKillLowHPChecked(object sender, EventArgs e)
             {
                 SettingsFunky.ClusterKillLowHPUnits = !SettingsFunky.ClusterKillLowHPUnits;
             }
             private void PickupCraftTomesChecked(object sender, EventArgs e)
             {
                 SettingsFunky.PickupCraftTomes = !SettingsFunky.PickupCraftTomes;
             }
             private void PickupCraftPlansChecked(object sender, EventArgs e)
             {
                 SettingsFunky.PickupCraftPlans = !SettingsFunky.PickupCraftPlans;
             }
             private void PickupFollowerItemsChecked(object sender, EventArgs e)
             {
                 SettingsFunky.PickupFollowerItems = !SettingsFunky.PickupFollowerItems;
             }
             private void GemsChecked(object sender, EventArgs e)
             {
                 CheckBox sender_ = (CheckBox)sender;
                 if (sender_.Name == "red") SettingsFunky.PickupGems[0] = !SettingsFunky.PickupGems[0];
                 if (sender_.Name == "green") SettingsFunky.PickupGems[1] = !SettingsFunky.PickupGems[1];
                 if (sender_.Name == "purple") SettingsFunky.PickupGems[2] = !SettingsFunky.PickupGems[2];
                 if (sender_.Name == "yellow") SettingsFunky.PickupGems[3] = !SettingsFunky.PickupGems[3];
             }
             private void MiscItemLevelSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.MiscItemLevel = Value;
                 TBMiscItemLevel.Text = Value.ToString();
             }
             private void IgnoreCombatRangeChecked(object sender, EventArgs e)
             {
                 SettingsFunky.IgnoreCombatRange = !SettingsFunky.IgnoreCombatRange;
             }
             private void IgnoreLootRangeChecked(object sender, EventArgs e)
             {
                 SettingsFunky.IgnoreLootRange = !SettingsFunky.IgnoreLootRange;
             }

             private void EliteRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.EliteCombatRange = Value;
                 TBEliteRange.Text = Value.ToString();
             }
             private void GoldRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.GoldRange = Value;
                 TBGoldRange.Text = Value.ToString();
             }
             private void ItemRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.ItemRange = Value;
                 TBItemRange.Text = Value.ToString();
             }
             private void ShrineRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.ShrineRange = Value;
                 TBShrineRange.Text = Value.ToString();
             }
             private void GlobeHealthSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
                 SettingsFunky.GlobeHealthPercent = Value;
                 TBGlobeHealth.Text = Value.ToString();
             }
             private void PotionHealthSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
                 SettingsFunky.PotionHealthPercent = Value;
                 TBPotionHealth.Text = Value.ToString();
             }
             private void ContainerRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.ContainerOpenRange = Value;
                 TBContainerRange.Text = Value.ToString();
             }
             private void NonEliteRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.NonEliteCombatRange = Value;
                 TBNonEliteRange.Text = Value.ToString();
             }
             private void TreasureGoblinRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.TreasureGoblinRange = Value;
                 TBGoblinRange.Text = Value.ToString();
             }
             private void TreasureGoblinMinimumRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.Class.GoblinMinimumRange = Value;
                 TBGoblinMinRange.Text = Value.ToString();
             }
             private void DestructibleSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.DestructibleRange = Value;
                 TBDestructibleRange.Text = Value.ToString();
             }
             private void ExtendCombatRangeSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.ExtendedCombatRange = Value;
                 TBExtendedCombatRange.Text = Value.ToString();
             }
             private void IgnoreCorpsesChecked(object sender, EventArgs e)
             {
                 SettingsFunky.IgnoreCorpses = !SettingsFunky.IgnoreCorpses;
             }
             private void IgnoreEliteMonstersChecked(object sender, EventArgs e)
             {
                 SettingsFunky.IgnoreAboveAverageMobs = !SettingsFunky.IgnoreAboveAverageMobs;
             }
             private void OutOfCombatMovementChecked(object sender, EventArgs e)
             {
                 SettingsFunky.OutOfCombatMovement = !SettingsFunky.OutOfCombatMovement;
             }
             private void KiteSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.KiteDistance = Value;
                 TBKiteDistance.Text = Value.ToString();
             }
             private void AfterCombatDelaySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.AfterCombatDelay = Value;
                 TBAfterCombatDelay.Text = Value.ToString();
             }
             private void AvoidanceMinimumRetrySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.AvoidanceRecheckMinimumRate = Value;
                 TBAvoidanceTimeLimits[0].Text = Value.ToString();
             }
             private void AvoidanceMaximumRetrySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.AvoidanceRecheckMaximumRate = Value;
                 TBAvoidanceTimeLimits[1].Text = Value.ToString();
             }
             private void KitingMaximumRetrySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.KitingRecheckMaximumRate = Value;
                 TBKiteTimeLimits[1].Text = Value.ToString();
             }
             private void KitingMinimumRetrySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.KitingRecheckMinimumRate = Value;
                 TBKiteTimeLimits[0].Text = Value.ToString();
             }
             //UseAdvancedProjectileTesting
             private void UseAdvancedProjectileTestingChecked(object sender, EventArgs e)
             {
                 SettingsFunky.UseAdvancedProjectileTesting = !SettingsFunky.UseAdvancedProjectileTesting;
             }
             private void AvoidanceAttemptMovementChecked(object sender, EventArgs e)
             {
                 SettingsFunky.AttemptAvoidanceMovements = !SettingsFunky.AttemptAvoidanceMovements;
             }
             private void AvoidanceRadiusSliderValueChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 string[] slider_info = slider_sender.Name.Split("_".ToCharArray());
                 int tb_index = Convert.ToInt16(slider_info[2]);
                 float currentValue = (int)slider_sender.Value;

                 TBavoidanceRadius[tb_index].Text = currentValue.ToString();
                 AvoidanceType avoidancetype = (AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);
                 dictAvoidanceRadius[avoidancetype] = currentValue;
             }

             private void AvoidanceHealthSliderValueChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 string[] slider_info = slider_sender.Name.Split("_".ToCharArray());
                 double currentValue = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
                 int tb_index = Convert.ToInt16(slider_info[2]);

                 TBavoidanceHealth[tb_index].Text = currentValue.ToString();
                 AvoidanceType avoidancetype = (AvoidanceType)Enum.Parse(typeof(AvoidanceType), slider_info[0]);


                 switch (ActorClass)
                 {
                     case Zeta.Internals.Actors.ActorClass.Barbarian:
                         dictAvoidanceHealthBarb[avoidancetype] = currentValue;
                         break;
                     case Zeta.Internals.Actors.ActorClass.DemonHunter:
                         dictAvoidanceHealthDemon[avoidancetype] = currentValue;
                         break;
                     case Zeta.Internals.Actors.ActorClass.Monk:
                         dictAvoidanceHealthMonk[avoidancetype] = currentValue;
                         break;
                     case Zeta.Internals.Actors.ActorClass.WitchDoctor:
                         dictAvoidanceHealthWitch[avoidancetype] = currentValue;
                         break;
                     case Zeta.Internals.Actors.ActorClass.Wizard:
                         dictAvoidanceHealthWizard[avoidancetype] = currentValue;
                         break;
                 }
             }

             #region ClassSettings

             private void bEnableCriticalMassChecked(object sender, EventArgs e)
             {
                 SettingsFunky.Class.bEnableCriticalMass = !SettingsFunky.Class.bEnableCriticalMass;
             }

             private void bWaitForArchonChecked(object sender, EventArgs e)
             {
                 SettingsFunky.Class.bWaitForArchon = !SettingsFunky.Class.bWaitForArchon;
             }
             private void bKiteOnlyArchonChecked(object sender, EventArgs e)
             {
                 SettingsFunky.Class.bKiteOnlyArchon = !SettingsFunky.Class.bKiteOnlyArchon;
             }

             private void bSelectiveWhirlwindChecked(object sender, EventArgs e)
             {
                 SettingsFunky.Class.bSelectiveWhirlwind = !SettingsFunky.Class.bSelectiveWhirlwind;
             }
             private void bWaitForWrathChecked(object sender, EventArgs e)
             {
                 SettingsFunky.Class.bWaitForWrath = !SettingsFunky.Class.bWaitForWrath;
             }
             private void bGoblinWrathChecked(object sender, EventArgs e)
             {
                 SettingsFunky.Class.bGoblinWrath = !SettingsFunky.Class.bGoblinWrath;
             }
             private void bFuryDumpWrathChecked(object sender, EventArgs e)
             {
                 SettingsFunky.Class.bFuryDumpWrath = !SettingsFunky.Class.bFuryDumpWrath;
             }
             private void bFuryDumpAlwaysChecked(object sender, EventArgs e)
             {
                 SettingsFunky.Class.bFuryDumpAlways = !SettingsFunky.Class.bFuryDumpAlways;
             }
             private void bMonkInnaSetChecked(object sender, EventArgs e)
             {
                 SettingsFunky.Class.bMonkInnaSet = !SettingsFunky.Class.bMonkInnaSet;
             }
             private void iDHVaultMovementDelaySliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.Class.iDHVaultMovementDelay = Value;
                 TBiDHVaultMovementDelay.Text = Value.ToString();
             }
             #endregion

             private void OOCIDChecked(object sender, EventArgs e)
             {
                 SettingsFunky.OOCIdentifyItems = !SettingsFunky.OOCIdentifyItems;
             }
             private void BuyPotionsDuringTownRunChecked(object sender, EventArgs e)
             {
                 SettingsFunky.BuyPotionsDuringTownRun = !SettingsFunky.BuyPotionsDuringTownRun;
             }
             private void EnableWaitAfterContainersChecked(object sender, EventArgs e)
             {
                 SettingsFunky.EnableWaitAfterContainers = !SettingsFunky.EnableWaitAfterContainers;
             }

             private void GemQualityLevelChanged(object sender, EventArgs e)
             {
                 SettingsFunky.MinimumGemItemLevel = (int)Enum.Parse(typeof(GemQuality), CBGemQualityLevel.Items[CBGemQualityLevel.SelectedIndex].ToString());
             }
             class GemQualityTypes : ObservableCollection<string>
             {
                 public GemQualityTypes()
                 {
                     string[] GemNames = Enum.GetNames(typeof(GemQuality));
                     foreach (var item in GemNames)
                     {
                         Add(item);
                     }
                 }
             }
             class ItemRuleTypes : ObservableCollection<string>
             {
                 public ItemRuleTypes()
                 {

                     Add("Custom");
                     Add("Soft");
                     Add("Hard");
                 }
             }
             class ItemRuleQuality : ObservableCollection<string>
             {
                 public ItemRuleQuality()
                 {
                     Add("Common");
                     Add("Normal");
                     Add("Magic");
                     Add("Rare");
                     Add("Legendary");
                 }
             }
             class GoblinPriority : ObservableCollection<string>
             {
                 public GoblinPriority()
                 {

                     Add("None");
                     Add("Normal");
                     Add("Important");
                     Add("Ridiculousness");
                 }
             }
             private void GoblinPriorityChanged(object sender, EventArgs e)
             {
                 ComboBox senderCB = (ComboBox)sender;
                 SettingsFunky.GoblinPriority = senderCB.SelectedIndex;
             }
             private void ItemRulesTypeChanged(object sender, EventArgs e)
             {
                 SettingsFunky.ItemRuleType = ItemRuleType.Items[ItemRuleType.SelectedIndex].ToString();
             }
             private void ItemRulesScoringChanged(object sender, EventArgs e)
             {
                 SettingsFunky.ItemRuleGilesScoring = ItemRuleGilesScoring.IsChecked.Value;
             }
             private void ItemRulesLogPickupChanged(object sender, EventArgs e)
             {
                 SettingsFunky.ItemRuleLogPickup = ItemRuleLogPickup.Items[ItemRuleLogPickup.SelectedIndex].ToString();
             }
             private void ItemRulesLogKeepChanged(object sender, EventArgs e)
             {
                 SettingsFunky.ItemRuleLogKeep = ItemRuleLogKeep.Items[ItemRuleLogKeep.SelectedIndex].ToString();
             }

             private void ItemRulesChecked(object sender, EventArgs e)
             {
                 SettingsFunky.UseItemRules = !SettingsFunky.UseItemRules;
             }
             private void ItemRulesPickupChecked(object sender, EventArgs e)
             {
                 SettingsFunky.UseItemRulesPickup = !SettingsFunky.UseItemRulesPickup;
             }
             private void ItemRulesItemIDsChecked(object sender, EventArgs e)
             {
                 SettingsFunky.ItemRuleUseItemIDs = !SettingsFunky.ItemRuleUseItemIDs;
             }
             private void ItemRulesDebugChecked(object sender, EventArgs e)
             {
                 SettingsFunky.ItemRuleDebug = !SettingsFunky.ItemRuleDebug;
             }
             private void ItemLevelingLogicChecked(object sender, EventArgs e)
             {
                 SettingsFunky.UseLevelingLogic = !SettingsFunky.UseLevelingLogic;
             }
             private void ItemRulesSalvagingChecked(object sender, EventArgs e)
             {
                 SettingsFunky.ItemRulesSalvaging = !SettingsFunky.ItemRulesSalvaging;
             }
             //UseLevelingLogic
             private void ItemRulesReload_Click(object sender, EventArgs e)
             {
                 try
                 {
                     Funky.ItemRulesEval.reloadFromUI();
                 }
                 catch (Exception ex)
                 {
                     Log(ex.Message + "\r\n" + ex.StackTrace);
                 }

             }
             private void DebugStatusBarChecked(object sender, EventArgs e)
             {
                 SettingsFunky.DebugStatusBar = !SettingsFunky.DebugStatusBar;
             }
             private void LogSafeMovementOutputChecked(object sender, EventArgs e)
             {
                 SettingsFunky.LogSafeMovementOutput = !SettingsFunky.LogSafeMovementOutput;
             }

             private void ExtendRangeRepChestChecked(object sender, EventArgs e)
             {
                 SettingsFunky.UseExtendedRangeRepChest = !SettingsFunky.UseExtendedRangeRepChest;
             }
             private void EnableCoffeeBreaksChecked(object sender, EventArgs e)
             {
                 SettingsFunky.EnableCoffeeBreaks = !SettingsFunky.EnableCoffeeBreaks;
             }
             private void BreakMinMinutesSliderChange(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.MinBreakTime = Value;
                 tbMinBreakTime.Text = Value.ToString();
             }
             private void BreakMaxMinutesSliderChange(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 int Value = (int)slider_sender.Value;
                 SettingsFunky.MaxBreakTime = Value;
                 tbMaxBreakTime.Text = Value.ToString();
             }

             #region OOCIDItemTextBox
             private void OOCIdentifyItemsMinValueChanged(object sender, EventArgs e)
             {
                 string lastText = OOCIdentfyItemsMinCount.Text;
                 if (isStringFullyNumerical(lastText))
                 {
                     SettingsFunky.OOCIdentifyItemsMinimumRequired = Convert.ToInt32(lastText);
                 }
             }
             private void OOCMinimumItems_KeyUp(object sender, EventArgs e)
             {
                 if (!Char.IsNumber(OOCIdentfyItemsMinCount.Text.Last()))
                 {
                     OOCIdentfyItemsMinCount.Text = OOCIdentfyItemsMinCount.Text.Substring(0, OOCIdentfyItemsMinCount.Text.Length - 1);
                 }
             }
             #endregion

             private void BreakTimeHourSliderChanged(object sender, EventArgs e)
             {
                 Slider slider_sender = (Slider)sender;
                 double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
                 SettingsFunky.breakTimeHour = Value;
                 TBBreakTimeHour.Text = Value.ToString();
             }

             private bool isStringFullyNumerical(String S, bool isDouble = false)
             {
                 if (!isDouble)
                 {
                     return !S.Any<Char>(c => !Char.IsNumber(c));
                 }
                 else
                 {
                     System.Text.RegularExpressions.Regex isnumber = new System.Text.RegularExpressions.Regex(@"^[0-9]+(\.[0-9]+)?$");
                     return isnumber.IsMatch(S);
                 }
             }

             private void OpenPluginSettings_Click(object sender, EventArgs e)
             {
                 var thisplugin = Zeta.Common.Plugins.PluginManager.Plugins.Where(p => p.Plugin.Name == "FunkyTrinity");
                 if (thisplugin.Any())
                     thisplugin.First().Plugin.DisplayWindow.Show();
             }
             private void OpenPluginFolder_Click(object sender, EventArgs e)
             {
                 System.Diagnostics.Process.Start(FolderPaths.sTrinityPluginPath);
             }

             protected override void OnClosed(EventArgs e)
             {
                 SaveFunkyConfiguration();
                 base.OnClosed(e);
             }

         }
    }
}