using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using fItemPlugin.Player;
using fItemPlugin.Townrun;
using Zeta.Bot;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListBox = System.Windows.Controls.ListBox;
using Orientation = System.Windows.Controls.Orientation;
using RadioButton = System.Windows.Controls.RadioButton;
using TextBox = System.Windows.Controls.TextBox;


namespace fItemPlugin
{
	internal partial class FunkyWindow : Window
	{
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
		class SalvageRuleTypes : ObservableCollection<string>
		{
			public SalvageRuleTypes()
			{
				Add("None");
				Add("ROS");
				Add("All");
			}
		}

		internal static FunkyWindow funkyConfigWindow;

		private CheckBox ItemRules, ItemRulesPickup, ItemRuleUseItemIDs, ItemRuleDebug;
		private CheckBox checkBox_ItemManager,checkBox_StashHoradricCache, checkBox_IdentifyLegendaries, checkBox_BuyPotions;
		private ComboBox[] comboBox_TownRunSalvage;

		private Button ItemRulesReload;

		private ComboBox ItemRuleType, ItemRuleLogKeep;
		private RadioButton ItemRuleGilesScoring, ItemRuleDBScoring;
		private TextBox tbCustomItemRulePath, tbMinimumBloodShards, tbPotionCount;
		private ListBox lbItemRulesContent;
		

		private StackPanel sp_BloodShardGambling;

		public FunkyWindow()
		{
			//Settings_Funky.LoadFunkyConfiguration();

			Owner = Demonbuddy.App.Current.MainWindow;
			Title = String.Format("Funky Town Run Settings -- {0} {1}", Character.CurrentAccountName, Character.CurrentHeroName);
			SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
			ResizeMode = System.Windows.ResizeMode.CanMinimize;
			Background = System.Windows.Media.Brushes.Black;
			Foreground = System.Windows.Media.Brushes.PaleGoldenrod;
			
			ListBox LBWindowContent = new ListBox
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Height=650,
				Width=650,
			};

			#region Item Rules Controls

			StackPanel spItemRules = new StackPanel
			{
				Background = Brushes.DimGray,
				Margin=new Thickness(0,0,0,Margin.Bottom+10),
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};

			TextBlock txt_ItemRules_Header = new TextBlock
			{
				Text = "Item Rules",
				FontSize = 15,
				Background = Brushes.RoyalBlue,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 10),
			};
			spItemRules.Children.Add(txt_ItemRules_Header);

