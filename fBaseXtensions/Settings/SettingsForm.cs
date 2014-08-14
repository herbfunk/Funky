using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using fBaseXtensions.Cache;
using fBaseXtensions.Cache.External.Objects;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Avoidance;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using fBaseXtensions.Stats;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Settings
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();

			try
			{

				tb_GlobeHealth.Value = (int)(FunkyBaseExtension.Settings.Combat.GlobeHealthPercent * 100);
				tb_GlobeHealth.ValueChanged += tb_GlobeHealth_ValueChanged;

				tb_PotionHealth.Value = (int)(FunkyBaseExtension.Settings.Combat.PotionHealthPercent * 100);
				tb_PotionHealth.ValueChanged += tb_PotionHealth_ValueChanged;

				tb_WellHealth.Value = (int)(FunkyBaseExtension.Settings.Combat.HealthWellHealthPercent * 100);
				tb_WellHealth.ValueChanged += tb_WellHealth_ValueChanged;

				txt_GlobeHealth.Text = FunkyBaseExtension.Settings.Combat.GlobeHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
				txt_WellHealth.Text = FunkyBaseExtension.Settings.Combat.HealthWellHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
				txt_PotionHealth.Text = FunkyBaseExtension.Settings.Combat.PotionHealthPercent.ToString("F2", CultureInfo.InvariantCulture);

				cb_CombatMovementGlobes.Checked = FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Globe);
				cb_CombatMovementGlobes.CheckedChanged += MovementTargetGlobeChecked;

				cb_CombatMovementGold.Checked = FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Gold);
				cb_CombatMovementGold.CheckedChanged += MovementTargetGoldChecked;

				cb_CombatMovementItems.Checked = FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Item);
				cb_CombatMovementItems.CheckedChanged += MovementTargetItemChecked;

				cb_CombatAllowDefaultAttack.Checked = FunkyBaseExtension.Settings.Combat.AllowDefaultAttackAlways;
				cb_CombatAllowDefaultAttack.CheckedChanged += AllowDefaultAttackAlwaysChecked;
				
				cb_MovementOutOfCombatSkills.Checked = FunkyBaseExtension.Settings.General.OutOfCombatMovement;
				cb_MovementOutOfCombatSkills.CheckedChanged += OutOfCombatMovementChecked;

				cb_ClusterTargetLogic.Checked = FunkyBaseExtension.Settings.Cluster.EnableClusteringTargetLogic;
				cb_ClusterTargetLogic.CheckedChanged += cb_ClusterTargetLogic_CheckedChanged;
				gb_ClusteringOptions.Enabled = FunkyBaseExtension.Settings.Cluster.EnableClusteringTargetLogic;

				txt_ClusterLogicDisableHealth.Text = FunkyBaseExtension.Settings.Cluster.IgnoreClusterLowHPValue.ToString("F2", CultureInfo.InvariantCulture);
				txt_ClusterLogicDistance.Text = FunkyBaseExtension.Settings.Cluster.ClusterDistance.ToString("F2", CultureInfo.InvariantCulture);
				txt_ClusterLogicMinimumUnits.Text = FunkyBaseExtension.Settings.Cluster.ClusterMinimumUnitCount.ToString("F2", CultureInfo.InvariantCulture);

				tb_ClusterLogicDisableHealth.Value = (int)(FunkyBaseExtension.Settings.Cluster.IgnoreClusterLowHPValue * 100);
				tb_ClusterLogicDisableHealth.ValueChanged += tb_ClusterLogicDisableHealth_ValueChanged;

				tb_ClusterLogicDistance.Value = (int)(FunkyBaseExtension.Settings.Cluster.ClusterDistance);
				tb_ClusterLogicDistance.ValueChanged += tb_ClusterLogicDistance_ValueChanged;

				tb_ClusterLogicMinimumUnits.Value = (int)(FunkyBaseExtension.Settings.Cluster.ClusterMinimumUnitCount);
				tb_ClusterLogicMinimumUnits.ValueChanged += tb_ClusterLogicMinimumUnits_ValueChanged;

				cb_GroupingLogic.Checked = FunkyBaseExtension.Settings.Grouping.AttemptGroupingMovements;
				cb_GroupingLogic.CheckedChanged += GroupingBehaviorChecked;

				txt_GroupingMaxDistance.Text = FunkyBaseExtension.Settings.Grouping.GroupingMaximumDistanceAllowed.ToString();
				txt_GroupingMinBotHealth.Text = FunkyBaseExtension.Settings.Grouping.GroupingMinimumBotHealth.ToString("F2", CultureInfo.InvariantCulture);
				txt_GroupingMinCluster.Text = FunkyBaseExtension.Settings.Grouping.GroupingMinimumClusterCount.ToString();
				txt_GroupingMinDistance.Text = FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitDistance.ToString();
				txt_GroupingMinUnitsInCluster.Text = FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitsInCluster.ToString();

				tb_GroupingMaxDistance.Value = FunkyBaseExtension.Settings.Grouping.GroupingMaximumDistanceAllowed;
				tb_GroupingMaxDistance.ValueChanged += GroupMaxDistanceSliderChanged;

				tb_GroupingMinBotHealth.Value = (int)(FunkyBaseExtension.Settings.Grouping.GroupingMinimumBotHealth * 100);
				tb_GroupingMinBotHealth.ValueChanged += GroupBotHealthSliderChanged;

				tb_GroupingMinCluster.Value = FunkyBaseExtension.Settings.Grouping.GroupingMinimumClusterCount;
				tb_GroupingMinCluster.ValueChanged += GroupMinimumClusterCountSliderChanged;

				tb_GroupingMinDistance.Value = FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitDistance;
				tb_GroupingMinDistance.ValueChanged += GroupMinimumUnitDistanceSliderChanged;

				tb_GroupingMinUnitsInCluster.Value = FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitsInCluster;
				tb_GroupingMinUnitsInCluster.ValueChanged += GroupMinimumUnitsInClusterSliderChanged;

				cb_AvoidanceLogic.Checked = FunkyBaseExtension.Settings.Avoidance.AttemptAvoidanceMovements;
				cb_AvoidanceLogic.CheckedChanged += AvoidanceAttemptMovementChecked;

				AvoidanceValue[] avoidanceValues = FunkyBaseExtension.Settings.Avoidance.Avoidances.ToArray();
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

				cb_EnableFleeing.Checked = FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior;
				cb_EnableFleeing.CheckedChanged += cb_EnableFleeing_CheckedChanged;

				trackbar_FleeMaxMonsterDistance.Value = FunkyBaseExtension.Settings.Fleeing.FleeMaxMonsterDistance;
				trackbar_FleeMaxMonsterDistance.ValueChanged += trackbar_FleeMaxMonsterDistance_ValueChanged;

				txt_FleeMaxMonsterDistance.Text = FunkyBaseExtension.Settings.Fleeing.FleeMaxMonsterDistance.ToString();

				trackbar_FleeBotMinHealth.Value = (int)(FunkyBaseExtension.Settings.Fleeing.FleeBotMinimumHealthPercent * 100);
				trackbar_FleeBotMinHealth.ValueChanged += trackbar_FleeBotMinHealth_ValueChanged;

				txt_FleeBotMinHealth.Text = FunkyBaseExtension.Settings.Fleeing.FleeBotMinimumHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
				groupBox_FleeOptions.Enabled = FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior;


				cb_FleeUnitElectrified.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitElectrified;
				cb_FleeUnitElectrified.CheckedChanged += cb_FleeUnitElectrified_CheckedChanged;

				cb_FleeUnitFast.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreFast;
				cb_FleeUnitFast.CheckedChanged += cb_FleeUnitFast_CheckedChanged;

				cb_FleeUnitNormal.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitNormal;
				cb_FleeUnitNormal.CheckedChanged += cb_FleeUnitNormal_CheckedChanged;

				cb_FleeUnitRanged.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreRanged;
				cb_FleeUnitRanged.CheckedChanged += cb_FleeUnitRanged_CheckedChanged;

				cb_FleeUnitRareElite.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitRareElite;
				cb_FleeUnitRareElite.CheckedChanged += cb_FleeUnitRareElite_CheckedChanged;

				cb_FleeUnitSucideBomber.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreSucideBomber;
				cb_FleeUnitSucideBomber.CheckedChanged += cb_FleeUnitSucideBomber_CheckedChanged;

				cb_FleeUnitAboveAverageHitPoints.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitAboveAverageHitPoints;
				cb_FleeUnitAboveAverageHitPoints.CheckedChanged += cb_FleeUnitAboveAverageHitPoints_CheckedChanged;


				List<TabPage> RemovalTabPages = new List<TabPage> { tabPage_DemonHunter, tabPage_Barbarian, tabPage_Wizard, tabPage_WitchDoctor, tabPage_Monk, tabPage_Crusader };
				switch (FunkyGame.CurrentActorClass)
				{
					case ActorClass.DemonHunter:
						trackBar_DemonHunterValutDelay.Value = FunkyBaseExtension.Settings.DemonHunter.iDHVaultMovementDelay;
						trackBar_DemonHunterValutDelay.ValueChanged += trackBar_DemonHunterValutDelay_ValueChanged;
						txt_DemonHunterVaultDelay.Text = FunkyBaseExtension.Settings.DemonHunter.iDHVaultMovementDelay.ToString();
						RemovalTabPages.Remove(tabPage_DemonHunter);
						break;
					case ActorClass.Barbarian:
						cb_BarbFuryDumpAlways.Checked = FunkyBaseExtension.Settings.Barbarian.bBarbUseWOTBAlways;
						cb_BarbFuryDumpAlways.CheckedChanged += cb_BarbFuryDumpAlways_CheckedChanged;

						cb_BarbFuryDumpWrath.Checked = FunkyBaseExtension.Settings.Barbarian.bFuryDumpWrath;
						cb_BarbFuryDumpWrath.CheckedChanged += cb_BarbFuryDumpWrath_CheckedChanged;

						cb_BarbGoblinWrath.Checked = FunkyBaseExtension.Settings.Barbarian.bGoblinWrath;
						cb_BarbGoblinWrath.CheckedChanged += cb_BarbGoblinWrath_CheckedChanged;

						cb_BarbUseWOTBAlways.Checked = FunkyBaseExtension.Settings.Barbarian.bBarbUseWOTBAlways;
						cb_BarbUseWOTBAlways.CheckedChanged += cb_BarbUseWOTBAlways_CheckedChanged;

						cb_BarbWaitForWrath.Checked = FunkyBaseExtension.Settings.Barbarian.bWaitForWrath;
						cb_BarbWaitForWrath.CheckedChanged += cb_BarbWaitForWrath_CheckedChanged;

						cb_BarbSelectiveWhirldWind.Checked = FunkyBaseExtension.Settings.Barbarian.bSelectiveWhirlwind;
						cb_BarbSelectiveWhirldWind.CheckedChanged += cb_BarbSelectiveWhirlwind_CheckedChanged;

						RemovalTabPages.Remove(tabPage_Barbarian);
						break;
					case ActorClass.Wizard:
						cb_WizardKiteOnlyArchon.Checked = FunkyBaseExtension.Settings.Wizard.bKiteOnlyArchon;
						cb_WizardKiteOnlyArchon.CheckedChanged += cb_WizardKiteOnlyArchon_CheckedChanged;

						cb_WizardTeleportFleeing.Checked = FunkyBaseExtension.Settings.Wizard.bTeleportFleeWhenLowHP;
						cb_WizardTeleportFleeing.CheckedChanged += cb_WizardTeleportFleeing_CheckedChanged;

						cb_WizardTeleportIntoGroups.Checked = FunkyBaseExtension.Settings.Wizard.bTeleportIntoGrouping;
						cb_WizardTeleportIntoGroups.CheckedChanged += cb_WizardTeleportIntoGroups_CheckedChanged;

						cb_WizardWaitForArchon.Checked = FunkyBaseExtension.Settings.Wizard.bWaitForArchon;
						cb_WizardWaitForArchon.CheckedChanged += cb_WizardWaitForArchon_CheckedChanged;

						cb_WizardCancelArchonRebuff.Checked = FunkyBaseExtension.Settings.Wizard.bCancelArchonRebuff;
						cb_WizardCancelArchonRebuff.CheckedChanged += cb_CacnelArchonRebuff_CheckedChanged;

						RemovalTabPages.Remove(tabPage_Wizard);
						break;
					case ActorClass.Witchdoctor:
						RemovalTabPages.Remove(tabPage_WitchDoctor);
						break;
					case ActorClass.Monk:
						cb_MonkMaintainSweepingWinds.Checked = FunkyBaseExtension.Settings.Monk.bMonkMaintainSweepingWind;
						cb_MonkMaintainSweepingWinds.CheckedChanged += bMonkMaintainSweepingWindChecked;

						cb_MonkSpamMantra.Checked = FunkyBaseExtension.Settings.Monk.bMonkSpamMantra;
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



				cb_TargetingIgnoreCorpses.Checked = FunkyBaseExtension.Settings.Targeting.IgnoreCorpses;
				cb_TargetingIgnoreCorpses.CheckedChanged += cb_TargetingIgnoreCorpses_CheckedChanged;

				cb_TargetingIgnoreRareElites.Checked = FunkyBaseExtension.Settings.Targeting.IgnoreAboveAverageMobs;
				cb_TargetingIgnoreRareElites.CheckedChanged += cb_TargetingIgnoreRareElites_CheckedChanged;

				cb_TargetingIncreaseRangeRareChests.Checked = FunkyBaseExtension.Settings.Targeting.UseExtendedRangeRepChest;
				cb_TargetingIncreaseRangeRareChests.CheckedChanged += cb_TargetingIncreaseRangeRareChests_CheckedChanged;

				comboBox_TargetingGoblinPriority.SelectedIndex = FunkyBaseExtension.Settings.Targeting.GoblinPriority;
				comboBox_TargetingGoblinPriority.SelectedIndexChanged += comboBox_TargetingGoblinPriority_SelectedIndexChanged;

				panel_TargetingPriorityCloseRangeMinUnits.Enabled = FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeUnits;
				tb_TargetingPriorityCloseRangeUnitsCount.Value = FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeMinimumUnits;
				tb_TargetingPriorityCloseRangeUnitsCount.ValueChanged += tb_TargetingPriorityCloseRangeUnitsCount_ValueChanged;

				txt_TargetingPriorityCloseRangeUnitsCount.Text = FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeMinimumUnits.ToString();

				cb_TargetingPrioritizeCloseRangeUnits.Checked = FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeUnits;
				cb_TargetingPrioritizeCloseRangeUnits.CheckedChanged += cb_TargetingPrioritizeCloseRangeUnits_CheckedChanged;

				cb_TargetingUnitExceptionsRanged.Checked = FunkyBaseExtension.Settings.Targeting.UnitExceptionRangedUnits;
				cb_TargetingUnitExceptionsRanged.CheckedChanged += cb_TargetingUnitExceptionsRanged_CheckedChanged;

				cb_TargetingUnitExceptionsSpawner.Checked = FunkyBaseExtension.Settings.Targeting.UnitExceptionSpawnerUnits;
				cb_TargetingUnitExceptionsSpawner.CheckedChanged += cb_TargetingUnitExceptionsSpawner_CheckedChanged;

				cb_TargetingUnitExceptionsSucideBomber.Checked = FunkyBaseExtension.Settings.Targeting.UnitExceptionSucideBombers;
				cb_TargetingUnitExceptionsSucideBomber.CheckedChanged += cb_TargetingUnitExceptionsSucideBomber_CheckedChanged;

				cb_TargetingUnitExceptionsLowHealth.Checked = FunkyBaseExtension.Settings.Targeting.UnitExceptionLowHP;
				cb_TargetingUnitExceptionsLowHealth.CheckedChanged += cb_TargetingUnitExceptionsLowHealth_CheckedChanged;

				panel_TargetingUnitExceptionsLowHealthMaxDistance.Enabled = FunkyBaseExtension.Settings.Targeting.UnitExceptionLowHP;
				tb_TargetingUnitExceptionLowHealthMaxDistance.Value = FunkyBaseExtension.Settings.Targeting.UnitExceptionLowHPMaximumDistance;
				tb_TargetingUnitExceptionLowHealthMaxDistance.ValueChanged += tb_TargetingUnitExceptionLowHealthMaxDistance_ValueChanged;

				txt_TargetingUnitExceptionLowHealthMaxDistance.Text = FunkyBaseExtension.Settings.Targeting.UnitExceptionLowHPMaximumDistance.ToString();



				UserControlTargetRange eliteRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.EliteCombatRange, "Rare/Elite/Unique Unit Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.EliteCombatRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(eliteRange);

				UserControlTargetRange normalRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.NonEliteCombatRange, "Normal Unit Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.NonEliteCombatRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(normalRange);

				UserControlTargetRange shrineRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.ShrineRange, "Shrine Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.ShrineRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(shrineRange);

				UserControlTargetRange CursedshrineRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.CursedShrineRange, "Cursed Shrine Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.CursedShrineRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(CursedshrineRange);

				UserControlTargetRange GoldRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.GoldRange, "Gold Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.GoldRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(GoldRange);

				UserControlTargetRange GlobeRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.GlobeRange, "Globe Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.GlobeRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(GlobeRange);


				UserControlTargetRange containerRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.ContainerOpenRange, "Container Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.ContainerOpenRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(containerRange);

				UserControlTargetRange CursedChestRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.CursedChestRange, "Cursed Chest Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.CursedChestRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(CursedChestRange);

				UserControlTargetRange DoorRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.NonEliteCombatRange, "Door Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.DoorRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(DoorRange);

				UserControlTargetRange destructibleRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.DestructibleRange, "Destructible Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.DestructibleRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(destructibleRange);

				UserControlTargetRange PoolsofReflectionRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.PoolsOfReflectionRange, "Pools of Reflection Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.PoolsOfReflectionRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(PoolsofReflectionRange);

				UserControlTargetRange itemRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.ItemRange, "Item Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.ItemRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(itemRange);

				UserControlTargetRange PotionRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.PotionRange, "Potion Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.PotionRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(PotionRange);

				UserControlTargetRange GoblinRange = new UserControlTargetRange(FunkyBaseExtension.Settings.Ranges.TreasureGoblinRange, "Goblin Range")
				{
					UpdateValue = i => { FunkyBaseExtension.Settings.Ranges.TreasureGoblinRange = i; }
				};
				flowLayout_TargetRanges.Controls.Add(GoblinRange);


				cb_TargetRangeIgnoreKillRangeProfile.Checked = FunkyBaseExtension.Settings.Ranges.IgnoreCombatRange;
				cb_TargetRangeIgnoreKillRangeProfile.CheckedChanged += cb_TargetRangeIgnoreKillRangeProfile_CheckedChanged;

				cb_TargetRangeIgnoreLootProfileRange.Checked = FunkyBaseExtension.Settings.Ranges.IgnoreLootRange;
				cb_TargetRangeIgnoreLootProfileRange.CheckedChanged += cb_TargetRangeIgnoreLootProfileRange_CheckedChanged;

				cb_TargetRangeIgnoreProfileBlacklists.Checked = FunkyBaseExtension.Settings.Ranges.IgnoreProfileBlacklists;
				cb_TargetRangeIgnoreProfileBlacklists.CheckedChanged += cb_TargetRangeIgnoreProfileBlacklists_CheckedChanged;

				cb_TargetLOSBossUniques.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowUniqueBoss;
				cb_TargetLOSBossUniques.CheckedChanged += cb_TargetLOSBossUniques_CheckedChanged;

				cb_TargetLOSCursedChestShrine.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowCursedChestShrines;
				cb_TargetLOSCursedChestShrine.CheckedChanged += cb_TargetLOSCursedChestShrine_CheckedChanged;

				cb_TargetLOSGoblins.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowTreasureGoblin;
				cb_TargetLOSGoblins.CheckedChanged += cb_TargetLOSGoblins_CheckedChanged;

				cb_TargetLOSRanged.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowRanged;
				cb_TargetLOSRanged.CheckedChanged += cb_TargetLOSRanged_CheckedChanged;

				cb_TargetLOSRareChests.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowRareLootContainer;
				cb_TargetLOSRareChests.CheckedChanged += cb_TargetLOSRareChests_CheckedChanged;

				cb_TargetLOSRareElite.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowRareElites;
				cb_TargetLOSRareElite.CheckedChanged += cb_TargetLOSRareElite_CheckedChanged;

				cb_TargetLOSSpawnerUnits.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowSpawnerUnits;
				cb_TargetLOSSpawnerUnits.CheckedChanged += cb_TargetLOSSpawnerUnits_CheckedChanged;

				cb_TargetLOSSucideBombers.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowSucideBomber;
				cb_TargetLOSSucideBombers.CheckedChanged += cb_TargetLOSSucideBombers_CheckedChanged;

				cb_LOSEventSwitches.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowEventSwitches;
				cb_LOSEventSwitches.CheckedChanged += cb_TargetLOSEventSwitches_CheckedChanged;

				cb_TargetingShrineEmpowered.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[5];
				cb_TargetingShrineEmpowered.CheckedChanged += TargetingShrineCheckedChanged;

				cb_TargetingShrineEnlightnment.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[1];
				cb_TargetingShrineEnlightnment.CheckedChanged += TargetingShrineCheckedChanged;

				cb_TargetingShrineFleeting.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[0];
				cb_TargetingShrineFleeting.CheckedChanged += TargetingShrineCheckedChanged;

				cb_TargetingShrineFortune.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[3];
				cb_TargetingShrineFortune.CheckedChanged += TargetingShrineCheckedChanged;

				cb_TargetingShrineFrenzy.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[2];
				cb_TargetingShrineFrenzy.CheckedChanged += TargetingShrineCheckedChanged;

				cb_TargetingShrineProtection.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[4];
				cb_TargetingShrineProtection.CheckedChanged += TargetingShrineCheckedChanged;

				txt_GoldInactivityTimeout.Text = FunkyBaseExtension.Settings.Monitoring.GoldInactivityTimeoutSeconds.ToString();
				tb_GoldInactivityTimeout.Value = FunkyBaseExtension.Settings.Monitoring.GoldInactivityTimeoutSeconds;
				tb_GoldInactivityTimeout.ValueChanged += tb_GoldInactivityTimeout_ValueChanged;

				cb_GeneralAllowBuffInTown.Checked = FunkyBaseExtension.Settings.General.AllowBuffingInTown;
				cb_GeneralAllowBuffInTown.CheckedChanged += cb_GeneralAllowBuffInTown_CheckedChanged;

				txt_GeneralEndOfCombatDelayValue.Text = FunkyBaseExtension.Settings.General.AfterCombatDelay.ToString();

				tb_GeneralEndOfCombatDelayValue.Value = FunkyBaseExtension.Settings.General.AfterCombatDelay;
				tb_GeneralEndOfCombatDelayValue.ValueChanged += tb_GeneralEndOfCombatDelayValue_ValueChanged;

				cb_GeneralApplyEndDelayToContainers.Checked = FunkyBaseExtension.Settings.General.EnableWaitAfterContainers;
				cb_GeneralApplyEndDelayToContainers.CheckedChanged += cb_GeneralApplyEndDelayToContainers_CheckedChanged;

				cb_GeneralSkipAhead.Checked=FunkyBaseExtension.Settings.Debugging.SkipAhead;
				cb_GeneralSkipAhead.CheckedChanged += cb_GeneralSkipAhead_CheckedChanged;

				//cb_DeathWaitForPotion.Checked = FunkyBaseExtension.Settings.Death.WaitForPotionCooldown;
				//cb_DeathWaitForPotion.CheckedChanged += cb_DeathPotion_CheckedChanged;
				
				//cb_DeathWaitForSkillsCooldown.Checked = FunkyBaseExtension.Settings.Death.WaitForAllSkillsCooldown;
				//cb_DeathWaitForSkillsCooldown.CheckedChanged += cb_DeathSkills_CheckedChanged;
				

				cb_AdventureModeEnabled.Checked = FunkyBaseExtension.Settings.AdventureMode.EnableAdventuringMode;
				cb_AdventureModeEnabled.CheckedChanged += cb_AdventureModeEnabled_CheckedChanged;

				comboBox_LootLegendaryItemQuality.SelectedIndex = FunkyBaseExtension.Settings.Loot.PickupLegendaryItems == 0 ? 0 : FunkyBaseExtension.Settings.Loot.PickupLegendaryItems == 61 ? 1 : 2;
				comboBox_LootLegendaryItemQuality.SelectedIndexChanged += ItemLootChanged;

				comboBox_LootMagicItemQuality.SelectedIndex = FunkyBaseExtension.Settings.Loot.PickupMagicItems == 0 ? 0 : FunkyBaseExtension.Settings.Loot.PickupMagicItems == 61 ? 1 : 2;
				comboBox_LootMagicItemQuality.SelectedIndexChanged += ItemLootChanged;

				comboBox_LootRareItemQuality.SelectedIndex = FunkyBaseExtension.Settings.Loot.PickupRareItems == 0 ? 0 : FunkyBaseExtension.Settings.Loot.PickupRareItems == 61 ? 1 : 2;
				comboBox_LootRareItemQuality.SelectedIndexChanged += ItemLootChanged;

				comboBox_LootWhiteItemQuality.SelectedIndex = FunkyBaseExtension.Settings.Loot.PickupWhiteItems == 0 ? 0 : FunkyBaseExtension.Settings.Loot.PickupWhiteItems == 61 ? 1 : 2;
				comboBox_LootWhiteItemQuality.SelectedIndexChanged += ItemLootChanged;

				cb_LootPickupCraftPlans.Checked = FunkyBaseExtension.Settings.Loot.PickupCraftPlans;
				cb_LootPickupCraftPlans.CheckedChanged += cb_LootPickupCraftPlans_CheckedChanged;

				comboBox_LootGemQuality.Text = Enum.GetName(typeof(GemQualityTypes), FunkyBaseExtension.Settings.Loot.MinimumGemItemLevel).ToString();
				comboBox_LootGemQuality.SelectedIndexChanged += GemQualityLevelChanged;

				cb_LootGemAMETHYST.Checked = FunkyBaseExtension.Settings.Loot.PickupGems[2];
				cb_LootGemAMETHYST.CheckedChanged += cb_LootGemAMETHYST_CheckedChanged;

				cb_LootGemDiamond.Checked = FunkyBaseExtension.Settings.Loot.PickupGemDiamond;
				cb_LootGemDiamond.CheckedChanged += cb_LootGemDiamond_CheckedChanged;

				cb_LootGemEMERALD.Checked = FunkyBaseExtension.Settings.Loot.PickupGems[1];
				cb_LootGemEMERALD.CheckedChanged += cb_LootGemEMERALD_CheckedChanged;

				cb_LootGemRUBY.Checked = FunkyBaseExtension.Settings.Loot.PickupGems[0];
				cb_LootGemRUBY.CheckedChanged += cb_LootGemRUBY_CheckedChanged;

				cb_LootGemTOPAZ.Checked = FunkyBaseExtension.Settings.Loot.PickupGems[3];
				cb_LootGemTOPAZ.CheckedChanged += cb_LootGemTOPAZ_CheckedChanged;

				txt_LootMinimumGold.Text = FunkyBaseExtension.Settings.Loot.MinimumGoldPile.ToString();
				tb_LootMinimumGold.Value = FunkyBaseExtension.Settings.Loot.MinimumGoldPile;
				tb_LootMinimumGold.ValueChanged += tb_LootMinimumGold_ValueChanged;

				txt_LootHealthPotions.Text = FunkyBaseExtension.Settings.Loot.MaximumHealthPotions.ToString();
				tb_LootHealthPotions.Value = FunkyBaseExtension.Settings.Loot.MaximumHealthPotions;
				tb_LootHealthPotions.ValueChanged += tb_LootHealthPotions_ValueChanged;

				cb_LootCraftMats.Checked = FunkyBaseExtension.Settings.Loot.PickupCraftMaterials;
				cb_LootCraftMats.CheckedChanged += cb_LootCraftMats_CheckedChanged;

				cb_LootInfernoKeys.Checked = FunkyBaseExtension.Settings.Loot.PickupInfernalKeys;
				cb_LootInfernoKeys.CheckedChanged += cb_LootInfernoKeys_CheckedChanged;

				cb_LootExpBooks.Checked = FunkyBaseExtension.Settings.Loot.ExpBooks;
				cb_LootExpBooks.CheckedChanged += cb_LootExpBooks_CheckedChanged;

				cb_LootKeyStones.Checked = FunkyBaseExtension.Settings.Loot.PickupKeystoneFragments;
				cb_LootKeyStones.CheckedChanged += cb_LootKeyStoneFragments_CheckedChanged;


				cb_DebugDataLogging.Checked = FunkyBaseExtension.Settings.Debugging.DebuggingData;
				cb_DebugDataLogging.CheckedChanged += cb_DebugDataLogging_CheckedChanged;

				cb_DebugStatusBar.Checked = FunkyBaseExtension.Settings.Debugging.DebugStatusBar;
				cb_DebugStatusBar.CheckedChanged += cb_DebugStatusBar_CheckedChanged;

				var LogLevels = Enum.GetValues(typeof(LogLevel));
				Func<object, string> fRetrieveLogLevelNames = s => Enum.GetName(typeof(LogLevel), s);
				bool noLogFlags = FunkyBaseExtension.Settings.Logging.LogFlags.Equals(LogLevel.None);
				foreach (var logLevel in LogLevels)
				{
					LogLevel thisloglevel = (LogLevel)logLevel;
					if (thisloglevel.Equals(LogLevel.None) || thisloglevel.Equals(LogLevel.All)) continue;

					string loglevelName = fRetrieveLogLevelNames(logLevel);
					CheckBox cb = new CheckBox
					{
						Name = loglevelName,
						Text = loglevelName,
						Checked = !noLogFlags && FunkyBaseExtension.Settings.Logging.LogFlags.HasFlag(thisloglevel),
					};
					cb.CheckedChanged += FunkyLogLevelChanged;

					flowLayout_DebugFunkyLogLevels.Controls.Add(cb);
				}



				//
				var DebugDataFlags = Enum.GetValues(typeof(DebugDataTypes));
				Func<object, string> fRetrieveDebugDataFlagsNames = s => Enum.GetName(typeof(DebugDataTypes), s);
				bool noDebugDataFlags = FunkyBaseExtension.Settings.Debugging.DebuggingDataTypes.Equals(DebugDataTypes.None);
				foreach (var logLevel in DebugDataFlags)
				{
					DebugDataTypes thisloglevel = (DebugDataTypes)logLevel;
					if (thisloglevel.Equals(DebugDataTypes.None)) continue;

					string loglevelName = fRetrieveDebugDataFlagsNames(logLevel);
					CheckBox cb = new CheckBox
					{
						Name = loglevelName,
						Text = loglevelName,
						Checked = !noDebugDataFlags && FunkyBaseExtension.Settings.Debugging.DebuggingDataTypes.HasFlag(thisloglevel),
					};
					cb.CheckedChanged += DebugDataTypesChanged;

					flowLayout_DebugDataFlags.Controls.Add(cb);
				}

				//Misc Stats
				try
				{

					fBaseXtensions.Stats.GameStats cur = FunkyGame.CurrentGameStats;

					flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry("\r\n== CURRENT GAME SUMMARY =="));
					flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry(String.Format("Total Profiles:{0}\r\nDeaths:{1} TotalTime:{2} TotalGold:{3} TotalXP:{4}\r\n Bounties Completed {6}\r\n{5}",
															cur.Profiles.Count, cur.TotalDeaths, cur.TotalTimeRunning.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), cur.TotalGold, cur.TotalXP, cur.TotalLootTracker.ToString(), cur.TotalBountiesCompleted)));

					if (FunkyGame.CurrentGameStats.Profiles.Count > 0)
					{
						flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry("\r\n== PROFILES =="));
						foreach (var item in FunkyGame.CurrentGameStats.Profiles)
						{
							flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry(String.Format("{0}\r\nDeaths:{1} TotalTime:{2} TotalGold:{3} TotalXP:{4}\r\n{5}",
								item.ProfileName, item.DeathCount, item.TotalTimeSpan.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), item.TotalGold, item.TotalXP, item.LootTracker.ToString())));
						}
					}


					TotalStats all = FunkyGame.TrackingStats;
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


		private void tb_GlobeHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			FunkyBaseExtension.Settings.Combat.GlobeHealthPercent = Value;
			txt_GlobeHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}

		private void tb_WellHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			FunkyBaseExtension.Settings.Combat.HealthWellHealthPercent = Value;
			txt_WellHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}

		private void tb_PotionHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			FunkyBaseExtension.Settings.Combat.PotionHealthPercent = Value;
			txt_PotionHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}



		private void SettingsFor_Load(object sender, EventArgs e)
		{
			Text = FunkyGame.CurrentHeroName;// + " " + Bot.Character.Account.CurrentAccountName;
		}



		//private void BloodShardGambleItemsChanged(object sender, EventArgs e)
		//{
		//	CheckBox cbSender = (CheckBox)sender;
		//	BloodShardGambleItems LogLevelValue = (BloodShardGambleItems)Enum.Parse(typeof(BloodShardGambleItems), cbSender.Name);

		//	if (FunkyBaseExtension.Settings.TownRun.BloodShardGambleItems.HasFlag(LogLevelValue))
		//		FunkyBaseExtension.Settings.TownRun.BloodShardGambleItems &= ~LogLevelValue;
		//	else
		//		FunkyBaseExtension.Settings.TownRun.BloodShardGambleItems |= LogLevelValue;
		//}



		private void AvoidanceAttemptMovementChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Avoidance.AttemptAvoidanceMovements = !FunkyBaseExtension.Settings.Avoidance.AttemptAvoidanceMovements;
		}
		private void MovementTargetGlobeChecked(object sender, EventArgs e)
		{
			if (FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Globe))
				FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes &= ~TargetType.Globe;
			else
				FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes |= TargetType.Globe;
		}
		private void MovementTargetGoldChecked(object sender, EventArgs e)
		{
			if (FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Gold))
				FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes &= ~TargetType.Gold;
			else
				FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes |= TargetType.Gold;
		}

		private void MovementTargetItemChecked(object sender, EventArgs e)
		{
			if (FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Item))
				FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes &= ~TargetType.Item;
			else
				FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes |= TargetType.Item;
		}
		private void AllowDefaultAttackAlwaysChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Combat.AllowDefaultAttackAlways = !FunkyBaseExtension.Settings.Combat.AllowDefaultAttackAlways;
		}
		private void OutOfCombatMovementChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.General.OutOfCombatMovement = !FunkyBaseExtension.Settings.General.OutOfCombatMovement;
		}

		private void cb_ClusterTargetLogic_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Cluster.EnableClusteringTargetLogic = !FunkyBaseExtension.Settings.Cluster.EnableClusteringTargetLogic;
			gb_ClusteringOptions.Enabled = FunkyBaseExtension.Settings.Cluster.EnableClusteringTargetLogic;
		}



		private void tb_ClusterLogicDisableHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			FunkyBaseExtension.Settings.Cluster.IgnoreClusterLowHPValue = Value;
			txt_ClusterLogicDisableHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}

		private void tb_ClusterLogicDistance_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = slider_sender.Value;
			FunkyBaseExtension.Settings.Cluster.ClusterDistance = Value;
			txt_ClusterLogicDistance.Text = Value.ToString();
		}

		private void tb_ClusterLogicMinimumUnits_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = slider_sender.Value;
			FunkyBaseExtension.Settings.Cluster.ClusterMinimumUnitCount = Value;
			txt_ClusterLogicMinimumUnits.Text = Value.ToString();
		}


		private void bWaitForArchonChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Wizard.bWaitForArchon = !FunkyBaseExtension.Settings.Wizard.bWaitForArchon;
		}
		private void bKiteOnlyArchonChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Wizard.bKiteOnlyArchon = !FunkyBaseExtension.Settings.Wizard.bKiteOnlyArchon;
		}
		private void bCancelArchonRebuffChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Wizard.bCancelArchonRebuff = !FunkyBaseExtension.Settings.Wizard.bCancelArchonRebuff;
		}
		private void bTeleportFleeWhenLowHPChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Wizard.bTeleportFleeWhenLowHP = !FunkyBaseExtension.Settings.Wizard.bTeleportFleeWhenLowHP;
		}
		private void bTeleportIntoGroupingChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Wizard.bTeleportIntoGrouping = !FunkyBaseExtension.Settings.Wizard.bTeleportIntoGrouping;
		}
		private void bSelectiveWhirlwindChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bSelectiveWhirlwind = !FunkyBaseExtension.Settings.Barbarian.bSelectiveWhirlwind;
		}
		private void bWaitForWrathChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bWaitForWrath = !FunkyBaseExtension.Settings.Barbarian.bWaitForWrath;
		}
		private void bGoblinWrathChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bGoblinWrath = !FunkyBaseExtension.Settings.Barbarian.bGoblinWrath;
		}
		private void bBarbUseWOTBAlwaysChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bBarbUseWOTBAlways = !FunkyBaseExtension.Settings.Barbarian.bBarbUseWOTBAlways;
		}
		private void bFuryDumpWrathChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bFuryDumpWrath = !FunkyBaseExtension.Settings.Barbarian.bFuryDumpWrath;
		}
		private void bFuryDumpAlwaysChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bFuryDumpAlways = !FunkyBaseExtension.Settings.Barbarian.bFuryDumpAlways;
		}
		private void bMonkMaintainSweepingWindChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Monk.bMonkMaintainSweepingWind = !FunkyBaseExtension.Settings.Monk.bMonkMaintainSweepingWind;
		}
		private void bMonkSpamMantraChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Monk.bMonkSpamMantra = !FunkyBaseExtension.Settings.Monk.bMonkSpamMantra;
		}
		private void iDHVaultMovementDelaySliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.DemonHunter.iDHVaultMovementDelay = Value;
			//TBiDHVaultMovementDelay.Text = Value.ToString();
		}
		private void DebugStatusBarChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Debugging.DebugStatusBar = !FunkyBaseExtension.Settings.Debugging.DebugStatusBar;
		}
		private void SkipAheadChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Debugging.SkipAhead = !FunkyBaseExtension.Settings.Debugging.SkipAhead;
		}

		private void GroupBotHealthSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			FunkyBaseExtension.Settings.Grouping.GroupingMinimumBotHealth = Value;
			txt_GroupingMinBotHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}
		private void GroupMinimumUnitDistanceSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitDistance = Value;
			txt_GroupingMinDistance.Text = Value.ToString();
		}
		private void GroupMaxDistanceSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.Grouping.GroupingMaximumDistanceAllowed = Value;
			txt_GroupingMaxDistance.Text = Value.ToString();
		}
		private void GroupMinimumClusterCountSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.Grouping.GroupingMinimumClusterCount = Value;
			txt_GroupingMinCluster.Text = Value.ToString();
		}
		private void GroupMinimumUnitsInClusterSliderChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitsInCluster = Value;
			txt_GroupingMinUnitsInCluster.Text = Value.ToString();
		}
		private void GroupingBehaviorChecked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Grouping.AttemptGroupingMovements = !FunkyBaseExtension.Settings.Grouping.AttemptGroupingMovements;
			bool enabled = FunkyBaseExtension.Settings.Grouping.AttemptGroupingMovements;
			//spGroupingOptions.IsEnabled = enabled;
		}

		private void trackbar_FleeMaxMonsterDistance_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.Fleeing.FleeMaxMonsterDistance = Value;
			txt_FleeMaxMonsterDistance.Text = Value.ToString();
		}

		private void trackbar_FleeBotMinHealth_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture)) / 100;
			FunkyBaseExtension.Settings.Fleeing.FleeBotMinimumHealthPercent = Value;
			txt_FleeBotMinHealth.Text = Value.ToString("F2", CultureInfo.InvariantCulture);
		}

		private void cb_EnableFleeing_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior = !FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior;
			//groupBox_FleeOptions.Enabled = FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior;
			groupBox_FleeOptions.Enabled = FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior;
		}

		private void cb_FleeUnitElectrified_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Fleeing.FleeUnitElectrified = !FunkyBaseExtension.Settings.Fleeing.FleeUnitElectrified;
		}

		private void cb_FleeUnitRareElite_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Fleeing.FleeUnitRareElite = !FunkyBaseExtension.Settings.Fleeing.FleeUnitRareElite;
		}

		private void cb_FleeUnitNormal_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Fleeing.FleeUnitNormal = !FunkyBaseExtension.Settings.Fleeing.FleeUnitNormal;
		}

		private void cb_FleeUnitAboveAverageHitPoints_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Fleeing.FleeUnitAboveAverageHitPoints = !FunkyBaseExtension.Settings.Fleeing.FleeUnitAboveAverageHitPoints;
		}

		private void cb_FleeUnitFast_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreFast = !FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreFast;
		}

		private void cb_FleeUnitSucideBomber_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreSucideBomber = !FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreSucideBomber;
		}

		private void cb_FleeUnitRanged_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreRanged = !FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreRanged;
		}

		private void cb_BarbSelectiveWhirlwind_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bSelectiveWhirlwind = !FunkyBaseExtension.Settings.Barbarian.bSelectiveWhirlwind;
		}

		private void cb_BarbWaitForWrath_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bWaitForWrath = !FunkyBaseExtension.Settings.Barbarian.bWaitForWrath;
		}

		private void cb_BarbGoblinWrath_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bGoblinWrath = !FunkyBaseExtension.Settings.Barbarian.bGoblinWrath;
		}

		private void cb_BarbFuryDumpWrath_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bFuryDumpWrath = !FunkyBaseExtension.Settings.Barbarian.bFuryDumpWrath;
		}

		private void cb_BarbFuryDumpAlways_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bFuryDumpAlways = !FunkyBaseExtension.Settings.Barbarian.bFuryDumpAlways;
		}

		private void cb_BarbUseWOTBAlways_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Barbarian.bBarbUseWOTBAlways = !FunkyBaseExtension.Settings.Barbarian.bBarbUseWOTBAlways;
		}

		private void trackBar_DemonHunterValutDelay_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.DemonHunter.iDHVaultMovementDelay = Value;
			txt_DemonHunterVaultDelay.Text = Value.ToString();
		}

		private void cb_MonkSpamMantra_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Monk.bMonkSpamMantra = !FunkyBaseExtension.Settings.Monk.bMonkSpamMantra;
		}

		private void cb_MonkMaintainSweepingWinds_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Monk.bMonkMaintainSweepingWind = !FunkyBaseExtension.Settings.Monk.bMonkMaintainSweepingWind;
		}

		private void cb_WizardWaitForArchon_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Wizard.bWaitForArchon = !FunkyBaseExtension.Settings.Wizard.bWaitForArchon;
		}

		private void cb_WizardKiteOnlyArchon_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Wizard.bKiteOnlyArchon = !FunkyBaseExtension.Settings.Wizard.bKiteOnlyArchon;
		}

		private void cb_CacnelArchonRebuff_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Wizard.bCancelArchonRebuff = !FunkyBaseExtension.Settings.Wizard.bCancelArchonRebuff;
		}

		private void cb_WizardTeleportIntoGroups_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Wizard.bTeleportIntoGrouping = !FunkyBaseExtension.Settings.Wizard.bTeleportIntoGrouping;
		}

		private void cb_WizardTeleportFleeing_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Wizard.bTeleportFleeWhenLowHP = !FunkyBaseExtension.Settings.Wizard.bTeleportFleeWhenLowHP;
		}

		private void cb_TargetingIgnoreRareElites_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.IgnoreAboveAverageMobs = !FunkyBaseExtension.Settings.Targeting.IgnoreAboveAverageMobs;
		}

		private void cb_TargetingIgnoreCorpses_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.IgnoreCorpses = !FunkyBaseExtension.Settings.Targeting.IgnoreCorpses;
		}

		private void cb_TargetingIncreaseRangeRareChests_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.UseExtendedRangeRepChest = !FunkyBaseExtension.Settings.Targeting.UseExtendedRangeRepChest;
		}

		private void comboBox_TargetingGoblinPriority_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox slider_sender = (ComboBox)sender;
			int Value = (int)slider_sender.SelectedIndex;
			FunkyBaseExtension.Settings.Targeting.GoblinPriority = Value;
		}

		private void cb_TargetingPrioritizeCloseRangeUnits_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeUnits = !FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeUnits;
			panel_TargetingPriorityCloseRangeMinUnits.Enabled = FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeUnits;

		}

		private void tb_TargetingPriorityCloseRangeUnitsCount_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeMinimumUnits = Value;
			txt_TargetingPriorityCloseRangeUnitsCount.Text = Value.ToString();
		}

		private void cb_TargetingUnitExceptionsRanged_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.UnitExceptionRangedUnits = !FunkyBaseExtension.Settings.Targeting.UnitExceptionRangedUnits;
		}

		private void cb_TargetingUnitExceptionsSpawner_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.UnitExceptionSpawnerUnits = !FunkyBaseExtension.Settings.Targeting.UnitExceptionSpawnerUnits;
		}

		private void cb_TargetingUnitExceptionsSucideBomber_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.UnitExceptionSucideBombers = !FunkyBaseExtension.Settings.Targeting.UnitExceptionSucideBombers;
		}

		private void cb_TargetingUnitExceptionsLowHealth_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.UnitExceptionLowHP = !FunkyBaseExtension.Settings.Targeting.UnitExceptionLowHP;
			panel_TargetingUnitExceptionsLowHealthMaxDistance.Enabled = FunkyBaseExtension.Settings.Targeting.UnitExceptionLowHP;
		}

		private void tb_TargetingUnitExceptionLowHealthMaxDistance_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.Targeting.UnitExceptionLowHPMaximumDistance = Value;
			txt_TargetingUnitExceptionLowHealthMaxDistance.Text = Value.ToString();
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			MessageBox.Show(FunkyBaseExtension.Settings.Ranges.ContainerOpenRange.ToString());
		}

		private void cb_TargetRangeIgnoreKillRangeProfile_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Ranges.IgnoreCombatRange = !FunkyBaseExtension.Settings.Ranges.IgnoreCombatRange;
		}

		private void cb_TargetRangeIgnoreProfileBlacklists_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Ranges.IgnoreProfileBlacklists = !FunkyBaseExtension.Settings.Ranges.IgnoreProfileBlacklists;
		}

		private void cb_TargetRangeIgnoreLootProfileRange_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Ranges.IgnoreLootRange = !FunkyBaseExtension.Settings.Ranges.IgnoreLootRange;
		}

		private void cb_TargetLOSGoblins_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.LOSMovement.AllowTreasureGoblin = !FunkyBaseExtension.Settings.LOSMovement.AllowTreasureGoblin;
		}

		private void cb_TargetLOSRareElite_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.LOSMovement.AllowRareElites = !FunkyBaseExtension.Settings.LOSMovement.AllowRareElites;
		}

		private void cb_TargetLOSBossUniques_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.LOSMovement.AllowUniqueBoss = !FunkyBaseExtension.Settings.LOSMovement.AllowUniqueBoss;
		}

		private void cb_TargetLOSRanged_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.LOSMovement.AllowRanged = !FunkyBaseExtension.Settings.LOSMovement.AllowRanged;
		}

		private void cb_TargetLOSSucideBombers_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.LOSMovement.AllowSucideBomber = !FunkyBaseExtension.Settings.LOSMovement.AllowSucideBomber;
		}
		private void cb_TargetLOSEventSwitches_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.LOSMovement.AllowEventSwitches = !FunkyBaseExtension.Settings.LOSMovement.AllowEventSwitches;
		}

		private void cb_TargetLOSSpawnerUnits_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.LOSMovement.AllowSpawnerUnits = !FunkyBaseExtension.Settings.LOSMovement.AllowSpawnerUnits;
		}

		private void cb_TargetLOSRareChests_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.LOSMovement.AllowRareLootContainer = !FunkyBaseExtension.Settings.LOSMovement.AllowRareLootContainer;
		}

		private void cb_TargetLOSCursedChestShrine_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.LOSMovement.AllowCursedChestShrines = !FunkyBaseExtension.Settings.LOSMovement.AllowCursedChestShrines;
		}

		private void TargetingShrineCheckedChanged(object sender, EventArgs e)
		{
			CheckBox cbSender = (CheckBox)sender;
			int index = (int)Enum.Parse(typeof(ShrineTypes), cbSender.Text);
			FunkyBaseExtension.Settings.Targeting.UseShrineTypes[index] = !(FunkyBaseExtension.Settings.Targeting.UseShrineTypes[index]);
		}

		private void tb_GoldInactivityTimeout_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.Monitoring.GoldInactivityTimeoutSeconds = Value;
			txt_GoldInactivityTimeout.Text = Value.ToString();
		}

		private void cb_GeneralAllowBuffInTown_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.General.AllowBuffingInTown = !FunkyBaseExtension.Settings.General.AllowBuffingInTown;
		}

		private void tb_GeneralEndOfCombatDelayValue_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.General.AfterCombatDelay = Value;
			txt_GeneralEndOfCombatDelayValue.Text = Value.ToString();
		}

		private void cb_GeneralApplyEndDelayToContainers_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.General.EnableWaitAfterContainers = !FunkyBaseExtension.Settings.General.EnableWaitAfterContainers;
		}
		private void cb_GeneralSkipAhead_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Debugging.SkipAhead = !FunkyBaseExtension.Settings.Debugging.SkipAhead;
		}
		private void cb_DeathPotion_CheckedChanged(object sender, EventArgs e)
		{
			//FunkyBaseExtension.Settings.Death.WaitForPotionCooldown = !FunkyBaseExtension.Settings.Death.WaitForPotionCooldown;
		}
		private void cb_DeathSkills_CheckedChanged(object sender, EventArgs e)
		{
			//FunkyBaseExtension.Settings.Death.WaitForAllSkillsCooldown = !FunkyBaseExtension.Settings.Death.WaitForAllSkillsCooldown;
		}
		private void cb_AdventureModeEnabled_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.AdventureMode.EnableAdventuringMode = !FunkyBaseExtension.Settings.AdventureMode.EnableAdventuringMode;
		}

	
		private void cb_LootPickupCraftPlans_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Loot.PickupCraftPlans = !FunkyBaseExtension.Settings.Loot.PickupCraftPlans;
		}

		private void cb_LootGemAMETHYST_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Loot.PickupGems[2] = !FunkyBaseExtension.Settings.Loot.PickupGems[2];
		}

		private void cb_LootGemDiamond_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Loot.PickupGemDiamond = !FunkyBaseExtension.Settings.Loot.PickupGemDiamond;
		}

		private void cb_LootGemEMERALD_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Loot.PickupGems[1] = !FunkyBaseExtension.Settings.Loot.PickupGems[1];
		}

		private void cb_LootGemRUBY_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Loot.PickupGems[0] = !FunkyBaseExtension.Settings.Loot.PickupGems[0];
		}

		private void cb_LootGemTOPAZ_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Loot.PickupGems[3] = !FunkyBaseExtension.Settings.Loot.PickupGems[3];
		}

		private void tb_LootMinimumGold_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.Loot.MinimumGoldPile = Value;
			txt_LootMinimumGold.Text = Value.ToString();
		}

		private void tb_LootHealthPotions_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.Loot.MaximumHealthPotions = Value;
			txt_LootHealthPotions.Text = Value.ToString();
		}

		private void cb_LootCraftMats_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Loot.PickupCraftMaterials = !FunkyBaseExtension.Settings.Loot.PickupCraftMaterials;
		}

		private void cb_LootInfernoKeys_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Loot.PickupInfernalKeys = !FunkyBaseExtension.Settings.Loot.PickupInfernalKeys;
		}
		private void cb_LootExpBooks_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Loot.ExpBooks = !FunkyBaseExtension.Settings.Loot.ExpBooks;
		}
		private void cb_LootKeyStoneFragments_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Loot.PickupKeystoneFragments = !FunkyBaseExtension.Settings.Loot.PickupKeystoneFragments;
		}
		private void ItemLootChanged(object sender, EventArgs e)
		{
			ComboBox cbSender = (ComboBox)sender;
			string tagStr = (string)cbSender.Tag;
			int newValue = cbSender.SelectedIndex == 0 ? 0 : cbSender.SelectedIndex == 1 ? 61 : 1;

			if (tagStr == "white")
				FunkyBaseExtension.Settings.Loot.PickupWhiteItems = newValue;
			else if (tagStr == "magic")
				FunkyBaseExtension.Settings.Loot.PickupMagicItems = newValue;
			else if (tagStr == "rare")
				FunkyBaseExtension.Settings.Loot.PickupRareItems = newValue;
			else if (tagStr == "legendary")
				FunkyBaseExtension.Settings.Loot.PickupLegendaryItems = newValue;

		}
		private void GemQualityLevelChanged(object sender, EventArgs e)
		{
			ComboBox cbSender = (ComboBox)sender;

			FunkyBaseExtension.Settings.Loot.MinimumGemItemLevel = (int)Enum.Parse(typeof(GemQualityTypes), cbSender.Items[cbSender.SelectedIndex].ToString());
		}

		private void cb_DebugStatusBar_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Debugging.DebugStatusBar = !FunkyBaseExtension.Settings.Debugging.DebugStatusBar;
		}

		private void cb_DebugDataLogging_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Debugging.DebuggingData = !FunkyBaseExtension.Settings.Debugging.DebuggingData;
		}

		private void FunkyLogLevelChanged(object sender, EventArgs e)
		{
			CheckBox cbSender = (CheckBox)sender;
			LogLevel LogLevelValue = (LogLevel)Enum.Parse(typeof(LogLevel), cbSender.Name);

			if (FunkyBaseExtension.Settings.Logging.LogFlags.HasFlag(LogLevelValue))
				FunkyBaseExtension.Settings.Logging.LogFlags &= ~LogLevelValue;
			else
				FunkyBaseExtension.Settings.Logging.LogFlags |= LogLevelValue;
		}
		private void DebugDataTypesChanged(object sender, EventArgs e)
		{
			CheckBox cbSender = (CheckBox)sender;
			DebugDataTypes LogLevelValue = (DebugDataTypes)Enum.Parse(typeof(DebugDataTypes), cbSender.Name);

			if (FunkyBaseExtension.Settings.Debugging.DebuggingDataTypes.HasFlag(LogLevelValue))
				FunkyBaseExtension.Settings.Debugging.DebuggingDataTypes &= ~LogLevelValue;
			else
				FunkyBaseExtension.Settings.Debugging.DebuggingDataTypes |= LogLevelValue;
		}
		private void SettingsForm_FormClosing_1(object sender, FormClosingEventArgs e)
		{
			PluginSettings.SerializeToXML(FunkyBaseExtension.Settings);
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

				LBDebug.Controls.Add(new UserControlDebugEntry(FunkyGame.Hero.DebugString()));

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
				LBDebug.Controls.Add(new UserControlDebugEntry(FunkyGame.Targeting.Cache.DebugString()));
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
				if (FunkyGame.Hero.Class == null) return;
				LBDebug.Controls.Add(new UserControlDebugEntry(FunkyGame.Hero.Class.DebugString()));

				LBDebug.Controls.Add(new UserControlDebugEntry("==Current Hotbar Abilities=="));
				foreach (var item in FunkyGame.Hero.Class.Abilities.Values)
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


				LBDebug.Controls.Add(new UserControlDebugEntry("==Buffs=="));
				foreach (var item in Hotbar.CurrentBuffs.Keys)
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
				LBDebug.Controls.Add(new UserControlDebugEntry(FunkyGame.Bounty.DebugString()));
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
				foreach (var cacheObject in ObjectCache.InteractableObjectCache)
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

		private void btn_DumpInventory_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning || !ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null)
				return;

			LBDebug.Controls.Clear();

			try
			{
				ZetaDia.Memory.DisableCache();
				ZetaDia.Actors.Update();

				foreach (var o in ZetaDia.Me.Inventory.Backpack)
				{
					try
					{
						//CacheACDItem item = new CacheACDItem(o);
						string s=String.Format("Type {0} {1} - SNO: {2} BalanceID: {3}",
															o.ItemType, o.InternalName, o.ActorSNO, o.GameBalanceId);

						LBDebug.Controls.Add(new UserControlDebugEntry(s));
					}
					catch (Exception)
					{

					}

				}
				
			}
			catch
			{
				LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Exception"));
			}

			LBDebug.Focus();
		}

		private void btn_DebugDataFormatText_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();

			string output = DebugData.ConvertDebugData();
			LBDebug.Controls.Add(new UserControlDebugEntry(output));

			LBDebug.Focus();
		}

		private void btn_Test_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();

			string OutPut = "";
			foreach (var entry in TheCache.ObjectIDCache.UnitEntries.Values.OrderBy(unit => unit.InternalName))
			{
				OutPut = OutPut + entry.ReturnCacheEntryString() + "\r\n";
			}
			Clipboard.SetText(OutPut);

			LBDebug.Controls.Add(new UserControlDebugEntry(OutPut));
		}
	}
}
