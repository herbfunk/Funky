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
using FunkyTrinity.Enums;

namespace FunkyTrinity
{
	 public partial class Funky
	 {


			private static void buttonFunkySettingDB_Click(object sender, RoutedEventArgs e)
			{
				 Bot.UpdateCurrentAccountDetails();

				 string settingsFolder=FolderPaths.sDemonBuddyPath+@"\Settings\FunkyTrinity\"+Bot.CurrentAccountName;
				 if (!Directory.Exists(settingsFolder)) Directory.CreateDirectory(settingsFolder);

				 try
				 {
						funkyConfigWindow=new FunkyWindow();
						funkyConfigWindow.Show();
				 }
				 catch (Exception ex)
				 {
						Logging.WriteVerbose("Failure to initilize Funky Setting Window! \r\n {0} \r\n {1} \r\n {2}", ex.Message, ex.Source, ex.StackTrace);
				 }
			}

			internal static FunkyWindow funkyConfigWindow;

			[System.Runtime.InteropServices.ComVisible(false)]
			internal partial class FunkyWindow : Window
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
				 private StackPanel spFleeingOptions, SPFleeing, spGroupingOptions, spBotStop;
				 private StackPanel spBlacksmithPlans, spJewelerPlans;
				 private TextBox TBGroupingMinUnitDistance, TBGroupingMaxDistance, TBGroupingMinimumClusterCount, TBGroupingMinimumUnitsInCluster;

				 private CheckBox CoffeeBreaks;
				 private TextBox tbMinBreakTime, tbMaxBreakTime;

				 private Button OpenPluginFolder;

				 private TextBox[] TBavoidanceRadius;
				 private TextBox[] TBavoidanceHealth;

				 private TextBox[] TBMinimumWeaponLevel;
				 private TextBox[] TBMinimumArmorLevel;
				 private TextBox[] TBMinimumJeweleryLevel;
				 private CheckBox[] CBGems;
				 //private ComboBox CBGemQualityLevel;

				 private TextBox TBBreakTimeHour, TBKiteDistance, TBGlobeHealth, TBPotionHealth, TBContainerRange, TBNonEliteRange, TBDestructibleRange, TBAfterCombatDelay, TBiDHVaultMovementDelay, TBShrineRange, TBEliteRange, TBExtendedCombatRange, TBGoldRange, TBMinLegendaryLevel, TBMaxHealthPots, TBMinGoldPile, TBMiscItemLevel, TBGilesWeaponScore, TBGilesArmorScore, TBGilesJeweleryScore, TBClusterDistance, TBClusterMinUnitCount, TBItemRange, TBGoblinRange, TBGoblinMinRange, TBClusterLowHPValue, TBGlobeRange, TBFleemonsterDistance, TBFleeMinimumHealth, TBBotStopHealthPercent;
				 private TextBox[] TBKiteTimeLimits;
				 private TextBox[] TBAvoidanceTimeLimits;

				 private TextBox tbCustomItemRulePath;

				 private ListBox LBDebug;


				 public FunkyWindow()
				 {
						Settings_Funky.LoadFunkyConfiguration();

						this.Owner=Demonbuddy.App.Current.MainWindow;
						this.Title="Funky Settings -- " + Bot.CurrentHeroName;
						this.SizeToContent=System.Windows.SizeToContent.WidthAndHeight;
						this.ResizeMode=System.Windows.ResizeMode.CanMinimize;
						this.Background=System.Windows.Media.Brushes.Black;
						this.Foreground=System.Windows.Media.Brushes.PaleGoldenrod;
						//this.Width=600;
						//this.Height=600;

						ListBox LBWindowContent=new ListBox
						{
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
							 VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
						};

						StackPanel StackPanelTopWindow=new StackPanel
						{
							 Height=40,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
							 FlowDirection=System.Windows.FlowDirection.LeftToRight,
							 Orientation=Orientation.Horizontal,
						};
						Button OpenPluginSettings=new Button
						{
							 Content="Open Plugin Settings",
							 Width=150,
							 Height=25,
						};
						OpenPluginSettings.Click+=OpenPluginSettings_Click;
						StackPanelTopWindow.Children.Add(OpenPluginSettings);


						OpenPluginFolder=new Button
						{
							 Content="Open Trinity Folder",
							 Width=150,
							 Height=25,
						};
						OpenPluginFolder.Click+=OpenPluginFolder_Click;
						StackPanelTopWindow.Children.Add(OpenPluginFolder);

						//LBWindowContent.Items.Add(StackPanelTopWindow);

						TabControl tabControl1=new TabControl
						{
							 Width=600,
							 Height=600,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
							 VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
							 VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch,
							 HorizontalContentAlignment=System.Windows.HorizontalAlignment.Stretch,
							 FontSize=12,
						};
						LBWindowContent.Items.Add(tabControl1);

						#region Combat
						//Character
						TabItem CombatSettingsTabItem=new TabItem();
						CombatSettingsTabItem.Header="Combat";

						tabControl1.Items.Add(CombatSettingsTabItem);
						TabControl CombatTabControl=new TabControl
						{
							 HorizontalAlignment=HorizontalAlignment.Stretch,
							 VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
						};

						#region General
						//Combat
						TabItem CombatGeneralTabItem=new TabItem();
						CombatGeneralTabItem.Header="General";
						CombatTabControl.Items.Add(CombatGeneralTabItem);
						ListBox CombatGeneralContentListBox=new ListBox();
						#region Avoidances

						StackPanel AvoidanceOptionsStackPanel=new StackPanel
						{
							 //Orientation= System.Windows.Controls.Orientation.Vertical,
							 //HorizontalAlignment= System.Windows.HorizontalAlignment.Stretch,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 Background=System.Windows.Media.Brushes.DimGray,
						};

						TextBlock Avoidance_Text_Header=new TextBlock
						{
							 Text="Avoidances",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.MediumSeaGreen,
							 TextAlignment=TextAlignment.Center,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						};

						#region AvoidanceCheckboxes

						StackPanel AvoidanceCheckBoxesPanel=new StackPanel
						{
							 Orientation=Orientation.Vertical,
							 Width=600,
						};

						CheckBox CBAttemptAvoidanceMovements=new CheckBox
						{
							 Content="Enable Avoidance",
							 IsChecked=Bot.SettingsFunky.AttemptAvoidanceMovements,

						};
						CBAttemptAvoidanceMovements.Checked+=AvoidanceAttemptMovementChecked;
						CBAttemptAvoidanceMovements.Unchecked+=AvoidanceAttemptMovementChecked;

						CheckBox CBAdvancedProjectileTesting=new CheckBox
						{
							 Content="Use Advanced Avoidance Projectile Test",
							 IsChecked=Bot.SettingsFunky.UseAdvancedProjectileTesting,
						};
						CBAdvancedProjectileTesting.Checked+=UseAdvancedProjectileTestingChecked;
						CBAdvancedProjectileTesting.Unchecked+=UseAdvancedProjectileTestingChecked;
						AvoidanceCheckBoxesPanel.Children.Add(CBAttemptAvoidanceMovements);
						AvoidanceCheckBoxesPanel.Children.Add(CBAdvancedProjectileTesting);
						#endregion;


						#region AvoidanceRetryTextInfo
						StackPanel AvoidDelayStackPanel=new StackPanel();
						TextBlock Avoid_Delay_Text=new TextBlock
						{
							 Text="Delay invtervals",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Left,
						};
						TextBlock Avoid_DelayInfo_Text=new TextBlock
						{
							 Text="Minimum is delay used if failed, Maximum is delay after successful searching",
							 FontSize=9,
							 FontStyle=FontStyles.Italic,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Left,
						};
						AvoidDelayStackPanel.Children.Add(Avoid_Delay_Text);
						AvoidDelayStackPanel.Children.Add(Avoid_DelayInfo_Text);
						#endregion


						#region AvoidanceRetry


						TextBlock Avoid_Retry_Min_Text=new TextBlock
						{
							 Text="Minimum",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Center,
						};
						Slider sliderAvoidMinimumRetry=new Slider
						{
							 Width=120,
							 Maximum=10000,
							 Minimum=0,
							 TickFrequency=500,
							 LargeChange=1000,
							 SmallChange=50,
							 Value=Bot.SettingsFunky.AvoidanceRecheckMinimumRate,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(6),
						};
						sliderAvoidMinimumRetry.ValueChanged+=AvoidanceMinimumRetrySliderChanged;
						TBAvoidanceTimeLimits=new TextBox[2];
						TBAvoidanceTimeLimits[0]=new TextBox
						{
							 Text=Bot.SettingsFunky.AvoidanceRecheckMinimumRate.ToString(),
							 IsReadOnly=true,
						};

						TextBlock Avoid_Retry_Max_Text=new TextBlock
						{
							 Text="Maximum",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Center,
						};

						Slider sliderAvoidMaximumRetry=new Slider
						{
							 Width=120,
							 Maximum=10000,
							 Minimum=0,
							 TickFrequency=500,
							 LargeChange=1000,
							 SmallChange=50,
							 Value=Bot.SettingsFunky.AvoidanceRecheckMaximumRate,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(6),
						};
						sliderAvoidMaximumRetry.ValueChanged+=AvoidanceMaximumRetrySliderChanged;
						TBAvoidanceTimeLimits[1]=new TextBox
						{
							 Text=Bot.SettingsFunky.AvoidanceRecheckMaximumRate.ToString(),
							 IsReadOnly=true,
						};

						StackPanel AvoidRetryTimeStackPanel=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top+5, Margin.Right, Margin.Bottom+5),
							 Orientation=Orientation.Horizontal,
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
						//AvoidanceOptionsStackPanel.Children.Add(AvoidDelayStackPanel);
						//AvoidanceOptionsStackPanel.Children.Add(AvoidRetryTimeStackPanel);
						CombatGeneralContentListBox.Items.Add(AvoidanceOptionsStackPanel);

						#endregion

						#region Fleeing

						ToolTip TTFleeInfo=new System.Windows.Controls.ToolTip
						{
							 Content="Trys to move away from any monsters that are within the distance set",
						};
						SPFleeing=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 Background=System.Windows.Media.Brushes.DimGray,
							 ToolTip=TTFleeInfo,
						};
						TextBlock Flee_Text_Header=new TextBlock
						{
							 Text="Fleeing",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.SeaGreen,
							 TextAlignment=TextAlignment.Center,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
							 ToolTip=TTFleeInfo,
						};

						CheckBox CBAttemptFleeingBehavior=new CheckBox
						{
							 Content="Enable Fleeing",
							 IsChecked=Bot.SettingsFunky.EnableFleeingBehavior,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
						};
						CBAttemptFleeingBehavior.Checked+=FleeingAttemptMovementChecked;
						CBAttemptFleeingBehavior.Unchecked+=FleeingAttemptMovementChecked;