			#region ItemRules Checkbox
			ItemRules = new CheckBox
			{
				Content = "Enable Item Rules",
				Height = 30,
				IsChecked = (FunkyTownRunPlugin.PluginSettings.UseItemRules),
				FontSize = 14,
				FontStyle = FontStyles.Oblique,

			};
			ItemRules.Checked += ItemRulesChecked;
			ItemRules.Unchecked += ItemRulesChecked;
			spItemRules.Children.Add(ItemRules);
			#endregion
			#region ItemRules Rule Set
			TextBlock txt_ItemRulesRule = new TextBlock
			{
				Text = "Rule Set",
				FontSize = 12,
				Background = Brushes.DarkSlateGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spItemRules.Children.Add(txt_ItemRulesRule);

			StackPanel spItemRules_RuleSet = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			ItemRuleType = new ComboBox
			{
				Height = 30,
				Width = 150,
				ItemsSource = new ItemRuleTypes(),
				//Text=Bot.SettingsFunky.ItemRules.ItemRuleType.ToString(),
			};
			string ruletype = FunkyTownRunPlugin.PluginSettings.ItemRuleType.ToLower();

			ItemRuleType.SelectedIndex = ruletype.Contains("hard") ? 2 : ruletype.Contains("soft") ? 1 : 0;
			ItemRuleType.SelectionChanged += ItemRulesTypeChanged;
			spItemRules_RuleSet.Children.Add(ItemRuleType);

			tbCustomItemRulePath = new TextBox
			{
				Height = 30,
				Width = 300,
				Text = FunkyTownRunPlugin.PluginSettings.ItemRuleCustomPath,
			};
			spItemRules_RuleSet.Children.Add(tbCustomItemRulePath);

			Button btnCustomItemRulesBrowse = new Button
			{
				Content = "Browse",
			};
			btnCustomItemRulesBrowse.Click += ItemRulesBrowse_Click;
			spItemRules_RuleSet.Children.Add(btnCustomItemRulesBrowse);

			spItemRules.Children.Add(spItemRules_RuleSet);
			#endregion

			#region ItemRulesLogging
			TextBlock txt_Header_ItemRulesLogging = new TextBlock
			{
				Text = "Logging",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				Background = Brushes.DarkSlateGray,
				Margin = new Thickness(Margin.Left, Margin.Top + 10, Margin.Right, Margin.Bottom + 5),
			};
			spItemRules.Children.Add(txt_Header_ItemRulesLogging);

			StackPanel spItemRulesLogging = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			#region Log Items Stashed
			StackPanel spItemRulesLoggingKeep = new StackPanel();
			TextBlock txt_LogItemKeep = new TextBlock
			{
				Text = "Items Stashed",
				FontSize = 11,
				Foreground = Brushes.GhostWhite,
			};
			spItemRulesLoggingKeep.Children.Add(txt_LogItemKeep);
			ItemRuleLogKeep = new ComboBox
			{
				Height = 30,
				Width = 150,
				ItemsSource = new ItemRuleQuality(),
				Text = FunkyTownRunPlugin.PluginSettings.ItemRuleLogKeep
			};
			ItemRuleLogKeep.SelectionChanged += ItemRulesLogKeepChanged;
			spItemRulesLoggingKeep.Children.Add(ItemRuleLogKeep);
			spItemRulesLogging.Children.Add(spItemRulesLoggingKeep);
			#endregion

			spItemRules.Children.Add(spItemRulesLogging);
			#endregion

			TextBlock txt_ItemRulesMisc = new TextBlock
			{
				Text = "Misc",
				FontSize = 12,
				Background = Brushes.DarkSlateGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top + 10, Margin.Right, Margin.Bottom + 5),
			};
			spItemRules.Children.Add(txt_ItemRulesMisc);

