using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using fBaseXtensions.Behaviors;
using fBaseXtensions.Cache.External.Debugging;
using fBaseXtensions.Cache.External.Enums;
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
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Settings
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();

		    var settingFiles = PluginSettings.ReturnAllSettingFiles();
		    int settingIndex = 0;

            foreach (var s in settingFiles)
            {
                comboBox_SettingFiles.Items.Add(s);
                if (s == FunkyGame.CurrentHeroName + "_Plugin.xml")
                    settingIndex = comboBox_SettingFiles.Items.Count - 1;
            }
		    comboBox_SettingFiles.SelectedIndex = settingIndex;
		    comboBox_SettingFiles.SelectedIndexChanged += comboBox_SettingFiles_SelectedIndexChanged;
		    initalizeControls();
		   

		}

	    private void initalizeControls()
	    {
	        initalizeControls_Combat();
	        initalizeControls_Targeting();
	        initalizeControls_General();
	        initalizeControls_Items();
	        initalizeControls_Advanced();
	        initalizeControls_MiscStats();
	    }

        #region Initalize Combat Controls

        private void initalizeControls_Combat()
        {
            initalizeControls_Combat_General();
            initalizeControls_Combat_Cluster();
            initalizeControls_Combat_Grouping();
            initalizeControls_Combat_Avoidance();
            initalizeControls_Combat_Fleeing();
            initalizeControls_Combat_ClassSettings();
        }

        private void initalizeControls_Combat_General()
        {
            tb_GlobeHealth.ValueChanged -= tb_GlobeHealth_ValueChanged;
            tb_GlobeHealth.Value = (int)(FunkyBaseExtension.Settings.Combat.GlobeHealthPercent * 100);
            tb_GlobeHealth.ValueChanged += tb_GlobeHealth_ValueChanged;

            tb_PotionHealth.ValueChanged -= tb_PotionHealth_ValueChanged;
            tb_PotionHealth.Value = (int)(FunkyBaseExtension.Settings.Combat.PotionHealthPercent * 100);
            tb_PotionHealth.ValueChanged += tb_PotionHealth_ValueChanged;

            tb_WellHealth.ValueChanged -= tb_WellHealth_ValueChanged;
            tb_WellHealth.Value = (int)(FunkyBaseExtension.Settings.Combat.HealthWellHealthPercent * 100);
            tb_WellHealth.ValueChanged += tb_WellHealth_ValueChanged;

            txt_GlobeHealth.Text = FunkyBaseExtension.Settings.Combat.GlobeHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
            txt_WellHealth.Text = FunkyBaseExtension.Settings.Combat.HealthWellHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
            txt_PotionHealth.Text = FunkyBaseExtension.Settings.Combat.PotionHealthPercent.ToString("F2", CultureInfo.InvariantCulture);

            cb_CombatMovementGlobes.CheckedChanged -= MovementTargetGlobeChecked;
            cb_CombatMovementGlobes.Checked = FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Globe);
            cb_CombatMovementGlobes.CheckedChanged += MovementTargetGlobeChecked;

            cb_CombatMovementGold.CheckedChanged -= MovementTargetGoldChecked;
            cb_CombatMovementGold.Checked = FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Gold);
            cb_CombatMovementGold.CheckedChanged += MovementTargetGoldChecked;

            cb_CombatMovementItems.CheckedChanged -= MovementTargetItemChecked;
            cb_CombatMovementItems.Checked = FunkyBaseExtension.Settings.Combat.CombatMovementTargetTypes.HasFlag(TargetType.Item);
            cb_CombatMovementItems.CheckedChanged += MovementTargetItemChecked;

            cb_CombatAllowDefaultAttack.CheckedChanged -= AllowDefaultAttackAlwaysChecked;
            cb_CombatAllowDefaultAttack.Checked = FunkyBaseExtension.Settings.Combat.AllowDefaultAttackAlways;
            cb_CombatAllowDefaultAttack.CheckedChanged += AllowDefaultAttackAlwaysChecked;
        }

        private void initalizeControls_Combat_Cluster()
        {
            cb_ClusterTargetLogic.CheckedChanged -= cb_ClusterTargetLogic_CheckedChanged;
            cb_ClusterTargetLogic.Checked = FunkyBaseExtension.Settings.Cluster.EnableClusteringTargetLogic;
            cb_ClusterTargetLogic.CheckedChanged += cb_ClusterTargetLogic_CheckedChanged;
            gb_ClusteringOptions.Enabled = FunkyBaseExtension.Settings.Cluster.EnableClusteringTargetLogic;

            txt_ClusterLogicDisableHealth.Text = FunkyBaseExtension.Settings.Cluster.IgnoreClusterLowHPValue.ToString("F2", CultureInfo.InvariantCulture);
            txt_ClusterLogicDistance.Text = FunkyBaseExtension.Settings.Cluster.ClusterDistance.ToString("F2", CultureInfo.InvariantCulture);
            txt_ClusterLogicMinimumUnits.Text = FunkyBaseExtension.Settings.Cluster.ClusterMinimumUnitCount.ToString("F2", CultureInfo.InvariantCulture);

            tb_ClusterLogicDisableHealth.ValueChanged -= tb_ClusterLogicDisableHealth_ValueChanged;
            tb_ClusterLogicDisableHealth.Value = (int)(FunkyBaseExtension.Settings.Cluster.IgnoreClusterLowHPValue * 100);
            tb_ClusterLogicDisableHealth.ValueChanged += tb_ClusterLogicDisableHealth_ValueChanged;

            tb_ClusterLogicDistance.ValueChanged -= tb_ClusterLogicDistance_ValueChanged;
            tb_ClusterLogicDistance.Value = (int)(FunkyBaseExtension.Settings.Cluster.ClusterDistance);
            tb_ClusterLogicDistance.ValueChanged += tb_ClusterLogicDistance_ValueChanged;

            tb_ClusterLogicMinimumUnits.ValueChanged -= tb_ClusterLogicMinimumUnits_ValueChanged;
            tb_ClusterLogicMinimumUnits.Value = (int)(FunkyBaseExtension.Settings.Cluster.ClusterMinimumUnitCount);
            tb_ClusterLogicMinimumUnits.ValueChanged += tb_ClusterLogicMinimumUnits_ValueChanged;

            cb_ClusterUnitException_RareElite.CheckedChanged -= cb_ClusterUnitException_RareElite_Checked;
            cb_ClusterUnitException_RareElite.Checked = FunkyBaseExtension.Settings.Cluster.UnitException_RareElites;
            cb_ClusterUnitException_RareElite.CheckedChanged += cb_ClusterUnitException_RareElite_Checked;


            flowLayoutPanel_ClusteringUnitExceptions.Controls.Clear();
            var unitflagValues = Enum.GetValues(typeof(UnitFlags));
            Func<object, string> fRetrieveunitflagNames = s => Enum.GetName(typeof(UnitFlags), s);
            bool noUnitFlags = FunkyBaseExtension.Settings.Cluster.UnitExceptions.Equals(UnitFlags.None);
            foreach (var logLevel in unitflagValues)
            {
                UnitFlags thisloglevel = (UnitFlags)logLevel;
                if (thisloglevel.Equals(UnitFlags.None)) continue;

                string loglevelName = fRetrieveunitflagNames(logLevel);
                CheckBox cb = new CheckBox
                {
                    Name = loglevelName,
                    Text = loglevelName,
                    Font = cb_ClusterUnitException_RareElite.Font,
                    FlatStyle = cb_ClusterUnitException_RareElite.FlatStyle,
                    AutoEllipsis = cb_ClusterUnitException_RareElite.AutoEllipsis,
                    AutoSize = cb_ClusterUnitException_RareElite.AutoSize,
                    Checked = !noUnitFlags && FunkyBaseExtension.Settings.Cluster.UnitExceptions.HasFlag(thisloglevel),
                };
                cb.CheckedChanged += ClusterUnitExceptionsChanged;

                flowLayoutPanel_ClusteringUnitExceptions.Controls.Add(cb);
            }
        }

        private void initalizeControls_Combat_Grouping()
        {
            cb_GroupingLogic.CheckedChanged -= GroupingBehaviorChecked;
            cb_GroupingLogic.Checked = FunkyBaseExtension.Settings.Grouping.AttemptGroupingMovements;
            cb_GroupingLogic.CheckedChanged += GroupingBehaviorChecked;

            txt_GroupingMaxDistance.Text = FunkyBaseExtension.Settings.Grouping.GroupingMaximumDistanceAllowed.ToString();
            txt_GroupingMinBotHealth.Text = FunkyBaseExtension.Settings.Grouping.GroupingMinimumBotHealth.ToString("F2", CultureInfo.InvariantCulture);
            txt_GroupingMinCluster.Text = FunkyBaseExtension.Settings.Grouping.GroupingMinimumClusterCount.ToString();
            txt_GroupingMinDistance.Text = FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitDistance.ToString();
            txt_GroupingMinUnitsInCluster.Text = FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitsInCluster.ToString();

            tb_GroupingMaxDistance.ValueChanged -= GroupMaxDistanceSliderChanged;
            tb_GroupingMaxDistance.Value = FunkyBaseExtension.Settings.Grouping.GroupingMaximumDistanceAllowed;
            tb_GroupingMaxDistance.ValueChanged += GroupMaxDistanceSliderChanged;

            tb_GroupingMinBotHealth.ValueChanged -= GroupBotHealthSliderChanged;
            tb_GroupingMinBotHealth.Value = (int)(FunkyBaseExtension.Settings.Grouping.GroupingMinimumBotHealth * 100);
            tb_GroupingMinBotHealth.ValueChanged += GroupBotHealthSliderChanged;

            tb_GroupingMinCluster.ValueChanged -= GroupMinimumClusterCountSliderChanged;
            tb_GroupingMinCluster.Value = FunkyBaseExtension.Settings.Grouping.GroupingMinimumClusterCount;
            tb_GroupingMinCluster.ValueChanged += GroupMinimumClusterCountSliderChanged;

            tb_GroupingMinDistance.ValueChanged -= GroupMinimumUnitDistanceSliderChanged;
            tb_GroupingMinDistance.Value = FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitDistance;
            tb_GroupingMinDistance.ValueChanged += GroupMinimumUnitDistanceSliderChanged;

            tb_GroupingMinUnitsInCluster.ValueChanged -= GroupMinimumUnitsInClusterSliderChanged;
            tb_GroupingMinUnitsInCluster.Value = FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitsInCluster;
            tb_GroupingMinUnitsInCluster.ValueChanged += GroupMinimumUnitsInClusterSliderChanged;

        }

        private void initalizeControls_Combat_Avoidance()
        {
            cb_AvoidanceLogic.CheckedChanged -= AvoidanceAttemptMovementChecked;
            cb_AvoidanceLogic.Checked = FunkyBaseExtension.Settings.Avoidance.AttemptAvoidanceMovements;
            cb_AvoidanceLogic.CheckedChanged += AvoidanceAttemptMovementChecked;

            flowLayoutPanel_Avoidances.Controls.Clear();
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

        }

        private void initalizeControls_Combat_Fleeing()
        {
            cb_EnableFleeing.CheckedChanged -= cb_EnableFleeing_CheckedChanged;
            cb_EnableFleeing.Checked = FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior;
            cb_EnableFleeing.CheckedChanged += cb_EnableFleeing_CheckedChanged;

            trackbar_FleeMaxMonsterDistance.ValueChanged -= trackbar_FleeMaxMonsterDistance_ValueChanged;
            trackbar_FleeMaxMonsterDistance.Value = FunkyBaseExtension.Settings.Fleeing.FleeMaxMonsterDistance;
            trackbar_FleeMaxMonsterDistance.ValueChanged += trackbar_FleeMaxMonsterDistance_ValueChanged;

            txt_FleeMaxMonsterDistance.Text = FunkyBaseExtension.Settings.Fleeing.FleeMaxMonsterDistance.ToString();

            trackbar_FleeBotMinHealth.ValueChanged -= trackbar_FleeBotMinHealth_ValueChanged;
            trackbar_FleeBotMinHealth.Value = (int)(FunkyBaseExtension.Settings.Fleeing.FleeBotMinimumHealthPercent * 100);
            trackbar_FleeBotMinHealth.ValueChanged += trackbar_FleeBotMinHealth_ValueChanged;

            txt_FleeBotMinHealth.Text = FunkyBaseExtension.Settings.Fleeing.FleeBotMinimumHealthPercent.ToString("F2", CultureInfo.InvariantCulture);
            groupBox_FleeOptions.Enabled = FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior;

            cb_FleeUnitElectrified.CheckedChanged -= cb_FleeUnitElectrified_CheckedChanged;
            cb_FleeUnitElectrified.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitElectrified;
            cb_FleeUnitElectrified.CheckedChanged += cb_FleeUnitElectrified_CheckedChanged;

            cb_FleeUnitFast.CheckedChanged -= cb_FleeUnitFast_CheckedChanged;
            cb_FleeUnitFast.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreFast;
            cb_FleeUnitFast.CheckedChanged += cb_FleeUnitFast_CheckedChanged;

            cb_FleeUnitNormal.CheckedChanged -= cb_FleeUnitNormal_CheckedChanged;
            cb_FleeUnitNormal.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitNormal;
            cb_FleeUnitNormal.CheckedChanged += cb_FleeUnitNormal_CheckedChanged;

            cb_FleeUnitRanged.CheckedChanged -= cb_FleeUnitRanged_CheckedChanged;
            cb_FleeUnitRanged.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreRanged;
            cb_FleeUnitRanged.CheckedChanged += cb_FleeUnitRanged_CheckedChanged;

            cb_FleeUnitRareElite.CheckedChanged -= cb_FleeUnitRareElite_CheckedChanged;
            cb_FleeUnitRareElite.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitRareElite;
            cb_FleeUnitRareElite.CheckedChanged += cb_FleeUnitRareElite_CheckedChanged;

            cb_FleeUnitSucideBomber.CheckedChanged -= cb_FleeUnitSucideBomber_CheckedChanged;
            cb_FleeUnitSucideBomber.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreSucideBomber;
            cb_FleeUnitSucideBomber.CheckedChanged += cb_FleeUnitSucideBomber_CheckedChanged;

            cb_FleeUnitAboveAverageHitPoints.CheckedChanged -= cb_FleeUnitAboveAverageHitPoints_CheckedChanged;
            cb_FleeUnitAboveAverageHitPoints.Checked = FunkyBaseExtension.Settings.Fleeing.FleeUnitAboveAverageHitPoints;
            cb_FleeUnitAboveAverageHitPoints.CheckedChanged += cb_FleeUnitAboveAverageHitPoints_CheckedChanged;
        }

        private void initalizeControls_Combat_ClassSettings()
        {
            //Remove any class tab pages
            if (tabControl_Combat.TabPages.Contains(tabPage_Barbarian))
                tabControl_Combat.TabPages.Remove(tabPage_Barbarian);

            if (tabControl_Combat.TabPages.Contains(tabPage_Monk))
                tabControl_Combat.TabPages.Remove(tabPage_Monk);

            if (tabControl_Combat.TabPages.Contains(tabPage_WitchDoctor))
                tabControl_Combat.TabPages.Remove(tabPage_WitchDoctor);

            if (tabControl_Combat.TabPages.Contains(tabPage_Wizard))
                tabControl_Combat.TabPages.Remove(tabPage_Wizard);

            if (tabControl_Combat.TabPages.Contains(tabPage_Crusader))
                tabControl_Combat.TabPages.Remove(tabPage_Crusader);

            if (tabControl_Combat.TabPages.Contains(tabPage_DemonHunter))
                tabControl_Combat.TabPages.Remove(tabPage_DemonHunter);

            //Add all class tab pages back -- just to remove them again!
            tabControl_Combat.TabPages.Add(tabPage_Barbarian);
            tabControl_Combat.TabPages.Add(tabPage_Monk);
            tabControl_Combat.TabPages.Add(tabPage_WitchDoctor);
            tabControl_Combat.TabPages.Add(tabPage_Wizard);
            tabControl_Combat.TabPages.Add(tabPage_Crusader);
            tabControl_Combat.TabPages.Add(tabPage_DemonHunter);

            List<TabPage> RemovalTabPages = new List<TabPage> { tabPage_DemonHunter, tabPage_Barbarian, tabPage_Wizard, tabPage_WitchDoctor, tabPage_Monk, tabPage_Crusader };

            if (FunkyBaseExtension.Settings.DemonHunter != null)
            {
                trackBar_DemonHunterValutDelay.ValueChanged -= trackBar_DemonHunterValutDelay_ValueChanged;
                trackBar_DemonHunterValutDelay.Value =
                    FunkyBaseExtension.Settings.DemonHunter.iDHVaultMovementDelay;
                trackBar_DemonHunterValutDelay.ValueChanged += trackBar_DemonHunterValutDelay_ValueChanged;

                txt_DemonHunterVaultDelay.Text =
                    FunkyBaseExtension.Settings.DemonHunter.iDHVaultMovementDelay.ToString();
                RemovalTabPages.Remove(tabPage_DemonHunter);
            }

            if (FunkyBaseExtension.Settings.Barbarian != null)
            {
                cb_BarbFuryDumpAlways.CheckedChanged -= cb_BarbFuryDumpAlways_CheckedChanged;
                cb_BarbFuryDumpAlways.Checked = FunkyBaseExtension.Settings.Barbarian.bBarbUseWOTBAlways;
                cb_BarbFuryDumpAlways.CheckedChanged += cb_BarbFuryDumpAlways_CheckedChanged;

                cb_BarbFuryDumpWrath.CheckedChanged -= cb_BarbFuryDumpWrath_CheckedChanged;
                cb_BarbFuryDumpWrath.Checked = FunkyBaseExtension.Settings.Barbarian.bFuryDumpWrath;
                cb_BarbFuryDumpWrath.CheckedChanged += cb_BarbFuryDumpWrath_CheckedChanged;

                cb_BarbGoblinWrath.CheckedChanged -= cb_BarbGoblinWrath_CheckedChanged;
                cb_BarbGoblinWrath.Checked = FunkyBaseExtension.Settings.Barbarian.bGoblinWrath;
                cb_BarbGoblinWrath.CheckedChanged += cb_BarbGoblinWrath_CheckedChanged;

                cb_BarbUseWOTBAlways.CheckedChanged -= cb_BarbUseWOTBAlways_CheckedChanged;
                cb_BarbUseWOTBAlways.Checked = FunkyBaseExtension.Settings.Barbarian.bBarbUseWOTBAlways;
                cb_BarbUseWOTBAlways.CheckedChanged += cb_BarbUseWOTBAlways_CheckedChanged;

                cb_BarbWaitForWrath.CheckedChanged -= cb_BarbWaitForWrath_CheckedChanged;
                cb_BarbWaitForWrath.Checked = FunkyBaseExtension.Settings.Barbarian.bWaitForWrath;
                cb_BarbWaitForWrath.CheckedChanged += cb_BarbWaitForWrath_CheckedChanged;

                cb_BarbSelectiveWhirldWind.CheckedChanged -= cb_BarbSelectiveWhirlwind_CheckedChanged;
                cb_BarbSelectiveWhirldWind.Checked = FunkyBaseExtension.Settings.Barbarian.bSelectiveWhirlwind;
                cb_BarbSelectiveWhirldWind.CheckedChanged += cb_BarbSelectiveWhirlwind_CheckedChanged;

                RemovalTabPages.Remove(tabPage_Barbarian);
            }

            if (FunkyBaseExtension.Settings.Wizard != null)
            {
                cb_WizardKiteOnlyArchon.CheckedChanged -= cb_WizardKiteOnlyArchon_CheckedChanged;
                cb_WizardKiteOnlyArchon.Checked = FunkyBaseExtension.Settings.Wizard.bKiteOnlyArchon;
                cb_WizardKiteOnlyArchon.CheckedChanged += cb_WizardKiteOnlyArchon_CheckedChanged;

                cb_WizardTeleportFleeing.CheckedChanged -= cb_WizardTeleportFleeing_CheckedChanged;
                cb_WizardTeleportFleeing.Checked = FunkyBaseExtension.Settings.Wizard.bTeleportFleeWhenLowHP;
                cb_WizardTeleportFleeing.CheckedChanged += cb_WizardTeleportFleeing_CheckedChanged;

                cb_WizardTeleportIntoGroups.CheckedChanged -= cb_WizardTeleportIntoGroups_CheckedChanged;
                cb_WizardTeleportIntoGroups.Checked = FunkyBaseExtension.Settings.Wizard.bTeleportIntoGrouping;
                cb_WizardTeleportIntoGroups.CheckedChanged += cb_WizardTeleportIntoGroups_CheckedChanged;

                cb_WizardWaitForArchon.CheckedChanged -= cb_WizardWaitForArchon_CheckedChanged;
                cb_WizardWaitForArchon.Checked = FunkyBaseExtension.Settings.Wizard.bWaitForArchon;
                cb_WizardWaitForArchon.CheckedChanged += cb_WizardWaitForArchon_CheckedChanged;

                cb_WizardCancelArchonRebuff.CheckedChanged -= cb_CacnelArchonRebuff_CheckedChanged;
                cb_WizardCancelArchonRebuff.Checked = FunkyBaseExtension.Settings.Wizard.bCancelArchonRebuff;
                cb_WizardCancelArchonRebuff.CheckedChanged += cb_CacnelArchonRebuff_CheckedChanged;

                RemovalTabPages.Remove(tabPage_Wizard);
            }

            if (FunkyBaseExtension.Settings.WitchDoctor != null)
            {
                RemovalTabPages.Remove(tabPage_WitchDoctor);
            }

            if (FunkyBaseExtension.Settings.Monk != null)
            {
                cb_MonkMaintainSweepingWinds.CheckedChanged -= bMonkMaintainSweepingWindChecked;
                cb_MonkMaintainSweepingWinds.Checked = FunkyBaseExtension.Settings.Monk.bMonkMaintainSweepingWind;
                cb_MonkMaintainSweepingWinds.CheckedChanged += bMonkMaintainSweepingWindChecked;

                cb_MonkSpamMantra.CheckedChanged -= bMonkSpamMantraChecked;
                cb_MonkSpamMantra.Checked = FunkyBaseExtension.Settings.Monk.bMonkSpamMantra;
                cb_MonkSpamMantra.CheckedChanged += bMonkSpamMantraChecked;

                RemovalTabPages.Remove(tabPage_Monk);
            }

            if (FunkyBaseExtension.Settings.Crusader != null)
            {
                RemovalTabPages.Remove(tabPage_Crusader);
            }

            foreach (var removalTabPage in RemovalTabPages)
            {
                tabControl_Combat.TabPages.Remove(removalTabPage);
            }

        }
        
        #endregion

        #region Initalize Targeting Controls

        private void initalizeControls_Targeting()
        {
            initalizeControls_Targeting_General();
            initalizeControls_Targeting_Ranges();
            initalizeControls_Targeting_LineOfSight();

        }

        private void initalizeControls_Targeting_General()
        {
            cb_TargetingShrineEmpowered.CheckedChanged -= TargetingShrineCheckedChanged;
            cb_TargetingShrineEmpowered.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[5];
            cb_TargetingShrineEmpowered.CheckedChanged += TargetingShrineCheckedChanged;

            cb_TargetingShrineEnlightnment.CheckedChanged -= TargetingShrineCheckedChanged;
            cb_TargetingShrineEnlightnment.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[1];
            cb_TargetingShrineEnlightnment.CheckedChanged += TargetingShrineCheckedChanged;

            cb_TargetingShrineFleeting.CheckedChanged -= TargetingShrineCheckedChanged;
            cb_TargetingShrineFleeting.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[0];
            cb_TargetingShrineFleeting.CheckedChanged += TargetingShrineCheckedChanged;

            cb_TargetingShrineFortune.CheckedChanged -= TargetingShrineCheckedChanged;
            cb_TargetingShrineFortune.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[3];
            cb_TargetingShrineFortune.CheckedChanged += TargetingShrineCheckedChanged;

            cb_TargetingShrineFrenzy.CheckedChanged -= TargetingShrineCheckedChanged;
            cb_TargetingShrineFrenzy.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[2];
            cb_TargetingShrineFrenzy.CheckedChanged += TargetingShrineCheckedChanged;

            cb_TargetingShrineProtection.CheckedChanged -= TargetingShrineCheckedChanged;
            cb_TargetingShrineProtection.Checked = FunkyBaseExtension.Settings.Targeting.UseShrineTypes[4];
            cb_TargetingShrineProtection.CheckedChanged += TargetingShrineCheckedChanged;


            cb_MovementOutOfCombatSkills.CheckedChanged -= OutOfCombatMovementChecked;
            cb_MovementOutOfCombatSkills.Checked = FunkyBaseExtension.Settings.General.OutOfCombatMovement;
            cb_MovementOutOfCombatSkills.CheckedChanged += OutOfCombatMovementChecked;

            cb_TargetingIgnoreCorpses.CheckedChanged -= cb_TargetingIgnoreCorpses_CheckedChanged;
            cb_TargetingIgnoreCorpses.Checked = FunkyBaseExtension.Settings.Targeting.IgnoreCorpses;
            cb_TargetingIgnoreCorpses.CheckedChanged += cb_TargetingIgnoreCorpses_CheckedChanged;

            cb_TargetingIgnoreArmorRacks.CheckedChanged -= cb_TargetingIgnoreArmorRacks_CheckedChanged;
            cb_TargetingIgnoreArmorRacks.Checked = FunkyBaseExtension.Settings.Targeting.IgnoreArmorRacks;
            cb_TargetingIgnoreArmorRacks.CheckedChanged += cb_TargetingIgnoreArmorRacks_CheckedChanged;

            cb_TargetingIgnoreWeaponRacks.CheckedChanged -= cb_TargetingIgnoreWeaponRacks_CheckedChanged;
            cb_TargetingIgnoreWeaponRacks.Checked = FunkyBaseExtension.Settings.Targeting.IgnoreWeaponRacks;
            cb_TargetingIgnoreWeaponRacks.CheckedChanged += cb_TargetingIgnoreWeaponRacks_CheckedChanged;

            cb_TargetingIgnoreFloorContainers.CheckedChanged -= cb_TargetingIgnoreFloorContainers_CheckedChanged;
            cb_TargetingIgnoreFloorContainers.Checked = FunkyBaseExtension.Settings.Targeting.IgnoreFloorContainers;
            cb_TargetingIgnoreFloorContainers.CheckedChanged += cb_TargetingIgnoreFloorContainers_CheckedChanged;

            cb_TargetingIgnoreNormalChests.CheckedChanged -= cb_TargetingIgnoreNormalChests_CheckedChanged;
            cb_TargetingIgnoreNormalChests.Checked = FunkyBaseExtension.Settings.Targeting.IgnoreNormalChests;
            cb_TargetingIgnoreNormalChests.CheckedChanged += cb_TargetingIgnoreNormalChests_CheckedChanged;

            cb_TargetingIgnoreRareChests.CheckedChanged -= cb_TargetingIgnoreRareChests_CheckedChanged;
            cb_TargetingIgnoreRareChests.Checked = FunkyBaseExtension.Settings.Targeting.IgnoreRareChests;
            cb_TargetingIgnoreRareChests.CheckedChanged += cb_TargetingIgnoreRareChests_CheckedChanged;

            cb_TargetingIgnoreRareElites.CheckedChanged -= cb_TargetingIgnoreRareElites_CheckedChanged;
            cb_TargetingIgnoreRareElites.Checked = FunkyBaseExtension.Settings.Targeting.IgnoreAboveAverageMobs;
            cb_TargetingIgnoreRareElites.CheckedChanged += cb_TargetingIgnoreRareElites_CheckedChanged;

            cb_TargetingIncreaseRangeRareChests.CheckedChanged -= cb_TargetingIncreaseRangeRareChests_CheckedChanged;
            cb_TargetingIncreaseRangeRareChests.Checked = FunkyBaseExtension.Settings.Targeting.UseExtendedRangeRepChest;
            cb_TargetingIncreaseRangeRareChests.CheckedChanged += cb_TargetingIncreaseRangeRareChests_CheckedChanged;

            comboBox_TargetingGoblinPriority.SelectedIndexChanged -= comboBox_TargetingGoblinPriority_SelectedIndexChanged;
            comboBox_TargetingGoblinPriority.SelectedIndex = FunkyBaseExtension.Settings.Targeting.GoblinPriority;
            comboBox_TargetingGoblinPriority.SelectedIndexChanged += comboBox_TargetingGoblinPriority_SelectedIndexChanged;

            panel_TargetingPriorityCloseRangeMinUnits.Enabled = FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeUnits;

            tb_TargetingPriorityCloseRangeUnitsCount.ValueChanged -= tb_TargetingPriorityCloseRangeUnitsCount_ValueChanged;
            tb_TargetingPriorityCloseRangeUnitsCount.Value = FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeMinimumUnits;
            tb_TargetingPriorityCloseRangeUnitsCount.ValueChanged += tb_TargetingPriorityCloseRangeUnitsCount_ValueChanged;

            txt_TargetingPriorityCloseRangeUnitsCount.Text = FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeMinimumUnits.ToString();

            cb_TargetingPrioritizeCloseRangeUnits.CheckedChanged -= cb_TargetingPrioritizeCloseRangeUnits_CheckedChanged;
            cb_TargetingPrioritizeCloseRangeUnits.Checked = FunkyBaseExtension.Settings.Targeting.PrioritizeCloseRangeUnits;
            cb_TargetingPrioritizeCloseRangeUnits.CheckedChanged += cb_TargetingPrioritizeCloseRangeUnits_CheckedChanged;

        }

        private void initalizeControls_Targeting_Ranges()
        {
            cb_TargetRangeIgnoreKillRangeProfile.CheckedChanged -= cb_TargetRangeIgnoreKillRangeProfile_CheckedChanged;
            cb_TargetRangeIgnoreKillRangeProfile.Checked = FunkyBaseExtension.Settings.Ranges.IgnoreCombatRange;
            cb_TargetRangeIgnoreKillRangeProfile.CheckedChanged += cb_TargetRangeIgnoreKillRangeProfile_CheckedChanged;

            cb_TargetRangeIgnoreLootProfileRange.CheckedChanged -= cb_TargetRangeIgnoreLootProfileRange_CheckedChanged;
            cb_TargetRangeIgnoreLootProfileRange.Checked = FunkyBaseExtension.Settings.Ranges.IgnoreLootRange;
            cb_TargetRangeIgnoreLootProfileRange.CheckedChanged += cb_TargetRangeIgnoreLootProfileRange_CheckedChanged;

            cb_TargetRangeIgnoreProfileBlacklists.CheckedChanged -= cb_TargetRangeIgnoreProfileBlacklists_CheckedChanged;
            cb_TargetRangeIgnoreProfileBlacklists.Checked = FunkyBaseExtension.Settings.Ranges.IgnoreProfileBlacklists;
            cb_TargetRangeIgnoreProfileBlacklists.CheckedChanged += cb_TargetRangeIgnoreProfileBlacklists_CheckedChanged;


            flowLayout_TargetRanges.Controls.Clear();

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

        }

        private void initalizeControls_Targeting_LineOfSight()
        {
            trackBar_LOS_MaxRange.ValueChanged -= trackbar_LOS_MaxRange_ValueChanged;
            trackBar_LOS_MaxRange.Value = FunkyBaseExtension.Settings.LOSMovement.MaximumRange;
            textBox_LOS_MaxRange.Text = FunkyBaseExtension.Settings.LOSMovement.MaximumRange.ToString();
            trackBar_LOS_MaxRange.ValueChanged += trackbar_LOS_MaxRange_ValueChanged;

            trackBar_LOS_MinRange.ValueChanged -= trackbar_LOS_MinRange_ValueChanged;
            trackBar_LOS_MinRange.Value = FunkyBaseExtension.Settings.LOSMovement.MiniumRangeObjects;
            textBox_LOS_MinRange.Text = FunkyBaseExtension.Settings.LOSMovement.MiniumRangeObjects.ToString();
            trackBar_LOS_MinRange.ValueChanged += trackbar_LOS_MinRange_ValueChanged;

            cb_TargetLOSBossUniques.CheckedChanged -= cb_TargetLOSBossUniques_CheckedChanged;
            cb_TargetLOSBossUniques.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowUniqueBoss;
            cb_TargetLOSBossUniques.CheckedChanged += cb_TargetLOSBossUniques_CheckedChanged;

            cb_TargetLOSCursedChestShrine.CheckedChanged -= cb_TargetLOSCursedChestShrine_CheckedChanged;
            cb_TargetLOSCursedChestShrine.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowCursedChestShrines;
            cb_TargetLOSCursedChestShrine.CheckedChanged += cb_TargetLOSCursedChestShrine_CheckedChanged;

            cb_TargetLOSGoblins.CheckedChanged -= cb_TargetLOSGoblins_CheckedChanged;
            cb_TargetLOSGoblins.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowTreasureGoblin;
            cb_TargetLOSGoblins.CheckedChanged += cb_TargetLOSGoblins_CheckedChanged;

            cb_TargetLOSRanged.CheckedChanged -= cb_TargetLOSRanged_CheckedChanged;
            cb_TargetLOSRanged.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowRanged;
            cb_TargetLOSRanged.CheckedChanged += cb_TargetLOSRanged_CheckedChanged;

            cb_TargetLOSRareChests.CheckedChanged -= cb_TargetLOSRareChests_CheckedChanged;
            cb_TargetLOSRareChests.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowRareLootContainer;
            cb_TargetLOSRareChests.CheckedChanged += cb_TargetLOSRareChests_CheckedChanged;

            cb_TargetLOSRareElite.CheckedChanged -= cb_TargetLOSRareElite_CheckedChanged;
            cb_TargetLOSRareElite.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowRareElites;
            cb_TargetLOSRareElite.CheckedChanged += cb_TargetLOSRareElite_CheckedChanged;

            cb_TargetLOSSpawnerUnits.CheckedChanged -= cb_TargetLOSSpawnerUnits_CheckedChanged;
            cb_TargetLOSSpawnerUnits.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowSpawnerUnits;
            cb_TargetLOSSpawnerUnits.CheckedChanged += cb_TargetLOSSpawnerUnits_CheckedChanged;

            cb_TargetLOSSucideBombers.CheckedChanged -= cb_TargetLOSSucideBombers_CheckedChanged;
            cb_TargetLOSSucideBombers.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowSucideBomber;
            cb_TargetLOSSucideBombers.CheckedChanged += cb_TargetLOSSucideBombers_CheckedChanged;

            cb_LOSEventSwitches.CheckedChanged -= cb_TargetLOSEventSwitches_CheckedChanged;
            cb_LOSEventSwitches.Checked = FunkyBaseExtension.Settings.LOSMovement.AllowEventSwitches;
            cb_LOSEventSwitches.CheckedChanged += cb_TargetLOSEventSwitches_CheckedChanged;

        }
        
        #endregion

	    private void initalizeControls_General()
	    {
	        initalizeControls_General_Misc();
	        initalizeControls_General_Death();
	        initalizeControls_General_AdventureMode();

            if (CharacterControl.HeroIndexInfo.Characters.Count == 0)
            {
                groupBox_BnetControl_Setup.Enabled = true;
            }
            else
            {
                groupBox_BnetControl_Setup.Enabled = false;
                groupBox_BnetControl_AltHero.Enabled = true;
                UpdateBnetHeroComboBox();
            }

            //
	        comboBox_BnetControl_CustomDifficulty.SelectedIndexChanged -= GameDifficulty_SelectedIndexChanged;
	        comboBox_BnetControl_CustomDifficulty.Items.Clear();
	        int index = -1;
	        foreach (var d in Enum.GetNames(typeof(GameDifficulty)))
	        {
	            comboBox_BnetControl_CustomDifficulty.Items.Add(d);
	            if (FunkyBaseExtension.Settings.General.CustomDifficulty == d)
	                index = comboBox_BnetControl_CustomDifficulty.Items.Count - 1;
	        }
            if (index>=0) comboBox_BnetControl_CustomDifficulty.SelectedIndex = index;
            comboBox_BnetControl_CustomDifficulty.SelectedIndexChanged += GameDifficulty_SelectedIndexChanged;
	    }

	    private void initalizeControls_General_Misc()
        {
            tb_GoldInactivityTimeout.ValueChanged -= tb_GoldInactivityTimeout_ValueChanged;
            txt_GoldInactivityTimeout.Text = FunkyBaseExtension.Settings.Monitoring.GoldInactivityTimeoutSeconds.ToString();
            tb_GoldInactivityTimeout.Value = FunkyBaseExtension.Settings.Monitoring.GoldInactivityTimeoutSeconds;
            tb_GoldInactivityTimeout.ValueChanged += tb_GoldInactivityTimeout_ValueChanged;

            cb_GeneralAllowBuffInTown.CheckedChanged -= cb_GeneralAllowBuffInTown_CheckedChanged;
            cb_GeneralAllowBuffInTown.Checked = FunkyBaseExtension.Settings.General.AllowBuffingInTown;
            cb_GeneralAllowBuffInTown.CheckedChanged += cb_GeneralAllowBuffInTown_CheckedChanged;

            txt_GeneralEndOfCombatDelayValue.Text = FunkyBaseExtension.Settings.General.AfterCombatDelay.ToString();

            tb_GeneralEndOfCombatDelayValue.ValueChanged -= tb_GeneralEndOfCombatDelayValue_ValueChanged;
            tb_GeneralEndOfCombatDelayValue.Value = FunkyBaseExtension.Settings.General.AfterCombatDelay;
            tb_GeneralEndOfCombatDelayValue.ValueChanged += tb_GeneralEndOfCombatDelayValue_ValueChanged;

            cb_GeneralApplyEndDelayToContainers.CheckedChanged -= cb_GeneralApplyEndDelayToContainers_CheckedChanged;
            cb_GeneralApplyEndDelayToContainers.Checked = FunkyBaseExtension.Settings.General.EnableWaitAfterContainers;
            cb_GeneralApplyEndDelayToContainers.CheckedChanged += cb_GeneralApplyEndDelayToContainers_CheckedChanged;

            cb_GeneralSkipAhead.CheckedChanged -= cb_GeneralSkipAhead_CheckedChanged;
            cb_GeneralSkipAhead.Checked = FunkyBaseExtension.Settings.Debugging.SkipAhead;
            cb_GeneralSkipAhead.CheckedChanged += cb_GeneralSkipAhead_CheckedChanged;

	    }

	    private void initalizeControls_General_Death()
	    {
            cb_DeathWaitForPotion.CheckedChanged -= cb_DeathPotion_CheckedChanged;
            cb_DeathWaitForPotion.Checked = FunkyBaseExtension.Settings.Death.WaitForPotionCooldown;
            cb_DeathWaitForPotion.CheckedChanged += cb_DeathPotion_CheckedChanged;

            cb_DeathWaitForSkillsCooldown.CheckedChanged -= cb_DeathSkills_CheckedChanged;
            cb_DeathWaitForSkillsCooldown.Checked = FunkyBaseExtension.Settings.Death.WaitForAllSkillsCooldown;
            cb_DeathWaitForSkillsCooldown.CheckedChanged += cb_DeathSkills_CheckedChanged;

	    }

	    private void initalizeControls_General_AdventureMode()
	    {
            cb_adventuremode_NavigateMinimapMarkers.CheckedChanged -= cb_adventuremode_NavigateMinimapMarkers_CheckedChanged;
            cb_adventuremode_NavigateMinimapMarkers.Checked = FunkyBaseExtension.Settings.AdventureMode.NavigatePointsOfInterest;
            cb_adventuremode_NavigateMinimapMarkers.CheckedChanged += cb_adventuremode_NavigateMinimapMarkers_CheckedChanged;

            cb_adventuremode_allowcombatmodification.CheckedChanged -= cb_adventuremode_allowcombatmodification_CheckedChanged;
            cb_adventuremode_allowcombatmodification.Checked = FunkyBaseExtension.Settings.AdventureMode.AllowCombatModifications;
            cb_adventuremode_allowcombatmodification.CheckedChanged += cb_adventuremode_allowcombatmodification_CheckedChanged;

            trackBar_TieredRiftKey.ValueChanged -= tb_TieredRiftKey_ValueChanged;
            trackBar_TieredRiftKey.Value = FunkyBaseExtension.Settings.AdventureMode.MaximumTieredRiftKeyAllowed;
            trackBar_TieredRiftKey.ValueChanged += tb_TieredRiftKey_ValueChanged;
            textBox_MaxTieredRiftKey.Text = FunkyBaseExtension.Settings.AdventureMode.MaximumTieredRiftKeyAllowed.ToString();

            comboBox_GemUpgrading_SuccessRate.SelectedIndexChanged -= cb_GemUpgrading_SuccessRate_CheckedChanged;
            comboBox_GemUpgrading_SuccessRate.Text = (FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate * 100).ToString() + "%";
            comboBox_GemUpgrading_SuccessRate.SelectedIndexChanged += cb_GemUpgrading_SuccessRate_CheckedChanged;

            radioButton_GemUpgrading_None.CheckedChanged -= radioButton_GemUpgrading_CheckedChanged;
            radioButton_GemUpgrading_Highest.CheckedChanged -= radioButton_GemUpgrading_CheckedChanged;
            radioButton_GemUpgrading_Lowest.CheckedChanged -= radioButton_GemUpgrading_CheckedChanged;
            radioButton_GemUpgrading_Priority.CheckedChanged -= radioButton_GemUpgrading_CheckedChanged;

            groupBox_PriorityGemUpgrading.Enabled = false;
            if (FunkyBaseExtension.Settings.AdventureMode.GemUpgradeType == SettingAdventureMode.GemUpgradingType.None)
                radioButton_GemUpgrading_None.Checked = true;
            else if (FunkyBaseExtension.Settings.AdventureMode.GemUpgradeType == SettingAdventureMode.GemUpgradingType.HighestRank)
                radioButton_GemUpgrading_Highest.Checked = true;
            else if (FunkyBaseExtension.Settings.AdventureMode.GemUpgradeType == SettingAdventureMode.GemUpgradingType.LowestRank)
                radioButton_GemUpgrading_Lowest.Checked = true;
            else if (FunkyBaseExtension.Settings.AdventureMode.GemUpgradeType == SettingAdventureMode.GemUpgradingType.Priority)
            {
                radioButton_GemUpgrading_Priority.Checked = true;
                groupBox_PriorityGemUpgrading.Enabled = true;
            }

            radioButton_GemUpgrading_None.CheckedChanged += radioButton_GemUpgrading_CheckedChanged;
            radioButton_GemUpgrading_Highest.CheckedChanged += radioButton_GemUpgrading_CheckedChanged;
            radioButton_GemUpgrading_Lowest.CheckedChanged += radioButton_GemUpgrading_CheckedChanged;
            radioButton_GemUpgrading_Priority.CheckedChanged += radioButton_GemUpgrading_CheckedChanged;

	        listBox_GemUpgrading_PriorityList.Items.Clear();
            var legendarGems = Enum.GetValues(typeof(LegendaryGemTypes));
            Func<object, string> fRetrieveLegendaryGemsNames = s => Enum.GetName(typeof(LegendaryGemTypes), s);

            foreach (var gem in FunkyBaseExtension.Settings.AdventureMode.GemUpgradePriorityList)
            {
                LegendaryGemTypes thisLegendaryGem = (LegendaryGemTypes)gem;
                if (thisLegendaryGem.Equals(LegendaryGemTypes.None)) continue;

                listBox_GemUpgrading_PriorityList.Items.Add(fRetrieveLegendaryGemsNames(gem));
            }

	        listBox_GemUpgrading_UnusedGems.Items.Clear();
            foreach (var gem in legendarGems)
            {
                LegendaryGemTypes thisLegendaryGem = (LegendaryGemTypes)gem;
                if (thisLegendaryGem.Equals(LegendaryGemTypes.None)) continue;

                if (!FunkyBaseExtension.Settings.AdventureMode.GemUpgradePriorityList.Contains(thisLegendaryGem))
                    listBox_GemUpgrading_UnusedGems.Items.Add(fRetrieveLegendaryGemsNames(gem));
            }
	    }

	    private void initalizeControls_Items()
	    {
            comboBox_LootLegendaryItemQuality.SelectedIndexChanged -= ItemLootChanged;
            comboBox_LootLegendaryItemQuality.SelectedIndex = FunkyBaseExtension.Settings.Loot.PickupLegendaryItems == 0 ? 0 : FunkyBaseExtension.Settings.Loot.PickupLegendaryItems == 61 ? 1 : 2;
            comboBox_LootLegendaryItemQuality.SelectedIndexChanged += ItemLootChanged;

            comboBox_LootMagicItemQuality.SelectedIndexChanged -= ItemLootChanged;
            comboBox_LootMagicItemQuality.SelectedIndex = FunkyBaseExtension.Settings.Loot.PickupMagicItems == 0 ? 0 : FunkyBaseExtension.Settings.Loot.PickupMagicItems == 61 ? 1 : 2;
            comboBox_LootMagicItemQuality.SelectedIndexChanged += ItemLootChanged;

            comboBox_LootRareItemQuality.SelectedIndexChanged -= ItemLootChanged;
            comboBox_LootRareItemQuality.SelectedIndex = FunkyBaseExtension.Settings.Loot.PickupRareItems == 0 ? 0 : FunkyBaseExtension.Settings.Loot.PickupRareItems == 61 ? 1 : 2;
            comboBox_LootRareItemQuality.SelectedIndexChanged += ItemLootChanged;

            comboBox_LootWhiteItemQuality.SelectedIndexChanged -= ItemLootChanged;
            comboBox_LootWhiteItemQuality.SelectedIndex = FunkyBaseExtension.Settings.Loot.PickupWhiteItems == 0 ? 0 : FunkyBaseExtension.Settings.Loot.PickupWhiteItems == 61 ? 1 : 2;
            comboBox_LootWhiteItemQuality.SelectedIndexChanged += ItemLootChanged;

            cb_LootPickupCraftPlans.CheckedChanged -= cb_LootPickupCraftPlans_CheckedChanged;
            cb_LootPickupCraftPlans.Checked = FunkyBaseExtension.Settings.Loot.PickupCraftPlans;
            cb_LootPickupCraftPlans.CheckedChanged += cb_LootPickupCraftPlans_CheckedChanged;

            comboBox_LootGemQuality.SelectedIndexChanged -= GemQualityLevelChanged;
            comboBox_LootGemQuality.Text = Enum.GetName(typeof(GemQualityTypes), FunkyBaseExtension.Settings.Loot.MinimumGemItemLevel).ToString();
            comboBox_LootGemQuality.SelectedIndexChanged += GemQualityLevelChanged;

            cb_LootGemAMETHYST.CheckedChanged -= cb_LootGemAMETHYST_CheckedChanged;
            cb_LootGemAMETHYST.Checked = FunkyBaseExtension.Settings.Loot.PickupGems[2];
            cb_LootGemAMETHYST.CheckedChanged += cb_LootGemAMETHYST_CheckedChanged;

            cb_LootGemDiamond.CheckedChanged -= cb_LootGemDiamond_CheckedChanged;
            cb_LootGemDiamond.Checked = FunkyBaseExtension.Settings.Loot.PickupGemDiamond;
            cb_LootGemDiamond.CheckedChanged += cb_LootGemDiamond_CheckedChanged;

            cb_LootGemEMERALD.CheckedChanged -= cb_LootGemEMERALD_CheckedChanged;
            cb_LootGemEMERALD.Checked = FunkyBaseExtension.Settings.Loot.PickupGems[1];
            cb_LootGemEMERALD.CheckedChanged += cb_LootGemEMERALD_CheckedChanged;

            cb_LootGemRUBY.CheckedChanged -= cb_LootGemRUBY_CheckedChanged;
            cb_LootGemRUBY.Checked = FunkyBaseExtension.Settings.Loot.PickupGems[0];
            cb_LootGemRUBY.CheckedChanged += cb_LootGemRUBY_CheckedChanged;

            cb_LootGemTOPAZ.CheckedChanged -= cb_LootGemTOPAZ_CheckedChanged;
            cb_LootGemTOPAZ.Checked = FunkyBaseExtension.Settings.Loot.PickupGems[3];
            cb_LootGemTOPAZ.CheckedChanged += cb_LootGemTOPAZ_CheckedChanged;

            tb_LootMinimumGold.ValueChanged -= tb_LootMinimumGold_ValueChanged;
            txt_LootMinimumGold.Text = FunkyBaseExtension.Settings.Loot.MinimumGoldPile.ToString();
            tb_LootMinimumGold.Value = FunkyBaseExtension.Settings.Loot.MinimumGoldPile;
            tb_LootMinimumGold.ValueChanged += tb_LootMinimumGold_ValueChanged;

            tb_LootHealthPotions.ValueChanged -= tb_LootHealthPotions_ValueChanged;
            txt_LootHealthPotions.Text = FunkyBaseExtension.Settings.Loot.MaximumHealthPotions.ToString();
            tb_LootHealthPotions.Value = FunkyBaseExtension.Settings.Loot.MaximumHealthPotions;
            tb_LootHealthPotions.ValueChanged += tb_LootHealthPotions_ValueChanged;

            cb_LootCraftMats.CheckedChanged -= cb_LootCraftMats_CheckedChanged;
            cb_LootCraftMats.Checked = FunkyBaseExtension.Settings.Loot.PickupCraftMaterials;
            cb_LootCraftMats.CheckedChanged += cb_LootCraftMats_CheckedChanged;

            cb_LootInfernoKeys.CheckedChanged -= cb_LootInfernoKeys_CheckedChanged;
            cb_LootInfernoKeys.Checked = FunkyBaseExtension.Settings.Loot.PickupInfernalKeys;
            cb_LootInfernoKeys.CheckedChanged += cb_LootInfernoKeys_CheckedChanged;

            cb_LootExpBooks.CheckedChanged -= cb_LootExpBooks_CheckedChanged;
            cb_LootExpBooks.Checked = FunkyBaseExtension.Settings.Loot.ExpBooks;
            cb_LootExpBooks.CheckedChanged += cb_LootExpBooks_CheckedChanged;

            cb_LootKeyStones.CheckedChanged -= cb_LootKeyStoneFragments_CheckedChanged;
            cb_LootKeyStones.Checked = FunkyBaseExtension.Settings.Loot.PickupKeystoneFragments;
            cb_LootKeyStones.CheckedChanged += cb_LootKeyStoneFragments_CheckedChanged;

	    }

	    private void initalizeControls_Advanced()
	    {
            cb_DebugDataLogging.CheckedChanged -= cb_DebugDataLogging_CheckedChanged;
            cb_DebugDataLogging.Checked = FunkyBaseExtension.Settings.Debugging.DebuggingData;
            cb_DebugDataLogging.CheckedChanged += cb_DebugDataLogging_CheckedChanged;

            cb_DebugStatusBar.CheckedChanged -= cb_DebugStatusBar_CheckedChanged;
            cb_DebugStatusBar.Checked = FunkyBaseExtension.Settings.Debugging.DebugStatusBar;
            cb_DebugStatusBar.CheckedChanged += cb_DebugStatusBar_CheckedChanged;

	        flowLayout_DebugFunkyLogLevels.Controls.Clear();
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
	        flowLayout_DebugDataFlags.Controls.Clear();
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
	    }

	    private void initalizeControls_MiscStats()
	    {
	        flowLayoutPanel_MiscStats.Controls.Clear();
            try
            {
                flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry("== TOTAL SUMMARY ==\r\n" + FunkyGame.CurrentStats.GenerateOutputString()));

                flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry("== PROFILE SUMMARY =="));
                foreach (var item in FunkyGame.CurrentStats.Profiles)
                {
                    flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry(item.GenerateOutput()));
                }
            }
            catch
            {
                flowLayoutPanel_MiscStats.Controls.Add(new UserControlDebugEntry("Exception Handled"));
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
		private void cb_ClusterUnitException_RareElite_Checked(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Cluster.UnitException_RareElites = !FunkyBaseExtension.Settings.Cluster.UnitException_RareElites;
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
		private void cb_TargetingIgnoreArmorRacks_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.IgnoreArmorRacks = !FunkyBaseExtension.Settings.Targeting.IgnoreArmorRacks;
		}
		private void cb_TargetingIgnoreWeaponRacks_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.IgnoreWeaponRacks = !FunkyBaseExtension.Settings.Targeting.IgnoreWeaponRacks;
		}
		private void cb_TargetingIgnoreFloorContainers_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.IgnoreFloorContainers = !FunkyBaseExtension.Settings.Targeting.IgnoreFloorContainers;
		}
		private void cb_TargetingIgnoreNormalChests_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.IgnoreNormalChests = !FunkyBaseExtension.Settings.Targeting.IgnoreNormalChests;
		}
		private void cb_TargetingIgnoreRareChests_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Targeting.IgnoreRareChests = !FunkyBaseExtension.Settings.Targeting.IgnoreRareChests;
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

        private void trackbar_LOS_MaxRange_ValueChanged(object sender, EventArgs e)
        {
            TrackBar slider_sender = (TrackBar)sender;
            int Value = (int)slider_sender.Value;
            FunkyBaseExtension.Settings.LOSMovement.MaximumRange = Value;
            textBox_LOS_MaxRange.Text = Value.ToString();
        }
        private void trackbar_LOS_MinRange_ValueChanged(object sender, EventArgs e)
        {
            TrackBar slider_sender = (TrackBar)sender;
            int Value = (int)slider_sender.Value;
            FunkyBaseExtension.Settings.LOSMovement.MiniumRangeObjects = Value;
            textBox_LOS_MinRange.Text = Value.ToString();
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
			FunkyBaseExtension.Settings.Death.WaitForPotionCooldown = !FunkyBaseExtension.Settings.Death.WaitForPotionCooldown;
		}
		private void cb_DeathSkills_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.Death.WaitForAllSkillsCooldown = !FunkyBaseExtension.Settings.Death.WaitForAllSkillsCooldown;
		}
		private void cb_adventuremode_NavigateMinimapMarkers_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.AdventureMode.NavigatePointsOfInterest = !FunkyBaseExtension.Settings.AdventureMode.NavigatePointsOfInterest;
		}
		private void cb_adventuremode_allowcombatmodification_CheckedChanged(object sender, EventArgs e)
		{
			FunkyBaseExtension.Settings.AdventureMode.AllowCombatModifications = !FunkyBaseExtension.Settings.AdventureMode.AllowCombatModifications;
		}
		private void tb_TieredRiftKey_ValueChanged(object sender, EventArgs e)
		{
			TrackBar slider_sender = (TrackBar)sender;
			int Value = (int)slider_sender.Value;
			FunkyBaseExtension.Settings.AdventureMode.MaximumTieredRiftKeyAllowed = Value;
			textBox_MaxTieredRiftKey.Text = Value.ToString();
		}
		private void cb_GemUpgrading_SuccessRate_CheckedChanged(object sender, EventArgs e)
		{
			//0 == 100%
			//90,80,70,60,30,15,8,4,2,1
			int selectedIndex=comboBox_GemUpgrading_SuccessRate.SelectedIndex;
			switch (selectedIndex)
			{
				case 0:
					FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate = 1;
					break;
				case 1:
					FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate = 0.9;
					break;
				case 2:
					FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate = 0.8;
					break;
				case 3:
					FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate = 0.7;
					break;
				case 4:
					FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate = 0.6;
					break;
				case 5:
					FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate = 0.3;
					break;
				case 6:
					FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate = 0.15;
					break;
				case 7:
					FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate = 0.08;
					break;
				case 8:
					FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate = 0.04;
					break;
				case 9:
					FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate = 0.02;
					break;
				case 10:
					FunkyBaseExtension.Settings.AdventureMode.GemUpgradingMinimumSuccessRate = 0.01;
					break;
			}
		}
		private void radioButton_GemUpgrading_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton senderObj = (RadioButton)sender;
			if (senderObj.Equals(radioButton_GemUpgrading_None))
			{
				FunkyBaseExtension.Settings.AdventureMode.GemUpgradeType = SettingAdventureMode.GemUpgradingType.None;
				groupBox_PriorityGemUpgrading.Enabled = false;
			}
			else if (senderObj.Equals(radioButton_GemUpgrading_Highest))
			{
				FunkyBaseExtension.Settings.AdventureMode.GemUpgradeType = SettingAdventureMode.GemUpgradingType.HighestRank;
				groupBox_PriorityGemUpgrading.Enabled = false;
			}
			else if (senderObj.Equals(radioButton_GemUpgrading_Lowest))
			{
				FunkyBaseExtension.Settings.AdventureMode.GemUpgradeType = SettingAdventureMode.GemUpgradingType.LowestRank;
				groupBox_PriorityGemUpgrading.Enabled = false;
			}
			else if (senderObj.Equals(radioButton_GemUpgrading_Priority))
			{
				FunkyBaseExtension.Settings.AdventureMode.GemUpgradeType = SettingAdventureMode.GemUpgradingType.Priority;
				groupBox_PriorityGemUpgrading.Enabled = true;
			}
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
		private void ClusterUnitExceptionsChanged(object sender, EventArgs e)
		{
			CheckBox cbSender = (CheckBox)sender;
			UnitFlags LogLevelValue = (UnitFlags)Enum.Parse(typeof(UnitFlags), cbSender.Name);

			if (FunkyBaseExtension.Settings.Cluster.UnitExceptions.HasFlag(LogLevelValue))
				FunkyBaseExtension.Settings.Cluster.UnitExceptions &= ~LogLevelValue;
			else
				FunkyBaseExtension.Settings.Cluster.UnitExceptions |= LogLevelValue;
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
            PluginSettings.SerializeToXML(FunkyBaseExtension.Settings, Path.Combine(FolderPaths.sFunkySettingsPath, comboBox_SettingFiles.SelectedItem.ToString()));
		}

		private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
            FunkyBaseExtension.Settings = PluginSettings.DeserializeFromXML();
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
				LBDebug.Controls.Add(new UserControlDebugEntry(FunkyGame.Targeting.DebugString()));
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
				foreach (var item in Hotbar.CurrentBuffs.Values)
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


        private void btn_DumpItemBalanceCache_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            try
            {
                foreach (var entry in CacheIDLookup.dictGameBalanceCache.Values)
                {
                    LBDebug.Controls.Add(new UserControlDebugEntry(entry.ToString()));
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

		private void btn_DumpProfileBehavior_Click(object sender, EventArgs e)
		{
			LBDebug.Controls.Clear();

			try
			{
				LBDebug.Controls.Add(new UserControlDebugEntry(FunkyGame.Profile.DebugString));
			}
			catch
			{
				LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Exception"));
			}

			LBDebug.Focus();
		}

		private void btn_DumpInventory_Click(object sender, EventArgs e)
		{

			LBDebug.Controls.Clear();

			try
			{


				foreach (var o in Equipment.EquippedItems)
				{
					try
					{
						LBDebug.Controls.Add(new UserControlDebugEntry(o.ToString()));
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

        private void btn_DumpBackpack_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            try
            {
                LBDebug.Controls.Add(new UserControlDebugEntry(Backpack.DebugString));
                foreach (var item in Backpack.CacheItemList.Values)
                {
                    LBDebug.Controls.Add(new UserControlDebugEntry(item.ToString()));
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
	        ZetaDia.Actors.Clear();
	        ZetaDia.Actors.Update();
	        foreach (var p in ZetaDia.Actors.ACDList)
	        {
	            try
	            {

	                if (p is ACDItem)
	                {
	                    ACDItem item = (ACDItem) p;
	                    if (item.InventorySlot == InventorySlot.None || item.InventorySlot== InventorySlot.BackpackItems)
	                    {
	                        CacheACDItem cacheItem = new CacheACDItem(item);
	                        LBDebug.Controls.Add(new UserControlDebugEntry(String.Format(cacheItem.ToString())));
	                    }
	                }
	            }
                catch(Exception ex)
	            {
                    LBDebug.Controls.Add(new UserControlDebugEntry(String.Format(ex.Message)));
	            }
	        }
	        
	    }
	

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listBox_GemUpgrading_PriorityList.SelectedIndex>=0)
			{
				string SelectedItemString = (string)listBox_GemUpgrading_PriorityList.Items[listBox_GemUpgrading_PriorityList.SelectedIndex];
				var enumValue = (LegendaryGemTypes)Enum.Parse(typeof(LegendaryGemTypes), SelectedItemString);
				listBox_GemUpgrading_PriorityList.Items.RemoveAt(listBox_GemUpgrading_PriorityList.SelectedIndex);
				listBox_GemUpgrading_UnusedGems.Items.Add(SelectedItemString);
				FunkyBaseExtension.Settings.AdventureMode.GemUpgradePriorityList.Remove(enumValue);
			}
		}

		private void addToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listBox_GemUpgrading_UnusedGems.SelectedIndex>=0)
			{
				string SelectedItemString = (string)listBox_GemUpgrading_UnusedGems.Items[listBox_GemUpgrading_UnusedGems.SelectedIndex];
				var enumValue = (LegendaryGemTypes)Enum.Parse(typeof(LegendaryGemTypes), SelectedItemString);
				listBox_GemUpgrading_UnusedGems.Items.RemoveAt(listBox_GemUpgrading_UnusedGems.SelectedIndex);
				listBox_GemUpgrading_PriorityList.Items.Add(SelectedItemString);
				FunkyBaseExtension.Settings.AdventureMode.GemUpgradePriorityList.Add(enumValue);
			}
		}

		private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listBox_GemUpgrading_PriorityList.SelectedIndex>0)
			{
				int selectedIndex = listBox_GemUpgrading_PriorityList.SelectedIndex;

				string SelectedItemString = (string)listBox_GemUpgrading_PriorityList.Items[listBox_GemUpgrading_PriorityList.SelectedIndex];
				string moveDownItem = (string)listBox_GemUpgrading_PriorityList.Items[selectedIndex-1];
				var enumSelectedValue = (LegendaryGemTypes)Enum.Parse(typeof(LegendaryGemTypes), SelectedItemString);
				var enummoveDownItemValue = (LegendaryGemTypes)Enum.Parse(typeof(LegendaryGemTypes), moveDownItem);

				listBox_GemUpgrading_PriorityList.Items[selectedIndex-1] = SelectedItemString;
				listBox_GemUpgrading_PriorityList.Items[selectedIndex] = moveDownItem;
				listBox_GemUpgrading_PriorityList.SelectedIndex = selectedIndex - 1;

				FunkyBaseExtension.Settings.AdventureMode.GemUpgradePriorityList[selectedIndex - 1] = enumSelectedValue;
				FunkyBaseExtension.Settings.AdventureMode.GemUpgradePriorityList[selectedIndex] = enummoveDownItemValue;
			}
		}

		private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int selectedIndex = listBox_GemUpgrading_PriorityList.SelectedIndex;
			if (listBox_GemUpgrading_PriorityList.SelectedIndex >= 0 && selectedIndex < listBox_GemUpgrading_PriorityList.Items.Count-1)
			{
				

				string SelectedItemString = (string)listBox_GemUpgrading_PriorityList.Items[listBox_GemUpgrading_PriorityList.SelectedIndex];
				string moveUpItem = (string)listBox_GemUpgrading_PriorityList.Items[selectedIndex + 1];
				var enumSelectedValue = (LegendaryGemTypes)Enum.Parse(typeof(LegendaryGemTypes), SelectedItemString);
				var enummoveUpItemValue = (LegendaryGemTypes)Enum.Parse(typeof(LegendaryGemTypes), moveUpItem);

				listBox_GemUpgrading_PriorityList.Items[selectedIndex + 1] = SelectedItemString;
				listBox_GemUpgrading_PriorityList.Items[selectedIndex] = moveUpItem;
				listBox_GemUpgrading_PriorityList.SelectedIndex = selectedIndex + 1;

				FunkyBaseExtension.Settings.AdventureMode.GemUpgradePriorityList[selectedIndex + 1] = enumSelectedValue;
				FunkyBaseExtension.Settings.AdventureMode.GemUpgradePriorityList[selectedIndex] = enummoveUpItemValue;
			}
		}

        private int curIndex = 0;
        private void btn_CharacterControl_Setup_Click(object sender, EventArgs e)
        {
            if (BotMain.IsRunning || BotMain.IsPaused) return;

            if (ZetaDia.IsInGame) return;

            if (!UI.ValidateUIElement(UI.GameMenu.SwitchHeroButton)) return;

            int MaxHeroSlots = ZetaDia.Service.GameAccount.MaxHeroSlots;

            if (curIndex == MaxHeroSlots)
            {
                Logger.DBLog.InfoFormat("Creating file with a total of {0} hero entries", CharacterControl.HeroIndexInfo.Characters.Count);
                BnetCharacterIndexInfo.SerializeToXML(CharacterControl.HeroIndexInfo, BnetCharacterIndexInfo.BnetCharacterInfoSettingsPath);
                curIndex = 0;
                groupBox_BnetControl_Setup.Enabled = false;
                groupBox_BnetControl_AltHero.Enabled = true;
                UpdateBnetHeroComboBox();
                return;
            }

            Logger.DBLog.InfoFormat("Switching to index {0}", curIndex);
            ZetaDia.Service.GameAccount.SwitchHero(curIndex);
            Thread.Sleep(1500);

            //Clear Cache -- and get hero info
            ZetaDia.Memory.ClearCache();
            CharacterControl.HeroInfo hinfo = new CharacterControl.HeroInfo(ZetaDia.Service.Hero);
            BnetCharacterIndexInfo.BnetCharacterEntry entry = new BnetCharacterIndexInfo.BnetCharacterEntry(curIndex,hinfo.Name, hinfo.Class);
            //Add entry
            CharacterControl.HeroIndexInfo.Characters.Add(entry);
            Logger.DBLog.InfoFormat("Recording hero info for index {0}\r\n{1}", curIndex, hinfo.ToString());

            if (curIndex == 0)
            {
                btn_CharacterControl_Setup.Text = "Next";
            }

            curIndex++;

        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedTab.Text == "Bnet Control")
            {
                if (CharacterControl.HeroIndexInfo.Characters.Count==0)
                {
                    groupBox_BnetControl_Setup.Enabled = true;
                }
                else
                {
                    groupBox_BnetControl_Setup.Enabled = false;
                    groupBox_BnetControl_AltHero.Enabled = true;
                    UpdateBnetHeroComboBox();
                }
            }
        }

	    private void UpdateBnetHeroComboBox()
	    {
	        comboBox_BnetControl_Heros.Items.Clear();
	        int herocount = CharacterControl.HeroIndexInfo.Characters.Count;
	        if (herocount > 0)
	        {
	            comboBox_BnetControl_Heros.SelectedIndexChanged -= comboBox_BnetControl_Heros_SelectedIndexChanged;
                foreach (var entry in CharacterControl.HeroIndexInfo.Characters)
                {
                    comboBox_BnetControl_Heros.Items.Add(String.Format("{0} -- {1} [{2}]",
                                                        entry.Index, entry.Name, entry.Class));
                }

	            if (FunkyBaseExtension.Settings.General.AltHeroIndex != -1 &&
	                FunkyBaseExtension.Settings.General.AltHeroIndex <= herocount)
	            {
	                comboBox_BnetControl_Heros.SelectedIndex = FunkyBaseExtension.Settings.General.AltHeroIndex;
	            }
	            else
	                comboBox_BnetControl_Heros.Text = String.Empty;

                comboBox_BnetControl_Heros.SelectedIndexChanged += comboBox_BnetControl_Heros_SelectedIndexChanged;
	        }
	    }

        private void comboBox_BnetControl_Heros_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_BnetControl_Heros.SelectedIndex >= 0)
            {
                FunkyBaseExtension.Settings.General.AltHeroIndex = comboBox_BnetControl_Heros.SelectedIndex;
            }
        }

        private void btn_BnetControl_TestSwitch_Click(object sender, EventArgs e)
        {
            if (BotMain.IsRunning || BotMain.IsPaused) return;

            if (ZetaDia.IsInGame) return;

            if (!UI.ValidateUIElement(UI.GameMenu.SwitchHeroButton)) return;

            if (comboBox_BnetControl_Heros.SelectedIndex >= 0)
            {
                int index = comboBox_BnetControl_Heros.SelectedIndex;
                Logger.DBLog.InfoFormat("Switching to index {0}", index);
                ZetaDia.Service.GameAccount.SwitchHero(index);
            }
        }

        private void btn_BnetControl_ResetIndexes_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DialogResult result = MessageBox.Show("Please confirm you wish to delete the current hero indexes file", "Delete Current Hero Indexes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                File.Delete(BnetCharacterIndexInfo.BnetCharacterInfoSettingsPath);
                CharacterControl.HeroIndexInfo.Characters.Clear();
                comboBox_BnetControl_Heros.Items.Clear();
                groupBox_BnetControl_Setup.Enabled = true;
                groupBox_BnetControl_AltHero.Enabled = false;
            }
        }

	    private void comboBox_SettingFiles_SelectedIndexChanged(object sender, EventArgs e)
	    {
	        if (comboBox_SettingFiles.SelectedIndex >= 0)
	        {
	            var currentActorClass = FunkyGame.CurrentActorClass;
                FunkyGame._CurrentActorClass = ActorClass.Invalid;

	            bool shouldrefreshclass = FunkyGame.ShouldRefreshClass;
                if (shouldrefreshclass)
	            {
	                FunkyGame.ShouldRefreshClass = false;
	            }

	            FunkyBaseExtension.Settings=PluginSettings.DeserializeFromXML(Path.Combine(FolderPaths.sFunkySettingsPath,comboBox_SettingFiles.SelectedItem.ToString()));
	            initalizeControls();
	            Text = comboBox_SettingFiles.SelectedItem.ToString();

	            FunkyGame._CurrentActorClass = FunkyGame.CurrentActorClass;
                FunkyGame.ShouldRefreshClass = shouldrefreshclass;
	        }
	    }

        private void GameDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_BnetControl_CustomDifficulty.SelectedIndex >= 0)
            {
                FunkyBaseExtension.Settings.General.CustomDifficulty =
                    comboBox_BnetControl_CustomDifficulty.Items[comboBox_BnetControl_CustomDifficulty.SelectedIndex]
                        .ToString();
            }
        }

        private void button_saveSettings_Click(object sender, EventArgs e)
        {
            PluginSettings.SerializeToXML(FunkyBaseExtension.Settings, Path.Combine(FolderPaths.sFunkySettingsPath, comboBox_SettingFiles.SelectedItem.ToString()));
        }

        


	}
}
