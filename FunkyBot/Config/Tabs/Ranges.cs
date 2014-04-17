using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using FunkyBot.Settings;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListBox = System.Windows.Controls.ListBox;
using Orientation = System.Windows.Controls.Orientation;
using TextBox = System.Windows.Controls.TextBox;

namespace FunkyBot
{
	internal partial class FunkyWindow : Window
	{
		#region EventHandling
		private void RangeLoadXMLClicked(object sender, EventArgs e)
		{
			OpenFileDialog OFD = new OpenFileDialog
			{
				InitialDirectory = Path.Combine(FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
				RestoreDirectory = false,
				Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
				Title = "Ranges Template",
			};
			DialogResult OFD_Result = OFD.ShowDialog();

			if (OFD_Result == System.Windows.Forms.DialogResult.OK)
			{
				try
				{
					//;
					SettingRanges newSettings = SettingRanges.DeserializeFromXML(OFD.FileName);
					Bot.Settings.Ranges = newSettings;

					funkyConfigWindow.Close();
				}
				catch
				{

				}
			}


		}

		private void IgnoreCombatRangeChecked(object sender, EventArgs e)
		{
			Bot.Settings.Ranges.IgnoreCombatRange = !Bot.Settings.Ranges.IgnoreCombatRange;
		}
		private void IgnoreLootRangeChecked(object sender, EventArgs e)
		{
			Bot.Settings.Ranges.IgnoreLootRange = !Bot.Settings.Ranges.IgnoreLootRange;
		}
		private void IgnoreProfileBlacklistChecked(object sender, EventArgs e)
		{
			Bot.Settings.Ranges.IgnoreProfileBlacklists = !Bot.Settings.Ranges.IgnoreProfileBlacklists;
		}
		private void EliteRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.EliteCombatRange = Value;
			TBEliteRange.Text = Value.ToString();
		}
		private void GoldRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.GoldRange = Value;
			TBGoldRange.Text = Value.ToString();
		}
		private void GlobeRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.GlobeRange = Value;
			TBGlobeRange.Text = Value.ToString();
		}
		private void PotionRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.PotionRange = Value;
			TBPotionRange.Text = Value.ToString();
		}
		private void ItemRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.ItemRange = Value;
			TBItemRange.Text = Value.ToString();
		}
		private void ShrineRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.ShrineRange = Value;
			TBShrineRange.Text = Value.ToString();
		}

		private void ContainerRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.ContainerOpenRange = Value;
			TBContainerRange.Text = Value.ToString();
		}
		private void NonEliteRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.NonEliteCombatRange = Value;
			TBNonEliteRange.Text = Value.ToString();
		}
		private void TreasureGoblinRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.TreasureGoblinRange = Value;
			TBGoblinRange.Text = Value.ToString();
		}
		private void DestructibleSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.DestructibleRange = Value;
			TBDestructibleRange.Text = Value.ToString();
		}
		private void PoolsOfReflectionRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.PoolsOfReflectionRange = Value;
			TBPoolReflectionRange.Text = Value.ToString();
		}
		private void CursedShrineRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.CursedShrineRange = Value;
			TBCursedShrineRange.Text = Value.ToString();
		}
		private void CursedChestRangeSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Ranges.CursedChestRange = Value;
			TBCursedChestRange.Text = Value.ToString();
		}
		#endregion


		private TextBox TBContainerRange, TBNonEliteRange, TBDestructibleRange,
							  TBGlobeRange, TBGoblinRange, TBItemRange,
							  TBShrineRange, TBEliteRange, TBGoldRange, TBPotionRange,
							  TBPoolReflectionRange, TBCursedShrineRange, TBCursedChestRange;


		internal void InitTargetRangeControls()
		{

			#region Targeting_Ranges
			TabItem RangeTabItem = new TabItem();
			RangeTabItem.Header = "Range";
			tcTargeting.Items.Add(RangeTabItem);
			ListBox lbTargetRange = new ListBox();

			StackPanel ProfileRelatedSettings = new StackPanel();

			TextBlock Profile_Values_Text = new TextBlock
			{
				Text = "Profile Related Values",
				FontSize = 13,
				Background = Brushes.DarkSeaGreen,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};
			ProfileRelatedSettings.Children.Add(Profile_Values_Text);

			StackPanel spIgnoreProfileValues = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};
			CheckBox cbIgnoreCombatRange = new CheckBox
			{
				Content = "Ignore Combat Range (Set by Profile)",
				// Width = 300,
				HorizontalContentAlignment = HorizontalAlignment.Left,
				Height = 30,
				IsChecked = (Bot.Settings.Ranges.IgnoreCombatRange)
			};
			cbIgnoreCombatRange.Checked += IgnoreCombatRangeChecked;
			cbIgnoreCombatRange.Unchecked += IgnoreCombatRangeChecked;
			spIgnoreProfileValues.Children.Add(cbIgnoreCombatRange);
			CheckBox cbIgnoreLootRange = new CheckBox
			{
				Content = "Ignore Loot Range (Set by Profile)",
				// Width = 300,
				Height = 30,
				HorizontalContentAlignment = HorizontalAlignment.Right,
				IsChecked = (Bot.Settings.Ranges.IgnoreLootRange)
			};
			cbIgnoreLootRange.Checked += IgnoreLootRangeChecked;
			cbIgnoreLootRange.Unchecked += IgnoreLootRangeChecked;
			spIgnoreProfileValues.Children.Add(cbIgnoreLootRange);
			ProfileRelatedSettings.Children.Add(spIgnoreProfileValues);
			CheckBox cbIgnoreProfileBlacklist = new CheckBox
			{
				Content = "Ignore Profile Blacklisted IDs",
				// Width = 300,
				Height = 30,
				HorizontalContentAlignment = HorizontalAlignment.Right,
				IsChecked = (Bot.Settings.Ranges.IgnoreProfileBlacklists)
			};
			cbIgnoreProfileBlacklist.Checked += IgnoreProfileBlacklistChecked;
			cbIgnoreProfileBlacklist.Unchecked += IgnoreProfileBlacklistChecked;
			ProfileRelatedSettings.Children.Add(cbIgnoreProfileBlacklist);
			lbTargetRange.Items.Add(ProfileRelatedSettings);


			TextBlock Target_Range_Text = new TextBlock
			{
				Text = "Targeting Extended Range Values",
				FontSize = 13,
				Background = Brushes.DarkSeaGreen,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};
			lbTargetRange.Items.Add(Target_Range_Text);

			#region EliteRange
			TextBlock TextBlock_EliteRange = new TextBlock
			{
				Text = "Elite Combat Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderEliteRange = new Slider
			{
				Width = 100,
				Maximum = 150,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.EliteCombatRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderEliteRange.ValueChanged += EliteRangeSliderChanged;
			TBEliteRange = new TextBox
			{
				Text = Bot.Settings.Ranges.EliteCombatRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel EliteRangeControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			EliteRangeControlStackPanel.Children.Add(sliderEliteRange);
			EliteRangeControlStackPanel.Children.Add(TBEliteRange);

			StackPanel EliteRangeStackPanel = new StackPanel
			{
				Height = 40,
				Orientation = Orientation.Vertical,
				Margin = new Thickness(Margin.Left,Margin.Top,Margin.Right+15,Margin.Bottom),
			};
			EliteRangeStackPanel.Children.Add(TextBlock_EliteRange);
			EliteRangeStackPanel.Children.Add(EliteRangeControlStackPanel);

			
			#endregion

			#region NonEliteRange
			TextBlock TextBlock_NonEliteRange = new TextBlock
			{
				Text = "Non-Elite Combat Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};

			Slider sliderNonEliteRange = new Slider
			{
				Width = 100,
				Maximum = 150,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.NonEliteCombatRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderNonEliteRange.ValueChanged += NonEliteRangeSliderChanged;
			TBNonEliteRange = new TextBox
			{
				Text = Bot.Settings.Ranges.NonEliteCombatRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel NonEliteControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			NonEliteControlStackPanel.Children.Add(sliderNonEliteRange);
			NonEliteControlStackPanel.Children.Add(TBNonEliteRange);

			StackPanel NonEliteRangeStackPanel = new StackPanel
			{
				Height = 40,
				Orientation = Orientation.Vertical,
			};
			NonEliteRangeStackPanel.Children.Add(TextBlock_NonEliteRange);
			NonEliteRangeStackPanel.Children.Add(NonEliteControlStackPanel);

			
			#endregion

			StackPanel MonsterRangeStackPanel = new StackPanel
			{
				Width=600,
				Height = 40,
				Orientation = Orientation.Horizontal,
			};
			MonsterRangeStackPanel.Children.Add(EliteRangeStackPanel);
			MonsterRangeStackPanel.Children.Add(NonEliteRangeStackPanel);
			lbTargetRange.Items.Add(MonsterRangeStackPanel);


			#region ShrineRange
			TextBlock TextBlock_ShrineRange = new TextBlock
			{
				Text = "Shrine Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderShrineRange = new Slider
			{
				Width = 100,
				Maximum = 75,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.ShrineRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderShrineRange.ValueChanged += ShrineRangeSliderChanged;
			TBShrineRange = new TextBox
			{
				Text = Bot.Settings.Ranges.ShrineRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel ShrineControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			ShrineControlStackPanel.Children.Add(sliderShrineRange);
			ShrineControlStackPanel.Children.Add(TBShrineRange);

			StackPanel ShrineRangeStackPanel = new StackPanel
			{
				Height = 40,
				Orientation = Orientation.Vertical,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 15, Margin.Bottom),
			};
			ShrineRangeStackPanel.Children.Add(TextBlock_ShrineRange);
			ShrineRangeStackPanel.Children.Add(ShrineControlStackPanel);


			//
			
			#endregion

			#region CursedShrineRange
			TextBlock TextBlock_CursedShrineRange = new TextBlock
			{
				Text = "Cursed Shrine Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderCursedShrineRange = new Slider
			{
				Width = 100,
				Maximum = 150,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.CursedShrineRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderCursedShrineRange.ValueChanged += CursedShrineRangeSliderChanged;
			TBCursedShrineRange = new TextBox
			{
				Text = Bot.Settings.Ranges.CursedShrineRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel CursedShrineRangeControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			CursedShrineRangeControlStackPanel.Children.Add(sliderCursedShrineRange);
			CursedShrineRangeControlStackPanel.Children.Add(TBCursedShrineRange);

			StackPanel CursedShrineRangeStackPanel = new StackPanel
			{
				Height = 40,
				Orientation = Orientation.Vertical,
			};
			CursedShrineRangeStackPanel.Children.Add(TextBlock_CursedShrineRange);
			CursedShrineRangeStackPanel.Children.Add(CursedShrineRangeControlStackPanel);


			//
			
			#endregion

			StackPanel ShrinesRangeStackPanel = new StackPanel
			{
				Width = 600,
				Height = 40,
				Orientation = Orientation.Horizontal,
			};
			ShrinesRangeStackPanel.Children.Add(ShrineRangeStackPanel);
			ShrinesRangeStackPanel.Children.Add(CursedShrineRangeStackPanel);
			lbTargetRange.Items.Add(ShrinesRangeStackPanel);


			#region GoldRange
			TextBlock TextBlock_GoldRange = new TextBlock
			{
				Text = "Gold Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderGoldRange = new Slider
			{
				Width = 100,
				Maximum = 150,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.GoldRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGoldRange.ValueChanged += GoldRangeSliderChanged;
			TBGoldRange = new TextBox
			{
				Text = Bot.Settings.Ranges.GoldRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel GoldRangeControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			GoldRangeControlStackPanel.Children.Add(sliderGoldRange);
			GoldRangeControlStackPanel.Children.Add(TBGoldRange);

			StackPanel GoldRangeStackPanel = new StackPanel
			{
				Height = 40,
				Orientation = Orientation.Vertical,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 15, Margin.Bottom),
			};
			GoldRangeStackPanel.Children.Add(TextBlock_GoldRange);
			GoldRangeStackPanel.Children.Add(GoldRangeControlStackPanel);

			#endregion

			#region GlobeRange
			TextBlock TextBlock_GlobeRange = new TextBlock
			{
				Text = "Globe Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderGlobeRange = new Slider
			{
				Width = 100,
				Maximum = 75,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.GlobeRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGlobeRange.ValueChanged += GlobeRangeSliderChanged;
			TBGlobeRange = new TextBox
			{
				Text = Bot.Settings.Ranges.GlobeRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel GlobeRangeControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			GlobeRangeControlStackPanel.Children.Add(sliderGlobeRange);
			GlobeRangeControlStackPanel.Children.Add(TBGlobeRange);

			StackPanel GlobeRangeStackPanel = new StackPanel
			{
				Height = 40,
				Orientation = Orientation.Vertical,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 15, Margin.Bottom),
			};
			GlobeRangeStackPanel.Children.Add(TextBlock_GlobeRange);
			GlobeRangeStackPanel.Children.Add(GlobeRangeControlStackPanel);


			//TextBlock_GlobeRange
			
			#endregion

			#region PotionRange
			TextBlock TextBlock_PotionRange = new TextBlock
			{
				Text = "Potion Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderPotionRange = new Slider
			{
				Width = 100,
				Maximum = 75,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.PotionRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderPotionRange.ValueChanged += PotionRangeSliderChanged;
			TBPotionRange = new TextBox
			{
				Text = Bot.Settings.Ranges.PotionRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel PotionRangeControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			PotionRangeControlStackPanel.Children.Add(sliderPotionRange);
			PotionRangeControlStackPanel.Children.Add(TBPotionRange);

			StackPanel PotionRangeStackPanel = new StackPanel
			{
				Height = 40,
				Orientation = Orientation.Vertical,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 15, Margin.Bottom),
			};
			PotionRangeStackPanel.Children.Add(TextBlock_PotionRange);
			PotionRangeStackPanel.Children.Add(PotionRangeControlStackPanel);


			//TextBlock_GlobeRange

			#endregion

			StackPanel PickupRangeStackPanel = new StackPanel
			{
				Width = 600,
				Height = 40,
				Orientation = Orientation.Horizontal,
			};
			PickupRangeStackPanel.Children.Add(GoldRangeStackPanel);
			PickupRangeStackPanel.Children.Add(GlobeRangeStackPanel);
			PickupRangeStackPanel.Children.Add(PotionRangeStackPanel);
			
			lbTargetRange.Items.Add(PickupRangeStackPanel);


			#region ContainerRange
			TextBlock TextBlock_ContainerRange = new TextBlock
			{
				Text = "Container Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderContainerRange = new Slider
			{
				Width = 100,
				Maximum = 75,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.ContainerOpenRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderContainerRange.ValueChanged += ContainerRangeSliderChanged;
			TBContainerRange = new TextBox
			{
				Text = Bot.Settings.Ranges.ContainerOpenRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel ContainerControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			ContainerControlStackPanel.Children.Add(sliderContainerRange);
			ContainerControlStackPanel.Children.Add(TBContainerRange);

			StackPanel ContainerRangeStackPanel = new StackPanel
			{
				Height = 40,
				Orientation = Orientation.Vertical,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 15, Margin.Bottom),
			};
			ContainerRangeStackPanel.Children.Add(TextBlock_ContainerRange);
			ContainerRangeStackPanel.Children.Add(ContainerControlStackPanel);

			//
			
			#endregion

			#region CursedChestRange
			TextBlock TextBlock_CursedChestRange = new TextBlock
			{
				Text = "Cursed Chest Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderCursedChestRange = new Slider
			{
				Width = 100,
				Maximum = 150,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.CursedChestRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderCursedChestRange.ValueChanged += CursedChestRangeSliderChanged;
			TBCursedChestRange = new TextBox
			{
				Text = Bot.Settings.Ranges.CursedChestRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel CursedChestRangeControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			CursedChestRangeControlStackPanel.Children.Add(sliderCursedChestRange);
			CursedChestRangeControlStackPanel.Children.Add(TBCursedChestRange);

			StackPanel CursedChestRangeStackPanel = new StackPanel
			{
				Height = 40,
				Orientation = Orientation.Vertical,
			};
			CursedChestRangeStackPanel.Children.Add(TextBlock_CursedChestRange);
			CursedChestRangeStackPanel.Children.Add(CursedChestRangeControlStackPanel);


			//

			#endregion

			StackPanel ContainersRangeStackPanel = new StackPanel
			{
				Width = 600,
				Height = 40,
				Orientation = Orientation.Horizontal,
			};
			ContainersRangeStackPanel.Children.Add(ContainerRangeStackPanel);
			ContainersRangeStackPanel.Children.Add(CursedChestRangeStackPanel);
			lbTargetRange.Items.Add(ContainersRangeStackPanel);

			#region DestructibleRange
			TextBlock TextBlock_DestuctibleRange = new TextBlock
			{
				Text = "Destuctible Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderDestructibleRange = new Slider
			{
				Width = 100,
				Maximum = 75,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.DestructibleRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderDestructibleRange.ValueChanged += DestructibleSliderChanged;
			TBDestructibleRange = new TextBox
			{
				Text = Bot.Settings.Ranges.DestructibleRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel DestructibleControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			DestructibleControlStackPanel.Children.Add(sliderDestructibleRange);
			DestructibleControlStackPanel.Children.Add(TBDestructibleRange);

			StackPanel DestructibleRangeStackPanel = new StackPanel
			{
				Width = 600,
				Height = 40,
				Orientation = Orientation.Vertical,
			};
			DestructibleRangeStackPanel.Children.Add(TextBlock_DestuctibleRange);
			DestructibleRangeStackPanel.Children.Add(DestructibleControlStackPanel);

			//
			lbTargetRange.Items.Add(DestructibleRangeStackPanel);
			#endregion

			#region ItemRange
			TextBlock TextBlock_ItemRange = new TextBlock
			{
				Text = "Item Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderItemRange = new Slider
			{
				Width = 100,
				Maximum = 150,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.ItemRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderItemRange.ValueChanged += ItemRangeSliderChanged;
			TBItemRange = new TextBox
			{
				Text = Bot.Settings.Ranges.ItemRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel ItemRangeControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			ItemRangeControlStackPanel.Children.Add(sliderItemRange);
			ItemRangeControlStackPanel.Children.Add(TBItemRange);

			StackPanel ItemRangeStackPanel = new StackPanel
			{
				Width = 600,
				Height = 40,
				Orientation = Orientation.Vertical,
			};
			ItemRangeStackPanel.Children.Add(TextBlock_ItemRange);
			ItemRangeStackPanel.Children.Add(ItemRangeControlStackPanel);


			//
			lbTargetRange.Items.Add(ItemRangeStackPanel);
			#endregion

			#region GoblinRange
			TextBlock TextBlock_GoblinRange = new TextBlock
			{
				Text = "Goblin Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderGoblinRange = new Slider
			{
				Width = 100,
				Maximum = 150,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.TreasureGoblinRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderGoblinRange.ValueChanged += TreasureGoblinRangeSliderChanged;
			TBGoblinRange = new TextBox
			{
				Text = Bot.Settings.Ranges.TreasureGoblinRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel GoblinRangeControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			GoblinRangeControlStackPanel.Children.Add(sliderGoblinRange);
			GoblinRangeControlStackPanel.Children.Add(TBGoblinRange);

			StackPanel GoblinRangeStackPanel = new StackPanel
			{
				Width = 600,
				Height = 40,
				Orientation = Orientation.Vertical,
			};
			GoblinRangeStackPanel.Children.Add(TextBlock_GoblinRange);
			GoblinRangeStackPanel.Children.Add(GoblinRangeControlStackPanel);


			//
			lbTargetRange.Items.Add(GoblinRangeStackPanel);
			#endregion

			#region PoolsOfReflectionRange
			TextBlock TextBlock_PoolsOfReflectionRange = new TextBlock
			{
				Text = "Pools Of Reflection Range",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			Slider sliderPoolsOfReflectionRange = new Slider
			{
				Width = 100,
				Maximum = 150,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Ranges.PoolsOfReflectionRange,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderPoolsOfReflectionRange.ValueChanged += PoolsOfReflectionRangeSliderChanged;
			TBPoolReflectionRange = new TextBox
			{
				Text = Bot.Settings.Ranges.PoolsOfReflectionRange.ToString(),
				IsReadOnly = true,
			};
			StackPanel PoolsOfReflectionRangeControlStackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			PoolsOfReflectionRangeControlStackPanel.Children.Add(sliderPoolsOfReflectionRange);
			PoolsOfReflectionRangeControlStackPanel.Children.Add(TBPoolReflectionRange);

			StackPanel PoolsOfReflectionRangeStackPanel = new StackPanel
			{
				Width = 600,
				Height = 40,
				Orientation = Orientation.Vertical,
			};
			PoolsOfReflectionRangeStackPanel.Children.Add(TextBlock_PoolsOfReflectionRange);
			PoolsOfReflectionRangeStackPanel.Children.Add(PoolsOfReflectionRangeControlStackPanel);


			//
			lbTargetRange.Items.Add(PoolsOfReflectionRangeStackPanel);
			#endregion

		

			Button BtnRangeTemplate = new Button
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
			BtnRangeTemplate.Click += RangeLoadXMLClicked;
			lbTargetRange.Items.Add(BtnRangeTemplate);

			RangeTabItem.Content = lbTargetRange;
			#endregion

		}
	}
}
