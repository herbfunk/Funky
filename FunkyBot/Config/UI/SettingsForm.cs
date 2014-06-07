using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FunkyBot.Cache;
using FunkyBot.Cache.Avoidance;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Config.Settings;
using FunkyBot.DBHandlers;
using FunkyBot.Game;
using FunkyBot.Movement;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace FunkyBot.Config.UI
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();

			try
			{

				tb_GlobeHealth.Value = (int)(Bot.Settings.Combat.GlobeHealthPercent * 100);
				tb_GlobeHealth.ValueChanged += tb_GlobeHealth_ValueChanged;

				tb_PotionHealth.Value = (int)(Bot.Settings.Combat.PotionHealthPercent * 100);
				tb_PotionHealth.ValueChanged += tb_PotionHealth_ValueChanged;

				tb_WellHealth.Value = (int)(Bot.Settings.Combat.HealthWellHealthPercent * 100);
				tb_WellHealth.ValueChanged += tb_WellHealth_ValueChanged;

				txt_GlobeHealth.Text = Bot.Settings.Combat.GlobeHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
				txt_WellHealth.Text = Bot.Settings.Combat.HealthWellHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
				txt_PotionHealth.Text = Bot.Settings.Combat.PotionHealthPercent.ToString("F2", CultureInfo.InvariantCulture);

				cb_CombatMovementGlobes.Checked = Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Globe);
				cb_CombatMovementGlobes.CheckedChanged += MovementTargetGlobeChecked;

				cb_CombatMovementGold.Checked = Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Gold);
				cb_CombatMovementGold.CheckedChanged += MovementTargetGoldChecked;

				cb_CombatMovementItems.Checked = Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Item);
				cb_CombatMovementItems.CheckedChanged += MovementTargetItemChecked;

				cb_CombatAllowDefaultAttack.Checked = Bot.Settings.Combat.AllowDefaultAttackAlways;
				cb_CombatAllowDefaultAttack.CheckedChanged += AllowDefaultAttackAlwaysChecked;

				cb_MovementOutOfCombatSkills.Checked = Bot.Settings.General.OutOfCombatMovement;
				cb_MovementOutOfCombatSkills.CheckedChanged += OutOfCombatMovementChecked;

				cb_ClusterTargetLogic.Checked = Bot.Settings.Cluster.EnableClusteringTargetLogic;
				cb_ClusterTargetLogic.CheckedChanged += cb_ClusterTargetLogic_CheckedChanged;
				gb_ClusteringOptions.Enabled = Bot.Settings.Cluster.EnableClusteringTargetLogic;

				txt_ClusterLogicDisableHealth.Text = Bot.Settings.Cluster.IgnoreClusterLowHPValue.ToString("F2", CultureInfo.InvariantCulture);
				txt_ClusterLogicDistance.Text = Bot.Settings.Cluster.ClusterDistance.ToString("F2", CultureInfo.InvariantCulture);
				txt_ClusterLogicMinimumUnits.Text = Bot.Settings.Cluster.ClusterMinimumUnitCount.ToString("F2", CultureInfo.InvariantCulture);

				tb_ClusterLogicDisableHealth.Value = (int)(Bot.Settings.Cluster.IgnoreClusterLowHPValue * 100);
				tb_ClusterLogicDisableHealth.ValueChanged += tb_ClusterLogicDisableHealth_ValueChanged;

				tb_ClusterLogicDistance.Value = (int)(Bot.Settings.Cluster.ClusterDistance);
				tb_ClusterLogicDistance.ValueChanged += tb_ClusterLogicDistance_ValueChanged;

				tb_ClusterLogicMinimumUnits.Value = (int)(Bot.Settings.Cluster.ClusterMinimumUnitCount);
				tb_ClusterLogicMinimumUnits.ValueChanged += tb_ClusterLogicMinimumUnits_ValueChanged;

				cb_GroupingLogic.Checked = Bot.Settings.Grouping.AttemptGroupingMovements;
				cb_GroupingLogic.CheckedChanged += GroupingBehaviorChecked;

				txt_GroupingMaxDistance.Text = Bot.Settings.Grouping.GroupingMaximumDistanceAllowed.ToString();
				txt_GroupingMinBotHealth.Text = Bot.Settings.Grouping.GroupingMinimumBotHealth.ToString("F2", CultureInfo.InvariantCulture);
				txt_GroupingMinCluster.Text = Bot.Settings.Grouping.GroupingMinimumClusterCount.ToString();
				txt_GroupingMinDistance.Text = Bot.Settings.Grouping.GroupingMinimumUnitDistance.ToString();
				txt_GroupingMinUnitsInCluster.Text = Bot.Settings.Grouping.GroupingMinimumUnitsInCluster.ToString();

				tb_GroupingMaxDistance.Value = Bot.Settings.Grouping.GroupingMaximumDistanceAllowed;
				tb_GroupingMaxDistance.ValueChanged += GroupMaxDistanceSliderChanged;

				tb_GroupingMinBotHealth.Value = (int)(Bot.Settings.Grouping.GroupingMinimumBotHealth * 100);
				tb_GroupingMinBotHealth.ValueChanged += GroupBotHealthSliderChanged;

				tb_GroupingMinCluster.Value = Bot.Settings.Grouping.GroupingMinimumClusterCount;
				tb_GroupingMinCluster.ValueChanged += GroupMinimumClusterCountSliderChanged;

				tb_GroupingMinDistance.Value = Bot.Settings.Grouping.GroupingMinimumUnitDistance;
				tb_GroupingMinDistance.ValueChanged += GroupMinimumUnitDistanceSliderChanged;

				tb_GroupingMinUnitsInCluster.Value = Bot.Settings.Grouping.GroupingMinimumUnitsInCluster;
				tb_GroupingMinUnitsInCluster.ValueChanged += GroupMinimumUnitsInClusterSliderChanged;

				cb_AvoidanceLogic.Checked = Bot.Settings.Avoidance.AttemptAvoidanceMovements;
				cb_AvoidanceLogic.CheckedChanged += AvoidanceAttemptMovementChecked;

				AvoidanceValue[] avoidanceValues = Bot.Settings.Avoidance.Avoidances.ToArray();
				TBavoidanceHealth = new TextBox[avoidanceValues.Length - 1];
				TBavoidanceRadius = new TextBox[avoidanceValues.Length - 1];
				TBavoidanceWeight = new TextBox[avoidanceValues.Length - 1];

				int alternatingColor = 0;
				for (int i = 0; i < avoidanceValues.Length - 1; i++)
				{
					UserControlAvoidance ac = new UserControlAvoidance(i);

					flowLayoutPanel_Avoidances.Controls.Add(ac);

					alternatingColor++;
				}

				cb_EnableFleeing.Checked = Bot.Settings.Fleeing.EnableFleeingBehavior;
				cb_EnableFleeing.CheckedChanged += cb_EnableFleeing_CheckedChanged;

				trackbar_FleeMaxMonsterDistance.Value = Bot.Settings.Fleeing.FleeMaxMonsterDistance;
				trackbar_FleeMaxMonsterDistance.ValueChanged += trackbar_FleeMaxMonsterDistance_ValueChanged;

				txt_FleeMaxMonsterDistance.Text = Bot.Settings.Fleeing.FleeMaxMonsterDistance.ToString();

				trackbar_FleeBotMinHealth.Value = (int)(Bot.Settings.Fleeing.FleeBotMinimumHealthPercent * 100);
				trackbar_FleeBotMinHealth.ValueChanged += trackbar_FleeBotMinHealth_ValueChanged;

				txt_FleeBotMinHealth.Text = Bot.Settings.Fleeing.FleeBotMinimumHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
				groupBox_FleeOptions.Enabled = Bot.Settings.Fleeing.EnableFleeingBehavior;


				cb_FleeUnitElectrified.Checked = Bot.Settings.Fleeing.FleeUnitElectrified;
				cb_FleeUnitElectrified.CheckedChanged += cb_FleeUnitElectrified_CheckedChanged;

				cb_FleeUnitFast.Checked = Bot.Settings.Fleeing.FleeUnitIgnoreFast;
				cb_FleeUnitFast.CheckedChanged += cb_FleeUnitFast_CheckedChanged;

				cb_FleeUnitNormal.Checked = Bot.Settings.Fleeing.FleeUnitNormal;
				cb_FleeUnitNormal.CheckedChanged += cb_FleeUnitNormal_CheckedChanged;

				cb_FleeUnitRanged.Checked = Bot.Settings.Fleeing.FleeUnitIgnoreRanged;
				cb_FleeUnitRanged.CheckedChanged += cb_FleeUnitRanged_CheckedChanged;

				cb_FleeUnitRareElite.Checked = Bot.Settings.Fleeing.FleeUnitRareElite;
				cb_FleeUnitRareElite.CheckedChanged += cb_FleeUnitRareElite_CheckedChanged;

				cb_FleeUnitSucideBomber.Checked = Bot.Settings.Fleeing.FleeUnitIgnoreSucideBomber;
				cb_FleeUnitSucideBomber.CheckedChanged += cb_FleeUnitSucideBomber_CheckedChanged;

				cb_FleeUnitAboveAverageHitPoints.Checked = Bot.Settings.Fleeing.FleeUnitAboveAverageHitPoints;
				cb_FleeUnitAboveAverageHitPoints.CheckedChanged += cb_FleeUnitAboveAverageHitPoints_CheckedChanged;


				List<TabPage> RemovalTabPages = new List<TabPage> { tabPage_DemonHunter, tabPage_Barbarian, tabPage_Wizard, tabPage_WitchDoctor, tabPage_Monk, tabPage_Crusader };
				switch (Bot.Character.Account.ActorClass)
				{
					case ActorClass.DemonHunter:
						trackBar_DemonHunterValutDelay.Value = Bot.Settings.DemonHunter.iDHVaultMovementDelay;
						trackBar_DemonHunterValutDelay.ValueChanged += trackBar_DemonHunterValutDelay_ValueChanged;
						txt_DemonHunterVaultDelay.Text = Bot.Settings.DemonHunter.iDHVaultMovementDelay.ToString();
						RemovalTabPages.Remove(tabPage_DemonHunter);
						break;
					case ActorClass.Barbarian:
						cb_BarbFuryDumpAlways.Checked = Bot.Settings.Barbarian.bBarbUseWOTBAlways;
						cb_BarbFuryDumpAlways.CheckedChanged += cb_BarbFuryDumpAlways_CheckedChanged;

						cb_BarbFuryDumpWrath.Checked = Bot.Settings.Barbarian.bFuryDumpWrath;
						cb_BarbFuryDumpWrath.CheckedChanged += cb_BarbFuryDumpWrath_CheckedChanged;

						cb_BarbGoblinWrath.Checked = Bot.Settings.Barbarian.bGoblinWrath;
						cb_BarbGoblinWrath.CheckedChanged += cb_BarbGoblinWrath_CheckedChanged;

						cb_BarbUseWOTBAlways.Checked = Bot.Settings.Barbarian.bBarbUseWOTBAlways;
						cb_BarbUseWOTBAlways.CheckedChanged += cb_BarbUseWOTBAlways_CheckedChanged;

						cb_BarbWaitForWrath.Checked = Bot.Settings.Barbarian.bWaitForWrath;
						cb_BarbWaitForWrath.CheckedChanged += cb_BarbWaitForWrath_CheckedChanged;

						cb_BarbSelectiveWhirldWind.Checked = Bot.Settings.Barbarian.bSelectiveWhirlwind;
						cb_BarbSelectiveWhirldWind.CheckedChanged += cb_BarbSelectiveWhirlwind_CheckedChanged;

						RemovalTabPages.Remove(tabPage_Barbarian);
						break;
					case ActorClass.Wizard:
						cb_WizardKiteOnlyArchon.Checked = Bot.Settings.Wizard.bKiteOnlyArchon;
						cb_WizardKiteOnlyArchon.CheckedChanged += cb_WizardKiteOnlyArchon_CheckedChanged;

						cb_WizardTeleportFleeing.Checked = Bot.Settings.Wizard.bTeleportFleeWhenLowHP;
						cb_WizardTeleportFleeing.CheckedChanged += cb_WizardTeleportFleeing_CheckedChanged;

						cb_WizardTeleportIntoGroups.Checked = Bot.Settings.Wizard.bTeleportIntoGrouping;
						cb_WizardTeleportIntoGroups.CheckedChanged += cb_WizardTeleportIntoGroups_CheckedChanged;

						cb_WizardWaitForArchon.Checked = Bot.Settings.Wizard.bWaitForArchon;
						cb_WizardWaitForArchon.CheckedChanged += cb_WizardWaitForArchon_CheckedChanged;

						cb_WizardCancelArchonRebuff.Checked = Bot.Settings.Wizard.bCancelArchonRebuff;
						cb_WizardCancelArchonRebuff.CheckedChanged += cb_CacnelArchonRebuff_CheckedChanged;

						RemovalTabPages.Remove(tabPage_Wizard);
						break;
					case ActorClass.Witchdoctor:
						RemovalTabPages.Remove(tabPage_WitchDoctor);
						break;
					case ActorClass.Monk:
						cb_MonkMaintainSweepingWinds.Checked = Bot.Settings.Monk.bMonkMaintainSweepingWind;
						cb_MonkMaintainSweepingWinds.CheckedChanged += bMonkMaintainSweepingWindChecked;

						cb_MonkSpamMantra.Checked = Bot.Settings.Monk.bMonkSpamMantra;
						cb_MonkSpamMantra.CheckedChanged += cb_MonkSpamMantra_CheckedChanged;

						RemovalTabPages.Remove(tabPage_Monk);
						break;
					case ActorClass.Crusader:
						RemovalTabPages.Remove(tabPage_Crusader);
						break;
				}
				foreach (var removalTabPage in RemovalTabPages)
				{
					tabControl_Combat.TabPages.Remove(removalTabPage);
				}



				cb_TargetingIgnoreCorpses.Checked = Bot.Settings.Targeting.IgnoreCorpses;
				cb_TargetingIgnoreCorpses.CheckedChanged += cb_TargetingIgnoreCorpses_CheckedChanged;

				cb_TargetingIgnoreRareElites.Checked = Bot.Settings.Targeting.IgnoreAboveAverageMobs;
				cb_TargetingIgnoreRareElites.CheckedChanged += cb_TargetingIgnoreRareElites_CheckedChanged;

				cb_TargetingIncreaseRangeRareChests.Checked = Bot.Settings.Targeting.UseExtendedRangeRepChest;
				cb_TargetingIncreaseRangeRareChests.CheckedChanged += cb_TargetingIncreaseRangeRareChests_CheckedChanged;

				comboBox_TargetingGoblinPriority.SelectedIndex = Bot.Settings.Targeting.GoblinPriority;
				comboBox_TargetingGoblinPriority.SelectedIndexChanged += comboBox_TargetingGoblinPriority_SelectedIndexChanged;

				panel_TargetingPriorityCloseRangeMinUnits.Enabled = Bot.Settings.Targeting.PrioritizeCloseRangeUnits;
				tb_TargetingPriorityCloseRangeUnitsCount.Value = Bot.Settings.Targeting.PrioritizeCloseRangeMinimumUnits;
				tb_TargetingPriorityCloseRangeUnitsCount.ValueChanged += tb_TargetingPriorityCloseRangeUnitsCount_ValueChanged;

				txt_TargetingPriorityCloseRangeUnitsCount.Text = Bot.Settings.Targeting.PrioritizeCloseRangeMinimumUnits.ToString();

				cb_TargetingPrioritizeCloseRangeUnits.Checked = Bot.Settings.Targeting.PrioritizeCloseRangeUnits;
				cb_TargetingPrioritizeCloseRangeUnits.CheckedChanged += cb_TargetingPrioritizeCloseRangeUnits_CheckedChanged;

				cb_TargetingUnitExceptionsRanged.Checked = Bot.Settings.Targeting.UnitExceptionRangedUnits;
				cb_TargetingUnitExceptionsRanged.CheckedChanged += cb_TargetingUnitExceptionsRanged_CheckedChanged;

				cb_TargetingUnitExceptionsSpawner.Checked = Bot.Settings.Targeting.UnitExceptionSpawnerUnits;
				cb_TargetingUnitExceptionsSpawner.CheckedChanged += cb_TargetingUnitExceptionsSpawner_CheckedChanged;

				cb_TargetingUnitExceptionsSucideBomber.Checked = Bot.Settings.Targeting.UnitExceptionSucideBombers;
				cb_TargetingUnitExceptionsSucideBomber.CheckedChanged += cb_TargetingUnitExceptionsSucideBomber_CheckedChanged;

				cb_TargetingUnitExceptionsLowHealth.Checked = Bot.Settings.Targeting.UnitExceptionLowHP;
				cb_TargetingUnitExceptionsLowHealth.CheckedChanged += cb_TargetingUnitExceptionsLowHealth_CheckedChanged;

				panel_TargetingUnitExceptionsLowHealthMaxDistance.Enabled = Bot.Settings.Targeting.UnitExceptionLowHP;
				tb_TargetingUnitExceptionLowHealthMaxDistance.Value = Bot.Settings.Targeting.UnitExceptionLowHPMaximumDistance;
				tb_TargetingUnitExceptionLowHealthMaxDistance.ValueChanged += tb_TargetingUnitExceptionLowHealthMaxDistance_ValueChanged;

				txt_TargetingUnitExceptionLowHealthMaxDistance.Text = Bot.Settings.Targeting.UnitExceptionLowHPMaximumDistance.ToString();



				UserControlTargetRange eliteRange = new UserControlTargetRange(Bot.Settings.Ranges.EliteCombatRange, "Rare/Elite/Unique Unit Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.EliteCombatRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(eliteRange);

				UserControlTargetRange normalRange = new UserControlTargetRange(Bot.Settings.Ranges.NonEliteCombatRange, "Normal Unit Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.NonEliteCombatRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(normalRange);

				UserControlTargetRange shrineRange = new UserControlTargetRange(Bot.Settings.Ranges.ShrineRange, "Shrine Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.ShrineRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(shrineRange);

				UserControlTargetRange CursedshrineRange = new UserControlTargetRange(Bot.Settings.Ranges.CursedShrineRange, "Cursed Shrine Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.CursedShrineRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(CursedshrineRange);

				UserControlTargetRange GoldRange = new UserControlTargetRange(Bot.Settings.Ranges.GoldRange, "Gold Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.GoldRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(GoldRange);

				UserControlTargetRange GlobeRange = new UserControlTargetRange(Bot.Settings.Ranges.GlobeRange, "Globe Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.GlobeRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(GlobeRange);


				UserControlTargetRange containerRange = new UserControlTargetRange(Bot.Settings.Ranges.ContainerOpenRange, "Container Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.ContainerOpenRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(containerRange);

				UserControlTargetRange CursedChestRange = new UserControlTargetRange(Bot.Settings.Ranges.CursedChestRange, "Cursed Chest Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.CursedChestRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(CursedChestRange);

				UserControlTargetRange DoorRange = new UserControlTargetRange(Bot.Settings.Ranges.NonEliteCombatRange, "Door Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.DoorRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(DoorRange);

				UserControlTargetRange destructibleRange = new UserControlTargetRange(Bot.Settings.Ranges.DestructibleRange, "Destructible Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.DestructibleRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(destructibleRange);

				UserControlTargetRange PoolsofReflectionRange = new UserControlTargetRange(Bot.Settings.Ranges.PoolsOfReflectionRange, "Pools of Reflection Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.PoolsOfReflectionRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(PoolsofReflectionRange);

				UserControlTargetRange itemRange = new UserControlTargetRange(Bot.Settings.Ranges.ItemRange, "Item Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.ItemRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(itemRange);

				UserControlTargetRange PotionRange = new UserControlTargetRange(Bot.Settings.Ranges.PotionRange, "Potion Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.PotionRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(PotionRange);

				UserControlTargetRange GoblinRange = new UserControlTargetRange(Bot.Settings.Ranges.TreasureGoblinRange, "Goblin Range")
				{
					UpdateValue = i => { Bot.Settings.Ranges.TreasureGoblinRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(GoblinRange);


				cb_TargetRangeIgnoreKillRangeProfile.Checked = Bot.Settings.Ranges.IgnoreCombatRange;
				cb_TargetRangeIgnoreKillRangeProfile.CheckedChanged += cb_TargetRangeIgnoreKillRangeProfile_CheckedChanged;

				cb_TargetRangeIgnoreLootProfileRange.Checked = Bot.Settings.Ranges.IgnoreLootRange;
				cb_TargetRangeIgnoreLootProfileRange.CheckedChanged += cb_TargetRangeIgnoreLootProfileRange_CheckedChanged;

				cb_TargetRangeIgnoreProfileBlacklists.Checked = Bot.Settings.Ranges.IgnoreProfileBlacklists;
				cb_TargetRangeIgnoreProfileBlacklists.CheckedChanged += cb_TargetRangeIgnoreProfileBlacklists_CheckedChanged;

				cb_TargetLOSBossUniques.Checked = Bot.Settings.LOSMovement.AllowUniqueBoss;
				cb_TargetLOSBossUniques.CheckedChanged += cb_TargetLOSBossUniques_CheckedChanged;

				cb_TargetLOSCursedChestShrine.Checked = Bot.Settings.LOSMovement.AllowCursedChestShrines;
				cb_TargetLOSCursedChestShrine.CheckedChanged += cb_TargetLOSCursedChestShrine_CheckedChanged;

				cb_TargetLOSGoblins.Checked = Bot.Settings.LOSMovement.AllowTreasureGoblin;
				cb_TargetLOSGoblins.CheckedChanged += cb_TargetLOSGoblins_CheckedChanged;

				cb_TargetLOSRanged.Checked = Bot.Settings.LOSMovement.AllowRanged;
				cb_TargetLOSRanged.CheckedChanged += cb_TargetLOSRanged_CheckedChanged;

				cb_TargetLOSRareChests.Checked = Bot.Settings.LOSMovement.AllowRareLootContainer;
				cb_TargetLOSRareChests.CheckedChanged += cb_TargetLOSRareChests_CheckedChanged;

				cb_TargetLOSRareElite.Checked = Bot.Settings.LOSMovement.AllowRareElites;
				cb_TargetLOSRareElite.CheckedChanged += cb_TargetLOSRareElite_CheckedChanged;

				cb_TargetLOSSpawnerUnits.Checked = Bot.Settings.LOSMovement.AllowSpawnerUnits;
				cb_TargetLOSSpawnerUnits.CheckedChanged += cb_TargetLOSSpawnerUnits_CheckedChanged;

				cb_TargetLOSSucideBombers.Checked = Bot.Settings.LOSMovement.AllowSucideBomber;
				cb_TargetLOSSucideBombers.CheckedChanged += cb_TargetLOSSucideBombers_CheckedChanged;

				cb_LOSEventSwitches.Checked = Bot.Settings.LOSMovement.AllowEventSwitches;
				cb_LOSEventSwitches.CheckedChanged += cb_TargetLOSEventSwitches_CheckedChanged;

				cb_TargetingShrineEmpowered.Checked = Bot.Settings.Targeting.UseShrineTypes[5];
				cb_TargetingShrineEmpowered.CheckedChanged += TargetingShrineCheckedChanged;

				cb_TargetingShrineEnlightnment.Checked = Bot.Settings.Targeting.UseShrineTypes[1];
				cb_TargetingShrineEnlightnment.CheckedChanged += TargetingShrineCheckedChanged;

				cb_TargetingShrineFleeting.Checked = Bot.Settings.Targeting.UseShrineTypes[0];
				cb_TargetingShrineFleeting.CheckedChanged += TargetingShrineCheckedChanged;

				cb_TargetingShrineFortune.Checked = Bot.Settings.Targeting.UseShrineTypes[3];
				cb_TargetingShrineFortune.CheckedChanged += TargetingShrineCheckedChanged;

				cb_TargetingShrineFrenzy.Checked = Bot.Settings.Targeting.UseShrineTypes[2];
				cb_TargetingShrineFrenzy.CheckedChanged += TargetingShrineCheckedChanged;

				cb_TargetingShrineProtection.Checked = Bot.Settings.Targeting.UseShrineTypes[4];
				cb_TargetingShrineProtection.CheckedChanged += TargetingShrineCheckedChanged;

				tb_GeneralGoldInactivityValue.Value = Bot.Settings.Plugin.GoldInactivityTimeoutSeconds;
				tb_GeneralGoldInactivityValue.ValueChanged += tb_GeneralGoldInactivityValue_ValueChanged;

				txt_GeneralGoldInactivityValue.Text = Bot.Settings.Plugin.GoldInactivityTimeoutSeconds.ToString();

				cb_GeneralAllowBuffInTown.Checked = Bot.Settings.General.AllowBuffingInTown;
				cb_GeneralAllowBuffInTown.CheckedChanged += cb_GeneralAllowBuffInTown_CheckedChanged;

				txt_GeneralEndOfCombatDelayValue.Text = Bot.Settings.General.AfterCombatDelay.ToString();

				tb_GeneralEndOfCombatDelayValue.Value = Bot.Settings.General.AfterCombatDelay;
				tb_GeneralEndOfCombatDelayValue.ValueChanged += tb_GeneralEndOfCombatDelayValue_ValueChanged;

				cb_GeneralApplyEndDelayToContainers.Checked = Bot.Settings.General.EnableWaitAfterContainers;
				cb_GeneralApplyEndDelayToContainers.CheckedChanged += cb_GeneralApplyEndDelayToContainers_CheckedChanged;

				cb_GeneralSkipAhead.Checked=Bot.Settings.Debug.SkipAhead;
				cb_GeneralSkipAhead.CheckedChanged += cb_GeneralSkipAhead_CheckedChanged;
				

				cb_AdventureModeEnabled.Checked = Bot.Settings.AdventureMode.EnableAdventuringMode;
				cb_AdventureModeEnabled.CheckedChanged += cb_AdventureModeEnabled_CheckedChanged;

				cb_TownRunStashHoradricCache.Checked = Bot.Settings.TownRun.StashHoradricCache;
				cb_TownRunStashHoradricCache.CheckedChanged += cb_TownRunStashHoradricCache_CheckedChanged;

				cb_TownRunBloodShardGambling.Checked = Bot.Settings.TownRun.EnableBloodShardGambling;
				cb_TownRunBloodShardGambling.CheckedChanged += cb_TownRunBloodShardGambling_CheckedChanged;

				txt_TownRunBloodShardMinimumValue.Text = Bot.Settings.TownRun.MinimumBloodShards.ToString();
				tb_TownRunBloodShardMinimumValue.Value = Bot.Settings.TownRun.MinimumBloodShards;
				tb_TownRunBloodShardMinimumValue.ValueChanged += tb_TownRunBloodShardMinimumValue_ValueChanged;

				cb_TownRunBuyPotions.Checked = Bot.Settings.TownRun.BuyPotionsDuringTownRun;
				cb_TownRunBuyPotions.CheckedChanged += cb_TownRunBuyPotionsCheckedChanged;

				cb_TownRunIDLegendaries.Checked = Bot.Settings.TownRun.IdentifyLegendaries;
				cb_TownRunIDLegendaries.CheckedChanged += cb_TownRunIDLegendariesCheckedChanged;

				combo_TownRunSalvageWhiteItems.SelectedIndex = Bot.Settings.TownRun.SalvageWhiteItemLevel == 0 ? 0 : Bot.Settings.TownRun.SalvageWhiteItemLevel == 1 ? 1 : 2;
				combo_TownRunSalvageWhiteItems.SelectedIndexChanged += comboBox_TownRunSalvageWhiteItems_SelectedIndexChanged;

				combo_TownRunSalvageRareItems.SelectedIndex = Bot.Settings.TownRun.SalvageRareItemLevel == 0 ? 0 : Bot.Settings.TownRun.SalvageRareItemLevel == 1 ? 1 : 2;
				combo_TownRunSalvageRareItems.SelectedIndexChanged += comboBox_TownRunSalvageRareItems_SelectedIndexChanged;

				combo_TownRunSalvageMagicItems.SelectedIndex = Bot.Settings.TownRun.SalvageMagicItemLevel == 0 ? 0 : Bot.Settings.TownRun.SalvageMagicItemLevel == 1 ? 1 : 2;
				combo_TownRunSalvageMagicItems.SelectedIndexChanged += comboBox_TownRunSalvageMagicalItems_SelectedIndexChanged;

				combo_TownRunSalvageLegendaryItems.SelectedIndex = Bot.Settings.TownRun.SalvageLegendaryItemLevel == 0 ? 0 : Bot.Settings.TownRun.SalvageLegendaryItemLevel == 1 ? 1 : 2;
				combo_TownRunSalvageLegendaryItems.SelectedIndexChanged += comboBox_TownRunSalvageLegendaryItems_SelectedIndexChanged;

				bool noFlags = Bot.Settings.TownRun.BloodShardGambleItems.Equals(BloodShardGambleItems.None);
				var gambleItems = Enum.GetValues(typeof(BloodShardGambleItems));
				Func<object, string> fRetrieveNames = s => Enum.GetName(typeof(BloodShardGambleItems), s);
				foreach (var gambleItem in gambleItems)
				{
					var thisGambleItem = (BloodShardGambleItems)gambleItem;
					if (thisGambleItem.Equals(BloodShardGambleItems.None) || thisGambleItem.Equals(BloodShardGambleItems.All)) continue;

					string gambleItemName = fRetrieveNames(gambleItem);
					CheckBox cb = new CheckBox
					{
						Name = gambleItemName,
						Text = gambleItemName,
						Checked = !noFlags && Bot.Settings.TownRun.BloodShardGambleItems.HasFlag(thisGambleItem),
					};
					cb.CheckedChanged += BloodShardGambleItemsChanged;

					flowLayout_TownRunBloodShardItems.Controls.Add(cb);
				}


				cb_ItemRules.Checked = Bot.Settings.ItemRules.UseItemRules;
				cb_ItemRules.CheckedChanged += cb_ItemRules_CheckedChanged;

				if (Bot.Settings.ItemRules.ItemRuleCustomPath != String.Empty)
				{
					txt_ItemRulesCustomLocation.Text = Bot.Settings.ItemRules.ItemRuleCustomPath;
				}

				cb_ItemRulesPickup.Checked = Bot.Settings.ItemRules.UseItemRulesPickup;
				cb_ItemRulesPickup.CheckedChanged += cb_ItemRulesPickup_CheckedChanged;

				//cb_ItemRulesUnidStashing.Checked = Bot.Settings.ItemRules.ItemRulesUnidStashing;
				//cb_ItemRulesUnidStashing.CheckedChanged += cb_ItemRulesUnidStashing_CheckedChanged;

				cb_ItemRulesDebugging.Checked = Bot.Settings.ItemRules.ItemRuleDebug;
				cb_ItemRulesDebugging.CheckedChanged += cb_ItemRulesDebugging_CheckedChanged;

				cb_ItemRulesUseItemIDs.Checked = Bot.Settings.ItemRules.ItemRuleUseItemIDs;
				cb_ItemRulesUseItemIDs.CheckedChanged += cb_ItemRulesUseItemIDs_CheckedChanged;

				btn_ItemRulesCustomBrowse.Click += ItemRulesBrowse_Click;
				btn_ItemRulesOpenFolder.Click += ItemRulesOpenFolder_Click;
				btn_ItemRulesReloadRules.Click += ItemRulesReload_Click;

				comboBox_ItemRulesType.SelectedIndex = Bot.Settings.ItemRules.ItemRuleType == "Hard" ? 1 : Bot.Settings.ItemRules.ItemRuleType == "Soft" ? 2 : 0;
				comboBox_ItemRulesType.SelectedIndexChanged += comboBox_ItemRulesType_SelectedIndexChanged;

				comboBox_ItemRulesLogPickup.Text = Bot.Settings.ItemRules.ItemRuleLogPickup;
				comboBox_ItemRulesLogPickup.SelectedIndexChanged += comboBox_ItemRulesLogPickup_SelectedIndexChanged;

				combobox_ItemRulesLogStashed.Text = Bot.Settings.ItemRules.ItemRuleLogKeep;
				combobox_ItemRulesLogStashed.SelectedIndexChanged += cb_ItemRulesLogStashed_SelectedIndexChanged;

				cb_LootPickupCraftPlans.Checked = Bot.Settings.Loot.PickupCraftPlans;
				cb_LootPickupCraftPlans.CheckedChanged += cb_LootPickupCraftPlans_CheckedChanged;

				comboBox_LootGemQuality.Text = Enum.GetName(typeof(GemQuality), Bot.Settings.Loot.MinimumGemItemLevel).ToString();
				comboBox_LootGemQuality.SelectedIndexChanged += GemQualityLevelChanged;

				cb_LootGemAMETHYST.Checked = Bot.Settings.Loot.PickupGems[2];
				cb_LootGemAMETHYST.CheckedChanged += cb_LootGemAMETHYST_CheckedChanged;

				cb_LootGemDiamond.Checked = Bot.Settings.Loot.PickupGemDiamond;
				cb_LootGemDiamond.CheckedChanged += cb_LootGemDiamond_CheckedChanged;

				cb_LootGemEMERALD.Checked = Bot.Settings.Loot.PickupGems[1];
				cb_LootGemEMERALD.CheckedChanged += cb_LootGemEMERALD_CheckedChanged;

				cb_LootGemRUBY.Checked = Bot.Settings.Loot.PickupGems[0];
				cb_LootGemRUBY.CheckedChanged += cb_LootGemRUBY_CheckedChanged;

				cb_LootGemTOPAZ.Checked = Bot.Settings.Loot.PickupGems[3];
				cb_LootGemTOPAZ.CheckedChanged += cb_LootGemTOPAZ_CheckedChanged;

				txt_LootMinimumGold.Text = Bot.Settings.Loot.MinimumGoldPile.ToString();
				tb_LootMinimumGold.Value = Bot.Settings.Loot.MinimumGoldPile;
				tb_LootMinimumGold.ValueChanged += tb_LootMinimumGold_ValueChanged;

				txt_LootHealthPotions.Text = Bot.Settings.Loot.MaximumHealthPotions.ToString();
				tb_LootHealthPotions.Value = Bot.Settings.Loot.MaximumHealthPotions;
				tb_LootHealthPotions.ValueChanged += tb_LootHealthPotions_ValueChanged;

				cb_LootCraftMats.Checked = Bot.Settings.Loot.PickupCraftMaterials;
				cb_LootCraftMats.CheckedChanged += cb_LootCraftMats_CheckedChanged;

				cb_LootInfernoKeys.Checked = Bot.Settings.Loot.PickupInfernalKeys;
				cb_LootInfernoKeys.CheckedChanged += cb_LootInfernoKeys_CheckedChanged;

				cb_LootExpBooks.Checked = Bot.Settings.Loot.ExpBooks;
				cb_LootExpBooks.CheckedChanged += cb_LootExpBooks_CheckedChanged;

				cb_LootKeyStones.Checked = Bot.Settings.Loot.PickupKeystoneFragments;
				cb_LootKeyStones.CheckedChanged += cb_LootKeyStoneFragments_CheckedChanged;


				cb_DebugDataLogging.Checked = Bot.Settings.Debug.DebuggingData;
				cb_DebugDataLogging.CheckedChanged += cb_DebugDataLogging_CheckedChanged;

				cb_DebugStatusBar.Checked = Bot.Settings.Debug.DebugStatusBar;
				cb_DebugStatusBar.CheckedChanged += cb_DebugStatusBar_CheckedChanged;

				var LogLevels = Enum.GetValues(typeof(LogLevel));
				Func<object, string> fRetrieveLogLevelNames = s => Enum.GetName(typeof(LogLevel), s);
				bool noLogFlags = Bot.Settings.Debug.FunkyLogFlags.Equals(LogLevel.None);
				foreach (var logLevel in LogLevels)
				{
					LogLevel thisloglevel = (LogLevel)logLevel;
					if (thisloglevel.Equals(LogLevel.None) || thisloglevel.Equals(LogLevel.All)) continue;

					string loglevelName = fRetrieveLogLevelNames(logLevel);
					CheckBox cb = new CheckBox
					{
						Name = loglevelName,
						Text = loglevelName,
						Checked = !noLogFlags && Bot.Settings.Debug.FunkyLogFlags.HasFlag(thisloglevel),
					};
					cb.CheckedChanged += FunkyLogLevelChanged;

					flowLayout_DebugFunkyLogLevels.Controls.Add(cb);
				}


				//Misc Stats
				try
				{
					GameStats cur = Bot.Game.CurrentGameStats;

					flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry("\r\n== CURRENT GAME SUMMARY =="));
					flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry(String.Format("Total Profiles:{0}\r\nDeaths:{1} TotalTime:{2} TotalGold:{3} TotalXP:{4}\r\nTownRuns {6} Items Gambled {7} Bounties Completed {8}\r\n{5}",
															cur.Profiles.Count, cur.TotalDeaths, cur.TotalTimeRunning.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), cur.TotalGold, cur.TotalXP, cur.TotalLootTracker.ToString(), cur.TotalTownRuns,cur.TotalItemsGambled,cur.TotalBountiesCompleted)));

					if (Bot.Game.CurrentGameStats.Profiles.Count > 0)
					{
						flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry("\r\n== PROFILES =="));
						foreach (var item in Bot.Game.CurrentGameStats.Profiles)
						{
							flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry(String.Format("{0}\r\nDeaths:{1} TotalTime:{2} TotalGold:{3} TotalXP:{4}\r\n{5}",
								item.ProfileName, item.DeathCount, item.TotalTimeSpan.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), item.TotalGold, item.TotalXP, item.LootTracker.ToString())));
						}
					}


					TotalStats all = Bot.Game.TrackingStats;
					flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry("\r\n== CURRENT GAME SUMMARY =="));
					flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry(String.Format("Total Games:{0} -- Total Unique Profiles:{1}\r\nDeaths:{2} TotalTime:{3} TotalGold:{4} TotalXP:{5}\r\n{6}",
															all.GameCount, all.Profiles.Count, all.TotalDeaths, all.TotalTimeRunning.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), all.TotalGold, all.TotalXP, all.TotalLootTracker.ToString())));


				}
				catch
				{
					flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry("Exception Handled"));
				}

			}
			catch (Exception ex)
			{
				Logger.DBLog.InfoFormat("Exception During Settings Form Initalize!\r\n{0}\r\n{1}\r\n{2}", ex.Message, ex.Source, ex.StackTrace);
				throw;
			}
		}

		private void ItemRulesOpenFolder_Click(object sender, EventArgs e)
		{
			Process.Start(Path.Combine(FolderPaths.PluginPath, "ItemRules", "Rules"));
		}
		private void ItemRulesReload_Click(object sender, EventArgs e)
		{
			if (Bot.Character.ItemRulesEval == null)
			{
				Logger.DBLog.InfoFormat("Cannot reload rules until bot has started", true);
				return;
			}

			try
			{
				Bot.Character.ItemRulesEval.reloadFromUI();
			}
			catch (Exception ex)
			{
				Logger.DBLog.InfoFormat(ex.Message + "\r\n" + ex.StackTrace);
			}

		}
		private void ItemRulesBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog OFD = new FolderBrowserDialog
			{

			};
			DialogResult OFD_Result = OFD.ShowDialog();

			if (OFD_Result == System.Windows.Forms.DialogResult.OK)
			{
				try
				{
					Bot.Settings.ItemRules.ItemRuleCustomPath = OFD.SelectedPath;
					txt_ItemRulesCustomLocation.Text = Bot.Settings.ItemRules.ItemRuleCustomPath;
				}
				catch
				{

				}
			}
		}

		private void tb_GlobeHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			Bot.Settings.Combat.GlobeHealthPercent = Value;
			txt_GlobeHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}

		private void tb_WellHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			Bot.Settings.Combat.HealthWellHealthPercent = Value;
			txt_WellHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}

		private void tb_PotionHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			Bot.Settings.Combat.PotionHealthPercent = Value;
			txt_PotionHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}



		private void SettingsFor_Load(object sender, EventArgs e)
		{
			Text = Bot.Character.Account.CurrentHeroName;
		}



		private void FunkyLogLevelChanged(object sender, EventArgs e)
		{
			CheckBox cbSender = (CheckBox)sender;
			LogLevel LogLevelValue = (LogLevel)Enum.Parse(typeof(LogLevel), cbSender.Name);

			if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevelValue))
				Bot.Settings.Debug.FunkyLogFlags &= ~LogLevelValue;
			else
				Bot.Settings.Debug.FunkyLogFlags |= LogLevelValue;
		}
		private void BloodShardGambleItemsChanged(object sender, EventArgs e)
		{
			CheckBox cbSender = (CheckBox)sender;
			BloodShardGambleItems LogLevelValue = (BloodShardGambleItems)Enum.Parse(typeof(BloodShardGambleItems), cbSender.Name);

			if (Bot.Settings.TownRun.BloodShardGambleItems.HasFlag(LogLevelValue))
				Bot.Settings.TownRun.BloodShardGambleItems &= ~LogLevelValue;
			else
				Bot.Settings.TownRun.BloodShardGambleItems |= LogLevelValue;
		}



		private void AvoidanceAttemptMovementChecked(object sender, EventArgs e)
		{
			Bot.Settings.Avoidance.AttemptAvoidanceMovements = !Bot.Settings.Avoidance.AttemptAvoidanceMovements;
		}
		private void MovementTargetGlobeChecked(object sender, EventArgs e)
		{
			if (Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Globe))
				Bot.Settings.Combat.CombatMovementTargetTypes &= ~TargetType.Globe;
			else
				Bot.Settings.Combat.CombatMovementTargetTypes |= TargetType.Globe;
		}
		private void MovementTargetGoldChecked(object sender, EventArgs e)
		{
			if (Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Gold))
				Bot.Settings.Combat.CombatMovementTargetTypes &= ~TargetType.Gold;
			else
				Bot.Settings.Combat.CombatMovementTargetTypes |= TargetType.Gold;
		}

		private void MovementTargetItemChecked(object sender, EventArgs e)
		{
			if (Bot.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Item))
				Bot.Settings.Combat.CombatMovementTargetTypes &= ~TargetType.Item;
			else
				Bot.Settings.Combat.CombatMovementTargetTypes |= TargetType.Item;
		}
		private void AllowDefaultAttackAlwaysChecked(object sender, EventArgs e)
		{
			Bot.Settings.Combat.AllowDefaultAttackAlways = !Bot.Settings.Combat.AllowDefaultAttackAlways;
		}
		private void OutOfCombatMovementChecked(object sender, EventArgs e)
		{
			Bot.Settings.General.OutOfCombatMovement = !Bot.Settings.General.OutOfCombatMovement;
		}

		private void cb_ClusterTargetLogic_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Cluster.EnableClusteringTargetLogic = !Bot.Settings.Cluster.EnableClusteringTargetLogic;
			gb_ClusteringOptions.Enabled = Bot.Settings.Cluster.EnableClusteringTargetLogic;
		}



		private void tb_ClusterLogicDisableHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			Bot.Settings.Cluster.IgnoreClusterLowHPValue = Value;
			txt_ClusterLogicDisableHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}

		private void tb_ClusterLogicDistance_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = slider_sender.Value;
			Bot.Settings.Cluster.ClusterDistance = Value;
			txt_ClusterLogicDistance.Text = Value.ToString();
		}

		private void tb_ClusterLogicMinimumUnits_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = slider_sender.Value;
			Bot.Settings.Cluster.ClusterMinimumUnitCount = Value;
			txt_ClusterLogicMinimumUnits.Text = Value.ToString();
		}


		private void bWaitForArchonChecked(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bWaitForArchon = !Bot.Settings.Wizard.bWaitForArchon;
		}
		private void bKiteOnlyArchonChecked(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bKiteOnlyArchon = !Bot.Settings.Wizard.bKiteOnlyArchon;
		}
		private void bCancelArchonRebuffChecked(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bCancelArchonRebuff = !Bot.Settings.Wizard.bCancelArchonRebuff;
		}
		private void bTeleportFleeWhenLowHPChecked(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bTeleportFleeWhenLowHP = !Bot.Settings.Wizard.bTeleportFleeWhenLowHP;
		}
		private void bTeleportIntoGroupingChecked(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bTeleportIntoGrouping = !Bot.Settings.Wizard.bTeleportIntoGrouping;
		}
		private void bSelectiveWhirlwindChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bSelectiveWhirlwind = !Bot.Settings.Barbarian.bSelectiveWhirlwind;
		}
		private void bWaitForWrathChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bWaitForWrath = !Bot.Settings.Barbarian.bWaitForWrath;
		}
		private void bGoblinWrathChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bGoblinWrath = !Bot.Settings.Barbarian.bGoblinWrath;
		}
		private void bBarbUseWOTBAlwaysChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bBarbUseWOTBAlways = !Bot.Settings.Barbarian.bBarbUseWOTBAlways;
		}
		private void bFuryDumpWrathChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bFuryDumpWrath = !Bot.Settings.Barbarian.bFuryDumpWrath;
		}
		private void bFuryDumpAlwaysChecked(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bFuryDumpAlways = !Bot.Settings.Barbarian.bFuryDumpAlways;
		}
		private void bMonkMaintainSweepingWindChecked(object sender, EventArgs e)
		{
			Bot.Settings.Monk.bMonkMaintainSweepingWind = !Bot.Settings.Monk.bMonkMaintainSweepingWind;
		}
		private void bMonkSpamMantraChecked(object sender, EventArgs e)
		{
			Bot.Settings.Monk.bMonkSpamMantra = !Bot.Settings.Monk.bMonkSpamMantra;
		}
		private void iDHVaultMovementDelaySliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.DemonHunter.iDHVaultMovementDelay = Value;
			//TBiDHVaultMovementDelay.Text = Value.ToString();
		}
		private void DebugStatusBarChecked(object sender, EventArgs e)
		{
			Bot.Settings.Debug.DebugStatusBar = !Bot.Settings.Debug.DebugStatusBar;
		}
		private void SkipAheadChecked(object sender, EventArgs e)
		{
			Bot.Settings.Debug.SkipAhead = !Bot.Settings.Debug.SkipAhead;
		}

		private void GroupBotHealthSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			Bot.Settings.Grouping.GroupingMinimumBotHealth = Value;
			txt_GroupingMinBotHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}
		private void GroupMinimumUnitDistanceSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMinimumUnitDistance = Value;
			txt_GroupingMinDistance.Text = Value.ToString();
		}
		private void GroupMaxDistanceSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMaximumDistanceAllowed = Value;
			txt_GroupingMaxDistance.Text = Value.ToString();
		}
		private void GroupMinimumClusterCountSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMinimumClusterCount = Value;
			txt_GroupingMinCluster.Text = Value.ToString();
		}
		private void GroupMinimumUnitsInClusterSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMinimumUnitsInCluster = Value;
			txt_GroupingMinUnitsInCluster.Text = Value.ToString();
		}
		private void GroupingBehaviorChecked(object sender, EventArgs e)
		{
			Bot.Settings.Grouping.AttemptGroupingMovements = !Bot.Settings.Grouping.AttemptGroupingMovements;
			bool enabled = Bot.Settings.Grouping.AttemptGroupingMovements;
			//spGroupingOptions.IsEnabled = enabled;
		}

		private void trackbar_FleeMaxMonsterDistance_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Fleeing.FleeMaxMonsterDistance = Value;
			txt_FleeMaxMonsterDistance.Text = Value.ToString();
		}

		private void trackbar_FleeBotMinHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			Bot.Settings.Fleeing.FleeBotMinimumHealthPercent = Value;
			txt_FleeBotMinHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}

		private void cb_EnableFleeing_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Fleeing.EnableFleeingBehavior = !Bot.Settings.Fleeing.EnableFleeingBehavior;
			//groupBox_FleeOptions.Enabled = Bot.Settings.Fleeing.EnableFleeingBehavior;
			groupBox_FleeOptions.Enabled = Bot.Settings.Fleeing.EnableFleeingBehavior;
		}

		private void cb_FleeUnitElectrified_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Fleeing.FleeUnitElectrified = !Bot.Settings.Fleeing.FleeUnitElectrified;
		}

		private void cb_FleeUnitRareElite_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Fleeing.FleeUnitRareElite = !Bot.Settings.Fleeing.FleeUnitRareElite;
		}

		private void cb_FleeUnitNormal_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Fleeing.FleeUnitNormal = !Bot.Settings.Fleeing.FleeUnitNormal;
		}

		private void cb_FleeUnitAboveAverageHitPoints_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Fleeing.FleeUnitAboveAverageHitPoints = !Bot.Settings.Fleeing.FleeUnitAboveAverageHitPoints;
		}

		private void cb_FleeUnitFast_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Fleeing.FleeUnitIgnoreFast = !Bot.Settings.Fleeing.FleeUnitIgnoreFast;
		}

		private void cb_FleeUnitSucideBomber_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Fleeing.FleeUnitIgnoreSucideBomber = !Bot.Settings.Fleeing.FleeUnitIgnoreSucideBomber;
		}

		private void cb_FleeUnitRanged_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Fleeing.FleeUnitIgnoreRanged = !Bot.Settings.Fleeing.FleeUnitIgnoreRanged;
		}

		private void cb_BarbSelectiveWhirlwind_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bSelectiveWhirlwind = !Bot.Settings.Barbarian.bSelectiveWhirlwind;
		}

		private void cb_BarbWaitForWrath_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bWaitForWrath = !Bot.Settings.Barbarian.bWaitForWrath;
		}

		private void cb_BarbGoblinWrath_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bGoblinWrath = !Bot.Settings.Barbarian.bGoblinWrath;
		}

		private void cb_BarbFuryDumpWrath_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bFuryDumpWrath = !Bot.Settings.Barbarian.bFuryDumpWrath;
		}

		private void cb_BarbFuryDumpAlways_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bFuryDumpAlways = !Bot.Settings.Barbarian.bFuryDumpAlways;
		}

		private void cb_BarbUseWOTBAlways_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Barbarian.bBarbUseWOTBAlways = !Bot.Settings.Barbarian.bBarbUseWOTBAlways;
		}

		private void trackBar_DemonHunterValutDelay_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.DemonHunter.iDHVaultMovementDelay = Value;
			txt_DemonHunterVaultDelay.Text = Value.ToString();
		}

		private void cb_MonkSpamMantra_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Monk.bMonkSpamMantra = !Bot.Settings.Monk.bMonkSpamMantra;
		}

		private void cb_MonkMaintainSweepingWinds_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Monk.bMonkMaintainSweepingWind = !Bot.Settings.Monk.bMonkMaintainSweepingWind;
		}

		private void cb_WizardWaitForArchon_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bWaitForArchon = !Bot.Settings.Wizard.bWaitForArchon;
		}

		private void cb_WizardKiteOnlyArchon_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bKiteOnlyArchon = !Bot.Settings.Wizard.bKiteOnlyArchon;
		}

		private void cb_CacnelArchonRebuff_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bCancelArchonRebuff = !Bot.Settings.Wizard.bCancelArchonRebuff;
		}

		private void cb_WizardTeleportIntoGroups_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bTeleportIntoGrouping = !Bot.Settings.Wizard.bTeleportIntoGrouping;
		}

		private void cb_WizardTeleportFleeing_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Wizard.bTeleportFleeWhenLowHP = !Bot.Settings.Wizard.bTeleportFleeWhenLowHP;
		}

		private void cb_TargetingIgnoreRareElites_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Targeting.IgnoreAboveAverageMobs = !Bot.Settings.Targeting.IgnoreAboveAverageMobs;
		}

		private void cb_TargetingIgnoreCorpses_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Targeting.IgnoreCorpses = !Bot.Settings.Targeting.IgnoreCorpses;
		}

		private void cb_TargetingIncreaseRangeRareChests_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Targeting.UseExtendedRangeRepChest = !Bot.Settings.Targeting.UseExtendedRangeRepChest;
		}

		private void comboBox_TargetingGoblinPriority_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox slider_sender = (ComboBox)sender;
			int Value = (int)slider_sender.SelectedIndex;
			Bot.Settings.Targeting.GoblinPriority = Value;
		}

		private void cb_TargetingPrioritizeCloseRangeUnits_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Targeting.PrioritizeCloseRangeUnits = !Bot.Settings.Targeting.PrioritizeCloseRangeUnits;
			panel_TargetingPriorityCloseRangeMinUnits.Enabled = Bot.Settings.Targeting.PrioritizeCloseRangeUnits;

		}

		private void tb_TargetingPriorityCloseRangeUnitsCount_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Targeting.PrioritizeCloseRangeMinimumUnits = Value;
			txt_TargetingPriorityCloseRangeUnitsCount.Text = Value.ToString();
		}

		private void cb_TargetingUnitExceptionsRanged_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Targeting.UnitExceptionRangedUnits = !Bot.Settings.Targeting.UnitExceptionRangedUnits;
		}

		private void cb_TargetingUnitExceptionsSpawner_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Targeting.UnitExceptionSpawnerUnits = !Bot.Settings.Targeting.UnitExceptionSpawnerUnits;
		}

		private void cb_TargetingUnitExceptionsSucideBomber_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Targeting.UnitExceptionSucideBombers = !Bot.Settings.Targeting.UnitExceptionSucideBombers;
		}

		private void cb_TargetingUnitExceptionsLowHealth_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Targeting.UnitExceptionLowHP = !Bot.Settings.Targeting.UnitExceptionLowHP;
			panel_TargetingUnitExceptionsLowHealthMaxDistance.Enabled = Bot.Settings.Targeting.UnitExceptionLowHP;
		}

		private void tb_TargetingUnitExceptionLowHealthMaxDistance_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Targeting.UnitExceptionLowHPMaximumDistance = Value;
			txt_TargetingUnitExceptionLowHealthMaxDistance.Text = Value.ToString();
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			MessageBox.Show(Bot.Settings.Ranges.ContainerOpenRange.ToString());
		}

		private void cb_TargetRangeIgnoreKillRangeProfile_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Ranges.IgnoreCombatRange = !Bot.Settings.Ranges.IgnoreCombatRange;
		}

		private void cb_TargetRangeIgnoreProfileBlacklists_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Ranges.IgnoreProfileBlacklists = !Bot.Settings.Ranges.IgnoreProfileBlacklists;
		}

		private void cb_TargetRangeIgnoreLootProfileRange_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Ranges.IgnoreLootRange = !Bot.Settings.Ranges.IgnoreLootRange;
		}

		private void cb_TargetLOSGoblins_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.LOSMovement.AllowTreasureGoblin = !Bot.Settings.LOSMovement.AllowTreasureGoblin;
		}

		private void cb_TargetLOSRareElite_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.LOSMovement.AllowRareElites = !Bot.Settings.LOSMovement.AllowRareElites;
		}

		private void cb_TargetLOSBossUniques_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.LOSMovement.AllowUniqueBoss = !Bot.Settings.LOSMovement.AllowUniqueBoss;
		}

		private void cb_TargetLOSRanged_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.LOSMovement.AllowRanged = !Bot.Settings.LOSMovement.AllowRanged;
		}

		private void cb_TargetLOSSucideBombers_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.LOSMovement.AllowSucideBomber = !Bot.Settings.LOSMovement.AllowSucideBomber;
		}
		private void cb_TargetLOSEventSwitches_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.LOSMovement.AllowEventSwitches = !Bot.Settings.LOSMovement.AllowEventSwitches;
		}

		private void cb_TargetLOSSpawnerUnits_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.LOSMovement.AllowSpawnerUnits = !Bot.Settings.LOSMovement.AllowSpawnerUnits;
		}

		private void cb_TargetLOSRareChests_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.LOSMovement.AllowRareLootContainer = !Bot.Settings.LOSMovement.AllowRareLootContainer;
		}

		private void cb_TargetLOSCursedChestShrine_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.LOSMovement.AllowCursedChestShrines = !Bot.Settings.LOSMovement.AllowCursedChestShrines;
		}

		private void TargetingShrineCheckedChanged(object sender, EventArgs e)
		{
			CheckBox cbSender = (CheckBox)sender;
			int index = (int)Enum.Parse(typeof(ShrineTypes), cbSender.Text);
			Bot.Settings.Targeting.UseShrineTypes[index] = !(Bot.Settings.Targeting.UseShrineTypes[index]);
		}

		private void tb_GeneralGoldInactivityValue_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Plugin.GoldInactivityTimeoutSeconds = Value;
			txt_GeneralGoldInactivityValue.Text = Value.ToString();
		}

		private void cb_GeneralAllowBuffInTown_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.General.AllowBuffingInTown = !Bot.Settings.General.AllowBuffingInTown;
		}

		private void tb_GeneralEndOfCombatDelayValue_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.General.AfterCombatDelay = Value;
			txt_GeneralEndOfCombatDelayValue.Text = Value.ToString();
		}

		private void cb_GeneralApplyEndDelayToContainers_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.General.EnableWaitAfterContainers = !Bot.Settings.General.EnableWaitAfterContainers;
		}
		private void cb_GeneralSkipAhead_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Debug.SkipAhead = !Bot.Settings.Debug.SkipAhead;
		}

		private void cb_AdventureModeEnabled_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.AdventureMode.EnableAdventuringMode = !Bot.Settings.AdventureMode.EnableAdventuringMode;
		}

		private void cb_TownRunStashHoradricCache_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.TownRun.StashHoradricCache = !Bot.Settings.TownRun.StashHoradricCache;
		}

		private void comboBox_TownRunSalvageWhiteItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox slider_sender = (ComboBox)sender;
			Bot.Settings.TownRun.SalvageWhiteItemLevel = slider_sender.SelectedIndex == 0 ? 0 : slider_sender.SelectedIndex == 1 ? 1 : 61;
		}
		private void comboBox_TownRunSalvageMagicalItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox slider_sender = (ComboBox)sender;
			Bot.Settings.TownRun.SalvageMagicItemLevel = slider_sender.SelectedIndex == 0 ? 0 : slider_sender.SelectedIndex == 1 ? 1 : 61;
		}
		private void comboBox_TownRunSalvageRareItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox slider_sender = (ComboBox)sender;
			Bot.Settings.TownRun.SalvageRareItemLevel = slider_sender.SelectedIndex == 0 ? 0 : slider_sender.SelectedIndex == 1 ? 1 : 61;
		}
		private void comboBox_TownRunSalvageLegendaryItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox slider_sender = (ComboBox)sender;
			Bot.Settings.TownRun.SalvageLegendaryItemLevel = slider_sender.SelectedIndex == 0 ? 0 : slider_sender.SelectedIndex == 1 ? 1 : 61;
		}
		private void tb_TownRunBloodShardMinimumValue_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.TownRun.MinimumBloodShards = Value;
			txt_TownRunBloodShardMinimumValue.Text = Value.ToString();
		}

		private void cb_ItemRules_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.ItemRules.UseItemRules = !Bot.Settings.ItemRules.UseItemRules;
		}

		private void cb_ItemRulesPickup_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.ItemRules.UseItemRulesPickup = !Bot.Settings.ItemRules.UseItemRulesPickup;
		}



		//private void cb_ItemRulesUnidStashing_CheckedChanged(object sender, EventArgs e)
		//{
		//	Bot.Settings.ItemRules.ItemRulesUnidStashing = !Bot.Settings.ItemRules.ItemRulesUnidStashing;
		//}

		private void comboBox_ItemRulesType_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox slider_sender = (ComboBox)sender;
			Bot.Settings.ItemRules.ItemRuleType = slider_sender.Text;
		}

		private void cb_ItemRulesLogStashed_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox slider_sender = (ComboBox)sender;
			Bot.Settings.ItemRules.ItemRuleLogKeep = slider_sender.Text;
		}

		private void comboBox_ItemRulesLogPickup_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox slider_sender = (ComboBox)sender;
			Bot.Settings.ItemRules.ItemRuleLogPickup = slider_sender.Text;
		}

		private void cb_ItemRulesUseItemIDs_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.ItemRules.ItemRuleUseItemIDs = !Bot.Settings.ItemRules.ItemRuleUseItemIDs;
		}

		private void cb_ItemRulesDebugging_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.ItemRules.ItemRuleDebug = !Bot.Settings.ItemRules.ItemRuleDebug;
		}

		private void cb_LootPickupCraftPlans_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupCraftPlans = !Bot.Settings.Loot.PickupCraftPlans;
		}

		private void cb_LootGemAMETHYST_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupGems[2] = !Bot.Settings.Loot.PickupGems[2];
		}

		private void cb_LootGemDiamond_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupGemDiamond = !Bot.Settings.Loot.PickupGemDiamond;
		}

		private void cb_LootGemEMERALD_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupGems[1] = !Bot.Settings.Loot.PickupGems[1];
		}

		private void cb_LootGemRUBY_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupGems[0] = !Bot.Settings.Loot.PickupGems[0];
		}

		private void cb_LootGemTOPAZ_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupGems[3] = !Bot.Settings.Loot.PickupGems[3];
		}

		private void tb_LootMinimumGold_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Loot.MinimumGoldPile = Value;
			txt_LootMinimumGold.Text = Value.ToString();
		}

		private void tb_LootHealthPotions_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Loot.MaximumHealthPotions = Value;
			txt_LootHealthPotions.Text = Value.ToString();
		}

		private void cb_LootCraftMats_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupCraftMaterials = !Bot.Settings.Loot.PickupCraftMaterials;
		}

		private void cb_LootInfernoKeys_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupInfernalKeys = !Bot.Settings.Loot.PickupInfernalKeys;
		}
		private void cb_LootExpBooks_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Loot.ExpBooks = !Bot.Settings.Loot.ExpBooks;
		}
		private void cb_LootKeyStoneFragments_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupKeystoneFragments = !Bot.Settings.Loot.PickupKeystoneFragments;
		}
		private void GemQualityLevelChanged(object sender, EventArgs e)
		{
			ComboBox cbSender = (ComboBox)sender;

			Bot.Settings.Loot.MinimumGemItemLevel = (int)Enum.Parse(typeof(GemQuality), cbSender.Items[cbSender.SelectedIndex].ToString());
		}

		private void cb_DebugStatusBar_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Debug.DebugStatusBar = !Bot.Settings.Debug.DebugStatusBar;
		}

		private void cb_DebugDataLogging_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.Debug.DebuggingData = !Bot.Settings.Debug.DebuggingData;
		}

		private void cb_TownRunBloodShardGambling_CheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.TownRun.EnableBloodShardGambling = !Bot.Settings.TownRun.EnableBloodShardGambling;
		}
		private void cb_TownRunBuyPotionsCheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.TownRun.BuyPotionsDuringTownRun = !Bot.Settings.TownRun.BuyPotionsDuringTownRun;
		}
		private void cb_TownRunIDLegendariesCheckedChanged(object sender, EventArgs e)
		{
			Bot.Settings.TownRun.IdentifyLegendaries = !Bot.Settings.TownRun.IdentifyLegendaries;
		}
		private void SettingsForm_FormClosing_1(object sender, FormClosingEventArgs e)
		{
			Settings_Funky.SerializeToXML(Bot.Settings);
		}

		private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
		{

		}

		private void btn_DumpObstacleCache_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();


			try
			{
				LBDebug.Controls.Add(new UserControlDebugEntry(ObjectCache.Obstacles.DumpDebugInfo()));

				Logger.DBLog.InfoFormat("Dumping Obstacle Cache");

				var SortedValues = ObjectCache.Obstacles.Values.OrderBy(obj => obj.Obstacletype.Value).ThenBy(obj => obj.CentreDistance);
				foreach (var item in ObjectCache.Obstacles)
				{
					LBDebug.Controls.Add(new UserControlDebugEntry(item.Value.DebugString));
				}
			}
			catch
			{

				LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Modification Exception"));
			}
			LBDebug.Focus();

		}

		private void btn_DumpSNOCache_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();

			try
			{
				LBDebug.Controls.Add(new UserControlDebugEntry(ObjectCache.cacheSnoCollection.DumpDebugInfo()));
				var SortedValues = ObjectCache.cacheSnoCollection.Values.OrderBy(obj => obj.SNOID);
				Logger.DBLog.InfoFormat("Dumping SNO Cache");

				foreach (var item in ObjectCache.cacheSnoCollection)
				{
					LBDebug.Controls.Add(new UserControlDebugEntry(item.Value.DebugString));
				}
			}
			catch
			{

				LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Modification Exception"));
			}
			LBDebug.Focus();
		}

		private void btn_DumpCharacterCache_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();
			try
			{
				Logger.DBLog.InfoFormat("Dumping Character Cache");

				LBDebug.Controls.Add(new UserControlDebugEntry(Bot.Character.Data.DebugString()));

			}
			catch (Exception ex)
			{
				Logger.DBLog.InfoFormat("Safely Handled Exception {0}", ex.Message);
			}
			LBDebug.Focus();
		}

		private void btn_DumpTargetingCache_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();
			try
			{
				LBDebug.Controls.Add(new UserControlDebugEntry(Bot.Targeting.Cache.DebugString()));
			}
			catch (Exception ex)
			{
				Logger.DBLog.InfoFormat("Safely Handled Exception {0}", ex.Message);
			}
			LBDebug.Focus();
		}

		private void btn_DumpSkillsCache_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();
			try
			{
				if (Bot.Character.Class == null) return;
				LBDebug.Controls.Add(new UserControlDebugEntry(Bot.Character.Class.DebugString()));

				LBDebug.Controls.Add(new UserControlDebugEntry("==Current HotBar Abilities=="));
				foreach (var item in Bot.Character.Class.Abilities.Values)
				{
					try
					{
						LBDebug.Controls.Add(new UserControlDebugEntry(item.DebugString()));
					}
					catch (Exception ex)
					{
						Logger.DBLog.InfoFormat("Safely Handled Exception {0}", ex.Message);
					}
				}

				LBDebug.Controls.Add(new UserControlDebugEntry("==Cached HotBar Abilities=="));
				foreach (var item in Bot.Character.Class.HotBar.CachedPowers)
				{
					try
					{
						LBDebug.Controls.Add(new UserControlDebugEntry(item.ToString()));
					}
					catch (Exception ex)
					{
						Logger.DBLog.InfoFormat("Safely Handled Exception {0}", ex.Message);
					}
				}

				LBDebug.Controls.Add(new UserControlDebugEntry("==Buffs=="));
				foreach (var item in Bot.Character.Class.HotBar.CurrentBuffs.Keys)
				{

					string Power = Enum.GetName(typeof(SNOPower), item);
					try
					{
						LBDebug.Controls.Add(new UserControlDebugEntry(Power));
					}
					catch (Exception ex)
					{
						Logger.DBLog.InfoFormat("Safely Handled Exception {0}", ex.Message);
					}
				}

			}
			catch (Exception ex)
			{
				Logger.DBLog.InfoFormat("Safely Handled Exception {0}", ex.Message);
			}
			LBDebug.Focus();
		}


		private void btn_DumpBountyCache_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();
			try
			{
				LBDebug.Controls.Add(new UserControlDebugEntry(Bot.Game.Bounty.DebugString()));
			}
			catch (Exception ex)
			{
				Logger.DBLog.InfoFormat("Safely Handled Exception {0}", ex.Message);
			}
			LBDebug.Focus();
		}

		private void btn_DumpObjectCache_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();
			try
			{

				string OutPut = ObjectCache.Objects.DumpDebugInfo();
				LBDebug.Controls.Add(new UserControlDebugEntry(OutPut));

				var SortedValues = ObjectCache.Objects.Values.OrderBy(obj => obj.targetType.Value).ThenBy(obj => obj.CentreDistance);
				foreach (var item in SortedValues)
				{
					string objDebugStr = item.DebugString;
					Color foreColor = (item is CacheItem) ? Color.Black : Color.GhostWhite;
					Color backColor = (item is CacheDestructable) ? Color.DarkSlateGray
								: (item is CacheUnit) ? Color.MediumSeaGreen
								: (item is CacheItem) ? Color.Gold
								: (item is CacheInteractable) ? Color.DimGray
								: Color.Gray;

					UserControlDebugEntry entry = new UserControlDebugEntry(objDebugStr, foreColor, backColor);
					LBDebug.Controls.Add(entry);

				}

			}
			catch
			{
				LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Modification Exception"));
			}
			LBDebug.Focus();

		}

		private void btn_Test_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();

			Game.UI.GameMenu.SelectHeroByIndex(0).Click();
			try
			{
				UIElement uie = Game.UI.GameMenu.HeroSelectListStackPanel;
				if (Game.UI.ValidateUIElement(uie))
				{
					LBDebug.Controls.Add(new UserControlDebugEntry(Game.UI.UIElementString(uie)));
					foreach (var u in Game.UI.GetChildren(uie))
					{
						LBDebug.Controls.Add(new UserControlDebugEntry(Game.UI.UIElementString(u)));
						foreach (var ui in Game.UI.GetChildren(u))
						{
							LBDebug.Controls.Add(new UserControlDebugEntry(Game.UI.UIElementString(ui)));
							foreach (var UI in Game.UI.GetChildren(ui))
							{
								LBDebug.Controls.Add(new UserControlDebugEntry(Game.UI.UIElementString(UI)));
							}
						}
					}
				}
				
				
			}
			catch (Exception ex)
			{
				LBDebug.Controls.Add(new UserControlDebugEntry(ex.Message));
			}
			//ZetaDia.Service.GameAccount.SwitchHero(1);
			LBDebug.Focus();
		}

		private void flowLayoutPanel_Avoidances_Click(object sender, EventArgs e)
		{
			flowLayoutPanel_Avoidances.Focus();
		}

		private void flowLayoutPanel_Avoidances_MouseEnter(object sender, EventArgs e)
		{
			flowLayoutPanel_Avoidances.Focus();
		}

		private void LBDebug_MouseClick(object sender, MouseEventArgs e)
		{
			LBDebug.Focus();
		}

		private void LBDebug_MouseEnter(object sender, EventArgs e)
		{
			LBDebug.Focus();
		}

		private void btn_DumpInteractiveCache_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();

			try
			{
				foreach (var cacheObject in Bot.Game.Profile.InteractableObjectCache)
				{
					LBDebug.Controls.Add(new UserControlDebugEntry(cacheObject.Value.DebugString));
				}
			}
			catch
			{
				LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Exception"));
			}

			LBDebug.Focus();
		}
	}
}
