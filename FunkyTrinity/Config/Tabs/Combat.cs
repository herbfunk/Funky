using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FunkyTrinity
{
	 internal partial class FunkyWindow : Window
	 {

		 internal void InitCombatControls()
		 {
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
					IsChecked=Bot.SettingsFunky.Fleeing.EnableFleeingBehavior,
					Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
			  };
			  CBAttemptFleeingBehavior.Checked+=FleeingAttemptMovementChecked;
			  CBAttemptFleeingBehavior.Unchecked+=FleeingAttemptMovementChecked;

			  spFleeingOptions=new StackPanel
			  {
					Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					IsEnabled=Bot.SettingsFunky.Fleeing.EnableFleeingBehavior,
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
					Value=Bot.SettingsFunky.Fleeing.FleeMaxMonsterDistance,
					HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
			  };
			  sliderFleeMonsterDistance.ValueChanged+=FleeMonsterDistanceSliderChanged;
			  TBFleemonsterDistance=new TextBox
			  {
					Text=Bot.SettingsFunky.Fleeing.FleeMaxMonsterDistance.ToString(),
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
					Value=Bot.SettingsFunky.Fleeing.FleeBotMinimumHealthPercent,
					HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
			  };
			  sliderFleeHealthPercent.ValueChanged+=FleeMinimumHealthSliderChanged;
			  TBFleeMinimumHealth=new TextBox
			  {
					Text=Bot.SettingsFunky.Fleeing.FleeBotMinimumHealthPercent.ToString(),
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
					IsChecked=Bot.SettingsFunky.Grouping.AttemptGroupingMovements,
					Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
			  };
			  CBGroupingBehavior.Checked+=GroupingBehaviorChecked;
			  CBGroupingBehavior.Unchecked+=GroupingBehaviorChecked;
			  GroupingOptionsStackPanel.Children.Add(CBGroupingBehavior);

			  spGroupingOptions=new StackPanel
			  {
					Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					IsEnabled=Bot.SettingsFunky.Grouping.AttemptGroupingMovements,
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
					Value=Bot.SettingsFunky.Grouping.GroupingMinimumUnitDistance,
					HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
			  };
			  sliderGroupingMinimumUnitDistance.ValueChanged+=GroupMinimumUnitDistanceSliderChanged;
			  TBGroupingMinUnitDistance=new TextBox
			  {
					Text=Bot.SettingsFunky.Grouping.GroupingMinimumUnitDistance.ToString(),
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
					Value=Bot.SettingsFunky.Grouping.GroupingMaximumDistanceAllowed,
					HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
			  };
			  sliderGroupingMaximumDistance.ValueChanged+=GroupMaxDistanceSliderChanged;
			  TBGroupingMaxDistance=new TextBox
			  {
					Text=Bot.SettingsFunky.Grouping.GroupingMaximumDistanceAllowed.ToString(),
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
					Value=Bot.SettingsFunky.Grouping.GroupingMinimumClusterCount,
					HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
			  };
			  sliderGroupingMinimumCluster.ValueChanged+=GroupMinimumClusterCountSliderChanged;
			  TBGroupingMinimumClusterCount=new TextBox
			  {
					Text=Bot.SettingsFunky.Grouping.GroupingMinimumClusterCount.ToString(),
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
					Value=Bot.SettingsFunky.Grouping.GroupingMinimumUnitsInCluster,
					HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
			  };
			  sliderGroupingMinimumUnits.ValueChanged+=GroupMinimumUnitsInClusterSliderChanged;
			  TBGroupingMinimumUnitsInCluster=new TextBox
			  {
					Text=Bot.SettingsFunky.Grouping.GroupingMinimumUnitsInCluster.ToString(),
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
		 }
	}

}
