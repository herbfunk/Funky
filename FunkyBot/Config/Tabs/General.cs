using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FunkyBot.Cache.Enums;


namespace FunkyBot
{
	internal partial class FunkyWindow : Window
	{
		#region EventHandling
		private void OutOfCombatMovementChecked(object sender, EventArgs e)
		{
			Bot.Settings.OutOfCombatMovement = !Bot.Settings.OutOfCombatMovement;
		}
		private void AllowBuffingInTownChecked(object sender, EventArgs e)
		{
			Bot.Settings.AllowBuffingInTown = !Bot.Settings.AllowBuffingInTown;
		}
		private void AfterCombatDelaySliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.AfterCombatDelay = Value;
			TBAfterCombatDelay.Text = Value.ToString();
		}
		private void GoldTimeoutValueSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Plugin.GoldInactivityTimeoutSeconds = Value;
			TBGoldInactivityTimeout.Text = Value.ToString();
		}
		private void BuyPotionsDuringTownRunChecked(object sender, EventArgs e)
		{
			Bot.Settings.BuyPotionsDuringTownRun = !Bot.Settings.BuyPotionsDuringTownRun;
		}
		private void EnableWaitAfterContainersChecked(object sender, EventArgs e)
		{
			Bot.Settings.EnableWaitAfterContainers = !Bot.Settings.EnableWaitAfterContainers;
		}





		#endregion


		private TextBox tbMonsterPower;
		private CheckBox BuyPotionsDuringTownRunCB;
		private CheckBox EnableWaitAfterContainersCB;
		private TextBox TBAfterCombatDelay;
		private TextBox TBGoldInactivityTimeout;

		internal void InitGeneralControls()
		{
			TabItem GeneralTab = new TabItem();
			GeneralTab.Header = "General";
			tcGeneral.Items.Add(GeneralTab);
			lbGeneralContent = new ListBox();

			#region Gold Inactivity Timeout
			TextBlock TextBlock_GoldTimeoutValue = new TextBlock
			{
				Text = "Gold Inactivity Seconds",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderGoldTimeoutValue = new Slider
			{
				Width = 100,
				Maximum = 900,
				Minimum = 0,
				TickFrequency = 60,
				LargeChange = 30,
				SmallChange = 5,
				Value = Bot.Settings.Plugin.GoldInactivityTimeoutSeconds,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGoldTimeoutValue.ValueChanged += GoldTimeoutValueSliderChanged;
			TBGoldInactivityTimeout = new TextBox
			{
				Text = Bot.Settings.Plugin.GoldInactivityTimeoutSeconds.ToString(),
				IsReadOnly = true,
			};
			StackPanel GoldTimeoutValueControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			GoldTimeoutValueControlStackPanel.Children.Add(sliderGoldTimeoutValue);
			GoldTimeoutValueControlStackPanel.Children.Add(TBGoldInactivityTimeout);

			StackPanel GoldTimeoutValueStackPanel = new StackPanel
			{
				Height = 40,
				Orientation = Orientation.Vertical,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 15, Margin.Bottom),
			};
			GoldTimeoutValueStackPanel.Children.Add(TextBlock_GoldTimeoutValue);
			GoldTimeoutValueStackPanel.Children.Add(GoldTimeoutValueControlStackPanel);
			lbGeneralContent.Items.Add(GoldTimeoutValueStackPanel);
			#endregion

			#region PotionsDuringTownRun
			BuyPotionsDuringTownRunCB = new CheckBox
			{
				Content = "Buy Potions During Town Run (Uses Maximum Potion Count Setting)",
				Width = 500,
				Height = 30,
				IsChecked = (Bot.Settings.BuyPotionsDuringTownRun)
			};
			BuyPotionsDuringTownRunCB.Checked += BuyPotionsDuringTownRunChecked;
			BuyPotionsDuringTownRunCB.Unchecked += BuyPotionsDuringTownRunChecked;
			lbGeneralContent.Items.Add(BuyPotionsDuringTownRunCB);
			#endregion



			#region AllowBuffingInTown
			CheckBox cbAllowBuffingInTown = new CheckBox
			{
				Content = "Allow Buffing In Town",
				Width = 300,
				Height = 30,
				IsChecked = (Bot.Settings.AllowBuffingInTown)
			};
			cbAllowBuffingInTown.Checked += AllowBuffingInTownChecked;
			cbAllowBuffingInTown.Unchecked += AllowBuffingInTownChecked;
			lbGeneralContent.Items.Add(cbAllowBuffingInTown);
			#endregion

			#region AfterCombatDelayOptions
			StackPanel AfterCombatDelayStackPanel = new StackPanel();
			#region AfterCombatDelay

			Slider sliderAfterCombatDelay = new Slider
			{
				Width = 100,
				Maximum = 2000,
				Minimum = 0,
				TickFrequency = 200,
				LargeChange = 100,
				SmallChange = 50,
				Value = Bot.Settings.AfterCombatDelay,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			sliderAfterCombatDelay.ValueChanged += AfterCombatDelaySliderChanged;
			TBAfterCombatDelay = new TextBox
			{
				Margin = new Thickness(Margin.Left + 5, Margin.Top, Margin.Right, Margin.Bottom),
				Text = Bot.Settings.AfterCombatDelay.ToString(),
				IsReadOnly = true,
			};
			StackPanel AfterCombatStackPanel = new StackPanel
			{
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				Orientation = Orientation.Horizontal,
			};
			AfterCombatStackPanel.Children.Add(sliderAfterCombatDelay);
			AfterCombatStackPanel.Children.Add(TBAfterCombatDelay);

			#endregion
			#region WaitTimerAfterContainers
			EnableWaitAfterContainersCB = new CheckBox
			{
				Content = "Apply Delay After Opening Containers",
				Width = 300,
				Height = 20,
				IsChecked = (Bot.Settings.EnableWaitAfterContainers)
			};
			EnableWaitAfterContainersCB.Checked += EnableWaitAfterContainersChecked;
			EnableWaitAfterContainersCB.Unchecked += EnableWaitAfterContainersChecked;

			#endregion

			TextBlock CombatLootDelay_Text_Info = new TextBlock
			{
				Text = "End of Combat Delay Timer",
				FontSize = 11,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			AfterCombatDelayStackPanel.Children.Add(CombatLootDelay_Text_Info);
			AfterCombatDelayStackPanel.Children.Add(AfterCombatStackPanel);
			AfterCombatDelayStackPanel.Children.Add(EnableWaitAfterContainersCB);
			lbGeneralContent.Items.Add(AfterCombatDelayStackPanel);

			#endregion


			StackPanel spShrinePanel = new StackPanel();
			TextBlock Shrines_Header_Text = new TextBlock
			{
				Text = "Shrines",
				FontSize = 13,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				TextAlignment = TextAlignment.Left,
			};
			spShrinePanel.Children.Add(Shrines_Header_Text);
			StackPanel spShrineUseOptions = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			CheckBox[] cbUseShrine = new CheckBox[6];
			string[] ShrineNames = Enum.GetNames(typeof(ShrineTypes));
			for (int i = 0; i < 6; i++)
			{
				cbUseShrine[i] = new CheckBox
				{
					Content = ShrineNames[i],
					Name = ShrineNames[i],
					IsChecked = Bot.Settings.Targeting.UseShrineTypes[i],
					Margin = new Thickness(Margin.Left + 3, Margin.Top, Margin.Right, Margin.Bottom + 5),
				};
				cbUseShrine[i].Checked += UseShrineChecked;
				cbUseShrine[i].Unchecked += UseShrineChecked;
				spShrineUseOptions.Children.Add(cbUseShrine[i]);
			}
			spShrinePanel.Children.Add(spShrineUseOptions);

			lbGeneralContent.Items.Add(spShrinePanel);

			GeneralTab.Content = lbGeneralContent;

			InitAdventureModeControls();
			InitBacktrackingControls();
		}
	}
}
