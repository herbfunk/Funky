using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using FunkyBot.Settings;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListBox = System.Windows.Controls.ListBox;
using Orientation = System.Windows.Controls.Orientation;
using TextBox = System.Windows.Controls.TextBox;

namespace FunkyBot
{
	public enum GemQuality
	{
		Chipped = 14,
		Flawed = 22,
		Normal = 30,
		Flawless = 36,
		Perfect = 42,
		Radiant = 48,
		Square = 54,
		FlawlessSquare = 60,
	}

	internal partial class FunkyWindow : Window
	{
		#region EventHandling
		private void PickUpLoadXMLClicked(object sender, EventArgs e)
		{
			OpenFileDialog OFD = new OpenFileDialog
			{
				InitialDirectory = Path.Combine(FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
				RestoreDirectory = false,
				Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
				Title = "Pickup Template",
			};
			DialogResult OFD_Result = OFD.ShowDialog();

			if (OFD_Result == System.Windows.Forms.DialogResult.OK)
			{
				try
				{
					//;
					SettingLoot newSettings = SettingLoot.DeserializeFromXML(OFD.FileName);
					Bot.Settings.Loot = newSettings;

					funkyConfigWindow.Close();
				}
				catch
				{

				}
			}


		}

		private void PickupCraftTomesChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupCraftTomes = !Bot.Settings.Loot.PickupCraftTomes;
		}
		private void PickupCraftMaterialsChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupCraftMaterials = !Bot.Settings.Loot.PickupCraftMaterials;
		}
		private void PickupCraftPlansChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupCraftPlans = !Bot.Settings.Loot.PickupCraftPlans;
			spBlacksmithPlans.IsEnabled = Bot.Settings.Loot.PickupCraftPlans;
			spJewelerPlans.IsEnabled = Bot.Settings.Loot.PickupCraftPlans;
		}

		private void PickupBlacksmithPlanSixChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupBlacksmithPlanSix = !Bot.Settings.Loot.PickupBlacksmithPlanSix;
		}
		private void PickupBlacksmithPlanFiveChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupBlacksmithPlanFive = !Bot.Settings.Loot.PickupBlacksmithPlanFive;
		}
		private void PickupBlacksmithPlanFourChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupBlacksmithPlanFour = !Bot.Settings.Loot.PickupBlacksmithPlanFour;
		}

		private void PickupBlacksmithPlanArchonGauntletsChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupBlacksmithPlanArchonGauntlets = !Bot.Settings.Loot.PickupBlacksmithPlanArchonGauntlets;
		}
		private void PickupBlacksmithPlanArchonSpauldersChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupBlacksmithPlanArchonSpaulders = !Bot.Settings.Loot.PickupBlacksmithPlanArchonSpaulders;
		}
		private void PickupBlacksmithPlanRazorspikesChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupBlacksmithPlanRazorspikes = !Bot.Settings.Loot.PickupBlacksmithPlanRazorspikes;
		}

		private void PickupJewelerDesignFlawlessStarChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupJewelerDesignFlawlessStar = !Bot.Settings.Loot.PickupJewelerDesignFlawlessStar;
		}
		private void PickupJewelerDesignPerfectStarChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupJewelerDesignPerfectStar = !Bot.Settings.Loot.PickupJewelerDesignPerfectStar;
		}
		private void PickupJewelerDesignRadiantStarChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupJewelerDesignRadiantStar = !Bot.Settings.Loot.PickupJewelerDesignRadiantStar;
		}
		private void PickupJewelerDesignMarquiseChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupJewelerDesignMarquise = !Bot.Settings.Loot.PickupJewelerDesignMarquise;
		}
		private void PickupJewelerDesignAmuletChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupJewelerDesignAmulet = !Bot.Settings.Loot.PickupJewelerDesignAmulet;
		}
		private void PickupInfernalKeysChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupInfernalKeys = !Bot.Settings.Loot.PickupInfernalKeys;
		}
		private void PickupDemonicEssenceChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupDemonicEssence = !Bot.Settings.Loot.PickupDemonicEssence;
		}

		private void PickupFollowerItemsChecked(object sender, EventArgs e)
		{
			Bot.Settings.Loot.PickupFollowerItems = !Bot.Settings.Loot.PickupFollowerItems;
		}
		private void GemsChecked(object sender, EventArgs e)
		{
			CheckBox sender_ = (CheckBox)sender;
			if (sender_.Name == "red") Bot.Settings.Loot.PickupGems[0] = !Bot.Settings.Loot.PickupGems[0];
			if (sender_.Name == "green") Bot.Settings.Loot.PickupGems[1] = !Bot.Settings.Loot.PickupGems[1];
			if (sender_.Name == "purple") Bot.Settings.Loot.PickupGems[2] = !Bot.Settings.Loot.PickupGems[2];
			if (sender_.Name == "yellow") Bot.Settings.Loot.PickupGems[3] = !Bot.Settings.Loot.PickupGems[3];
			if (sender_.Name == "white") Bot.Settings.Loot.PickupGemDiamond = !Bot.Settings.Loot.PickupGemDiamond;
		}
		private void MiscItemLevelSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Loot.MiscItemLevel = Value;
			TBMiscItemLevel.Text = Value.ToString();
		}
		private void GemQualityLevelChanged(object sender, EventArgs e)
		{
			ComboBox cbSender = (ComboBox)sender;

			Bot.Settings.Loot.MinimumGemItemLevel = (int)Enum.Parse(typeof(GemQuality), cbSender.Items[cbSender.SelectedIndex].ToString());
		}
		class GemQualityTypes : ObservableCollection<string>
		{
			public GemQualityTypes()
			{
				string[] GemNames = Enum.GetNames(typeof(GemQuality));
				foreach (var item in GemNames)
				{
					Add(item);
				}
			}
		}
		private void LegendaryItemLevelSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Loot.MinimumLegendaryItemLevel = Value;
			TBMinLegendaryLevel.Text = Value.ToString();
		}
		private void HealthPotionSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Loot.MaximumHealthPotions = Value;
			TBMaxHealthPots.Text = Value.ToString();
		}
		private void GoldAmountSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			Bot.Settings.Loot.MinimumGoldPile = Value;
			TBMinGoldPile.Text = Value.ToString();
		}

		private void WeaponItemLevelSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			if (slider_sender.Name == "Magic")
			{
				Bot.Settings.Loot.MinimumWeaponItemLevel[0] = Value;
				TBMinimumWeaponLevel[0].Text = Value.ToString();
			}
			else
			{
				Bot.Settings.Loot.MinimumWeaponItemLevel[1] = Value;
				TBMinimumWeaponLevel[1].Text = Value.ToString();
			}
		}
		private void ArmorItemLevelSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			if (slider_sender.Name == "Magic")
			{
				Bot.Settings.Loot.MinimumArmorItemLevel[0] = Value;
				TBMinimumArmorLevel[0].Text = Value.ToString();
			}
			else
			{
				Bot.Settings.Loot.MinimumArmorItemLevel[1] = Value;
				TBMinimumArmorLevel[1].Text = Value.ToString();
			}
		}
		private void JeweleryItemLevelSliderChanged(object sender, EventArgs e)
		{
			Slider slider_sender = (Slider)sender;
			int Value = (int)slider_sender.Value;
			if (slider_sender.Name == "Magic")
			{
				Bot.Settings.Loot.MinimumJeweleryItemLevel[0] = Value;
				TBMinimumJeweleryLevel[0].Text = Value.ToString();
			}
			else
			{
				Bot.Settings.Loot.MinimumJeweleryItemLevel[1] = Value;
				TBMinimumJeweleryLevel[1].Text = Value.ToString();
			}
		}

		#endregion

		private StackPanel spBlacksmithPlans, spJewelerPlans;
		private TextBox[] TBMinimumWeaponLevel, TBMinimumJeweleryLevel, TBMinimumArmorLevel;
		private TextBox TBMinLegendaryLevel, TBMaxHealthPots, TBMinGoldPile, TBMiscItemLevel;
		private CheckBox[] CBGems;

		internal void InitLootPickUpControls()
		{

			#region Pickup
			TabItem ItemGilesTabItem = new TabItem();
			ItemGilesTabItem.Header = "Pickup";
			tcItems.Items.Add(ItemGilesTabItem);
			ListBox lbGilesContent = new ListBox();

			#region Item Level Pickup
			StackPanel spItemPickupLevel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			#region minimumWeaponILevel
			StackPanel spWeaponPickupLevel = new StackPanel
			{
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};

			TextBlock txt_weaponIlvl = new TextBlock
			{
				Text = "Weapons",
				FontSize = 12,
				Background = Brushes.DarkSlateGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spWeaponPickupLevel.Children.Add(txt_weaponIlvl);

			StackPanel spWeaponPickupMagical = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			TextBlock txt_weaponMagical = new TextBlock
			{
				Text = "Magic",
				FontSize = 12,
				Background = Brushes.DarkBlue,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			TBMinimumWeaponLevel = new TextBox[2];
			Slider weaponMagicLevelSlider = new Slider
			{
				Name = "Magic",
				Width = 120,
				Maximum = 63,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Loot.MinimumWeaponItemLevel[0],
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			weaponMagicLevelSlider.ValueChanged += WeaponItemLevelSliderChanged;
			TBMinimumWeaponLevel[0] = new TextBox
			{
				Text = Bot.Settings.Loot.MinimumWeaponItemLevel[0].ToString(),
				IsReadOnly = true,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spWeaponPickupMagical.Children.Add(txt_weaponMagical);
			spWeaponPickupMagical.Children.Add(weaponMagicLevelSlider);
			spWeaponPickupMagical.Children.Add(TBMinimumWeaponLevel[0]);
			spWeaponPickupLevel.Children.Add(spWeaponPickupMagical);


			StackPanel spWeaponPickupRare = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			TextBlock txt_weaponRare = new TextBlock
			{
				Text = "Rare",
				FontSize = 12,
				Foreground = Brushes.Black,
				Background = Brushes.Gold,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			Slider weaponRareLevelSlider = new Slider
			{
				Name = "Rare",
				Width = 120,
				Maximum = 63,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Loot.MinimumWeaponItemLevel[1],
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			weaponRareLevelSlider.ValueChanged += WeaponItemLevelSliderChanged;
			TBMinimumWeaponLevel[1] = new TextBox
			{
				Text = Bot.Settings.Loot.MinimumWeaponItemLevel[1].ToString(),
				IsReadOnly = true,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spWeaponPickupRare.Children.Add(txt_weaponRare);
			spWeaponPickupRare.Children.Add(weaponRareLevelSlider);
			spWeaponPickupRare.Children.Add(TBMinimumWeaponLevel[1]);
			spWeaponPickupLevel.Children.Add(spWeaponPickupRare);

			spItemPickupLevel.Children.Add(spWeaponPickupLevel);
			#endregion
			#region minimumArmorILevel
			StackPanel spArmorPickupLevel = new StackPanel
			{
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};

			TBMinimumArmorLevel = new TextBox[2];
			TextBlock txt_armorIlvl = new TextBlock
			{
				Text = "Armor",
				FontSize = 12,
				Background = Brushes.DarkSlateGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spArmorPickupLevel.Children.Add(txt_armorIlvl);

			StackPanel spArmorPickupMagical = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			TextBlock txt_armorMagic = new TextBlock
			{
				Text = "Magic",
				FontSize = 12,
				Background = Brushes.DarkBlue,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			Slider armorMagicLevelSlider = new Slider
			{
				Name = "Magic",
				Width = 120,
				Maximum = 63,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Loot.MinimumArmorItemLevel[0],
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			armorMagicLevelSlider.ValueChanged += ArmorItemLevelSliderChanged;
			TBMinimumArmorLevel[0] = new TextBox
			{
				Text = Bot.Settings.Loot.MinimumArmorItemLevel[0].ToString(),
				IsReadOnly = true,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spArmorPickupMagical.Children.Add(txt_armorMagic);
			spArmorPickupMagical.Children.Add(armorMagicLevelSlider);
			spArmorPickupMagical.Children.Add(TBMinimumArmorLevel[0]);
			spArmorPickupLevel.Children.Add(spArmorPickupMagical);


			StackPanel spArmorPickupRare = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			TextBlock txt_armorRare = new TextBlock
			{
				Text = "Rare",
				FontSize = 12,
				Foreground = Brushes.Black,
				Background = Brushes.Gold,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			Slider armorRareLevelSlider = new Slider
			{
				Name = "Rare",
				Maximum = 63,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Width = 120,
				Value = Bot.Settings.Loot.MinimumArmorItemLevel[1],
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			armorRareLevelSlider.ValueChanged += ArmorItemLevelSliderChanged;
			TBMinimumArmorLevel[1] = new TextBox
			{
				Text = Bot.Settings.Loot.MinimumArmorItemLevel[1].ToString(),
				IsReadOnly = true,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spArmorPickupRare.Children.Add(txt_armorRare);
			spArmorPickupRare.Children.Add(armorRareLevelSlider);
			spArmorPickupRare.Children.Add(TBMinimumArmorLevel[1]);
			spArmorPickupLevel.Children.Add(spArmorPickupRare);

			spItemPickupLevel.Children.Add(spArmorPickupLevel);
			#endregion
			#region minimumJeweleryILevel
			TBMinimumJeweleryLevel = new TextBox[2];
			TextBlock txt_jeweleryIlvl = new TextBlock
			{
				Text = "Jewelery",
				FontSize = 12,
				Background = Brushes.DarkSlateGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			StackPanel spJeweleryPickupLevel = new StackPanel
			{

			};
			spJeweleryPickupLevel.Children.Add(txt_jeweleryIlvl);

			StackPanel spJeweleryPickupMagical = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			TextBlock txt_jeweleryMagic = new TextBlock
			{
				Text = "Magic",
				FontSize = 12,
				Background = Brushes.DarkBlue,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			Slider jeweleryMagicLevelSlider = new Slider
			{
				Name = "Magic",
				Width = 120,
				Maximum = 63,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Loot.MinimumJeweleryItemLevel[0],
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			jeweleryMagicLevelSlider.ValueChanged += JeweleryItemLevelSliderChanged;
			TBMinimumJeweleryLevel[0] = new TextBox
			{
				Text = Bot.Settings.Loot.MinimumJeweleryItemLevel[0].ToString(),
				IsReadOnly = true,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spJeweleryPickupMagical.Children.Add(txt_jeweleryMagic);
			spJeweleryPickupMagical.Children.Add(jeweleryMagicLevelSlider);
			spJeweleryPickupMagical.Children.Add(TBMinimumJeweleryLevel[0]);
			spJeweleryPickupLevel.Children.Add(spJeweleryPickupMagical);

			StackPanel spJeweleryPickupRare = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			TextBlock txt_jeweleryRare = new TextBlock
			{
				Text = "Rare",
				FontSize = 12,
				Foreground = Brushes.Black,
				Background = Brushes.Gold,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			Slider jeweleryRareLevelSlider = new Slider
			{
				Name = "Rare",
				Maximum = 63,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Width = 120,
				Value = Bot.Settings.Loot.MinimumJeweleryItemLevel[1],
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			jeweleryRareLevelSlider.ValueChanged += JeweleryItemLevelSliderChanged;
			TBMinimumJeweleryLevel[1] = new TextBox
			{
				Text = Bot.Settings.Loot.MinimumJeweleryItemLevel[1].ToString(),
				IsReadOnly = true,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spJeweleryPickupRare.Children.Add(txt_jeweleryRare);
			spJeweleryPickupRare.Children.Add(jeweleryRareLevelSlider);
			spJeweleryPickupRare.Children.Add(TBMinimumJeweleryLevel[1]);
			spJeweleryPickupLevel.Children.Add(spJeweleryPickupRare);
			spItemPickupLevel.Children.Add(spJeweleryPickupLevel);
			#endregion

			StackPanel spItemPickup = new StackPanel
			{
				Background = Brushes.DimGray,
			};
			TextBlock Text_Header_ItemPickup = new TextBlock
			{
				Text = "Item Level Pickup",
				FontSize = 14,
				Background = Brushes.DimGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				TextAlignment = TextAlignment.Center,
			};
			spItemPickup.Children.Add(Text_Header_ItemPickup);
			spItemPickup.Children.Add(spItemPickupLevel);

			#region LegendaryLevel
			TextBlock Text_Legendary_ItemLevel = new TextBlock
			{
				Text = "Legendary Items",
				FontSize = 12,
				//Background=System.Windows.Media.Brushes.DarkGreen,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				TextAlignment = TextAlignment.Left,
			};
			spItemPickup.Children.Add(Text_Legendary_ItemLevel);
			Slider sliderLegendaryILevel = new Slider
			{
				Width = 120,
				Maximum = 63,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Loot.MinimumLegendaryItemLevel,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderLegendaryILevel.ValueChanged += LegendaryItemLevelSliderChanged;
			TBMinLegendaryLevel = new TextBox
			{
				Text = Bot.Settings.Loot.MinimumLegendaryItemLevel.ToString(),
				IsReadOnly = true,
			};
			StackPanel LegendaryILvlStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			LegendaryILvlStackPanel.Children.Add(sliderLegendaryILevel);
			LegendaryILvlStackPanel.Children.Add(TBMinLegendaryLevel);

			spItemPickup.Children.Add(LegendaryILvlStackPanel);
			#endregion


			#region MinMiscItemLevel
			TextBlock Text_Misc_ItemLevel = new TextBlock
			{
				Text = "Misc Item",
				FontSize = 12,
				//Background=System.Windows.Media.Brushes.DarkGreen,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom + 5),
				TextAlignment = TextAlignment.Left,
			};
			spItemPickup.Children.Add(Text_Misc_ItemLevel);
			Slider slideMinMiscItemLevel = new Slider
			{
				Width = 100,
				Maximum = 63,
				Minimum = 0,
				TickFrequency = 5,
				LargeChange = 5,
				SmallChange = 1,
				Value = Bot.Settings.Loot.MiscItemLevel,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			slideMinMiscItemLevel.ValueChanged += MiscItemLevelSliderChanged;
			TBMiscItemLevel = new TextBox
			{
				Text = Bot.Settings.Loot.MiscItemLevel.ToString(),
				IsReadOnly = true,
			};
			StackPanel MinMiscItemLevelStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			MinMiscItemLevelStackPanel.Children.Add(slideMinMiscItemLevel);
			MinMiscItemLevelStackPanel.Children.Add(TBMiscItemLevel);
			spItemPickup.Children.Add(MinMiscItemLevelStackPanel);
			#endregion


			lbGilesContent.Items.Add(spItemPickup);
			#endregion



			#region MaxHealthPotions
			Slider sliderMaxHealthPots = new Slider
			{
				Width = 100,
				Maximum = 100,
				Minimum = 0,
				TickFrequency = 25,
				LargeChange = 20,
				SmallChange = 5,
				Value = Bot.Settings.Loot.MaximumHealthPotions,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			sliderMaxHealthPots.ValueChanged += HealthPotionSliderChanged;
			TBMaxHealthPots = new TextBox
			{
				Text = Bot.Settings.Loot.MaximumHealthPotions.ToString(),
				IsReadOnly = true,
			};
			StackPanel MaxHealthPotsStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			MaxHealthPotsStackPanel.Children.Add(sliderMaxHealthPots);
			MaxHealthPotsStackPanel.Children.Add(TBMaxHealthPots);
			StackPanel spHealthPotions = new StackPanel();

			TextBlock txt_HealthPotions = new TextBlock
			{
				Text = "Health Potions",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spHealthPotions.Children.Add(txt_HealthPotions);
			spHealthPotions.Children.Add(MaxHealthPotsStackPanel);
			#endregion

			#region MinimumGoldPile
			Slider slideMinGoldPile = new Slider
			{
				Width = 120,
				Maximum = 7500,
				Minimum = 0,
				TickFrequency = 1000,
				LargeChange = 1000,
				SmallChange = 1,
				Value = Bot.Settings.Loot.MinimumGoldPile,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
			slideMinGoldPile.ValueChanged += GoldAmountSliderChanged;
			TBMinGoldPile = new TextBox
			{
				Text = Bot.Settings.Loot.MinimumGoldPile.ToString(),
				IsReadOnly = true,
			};
			StackPanel MinGoldPileStackPanel = new StackPanel
			{
				Width = 600,
				Height = 20,
				Orientation = Orientation.Horizontal,
			};
			MinGoldPileStackPanel.Children.Add(slideMinGoldPile);
			MinGoldPileStackPanel.Children.Add(TBMinGoldPile);
			StackPanel spMinimumGold = new StackPanel();

			TextBlock txt_MinimumGold = new TextBlock
			{
				Text = "Minimum Gold Pile",
				FontSize = 12,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spMinimumGold.Children.Add(txt_MinimumGold);
			spMinimumGold.Children.Add(MinGoldPileStackPanel);
			#endregion


			StackPanel spCraftPlans = new StackPanel
			{
				Background = Brushes.DimGray,
			};
			TextBlock txt_CraftPlansPickup = new TextBlock
			{
				Text = "Craft Plan Options",
				FontSize = 12,
				Background = Brushes.DarkSlateGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			spCraftPlans.Children.Add(txt_CraftPlansPickup);

			#region PickupCraftPlans
			CheckBox cbPickupCraftPlans = new CheckBox
			{
				Content = "Pickup Craft Plans",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupCraftPlans),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbPickupCraftPlans.Checked += PickupCraftPlansChecked;
			cbPickupCraftPlans.Unchecked += PickupCraftPlansChecked;
			spCraftPlans.Children.Add(cbPickupCraftPlans);
			#endregion

			spBlacksmithPlans = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				IsEnabled = Bot.Settings.Loot.PickupCraftPlans,
			};
			StackPanel spBlacksmithPlansProperties = new StackPanel();

			#region Blacksmith_property_six
			CheckBox cbBlacksmithPropertySix = new CheckBox
			{
				Content = "Plans: Property Six",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupBlacksmithPlanSix),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbBlacksmithPropertySix.Checked += PickupBlacksmithPlanSixChecked;
			cbBlacksmithPropertySix.Unchecked += PickupBlacksmithPlanSixChecked;
			spBlacksmithPlansProperties.Children.Add(cbBlacksmithPropertySix);
			#endregion

			#region Blacksmith_property_five
			CheckBox cbBlacksmithPropertyFive = new CheckBox
			{
				Content = "Plans: Property Five",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupBlacksmithPlanFive),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbBlacksmithPropertyFive.Checked += PickupBlacksmithPlanFiveChecked;
			cbBlacksmithPropertyFive.Unchecked += PickupBlacksmithPlanFiveChecked;
			spBlacksmithPlansProperties.Children.Add(cbBlacksmithPropertyFive);
			#endregion

			#region Blacksmith_property_four
			CheckBox cbBlacksmithPropertyFour = new CheckBox
			{
				Content = "Plans: Property Four",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupBlacksmithPlanFour),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbBlacksmithPropertyFive.Checked += PickupBlacksmithPlanFourChecked;
			cbBlacksmithPropertyFive.Unchecked += PickupBlacksmithPlanFourChecked;
			spBlacksmithPlansProperties.Children.Add(cbBlacksmithPropertyFour);
			#endregion

			spBlacksmithPlans.Children.Add(spBlacksmithPlansProperties);

			StackPanel spBlacksmithPlansArchonRazor = new StackPanel();

			#region Blacksmith_archon_gauntlets
			CheckBox cbBlacksmithArchonGauntlets = new CheckBox
			{
				Content = "Plans: Archon Gauntlets",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupBlacksmithPlanArchonGauntlets),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbBlacksmithArchonGauntlets.Checked += PickupBlacksmithPlanArchonGauntletsChecked;
			cbBlacksmithArchonGauntlets.Unchecked += PickupBlacksmithPlanArchonGauntletsChecked;
			spBlacksmithPlansArchonRazor.Children.Add(cbBlacksmithArchonGauntlets);
			#endregion

			#region Blacksmith_archon_spaulders
			CheckBox cbBlacksmithArchonSpaulders = new CheckBox
			{
				Content = "Plans: Archon Spaulders",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupBlacksmithPlanArchonSpaulders),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbBlacksmithArchonSpaulders.Checked += PickupBlacksmithPlanArchonSpauldersChecked;
			cbBlacksmithArchonSpaulders.Unchecked += PickupBlacksmithPlanArchonSpauldersChecked;
			spBlacksmithPlansArchonRazor.Children.Add(cbBlacksmithArchonSpaulders);
			#endregion

			#region Blacksmith_archon_razorspikes
			CheckBox cbBlacksmithRazorSpikes = new CheckBox
			{
				Content = "Plans: Razorspikes",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupBlacksmithPlanRazorspikes),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbBlacksmithRazorSpikes.Checked += PickupBlacksmithPlanRazorspikesChecked;
			cbBlacksmithRazorSpikes.Unchecked += PickupBlacksmithPlanRazorspikesChecked;
			spBlacksmithPlansArchonRazor.Children.Add(cbBlacksmithRazorSpikes);
			#endregion

			spBlacksmithPlans.Children.Add(spBlacksmithPlansArchonRazor);
			spCraftPlans.Children.Add(spBlacksmithPlans);

			spJewelerPlans = new StackPanel
			{
				IsEnabled = Bot.Settings.Loot.PickupCraftPlans,
			};

			#region design_flawlessStar
			CheckBox cbJewelerFlawlessStar = new CheckBox
			{
				Content = "Design: Flawless Star",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupJewelerDesignFlawlessStar),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbJewelerFlawlessStar.Checked += PickupJewelerDesignFlawlessStarChecked;
			cbJewelerFlawlessStar.Unchecked += PickupJewelerDesignFlawlessStarChecked;
			spJewelerPlans.Children.Add(cbJewelerFlawlessStar);
			#endregion

			#region design_perfectstar
			CheckBox cbJewelerPerfectStar = new CheckBox
			{
				Content = "Design: Perfect Star",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupJewelerDesignPerfectStar),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbJewelerPerfectStar.Checked += PickupJewelerDesignPerfectStarChecked;
			cbJewelerPerfectStar.Unchecked += PickupJewelerDesignPerfectStarChecked;
			spJewelerPlans.Children.Add(cbJewelerPerfectStar);
			#endregion

			#region design_radiantstar
			CheckBox cbJewelerRadiantStar = new CheckBox
			{
				Content = "Design: Radiant Star",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupJewelerDesignRadiantStar),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbJewelerRadiantStar.Checked += PickupJewelerDesignRadiantStarChecked;
			cbJewelerRadiantStar.Unchecked += PickupJewelerDesignRadiantStarChecked;
			spJewelerPlans.Children.Add(cbJewelerRadiantStar);
			#endregion

			#region design_marquise
			CheckBox cbJewelerMarquise = new CheckBox
			{
				Content = "Design: Marquise",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupJewelerDesignMarquise),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbJewelerMarquise.Checked += PickupJewelerDesignMarquiseChecked;
			cbJewelerMarquise.Unchecked += PickupJewelerDesignMarquiseChecked;
			spJewelerPlans.Children.Add(cbJewelerMarquise);
			#endregion

			#region design_amulets
			CheckBox cbJewelerAmulets = new CheckBox
			{
				Content = "Design: Amulets",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupJewelerDesignAmulet),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbJewelerAmulets.Checked += PickupJewelerDesignAmuletChecked;
			cbJewelerAmulets.Unchecked += PickupJewelerDesignAmuletChecked;
			spJewelerPlans.Children.Add(cbJewelerAmulets);
			#endregion

			spCraftPlans.Children.Add(spJewelerPlans);

			lbGilesContent.Items.Add(spCraftPlans);


			StackPanel spMiscItemPickupOptions = new StackPanel
			{
				Orientation = Orientation.Vertical,
			};
			#region PickupCraftMaterials
			CheckBox cbPickupCraftMaterials = new CheckBox
			{
				Content = "Pickup Craft Materials",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupCraftMaterials),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbPickupCraftMaterials.Checked += PickupCraftMaterialsChecked;
			cbPickupCraftMaterials.Unchecked += PickupCraftMaterialsChecked;
			spMiscItemPickupOptions.Children.Add(cbPickupCraftMaterials);
			#endregion
			#region PickupCraftTomes
			CheckBox cbPickupCraftTomes = new CheckBox
			{
				Content = "Pickup Craft Tomes",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupCraftTomes),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbPickupCraftTomes.Checked += PickupCraftTomesChecked;
			cbPickupCraftTomes.Unchecked += PickupCraftTomesChecked;
			spMiscItemPickupOptions.Children.Add(cbPickupCraftTomes);
			#endregion
			#region PickupFollowerItems
			CheckBox cbPickupFollowerItems = new CheckBox
			{
				Content = "Pickup Follower Items",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupFollowerItems),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbPickupFollowerItems.Checked += PickupFollowerItemsChecked;
			cbPickupFollowerItems.Unchecked += PickupFollowerItemsChecked;
			spMiscItemPickupOptions.Children.Add(cbPickupFollowerItems);
			#endregion
			#region PickupInfernalKeys
			CheckBox cbPickupInfernalKeys = new CheckBox
			{
				Content = "Pickup Inferno Keys",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupInfernalKeys),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbPickupInfernalKeys.Checked += PickupInfernalKeysChecked;
			cbPickupInfernalKeys.Unchecked += PickupInfernalKeysChecked;
			spMiscItemPickupOptions.Children.Add(cbPickupInfernalKeys);
			#endregion
			#region PickupDemonicEssence
			CheckBox cbPickupDemonicEssence = new CheckBox
			{
				Content = "Pickup Demonic Essences",
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupDemonicEssence),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 5, Margin.Bottom),
			};
			cbPickupDemonicEssence.Checked += PickupDemonicEssenceChecked;
			cbPickupDemonicEssence.Unchecked += PickupDemonicEssenceChecked;
			spMiscItemPickupOptions.Children.Add(cbPickupDemonicEssence);
			#endregion
			//
			TextBlock txt_miscPickup = new TextBlock
			{
				Text = "Misc Pickup Options",
				FontSize = 12,
				Background = Brushes.DarkSlateGray,
				Foreground = Brushes.GhostWhite,
				Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + 4, Margin.Bottom + 4),
			};
			StackPanel spMiscItemPickup = new StackPanel
			{
				Background = Brushes.DimGray,
			};
			spMiscItemPickup.Children.Add(txt_miscPickup);
			spMiscItemPickup.Children.Add(spMiscItemPickupOptions);
			spMiscItemPickup.Children.Add(spMinimumGold);
			spMiscItemPickup.Children.Add(spHealthPotions);

			lbGilesContent.Items.Add(spMiscItemPickup);


			#region Gems
			StackPanel spGemOptions = new StackPanel
			{
				Orientation = Orientation.Vertical,
				Background = Brushes.DimGray,
			};
			TextBlock Text_GemOptions = new TextBlock
			{
				Text = "Gems",
				FontSize = 12,
				Foreground = Brushes.Black,
				Background = Brushes.Gold,
				TextAlignment = TextAlignment.Left,
			};
			spGemOptions.Children.Add(Text_GemOptions);

			#region GemQuality
			TextBlock Text_MinimumGemQuality = new TextBlock
			{
				Text = "Minimum Gem Quality",
				FontSize = 11,
				Foreground = Brushes.GhostWhite,
				TextAlignment = TextAlignment.Left,
			};
			ComboBox CBGemQualityLevel = new ComboBox
			{
				Height = 20,
				ItemsSource = new GemQualityTypes(),
				Text = Enum.GetName(typeof(GemQuality), Bot.Settings.Loot.MinimumGemItemLevel).ToString(),
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(5),
			};
			CBGemQualityLevel.SelectionChanged += GemQualityLevelChanged;
			spGemOptions.Children.Add(Text_MinimumGemQuality);
			spGemOptions.Children.Add(CBGemQualityLevel);
			#endregion

			CBGems = new CheckBox[5];

			StackPanel spGemColorsREDGREEN = new StackPanel
			{
				Orientation = Orientation.Vertical,
			};
			#region PickupGemsRed
			CBGems[0] = new CheckBox
			{
				Content = "Pickup Gem Red",
				Name = "red",
				Width = 150,
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupGems[0])
			};
			CBGems[0].Checked += GemsChecked;
			CBGems[0].Unchecked += GemsChecked;
			spGemColorsREDGREEN.Children.Add(CBGems[0]);
			#endregion
			#region PickupGemsGreen
			CBGems[1] = new CheckBox
			{
				Content = "Pickup Gem Green",
				Name = "green",
				Width = 150,
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupGems[1])
			};
			CBGems[1].Checked += GemsChecked;
			CBGems[1].Unchecked += GemsChecked;
			spGemColorsREDGREEN.Children.Add(CBGems[1]);
			#endregion

			StackPanel spGemColorsPurpleYellow = new StackPanel
			{
				Orientation = Orientation.Vertical,
			};
			#region PickupGemsPurple
			CBGems[2] = new CheckBox
			{
				Content = "Pickup Gem Purple",
				Name = "purple",
				Width = 150,
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupGems[2])
			};
			CBGems[2].Checked += GemsChecked;
			CBGems[2].Unchecked += GemsChecked;
			spGemColorsPurpleYellow.Children.Add(CBGems[2]);
			#endregion
			#region PickupGemsYellow
			CBGems[3] = new CheckBox
			{
				Content = "Pickup Gem Yellow",
				Name = "yellow",
				Width = 150,
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupGems[3])
			};
			CBGems[3].Checked += GemsChecked;
			CBGems[3].Unchecked += GemsChecked;
			spGemColorsPurpleYellow.Children.Add(CBGems[3]);
			#endregion

			StackPanel spGemColorsWhite = new StackPanel
			{
				Orientation = Orientation.Vertical,
			};
			#region PickupGemsWhite
			CheckBox cbPickupGemDiamond = new CheckBox
			{
				Content = "Pickup Gem White",
				Name = "white",
				Width = 150,
				Height = 20,
				IsChecked = (Bot.Settings.Loot.PickupGemDiamond)
			};
			cbPickupGemDiamond.Checked += GemsChecked;
			cbPickupGemDiamond.Unchecked += GemsChecked;
			spGemColorsWhite.Children.Add(cbPickupGemDiamond);
			#endregion
			StackPanel spGemColorPanels = new StackPanel
			{
				Orientation = Orientation.Horizontal,
			};
			spGemColorPanels.Children.Add(spGemColorsREDGREEN);
			spGemColorPanels.Children.Add(spGemColorsPurpleYellow);
			spGemColorPanels.Children.Add(spGemColorsWhite);

			spGemOptions.Children.Add(spGemColorPanels);
			lbGilesContent.Items.Add(spGemOptions);
			#endregion

			Button BtnPickUpLoadTemplate = new Button
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
			BtnPickUpLoadTemplate.Click += PickUpLoadXMLClicked;
			lbGilesContent.Items.Add(BtnPickUpLoadTemplate);

			ItemGilesTabItem.Content = lbGilesContent;
			#endregion

		}
	}
}
