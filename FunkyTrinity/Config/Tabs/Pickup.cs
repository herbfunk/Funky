using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using FunkyTrinity.Settings;

namespace FunkyTrinity
{
	 public enum GemQuality
	 {
		  Chipped=14,
		  Flawed=22,
		  Normal=30,
		  Flawless=36,
		  Perfect=42,
		  Radiant=48,
		  Square=54,
		  FlawlessSquare=60,
	 }

	 internal partial class FunkyWindow : Window
	 {
		  #region EventHandling
		  private void PickUpLoadXMLClicked(object sender, EventArgs e)
		  {
				System.Windows.Forms.OpenFileDialog OFD=new System.Windows.Forms.OpenFileDialog
				{
					 InitialDirectory=Path.Combine(Funky.FolderPaths.sTrinityPluginPath, "Config", "Defaults"),
					 RestoreDirectory=false,
					 Filter="xml files (*.xml)|*.xml|All files (*.*)|*.*",
					 Title="Pickup Template",
				};
				System.Windows.Forms.DialogResult OFD_Result=OFD.ShowDialog();

				if (OFD_Result==System.Windows.Forms.DialogResult.OK)
				{
					 try
					 {
						  //;
						  SettingLoot newSettings=SettingLoot.DeserializeFromXML(OFD.FileName);
						  Bot.SettingsFunky.Loot=newSettings;

						  Funky.funkyConfigWindow.Close();
					 } catch
					 {

					 }
				}


		  }

