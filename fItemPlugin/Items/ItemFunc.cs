using Zeta.Game.Internals.Actors;

namespace fItemPlugin.Items
{
	public static class ItemFunc
	{
		public static PluginItemType DetermineItemType(CacheACDItem cacheItem)
		{
			return DetermineItemType(cacheItem.ThisInternalName, cacheItem.ThisDBItemType, cacheItem.ThisFollowerType, cacheItem.SNO);
		}
		public static PluginItemType DetermineItemType(string sThisInternalName, ItemType DBItemType, FollowerType dbFollowerType = FollowerType.None, int snoid = -1)
		{
			sThisInternalName = sThisInternalName.ToLower();

			if (snoid != -1)
			{
				PluginItemType type;
				if (DetermineItemTypeUsingSNO(snoid, out type))
				{
					return type;
				}
			}

			if (sThisInternalName.Contains("ruby_")) return PluginItemType.Ruby;
			if (sThisInternalName.Contains("emerald_")) return PluginItemType.Emerald;
			if (sThisInternalName.Contains("topaz_")) return PluginItemType.Topaz;
			if (sThisInternalName.Contains("amethyst")) return PluginItemType.Amethyst;
			if (sThisInternalName.Contains("diamond_")) return PluginItemType.Diamond;


			if (sThisInternalName.Contains("horadriccache")) return PluginItemType.HoradricCache;


			if (IsUberKey(snoid) || sThisInternalName.Contains("demonkey_") || sThisInternalName.Contains("demontrebuchetkey"))
				return PluginItemType.InfernalKey;


			if (sThisInternalName.Contains("craftingreagent_legendary")) return PluginItemType.LegendaryCraftingMaterial;
			if (sThisInternalName.Contains("craftingreagent")) return PluginItemType.CraftingMaterial;
			if (sThisInternalName.Contains("lootrunkey")) return PluginItemType.KeyStone;

			// Fall back on some partial DB item type checking 
			if (sThisInternalName.Contains("crafting_") || sThisInternalName.Contains("craftingmaterials_"))
			{
				if (ItemSnoCache.CraftingMaterialSNOIds.Contains(snoid)) return PluginItemType.CraftingMaterial;
				if (DBItemType == ItemType.CraftingPage) return PluginItemType.CraftTome;
				return PluginItemType.CraftingMaterial;
			}

			if (sThisInternalName.Contains("flail1h_")) return PluginItemType.Flail;
			if (sThisInternalName.Contains("flail")) return PluginItemType.TwoHandFlail;

			if (sThisInternalName.Contains("twohandedaxe_")) return PluginItemType.TwoHandAxe;
			if (sThisInternalName.Contains("axe_")) return PluginItemType.Axe;

			if (sThisInternalName.Contains("ceremonialdagger_")) return PluginItemType.CeremonialKnife;
			if (sThisInternalName.Contains("handxbow_")) return PluginItemType.HandCrossbow;
			if (sThisInternalName.Contains("dagger_")) return PluginItemType.Dagger;
			if (sThisInternalName.Contains("fistweapon_")) return PluginItemType.FistWeapon;

			if (sThisInternalName.Contains("twohandedmace_")) return PluginItemType.TwoHandMace;
			if (sThisInternalName.Contains("mace_")) return PluginItemType.Mace;

			if (sThisInternalName.Contains("mightyweapon_1h_")) return PluginItemType.MightyWeapon;
			if (sThisInternalName.Contains("spear_")) return PluginItemType.Spear;

			if (sThisInternalName.Contains("twohandedsword_")) return PluginItemType.TwoHandSword;
			if (sThisInternalName.Contains("sword_")) return PluginItemType.Sword;

			if (sThisInternalName.Contains("wand_")) return PluginItemType.Wand;

			if (sThisInternalName.Contains("xbow_")) return PluginItemType.TwoHandCrossbow;
			if (sThisInternalName.Contains("bow_")) return PluginItemType.TwoHandBow;
			if (sThisInternalName.Contains("combatstaff_")) return PluginItemType.TwoHandDaibo;


			if (sThisInternalName.Contains("mightyweapon_2h_")) return PluginItemType.TwoHandMighty;
			if (sThisInternalName.Contains("polearm_")) return PluginItemType.TwoHandPolearm;
			if (sThisInternalName.Contains("staff_")) return PluginItemType.TwoHandStaff;

			if (sThisInternalName.Contains("staffofcow")) return PluginItemType.StaffOfHerding;
			if (sThisInternalName.Contains("mojo_")) return PluginItemType.Mojo;
			if (sThisInternalName.Contains("orb_")) return PluginItemType.Source;
			if (sThisInternalName.Contains("quiver_")) return PluginItemType.Quiver;

			if (sThisInternalName.Contains("crushield_")) return PluginItemType.CrusaderShield;
			if (sThisInternalName.Contains("shield_")) return PluginItemType.Shield;

			if (sThisInternalName.Contains("amulet_")) return PluginItemType.Amulet;
			if (sThisInternalName.Contains("ring_")) return PluginItemType.Ring;
			if (sThisInternalName.Contains("boots_")) return PluginItemType.Boots;
			if (sThisInternalName.Contains("bracers_")) return PluginItemType.Bracers;
			if (sThisInternalName.Contains("cloak_")) return PluginItemType.Cloak;
			if (sThisInternalName.Contains("gloves_")) return PluginItemType.Gloves;
			if (sThisInternalName.Contains("pants_")) return PluginItemType.Pants;
			if (sThisInternalName.Contains("barbbelt_")) return PluginItemType.MightyBelt;
			if (sThisInternalName.Contains("shoulderpads_")) return PluginItemType.Shoulders;
			if (sThisInternalName.Contains("spiritstone_")) return PluginItemType.SpiritStone;
			if (sThisInternalName.Contains("voodoomask_")) return PluginItemType.VoodooMask;
			if (sThisInternalName.Contains("wizardhat_")) return PluginItemType.WizardHat;
			if (sThisInternalName.Contains("lore_book_")) return PluginItemType.MiscBook;
			if (sThisInternalName.Contains("page_of_")) return PluginItemType.CraftTome;
			if (sThisInternalName.Contains("blacksmithstome")) return PluginItemType.CraftTome;

			if (sThisInternalName.Contains("healthpotion")) return PluginItemType.HealthPotion;
			if (sThisInternalName.Contains("followeritem_enchantress_")) return PluginItemType.FollowerEnchantress;
			if (sThisInternalName.Contains("followeritem_scoundrel_")) return PluginItemType.FollowerScoundrel;
			if (sThisInternalName.Contains("followeritem_templar_")) return PluginItemType.FollowerTemplar;
			if (sThisInternalName.Contains("craftingplan_")) return PluginItemType.CraftingPlan;
			if (sThisInternalName.Contains("dye_")) return PluginItemType.Dye;
			if (sThisInternalName.Contains("a1_")) return PluginItemType.SpecialItem;
			if (sThisInternalName.Contains("healthglobe")) return PluginItemType.HealthGlobe;


			// Follower item types
			if (sThisInternalName.Contains("jewelbox_") || DBItemType == ItemType.FollowerSpecial)
			{
				if (dbFollowerType == FollowerType.Scoundrel)
					return PluginItemType.FollowerScoundrel;
				if (dbFollowerType == FollowerType.Templar)
					return PluginItemType.FollowerTemplar;
				if (dbFollowerType == FollowerType.Enchantress)
					return PluginItemType.FollowerEnchantress;
			}

			if (sThisInternalName.Contains("chestarmor_"))
			{
				if (DBItemType == ItemType.Cloak) return PluginItemType.Cloak;
				return PluginItemType.Chest;
			}
			if (sThisInternalName.Contains("helm_"))
			{
				if (DBItemType == ItemType.SpiritStone) return PluginItemType.SpiritStone;
				if (DBItemType == ItemType.VoodooMask) return PluginItemType.VoodooMask;
				if (DBItemType == ItemType.WizardHat) return PluginItemType.WizardHat;
				return PluginItemType.Helm;
			}
			if (sThisInternalName.Contains("helmcloth_"))
			{
				if (DBItemType == ItemType.SpiritStone) return PluginItemType.SpiritStone;
				if (DBItemType == ItemType.VoodooMask) return PluginItemType.VoodooMask;
				if (DBItemType == ItemType.WizardHat) return PluginItemType.WizardHat;
				return PluginItemType.Helm;
			}
			if (sThisInternalName.Contains("belt_"))
			{
				if (DBItemType == ItemType.MightyBelt) return PluginItemType.MightyBelt;
				return PluginItemType.Belt;
			}


			return PluginItemType.Unknown;
		}
		public static bool DetermineItemTypeUsingSNO(int snoid, out PluginItemType type)
		{
			type = PluginItemType.Unknown;

			if (ItemSnoCache.GEMS_AmethystSNOIds.Contains(snoid))
			{
				type = PluginItemType.Amethyst;
				return true;
			}
			if (ItemSnoCache.GEMS_RubySNOIds.Contains(snoid))
			{
				type = PluginItemType.Ruby;
				return true;
			}
			if (ItemSnoCache.GEMS_TopazSNOIds.Contains(snoid))
			{
				type = PluginItemType.Topaz;
				return true;
			}
			if (ItemSnoCache.GEMS_EmeraldSNOIds.Contains(snoid))
			{
				type = PluginItemType.Emerald;
				return true;
			}
			if (ItemSnoCache.GEMS_DiamondSNOIds.Contains(snoid))
			{
				type = PluginItemType.Diamond;
				return true;
			}
			if (ItemSnoCache.HealthPotionSNOIds.Contains(snoid))
			{
				type = PluginItemType.HealthPotion;
				return true;
			}
			if (ItemSnoCache.CraftingMaterialSNOIds.Contains(snoid))
			{
				type = PluginItemType.CraftingMaterial;
				return true;
			}
			if (ItemSnoCache.MiscItemSNOIds.Contains(snoid))
			{
				type = PluginItemType.CraftingMaterial;
				return true;
			}
			if (ItemSnoCache.DyesSNOIds.Contains(snoid))
			{
				type = PluginItemType.Dye;
				return true;
			}
			if (ItemSnoCache.HoradricCacheSNOIds.Contains(snoid))
			{
				type=PluginItemType.HoradricCache;
				return true;
			}

			return type != PluginItemType.Unknown;
		}
		public static PluginBaseItemType DetermineBaseType(PluginItemType thisPluginItemType)
		{
			PluginBaseItemType thisGilesBaseType = PluginBaseItemType.Unknown;
			if (thisPluginItemType == PluginItemType.Axe || thisPluginItemType == PluginItemType.CeremonialKnife || thisPluginItemType == PluginItemType.Dagger ||
				 thisPluginItemType == PluginItemType.FistWeapon || thisPluginItemType == PluginItemType.Mace || thisPluginItemType == PluginItemType.MightyWeapon ||
				 thisPluginItemType == PluginItemType.Spear || thisPluginItemType == PluginItemType.Sword || thisPluginItemType == PluginItemType.Wand || thisPluginItemType == PluginItemType.Flail)
			{
				thisGilesBaseType = PluginBaseItemType.WeaponOneHand;
			}
			else if (thisPluginItemType == PluginItemType.TwoHandDaibo || thisPluginItemType == PluginItemType.TwoHandMace ||
				 thisPluginItemType == PluginItemType.TwoHandMighty || thisPluginItemType == PluginItemType.TwoHandPolearm || thisPluginItemType == PluginItemType.TwoHandStaff ||
				 thisPluginItemType == PluginItemType.TwoHandSword || thisPluginItemType == PluginItemType.TwoHandAxe || thisPluginItemType == PluginItemType.TwoHandFlail)
			{
				thisGilesBaseType = PluginBaseItemType.WeaponTwoHand;
			}
			else if (thisPluginItemType == PluginItemType.TwoHandCrossbow || thisPluginItemType == PluginItemType.HandCrossbow || thisPluginItemType == PluginItemType.TwoHandBow)
			{
				thisGilesBaseType = PluginBaseItemType.WeaponRange;
			}
			else if (thisPluginItemType == PluginItemType.Mojo || thisPluginItemType == PluginItemType.Source ||
				 thisPluginItemType == PluginItemType.Quiver || thisPluginItemType == PluginItemType.Shield || thisPluginItemType == PluginItemType.CrusaderShield)
			{
				thisGilesBaseType = PluginBaseItemType.Offhand;
			}
			else if (thisPluginItemType == PluginItemType.Boots || thisPluginItemType == PluginItemType.Bracers || thisPluginItemType == PluginItemType.Chest ||
				 thisPluginItemType == PluginItemType.Cloak || thisPluginItemType == PluginItemType.Gloves || thisPluginItemType == PluginItemType.Helm ||
				 thisPluginItemType == PluginItemType.Pants || thisPluginItemType == PluginItemType.Shoulders || thisPluginItemType == PluginItemType.SpiritStone ||
				 thisPluginItemType == PluginItemType.VoodooMask || thisPluginItemType == PluginItemType.WizardHat || thisPluginItemType == PluginItemType.Belt ||
				 thisPluginItemType == PluginItemType.MightyBelt)
			{
				thisGilesBaseType = PluginBaseItemType.Armor;
			}
			else if (thisPluginItemType == PluginItemType.Amulet || thisPluginItemType == PluginItemType.Ring)
			{
				thisGilesBaseType = PluginBaseItemType.Jewelry;
			}
			else if (thisPluginItemType == PluginItemType.FollowerEnchantress || thisPluginItemType == PluginItemType.FollowerScoundrel ||
				 thisPluginItemType == PluginItemType.FollowerTemplar)
			{
				thisGilesBaseType = PluginBaseItemType.FollowerItem;
			}
			else if (thisPluginItemType == PluginItemType.CraftingMaterial || thisPluginItemType == PluginItemType.CraftTome || thisPluginItemType == PluginItemType.MiscBook ||
				 thisPluginItemType == PluginItemType.SpecialItem || thisPluginItemType == PluginItemType.CraftingPlan || thisPluginItemType == PluginItemType.HealthPotion ||
				 thisPluginItemType == PluginItemType.Dye || thisPluginItemType == PluginItemType.StaffOfHerding || thisPluginItemType == PluginItemType.InfernalKey ||
				thisPluginItemType == PluginItemType.KeyStone || thisPluginItemType == PluginItemType.HoradricCache || thisPluginItemType == PluginItemType.BloodShard)
			{
				thisGilesBaseType = PluginBaseItemType.Misc;
			}
			else if (thisPluginItemType == PluginItemType.Ruby || thisPluginItemType == PluginItemType.Emerald || thisPluginItemType == PluginItemType.Topaz ||
				 thisPluginItemType == PluginItemType.Amethyst || thisPluginItemType == PluginItemType.Diamond)
			{
				thisGilesBaseType = PluginBaseItemType.Gem;
			}
			else if (thisPluginItemType == PluginItemType.HealthGlobe)
			{
				thisGilesBaseType = PluginBaseItemType.HealthGlobe;
			}
			return thisGilesBaseType;
		}
		public static bool DetermineIsStackable(PluginItemType thisPluginItemType)
		{
			bool bIsStackable = thisPluginItemType == PluginItemType.CraftingMaterial || thisPluginItemType == PluginItemType.CraftTome || thisPluginItemType == PluginItemType.Ruby ||
									  thisPluginItemType == PluginItemType.Diamond || thisPluginItemType == PluginItemType.Emerald || thisPluginItemType == PluginItemType.Topaz || thisPluginItemType == PluginItemType.Amethyst ||
									  thisPluginItemType == PluginItemType.HealthPotion || thisPluginItemType == PluginItemType.CraftingPlan || thisPluginItemType == PluginItemType.Dye ||
									  thisPluginItemType == PluginItemType.InfernalKey || thisPluginItemType == PluginItemType.KeyStone;
			return bIsStackable;
		}
		public static bool DetermineIsStackable(CacheACDItem item)
		{
			PluginItemType thisPluginItemType = DetermineItemType(item);
			bool bIsStackable = thisPluginItemType == PluginItemType.CraftingMaterial || thisPluginItemType == PluginItemType.CraftTome || thisPluginItemType == PluginItemType.Ruby ||
									  thisPluginItemType == PluginItemType.Diamond || thisPluginItemType == PluginItemType.Emerald || thisPluginItemType == PluginItemType.Topaz || thisPluginItemType == PluginItemType.Amethyst ||
									  thisPluginItemType == PluginItemType.HealthPotion || thisPluginItemType == PluginItemType.CraftingPlan || thisPluginItemType == PluginItemType.Dye ||
									  thisPluginItemType == PluginItemType.InfernalKey || thisPluginItemType == PluginItemType.KeyStone;
			return bIsStackable;
		}
		public static bool DetermineIsTwoSlot(PluginItemType thisPluginItemType)
		{
			if (thisPluginItemType == PluginItemType.Axe || thisPluginItemType == PluginItemType.CeremonialKnife || thisPluginItemType == PluginItemType.Dagger ||
				 thisPluginItemType == PluginItemType.FistWeapon || thisPluginItemType == PluginItemType.Mace || thisPluginItemType == PluginItemType.MightyWeapon ||
				 thisPluginItemType == PluginItemType.Spear || thisPluginItemType == PluginItemType.Sword || thisPluginItemType == PluginItemType.Wand ||
				 thisPluginItemType == PluginItemType.TwoHandDaibo || thisPluginItemType == PluginItemType.TwoHandCrossbow || thisPluginItemType == PluginItemType.TwoHandMace ||
				 thisPluginItemType == PluginItemType.TwoHandMighty || thisPluginItemType == PluginItemType.TwoHandPolearm || thisPluginItemType == PluginItemType.TwoHandStaff ||
				 thisPluginItemType == PluginItemType.TwoHandSword || thisPluginItemType == PluginItemType.TwoHandAxe || thisPluginItemType == PluginItemType.HandCrossbow ||
				 thisPluginItemType == PluginItemType.TwoHandBow || thisPluginItemType == PluginItemType.Mojo || thisPluginItemType == PluginItemType.Source ||
				 thisPluginItemType == PluginItemType.Quiver || thisPluginItemType == PluginItemType.Shield || thisPluginItemType == PluginItemType.Boots ||
				 thisPluginItemType == PluginItemType.Bracers || thisPluginItemType == PluginItemType.Chest || thisPluginItemType == PluginItemType.Cloak ||
				 thisPluginItemType == PluginItemType.Gloves || thisPluginItemType == PluginItemType.Helm || thisPluginItemType == PluginItemType.Pants ||
				 thisPluginItemType == PluginItemType.Shoulders || thisPluginItemType == PluginItemType.SpiritStone ||
				 thisPluginItemType == PluginItemType.VoodooMask || thisPluginItemType == PluginItemType.WizardHat || thisPluginItemType == PluginItemType.StaffOfHerding ||
				 thisPluginItemType == PluginItemType.Flail || thisPluginItemType == PluginItemType.TwoHandFlail || thisPluginItemType == PluginItemType.CrusaderShield || thisPluginItemType == PluginItemType.HoradricCache)
				return true;
			return false;
		}
		public static ItemType PluginItemTypeToDBItemType(PluginItemType thisPluginItemType)
		{
			switch (thisPluginItemType)
			{
				case PluginItemType.Axe: return ItemType.Axe;
				case PluginItemType.CeremonialKnife: return ItemType.CeremonialDagger;
				case PluginItemType.HandCrossbow: return ItemType.HandCrossbow;
				case PluginItemType.Dagger: return ItemType.Dagger;
				case PluginItemType.FistWeapon: return ItemType.FistWeapon;

				case PluginItemType.Flail: return ItemType.Flail;
				case PluginItemType.TwoHandFlail: return ItemType.Flail;
				case PluginItemType.CrusaderShield: return ItemType.CrusaderShield;

				case PluginItemType.Mace: return ItemType.Mace;
				case PluginItemType.MightyWeapon: return ItemType.MightyWeapon;
				case PluginItemType.Spear: return ItemType.Spear;
				case PluginItemType.Sword: return ItemType.Sword;
				case PluginItemType.Wand: return ItemType.Wand;
				case PluginItemType.TwoHandAxe: return ItemType.Axe;
				case PluginItemType.TwoHandBow: return ItemType.Bow;
				case PluginItemType.TwoHandDaibo: return ItemType.Daibo;
				case PluginItemType.TwoHandCrossbow: return ItemType.Crossbow;
				case PluginItemType.TwoHandMace: return ItemType.Mace;
				case PluginItemType.TwoHandMighty: return ItemType.MightyWeapon;
				case PluginItemType.TwoHandPolearm: return ItemType.Polearm;
				case PluginItemType.TwoHandStaff: return ItemType.Staff;
				case PluginItemType.TwoHandSword: return ItemType.Sword;
				case PluginItemType.StaffOfHerding: return ItemType.Staff;
				case PluginItemType.Mojo: return ItemType.Mojo;
				case PluginItemType.Source: return ItemType.Orb;
				case PluginItemType.Quiver: return ItemType.Quiver;
				case PluginItemType.Shield: return ItemType.Shield;
				case PluginItemType.Amulet: return ItemType.Amulet;
				case PluginItemType.Ring: return ItemType.Ring;
				case PluginItemType.Belt: return ItemType.Belt;
				case PluginItemType.Boots: return ItemType.Boots;
				case PluginItemType.Bracers: return ItemType.Bracer;
				case PluginItemType.Chest: return ItemType.Chest;
				case PluginItemType.Cloak: return ItemType.Cloak;
				case PluginItemType.Gloves: return ItemType.Gloves;
				case PluginItemType.Helm: return ItemType.Helm;
				case PluginItemType.Pants: return ItemType.Legs;
				case PluginItemType.MightyBelt: return ItemType.MightyBelt;
				case PluginItemType.Shoulders: return ItemType.Shoulder;
				case PluginItemType.SpiritStone: return ItemType.SpiritStone;
				case PluginItemType.VoodooMask: return ItemType.VoodooMask;
				case PluginItemType.WizardHat: return ItemType.WizardHat;
				case PluginItemType.FollowerEnchantress: return ItemType.FollowerSpecial;
				case PluginItemType.FollowerScoundrel: return ItemType.FollowerSpecial;
				case PluginItemType.FollowerTemplar: return ItemType.FollowerSpecial;
				case PluginItemType.CraftingMaterial: return ItemType.CraftingReagent;
				case PluginItemType.CraftTome: return ItemType.CraftingPage;
				case PluginItemType.Ruby: return ItemType.Gem;
				case PluginItemType.Emerald: return ItemType.Gem;
				case PluginItemType.Topaz: return ItemType.Gem;
				case PluginItemType.Amethyst: return ItemType.Gem;
				case PluginItemType.Diamond: return ItemType.Gem;
				case PluginItemType.SpecialItem: return ItemType.Unknown;
				case PluginItemType.CraftingPlan: return ItemType.CraftingPlan;
				case PluginItemType.HealthPotion: return ItemType.Potion;
				case PluginItemType.Dye: return ItemType.Unknown;
				case PluginItemType.InfernalKey: return ItemType.Unknown;
				case PluginItemType.MiscBook: return ItemType.CraftingPage;
				case PluginItemType.KeyStone: return ItemType.KeystoneFragment;
				case PluginItemType.HoradricCache: return ItemType.HoradricCache;
			}
			return ItemType.Unknown;
		}

