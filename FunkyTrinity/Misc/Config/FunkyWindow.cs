using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Markup;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using Zeta;
using Zeta.Common;

namespace FunkyTrinity
{
    public partial class Funky
    {
        private static Zeta.Internals.Actors.ActorClass ActorClass;
        private static string CurrentAccountName;
        private static string CurrentHeroName;

        private static void UpdateCurrentAccountDetails()
        {
            ActorClass = Zeta.ZetaDia.Service.CurrentHero.Class;
            CurrentAccountName = Zeta.ZetaDia.Service.CurrentHero.BattleTagName;
            CurrentHeroName = Zeta.ZetaDia.Service.CurrentHero.Name;
        }

        private static void buttonFunkySettingDB_Click(object sender, RoutedEventArgs e)
        {
            ActorClass = Zeta.ZetaDia.Service.CurrentHero.Class;
            CurrentAccountName = Zeta.ZetaDia.Service.CurrentHero.BattleTagName;
            CurrentHeroName = Zeta.ZetaDia.Service.CurrentHero.Name;

            string settingsFolder = FolderPaths.sDemonBuddyPath + @"\Settings\FunkyTrinity\" + CurrentAccountName;
            if(!Directory.Exists(settingsFolder))
                Directory.CreateDirectory(settingsFolder);
            try
            {
                funkyConfigWindow = new FunkyWindow();
                funkyConfigWindow.Show();
            }
            catch(Exception ex)
            {
                Logging.WriteVerbose("Failure to initilize Funky Setting Window! \r\n {0} \r\n {1} \r\n {2}", ex.Message, ex.Source, ex.StackTrace);
            }
        }

        internal static FunkyWindow funkyConfigWindow;

        [System.Runtime.InteropServices.ComVisible(false)]
        public partial class FunkyWindow : Window
        {
            private ListBox lbGeneralContent;

            private CheckBox OOCIdentifyItems;
            private TextBox OOCIdentfyItemsMinCount;

            private CheckBox BuyPotionsDuringTownRunCB;
            private CheckBox EnableWaitAfterContainersCB;
            private CheckBox UseExtendedRangeRepChestCB;

            private CheckBox ItemRules;
            private CheckBox ItemRulesPickup;
            private Button ItemRulesReload;

            private CheckBox ItemRuleUseItemIDs;
            private CheckBox ItemRuleDebug;
            private ComboBox ItemRuleLogKeep;
            private ComboBox ItemRuleLogPickup;
            private ComboBox ItemRuleType;

            private RadioButton ItemRuleGilesScoring, ItemRuleDBScoring;

            private CheckBox CoffeeBreaks;
            private TextBox tbMinBreakTime, tbMaxBreakTime;

            private Button OpenPluginFolder;

            private TextBox[] TBavoidanceRadius;
            private TextBox[] TBavoidanceHealth;

            private TextBox[] TBMinimumWeaponLevel;
            private TextBox[] TBMinimumArmorLevel;
            private TextBox[] TBMinimumJeweleryLevel;
            private CheckBox[] CBGems;
            private ComboBox CBGemQualityLevel;

            private TextBox TBBreakTimeHour, TBKiteDistance, TBGlobeHealth, TBPotionHealth, TBContainerRange, TBNonEliteRange, TBDestructibleRange, TBAfterCombatDelay, TBiDHVaultMovementDelay, TBShrineRange, TBEliteRange, TBExtendedCombatRange, TBGoldRange, TBMinLegendaryLevel, TBMaxHealthPots, TBMinGoldPile, TBMiscItemLevel, TBGilesWeaponScore, TBGilesArmorScore, TBGilesJeweleryScore, TBClusterDistance, TBClusterMinUnitCount, TBItemRange, TBGoblinRange, TBGoblinMinRange, TBClusterLowHPValue;
            private TextBox[] TBKiteTimeLimits;
            private TextBox[] TBAvoidanceTimeLimits;

            private ListBox LBDebug;


            public FunkyWindow()
            {
                LoadFunkyConfiguration();

                this.Owner = Demonbuddy.App.Current.MainWindow;
                this.Title = "Funky Window";
                this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                this.ResizeMode = System.Windows.ResizeMode.CanMinimize;
                this.Background = System.Windows.Media.Brushes.Black;
                this.Foreground = System.Windows.Media.Brushes.PaleGoldenrod;
                //this.Width=600;
                //this.Height=600;

                ListBox LBWindowContent = new ListBox
                {
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                };

                StackPanel StackPanelTopWindow = new StackPanel
                {
                    Height = 40,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    FlowDirection = System.Windows.FlowDirection.LeftToRight,
                    Orientation = Orientation.Horizontal,
                };
                Button OpenPluginSettings = new Button
                {
                    Content = "Open Plugin Settings",
                    Width = 150,
                    Height = 25,
                };
                OpenPluginSettings.Click += OpenPluginSettings_Click;
                StackPanelTopWindow.Children.Add(OpenPluginSettings);


                OpenPluginFolder = new Button
                {
                    Content = "Open Trinity Folder",
                    Width = 150,
                    Height = 25,
                };
                OpenPluginFolder.Click += OpenPluginFolder_Click;
                StackPanelTopWindow.Children.Add(OpenPluginFolder);

                //LBWindowContent.Items.Add(StackPanelTopWindow);

                TabControl tabControl1 = new TabControl
                {
                    Width = 600,
                    Height = 600,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                    VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch,
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                    FontSize = 12,
                };
                LBWindowContent.Items.Add(tabControl1);

                #region Combat
                //Character
                TabItem CombatSettingsTabItem = new TabItem();
                CombatSettingsTabItem.Header = "Combat";

                tabControl1.Items.Add(CombatSettingsTabItem);
                TabControl CombatTabControl = new TabControl
                {
                    Width = 600,
                    Height = 600,

                };

                #region General
                //Combat
                TabItem CombatGeneralTabItem = new TabItem();
                CombatGeneralTabItem.Header = "General";
                CombatTabControl.Items.Add(CombatGeneralTabItem);
                ListBox CombatGeneralContentListBox = new ListBox();
                #region Avoidances

                StackPanel AvoidanceOptionsStackPanel = new StackPanel
                {
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
                };

                TextBlock Avoidance_Text_Header = new TextBlock
                {
                    Text = "Avoidances",
                    FontSize = 12,
                    Background = System.Windows.Media.Brushes.MediumSeaGreen,
                    TextAlignment = TextAlignment.Center,
                };

                #region AvoidanceCheckboxes

                StackPanel AvoidanceCheckBoxesPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                };

                CheckBox CBAttemptAvoidanceMovements = new CheckBox
                {
                    Content = "Enable Avoidance",
                    IsChecked = SettingsFunky.AttemptAvoidanceMovements,

                };
                CBAttemptAvoidanceMovements.Checked += AvoidanceAttemptMovementChecked;
                CBAttemptAvoidanceMovements.Unchecked += AvoidanceAttemptMovementChecked;

                CheckBox CBAdvancedProjectileTesting = new CheckBox
                {
                    Content = "Use Advanced Avoidance Projectile Test",
                    IsChecked = SettingsFunky.UseAdvancedProjectileTesting,
                };
                CBAdvancedProjectileTesting.Checked += UseAdvancedProjectileTestingChecked;
                CBAdvancedProjectileTesting.Unchecked += UseAdvancedProjectileTestingChecked;
                AvoidanceCheckBoxesPanel.Children.Add(CBAttemptAvoidanceMovements);
                AvoidanceCheckBoxesPanel.Children.Add(CBAdvancedProjectileTesting);
                #endregion;


                #region AvoidanceRetryTextInfo
                StackPanel AvoidDelayStackPanel = new StackPanel();
                TextBlock Avoid_Delay_Text = new TextBlock
                {
                    Text = "Delay invtervals",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Left,
                };
                TextBlock Avoid_DelayInfo_Text = new TextBlock
                {
                    Text = "Minimum is delay used if failed, Maximum is delay after successful searching",
                    FontSize = 9,
                    FontStyle = FontStyles.Italic,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Left,
                };
                AvoidDelayStackPanel.Children.Add(Avoid_Delay_Text);
                AvoidDelayStackPanel.Children.Add(Avoid_DelayInfo_Text);
                #endregion


                #region AvoidanceRetry


                TextBlock Avoid_Retry_Min_Text = new TextBlock
                {
                    Text = "Minimum",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Center,
                };
                Slider sliderAvoidMinimumRetry = new Slider
                {
                    Width = 120,
                    Maximum = 10000,
                    Minimum = 0,
                    TickFrequency = 500,
                    LargeChange = 1000,
                    SmallChange = 50,
                    Value = SettingsFunky.AvoidanceRecheckMinimumRate,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    Margin = new Thickness(6),
                };
                sliderAvoidMinimumRetry.ValueChanged += AvoidanceMinimumRetrySliderChanged;
                TBAvoidanceTimeLimits = new TextBox[2];
                TBAvoidanceTimeLimits[0] = new TextBox
                {
                    Text = SettingsFunky.AvoidanceRecheckMinimumRate.ToString(),
                    IsReadOnly = true,
                };

                TextBlock Avoid_Retry_Max_Text = new TextBlock
                {
                    Text = "Maximum",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Center,
                };

                Slider sliderAvoidMaximumRetry = new Slider
                {
                    Width = 120,
                    Maximum = 10000,
                    Minimum = 0,
                    TickFrequency = 500,
                    LargeChange = 1000,
                    SmallChange = 50,
                    Value = SettingsFunky.AvoidanceRecheckMaximumRate,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    Margin = new Thickness(6),
                };
                sliderAvoidMaximumRetry.ValueChanged += AvoidanceMaximumRetrySliderChanged;
                TBAvoidanceTimeLimits[1] = new TextBox
                {
                    Text = SettingsFunky.AvoidanceRecheckMaximumRate.ToString(),
                    IsReadOnly = true,
                };

                StackPanel AvoidRetryTimeStackPanel = new StackPanel
                {
                    Margin = new Thickness(Margin.Left, Margin.Top + 5, Margin.Right, Margin.Bottom + 5),
                    Orientation = Orientation.Horizontal,
                };
                AvoidRetryTimeStackPanel.Children.Add(Avoid_Retry_Min_Text);
                AvoidRetryTimeStackPanel.Children.Add(sliderAvoidMinimumRetry);
                AvoidRetryTimeStackPanel.Children.Add(TBAvoidanceTimeLimits[0]);
                AvoidRetryTimeStackPanel.Children.Add(Avoid_Retry_Max_Text);
                AvoidRetryTimeStackPanel.Children.Add(sliderAvoidMaximumRetry);
                AvoidRetryTimeStackPanel.Children.Add(TBAvoidanceTimeLimits[1]);
                //LBcharacterCombat.Items.Add(AvoidRetryTimeStackPanel);
                #endregion

                //

                AvoidanceOptionsStackPanel.Children.Add(Avoidance_Text_Header);
                AvoidanceOptionsStackPanel.Children.Add(AvoidanceCheckBoxesPanel);
                AvoidanceOptionsStackPanel.Children.Add(AvoidDelayStackPanel);
                AvoidanceOptionsStackPanel.Children.Add(AvoidRetryTimeStackPanel);
                CombatGeneralContentListBox.Items.Add(AvoidanceOptionsStackPanel);

