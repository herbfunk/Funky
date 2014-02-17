using System;
using System.Windows;
using System.Windows.Controls;

namespace FunkyBot
{
	internal partial class FunkyWindow : Window
	{
		#region EventHandling
		private void GilesWeaponScoreSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Loot.GilesMinimumWeaponScore = Value;
			TBGilesWeaponScore.Text = Value.ToString();
		}
		private void GilesArmorScoreSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Loot.GilesMinimumArmorScore = Value;
			TBGilesArmorScore.Text = Value.ToString();
		}
		private void GilesJeweleryScoreSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Loot.GilesMinimumJeweleryScore = Value;
			TBGilesJeweleryScore.Text = Value.ToString();
		}
		#endregion

		private TextBox TBGilesWeaponScore, TBGilesArmorScore, TBGilesJeweleryScore;

		internal void InitItemScoringControls()
		{

			#region Scoring
			TabItem ItemGilesScoringTabItem = new TabItem();
			ItemGilesScoringTabItem.Header = "Scoring";
			tcItems.Items.Add(ItemGilesScoringTabItem);
			ListBox lbGilesScoringContent = new ListBox();

			#region WeaponScore
			lbGilesScoringContent.Items.Add("Minimum Weapon Score to Stash");
			Slider sliderGilesWeaponScore = new Slider
			{
				Width = 100,
				Maximum = 100000,
				Minimum = 1,
				TickFrequency = 1000,
				LargeChange = 5000,
				SmallChange = 1000,
				Value = Bot.Settings.Loot.GilesMinimumWeaponScore,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGilesWeaponScore.ValueChanged += GilesWeaponScoreSliderChanged;
			TBGilesWeaponScore = new TextBox
			{
				Text = Bot.Settings.Loot.GilesMinimumWeaponScore.ToString(),
				IsReadOnly = true,
			};
			StackPanel GilesWeaponScoreStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			GilesWeaponScoreStackPanel.Children.Add(sliderGilesWeaponScore);
			GilesWeaponScoreStackPanel.Children.Add(TBGilesWeaponScore);
			lbGilesScoringContent.Items.Add(GilesWeaponScoreStackPanel);
			#endregion

			#region ArmorScore
			lbGilesScoringContent.Items.Add("Minimum Armor Score to Stash");
			Slider sliderGilesArmorScore = new Slider
			{
				Width = 100,
				Maximum = 100000,
				Minimum = 1,
				TickFrequency = 1000,
				LargeChange = 5000,
				SmallChange = 1000,
				Value = Bot.Settings.Loot.GilesMinimumArmorScore,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGilesArmorScore.ValueChanged += GilesArmorScoreSliderChanged;
			TBGilesArmorScore = new TextBox
			{
				Text = Bot.Settings.Loot.GilesMinimumArmorScore.ToString(),
				IsReadOnly = true,
			};
			StackPanel GilesArmorScoreStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			GilesArmorScoreStackPanel.Children.Add(sliderGilesArmorScore);
			GilesArmorScoreStackPanel.Children.Add(TBGilesArmorScore);
			lbGilesScoringContent.Items.Add(GilesArmorScoreStackPanel);
			#endregion

			#region JeweleryScore
			lbGilesScoringContent.Items.Add("Minimum Jewelery Score to Stash");
			Slider sliderGilesJeweleryScore = new Slider
			{
				Width = 100,
				Maximum = 100000,
				Minimum = 1,
				TickFrequency = 1000,
				LargeChange = 5000,
				SmallChange = 1000,
				Value = Bot.Settings.Loot.GilesMinimumJeweleryScore,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGilesJeweleryScore.ValueChanged += GilesJeweleryScoreSliderChanged;
			TBGilesJeweleryScore = new TextBox
			{
				Text = Bot.Settings.Loot.GilesMinimumJeweleryScore.ToString(),
				IsReadOnly = true,
			};
			StackPanel GilesJeweleryScoreStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			GilesJeweleryScoreStackPanel.Children.Add(sliderGilesJeweleryScore);
			GilesJeweleryScoreStackPanel.Children.Add(TBGilesJeweleryScore);
			lbGilesScoringContent.Items.Add(GilesJeweleryScoreStackPanel);
			#endregion

			ItemGilesScoringTabItem.Content = lbGilesScoringContent;
			#endregion

		}
	}
}
