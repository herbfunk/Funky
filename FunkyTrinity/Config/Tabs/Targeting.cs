using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FunkyTrinity.Cache.Enums;
using FunkyTrinity.Settings;

namespace FunkyTrinity
{
	 internal partial class FunkyWindow : Window
	 {
		  #region EventHandling
		  private void UnitExceptionKillLowHPChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Targeting.UnitExceptionLowHP=!Bot.SettingsFunky.Targeting.UnitExceptionLowHP;
		  }
		  private void UnitExceptionAllowRangedUnitsChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Targeting.UnitExceptionRangedUnits=!Bot.SettingsFunky.Targeting.UnitExceptionRangedUnits;
		  }
		  private void UnitExceptionAllowSpawnerUnitsChecked(object sender, EventArgs e)
		  {

				Bot.SettingsFunky.Targeting.UnitExceptionSpawnerUnits=!Bot.SettingsFunky.Targeting.UnitExceptionSpawnerUnits;
		  }
		  private void UnitExceptionAllowSucideBombersChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Targeting.UnitExceptionSucideBombers=!Bot.SettingsFunky.Targeting.UnitExceptionSucideBombers;
		  }

		  private void TargetingLoadXMLClicked(object sender, EventArgs e)
		  {
				System.Windows.Forms.OpenFileDialog OFD=new System.Windows.Forms.OpenFileDialog
				{
					 InitialDirectory=Path.Combine(Funky.FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
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
						  Bot.SettingsFunky.Targeting=newSettings;

						  Funky.funkyConfigWindow.Close();
					 } catch
					 {

					 }
				}


		  }

		  private void IgnoreCorpsesChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Targeting.IgnoreCorpses=!Bot.SettingsFunky.Targeting.IgnoreCorpses;
		  }
		  private void IgnoreEliteMonstersChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Targeting.IgnoreAboveAverageMobs=!Bot.SettingsFunky.Targeting.IgnoreAboveAverageMobs;
		  }
		  private void MissileDampeningChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Targeting.MissleDampeningEnforceCloseRange=!Bot.SettingsFunky.Targeting.MissleDampeningEnforceCloseRange;
		  }
		  private void UseShrineChecked(object sender, EventArgs e)
		  {
				CheckBox cbSender=(CheckBox)sender;
				int index=(int)Enum.Parse(typeof(ShrineTypes), cbSender.Name);
				Bot.SettingsFunky.Targeting.UseShrineTypes[index]=!(Bot.SettingsFunky.Targeting.UseShrineTypes[index]);
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
				Bot.SettingsFunky.Targeting.GoblinPriority=senderCB.SelectedIndex;
		  }
		  private void ExtendRangeRepChestChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Targeting.UseExtendedRangeRepChest=!Bot.SettingsFunky.Targeting.UseExtendedRangeRepChest;
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
					 IsChecked=(Bot.SettingsFunky.Targeting.IgnoreAboveAverageMobs),
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
					 IsChecked=(Bot.SettingsFunky.Targeting.IgnoreCorpses),
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
					 IsChecked=(Bot.SettingsFunky.Targeting.UseExtendedRangeRepChest),
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
					 SelectedIndex=Bot.SettingsFunky.Targeting.GoblinPriority,
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

				#region KillLOWHPUnits
				CheckBox cbClusterKillLowHPUnits=new CheckBox
				{
					 Content="Allow Units with 25% or less HP",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.Targeting.UnitExceptionLowHP),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				cbClusterKillLowHPUnits.Checked+=UnitExceptionKillLowHPChecked;
				cbClusterKillLowHPUnits.Unchecked+=UnitExceptionKillLowHPChecked;
				spClusteringExceptions.Children.Add(cbClusterKillLowHPUnits);
				#endregion

				#region AllowRangedUnits
				CheckBox cbClusteringAllowRangedUnits=new CheckBox
				{
					 Content="Allow Ranged Units",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.Targeting.UnitExceptionRangedUnits),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				cbClusteringAllowRangedUnits.Checked+=UnitExceptionAllowRangedUnitsChecked;
				cbClusteringAllowRangedUnits.Unchecked+=UnitExceptionAllowRangedUnitsChecked;
				spClusteringExceptions.Children.Add(cbClusteringAllowRangedUnits);
				#endregion

				#region AllowSpawnerUnits
				CheckBox cbClusteringAllowSpawnerUnits=new CheckBox
				{
					 Content="Allow Spawner Units",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.Targeting.UnitExceptionSpawnerUnits),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				cbClusteringAllowSpawnerUnits.Checked+=UnitExceptionAllowSpawnerUnitsChecked;
				cbClusteringAllowSpawnerUnits.Unchecked+=UnitExceptionAllowSpawnerUnitsChecked;
				spClusteringExceptions.Children.Add(cbClusteringAllowSpawnerUnits);
				#endregion

				#region AllowSucideBombers
				CheckBox cbClusteringAllowSucideBombers=new CheckBox
				{
					 Content="Allow Sucide Bombers",
					 Width=300,
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.Targeting.UnitExceptionSucideBombers),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				cbClusteringAllowSucideBombers.Checked+=UnitExceptionAllowSucideBombersChecked;
				cbClusteringAllowSucideBombers.Unchecked+=UnitExceptionAllowSucideBombersChecked;
				spClusteringExceptions.Children.Add(cbClusteringAllowSucideBombers);
				#endregion

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
