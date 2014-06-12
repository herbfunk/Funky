using System.Linq;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Misc;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player
{
	internal static class ItemFunc
	{
		#region Static Item Methods

		// **********************************************************************************************
		// *****       Pickup Validation - Determines what should or should not be picked up        *****
		// **********************************************************************************************
		internal static bool PickupItemValidation(CacheItem item)
		{
			// Calculate giles item types and base types etc.
			GilesItemType thisGilesItemType = DetermineItemType(item.InternalName, item.BalanceData.thisItemType, item.BalanceData.thisFollowerType);
			GilesBaseItemType thisGilesBaseType = DetermineBaseType(thisGilesItemType);

			if (thisGilesItemType == GilesItemType.MiscBook)
				return Bot.Settings.Loot.ExpBooks;


			// Error logging for DemonBuddy item mis-reading
			ItemType gilesDBItemType = GilesToDBItemType(thisGilesItemType);
			if (gilesDBItemType != item.BalanceData.thisItemType)
			{
				Logger.DBLog.InfoFormat("GSError: Item type mis-match detected: Item Internal=" + item.InternalName + ". DemonBuddy ItemType thinks item type is=" + item.BalanceData.thisItemType + ". Giles thinks item type is=" +
					 gilesDBItemType + ". [pickup]", true);
			}

			switch (thisGilesBaseType)
			{
				case GilesBaseItemType.WeaponTwoHand:
				case GilesBaseItemType.WeaponOneHand:
				case GilesBaseItemType.WeaponRange:
				case GilesBaseItemType.Armor:
				case GilesBaseItemType.Offhand:
				case GilesBaseItemType.Jewelry:
				case GilesBaseItemType.FollowerItem:
					if (item.Itemquality.HasValue)
					{
						switch (item.Itemquality.Value)
						{
							case ItemQuality.Inferior:
							case ItemQuality.Normal:
							case ItemQuality.Superior:
								return Bot.Settings.Loot.PickupWhiteItems>0 && (item.BalanceData.iThisItemLevel >= Bot.Settings.Loot.PickupWhiteItems);
							case ItemQuality.Magic1:
							case ItemQuality.Magic2:
							case ItemQuality.Magic3:
								return Bot.Settings.Loot.PickupMagicItems>0&&(item.BalanceData.iThisItemLevel >= Bot.Settings.Loot.PickupMagicItems);
							case ItemQuality.Rare4:
							case ItemQuality.Rare5:
							case ItemQuality.Rare6:
								return Bot.Settings.Loot.PickupRareItems>0&&(item.BalanceData.iThisItemLevel >= Bot.Settings.Loot.PickupRareItems);
							case ItemQuality.Legendary:
								return Bot.Settings.Loot.PickupLegendaryItems>0&&(item.BalanceData.iThisItemLevel >= Bot.Settings.Loot.PickupLegendaryItems);
						}
					}

					return false;
				case GilesBaseItemType.Gem:
					if (item.BalanceData.iThisItemLevel < Bot.Settings.Loot.MinimumGemItemLevel ||
						(thisGilesItemType == GilesItemType.Ruby && !Bot.Settings.Loot.PickupGems[0]) ||
						(thisGilesItemType == GilesItemType.Emerald && !Bot.Settings.Loot.PickupGems[1]) ||
						(thisGilesItemType == GilesItemType.Amethyst && !Bot.Settings.Loot.PickupGems[2]) ||
						(thisGilesItemType == GilesItemType.Topaz && !Bot.Settings.Loot.PickupGems[3]) ||
						(thisGilesItemType == GilesItemType.Diamond && !Bot.Settings.Loot.PickupGemDiamond))
					{
						return false;
					}
					break;
				case GilesBaseItemType.Misc:
					// Note; Infernal keys are misc, so should be picked up here - we aren't filtering them out, so should default to true at the end of this function
					if (thisGilesItemType == GilesItemType.CraftingMaterial)
					{
						return Bot.Settings.Loot.PickupCraftMaterials;
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
						return Bot.Settings.Loot.PickupInfernalKeys;
					}

					if (thisGilesItemType == GilesItemType.KeyStone)
					{
						return Bot.Settings.Loot.PickupKeystoneFragments;
					}

					if (thisGilesItemType == GilesItemType.BloodShard)
					{
						return Bot.Character.Data.BackPack.GetBloodShardCount() < 500;
					}

					// Potion filtering
					if (thisGilesItemType == GilesItemType.HealthPotion)
					{
						if (item.BalanceData.IsRegularPotion)
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
					}


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
		// ***** DetermineItemType - Calculates what kind of item it is from D3 internalnames       *****
		// **********************************************************************************************
		internal static GilesItemType DetermineItemType(string sThisInternalName, ItemType DBItemType, FollowerType dbFollowerType = FollowerType.None)
		{
			sThisInternalName = sThisInternalName.ToLower();

			if (sThisInternalName.Contains("horadriccache")) return GilesItemType.HoradricCache;
			if (sThisInternalName.Contains("horadricrelic")) return GilesItemType.BloodShard;

			if (sThisInternalName.Contains("craftingreagent")) return GilesItemType.CraftingMaterial;
			if (sThisInternalName.Contains("lootrunkey")) return GilesItemType.KeyStone;

			// Fall back on some partial DB item type checking 
			if (sThisInternalName.Contains("crafting_") || sThisInternalName.Contains("craftingmaterials_"))
			{
				if (DBItemType == ItemType.CraftingPage) return GilesItemType.CraftTome;
				return GilesItemType.CraftingMaterial;
			}

			if (sThisInternalName.Contains("flail1h_")) return GilesItemType.Flail;
			//TODO:: Update With Proper Name!
			if (sThisInternalName.Contains("flail")) return GilesItemType.TwoHandFlail;

			if (sThisInternalName.Contains("twohandedaxe_")) return GilesItemType.TwoHandAxe;
			if (sThisInternalName.Contains("axe_")) return GilesItemType.Axe;

			if (sThisInternalName.Contains("ceremonialdagger_")) return GilesItemType.CeremonialKnife;
			if (sThisInternalName.Contains("handxbow_")) return GilesItemType.HandCrossbow;
			if (sThisInternalName.Contains("dagger_")) return GilesItemType.Dagger;
			if (sThisInternalName.Contains("fistweapon_")) return GilesItemType.FistWeapon;

			if (sThisInternalName.Contains("twohandedmace_")) return GilesItemType.TwoHandMace;
			if (sThisInternalName.Contains("mace_")) return GilesItemType.Mace;

			if (sThisInternalName.Contains("mightyweapon_1h_")) return GilesItemType.MightyWeapon;
			if (sThisInternalName.Contains("spear_")) return GilesItemType.Spear;

			if (sThisInternalName.Contains("twohandedsword_")) return GilesItemType.TwoHandSword;
			if (sThisInternalName.Contains("sword_")) return GilesItemType.Sword;

			if (sThisInternalName.Contains("wand_")) return GilesItemType.Wand;

			if (sThisInternalName.Contains("xbow_")) return GilesItemType.TwoHandCrossbow;
			if (sThisInternalName.Contains("bow_")) return GilesItemType.TwoHandBow;
			if (sThisInternalName.Contains("combatstaff_")) return GilesItemType.TwoHandDaibo;


			if (sThisInternalName.Contains("mightyweapon_2h_")) return GilesItemType.TwoHandMighty;
			if (sThisInternalName.Contains("polearm_")) return GilesItemType.TwoHandPolearm;
			if (sThisInternalName.Contains("staff_")) return GilesItemType.TwoHandStaff;

			if (sThisInternalName.Contains("staffofcow")) return GilesItemType.StaffOfHerding;
			if (sThisInternalName.Contains("mojo_")) return GilesItemType.Mojo;
			if (sThisInternalName.Contains("orb_")) return GilesItemType.Source;
			if (sThisInternalName.Contains("quiver_")) return GilesItemType.Quiver;

			if (sThisInternalName.Contains("crushield_")) return GilesItemType.CrusaderShield;
			if (sThisInternalName.Contains("shield_")) return GilesItemType.Shield;

			if (sThisInternalName.Contains("amulet_")) return GilesItemType.Amulet;
			if (sThisInternalName.Contains("ring_")) return GilesItemType.Ring;
			if (sThisInternalName.Contains("boots_")) return GilesItemType.Boots;
			if (sThisInternalName.Contains("bracers_")) return GilesItemType.Bracers;
			if (sThisInternalName.Contains("cloak_")) return GilesItemType.Cloak;
			if (sThisInternalName.Contains("gloves_")) return GilesItemType.Gloves;
			if (sThisInternalName.Contains("pants_")) return GilesItemType.Pants;
			if (sThisInternalName.Contains("barbbelt_")) return GilesItemType.MightyBelt;
			if (sThisInternalName.Contains("shoulderpads_")) return GilesItemType.Shoulders;
			if (sThisInternalName.Contains("spiritstone_")) return GilesItemType.SpiritStone;
			if (sThisInternalName.Contains("voodoomask_")) return GilesItemType.VoodooMask;
			if (sThisInternalName.Contains("wizardhat_")) return GilesItemType.WizardHat;
			if (sThisInternalName.Contains("lore_book_")) return GilesItemType.MiscBook;
			if (sThisInternalName.Contains("page_of_")) return GilesItemType.CraftTome;
			if (sThisInternalName.Contains("blacksmithstome")) return GilesItemType.CraftTome;
			if (sThisInternalName.Contains("ruby_")) return GilesItemType.Ruby;
			if (sThisInternalName.Contains("emerald_")) return GilesItemType.Emerald;
			if (sThisInternalName.Contains("topaz_")) return GilesItemType.Topaz;
			if (sThisInternalName.Contains("amethyst")) return GilesItemType.Amethyst;
			if (sThisInternalName.Contains("diamond_")) return GilesItemType.Diamond;
			if (sThisInternalName.Contains("healthpotion")) return GilesItemType.HealthPotion;
			if (sThisInternalName.Contains("followeritem_enchantress_")) return GilesItemType.FollowerEnchantress;
			if (sThisInternalName.Contains("followeritem_scoundrel_")) return GilesItemType.FollowerScoundrel;
			if (sThisInternalName.Contains("followeritem_templar_")) return GilesItemType.FollowerTemplar;
			if (sThisInternalName.Contains("craftingplan_")) return GilesItemType.CraftingPlan;
			if (sThisInternalName.Contains("dye_")) return GilesItemType.Dye;
			if (sThisInternalName.Contains("a1_")) return GilesItemType.SpecialItem;
			if (sThisInternalName.Contains("healthglobe")) return GilesItemType.HealthGlobe;


			// Follower item types
			if (sThisInternalName.Contains("jewelbox_") || DBItemType == ItemType.FollowerSpecial)
			{
				if (dbFollowerType == FollowerType.Scoundrel)
					return GilesItemType.FollowerScoundrel;
				if (dbFollowerType == FollowerType.Templar)
					return GilesItemType.FollowerTemplar;
				if (dbFollowerType == FollowerType.Enchantress)
					return GilesItemType.FollowerEnchantress;
			}

			if (sThisInternalName.Contains("chestarmor_"))
			{
				if (DBItemType == ItemType.Cloak) return GilesItemType.Cloak;
				return GilesItemType.Chest;
			}
			if (sThisInternalName.Contains("helm_"))
			{
				if (DBItemType == ItemType.SpiritStone) return GilesItemType.SpiritStone;
				if (DBItemType == ItemType.VoodooMask) return GilesItemType.VoodooMask;
				if (DBItemType == ItemType.WizardHat) return GilesItemType.WizardHat;
				return GilesItemType.Helm;
			}
			if (sThisInternalName.Contains("helmcloth_"))
			{
				if (DBItemType == ItemType.SpiritStone) return GilesItemType.SpiritStone;
				if (DBItemType == ItemType.VoodooMask) return GilesItemType.VoodooMask;
				if (DBItemType == ItemType.WizardHat) return GilesItemType.WizardHat;
				return GilesItemType.Helm;
			}
			if (sThisInternalName.Contains("belt_"))
			{
				if (DBItemType == ItemType.MightyBelt) return GilesItemType.MightyBelt;
				return GilesItemType.Belt;
			}
			if (sThisInternalName.Contains("demonkey_") || sThisInternalName.Contains("demontrebuchetkey"))
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
				 thisGilesItemType == GilesItemType.Spear || thisGilesItemType == GilesItemType.Sword || thisGilesItemType == GilesItemType.Wand || thisGilesItemType == GilesItemType.Flail)
			{
				thisGilesBaseType = GilesBaseItemType.WeaponOneHand;
			}
			else if (thisGilesItemType == GilesItemType.TwoHandDaibo || thisGilesItemType == GilesItemType.TwoHandMace ||
				 thisGilesItemType == GilesItemType.TwoHandMighty || thisGilesItemType == GilesItemType.TwoHandPolearm || thisGilesItemType == GilesItemType.TwoHandStaff ||
				 thisGilesItemType == GilesItemType.TwoHandSword || thisGilesItemType == GilesItemType.TwoHandAxe || thisGilesItemType == GilesItemType.TwoHandFlail)
			{
				thisGilesBaseType = GilesBaseItemType.WeaponTwoHand;
			}
			else if (thisGilesItemType == GilesItemType.TwoHandCrossbow || thisGilesItemType == GilesItemType.HandCrossbow || thisGilesItemType == GilesItemType.TwoHandBow)
			{
				thisGilesBaseType = GilesBaseItemType.WeaponRange;
			}
			else if (thisGilesItemType == GilesItemType.Mojo || thisGilesItemType == GilesItemType.Source ||
				 thisGilesItemType == GilesItemType.Quiver || thisGilesItemType == GilesItemType.Shield || thisGilesItemType == GilesItemType.CrusaderShield)
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
			else if (thisGilesItemType == GilesItemType.CraftingMaterial || thisGilesItemType == GilesItemType.CraftTome || thisGilesItemType == GilesItemType.MiscBook ||
				 thisGilesItemType == GilesItemType.SpecialItem || thisGilesItemType == GilesItemType.CraftingPlan || thisGilesItemType == GilesItemType.HealthPotion ||
				 thisGilesItemType == GilesItemType.Dye || thisGilesItemType == GilesItemType.StaffOfHerding || thisGilesItemType == GilesItemType.InfernalKey ||
				thisGilesItemType == GilesItemType.KeyStone || thisGilesItemType == GilesItemType.HoradricCache || thisGilesItemType == GilesItemType.BloodShard)
			{
				thisGilesBaseType = GilesBaseItemType.Misc;
			}
			else if (thisGilesItemType == GilesItemType.Ruby || thisGilesItemType == GilesItemType.Emerald || thisGilesItemType == GilesItemType.Topaz ||
				 thisGilesItemType == GilesItemType.Amethyst || thisGilesItemType == GilesItemType.Diamond)
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
									  thisGilesItemType == GilesItemType.Diamond || thisGilesItemType == GilesItemType.Emerald || thisGilesItemType == GilesItemType.Topaz || thisGilesItemType == GilesItemType.Amethyst ||
									  thisGilesItemType == GilesItemType.HealthPotion || thisGilesItemType == GilesItemType.CraftingPlan || thisGilesItemType == GilesItemType.Dye ||
									  thisGilesItemType == GilesItemType.InfernalKey || thisGilesItemType == GilesItemType.KeyStone;
			return bIsStackable;
		}
		internal static bool DetermineIsStackable(CacheACDItem item)
		{
			GilesItemType thisGilesItemType = DetermineItemType(item.ThisInternalName, item.ThisDBItemType, item.ThisFollowerType);
			bool bIsStackable = thisGilesItemType == GilesItemType.CraftingMaterial || thisGilesItemType == GilesItemType.CraftTome || thisGilesItemType == GilesItemType.Ruby ||
									  thisGilesItemType == GilesItemType.Diamond || thisGilesItemType == GilesItemType.Emerald || thisGilesItemType == GilesItemType.Topaz || thisGilesItemType == GilesItemType.Amethyst ||
									  thisGilesItemType == GilesItemType.HealthPotion || thisGilesItemType == GilesItemType.CraftingPlan || thisGilesItemType == GilesItemType.Dye ||
									  thisGilesItemType == GilesItemType.InfernalKey || thisGilesItemType == GilesItemType.KeyStone;
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
				 thisGilesItemType == GilesItemType.VoodooMask || thisGilesItemType == GilesItemType.WizardHat || thisGilesItemType == GilesItemType.StaffOfHerding ||
				 thisGilesItemType == GilesItemType.Flail || thisGilesItemType == GilesItemType.TwoHandFlail || thisGilesItemType == GilesItemType.CrusaderShield || thisGilesItemType == GilesItemType.HoradricCache)
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

				case GilesItemType.Flail: return ItemType.Flail;
				case GilesItemType.TwoHandFlail: return ItemType.Flail;
				case GilesItemType.CrusaderShield: return ItemType.CrusaderShield;

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
				case GilesItemType.Diamond: return ItemType.Gem;
				case GilesItemType.SpecialItem: return ItemType.Unknown;
				case GilesItemType.CraftingPlan: return ItemType.CraftingPlan;
				case GilesItemType.HealthPotion: return ItemType.Potion;
				case GilesItemType.Dye: return ItemType.Unknown;
				case GilesItemType.InfernalKey: return ItemType.Unknown;
				case GilesItemType.MiscBook: return ItemType.CraftingPage;
				case GilesItemType.KeyStone: return ItemType.KeystoneFragment;
				case GilesItemType.HoradricCache: return ItemType.HoradricCache;
			}
			return ItemType.Unknown;
		}





		#endregion
	}
}