			StackPanel spItemRulesMisc = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			#region ItemRulesIDs
			ItemRuleUseItemIDs = new CheckBox
			{
				Content = "Use Item IDs",
				Height = 30,
				IsChecked = (FunkyTownRunPlugin.PluginSettings.ItemRuleUseItemIDs),
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),

			};
			ItemRuleUseItemIDs.Checked += ItemRulesItemIDsChecked;
			ItemRuleUseItemIDs.Unchecked += ItemRulesItemIDsChecked;
			spItemRulesMisc.Children.Add(ItemRuleUseItemIDs);

			#endregion
			#region ItemRulesDebug
			ItemRuleDebug = new CheckBox
			{
				Content = "Debugging",
				Height = 30,
				IsChecked = (FunkyTownRunPlugin.PluginSettings.ItemRuleDebug),
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom),

			};
			ItemRuleDebug.Checked += ItemRulesDebugChecked;
			ItemRuleDebug.Unchecked += ItemRulesDebugChecked;
			spItemRulesMisc.Children.Add(ItemRuleDebug);

			#endregion
			spItemRules.Children.Add(spItemRulesMisc);
			LBWindowContent.Items.Add(spItemRules);

			#endregion

			#region General Options

			StackPanel spGeneralOptions = new StackPanel
			{
				Background = Brushes.DimGray,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};
			TextBlock txt_General_Header = new TextBlock
			{
				Text = "General Options",
				FontSize = 15,
				Background = Brushes.RoyalBlue,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 10),
				TextAlignment=TextAlignment.Center,
			};
			spGeneralOptions.Children.Add(txt_General_Header);

			#region General Town Run Options

			StackPanel spTownRun_GeneralOptions = new StackPanel
			{
				Background = Brushes.DimGray,
				Orientation = Orientation.Vertical,
			};

			#region Item Manager
			checkBox_ItemManager = new CheckBox
			{
				Content = "Use Item Manager",
				Height = 30,
				IsChecked = (FunkyTownRunPlugin.PluginSettings.UseItemManagerEvaluation),
				FontSize = 14,

			};
			checkBox_ItemManager.Checked += ItemManagerChecked;
			checkBox_ItemManager.Unchecked += ItemManagerChecked;
			spTownRun_GeneralOptions.Children.Add(checkBox_ItemManager);
			#endregion

			#region Stash Horadric Caches
			checkBox_StashHoradricCache = new CheckBox
			{
				Content = "Stash Horadric Caches",
				Height = 30,
				IsChecked = (FunkyTownRunPlugin.PluginSettings.StashHoradricCache),
				FontSize = 14,

			};
			checkBox_StashHoradricCache.Checked += StashHoradricCacheChecked;
			checkBox_StashHoradricCache.Unchecked += StashHoradricCacheChecked;
			spTownRun_GeneralOptions.Children.Add(checkBox_StashHoradricCache);

			#endregion

			#region Identify Legendaries
			checkBox_IdentifyLegendaries = new CheckBox
			{
				Content = "Identify Legendaries",
				Height = 30,
				IsChecked = (FunkyTownRunPlugin.PluginSettings.IdentifyLegendaries),
				FontSize = 14,
			};
			checkBox_IdentifyLegendaries.Checked += IdentifyLegendariesChecked;
			checkBox_IdentifyLegendaries.Unchecked += IdentifyLegendariesChecked;
			spTownRun_GeneralOptions.Children.Add(checkBox_IdentifyLegendaries);
			#endregion

			#region Buy Potions
			checkBox_BuyPotions = new CheckBox
			{
				Content = "Buy Potions",
				Height = 30,
				IsChecked = (FunkyTownRunPlugin.PluginSettings.BuyPotionsDuringTownRun),
				FontSize = 14,
				IsEnabled=false,
			};
			checkBox_BuyPotions.Checked += BuyPotionsDuringTownRunChecked;
			checkBox_BuyPotions.Unchecked += BuyPotionsDuringTownRunChecked;
			spTownRun_GeneralOptions.Children.Add(checkBox_BuyPotions);
			#endregion

			#region Potions Count

			StackPanel spPotionsCount = new StackPanel
			{
				Margin = new Thickness(0, 0, 0, 10),
			};

			TextBlock txt_General_PotionCount = new TextBlock
			{
				Text = "Maximum Potions",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};
			spPotionsCount.Children.Add(txt_General_PotionCount);

			Slider sliderPotionsCount = new Slider
			{
				Width = 100,
				Maximum = 100,
				Minimum = 0,
				TickFrequency = 25,
				LargeChange = 10,
				SmallChange = 1,
				Value = FunkyTownRunPlugin.PluginSettings.PotionsCount,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderPotionsCount.ValueChanged += PotionCountSliderChanged;
			tbPotionCount = new TextBox
			{
				Text = FunkyTownRunPlugin.PluginSettings.PotionsCount.ToString(),
				IsReadOnly = true,
			};
			StackPanel spPotionCountControls = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			spPotionCountControls.Children.Add(sliderPotionsCount);
			spPotionCountControls.Children.Add(tbPotionCount);
			spPotionsCount.Children.Add(spPotionCountControls);
			spTownRun_GeneralOptions.Children.Add(spPotionsCount);
			#endregion


			
			#endregion


			spGeneralOptions.Children.Add(spTownRun_GeneralOptions);
			LBWindowContent.Items.Add(spGeneralOptions);
			
			#endregion

			#region Salavge Options

			StackPanel spSalvageOptions = new StackPanel
			{
				Background = Brushes.DimGray,
				Orientation = Orientation.Vertical,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};

			TextBlock txt_Salvage_Header = new TextBlock
			{
				Text = "Salavge Options",
				FontSize = 15,
				Background = Brushes.RoyalBlue,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 10),
			};
			spSalvageOptions.Children.Add(txt_Salvage_Header);

			StackPanel spTownRun_SalvageOptions = new StackPanel
			{
				Background = Brushes.DimGray,
				Orientation = Orientation.Horizontal,
			};
			comboBox_TownRunSalvage = new ComboBox[4];

			#region Salvage White Item

			StackPanel spTownRun_SalvageOptions_WhiteItems = new StackPanel
			{
				Background = Brushes.DimGray,
				Orientation = Orientation.Vertical,
			};
			TextBlock txt_SalvageOptions_WhiteItems = new TextBlock
			{
				Text = "White Items",
				FontSize = 12,
				Background = Brushes.DarkSlateGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top + 10, Margin.Right, Margin.Bottom + 5),
			};
			spTownRun_SalvageOptions_WhiteItems.Children.Add(txt_SalvageOptions_WhiteItems);
			comboBox_TownRunSalvage[0] = new ComboBox
			{
				Name = "White",
				Height = 30,
				Width = 150,
				ItemsSource = new SalvageRuleTypes(),
				//Text=Bot.SettingsFunky.ItemRules.ItemRuleType.ToString(),
			};
			comboBox_TownRunSalvage[0].SelectedIndex = FunkyTownRunPlugin.PluginSettings.SalvageWhiteItemLevel == 0 ? 0 : FunkyTownRunPlugin.PluginSettings.SalvageWhiteItemLevel == 1 ? 1 : 2;
			comboBox_TownRunSalvage[0].SelectionChanged += SalvageRuleTypeChanged;
			spTownRun_SalvageOptions_WhiteItems.Children.Add(comboBox_TownRunSalvage[0]);
			spTownRun_SalvageOptions.Children.Add(spTownRun_SalvageOptions_WhiteItems);
			#endregion
			#region Salavge Magic Items

			StackPanel spTownRun_SalvageOptions_MagicItems = new StackPanel
			{
				Background = Brushes.DimGray,
				Orientation = Orientation.Vertical,
			};
			TextBlock txt_SalvageOptions_MagicItems = new TextBlock
			{
				Text = "Magic Items",
				FontSize = 12,
				Background = Brushes.DarkSlateGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top + 10, Margin.Right, Margin.Bottom + 5),
			};
			spTownRun_SalvageOptions_MagicItems.Children.Add(txt_SalvageOptions_MagicItems);
			comboBox_TownRunSalvage[1] = new ComboBox
			{
				Name = "Magic",
				Height = 30,
				Width = 150,
				ItemsSource = new SalvageRuleTypes(),
				//Text=Bot.SettingsFunky.ItemRules.ItemRuleType.ToString(),
			};
			comboBox_TownRunSalvage[1].SelectedIndex = FunkyTownRunPlugin.PluginSettings.SalvageMagicItemLevel == 0 ? 0 : FunkyTownRunPlugin.PluginSettings.SalvageMagicItemLevel == 1 ? 1 : 2;
			comboBox_TownRunSalvage[1].SelectionChanged += SalvageRuleTypeChanged;
			spTownRun_SalvageOptions_MagicItems.Children.Add(comboBox_TownRunSalvage[1]);
			spTownRun_SalvageOptions.Children.Add(spTownRun_SalvageOptions_MagicItems);

			#endregion
			#region Salavge Rare Items

			StackPanel spTownRun_SalvageOptions_RareItems = new StackPanel
			{
				Background = Brushes.DimGray,
				Orientation = Orientation.Vertical,
			};
			TextBlock txt_SalvageOptions_RareItems = new TextBlock
			{
				Text = "Rare Items",
				FontSize = 12,
				Background = Brushes.DarkSlateGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top + 10, Margin.Right, Margin.Bottom + 5),
			};
			spTownRun_SalvageOptions_RareItems.Children.Add(txt_SalvageOptions_RareItems);
			comboBox_TownRunSalvage[2] = new ComboBox
			{
				Name = "Rare",
				Height = 30,
				Width = 150,
				ItemsSource = new SalvageRuleTypes(),
				//Text=Bot.SettingsFunky.ItemRules.ItemRuleType.ToString(),
			};
			comboBox_TownRunSalvage[2].SelectedIndex = FunkyTownRunPlugin.PluginSettings.SalvageRareItemLevel == 0 ? 0 : FunkyTownRunPlugin.PluginSettings.SalvageRareItemLevel == 1 ? 1 : 2;
			comboBox_TownRunSalvage[2].SelectionChanged += SalvageRuleTypeChanged;
			spTownRun_SalvageOptions_RareItems.Children.Add(comboBox_TownRunSalvage[2]);
			spTownRun_SalvageOptions.Children.Add(spTownRun_SalvageOptions_RareItems);

			#endregion
			#region Salavge Legendary Items

			StackPanel spTownRun_SalvageOptions_LegendaryItems = new StackPanel
			{
				Background = Brushes.DimGray,
				Orientation = Orientation.Vertical,
			};
			TextBlock txt_SalvageOptions_LegendaryItems = new TextBlock
			{
				Text = "Legendary Items",
				FontSize = 12,
				Background = Brushes.DarkSlateGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top + 10, Margin.Right, Margin.Bottom + 5),
			};
			spTownRun_SalvageOptions_LegendaryItems.Children.Add(txt_SalvageOptions_LegendaryItems);
			comboBox_TownRunSalvage[3] = new ComboBox
			{
				Name = "Legendary",
				Height = 30,
				Width = 150,
				ItemsSource = new SalvageRuleTypes(),
				//Text=Bot.SettingsFunky.ItemRules.ItemRuleType.ToString(),
			};
			comboBox_TownRunSalvage[3].SelectedIndex = FunkyTownRunPlugin.PluginSettings.SalvageLegendaryItemLevel == 0 ? 0 : FunkyTownRunPlugin.PluginSettings.SalvageLegendaryItemLevel == 1 ? 1 : 2;
			comboBox_TownRunSalvage[3].SelectionChanged += SalvageRuleTypeChanged;
			spTownRun_SalvageOptions_LegendaryItems.Children.Add(comboBox_TownRunSalvage[3]);
			spTownRun_SalvageOptions.Children.Add(spTownRun_SalvageOptions_LegendaryItems);

			#endregion

			spSalvageOptions.Children.Add(spTownRun_SalvageOptions);
			LBWindowContent.Items.Add(spSalvageOptions);

			#endregion

			StackPanel spGambling = new StackPanel
			{
				Background = Brushes.DimGray,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};
			TextBlock txt_Gambling_Header = new TextBlock
			{
				Text = "Gambling Options",
				FontSize = 15,
				Background = Brushes.RoyalBlue,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 10),
			};
			spGambling.Children.Add(txt_Gambling_Header);

			CheckBox checkBox_Gambling_Enable = new CheckBox
			{
				Content = "Enable Gambling",
				Height = 30,
				IsChecked = (FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling),
				FontSize = 14,
			};
			checkBox_Gambling_Enable.Checked += BloodShardGamblingChecked;
			checkBox_Gambling_Enable.Unchecked += BloodShardGamblingChecked;
			spGambling.Children.Add(checkBox_Gambling_Enable);

			sp_BloodShardGambling = new StackPanel
			{
				IsEnabled = FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling,
			};
			
			#region Minimum Blood Shards

			StackPanel spMinimumBloodShards = new StackPanel
			{
				Margin = new Thickness(0, 0, 0, 10),
			};

			TextBlock txt_Gambling_MinimumBloodShards = new TextBlock
			{
				Text = "Minimum Blood Shards Required",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};
			spMinimumBloodShards.Children.Add(txt_Gambling_MinimumBloodShards);

			Slider sliderMinimumBloodShards = new Slider
			{
				Width = 100,
				Maximum = 500,
				Minimum = 5,
				TickFrequency = 50,
				LargeChange = 25,
				SmallChange = 5,
				Value = FunkyTownRunPlugin.PluginSettings.MinimumBloodShards,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderMinimumBloodShards.ValueChanged += MinimumBloodShardSliderChanged;
			tbMinimumBloodShards = new TextBox
			{
				Text = FunkyTownRunPlugin.PluginSettings.MinimumBloodShards.ToString(),
				IsReadOnly = true,
			};
			StackPanel spMinimumBloodShardControls = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			spMinimumBloodShardControls.Children.Add(sliderMinimumBloodShards);
			spMinimumBloodShardControls.Children.Add(tbMinimumBloodShards);
			spMinimumBloodShards.Children.Add(spMinimumBloodShardControls);
			sp_BloodShardGambling.Children.Add(spMinimumBloodShards);
			
			#endregion


			TextBlock txt_Gambling_ItemTypes = new TextBlock
			{
				Text = "Item Types",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};
			sp_BloodShardGambling.Children.Add(txt_Gambling_ItemTypes);

			WrapPanel wrapPanel_GamblingItemTypes = new WrapPanel
			{
				Orientation= Orientation.Vertical,
				HorizontalAlignment= HorizontalAlignment.Stretch,
				VerticalAlignment= VerticalAlignment.Stretch,
			};

			bool noFlags = FunkyTownRunPlugin.PluginSettings.BloodShardGambleItems.Equals(BloodShardGambleItems.None);
			var gambleItems = Enum.GetValues(typeof(BloodShardGambleItems));
			Func<object, string> fRetrieveNames = s => Enum.GetName(typeof(BloodShardGambleItems), s);
			foreach (var gambleItem in gambleItems)
			{
				var thisGambleItem = (BloodShardGambleItems)gambleItem;
				if (thisGambleItem.Equals(BloodShardGambleItems.None) || thisGambleItem.Equals(BloodShardGambleItems.All)) continue;

				string gambleItemName = fRetrieveNames(gambleItem);
				CheckBox cb = new CheckBox
				{
					Name = gambleItemName,
					Content = gambleItemName,
					IsChecked = !noFlags && FunkyTownRunPlugin.PluginSettings.BloodShardGambleItems.HasFlag(thisGambleItem),
					FontSize=12,
				};
				cb.Checked += BloodShardGambleItemsChanged;
				cb.Unchecked += BloodShardGambleItemsChanged;
				wrapPanel_GamblingItemTypes.Children.Add(cb);
			}
			sp_BloodShardGambling.Children.Add(wrapPanel_GamblingItemTypes);

			spGambling.Children.Add(sp_BloodShardGambling);

			LBWindowContent.Items.Add(spGambling);

			AddChild(LBWindowContent);
		}

		private void ItemRulesChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.UseItemRules = !FunkyTownRunPlugin.PluginSettings.UseItemRules;
		}
		private void ItemRulesTypeChanged(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.ItemRuleType = ItemRuleType.Items[ItemRuleType.SelectedIndex].ToString();
		}
		private void ItemRulesBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog OFD = new FolderBrowserDialog
			{

			};
			DialogResult OFD_Result = OFD.ShowDialog();

			if (OFD_Result == System.Windows.Forms.DialogResult.OK)
			{
				try
				{
					FunkyTownRunPlugin.PluginSettings.ItemRuleCustomPath = OFD.SelectedPath;
					tbCustomItemRulePath.Text = FunkyTownRunPlugin.PluginSettings.ItemRuleCustomPath;
				}
				catch
				{

				}
			}
		}
		private void ItemRulesLogKeepChanged(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.ItemRuleLogKeep = ItemRuleLogKeep.Items[ItemRuleLogKeep.SelectedIndex].ToString();
		}
		private void ItemRulesItemIDsChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.ItemRuleUseItemIDs = !FunkyTownRunPlugin.PluginSettings.ItemRuleUseItemIDs;
		}
		private void ItemRulesDebugChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.ItemRuleDebug = !FunkyTownRunPlugin.PluginSettings.ItemRuleDebug;
		}

		private void ItemManagerChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.UseItemManagerEvaluation = !FunkyTownRunPlugin.PluginSettings.UseItemManagerEvaluation;
		}
		private void StashHoradricCacheChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.StashHoradricCache = !FunkyTownRunPlugin.PluginSettings.StashHoradricCache;
		}
		private void BuyPotionsDuringTownRunChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.BuyPotionsDuringTownRun = !FunkyTownRunPlugin.PluginSettings.BuyPotionsDuringTownRun;
		}
		private void PotionCountSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = Convert.ToInt32(slider_sender.Value);
			FunkyTownRunPlugin.PluginSettings.PotionsCount = Value;
			tbPotionCount.Text = Value.ToString();
		}
		private void IdentifyLegendariesChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.IdentifyLegendaries = !FunkyTownRunPlugin.PluginSettings.IdentifyLegendaries;
		}
		private void SalvageRuleTypeChanged(object sender, EventArgs e)
		{
			ComboBox cbSender = (ComboBox)sender;
			if (cbSender.Name == "White")
				FunkyTownRunPlugin.PluginSettings.SalvageWhiteItemLevel = cbSender.SelectedIndex == 0 ? 0 : cbSender.SelectedIndex == 1 ? 1 : 61;
			else if (cbSender.Name == "Magic")
				FunkyTownRunPlugin.PluginSettings.SalvageMagicItemLevel = cbSender.SelectedIndex == 0 ? 0 : cbSender.SelectedIndex == 1 ? 1 : 61;
			else if (cbSender.Name == "Rare")
				FunkyTownRunPlugin.PluginSettings.SalvageRareItemLevel = cbSender.SelectedIndex == 0 ? 0 : cbSender.SelectedIndex == 1 ? 1 : 61;
			else if (cbSender.Name == "Legendary")
				FunkyTownRunPlugin.PluginSettings.SalvageLegendaryItemLevel = cbSender.SelectedIndex == 0 ? 0 : cbSender.SelectedIndex == 1 ? 1 : 61;
		}

		private void BloodShardGamblingChecked(object sender, EventArgs e)
		{
			FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling = !FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling;
			sp_BloodShardGambling.IsEnabled = FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling;
		}
		private void MinimumBloodShardSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = Convert.ToInt32(slider_sender.Value);
			FunkyTownRunPlugin.PluginSettings.MinimumBloodShards = Value;
			tbMinimumBloodShards.Text = Value.ToString();
		}

		private void BloodShardGambleItemsChanged(object sender, EventArgs e)
		{
			CheckBox cbSender = (CheckBox)sender;
			BloodShardGambleItems LogLevelValue = (BloodShardGambleItems)Enum.Parse(typeof(BloodShardGambleItems), cbSender.Name);

			if (FunkyTownRunPlugin.PluginSettings.BloodShardGambleItems.HasFlag(LogLevelValue))
				FunkyTownRunPlugin.PluginSettings.BloodShardGambleItems &= ~LogLevelValue;
			else
				FunkyTownRunPlugin.PluginSettings.BloodShardGambleItems |= LogLevelValue;
		}

		protected override void OnClosed(EventArgs e)
		{
			Settings.SerializeToXML(FunkyTownRunPlugin.PluginSettings);
			
			if (BotMain.IsRunning)
				FunkyTownRunPlugin.ItemRulesEval.reloadFromUI();
		
			base.OnClosed(e);
		}
	}
}
