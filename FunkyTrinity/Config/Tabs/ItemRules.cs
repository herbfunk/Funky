using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FunkyTrinity.Settings;

namespace FunkyTrinity
{
	 internal partial class FunkyWindow : Window
	 {
		  private void ItemRulesLoadXMLClicked(object sender, EventArgs e)
		  {
				System.Windows.Forms.OpenFileDialog OFD=new System.Windows.Forms.OpenFileDialog
				{
					 InitialDirectory=Path.Combine(Funky.FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
					 RestoreDirectory=false,
					 Filter="xml files (*.xml)|*.xml|All files (*.*)|*.*",
					 Title="ItemRules Template",
				};
				System.Windows.Forms.DialogResult OFD_Result=OFD.ShowDialog();

				if (OFD_Result==System.Windows.Forms.DialogResult.OK)
				{
					 try
					 {
						  //;
						  SettingItemRules newSettings=SettingItemRules.DeserializeFromXML(OFD.FileName);
						  Bot.SettingsFunky.ItemRules=newSettings;

						  Funky.funkyConfigWindow.Close();
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
				Bot.SettingsFunky.ItemRules.ItemRuleType=ItemRuleType.Items[ItemRuleType.SelectedIndex].ToString();
		  }
		  private void ItemRulesBrowse_Click(object sender, EventArgs e)
		  {
				System.Windows.Forms.FolderBrowserDialog OFD=new System.Windows.Forms.FolderBrowserDialog
				{

				};
				System.Windows.Forms.DialogResult OFD_Result=OFD.ShowDialog();

				if (OFD_Result==System.Windows.Forms.DialogResult.OK)
				{
					 try
					 {
						  Bot.SettingsFunky.ItemRules.ItemRuleCustomPath=OFD.SelectedPath;
						  tbCustomItemRulePath.Text=Bot.SettingsFunky.ItemRules.ItemRuleCustomPath;
					 } catch
					 {

					 }
				}
		  }
		  private void ItemRulesScoringChanged(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.ItemRules.ItemRuleGilesScoring=ItemRuleGilesScoring.IsChecked.Value;
		  }
		  private void ItemRulesLogPickupChanged(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.ItemRules.ItemRuleLogPickup=ItemRuleLogPickup.Items[ItemRuleLogPickup.SelectedIndex].ToString();
		  }
		  private void ItemRulesLogKeepChanged(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.ItemRules.ItemRuleLogKeep=ItemRuleLogKeep.Items[ItemRuleLogKeep.SelectedIndex].ToString();
		  }

		  private void ItemRulesChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.ItemRules.UseItemRules=!Bot.SettingsFunky.ItemRules.UseItemRules;
				ItemRuleGilesScoring.IsEnabled=!Bot.SettingsFunky.ItemRules.UseItemRules;
				ItemRuleDBScoring.IsEnabled=!Bot.SettingsFunky.ItemRules.UseItemRules;
		  }
		  private void ItemRulesPickupChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.ItemRules.UseItemRulesPickup=!Bot.SettingsFunky.ItemRules.UseItemRulesPickup;
		  }
		  private void ItemRulesItemIDsChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.ItemRules.ItemRuleUseItemIDs=!Bot.SettingsFunky.ItemRules.ItemRuleUseItemIDs;
		  }
		  private void ItemRulesDebugChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.ItemRules.ItemRuleDebug=!Bot.SettingsFunky.ItemRules.ItemRuleDebug;
		  }
		  private void ItemLevelingLogicChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.UseLevelingLogic=!Bot.SettingsFunky.UseLevelingLogic;
		  }
		  private void ItemRulesSalvagingChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.ItemRules.ItemRulesSalvaging=!Bot.SettingsFunky.ItemRules.ItemRulesSalvaging;
		  }
		  private void ItemRulesUnidStashingChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.ItemRules.ItemRulesUnidStashing=!Bot.SettingsFunky.ItemRules.ItemRulesUnidStashing;
		  }
		  //UseLevelingLogic
		  private void ItemRulesOpenFolder_Click(object sender, EventArgs e)
		  {
				System.Diagnostics.Process.Start(System.IO.Path.Combine(Funky.FolderPaths.sTrinityPluginPath, "ItemRules", "Rules"));
		  }
		  private void ItemRulesReload_Click(object sender, EventArgs e)
		  {
				if (Funky.ItemRulesEval==null)
				{
					 Funky.Log("Cannot reload rules until bot has started", true);
					 return;
				}

				try
				{
					 Funky.ItemRulesEval.reloadFromUI();
				} catch (Exception ex)
				{
					 Funky.Log(ex.Message+"\r\n"+ex.StackTrace);
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
					 Background=System.Windows.Media.Brushes.DimGray,
				};
				#region ItemRules Checkbox
				ItemRules=new CheckBox
				{
					 Content="Enable Item Rules",
					 Height=30,
					 IsChecked=(Bot.SettingsFunky.ItemRules.UseItemRules),
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
					 IsChecked=(Bot.SettingsFunky.ItemRules.UseItemRulesPickup),
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
					 IsChecked=(Bot.SettingsFunky.ItemRules.ItemRulesSalvaging),
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
					 IsChecked=(Bot.SettingsFunky.ItemRules.ItemRulesUnidStashing),
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
					 ItemsSource=new FunkyWindow.ItemRuleTypes(),
					 //Text=Bot.SettingsFunky.ItemRules.ItemRuleType.ToString(),
				};
				ItemRuleType.SelectedIndex=Bot.SettingsFunky.ItemRules.ItemRuleType.ToLower().Contains("soft")?1:Bot.SettingsFunky.ItemRules.ItemRuleType.ToLower().Contains("hard")?2:0;
				ItemRuleType.SelectionChanged+=ItemRulesTypeChanged;
				spItemRules_RuleSet.Children.Add(ItemRuleType);

				tbCustomItemRulePath=new TextBox
				{
					 Height=30,
					 Width=300,
					 Text=Bot.SettingsFunky.ItemRules.ItemRuleCustomPath,
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
					 ItemsSource=new FunkyWindow.ItemRuleQuality(),
					 Text=Bot.SettingsFunky.ItemRules.ItemRuleLogKeep
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
					 ItemsSource=new FunkyWindow.ItemRuleQuality(),
					 Text=Bot.SettingsFunky.ItemRules.ItemRuleLogPickup
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
					 IsChecked=(Bot.SettingsFunky.ItemRules.ItemRuleUseItemIDs),
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
					 IsChecked=(Bot.SettingsFunky.ItemRules.ItemRuleDebug),
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
					 IsChecked=Bot.SettingsFunky.ItemRules.ItemRuleGilesScoring,
					 IsEnabled=!Bot.SettingsFunky.ItemRules.UseItemRules,
				};
				ItemRuleDBScoring=new RadioButton
				{
					 GroupName="Scoring",
					 Content="DB Weight Scoring",
					 Width=300,
					 Height=30,
					 IsChecked=!Bot.SettingsFunky.ItemRules.ItemRuleGilesScoring,
					 IsEnabled=!Bot.SettingsFunky.ItemRules.UseItemRules,
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
				BtnItemRulesLoadTemplate.Click+=ItemRulesLoadXMLClicked;
				lbItemRulesContent.Items.Add(BtnItemRulesLoadTemplate);

				ItemRulesTabItem.Content=lbItemRulesContent;
				#endregion

		  }
	 }
}