						spFleeingOptions=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 IsEnabled=Bot.SettingsFunky.EnableFleeingBehavior,
						};
						#region Fleeing Monster Distance
						TextBlock Flee_MonsterDistance_Label=new TextBlock
						{
							 Text="Maximum Monster Distance",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 //Background = System.Windows.Media.Brushes.Crimson,
							 TextAlignment=TextAlignment.Left,
						};

						Slider sliderFleeMonsterDistance=new Slider
						{
							 Width=100,
							 Maximum=20,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.FleeMaxMonsterDistance,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderFleeMonsterDistance.ValueChanged+=FleeMonsterDistanceSliderChanged;
						TBFleemonsterDistance=new TextBox
						{
							 Text=Bot.SettingsFunky.FleeMaxMonsterDistance.ToString(),
							 IsReadOnly=true,
						};
						StackPanel FleeMonsterDistanceStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+6),
						};

						FleeMonsterDistanceStackPanel.Children.Add(sliderFleeMonsterDistance);
						FleeMonsterDistanceStackPanel.Children.Add(TBFleemonsterDistance);
						#endregion

						#region Fleeing Minimum Health Percent
						TextBlock Flee_HealthPercent_Label=new TextBlock
						{
							 Text="Bot Min Health Percent",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 //Background = System.Windows.Media.Brushes.Crimson,
							 TextAlignment=TextAlignment.Left,
						};


						Slider sliderFleeHealthPercent=new Slider
						{
							 Width=100,
							 Maximum=1,
							 Minimum=0,
							 TickFrequency=0.25,
							 LargeChange=0.1,
							 SmallChange=0.05,
							 Value=Bot.SettingsFunky.FleeBotMinimumHealthPercent,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderFleeHealthPercent.ValueChanged+=FleeMinimumHealthSliderChanged;
						TBFleeMinimumHealth=new TextBox
						{
							 Text=Bot.SettingsFunky.FleeBotMinimumHealthPercent.ToString(),
							 IsReadOnly=true,
						};
						StackPanel FleeMinimumHealthtackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+6),
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


						CombatGeneralContentListBox.Items.Add(SPFleeing);
						#endregion

						#region Grouping


						ToolTip TTGrouping=new System.Windows.Controls.ToolTip
						{
							 Content="Attempts to engage additional nearby monster groups",
						};
						StackPanel GroupingOptionsStackPanel=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 Background=System.Windows.Media.Brushes.DimGray,
							 ToolTip=TTGrouping,
						};
						TextBlock Grouping_Text_Header=new TextBlock
						{
							 Text="Grouping",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.LightSeaGreen,
							 TextAlignment=TextAlignment.Center,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
							 ToolTip=TTGrouping,
						};
						GroupingOptionsStackPanel.Children.Add(Grouping_Text_Header);
						CheckBox CBGroupingBehavior=new CheckBox
						{
							 Content="Enable Grouping",
							 Height=20,
							 HorizontalContentAlignment=HorizontalAlignment.Left,
							 IsChecked=Bot.SettingsFunky.AttemptGroupingMovements,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
						};
						CBGroupingBehavior.Checked+=GroupingBehaviorChecked;
						CBGroupingBehavior.Unchecked+=GroupingBehaviorChecked;
						GroupingOptionsStackPanel.Children.Add(CBGroupingBehavior);

						spGroupingOptions=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 IsEnabled=Bot.SettingsFunky.AttemptGroupingMovements,
						};

						#region Grouping Minimum Distance
						TextBlock Group_MinimumUnitDistance_Label=new TextBlock
						{
							 Text="Distant-Unit Minimum Distance",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 //Background = System.Windows.Media.Brushes.Crimson,
							 TextAlignment=TextAlignment.Left,
						};

						Slider sliderGroupingMinimumUnitDistance=new Slider
						{
							 Width=100,
							 Maximum=100,
							 Minimum=35,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.GroupingMinimumUnitDistance,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderGroupingMinimumUnitDistance.ValueChanged+=GroupMinimumUnitDistanceSliderChanged;
						TBGroupingMinUnitDistance=new TextBox
						{
							 Text=Bot.SettingsFunky.GroupingMinimumUnitDistance.ToString(),
							 IsReadOnly=true,
						};
						StackPanel GroupingMinUnitDistanceStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+6),
						};

						GroupingMinUnitDistanceStackPanel.Children.Add(sliderGroupingMinimumUnitDistance);
						GroupingMinUnitDistanceStackPanel.Children.Add(TBGroupingMinUnitDistance);
						#endregion

						spGroupingOptions.Children.Add(Group_MinimumUnitDistance_Label);
						spGroupingOptions.Children.Add(GroupingMinUnitDistanceStackPanel);

						#region Grouping Maximum Distance
						TextBlock Group_MonsterDistance_Label=new TextBlock
						{
							 Text="Grouping Maximum Distance",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 //Background = System.Windows.Media.Brushes.Crimson,
							 TextAlignment=TextAlignment.Left,
						};

						Slider sliderGroupingMaximumDistance=new Slider
						{
							 Width=100,
							 Maximum=200,
							 Minimum=50,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.GroupingMaximumDistanceAllowed,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderGroupingMaximumDistance.ValueChanged+=GroupMaxDistanceSliderChanged;
						TBGroupingMaxDistance=new TextBox
						{
							 Text=Bot.SettingsFunky.GroupingMaximumDistanceAllowed.ToString(),
							 IsReadOnly=true,
						};
						StackPanel GroupingMaxDistanceStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+6),
						};

						GroupingMaxDistanceStackPanel.Children.Add(sliderGroupingMaximumDistance);
						GroupingMaxDistanceStackPanel.Children.Add(TBGroupingMaxDistance);
						#endregion

						spGroupingOptions.Children.Add(Group_MonsterDistance_Label);
						spGroupingOptions.Children.Add(GroupingMaxDistanceStackPanel);

						#region Grouping Minimum Cluster Count
						TextBlock Group_MinimumCluster_Label=new TextBlock
						{
							 Text="Minimum Cluster Count",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 //Background = System.Windows.Media.Brushes.Crimson,
							 TextAlignment=TextAlignment.Left,
						};

						Slider sliderGroupingMinimumCluster=new Slider
						{
							 Width=100,
							 Maximum=8,
							 Minimum=1,
							 TickFrequency=1,
							 LargeChange=1,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.GroupingMinimumClusterCount,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderGroupingMinimumCluster.ValueChanged+=GroupMinimumClusterCountSliderChanged;
						TBGroupingMinimumClusterCount=new TextBox
						{
							 Text=Bot.SettingsFunky.GroupingMinimumClusterCount.ToString(),
							 IsReadOnly=true,
						};
						StackPanel GroupingMinimumClusterCountStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+6),
						};

						GroupingMinimumClusterCountStackPanel.Children.Add(sliderGroupingMinimumCluster);
						GroupingMinimumClusterCountStackPanel.Children.Add(TBGroupingMinimumClusterCount);
						#endregion

						spGroupingOptions.Children.Add(Group_MinimumCluster_Label);
						spGroupingOptions.Children.Add(GroupingMinimumClusterCountStackPanel);

						#region Grouping Minimum Units In Cluster
						TextBlock Group_MinimumUnits_Label=new TextBlock
						{
							 Text="Minimum Units In Cluster",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 //Background = System.Windows.Media.Brushes.Crimson,
							 TextAlignment=TextAlignment.Left,
						};

						Slider sliderGroupingMinimumUnits=new Slider
						{
							 Width=100,
							 Maximum=10,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=2,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.GroupingMinimumUnitsInCluster,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderGroupingMinimumUnits.ValueChanged+=GroupMinimumUnitsInClusterSliderChanged;
						TBGroupingMinimumUnitsInCluster=new TextBox
						{
							 Text=Bot.SettingsFunky.GroupingMinimumUnitsInCluster.ToString(),
							 IsReadOnly=true,
						};
						StackPanel GroupingMinimumUnitstStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+6),
						};

						GroupingMinimumUnitstStackPanel.Children.Add(sliderGroupingMinimumUnits);
						GroupingMinimumUnitstStackPanel.Children.Add(TBGroupingMinimumUnitsInCluster);
						#endregion

						spGroupingOptions.Children.Add(Group_MinimumUnits_Label);
						spGroupingOptions.Children.Add(GroupingMinimumUnitstStackPanel);


						GroupingOptionsStackPanel.Children.Add(spGroupingOptions);
						CombatGeneralContentListBox.Items.Add(GroupingOptionsStackPanel);

						#endregion

						#region HealthOptions
						StackPanel HealthOptionsStackPanel=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 Background=System.Windows.Media.Brushes.DimGray,
						};
						TextBlock Health_Options_Text=new TextBlock
						{
							 Text="Health",
							 FontSize=13,
							 Background=System.Windows.Media.Brushes.DarkSeaGreen,
							 TextAlignment=TextAlignment.Center,
						};
						TextBlock Health_Info_Text=new TextBlock
						{
							 Text="Actions will occur when life is below given value",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 FontStyle=FontStyles.Italic,
							 //Background = System.Windows.Media.Brushes.Crimson,
							 TextAlignment=TextAlignment.Left,
						};

						TextBlock HealthGlobe_Info_Text=new TextBlock
						{
							 Text="Globe Health Percent",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 //Background = System.Windows.Media.Brushes.Crimson,
							 TextAlignment=TextAlignment.Left,
						};
						#region GlobeHealthPercent

						Slider sliderGlobeHealth=new Slider
						{
							 Width=100,
							 Maximum=1,
							 Minimum=0,
							 TickFrequency=0.25,
							 LargeChange=0.20,
							 SmallChange=0.10,
							 Value=Bot.SettingsFunky.GlobeHealthPercent,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderGlobeHealth.ValueChanged+=GlobeHealthSliderChanged;
						TBGlobeHealth=new TextBox
						{
							 Text=Bot.SettingsFunky.GlobeHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
							 IsReadOnly=true,
						};
						StackPanel GlobeHealthStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						GlobeHealthStackPanel.Children.Add(sliderGlobeHealth);
						GlobeHealthStackPanel.Children.Add(TBGlobeHealth);
						#endregion

						TextBlock HealthPotion_Info_Text=new TextBlock
						{
							 Text="Potion Health Percent",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 //Background = System.Windows.Media.Brushes.Crimson,
							 TextAlignment=TextAlignment.Left,
						};

						#region PotionHealthPercent

						Slider sliderPotionHealth=new Slider
						{
							 Width=100,
							 Maximum=1,
							 Minimum=0,
							 TickFrequency=0.25,
							 LargeChange=0.20,
							 SmallChange=0.10,
							 Value=Bot.SettingsFunky.PotionHealthPercent,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderPotionHealth.ValueChanged+=PotionHealthSliderChanged;
						TBPotionHealth=new TextBox
						{
							 Text=Bot.SettingsFunky.PotionHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
							 IsReadOnly=true,
						};
						StackPanel PotionHealthStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
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

						#endregion

						CombatGeneralTabItem.Content=CombatGeneralContentListBox;
						#endregion

						#region Clustering
						TabItem CombatClusterTabItem=new TabItem();
						CombatClusterTabItem.Header="Clustering";
						CombatTabControl.Items.Add(CombatClusterTabItem);
						ListBox CombatClusteringContentListBox=new ListBox();

						ToolTip TTClustering=new System.Windows.Controls.ToolTip
						{
							 Content="Determines eligible targets using only units from valid clusters",
						};
						StackPanel spClusteringOptions=new StackPanel
						{
							 Background=System.Windows.Media.Brushes.DimGray,
							 ToolTip=TTClustering,
						};
						TextBlock Clustering_Text_Header=new TextBlock
						{
							 Text="Target Clustering Options",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Background=System.Windows.Media.Brushes.DarkGreen,
							 TextAlignment=TextAlignment.Center,
							 ToolTip=TTClustering,
						};
						spClusteringOptions.Children.Add(Clustering_Text_Header);

						#region ClusterTargetLogic
						CheckBox cbClusterEnabled=new CheckBox
						{
							 Content="Enable Clustering Target Logic",
							 IsChecked=(Bot.SettingsFunky.EnableClusteringTargetLogic),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+10),
						};
						cbClusterEnabled.Checked+=EnableClusteringTargetLogicChecked;
						cbClusterEnabled.Unchecked+=EnableClusteringTargetLogicChecked;
						spClusteringOptions.Children.Add(cbClusterEnabled);
						#endregion

						#region LowHP
						StackPanel spClusterLowHPOption=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};

						#region ClusterLowHPSliderValue


						StackPanel spClusterLowHP=new StackPanel
						{
							 Orientation=Orientation.Vertical,
						};
						TextBlock ClusterLowHP_Text_Header=new TextBlock
						{
							 Text="Disable Health Percent",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 //Background=System.Windows.Media.Brushes.MediumSeaGreen,
						};
						spClusterLowHP.Children.Add(ClusterLowHP_Text_Header);

						Slider sliderClusterLowHPValue=new Slider
						{
							 Width=100,
							 Maximum=1,
							 Minimum=0,
							 TickFrequency=0.25,
							 LargeChange=0.25,
							 SmallChange=0.10,
							 Value=Bot.SettingsFunky.IgnoreClusterLowHPValue,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderClusterLowHPValue.ValueChanged+=ClusterLowHPValueSliderChanged;
						TBClusterLowHPValue=new TextBox
						{
							 Text=Bot.SettingsFunky.IgnoreClusterLowHPValue.ToString("F2", CultureInfo.InvariantCulture),
							 IsReadOnly=true,
						};
						StackPanel ClusterLowHPValueStackPanel=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						ClusterLowHPValueStackPanel.Children.Add(sliderClusterLowHPValue);
						ClusterLowHPValueStackPanel.Children.Add(TBClusterLowHPValue);
						spClusterLowHP.Children.Add(ClusterLowHPValueStackPanel);

						spClusterLowHPOption.Children.Add(spClusterLowHP);



						#endregion

						CheckBox cbClusterIgnoreBotLowHP=new CheckBox
						{
							 Content="Cluster Logic Disable at HP %",
							 IsChecked=(Bot.SettingsFunky.IgnoreClusteringWhenLowHP),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
							 VerticalAlignment=System.Windows.VerticalAlignment.Bottom,
							 Margin=new Thickness(Margin.Left+5, Margin.Top, Margin.Right, Margin.Bottom),
						};
						cbClusterIgnoreBotLowHP.Checked+=IgnoreClusteringBotLowHPisChecked;
						cbClusterIgnoreBotLowHP.Unchecked+=IgnoreClusteringBotLowHPisChecked;
						spClusterLowHPOption.Children.Add(cbClusterIgnoreBotLowHP);
						spClusteringOptions.Children.Add(spClusterLowHPOption);
						#endregion

						#region ClusterDistance
						StackPanel spClusterDistanceOptions=new StackPanel
						{
							 Orientation=Orientation.Vertical,
						};
						TextBlock ClusterDistance_Text_Header=new TextBlock
						{
							 Text="Cluster Distance",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
						};
						spClusterDistanceOptions.Children.Add(ClusterDistance_Text_Header);

						Slider sliderClusterDistance=new Slider
						{
							 Width=100,
							 Maximum=20,
							 Minimum=0,
							 TickFrequency=4,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.ClusterDistance,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderClusterDistance.ValueChanged+=ClusterDistanceSliderChanged;
						TBClusterDistance=new TextBox
						{
							 Text=Bot.SettingsFunky.ClusterDistance.ToString(),
							 IsReadOnly=true,
						};
						StackPanel ClusterDistanceStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						ClusterDistanceStackPanel.Children.Add(sliderClusterDistance);
						ClusterDistanceStackPanel.Children.Add(TBClusterDistance);
						spClusterDistanceOptions.Children.Add(ClusterDistanceStackPanel);
						spClusteringOptions.Children.Add(spClusterDistanceOptions);
						#endregion

						#region ClusterMinUnitCount
						StackPanel spClusterMinUnitOptions=new StackPanel
						{
							 Orientation=Orientation.Vertical,
						};
						TextBlock ClusterMinUnitCount_Text_Header=new TextBlock
						{
							 Text="Cluster Minimum Unit Count",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
						};
						spClusterMinUnitOptions.Children.Add(ClusterMinUnitCount_Text_Header);

						Slider sliderClusterMinUnitCount=new Slider
						{
							 Width=100,
							 Maximum=10,
							 Minimum=1,
							 TickFrequency=2,
							 LargeChange=2,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.ClusterMinimumUnitCount,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderClusterMinUnitCount.ValueChanged+=ClusterMinUnitSliderChanged;
						TBClusterMinUnitCount=new TextBox
						{
							 Text=Bot.SettingsFunky.ClusterMinimumUnitCount.ToString(),
							 IsReadOnly=true,
						};
						StackPanel ClusterMinUnitCountStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						ClusterMinUnitCountStackPanel.Children.Add(sliderClusterMinUnitCount);
						ClusterMinUnitCountStackPanel.Children.Add(TBClusterMinUnitCount);
						spClusterMinUnitOptions.Children.Add(ClusterMinUnitCountStackPanel);
						spClusteringOptions.Children.Add(spClusterMinUnitOptions);
						#endregion

						CombatClusteringContentListBox.Items.Add(spClusteringOptions);



						StackPanel spClusteringExceptions=new StackPanel
						{
							 Background=System.Windows.Media.Brushes.DimGray,
						};
						TextBlock ClusteringExceptions_Text_Header=new TextBlock
						{
							 Text="Clustering Exceptions",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Background=System.Windows.Media.Brushes.OrangeRed,
							 TextAlignment=TextAlignment.Center,
						};
						spClusteringExceptions.Children.Add(ClusteringExceptions_Text_Header);

						#region KillLOWHPUnits
						CheckBox cbClusterKillLowHPUnits=new CheckBox
						{
							 Content="Allow Units with 25% or less HP",
							 Width=300,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.ClusterKillLowHPUnits),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						cbClusterKillLowHPUnits.Checked+=ClusteringKillLowHPChecked;
						cbClusterKillLowHPUnits.Unchecked+=ClusteringKillLowHPChecked;
						spClusteringExceptions.Children.Add(cbClusterKillLowHPUnits);
						#endregion

						#region AllowRangedUnits
						CheckBox cbClusteringAllowRangedUnits=new CheckBox
						{
							 Content="Allow Ranged Units",
							 Width=300,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.ClusteringAllowRangedUnits),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						cbClusteringAllowRangedUnits.Checked+=ClusteringAllowRangedUnitsChecked;
						cbClusteringAllowRangedUnits.Unchecked+=ClusteringAllowRangedUnitsChecked;
						spClusteringExceptions.Children.Add(cbClusteringAllowRangedUnits);
						#endregion

						#region AllowSpawnerUnits
						CheckBox cbClusteringAllowSpawnerUnits=new CheckBox
						{
							 Content="Allow Spawner Units",
							 Width=300,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.ClusteringAllowSpawnerUnits),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						cbClusteringAllowSpawnerUnits.Checked+=ClusteringAllowSpawnerUnitsChecked;
						cbClusteringAllowSpawnerUnits.Unchecked+=ClusteringAllowSpawnerUnitsChecked;
						spClusteringExceptions.Children.Add(cbClusteringAllowSpawnerUnits);
						#endregion

						CombatClusteringContentListBox.Items.Add(spClusteringExceptions);


						CombatClusterTabItem.Content=CombatClusteringContentListBox;



						#endregion


						#region Avoidances
						TabItem AvoidanceTabItem=new TabItem
						{
							 Header="Avoidances",
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
							 VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
						};
						AvoidanceTabItem.Header="Avoidances";
						CombatTabControl.Items.Add(AvoidanceTabItem);
						ListBox LBcharacterAvoidance=new ListBox
						{
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
							 VerticalAlignment=System.Windows.VerticalAlignment.Stretch,

						};




						Grid AvoidanceLayoutGrid=new Grid
						{
							 UseLayoutRounding=true,
							 ShowGridLines=false,
							 VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
							 FlowDirection=System.Windows.FlowDirection.LeftToRight,
							 Focusable=false,
						};

						ColumnDefinition colDef1=new ColumnDefinition();
						ColumnDefinition colDef2=new ColumnDefinition();
						ColumnDefinition colDef3=new ColumnDefinition();
						AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef1);
						AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef2);
						AvoidanceLayoutGrid.ColumnDefinitions.Add(colDef3);
						RowDefinition rowDef1=new RowDefinition();
						AvoidanceLayoutGrid.RowDefinitions.Add(rowDef1);

						TextBlock ColumnHeader1=new TextBlock
						{
							 Text="Type",
							 FontSize=12,
							 TextAlignment=System.Windows.TextAlignment.Center,
							 Background=System.Windows.Media.Brushes.DarkTurquoise,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
						};
						TextBlock ColumnHeader2=new TextBlock
						{
							 Text="Radius",
							 FontSize=12,
							 TextAlignment=System.Windows.TextAlignment.Center,
							 Background=System.Windows.Media.Brushes.DarkGoldenrod,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
						};
						TextBlock ColumnHeader3=new TextBlock
						{
							 Text="Health",
							 FontSize=12,
							 TextAlignment=System.Windows.TextAlignment.Center,
							 Background=System.Windows.Media.Brushes.DarkRed,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
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

						Dictionary<AvoidanceType, double> currentDictionaryAvoidance=ReturnDictionaryUsingActorClass(Bot.ActorClass);
						AvoidanceType[] avoidanceTypes=currentDictionaryAvoidance.Keys.ToArray();
						TBavoidanceHealth=new TextBox[avoidanceTypes.Length-1];
						TBavoidanceRadius=new TextBox[avoidanceTypes.Length-1];
						int alternatingColor=0;

						for (int i=0; i<avoidanceTypes.Length-1; i++)
						{
							 if (alternatingColor>1) alternatingColor=0;

							 string avoidanceString=avoidanceTypes[i].ToString();

							 float defaultRadius=0f;
							 dictAvoidanceRadius.TryGetValue(avoidanceTypes[i], out defaultRadius);
							 Slider avoidanceRadius=new Slider
							 {
									Width=125,
									Name=avoidanceString+"_radius_"+i.ToString(),
									Maximum=30,
									Minimum=0,
									TickFrequency=5,
									LargeChange=5,
									SmallChange=1,
									Value=defaultRadius,
									HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
									VerticalAlignment=System.Windows.VerticalAlignment.Center,
									//Padding=new Thickness(2),
									Margin=new Thickness(5),
							 };
							 avoidanceRadius.ValueChanged+=AvoidanceRadiusSliderValueChanged;
							 TBavoidanceRadius[i]=new TextBox
							 {
									Text=defaultRadius.ToString(),
									IsReadOnly=true,
									VerticalAlignment=System.Windows.VerticalAlignment.Top,
									HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
							 };

							 double defaultHealth=0d;
							 ReturnDictionaryUsingActorClass(Bot.ActorClass).TryGetValue(avoidanceTypes[i], out defaultHealth);
							 Slider avoidanceHealth=new Slider
							 {
									Name=avoidanceString+"_health_"+i.ToString(),
									Width=125,
									Maximum=1,
									Minimum=0,
									TickFrequency=0.10,
									LargeChange=0.10,
									SmallChange=0.05,
									Value=defaultHealth,
									HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
									VerticalAlignment=System.Windows.VerticalAlignment.Center,
									Margin=new Thickness(5),
							 };
							 avoidanceHealth.ValueChanged+=AvoidanceHealthSliderValueChanged;
							 TBavoidanceHealth[i]=new TextBox
							 {
									Text=defaultHealth.ToString("F2", CultureInfo.InvariantCulture),
									IsReadOnly=true,
									VerticalAlignment=System.Windows.VerticalAlignment.Top,
									HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
							 };

							 RowDefinition newRow=new RowDefinition();
							 AvoidanceLayoutGrid.RowDefinitions.Add(newRow);


							 TextBlock txt1=new TextBlock
							 {
									Text=avoidanceString,
									FontSize=12,
									VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
									Background=alternatingColor==0?System.Windows.Media.Brushes.DarkSeaGreen:Background=System.Windows.Media.Brushes.SlateGray,
									Foreground=System.Windows.Media.Brushes.GhostWhite,
									FontStretch=FontStretches.SemiCondensed,
							 };

							 StackPanel avoidRadiusStackPanel=new StackPanel
							 {
									Width=175,
									Height=25,
									Orientation=Orientation.Horizontal,
									Background=alternatingColor==0?System.Windows.Media.Brushes.DarkSeaGreen:Background=System.Windows.Media.Brushes.SlateGray,

							 };
							 avoidRadiusStackPanel.Children.Add(avoidanceRadius);
							 avoidRadiusStackPanel.Children.Add(TBavoidanceRadius[i]);

							 StackPanel avoidHealthStackPanel=new StackPanel
							 {
									Width=175,
									Height=25,
									Orientation=Orientation.Horizontal,
									Background=alternatingColor==0?System.Windows.Media.Brushes.DarkSeaGreen:Background=System.Windows.Media.Brushes.SlateGray,

							 };
							 avoidHealthStackPanel.Children.Add(avoidanceHealth);
							 avoidHealthStackPanel.Children.Add(TBavoidanceHealth[i]);

							 Grid.SetColumn(txt1, 0);
							 Grid.SetColumn(avoidRadiusStackPanel, 1);
							 Grid.SetColumn(avoidHealthStackPanel, 2);

							 int currentIndex=AvoidanceLayoutGrid.RowDefinitions.Count-1;
							 Grid.SetRow(avoidRadiusStackPanel, currentIndex);
							 Grid.SetRow(avoidHealthStackPanel, currentIndex);
							 Grid.SetRow(txt1, currentIndex);

							 AvoidanceLayoutGrid.Children.Add(txt1);
							 AvoidanceLayoutGrid.Children.Add(avoidRadiusStackPanel);
							 AvoidanceLayoutGrid.Children.Add(avoidHealthStackPanel);
							 alternatingColor++;
						}

						LBcharacterAvoidance.Items.Add(AvoidanceLayoutGrid);


						AvoidanceTabItem.Content=LBcharacterAvoidance;
						#endregion


						#region ClassSettings
						//Class Specific
						TabItem ClassTabItem=new TabItem();
						ClassTabItem.Header="Class";
						CombatTabControl.Items.Add(ClassTabItem);
						ListBox LBClass=new ListBox();

						switch (Bot.ActorClass)
						{
							 case Zeta.Internals.Actors.ActorClass.Barbarian:
									CheckBox cbbSelectiveWhirlwind=new CheckBox
									{
										 Content="Selective Whirlwind Targeting",
										 Width=300,
										 Height=30,
										 IsChecked=(Bot.SettingsFunky.Class.bSelectiveWhirlwind)
									};
									cbbSelectiveWhirlwind.Checked+=bSelectiveWhirlwindChecked;
									cbbSelectiveWhirlwind.Unchecked+=bSelectiveWhirlwindChecked;
									LBClass.Items.Add(cbbSelectiveWhirlwind);

									TextBlock txtblockWrathOptions=new TextBlock
									{
										 Text="Wrath of the Berserker Options",
										 FontStyle=FontStyles.Oblique,
										 Foreground=System.Windows.Media.Brushes.GhostWhite,
										 FontSize=11,
										 TextAlignment=TextAlignment.Left,
									};
									LBClass.Items.Add(txtblockWrathOptions);

									StackPanel spWrathOptions=new StackPanel
									{
										 Orientation=Orientation.Horizontal,
									};
									CheckBox cbbWaitForWrath=new CheckBox
									{
										 Content="Wait for Wrath",
										 Height=30,
										 IsChecked=(Bot.SettingsFunky.Class.bWaitForWrath),
										 Margin=new Thickness(5),
									};
									cbbWaitForWrath.Checked+=bWaitForWrathChecked;
									cbbWaitForWrath.Unchecked+=bWaitForWrathChecked;
									spWrathOptions.Children.Add(cbbWaitForWrath);

									CheckBox cbbGoblinWrath=new CheckBox
									{
										 Content="Use Wrath on Goblins",
										 Height=30,
										 IsChecked=(Bot.SettingsFunky.Class.bGoblinWrath),
										 Margin=new Thickness(5),
									};
									cbbGoblinWrath.Checked+=bGoblinWrathChecked;
									cbbGoblinWrath.Unchecked+=bGoblinWrathChecked;
									spWrathOptions.Children.Add(cbbGoblinWrath);

									CheckBox cbbBarbUseWOTBAlways=new CheckBox
													{
														 Content="Use Wrath on Always",
														 Height=30,
														 IsChecked=(Bot.SettingsFunky.Class.bBarbUseWOTBAlways),
														 Margin=new Thickness(5),
													};
									cbbBarbUseWOTBAlways.Checked+=bBarbUseWOTBAlwaysChecked;
									cbbBarbUseWOTBAlways.Unchecked+=bBarbUseWOTBAlwaysChecked;
									spWrathOptions.Children.Add(cbbBarbUseWOTBAlways);
									LBClass.Items.Add(spWrathOptions);



									CheckBox cbbFuryDumpWrath=new CheckBox
									{
										 Content="Fury Dump during Wrath",
										 Width=300,
										 Height=30,
										 IsChecked=(Bot.SettingsFunky.Class.bFuryDumpWrath)
									};
									cbbFuryDumpWrath.Checked+=bFuryDumpWrathChecked;
									cbbFuryDumpWrath.Unchecked+=bFuryDumpWrathChecked;
									LBClass.Items.Add(cbbFuryDumpWrath);

									CheckBox cbbFuryDumpAlways=new CheckBox
									{
										 Content="Fury Dump Always",
										 Width=300,
										 Height=30,
										 IsChecked=(Bot.SettingsFunky.Class.bFuryDumpAlways)
									};
									cbbFuryDumpAlways.Checked+=bFuryDumpAlwaysChecked;
									cbbFuryDumpAlways.Unchecked+=bFuryDumpAlwaysChecked;
									LBClass.Items.Add(cbbFuryDumpAlways);

									break;
							 case Zeta.Internals.Actors.ActorClass.DemonHunter:
									LBClass.Items.Add("Reuse Vault Delay");
									Slider iDHVaultMovementDelayslider=new Slider
									{
										 Width=200,
										 Maximum=4000,
										 Minimum=400,
										 TickFrequency=5,
										 LargeChange=5,
										 SmallChange=1,
										 Value=Bot.SettingsFunky.Class.iDHVaultMovementDelay,
										 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
									};
									iDHVaultMovementDelayslider.ValueChanged+=iDHVaultMovementDelaySliderChanged;
									TBiDHVaultMovementDelay=new TextBox
									{
										 Text=Bot.SettingsFunky.Class.iDHVaultMovementDelay.ToString(),
										 IsReadOnly=true,
									};
									StackPanel DhVaultPanel=new StackPanel
									{
										 Width=600,
										 Height=30,
										 Orientation=Orientation.Horizontal,
									};
									DhVaultPanel.Children.Add(iDHVaultMovementDelayslider);
									DhVaultPanel.Children.Add(TBiDHVaultMovementDelay);
									LBClass.Items.Add(DhVaultPanel);

									break;
							 case Zeta.Internals.Actors.ActorClass.Monk:
									CheckBox cbbMonkSpamMantra=new CheckBox
													{
														 Content="Spam Mantra Ability",
														 Width=300,
														 Height=30,
														 IsChecked=(Bot.SettingsFunky.Class.bMonkSpamMantra)
													};
									cbbMonkSpamMantra.Checked+=bMonkSpamMantraChecked;
									cbbMonkSpamMantra.Unchecked+=bMonkSpamMantraChecked;
									LBClass.Items.Add(cbbMonkSpamMantra);

									CheckBox cbbMonkInnaSet=new CheckBox
									{
										 Content="Full Inna Set Bonus",
										 Width=300,
										 Height=30,
										 IsChecked=(Bot.SettingsFunky.Class.bMonkInnaSet)
									};
									cbbMonkInnaSet.Checked+=bMonkInnaSetChecked;
									cbbMonkInnaSet.Unchecked+=bMonkInnaSetChecked;
									LBClass.Items.Add(cbbMonkInnaSet);

									break;
							 case Zeta.Internals.Actors.ActorClass.WitchDoctor:
							 case Zeta.Internals.Actors.ActorClass.Wizard:
									//CheckBox cbbEnableCriticalMass = new CheckBox
									//{
									//    Content = "Critical Mass",
									//    Width = 300,
									//    Height = 30,
									//    IsChecked = (Bot.SettingsFunky.Class.bEnableCriticalMass)
									//};
									//cbbEnableCriticalMass.Checked += bEnableCriticalMassChecked;
									//cbbEnableCriticalMass.Unchecked += bEnableCriticalMassChecked;
									//LBClass.Items.Add(cbbEnableCriticalMass);

									if (Bot.ActorClass==Zeta.Internals.Actors.ActorClass.Wizard)
									{
										 CheckBox cbbWaitForArchon=new CheckBox
										 {
												Content="Wait for Archon",
												Width=300,
												Height=30,
												IsChecked=(Bot.SettingsFunky.Class.bWaitForArchon)
										 };
										 cbbWaitForArchon.Checked+=bWaitForArchonChecked;
										 cbbWaitForArchon.Unchecked+=bWaitForArchonChecked;
										 LBClass.Items.Add(cbbWaitForArchon);

										 CheckBox cbbKiteOnlyArchon=new CheckBox
										 {
												Content="Do NOT Kite During Archon",
												Width=300,
												Height=30,
												IsChecked=(Bot.SettingsFunky.Class.bKiteOnlyArchon)
										 };
										 cbbKiteOnlyArchon.Checked+=bKiteOnlyArchonChecked;
										 cbbKiteOnlyArchon.Unchecked+=bKiteOnlyArchonChecked;
										 LBClass.Items.Add(cbbKiteOnlyArchon);

										 CheckBox cbbCancelArchonRebuff=new CheckBox
										 {
												Content="Cancel Archon for Rebuff",
												Height=30,
												IsChecked=(Bot.SettingsFunky.Class.bCancelArchonRebuff),
										 };
										 cbbCancelArchonRebuff.Checked+=bCancelArchonRebuffChecked;
										 cbbCancelArchonRebuff.Unchecked+=bCancelArchonRebuffChecked;
										 LBClass.Items.Add(cbbCancelArchonRebuff);

										 CheckBox cbbTeleportFleeWhenLowHP=new CheckBox
										 {
												Content="Teleport: Flee When Low HP",
												Height=30,
												IsChecked=(Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP),
										 };
										 cbbTeleportFleeWhenLowHP.Checked+=bTeleportFleeWhenLowHPChecked;
										 cbbTeleportFleeWhenLowHP.Unchecked+=bTeleportFleeWhenLowHPChecked;
										 LBClass.Items.Add(cbbTeleportFleeWhenLowHP);

										 CheckBox cbbTeleportIntoGrouping=new CheckBox
										 {
												Content="Teleport: Into Monster Groups",
												Height=30,
												IsChecked=(Bot.SettingsFunky.Class.bTeleportIntoGrouping),
										 };
										 cbbTeleportIntoGrouping.Checked+=bTeleportIntoGroupingChecked;
										 cbbTeleportIntoGrouping.Unchecked+=bTeleportIntoGroupingChecked;
										 LBClass.Items.Add(cbbTeleportIntoGrouping);
										 //

									}

									break;
						}
						if (Bot.ActorClass==Zeta.Internals.Actors.ActorClass.DemonHunter||Bot.ActorClass==Zeta.Internals.Actors.ActorClass.WitchDoctor||Bot.ActorClass==Zeta.Internals.Actors.ActorClass.Wizard)
						{

							 #region GoblinMinimumRange
							 LBClass.Items.Add("Treasure Goblin Minimum Range");
							 Slider sliderGoblinMinRange=new Slider
							 {
									Width=200,
									Maximum=75,
									Minimum=0,
									TickFrequency=5,
									LargeChange=5,
									SmallChange=1,
									Value=Bot.SettingsFunky.Class.GoblinMinimumRange,
									HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 };
							 sliderGoblinMinRange.ValueChanged+=TreasureGoblinMinimumRangeSliderChanged;
							 TBGoblinMinRange=new TextBox
							 {
									Text=Bot.SettingsFunky.Class.GoblinMinimumRange.ToString(),
									IsReadOnly=true,
							 };
							 StackPanel GoblinMinRangeStackPanel=new StackPanel
							 {
									Width=600,
									Height=30,
									Orientation=Orientation.Horizontal,
							 };
							 GoblinMinRangeStackPanel.Children.Add(sliderGoblinMinRange);
							 GoblinMinRangeStackPanel.Children.Add(TBGoblinMinRange);
							 LBClass.Items.Add(GoblinMinRangeStackPanel);
							 #endregion


							 CheckBox cbMissleDampeningCloseRange=new CheckBox
							 {
									Content="Close Range on Missile Dampening Monsters",
									IsChecked=Bot.SettingsFunky.MissleDampeningEnforceCloseRange,
							 };
							 cbMissleDampeningCloseRange.Checked+=MissileDampeningChecked;
							 cbMissleDampeningCloseRange.Unchecked+=MissileDampeningChecked;
							 LBClass.Items.Add(cbMissleDampeningCloseRange);

						}
						ClassTabItem.Content=LBClass;
						#endregion


						CombatSettingsTabItem.Content=CombatTabControl;
						#endregion


						#region Targeting
						TabItem TargetTabItem=new TabItem();
						TargetTabItem.Header="Targeting";
						tabControl1.Items.Add(TargetTabItem);

						TabControl tcTargeting=new TabControl
						{
							 Height=600,
							 Width=600,
							 Focusable=false,
						};

						#region Targeting_General
						TabItem TargetingMiscTabItem=new TabItem();

						TargetingMiscTabItem.Header="General";
						tcTargeting.Items.Add(TargetingMiscTabItem);
						ListBox Target_General_ContentListBox=new ListBox
						{
							 Focusable=false,
						};

						StackPanel Targeting_General_Options_Stackpanel=new StackPanel
						{
							 Orientation=Orientation.Vertical,
							 Focusable=false,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
							 Background=System.Windows.Media.Brushes.DimGray,
						};
						TextBlock Target_General_Text=new TextBlock
						{
							 Text="General Targeting Options",
							 FontSize=13,
							 Background=System.Windows.Media.Brushes.OrangeRed,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Center,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						};



						#region IgnoreElites
						CheckBox cbIgnoreElites=new CheckBox
						{
							 Content="Ignore Rare/Elite/Unique Monsters",
							 Width=300,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.IgnoreAboveAverageMobs),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						cbIgnoreElites.Checked+=IgnoreEliteMonstersChecked;
						cbIgnoreElites.Unchecked+=IgnoreEliteMonstersChecked;
						#endregion

						#region IgnoreCorpses
						CheckBox cbIgnoreCorpses=new CheckBox
						{
							 Content="Ignore Looting Corpses",
							 Width=300,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.IgnoreCorpses),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						cbIgnoreCorpses.Checked+=IgnoreCorpsesChecked;
						cbIgnoreCorpses.Unchecked+=IgnoreCorpsesChecked;
						#endregion

						#region ExtendedRepChestRange
						UseExtendedRangeRepChestCB=new CheckBox
						{
							 Content="Allow high range on rare chests",
							 Width=300,
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.UseExtendedRangeRepChest),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,

						};
						UseExtendedRangeRepChestCB.Checked+=ExtendRangeRepChestChecked;
						UseExtendedRangeRepChestCB.Unchecked+=ExtendRangeRepChestChecked;
						#endregion

						#region GoblinPriority
						StackPanel GoblinPriority_StackPanel=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						TextBlock Target_GoblinPriority_Text=new TextBlock
						{
							 Text="Goblin Priority",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(4),
						};
						GoblinPriority_StackPanel.Children.Add(Target_GoblinPriority_Text);
						ComboBox CBGoblinPriority=new ComboBox
						{
							 Height=25,
							 Width=300,
							 ItemsSource=new GoblinPriority(),
							 SelectedIndex=Bot.SettingsFunky.GoblinPriority,
							 Margin=new Thickness(4),
						};
						CBGoblinPriority.SelectionChanged+=GoblinPriorityChanged;
						GoblinPriority_StackPanel.Children.Add(CBGoblinPriority);
						#endregion

						Targeting_General_Options_Stackpanel.Children.Add(Target_General_Text);
						Targeting_General_Options_Stackpanel.Children.Add(cbIgnoreElites);
						Targeting_General_Options_Stackpanel.Children.Add(cbIgnoreCorpses);
						Targeting_General_Options_Stackpanel.Children.Add(UseExtendedRangeRepChestCB);
						Targeting_General_Options_Stackpanel.Children.Add(GoblinPriority_StackPanel);
						Target_General_ContentListBox.Items.Add(Targeting_General_Options_Stackpanel);





						TargetingMiscTabItem.Content=Target_General_ContentListBox;
						#endregion

						#region Targeting_Ranges
						TabItem RangeTabItem=new TabItem();
						RangeTabItem.Header="Range";
						tcTargeting.Items.Add(RangeTabItem);
						ListBox lbTargetRange=new ListBox();

						StackPanel spIgnoreProfileValues=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						};

						CheckBox cbIgnoreCombatRange=new CheckBox
						{
							 Content="Ignore Combat Range (Set by Profile)",
							 // Width = 300,
							 HorizontalContentAlignment=System.Windows.HorizontalAlignment.Left,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.IgnoreCombatRange)
						};
						cbIgnoreCombatRange.Checked+=IgnoreCombatRangeChecked;
						cbIgnoreCombatRange.Unchecked+=IgnoreCombatRangeChecked;
						spIgnoreProfileValues.Children.Add(cbIgnoreCombatRange);

						CheckBox cbIgnoreLootRange=new CheckBox
						{
							 Content="Ignore Loot Range (Set by Profile)",
							 // Width = 300,
							 Height=30,
							 HorizontalContentAlignment=System.Windows.HorizontalAlignment.Right,
							 IsChecked=(Bot.SettingsFunky.IgnoreLootRange)
						};
						cbIgnoreLootRange.Checked+=IgnoreLootRangeChecked;
						cbIgnoreLootRange.Unchecked+=IgnoreLootRangeChecked;
						spIgnoreProfileValues.Children.Add(cbIgnoreLootRange);

						lbTargetRange.Items.Add(spIgnoreProfileValues);


						TextBlock Target_Range_Text=new TextBlock
						{
							 Text="Targeting Extended Range Values",
							 FontSize=13,
							 Background=System.Windows.Media.Brushes.DarkSeaGreen,
							 TextAlignment=TextAlignment.Center,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						};
						lbTargetRange.Items.Add(Target_Range_Text);

						#region EliteRange
						lbTargetRange.Items.Add("Elite Combat Range");
						Slider sliderEliteRange=new Slider
						{
							 Width=100,
							 Maximum=150,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.EliteCombatRange,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderEliteRange.ValueChanged+=EliteRangeSliderChanged;
						TBEliteRange=new TextBox
						{
							 Text=Bot.SettingsFunky.EliteCombatRange.ToString(),
							 IsReadOnly=true,
						};
						StackPanel EliteStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						EliteStackPanel.Children.Add(sliderEliteRange);
						EliteStackPanel.Children.Add(TBEliteRange);
						lbTargetRange.Items.Add(EliteStackPanel);
						#endregion

						#region NonEliteRange
						lbTargetRange.Items.Add("Non-Elite Combat Range");
						Slider sliderNonEliteRange=new Slider
						{
							 Width=100,
							 Maximum=150,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.NonEliteCombatRange,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderNonEliteRange.ValueChanged+=NonEliteRangeSliderChanged;
						TBNonEliteRange=new TextBox
						{
							 Text=Bot.SettingsFunky.NonEliteCombatRange.ToString(),
							 IsReadOnly=true,
						};
						StackPanel NonEliteStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						NonEliteStackPanel.Children.Add(sliderNonEliteRange);
						NonEliteStackPanel.Children.Add(TBNonEliteRange);
						lbTargetRange.Items.Add(NonEliteStackPanel);
						#endregion

						#region ExtendedCombatRange
						//lbTargetRange.Items.Add("Extended Combat Range");
						//Slider sliderExtendedCombatRange = new Slider
						//{
						//	 Width = 100,
						//	 Maximum = 50,
						//	 Minimum = 0,
						//	 TickFrequency = 5,
						//	 LargeChange = 5,
						//	 SmallChange = 1,
						//	 Value = Bot.SettingsFunky.ExtendedCombatRange,
						//	 HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
						//};
						//sliderExtendedCombatRange.ValueChanged += ExtendCombatRangeSliderChanged;
						//TBExtendedCombatRange = new TextBox
						//{
						//	 Text = Bot.SettingsFunky.ExtendedCombatRange.ToString(),
						//	 IsReadOnly = true,
						//};
						//StackPanel ExtendedRangeStackPanel = new StackPanel
						//{
						//	 Width = 600,
						//	 Height = 20,
						//	 Orientation = Orientation.Horizontal,
						//};
						//ExtendedRangeStackPanel.Children.Add(sliderExtendedCombatRange);
						//ExtendedRangeStackPanel.Children.Add(TBExtendedCombatRange);
						//lbTargetRange.Items.Add(ExtendedRangeStackPanel);
						#endregion

						#region ShrineRange
						lbTargetRange.Items.Add("Shrine Range");
						Slider sliderShrineRange=new Slider
						{
							 Width=100,
							 Maximum=75,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.ShrineRange,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderShrineRange.ValueChanged+=ShrineRangeSliderChanged;
						TBShrineRange=new TextBox
						{
							 Text=Bot.SettingsFunky.ShrineRange.ToString(),
							 IsReadOnly=true,
						};
						StackPanel ShrineStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						ShrineStackPanel.Children.Add(sliderShrineRange);
						ShrineStackPanel.Children.Add(TBShrineRange);
						lbTargetRange.Items.Add(ShrineStackPanel);
						#endregion

						#region ContainerRange
						lbTargetRange.Items.Add("Container Range");
						Slider sliderContainerRange=new Slider
						{
							 Width=100,
							 Maximum=75,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.ContainerOpenRange,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderContainerRange.ValueChanged+=ContainerRangeSliderChanged;
						TBContainerRange=new TextBox
						{
							 Text=Bot.SettingsFunky.ContainerOpenRange.ToString(),
							 IsReadOnly=true,
						};
						StackPanel ContainerStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						ContainerStackPanel.Children.Add(sliderContainerRange);
						ContainerStackPanel.Children.Add(TBContainerRange);
						lbTargetRange.Items.Add(ContainerStackPanel);
						#endregion

						#region DestructibleRange
						lbTargetRange.Items.Add("Destuctible Range");
						Slider sliderDestructibleRange=new Slider
						{
							 Width=100,
							 Maximum=75,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.DestructibleRange,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderDestructibleRange.ValueChanged+=DestructibleSliderChanged;
						TBDestructibleRange=new TextBox
						{
							 Text=Bot.SettingsFunky.DestructibleRange.ToString(),
							 IsReadOnly=true,
						};
						StackPanel DestructibleStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						DestructibleStackPanel.Children.Add(sliderDestructibleRange);
						DestructibleStackPanel.Children.Add(TBDestructibleRange);
						lbTargetRange.Items.Add(DestructibleStackPanel);
						#endregion

						#region GoldRange
						lbTargetRange.Items.Add("Gold Range");
						Slider sliderGoldRange=new Slider
						{
							 Width=100,
							 Maximum=150,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.GoldRange,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderGoldRange.ValueChanged+=GoldRangeSliderChanged;
						TBGoldRange=new TextBox
						{
							 Text=Bot.SettingsFunky.GoldRange.ToString(),
							 IsReadOnly=true,
						};
						StackPanel GoldRangeStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						GoldRangeStackPanel.Children.Add(sliderGoldRange);
						GoldRangeStackPanel.Children.Add(TBGoldRange);
						lbTargetRange.Items.Add(GoldRangeStackPanel);
						#endregion

						#region GlobeRange
						lbTargetRange.Items.Add("Globe Range");
						Slider sliderGlobeRange=new Slider
						{
							 Width=100,
							 Maximum=75,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.GlobeRange,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderGlobeRange.ValueChanged+=GlobeRangeSliderChanged;
						TBGlobeRange=new TextBox
						{
							 Text=Bot.SettingsFunky.GlobeRange.ToString(),
							 IsReadOnly=true,
						};
						StackPanel GlobeRangeStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						GlobeRangeStackPanel.Children.Add(sliderGlobeRange);
						GlobeRangeStackPanel.Children.Add(TBGlobeRange);
						lbTargetRange.Items.Add(GlobeRangeStackPanel);
						#endregion

						#region ItemRange
						lbTargetRange.Items.Add("Item Loot Range");
						Slider sliderItemRange=new Slider
						{
							 Width=100,
							 Maximum=150,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.ItemRange,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderItemRange.ValueChanged+=ItemRangeSliderChanged;
						TBItemRange=new TextBox
						{
							 Text=Bot.SettingsFunky.ItemRange.ToString(),
							 IsReadOnly=true,
						};
						StackPanel ItemRangeStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						ItemRangeStackPanel.Children.Add(sliderItemRange);
						ItemRangeStackPanel.Children.Add(TBItemRange);
						lbTargetRange.Items.Add(ItemRangeStackPanel);
						#endregion

						#region GoblinRange
						lbTargetRange.Items.Add("Treasure Goblin Range");
						Slider sliderGoblinRange=new Slider
						{
							 Width=100,
							 Maximum=150,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.TreasureGoblinRange,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderGoblinRange.ValueChanged+=TreasureGoblinRangeSliderChanged;
						TBGoblinRange=new TextBox
						{
							 Text=Bot.SettingsFunky.TreasureGoblinRange.ToString(),
							 IsReadOnly=true,
						};
						StackPanel GoblinRangeStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						GoblinRangeStackPanel.Children.Add(sliderGoblinRange);
						GoblinRangeStackPanel.Children.Add(TBGoblinRange);
						lbTargetRange.Items.Add(GoblinRangeStackPanel);
						#endregion

						RangeTabItem.Content=lbTargetRange;
						#endregion

						TargetTabItem.Content=tcTargeting;

						#endregion


						#region General
						TabControl tcGeneral=new TabControl
						{
							 Width=600,
							 Height=600,
						};

						TabItem GeneralSettingsTabItem=new TabItem();
						GeneralSettingsTabItem.Header="General";
						tabControl1.Items.Add(GeneralSettingsTabItem);

						TabItem GeneralTab=new TabItem();
						GeneralTab.Header="General";
						tcGeneral.Items.Add(GeneralTab);
						lbGeneralContent=new ListBox();

						#region OOCItemBehavior
						StackPanel OOCItemBehaviorStackPanel=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 Background=System.Windows.Media.Brushes.DimGray,
						};
						TextBlock OOCItemBehavior_Header_Text=new TextBlock
						{
							 Text="Out-Of-Combat Item Idenification",
							 FontSize=13,
							 Background=System.Windows.Media.Brushes.LightSeaGreen,
							 TextAlignment=TextAlignment.Left,
						};
						TextBlock OOCItemBehavior_Header_Info=new TextBlock
						{
							 Text="Behavior Preforms idenfication of items (Individual IDing) when unid count is surpassed",
							 FontSize=11,
							 FontStyle=FontStyles.Italic,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Left,
						};

						#region OOC_ID_Items
						OOCIdentifyItems=new CheckBox
						{
							 Content="Enable Out Of Combat Idenification Behavior",
							 IsChecked=(Bot.SettingsFunky.OOCIdentifyItems),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,

						};
						OOCIdentifyItems.Checked+=OOCIDChecked;
						OOCIdentifyItems.Unchecked+=OOCIDChecked;
						#endregion
						TextBlock OOCItemBehavior_MinItem_Text=new TextBlock
						{
							 Text="Minimum Unid Items",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Left,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};

						#region OOC_Min_Item_Count

						OOCIdentfyItemsMinCount=new TextBox
						{
							 Text=Bot.SettingsFunky.OOCIdentifyItemsMinimumRequired.ToString(),
							 Width=100,
							 Height=25
						};
						OOCIdentfyItemsMinCount.KeyUp+=OOCMinimumItems_KeyUp;
						OOCIdentfyItemsMinCount.TextChanged+=OOCIdentifyItemsMinValueChanged;

						#endregion


						OOCItemBehaviorStackPanel.Children.Add(OOCItemBehavior_Header_Text);
						OOCItemBehaviorStackPanel.Children.Add(OOCIdentifyItems);
						OOCItemBehaviorStackPanel.Children.Add(OOCItemBehavior_Header_Info);
						OOCItemBehaviorStackPanel.Children.Add(OOCItemBehavior_MinItem_Text);
						OOCItemBehaviorStackPanel.Children.Add(OOCIdentfyItemsMinCount);
						lbGeneralContent.Items.Add(OOCItemBehaviorStackPanel);
						#endregion

						#region Bot Stop Feature
						StackPanel spBotStopLowHP=new StackPanel
						{
							 Orientation=Orientation.Vertical,
							 Background=System.Windows.Media.Brushes.DimGray,
						};
						TextBlock BotStop_Text_Header=new TextBlock
						{
							 Text="Emergency Bot Health Stopping",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Background=System.Windows.Media.Brushes.IndianRed,
						};
						spBotStopLowHP.Children.Add(BotStop_Text_Header);

						#region StopGameOnBotLowHealth
						CheckBox CBStopGameOnBotLowHealth=new CheckBox
						{
							 Content="Enable Bot Stop Behavior",
							 Width=300,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.StopGameOnBotLowHealth),
						};
						CBStopGameOnBotLowHealth.Checked+=StopGameOnBotLowHealthChecked;
						CBStopGameOnBotLowHealth.Unchecked+=StopGameOnBotLowHealthChecked;
						spBotStopLowHP.Children.Add(CBStopGameOnBotLowHealth);
						#endregion

						#region StopBotOnLowHealth--Slider
						spBotStop=new StackPanel
						{
							 IsEnabled=Bot.SettingsFunky.StopGameOnBotLowHealth,
						};
						TextBlock BotStopLowHP_Text_Header=new TextBlock
						{
							 Text="Bot Stop Health Percent",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 //Background=System.Windows.Media.Brushes.MediumSeaGreen,
						};
						spBotStop.Children.Add(BotStopLowHP_Text_Header);

						Slider sliderBotStopLowHPValue=new Slider
						{
							 Width=100,
							 Maximum=1,
							 Minimum=0,
							 TickFrequency=0.25,
							 LargeChange=0.25,
							 SmallChange=0.10,
							 Value=Bot.SettingsFunky.StopGameOnBotHealthPercent,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderBotStopLowHPValue.ValueChanged+=BotStopHPValueSliderChanged;
						TBBotStopHealthPercent=new TextBox
						{
							 Text=Bot.SettingsFunky.StopGameOnBotHealthPercent.ToString("F2", CultureInfo.InvariantCulture),
							 IsReadOnly=true,
						};
						StackPanel BotStopHPValueStackPanel=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						BotStopHPValueStackPanel.Children.Add(sliderBotStopLowHPValue);
						BotStopHPValueStackPanel.Children.Add(TBBotStopHealthPercent);
						spBotStop.Children.Add(BotStopHPValueStackPanel);
						#endregion

						#region StopGameOnBotLowHealth-ScreenShot
						CheckBox CBStopGameOnBotEnableScreenShot=new CheckBox
						{
							 Content="Take A Screenshot before stopping",
							 Width=300,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.StopGameOnBotEnableScreenShot),
						};
						CBStopGameOnBotEnableScreenShot.Checked+=StopGameOnBotEnableScreenShotChecked;
						CBStopGameOnBotEnableScreenShot.Unchecked+=StopGameOnBotEnableScreenShotChecked;
						spBotStop.Children.Add(CBStopGameOnBotEnableScreenShot);
						#endregion

						spBotStopLowHP.Children.Add(spBotStop);
						lbGeneralContent.Items.Add(spBotStopLowHP);
						#endregion

						#region LevelingLogic
						ToolTip TTLevelingLogic=new System.Windows.Controls.ToolTip
						{
							 Content="Enables auto-equipping of items, abilities. This overrides default item loot settings.",
						};
						CheckBox LevelingLogic=new CheckBox
						{
							 Content="Leveling Item Logic",
							 Width=300,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.UseLevelingLogic),
							 ToolTip=TTLevelingLogic,
						};
						LevelingLogic.Checked+=ItemLevelingLogicChecked;
						LevelingLogic.Unchecked+=ItemLevelingLogicChecked;
						lbGeneralContent.Items.Add(LevelingLogic);
						#endregion

						#region PotionsDuringTownRun
						BuyPotionsDuringTownRunCB=new CheckBox
						{
							 Content="Buy Potions During Town Run (Uses Maximum Potion Count Setting)",
							 Width=500,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.BuyPotionsDuringTownRun)
						};
						BuyPotionsDuringTownRunCB.Checked+=BuyPotionsDuringTownRunChecked;
						BuyPotionsDuringTownRunCB.Unchecked+=BuyPotionsDuringTownRunChecked;
						lbGeneralContent.Items.Add(BuyPotionsDuringTownRunCB);
						#endregion

						#region OutOfCombatMovement
						CheckBox cbOutOfCombatMovement=new CheckBox
						{
							 Content="Use Out Of Combat Ability Movements",
							 Width=300,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.OutOfCombatMovement)
						};
						cbOutOfCombatMovement.Checked+=OutOfCombatMovementChecked;
						cbOutOfCombatMovement.Unchecked+=OutOfCombatMovementChecked;
						lbGeneralContent.Items.Add(cbOutOfCombatMovement);
						#endregion

						#region AllowBuffingInTown
						CheckBox cbAllowBuffingInTown=new CheckBox
						{
							 Content="Allow Buffing In Town",
							 Width=300,
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.AllowBuffingInTown)
						};
						cbAllowBuffingInTown.Checked+=AllowBuffingInTownChecked;
						cbAllowBuffingInTown.Unchecked+=AllowBuffingInTownChecked;
						lbGeneralContent.Items.Add(cbAllowBuffingInTown);
						#endregion

						#region AfterCombatDelayOptions
						StackPanel AfterCombatDelayStackPanel=new StackPanel();
						#region AfterCombatDelay

						Slider sliderAfterCombatDelay=new Slider
						{
							 Width=100,
							 Maximum=2000,
							 Minimum=0,
							 TickFrequency=200,
							 LargeChange=100,
							 SmallChange=50,
							 Value=Bot.SettingsFunky.AfterCombatDelay,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						sliderAfterCombatDelay.ValueChanged+=AfterCombatDelaySliderChanged;
						TBAfterCombatDelay=new TextBox
						{
							 Margin=new Thickness(Margin.Left+5, Margin.Top, Margin.Right, Margin.Bottom),
							 Text=Bot.SettingsFunky.AfterCombatDelay.ToString(),
							 IsReadOnly=true,
						};
						StackPanel AfterCombatStackPanel=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 Orientation=Orientation.Horizontal,
						};
						AfterCombatStackPanel.Children.Add(sliderAfterCombatDelay);
						AfterCombatStackPanel.Children.Add(TBAfterCombatDelay);

						#endregion
						#region WaitTimerAfterContainers
						EnableWaitAfterContainersCB=new CheckBox
						{
							 Content="Apply Delay After Opening Containers",
							 Width=300,
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.EnableWaitAfterContainers)
						};
						EnableWaitAfterContainersCB.Checked+=EnableWaitAfterContainersChecked;
						EnableWaitAfterContainersCB.Unchecked+=EnableWaitAfterContainersChecked;

						#endregion

						TextBlock CombatLootDelay_Text_Info=new TextBlock
						{
							 Text="End of Combat Delay Timer",
							 FontSize=11,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Left,
						};
						AfterCombatDelayStackPanel.Children.Add(CombatLootDelay_Text_Info);
						AfterCombatDelayStackPanel.Children.Add(AfterCombatStackPanel);
						AfterCombatDelayStackPanel.Children.Add(EnableWaitAfterContainersCB);
						lbGeneralContent.Items.Add(AfterCombatDelayStackPanel);

						#endregion


						StackPanel spShrinePanel=new StackPanel();
						TextBlock Shrines_Header_Text=new TextBlock
						{
							 Text="Shrines",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 TextAlignment=TextAlignment.Left,
						};
						spShrinePanel.Children.Add(Shrines_Header_Text);
						StackPanel spShrineUseOptions=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						CheckBox[] cbUseShrine=new CheckBox[6];
						string[] ShrineNames=Enum.GetNames(typeof(ShrineTypes));
						for (int i=0; i<6; i++)
						{
							 cbUseShrine[i]=new CheckBox
							 {
									Content=ShrineNames[i],
									Name=ShrineNames[i],
									IsChecked=Bot.SettingsFunky.UseShrineTypes[i],
									Margin=new Thickness(Margin.Left+3, Margin.Top, Margin.Right, Margin.Bottom+5),
							 };
							 cbUseShrine[i].Checked+=UseShrineChecked;
							 cbUseShrine[i].Unchecked+=UseShrineChecked;
							 spShrineUseOptions.Children.Add(cbUseShrine[i]);
						}
						spShrinePanel.Children.Add(spShrineUseOptions);

						lbGeneralContent.Items.Add(spShrinePanel);

						GeneralTab.Content=lbGeneralContent;

						#region CoffeeBreaks
						TabItem CoffeeBreakTab=new TabItem();
						CoffeeBreakTab.Header="Coffee Breaks";
						tcGeneral.Items.Add(CoffeeBreakTab);
						ListBox LBCoffeebreak=new ListBox();

						StackPanel CoffeeBreaksStackPanel=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 Orientation=Orientation.Vertical,

						};

						TextBlock CoffeeBreaks_Header_Text=new TextBlock
						{
							 Text="Coffee Breaks",
							 FontSize=13,
							 Background=System.Windows.Media.Brushes.LightSeaGreen,
							 TextAlignment=TextAlignment.Center,
						};

						#region CoffeeBreakCheckBox
						CoffeeBreaks=new CheckBox
						{
							 Content="Enable Coffee Breaks",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.EnableCoffeeBreaks)

						};
						CoffeeBreaks.Checked+=EnableCoffeeBreaksChecked;
						CoffeeBreaks.Unchecked+=EnableCoffeeBreaksChecked;
						#endregion

						TextBlock CoffeeBreak_Minutes_Text=new TextBlock
						{
							 Text="Break time range (Minutues)",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
						};

						#region BreakTimeMinMinutes
						TextBlock CoffeeBreaks_Min_Text=new TextBlock
						{
							 Text="Minimum",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Center,
						};
						Slider sliderBreakMinMinutes=new Slider
						{
							 Width=100,
							 Maximum=20,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=2,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.MinBreakTime,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderBreakMinMinutes.ValueChanged+=BreakMinMinutesSliderChange;
						tbMinBreakTime=new TextBox
						{
							 Text=sliderBreakMinMinutes.Value.ToString(),
							 IsReadOnly=true,
						};
						StackPanel BreakTimeMinMinutestackPanel=new StackPanel
						{
							 Height=30,
							 Orientation=Orientation.Horizontal,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						BreakTimeMinMinutestackPanel.Children.Add(CoffeeBreaks_Min_Text);
						BreakTimeMinMinutestackPanel.Children.Add(sliderBreakMinMinutes);
						BreakTimeMinMinutestackPanel.Children.Add(tbMinBreakTime);

						#endregion

						#region BreakTimeMaxMinutes
						TextBlock CoffeeBreaks_Max_Text=new TextBlock
						{
							 Text="Maximum",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Center,
						};
						Slider sliderBreakMaxMinutes=new Slider
						{
							 Width=100,
							 Maximum=20,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=2,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.MaxBreakTime,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderBreakMaxMinutes.ValueChanged+=BreakMaxMinutesSliderChange;
						tbMaxBreakTime=new TextBox
						{
							 Text=sliderBreakMaxMinutes.Value.ToString(),
							 IsReadOnly=true,
						};
						StackPanel BreakTimeMaxMinutestackPanel=new StackPanel
						{
							 Height=20,
							 Orientation=Orientation.Horizontal,
							 Margin=new Thickness(Margin.Left+5, Margin.Top, Margin.Right, Margin.Bottom),
						};
						BreakTimeMaxMinutestackPanel.Children.Add(CoffeeBreaks_Max_Text);
						BreakTimeMaxMinutestackPanel.Children.Add(sliderBreakMaxMinutes);
						BreakTimeMaxMinutestackPanel.Children.Add(tbMaxBreakTime);
						#endregion

						StackPanel CoffeeBreakTimeRangeStackPanel=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 Orientation=Orientation.Horizontal,
						};

						CoffeeBreakTimeRangeStackPanel.Children.Add(BreakTimeMinMinutestackPanel);
						CoffeeBreakTimeRangeStackPanel.Children.Add(BreakTimeMaxMinutestackPanel);

						#region BreakTimeIntervalHour


						Slider sliderBreakTimeHour=new Slider
						{
							 Width=200,
							 Maximum=10,
							 Minimum=0,
							 TickFrequency=1,
							 LargeChange=0.50,
							 SmallChange=0.05,
							 Value=Bot.SettingsFunky.breakTimeHour,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderBreakTimeHour.ValueChanged+=BreakTimeHourSliderChanged;
						TBBreakTimeHour=new TextBox
						{
							 Text=sliderBreakTimeHour.Value.ToString("F2", CultureInfo.InvariantCulture),
							 IsReadOnly=true,
						};
						StackPanel BreakTimeHourStackPanel=new StackPanel
						{
							 Width=600,
							 Height=30,
							 Orientation=Orientation.Horizontal,
						};
						BreakTimeHourStackPanel.Children.Add(sliderBreakTimeHour);
						BreakTimeHourStackPanel.Children.Add(TBBreakTimeHour);

						#endregion
						TextBlock CoffeeBreakInterval_Text=new TextBlock
						{
							 Text="Break Hour Interval (1 Equals One Hour)",
							 FontSize=13,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Left,
						};

						CoffeeBreaksStackPanel.Children.Add(CoffeeBreaks_Header_Text);
						CoffeeBreaksStackPanel.Children.Add(CoffeeBreaks);
						CoffeeBreaksStackPanel.Children.Add(CoffeeBreak_Minutes_Text);
						CoffeeBreaksStackPanel.Children.Add(CoffeeBreakTimeRangeStackPanel);
						CoffeeBreaksStackPanel.Children.Add(CoffeeBreakInterval_Text);
						CoffeeBreaksStackPanel.Children.Add(BreakTimeHourStackPanel);
						LBCoffeebreak.Items.Add(CoffeeBreaksStackPanel);
						CoffeeBreakTab.Content=LBCoffeebreak;
						#endregion



						GeneralSettingsTabItem.Content=tcGeneral;
						#endregion

						#region Items


						TabItem CustomSettingsTabItem=new TabItem();
						CustomSettingsTabItem.Header="Items";
						tabControl1.Items.Add(CustomSettingsTabItem);

						TabControl tcItems=new TabControl
						{
							 Width=600,
							 Height=600
						};

						#region ItemRules
						TabItem ItemRulesTabItem=new TabItem();
						ItemRulesTabItem.Header="Item Rules";
						tcItems.Items.Add(ItemRulesTabItem);
						ListBox lbItemRulesContent=new ListBox();

						StackPanel spItemRules=new StackPanel
						{
							 Background=System.Windows.Media.Brushes.DimGray,
						};
						#region ItemRules Checkbox
						ItemRules=new CheckBox
						{
							 Content="Enable Item Rules",
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.UseItemRules),
							 FontSize=14,
							 FontStyle=FontStyles.Oblique,

						};
						ItemRules.Checked+=ItemRulesChecked;
						ItemRules.Unchecked+=ItemRulesChecked;
						spItemRules.Children.Add(ItemRules);
						#endregion
						TextBlock txt_ItemRulesOptions=new TextBlock
						{
							 Text="Additional Rules",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.DarkSlateGray,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spItemRules.Children.Add(txt_ItemRulesOptions);

						StackPanel spItemRulesOptions=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						#region ItemRules Pickup Checkbox
						ItemRulesPickup=new CheckBox
						{
							 Content="ItemRules Pickup",
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.UseItemRulesPickup),
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+10, Margin.Bottom),

						};
						ItemRulesPickup.Checked+=ItemRulesPickupChecked;
						ItemRulesPickup.Unchecked+=ItemRulesPickupChecked;
						spItemRulesOptions.Children.Add(ItemRulesPickup);
						#endregion
						#region ItemRules Salvage Checkbox
						CheckBox CBItemRulesSalvaging=new CheckBox
						{
							 Content="ItemRules Salvaging",
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.ItemRulesSalvaging),
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+10, Margin.Bottom),
						};
						CBItemRulesSalvaging.Checked+=ItemRulesSalvagingChecked;
						CBItemRulesSalvaging.Unchecked+=ItemRulesSalvagingChecked;
						spItemRulesOptions.Children.Add(CBItemRulesSalvaging);
						#endregion
						#region ItemRules Unid Stashing Checkbox
						CheckBox CBItemRulesUnidStashing=new CheckBox
						{
							 Content="ItemRules Unid Stashing",
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.ItemRulesUnidStashing),
						};
						CBItemRulesSalvaging.Checked+=ItemRulesUnidStashingChecked;
						CBItemRulesSalvaging.Unchecked+=ItemRulesUnidStashingChecked;
						spItemRulesOptions.Children.Add(CBItemRulesUnidStashing);
						#endregion
						spItemRules.Children.Add(spItemRulesOptions);
						#region ItemRules Rule Set
						TextBlock txt_ItemRulesRule=new TextBlock
						{
							 Text="Rule Set",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.DarkSlateGray,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spItemRules.Children.Add(txt_ItemRulesRule);

						StackPanel spItemRules_RuleSet=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};

						ItemRuleType=new ComboBox
						{
							 Height=30,
							 Width=150,
							 ItemsSource=new ItemRuleTypes(),
							 //Text=Bot.SettingsFunky.ItemRuleType.ToString(),
						};
						ItemRuleType.SelectedIndex=Bot.SettingsFunky.ItemRuleType.ToLower().Contains("soft")?1:Bot.SettingsFunky.ItemRuleType.ToLower().Contains("hard")?2:0;
						ItemRuleType.SelectionChanged+=ItemRulesTypeChanged;
						spItemRules_RuleSet.Children.Add(ItemRuleType);

						tbCustomItemRulePath=new TextBox
						{
							 Height=30,
							 Width=300,
							 Text=Bot.SettingsFunky.ItemRuleCustomPath,
						};
						spItemRules_RuleSet.Children.Add(tbCustomItemRulePath);

						Button btnCustomItemRulesBrowse=new Button
						{
							 Content="Browse",
						};
						btnCustomItemRulesBrowse.Click+=ItemRulesBrowse_Click;
						spItemRules_RuleSet.Children.Add(btnCustomItemRulesBrowse);

						spItemRules.Children.Add(spItemRules_RuleSet);
						#endregion

						#region ItemRulesLogging
						TextBlock txt_Header_ItemRulesLogging=new TextBlock
						{
							 Text="Logging",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Background=System.Windows.Media.Brushes.DarkSlateGray,
							 Margin=new Thickness(Margin.Left, Margin.Top+10, Margin.Right, Margin.Bottom+5),
						};
						spItemRules.Children.Add(txt_Header_ItemRulesLogging);

						StackPanel spItemRulesLogging=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						#region Log Items Stashed
						StackPanel spItemRulesLoggingKeep=new StackPanel();
						TextBlock txt_LogItemKeep=new TextBlock
						{
							 Text="Items Stashed",
							 FontSize=11,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
						};
						spItemRulesLoggingKeep.Children.Add(txt_LogItemKeep);
						ItemRuleLogKeep=new ComboBox
						{
							 Height=30,
							 Width=150,
							 ItemsSource=new ItemRuleQuality(),
							 Text=Bot.SettingsFunky.ItemRuleLogKeep
						};
						ItemRuleLogKeep.SelectionChanged+=ItemRulesLogKeepChanged;
						spItemRulesLoggingKeep.Children.Add(ItemRuleLogKeep);
						spItemRulesLogging.Children.Add(spItemRulesLoggingKeep);
						#endregion

						#region Log Items Pickup
						StackPanel spItemRulesLoggingPickup=new StackPanel();
						TextBlock txt_LogItemPickup=new TextBlock
						{
							 Text="Items Pickup",
							 FontSize=11,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
						};
						spItemRulesLoggingPickup.Children.Add(txt_LogItemPickup);
						ItemRuleLogPickup=new ComboBox
						{
							 Height=30,
							 Width=150,
							 ItemsSource=new ItemRuleQuality(),
							 Text=Bot.SettingsFunky.ItemRuleLogPickup
						};
						ItemRuleLogPickup.SelectionChanged+=ItemRulesLogPickupChanged;
						spItemRulesLoggingPickup.Children.Add(ItemRuleLogPickup);
						spItemRulesLogging.Children.Add(spItemRulesLoggingPickup);
						#endregion

						spItemRules.Children.Add(spItemRulesLogging);
						#endregion

						TextBlock txt_ItemRulesMisc=new TextBlock
						{
							 Text="Misc",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.DarkSlateGray,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top+10, Margin.Right, Margin.Bottom+5),
						};
						spItemRules.Children.Add(txt_ItemRulesMisc);

						StackPanel spItemRulesMisc=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						#region ItemRulesIDs
						ItemRuleUseItemIDs=new CheckBox
						{
							 Content="Use Item IDs",
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.ItemRuleUseItemIDs),
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),

						};
						ItemRuleUseItemIDs.Checked+=ItemRulesItemIDsChecked;
						ItemRuleUseItemIDs.Unchecked+=ItemRulesItemIDsChecked;
						spItemRulesMisc.Children.Add(ItemRuleUseItemIDs);

						#endregion
						#region ItemRulesDebug
						ItemRuleDebug=new CheckBox
						{
							 Content="Debugging",
							 Height=30,
							 IsChecked=(Bot.SettingsFunky.ItemRuleDebug),
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom),

						};
						ItemRuleDebug.Checked+=ItemRulesDebugChecked;
						ItemRuleDebug.Unchecked+=ItemRulesDebugChecked;
						spItemRulesMisc.Children.Add(ItemRuleDebug);

						#endregion
						spItemRules.Children.Add(spItemRulesMisc);

						lbItemRulesContent.Items.Add(spItemRules);

						Button ItemRulesExploreFolder=new Button
						{
							 Content="Open Item Rules Folder",
							 Width=300,
							 Height=30
						};
						ItemRulesExploreFolder.Click+=ItemRulesOpenFolder_Click;
						lbItemRulesContent.Items.Add(ItemRulesExploreFolder);

						ItemRulesReload=new Button
								 {
										Content="Reload rules",
										Width=300,
										Height=30
								 };
						ItemRulesReload.Click+=ItemRulesReload_Click;
						lbItemRulesContent.Items.Add(ItemRulesReload);



						#region DefaultItemScoring
						StackPanel spDefaultItemScoring=new StackPanel();
						TextBlock Text_DefaultItemScoring=new TextBlock
						{
							 Text="Default Scoring Option",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 TextAlignment=TextAlignment.Left,
						};
						spDefaultItemScoring.Children.Add(Text_DefaultItemScoring);
						ItemRuleGilesScoring=new RadioButton
						{
							 GroupName="Scoring",
							 Content="Giles Item Scoring",
							 Width=300,
							 Height=30,
							 IsChecked=Bot.SettingsFunky.ItemRuleGilesScoring,
							 IsEnabled=!Bot.SettingsFunky.UseItemRules,
						};
						ItemRuleDBScoring=new RadioButton
						{
							 GroupName="Scoring",
							 Content="DB Weight Scoring",
							 Width=300,
							 Height=30,
							 IsChecked=!Bot.SettingsFunky.ItemRuleGilesScoring,
							 IsEnabled=!Bot.SettingsFunky.UseItemRules,
						};
						ItemRuleGilesScoring.Checked+=ItemRulesScoringChanged;
						ItemRuleDBScoring.Checked+=ItemRulesScoringChanged;
						spDefaultItemScoring.Children.Add(ItemRuleGilesScoring);
						spDefaultItemScoring.Children.Add(ItemRuleDBScoring);
						lbItemRulesContent.Items.Add(spDefaultItemScoring);
						#endregion


						ItemRulesTabItem.Content=lbItemRulesContent;
						#endregion

						#region Pickup
						TabItem ItemGilesTabItem=new TabItem();
						ItemGilesTabItem.Header="Pickup";
						tcItems.Items.Add(ItemGilesTabItem);
						ListBox lbGilesContent=new ListBox();

						#region Item Level Pickup
						StackPanel spItemPickupLevel=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						#region minimumWeaponILevel
						StackPanel spWeaponPickupLevel=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};

						TextBlock txt_weaponIlvl=new TextBlock
						{
							 Text="Weapons",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.DarkSlateGray,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spWeaponPickupLevel.Children.Add(txt_weaponIlvl);

						StackPanel spWeaponPickupMagical=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						TextBlock txt_weaponMagical=new TextBlock
						{
							 Text="Magic",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.DarkBlue,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						TBMinimumWeaponLevel=new TextBox[2];
						Slider weaponMagicLevelSlider=new Slider
						{
							 Name="Magic",
							 Width=120,
							 Maximum=63,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.MinimumWeaponItemLevel[0],
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						weaponMagicLevelSlider.ValueChanged+=WeaponItemLevelSliderChanged;
						TBMinimumWeaponLevel[0]=new TextBox
						{
							 Text=Bot.SettingsFunky.MinimumWeaponItemLevel[0].ToString(),
							 IsReadOnly=true,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spWeaponPickupMagical.Children.Add(txt_weaponMagical);
						spWeaponPickupMagical.Children.Add(weaponMagicLevelSlider);
						spWeaponPickupMagical.Children.Add(TBMinimumWeaponLevel[0]);
						spWeaponPickupLevel.Children.Add(spWeaponPickupMagical);


						StackPanel spWeaponPickupRare=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						TextBlock txt_weaponRare=new TextBlock
						{
							 Text="Rare",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.Black,
							 Background=System.Windows.Media.Brushes.Gold,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						Slider weaponRareLevelSlider=new Slider
						{
							 Name="Rare",
							 Width=120,
							 Maximum=63,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.MinimumWeaponItemLevel[1],
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						weaponRareLevelSlider.ValueChanged+=WeaponItemLevelSliderChanged;
						TBMinimumWeaponLevel[1]=new TextBox
						{
							 Text=Bot.SettingsFunky.MinimumWeaponItemLevel[1].ToString(),
							 IsReadOnly=true,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spWeaponPickupRare.Children.Add(txt_weaponRare);
						spWeaponPickupRare.Children.Add(weaponRareLevelSlider);
						spWeaponPickupRare.Children.Add(TBMinimumWeaponLevel[1]);
						spWeaponPickupLevel.Children.Add(spWeaponPickupRare);

						spItemPickupLevel.Children.Add(spWeaponPickupLevel);
						#endregion
						#region minimumArmorILevel
						StackPanel spArmorPickupLevel=new StackPanel
						{
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};

						TBMinimumArmorLevel=new TextBox[2];
						TextBlock txt_armorIlvl=new TextBlock
						{
							 Text="Armor",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.DarkSlateGray,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spArmorPickupLevel.Children.Add(txt_armorIlvl);

						StackPanel spArmorPickupMagical=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						TextBlock txt_armorMagic=new TextBlock
						{
							 Text="Magic",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.DarkBlue,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						Slider armorMagicLevelSlider=new Slider
						{
							 Name="Magic",
							 Width=120,
							 Maximum=63,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.MinimumArmorItemLevel[0],
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						armorMagicLevelSlider.ValueChanged+=ArmorItemLevelSliderChanged;
						TBMinimumArmorLevel[0]=new TextBox
						{
							 Text=Bot.SettingsFunky.MinimumArmorItemLevel[0].ToString(),
							 IsReadOnly=true,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spArmorPickupMagical.Children.Add(txt_armorMagic);
						spArmorPickupMagical.Children.Add(armorMagicLevelSlider);
						spArmorPickupMagical.Children.Add(TBMinimumArmorLevel[0]);
						spArmorPickupLevel.Children.Add(spArmorPickupMagical);


						StackPanel spArmorPickupRare=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						TextBlock txt_armorRare=new TextBlock
						{
							 Text="Rare",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.Black,
							 Background=System.Windows.Media.Brushes.Gold,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						Slider armorRareLevelSlider=new Slider
						{
							 Name="Rare",
							 Maximum=63,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Width=120,
							 Value=Bot.SettingsFunky.MinimumArmorItemLevel[1],
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						armorRareLevelSlider.ValueChanged+=ArmorItemLevelSliderChanged;
						TBMinimumArmorLevel[1]=new TextBox
						{
							 Text=Bot.SettingsFunky.MinimumArmorItemLevel[1].ToString(),
							 IsReadOnly=true,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spArmorPickupRare.Children.Add(txt_armorRare);
						spArmorPickupRare.Children.Add(armorRareLevelSlider);
						spArmorPickupRare.Children.Add(TBMinimumArmorLevel[1]);
						spArmorPickupLevel.Children.Add(spArmorPickupRare);

						spItemPickupLevel.Children.Add(spArmorPickupLevel);
						#endregion
						#region minimumJeweleryILevel
						TBMinimumJeweleryLevel=new TextBox[2];
						TextBlock txt_jeweleryIlvl=new TextBlock
						{
							 Text="Jewelery",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.DarkSlateGray,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						StackPanel spJeweleryPickupLevel=new StackPanel
						{

						};
						spJeweleryPickupLevel.Children.Add(txt_jeweleryIlvl);

						StackPanel spJeweleryPickupMagical=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						TextBlock txt_jeweleryMagic=new TextBlock
						{
							 Text="Magic",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.DarkBlue,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						Slider jeweleryMagicLevelSlider=new Slider
						{
							 Name="Magic",
							 Width=120,
							 Maximum=63,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.MinimumJeweleryItemLevel[0],
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						jeweleryMagicLevelSlider.ValueChanged+=JeweleryItemLevelSliderChanged;
						TBMinimumJeweleryLevel[0]=new TextBox
						{
							 Text=Bot.SettingsFunky.MinimumJeweleryItemLevel[0].ToString(),
							 IsReadOnly=true,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spJeweleryPickupMagical.Children.Add(txt_jeweleryMagic);
						spJeweleryPickupMagical.Children.Add(jeweleryMagicLevelSlider);
						spJeweleryPickupMagical.Children.Add(TBMinimumJeweleryLevel[0]);
						spJeweleryPickupLevel.Children.Add(spJeweleryPickupMagical);

						StackPanel spJeweleryPickupRare=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						TextBlock txt_jeweleryRare=new TextBlock
						{
							 Text="Rare",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.Black,
							 Background=System.Windows.Media.Brushes.Gold,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						Slider jeweleryRareLevelSlider=new Slider
						{
							 Name="Rare",
							 Maximum=63,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Width=120,
							 Value=Bot.SettingsFunky.MinimumJeweleryItemLevel[1],
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						jeweleryRareLevelSlider.ValueChanged+=JeweleryItemLevelSliderChanged;
						TBMinimumJeweleryLevel[1]=new TextBox
						{
							 Text=Bot.SettingsFunky.MinimumJeweleryItemLevel[1].ToString(),
							 IsReadOnly=true,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spJeweleryPickupRare.Children.Add(txt_jeweleryRare);
						spJeweleryPickupRare.Children.Add(jeweleryRareLevelSlider);
						spJeweleryPickupRare.Children.Add(TBMinimumJeweleryLevel[1]);
						spJeweleryPickupLevel.Children.Add(spJeweleryPickupRare);
						spItemPickupLevel.Children.Add(spJeweleryPickupLevel);
						#endregion

						StackPanel spItemPickup=new StackPanel
						{
							 Background=System.Windows.Media.Brushes.DimGray,
						};
						TextBlock Text_Header_ItemPickup=new TextBlock
						{
							 Text="Item Level Pickup",
							 FontSize=14,
							 Background=System.Windows.Media.Brushes.DimGray,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 TextAlignment=TextAlignment.Center,
						};
						spItemPickup.Children.Add(Text_Header_ItemPickup);
						spItemPickup.Children.Add(spItemPickupLevel);

						#region LegendaryLevel
						TextBlock Text_Legendary_ItemLevel=new TextBlock
						{
							 Text="Legendary Items",
							 FontSize=12,
							 //Background=System.Windows.Media.Brushes.DarkGreen,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 TextAlignment=TextAlignment.Left,
						};
						spItemPickup.Children.Add(Text_Legendary_ItemLevel);
						Slider sliderLegendaryILevel=new Slider
						{
							 Width=120,
							 Maximum=63,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.MinimumLegendaryItemLevel,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderLegendaryILevel.ValueChanged+=LegendaryItemLevelSliderChanged;
						TBMinLegendaryLevel=new TextBox
						{
							 Text=Bot.SettingsFunky.MinimumLegendaryItemLevel.ToString(),
							 IsReadOnly=true,
						};
						StackPanel LegendaryILvlStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						LegendaryILvlStackPanel.Children.Add(sliderLegendaryILevel);
						LegendaryILvlStackPanel.Children.Add(TBMinLegendaryLevel);

						spItemPickup.Children.Add(LegendaryILvlStackPanel);
						#endregion


						#region MinMiscItemLevel
						TextBlock Text_Misc_ItemLevel=new TextBlock
						{
							 Text="Misc Item",
							 FontSize=12,
							 //Background=System.Windows.Media.Brushes.DarkGreen,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
							 TextAlignment=TextAlignment.Left,
						};
						spItemPickup.Children.Add(Text_Misc_ItemLevel);
						Slider slideMinMiscItemLevel=new Slider
						{
							 Width=100,
							 Maximum=63,
							 Minimum=0,
							 TickFrequency=5,
							 LargeChange=5,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.MiscItemLevel,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						slideMinMiscItemLevel.ValueChanged+=MiscItemLevelSliderChanged;
						TBMiscItemLevel=new TextBox
						{
							 Text=Bot.SettingsFunky.MiscItemLevel.ToString(),
							 IsReadOnly=true,
						};
						StackPanel MinMiscItemLevelStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						MinMiscItemLevelStackPanel.Children.Add(slideMinMiscItemLevel);
						MinMiscItemLevelStackPanel.Children.Add(TBMiscItemLevel);
						spItemPickup.Children.Add(MinMiscItemLevelStackPanel);
						#endregion


						lbGilesContent.Items.Add(spItemPickup);
						#endregion



						#region MaxHealthPotions
						Slider sliderMaxHealthPots=new Slider
						{
							 Width=100,
							 Maximum=100,
							 Minimum=0,
							 TickFrequency=25,
							 LargeChange=20,
							 SmallChange=5,
							 Value=Bot.SettingsFunky.MaximumHealthPotions,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderMaxHealthPots.ValueChanged+=HealthPotionSliderChanged;
						TBMaxHealthPots=new TextBox
						{
							 Text=Bot.SettingsFunky.MaximumHealthPotions.ToString(),
							 IsReadOnly=true,
						};
						StackPanel MaxHealthPotsStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						MaxHealthPotsStackPanel.Children.Add(sliderMaxHealthPots);
						MaxHealthPotsStackPanel.Children.Add(TBMaxHealthPots);
						StackPanel spHealthPotions=new StackPanel();

						TextBlock txt_HealthPotions=new TextBlock
						{
							 Text="Health Potions",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spHealthPotions.Children.Add(txt_HealthPotions);
						spHealthPotions.Children.Add(MaxHealthPotsStackPanel);
						#endregion

						#region MinimumGoldPile
						Slider slideMinGoldPile=new Slider
						{
							 Width=120,
							 Maximum=7500,
							 Minimum=0,
							 TickFrequency=1000,
							 LargeChange=1000,
							 SmallChange=1,
							 Value=Bot.SettingsFunky.MinimumGoldPile,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						slideMinGoldPile.ValueChanged+=GoldAmountSliderChanged;
						TBMinGoldPile=new TextBox
						{
							 Text=Bot.SettingsFunky.MinimumGoldPile.ToString(),
							 IsReadOnly=true,
						};
						StackPanel MinGoldPileStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						MinGoldPileStackPanel.Children.Add(slideMinGoldPile);
						MinGoldPileStackPanel.Children.Add(TBMinGoldPile);
						StackPanel spMinimumGold=new StackPanel();

						TextBlock txt_MinimumGold=new TextBlock
						{
							 Text="Minimum Gold Pile",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spMinimumGold.Children.Add(txt_MinimumGold);
						spMinimumGold.Children.Add(MinGoldPileStackPanel);
						#endregion


						StackPanel spCraftPlans=new StackPanel
						{
							 Background=System.Windows.Media.Brushes.DimGray,
						};
						TextBlock txt_CraftPlansPickup=new TextBlock
						{
							 Text="Craft Plan Options",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.DarkSlateGray,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						spCraftPlans.Children.Add(txt_CraftPlansPickup);

						#region PickupCraftPlans
						CheckBox cbPickupCraftPlans=new CheckBox
						{
							 Content="Pickup Craft Plans",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupCraftPlans),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbPickupCraftPlans.Checked+=PickupCraftPlansChecked;
						cbPickupCraftPlans.Unchecked+=PickupCraftPlansChecked;
						spCraftPlans.Children.Add(cbPickupCraftPlans);
						#endregion

						spBlacksmithPlans=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
							 IsEnabled=Bot.SettingsFunky.PickupCraftPlans,
						};
						StackPanel spBlacksmithPlansProperties=new StackPanel();

						#region Blacksmith_property_six
						CheckBox cbBlacksmithPropertySix=new CheckBox
						{
							 Content="Plans: Property Six",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupBlacksmithPlanSix),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbBlacksmithPropertySix.Checked+=PickupBlacksmithPlanSixChecked;
						cbBlacksmithPropertySix.Unchecked+=PickupBlacksmithPlanSixChecked;
						spBlacksmithPlansProperties.Children.Add(cbBlacksmithPropertySix);
						#endregion

						#region Blacksmith_property_five
						CheckBox cbBlacksmithPropertyFive=new CheckBox
						{
							 Content="Plans: Property Five",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupBlacksmithPlanFive),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbBlacksmithPropertyFive.Checked+=PickupBlacksmithPlanFiveChecked;
						cbBlacksmithPropertyFive.Unchecked+=PickupBlacksmithPlanFiveChecked;
						spBlacksmithPlansProperties.Children.Add(cbBlacksmithPropertyFive);
						#endregion

						#region Blacksmith_property_four
						CheckBox cbBlacksmithPropertyFour=new CheckBox
						{
							 Content="Plans: Property Four",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupBlacksmithPlanFour),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbBlacksmithPropertyFive.Checked+=PickupBlacksmithPlanFourChecked;
						cbBlacksmithPropertyFive.Unchecked+=PickupBlacksmithPlanFourChecked;
						spBlacksmithPlansProperties.Children.Add(cbBlacksmithPropertyFour);
						#endregion

						spBlacksmithPlans.Children.Add(spBlacksmithPlansProperties);

						StackPanel spBlacksmithPlansArchonRazor=new StackPanel();

						#region Blacksmith_archon_gauntlets
						CheckBox cbBlacksmithArchonGauntlets=new CheckBox
						{
							 Content="Plans: Archon Gauntlets",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupBlacksmithPlanArchonGauntlets),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbBlacksmithArchonGauntlets.Checked+=PickupBlacksmithPlanArchonGauntletsChecked;
						cbBlacksmithArchonGauntlets.Unchecked+=PickupBlacksmithPlanArchonGauntletsChecked;
						spBlacksmithPlansArchonRazor.Children.Add(cbBlacksmithArchonGauntlets);
						#endregion

						#region Blacksmith_archon_spaulders
						CheckBox cbBlacksmithArchonSpaulders=new CheckBox
						{
							 Content="Plans: Archon Spaulders",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupBlacksmithPlanArchonSpaulders),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbBlacksmithArchonSpaulders.Checked+=PickupBlacksmithPlanArchonSpauldersChecked;
						cbBlacksmithArchonSpaulders.Unchecked+=PickupBlacksmithPlanArchonSpauldersChecked;
						spBlacksmithPlansArchonRazor.Children.Add(cbBlacksmithArchonSpaulders);
						#endregion

						#region Blacksmith_archon_razorspikes
						CheckBox cbBlacksmithRazorSpikes=new CheckBox
						{
							 Content="Plans: Razorspikes",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupBlacksmithPlanRazorspikes),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbBlacksmithRazorSpikes.Checked+=PickupBlacksmithPlanRazorspikesChecked;
						cbBlacksmithRazorSpikes.Unchecked+=PickupBlacksmithPlanRazorspikesChecked;
						spBlacksmithPlansArchonRazor.Children.Add(cbBlacksmithRazorSpikes);
						#endregion

						spBlacksmithPlans.Children.Add(spBlacksmithPlansArchonRazor);
						spCraftPlans.Children.Add(spBlacksmithPlans);

						spJewelerPlans=new StackPanel
						{
							 IsEnabled=Bot.SettingsFunky.PickupCraftPlans,
						};

						#region design_flawlessStar
						CheckBox cbJewelerFlawlessStar=new CheckBox
						{
							 Content="Design: Flawless Star",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupJewelerDesignFlawlessStar),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbJewelerFlawlessStar.Checked+=PickupJewelerDesignFlawlessStarChecked;
						cbJewelerFlawlessStar.Unchecked+=PickupJewelerDesignFlawlessStarChecked;
						spJewelerPlans.Children.Add(cbJewelerFlawlessStar);
						#endregion

						#region design_perfectstar
						CheckBox cbJewelerPerfectStar=new CheckBox
						{
							 Content="Design: Perfect Star",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupJewelerDesignPerfectStar),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbJewelerPerfectStar.Checked+=PickupJewelerDesignPerfectStarChecked;
						cbJewelerPerfectStar.Unchecked+=PickupJewelerDesignPerfectStarChecked;
						spJewelerPlans.Children.Add(cbJewelerPerfectStar);
						#endregion

						#region design_radiantstar
						CheckBox cbJewelerRadiantStar=new CheckBox
						{
							 Content="Design: Radiant Star",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupJewelerDesignRadiantStar),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbJewelerRadiantStar.Checked+=PickupJewelerDesignRadiantStarChecked;
						cbJewelerRadiantStar.Unchecked+=PickupJewelerDesignRadiantStarChecked;
						spJewelerPlans.Children.Add(cbJewelerRadiantStar);
						#endregion

						#region design_marquise
						CheckBox cbJewelerMarquise=new CheckBox
						{
							 Content="Design: Marquise",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupJewelerDesignMarquise),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbJewelerMarquise.Checked+=PickupJewelerDesignMarquiseChecked;
						cbJewelerMarquise.Unchecked+=PickupJewelerDesignMarquiseChecked;
						spJewelerPlans.Children.Add(cbJewelerMarquise);
						#endregion

						#region design_amulets
						CheckBox cbJewelerAmulets=new CheckBox
						{
							 Content="Design: Amulets",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupJewelerDesignAmulet),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbJewelerAmulets.Checked+=PickupJewelerDesignAmuletChecked;
						cbJewelerAmulets.Unchecked+=PickupJewelerDesignAmuletChecked;
						spJewelerPlans.Children.Add(cbJewelerAmulets);
						#endregion

						spCraftPlans.Children.Add(spJewelerPlans);

						lbGilesContent.Items.Add(spCraftPlans);


						StackPanel spMiscItemPickupOptions=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						#region PickupCraftTomes
						CheckBox cbPickupCraftTomes=new CheckBox
						{
							 Content="Pickup Craft Tomes",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupCraftTomes),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbPickupCraftTomes.Checked+=PickupCraftTomesChecked;
						cbPickupCraftTomes.Unchecked+=PickupCraftTomesChecked;
						spMiscItemPickupOptions.Children.Add(cbPickupCraftTomes);
						#endregion
						#region PickupFollowerItems
						CheckBox cbPickupFollowerItems=new CheckBox
						{
							 Content="Pickup Follower Items",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupFollowerItems),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbPickupFollowerItems.Checked+=PickupFollowerItemsChecked;
						cbPickupFollowerItems.Unchecked+=PickupFollowerItemsChecked;
						spMiscItemPickupOptions.Children.Add(cbPickupFollowerItems);
						#endregion
						#region PickupInfernalKeys
						CheckBox cbPickupInfernalKeys=new CheckBox
						{
							 Content="Pickup Inferno Keys",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupInfernalKeys),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbPickupInfernalKeys.Checked+=PickupInfernalKeysChecked;
						cbPickupInfernalKeys.Unchecked+=PickupInfernalKeysChecked;
						spMiscItemPickupOptions.Children.Add(cbPickupInfernalKeys);
						#endregion
						#region PickupDemonicEssence
						CheckBox cbPickupDemonicEssence=new CheckBox
						{
							 Content="Pickup Demonic Essences",
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupDemonicEssence),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
						};
						cbPickupDemonicEssence.Checked+=PickupDemonicEssenceChecked;
						cbPickupDemonicEssence.Unchecked+=PickupDemonicEssenceChecked;
						spMiscItemPickupOptions.Children.Add(cbPickupDemonicEssence);
						#endregion
						//
						TextBlock txt_miscPickup=new TextBlock
						{
							 Text="Misc Pickup Options",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.DarkSlateGray,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
						};
						StackPanel spMiscItemPickup=new StackPanel
						{
							 Background=System.Windows.Media.Brushes.DimGray,
						};
						spMiscItemPickup.Children.Add(txt_miscPickup);
						spMiscItemPickup.Children.Add(spMiscItemPickupOptions);
						spMiscItemPickup.Children.Add(spMinimumGold);
						spMiscItemPickup.Children.Add(spHealthPotions);

						lbGilesContent.Items.Add(spMiscItemPickup);


						#region Gems
						StackPanel spGemOptions=new StackPanel
						{
							 Orientation=Orientation.Vertical,
							 Background=System.Windows.Media.Brushes.DimGray,
						};
						TextBlock Text_GemOptions=new TextBlock
						{
							 Text="Gems",
							 FontSize=12,
							 Foreground=System.Windows.Media.Brushes.Black,
							 Background=System.Windows.Media.Brushes.Gold,
							 TextAlignment=TextAlignment.Left,
						};
						spGemOptions.Children.Add(Text_GemOptions);

						#region GemQuality
						TextBlock Text_MinimumGemQuality=new TextBlock
						{
							 Text="Minimum Gem Quality",
							 FontSize=11,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
							 TextAlignment=TextAlignment.Left,
						};
						ComboBox CBGemQualityLevel=new ComboBox
						{
							 Height=20,
							 ItemsSource=new GemQualityTypes(),
							 Text=Enum.GetName(typeof(GemQuality), Bot.SettingsFunky.MinimumGemItemLevel).ToString(),
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
							 Margin=new Thickness(5),
						};
						CBGemQualityLevel.SelectionChanged+=GemQualityLevelChanged;
						spGemOptions.Children.Add(Text_MinimumGemQuality);
						spGemOptions.Children.Add(CBGemQualityLevel);
						#endregion

						CBGems=new CheckBox[4];

						StackPanel spGemColorsREDGREEN=new StackPanel
						{
							 Orientation=Orientation.Vertical,
						};
						#region PickupGemsRed
						CBGems[0]=new CheckBox
						{
							 Content="Pickup Gem Red",
							 Name="red",
							 Width=150,
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupGems[0])
						};
						CBGems[0].Checked+=GemsChecked;
						CBGems[0].Unchecked+=GemsChecked;
						spGemColorsREDGREEN.Children.Add(CBGems[0]);
						#endregion
						#region PickupGemsGreen
						CBGems[1]=new CheckBox
						{
							 Content="Pickup Gem Green",
							 Name="green",
							 Width=150,
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupGems[1])
						};
						CBGems[1].Checked+=GemsChecked;
						CBGems[1].Unchecked+=GemsChecked;
						spGemColorsREDGREEN.Children.Add(CBGems[1]);
						#endregion

						StackPanel spGemColorsPurpleYellow=new StackPanel
						{
							 Orientation=Orientation.Vertical,
						};
						#region PickupGemsPurple
						CBGems[2]=new CheckBox
						{
							 Content="Pickup Gem Purple",
							 Name="purple",
							 Width=150,
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupGems[2])
						};
						CBGems[2].Checked+=GemsChecked;
						CBGems[2].Unchecked+=GemsChecked;
						spGemColorsPurpleYellow.Children.Add(CBGems[2]);
						#endregion
						#region PickupGemsYellow
						CBGems[3]=new CheckBox
						{
							 Content="Pickup Gem Yellow",
							 Name="yellow",
							 Width=150,
							 Height=20,
							 IsChecked=(Bot.SettingsFunky.PickupGems[3])
						};
						CBGems[3].Checked+=GemsChecked;
						CBGems[3].Unchecked+=GemsChecked;
						spGemColorsPurpleYellow.Children.Add(CBGems[3]);
						#endregion
						StackPanel spGemColorPanels=new StackPanel
						{
							 Orientation=Orientation.Horizontal,
						};
						spGemColorPanels.Children.Add(spGemColorsREDGREEN);
						spGemColorPanels.Children.Add(spGemColorsPurpleYellow);

						spGemOptions.Children.Add(spGemColorPanels);
						lbGilesContent.Items.Add(spGemOptions);
						#endregion

						ItemGilesTabItem.Content=lbGilesContent;
						#endregion

						#region Scoring
						TabItem ItemGilesScoringTabItem=new TabItem();
						ItemGilesScoringTabItem.Header="Scoring";
						tcItems.Items.Add(ItemGilesScoringTabItem);
						ListBox lbGilesScoringContent=new ListBox();

						#region WeaponScore
						lbGilesScoringContent.Items.Add("Minimum Weapon Score to Stash");
						Slider sliderGilesWeaponScore=new Slider
						{
							 Width=100,
							 Maximum=100000,
							 Minimum=1,
							 TickFrequency=1000,
							 LargeChange=5000,
							 SmallChange=1000,
							 Value=Bot.SettingsFunky.GilesMinimumWeaponScore,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderGilesWeaponScore.ValueChanged+=GilesWeaponScoreSliderChanged;
						TBGilesWeaponScore=new TextBox
						{
							 Text=Bot.SettingsFunky.GilesMinimumWeaponScore.ToString(),
							 IsReadOnly=true,
						};
						StackPanel GilesWeaponScoreStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						GilesWeaponScoreStackPanel.Children.Add(sliderGilesWeaponScore);
						GilesWeaponScoreStackPanel.Children.Add(TBGilesWeaponScore);
						lbGilesScoringContent.Items.Add(GilesWeaponScoreStackPanel);
						#endregion

						#region ArmorScore
						lbGilesScoringContent.Items.Add("Minimum Armor Score to Stash");
						Slider sliderGilesArmorScore=new Slider
						{
							 Width=100,
							 Maximum=100000,
							 Minimum=1,
							 TickFrequency=1000,
							 LargeChange=5000,
							 SmallChange=1000,
							 Value=Bot.SettingsFunky.GilesMinimumArmorScore,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderGilesArmorScore.ValueChanged+=GilesArmorScoreSliderChanged;
						TBGilesArmorScore=new TextBox
						{
							 Text=Bot.SettingsFunky.GilesMinimumArmorScore.ToString(),
							 IsReadOnly=true,
						};
						StackPanel GilesArmorScoreStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						GilesArmorScoreStackPanel.Children.Add(sliderGilesArmorScore);
						GilesArmorScoreStackPanel.Children.Add(TBGilesArmorScore);
						lbGilesScoringContent.Items.Add(GilesArmorScoreStackPanel);
						#endregion

						#region JeweleryScore
						lbGilesScoringContent.Items.Add("Minimum Jewelery Score to Stash");
						Slider sliderGilesJeweleryScore=new Slider
						{
							 Width=100,
							 Maximum=100000,
							 Minimum=1,
							 TickFrequency=1000,
							 LargeChange=5000,
							 SmallChange=1000,
							 Value=Bot.SettingsFunky.GilesMinimumJeweleryScore,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
						};
						sliderGilesJeweleryScore.ValueChanged+=GilesJeweleryScoreSliderChanged;
						TBGilesJeweleryScore=new TextBox
						{
							 Text=Bot.SettingsFunky.GilesMinimumJeweleryScore.ToString(),
							 IsReadOnly=true,
						};
						StackPanel GilesJeweleryScoreStackPanel=new StackPanel
						{
							 Width=600,
							 Height=20,
							 Orientation=Orientation.Horizontal,
						};
						GilesJeweleryScoreStackPanel.Children.Add(sliderGilesJeweleryScore);
						GilesJeweleryScoreStackPanel.Children.Add(TBGilesJeweleryScore);
						lbGilesScoringContent.Items.Add(GilesJeweleryScoreStackPanel);
						#endregion

						ItemGilesScoringTabItem.Content=lbGilesScoringContent;
						#endregion


						CustomSettingsTabItem.Content=tcItems;
						#endregion


						TabItem AdvancedTabItem=new TabItem();
						AdvancedTabItem.Header="Advanced";
						tabControl1.Items.Add(AdvancedTabItem);
						ListBox lbAdvancedContent=new ListBox();

						#region Debug Logging
						StackPanel SPLoggingOptions=new StackPanel();
						TextBlock Logging_Text_Header=new TextBlock
						{
							 Text="Debug Output Options",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.LightSeaGreen,
							 TextAlignment=TextAlignment.Center,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						};
						SPLoggingOptions.Children.Add(Logging_Text_Header);

						CheckBox CBDebugStatusBar=new CheckBox
						{
							 Content="Enable Debug Status Bar",
							 Width=300,
							 Height=20,
							 IsChecked=Bot.SettingsFunky.DebugStatusBar,
						};
						CBDebugStatusBar.Checked+=DebugStatusBarChecked;
						CBDebugStatusBar.Unchecked+=DebugStatusBarChecked;
						SPLoggingOptions.Children.Add(CBDebugStatusBar);


						TextBlock Logging_FunkyLogLevels_Header=new TextBlock
						{
							 Text="Funky Logging Options",
							 FontSize=12,
							 Background=System.Windows.Media.Brushes.MediumSeaGreen,
							 TextAlignment=TextAlignment.Center,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
						};

						StackPanel panelFunkyLogFlags=new StackPanel
						{
							 Orientation= System.Windows.Controls.Orientation.Vertical,
							 VerticalAlignment= System.Windows.VerticalAlignment.Stretch,
						};
						panelFunkyLogFlags.Children.Add(Logging_FunkyLogLevels_Header);

						var LogLevels=Enum.GetValues(typeof(LogLevel));
					   Logger.GetLogLevelName fRetrieveNames=new Logger.GetLogLevelName((s)=>
					   {
							 return Enum.GetName(typeof(LogLevel), s);
					   });

						foreach (var logLevel in LogLevels)
						{
							 LogLevel thisloglevel=(LogLevel)logLevel;
							 if (thisloglevel.Equals(LogLevel.None)) continue;

							 string loglevelName=fRetrieveNames(logLevel);
							 CheckBox cbLevel=new CheckBox
							 {
								  Name=loglevelName,
								  Content=loglevelName,
								  IsChecked=Bot.SettingsFunky.FunkyLogFlags.HasFlag(thisloglevel),
							 };
							 cbLevel.Checked+=FunkyLogLevelChanged;
							 cbLevel.Unchecked+=FunkyLogLevelChanged;

							 panelFunkyLogFlags.Children.Add(cbLevel);
						}
						SPLoggingOptions.Children.Add(panelFunkyLogFlags);

						lbAdvancedContent.Items.Add(SPLoggingOptions);
						#endregion

						


						CheckBox CBSkipAhead=new CheckBox
						{
							 Content="Skip Ahead Feature (TrinityMoveTo/Explore)",
							 Width=300,
							 Height=20,
							 IsChecked=Bot.SettingsFunky.SkipAhead,
						};
						CBSkipAhead.Checked+=SkipAheadChecked;
						CBSkipAhead.Unchecked+=SkipAheadChecked;
						lbAdvancedContent.Items.Add(CBSkipAhead);


						AdvancedTabItem.Content=lbAdvancedContent;

						TabItem MiscTabItem=new TabItem();
						MiscTabItem.Header="Misc";
						tabControl1.Items.Add(MiscTabItem);
						ListBox lbMiscContent=new ListBox();
						try
						{
							 lbMiscContent.Items.Add(ReturnLogOutputString());
						}
						catch
						{
							 lbMiscContent.Items.Add("Exception Handled");
						}

						MiscTabItem.Content=lbMiscContent;



						TabItem DebuggingTabItem=new TabItem
						{
							 Header="Debug",
							 VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch,
							 VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
							 //Width=600,
							 //Height=600,
							 //VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch,
							 //HorizontalContentAlignment=System.Windows.HorizontalAlignment.Stretch,
						};
						tabControl1.Items.Add(DebuggingTabItem);
						DockPanel DockPanel_Debug=new DockPanel
						{
							 LastChildFill=true,
							 FlowDirection=System.Windows.FlowDirection.LeftToRight,

						};

						//ListBox lbDebugContent=new ListBox();
						//lbDebugContent.HorizontalContentAlignment=System.Windows.HorizontalAlignment.Stretch;
						//lbDebugContent.VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch;

						Button btnObjects_Debug=new Button
						{
							 Content="Object Cache",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="Objects",
						};
						btnObjects_Debug.Click+=DebugButtonClicked;
						Button btnObstacles_Debug=new Button
						{
							 Content="Obstacle Cache",
							 Width=80,
							 FontSize=10,
							 Height=25,
							 Name="Obstacles",
						};
						btnObstacles_Debug.Click+=DebugButtonClicked;
						Button btnSNO_Debug=new Button
						{
							 Content="SNO Cache",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="SNO",
						};
						btnSNO_Debug.Click+=DebugButtonClicked;
						Button btnAbility_Debug=new Button
						{
							 Content="Ability Cache",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="Ability",
						};
						btnAbility_Debug.Click+=DebugButtonClicked;
						Button btnMGP_Debug=new Button
						{
							 Content="MGP Details",
							 Width=150,
							 Height=25,
							 Name="MGP",
						};
						btnMGP_Debug.Click+=DebugButtonClicked;
						Button btnCharacterCache_Debug=new Button
						{
							 Content="Character",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="CHARACTER",
						};
						btnCharacterCache_Debug.Click+=DebugButtonClicked;
						Button btnTargetMovement_Debug=new Button
						{
							 Content="TargetMove",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="TargetMove",
						};
						btnTargetMovement_Debug.Click+=DebugButtonClicked;
						Button btnCombatCache_Debug=new Button
						{
							 Content="CombatCache",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="CombatCache",
						};
						btnCombatCache_Debug.Click+=DebugButtonClicked;
						Button btnTEST_Debug=new Button
						{
							 Content="Test",
							 FontSize=10,
							 Width=80,
							 Height=25,
							 Name="TEST",
						};
						btnTEST_Debug.Click+=DebugButtonClicked;
						StackPanel StackPanel_DebugButtons=new StackPanel
						{
							 Height=40,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
							 FlowDirection=System.Windows.FlowDirection.LeftToRight,
							 Orientation=Orientation.Horizontal,
							 VerticalAlignment=System.Windows.VerticalAlignment.Top,

						};
						StackPanel_DebugButtons.Children.Add(btnObjects_Debug);
						StackPanel_DebugButtons.Children.Add(btnObstacles_Debug);
						StackPanel_DebugButtons.Children.Add(btnSNO_Debug);
						StackPanel_DebugButtons.Children.Add(btnAbility_Debug);
						StackPanel_DebugButtons.Children.Add(btnCharacterCache_Debug);
						StackPanel_DebugButtons.Children.Add(btnCombatCache_Debug);
						StackPanel_DebugButtons.Children.Add(btnTEST_Debug);

						//StackPanel_DebugButtons.Children.Add(btnGPC_Debug);

						DockPanel.SetDock(StackPanel_DebugButtons, Dock.Top);
						DockPanel_Debug.Children.Add(StackPanel_DebugButtons);

						LBDebug=new ListBox
						{
							 SelectionMode=SelectionMode.Extended,
							 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
							 VerticalAlignment=System.Windows.VerticalAlignment.Stretch,
							 VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch,
							 HorizontalContentAlignment=System.Windows.HorizontalAlignment.Stretch,
							 FontSize=10,
							 FontStretch=FontStretches.SemiCondensed,
							 Background=System.Windows.Media.Brushes.Black,
							 Foreground=System.Windows.Media.Brushes.GhostWhite,
						};

						DockPanel_Debug.Children.Add(LBDebug);

						DebuggingTabItem.Content=DockPanel_Debug;


						this.AddChild(LBWindowContent);
				 }

			}


	 }
}