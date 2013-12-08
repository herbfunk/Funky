using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using FunkyBot.Settings;
using Zeta.Common;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListBox = System.Windows.Controls.ListBox;
using Orientation = System.Windows.Controls.Orientation;
using RadioButton = System.Windows.Controls.RadioButton;
using TextBox = System.Windows.Controls.TextBox;

namespace FunkyBot
{
	 internal partial class FunkyWindow : Window
	 {
		  private void ItemRulesLoadXMLClicked(object sender, EventArgs e)
		  {
				OpenFileDialog OFD=new OpenFileDialog
				{
					 InitialDirectory=Path.Combine(FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
					 RestoreDirectory=false,
					 Filter="xml files (*.xml)|*.xml|All files (*.*)|*.*",
					 Title="ItemRules Template",
				};
				DialogResult OFD_Result=OFD.ShowDialog();

				if (OFD_Result==System.Windows.Forms.DialogResult.OK)
				{
					 try
					 {
						  //;
						  SettingItemRules newSettings=SettingItemRules.DeserializeFromXML(OFD.FileName);
						  Bot.Settings.ItemRules=newSettings;

						  funkyConfigWindow.Close();
					 } catch
					 {

					 }
				}
		  }
		  class ItemRuleTypes : ObservableCollection<string>
		  {
				public ItemRuleTypes()
				{

					 Add("Custom");
					 Add("Soft");
					 Add("Hard");
				}
		  }
		  class ItemRuleQuality : ObservableCollection<string>
		  {
				public ItemRuleQuality()
				{
					 Add("Common");
					 Add("Normal");
					 Add("Magic");
					 Add("Rare");
					 Add("Legendary");
				}
		  }
		  #region EventHandling
		  private void ItemRulesTypeChanged(object sender, EventArgs e)
		  {
				Bot.Settings.ItemRules.ItemRuleType=ItemRuleType.Items[ItemRuleType.SelectedIndex].ToString();
		  }
		  private void ItemRulesBrowse_Click(object sender, EventArgs e)
		  {
				FolderBrowserDialog OFD=new FolderBrowserDialog
				{

				};
				DialogResult OFD_Result=OFD.ShowDialog();

				if (OFD_Result==System.Windows.Forms.DialogResult.OK)
				{
					 try
					 {
						  Bot.Settings.ItemRules.ItemRuleCustomPath=OFD.SelectedPath;
						  tbCustomItemRulePath.Text=Bot.Settings.ItemRules.ItemRuleCustomPath;
					 } catch
					 {

					 }
				}
		  }
		  private void ItemRulesScoringChanged(object sender, EventArgs e)
		  {
				Bot.Settings.ItemRules.ItemRuleGilesScoring=ItemRuleGilesScoring.IsChecked.Value;
		  }
		  private void ItemRulesLogPickupChanged(object sender, EventArgs e)
		  {
				Bot.Settings.ItemRules.ItemRuleLogPickup=ItemRuleLogPickup.Items[ItemRuleLogPickup.SelectedIndex].ToString();
		  }
		  private void ItemRulesLogKeepChanged(object sender, EventArgs e)
		  {
				Bot.Settings.ItemRules.ItemRuleLogKeep=ItemRuleLogKeep.Items[ItemRuleLogKeep.SelectedIndex].ToString();
		  }

		  private void ItemRulesChecked(object sender, EventArgs e)
		  {
				Bot.Settings.ItemRules.UseItemRules=!Bot.Settings.ItemRules.UseItemRules;
				ItemRuleGilesScoring.IsEnabled=!Bot.Settings.ItemRules.UseItemRules;
				ItemRuleDBScoring.IsEnabled=!Bot.Settings.ItemRules.UseItemRules;
		  }
		  private void ItemRulesPickupChecked(object sender, EventArgs e)
		  {
				Bot.Settings.ItemRules.UseItemRulesPickup=!Bot.Settings.ItemRules.UseItemRulesPickup;
		  }
		  private void ItemRulesItemIDsChecked(object sender, EventArgs e)
		  {
				Bot.Settings.ItemRules.ItemRuleUseItemIDs=!Bot.Settings.ItemRules.ItemRuleUseItemIDs;
		  }
		  private void ItemRulesDebugChecked(object sender, EventArgs e)
		  {
				Bot.Settings.ItemRules.ItemRuleDebug=!Bot.Settings.ItemRules.ItemRuleDebug;
		  }
		  private void ItemRulesSalvagingChecked(object sender, EventArgs e)
		  {
				Bot.Settings.ItemRules.ItemRulesSalvaging=!Bot.Settings.ItemRules.ItemRulesSalvaging;
		  }
		  private void ItemRulesUnidStashingChecked(object sender, EventArgs e)
		  {
				Bot.Settings.ItemRules.ItemRulesUnidStashing=!Bot.Settings.ItemRules.ItemRulesUnidStashing;
		  }
		  //UseLevelingLogic
		  private void ItemRulesOpenFolder_Click(object sender, EventArgs e)
		  {
				Process.Start(Path.Combine(FolderPaths.sTrinityPluginPath, "ItemRules", "Rules"));
		  }
		  private void ItemRulesReload_Click(object sender, EventArgs e)
		  {
				if (Bot.ItemRulesEval==null)
				{
					 Logging.Write("Cannot reload rules until bot has started", true);
					 return;
				}

				try
				{
					 Bot.ItemRulesEval.reloadFromUI();
				} catch (Exception ex)
				{
					 Logging.Write(ex.Message+"\r\n"+ex.StackTrace);
				}

		  }
		  #endregion

		  private CheckBox ItemRules;
		  private CheckBox ItemRulesPickup;
		  private Button ItemRulesReload;
		  private CheckBox ItemRuleUseItemIDs;
		  private CheckBox ItemRuleDebug;
		  private ComboBox ItemRuleLogKeep;
		  private ComboBox ItemRuleLogPickup;
		  private ComboBox ItemRuleType;
		  private RadioButton ItemRuleGilesScoring, ItemRuleDBScoring;
		  private TextBox tbCustomItemRulePath;

		  internal void InitItemRulesControls()
		  {

				#region ItemRules
				TabItem ItemRulesTabItem=new TabItem();
				ItemRulesTabItem.Header="Item Rules";
				tcItems.Items.Add(ItemRulesTabItem);
				ListBox lbItemRulesContent=new ListBox();

				StackPanel spItemRules=new StackPanel
				{
					 Background=Brushes.DimGray,
				};
				#region ItemRules Checkbox
				ItemRules=new CheckBox
				{
					 Content="Enable Item Rules",
					 Height=30,
					 IsChecked=(Bot.Settings.ItemRules.UseItemRules),
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
					 Background=Brushes.DarkSlateGray,
					 Foreground=Brushes.GhostWhite,
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
					 IsChecked=(Bot.Settings.ItemRules.UseItemRulesPickup),
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
					 IsChecked=(Bot.Settings.ItemRules.ItemRulesSalvaging),
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
					 IsChecked=(Bot.Settings.ItemRules.ItemRulesUnidStashing),
				};
				CBItemRulesUnidStashing.Checked+=ItemRulesUnidStashingChecked;
				CBItemRulesUnidStashing.Unchecked+=ItemRulesUnidStashingChecked;
				spItemRulesOptions.Children.Add(CBItemRulesUnidStashing);
				#endregion
				spItemRules.Children.Add(spItemRulesOptions);
				#region ItemRules Rule Set
				TextBlock txt_ItemRulesRule=new TextBlock
				{
					 Text="Rule Set",
					 FontSize=12,
					 Background=Brushes.DarkSlateGray,
					 Foreground=Brushes.GhostWhite,
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
					 //Text=Bot.SettingsFunky.ItemRules.ItemRuleType.ToString(),
				};
				ItemRuleType.SelectedIndex=Bot.Settings.ItemRules.ItemRuleType.ToLower().Contains("soft")?1:Bot.Settings.ItemRules.ItemRuleType.ToLower().Contains("hard")?2:0;
				ItemRuleType.SelectionChanged+=ItemRulesTypeChanged;
				spItemRules_RuleSet.Children.Add(ItemRuleType);

				tbCustomItemRulePath=new TextBox
				{
					 Height=30,
					 Width=300,
					 Text=Bot.Settings.ItemRules.ItemRuleCustomPath,
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
					 Foreground=Brushes.GhostWhite,
					 Background=Brushes.DarkSlateGray,
					 Margin=new Thickness(Margin.Left, Margin.Top+10, Margin.Right, Margin.Bottom+5),
				};
				spItemRules.Children.Add(txt_Header_ItemRulesLogging);

				StackPanel spItemRulesLogging=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				#region Logging.Write Items Stashed
				StackPanel spItemRulesLoggingKeep=new StackPanel();
				TextBlock txt_LogItemKeep=new TextBlock
				{
					 Text="Items Stashed",
					 FontSize=11,
					 Foreground=Brushes.GhostWhite,
				};
				spItemRulesLoggingKeep.Children.Add(txt_LogItemKeep);
				ItemRuleLogKeep=new ComboBox
				{
					 Height=30,
					 Width=150,
					 ItemsSource=new ItemRuleQuality(),
					 Text=Bot.Settings.ItemRules.ItemRuleLogKeep
				};
				ItemRuleLogKeep.SelectionChanged+=ItemRulesLogKeepChanged;
				spItemRulesLoggingKeep.Children.Add(ItemRuleLogKeep);
				spItemRulesLogging.Children.Add(spItemRulesLoggingKeep);
				#endregion

				#region Logging.Write Items Pickup
				StackPanel spItemRulesLoggingPickup=new StackPanel();
				TextBlock txt_LogItemPickup=new TextBlock
				{
					 Text="Items Pickup",
					 FontSize=11,
					 Foreground=Brushes.GhostWhite,
				};
				spItemRulesLoggingPickup.Children.Add(txt_LogItemPickup);
				ItemRuleLogPickup=new ComboBox
				{
					 Height=30,
					 Width=150,
					 ItemsSource=new ItemRuleQuality(),
					 Text=Bot.Settings.ItemRules.ItemRuleLogPickup
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
					 Background=Brushes.DarkSlateGray,
					 Foreground=Brushes.GhostWhite,
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
					 IsChecked=(Bot.Settings.ItemRules.ItemRuleUseItemIDs),
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
					 IsChecked=(Bot.Settings.ItemRules.ItemRuleDebug),
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
					 Foreground=Brushes.GhostWhite,
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
					 IsChecked=Bot.Settings.ItemRules.ItemRuleGilesScoring,
					 IsEnabled=!Bot.Settings.ItemRules.UseItemRules,
				};
				ItemRuleDBScoring=new RadioButton
				{
					 GroupName="Scoring",
					 Content="DB Weight Scoring",
					 Width=300,
					 Height=30,
					 IsChecked=!Bot.Settings.ItemRules.ItemRuleGilesScoring,
					 IsEnabled=!Bot.Settings.ItemRules.UseItemRules,
				};
				ItemRuleGilesScoring.Checked+=ItemRulesScoringChanged;
				ItemRuleDBScoring.Checked+=ItemRulesScoringChanged;
				spDefaultItemScoring.Children.Add(ItemRuleGilesScoring);
				spDefaultItemScoring.Children.Add(ItemRuleDBScoring);
				lbItemRulesContent.Items.Add(spDefaultItemScoring);
				#endregion
				Button BtnItemRulesLoadTemplate=new Button
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
				BtnItemRulesLoadTemplate.Click+=ItemRulesLoadXMLClicked;
				lbItemRulesContent.Items.Add(BtnItemRulesLoadTemplate);

				ItemRulesTabItem.Content=lbItemRulesContent;
				#endregion

		  }
	 }
}
