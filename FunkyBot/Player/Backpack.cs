using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.DBHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot.Settings;
using Zeta.Internals;
using Zeta.Internals.Actors;

namespace FunkyBot.Player
{
	internal class Backpack
	{
		#region Static Item Methods

		// **********************************************************************************************
		// *****      Determine if we should stash this item or not based on item type and score    *****
		// **********************************************************************************************
		internal static bool ShouldWeStashThis(CacheACDItem thisitem)
		{
			// Stash all unidentified items - assume we want to keep them since we are using an identifier over-ride
			if (thisitem.IsUnidentified)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] = (autokeep unidentified items)");
				return true;
			}
			// Now look for Misc items we might want to keep
			GilesItemType TrueItemType = DetermineItemType(thisitem.ThisInternalName, thisitem.ThisDBItemType, thisitem.ThisFollowerType);

			if (TrueItemType == GilesItemType.StaffOfHerding)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep staff of herding)");
				return true;
			}
			if (TrueItemType == GilesItemType.CraftingMaterial)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep craft materials)");
				return true;
			}
			if (TrueItemType == GilesItemType.CraftingPlan)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep plans)");
				return true;
			}
			if (TrueItemType == GilesItemType.Emerald)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == GilesItemType.Amethyst)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == GilesItemType.Topaz)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == GilesItemType.Ruby)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == GilesItemType.CraftTome)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep tomes)");
				return true;
			}
			if (TrueItemType == GilesItemType.InfernalKey)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep infernal key)");
				return true;
			}
			if (TrueItemType == GilesItemType.HealthPotion)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (ignoring potions)");
				return false;
			}

			if (thisitem.ThisQuality >= ItemQuality.Legendary)
			{
				if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep legendaries)");
				return true;
			}

			// Ok now try to do some decent item scoring based on item types
			double iNeedScore = ScoreNeeded(TrueItemType);
			double iMyScore = ValueThisItem(thisitem, TrueItemType);
			if (bOutputItemScores) Logging.Write(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = " + iMyScore);
			if (iMyScore >= iNeedScore) return true;

			// If we reached this point, then we found no reason to keep the item!
			return false;
		}

		// **********************************************************************************************
		// *****       Pickup Validation - Determines what should or should not be picked up        *****
		// **********************************************************************************************
		internal static bool GilesPickupItemValidation(CacheItem item)
		{
			// Calculate giles item types and base types etc.
			GilesItemType thisGilesItemType = DetermineItemType(item.InternalName, item.BalanceData.thisItemType, item.BalanceData.thisFollowerType);
			GilesBaseItemType thisGilesBaseType = DetermineBaseType(thisGilesItemType);

			// If it's legendary, we always want it *IF* it's level is right
			if (item.Itemquality >= ItemQuality.Legendary)
			{
				if (Bot.Settings.Loot.MinimumLegendaryItemLevel > 0 && (item.BalanceData.iThisItemLevel >= Bot.Settings.Loot.MinimumLegendaryItemLevel || Bot.Settings.Loot.MinimumLegendaryItemLevel == 1))
					return true;

				return false;
			}



			// Error logging for DemonBuddy item mis-reading
			ItemType gilesDBItemType = GilesToDBItemType(thisGilesItemType);
			if (gilesDBItemType != item.BalanceData.thisItemType)
			{
				Logging.Write("GSError: Item type mis-match detected: Item Internal=" + item.InternalName + ". DemonBuddy ItemType thinks item type is=" + item.BalanceData.thisItemType + ". Giles thinks item type is=" +
					 gilesDBItemType + ". [pickup]", true);
			}

			switch (thisGilesBaseType)
			{
				case GilesBaseItemType.WeaponTwoHand:
				case GilesBaseItemType.WeaponOneHand:
				case GilesBaseItemType.WeaponRange:
					// Not enough DPS, so analyse for possibility to blacklist
					if (item.Itemquality < ItemQuality.Magic1)
					{
						// White item, blacklist
						return false;
					}
					if (item.Itemquality >= ItemQuality.Magic1 && item.Itemquality < ItemQuality.Rare4)
					{
						if (Bot.Settings.Loot.MinimumWeaponItemLevel[0] == 0 || (Bot.Settings.Loot.MinimumWeaponItemLevel[0] != 0 && item.BalanceData.iThisItemLevel < Bot.Settings.Loot.MinimumWeaponItemLevel[0]))
						{
							// Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
							return false;
						}
					}
					else
					{
						if (Bot.Settings.Loot.MinimumWeaponItemLevel[1] == 0 || (Bot.Settings.Loot.MinimumWeaponItemLevel[1] != 0 && item.BalanceData.iThisItemLevel < Bot.Settings.Loot.MinimumWeaponItemLevel[1]))
						{
							// Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
							return false;
						}
					}
					break;
				case GilesBaseItemType.Armor:
				case GilesBaseItemType.Offhand:
					if (item.Itemquality < ItemQuality.Magic1)
					{
						// White item, blacklist
						return false;
					}
					if (item.Itemquality >= ItemQuality.Magic1 && item.Itemquality < ItemQuality.Rare4)
					{
						if (Bot.Settings.Loot.MinimumArmorItemLevel[0] == 0 || (Bot.Settings.Loot.MinimumArmorItemLevel[0] != 0 && item.BalanceData.iThisItemLevel < Bot.Settings.Loot.MinimumArmorItemLevel[0]))
						{
							// Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
							return false;
						}
					}
					else
					{
						if (Bot.Settings.Loot.MinimumArmorItemLevel[1] == 0 || (Bot.Settings.Loot.MinimumArmorItemLevel[1] != 0 && item.BalanceData.iThisItemLevel < Bot.Settings.Loot.MinimumArmorItemLevel[1]))
						{
							// Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
							return false;
						}
					}
					break;
				case GilesBaseItemType.Jewelry:
					if (item.Itemquality < ItemQuality.Magic1)
					{
						// White item, blacklist
						return false;
					}
					if (item.Itemquality >= ItemQuality.Magic1 && item.Itemquality < ItemQuality.Rare4)
					{
						if (Bot.Settings.Loot.MinimumJeweleryItemLevel[0] == 0 || (Bot.Settings.Loot.MinimumJeweleryItemLevel[0] != 0 && item.BalanceData.iThisItemLevel < Bot.Settings.Loot.MinimumJeweleryItemLevel[0]))
						{
							// Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
							return false;
						}
					}
					else
					{
						if (Bot.Settings.Loot.MinimumJeweleryItemLevel[1] == 0 || (Bot.Settings.Loot.MinimumJeweleryItemLevel[1] != 0 && item.BalanceData.iThisItemLevel < Bot.Settings.Loot.MinimumJeweleryItemLevel[1]))
						{
							// Between magic and rare, and either we want no blues, or this level is higher than the blue level we want
							return false;
						}
					}
					break;
				case GilesBaseItemType.FollowerItem:
					if (item.BalanceData.iThisItemLevel < 60 || !Bot.Settings.Loot.PickupFollowerItems || item.Itemquality < ItemQuality.Rare4)
					{
						return false;
					}
					break;
				case GilesBaseItemType.Gem:
					if (item.BalanceData.iThisItemLevel < Bot.Settings.Loot.MinimumGemItemLevel || (thisGilesItemType == GilesItemType.Ruby && !Bot.Settings.Loot.PickupGems[0]) || (thisGilesItemType == GilesItemType.Emerald && !Bot.Settings.Loot.PickupGems[1]) ||
						  (thisGilesItemType == GilesItemType.Amethyst && !Bot.Settings.Loot.PickupGems[2]) || (thisGilesItemType == GilesItemType.Topaz && !Bot.Settings.Loot.PickupGems[3]))
					{
						return false;
					}
					break;
				case GilesBaseItemType.Misc:
					// Note; Infernal keys are misc, so should be picked up here - we aren't filtering them out, so should default to true at the end of this function
					if (thisGilesItemType == GilesItemType.CraftingMaterial)
					{
						if (item.BalanceData.iThisItemLevel == 63 && Bot.Settings.Loot.PickupDemonicEssence)
							return true;

						return item.BalanceData.iThisItemLevel < Bot.Settings.Loot.MiscItemLevel;
					}
					if (thisGilesItemType == GilesItemType.CraftTome && (item.BalanceData.iThisItemLevel < Bot.Settings.Loot.MiscItemLevel || !Bot.Settings.Loot.PickupCraftTomes))
					{
						return false;
					}
					if (thisGilesItemType == GilesItemType.CraftingPlan)
					{
						if (!Bot.Settings.Loot.PickupCraftPlans) return false;

						int gamebalanceID = item.BalanceID.Value;

						if (item.BalanceData.IsBlacksmithPlanSixProperties && !Bot.Settings.Loot.PickupBlacksmithPlanSix) return false;
						if (item.BalanceData.IsBlacksmithPlanFiveProperties && !Bot.Settings.Loot.PickupBlacksmithPlanFive) return false;
						if (item.BalanceData.IsBlacksmithPlanFourProperties && !Bot.Settings.Loot.PickupBlacksmithPlanFour) return false;

						if (item.BalanceData.IsBlacksmithPlanArchonSpaulders && !Bot.Settings.Loot.PickupBlacksmithPlanArchonSpaulders) return false;
						if (item.BalanceData.IsBlacksmithPlanArchonGauntlets && !Bot.Settings.Loot.PickupBlacksmithPlanArchonGauntlets) return false;
						if (item.BalanceData.IsBlacksmithPlanRazorspikes && !Bot.Settings.Loot.PickupBlacksmithPlanRazorspikes) return false;


						if (item.BalanceData.IsJewelcraftDesignAmulet && !Bot.Settings.Loot.PickupJewelerDesignAmulet) return false;
						if (item.BalanceData.IsJewelcraftDesignFlawlessStarGem && !Bot.Settings.Loot.PickupJewelerDesignFlawlessStar) return false;
						if (item.BalanceData.IsJewelcraftDesignMarquiseGem && !Bot.Settings.Loot.PickupJewelerDesignMarquise) return false;
						if (item.BalanceData.IsJewelcraftDesignPerfectStarGem && !Bot.Settings.Loot.PickupJewelerDesignPerfectStar) return false;
						if (item.BalanceData.IsJewelcraftDesignRadiantStarGem && !Bot.Settings.Loot.PickupJewelerDesignRadiantStar) return false;


					}
					if (thisGilesItemType == GilesItemType.InfernalKey)
					{
						if (!Bot.Settings.Loot.PickupInfernalKeys) return false;
					}
					// Potion filtering
					if (thisGilesItemType == GilesItemType.HealthPotion)
					{
						if (Bot.Settings.Loot.MaximumHealthPotions <= 0)
							return false;

						var Potions = Bot.Character.Data.BackPack.ReturnCurrentPotions();

						if (Potions == null || !Potions.Any() || Bot.Character.Data.BackPack.BestPotionToUse == null)
							return true;
						if (Bot.Character.Data.BackPack.BestPotionToUse != null && item.BalanceData.iThisItemLevel < Bot.Character.Data.BackPack.BestPotionToUse.Level)
							return false;
						if (Potions.Sum(potions => potions.ThisItemStackQuantity) >= Bot.Settings.Loot.MaximumHealthPotions)
							return false;
					}
					if (thisGilesItemType == GilesItemType.MiscBook && item.BalanceData.iThisItemLevel < Bot.Settings.Loot.MiscItemLevel)
						return false;

					break;
				case GilesBaseItemType.HealthGlobe:
					return false;
				case GilesBaseItemType.Unknown:
					return false;
				default:
					return false;
			} // Switch giles base item type
			// Didn't cancel it, so default to true!
			return true;
		}

		// **********************************************************************************************
		// *****      Sell Validation - Determines what should or should not be sold to vendor      *****
		// **********************************************************************************************
		internal static bool GilesSellValidation(string thisinternalname, int thislevel, ItemQuality thisquality, ItemType thisdbitemtype, FollowerType thisfollowertype)
		{
			GilesItemType thisGilesItemType = DetermineItemType(thisinternalname, thisdbitemtype, thisfollowertype);
			GilesBaseItemType thisGilesBaseType = DetermineBaseType(thisGilesItemType);

			switch (thisGilesBaseType)
			{
				case GilesBaseItemType.WeaponRange:
				case GilesBaseItemType.WeaponOneHand:
				case GilesBaseItemType.WeaponTwoHand:
				case GilesBaseItemType.Armor:
				case GilesBaseItemType.Offhand:
				case GilesBaseItemType.Jewelry:
				case GilesBaseItemType.FollowerItem:
					return true;
				case GilesBaseItemType.Gem:
				case GilesBaseItemType.Misc:
				case GilesBaseItemType.Unknown:
					//Sell any plans not already stashed.
					return thisdbitemtype == ItemType.CraftingPlan;
			} // Switch giles base item type
			return false;
		}

		// **********************************************************************************************
		// ***** DetermineItemType - Calculates what kind of item it is from D3 internalnames       *****
		// **********************************************************************************************
		internal static GilesItemType DetermineItemType(string sThisInternalName, ItemType DBItemType, FollowerType dbFollowerType = FollowerType.None)
		{
			sThisInternalName = sThisInternalName.ToLower();
			if (sThisInternalName.StartsWith("axe_")) return GilesItemType.Axe;
			if (sThisInternalName.StartsWith("ceremonialdagger_")) return GilesItemType.CeremonialKnife;
			if (sThisInternalName.StartsWith("handxbow_")) return GilesItemType.HandCrossbow;
			if (sThisInternalName.StartsWith("dagger_")) return GilesItemType.Dagger;
			if (sThisInternalName.StartsWith("fistweapon_")) return GilesItemType.FistWeapon;
			if (sThisInternalName.StartsWith("mace_")) return GilesItemType.Mace;
			if (sThisInternalName.StartsWith("mightyweapon_1h_")) return GilesItemType.MightyWeapon;
			if (sThisInternalName.StartsWith("spear_")) return GilesItemType.Spear;
			if (sThisInternalName.StartsWith("sword_")) return GilesItemType.Sword;
			if (sThisInternalName.StartsWith("wand_")) return GilesItemType.Wand;
			if (sThisInternalName.StartsWith("twohandedaxe_")) return GilesItemType.TwoHandAxe;
			if (sThisInternalName.StartsWith("bow_")) return GilesItemType.TwoHandBow;
			if (sThisInternalName.StartsWith("combatstaff_")) return GilesItemType.TwoHandDaibo;
			if (sThisInternalName.StartsWith("xbow_")) return GilesItemType.TwoHandCrossbow;
			if (sThisInternalName.StartsWith("twohandedmace_")) return GilesItemType.TwoHandMace;
			if (sThisInternalName.StartsWith("mightyweapon_2h_")) return GilesItemType.TwoHandMighty;
			if (sThisInternalName.StartsWith("polearm_")) return GilesItemType.TwoHandPolearm;
			if (sThisInternalName.StartsWith("staff_")) return GilesItemType.TwoHandStaff;
			if (sThisInternalName.StartsWith("twohandedsword_")) return GilesItemType.TwoHandSword;
			if (sThisInternalName.StartsWith("staffofcow")) return GilesItemType.StaffOfHerding;
			if (sThisInternalName.StartsWith("mojo_")) return GilesItemType.Mojo;
			if (sThisInternalName.StartsWith("orb_")) return GilesItemType.Source;
			if (sThisInternalName.StartsWith("quiver_")) return GilesItemType.Quiver;
			if (sThisInternalName.StartsWith("shield_")) return GilesItemType.Shield;
			if (sThisInternalName.StartsWith("amulet_")) return GilesItemType.Amulet;
			if (sThisInternalName.StartsWith("ring_")) return GilesItemType.Ring;
			if (sThisInternalName.StartsWith("boots_")) return GilesItemType.Boots;
			if (sThisInternalName.StartsWith("bracers_")) return GilesItemType.Bracers;
			if (sThisInternalName.StartsWith("cloak_")) return GilesItemType.Cloak;
			if (sThisInternalName.StartsWith("gloves_")) return GilesItemType.Gloves;
			if (sThisInternalName.StartsWith("pants_")) return GilesItemType.Pants;
			if (sThisInternalName.StartsWith("barbbelt_")) return GilesItemType.MightyBelt;
			if (sThisInternalName.StartsWith("shoulderpads_")) return GilesItemType.Shoulders;
			if (sThisInternalName.StartsWith("spiritstone_")) return GilesItemType.SpiritStone;
			if (sThisInternalName.StartsWith("voodoomask_")) return GilesItemType.VoodooMask;
			if (sThisInternalName.StartsWith("wizardhat_")) return GilesItemType.WizardHat;
			if (sThisInternalName.StartsWith("lore_book_")) return GilesItemType.MiscBook;
			if (sThisInternalName.StartsWith("page_of_")) return GilesItemType.CraftTome;
			if (sThisInternalName.StartsWith("blacksmithstome")) return GilesItemType.CraftTome;
			if (sThisInternalName.StartsWith("ruby_")) return GilesItemType.Ruby;
			if (sThisInternalName.StartsWith("emerald_")) return GilesItemType.Emerald;
			if (sThisInternalName.StartsWith("topaz_")) return GilesItemType.Topaz;
			if (sThisInternalName.StartsWith("amethyst")) return GilesItemType.Amethyst;
			if (sThisInternalName.StartsWith("healthpotion")) return GilesItemType.HealthPotion;
			if (sThisInternalName.StartsWith("followeritem_enchantress_")) return GilesItemType.FollowerEnchantress;
			if (sThisInternalName.StartsWith("followeritem_scoundrel_")) return GilesItemType.FollowerScoundrel;
			if (sThisInternalName.StartsWith("followeritem_templar_")) return GilesItemType.FollowerTemplar;
			if (sThisInternalName.StartsWith("craftingplan_")) return GilesItemType.CraftingPlan;
			if (sThisInternalName.StartsWith("dye_")) return GilesItemType.Dye;
			if (sThisInternalName.StartsWith("a1_")) return GilesItemType.SpecialItem;
			if (sThisInternalName.StartsWith("healthglobe")) return GilesItemType.HealthGlobe;

			// Follower item types
			if (sThisInternalName.StartsWith("jewelbox_") || DBItemType == ItemType.FollowerSpecial)
			{
				if (dbFollowerType == FollowerType.Scoundrel)
					return GilesItemType.FollowerScoundrel;
				if (dbFollowerType == FollowerType.Templar)
					return GilesItemType.FollowerTemplar;
				if (dbFollowerType == FollowerType.Enchantress)
					return GilesItemType.FollowerEnchantress;
			}

			// Fall back on some partial DB item type checking 
			if (sThisInternalName.StartsWith("crafting_") || sThisInternalName.StartsWith("craftingmaterials_"))
			{
				if (DBItemType == ItemType.CraftingPage) return GilesItemType.CraftTome;
				return GilesItemType.CraftingMaterial;
			}

			if (sThisInternalName.StartsWith("chestarmor_"))
			{
				if (DBItemType == ItemType.Cloak) return GilesItemType.Cloak;
				return GilesItemType.Chest;
			}
			if (sThisInternalName.StartsWith("helm_"))
			{
				if (DBItemType == ItemType.SpiritStone) return GilesItemType.SpiritStone;
				if (DBItemType == ItemType.VoodooMask) return GilesItemType.VoodooMask;
				if (DBItemType == ItemType.WizardHat) return GilesItemType.WizardHat;
				return GilesItemType.Helm;
			}
			if (sThisInternalName.StartsWith("helmcloth_"))
			{
				if (DBItemType == ItemType.SpiritStone) return GilesItemType.SpiritStone;
				if (DBItemType == ItemType.VoodooMask) return GilesItemType.VoodooMask;
				if (DBItemType == ItemType.WizardHat) return GilesItemType.WizardHat;
				return GilesItemType.Helm;
			}
			if (sThisInternalName.StartsWith("belt_"))
			{
				if (DBItemType == ItemType.MightyBelt) return GilesItemType.MightyBelt;
				return GilesItemType.Belt;
			}
			if (sThisInternalName.StartsWith("demonkey_") || sThisInternalName.StartsWith("demontrebuchetkey"))
			{
				return GilesItemType.InfernalKey;
			}

			return GilesItemType.Unknown;
		}

		// **********************************************************************************************
		// *****      DetermineBaseType - Calculates a more generic, "basic" type of item           *****
		// **********************************************************************************************
		internal static GilesBaseItemType DetermineBaseType(GilesItemType thisGilesItemType)
		{
			GilesBaseItemType thisGilesBaseType = GilesBaseItemType.Unknown;
			if (thisGilesItemType == GilesItemType.Axe || thisGilesItemType == GilesItemType.CeremonialKnife || thisGilesItemType == GilesItemType.Dagger ||
				 thisGilesItemType == GilesItemType.FistWeapon || thisGilesItemType == GilesItemType.Mace || thisGilesItemType == GilesItemType.MightyWeapon ||
				 thisGilesItemType == GilesItemType.Spear || thisGilesItemType == GilesItemType.Sword || thisGilesItemType == GilesItemType.Wand)
			{
				thisGilesBaseType = GilesBaseItemType.WeaponOneHand;
			}
			else if (thisGilesItemType == GilesItemType.TwoHandDaibo || thisGilesItemType == GilesItemType.TwoHandMace ||
				 thisGilesItemType == GilesItemType.TwoHandMighty || thisGilesItemType == GilesItemType.TwoHandPolearm || thisGilesItemType == GilesItemType.TwoHandStaff ||
				 thisGilesItemType == GilesItemType.TwoHandSword || thisGilesItemType == GilesItemType.TwoHandAxe)
			{
				thisGilesBaseType = GilesBaseItemType.WeaponTwoHand;
			}
			else if (thisGilesItemType == GilesItemType.TwoHandCrossbow || thisGilesItemType == GilesItemType.HandCrossbow || thisGilesItemType == GilesItemType.TwoHandBow)
			{
				thisGilesBaseType = GilesBaseItemType.WeaponRange;
			}
			else if (thisGilesItemType == GilesItemType.Mojo || thisGilesItemType == GilesItemType.Source ||
				 thisGilesItemType == GilesItemType.Quiver || thisGilesItemType == GilesItemType.Shield)
			{
				thisGilesBaseType = GilesBaseItemType.Offhand;
			}
			else if (thisGilesItemType == GilesItemType.Boots || thisGilesItemType == GilesItemType.Bracers || thisGilesItemType == GilesItemType.Chest ||
				 thisGilesItemType == GilesItemType.Cloak || thisGilesItemType == GilesItemType.Gloves || thisGilesItemType == GilesItemType.Helm ||
				 thisGilesItemType == GilesItemType.Pants || thisGilesItemType == GilesItemType.Shoulders || thisGilesItemType == GilesItemType.SpiritStone ||
				 thisGilesItemType == GilesItemType.VoodooMask || thisGilesItemType == GilesItemType.WizardHat || thisGilesItemType == GilesItemType.Belt ||
				 thisGilesItemType == GilesItemType.MightyBelt)
			{
				thisGilesBaseType = GilesBaseItemType.Armor;
			}
			else if (thisGilesItemType == GilesItemType.Amulet || thisGilesItemType == GilesItemType.Ring)
			{
				thisGilesBaseType = GilesBaseItemType.Jewelry;
			}
			else if (thisGilesItemType == GilesItemType.FollowerEnchantress || thisGilesItemType == GilesItemType.FollowerScoundrel ||
				 thisGilesItemType == GilesItemType.FollowerTemplar)
			{
				thisGilesBaseType = GilesBaseItemType.FollowerItem;
			}
			else if (thisGilesItemType == GilesItemType.CraftingMaterial || thisGilesItemType == GilesItemType.CraftTome || thisGilesItemType== GilesItemType.MiscBook ||
				 thisGilesItemType == GilesItemType.SpecialItem || thisGilesItemType == GilesItemType.CraftingPlan || thisGilesItemType == GilesItemType.HealthPotion ||
				 thisGilesItemType == GilesItemType.Dye || thisGilesItemType == GilesItemType.StaffOfHerding || thisGilesItemType == GilesItemType.InfernalKey)
			{
				thisGilesBaseType = GilesBaseItemType.Misc;
			}
			else if (thisGilesItemType == GilesItemType.Ruby || thisGilesItemType == GilesItemType.Emerald || thisGilesItemType == GilesItemType.Topaz ||
				 thisGilesItemType == GilesItemType.Amethyst)
			{
				thisGilesBaseType = GilesBaseItemType.Gem;
			}
			else if (thisGilesItemType == GilesItemType.HealthGlobe)
			{
				thisGilesBaseType = GilesBaseItemType.HealthGlobe;
			}
			return thisGilesBaseType;
		}

		// **********************************************************************************************
		// *****          DetermineIsStackable - Calculates what items can be stacked up            *****
		// **********************************************************************************************
		internal static bool DetermineIsStackable(GilesItemType thisGilesItemType)
		{
			bool bIsStackable = thisGilesItemType == GilesItemType.CraftingMaterial || thisGilesItemType == GilesItemType.CraftTome || thisGilesItemType == GilesItemType.Ruby ||
									  thisGilesItemType == GilesItemType.Emerald || thisGilesItemType == GilesItemType.Topaz || thisGilesItemType == GilesItemType.Amethyst ||
									  thisGilesItemType == GilesItemType.HealthPotion || thisGilesItemType == GilesItemType.CraftingPlan || thisGilesItemType == GilesItemType.Dye ||
									  thisGilesItemType == GilesItemType.InfernalKey;
			return bIsStackable;
		}
		internal static bool DetermineIsStackable(CacheACDItem item)
		{
			GilesItemType thisGilesItemType = DetermineItemType(item.ThisInternalName, item.ThisDBItemType, item.ThisFollowerType);
			bool bIsStackable = thisGilesItemType == GilesItemType.CraftingMaterial || thisGilesItemType == GilesItemType.CraftTome || thisGilesItemType == GilesItemType.Ruby ||
									  thisGilesItemType == GilesItemType.Emerald || thisGilesItemType == GilesItemType.Topaz || thisGilesItemType == GilesItemType.Amethyst ||
									  thisGilesItemType == GilesItemType.HealthPotion || thisGilesItemType == GilesItemType.CraftingPlan || thisGilesItemType == GilesItemType.Dye ||
									  thisGilesItemType == GilesItemType.InfernalKey;
			return bIsStackable;
		}

		// **********************************************************************************************
		// *****      DetermineIsTwoSlot - Tries to calculate what items take up 2 slots or 1       *****
		// **********************************************************************************************
		internal static bool DetermineIsTwoSlot(GilesItemType thisGilesItemType)
		{
			if (thisGilesItemType == GilesItemType.Axe || thisGilesItemType == GilesItemType.CeremonialKnife || thisGilesItemType == GilesItemType.Dagger ||
				 thisGilesItemType == GilesItemType.FistWeapon || thisGilesItemType == GilesItemType.Mace || thisGilesItemType == GilesItemType.MightyWeapon ||
				 thisGilesItemType == GilesItemType.Spear || thisGilesItemType == GilesItemType.Sword || thisGilesItemType == GilesItemType.Wand ||
				 thisGilesItemType == GilesItemType.TwoHandDaibo || thisGilesItemType == GilesItemType.TwoHandCrossbow || thisGilesItemType == GilesItemType.TwoHandMace ||
				 thisGilesItemType == GilesItemType.TwoHandMighty || thisGilesItemType == GilesItemType.TwoHandPolearm || thisGilesItemType == GilesItemType.TwoHandStaff ||
				 thisGilesItemType == GilesItemType.TwoHandSword || thisGilesItemType == GilesItemType.TwoHandAxe || thisGilesItemType == GilesItemType.HandCrossbow ||
				 thisGilesItemType == GilesItemType.TwoHandBow || thisGilesItemType == GilesItemType.Mojo || thisGilesItemType == GilesItemType.Source ||
				 thisGilesItemType == GilesItemType.Quiver || thisGilesItemType == GilesItemType.Shield || thisGilesItemType == GilesItemType.Boots ||
				 thisGilesItemType == GilesItemType.Bracers || thisGilesItemType == GilesItemType.Chest || thisGilesItemType == GilesItemType.Cloak ||
				 thisGilesItemType == GilesItemType.Gloves || thisGilesItemType == GilesItemType.Helm || thisGilesItemType == GilesItemType.Pants ||
				 thisGilesItemType == GilesItemType.Shoulders || thisGilesItemType == GilesItemType.SpiritStone ||
				 thisGilesItemType == GilesItemType.VoodooMask || thisGilesItemType == GilesItemType.WizardHat || thisGilesItemType == GilesItemType.StaffOfHerding)
				return true;
			return false;
		}

		// **********************************************************************************************
		// *****   This is for DemonBuddy error checking - see what sort of item DB THINKS it is    *****
		// **********************************************************************************************
		internal static ItemType GilesToDBItemType(GilesItemType thisgilesitemtype)
		{
			switch (thisgilesitemtype)
			{
				case GilesItemType.Axe: return ItemType.Axe;
				case GilesItemType.CeremonialKnife: return ItemType.CeremonialDagger;
				case GilesItemType.HandCrossbow: return ItemType.HandCrossbow;
				case GilesItemType.Dagger: return ItemType.Dagger;
				case GilesItemType.FistWeapon: return ItemType.FistWeapon;
				case GilesItemType.Mace: return ItemType.Mace;
				case GilesItemType.MightyWeapon: return ItemType.MightyWeapon;
				case GilesItemType.Spear: return ItemType.Spear;
				case GilesItemType.Sword: return ItemType.Sword;
				case GilesItemType.Wand: return ItemType.Wand;
				case GilesItemType.TwoHandAxe: return ItemType.Axe;
				case GilesItemType.TwoHandBow: return ItemType.Bow;
				case GilesItemType.TwoHandDaibo: return ItemType.Daibo;
				case GilesItemType.TwoHandCrossbow: return ItemType.Crossbow;
				case GilesItemType.TwoHandMace: return ItemType.Mace;
				case GilesItemType.TwoHandMighty: return ItemType.MightyWeapon;
				case GilesItemType.TwoHandPolearm: return ItemType.Polearm;
				case GilesItemType.TwoHandStaff: return ItemType.Staff;
				case GilesItemType.TwoHandSword: return ItemType.Sword;
				case GilesItemType.StaffOfHerding: return ItemType.Staff;
				case GilesItemType.Mojo: return ItemType.Mojo;
				case GilesItemType.Source: return ItemType.Orb;
				case GilesItemType.Quiver: return ItemType.Quiver;
				case GilesItemType.Shield: return ItemType.Shield;
				case GilesItemType.Amulet: return ItemType.Amulet;
				case GilesItemType.Ring: return ItemType.Ring;
				case GilesItemType.Belt: return ItemType.Belt;
				case GilesItemType.Boots: return ItemType.Boots;
				case GilesItemType.Bracers: return ItemType.Bracer;
				case GilesItemType.Chest: return ItemType.Chest;
				case GilesItemType.Cloak: return ItemType.Cloak;
				case GilesItemType.Gloves: return ItemType.Gloves;
				case GilesItemType.Helm: return ItemType.Helm;
				case GilesItemType.Pants: return ItemType.Legs;
				case GilesItemType.MightyBelt: return ItemType.MightyBelt;
				case GilesItemType.Shoulders: return ItemType.Shoulder;
				case GilesItemType.SpiritStone: return ItemType.SpiritStone;
				case GilesItemType.VoodooMask: return ItemType.VoodooMask;
				case GilesItemType.WizardHat: return ItemType.WizardHat;
				case GilesItemType.FollowerEnchantress: return ItemType.FollowerSpecial;
				case GilesItemType.FollowerScoundrel: return ItemType.FollowerSpecial;
				case GilesItemType.FollowerTemplar: return ItemType.FollowerSpecial;
				case GilesItemType.CraftingMaterial: return ItemType.CraftingReagent;
				case GilesItemType.CraftTome: return ItemType.CraftingPage;
				case GilesItemType.Ruby: return ItemType.Gem;
				case GilesItemType.Emerald: return ItemType.Gem;
				case GilesItemType.Topaz: return ItemType.Gem;
				case GilesItemType.Amethyst: return ItemType.Gem;
				case GilesItemType.SpecialItem: return ItemType.Unknown;
				case GilesItemType.CraftingPlan: return ItemType.CraftingPlan;
				case GilesItemType.HealthPotion: return ItemType.Potion;
				case GilesItemType.Dye: return ItemType.Unknown;
				case GilesItemType.InfernalKey: return ItemType.Unknown;
				case GilesItemType.MiscBook: return ItemType.CraftingPage;
			}
			return ItemType.Unknown;
		}

		#region Constants
		// These constants are used for item scoring and stashing
		private const int DEXTERITY = 0;
		private const int INTELLIGENCE = 1;
		private const int STRENGTH = 2;
		private const int VITALITY = 3;
		private const int LIFEPERCENT = 4;
		private const int LIFEONHIT = 5;
		private const int LIFESTEAL = 6;
		private const int LIFEREGEN = 7;
		private const int MAGICFIND = 8;
		private const int GOLDFIND = 9;
		private const int MOVEMENTSPEED = 10;
		private const int PICKUPRADIUS = 11;
		private const int SOCKETS = 12;
		private const int CRITCHANCE = 13;
		private const int CRITDAMAGE = 14;
		private const int ATTACKSPEED = 15;
		private const int MINDAMAGE = 16;
		private const int MAXDAMAGE = 17;
		private const int BLOCKCHANCE = 18;
		private const int THORNS = 19;
		private const int ALLRESIST = 20;
		private const int RANDOMRESIST = 21;
		private const int TOTALDPS = 22;
		private const int ARMOR = 23;
		private const int MAXDISCIPLINE = 24;
		private const int MAXMANA = 25;
		private const int ARCANECRIT = 26;
		private const int MANAREGEN = 27;
		private const int GLOBEBONUS = 28;
		private const int TOTALSTATS = 29; // starts at 0, remember... 0-26 = 1-27!

		private const int QUALITYWHITE = 0;
		private const int QUALITYBLUE = 1;
		private const int QUALITYYELLOW = 2;
		private const int QUALITYORANGE = 3;
		private const int GEMRUBY = 0;
		private const int GEMTOPAZ = 1;
		private const int GEMAMETHYST = 2;
		private const int GEMEMERALD = 3;
		#endregion


		internal static bool bOutputItemScores = false;

		// **********************************************************************************************
		// *****             Return the score needed to keep something by the item type             *****
		// **********************************************************************************************
		internal static double ScoreNeeded(GilesItemType thisGilesItemType)
		{
			double iThisNeedScore = 0;
			// Weapons
			if (thisGilesItemType == GilesItemType.Axe || thisGilesItemType == GilesItemType.CeremonialKnife || thisGilesItemType == GilesItemType.Dagger ||
				 thisGilesItemType == GilesItemType.FistWeapon || thisGilesItemType == GilesItemType.Mace || thisGilesItemType == GilesItemType.MightyWeapon ||
				 thisGilesItemType == GilesItemType.Spear || thisGilesItemType == GilesItemType.Sword || thisGilesItemType == GilesItemType.Wand ||
				 thisGilesItemType == GilesItemType.TwoHandDaibo || thisGilesItemType == GilesItemType.TwoHandCrossbow || thisGilesItemType == GilesItemType.TwoHandMace ||
				 thisGilesItemType == GilesItemType.TwoHandMighty || thisGilesItemType == GilesItemType.TwoHandPolearm || thisGilesItemType == GilesItemType.TwoHandStaff ||
				 thisGilesItemType == GilesItemType.TwoHandSword || thisGilesItemType == GilesItemType.TwoHandAxe || thisGilesItemType == GilesItemType.HandCrossbow || thisGilesItemType == GilesItemType.TwoHandBow)
				iThisNeedScore = Bot.Settings.Loot.GilesMinimumWeaponScore;
			// Jewelry
			if (thisGilesItemType == GilesItemType.Ring || thisGilesItemType == GilesItemType.Amulet || thisGilesItemType == GilesItemType.FollowerEnchantress ||
				 thisGilesItemType == GilesItemType.FollowerScoundrel || thisGilesItemType == GilesItemType.FollowerTemplar)
				iThisNeedScore = Bot.Settings.Loot.GilesMinimumJeweleryScore;

			// Armor
			if (thisGilesItemType == GilesItemType.Mojo || thisGilesItemType == GilesItemType.Source || thisGilesItemType == GilesItemType.Quiver ||
				 thisGilesItemType == GilesItemType.Shield || thisGilesItemType == GilesItemType.Belt || thisGilesItemType == GilesItemType.Boots ||
				 thisGilesItemType == GilesItemType.Bracers || thisGilesItemType == GilesItemType.Chest || thisGilesItemType == GilesItemType.Cloak ||
				 thisGilesItemType == GilesItemType.Gloves || thisGilesItemType == GilesItemType.Helm || thisGilesItemType == GilesItemType.Pants ||
				 thisGilesItemType == GilesItemType.MightyBelt || thisGilesItemType == GilesItemType.Shoulders || thisGilesItemType == GilesItemType.SpiritStone ||
				 thisGilesItemType == GilesItemType.VoodooMask || thisGilesItemType == GilesItemType.WizardHat)
				iThisNeedScore = Bot.Settings.Loot.GilesMinimumArmorScore;
			return Math.Round(iThisNeedScore);
		}

		// **********************************************************************************************
		// *****             The bizarre mystery function to score your lovely items!               *****
		// **********************************************************************************************
		internal static double ValueThisItem(CacheACDItem thisitem, GilesItemType thisGilesItemType)
		{
			double iTotalPoints = 0;
			bool bAbandonShip = true;
			double[] iThisItemsMaxStats = new double[TOTALSTATS];
			double[] iThisItemsMaxPoints = new double[TOTALSTATS];
			GilesBaseItemType thisGilesBaseType = DetermineBaseType(thisGilesItemType);

			#region CopyTotalStats

			// One Handed Weapons 
			if (thisGilesItemType == GilesItemType.Axe || thisGilesItemType == GilesItemType.CeremonialKnife || thisGilesItemType == GilesItemType.Dagger ||
				 thisGilesItemType == GilesItemType.FistWeapon || thisGilesItemType == GilesItemType.Mace || thisGilesItemType == GilesItemType.MightyWeapon ||
				 thisGilesItemType == GilesItemType.Spear || thisGilesItemType == GilesItemType.Sword || thisGilesItemType == GilesItemType.Wand)
			{
				Array.Copy(iMaxWeaponOneHand, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iWeaponPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Two Handed Weapons
			if (thisGilesItemType == GilesItemType.TwoHandAxe || thisGilesItemType == GilesItemType.TwoHandDaibo || thisGilesItemType == GilesItemType.TwoHandMace ||
				thisGilesItemType == GilesItemType.TwoHandMighty || thisGilesItemType == GilesItemType.TwoHandPolearm || thisGilesItemType == GilesItemType.TwoHandStaff ||
				thisGilesItemType == GilesItemType.TwoHandSword)
			{
				Array.Copy(iMaxWeaponTwoHand, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iWeaponPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Ranged Weapons
			if (thisGilesItemType == GilesItemType.TwoHandCrossbow || thisGilesItemType == GilesItemType.TwoHandBow || thisGilesItemType == GilesItemType.HandCrossbow)
			{
				Array.Copy(iMaxWeaponRanged, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iWeaponPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				if (thisGilesItemType == GilesItemType.HandCrossbow)
				{
					iThisItemsMaxStats[TOTALDPS] -= 150;
				}
				bAbandonShip = false;
			}
			// Off-handed stuff
			// Mojo, Source, Quiver
			if (thisGilesItemType == GilesItemType.Mojo || thisGilesItemType == GilesItemType.Source || thisGilesItemType == GilesItemType.Quiver)
			{
				Array.Copy(iMaxOffHand, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Shields
			if (thisGilesItemType == GilesItemType.Shield)
			{
				Array.Copy(iMaxShield, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Jewelry
			// Ring
			if (thisGilesItemType == GilesItemType.Amulet)
			{
				Array.Copy(iMaxAmulet, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iJewelryPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Ring
			if (thisGilesItemType == GilesItemType.Ring)
			{
				Array.Copy(iMaxRing, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iJewelryPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Armor
			// Belt
			if (thisGilesItemType == GilesItemType.Belt)
			{
				Array.Copy(iMaxBelt, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Boots
			if (thisGilesItemType == GilesItemType.Boots)
			{
				Array.Copy(iMaxBoots, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Bracers
			if (thisGilesItemType == GilesItemType.Bracers)
			{
				Array.Copy(iMaxBracer, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Chest
			if (thisGilesItemType == GilesItemType.Chest)
			{
				Array.Copy(iMaxChest, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			if (thisGilesItemType == GilesItemType.Cloak)
			{
				Array.Copy(iMaxCloak, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Gloves
			if (thisGilesItemType == GilesItemType.Gloves)
			{
				Array.Copy(iMaxGloves, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Helm
			if (thisGilesItemType == GilesItemType.Helm)
			{
				Array.Copy(iMaxHelm, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Pants
			if (thisGilesItemType == GilesItemType.Pants)
			{
				Array.Copy(iMaxPants, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			if (thisGilesItemType == GilesItemType.MightyBelt)
			{
				Array.Copy(iMaxMightyBelt, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Shoulders
			if (thisGilesItemType == GilesItemType.Shoulders)
			{
				Array.Copy(iMaxShoulders, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			if (thisGilesItemType == GilesItemType.SpiritStone)
			{
				Array.Copy(iMaxSpiritStone, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			if (thisGilesItemType == GilesItemType.VoodooMask)
			{
				Array.Copy(iMaxVoodooMask, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Wizard Hat
			if (thisGilesItemType == GilesItemType.WizardHat)
			{
				Array.Copy(iMaxWizardHat, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			// Follower Items
			if (thisGilesItemType == GilesItemType.FollowerEnchantress || thisGilesItemType == GilesItemType.FollowerScoundrel || thisGilesItemType == GilesItemType.FollowerTemplar)
			{
				Array.Copy(iMaxFollower, iThisItemsMaxStats, TOTALSTATS);
				Array.Copy(iJewelryPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
				bAbandonShip = false;
			}
			#endregion

			// Constants for convenient stat names
			double[] iHadStat = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			double[] iHadPoints = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

			double iSafeLifePercentage = 0;

			bool bSocketsCanReplacePrimaries = false;

			double iHighestScoringPrimary = 0;
			int iWhichPrimaryIsHighest = 0;
			double iAmountHighestScoringPrimary = 0;

			// Double safety check for unidentified items
			if (thisitem.IsUnidentified) bAbandonShip = true;

			// Make sure we got a valid item here, otherwise score it a big fat 0
			if (bAbandonShip)
			{

				return 0;
			}

			double iGlobalMultiplier = 1;

			TownRunManager.sValueItemStatString = "";
			TownRunManager.sJunkItemStatString = "";
			// We loop through all of the stats, in a particular order. The order *IS* important, because it pulls up primary stats first, BEFORE other stats
			for (int i = 0; i <= (TOTALSTATS - 1); i++)
			{
				double iTempStatistic = 0;
				// Now we lookup each stat on this item we are scoring, and store it in the variable "iTempStatistic" - which is used for calculations further down

				#region statSwitch
				switch (i)
				{
					case DEXTERITY: iTempStatistic = thisitem.Dexterity; break;
					case INTELLIGENCE: iTempStatistic = thisitem.Intelligence; break;
					case STRENGTH: iTempStatistic = thisitem.Strength; break;
					case VITALITY: iTempStatistic = thisitem.Vitality; break;
					case LIFEPERCENT: iTempStatistic = thisitem.LifePercent; break;
					case LIFEONHIT: iTempStatistic = thisitem.LifeOnHit; break;
					case LIFESTEAL: iTempStatistic = thisitem.LifeSteal; break;
					case LIFEREGEN: iTempStatistic = thisitem.HealthPerSecond; break;
					case MAGICFIND: iTempStatistic = thisitem.MagicFind; break;
					case GOLDFIND: iTempStatistic = thisitem.GoldFind; break;
					case MOVEMENTSPEED: iTempStatistic = thisitem.MovementSpeed; break;
					case PICKUPRADIUS: iTempStatistic = thisitem.PickUpRadius; break;
					case SOCKETS: iTempStatistic = thisitem.Sockets; break;
					case CRITCHANCE: iTempStatistic = thisitem.CritPercent; break;
					case CRITDAMAGE: iTempStatistic = thisitem.CritDamagePercent; break;
					case ATTACKSPEED: iTempStatistic = thisitem.AttackSpeedPercent; break;
					case MINDAMAGE: iTempStatistic = thisitem.MinDamage; break;
					case MAXDAMAGE: iTempStatistic = thisitem.MaxDamage; break;
					case BLOCKCHANCE: iTempStatistic = thisitem.BlockChance; break;
					case THORNS: iTempStatistic = thisitem.Thorns; break;
					case ALLRESIST: iTempStatistic = thisitem.ResistAll; break;
					case RANDOMRESIST:
						if (thisitem.ResistArcane > iTempStatistic) iTempStatistic = thisitem.ResistArcane;
						if (thisitem.ResistCold > iTempStatistic) iTempStatistic = thisitem.ResistCold;
						if (thisitem.ResistFire > iTempStatistic) iTempStatistic = thisitem.ResistFire;
						if (thisitem.ResistHoly > iTempStatistic) iTempStatistic = thisitem.ResistHoly;
						if (thisitem.ResistLightning > iTempStatistic) iTempStatistic = thisitem.ResistLightning;
						if (thisitem.ResistPhysical > iTempStatistic) iTempStatistic = thisitem.ResistPhysical;
						if (thisitem.ResistPoison > iTempStatistic) iTempStatistic = thisitem.ResistPoison;
						break;
					case TOTALDPS: iTempStatistic = thisitem.WeaponDamagePerSecond; break;
					case ARMOR: iTempStatistic = thisitem.ArmorBonus; break;
					case MAXDISCIPLINE: iTempStatistic = thisitem.MaxDiscipline; break;
					case MAXMANA: iTempStatistic = thisitem.MaxMana; break;
					case ARCANECRIT: iTempStatistic = thisitem.ArcaneOnCrit; break;
					case MANAREGEN: iTempStatistic = thisitem.ManaRegen; break;
					case GLOBEBONUS: iTempStatistic = thisitem.GlobeBonus; break;
				}
				#endregion

				iHadStat[i] = iTempStatistic;
				iHadPoints[i] = 0;
				// Now we check that the current statistic in the "for" loop, actually exists on this item, and is a stat we are measuring (has >0 in the "max stats" array)
				if (iThisItemsMaxStats[i] > 0 && iTempStatistic > 0)
				#region AttributeScoring
				{
					// Final bonus granted is an end-of-score multiplier. 1 = 100%, so all items start off with 100%, of course!
					double iFinalBonusGranted = 1;

					// Temp percent is what PERCENTAGE of the *MAXIMUM POSSIBLE STAT*, this stat is at.
					// Note that stats OVER the max will get a natural score boost, since this value will be over 1!
					double iTempPercent = iTempStatistic / iThisItemsMaxStats[i];
					// Now multiply the "max points" value, by that percentage, as the start/basis of the scoring for this statistic
					double iTempPoints = iThisItemsMaxPoints[i] * iTempPercent;

					// Check if this statistic is over the "bonus threshold" array value for this stat - if it is, then it gets a score bonus when over a certain % of max-stat
					if (iTempPercent > iBonusThreshold[i] && iBonusThreshold[i] > 0f)
					{
						iFinalBonusGranted += ((iTempPercent - iBonusThreshold[i]) * 0.9);
					}

					// We're going to store the life % stat here for quick-calculations against other stats. Don't edit this bit!
					if (i == LIFEPERCENT)
					{
						if (iThisItemsMaxStats[LIFEPERCENT] > 0)
						{
							iSafeLifePercentage = (iTempStatistic / iThisItemsMaxStats[LIFEPERCENT]);
						}
						else
						{
							iSafeLifePercentage = 0;
						}
					}

					// This *REMOVES* score from follower items for stats that followers don't care about
					if (thisGilesBaseType == GilesBaseItemType.FollowerItem && (i == CRITDAMAGE || i == LIFEONHIT || i == ALLRESIST))
						iFinalBonusGranted -= 0.9;

					// Bonus 15% for being *at* the stat cap (ie - completely maxed out, or very very close to), but not for the socket stat (since sockets are usually 0 or 1!)
					if (i != SOCKETS)
					{
						if ((iTempStatistic / iThisItemsMaxStats[i]) >= 0.99)
							iFinalBonusGranted += 0.15;
						// Else bonus 10% for being in final 95%
						else if ((iTempStatistic / iThisItemsMaxStats[i]) >= 0.95)
							iFinalBonusGranted += 0.10;
					}

					// ***************
					// Socket handling
					// ***************
					// Sockets give special bonuses for certain items, depending how close to the max-socket-count it is for that item
					// It also enables bonus scoring for stats which usually rely on a high primary stat - since a socket can make up for a lack of a high primary (you can socket a +primary stat!)
					if (i == SOCKETS)
					{
						// Off-handers get less value from sockets
						if (thisGilesBaseType == GilesBaseItemType.Offhand)
						{
							iFinalBonusGranted -= 0.35;
						}

						// Chest
						if (thisGilesItemType == GilesItemType.Chest || thisGilesItemType == GilesItemType.Cloak)
						{
							if (iTempStatistic >= 2)
							{
								bSocketsCanReplacePrimaries = true;
								if (iTempStatistic >= 3)
									iFinalBonusGranted += 0.25;
							}
						}

						// Pants
						if (thisGilesItemType == GilesItemType.Pants)
						{
							if (iTempStatistic >= 2)
							{
								bSocketsCanReplacePrimaries = true;
								iFinalBonusGranted += 0.25;
							}
						}
						// Helmets can have a bonus for a socket since it gives amazing MF/GF
						if (iTempStatistic >= 1 && (thisGilesItemType == GilesItemType.Helm || thisGilesItemType == GilesItemType.WizardHat || thisGilesItemType == GilesItemType.VoodooMask ||
							thisGilesItemType == GilesItemType.SpiritStone))
						{
							bSocketsCanReplacePrimaries = true;
						}
						// And rings and amulets too
						if (iTempStatistic >= 1 && (thisGilesItemType == GilesItemType.Ring || thisGilesItemType == GilesItemType.Amulet))
						{
							bSocketsCanReplacePrimaries = true;
						}

					}

					// Right, here's quite a long bit of code, but this is basically all about granting all sorts of bonuses based on primary stat values of all different ranges
					// For all item types *EXCEPT* weapons
					if (thisGilesBaseType != GilesBaseItemType.WeaponRange && thisGilesBaseType != GilesBaseItemType.WeaponOneHand && thisGilesBaseType != GilesBaseItemType.WeaponTwoHand)
					{
						double iSpecialBonus = 0;
						if (i > LIFEPERCENT)
						{
							// Knock off points for being particularly low
							if ((iTempStatistic / iThisItemsMaxStats[i]) < 0.2 && (iBonusThreshold[i] <= 0f || iBonusThreshold[i] >= 0.2))
								iFinalBonusGranted -= 0.35;
							else if ((iTempStatistic / iThisItemsMaxStats[i]) < 0.4 && (iBonusThreshold[i] <= 0f || iBonusThreshold[i] >= 0.4))
								iFinalBonusGranted -= 0.15;
							// Remove 80% if below minimum threshold
							if ((iTempStatistic / iThisItemsMaxStats[i]) < iMinimumThreshold[i] && iMinimumThreshold[i] > 0f)
								iFinalBonusGranted -= 0.8;

							// Primary stat/vitality minimums or zero-check reductions on other stats
							if (iStatMinimumPrimary[i] > 0)
							{
								// Remove 40% from all stats if there is no prime stat present or vitality/life present and this is below 90% of max
								if (((iTempStatistic / iThisItemsMaxStats[i]) < .90) && ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) < iStatMinimumPrimary[i]) &&
									((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) < (iStatMinimumPrimary[i] + 0.1)) && ((iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) < iStatMinimumPrimary[i]) &&
									((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) < iStatMinimumPrimary[i]) && (iSafeLifePercentage < (iStatMinimumPrimary[i] * 2.5)) && !bSocketsCanReplacePrimaries)
								{
									if (thisGilesItemType != GilesItemType.Ring && thisGilesItemType != GilesItemType.Amulet)
										iFinalBonusGranted -= 0.4;
									else
										iFinalBonusGranted -= 0.3;
									// And another 25% off for armor and all resist which are more useful with primaries, as long as not jewelry
									if ((i == ARMOR || i == ALLRESIST || i == RANDOMRESIST) && thisGilesItemType != GilesItemType.Ring && thisGilesItemType != GilesItemType.Amulet && !bSocketsCanReplacePrimaries)
										iFinalBonusGranted -= 0.15;
								}
							}
							else
							{
								// Almost no primary stats or health at all
								if (iHadStat[DEXTERITY] <= 60 && iHadStat[STRENGTH] <= 60 && iHadStat[INTELLIGENCE] <= 60 && iHadStat[VITALITY] <= 60 && iSafeLifePercentage < 0.9 && !bSocketsCanReplacePrimaries)
								{
									// So 35% off for all items except jewelry which is 20% off
									if (thisGilesItemType != GilesItemType.Ring && thisGilesItemType != GilesItemType.Amulet)
									{
										iFinalBonusGranted -= 0.35;
										// And another 25% off for armor and all resist which are more useful with primaries
										if (i == ARMOR || i == ALLRESIST)
											iFinalBonusGranted -= 0.15;
									}
									else
									{
										iFinalBonusGranted -= 0.20;
									}

								}
							}

							if (thisGilesBaseType == GilesBaseItemType.Armor || thisGilesBaseType == GilesBaseItemType.Jewelry)
							{
								// Grant a 50% bonus to stats if a primary is above 200 AND (vitality above 200 or life% within 90% max)
								if ((iHadStat[DEXTERITY] > 200 || iHadStat[STRENGTH] > 200 || iHadStat[INTELLIGENCE] > 200) && (iHadStat[VITALITY] > 200 || iSafeLifePercentage > .97))
								{
									if (0.5 > iSpecialBonus) iSpecialBonus = 0.5;
								}
								// Else grant a 40% bonus to stats if a primary is above 200
								if (iHadStat[DEXTERITY] > 200 || iHadStat[STRENGTH] > 200 || iHadStat[INTELLIGENCE] > 200)
								{
									if (0.4 > iSpecialBonus) iSpecialBonus = 0.4;
								}
								// Grant a 30% bonus if vitality > 200 or life percent within 90% of max
								if (iHadStat[VITALITY] > 200 || iSafeLifePercentage > .97)
								{
									if (0.3 > iSpecialBonus) iSpecialBonus = 0.3;
								}
							}

							// Checks for various primary & health levels
							if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .85 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .85
								|| (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .85)
							{
								if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
								{
									if (0.5 > iSpecialBonus) iSpecialBonus = 0.5;
								}
								else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
								{
									if (0.4 > iSpecialBonus) iSpecialBonus = 0.4;
								}
								else
								{
									if (0.2 > iSpecialBonus) iSpecialBonus = 0.2;
								}
							}
							if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .75 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .75
								|| (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .75)
							{
								if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
								{
									if (0.35 > iSpecialBonus) iSpecialBonus = 0.35;
								}
								else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
								{
									if (0.30 > iSpecialBonus) iSpecialBonus = 0.30;
								}
								else
								{
									if (0.15 > iSpecialBonus) iSpecialBonus = 0.15;
								}
							}
							if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .65 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .65
								|| (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .65)
							{
								if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
								{
									if (0.26 > iSpecialBonus) iSpecialBonus = 0.26;
								}
								else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
								{
									if (0.22 > iSpecialBonus) iSpecialBonus = 0.22;
								}
								else
								{
									if (0.11 > iSpecialBonus) iSpecialBonus = 0.11;
								}
							}
							if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .55 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .55
								|| (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .55)
							{
								if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
								{
									if (0.18 > iSpecialBonus) iSpecialBonus = 0.18;
								}
								else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
								{
									if (0.14 > iSpecialBonus) iSpecialBonus = 0.14;
								}
								else
								{
									if (0.08 > iSpecialBonus) iSpecialBonus = 0.08;
								}
							}
							if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .5 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .5
								|| (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .5)
							{
								if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
								{
									if (0.12 > iSpecialBonus) iSpecialBonus = 0.12;
								}
								else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
								{
									if (0.05 > iSpecialBonus) iSpecialBonus = 0.05;
								}
								else
								{
									if (0.03 > iSpecialBonus) iSpecialBonus = 0.03;
								}
							}
							if (thisGilesItemType == GilesItemType.Ring || thisGilesItemType == GilesItemType.Amulet)
							{
								if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .4 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .4
									|| (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .4)
								{
									if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
									{
										if (0.10 > iSpecialBonus) iSpecialBonus = 0.10;
									}
									else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
									{
										if (0.08 > iSpecialBonus) iSpecialBonus = 0.08;
									}
									else
									{
										if (0.05 > iSpecialBonus) iSpecialBonus = 0.05;
									}
								}
							}
							if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .8 || iSafeLifePercentage > .98)
							{
								if (0.20 > iSpecialBonus) iSpecialBonus = 0.20;
							}
							if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .7 || iSafeLifePercentage > .95)
							{
								if (0.16 > iSpecialBonus) iSpecialBonus = 0.16;
							}
							if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .92)
							{
								if (0.12 > iSpecialBonus) iSpecialBonus = 0.12;
							}
							if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .55 || iSafeLifePercentage > .89)
							{
								if (0.07 > iSpecialBonus) iSpecialBonus = 0.07;
							}
							else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .5 || iSafeLifePercentage > .87)
							{
								if (0.05 > iSpecialBonus) iSpecialBonus = 0.05;
							}
							else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .45 || iSafeLifePercentage > .86)
							{
								if (0.02 > iSpecialBonus) iSpecialBonus = 0.02;
							}
						} // This stat is one after life percent stat
						// Shields get less of a special bonus from high prime stats
						if (thisGilesItemType == GilesItemType.Shield)
							iSpecialBonus *= 0.7;

						iFinalBonusGranted += iSpecialBonus;
					} // NOT A WEAPON!?

					// Knock off points for being particularly low
					if ((iTempStatistic / iThisItemsMaxStats[i]) < iMinimumThreshold[i] && iMinimumThreshold[i] > 0f)
						iFinalBonusGranted -= 0.35;
					// Grant a 20% bonus to vitality or Life%, for being paired with any prime stat above minimum threshold +.1
					if (((i == VITALITY && (iTempStatistic / iThisItemsMaxStats[VITALITY]) > iMinimumThreshold[VITALITY]) ||
						  i == LIFEPERCENT && (iTempStatistic / iThisItemsMaxStats[LIFEPERCENT]) > iMinimumThreshold[LIFEPERCENT]) &&
						((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > (iMinimumThreshold[DEXTERITY] + 0.1)
						|| (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > (iMinimumThreshold[STRENGTH] + 0.1) ||
						 (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > (iMinimumThreshold[INTELLIGENCE] + 0.1)))
						iFinalBonusGranted += 0.2;

					// Blue item point reduction for non-weapons
					if (thisitem.ThisQuality < ItemQuality.Rare4 && (thisGilesBaseType == GilesBaseItemType.Armor || thisGilesBaseType == GilesBaseItemType.Offhand ||
						thisGilesBaseType == GilesBaseItemType.Jewelry || thisGilesBaseType == GilesBaseItemType.FollowerItem) && ((iTempStatistic / iThisItemsMaxStats[i]) < 0.88))
						iFinalBonusGranted -= 0.9;

					// Special all-resist bonuses
					if (i == ALLRESIST)
					{
						// Shields with < 60% max all resist, lost some all resist score
						if (thisGilesItemType == GilesItemType.Shield && (iTempStatistic / iThisItemsMaxStats[i]) <= 0.6)
							iFinalBonusGranted -= 0.30;

						double iSpecialBonus = 0;
						// All resist gets a special bonus if paired with good strength and some vitality
						if ((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > 0.7 && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > 0.3)
							if (0.45 > iSpecialBonus) iSpecialBonus = 0.45;
						// All resist gets a smaller special bonus if paired with good dexterity and some vitality
						if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > 0.7 && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > 0.3)
							if (0.35 > iSpecialBonus) iSpecialBonus = 0.35;
						// All resist gets a slight special bonus if paired with good intelligence and some vitality
						if ((iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > 0.7 && (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > 0.3)
							if (0.25 > iSpecialBonus) iSpecialBonus = 0.25;

						// Smaller bonuses for smaller stats
						// All resist gets a special bonus if paired with good strength and some vitality
						if ((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > 0.55 && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > 0.3)
							if (0.45 > iSpecialBonus) iSpecialBonus = 0.20;
						// All resist gets a smaller special bonus if paired with good dexterity and some vitality
						if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > 0.55 && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > 0.3)
							if (0.35 > iSpecialBonus) iSpecialBonus = 0.15;
						// All resist gets a slight special bonus if paired with good intelligence and some vitality
						if ((iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > 0.55 && (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > 0.3)
							if (0.25 > iSpecialBonus) iSpecialBonus = 0.10;

						// This stat is one after life percent stat
						iFinalBonusGranted += iSpecialBonus;

						// Global bonus to everything
						if ((iThisItemsMaxStats[i] - iTempStatistic) < 10.2f)
							iGlobalMultiplier += 0.05;
					} // All resist special bonuses

					if (thisGilesItemType != GilesItemType.Ring && thisGilesItemType != GilesItemType.Amulet)
					{
						// Shields get 10% less on everything
						if (thisGilesItemType == GilesItemType.Shield)
							iFinalBonusGranted -= 0.10;

						// Prime stat gets a 20% bonus if 50 from max possible
						if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE || i == VITALITY) && (iThisItemsMaxStats[i] - iTempStatistic) < 50.5f)
							iFinalBonusGranted += 0.25;

						// Reduce a prime stat by 75% if less than 100 *OR* less than 50% max
						if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE) && (iTempStatistic < 100 || ((iTempStatistic / iThisItemsMaxStats[i]) < 0.5)))
							iFinalBonusGranted -= 0.75;
						// Reduce a vitality/life% stat by 60% if less than 80 vitality/less than 60% max possible life%
						if ((i == VITALITY && iTempStatistic < 80) || (i == LIFEPERCENT && ((iTempStatistic / iThisItemsMaxStats[LIFEPERCENT]) < 0.6)))
							iFinalBonusGranted -= 0.6;
						// Grant 10% to any 4 main stat above 200
						if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE || i == VITALITY) && iTempStatistic > 200)
							iFinalBonusGranted += 0.1;

						// *************************************************
						// Special stat handling stuff for non-jewelry types
						// *************************************************
						// Within 2 block chance
						if (i == BLOCKCHANCE && (iThisItemsMaxStats[i] - iTempStatistic) < 2.3f)
							iFinalBonusGranted += 1;

						// Within final 5 gold find
						if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 5.3f)
						{
							iFinalBonusGranted += 0.04;
							// Even bigger bonus if got prime stat & vit
							if (((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > iMinimumThreshold[DEXTERITY] || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH] ||
								(iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > iMinimumThreshold[INTELLIGENCE]) && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > iMinimumThreshold[VITALITY])
								iFinalBonusGranted += 0.02;
						}
						// Within final 3 gold find
						if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 3.3f)
						{
							iFinalBonusGranted += 0.04;
						}
						// Within final 2 gold find
						if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 2.3f)
						{
							iFinalBonusGranted += 0.05;
						}
						// Within final 3 magic find
						if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 3.3f)
							iFinalBonusGranted += 0.08;
						// Within final 2 magic find
						if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 2.3f)
						{
							iFinalBonusGranted += 0.04;
							// Even bigger bonus if got prime stat & vit
							if (((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > iMinimumThreshold[DEXTERITY] || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH] ||
								(iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > iMinimumThreshold[INTELLIGENCE]) && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > iMinimumThreshold[VITALITY])
								iFinalBonusGranted += 0.03;
						}
						// Within final magic find
						if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 1.3f)
						{
							iFinalBonusGranted += 0.05;
						}
						// Within final 10 all resist
						if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) < 10.2f)
						{
							iFinalBonusGranted += 0.05;
							// Even bigger bonus if got prime stat & vit
							if (((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > iMinimumThreshold[DEXTERITY] || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH] ||
								(iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > iMinimumThreshold[INTELLIGENCE]) && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > iMinimumThreshold[VITALITY])
								iFinalBonusGranted += 0.20;
						}
						// Within final 50 armor
						if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 50.2f)
						{
							iFinalBonusGranted += 0.10;
							if ((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH])
								iFinalBonusGranted += 0.10;
						}
						// Within final 15 armor
						if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 15.2f)
							iFinalBonusGranted += 0.15;

						// Within final 5 critical hit damage
						if (i == CRITDAMAGE && (iThisItemsMaxStats[i] - iTempStatistic) < 5.2f)
							iFinalBonusGranted += 0.25;
						// More than 2.5 crit chance out
						if (i == CRITCHANCE && (iThisItemsMaxStats[i] - iTempStatistic) > 2.45f)
							iFinalBonusGranted -= 0.35;
						// More than 20 crit damage out
						if (i == CRITDAMAGE && (iThisItemsMaxStats[i] - iTempStatistic) > 19.95f)
							iFinalBonusGranted -= 0.35;
						// More than 2 attack speed out
						if (i == ATTACKSPEED && (iThisItemsMaxStats[i] - iTempStatistic) > 1.95f)
							iFinalBonusGranted -= 0.35;
						// More than 2 move speed
						if (i == MOVEMENTSPEED && (iThisItemsMaxStats[i] - iTempStatistic) > 1.95f)
							iFinalBonusGranted -= 0.35;
						// More than 5 gold find out
						if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 5.2f)
							iFinalBonusGranted -= 0.40;
						// More than 8 gold find out
						if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 8.2f)
							iFinalBonusGranted -= 0.1;
						// More than 5 magic find out
						if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 5.2f)
							iFinalBonusGranted -= 0.40;
						// More than 7 magic find out
						if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 7.2f)
							iFinalBonusGranted -= 0.1;
						// More than 20 all resist out
						if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 20.2f)
							iFinalBonusGranted -= 0.50;
						// More than 30 all resist out
						if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 30.2f)
							iFinalBonusGranted -= 0.20;
					}
					// And now for jewelry checks...
					else
					{
						// Global bonus to everything if jewelry has an all resist above 50%
						if (i == ALLRESIST && (iTempStatistic / iThisItemsMaxStats[i]) > 0.5)
							iGlobalMultiplier += 0.08;
						// Within final 10 all resist
						if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) < 10.2f)
							iFinalBonusGranted += 0.10;

						// Within final 5 critical hit damage
						if (i == CRITDAMAGE && (iThisItemsMaxStats[i] - iTempStatistic) < 5.2f)
							iFinalBonusGranted += 0.25;

						// Within 3 block chance
						if (i == BLOCKCHANCE && (iThisItemsMaxStats[i] - iTempStatistic) < 3.3f)
							iFinalBonusGranted += 0.15;

						// Reduce a prime stat by 60% if less than 60
						if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE) && (iTempStatistic < 60 || ((iTempStatistic / iThisItemsMaxStats[i]) < 0.3)))
							iFinalBonusGranted -= 0.6;
						// Reduce a vitality/life% stat by 50% if less than 50 vitality/less than 40% max possible life%
						if ((i == VITALITY && iTempStatistic < 50) || (i == LIFEPERCENT && ((iTempStatistic / iThisItemsMaxStats[LIFEPERCENT]) < 0.4)))
							iFinalBonusGranted -= 0.5;
						// Grant 20% to any 4 main stat above 150
						if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE || i == VITALITY) && iTempStatistic > 150)
							iFinalBonusGranted += 0.2;


						// ***************************************
						// Special stat handling stuff for jewelry
						// ***************************************
						if (thisGilesItemType == GilesItemType.Ring)
						{
							// Prime stat gets a 25% bonus if 30 from max possible
							if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE || i == VITALITY) && (iThisItemsMaxStats[i] - iTempStatistic) < 30.5f)
								iFinalBonusGranted += 0.25;

							// Within final 5 magic find
							if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 5.2f)
								iFinalBonusGranted += 0.4;
							// Within final 5 gold find
							if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 5.2f)
								iFinalBonusGranted += 0.35;
							// Within final 45 life on hit
							if (i == LIFEONHIT && (iThisItemsMaxStats[i] - iTempStatistic) < 45.2f)
								iFinalBonusGranted += 1.2;
							// Within final 50 armor
							if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 50.2f)
							{
								iFinalBonusGranted += 0.30;
								if ((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH])
									iFinalBonusGranted += 0.30;
							}
							// Within final 15 armor
							if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 15.2f)
								iFinalBonusGranted += 0.20;

							// More than 2.5 crit chance out
							if (i == CRITCHANCE && (iThisItemsMaxStats[i] - iTempStatistic) > 5.55f)
								iFinalBonusGranted -= 0.20;
							// More than 20 crit damage out
							if (i == CRITDAMAGE && (iThisItemsMaxStats[i] - iTempStatistic) > 19.95f)
								iFinalBonusGranted -= 0.20;
							// More than 2 attack speed out
							if (i == ATTACKSPEED && (iThisItemsMaxStats[i] - iTempStatistic) > 1.95f)
								iFinalBonusGranted -= 0.20;
							// More than 15 gold find out
							if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 15.2f)
								iFinalBonusGranted -= 0.1;
							// More than 15 magic find out
							if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 15.2f)
								iFinalBonusGranted -= 0.1;
							// More than 30 all resist out
							if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 20.2f)
								iFinalBonusGranted -= 0.1;
							// More than 40 all resist out
							if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 30.2f)
								iFinalBonusGranted -= 0.1;
						}
						else
						{
							// Prime stat gets a 25% bonus if 60 from max possible
							if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE || i == VITALITY) && (iThisItemsMaxStats[i] - iTempStatistic) < 60.5f)
								iFinalBonusGranted += 0.25;

							// Within final 10 magic find
							if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 10.2f)
								iFinalBonusGranted += 0.4;
							// Within final 10 gold find
							if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 10.2f)
								iFinalBonusGranted += 0.35;
							// Within final 40 life on hit
							if (i == LIFEONHIT && (iThisItemsMaxStats[i] - iTempStatistic) < 40.2f)
								iFinalBonusGranted += 1.2;
							// Within final 50 armor
							if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 50.2f)
							{
								iFinalBonusGranted += 0.30;
								if ((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH])
									iFinalBonusGranted += 0.30;
							}
							// Within final 15 armor
							if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 15.2f)
								iFinalBonusGranted += 0.20;

							// More than 2.5 crit chance out
							if (i == CRITCHANCE && (iThisItemsMaxStats[i] - iTempStatistic) > 5.55f)
								iFinalBonusGranted -= 0.20;
							// More than 20 crit damage out
							if (i == CRITDAMAGE && (iThisItemsMaxStats[i] - iTempStatistic) > 19.95f)
								iFinalBonusGranted -= 0.20;
							// More than 2 attack speed out
							if (i == ATTACKSPEED && (iThisItemsMaxStats[i] - iTempStatistic) > 1.95f)
								iFinalBonusGranted -= 0.20;
							// More than 15 gold find out
							if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 15.2f)
								iFinalBonusGranted -= 0.1;
							// More than 15 magic find out
							if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 15.2f)
								iFinalBonusGranted -= 0.1;
							// More than 30 all resist out
							if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 20.2f)
								iFinalBonusGranted -= 0.1;
							// More than 40 all resist out
							if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 30.2f)
								iFinalBonusGranted -= 0.1;
						}
					}

					// *****************************
					// All the "set to 0" checks now
					// *****************************

					// Disable specific primary stat scoring for certain class-specific item types
					if ((thisGilesItemType == GilesItemType.VoodooMask || thisGilesItemType == GilesItemType.WizardHat || thisGilesItemType == GilesItemType.Wand ||
						thisGilesItemType == GilesItemType.CeremonialKnife || thisGilesItemType == GilesItemType.Mojo || thisGilesItemType == GilesItemType.Source)
						&& (i == STRENGTH || i == DEXTERITY))
						iFinalBonusGranted = 0;
					if ((thisGilesItemType == GilesItemType.Quiver || thisGilesItemType == GilesItemType.HandCrossbow || thisGilesItemType == GilesItemType.Cloak ||
						thisGilesItemType == GilesItemType.SpiritStone || thisGilesItemType == GilesItemType.TwoHandDaibo || thisGilesItemType == GilesItemType.FistWeapon)
						&& (i == STRENGTH || i == INTELLIGENCE))
						iFinalBonusGranted = 0;
					if ((thisGilesItemType == GilesItemType.MightyBelt || thisGilesItemType == GilesItemType.MightyWeapon || thisGilesItemType == GilesItemType.TwoHandMighty)
						&& (i == DEXTERITY || i == INTELLIGENCE))
						iFinalBonusGranted = 0;
					// Remove unwanted follower stats for specific follower types
					if (thisGilesItemType == GilesItemType.FollowerEnchantress && (i == STRENGTH || i == DEXTERITY))
						iFinalBonusGranted = 0;
					if (thisGilesItemType == GilesItemType.FollowerEnchantress && (i == INTELLIGENCE || i == VITALITY))
						iFinalBonusGranted -= 0.4;
					if (thisGilesItemType == GilesItemType.FollowerScoundrel && (i == STRENGTH || i == INTELLIGENCE))
						iFinalBonusGranted = 0;
					if (thisGilesItemType == GilesItemType.FollowerScoundrel && (i == DEXTERITY || i == VITALITY))
						iFinalBonusGranted -= 0.4;
					if (thisGilesItemType == GilesItemType.FollowerTemplar && (i == DEXTERITY || i == INTELLIGENCE))
						iFinalBonusGranted = 0;
					if (thisGilesItemType == GilesItemType.FollowerTemplar && (i == STRENGTH || i == VITALITY))
						iFinalBonusGranted -= 0.4;
					// Attack speed is always on a quiver so forget it
					if ((thisGilesItemType == GilesItemType.Quiver) && (i == ATTACKSPEED))
						iFinalBonusGranted = 0;
					// Single resists worth nothing without all-resist
					if (i == RANDOMRESIST && (iHadStat[ALLRESIST] / iThisItemsMaxStats[ALLRESIST]) < iMinimumThreshold[ALLRESIST])
						iFinalBonusGranted = 0;

					if (iFinalBonusGranted < 0)
						iFinalBonusGranted = 0;

					// ***************************
					// Grant the final bonus total
					// ***************************
					iTempPoints *= iFinalBonusGranted;

					// If it's a primary stat, log the highest scoring primary... else add these points to the running total
					if (i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE)
					{
						if (iTempPoints > iHighestScoringPrimary)
						{
							iHighestScoringPrimary = iTempPoints;
							iWhichPrimaryIsHighest = i;
							iAmountHighestScoringPrimary = iTempStatistic;
						}
					}
					else
					{

						iTotalPoints += iTempPoints;
					}

					iHadPoints[i] = iTempPoints;

					// For item logs
					if (i != DEXTERITY && i != STRENGTH && i != INTELLIGENCE)
					{
						if (String.IsNullOrEmpty(TownRunManager.sValueItemStatString))
							TownRunManager.sValueItemStatString += ". ";
						TownRunManager.sValueItemStatString += StatNames[i] + "=" + Math.Round(iTempStatistic);
						if (!String.IsNullOrEmpty(TownRunManager.sJunkItemStatString))
							TownRunManager.sJunkItemStatString += ". ";
						TownRunManager.sJunkItemStatString += StatNames[i] + "=" + Math.Round(iTempStatistic);
					}
				}
				#endregion
			} // End of main 0-TOTALSTATS stat loop



			int iTotalRequirements;
			// Now add on one of the three primary stat scores, whichever was higher
			if (iHighestScoringPrimary > 0)
			{
				// Give a 30% of primary-stat-score-possible bonus to the primary scoring if paired with a good amount of life % or vitality
				if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY] > (iMinimumThreshold[VITALITY] + 0.1)) || iSafeLifePercentage > 0.85)
					iHighestScoringPrimary += iThisItemsMaxPoints[iWhichPrimaryIsHighest] * 0.3;
				// Reduce a primary a little if there is no vitality or life
				if ((iHadStat[VITALITY] < 40) || iSafeLifePercentage < 0.7)
					iHighestScoringPrimary *= 0.8;

				iTotalPoints += iHighestScoringPrimary;
				TownRunManager.sValueItemStatString = StatNames[iWhichPrimaryIsHighest] + "=" + Math.Round(iAmountHighestScoringPrimary) + ". " + TownRunManager.sValueItemStatString;
				TownRunManager.sJunkItemStatString = StatNames[iWhichPrimaryIsHighest] + "=" + Math.Round(iAmountHighestScoringPrimary) + ". " + TownRunManager.sJunkItemStatString;
			}


			// Global multiplier
			iTotalPoints *= iGlobalMultiplier;

			// 2 handed weapons and ranged weapons lose a large score for low DPS
			if (thisGilesBaseType == GilesBaseItemType.WeaponRange || thisGilesBaseType == GilesBaseItemType.WeaponTwoHand)
			{
				if ((iHadStat[TOTALDPS] / iThisItemsMaxStats[TOTALDPS]) <= 0.7)
					iTotalPoints *= 0.75;
			}
			else if (thisGilesBaseType == GilesBaseItemType.WeaponOneHand)
			{
				if ((iHadStat[TOTALDPS] / iThisItemsMaxStats[TOTALDPS]) < 0.6)
					iTotalPoints *= 0.75;
			}

			// Weapons should get a nice 15% bonus score for having very high primaries
			if (thisGilesBaseType == GilesBaseItemType.WeaponRange || thisGilesBaseType == GilesBaseItemType.WeaponOneHand || thisGilesBaseType == GilesBaseItemType.WeaponTwoHand)
			{
				if (iHighestScoringPrimary > 0 && (iHighestScoringPrimary >= iThisItemsMaxPoints[iWhichPrimaryIsHighest] * 0.9))
				{
					iTotalPoints *= 1.15;
				}
				// And an extra 15% for a very high vitality
				if (iHadStat[VITALITY] > 0 && (iHadStat[VITALITY] >= iThisItemsMaxPoints[VITALITY] * 0.9))
				{
					iTotalPoints *= 1.15;
				}
				// And an extra 15% for a very high life-on-hit
				if (iHadStat[LIFEONHIT] > 0 && (iHadStat[LIFEONHIT] >= iThisItemsMaxPoints[LIFEONHIT] * 0.9))
				{
					iTotalPoints *= 1.15;
				}
			}

			// Shields 
			if (thisGilesItemType == GilesItemType.Shield)
			{
				// Strength/Dex based shield calculations
				if (iWhichPrimaryIsHighest == STRENGTH || iWhichPrimaryIsHighest == DEXTERITY)
				{
					if (iHadStat[BLOCKCHANCE] < 20)
					{
						iTotalPoints *= 0.7;
					}
					else if (iHadStat[BLOCKCHANCE] < 25)
					{
						iTotalPoints *= 0.9;
					}
				}
				// Intelligence/no primary based shields
				else
				{
					if (iHadStat[BLOCKCHANCE] < 28)
						iTotalPoints -= iHadPoints[BLOCKCHANCE];
				}
			}

			// Quivers
			if (thisGilesItemType == GilesItemType.Quiver)
			{
				iTotalRequirements = 0;
				if (iHadStat[DEXTERITY] >= 100)
					iTotalRequirements++;
				else
					iTotalRequirements -= 3;
				if (iHadStat[DEXTERITY] >= 160)
					iTotalRequirements++;
				if (iHadStat[DEXTERITY] >= 250)
					iTotalRequirements++;
				if (iHadStat[ATTACKSPEED] < 14)
					iTotalRequirements -= 2;
				if (iHadStat[VITALITY] >= 70 || iSafeLifePercentage >= 0.85)
					iTotalRequirements++;
				else
					iTotalRequirements--;
				if (iHadStat[VITALITY] >= 260)
					iTotalRequirements++;
				if (iHadStat[MAXDISCIPLINE] >= 8)
					iTotalRequirements++;
				if (iHadStat[MAXDISCIPLINE] >= 10)
					iTotalRequirements++;
				if (iHadStat[SOCKETS] >= 1)
					iTotalRequirements++;
				if (iHadStat[CRITCHANCE] >= 6)
					iTotalRequirements++;
				if (iHadStat[CRITCHANCE] >= 8)
					iTotalRequirements++;
				if (iHadStat[LIFEPERCENT] >= 8)
					iTotalRequirements++;
				if (iHadStat[MAGICFIND] >= 18)
					iTotalRequirements++;
				if (iTotalRequirements < 4)
					iTotalPoints *= 0.4;
				else if (iTotalRequirements < 5)
					iTotalPoints *= 0.5;
				if (iTotalRequirements >= 7)
					iTotalPoints *= 1.2;
			}
			// Mojos and Sources
			if (thisGilesItemType == GilesItemType.Source || thisGilesItemType == GilesItemType.Mojo)
			{
				iTotalRequirements = 0;
				if (iHadStat[INTELLIGENCE] >= 100)
					iTotalRequirements++;
				else if (iHadStat[INTELLIGENCE] < 80)
					iTotalRequirements -= 3;
				else if (iHadStat[INTELLIGENCE] < 100)
					iTotalRequirements -= 1;
				if (iHadStat[INTELLIGENCE] >= 160)
					iTotalRequirements++;
				if (iHadStat[MAXDAMAGE] >= 250)
					iTotalRequirements++;
				else
					iTotalRequirements -= 2;
				if (iHadStat[MAXDAMAGE] >= 340)
					iTotalRequirements++;
				if (iHadStat[MINDAMAGE] >= 50)
					iTotalRequirements++;
				else
					iTotalRequirements--;
				if (iHadStat[MINDAMAGE] >= 85)
					iTotalRequirements++;
				if (iHadStat[VITALITY] >= 70)
					iTotalRequirements++;
				if (iHadStat[SOCKETS] >= 1)
					iTotalRequirements++;
				if (iHadStat[CRITCHANCE] >= 6)
					iTotalRequirements++;
				if (iHadStat[CRITCHANCE] >= 8)
					iTotalRequirements++;
				if (iHadStat[LIFEPERCENT] >= 8)
					iTotalRequirements++;
				if (iHadStat[MAGICFIND] >= 15)
					iTotalRequirements++;
				if (iHadStat[MAXMANA] >= 60)
					iTotalRequirements++;
				if (iHadStat[ARCANECRIT] >= 8)
					iTotalRequirements++;
				if (iHadStat[ARCANECRIT] >= 10)
					iTotalRequirements++;
				if (iTotalRequirements < 4)
					iTotalPoints *= 0.4;
				else if (iTotalRequirements < 5)
					iTotalPoints *= 0.5;
				if (iTotalRequirements >= 8)
					iTotalPoints *= 1.2;
			}

			// Chests/cloaks/pants without a socket lose 17% of total score
			if ((thisGilesItemType == GilesItemType.Chest || thisGilesItemType == GilesItemType.Cloak || thisGilesItemType == GilesItemType.Pants) && iHadStat[SOCKETS] == 0)
				iTotalPoints *= 0.83;

			// Boots with no movement speed get reduced score
			if ((thisGilesItemType == GilesItemType.Boots) && iHadStat[MOVEMENTSPEED] <= 6)
				iTotalPoints *= 0.75;

			// Helmets
			if (thisGilesItemType == GilesItemType.Helm || thisGilesItemType == GilesItemType.WizardHat || thisGilesItemType == GilesItemType.VoodooMask || thisGilesItemType == GilesItemType.SpiritStone)
			{
				// Helmets without a socket lose 20% of total score, and most of any MF/GF bonus
				if (iHadStat[SOCKETS] == 0)
				{
					iTotalPoints *= 0.8;
					if (iHadStat[MAGICFIND] > 0 || iHadStat[GOLDFIND] > 0)
					{
						if (iHadStat[MAGICFIND] > 0 && iHadStat[GOLDFIND] > 0)
							iTotalPoints -= ((iHadPoints[MAGICFIND] * 0.25) + (iHadPoints[GOLDFIND] * 0.25));
						else
							iTotalPoints -= ((iHadPoints[MAGICFIND] * 0.65) + (iHadPoints[GOLDFIND] * 0.65));
					}
				}
			}

			// Gold-find and pickup radius combined
			if ((iHadStat[GOLDFIND] / iThisItemsMaxStats[GOLDFIND] > 0.55) && (iHadStat[PICKUPRADIUS] / iThisItemsMaxStats[PICKUPRADIUS] > 0.5))
				iTotalPoints += (((iThisItemsMaxPoints[PICKUPRADIUS] + iThisItemsMaxPoints[GOLDFIND]) / 2) * 0.25);

			// All-resist and pickup radius combined
			if ((iHadStat[ALLRESIST] / iThisItemsMaxStats[ALLRESIST] > 0.55) && (iHadStat[PICKUPRADIUS] > 0))
				iTotalPoints += (((iThisItemsMaxPoints[PICKUPRADIUS] + iThisItemsMaxPoints[ALLRESIST]) / 2) * 0.65);

			// Special crit hit/crit chance/attack speed combos
			double dBestFinalBonus = 1d;
			if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.8)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.8)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.8)))
			{
				if (dBestFinalBonus < 3.2 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 3.2;
			}
			if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.8)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.8)))
			{
				if (dBestFinalBonus < 2.3) dBestFinalBonus = 2.3;
			}
			if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.8)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.8)))
			{
				if (dBestFinalBonus < 2.1 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 2.1;
			}
			if ((iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.8)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.8)))
			{
				if (dBestFinalBonus < 1.8 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.8;
			}
			if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.65)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.65)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.65)))
			{
				if (dBestFinalBonus < 2.1 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 2.1;
			}
			if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.65)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.65)))
			{
				if (dBestFinalBonus < 1.9) dBestFinalBonus = 1.9;
			}
			if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.65)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.65)))
			{
				if (dBestFinalBonus < 1.7 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.7;
			}
			if ((iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.65)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.65)))
			{
				if (dBestFinalBonus < 1.5 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.5;
			}
			if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.45)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.45)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.45)))
			{
				if (dBestFinalBonus < 1.7 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.7;
			}
			if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.45)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.45)))
			{
				if (dBestFinalBonus < 1.4) dBestFinalBonus = 1.4;
			}
			if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.45)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.45)))
			{
				if (dBestFinalBonus < 1.3 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.3;
			}
			if ((iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.45)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.45)))
			{
				if (dBestFinalBonus < 1.1 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.1;
			}


			iTotalPoints *= dBestFinalBonus;

			//1h Weapons that fall below the 60% range get docked!




			return Math.Round(iTotalPoints);
		}
		// Readable names of the above stats that get output into the trash/stash log files
		private static readonly string[] StatNames =
		{ 
			"Dexterity", "Intelligence", "Strength", "Vitality", 
			"Life %", "Life On Hit", "Life Steal %", "Life Regen", 
			"Magic Find %", "Gold Find   %", "Movement Speed %", "Pickup Radius", "Sockets", 
			"Crit Chance %", "Crit Damage %", "Attack Speed %", "+Min Damage", "+Max Damage",
			"Total Block %", "Thorns", "+All Resist", "+Highest Single Resist", "DPS", "Armor", "Max Disc.", "Max Mana", "Arcane-On-Crit", "Mana Regen", "Globe Bonus"};





		// Stores the apparent maximums of each stat for each item slot
		// Note that while these SHOULD be *actual* maximums for most stats - for things like DPS, these can just be more sort of "what a best-in-slot DPS would be"
		//												             Dex  Int  Str  Vit  Life%     LOH Steal%  LPS Magic% Gold% MSPD Rad. Sox Crit% CDam% ASPD Min+ Max+ Block% Thorn Allres Res   DPS ARMOR Disc.Mana Arc. Regen  Globes
		#region ItemAttributeWeights
		//Weapons/Offhand
		private static readonly double[] iMaxWeaponOneHand =
		{ 
			320, 320, 320, 320, 
			0, 850, 3, 0,
			0, 0, 0, 0, 1,
			0, 100, 0, 0, 0,
			0, 0, 0, 0, 1429, 0, 10, 150, 10, 14, 0 };
		private static readonly double[] iMaxWeaponTwoHand =
		{ 
			530, 530, 530, 530,
			0, 1800, 6, 0,
			0, 0, 0, 0, 1,
			0, 200, 0, 0, 0,
			0, 0, 0, 0, 1680, 0, 10, 119, 10, 14, 0 };
		private static readonly double[] iMaxWeaponRanged =
		{  
			320, 320, 320, 320,
			0, 850, 3, 0,
			0, 0, 0, 0, 1,
			0, 100, 0, 0, 0,
			0, 0, 0, 0, 1618, 0, 0, 0, 0, 14, 0 };
		private static readonly double[] iMaxOffHand =
		{       
			300, 300, 300, 300,
			9, 0, 0, 234,
			18, 20, 0, 0, 1,
			8.5, 0, 15, 110, 402,
			0, 979, 0, 0, 0, 0, 10, 119, 10, 11, 12794 };
		private static readonly double[] iMaxShield =
		{        
			330, 330, 330, 330,
			16, 0, 0, 342,
			20, 25, 0, 0, 1,
			10, 0, 0, 0, 0,
			30, 2544, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };

		//Ring/Ammy
		private static readonly double[] iMaxRing =
		{          
			200, 200, 200, 200,
			12, 479, 0, 340,
			20, 25, 0, 0, 1,
			6, 50, 9, 36, 100,
			0, 979, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
		private static readonly double[] iMaxAmulet =
		{        
			350, 350, 350, 350,
			16, 959, 0, 599,
			45, 50, 0, 0, 1,
			10, 100, 9, 36, 100,
			0, 1712, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };

		//Armor
		private static readonly double[] iMaxShoulders =
		{     
			200, 200, 300, 200,
			12, 0, 0, 342,
			20, 25, 0, 7, 0,
			0, 0, 0, 0, 0,
			0, 2544, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
		private static readonly double[] iMaxHelm =
		{          
			200, 300, 200, 200,
			12, 0, 0, 342,
			20, 25, 0, 7, 1,
			6, 0, 0, 0, 0,
			0, 1454, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };
		private static readonly double[] iMaxPants =
		{         
			200, 200, 200, 300,
			0, 0, 0, 342,
			20, 25, 0, 7, 2,
			0, 0, 0, 0, 0,
			0, 1454, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };
		private static readonly double[] iMaxGloves =
		{        
			300, 300, 200, 200,
			0, 0, 0, 342,
			20, 25, 0, 7, 0,
			10, 50, 9, 0, 0,
			0, 1454, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
		private static readonly double[] iMaxChest =
		{        
			200, 200, 200, 300,
			12, 0, 0, 599,
			20, 25, 0, 7, 3,
			0, 0, 0, 0, 0,
			0, 2544, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };
		private static readonly double[] iMaxBracer =
		{        
			200, 200, 200, 200,
			0, 0, 0, 342,
			20, 25, 0, 7, 0,
			6, 0, 0, 0, 0,
			0, 1454, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
		private static readonly double[] iMaxBoots =
		{         
			300, 200, 200, 200,
			0, 0, 0, 342,
			20, 25, 12, 7, 0,
			0, 0, 0, 0, 0,
			0, 1454, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
		private static readonly double[] iMaxBelt =
		{          
			200, 200, 300, 200,
			12, 0, 0, 342,
			20, 25, 0, 7, 0,
			0, 0, 0, 0, 0,
			0, 2544, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };

		private static readonly double[] iMaxCloak = { 200, 200, 200, 300, 12, 0, 0, 410, 20, 25, 0, 7, 3, 0, 0, 0, 0, 0, 0, 2544, 70, 50, 0, 397, 10, 0, 0, 0, 12794 };
		private static readonly double[] iMaxMightyBelt = { 200, 200, 300, 200, 12, 0, 3, 342, 20, 25, 0, 7, 0, 0, 0, 0, 0, 0, 0, 2544, 70, 50, 0, 265, 0, 0, 0, 0, 12794 };
		private static readonly double[] iMaxSpiritStone = { 200, 300, 200, 200, 12, 0, 0, 342, 20, 25, 0, 7, 1, 6, 0, 0, 0, 0, 0, 1454, 70, 50, 0, 397, 0, 0, 0, 0, 12794 };
		private static readonly double[] iMaxVoodooMask = { 200, 300, 200, 200, 12, 0, 0, 342, 20, 25, 0, 7, 1, 6, 0, 0, 0, 0, 0, 1454, 70, 50, 0, 397, 0, 119, 0, 11, 12794 };
		private static readonly double[] iMaxWizardHat = { 200, 300, 200, 200, 12, 0, 0, 342, 20, 25, 0, 7, 1, 6, 0, 0, 0, 0, 0, 1454, 70, 50, 0, 397, 0, 0, 10, 0, 12794 };

		private static readonly double[] iMaxFollower = { 200, 200, 200, 200, 0, 300, 0, 234, 0, 0, 0, 0, 0, 0, 55, 0, 0, 0, 0, 0, 50, 40, 0, 0, 0, 0, 0, 0, 0 };

		// Stores the total points this stat is worth at the above % point of maximum
		// Note that these values get all sorts of bonuses, multipliers, and extra things applied in the actual scoring routine. These values are more of a "base" value.
		//                                                              Dex    Int    Str    Vit    Life%  LOH    Steal% LPS   Magic%  Gold%  MSPD   Rad  Sox    Crit%  CDam%  ASPD   Min+  Max+ Block% Thorn Allres Res   DPS    ARMOR  Disc.  Mana  Arc.  Regen  Globes
		private static readonly double[] iWeaponPointsAtMax = { 14000, 14000, 14000, 14000, 13000, 20000, 7000, 1000, 6000, 6000, 6000, 500, 16000, 15000, 15000, 0, 0, 0, 0, 1000, 11000, 0, 64000, 0, 10000, 8500, 8500, 10000, 8000 };
		//                                                              Dex    Int    Str    Vit    Life%  LOH    Steal% LPS   Magic%  Gold%  MSPD   Rad. Sox    Crit%  CDam%  ASPD   Min+  Max+ Block% Thorn Allres Res   DPS    ARMOR  Disc.  Mana  Arc.  Regen  Globes
		private static readonly double[] iArmorPointsAtMax = { 11000, 11000, 11000, 9500, 9000, 10000, 4000, 1200, 3000, 3000, 3500, 1000, 4300, 9000, 6100, 7000, 3000, 3000, 5000, 1200, 7500, 1500, 0, 5000, 4000, 3000, 3000, 6000, 5000 };
		private static readonly double[] iJewelryPointsAtMax = { 11500, 11000, 11000, 10000, 8000, 11000, 4000, 1200, 4500, 4500, 3500, 1000, 3500, 7500, 6300, 6800, 800, 800, 5000, 1200, 7500, 1500, 0, 4500, 4000, 3000, 3000, 6000, 5000 };

		// Some special values for score calculations
		// BonusThreshold is a percentage of the "max-stat possible", that the stat starts to get a multiplier on it's score. 1 means it has to be above 100% of the "max-stat" to get a multiplier (so only possible if the max-stat isn't ACTUALLY the max possible)
		// MinimumThreshold is a percentage of the "max-stat possible", that the stat will simply be ignored for being too low. eg if set to .5 - then anything less than 50% of the max-stat will be ignored.
		// MinimumPrimary is used for some stats only - and means that at least ONE primary stat has to be above that level, to get score. Eg magic-find has .5 - meaning any item without at least 50% of a max-stat primary, will ignore magic-find scoring.
		//                                                             Dex  Int  Str  Vit  Life%  LOH  Steal%   LPS Magic% Gold% MSPD Radi  Sox  Crit% CDam% ASPD  Min+  Max+  Block%  Thorn  Allres  Res   DPS  ARMOR   Disc. Mana  Arc. Regen  Globes
		private static readonly double[] iBonusThreshold = { .75, .75, .75, .75, .80, .80, .9, 1, 1, 1, .95, 1, 1, .70, .90, 1, .9, .9, .83, 1, .85, .95, .90, .90, 1, 1, 1, .9, 1 };
		private static readonly double[] iMinimumThreshold = { .40, .40, .40, .30, .60, .45, .7, .7, .64, .64, .75, .8, .4, .40, .60, .40, .2, .2, .65, .6, .40, .55, .40, .80, .7, .7, .7, .7, .8 };
		private static readonly double[] iStatMinimumPrimary = { 0, 0, 0, 0, 0, 0, 0, .2, .50, .50, .30, 0, 0, 0, 0, 0, .40, .40, .40, .40, .40, .40, 0, .40, .40, .40, .40, .4, .4 };

		#endregion

		#endregion


		//Cache Backpack Item
		//Used in caching item ACDGUID and ACDITEM as reference for Item Loot Confirmation.
		internal class BackpackItem
		{
			public int ACDGUID { get; set; }
			public CacheACDItem Item { get; set; }
			public BackpackItem(CacheACDItem item)
			{
				ACDGUID = item.ACDGUID;
				Item = item;
			}
		}


		public Backpack()
		{
			townRunCache = new TownRunCache();
			BPItems = new List<BackpackItem>();
			CacheItemList = new Dictionary<int, CacheACDItem>();
		}
		public List<BackpackItem> BPItems { get; set; }
		public Dictionary<int, CacheACDItem> CacheItemList { get; set; }

		public ACDItem BestPotionToUse { get; set; }

		public int CurrentPotionACDGUID = -1;

		public TownRunCache townRunCache { get; set; }


		//Sets List to current backpack contents
		public void Update()
		{
			List<int> SeenACDGUIDs = new List<int>();
			using (ZetaDia.Memory.AcquireFrame(true))
			{
				foreach (var thisitem in ZetaDia.Me.Inventory.Backpack)
				{
					int ACDGUID = thisitem.ACDGuid;
					SeenACDGUIDs.Add(ACDGUID);
					if (CacheItemList.ContainsKey(ACDGUID))
					{
						if (CacheItemList[ACDGUID].IsStackableItem&&CacheItemList[ACDGUID].ThisItemStackQuantity!=thisitem.ItemStackQuantity)
							CacheItemList[ACDGUID]=new CacheACDItem(thisitem);
						continue;
					}

					CacheACDItem thiscacheditem = new CacheACDItem(thisitem);
					CacheItemList.Add(thiscacheditem.ACDGUID, thiscacheditem);
				}
			}

			List<int> UnseenACDGUIDs = CacheItemList.Keys.Where(k => !SeenACDGUIDs.Contains(k)).ToList();
			foreach (var unseenAcdguiD in UnseenACDGUIDs)
			{
				CacheItemList.Remove(unseenAcdguiD);
			}


			//We refresh our BPItem Cache whenever we are checking for looted items!
			if (Bot.Targeting.ShouldCheckItemLooted)
			{
				//Get a list of current BP Cached ACDItems
				List<int> BPItemsACDItemList = (from backpackItems in BPItems
												select backpackItems.ACDGUID).ToList();

				//Now get items that are not currently in the BPItems List.
				foreach (var item in CacheItemList.Values.Where(I => !BPItemsACDItemList.Contains(I.ACDGUID)))
				{
					BPItems.Add(new BackpackItem(item));
				}
			}
		}

		//Used to check if backpack is visible
		public bool InventoryBackpackVisible()
		{
			bool InvVisible = false;
			try
			{
				InvVisible = UIElements.InventoryWindow.IsVisible;
			}
			catch
			{
			}

			return InvVisible;
		}

		//Used to toggle current backpack
		public void InventoryBackPackToggle(bool show)
		{
			bool InvVisible = InventoryBackpackVisible();

			if (InvVisible && !show)
				UIElements.BackgroundScreenPCButtonInventory.Click();
			else if (!InvVisible && show)
				UIElements.BackgroundScreenPCButtonInventory.Click();
		}

		public List<CacheACDItem> ReturnCurrentPotions()
		{
			//Always update!
			Update();
			BestPotionToUse = null;


			var Potions = CacheItemList.Values.Where(i => i.IsPotion);
			if (!Potions.Any()) return null;
			Potions = Potions.OrderByDescending(i => i.ThisLevel).ThenByDescending(i => i.ThisItemStackQuantity);
			//Set Best Potion to use..
			CurrentPotionACDGUID = Potions.FirstOrDefault().ACDGUID;
			int balanceID = Potions.FirstOrDefault().ThisBalanceID;
			//Find best potion to use based upon stack
			BestPotionToUse = Potions.Where(i => i.ThisBalanceID == balanceID).OrderBy(i => i.ThisItemStackQuantity).FirstOrDefault().ACDItem;
			return Potions.ToList();

				//var Potions = ZetaDia.Me.Inventory.Backpack.Where(i => i.IsPotion);
				//if (!Potions.Any()) return null;
				//Potions = Potions.OrderByDescending(i => i.HitpointsGranted).ThenByDescending(i => i.ItemStackQuantity);
				////Set Best Potion to use..
				//CurrentPotionACDGUID = Potions.FirstOrDefault().ACDGuid;
				//int balanceID = Potions.FirstOrDefault().GameBalanceId;
				////Find best potion to use based upon stack
				//BestPotionToUse = Potions.Where(i => i.GameBalanceId == balanceID).OrderBy(i => i.ItemStackQuantity).FirstOrDefault();
				//return Potions.ToList();
			
		}

		public Queue<ACDItem> ReturnUnidenifiedItems()
		{
			Queue<ACDItem> returnQueue = new Queue<ACDItem>();

			Update();


			var filteredItems = ZetaDia.Me.Inventory.Backpack.Where(i =>
				i.IsValid && !i.IsMiscItem);

			if (filteredItems.Any())
			{
				foreach (ACDItem item in filteredItems)
				{
					try
					{
						if (item.IsUnidentified)
							returnQueue.Enqueue(item);
					}
					catch
					{
						Logging.WriteDiagnostic("[Funky] Safetly Handled Exception: occured checking of item unidentified flag");
					}
				}
			}


			return returnQueue;
		}

		private Queue<ACDItem> ReturnUnidenifiedItemsSorted(bool backwards = false)
		{
			Queue<ACDItem> returnQueue = new Queue<ACDItem>();
			foreach (var item in GetUnidenifiedItemsSorted(backwards))
			{
				returnQueue.Enqueue(item);
			}
			return returnQueue;
		}

		//Combines inventory rows into 3 groupings
		private int InventoryRowCombine(int i)
		{
			if ((i & 1) == 0)
				return i;
			return i - 1;
		}

		private List<ACDItem> GetUnidenifiedItemsSorted(bool Backwards = false)
		{
			Update();

			var filteredItems = ZetaDia.Me.Inventory.Backpack.Where(i =>
				i.IsValid && !i.IsMiscItem && i.IsUnidentified);
			if (Backwards)
				return filteredItems.OrderByDescending(o => InventoryRowCombine(o.InventoryRow)).ThenByDescending(o => o.InventoryColumn).ToList();
			return filteredItems.OrderBy(o => InventoryRowCombine(o.InventoryRow)).ThenBy(o => o.InventoryColumn).ToList();
		}
		private int ItemBaseTypePriorty(ItemBaseType type)
		{
			switch (type)
			{
				case ItemBaseType.Jewelry:
					return 0;
				case ItemBaseType.Weapon:
					return 1;
				case ItemBaseType.Armor:
					return 2;
			}
			return 3;
		}
		private Queue<ACDItem> ReturnUnidenifiedItemsSortedByType()
		{
			//Get sorted items, iterate and add into seperate collections, combine according to importance.
			bool backwards = ((int)MathEx.Random(0, 1) == 0);
			Queue<ACDItem> returnQueue = new Queue<ACDItem>();
			List<ACDItem> SortedItems = GetUnidenifiedItemsSorted(backwards);

			//Jewelery, Weapons, Armor
			foreach (var item in SortedItems.OrderBy(I => ItemBaseTypePriorty(I.ItemBaseType)))
			{
				returnQueue.Enqueue(item);
			}

			return returnQueue;
		}


		public Queue<ACDItem> ReturnUnidifiedItemsRandomizedSorted()
		{
			switch ((int)MathEx.Random(0, 1))
			{
				case 0:
					return ReturnUnidenifiedItemsSorted();
				case 1:
					return ReturnUnidenifiedItemsSorted(true);
				case 2:
					return ReturnUnidenifiedItemsSortedByType();
			}
			return ReturnUnidenifiedItems();
		}

		public bool ShouldRepairItems()
		{
			try
			{
				float repairVar = CharacterSettings.Instance.RepairWhenDurabilityBelow;
				bool ShouldRepair = false;
				using (ZetaDia.Memory.AcquireFrame())
				{
					bool intown = ZetaDia.Me.IsInTown;
					List<float> repairPct = ZetaDia.Me.Inventory.Equipped.Select(o => o.DurabilityPercent).ToList();

					//Already in town? Have gear with 50% or less durability?
					ShouldRepair = (repairPct.Any(o => o <= repairVar) || intown && repairPct.Any(o => o <= 50));
				}

				return ShouldRepair;
			}
			catch
			{
				return false;
			}

		}

		public bool ContainsItem(int ACDGUID, int prevstackcount=0)
		{
			//Update Item List
			Update();
			bool found = CacheItemList.ContainsKey(ACDGUID) && (prevstackcount==0||CacheItemList[ACDGUID].ThisItemStackQuantity>prevstackcount);
			return found;
		}

		public List<CacheACDItem> ReturnCurrentEquippedItems()
		{
			List<CacheACDItem> returnItems = new List<CacheACDItem>();
			try
			{
				using (ZetaDia.Memory.AcquireFrame())
				{
					foreach (ACDItem item in ZetaDia.Me.Inventory.Equipped)
					{
						CacheACDItem thiscacheditem = new CacheACDItem(item);
						returnItems.Add(thiscacheditem);
					}
				}

			}
			catch (Exception)
			{

			}
			return returnItems;
		}


		//Used to hold Town Run Data
		public class TownRunCache
		{
			private int InventoryRowCombine(int i)
			{
				if ((i & 1) == 0)
					return i;
				else
					return i - 1;
			}

			// These three lists are used to cache item data from the backpack when handling sales, salvaging and stashing
			// It completely minimized D3 <-> DB memory access, to reduce any random bugs/crashes etc.
			public HashSet<CacheACDItem> hashGilesCachedKeepItems = new HashSet<CacheACDItem>();
			public HashSet<CacheACDItem> hashGilesCachedSalvageItems = new HashSet<CacheACDItem>();
			public HashSet<CacheACDItem> hashGilesCachedSellItems = new HashSet<CacheACDItem>();
			public HashSet<CacheACDItem> hashGilesCachedUnidStashItems = new HashSet<CacheACDItem>();

			public void sortSellList()
			{
				List<CacheACDItem> sortedList = hashGilesCachedSellItems.OrderBy(o => InventoryRowCombine(o.invRow)).ThenBy(o => o.invCol).ToList();
				var newSortedHashSet = new HashSet<CacheACDItem>();
				foreach (var item in sortedList)
				{
					newSortedHashSet.Add(item);
				}

				hashGilesCachedSellItems = newSortedHashSet;

			}

			public void sortSalvagelist()
			{
				List<CacheACDItem> sortedList = hashGilesCachedSalvageItems.OrderBy(o => InventoryRowCombine(o.invRow)).ThenBy(o => o.invCol).ToList();
				var newSortedHashSet = new HashSet<CacheACDItem>();
				foreach (var item in sortedList)
				{
					newSortedHashSet.Add(item);
				}

				hashGilesCachedSalvageItems = newSortedHashSet;

			}
		}

	}
}