		public static GemQualityType ReturnGemQualityType(int snoid, int itemLevel)
		{
			if (ItemSnoCache.GemsSNOIds.Contains(snoid))
			{
				if (ItemSnoCache.GEMS_FlawlessSNOIds.Contains(snoid)) return GemQualityType.Flawless;
				if (ItemSnoCache.GEMS_PerfectSNOIds.Contains(snoid)) return GemQualityType.Perfect;
				if (ItemSnoCache.GEMS_RadiantSNOIds.Contains(snoid)) return GemQualityType.Radiant;
				if (ItemSnoCache.GEMS_SquareSNOIds.Contains(snoid)) return GemQualityType.Square;
				if (ItemSnoCache.GEMS_FlawlessSquareSNOIds.Contains(snoid)) return GemQualityType.FlawlessSquare;
				if (ItemSnoCache.GEMS_MarquiseSNOIds.Contains(snoid)) return GemQualityType.Marquise;
				if (ItemSnoCache.GEMS_ImperialSNOIds.Contains(snoid)) return GemQualityType.Imperial;
				if (ItemSnoCache.GEMS_FlawlessImperialSNOIds.Contains(snoid)) return GemQualityType.FlawlessImperial;
				if (ItemSnoCache.GEMS_RoyalSNOIds.Contains(snoid)) return GemQualityType.Royal;
				if (ItemSnoCache.GEMS_FlawlessRoyalSNOIds.Contains(snoid)) return GemQualityType.FlawlessRoyal;
			}

			if (itemLevel == 14) return GemQualityType.Chipped;
			if (itemLevel == 22) return GemQualityType.Flawed;
			if (itemLevel == 30) return GemQualityType.Normal;
			if (itemLevel == 36) return GemQualityType.Flawless;
			if (itemLevel == 42) return GemQualityType.Perfect;
			if (itemLevel == 48) return GemQualityType.Radiant;
			if (itemLevel == 54) return GemQualityType.Square;
			if (itemLevel == 60) return GemQualityType.FlawlessSquare;

			return GemQualityType.Unknown;
		}
		public static PotionTypes ReturnPotionType(int snoid)
		{
			if (snoid==304319) return PotionTypes.Regular;
			if (snoid == 344093) return PotionTypes.KulleAid;
			if (snoid == 342824) return PotionTypes.Mutilation;
			if (snoid == 341343) return PotionTypes.Regeneration;
			if (snoid == 341342) return PotionTypes.Diamond;
			if (snoid == 342823) return PotionTypes.Leech;
			if (snoid == 341333) return PotionTypes.Tower;
			return PotionTypes.None;
		}

		public static bool IsUberKey(int SNOID)
		{
			return SNOID == 364694 || SNOID == 364697 || SNOID == 364695 || SNOID == 364696;
		}
	}
}
