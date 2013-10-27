using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FunkyBot.Cache.Enums;
using FunkyBot.Settings;

namespace FunkyBot
{
	 internal partial class FunkyWindow : Window
	 {
		  TextBox TBPrioritizeCloseRangeMinimumUnits,TBLowHPMaxDistance;

		  #region EventHandling
		  private void PrioritizeCloseUnitsMinimumSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.Settings.Targeting.PrioritizeCloseRangeMinimumUnits=Value;
				TBPrioritizeCloseRangeMinimumUnits.Text=Value.ToString();
		  }
		  private void UnitExceptionKillLowHPChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Targeting.UnitExceptionLowHP=!Bot.Settings.Targeting.UnitExceptionLowHP;
		  }
          private void UnitExceptionKillLowHPMaxDistanceSliderChanged(object sender, EventArgs e)
          {
              Slider slider_sender = (Slider)sender;
              int Value = (int)slider_sender.Value;
              Bot.Settings.Targeting.UnitExceptionLowHPMaximumDistance = Value;
              TBLowHPMaxDistance.Text = Value.ToString();
          }
		  private void UnitExceptionAllowRangedUnitsChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Targeting.UnitExceptionRangedUnits=!Bot.Settings.Targeting.UnitExceptionRangedUnits;
		  }
		  private void UnitExceptionAllowSpawnerUnitsChecked(object sender, EventArgs e)
		  {

				Bot.Settings.Targeting.UnitExceptionSpawnerUnits=!Bot.Settings.Targeting.UnitExceptionSpawnerUnits;
		  }
		  private void UnitExceptionAllowSucideBombersChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Targeting.UnitExceptionSucideBombers=!Bot.Settings.Targeting.UnitExceptionSucideBombers;
		  }

		  private void TargetingLoadXMLClicked(object sender, EventArgs e)
		  {
				System.Windows.Forms.OpenFileDialog OFD=new System.Windows.Forms.OpenFileDialog
				{
					 InitialDirectory=Path.Combine(FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
					 RestoreDirectory=false,
					 Filter="xml files (*.xml)|*.xml|All files (*.*)|*.*",
					 Title="Targeting Template",
				};
				System.Windows.Forms.DialogResult OFD_Result=OFD.ShowDialog();

				if (OFD_Result==System.Windows.Forms.DialogResult.OK)
				{
					 try
					 {
						  //;
						  SettingTargeting newSettings=SettingTargeting.DeserializeFromXML(OFD.FileName);
						  Bot.Settings.Targeting=newSettings;

						  FunkyWindow.funkyConfigWindow.Close();
					 } catch
					 {

					 }
				}


		  }

		  private void IgnoreCorpsesChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Targeting.IgnoreCorpses=!Bot.Settings.Targeting.IgnoreCorpses;
		  }
		  private void IgnoreEliteMonstersChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Targeting.IgnoreAboveAverageMobs=!Bot.Settings.Targeting.IgnoreAboveAverageMobs;
		  }
		  private void MissileDampeningChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Targeting.MissleDampeningEnforceCloseRange=!Bot.Settings.Targeting.MissleDampeningEnforceCloseRange;
		  }
		  private void PrioritizeCloseRangeUnitsChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Targeting.PrioritizeCloseRangeUnits=!Bot.Settings.Targeting.PrioritizeCloseRangeUnits;
		  }
		  private void UseShrineChecked(object sender, EventArgs e)
		  {
				CheckBox cbSender=(CheckBox)sender;
				int index=(int)Enum.Parse(typeof(ShrineTypes), cbSender.Name);
				Bot.Settings.Targeting.UseShrineTypes[index]=!(Bot.Settings.Targeting.UseShrineTypes[index]);
		  }
		  class GoblinPriority : ObservableCollection<string>
		  {
				public GoblinPriority()
				{

					 Add("None");
					 Add("Normal");
					 Add("Important");
					 Add("Ridiculousness");
				}
		  }
		  private void GoblinPriorityChanged(object sender, EventArgs e)
		  {
				ComboBox senderCB=(ComboBox)sender;
				Bot.Settings.Targeting.GoblinPriority=senderCB.SelectedIndex;
		  }
		  private void ExtendRangeRepChestChecked(object sender, EventArgs e)
		  {
				Bot.Settings.Targeting.UseExtendedRangeRepChest=!Bot.Settings.Targeting.UseExtendedRangeRepChest;
		  }
		  #endregion


		  internal void InitTargetingGeneralControls()
		  {
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
					 IsChecked=(Bot.Settings.Targeting.IgnoreAboveAverageMobs),
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
					 IsChecked=(Bot.Settings.Targeting.IgnoreCorpses),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				cbIgnoreCorpses.Checked+=IgnoreCorpsesChecked;
				cbIgnoreCorpses.Unchecked+=IgnoreCorpsesChecked;
				#endregion

				#region ExtendedRepChestRange
				ToolTip TTExtendedRareChestRange=new System.Windows.Controls.ToolTip
				{
					 Content="This will use double the Container Range Setting for all rare chests.",
				};
				CheckBox UseExtendedRangeRepChestCB=new CheckBox
				{
					 Content="Increased range for rare chests",
					 Width=300,
					 Height=20,
					 IsChecked=(Bot.Settings.Targeting.UseExtendedRangeRepChest),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 ToolTip=TTExtendedRareChestRange,
				};
				UseExtendedRangeRepChestCB.Checked+=ExtendRangeRepChestChecked;
				UseExtendedRangeRepChestCB.Unchecked+=ExtendRangeRepChestChecked;
				#endregion

				#region GoblinPriority
				ToolTip TTGoblinPriority=new System.Windows.Controls.ToolTip
				{
					 Content="Note: Priority above normal will consider goblins as special objects",
				};
				StackPanel GoblinPriority_StackPanel=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
					 ToolTip=TTGoblinPriority,
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
					 SelectedIndex=Bot.Settings.Targeting.GoblinPriority,
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



				ToolTip TTPrioritizeCloseRange=new System.Windows.Controls.ToolTip
				{
					 Content="When active it will weight units based on the distance from Bot. Closer to the bot, the higher the weight given.",
				};
				StackPanel Targeting_PrioritizeCloseRange_Options_Stackpanel=new StackPanel
				{
					 Orientation=Orientation.Vertical,
					 Focusable=false,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
					 Background=System.Windows.Media.Brushes.DimGray,
					 ToolTip=TTPrioritizeCloseRange,
				};
				TextBlock Target_PrioritizeCloseRange_Text=new TextBlock
				{
					 Text="Prioritize Range",
					 FontSize=13,
					 Background=System.Windows.Media.Brushes.OrangeRed,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 TextAlignment=TextAlignment.Center,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
				};

				#region PrioritizeCloseRange
				CheckBox cbPrioritizeCloseRange=new CheckBox
				{
					 Content="Enable Priority of Close Units",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.Settings.Targeting.PrioritizeCloseRangeUnits),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				cbPrioritizeCloseRange.Checked+=PrioritizeCloseRangeUnitsChecked;
				cbPrioritizeCloseRange.Unchecked+=PrioritizeCloseRangeUnitsChecked;
				#endregion

				#region PrioritizeCloseRangeMinimumUnits
				TextBlock PrioritizeCloseRangeMinimumUnits_Label=new TextBlock
				{
					 Text="Minimum Nearby Units",
					 FontSize=13,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 //Background = System.Windows.Media.Brushes.Crimson,
					 TextAlignment=TextAlignment.Left,
				};

				Slider sliderPrioritizeCloseRangeMinimumUnits=new Slider
				{
					 Width=100,
					 Maximum=20,
					 Minimum=0,
					 TickFrequency=5,
					 LargeChange=5,
					 SmallChange=1,
					 Value=Bot.Settings.Targeting.PrioritizeCloseRangeMinimumUnits,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				sliderPrioritizeCloseRangeMinimumUnits.ValueChanged+=PrioritizeCloseUnitsMinimumSliderChanged;
				TBPrioritizeCloseRangeMinimumUnits=new TextBox
				{
					 Text=Bot.Settings.Targeting.PrioritizeCloseRangeMinimumUnits.ToString(),
					 IsReadOnly=true,
				};
				StackPanel PrioritizeMinimumUnitsStackPanel=new StackPanel
				{
					 Height=20,
					 Orientation=Orientation.Horizontal,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+6),
				};

				PrioritizeMinimumUnitsStackPanel.Children.Add(sliderPrioritizeCloseRangeMinimumUnits);
				PrioritizeMinimumUnitsStackPanel.Children.Add(TBPrioritizeCloseRangeMinimumUnits);
				#endregion

				Targeting_PrioritizeCloseRange_Options_Stackpanel.Children.Add(Target_PrioritizeCloseRange_Text);
				Targeting_PrioritizeCloseRange_Options_Stackpanel.Children.Add(cbPrioritizeCloseRange);
				Targeting_PrioritizeCloseRange_Options_Stackpanel.Children.Add(PrioritizeCloseRangeMinimumUnits_Label);
				Targeting_PrioritizeCloseRange_Options_Stackpanel.Children.Add(PrioritizeMinimumUnitsStackPanel);

				Target_General_ContentListBox.Items.Add(Targeting_PrioritizeCloseRange_Options_Stackpanel);





				StackPanel spClusteringExceptions=new StackPanel
				{
					 Background=System.Windows.Media.Brushes.DimGray,
				};
				ToolTip TTClusteringExceptions=new System.Windows.Controls.ToolTip
				{
					 Content="Exceptions are used in clustering and also used to determine if object is special",
				};
				TextBlock ClusteringExceptions_Text_Header=new TextBlock
				{
					 Text="Unit Exceptions",
					 FontSize=12,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Background=System.Windows.Media.Brushes.OrangeRed,
					 TextAlignment=TextAlignment.Center,
					 ToolTip=TTClusteringExceptions,
				};
				spClusteringExceptions.Children.Add(ClusteringExceptions_Text_Header);

				#region AllowRangedUnits
                ToolTip TTAllowRangedUnits = new System.Windows.Controls.ToolTip
                {
                    Content = "Uses the MonsterSize property from DB to determine if Ranged",
                };
				CheckBox cbClusteringAllowRangedUnits=new CheckBox
				{
					 Content="Allow Ranged Units",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.Settings.Targeting.UnitExceptionRangedUnits),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
                     ToolTip = TTAllowRangedUnits,
				};
				cbClusteringAllowRangedUnits.Checked+=UnitExceptionAllowRangedUnitsChecked;
				cbClusteringAllowRangedUnits.Unchecked+=UnitExceptionAllowRangedUnitsChecked;
				spClusteringExceptions.Children.Add(cbClusteringAllowRangedUnits);
				#endregion

				#region AllowSpawnerUnits
                ToolTip TTAllowSpawnerUnits = new System.Windows.Controls.ToolTip
                {
                    Content = "Spawner is an Unit that summons additional units",
                };
				CheckBox cbClusteringAllowSpawnerUnits=new CheckBox
				{
					 Content="Allow Spawner Units",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.Settings.Targeting.UnitExceptionSpawnerUnits),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
                     ToolTip = TTAllowSpawnerUnits,
				};
				cbClusteringAllowSpawnerUnits.Checked+=UnitExceptionAllowSpawnerUnitsChecked;
				cbClusteringAllowSpawnerUnits.Unchecked+=UnitExceptionAllowSpawnerUnitsChecked;
				spClusteringExceptions.Children.Add(cbClusteringAllowSpawnerUnits);
				#endregion

				#region AllowSucideBombers
                ToolTip TTAllowSucideBombers = new System.Windows.Controls.ToolTip
                {
                    Content = "Units found in Act 2 and 3 that explode and are fast!",
                };
				CheckBox cbClusteringAllowSucideBombers=new CheckBox
				{
					 Content="Allow Sucide Bombers",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.Settings.Targeting.UnitExceptionSucideBombers),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
                     ToolTip = TTAllowSucideBombers,
				};
				cbClusteringAllowSucideBombers.Checked+=UnitExceptionAllowSucideBombersChecked;
				cbClusteringAllowSucideBombers.Unchecked+=UnitExceptionAllowSucideBombersChecked;
				spClusteringExceptions.Children.Add(cbClusteringAllowSucideBombers);
				#endregion


                #region KillLOWHPUnits
                CheckBox cbClusterKillLowHPUnits = new CheckBox
                {
                    Content = "Allow Units with 25% or less HP",
                    //Width = 300,
                    Height = 30,
                    IsChecked = (Bot.Settings.Targeting.UnitExceptionLowHP),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 12, Margin.Bottom),
                };
                cbClusterKillLowHPUnits.Checked += UnitExceptionKillLowHPChecked;
                cbClusterKillLowHPUnits.Unchecked += UnitExceptionKillLowHPChecked;
                //spClusteringExceptions.Children.Add(cbClusterKillLowHPUnits);
                #endregion

                #region LowHPMaximumDistance
                TextBlock LowHPMaximumDistance_Text = new TextBlock
                {
                    Text = "Low HP Maximum Distance",
                    FontSize = 12,
                    Foreground = System.Windows.Media.Brushes.GhostWhite,
                    TextAlignment = TextAlignment.Left,
                };
                Slider sliderLowHPMaxDistance = new Slider
                {
                    Width = 100,
                    Maximum = 150,
                    Minimum = 0,
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Value = Bot.Settings.Targeting.UnitExceptionLowHPMaximumDistance,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                sliderLowHPMaxDistance.ValueChanged += UnitExceptionKillLowHPMaxDistanceSliderChanged;

                TBLowHPMaxDistance = new TextBox
                {
                    Text = Bot.Settings.Targeting.UnitExceptionLowHPMaximumDistance.ToString(),
                    IsReadOnly = true,
                };
                StackPanel LowHPMaxDistanceStackPanel = new StackPanel
                {
                    //Width = 600,
                    Height = 20,
                    Orientation = Orientation.Horizontal,
                };
                LowHPMaxDistanceStackPanel.Children.Add(sliderLowHPMaxDistance);
                LowHPMaxDistanceStackPanel.Children.Add(TBLowHPMaxDistance);
                StackPanel spLowHpMaxDistance = new StackPanel();
                spLowHpMaxDistance.Children.Add(LowHPMaximumDistance_Text);
                spLowHpMaxDistance.Children.Add(LowHPMaxDistanceStackPanel);
                #endregion

                StackPanel spLowHealthException = new StackPanel
                {
                    Orientation= Orientation.Horizontal,
                };
                spLowHealthException.Children.Add(cbClusterKillLowHPUnits);
                spLowHealthException.Children.Add(spLowHpMaxDistance);
                spClusteringExceptions.Children.Add(spLowHealthException);

				Target_General_ContentListBox.Items.Add(spClusteringExceptions);

				Button BtnTargetTemplate=new Button
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
				BtnTargetTemplate.Click+=TargetingLoadXMLClicked;
				Target_General_ContentListBox.Items.Add(BtnTargetTemplate);


				TargetingMiscTabItem.Content=Target_General_ContentListBox;
		  }
	}
}
