using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using FunkyBot.Settings;
using System;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListBox = System.Windows.Controls.ListBox;
using Orientation = System.Windows.Controls.Orientation;
using TextBox = System.Windows.Controls.TextBox;
using ToolTip = System.Windows.Controls.ToolTip;

namespace FunkyBot
{
    internal partial class FunkyWindow : Window
    {
        private CheckBox CBAttemptFleeingBehavior;
        private Slider sliderFleeMonsterDistance, sliderFleeHealthPercent;
        private TextBox TBFleemonsterDistance, TBFleeMinimumHealth;
        private StackPanel spFleeingOptions, SPFleeing;

        #region EventHandling
        private void FleeUnitElectrifiedChecked(object sender, EventArgs e)
        {
            Bot.Settings.Fleeing.FleeUnitElectrified = !Bot.Settings.Fleeing.FleeUnitElectrified;
        }
        private void FleeUnitRareEliteChecked(object sender, EventArgs e)
        {
            Bot.Settings.Fleeing.FleeUnitRareElite = !Bot.Settings.Fleeing.FleeUnitRareElite;
        }
        private void FleeUnitNormalChecked(object sender, EventArgs e)
        {
            Bot.Settings.Fleeing.FleeUnitNormal = !Bot.Settings.Fleeing.FleeUnitNormal;
        }
        private void FleeUnitAboveAverageHitPointsChecked(object sender, EventArgs e)
        {
            Bot.Settings.Fleeing.FleeUnitAboveAverageHitPoints = !Bot.Settings.Fleeing.FleeUnitAboveAverageHitPoints;
        }
        private void FleeUnitIgnoreFastChecked(object sender, EventArgs e)
        {
            Bot.Settings.Fleeing.FleeUnitIgnoreFast = !Bot.Settings.Fleeing.FleeUnitIgnoreFast;
        }
        private void FleeUnitIgnoreSucideBomberChecked(object sender, EventArgs e)
        {
            Bot.Settings.Fleeing.FleeUnitIgnoreSucideBomber = !Bot.Settings.Fleeing.FleeUnitIgnoreSucideBomber;
        }
        private void FleeUnitIgnoreRangedChecked(object sender, EventArgs e)
        {
            Bot.Settings.Fleeing.FleeUnitIgnoreRanged = !Bot.Settings.Fleeing.FleeUnitIgnoreRanged;
        }
        private void FleeingAttemptMovementChecked(object sender, EventArgs e)
        {
            Bot.Settings.Fleeing.EnableFleeingBehavior = !Bot.Settings.Fleeing.EnableFleeingBehavior;
            bool enabled = Bot.Settings.Fleeing.EnableFleeingBehavior;
            spFleeingOptions.IsEnabled = enabled;
        }
        private void FleeMonsterDistanceSliderChanged(object sender, EventArgs e)
        {
            Slider slider_sender = (Slider)sender;
            int Value = (int)slider_sender.Value;
            Bot.Settings.Fleeing.FleeMaxMonsterDistance = Value;
            TBFleemonsterDistance.Text = Value.ToString();
        }
        private void FleeMinimumHealthSliderChanged(object sender, EventArgs e)
        {
            Slider slider_sender = (Slider)sender;
            double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
            Bot.Settings.Fleeing.FleeBotMinimumHealthPercent = Value;
            TBFleeMinimumHealth.Text = Value.ToString();
        }
        private void FleeingLoadXMLClicked(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog
            {
                InitialDirectory = Path.Combine(FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
                RestoreDirectory = false,
                Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "Fleeing Template",
            };
            DialogResult OFD_Result = OFD.ShowDialog();

            if (OFD_Result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    //;
                    SettingFleeing newSettings = SettingFleeing.DeserializeFromXML(OFD.FileName);
                    Bot.Settings.Fleeing = newSettings;

                    funkyConfigWindow.Close();
                }
                catch
                {

                }
            }
        }
        #endregion

