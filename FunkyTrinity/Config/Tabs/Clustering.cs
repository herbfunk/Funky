using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FunkyTrinity.Settings;

namespace FunkyTrinity
{
	 internal partial class FunkyWindow : Window
	 {
		  #region EventHandling
		  private void ClusterDistanceSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Cluster.ClusterDistance=Value;
				TBClusterDistance.Text=Value.ToString();
		  }
		  private void ClusterMinUnitSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Cluster.ClusterMinimumUnitCount=Value;
				TBClusterMinUnitCount.Text=Value.ToString();
		  }
		  private void ClusterLowHPValueSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
				Bot.SettingsFunky.Cluster.IgnoreClusterLowHPValue=Value;
				TBClusterLowHPValue.Text=Value.ToString();
		  }
		  private void EnableClusteringTargetLogicChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Cluster.EnableClusteringTargetLogic=!Bot.SettingsFunky.Cluster.EnableClusteringTargetLogic;
		  }
		  private void IgnoreClusteringBotLowHPisChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Cluster.IgnoreClusteringWhenLowHP=!Bot.SettingsFunky.Cluster.IgnoreClusteringWhenLowHP;
		  }

		  private void ClusteringLoadXMLClicked(object sender, EventArgs e)
		  {
				System.Windows.Forms.OpenFileDialog OFD=new System.Windows.Forms.OpenFileDialog
				{
					 InitialDirectory=Path.Combine(Funky.FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
					 RestoreDirectory=false,
					 Filter="xml files (*.xml)|*.xml|All files (*.*)|*.*",
					 Title="Clustering Template",
				};
				System.Windows.Forms.DialogResult OFD_Result=OFD.ShowDialog();

				if (OFD_Result==System.Windows.Forms.DialogResult.OK)
				{
					 try
					 {
						  //;
						  SettingCluster newSettings=SettingCluster.DeserializeFromXML(OFD.FileName);
						  Bot.SettingsFunky.Cluster=newSettings;
						  
						  Funky.funkyConfigWindow.Close();
					 } catch
					 {

					 }
				}

				
		  }

		  #endregion

		  private StackPanel spClusteringOptions;
		  private TextBox TBClusterLowHPValue;

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




				Button BtnClusteringLoadTemplate=new Button
				{
					 Content="Load Setup",
					 Background=System.Windows.Media.Brushes.OrangeRed,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 FontStyle=FontStyles.Italic,
					 FontSize=12,

					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 VerticalAlignment=System.Windows.VerticalAlignment.Top,
					 Width=75,
					 Height=30,

					 Margin=new Thickness(Margin.Left, Margin.Top+5, Margin.Right, Margin.Bottom+5),
				};
				BtnClusteringLoadTemplate.Click+=ClusteringLoadXMLClicked;
				CombatClusteringContentListBox.Items.Add(BtnClusteringLoadTemplate);

				CombatClusterTabItem.Content=CombatClusteringContentListBox;
		  }
	 }
}
