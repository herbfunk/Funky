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
		  private void GroupMinimumUnitDistanceSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Grouping.GroupingMinimumUnitDistance=Value;
				TBGroupingMinUnitDistance.Text=Value.ToString();
		  }
		  private void GroupMaxDistanceSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Grouping.GroupingMaximumDistanceAllowed=Value;
				TBGroupingMaxDistance.Text=Value.ToString();
		  }
		  private void GroupMinimumClusterCountSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Grouping.GroupingMinimumClusterCount=Value;
				TBGroupingMinimumClusterCount.Text=Value.ToString();
		  }
		  private void GroupMinimumUnitsInClusterSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Grouping.GroupingMinimumUnitsInCluster=Value;
				TBGroupingMinimumUnitsInCluster.Text=Value.ToString();
		  }
		  private void GroupingBehaviorChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Grouping.AttemptGroupingMovements=!Bot.SettingsFunky.Grouping.AttemptGroupingMovements;
				bool enabled=Bot.SettingsFunky.Grouping.AttemptGroupingMovements;
				spGroupingOptions.IsEnabled=enabled;
		  }
		  private void FleeingAttemptMovementChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Fleeing.EnableFleeingBehavior=!Bot.SettingsFunky.Fleeing.EnableFleeingBehavior;
				bool enabled=Bot.SettingsFunky.Fleeing.EnableFleeingBehavior;
				spFleeingOptions.IsEnabled=enabled;
		  }
		  private void FleeMonsterDistanceSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Fleeing.FleeMaxMonsterDistance=Value;
				TBFleemonsterDistance.Text=Value.ToString();
		  }
		  private void FleeMinimumHealthSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
				Bot.SettingsFunky.Fleeing.FleeBotMinimumHealthPercent=Value;
				TBFleeMinimumHealth.Text=Value.ToString();
		  }
		  private void UseAdvancedProjectileTestingChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.UseAdvancedProjectileTesting=!Bot.SettingsFunky.UseAdvancedProjectileTesting;
		  }
		  private void AvoidanceAttemptMovementChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.AttemptAvoidanceMovements=!Bot.SettingsFunky.AttemptAvoidanceMovements;
		  }
		  private void GlobeHealthSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
				Bot.SettingsFunky.GlobeHealthPercent=Value;
				TBGlobeHealth.Text=Value.ToString();
		  }
		  private void PotionHealthSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				double Value=Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
				Bot.SettingsFunky.PotionHealthPercent=Value;
				TBPotionHealth.Text=Value.ToString();
		  }
		  private void FleeingLoadXMLClicked(object sender, EventArgs e)
		  {
				System.Windows.Forms.OpenFileDialog OFD=new System.Windows.Forms.OpenFileDialog
				{
					 InitialDirectory=Path.Combine(Funky.FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
					 RestoreDirectory=false,
					 Filter="xml files (*.xml)|*.xml|All files (*.*)|*.*",
					 Title="Fleeing Template",
				};
				System.Windows.Forms.DialogResult OFD_Result=OFD.ShowDialog();

				if (OFD_Result==System.Windows.Forms.DialogResult.OK)
				{
					 try
					 {
						  //;
						  SettingFleeing newSettings=SettingFleeing.DeserializeFromXML(OFD.FileName);
						  Bot.SettingsFunky.Fleeing=newSettings;
						
						  Funky.funkyConfigWindow.Close();
					 } catch
					 {

					 }
				}
		  }
		  private void GroupingLoadXMLClicked(object sender, EventArgs e)
		  {
				System.Windows.Forms.OpenFileDialog OFD=new System.Windows.Forms.OpenFileDialog
				{
					 InitialDirectory=Path.Combine(Funky.FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
					 RestoreDirectory=false,
					 Filter="xml files (*.xml)|*.xml|All files (*.*)|*.*",
					 Title="Grouping Template",
				};
				System.Windows.Forms.DialogResult OFD_Result=OFD.ShowDialog();

				if (OFD_Result==System.Windows.Forms.DialogResult.OK)
				{
					 try
					 {
						  //;
						  SettingGrouping newSettings=SettingGrouping.DeserializeFromXML(OFD.FileName);
						  Bot.SettingsFunky.Grouping=newSettings;
						
						  Funky.funkyConfigWindow.Close();
					 } catch
					 {

					 }
				}

			
		  }
		  #endregion

		  private TextBox TBClusterDistance, TBClusterMinUnitCount,
								TBPotionHealth, TBGlobeHealth;

		  private StackPanel spFleeingOptions, SPFleeing, spGroupingOptions;

		  private CheckBox CBAttemptFleeingBehavior;
		  private Slider sliderFleeMonsterDistance, sliderFleeHealthPercent;
		  private TextBox TBFleemonsterDistance, TBFleeMinimumHealth;
		  private void UpdateFleeingValues()
		  {
				CBAttemptFleeingBehavior.IsChecked=!Bot.SettingsFunky.Fleeing.EnableFleeingBehavior;

				//bool enabled=Bot.SettingsFunky.Fleeing.EnableFleeingBehavior;
				//spFleeingOptions.IsEnabled=enabled;

				sliderFleeMonsterDistance.Value=Bot.SettingsFunky.Fleeing.FleeMaxMonsterDistance;
				sliderFleeHealthPercent.Value=Bot.SettingsFunky.Fleeing.FleeBotMinimumHealthPercent;
				TBFleemonsterDistance.Text=Bot.SettingsFunky.Fleeing.FleeMaxMonsterDistance.ToString();
				TBFleeMinimumHealth.Text=Bot.SettingsFunky.Fleeing.FleeBotMinimumHealthPercent.ToString();

				spFleeingOptions.InvalidateVisual();

		  }


		  private CheckBox CBGroupingBehavior;
		  private Slider sliderGroupingMinimumUnitDistance, sliderGroupingMaximumDistance, sliderGroupingMinimumUnits, sliderGroupingMinimumCluster;
		  private TextBox TBGroupingMinUnitDistance, TBGroupingMaxDistance, TBGroupingMinimumClusterCount, TBGroupingMinimumUnitsInCluster;
		  private void UpdateGroupingValues()
		  {
				CBGroupingBehavior.IsChecked=Bot.SettingsFunky.Grouping.AttemptGroupingMovements;

				bool enabled=Bot.SettingsFunky.Grouping.AttemptGroupingMovements;
				spGroupingOptions.IsEnabled=enabled;
				spGroupingOptions.UpdateLayout();

				sliderGroupingMaximumDistance.Value=Bot.SettingsFunky.Grouping.GroupingMaximumDistanceAllowed;
				sliderGroupingMinimumUnitDistance.Value=Bot.SettingsFunky.Grouping.GroupingMinimumUnitDistance;
				sliderGroupingMinimumUnits.Value=Bot.SettingsFunky.Grouping.GroupingMinimumUnitsInCluster;
				sliderGroupingMinimumCluster.Value=Bot.SettingsFunky.Grouping.GroupingMinimumClusterCount;

				TBGroupingMinUnitDistance.Text=Bot.SettingsFunky.Grouping.GroupingMinimumUnitDistance.ToString();
				TBGroupingMaxDistance.Text=Bot.SettingsFunky.Grouping.GroupingMaximumDistanceAllowed.ToString();
				TBGroupingMinimumClusterCount.Text=Bot.SettingsFunky.Grouping.GroupingMinimumClusterCount.ToString();
				TBGroupingMinimumUnitsInCluster.Text=Bot.SettingsFunky.Grouping.GroupingMinimumUnitsInCluster.ToString();

				
		  }


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





			  AvoidanceOptionsStackPanel.Children.Add(Avoidance_Text_Header);
			  AvoidanceOptionsStackPanel.Children.Add(AvoidanceCheckBoxesPanel);
			  CombatGeneralContentListBox.Items.Add(AvoidanceOptionsStackPanel);

			  #endregion

			  #region Fleeing
			  Button BtnFleeingLoadTemplate=new Button
			  {
					Content="Load Setup",
					Background= System.Windows.Media.Brushes.OrangeRed,
					Foreground= System.Windows.Media.Brushes.GhostWhite,
					FontStyle= FontStyles.Italic,
					FontSize=12,

					HorizontalAlignment= System.Windows.HorizontalAlignment.Left,
					VerticalAlignment= System.Windows.VerticalAlignment.Top,
					Width=75,
					Height=30,

					Margin=new Thickness(Margin.Left, Margin.Top+5, Margin.Right, Margin.Bottom+5),
			  };
			  BtnFleeingLoadTemplate.Click+=FleeingLoadXMLClicked;

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

			  CBAttemptFleeingBehavior=new CheckBox
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

			  sliderFleeMonsterDistance=new Slider
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


			  sliderFleeHealthPercent=new Slider
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
			  SPFleeing.Children.Add(BtnFleeingLoadTemplate);

			  CombatGeneralContentListBox.Items.Add(SPFleeing);
			  #endregion

			  #region Grouping

			  Button BtnGroupingLoadTemplate=new Button
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
			  BtnGroupingLoadTemplate.Click+=GroupingLoadXMLClicked;

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
			  CBGroupingBehavior=new CheckBox
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

			  sliderGroupingMinimumUnitDistance=new Slider
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

			  sliderGroupingMaximumDistance=new Slider
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

			  sliderGroupingMinimumCluster=new Slider
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

			  sliderGroupingMinimumUnits=new Slider
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
			  GroupingOptionsStackPanel.Children.Add(BtnGroupingLoadTemplate);

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