		  private void PickupCraftTomesChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupCraftTomes=!Bot.SettingsFunky.Loot.PickupCraftTomes;
		  }
		  private void PickupCraftPlansChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupCraftPlans=!Bot.SettingsFunky.Loot.PickupCraftPlans;
				spBlacksmithPlans.IsEnabled=Bot.SettingsFunky.Loot.PickupCraftPlans;
				spJewelerPlans.IsEnabled=Bot.SettingsFunky.Loot.PickupCraftPlans;
		  }

		  private void PickupBlacksmithPlanSixChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupBlacksmithPlanSix=!Bot.SettingsFunky.Loot.PickupBlacksmithPlanSix;
		  }
		  private void PickupBlacksmithPlanFiveChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupBlacksmithPlanFive=!Bot.SettingsFunky.Loot.PickupBlacksmithPlanFive;
		  }
		  private void PickupBlacksmithPlanFourChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupBlacksmithPlanFour=!Bot.SettingsFunky.Loot.PickupBlacksmithPlanFour;
		  }

		  private void PickupBlacksmithPlanArchonGauntletsChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupBlacksmithPlanArchonGauntlets=!Bot.SettingsFunky.Loot.PickupBlacksmithPlanArchonGauntlets;
		  }
		  private void PickupBlacksmithPlanArchonSpauldersChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupBlacksmithPlanArchonSpaulders=!Bot.SettingsFunky.Loot.PickupBlacksmithPlanArchonSpaulders;
		  }
		  private void PickupBlacksmithPlanRazorspikesChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupBlacksmithPlanRazorspikes=!Bot.SettingsFunky.Loot.PickupBlacksmithPlanRazorspikes;
		  }

		  private void PickupJewelerDesignFlawlessStarChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupJewelerDesignFlawlessStar=!Bot.SettingsFunky.Loot.PickupJewelerDesignFlawlessStar;
		  }
		  private void PickupJewelerDesignPerfectStarChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupJewelerDesignPerfectStar=!Bot.SettingsFunky.Loot.PickupJewelerDesignPerfectStar;
		  }
		  private void PickupJewelerDesignRadiantStarChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupJewelerDesignRadiantStar=!Bot.SettingsFunky.Loot.PickupJewelerDesignRadiantStar;
		  }
		  private void PickupJewelerDesignMarquiseChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupJewelerDesignMarquise=!Bot.SettingsFunky.Loot.PickupJewelerDesignMarquise;
		  }
		  private void PickupJewelerDesignAmuletChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupJewelerDesignAmulet=!Bot.SettingsFunky.Loot.PickupJewelerDesignAmulet;
		  }
		  private void PickupInfernalKeysChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupInfernalKeys=!Bot.SettingsFunky.Loot.PickupInfernalKeys;
		  }
		  private void PickupDemonicEssenceChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupDemonicEssence=!Bot.SettingsFunky.Loot.PickupDemonicEssence;
		  }

		  private void PickupFollowerItemsChecked(object sender, EventArgs e)
		  {
				Bot.SettingsFunky.Loot.PickupFollowerItems=!Bot.SettingsFunky.Loot.PickupFollowerItems;
		  }
		  private void GemsChecked(object sender, EventArgs e)
		  {
				CheckBox sender_=(CheckBox)sender;
				if (sender_.Name=="red") Bot.SettingsFunky.Loot.PickupGems[0]=!Bot.SettingsFunky.Loot.PickupGems[0];
				if (sender_.Name=="green") Bot.SettingsFunky.Loot.PickupGems[1]=!Bot.SettingsFunky.Loot.PickupGems[1];
				if (sender_.Name=="purple") Bot.SettingsFunky.Loot.PickupGems[2]=!Bot.SettingsFunky.Loot.PickupGems[2];
				if (sender_.Name=="yellow") Bot.SettingsFunky.Loot.PickupGems[3]=!Bot.SettingsFunky.Loot.PickupGems[3];
		  }
		  private void MiscItemLevelSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Loot.MiscItemLevel=Value;
				TBMiscItemLevel.Text=Value.ToString();
		  }
		  private void GemQualityLevelChanged(object sender, EventArgs e)
		  {
				ComboBox cbSender=(ComboBox)sender;

				Bot.SettingsFunky.Loot.MinimumGemItemLevel=(int)Enum.Parse(typeof(GemQuality), cbSender.Items[cbSender.SelectedIndex].ToString());
		  }
		  class GemQualityTypes : ObservableCollection<string>
		  {
				public GemQualityTypes()
				{
					 string[] GemNames=Enum.GetNames(typeof(GemQuality));
					 foreach (var item in GemNames)
					 {
						  Add(item);
					 }
				}
		  }
		  private void LegendaryItemLevelSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Loot.MinimumLegendaryItemLevel=Value;
				TBMinLegendaryLevel.Text=Value.ToString();
		  }
		  private void HealthPotionSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Loot.MaximumHealthPotions=Value;
				TBMaxHealthPots.Text=Value.ToString();
		  }
		  private void GoldAmountSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				Bot.SettingsFunky.Loot.MinimumGoldPile=Value;
				TBMinGoldPile.Text=Value.ToString();
		  }

		  private void WeaponItemLevelSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				if (slider_sender.Name=="Magic")
				{
					 Bot.SettingsFunky.Loot.MinimumWeaponItemLevel[0]=Value;
					 TBMinimumWeaponLevel[0].Text=Value.ToString();
				}
				else
				{
					 Bot.SettingsFunky.Loot.MinimumWeaponItemLevel[1]=Value;
					 TBMinimumWeaponLevel[1].Text=Value.ToString();
				}
		  }
		  private void ArmorItemLevelSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				if (slider_sender.Name=="Magic")
				{
					 Bot.SettingsFunky.Loot.MinimumArmorItemLevel[0]=Value;
					 TBMinimumArmorLevel[0].Text=Value.ToString();
				}
				else
				{
					 Bot.SettingsFunky.Loot.MinimumArmorItemLevel[1]=Value;
					 TBMinimumArmorLevel[1].Text=Value.ToString();
				}
		  }
		  private void JeweleryItemLevelSliderChanged(object sender, EventArgs e)
		  {
				Slider slider_sender=(Slider)sender;
				int Value=(int)slider_sender.Value;
				if (slider_sender.Name=="Magic")
				{
					 Bot.SettingsFunky.Loot.MinimumJeweleryItemLevel[0]=Value;
					 TBMinimumJeweleryLevel[0].Text=Value.ToString();
				}
				else
				{
					 Bot.SettingsFunky.Loot.MinimumJeweleryItemLevel[1]=Value;
					 TBMinimumJeweleryLevel[1].Text=Value.ToString();
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
				TabItem ItemGilesTabItem=new TabItem();
				ItemGilesTabItem.Header="Pickup";
				tcItems.Items.Add(ItemGilesTabItem);
				ListBox lbGilesContent=new ListBox();

				#region Item Level Pickup
				StackPanel spItemPickupLevel=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				#region minimumWeaponILevel
				StackPanel spWeaponPickupLevel=new StackPanel
				{
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};

				TextBlock txt_weaponIlvl=new TextBlock
				{
					 Text="Weapons",
					 FontSize=12,
					 Background=System.Windows.Media.Brushes.DarkSlateGray,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				spWeaponPickupLevel.Children.Add(txt_weaponIlvl);

				StackPanel spWeaponPickupMagical=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				TextBlock txt_weaponMagical=new TextBlock
				{
					 Text="Magic",
					 FontSize=12,
					 Background=System.Windows.Media.Brushes.DarkBlue,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				TBMinimumWeaponLevel=new TextBox[2];
				Slider weaponMagicLevelSlider=new Slider
				{
					 Name="Magic",
					 Width=120,
					 Maximum=63,
					 Minimum=0,
					 TickFrequency=5,
					 LargeChange=5,
					 SmallChange=1,
					 Value=Bot.SettingsFunky.Loot.MinimumWeaponItemLevel[0],
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				weaponMagicLevelSlider.ValueChanged+=WeaponItemLevelSliderChanged;
				TBMinimumWeaponLevel[0]=new TextBox
				{
					 Text=Bot.SettingsFunky.Loot.MinimumWeaponItemLevel[0].ToString(),
					 IsReadOnly=true,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				spWeaponPickupMagical.Children.Add(txt_weaponMagical);
				spWeaponPickupMagical.Children.Add(weaponMagicLevelSlider);
				spWeaponPickupMagical.Children.Add(TBMinimumWeaponLevel[0]);
				spWeaponPickupLevel.Children.Add(spWeaponPickupMagical);


				StackPanel spWeaponPickupRare=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				TextBlock txt_weaponRare=new TextBlock
				{
					 Text="Rare",
					 FontSize=12,
					 Foreground=System.Windows.Media.Brushes.Black,
					 Background=System.Windows.Media.Brushes.Gold,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				Slider weaponRareLevelSlider=new Slider
				{
					 Name="Rare",
					 Width=120,
					 Maximum=63,
					 Minimum=0,
					 TickFrequency=5,
					 LargeChange=5,
					 SmallChange=1,
					 Value=Bot.SettingsFunky.Loot.MinimumWeaponItemLevel[1],
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				weaponRareLevelSlider.ValueChanged+=WeaponItemLevelSliderChanged;
				TBMinimumWeaponLevel[1]=new TextBox
				{
					 Text=Bot.SettingsFunky.Loot.MinimumWeaponItemLevel[1].ToString(),
					 IsReadOnly=true,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				spWeaponPickupRare.Children.Add(txt_weaponRare);
				spWeaponPickupRare.Children.Add(weaponRareLevelSlider);
				spWeaponPickupRare.Children.Add(TBMinimumWeaponLevel[1]);
				spWeaponPickupLevel.Children.Add(spWeaponPickupRare);

				spItemPickupLevel.Children.Add(spWeaponPickupLevel);
				#endregion
				#region minimumArmorILevel
				StackPanel spArmorPickupLevel=new StackPanel
				{
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};

				TBMinimumArmorLevel=new TextBox[2];
				TextBlock txt_armorIlvl=new TextBlock
				{
					 Text="Armor",
					 FontSize=12,
					 Background=System.Windows.Media.Brushes.DarkSlateGray,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				spArmorPickupLevel.Children.Add(txt_armorIlvl);

				StackPanel spArmorPickupMagical=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				TextBlock txt_armorMagic=new TextBlock
				{
					 Text="Magic",
					 FontSize=12,
					 Background=System.Windows.Media.Brushes.DarkBlue,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				Slider armorMagicLevelSlider=new Slider
				{
					 Name="Magic",
					 Width=120,
					 Maximum=63,
					 Minimum=0,
					 TickFrequency=5,
					 LargeChange=5,
					 SmallChange=1,
					 Value=Bot.SettingsFunky.Loot.MinimumArmorItemLevel[0],
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				armorMagicLevelSlider.ValueChanged+=ArmorItemLevelSliderChanged;
				TBMinimumArmorLevel[0]=new TextBox
				{
					 Text=Bot.SettingsFunky.Loot.MinimumArmorItemLevel[0].ToString(),
					 IsReadOnly=true,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				spArmorPickupMagical.Children.Add(txt_armorMagic);
				spArmorPickupMagical.Children.Add(armorMagicLevelSlider);
				spArmorPickupMagical.Children.Add(TBMinimumArmorLevel[0]);
				spArmorPickupLevel.Children.Add(spArmorPickupMagical);


				StackPanel spArmorPickupRare=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				TextBlock txt_armorRare=new TextBlock
				{
					 Text="Rare",
					 FontSize=12,
					 Foreground=System.Windows.Media.Brushes.Black,
					 Background=System.Windows.Media.Brushes.Gold,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				Slider armorRareLevelSlider=new Slider
				{
					 Name="Rare",
					 Maximum=63,
					 Minimum=0,
					 TickFrequency=5,
					 LargeChange=5,
					 SmallChange=1,
					 Width=120,
					 Value=Bot.SettingsFunky.Loot.MinimumArmorItemLevel[1],
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				armorRareLevelSlider.ValueChanged+=ArmorItemLevelSliderChanged;
				TBMinimumArmorLevel[1]=new TextBox
				{
					 Text=Bot.SettingsFunky.Loot.MinimumArmorItemLevel[1].ToString(),
					 IsReadOnly=true,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				spArmorPickupRare.Children.Add(txt_armorRare);
				spArmorPickupRare.Children.Add(armorRareLevelSlider);
				spArmorPickupRare.Children.Add(TBMinimumArmorLevel[1]);
				spArmorPickupLevel.Children.Add(spArmorPickupRare);

				spItemPickupLevel.Children.Add(spArmorPickupLevel);
				#endregion
				#region minimumJeweleryILevel
				TBMinimumJeweleryLevel=new TextBox[2];
				TextBlock txt_jeweleryIlvl=new TextBlock
				{
					 Text="Jewelery",
					 FontSize=12,
					 Background=System.Windows.Media.Brushes.DarkSlateGray,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				StackPanel spJeweleryPickupLevel=new StackPanel
				{

				};
				spJeweleryPickupLevel.Children.Add(txt_jeweleryIlvl);

				StackPanel spJeweleryPickupMagical=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				TextBlock txt_jeweleryMagic=new TextBlock
				{
					 Text="Magic",
					 FontSize=12,
					 Background=System.Windows.Media.Brushes.DarkBlue,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				Slider jeweleryMagicLevelSlider=new Slider
				{
					 Name="Magic",
					 Width=120,
					 Maximum=63,
					 Minimum=0,
					 TickFrequency=5,
					 LargeChange=5,
					 SmallChange=1,
					 Value=Bot.SettingsFunky.Loot.MinimumJeweleryItemLevel[0],
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				jeweleryMagicLevelSlider.ValueChanged+=JeweleryItemLevelSliderChanged;
				TBMinimumJeweleryLevel[0]=new TextBox
				{
					 Text=Bot.SettingsFunky.Loot.MinimumJeweleryItemLevel[0].ToString(),
					 IsReadOnly=true,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				spJeweleryPickupMagical.Children.Add(txt_jeweleryMagic);
				spJeweleryPickupMagical.Children.Add(jeweleryMagicLevelSlider);
				spJeweleryPickupMagical.Children.Add(TBMinimumJeweleryLevel[0]);
				spJeweleryPickupLevel.Children.Add(spJeweleryPickupMagical);

				StackPanel spJeweleryPickupRare=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				TextBlock txt_jeweleryRare=new TextBlock
				{
					 Text="Rare",
					 FontSize=12,
					 Foreground=System.Windows.Media.Brushes.Black,
					 Background=System.Windows.Media.Brushes.Gold,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				Slider jeweleryRareLevelSlider=new Slider
				{
					 Name="Rare",
					 Maximum=63,
					 Minimum=0,
					 TickFrequency=5,
					 LargeChange=5,
					 SmallChange=1,
					 Width=120,
					 Value=Bot.SettingsFunky.Loot.MinimumJeweleryItemLevel[1],
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				jeweleryRareLevelSlider.ValueChanged+=JeweleryItemLevelSliderChanged;
				TBMinimumJeweleryLevel[1]=new TextBox
				{
					 Text=Bot.SettingsFunky.Loot.MinimumJeweleryItemLevel[1].ToString(),
					 IsReadOnly=true,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				spJeweleryPickupRare.Children.Add(txt_jeweleryRare);
				spJeweleryPickupRare.Children.Add(jeweleryRareLevelSlider);
				spJeweleryPickupRare.Children.Add(TBMinimumJeweleryLevel[1]);
				spJeweleryPickupLevel.Children.Add(spJeweleryPickupRare);
				spItemPickupLevel.Children.Add(spJeweleryPickupLevel);
				#endregion

				StackPanel spItemPickup=new StackPanel
				{
					 Background=System.Windows.Media.Brushes.DimGray,
				};
				TextBlock Text_Header_ItemPickup=new TextBlock
				{
					 Text="Item Level Pickup",
					 FontSize=14,
					 Background=System.Windows.Media.Brushes.DimGray,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					 TextAlignment=TextAlignment.Center,
				};
				spItemPickup.Children.Add(Text_Header_ItemPickup);
				spItemPickup.Children.Add(spItemPickupLevel);

				#region LegendaryLevel
				TextBlock Text_Legendary_ItemLevel=new TextBlock
				{
					 Text="Legendary Items",
					 FontSize=12,
					 //Background=System.Windows.Media.Brushes.DarkGreen,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					 TextAlignment=TextAlignment.Left,
				};
				spItemPickup.Children.Add(Text_Legendary_ItemLevel);
				Slider sliderLegendaryILevel=new Slider
				{
					 Width=120,
					 Maximum=63,
					 Minimum=0,
					 TickFrequency=5,
					 LargeChange=5,
					 SmallChange=1,
					 Value=Bot.SettingsFunky.Loot.MinimumLegendaryItemLevel,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				sliderLegendaryILevel.ValueChanged+=LegendaryItemLevelSliderChanged;
				TBMinLegendaryLevel=new TextBox
				{
					 Text=Bot.SettingsFunky.Loot.MinimumLegendaryItemLevel.ToString(),
					 IsReadOnly=true,
				};
				StackPanel LegendaryILvlStackPanel=new StackPanel
				{
					 Width=600,
					 Height=20,
					 Orientation=Orientation.Horizontal,
				};
				LegendaryILvlStackPanel.Children.Add(sliderLegendaryILevel);
				LegendaryILvlStackPanel.Children.Add(TBMinLegendaryLevel);

				spItemPickup.Children.Add(LegendaryILvlStackPanel);
				#endregion


				#region MinMiscItemLevel
				TextBlock Text_Misc_ItemLevel=new TextBlock
				{
					 Text="Misc Item",
					 FontSize=12,
					 //Background=System.Windows.Media.Brushes.DarkGreen,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom+5),
					 TextAlignment=TextAlignment.Left,
				};
				spItemPickup.Children.Add(Text_Misc_ItemLevel);
				Slider slideMinMiscItemLevel=new Slider
				{
					 Width=100,
					 Maximum=63,
					 Minimum=0,
					 TickFrequency=5,
					 LargeChange=5,
					 SmallChange=1,
					 Value=Bot.SettingsFunky.Loot.MiscItemLevel,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				slideMinMiscItemLevel.ValueChanged+=MiscItemLevelSliderChanged;
				TBMiscItemLevel=new TextBox
				{
					 Text=Bot.SettingsFunky.Loot.MiscItemLevel.ToString(),
					 IsReadOnly=true,
				};
				StackPanel MinMiscItemLevelStackPanel=new StackPanel
				{
					 Width=600,
					 Height=20,
					 Orientation=Orientation.Horizontal,
				};
				MinMiscItemLevelStackPanel.Children.Add(slideMinMiscItemLevel);
				MinMiscItemLevelStackPanel.Children.Add(TBMiscItemLevel);
				spItemPickup.Children.Add(MinMiscItemLevelStackPanel);
				#endregion


				lbGilesContent.Items.Add(spItemPickup);
				#endregion



				#region MaxHealthPotions
				Slider sliderMaxHealthPots=new Slider
				{
					 Width=100,
					 Maximum=100,
					 Minimum=0,
					 TickFrequency=25,
					 LargeChange=20,
					 SmallChange=5,
					 Value=Bot.SettingsFunky.Loot.MaximumHealthPotions,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				sliderMaxHealthPots.ValueChanged+=HealthPotionSliderChanged;
				TBMaxHealthPots=new TextBox
				{
					 Text=Bot.SettingsFunky.Loot.MaximumHealthPotions.ToString(),
					 IsReadOnly=true,
				};
				StackPanel MaxHealthPotsStackPanel=new StackPanel
				{
					 Width=600,
					 Height=20,
					 Orientation=Orientation.Horizontal,
				};
				MaxHealthPotsStackPanel.Children.Add(sliderMaxHealthPots);
				MaxHealthPotsStackPanel.Children.Add(TBMaxHealthPots);
				StackPanel spHealthPotions=new StackPanel();

				TextBlock txt_HealthPotions=new TextBlock
				{
					 Text="Health Potions",
					 FontSize=12,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				spHealthPotions.Children.Add(txt_HealthPotions);
				spHealthPotions.Children.Add(MaxHealthPotsStackPanel);
				#endregion

				#region MinimumGoldPile
				Slider slideMinGoldPile=new Slider
				{
					 Width=120,
					 Maximum=7500,
					 Minimum=0,
					 TickFrequency=1000,
					 LargeChange=1000,
					 SmallChange=1,
					 Value=Bot.SettingsFunky.Loot.MinimumGoldPile,
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
				};
				slideMinGoldPile.ValueChanged+=GoldAmountSliderChanged;
				TBMinGoldPile=new TextBox
				{
					 Text=Bot.SettingsFunky.Loot.MinimumGoldPile.ToString(),
					 IsReadOnly=true,
				};
				StackPanel MinGoldPileStackPanel=new StackPanel
				{
					 Width=600,
					 Height=20,
					 Orientation=Orientation.Horizontal,
				};
				MinGoldPileStackPanel.Children.Add(slideMinGoldPile);
				MinGoldPileStackPanel.Children.Add(TBMinGoldPile);
				StackPanel spMinimumGold=new StackPanel();

				TextBlock txt_MinimumGold=new TextBlock
				{
					 Text="Minimum Gold Pile",
					 FontSize=12,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				spMinimumGold.Children.Add(txt_MinimumGold);
				spMinimumGold.Children.Add(MinGoldPileStackPanel);
				#endregion


				StackPanel spCraftPlans=new StackPanel
				{
					 Background=System.Windows.Media.Brushes.DimGray,
				};
				TextBlock txt_CraftPlansPickup=new TextBlock
				{
					 Text="Craft Plan Options",
					 FontSize=12,
					 Background=System.Windows.Media.Brushes.DarkSlateGray,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				spCraftPlans.Children.Add(txt_CraftPlansPickup);

				#region PickupCraftPlans
				CheckBox cbPickupCraftPlans=new CheckBox
				{
					 Content="Pickup Craft Plans",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupCraftPlans),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbPickupCraftPlans.Checked+=PickupCraftPlansChecked;
				cbPickupCraftPlans.Unchecked+=PickupCraftPlansChecked;
				spCraftPlans.Children.Add(cbPickupCraftPlans);
				#endregion

				spBlacksmithPlans=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
					 IsEnabled=Bot.SettingsFunky.Loot.PickupCraftPlans,
				};
				StackPanel spBlacksmithPlansProperties=new StackPanel();

				#region Blacksmith_property_six
				CheckBox cbBlacksmithPropertySix=new CheckBox
				{
					 Content="Plans: Property Six",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupBlacksmithPlanSix),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbBlacksmithPropertySix.Checked+=PickupBlacksmithPlanSixChecked;
				cbBlacksmithPropertySix.Unchecked+=PickupBlacksmithPlanSixChecked;
				spBlacksmithPlansProperties.Children.Add(cbBlacksmithPropertySix);
				#endregion

				#region Blacksmith_property_five
				CheckBox cbBlacksmithPropertyFive=new CheckBox
				{
					 Content="Plans: Property Five",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupBlacksmithPlanFive),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbBlacksmithPropertyFive.Checked+=PickupBlacksmithPlanFiveChecked;
				cbBlacksmithPropertyFive.Unchecked+=PickupBlacksmithPlanFiveChecked;
				spBlacksmithPlansProperties.Children.Add(cbBlacksmithPropertyFive);
				#endregion

				#region Blacksmith_property_four
				CheckBox cbBlacksmithPropertyFour=new CheckBox
				{
					 Content="Plans: Property Four",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupBlacksmithPlanFour),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbBlacksmithPropertyFive.Checked+=PickupBlacksmithPlanFourChecked;
				cbBlacksmithPropertyFive.Unchecked+=PickupBlacksmithPlanFourChecked;
				spBlacksmithPlansProperties.Children.Add(cbBlacksmithPropertyFour);
				#endregion

				spBlacksmithPlans.Children.Add(spBlacksmithPlansProperties);

				StackPanel spBlacksmithPlansArchonRazor=new StackPanel();

				#region Blacksmith_archon_gauntlets
				CheckBox cbBlacksmithArchonGauntlets=new CheckBox
				{
					 Content="Plans: Archon Gauntlets",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupBlacksmithPlanArchonGauntlets),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbBlacksmithArchonGauntlets.Checked+=PickupBlacksmithPlanArchonGauntletsChecked;
				cbBlacksmithArchonGauntlets.Unchecked+=PickupBlacksmithPlanArchonGauntletsChecked;
				spBlacksmithPlansArchonRazor.Children.Add(cbBlacksmithArchonGauntlets);
				#endregion

				#region Blacksmith_archon_spaulders
				CheckBox cbBlacksmithArchonSpaulders=new CheckBox
				{
					 Content="Plans: Archon Spaulders",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupBlacksmithPlanArchonSpaulders),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbBlacksmithArchonSpaulders.Checked+=PickupBlacksmithPlanArchonSpauldersChecked;
				cbBlacksmithArchonSpaulders.Unchecked+=PickupBlacksmithPlanArchonSpauldersChecked;
				spBlacksmithPlansArchonRazor.Children.Add(cbBlacksmithArchonSpaulders);
				#endregion

				#region Blacksmith_archon_razorspikes
				CheckBox cbBlacksmithRazorSpikes=new CheckBox
				{
					 Content="Plans: Razorspikes",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupBlacksmithPlanRazorspikes),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbBlacksmithRazorSpikes.Checked+=PickupBlacksmithPlanRazorspikesChecked;
				cbBlacksmithRazorSpikes.Unchecked+=PickupBlacksmithPlanRazorspikesChecked;
				spBlacksmithPlansArchonRazor.Children.Add(cbBlacksmithRazorSpikes);
				#endregion

				spBlacksmithPlans.Children.Add(spBlacksmithPlansArchonRazor);
				spCraftPlans.Children.Add(spBlacksmithPlans);

				spJewelerPlans=new StackPanel
				{
					 IsEnabled=Bot.SettingsFunky.Loot.PickupCraftPlans,
				};

				#region design_flawlessStar
				CheckBox cbJewelerFlawlessStar=new CheckBox
				{
					 Content="Design: Flawless Star",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupJewelerDesignFlawlessStar),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbJewelerFlawlessStar.Checked+=PickupJewelerDesignFlawlessStarChecked;
				cbJewelerFlawlessStar.Unchecked+=PickupJewelerDesignFlawlessStarChecked;
				spJewelerPlans.Children.Add(cbJewelerFlawlessStar);
				#endregion

				#region design_perfectstar
				CheckBox cbJewelerPerfectStar=new CheckBox
				{
					 Content="Design: Perfect Star",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupJewelerDesignPerfectStar),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbJewelerPerfectStar.Checked+=PickupJewelerDesignPerfectStarChecked;
				cbJewelerPerfectStar.Unchecked+=PickupJewelerDesignPerfectStarChecked;
				spJewelerPlans.Children.Add(cbJewelerPerfectStar);
				#endregion

				#region design_radiantstar
				CheckBox cbJewelerRadiantStar=new CheckBox
				{
					 Content="Design: Radiant Star",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupJewelerDesignRadiantStar),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbJewelerRadiantStar.Checked+=PickupJewelerDesignRadiantStarChecked;
				cbJewelerRadiantStar.Unchecked+=PickupJewelerDesignRadiantStarChecked;
				spJewelerPlans.Children.Add(cbJewelerRadiantStar);
				#endregion

				#region design_marquise
				CheckBox cbJewelerMarquise=new CheckBox
				{
					 Content="Design: Marquise",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupJewelerDesignMarquise),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbJewelerMarquise.Checked+=PickupJewelerDesignMarquiseChecked;
				cbJewelerMarquise.Unchecked+=PickupJewelerDesignMarquiseChecked;
				spJewelerPlans.Children.Add(cbJewelerMarquise);
				#endregion

				#region design_amulets
				CheckBox cbJewelerAmulets=new CheckBox
				{
					 Content="Design: Amulets",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupJewelerDesignAmulet),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbJewelerAmulets.Checked+=PickupJewelerDesignAmuletChecked;
				cbJewelerAmulets.Unchecked+=PickupJewelerDesignAmuletChecked;
				spJewelerPlans.Children.Add(cbJewelerAmulets);
				#endregion

				spCraftPlans.Children.Add(spJewelerPlans);

				lbGilesContent.Items.Add(spCraftPlans);


				StackPanel spMiscItemPickupOptions=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				#region PickupCraftTomes
				CheckBox cbPickupCraftTomes=new CheckBox
				{
					 Content="Pickup Craft Tomes",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupCraftTomes),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbPickupCraftTomes.Checked+=PickupCraftTomesChecked;
				cbPickupCraftTomes.Unchecked+=PickupCraftTomesChecked;
				spMiscItemPickupOptions.Children.Add(cbPickupCraftTomes);
				#endregion
				#region PickupFollowerItems
				CheckBox cbPickupFollowerItems=new CheckBox
				{
					 Content="Pickup Follower Items",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupFollowerItems),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbPickupFollowerItems.Checked+=PickupFollowerItemsChecked;
				cbPickupFollowerItems.Unchecked+=PickupFollowerItemsChecked;
				spMiscItemPickupOptions.Children.Add(cbPickupFollowerItems);
				#endregion
				#region PickupInfernalKeys
				CheckBox cbPickupInfernalKeys=new CheckBox
				{
					 Content="Pickup Inferno Keys",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupInfernalKeys),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbPickupInfernalKeys.Checked+=PickupInfernalKeysChecked;
				cbPickupInfernalKeys.Unchecked+=PickupInfernalKeysChecked;
				spMiscItemPickupOptions.Children.Add(cbPickupInfernalKeys);
				#endregion
				#region PickupDemonicEssence
				CheckBox cbPickupDemonicEssence=new CheckBox
				{
					 Content="Pickup Demonic Essences",
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupDemonicEssence),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Right,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+5, Margin.Bottom),
				};
				cbPickupDemonicEssence.Checked+=PickupDemonicEssenceChecked;
				cbPickupDemonicEssence.Unchecked+=PickupDemonicEssenceChecked;
				spMiscItemPickupOptions.Children.Add(cbPickupDemonicEssence);
				#endregion
				//
				TextBlock txt_miscPickup=new TextBlock
				{
					 Text="Misc Pickup Options",
					 FontSize=12,
					 Background=System.Windows.Media.Brushes.DarkSlateGray,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 Margin=new Thickness(Margin.Left, Margin.Top, Margin.Right+4, Margin.Bottom+4),
				};
				StackPanel spMiscItemPickup=new StackPanel
				{
					 Background=System.Windows.Media.Brushes.DimGray,
				};
				spMiscItemPickup.Children.Add(txt_miscPickup);
				spMiscItemPickup.Children.Add(spMiscItemPickupOptions);
				spMiscItemPickup.Children.Add(spMinimumGold);
				spMiscItemPickup.Children.Add(spHealthPotions);

				lbGilesContent.Items.Add(spMiscItemPickup);


				#region Gems
				StackPanel spGemOptions=new StackPanel
				{
					 Orientation=Orientation.Vertical,
					 Background=System.Windows.Media.Brushes.DimGray,
				};
				TextBlock Text_GemOptions=new TextBlock
				{
					 Text="Gems",
					 FontSize=12,
					 Foreground=System.Windows.Media.Brushes.Black,
					 Background=System.Windows.Media.Brushes.Gold,
					 TextAlignment=TextAlignment.Left,
				};
				spGemOptions.Children.Add(Text_GemOptions);

				#region GemQuality
				TextBlock Text_MinimumGemQuality=new TextBlock
				{
					 Text="Minimum Gem Quality",
					 FontSize=11,
					 Foreground=System.Windows.Media.Brushes.GhostWhite,
					 TextAlignment=TextAlignment.Left,
				};
				ComboBox CBGemQualityLevel=new ComboBox
				{
					 Height=20,
					 ItemsSource=new GemQualityTypes(),
					 Text=Enum.GetName(typeof(GemQuality), Bot.SettingsFunky.Loot.MinimumGemItemLevel).ToString(),
					 HorizontalAlignment=System.Windows.HorizontalAlignment.Left,
					 Margin=new Thickness(5),
				};
				CBGemQualityLevel.SelectionChanged+=GemQualityLevelChanged;
				spGemOptions.Children.Add(Text_MinimumGemQuality);
				spGemOptions.Children.Add(CBGemQualityLevel);
				#endregion

				CBGems=new CheckBox[4];

				StackPanel spGemColorsREDGREEN=new StackPanel
				{
					 Orientation=Orientation.Vertical,
				};
				#region PickupGemsRed
				CBGems[0]=new CheckBox
				{
					 Content="Pickup Gem Red",
					 Name="red",
					 Width=150,
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupGems[0])
				};
				CBGems[0].Checked+=GemsChecked;
				CBGems[0].Unchecked+=GemsChecked;
				spGemColorsREDGREEN.Children.Add(CBGems[0]);
				#endregion
				#region PickupGemsGreen
				CBGems[1]=new CheckBox
				{
					 Content="Pickup Gem Green",
					 Name="green",
					 Width=150,
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupGems[1])
				};
				CBGems[1].Checked+=GemsChecked;
				CBGems[1].Unchecked+=GemsChecked;
				spGemColorsREDGREEN.Children.Add(CBGems[1]);
				#endregion

				StackPanel spGemColorsPurpleYellow=new StackPanel
				{
					 Orientation=Orientation.Vertical,
				};
				#region PickupGemsPurple
				CBGems[2]=new CheckBox
				{
					 Content="Pickup Gem Purple",
					 Name="purple",
					 Width=150,
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupGems[2])
				};
				CBGems[2].Checked+=GemsChecked;
				CBGems[2].Unchecked+=GemsChecked;
				spGemColorsPurpleYellow.Children.Add(CBGems[2]);
				#endregion
				#region PickupGemsYellow
				CBGems[3]=new CheckBox
				{
					 Content="Pickup Gem Yellow",
					 Name="yellow",
					 Width=150,
					 Height=20,
					 IsChecked=(Bot.SettingsFunky.Loot.PickupGems[3])
				};
				CBGems[3].Checked+=GemsChecked;
				CBGems[3].Unchecked+=GemsChecked;
				spGemColorsPurpleYellow.Children.Add(CBGems[3]);
				#endregion
				StackPanel spGemColorPanels=new StackPanel
				{
					 Orientation=Orientation.Horizontal,
				};
				spGemColorPanels.Children.Add(spGemColorsREDGREEN);
				spGemColorPanels.Children.Add(spGemColorsPurpleYellow);

				spGemOptions.Children.Add(spGemColorPanels);
				lbGilesContent.Items.Add(spGemOptions);
				#endregion

				Button BtnPickUpLoadTemplate=new Button
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
				BtnPickUpLoadTemplate.Click+=PickUpLoadXMLClicked;
				lbGilesContent.Items.Add(BtnPickUpLoadTemplate);

				ItemGilesTabItem.Content=lbGilesContent;
				#endregion

		  }
	 }
}
