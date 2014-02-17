using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FunkyBot
{
	internal partial class FunkyWindow : Window
	{
		#region EventHandling
		private void GroupBotHealthSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			double Value = Convert.ToDouble(slider_sender.Value.ToString("F2", CultureInfo.InvariantCulture));
			Bot.Settings.Grouping.GroupingMinimumBotHealth = Value;
			TBGroupingMinimumBotHP.Text = Value.ToString();
		}
		private void GroupMinimumUnitDistanceSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMinimumUnitDistance = Value;
			TBGroupingMinUnitDistance.Text = Value.ToString();
		}
		private void GroupMaxDistanceSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMaximumDistanceAllowed = Value;
			TBGroupingMaxDistance.Text = Value.ToString();
		}
		private void GroupMinimumClusterCountSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMinimumClusterCount = Value;
			TBGroupingMinimumClusterCount.Text = Value.ToString();
		}
		private void GroupMinimumUnitsInClusterSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Grouping.GroupingMinimumUnitsInCluster = Value;
			TBGroupingMinimumUnitsInCluster.Text = Value.ToString();
		}
		private void GroupingBehaviorChecked(object sender, EventArgs e)
		{
			Bot.Settings.Grouping.AttemptGroupingMovements = !Bot.Settings.Grouping.AttemptGroupingMovements;
			bool enabled = Bot.Settings.Grouping.AttemptGroupingMovements;
			spGroupingOptions.IsEnabled = enabled;
		}
		#endregion

		private TextBox TBGroupingMinUnitDistance, TBGroupingMinimumBotHP, TBGroupingMaxDistance, TBGroupingMinimumClusterCount, TBGroupingMinimumUnitsInCluster;

		internal void InitGroupingControls()
		{
			TabItem CombatGroupingTabItem = new TabItem();
			CombatGroupingTabItem.Header = "Grouping";
			CombatTabControl.Items.Add(CombatGroupingTabItem);
			ListBox CombatGroupingContentListBox = new ListBox();

			#region Grouping

			Button BtnGroupingLoadTemplate = new Button
			{
				Content = "Load Setup",
				Background = Brushes.OrangeRed,
				Foreground = Brushes.GhostWhite,
				FontStyle = FontStyles.Italic,
				FontSize = 12,

				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Width = 75,
				Height = 30,

				Margin = new Thickness(Margin.Left, Margin.Top + 5, Margin.Right, Margin.Bottom + 5),
			};
			BtnGroupingLoadTemplate.Click += GroupingLoadXMLClicked;

			ToolTip TTGrouping = new ToolTip
			{
				Content = "Attempts to engage additional nearby monster groups",
			};
			StackPanel GroupingOptionsStackPanel = new StackPanel
			{
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				Background = Brushes.DimGray,
				ToolTip = TTGrouping,
			};
			TextBlock Grouping_Text_Header = new TextBlock
			{
				Text = "Grouping",
				FontSize = 12,
				Background = Brushes.LightSeaGreen,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				ToolTip = TTGrouping,
			};
			GroupingOptionsStackPanel.Children.Add(Grouping_Text_Header);
			CBGroupingBehavior = new CheckBox
			{
				Content = "Enable Grouping",
				Height = 20,
				HorizontalContentAlignment = HorizontalAlignment.Left,
				IsChecked = Bot.Settings.Grouping.AttemptGroupingMovements,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
			};
			CBGroupingBehavior.Checked += GroupingBehaviorChecked;
			CBGroupingBehavior.Unchecked += GroupingBehaviorChecked;
			GroupingOptionsStackPanel.Children.Add(CBGroupingBehavior);

			spGroupingOptions = new StackPanel
			{
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				IsEnabled = Bot.Settings.Grouping.AttemptGroupingMovements,
			};

			#region Grouping Minimum Distance
			TextBlock Group_MinimumUnitDistance_Label = new TextBlock
			{
				Text = "Unit Min Distance",
				FontSize = 13,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};

			sliderGroupingMinimumUnitDistance = new Slider
			{
				Width = 100,
				Maximum = 100,
				Minimum = 35,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Grouping.GroupingMinimumUnitDistance,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGroupingMinimumUnitDistance.ValueChanged += GroupMinimumUnitDistanceSliderChanged;
			TBGroupingMinUnitDistance = new TextBox
			{
				Text = Bot.Settings.Grouping.GroupingMinimumUnitDistance.ToString(),
				IsReadOnly = true,
			};
			StackPanel GroupingMinUnitDistanceStackPanel = new StackPanel
			{
				// Width=600,
				Height = 20,
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 6),
			};

			GroupingMinUnitDistanceStackPanel.Children.Add(sliderGroupingMinimumUnitDistance);
			GroupingMinUnitDistanceStackPanel.Children.Add(TBGroupingMinUnitDistance);

			StackPanel SPGroupMinDistance = new StackPanel
			{
				Orientation = Orientation.Vertical,
			};
			SPGroupMinDistance.Children.Add(Group_MinimumUnitDistance_Label);
			SPGroupMinDistance.Children.Add(GroupingMinUnitDistanceStackPanel);
			#endregion

			#region Grouping Maximum Distance
			TextBlock Group_MonsterDistance_Label = new TextBlock
			{
				Text = "Group Max Distance",
				FontSize = 13,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};

			sliderGroupingMaximumDistance = new Slider
			{
				Width = 100,
				Maximum = 200,
				Minimum = 50,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Grouping.GroupingMaximumDistanceAllowed,
				HorizontalAlignment = HorizontalAlignment.Center,
			};
			sliderGroupingMaximumDistance.ValueChanged += GroupMaxDistanceSliderChanged;
			TBGroupingMaxDistance = new TextBox
			{
				Text = Bot.Settings.Grouping.GroupingMaximumDistanceAllowed.ToString(),
				IsReadOnly = true,
			};
			StackPanel GroupingMaxDistanceStackPanel = new StackPanel
			{
				//	 Width=600,
				Height = 20,
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 6),
			};

			GroupingMaxDistanceStackPanel.Children.Add(sliderGroupingMaximumDistance);
			GroupingMaxDistanceStackPanel.Children.Add(TBGroupingMaxDistance);

			StackPanel SPGroupMaxDistance = new StackPanel
			{
				Orientation = Orientation.Vertical,
			};
			SPGroupMaxDistance.Children.Add(Group_MonsterDistance_Label);
			SPGroupMaxDistance.Children.Add(GroupingMaxDistanceStackPanel);
			#endregion

			StackPanel SPGroupDistances = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			SPGroupDistances.Children.Add(SPGroupMinDistance);
			SPGroupDistances.Children.Add(SPGroupMaxDistance);
			spGroupingOptions.Children.Add(SPGroupDistances);


			#region Grouping Minimum Cluster Count
			TextBlock Group_MinimumCluster_Label = new TextBlock
			{
				Text = "Minimum Cluster Count",
				FontSize = 13,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};

			sliderGroupingMinimumCluster = new Slider
			{
				Width = 100,
				Maximum = 8,
				Minimum = 1,
				TickFrequency = 1,
				LargeChange = 1,
				SmallChange = 1,
				Value = Bot.Settings.Grouping.GroupingMinimumClusterCount,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGroupingMinimumCluster.ValueChanged += GroupMinimumClusterCountSliderChanged;
			TBGroupingMinimumClusterCount = new TextBox
			{
				Text = Bot.Settings.Grouping.GroupingMinimumClusterCount.ToString(),
				IsReadOnly = true,
			};
			StackPanel GroupingMinimumClusterCountStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 6),
			};

			GroupingMinimumClusterCountStackPanel.Children.Add(sliderGroupingMinimumCluster);
			GroupingMinimumClusterCountStackPanel.Children.Add(TBGroupingMinimumClusterCount);
			#endregion

			spGroupingOptions.Children.Add(Group_MinimumCluster_Label);
			spGroupingOptions.Children.Add(GroupingMinimumClusterCountStackPanel);

			#region Grouping Minimum Units In Cluster
			TextBlock Group_MinimumUnits_Label = new TextBlock
			{
				Text = "Minimum Units In Cluster",
				FontSize = 13,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};

			sliderGroupingMinimumUnits = new Slider
			{
				Width = 100,
				Maximum = 10,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 2,
				SmallChange = 1,
				Value = Bot.Settings.Grouping.GroupingMinimumUnitsInCluster,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGroupingMinimumUnits.ValueChanged += GroupMinimumUnitsInClusterSliderChanged;
			TBGroupingMinimumUnitsInCluster = new TextBox
			{
				Text = Bot.Settings.Grouping.GroupingMinimumUnitsInCluster.ToString(),
				IsReadOnly = true,
			};
			StackPanel GroupingMinimumUnitstStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 6),
			};

			GroupingMinimumUnitstStackPanel.Children.Add(sliderGroupingMinimumUnits);
			GroupingMinimumUnitstStackPanel.Children.Add(TBGroupingMinimumUnitsInCluster);
			#endregion

			spGroupingOptions.Children.Add(Group_MinimumUnits_Label);
			spGroupingOptions.Children.Add(GroupingMinimumUnitstStackPanel);

			#region Grouping Minimum Bot Health
			TextBlock Group_MinimumBotHP_Label = new TextBlock
			{
				Text = "Minimum Bot HP To Engage",
				FontSize = 13,
				Foreground = Brushes.GhostWhite,
				//Background = System.Windows.Media.Brushes.Crimson,
				TextAlignment = TextAlignment.Left,
			};

			Slider sliderGroupingMinimumBotHP = new Slider
			{
				Width = 100,
				Maximum = 1,
				Minimum = 0,
				TickFrequency = 0.10,
				LargeChange = 0.25,
				SmallChange = 0.05,
				Value = Bot.Settings.Grouping.GroupingMinimumBotHealth,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGroupingMinimumBotHP.ValueChanged += GroupBotHealthSliderChanged;
			TBGroupingMinimumBotHP = new TextBox
			{
				Text = Bot.Settings.Grouping.GroupingMinimumBotHealth.ToString(),
				IsReadOnly = true,
			};
			ToolTip TTGroupingBotHP = new ToolTip
			{
				Content = "Minimum Bot Health Percent Allowed To Start Grouping Behavior",
			};
			StackPanel GroupingMinimumBotHPStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 6),
				ToolTip = TTGroupingBotHP,
			};

			GroupingMinimumBotHPStackPanel.Children.Add(sliderGroupingMinimumBotHP);
			GroupingMinimumBotHPStackPanel.Children.Add(TBGroupingMinimumBotHP);
			#endregion

			spGroupingOptions.Children.Add(Group_MinimumBotHP_Label);
			spGroupingOptions.Children.Add(GroupingMinimumBotHPStackPanel);

			//


			GroupingOptionsStackPanel.Children.Add(spGroupingOptions);
			GroupingOptionsStackPanel.Children.Add(BtnGroupingLoadTemplate);

			CombatGroupingContentListBox.Items.Add(GroupingOptionsStackPanel);

			#endregion

			CombatGroupingTabItem.Content = CombatGroupingContentListBox;
		}
	}
}
