using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FunkyBot.DBHandlers;
using Zeta.Common;

namespace FunkyBot
{
	internal partial class FunkyWindow : Window
	{
		#region EventHandling
		private void StashHoradricCacheChecked(object sender, EventArgs e)
		{
			Bot.Settings.TownRun.StashHoradricCache = !Bot.Settings.TownRun.StashHoradricCache;
		}
		private void BloodShardGamblingChecked(object sender, EventArgs e)
		{
			Bot.Settings.TownRun.EnableBloodShardGambling = !Bot.Settings.TownRun.EnableBloodShardGambling;
		}
		private void BloodShardMinimumCountSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.TownRun.MinimumBloodShards = Value;
			TBBloodShardMinimumCount.Text = Value.ToString();
		}
		private void BloodShardGambleItemsChanged(object sender, EventArgs e)
		{
			CheckBox cbSender = (CheckBox)sender;
			BloodShardGambleItems LogLevelValue = (BloodShardGambleItems)Enum.Parse(typeof(BloodShardGambleItems), cbSender.Name);

			if (Bot.Settings.TownRun.BloodShardGambleItems.HasFlag(LogLevelValue))
				Bot.Settings.TownRun.BloodShardGambleItems &= ~LogLevelValue;
			else
				Bot.Settings.TownRun.BloodShardGambleItems |= LogLevelValue;
		}
		private void BloodShardGambleItemComboBoxSelected(object sender, EventArgs e)
		{
			RadioButton CBsender = (RadioButton)sender;
			if (CBsender.Name == "ItemsNone")
			{
				CBBloodShardGambleItems.ForEach(cb => cb.IsChecked = false);
				Bot.Settings.TownRun.BloodShardGambleItems = BloodShardGambleItems.None;
			}
			else
			{
				CBBloodShardGambleItems.ForEach(cb => cb.IsChecked = true);
				Bot.Settings.TownRun.BloodShardGambleItems = BloodShardGambleItems.All;
			}
		}
		#endregion


		private ListBox lbTownRunContent;
		private Slider sliderBloodShardMinimumCount;
		private TextBox TBBloodShardMinimumCount;
		private CheckBox[] CBBloodShardGambleItems;