        internal void InitFleeingControls()
        {
            TabItem FleeingTabItem = new TabItem
            {
                Header = "Fleeing",
            };
            FleeingTabItem.Header = "Fleeing";
            CombatTabControl.Items.Add(FleeingTabItem);
            ListBox LBcharacterFleeingTabItem = new ListBox();

            #region Fleeing
            Button BtnFleeingLoadTemplate = new Button
            {
                Content = "Load Setup",
                Background = Brushes.OrangeRed,
                Foreground = Brushes.GhostWhite,
                FontStyle = FontStyles.Italic,
                FontSize = 12,

                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 75,
                Height = 30,

                Margin = new Thickness(Margin.Left, Margin.Top + 5, Margin.Right, Margin.Bottom + 5),
            };
            BtnFleeingLoadTemplate.Click += FleeingLoadXMLClicked;

            ToolTip TTFleeInfo = new ToolTip
            {
                Content = "Trys to move away from any monsters that are within the distance set",
            };
            SPFleeing = new StackPanel
            {
                Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
                Background = Brushes.DimGray,
            };
            TextBlock Flee_Text_Header = new TextBlock
            {
                Text = "Fleeing",
                FontSize = 12,
                Background = Brushes.SeaGreen,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ToolTip = TTFleeInfo,
            };

            CBAttemptFleeingBehavior = new CheckBox
            {
                Content = "Enable Fleeing",
                IsChecked = Bot.Settings.Fleeing.EnableFleeingBehavior,
                Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
            };
            CBAttemptFleeingBehavior.Checked += FleeingAttemptMovementChecked;
            CBAttemptFleeingBehavior.Unchecked += FleeingAttemptMovementChecked;

            spFleeingOptions = new StackPanel
            {
                Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
                IsEnabled = Bot.Settings.Fleeing.EnableFleeingBehavior,
            };
            #region Fleeing Monster Distance
            TextBlock Flee_MonsterDistance_Label = new TextBlock
            {
                Text = "Maximum Monster Distance",
                FontSize = 13,
                Foreground = Brushes.GhostWhite,
                //Background = System.Windows.Media.Brushes.Crimson,
                TextAlignment = TextAlignment.Left,
            };

            sliderFleeMonsterDistance = new Slider
            {
                Width = 100,
                Maximum = 20,
                Minimum = 0,
                TickFrequency = 5,
                LargeChange = 5,
                SmallChange = 1,
                Value = Bot.Settings.Fleeing.FleeMaxMonsterDistance,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            sliderFleeMonsterDistance.ValueChanged += FleeMonsterDistanceSliderChanged;
            TBFleemonsterDistance = new TextBox
            {
                Text = Bot.Settings.Fleeing.FleeMaxMonsterDistance.ToString(),
                IsReadOnly = true,
            };

            ToolTip TTFleeMonsterDistance = new ToolTip
            {
                Content = "The maximum distance allowed for units that trigger fleeing",
            };
            StackPanel FleeMonsterDistanceStackPanel = new StackPanel
            {
                Width = 600,
                Height = 20,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 6),
                ToolTip = TTFleeMonsterDistance,
            };

            FleeMonsterDistanceStackPanel.Children.Add(sliderFleeMonsterDistance);
            FleeMonsterDistanceStackPanel.Children.Add(TBFleemonsterDistance);
            #endregion

            #region Fleeing Minimum Health Percent
            TextBlock Flee_HealthPercent_Label = new TextBlock
            {
                Text = "Bot Min Health Percent",
                FontSize = 13,
                Foreground = Brushes.GhostWhite,
                //Background = System.Windows.Media.Brushes.Crimson,
                TextAlignment = TextAlignment.Left,
            };


            sliderFleeHealthPercent = new Slider
            {
                Width = 100,
                Maximum = 1,
                Minimum = 0,
                TickFrequency = 0.25,
                LargeChange = 0.1,
                SmallChange = 0.05,
                Value = Bot.Settings.Fleeing.FleeBotMinimumHealthPercent,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            sliderFleeHealthPercent.ValueChanged += FleeMinimumHealthSliderChanged;
            TBFleeMinimumHealth = new TextBox
            {
                Text = Bot.Settings.Fleeing.FleeBotMinimumHealthPercent.ToString(),
                IsReadOnly = true,
            };
            ToolTip TTFleeMinimumHealth = new ToolTip
            {
                Content = "Minimum Health Percent before Fleeing is Allowed",
            };
            StackPanel FleeMinimumHealthtackPanel = new StackPanel
            {
                Width = 600,
                Height = 20,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 6),
                ToolTip = TTFleeMinimumHealth,
            };
            FleeMinimumHealthtackPanel.Children.Add(sliderFleeHealthPercent);
            FleeMinimumHealthtackPanel.Children.Add(TBFleeMinimumHealth);
            #endregion


            SPFleeing.Children.Add(Flee_Text_Header);
            SPFleeing.Children.Add(CBAttemptFleeingBehavior);

            spFleeingOptions.Children.Add(Flee_MonsterDistance_Label);
            spFleeingOptions.Children.Add(FleeMonsterDistanceStackPanel);
            spFleeingOptions.Children.Add(Flee_HealthPercent_Label);
            spFleeingOptions.Children.Add(FleeMinimumHealthtackPanel);

            SPFleeing.Children.Add(spFleeingOptions);

            #endregion

            #region Flee Unit Triggers

            StackPanel SPFleeUnitOptions = new StackPanel();
            ToolTip TTFleeUnitInfo = new ToolTip
            {
                Content = "This determines what units should trigger fleeing",
            };
            TextBlock FleeUnit_Text_Header = new TextBlock
            {
                Text = "Fleeing Unit Triggers",
                FontSize = 12,
                Background = Brushes.SeaGreen,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ToolTip = TTFleeUnitInfo,
            };
            SPFleeUnitOptions.Children.Add(FleeUnit_Text_Header);

            #region Electrified

            CheckBox CBFleeUnitElectrified = new CheckBox
            {
                Content = "Electrified",
                IsChecked = Bot.Settings.Fleeing.FleeUnitElectrified,

            };
            CBFleeUnitElectrified.Checked += FleeUnitElectrifiedChecked;
            CBFleeUnitElectrified.Unchecked += FleeUnitElectrifiedChecked;
            SPFleeUnitOptions.Children.Add(CBFleeUnitElectrified);
            #endregion
            #region Rare/Elites

            CheckBox CBFleeUnitRareElite = new CheckBox
            {
                Content = "Rare/Elite",
                IsChecked = Bot.Settings.Fleeing.FleeUnitRareElite,

            };
            CBFleeUnitRareElite.Checked += FleeUnitRareEliteChecked;
            CBFleeUnitRareElite.Unchecked += FleeUnitRareEliteChecked;
            SPFleeUnitOptions.Children.Add(CBFleeUnitRareElite);
            #endregion
            #region Normal

            CheckBox CBFleeUnitNormal = new CheckBox
            {
                Content = "Normal",
                IsChecked = Bot.Settings.Fleeing.FleeUnitNormal,

            };
            CBFleeUnitNormal.Checked += FleeUnitNormalChecked;
            CBFleeUnitNormal.Unchecked += FleeUnitNormalChecked;
            SPFleeUnitOptions.Children.Add(CBFleeUnitNormal);
            #endregion

            #region Above Avg Hitpoints

            CheckBox CBFleeUnitAboveAverageHitPoints = new CheckBox
            {
                Content = "Above Average Hitpoints",
                IsChecked = Bot.Settings.Fleeing.FleeUnitAboveAverageHitPoints,

            };
            CBFleeUnitAboveAverageHitPoints.Checked += FleeUnitAboveAverageHitPointsChecked;
            CBFleeUnitAboveAverageHitPoints.Unchecked += FleeUnitAboveAverageHitPointsChecked;
            SPFleeUnitOptions.Children.Add(CBFleeUnitAboveAverageHitPoints);
            #endregion
            #region Fast

            CheckBox CBFleeUnitIgnoreFast = new CheckBox
            {
                Content = "Ignore Fast",
                IsChecked = Bot.Settings.Fleeing.FleeUnitIgnoreFast,

            };
            CBFleeUnitIgnoreFast.Checked += FleeUnitIgnoreFastChecked;
            CBFleeUnitIgnoreFast.Unchecked += FleeUnitIgnoreFastChecked;
            SPFleeUnitOptions.Children.Add(CBFleeUnitIgnoreFast);
            #endregion
            #region SucideBomber

            CheckBox CBFleeUnitIgnoreSucideBomber = new CheckBox
            {
                Content = "Ignore Sucide Bomber",
                IsChecked = Bot.Settings.Fleeing.FleeUnitIgnoreSucideBomber,

            };
            CBFleeUnitIgnoreSucideBomber.Checked += FleeUnitIgnoreSucideBomberChecked;
            CBFleeUnitIgnoreSucideBomber.Unchecked += FleeUnitIgnoreSucideBomberChecked;
            SPFleeUnitOptions.Children.Add(CBFleeUnitIgnoreSucideBomber);
            #endregion
            #region Ranged
            CheckBox CBFleeUnitIgnoreRanged = new CheckBox
            {
                Content = "Ignore Ranged",
                IsChecked = Bot.Settings.Fleeing.FleeUnitIgnoreRanged,

            };
            CBFleeUnitIgnoreRanged.Checked += FleeUnitIgnoreRangedChecked;
            CBFleeUnitIgnoreRanged.Unchecked += FleeUnitIgnoreRangedChecked;
            SPFleeUnitOptions.Children.Add(CBFleeUnitIgnoreRanged);
            #endregion

            SPFleeing.Children.Add(SPFleeUnitOptions);
            
            #endregion

            SPFleeing.Children.Add(BtnFleeingLoadTemplate);
            LBcharacterFleeingTabItem.Items.Add(SPFleeing);
            FleeingTabItem.Content = LBcharacterFleeingTabItem;
        }
    }
}