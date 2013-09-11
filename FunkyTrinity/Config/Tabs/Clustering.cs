using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FunkyTrinity
{
	 internal partial class FunkyWindow : Window
	 {


		  internal void InitClusteringControls()
		  {
				TabItem CombatClusterTabItem=new TabItem();
				CombatClusterTabItem.Header="Clustering";
				CombatTabControl.Items.Add(CombatClusterTabItem);
				ListBox CombatClusteringContentListBox=new ListBox();

				ToolTip TTClustering=new System.Windows.Controls.ToolTip
				{
					 Content="Determines eligible targets using only units from valid clusters",
				};
				spClusteringOptions=new StackPanel
				{
					 Background=System.Windows.Media.Brushes.DimGray,
					 ToolTip=TTClustering,
				};
				Button Clustering_LoadSettings=new Button
				{
					 Content="Load",
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+10),
				};
				Clustering_LoadSettings.Click+=ClusteringLoadXMLClicked;
				spClusteringOptions.Children.Add(Clustering_LoadSettings);

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
					 IsChecked=(Bot.SettingsFunky.Cluster.EnableClusteringTargetLogic),
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
					 Value=Bot.SettingsFunky.Cluster.IgnoreClusterLowHPValue,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				sliderClusterLowHPValue.ValueChanged+=ClusterLowHPValueSliderChanged;
				TBClusterLowHPValue=new TextBox
				{
					 Text=Bot.SettingsFunky.Cluster.IgnoreClusterLowHPValue.ToString("F2", CultureInfo.InvariantCulture),
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
					 IsChecked=(Bot.SettingsFunky.Cluster.IgnoreClusteringWhenLowHP),
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
					 Value=Bot.SettingsFunky.Cluster.ClusterDistance,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				sliderClusterDistance.ValueChanged+=ClusterDistanceSliderChanged;
				TBClusterDistance=new TextBox
				{
					 Text=Bot.SettingsFunky.Cluster.ClusterDistance.ToString(),
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
					 Value=Bot.SettingsFunky.Cluster.ClusterMinimumUnitCount,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				sliderClusterMinUnitCount.ValueChanged+=ClusterMinUnitSliderChanged;
				TBClusterMinUnitCount=new TextBox
				{
					 Text=Bot.SettingsFunky.Cluster.ClusterMinimumUnitCount.ToString(),
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
				ToolTip TTClusteringExceptions=new System.Windows.Controls.ToolTip
				{
					 Content="Exceptions are also used to determine if object is special",
				};
				TextBlock ClusteringExceptions_Text_Header=new TextBlock
				{
					 Text="Clustering Exceptions",
					 FontSize=12,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Background=System.Windows.Media.Brushes.OrangeRed,
					 TextAlignment=TextAlignment.Center,
					 ToolTip=TTClusteringExceptions,
				};
				spClusteringExceptions.Children.Add(ClusteringExceptions_Text_Header);

				#region KillLOWHPUnits
				CheckBox cbClusterKillLowHPUnits=new CheckBox
				{
					 Content="Allow Units with 25% or less HP",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.Cluster.ClusterKillLowHPUnits),
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
					 IsChecked=(Bot.SettingsFunky.Cluster.ClusteringAllowRangedUnits),
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
					 IsChecked=(Bot.SettingsFunky.Cluster.ClusteringAllowSpawnerUnits),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				cbClusteringAllowSpawnerUnits.Checked+=ClusteringAllowSpawnerUnitsChecked;
				cbClusteringAllowSpawnerUnits.Unchecked+=ClusteringAllowSpawnerUnitsChecked;
				spClusteringExceptions.Children.Add(cbClusteringAllowSpawnerUnits);
				#endregion

				#region AllowSucideBombers
				CheckBox cbClusteringAllowSucideBombers=new CheckBox
				{
					 Content="Allow Sucide Bombers",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.Cluster.ClusteringAllowSucideBombers),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				cbClusteringAllowSucideBombers.Checked+=ClusteringAllowSucideBombersChecked;
				cbClusteringAllowSucideBombers.Unchecked+=ClusteringAllowSucideBombersChecked;
				spClusteringExceptions.Children.Add(cbClusteringAllowSucideBombers);
				#endregion

				CombatClusteringContentListBox.Items.Add(spClusteringExceptions);


				CombatClusterTabItem.Content=CombatClusteringContentListBox;
		  }
	 }
}