		internal void InitTownRunControls()
		{
			TabItem TownRunTab = new TabItem();
			TownRunTab.Header = "Town Run";
			tcItems.Items.Add(TownRunTab);
			lbTownRunContent = new ListBox();

			StackPanel TownRunStackPanel = new StackPanel
			{
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				Background = Brushes.DimGray,
			};
			TextBlock TownRun_Text_Header = new TextBlock
			{
				Text = "Town Run Options",
				FontSize = 12,
				Background = Brushes.LightSeaGreen,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};
			TownRunStackPanel.Children.Add(TownRun_Text_Header);

			#region Stash Horadric Cache
			CheckBox cbStashHoradricCache = new CheckBox
			{
				Content = "Stash Horadric Cache",
				Width = 500,
				Height = 30,
				IsChecked = (Bot.Settings.TownRun.StashHoradricCache),
			};
			cbStashHoradricCache.Checked += StashHoradricCacheChecked;
			cbStashHoradricCache.Unchecked += StashHoradricCacheChecked;
			TownRunStackPanel.Children.Add(cbStashHoradricCache);
			#endregion

			lbTownRunContent.Items.Add(TownRunStackPanel);


			StackPanel BloodshardGamblingStackPanel = new StackPanel
			{
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				Background = Brushes.DimGray,
			};

			TextBlock BloodShard_Text_Header = new TextBlock
			{
				Text = "BloodShard Gambling",
				FontSize = 12,
				Background = Brushes.LightSeaGreen,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};
			BloodshardGamblingStackPanel.Children.Add(BloodShard_Text_Header);

			#region Bloodshard Gambling Checkbox
			CheckBox cbBloodShardGambling = new CheckBox
			{
				Content = "Allow Bloodshard Gambling",
				Width = 500,
				Height = 30,
				IsChecked = (Bot.Settings.TownRun.EnableBloodShardGambling),
			};
			cbBloodShardGambling.Checked += BloodShardGamblingChecked;
			cbBloodShardGambling.Unchecked += BloodShardGamblingChecked;
			BloodshardGamblingStackPanel.Children.Add(cbBloodShardGambling);
			#endregion

			#region Bloodshard Minimum Count
			TextBlock Bloodshard_MinimumCount_Label = new TextBlock
			{
				Text = "Bloodshard Minimum Count",
				FontSize = 13,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};

			sliderBloodShardMinimumCount = new Slider
			{
				Width = 100,
				Maximum = 500,
				Minimum = 5,
				TickFrequency = 100,
				LargeChange = 50,
				SmallChange = 1,
				Value = Bot.Settings.TownRun.MinimumBloodShards,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderBloodShardMinimumCount.ValueChanged += BloodShardMinimumCountSliderChanged;
			TBBloodShardMinimumCount = new TextBox
			{
				Text = Bot.Settings.TownRun.MinimumBloodShards.ToString(),
				IsReadOnly = true,
			};
			StackPanel BloodShardMinimumCountStackPanel = new StackPanel
			{
				// Width=600,
				Height = 20,
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 6),
			};

			BloodShardMinimumCountStackPanel.Children.Add(sliderBloodShardMinimumCount);
			BloodShardMinimumCountStackPanel.Children.Add(TBBloodShardMinimumCount);

			StackPanel SPBloodShardMinCount = new StackPanel
			{
				Orientation = Orientation.Vertical,
			};
			SPBloodShardMinCount.Children.Add(Bloodshard_MinimumCount_Label);
			SPBloodShardMinCount.Children.Add(BloodShardMinimumCountStackPanel);
			BloodshardGamblingStackPanel.Children.Add(SPBloodShardMinCount);
			#endregion

			


			TextBlock Bloodshard_Gambling_Items_Header = new TextBlock
			{
				Text = "Bloodshard Gambling Items",
				FontSize = 12,
				Background = Brushes.MediumSeaGreen,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};

			StackPanel StackPanelBloodShardGamblingItems = new StackPanel
			{
				Orientation = Orientation.Vertical,
				VerticalAlignment = VerticalAlignment.Stretch,
			};
			StackPanelBloodShardGamblingItems.Children.Add(Bloodshard_Gambling_Items_Header);

			var gambleItems = Enum.GetValues(typeof(BloodShardGambleItems));
			Logger.GetLogLevelName fRetrieveNames = s => Enum.GetName(typeof(BloodShardGambleItems), s);

			CBBloodShardGambleItems = new CheckBox[gambleItems.Length - 2];
			int counter = 0;
			bool noFlags = Bot.Settings.TownRun.BloodShardGambleItems.Equals(BloodShardGambleItems.None);
			foreach (var gambleItem in gambleItems)
			{
				var thisGambleItem = (BloodShardGambleItems)gambleItem;
				if (thisGambleItem.Equals(BloodShardGambleItems.None) || thisGambleItem.Equals(BloodShardGambleItems.All)) continue;

				string gambleItemName = fRetrieveNames(gambleItem);
				CBBloodShardGambleItems[counter] = new CheckBox
				{
					Name = gambleItemName,
					Content = gambleItemName,
					IsChecked = !noFlags ? Bot.Settings.TownRun.BloodShardGambleItems.HasFlag(thisGambleItem) : false,
				};
				CBBloodShardGambleItems[counter].Checked += BloodShardGambleItemsChanged;
				CBBloodShardGambleItems[counter].Unchecked += BloodShardGambleItemsChanged;

				StackPanelBloodShardGamblingItems.Children.Add(CBBloodShardGambleItems[counter]);
				counter++;
			}
			StackPanel StackPanelGamblingItemComboBoxes = new StackPanel();
			RadioButton cbGambleItemsNone = new RadioButton
			{
				Name = "ItemsNone",
				Content = "None",
			};
			cbGambleItemsNone.Checked += BloodShardGambleItemComboBoxSelected;
			RadioButton cbGambleItemsAll = new RadioButton
			{
				Name = "ItemsAll",
				Content = "All",
			};
			cbGambleItemsAll.Checked += BloodShardGambleItemComboBoxSelected;

			StackPanelGamblingItemComboBoxes.Children.Add(cbGambleItemsNone);
			StackPanelGamblingItemComboBoxes.Children.Add(cbGambleItemsAll);
			StackPanelBloodShardGamblingItems.Children.Add(StackPanelGamblingItemComboBoxes);


			BloodshardGamblingStackPanel.Children.Add(StackPanelBloodShardGamblingItems);

			lbTownRunContent.Items.Add(BloodshardGamblingStackPanel);

			TownRunTab.Content = lbTownRunContent;

		}
	}
}
