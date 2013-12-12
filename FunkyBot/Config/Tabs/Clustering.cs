using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using FunkyBot.Settings;
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
		  #region EventHandling
		  private void ClusterDistanceSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.Settings.Cluster.ClusterDistance=Value;
				TBClusterDistance.Text=Value.ToString();
		  }
		  private void ClusterMinUnitSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.Settings.Cluster.ClusterMinimumUnitCount=Value;
				TBClusterMinUnitCount.Text=Value.ToString();
		  }
		  private void ClusterLowHPValueSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
				Bot.Settings.Cluster.IgnoreClusterLowHPValue=Value;
				TBClusterLowHPValue.Text=Value.ToString();
		  }
		  private void EnableClusteringTargetLogicChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Cluster.EnableClusteringTargetLogic=!Bot.Settings.Cluster.EnableClusteringTargetLogic;
		  }
		  private void IgnoreClusteringBotLowHPisChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Cluster.IgnoreClusteringWhenLowHP=!Bot.Settings.Cluster.IgnoreClusteringWhenLowHP;
		  }

		  private void ClusteringLoadXMLClicked(object sender, EventArgs e)
		  {
				OpenFileDialog OFD=new OpenFileDialog
				{
					 InitialDirectory=Path.Combine(FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
					 RestoreDirectory=false,
					 Filter="xml files (*.xml)|*.xml|All files (*.*)|*.*",
					 Title="Clustering Template",
				};
				DialogResult OFD_Result=OFD.ShowDialog();

				if (OFD_Result==System.Windows.Forms.DialogResult.OK)
				{
					 try
					 {
						  //;
						  SettingCluster newSettings=SettingCluster.DeserializeFromXML(OFD.FileName);
						  Bot.Settings.Cluster=newSettings;

						  funkyConfigWindow.Close();
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

				ToolTip TTClustering=new ToolTip
				{
					 Content="Determines eligible targets using only units from valid clusters",
				};
				spClusteringOptions=new StackPanel
				{
					 Background=Brushes.DimGray,
					 ToolTip=TTClustering,
				};

				

				TextBlock Clustering_Text_Header=new TextBlock
				{
					 Text="Target Clustering Options",
					 FontSize=12,
					 Foreground=Brushes.GhostWhite,
					 Background=Brushes.DarkGreen,
					 TextAlignment=TextAlignment.Center,
					 ToolTip=TTClustering,
				};
				spClusteringOptions.Children.Add(Clustering_Text_Header);

				#region ClusterTargetLogic
				CheckBox cbClusterEnabled=new CheckBox
				{
					 Content="Enable Clustering Target Logic",
					 IsChecked=(Bot.Settings.Cluster.EnableClusteringTargetLogic),
					 HorizontalAlignment=HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+10),
				};
				cbClusterEnabled.Checked+=EnableClusteringTargetLogicChecked;
				cbClusterEnabled.Unchecked+=EnableClusteringTargetLogicChecked;
				spClusteringOptions.Children.Add(cbClusterEnabled);
				#endregion

				#region LowHP
                ToolTip TTClusterLowHPOption = new ToolTip
                {
                    Content = "When bots health is not greater than value set then all units are valid for targeting",
                };
				StackPanel spClusterLowHPOption=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
                     ToolTip = TTClusterLowHPOption,
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
					 Foreground=Brushes.GhostWhite,
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
					 Value=Bot.Settings.Cluster.IgnoreClusterLowHPValue,
					 HorizontalAlignment=HorizontalAlignment.Left,
				};
				sliderClusterLowHPValue.ValueChanged+=ClusterLowHPValueSliderChanged;
				TBClusterLowHPValue=new TextBox
				{
					 Text=Bot.Settings.Cluster.IgnoreClusterLowHPValue.ToString("F2", CultureInfo.InvariantCulture),
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
					 IsChecked=(Bot.Settings.Cluster.IgnoreClusteringWhenLowHP),
					 HorizontalAlignment=HorizontalAlignment.Right,
					 VerticalAlignment=VerticalAlignment.Bottom,
					 Margin=new Thickness(Margin.Left+5, Margin.Top, Margin.Right, Margin.Bottom),
				};
				cbClusterIgnoreBotLowHP.Checked+=IgnoreClusteringBotLowHPisChecked;
				cbClusterIgnoreBotLowHP.Unchecked+=IgnoreClusteringBotLowHPisChecked;
				spClusterLowHPOption.Children.Add(cbClusterIgnoreBotLowHP);
				spClusteringOptions.Children.Add(spClusterLowHPOption);
				#endregion

				#region ClusterDistance
                ToolTip TTClusterDistanceOptions = new ToolTip
                {
                    Content = "The max radius between each unit within a cluster",
                };
				StackPanel spClusterDistanceOptions=new StackPanel
				{
					 Orientation=Orientation.Vertical,
                     ToolTip = TTClusterDistanceOptions,
				};
				TextBlock ClusterDistance_Text_Header=new TextBlock
				{
					 Text="Cluster Distance",
					 FontSize=12,
					 Foreground=Brushes.GhostWhite,
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
					 Value=Bot.Settings.Cluster.ClusterDistance,
					 HorizontalAlignment=HorizontalAlignment.Left,
				};
				sliderClusterDistance.ValueChanged+=ClusterDistanceSliderChanged;
				TBClusterDistance=new TextBox
				{
					 Text=Bot.Settings.Cluster.ClusterDistance.ToString(),
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
                ToolTip TTClusterMinUnitOptions = new ToolTip
                {
                    Content = "Allow only clusters with the minimum number of units",
                };
				StackPanel spClusterMinUnitOptions=new StackPanel
				{
					 Orientation=Orientation.Vertical,
                     ToolTip = TTClusterMinUnitOptions,
				};
				TextBlock ClusterMinUnitCount_Text_Header=new TextBlock
				{
					 Text="Cluster Minimum Unit Count",
					 FontSize=12,
					 Foreground=Brushes.GhostWhite,
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
					 Value=Bot.Settings.Cluster.ClusterMinimumUnitCount,
					 HorizontalAlignment=HorizontalAlignment.Left,
				};
				sliderClusterMinUnitCount.ValueChanged+=ClusterMinUnitSliderChanged;
				TBClusterMinUnitCount=new TextBox
				{
					 Text=Bot.Settings.Cluster.ClusterMinimumUnitCount.ToString(),
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
					 Background=Brushes.OrangeRed,
					 Foreground=Brushes.GhostWhite,
					 FontStyle=FontStyles.Italic,
					 FontSize=12,

					 HorizontalAlignment=HorizontalAlignment.Left,
					 VerticalAlignment=VerticalAlignment.Top,
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