                #endregion
                #region Kiting

                StackPanel KitingOptionsStackPanel = new StackPanel
                {
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
                };


                #region KitingRetry

                Slider sliderKiteMinimumRetry = new Slider
                {
                    Width = 120,
                    Maximum = 10000,
                    Minimum = 0,
                    TickFrequency = 500,
                    LargeChange = 1000,
                    SmallChange = 50,
                    Value = SettingsFunky.KitingRecheckMinimumRate,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    Margin = new Thickness(6),

                };
                sliderKiteMinimumRetry.ValueChanged += KitingMinimumRetrySliderChanged;
                TBKiteTimeLimits = new TextBox[2];
                TBKiteTimeLimits[0] = new TextBox
                {
                    Text = SettingsFunky.KitingRecheckMinimumRate.ToString(),
                    IsReadOnly = true,
                };

                TextBlock Kite_Retry_Min_Text = new TextBlock
                {
                    Text = "Minimum",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Center,
                };


                Slider sliderKiteMaximumRetry = new Slider
                {
                    Width = 120,
                    Maximum = 10000,
                    Minimum = 0,
                    TickFrequency = 500,
                    LargeChange = 1000,
                    SmallChange = 50,
                    Value = SettingsFunky.KitingRecheckMaximumRate,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    Margin = new Thickness(6),
                };
                sliderKiteMaximumRetry.ValueChanged += KitingMaximumRetrySliderChanged;
                TBKiteTimeLimits[1] = new TextBox
                {
                    Text = SettingsFunky.KitingRecheckMaximumRate.ToString(),
                    IsReadOnly = true,
                };
                TextBlock Kite_Retry_Max_Text = new TextBlock
                {
                    Text = "Maximum",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Center,
                };
                StackPanel KiteRetryTimeStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 30,
                    Orientation = Orientation.Horizontal,
                };
                //
                KiteRetryTimeStackPanel.Children.Add(Kite_Retry_Min_Text);
                KiteRetryTimeStackPanel.Children.Add(sliderKiteMinimumRetry);
                KiteRetryTimeStackPanel.Children.Add(TBKiteTimeLimits[0]);
                KiteRetryTimeStackPanel.Children.Add(Kite_Retry_Max_Text);
                KiteRetryTimeStackPanel.Children.Add(sliderKiteMaximumRetry);
                KiteRetryTimeStackPanel.Children.Add(TBKiteTimeLimits[1]);
                #endregion

                #region Kiting Distance
                Slider sliderKiteDistance = new Slider
                {
                    Width = 100,
                    Maximum = 20,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.KiteDistance,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderKiteDistance.ValueChanged += KiteSliderChanged;
                TBKiteDistance = new TextBox
                {
                    Text = SettingsFunky.KiteDistance.ToString(),
                    IsReadOnly = true,
                };
                StackPanel KiteDistanceStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 6),
                };
                //KiteDistanceStackPanel.Children.Add(Kite_Text);
                KiteDistanceStackPanel.Children.Add(sliderKiteDistance);
                KiteDistanceStackPanel.Children.Add(TBKiteDistance);
                #endregion
                StackPanel KiteDelayInfoStackPanel = new StackPanel();
                #region KiteRetryTextInfo
                TextBlock Kite_Delay_Text = new TextBlock
                {
                    Text = "Delay invtervals",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Left,
                };
                TextBlock Kite_DelayInfo_Text = new TextBlock
                {
                    Text = "Minimum is delay used if failed, Maximum is delay after successful searching",
                    FontSize = 9,
                    FontStyle = FontStyles.Italic,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Left,
                };

                KiteDelayInfoStackPanel.Children.Add(Kite_Delay_Text);
                KiteDelayInfoStackPanel.Children.Add(Kite_DelayInfo_Text);
                #endregion


                TextBlock Kite_Header_Text = new TextBlock
                {
                    Text = "Kiting",
                    FontSize = 13,
                    Background = System.Windows.Media.Brushes.LightSeaGreen,
                    TextAlignment = TextAlignment.Center,
                };

                StackPanel KiteDistanceInfoStackPanel = new StackPanel();
                #region KiteDistanceText
                TextBlock Kite_Distance_Label = new TextBlock
                {
                    Text = "Distance",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    //Background = System.Windows.Media.Brushes.Crimson,
                    TextAlignment = TextAlignment.Left,
                };
                TextBlock Kite_Distance_Info_Text = new TextBlock
                {
                    Text = "This determines how close a valid monster is before trying to flee",
                    FontSize = 11,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    FontStyle = FontStyles.Italic,
                    //Background = System.Windows.Media.Brushes.Crimson,
                    TextAlignment = TextAlignment.Left,
                };
                KiteDistanceInfoStackPanel.Children.Add(Kite_Distance_Label);
                KiteDistanceInfoStackPanel.Children.Add(Kite_Distance_Info_Text);
                #endregion

                KitingOptionsStackPanel.Children.Add(Kite_Header_Text);
                KitingOptionsStackPanel.Children.Add(KiteDistanceInfoStackPanel);
                KitingOptionsStackPanel.Children.Add(KiteDistanceStackPanel);
                KitingOptionsStackPanel.Children.Add(KiteDelayInfoStackPanel);
                KitingOptionsStackPanel.Children.Add(KiteRetryTimeStackPanel);
                CombatGeneralContentListBox.Items.Add(KitingOptionsStackPanel);

                #endregion
                #region HealthOptions
                StackPanel HealthOptionsStackPanel = new StackPanel
                {
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
                };
                TextBlock Health_Options_Text = new TextBlock
                {
                    Text = "Health",
                    FontSize = 13,
                    Background = System.Windows.Media.Brushes.DarkSeaGreen,
                    TextAlignment = TextAlignment.Center,
                };
                TextBlock Health_Info_Text = new TextBlock
                {
                    Text = "Actions will occur when life is below given value",
                    FontSize = 12,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    FontStyle = FontStyles.Italic,
                    //Background = System.Windows.Media.Brushes.Crimson,
                    TextAlignment = TextAlignment.Left,
                };

                TextBlock HealthGlobe_Info_Text = new TextBlock
                {
                    Text = "Globe Health Percent",
                    FontSize = 12,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    //Background = System.Windows.Media.Brushes.Crimson,
                    TextAlignment = TextAlignment.Left,
                };
                #region GlobeHealthPercent

