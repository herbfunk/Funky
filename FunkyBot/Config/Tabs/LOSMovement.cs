using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FunkyBot.Avoidances;
using FunkyBot.Settings;
using FunkyBot.Cache;

namespace FunkyBot
{
    internal partial class FunkyWindow : Window
    {
        #region EventHandling
        private void LineOfSightBehaviorChecked(object sender, EventArgs e)
        {
            Bot.Settings.LOSMovement.EnableLOSMovementBehavior = !Bot.Settings.LOSMovement.EnableLOSMovementBehavior;
        }
        private void LineOfSightBehaviorAllowTreasureGoblinChecked(object sender, EventArgs e)
        {
            Bot.Settings.LOSMovement.AllowTreasureGoblin = !Bot.Settings.LOSMovement.AllowTreasureGoblin;
        }
        private void LineOfSightBehaviorAllowRareElitesChecked(object sender, EventArgs e)
        {
            Bot.Settings.LOSMovement.AllowRareElites = !Bot.Settings.LOSMovement.AllowRareElites;
        }
        private void LineOfSightBehaviorAllowUniqueBossChecked(object sender, EventArgs e)
        {
            Bot.Settings.LOSMovement.AllowUniqueBoss = !Bot.Settings.LOSMovement.AllowUniqueBoss;
        }
        private void LineOfSightBehaviorAllowRangedChecked(object sender, EventArgs e)
        {
            Bot.Settings.LOSMovement.AllowRanged = !Bot.Settings.LOSMovement.AllowRanged;
        }
        private void LineOfSightBehaviorAllowSucideBomberChecked(object sender, EventArgs e)
        {
            Bot.Settings.LOSMovement.AllowSucideBomber = !Bot.Settings.LOSMovement.AllowSucideBomber;
        }
        private void LineOfSightBehaviorAllowSpawnerUnitsChecked(object sender, EventArgs e)
        {
            Bot.Settings.LOSMovement.AllowSpawnerUnits = !Bot.Settings.LOSMovement.AllowSpawnerUnits;
        }
        private void LineOfSightBehaviorAllowRareLootContainerChecked(object sender, EventArgs e)
        {
            Bot.Settings.LOSMovement.AllowRareLootContainer = !Bot.Settings.LOSMovement.AllowRareLootContainer;
        }

        #endregion

