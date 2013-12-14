using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace FunkyBot
{
	internal partial class FunkyWindow : Window
	{
		#region EventHandling
		private void BacktrackingEnableChecked(object sender, EventArgs e)
		{
			Bot.Settings.Backtracking.EnableBacktracking = !Bot.Settings.Backtracking.EnableBacktracking;
		}
		private void BacktrackingCombatStartChecked(object sender, EventArgs e)
		{
			Bot.Settings.Backtracking.TrackStartOfCombatEngagment = !Bot.Settings.Backtracking.TrackStartOfCombatEngagment;
		}
		private void BacktrackingItemLootChecked(object sender, EventArgs e)
		{
			Bot.Settings.Backtracking.TrackLootableItems = !Bot.Settings.Backtracking.TrackLootableItems;
		}

		private void BacktrackingRangeSliderChange(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Backtracking.MinimumDistanceFromStart = Value;
			tbBacktrackRange.Text = Value.ToString();
		}

		#endregion


		private TextBox tbBacktrackRange;
		private ListBox lbBacktrackingContent;

		internal void InitBacktrackingControls()
		{
			TabItem BacktrackingTab = new TabItem();
			BacktrackingTab.Header = "Backtracking";
			tcGeneral.Items.Add(BacktrackingTab);
			lbBacktrackingContent = new ListBox();

			#region EnableBacktracking
			CheckBox cbEnableBacktracking = new CheckBox
			{
				Content = "Enable Backtracking",
				Width = 500,
				Height = 30,
				IsChecked = (Bot.Settings.Backtracking.EnableBacktracking),
				IsEnabled=false, //Disabled Until Further Developement!
			};
			cbEnableBacktracking.Checked += BacktrackingEnableChecked;
			cbEnableBacktracking.Unchecked += BacktrackingEnableChecked;
			lbBacktrackingContent.Items.Add(cbEnableBacktracking);
			#endregion

			#region BacktrackStartOfCombat
			ToolTip backtrackStartCombat = new ToolTip
			{
				Content="This will set the return position where combat first begins, otherwise it uses the last non-combat position.",
			};

			CheckBox cbBacktrackStartOfCombat = new CheckBox
			{
				Content = "Backtrack to combat start position",
				Width = 500,
				Height = 30,
				IsChecked = (Bot.Settings.Backtracking.TrackStartOfCombatEngagment),
				ToolTip = backtrackStartCombat,
				IsEnabled=false, //Disabled Until Further Developement!
			};
			cbBacktrackStartOfCombat.Checked += BacktrackingCombatStartChecked;
			cbBacktrackStartOfCombat.Unchecked += BacktrackingCombatStartChecked;
			lbBacktrackingContent.Items.Add(cbBacktrackStartOfCombat);
			#endregion

			#region Enable Item Backtracking
			ToolTip ItemBacktrackingToolTip = new ToolTip
			{
				Content = "Items will be added to Line of Sight Movement list",
			};
			CheckBox cbItemBacktracking = new CheckBox
			{
				Content = "Enable Item Backtracking",
				Width = 300,
				Height = 30,
				IsChecked = (Bot.Settings.Backtracking.TrackLootableItems),
				ToolTip = ItemBacktrackingToolTip,
			};
			cbItemBacktracking.Checked += BacktrackingItemLootChecked;
			cbItemBacktracking.Unchecked += BacktrackingItemLootChecked;
			lbBacktrackingContent.Items.Add(cbItemBacktracking);
			#endregion

			#region BacktrackingRange
			TextBlock BacktrackingRange_Text = new TextBlock
			{
				Text = " Backtracking Range",
				FontSize = 13,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Center,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			Slider sliderBacktrackingRange = new Slider
			{
				Width = 100,
				Maximum = 100,
				Minimum = 5,
				TickFrequency = 5,
				LargeChange = 25,
				SmallChange = 2.5,
				Value = Bot.Settings.Backtracking.MinimumDistanceFromStart,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderBacktrackingRange.ValueChanged += BacktrackingRangeSliderChange;
			tbBacktrackRange = new TextBox
			{
				Text = sliderBacktrackingRange.Value.ToString(),
				IsReadOnly = true,
			};
			StackPanel SPBacktrackRange = new StackPanel
			{
				Height = 30,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
				IsEnabled=false, //Disabled Until Further Developement!
			};
			SPBacktrackRange.Children.Add(BacktrackingRange_Text);
			SPBacktrackRange.Children.Add(sliderBacktrackingRange);
			SPBacktrackRange.Children.Add(tbBacktrackRange);
			lbBacktrackingContent.Items.Add(SPBacktrackRange);
			#endregion

			BacktrackingTab.Content = lbBacktrackingContent;

		}
	}
}