                Slider sliderGlobeHealth = new Slider
                {
                    Width = 100,
                    Maximum = 1,
                    Minimum = 0,
                    TickFrequency = 0.25,
                    LargeChange = 0.20,
                    SmallChange = 0.10,
                    Value = SettingsFunky.GlobeHealthPercent,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderGlobeHealth.ValueChanged += GlobeHealthSliderChanged;
                TBGlobeHealth = new TextBox
                {
                    Text = SettingsFunky.GlobeHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
                    IsReadOnly = true,
                };
                StackPanel GlobeHealthStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                GlobeHealthStackPanel.Children.Add(sliderGlobeHealth);
                GlobeHealthStackPanel.Children.Add(TBGlobeHealth);
                #endregion

                TextBlock HealthPotion_Info_Text = new TextBlock
                {
                    Text = "Potion Health Percent",
                    FontSize = 12,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    //Background = System.Windows.Media.Brushes.Crimson,
                    TextAlignment = TextAlignment.Left,
                };

                #region PotionHealthPercent

                Slider sliderPotionHealth = new Slider
                {
                    Width = 100,
                    Maximum = 1,
                    Minimum = 0,
                    TickFrequency = 0.25,
                    LargeChange = 0.20,
                    SmallChange = 0.10,
                    Value = SettingsFunky.PotionHealthPercent,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderPotionHealth.ValueChanged += PotionHealthSliderChanged;
                TBPotionHealth = new TextBox
                {
                    Text = SettingsFunky.PotionHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
                    IsReadOnly = true,
                };
                StackPanel PotionHealthStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                PotionHealthStackPanel.Children.Add(sliderPotionHealth);
                PotionHealthStackPanel.Children.Add(TBPotionHealth);
                #endregion
                //
                HealthOptionsStackPanel.Children.Add(Health_Options_Text);
                HealthOptionsStackPanel.Children.Add(Health_Info_Text);
                HealthOptionsStackPanel.Children.Add(HealthGlobe_Info_Text);
                HealthOptionsStackPanel.Children.Add(GlobeHealthStackPanel);
                HealthOptionsStackPanel.Children.Add(HealthPotion_Info_Text);
                HealthOptionsStackPanel.Children.Add(PotionHealthStackPanel);
                CombatGeneralContentListBox.Items.Add(HealthOptionsStackPanel);
                CombatGeneralTabItem.Content = CombatGeneralContentListBox;
                #endregion

                #endregion


                #region Avoidances
                TabItem AvoidanceTabItem = new TabItem();
                AvoidanceTabItem.Header = "Avoidances";
                CombatTabControl.Items.Add(AvoidanceTabItem);
                ListBox LBcharacterAvoidance = new ListBox();




                Grid AvoidanceLayoutGrid = new Grid
                {
                    UseLayoutRounding = true,
                    ShowGridLines = false,
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    FlowDirection = System.Windows.FlowDirection.LeftToRight,
                    Focusable = false,
                };

                ColumnDefinition colDef1 = new ColumnDefinition();
                ColumnDefinition colDef2 = new ColumnDefinition();
                ColumnDefinition colDef3 = new ColumnDefinition();
                AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef1);
                AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef2);
                AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef3);
                RowDefinition rowDef1 = new RowDefinition();
                AvoidanceLayoutGrid.RowDefinitions.Add(rowDef1);

                TextBlock ColumnHeader1 = new TextBlock
                {
                    Text = "Type",
                    FontSize = 12,
                    TextAlignment = System.Windows.TextAlignment.Center,
                    Background = System.Windows.Media.Brushes.WhiteSmoke,
                };
                TextBlock ColumnHeader2 = new TextBlock
                {
                    Text = "Radius",
                    FontSize = 12,
                    TextAlignment = System.Windows.TextAlignment.Center,
                    Background = System.Windows.Media.Brushes.Goldenrod,
                };
                TextBlock ColumnHeader3 = new TextBlock
                {
                    Text = "Health",
                    FontSize = 12,
                    TextAlignment = System.Windows.TextAlignment.Center,
                    Background = System.Windows.Media.Brushes.DarkRed,
                };
                Grid.SetColumn(ColumnHeader1, 0);
                Grid.SetColumn(ColumnHeader2, 1);
                Grid.SetColumn(ColumnHeader3, 2);
                Grid.SetRow(ColumnHeader1, 0);
                Grid.SetRow(ColumnHeader2, 0);
                Grid.SetRow(ColumnHeader3, 0);
                AvoidanceLayoutGrid.Children.Add(ColumnHeader1);
                AvoidanceLayoutGrid.Children.Add(ColumnHeader2);
                AvoidanceLayoutGrid.Children.Add(ColumnHeader3);

                Dictionary<AvoidanceType, double> currentDictionaryAvoidance = ReturnDictionaryUsingActorClass(ActorClass);
                AvoidanceType[] avoidanceTypes = currentDictionaryAvoidance.Keys.ToArray();
                TBavoidanceHealth = new TextBox[avoidanceTypes.Length - 1];
                TBavoidanceRadius = new TextBox[avoidanceTypes.Length - 1];
                for(int i = 0; i < avoidanceTypes.Length - 1; i++)
                {
                    string avoidanceString = avoidanceTypes[i].ToString();

                    float defaultRadius = 0f;
                    dictAvoidanceRadius.TryGetValue(avoidanceTypes[i], out defaultRadius);
                    Slider avoidanceRadius = new Slider
                    {
                        Width = 125,
                        Name = avoidanceString + "_radius_" + i.ToString(),
                        Maximum = 25,
                        Minimum = 0,
                        TickFrequency = 5,
                        LargeChange = 5,
                        SmallChange = 1,
                        Value = defaultRadius,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        //Padding=new Thickness(2),
                        Margin = new Thickness(5),
                    };
                    avoidanceRadius.ValueChanged += AvoidanceRadiusSliderValueChanged;
                    TBavoidanceRadius[i] = new TextBox
                    {
                        Text = defaultRadius.ToString(),
                        IsReadOnly = true,
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    };

                    double defaultHealth = 0d;
                    ReturnDictionaryUsingActorClass(ActorClass).TryGetValue(avoidanceTypes[i], out defaultHealth);
                    Slider avoidanceHealth = new Slider
                    {
                        Name = avoidanceString + "_health_" + i.ToString(),
                        Width = 125,
                        Maximum = 1,
                        Minimum = 0,
                        TickFrequency = 0.10,
                        LargeChange = 0.10,
                        SmallChange = 0.05,
                        Value = defaultHealth,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Margin = new Thickness(5),
                    };
                    avoidanceHealth.ValueChanged += AvoidanceHealthSliderValueChanged;
                    TBavoidanceHealth[i] = new TextBox
                    {
                        Text = defaultHealth.ToString("F2", CultureInfo.InvariantCulture),
                        IsReadOnly = true,
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    };

                    RowDefinition newRow = new RowDefinition();
                    AvoidanceLayoutGrid.RowDefinitions.Add(newRow);


                    TextBlock txt1 = new TextBlock();
                    txt1.Text = avoidanceString;
                    txt1.FontSize = 14;
                    txt1.Background = System.Windows.Media.Brushes.Moccasin;

                    StackPanel avoidRadiusStackPanel = new StackPanel
                    {
                        Width = 175,
                        Height = 30,
                        Orientation = Orientation.Horizontal,
                    };
                    avoidRadiusStackPanel.Children.Add(avoidanceRadius);
                    avoidRadiusStackPanel.Children.Add(TBavoidanceRadius[i]);

                    StackPanel avoidHealthStackPanel = new StackPanel
                    {
                        Width = 175,
                        Height = 30,
                        Orientation = Orientation.Horizontal,
                    };
                    avoidHealthStackPanel.Children.Add(avoidanceHealth);
                    avoidHealthStackPanel.Children.Add(TBavoidanceHealth[i]);

                    Grid.SetColumn(txt1, 0);
                    Grid.SetColumn(avoidRadiusStackPanel, 1);
                    Grid.SetColumn(avoidHealthStackPanel, 2);

                    int currentIndex = AvoidanceLayoutGrid.RowDefinitions.Count - 1;
                    Grid.SetRow(avoidRadiusStackPanel, currentIndex);
                    Grid.SetRow(avoidHealthStackPanel, currentIndex);
                    Grid.SetRow(txt1, currentIndex);

                    AvoidanceLayoutGrid.Children.Add(txt1);
                    AvoidanceLayoutGrid.Children.Add(avoidRadiusStackPanel);
                    AvoidanceLayoutGrid.Children.Add(avoidHealthStackPanel);
                }

                LBcharacterAvoidance.Items.Add(AvoidanceLayoutGrid);


                AvoidanceTabItem.Content = LBcharacterAvoidance;
                #endregion


                #region ClassSettings
                //Class Specific
                TabItem ClassTabItem = new TabItem();
                ClassTabItem.Header = "Class";
                CombatTabControl.Items.Add(ClassTabItem);
                ListBox LBClass = new ListBox();

                switch(ActorClass)
                {
                    case Zeta.Internals.Actors.ActorClass.Barbarian:
                        CheckBox cbbSelectiveWhirlwind = new CheckBox
                        {
                            Content = "Selective Whirlwind Targeting",
                            Width = 300,
                            Height = 30,
                            IsChecked = (SettingsFunky.Class.bSelectiveWhirlwind)
                        };
                        cbbSelectiveWhirlwind.Checked += bSelectiveWhirlwindChecked;
                        cbbSelectiveWhirlwind.Unchecked += bSelectiveWhirlwindChecked;
                        LBClass.Items.Add(cbbSelectiveWhirlwind);

                        CheckBox cbbWaitForWrath = new CheckBox
                        {
                            Content = "Wait for Wrath",
                            Width = 300,
                            Height = 30,
                            IsChecked = (SettingsFunky.Class.bWaitForWrath)
                        };
                        cbbWaitForWrath.Checked += bWaitForWrathChecked;
                        cbbWaitForWrath.Unchecked += bWaitForWrathChecked;
                        LBClass.Items.Add(cbbWaitForWrath);

                        CheckBox cbbGoblinWrath = new CheckBox
                        {
                            Content = "Use Wrath on Goblins",
                            Width = 300,
                            Height = 30,
                            IsChecked = (SettingsFunky.Class.bGoblinWrath)
                        };
                        cbbGoblinWrath.Checked += bGoblinWrathChecked;
                        cbbGoblinWrath.Unchecked += bGoblinWrathChecked;
                        LBClass.Items.Add(cbbGoblinWrath);

                        CheckBox cbbFuryDumpWrath = new CheckBox
                        {
                            Content = "Fury Dump during Wrath",
                            Width = 300,
                            Height = 30,
                            IsChecked = (SettingsFunky.Class.bFuryDumpWrath)
                        };
                        cbbFuryDumpWrath.Checked += bFuryDumpWrathChecked;
                        cbbFuryDumpWrath.Unchecked += bFuryDumpWrathChecked;
                        LBClass.Items.Add(cbbFuryDumpWrath);

                        CheckBox cbbFuryDumpAlways = new CheckBox
                        {
                            Content = "Fury Dump Always",
                            Width = 300,
                            Height = 30,
                            IsChecked = (SettingsFunky.Class.bFuryDumpAlways)
                        };
                        cbbFuryDumpAlways.Checked += bFuryDumpAlwaysChecked;
                        cbbFuryDumpAlways.Unchecked += bFuryDumpAlwaysChecked;
                        LBClass.Items.Add(cbbFuryDumpAlways);

                        break;
                    case Zeta.Internals.Actors.ActorClass.DemonHunter:
                        LBClass.Items.Add("Reuse Vault Delay");
                        Slider iDHVaultMovementDelayslider = new Slider
                        {
                            Width = 200,
                            Maximum = 4000,
                            Minimum = 400,
                            TickFrequency = 5,
                            LargeChange = 5,
                            SmallChange = 1,
                            Value = SettingsFunky.Class.iDHVaultMovementDelay,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        };
                        iDHVaultMovementDelayslider.ValueChanged += iDHVaultMovementDelaySliderChanged;
                        TBiDHVaultMovementDelay = new TextBox
                        {
                            Text = SettingsFunky.Class.iDHVaultMovementDelay.ToString(),
                            IsReadOnly = true,
                        };
                        StackPanel DhVaultPanel = new StackPanel
                        {
                            Width = 600,
                            Height = 30,
                            Orientation = Orientation.Horizontal,
                        };
                        DhVaultPanel.Children.Add(iDHVaultMovementDelayslider);
                        DhVaultPanel.Children.Add(TBiDHVaultMovementDelay);
                        LBClass.Items.Add(DhVaultPanel);

                        break;
                    case Zeta.Internals.Actors.ActorClass.Monk:
                        CheckBox cbbMonkInnaSet = new CheckBox
                        {
                            Content = "Full Inna Set Bonus",
                            Width = 300,
                            Height = 30,
                            IsChecked = (SettingsFunky.Class.bMonkInnaSet)
                        };
                        cbbMonkInnaSet.Checked += bMonkInnaSetChecked;
                        cbbMonkInnaSet.Unchecked += bMonkInnaSetChecked;
                        LBClass.Items.Add(cbbMonkInnaSet);

                        break;
                    case Zeta.Internals.Actors.ActorClass.WitchDoctor:
                    case Zeta.Internals.Actors.ActorClass.Wizard:
                        CheckBox cbbEnableCriticalMass = new CheckBox
                        {
                            Content = "Critical Mass",
                            Width = 300,
                            Height = 30,
                            IsChecked = (SettingsFunky.Class.bEnableCriticalMass)
                        };
                        cbbEnableCriticalMass.Checked += bEnableCriticalMassChecked;
                        cbbEnableCriticalMass.Unchecked += bEnableCriticalMassChecked;
                        LBClass.Items.Add(cbbEnableCriticalMass);

                        if(ActorClass == Zeta.Internals.Actors.ActorClass.Wizard)
                        {
                            CheckBox cbbWaitForArchon = new CheckBox
                            {
                                Content = "Wait for Archon",
                                Width = 300,
                                Height = 30,
                                IsChecked = (SettingsFunky.Class.bWaitForArchon)
                            };
                            cbbWaitForArchon.Checked += bWaitForArchonChecked;
                            cbbWaitForArchon.Unchecked += bWaitForArchonChecked;
                            LBClass.Items.Add(cbbWaitForArchon);

                            CheckBox cbbKiteOnlyArchon = new CheckBox
                            {
                                Content = "Kite Only During Archon",
                                Width = 300,
                                Height = 30,
                                IsChecked = (SettingsFunky.Class.bKiteOnlyArchon)
                            };
                            cbbKiteOnlyArchon.Checked += bKiteOnlyArchonChecked;
                            cbbKiteOnlyArchon.Unchecked += bKiteOnlyArchonChecked;
                            LBClass.Items.Add(cbbKiteOnlyArchon);

                        }

                        break;
                }
                if(ActorClass == Zeta.Internals.Actors.ActorClass.DemonHunter || ActorClass == Zeta.Internals.Actors.ActorClass.WitchDoctor || ActorClass == Zeta.Internals.Actors.ActorClass.Wizard)
                {

                    #region GoblinMinimumRange
                    LBClass.Items.Add("Treasure Goblin Minimum Range");
                    Slider sliderGoblinMinRange = new Slider
                    {
                        Width = 200,
                        Maximum = 75,
                        Minimum = 0,
                        TickFrequency = 5,
                        LargeChange = 5,
                        SmallChange = 1,
                        Value = SettingsFunky.Class.GoblinMinimumRange,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    };
                    sliderGoblinMinRange.ValueChanged += TreasureGoblinMinimumRangeSliderChanged;
                    TBGoblinMinRange = new TextBox
                    {
                        Text = SettingsFunky.Class.GoblinMinimumRange.ToString(),
                        IsReadOnly = true,
                    };
                    StackPanel GoblinMinRangeStackPanel = new StackPanel
                    {
                        Width = 600,
                        Height = 30,
                        Orientation = Orientation.Horizontal,
                    };
                    GoblinMinRangeStackPanel.Children.Add(sliderGoblinMinRange);
                    GoblinMinRangeStackPanel.Children.Add(TBGoblinMinRange);
                    LBClass.Items.Add(GoblinMinRangeStackPanel);
                    #endregion
                }
                ClassTabItem.Content = LBClass;
                #endregion


                CombatSettingsTabItem.Content = CombatTabControl;
                #endregion


                #region Targeting
                TabItem TargetTabItem = new TabItem();
                TargetTabItem.Header = "Targeting";
                tabControl1.Items.Add(TargetTabItem);

                TabControl tcTargeting = new TabControl
                {
                    Height = 600,
                    Width = 600,
                };

                #region Targeting_General
                TabItem TargetingMiscTabItem = new TabItem();
                TargetingMiscTabItem.Header = "General";
                tcTargeting.Items.Add(TargetingMiscTabItem);
                ListBox Target_General_ContentListBox = new ListBox();

                StackPanel Targeting_General_Options_Stackpanel = new StackPanel();
                #region KillLOWHPUnits
                CheckBox cbClusterKillLowHPUnits = new CheckBox
                {
                    Content = "Finish Units with 25% or less HP",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.ClusterKillLowHPUnits)
                };
                cbClusterKillLowHPUnits.Checked += ClusteringKillLowHPChecked;
                cbClusterKillLowHPUnits.Unchecked += ClusteringKillLowHPChecked;
                #endregion

                #region IgnoreElites
                CheckBox cbIgnoreElites = new CheckBox
                {
                    Content = "Ignore Rare/Elite/Unique Monsters",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.IgnoreAboveAverageMobs)
                };
                cbIgnoreElites.Checked += IgnoreEliteMonstersChecked;
                cbIgnoreElites.Unchecked += IgnoreEliteMonstersChecked;
                #endregion

                #region IgnoreCorpses
                CheckBox cbIgnoreCorpses = new CheckBox
                {
                    Content = "Ignore Looting Corpses",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.IgnoreCorpses)
                };
                cbIgnoreCorpses.Checked += IgnoreCorpsesChecked;
                cbIgnoreCorpses.Unchecked += IgnoreCorpsesChecked;
                #endregion

                Targeting_General_Options_Stackpanel.Children.Add(cbClusterKillLowHPUnits);
                Targeting_General_Options_Stackpanel.Children.Add(cbIgnoreElites);
                Targeting_General_Options_Stackpanel.Children.Add(cbIgnoreCorpses);
                Target_General_ContentListBox.Items.Add(Targeting_General_Options_Stackpanel);

                #region ExtendedRepChestRange
                UseExtendedRangeRepChestCB = new CheckBox
                {
                    Content = "Allow high range on rare chests",
                    Width = 300,
                    Height = 20,
                    IsChecked = (SettingsFunky.UseExtendedRangeRepChest)

                };
                UseExtendedRangeRepChestCB.Checked += ExtendRangeRepChestChecked;
                UseExtendedRangeRepChestCB.Unchecked += ExtendRangeRepChestChecked;
                Target_General_ContentListBox.Items.Add(UseExtendedRangeRepChestCB);
                #endregion

                #region GoblinPriority
                Target_General_ContentListBox.Items.Add("Goblin Priority");
                ComboBox CBGoblinPriority = new ComboBox
                {
                    Height = 25,
                    Width = 300,
                    ItemsSource = new GoblinPriority(),
                    SelectedIndex = SettingsFunky.GoblinPriority,
                };
                CBGoblinPriority.SelectionChanged += GoblinPriorityChanged;
                Target_General_ContentListBox.Items.Add(CBGoblinPriority);
                #endregion

                TargetingMiscTabItem.Content = Target_General_ContentListBox;
                #endregion

                #region Targeting_Ranges
                TabItem RangeTabItem = new TabItem();
                RangeTabItem.Header = "Range";
                tcTargeting.Items.Add(RangeTabItem);
                ListBox lbTargetRange = new ListBox();

                CheckBox cbIgnoreCombatRange = new CheckBox
                {
                    Content = "Ignore Combat Range (Set by Profile)",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.IgnoreCombatRange)
                };
                cbIgnoreCombatRange.Checked += IgnoreCombatRangeChecked;
                cbIgnoreCombatRange.Unchecked += IgnoreCombatRangeChecked;
                lbTargetRange.Items.Add(cbIgnoreCombatRange);

                CheckBox cbIgnoreLootRange = new CheckBox
                {
                    Content = "Ignore Loot Range (Set by Profile)",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.IgnoreLootRange)
                };
                cbIgnoreLootRange.Checked += IgnoreLootRangeChecked;
                cbIgnoreLootRange.Unchecked += IgnoreLootRangeChecked;
                lbTargetRange.Items.Add(cbIgnoreLootRange);

                #region EliteRange
                lbTargetRange.Items.Add("Elite Combat Range");
                Slider sliderEliteRange = new Slider
                {
                    Width = 100,
                    Maximum = 75,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.EliteCombatRange,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderEliteRange.ValueChanged += EliteRangeSliderChanged;
                TBEliteRange = new TextBox
                {
                    Text = SettingsFunky.EliteCombatRange.ToString(),
                    IsReadOnly = true,
                };
                StackPanel EliteStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                EliteStackPanel.Children.Add(sliderEliteRange);
                EliteStackPanel.Children.Add(TBEliteRange);
                lbTargetRange.Items.Add(EliteStackPanel);
                #endregion

                #region NonEliteRange
                lbTargetRange.Items.Add("Non-Elite Combat Range");
                Slider sliderNonEliteRange = new Slider
                {
                    Width = 100,
                    Maximum = 75,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.NonEliteCombatRange,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderNonEliteRange.ValueChanged += NonEliteRangeSliderChanged;
                TBNonEliteRange = new TextBox
                {
                    Text = SettingsFunky.NonEliteCombatRange.ToString(),
                    IsReadOnly = true,
                };
                StackPanel NonEliteStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                NonEliteStackPanel.Children.Add(sliderNonEliteRange);
                NonEliteStackPanel.Children.Add(TBNonEliteRange);
                lbTargetRange.Items.Add(NonEliteStackPanel);
                #endregion

                #region ExtendedCombatRange
                lbTargetRange.Items.Add("Extended Combat Range");
                Slider sliderExtendedCombatRange = new Slider
                {
                    Width = 100,
                    Maximum = 50,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.ExtendedCombatRange,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderExtendedCombatRange.ValueChanged += ExtendCombatRangeSliderChanged;
                TBExtendedCombatRange = new TextBox
                {
                    Text = SettingsFunky.ExtendedCombatRange.ToString(),
                    IsReadOnly = true,
                };
                StackPanel ExtendedRangeStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                ExtendedRangeStackPanel.Children.Add(sliderExtendedCombatRange);
                ExtendedRangeStackPanel.Children.Add(TBExtendedCombatRange);
                lbTargetRange.Items.Add(ExtendedRangeStackPanel);
                #endregion

                #region ShrineRange
                lbTargetRange.Items.Add("Shrine Range");
                Slider sliderShrineRange = new Slider
                {
                    Width = 100,
                    Maximum = 70,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.ShrineRange,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderShrineRange.ValueChanged += ShrineRangeSliderChanged;
                TBShrineRange = new TextBox
                {
                    Text = SettingsFunky.ShrineRange.ToString(),
                    IsReadOnly = true,
                };
                StackPanel ShrineStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                ShrineStackPanel.Children.Add(sliderShrineRange);
                ShrineStackPanel.Children.Add(TBShrineRange);
                lbTargetRange.Items.Add(ShrineStackPanel);
                #endregion

                #region ContainerRange
                lbTargetRange.Items.Add("Container Range");
                Slider sliderContainerRange = new Slider
                {
                    Width = 100,
                    Maximum = 70,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.ContainerOpenRange,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderContainerRange.ValueChanged += ContainerRangeSliderChanged;
                TBContainerRange = new TextBox
                {
                    Text = SettingsFunky.ContainerOpenRange.ToString(),
                    IsReadOnly = true,
                };
                StackPanel ContainerStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                ContainerStackPanel.Children.Add(sliderContainerRange);
                ContainerStackPanel.Children.Add(TBContainerRange);
                lbTargetRange.Items.Add(ContainerStackPanel);
                #endregion

                #region DestructibleRange
                lbTargetRange.Items.Add("Destuctible Range");
                Slider sliderDestructibleRange = new Slider
                {
                    Width = 100,
                    Maximum = 70,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.DestructibleRange,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderDestructibleRange.ValueChanged += DestructibleSliderChanged;
                TBDestructibleRange = new TextBox
                {
                    Text = SettingsFunky.DestructibleRange.ToString(),
                    IsReadOnly = true,
                };
                StackPanel DestructibleStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                DestructibleStackPanel.Children.Add(sliderDestructibleRange);
                DestructibleStackPanel.Children.Add(TBDestructibleRange);
                lbTargetRange.Items.Add(DestructibleStackPanel);
                #endregion

                #region GoldRange
                lbTargetRange.Items.Add("Gold Range");
                Slider sliderGoldRange = new Slider
                {
                    Width = 100,
                    Maximum = 75,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.GoldRange,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderGoldRange.ValueChanged += GoldRangeSliderChanged;
                TBGoldRange = new TextBox
                {
                    Text = SettingsFunky.GoldRange.ToString(),
                    IsReadOnly = true,
                };
                StackPanel GoldRangeStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                GoldRangeStackPanel.Children.Add(sliderGoldRange);
                GoldRangeStackPanel.Children.Add(TBGoldRange);
                lbTargetRange.Items.Add(GoldRangeStackPanel);
                #endregion

                #region ItemRange
                lbTargetRange.Items.Add("Item Loot Range");
                Slider sliderItemRange = new Slider
                {
                    Width = 100,
                    Maximum = 75,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.ItemRange,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderItemRange.ValueChanged += ItemRangeSliderChanged;
                TBItemRange = new TextBox
                {
                    Text = SettingsFunky.ItemRange.ToString(),
                    IsReadOnly = true,
                };
                StackPanel ItemRangeStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                ItemRangeStackPanel.Children.Add(sliderItemRange);
                ItemRangeStackPanel.Children.Add(TBItemRange);
                lbTargetRange.Items.Add(ItemRangeStackPanel);
                #endregion

                #region GoblinRange
                lbTargetRange.Items.Add("Treasure Goblin Range");
                Slider sliderGoblinRange = new Slider
                {
                    Width = 100,
                    Maximum = 75,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.TreasureGoblinRange,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderGoblinRange.ValueChanged += TreasureGoblinRangeSliderChanged;
                TBGoblinRange = new TextBox
                {
                    Text = SettingsFunky.TreasureGoblinRange.ToString(),
                    IsReadOnly = true,
                };
                StackPanel GoblinRangeStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                GoblinRangeStackPanel.Children.Add(sliderGoblinRange);
                GoblinRangeStackPanel.Children.Add(TBGoblinRange);
                lbTargetRange.Items.Add(GoblinRangeStackPanel);
                #endregion

                RangeTabItem.Content = lbTargetRange;
                #endregion

                #region Targeting_Clustering
                TabItem TargetingClusterItem = new TabItem();
                TargetingClusterItem.Header = "Clustering";
                tcTargeting.Items.Add(TargetingClusterItem);
                ListBox lbTargetCluster = new ListBox();


                #region ClusterTargetLogic
                CheckBox cbClusterEnabled = new CheckBox
                {
                    Content = "Enable Clustering Target Logic",
                    Width = 300,
                    Height = 20,
                    IsChecked = (SettingsFunky.EnableClusteringTargetLogic)
                };
                cbClusterEnabled.Checked += EnableClusteringTargetLogicChecked;
                cbClusterEnabled.Unchecked += EnableClusteringTargetLogicChecked;
                lbTargetCluster.Items.Add(cbClusterEnabled);
                #endregion

                #region LowHP
                CheckBox cbClusterIgnoreBotLowHP = new CheckBox
                {
                    Content = "Disable Logic on Bot HP",
                    Width = 300,
                    Height = 20,
                    IsChecked = (SettingsFunky.IgnoreClusteringWhenLowHP)
                };
                cbClusterIgnoreBotLowHP.Checked += IgnoreClusteringBotLowHPisChecked;
                cbClusterIgnoreBotLowHP.Unchecked += IgnoreClusteringBotLowHPisChecked;
                // lbTargetCluster.Items.Add(cbClusterIgnoreBotLowHP);

                #region ClusterLowHPSliderValue
                lbTargetCluster.Items.Add("HP Value");
                Slider sliderClusterLowHPValue = new Slider
                {
                    Width = 100,
                    Maximum = 1,
                    Minimum = 0,
                    TickFrequency = 0.25,
                    LargeChange = 0.25,
                    SmallChange = 0.10,
                    Value = SettingsFunky.IgnoreClusterLowHPValue,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderClusterLowHPValue.ValueChanged += ClusterLowHPValueSliderChanged;
                TBClusterLowHPValue = new TextBox
                {
                    Text = SettingsFunky.IgnoreClusterLowHPValue.ToString("F2", CultureInfo.InvariantCulture),
                    IsReadOnly = true,
                };
                StackPanel ClusterLowHPValueStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                ClusterLowHPValueStackPanel.Children.Add(cbClusterIgnoreBotLowHP);
                ClusterLowHPValueStackPanel.Children.Add(sliderClusterLowHPValue);
                ClusterLowHPValueStackPanel.Children.Add(TBClusterLowHPValue);
                lbTargetCluster.Items.Add(ClusterLowHPValueStackPanel);
                #endregion
                #endregion

                #region ClusterDistance
                lbTargetCluster.Items.Add("Cluster Distance");
                Slider sliderClusterDistance = new Slider
                {
                    Width = 100,
                    Maximum = 20,
                    Minimum = 0,
                    TickFrequency = 4,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.ClusterDistance,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderClusterDistance.ValueChanged += ClusterDistanceSliderChanged;
                TBClusterDistance = new TextBox
                {
                    Text = SettingsFunky.ClusterDistance.ToString(),
                    IsReadOnly = true,
                };
                StackPanel ClusterDistanceStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                ClusterDistanceStackPanel.Children.Add(sliderClusterDistance);
                ClusterDistanceStackPanel.Children.Add(TBClusterDistance);
                lbTargetCluster.Items.Add(ClusterDistanceStackPanel);
                #endregion

                #region ClusterMinUnitCount
                lbTargetCluster.Items.Add("Cluster Minimum Unit Count");
                Slider sliderClusterMinUnitCount = new Slider
                {
                    Width = 100,
                    Maximum = 10,
                    Minimum = 1,
                    TickFrequency = 2,
                    LargeChange = 2,
                    SmallChange = 1,
                    Value = SettingsFunky.ClusterMinimumUnitCount,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderClusterMinUnitCount.ValueChanged += ClusterMinUnitSliderChanged;
                TBClusterMinUnitCount = new TextBox
                {
                    Text = SettingsFunky.ClusterMinimumUnitCount.ToString(),
                    IsReadOnly = true,
                };
                StackPanel ClusterMinUnitCountStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                ClusterMinUnitCountStackPanel.Children.Add(sliderClusterMinUnitCount);
                ClusterMinUnitCountStackPanel.Children.Add(TBClusterMinUnitCount);
                lbTargetCluster.Items.Add(ClusterMinUnitCountStackPanel);
                #endregion


                TargetingClusterItem.Content = lbTargetCluster;

                #endregion

                TargetTabItem.Content = tcTargeting;

                #endregion


                #region General
                TabControl tcGeneral = new TabControl
                {
                    Width = 600,
                    Height = 600,
                };

                TabItem GeneralSettingsTabItem = new TabItem();
                GeneralSettingsTabItem.Header = "General";
                tabControl1.Items.Add(GeneralSettingsTabItem);

                TabItem GeneralTab = new TabItem();
                GeneralTab.Header = "General";
                tcGeneral.Items.Add(GeneralTab);
                lbGeneralContent = new ListBox();


                #region OOCItemBehavior
                StackPanel OOCItemBehaviorStackPanel = new StackPanel
                {
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
                };
                TextBlock OOCItemBehavior_Header_Text = new TextBlock
                {
                    Text = "Out-Of-Combat Item Idenification",
                    FontSize = 13,
                    Background = System.Windows.Media.Brushes.LightSeaGreen,
                    TextAlignment = TextAlignment.Left,
                };
                TextBlock OOCItemBehavior_Header_Info = new TextBlock
                {
                    Text = "Behavior Preforms idenfication of items (Individual IDing) when unid count is surpassed",
                    FontSize = 11,
                    FontStyle = FontStyles.Italic,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Left,
                };

                #region OOC_ID_Items
                OOCIdentifyItems = new CheckBox
                {
                    Content = "Enable Out Of Combat Idenification Behavior",
                    IsChecked = (SettingsFunky.OOCIdentifyItems),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,

                };
                OOCIdentifyItems.Checked += OOCIDChecked;
                OOCIdentifyItems.Unchecked += OOCIDChecked;
                #endregion
                TextBlock OOCItemBehavior_MinItem_Text = new TextBlock
                {
                    Text = "Minimum Unid Items",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Left,
                    HorizontalAlignment= System.Windows.HorizontalAlignment.Left,
                };

                #region OOC_Min_Item_Count

                OOCIdentfyItemsMinCount = new TextBox
                {
                    Text = SettingsFunky.OOCIdentifyItemsMinimumRequired.ToString(),
                    Width = 100,
                    Height = 25
                };
                OOCIdentfyItemsMinCount.KeyUp += OOCMinimumItems_KeyUp;
                OOCIdentfyItemsMinCount.TextChanged += OOCIdentifyItemsMinValueChanged;

                #endregion


                OOCItemBehaviorStackPanel.Children.Add(OOCItemBehavior_Header_Text);
                OOCItemBehaviorStackPanel.Children.Add(OOCIdentifyItems);
                OOCItemBehaviorStackPanel.Children.Add(OOCItemBehavior_Header_Info);
                OOCItemBehaviorStackPanel.Children.Add(OOCItemBehavior_MinItem_Text);
                OOCItemBehaviorStackPanel.Children.Add(OOCIdentfyItemsMinCount);
                lbGeneralContent.Items.Add(OOCItemBehaviorStackPanel);
                #endregion

                #region PotionsDuringTownRun
                BuyPotionsDuringTownRunCB = new CheckBox
                {
                    Content = "Buy Potions During Town Run (Uses Maximum Potion Count Setting)",
                    Width = 500,
                    Height = 30,
                    IsChecked = (SettingsFunky.BuyPotionsDuringTownRun)
                };
                BuyPotionsDuringTownRunCB.Checked += BuyPotionsDuringTownRunChecked;
                BuyPotionsDuringTownRunCB.Unchecked += BuyPotionsDuringTownRunChecked;
                lbGeneralContent.Items.Add(BuyPotionsDuringTownRunCB);
                #endregion

                #region OutOfCombatMovement
                CheckBox cbOutOfCombatMovement = new CheckBox
                {
                    Content = "Use Out Of Combat Ability Movements",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.OutOfCombatMovement)
                };
                cbOutOfCombatMovement.Checked += OutOfCombatMovementChecked;
                cbOutOfCombatMovement.Unchecked += OutOfCombatMovementChecked;
                lbGeneralContent.Items.Add(cbOutOfCombatMovement);
                #endregion

                #region AfterCombatDelayOptions
                StackPanel AfterCombatDelayStackPanel = new StackPanel();
                #region AfterCombatDelay

                Slider sliderAfterCombatDelay = new Slider
                {
                    Width = 100,
                    Maximum = 2000,
                    Minimum = 0,
                    TickFrequency = 200,
                    LargeChange = 100,
                    SmallChange = 50,
                    Value = SettingsFunky.AfterCombatDelay,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
                };
                sliderAfterCombatDelay.ValueChanged += AfterCombatDelaySliderChanged;
                TBAfterCombatDelay = new TextBox
                {
                    Margin = new Thickness(Margin.Left + 5, Margin.Top, Margin.Right, Margin.Bottom),
                    Text = SettingsFunky.AfterCombatDelay.ToString(),
                    IsReadOnly = true,
                };
                StackPanel AfterCombatStackPanel = new StackPanel
                {
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
                    Orientation = Orientation.Horizontal,
                };
                AfterCombatStackPanel.Children.Add(sliderAfterCombatDelay);
                AfterCombatStackPanel.Children.Add(TBAfterCombatDelay);

                #endregion
                #region WaitTimerAfterContainers
                EnableWaitAfterContainersCB = new CheckBox
                {
                    Content = "Apply Delay After Opening Containers",
                    Width = 300,
                    Height = 20,
                    IsChecked = (SettingsFunky.EnableWaitAfterContainers)
                };
                EnableWaitAfterContainersCB.Checked += EnableWaitAfterContainersChecked;
                EnableWaitAfterContainersCB.Unchecked += EnableWaitAfterContainersChecked;

                #endregion

                TextBlock CombatLootDelay_Text_Info = new TextBlock
                {
                    Text = "End of Combat Delay Timer",
                    FontSize = 11,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Left,
                };
                AfterCombatDelayStackPanel.Children.Add(CombatLootDelay_Text_Info);
                AfterCombatDelayStackPanel.Children.Add(AfterCombatStackPanel);
                AfterCombatDelayStackPanel.Children.Add(EnableWaitAfterContainersCB);
                lbGeneralContent.Items.Add(AfterCombatDelayStackPanel);

                #endregion

                GeneralTab.Content = lbGeneralContent;



                #region CoffeeBreaks
                TabItem CoffeeBreakTab = new TabItem();
                CoffeeBreakTab.Header = "Coffee Breaks";
                tcGeneral.Items.Add(CoffeeBreakTab);
                ListBox LBCoffeebreak = new ListBox();

                StackPanel CoffeeBreaksStackPanel = new StackPanel
                {
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
                    Orientation = Orientation.Vertical,

                };

                TextBlock CoffeeBreaks_Header_Text = new TextBlock
                {
                    Text = "Coffee Breaks",
                    FontSize = 13,
                    Background = System.Windows.Media.Brushes.LightSeaGreen,
                    TextAlignment = TextAlignment.Center,
                };

                #region CoffeeBreakCheckBox
                CoffeeBreaks = new CheckBox
                {
                    Content = "Enable Coffee Breaks",
                    Height = 20,
                    IsChecked = (SettingsFunky.EnableCoffeeBreaks)

                };
                CoffeeBreaks.Checked += EnableCoffeeBreaksChecked;
                CoffeeBreaks.Unchecked += EnableCoffeeBreaksChecked;
                #endregion

                TextBlock CoffeeBreak_Minutes_Text = new TextBlock
                {
                    Text = "Break time range (Minutues)",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Left,
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
                };

                #region BreakTimeMinMinutes
                TextBlock CoffeeBreaks_Min_Text = new TextBlock
                {
                    Text = "Minimum",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Center,
                };
                Slider sliderBreakMinMinutes = new Slider
                {
                    Width = 100,
                    Maximum = 20,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 2,
                    SmallChange = 1,
                    Value = SettingsFunky.MinBreakTime,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderBreakMinMinutes.ValueChanged += BreakMinMinutesSliderChange;
                tbMinBreakTime = new TextBox
                {
                    Text = sliderBreakMinMinutes.Value.ToString(),
                    IsReadOnly = true,
                };
                StackPanel BreakTimeMinMinutestackPanel = new StackPanel
                {
                    Height = 30,
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
                };
                BreakTimeMinMinutestackPanel.Children.Add(CoffeeBreaks_Min_Text);
                BreakTimeMinMinutestackPanel.Children.Add(sliderBreakMinMinutes);
                BreakTimeMinMinutestackPanel.Children.Add(tbMinBreakTime);

                #endregion

                #region BreakTimeMaxMinutes
                TextBlock CoffeeBreaks_Max_Text = new TextBlock
                {
                    Text = "Maximum",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Center,
                };
                Slider sliderBreakMaxMinutes = new Slider
                {
                    Width = 100,
                    Maximum = 20,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 2,
                    SmallChange = 1,
                    Value = SettingsFunky.MaxBreakTime,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderBreakMaxMinutes.ValueChanged += BreakMaxMinutesSliderChange;
                tbMaxBreakTime = new TextBox
                {
                    Text = sliderBreakMaxMinutes.Value.ToString(),
                    IsReadOnly = true,
                };
                StackPanel BreakTimeMaxMinutestackPanel = new StackPanel
                {
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(Margin.Left + 5, Margin.Top, Margin.Right, Margin.Bottom),
                };
                BreakTimeMaxMinutestackPanel.Children.Add(CoffeeBreaks_Max_Text);
                BreakTimeMaxMinutestackPanel.Children.Add(sliderBreakMaxMinutes);
                BreakTimeMaxMinutestackPanel.Children.Add(tbMaxBreakTime);
                #endregion

                StackPanel CoffeeBreakTimeRangeStackPanel = new StackPanel
                {
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
                    Orientation = Orientation.Horizontal,
                };

                CoffeeBreakTimeRangeStackPanel.Children.Add(BreakTimeMinMinutestackPanel);
                CoffeeBreakTimeRangeStackPanel.Children.Add(BreakTimeMaxMinutestackPanel);

                #region BreakTimeIntervalHour


                Slider sliderBreakTimeHour = new Slider
                {
                    Width = 200,
                    Maximum = 10,
                    Minimum = 0,
                    TickFrequency = 1,
                    LargeChange = 0.50,
                    SmallChange = 0.05,
                    Value = SettingsFunky.breakTimeHour,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderBreakTimeHour.ValueChanged += BreakTimeHourSliderChanged;
                TBBreakTimeHour = new TextBox
                {
                    Text = sliderBreakTimeHour.Value.ToString("F2", CultureInfo.InvariantCulture),
                    IsReadOnly = true,
                };
                StackPanel BreakTimeHourStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 30,
                    Orientation = Orientation.Horizontal,
                };
                BreakTimeHourStackPanel.Children.Add(sliderBreakTimeHour);
                BreakTimeHourStackPanel.Children.Add(TBBreakTimeHour);

                #endregion
                TextBlock CoffeeBreakInterval_Text = new TextBlock
                {
                    Text = "Break Hour Interval (1 Equals One Hour)",
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Left,
                };

                CoffeeBreaksStackPanel.Children.Add(CoffeeBreaks_Header_Text);
                CoffeeBreaksStackPanel.Children.Add(CoffeeBreaks);
                CoffeeBreaksStackPanel.Children.Add(CoffeeBreak_Minutes_Text);
                CoffeeBreaksStackPanel.Children.Add(CoffeeBreakTimeRangeStackPanel);
                CoffeeBreaksStackPanel.Children.Add(CoffeeBreakInterval_Text);
                CoffeeBreaksStackPanel.Children.Add(BreakTimeHourStackPanel);
                LBCoffeebreak.Items.Add(CoffeeBreaksStackPanel);
                CoffeeBreakTab.Content = LBCoffeebreak;
                #endregion



                GeneralSettingsTabItem.Content = tcGeneral;
                #endregion

                #region Items


                TabItem CustomSettingsTabItem = new TabItem();
                CustomSettingsTabItem.Header = "Items";
                tabControl1.Items.Add(CustomSettingsTabItem);

                TabControl tcItems = new TabControl
                {
                    Width = 600,
                    Height = 600
                };

                #region ItemGeneral
                TabItem ItemGeneralTabItem = new TabItem();
                ItemGeneralTabItem.Header = "General";
                tcItems.Items.Add(ItemGeneralTabItem);
                ListBox lbLootContent = new ListBox();

                lbLootContent.Items.Add("Default Scoring Option");
                ItemRuleGilesScoring = new RadioButton
                {
                    GroupName = "Scoring",
                    Content = "Giles Item Scoring",
                    Width = 300,
                    Height = 30,
                    IsChecked = SettingsFunky.ItemRuleGilesScoring
                };
                ItemRuleDBScoring = new RadioButton
                {
                    GroupName = "Scoring",
                    Content = "DB Weight Scoring",
                    Width = 300,
                    Height = 30,
                    IsChecked = !SettingsFunky.ItemRuleGilesScoring
                };

                ItemRuleGilesScoring.Checked += ItemRulesScoringChanged;
                ItemRuleDBScoring.Checked += ItemRulesScoringChanged;
                lbLootContent.Items.Add(ItemRuleGilesScoring);
                lbLootContent.Items.Add(ItemRuleDBScoring);

                CheckBox LevelingLogic = new CheckBox
                {
                    Content = "Leveling Item Logic",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.UseLevelingLogic),
                };
                LevelingLogic.Checked += ItemLevelingLogicChecked;
                LevelingLogic.Unchecked += ItemLevelingLogicChecked;
                lbLootContent.Items.Add(LevelingLogic);

                ItemGeneralTabItem.Content = lbLootContent;
                #endregion

                #region ItemRules
                TabItem ItemRulesTabItem = new TabItem();
                ItemRulesTabItem.Header = "Item Rules";
                tcItems.Items.Add(ItemRulesTabItem);
                ListBox lbItemRulesContent = new ListBox();

                ItemRules = new CheckBox
                {
                    Content = "Use Item Rules",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.UseItemRules)

                };
                ItemRules.Checked += ItemRulesChecked;
                ItemRules.Unchecked += ItemRulesChecked;
                lbItemRulesContent.Items.Add(ItemRules);

                ItemRulesPickup = new CheckBox
                {
                    Content = "Use Item Rules Pickup",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.UseItemRulesPickup)

                };
                ItemRulesPickup.Checked += ItemRulesPickupChecked;
                ItemRulesPickup.Unchecked += ItemRulesPickupChecked;
                lbItemRulesContent.Items.Add(ItemRulesPickup);

                CheckBox CBItemRulesSalvaging = new CheckBox
                {
                    Content = "Item Rules Salvaging",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.ItemRulesSalvaging),
                };
                CBItemRulesSalvaging.Checked += ItemRulesSalvagingChecked;
                CBItemRulesSalvaging.Unchecked += ItemRulesSalvagingChecked;
                lbItemRulesContent.Items.Add(CBItemRulesSalvaging);

                lbItemRulesContent.Items.Add("Rule Set");
                
                ItemRuleType = new ComboBox
                {
                    Height = 30,
                    Width = 300,
                    ItemsSource = new ItemRuleTypes(),
                    SelectedIndex=SettingsFunky.ItemRuleType.ToLower().Contains("soft")?1:SettingsFunky.ItemRuleType.ToLower().Contains("hard")?2:0,
                    Text = SettingsFunky.ItemRuleType.ToString(),
                };
                ItemRuleType.SelectionChanged += ItemRulesTypeChanged;
                lbItemRulesContent.Items.Add(ItemRuleType);


                lbItemRulesContent.Items.Add("Log Items Keep");
                ItemRuleLogKeep = new ComboBox
                {
                    Height = 30,
                    Width = 300,
                    ItemsSource = new ItemRuleQuality(),
                    Text = SettingsFunky.ItemRuleLogKeep
                };
                ItemRuleLogKeep.SelectionChanged += ItemRulesLogKeepChanged;
                lbItemRulesContent.Items.Add(ItemRuleLogKeep);

                lbItemRulesContent.Items.Add("Log Items Pickup");
                ItemRuleLogPickup = new ComboBox
                {
                    Height = 30,
                    Width = 300,
                    ItemsSource = new ItemRuleQuality(),
                    Text = SettingsFunky.ItemRuleLogPickup
                };
                ItemRuleLogPickup.SelectionChanged += ItemRulesLogPickupChanged;
                lbItemRulesContent.Items.Add(ItemRuleLogPickup);

                ItemRuleUseItemIDs = new CheckBox
                {
                    Content = "Use Item IDs",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.ItemRuleUseItemIDs)

                };
                ItemRuleUseItemIDs.Checked += ItemRulesItemIDsChecked;
                ItemRuleUseItemIDs.Unchecked += ItemRulesItemIDsChecked;
                lbItemRulesContent.Items.Add(ItemRuleUseItemIDs);

                ItemRuleDebug = new CheckBox
                {
                    Content = "Debug",
                    Width = 300,
                    Height = 30,
                    IsChecked = (SettingsFunky.ItemRuleDebug)

                };
                ItemRuleDebug.Checked += ItemRulesDebugChecked;
                ItemRuleDebug.Unchecked += ItemRulesDebugChecked;
                lbItemRulesContent.Items.Add(ItemRuleDebug);


                ItemRulesReload = new Button
                {
                    Content = "Reload rules",
                    Width = 300,
                    Height = 30
                };
                ItemRulesReload.Click += ItemRulesReload_Click;
                lbItemRulesContent.Items.Add(ItemRulesReload);

                ItemRulesTabItem.Content = lbItemRulesContent;
                #endregion

                #region GilesPickup
                TabItem ItemGilesTabItem = new TabItem();
                ItemGilesTabItem.Header = "Giles Pickup";
                tcItems.Items.Add(ItemGilesTabItem);
                ListBox lbGilesContent = new ListBox();

                lbGilesContent.Items.Add("Item Level Pickup");
                #region minimumWeaponILevel
                TextBlock txt_weaponIlvl = new TextBlock
                {
                    Text = "Weapons",
                    FontSize = 12,
                    Background = System.Windows.Media.Brushes.DarkSlateGray,
                };
                lbGilesContent.Items.Add(txt_weaponIlvl);

                TextBlock txt_weaponMagical = new TextBlock
                {
                    Text = "Magic",
                    FontSize = 12,
                    Background = System.Windows.Media.Brushes.LightSteelBlue,
                };
                TBMinimumWeaponLevel = new TextBox[2];
                Slider weaponMagicLevelSlider = new Slider
                {
                    Name = "Magic",
                    Width = 120,
                    Maximum = 63,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.MinimumWeaponItemLevel[0],
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                weaponMagicLevelSlider.ValueChanged += WeaponItemLevelSliderChanged;
                TBMinimumWeaponLevel[0] = new TextBox
                {
                    Text = SettingsFunky.MinimumWeaponItemLevel[0].ToString(),
                    IsReadOnly = true,
                };

                TextBlock txt_weaponRare = new TextBlock
                {
                    Text = "Rare",
                    FontSize = 12,
                    Background = System.Windows.Media.Brushes.LightGoldenrodYellow,
                };
                Slider weaponRareLevelSlider = new Slider
                {
                    Name = "Rare",
                    Width = 120,
                    Maximum = 63,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.MinimumWeaponItemLevel[1],
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                };
                weaponRareLevelSlider.ValueChanged += WeaponItemLevelSliderChanged;
                TBMinimumWeaponLevel[1] = new TextBox
                {
                    Text = SettingsFunky.MinimumWeaponItemLevel[1].ToString(),
                    IsReadOnly = true,
                };
                StackPanel weaponLevelSPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                weaponLevelSPanel.Children.Add(txt_weaponMagical);
                weaponLevelSPanel.Children.Add(weaponMagicLevelSlider);
                weaponLevelSPanel.Children.Add(TBMinimumWeaponLevel[0]);
                weaponLevelSPanel.Children.Add(txt_weaponRare);
                weaponLevelSPanel.Children.Add(weaponRareLevelSlider);
                weaponLevelSPanel.Children.Add(TBMinimumWeaponLevel[1]);
                lbGilesContent.Items.Add(weaponLevelSPanel);
                #endregion
                #region minimumArmorILevel
                TBMinimumArmorLevel = new TextBox[2];
                TextBlock txt_armorIlvl = new TextBlock
                {
                    Text = "Armor",
                    FontSize = 12,
                    Background = System.Windows.Media.Brushes.DarkSlateGray,
                };
                lbGilesContent.Items.Add(txt_armorIlvl);

                TextBlock txt_armorMagic = new TextBlock
                {
                    Text = "Magic",
                    FontSize = 12,
                    Background = System.Windows.Media.Brushes.LightSteelBlue,
                };
                Slider armorMagicLevelSlider = new Slider
                {
                    Name = "Magic",
                    Width = 120,
                    Maximum = 63,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.MinimumArmorItemLevel[0],
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                armorMagicLevelSlider.ValueChanged += ArmorItemLevelSliderChanged;
                TBMinimumArmorLevel[0] = new TextBox
                {
                    Text = SettingsFunky.MinimumArmorItemLevel[0].ToString(),
                    IsReadOnly = true,
                };

                TextBlock txt_armorRare = new TextBlock
                {
                    Text = "Rare",
                    FontSize = 12,
                    Background = System.Windows.Media.Brushes.LightGoldenrodYellow,
                };
                Slider armorRareLevelSlider = new Slider
                {
                    Name = "Rare",
                    Maximum = 63,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Width = 120,
                    Value = SettingsFunky.MinimumArmorItemLevel[1],
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                };
                armorRareLevelSlider.ValueChanged += ArmorItemLevelSliderChanged;
                TBMinimumArmorLevel[1] = new TextBox
                {
                    Text = SettingsFunky.MinimumArmorItemLevel[1].ToString(),
                    IsReadOnly = true,
                };
                StackPanel armorLevelSPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                armorLevelSPanel.Children.Add(txt_armorMagic);
                armorLevelSPanel.Children.Add(armorMagicLevelSlider);
                armorLevelSPanel.Children.Add(TBMinimumArmorLevel[0]);
                armorLevelSPanel.Children.Add(txt_armorRare);
                armorLevelSPanel.Children.Add(armorRareLevelSlider);
                armorLevelSPanel.Children.Add(TBMinimumArmorLevel[1]);
                lbGilesContent.Items.Add(armorLevelSPanel);
                #endregion
                #region minimumJeweleryILevel
                TBMinimumJeweleryLevel = new TextBox[2];
                TextBlock txt_jeweleryIlvl = new TextBlock
                {
                    Text = "Jewelery",
                    FontSize = 12,
                    Background = System.Windows.Media.Brushes.DarkSlateGray,
                };
                lbGilesContent.Items.Add(txt_jeweleryIlvl);

                TextBlock txt_jeweleryMagic = new TextBlock
                {
                    Text = "Magic",
                    FontSize = 12,
                    Background = System.Windows.Media.Brushes.LightSteelBlue,
                };
                Slider jeweleryMagicLevelSlider = new Slider
                {
                    Name = "Magic",
                    Width = 120,
                    Maximum = 63,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.MinimumJeweleryItemLevel[0],
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                jeweleryMagicLevelSlider.ValueChanged += JeweleryItemLevelSliderChanged;
                TBMinimumJeweleryLevel[0] = new TextBox
                {
                    Text = SettingsFunky.MinimumJeweleryItemLevel[0].ToString(),
                    IsReadOnly = true,
                };
                TextBlock txt_jeweleryRare = new TextBlock
                {
                    Text = "Rare",
                    FontSize = 12,
                    Background = System.Windows.Media.Brushes.LightGoldenrodYellow,
                };
                Slider jeweleryRareLevelSlider = new Slider
                {
                    Name = "Rare",
                    Maximum = 63,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Width = 120,
                    Value = SettingsFunky.MinimumJeweleryItemLevel[1],
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                };
                jeweleryRareLevelSlider.ValueChanged += JeweleryItemLevelSliderChanged;
                TBMinimumJeweleryLevel[1] = new TextBox
                {
                    Text = SettingsFunky.MinimumJeweleryItemLevel[1].ToString(),
                    IsReadOnly = true,
                };
                StackPanel jeweleryLevelSPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                jeweleryLevelSPanel.Children.Add(txt_jeweleryMagic);
                jeweleryLevelSPanel.Children.Add(jeweleryMagicLevelSlider);
                jeweleryLevelSPanel.Children.Add(TBMinimumJeweleryLevel[0]);
                jeweleryLevelSPanel.Children.Add(txt_jeweleryRare);
                jeweleryLevelSPanel.Children.Add(jeweleryRareLevelSlider);
                jeweleryLevelSPanel.Children.Add(TBMinimumJeweleryLevel[1]);
                lbGilesContent.Items.Add(jeweleryLevelSPanel);
                #endregion


                #region LegendaryLevel
                lbGilesContent.Items.Add("Minimum Legendary Item Level");
                Slider sliderLegendaryILevel = new Slider
                {
                    Width = 120,
                    Maximum = 63,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.MinimumLegendaryItemLevel,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderLegendaryILevel.ValueChanged += LegendaryItemLevelSliderChanged;
                TBMinLegendaryLevel = new TextBox
                {
                    Text = SettingsFunky.MinimumLegendaryItemLevel.ToString(),
                    IsReadOnly = true,
                };
                StackPanel LegendaryILvlStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                LegendaryILvlStackPanel.Children.Add(sliderLegendaryILevel);
                LegendaryILvlStackPanel.Children.Add(TBMinLegendaryLevel);
                lbGilesContent.Items.Add(LegendaryILvlStackPanel);
                #endregion

                #region MaxHealthPotions
                lbGilesContent.Items.Add("Maximum Health Potions");
                Slider sliderMaxHealthPots = new Slider
                {
                    Width = 100,
                    Maximum = 100,
                    Minimum = 0,
                    TickFrequency = 25,
                    LargeChange = 20,
                    SmallChange = 5,
                    Value = SettingsFunky.MaximumHealthPotions,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderMaxHealthPots.ValueChanged += HealthPotionSliderChanged;
                TBMaxHealthPots = new TextBox
                {
                    Text = SettingsFunky.MaximumHealthPotions.ToString(),
                    IsReadOnly = true,
                };
                StackPanel MaxHealthPotsStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                MaxHealthPotsStackPanel.Children.Add(sliderMaxHealthPots);
                MaxHealthPotsStackPanel.Children.Add(TBMaxHealthPots);
                lbGilesContent.Items.Add(MaxHealthPotsStackPanel);
                #endregion

                #region MinimumGoldPile
                lbGilesContent.Items.Add("Minimum Gold Pile");
                Slider slideMinGoldPile = new Slider
                {
                    Width = 120,
                    Maximum = 7500,
                    Minimum = 0,
                    TickFrequency = 1000,
                    LargeChange = 1000,
                    SmallChange = 1,
                    Value = SettingsFunky.MinimumGoldPile,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                slideMinGoldPile.ValueChanged += GoldAmountSliderChanged;
                TBMinGoldPile = new TextBox
                {
                    Text = SettingsFunky.MinimumGoldPile.ToString(),
                    IsReadOnly = true,
                };
                StackPanel MinGoldPileStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                MinGoldPileStackPanel.Children.Add(slideMinGoldPile);
                MinGoldPileStackPanel.Children.Add(TBMinGoldPile);
                lbGilesContent.Items.Add(MinGoldPileStackPanel);
                #endregion


                #region PickupCraftTomes
                CheckBox cbPickupCraftTomes = new CheckBox
                {
                    Content = "Pickup Craft Tomes",
                    Width = 300,
                    Height = 20,
                    IsChecked = (SettingsFunky.PickupCraftTomes)
                };
                cbPickupCraftTomes.Checked += PickupCraftTomesChecked;
                cbPickupCraftTomes.Unchecked += PickupCraftTomesChecked;
                lbGilesContent.Items.Add(cbPickupCraftTomes);
                #endregion
                #region PickupCraftPlans
                CheckBox cbPickupCraftPlans = new CheckBox
                {
                    Content = "Pickup Craft Plans",
                    Width = 300,
                    Height = 20,
                    IsChecked = (SettingsFunky.PickupCraftPlans)
                };
                cbPickupCraftPlans.Checked += PickupCraftPlansChecked;
                cbPickupCraftPlans.Unchecked += PickupCraftPlansChecked;
                lbGilesContent.Items.Add(cbPickupCraftPlans);
                #endregion
                #region PickupFollowerItems
                CheckBox cbPickupFollowerItems = new CheckBox
                {
                    Content = "Pickup Follower Items",
                    Width = 300,
                    Height = 20,
                    IsChecked = (SettingsFunky.PickupFollowerItems)
                };
                cbPickupFollowerItems.Checked += PickupFollowerItemsChecked;
                cbPickupFollowerItems.Unchecked += PickupFollowerItemsChecked;
                lbGilesContent.Items.Add(cbPickupFollowerItems);
                #endregion
                #region MinMiscItemLevel
                lbGilesContent.Items.Add("Misc Item Level");
                Slider slideMinMiscItemLevel = new Slider
                {
                    Width = 100,
                    Maximum = 63,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = SettingsFunky.MiscItemLevel,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                slideMinMiscItemLevel.ValueChanged += MiscItemLevelSliderChanged;
                TBMiscItemLevel = new TextBox
                {
                    Text = SettingsFunky.MiscItemLevel.ToString(),
                    IsReadOnly = true,
                };
                StackPanel MinMiscItemLevelStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                MinMiscItemLevelStackPanel.Children.Add(slideMinMiscItemLevel);
                MinMiscItemLevelStackPanel.Children.Add(TBMiscItemLevel);
                lbGilesContent.Items.Add(MinMiscItemLevelStackPanel);
                #endregion


                #region GemQuality
                lbGilesContent.Items.Add("Minimum Gem Quality");
                CBGemQualityLevel = new ComboBox
                {
                    Height = 20,
                    Width = 200,
                    ItemsSource = new GemQualityTypes(),
                    Text = Enum.GetName(typeof(GemQuality), SettingsFunky.MinimumGemItemLevel).ToString(),
                };
                CBGemQualityLevel.SelectionChanged += GemQualityLevelChanged;
                lbGilesContent.Items.Add(CBGemQualityLevel);
                #endregion

                CBGems = new CheckBox[4];

                #region PickupGemsRed
                CBGems[0] = new CheckBox
                {
                    Content = "Pickup Gem Red",
                    Name = "red",
                    Width = 300,
                    Height = 20,
                    IsChecked = (SettingsFunky.PickupGems[0])
                };
                CBGems[0].Checked += GemsChecked;
                CBGems[0].Unchecked += GemsChecked;
                lbGilesContent.Items.Add(CBGems[0]);
                #endregion
                #region PickupGemsGreen
                CBGems[1] = new CheckBox
                {
                    Content = "Pickup Gem Green",
                    Name = "green",
                    Width = 300,
                    Height = 20,
                    IsChecked = (SettingsFunky.PickupGems[1])
                };
                CBGems[1].Checked += GemsChecked;
                CBGems[1].Unchecked += GemsChecked;
                lbGilesContent.Items.Add(CBGems[1]);
                #endregion
                #region PickupGemsPurple
                CBGems[2] = new CheckBox
                {
                    Content = "Pickup Gem Purple",
                    Name = "purple",
                    Width = 300,
                    Height = 20,
                    IsChecked = (SettingsFunky.PickupGems[2])
                };
                CBGems[2].Checked += GemsChecked;
                CBGems[2].Unchecked += GemsChecked;
                lbGilesContent.Items.Add(CBGems[2]);
                #endregion
                #region PickupGemsYellow
                CBGems[3] = new CheckBox
                {
                    Content = "Pickup Gem Yellow",
                    Name = "yellow",
                    Width = 300,
                    Height = 20,
                    IsChecked = (SettingsFunky.PickupGems[3])
                };
                CBGems[3].Checked += GemsChecked;
                CBGems[3].Unchecked += GemsChecked;
                lbGilesContent.Items.Add(CBGems[3]);
                #endregion

                ItemGilesTabItem.Content = lbGilesContent;
                #endregion

                #region GilesScoring
                TabItem ItemGilesScoringTabItem = new TabItem();
                ItemGilesScoringTabItem.Header = "Giles Scoring";
                tcItems.Items.Add(ItemGilesScoringTabItem);
                ListBox lbGilesScoringContent = new ListBox();

                #region WeaponScore
                lbGilesScoringContent.Items.Add("Minimum Weapon Score to Stash");
                Slider sliderGilesWeaponScore = new Slider
                {
                    Width = 100,
                    Maximum = 100000,
                    Minimum = 1,
                    TickFrequency = 1000,
                    LargeChange = 5000,
                    SmallChange = 1000,
                    Value = SettingsFunky.GilesMinimumWeaponScore,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderGilesWeaponScore.ValueChanged += GilesWeaponScoreSliderChanged;
                TBGilesWeaponScore = new TextBox
                {
                    Text = SettingsFunky.GilesMinimumWeaponScore.ToString(),
                    IsReadOnly = true,
                };
                StackPanel GilesWeaponScoreStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                GilesWeaponScoreStackPanel.Children.Add(sliderGilesWeaponScore);
                GilesWeaponScoreStackPanel.Children.Add(TBGilesWeaponScore);
                lbGilesScoringContent.Items.Add(GilesWeaponScoreStackPanel);
                #endregion

                #region ArmorScore
                lbGilesScoringContent.Items.Add("Minimum Armor Score to Stash");
                Slider sliderGilesArmorScore = new Slider
                {
                    Width = 100,
                    Maximum = 100000,
                    Minimum = 1,
                    TickFrequency = 1000,
                    LargeChange = 5000,
                    SmallChange = 1000,
                    Value = SettingsFunky.GilesMinimumArmorScore,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderGilesArmorScore.ValueChanged += GilesArmorScoreSliderChanged;
                TBGilesArmorScore = new TextBox
                {
                    Text = SettingsFunky.GilesMinimumArmorScore.ToString(),
                    IsReadOnly = true,
                };
                StackPanel GilesArmorScoreStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                GilesArmorScoreStackPanel.Children.Add(sliderGilesArmorScore);
                GilesArmorScoreStackPanel.Children.Add(TBGilesArmorScore);
                lbGilesScoringContent.Items.Add(GilesArmorScoreStackPanel);
                #endregion

                #region JeweleryScore
                lbGilesScoringContent.Items.Add("Minimum Jewelery Score to Stash");
                Slider sliderGilesJeweleryScore = new Slider
                {
                    Width = 100,
                    Maximum = 100000,
                    Minimum = 1,
                    TickFrequency = 1000,
                    LargeChange = 5000,
                    SmallChange = 1000,
                    Value = SettingsFunky.GilesMinimumJeweleryScore,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderGilesJeweleryScore.ValueChanged += GilesJeweleryScoreSliderChanged;
                TBGilesJeweleryScore = new TextBox
                {
                    Text = SettingsFunky.GilesMinimumJeweleryScore.ToString(),
                    IsReadOnly = true,
                };
                StackPanel GilesJeweleryScoreStackPanel = new StackPanel
                {
                    Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                GilesJeweleryScoreStackPanel.Children.Add(sliderGilesJeweleryScore);
                GilesJeweleryScoreStackPanel.Children.Add(TBGilesJeweleryScore);
                lbGilesScoringContent.Items.Add(GilesJeweleryScoreStackPanel);
                #endregion

                ItemGilesScoringTabItem.Content = lbGilesScoringContent;
                #endregion


                CustomSettingsTabItem.Content = tcItems;
                #endregion


                TabItem AdvancedTabItem = new TabItem();
                AdvancedTabItem.Header = "Advanced";
                tabControl1.Items.Add(AdvancedTabItem);
                ListBox lbAdvancedContent = new ListBox();

                CheckBox CBDebugStatusBar = new CheckBox
                {
                    Content = "Enable Debug Status Bar",
                    Width = 300,
                    Height = 20,
                    IsChecked = SettingsFunky.DebugStatusBar,
                };
                CBDebugStatusBar.Checked += DebugStatusBarChecked;
                CBDebugStatusBar.Unchecked += DebugStatusBarChecked;
                lbAdvancedContent.Items.Add(CBDebugStatusBar);

                CheckBox CBLogSafeMovementOutput = new CheckBox
                {
                    Content = "Enable Logging For Safe Movement",
                    Width = 300,
                    Height = 20,
                    IsChecked = SettingsFunky.LogSafeMovementOutput,
                };
                CBLogSafeMovementOutput.Checked += LogSafeMovementOutputChecked;
                CBLogSafeMovementOutput.Unchecked += LogSafeMovementOutputChecked;
                lbAdvancedContent.Items.Add(CBLogSafeMovementOutput);

                AdvancedTabItem.Content = lbAdvancedContent;

                TabItem MiscTabItem = new TabItem();
                MiscTabItem.Header = "Misc";
                tabControl1.Items.Add(MiscTabItem);
                ListBox lbMiscContent = new ListBox();
                try
                {
                    lbMiscContent.Items.Add(ReturnLogOutputString());
                }
                catch
                {
                    lbMiscContent.Items.Add("Exception Handled");
                }

                MiscTabItem.Content = lbMiscContent;



                TabItem DebuggingTabItem = new TabItem
                {
                    Header = "Debug",
                    VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch,
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                    //Width=600,
                    //Height=600,
                    //VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch,
                    //HorizontalContentAlignment=System.Windows.HorizontalAlignment.Stretch,
                };
                tabControl1.Items.Add(DebuggingTabItem);
                DockPanel DockPanel_Debug = new DockPanel
                {
                    LastChildFill = true,
                    FlowDirection = System.Windows.FlowDirection.LeftToRight,

                };

                //ListBox lbDebugContent=new ListBox();
                //lbDebugContent.HorizontalContentAlignment=System.Windows.HorizontalAlignment.Stretch;
                //lbDebugContent.VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch;

                Button btnObjects_Debug = new Button
                {
                    Content = "Dump Object Cache",
                    Width = 150,
                    Height = 25,
                    Name = "Objects",
                };
                btnObjects_Debug.Click += DebugButtonClicked;
                Button btnObstacles_Debug = new Button
                {
                    Content = "Dump Obstacle Cache",
                    Width = 150,
                    Height = 25,
                    Name = "Obstacles",
                };
                btnObstacles_Debug.Click += DebugButtonClicked;
                Button btnSNO_Debug = new Button
                {
                    Content = "Dump SNO Cache",
                    Width = 150,
                    Height = 25,
                    Name = "SNO",
                };
                btnSNO_Debug.Click += DebugButtonClicked;
                Button btnGPC_Debug = new Button
                {
                    Content = "Reset Bot Cache",
                    Width = 150,
                    Height = 25,
                    Name = "RESET",
                };
                btnGPC_Debug.Click += DebugButtonClicked;
                Button btnMGP_Debug = new Button
                {
                    Content = "MGP Details",
                    Width = 150,
                    Height = 25,
                    Name = "MGP",
                };
                btnMGP_Debug.Click += DebugButtonClicked;
                Button btnTEST_Debug = new Button
                {
                    Content = "Test",
                    Width = 150,
                    Height = 25,
                    Name = "TEST",
                };
                btnTEST_Debug.Click += DebugButtonClicked;
                StackPanel StackPanel_DebugButtons = new StackPanel
                {
                    Height = 40,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    FlowDirection = System.Windows.FlowDirection.LeftToRight,
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,

                };
                StackPanel_DebugButtons.Children.Add(btnObjects_Debug);
                StackPanel_DebugButtons.Children.Add(btnObstacles_Debug);
                StackPanel_DebugButtons.Children.Add(btnSNO_Debug);
                StackPanel_DebugButtons.Children.Add(btnTEST_Debug);
                //StackPanel_DebugButtons.Children.Add(btnMGP_Debug);
                //StackPanel_DebugButtons.Children.Add(btnGPC_Debug);

                DockPanel.SetDock(StackPanel_DebugButtons, Dock.Top);
                DockPanel_Debug.Children.Add(StackPanel_DebugButtons);

                LBDebug = new ListBox
                {
                    SelectionMode = SelectionMode.Extended,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                    VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch,
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                };

                DockPanel_Debug.Children.Add(LBDebug);

                DebuggingTabItem.Content = DockPanel_Debug;


                this.AddChild(LBWindowContent);
            }

        }


    }
}