        internal void InitLOSMovementControls()
        {
            TabItem LOSTabItem = new TabItem();
            LOSTabItem.Header = "LOS";
            tcTargeting.Items.Add(LOSTabItem);
            ListBox LBTargetLOS = new ListBox();

            StackPanel SPTargetLOS = new StackPanel
            {
                Background = System.Windows.Media.Brushes.DimGray,
                Width = tcTargeting.Width,
            };


            TextBlock LOSMovement_Text_Header = new TextBlock
            {
                Text = "Line of Sight Behavior Options",
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.GhostWhite,
                Background = System.Windows.Media.Brushes.DarkGreen,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment= System.Windows.HorizontalAlignment.Stretch,
            };
            SPTargetLOS.Children.Add(LOSMovement_Text_Header);

            #region LOS_EnableBehavior
            CheckBox cbLOSEnableBehavior = new CheckBox
            {
                Content = "Enable Line of Sight Behavior",
                Height = 30,
                IsChecked = (Bot.Settings.LOSMovement.EnableLOSMovementBehavior)
            };
            cbLOSEnableBehavior.Checked += LineOfSightBehaviorChecked;
            cbLOSEnableBehavior.Unchecked += LineOfSightBehaviorChecked;
            SPTargetLOS.Children.Add(cbLOSEnableBehavior);
            #endregion

            #region LOS_AllowTreasureGoblin
            CheckBox cbLOSAllowTreasureGoblin = new CheckBox
            {
                Content = "Allow Treasure Goblin",
                Height = 30,
                IsChecked = (Bot.Settings.LOSMovement.AllowTreasureGoblin)
            };
            cbLOSAllowTreasureGoblin.Checked += LineOfSightBehaviorAllowTreasureGoblinChecked;
            cbLOSAllowTreasureGoblin.Unchecked += LineOfSightBehaviorAllowTreasureGoblinChecked;
            SPTargetLOS.Children.Add(cbLOSAllowTreasureGoblin);
            #endregion

            #region LOS_AllowRareElites
            CheckBox cbLOSAllowRareElites = new CheckBox
            {
                Content = "Allow Rare / Elites",
                Height = 30,
                IsChecked = (Bot.Settings.LOSMovement.AllowRareElites)
            };
            cbLOSAllowRareElites.Checked += LineOfSightBehaviorAllowRareElitesChecked;
            cbLOSAllowRareElites.Unchecked += LineOfSightBehaviorAllowRareElitesChecked;
            SPTargetLOS.Children.Add(cbLOSAllowRareElites);
            #endregion

            #region LOS_AllowUniqueBoss
            CheckBox cbLOSAllowUniqueBoss = new CheckBox
            {
                Content = "Allow Bosses / Uniques",
                Height = 30,
                IsChecked = (Bot.Settings.LOSMovement.AllowUniqueBoss)
            };
            cbLOSAllowUniqueBoss.Checked += LineOfSightBehaviorAllowUniqueBossChecked;
            cbLOSAllowUniqueBoss.Unchecked += LineOfSightBehaviorAllowUniqueBossChecked;
            SPTargetLOS.Children.Add(cbLOSAllowUniqueBoss);
            #endregion

            #region LOS_AllowRanged
            CheckBox cbLOSAllowRanged = new CheckBox
            {
                Content = "Allow Ranged Units",
                Height = 30,
                IsChecked = (Bot.Settings.LOSMovement.AllowRanged)
            };
            cbLOSAllowRanged.Checked += LineOfSightBehaviorAllowRangedChecked;
            cbLOSAllowRanged.Unchecked += LineOfSightBehaviorAllowRangedChecked;
            SPTargetLOS.Children.Add(cbLOSAllowRanged);
            #endregion

            #region LOS_AllowSucideBomber
            CheckBox cbLOSAllowSucideBomber = new CheckBox
            {
                Content = "Allow Sucide Bombers",
                Height = 30,
                IsChecked = (Bot.Settings.LOSMovement.AllowSucideBomber)
            };
            cbLOSAllowSucideBomber.Checked += LineOfSightBehaviorAllowSucideBomberChecked;
            cbLOSAllowSucideBomber.Unchecked += LineOfSightBehaviorAllowSucideBomberChecked;
            SPTargetLOS.Children.Add(cbLOSAllowSucideBomber);
            #endregion

            #region LOS_AllowSpawnerUnits
            CheckBox cbLOSAllowSpawnerUnits = new CheckBox
            {
                Content = "Allow Spawner Units",
                Height = 30,
                IsChecked = (Bot.Settings.LOSMovement.AllowSpawnerUnits)
            };
            cbLOSAllowSpawnerUnits.Checked += LineOfSightBehaviorAllowSpawnerUnitsChecked;
            cbLOSAllowSpawnerUnits.Unchecked += LineOfSightBehaviorAllowSpawnerUnitsChecked;
            SPTargetLOS.Children.Add(cbLOSAllowSpawnerUnits);
            #endregion

            #region LOS_AllowRareLootContainer
            CheckBox cbLOSAllowRareLootContainer = new CheckBox
            {
                Content = "Allow Rare Chest Containers",
                Height = 30,
                IsChecked = (Bot.Settings.LOSMovement.AllowRareLootContainer)
            };
            cbLOSAllowRareLootContainer.Checked += LineOfSightBehaviorAllowRareLootContainerChecked;
            cbLOSAllowRareLootContainer.Unchecked += LineOfSightBehaviorAllowRareLootContainerChecked;
            SPTargetLOS.Children.Add(cbLOSAllowRareLootContainer);
            #endregion

            LBTargetLOS.Items.Add(SPTargetLOS);
            LOSTabItem.Content = LBTargetLOS;
        }
    }
}
