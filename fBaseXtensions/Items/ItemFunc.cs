using System;
using System.Linq;
using fBaseXtensions.Cache;
using fBaseXtensions.Cache.External.Objects;
using fBaseXtensions.Cache.Internal;
using Zeta.Game.Internals.Actors;
using fBaseXtensions.Items.Enums;

namespace fBaseXtensions.Items
{
	public static class ItemFunc
	{
		public static ItemStringEntry DetermineIsItemActorType(string internalName)
		{
			internalName = internalName.ToLower();
			return TheCache.ObjectIDCache.ItemDroppedInternalNames.FirstOrDefault(entry => internalName.Contains(entry.ID));
		}
		public static PluginDroppedItemTypes DetermineDroppedItemType(int SNO, string internalname="")
		{
			bool searchName = internalname != "";
			//TheCache.ObjectIDCache.FindDroppedItemEntry(SNO);
			var retEntry = TheCache.ObjectIDCache.ItemDroppedEntries.Values.FirstOrDefault(e => e.SnoId == SNO || (searchName && e.InternalName != String.Empty && String.Compare(internalname, e.InternalName, StringComparison.InvariantCultureIgnoreCase) > 0));
			//var retEntry = SNOCache.IdCollections.ItemsSno.DroppedItems.FirstOrDefault(e => e.Sno == SNO);
			if (retEntry != null) return (PluginDroppedItemTypes)retEntry.ObjectType;
			return PluginDroppedItemTypes.Unknown;
		}
		public static PluginDroppedItemTypes DetermineDroppedItemType(string internalName, int SNOId=-1)
		{
			internalName = internalName.ToLower();

			if (internalName.Contains("ruby_")) return PluginDroppedItemTypes.Ruby;
			if (internalName.Contains("emerald_")) return PluginDroppedItemTypes.Emerald;
			if (internalName.Contains("topaz_")) return PluginDroppedItemTypes.Topaz;
			if (internalName.Contains("amethyst")) return PluginDroppedItemTypes.Amethyst;
			if (internalName.Contains("diamond_")) return PluginDroppedItemTypes.Diamond;
			if (internalName.Contains("horadricrelic")) return PluginDroppedItemTypes.BloodShard;
			if (IsUberKey(SNOId) || internalName.Contains("demonkey_") || internalName.Contains("demontrebuchetkey"))
				return PluginDroppedItemTypes.InfernalKey;
			if (internalName.Contains("craftingreagent_legendary")) return PluginDroppedItemTypes.CraftingMaterial;
			if (internalName.Contains("craftingreagent")) return PluginDroppedItemTypes.CraftingMaterial;
			if (internalName.Contains("lootrunkey")) return PluginDroppedItemTypes.KeyFragment;
			if (internalName.Contains("crafting_") || internalName.Contains("craftingmaterials_")) return PluginDroppedItemTypes.CraftingMaterial;


			if (internalName.Contains("flail1h_")) return PluginDroppedItemTypes.Flail;
			if (internalName.Contains("flail")) return PluginDroppedItemTypes.FlailTwoHanded;

			if (internalName.Contains("twohandedaxe_")) return PluginDroppedItemTypes.AxeTwoHanded;
			if (internalName.Contains("axe_")) return PluginDroppedItemTypes.Axe;

			if (internalName.Contains("ceremonialdagger_")) return PluginDroppedItemTypes.CeremonialKnife;
			if (internalName.Contains("handxbow_")) return PluginDroppedItemTypes.HandCrossbow;
			if (internalName.Contains("dagger_")) return PluginDroppedItemTypes.Dagger;
			if (internalName.Contains("fistweapon_")) return PluginDroppedItemTypes.FistWeapon;

			if (internalName.Contains("twohandedmace_")) return PluginDroppedItemTypes.MaceTwoHanded;
			if (internalName.Contains("mace_")) return PluginDroppedItemTypes.Mace;

			if (internalName.Contains("mightyweapon_1h_")) return PluginDroppedItemTypes.MightyWeapon;
			if (internalName.Contains("spear_")) return PluginDroppedItemTypes.Spear;

			if (internalName.Contains("twohandedsword_")) return PluginDroppedItemTypes.SwordTwoHanded;
			if (internalName.Contains("sword_")) return PluginDroppedItemTypes.Sword;

			if (internalName.Contains("wand_")) return PluginDroppedItemTypes.Wand;

			if (internalName.Contains("xbow_")) return PluginDroppedItemTypes.Crossbow;
			if (internalName.Contains("bow_")) return PluginDroppedItemTypes.Bow;
			if (internalName.Contains("combatstaff_")) return PluginDroppedItemTypes.Daibo;



			if (internalName.Contains("mightyweapon_2h_")) return PluginDroppedItemTypes.MightyWeaponTwoHanded;
			if (internalName.Contains("polearm_")) return PluginDroppedItemTypes.Polearm;
			if (internalName.Contains("staff_")) return PluginDroppedItemTypes.Staff;


			if (internalName.Contains("mojo_")) return PluginDroppedItemTypes.Mojo;
			if (internalName.Contains("orb_")) return PluginDroppedItemTypes.Source;
			if (internalName.Contains("quiver_")) return PluginDroppedItemTypes.Quiver;
			if (internalName.Contains("crushield_")) return PluginDroppedItemTypes.CrusaderShield;
			if (internalName.Contains("shield_")) return PluginDroppedItemTypes.Shield;

			if (internalName.Contains("amulet_")) return PluginDroppedItemTypes.Amulet;
			if (internalName.Contains("ring_")) return PluginDroppedItemTypes.Ring;
			if (internalName.Contains("boots_")) return PluginDroppedItemTypes.Boots;
			if (internalName.Contains("bracers_")) return PluginDroppedItemTypes.Bracers;
			if (internalName.Contains("cloak_")) return PluginDroppedItemTypes.Chest;
			if (internalName.Contains("gloves_")) return PluginDroppedItemTypes.Gloves;
			if (internalName.Contains("pants_")) return PluginDroppedItemTypes.Pants;
			if (internalName.Contains("barbbelt_")) return PluginDroppedItemTypes.Belt;
			if (internalName.Contains("shoulderpads_")) return PluginDroppedItemTypes.Shoulders;
			if (internalName.Contains("spiritstone_")) return PluginDroppedItemTypes.Helm;
			if (internalName.Contains("voodoomask_")) return PluginDroppedItemTypes.Helm;
			if (internalName.Contains("wizardhat_")) return PluginDroppedItemTypes.Helm;
			if (internalName.Contains("lore_book_")) return PluginDroppedItemTypes.LoreBook;
			if (internalName.Contains("page_of_")) return PluginDroppedItemTypes.CraftingMaterial;
			if (internalName.Contains("blacksmithstome")) return PluginDroppedItemTypes.CraftingMaterial;

			if (internalName.Contains("healthpotion")) return PluginDroppedItemTypes.Potion;
			if (internalName.Contains("followeritem_enchantress_")) return PluginDroppedItemTypes.FollowerTrinket;
			if (internalName.Contains("followeritem_scoundrel_")) return PluginDroppedItemTypes.FollowerTrinket;
			if (internalName.Contains("followeritem_templar_")) return PluginDroppedItemTypes.FollowerTrinket;
			if (internalName.Contains("jewelbox_")) return PluginDroppedItemTypes.FollowerTrinket;
			if (internalName.Contains("craftingplan_")) return PluginDroppedItemTypes.CraftingMaterial;

	
			if (internalName.Contains("healthglobe")) return PluginDroppedItemTypes.HealthGlobe;
			if (internalName.Contains("chestarmor_")) return PluginDroppedItemTypes.Chest;
			if (internalName.Contains("helm_")) return PluginDroppedItemTypes.Helm;
			if (internalName.Contains("helmcloth_")) return PluginDroppedItemTypes.Helm;
			if (internalName.Contains("belt_")) return PluginDroppedItemTypes.Belt;


			return PluginDroppedItemTypes.Unknown;
		}
		public static PluginItemTypes DetermineItemType(CacheACDItem cacheItem)
		{
			return DetermineItemType(cacheItem.ThisInternalName, cacheItem.ThisDBItemType, cacheItem.ThisFollowerType, cacheItem.SNO);
		}
		public static PluginBaseItemTypes DetermineBaseItemType(PluginDroppedItemTypes dropitemtype)
		{
			switch (dropitemtype)
			{
				case PluginDroppedItemTypes.HealthGlobe:
				case PluginDroppedItemTypes.PowerGlobe:
					return PluginBaseItemTypes.HealthGlobe;

				case PluginDroppedItemTypes.LoreBook:
				case PluginDroppedItemTypes.InfernalKey:
				case PluginDroppedItemTypes.KeyFragment:
				case PluginDroppedItemTypes.CraftingMaterial:
				case PluginDroppedItemTypes.Potion:
				case PluginDroppedItemTypes.Gold:
				case PluginDroppedItemTypes.BloodShard:
					return PluginBaseItemTypes.Misc;

				case PluginDroppedItemTypes.Amethyst:
				case PluginDroppedItemTypes.Diamond:
				case PluginDroppedItemTypes.Emerald:
				case PluginDroppedItemTypes.Ruby:
				case PluginDroppedItemTypes.Topaz:
					return PluginBaseItemTypes.Gem;

				case PluginDroppedItemTypes.Belt:
				case PluginDroppedItemTypes.Boots:
				case PluginDroppedItemTypes.Bracers:
				case PluginDroppedItemTypes.Chest:
				case PluginDroppedItemTypes.Gloves:
				case PluginDroppedItemTypes.Helm:
				case PluginDroppedItemTypes.Pants:
				case PluginDroppedItemTypes.Shoulders:
					return PluginBaseItemTypes.Armor;

				case PluginDroppedItemTypes.Axe:
				case PluginDroppedItemTypes.CeremonialKnife:
				case PluginDroppedItemTypes.Dagger:
				case PluginDroppedItemTypes.FistWeapon:
				case PluginDroppedItemTypes.Flail:
				case PluginDroppedItemTypes.Mace:
				case PluginDroppedItemTypes.MightyWeapon:
				case PluginDroppedItemTypes.Spear:
				case PluginDroppedItemTypes.Sword:
				case PluginDroppedItemTypes.Wand:
					return PluginBaseItemTypes.WeaponOneHand;

				case PluginDroppedItemTypes.AxeTwoHanded:
				case PluginDroppedItemTypes.Daibo:
				case PluginDroppedItemTypes.FlailTwoHanded:
				case PluginDroppedItemTypes.MaceTwoHanded:
				case PluginDroppedItemTypes.MightyWeaponTwoHanded:
				case PluginDroppedItemTypes.Polearm:
				case PluginDroppedItemTypes.Staff:
				case PluginDroppedItemTypes.SwordTwoHanded:
					return PluginBaseItemTypes.WeaponTwoHand;

				case PluginDroppedItemTypes.Bow:
				case PluginDroppedItemTypes.Crossbow:
				case PluginDroppedItemTypes.HandCrossbow:
					return PluginBaseItemTypes.WeaponRange;

				case PluginDroppedItemTypes.CrusaderShield:
				case PluginDroppedItemTypes.Mojo:
				case PluginDroppedItemTypes.Quiver:
				case PluginDroppedItemTypes.Shield:
				case PluginDroppedItemTypes.Source:
					return PluginBaseItemTypes.Offhand;

				case PluginDroppedItemTypes.Ring:
				case PluginDroppedItemTypes.Amulet:
					return PluginBaseItemTypes.Jewelry;

				case PluginDroppedItemTypes.FollowerTrinket:
					return PluginBaseItemTypes.FollowerItem;
			}
			return PluginBaseItemTypes.Unknown;
		}
		public static PluginBaseItemTypes DetermineBaseItemType(string internalName, int SNOId=-1)
		{
			internalName = internalName.ToLower();

			if (internalName.Contains("ruby_")) return PluginBaseItemTypes.Gem;
			if (internalName.Contains("emerald_")) return PluginBaseItemTypes.Gem;
			if (internalName.Contains("topaz_")) return PluginBaseItemTypes.Gem;
			if (internalName.Contains("amethyst")) return PluginBaseItemTypes.Gem;
			if (internalName.Contains("diamond_")) return PluginBaseItemTypes.Gem;
			if (internalName.Contains("horadriccache")) return PluginBaseItemTypes.Misc;
			if (internalName.Contains("horadricrelic")) return PluginBaseItemTypes.Misc;
			if (IsUberKey(SNOId) || internalName.Contains("demonkey_") || internalName.Contains("demontrebuchetkey"))
				return PluginBaseItemTypes.Misc;
			if (internalName.Contains("craftingreagent_legendary")) return PluginBaseItemTypes.Misc;
			if (internalName.Contains("craftingreagent")) return PluginBaseItemTypes.Misc;
			if (internalName.Contains("lootrunkey")) return PluginBaseItemTypes.Misc;
			if (internalName.Contains("crafting_") || internalName.Contains("craftingmaterials_")) return PluginBaseItemTypes.Misc;


			if (internalName.Contains("flail1h_")) return PluginBaseItemTypes.WeaponOneHand;
			if (internalName.Contains("flail")) return PluginBaseItemTypes.WeaponTwoHand;

			if (internalName.Contains("twohandedaxe_")) return PluginBaseItemTypes.WeaponTwoHand;
			if (internalName.Contains("axe_")) return PluginBaseItemTypes.WeaponOneHand;

			if (internalName.Contains("ceremonialdagger_")) return PluginBaseItemTypes.WeaponOneHand;
			if (internalName.Contains("handxbow_")) return PluginBaseItemTypes.WeaponRange;
			if (internalName.Contains("dagger_")) return PluginBaseItemTypes.WeaponOneHand;
			if (internalName.Contains("fistweapon_")) return PluginBaseItemTypes.WeaponOneHand;

			if (internalName.Contains("twohandedmace_")) return PluginBaseItemTypes.WeaponTwoHand;
			if (internalName.Contains("mace_")) return PluginBaseItemTypes.WeaponOneHand;

			if (internalName.Contains("mightyweapon_1h_")) return PluginBaseItemTypes.WeaponOneHand;
			if (internalName.Contains("spear_")) return PluginBaseItemTypes.WeaponOneHand;

			if (internalName.Contains("twohandedsword_")) return PluginBaseItemTypes.WeaponTwoHand;
			if (internalName.Contains("sword_")) return PluginBaseItemTypes.WeaponOneHand;

			if (internalName.Contains("wand_")) return PluginBaseItemTypes.WeaponOneHand;

			if (internalName.Contains("xbow_")) return PluginBaseItemTypes.WeaponRange;
			if (internalName.Contains("bow_")) return PluginBaseItemTypes.WeaponRange;
			if (internalName.Contains("combatstaff_")) return PluginBaseItemTypes.WeaponTwoHand;



			if (internalName.Contains("mightyweapon_2h_")) return PluginBaseItemTypes.WeaponTwoHand;
			if (internalName.Contains("polearm_")) return PluginBaseItemTypes.WeaponTwoHand;
			if (internalName.Contains("staff_")) return PluginBaseItemTypes.WeaponTwoHand;

			if (internalName.Contains("staffofcow")) return PluginBaseItemTypes.Misc;

			if (internalName.Contains("mojo_")) return PluginBaseItemTypes.Offhand;
			if (internalName.Contains("orb_")) return PluginBaseItemTypes.Offhand;
			if (internalName.Contains("quiver_")) return PluginBaseItemTypes.Offhand;
			if (internalName.Contains("crushield_")) return PluginBaseItemTypes.Offhand;
			if (internalName.Contains("shield_")) return PluginBaseItemTypes.Offhand;

			if (internalName.Contains("amulet_")) return PluginBaseItemTypes.Jewelry;
			if (internalName.Contains("ring_")) return PluginBaseItemTypes.Jewelry;
			if (internalName.Contains("boots_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("bracers_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("cloak_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("gloves_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("pants_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("barbbelt_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("shoulderpads_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("spiritstone_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("voodoomask_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("wizardhat_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("lore_book_")) return PluginBaseItemTypes.Misc;
			if (internalName.Contains("page_of_")) return PluginBaseItemTypes.Misc;
			if (internalName.Contains("blacksmithstome")) return PluginBaseItemTypes.Misc;

			if (internalName.Contains("healthpotion")) return PluginBaseItemTypes.Misc;
			if (internalName.Contains("followeritem_enchantress_")) return PluginBaseItemTypes.FollowerItem;
			if (internalName.Contains("followeritem_scoundrel_")) return PluginBaseItemTypes.FollowerItem;
			if (internalName.Contains("followeritem_templar_")) return PluginBaseItemTypes.FollowerItem;
			if (internalName.Contains("jewelbox_")) return PluginBaseItemTypes.FollowerItem;
			if (internalName.Contains("craftingplan_")) return PluginBaseItemTypes.Misc;
			if (internalName.Contains("dye_")) return PluginBaseItemTypes.Misc;
			if (internalName.Contains("a1_")) return PluginBaseItemTypes.Misc;
			if (internalName.Contains("healthglobe")) return PluginBaseItemTypes.HealthGlobe;
			if (internalName.Contains("chestarmor_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("helm_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("helmcloth_")) return PluginBaseItemTypes.Armor;
			if (internalName.Contains("belt_")) return PluginBaseItemTypes.Armor;


			return PluginBaseItemTypes.Unknown;
		}

		public static PluginItemTypes DetermineItemType(string sThisInternalName, ItemType DBItemType, FollowerType dbFollowerType = FollowerType.None, int snoid = -1)
		{
			sThisInternalName = sThisInternalName.ToLower();

			if (snoid != -1)
			{
				ItemDataEntry itemEntry;
				if (TheCache.ObjectIDCache.ItemDataEntries.TryGetValue(snoid, out itemEntry))
				{
					return itemEntry.ItemType;
				}
			}

			if (sThisInternalName.Contains("ruby_")) return PluginItemTypes.Ruby;
			if (sThisInternalName.Contains("emerald_")) return PluginItemTypes.Emerald;
			if (sThisInternalName.Contains("topaz_")) return PluginItemTypes.Topaz;
			if (sThisInternalName.Contains("amethyst")) return PluginItemTypes.Amethyst;
			if (sThisInternalName.Contains("diamond_")) return PluginItemTypes.Diamond;


			if (sThisInternalName.Contains("horadriccache")) return PluginItemTypes.HoradricCache;
			if (sThisInternalName.Contains("horadricrelic")) return PluginItemTypes.BloodShard;
			//

			if (IsUberKey(snoid) || sThisInternalName.Contains("demonkey_") || sThisInternalName.Contains("demontrebuchetkey"))
				return PluginItemTypes.InfernalKey;


			if (sThisInternalName.Contains("craftingreagent_legendary")) return PluginItemTypes.LegendaryCraftingMaterial;
			if (sThisInternalName.Contains("craftingreagent")) return PluginItemTypes.CraftingMaterial;
			if (sThisInternalName.Contains("lootrunkey")) return PluginItemTypes.KeyStone;

			// Fall back on some partial DB item type checking 
			if (sThisInternalName.Contains("crafting_") || sThisInternalName.Contains("craftingmaterials_"))
			{
				if (DBItemType == ItemType.CraftingPage) return PluginItemTypes.CraftTome;
				return PluginItemTypes.CraftingMaterial;
			}

			if (sThisInternalName.Contains("flail1h_")) return PluginItemTypes.Flail;
			if (sThisInternalName.Contains("flail")) return PluginItemTypes.TwoHandFlail;

			if (sThisInternalName.Contains("twohandedaxe_")) return PluginItemTypes.TwoHandAxe;
			if (sThisInternalName.Contains("axe_")) return PluginItemTypes.Axe;

			if (sThisInternalName.Contains("ceremonialdagger_")) return PluginItemTypes.CeremonialKnife;
			if (sThisInternalName.Contains("handxbow_")) return PluginItemTypes.HandCrossbow;
			if (sThisInternalName.Contains("dagger_")) return PluginItemTypes.Dagger;
			if (sThisInternalName.Contains("fistweapon_")) return PluginItemTypes.FistWeapon;

			if (sThisInternalName.Contains("twohandedmace_")) return PluginItemTypes.TwoHandMace;
			if (sThisInternalName.Contains("mace_")) return PluginItemTypes.Mace;

			if (sThisInternalName.Contains("mightyweapon_1h_")) return PluginItemTypes.MightyWeapon;
			if (sThisInternalName.Contains("spear_")) return PluginItemTypes.Spear;

			if (sThisInternalName.Contains("twohandedsword_")) return PluginItemTypes.TwoHandSword;
			if (sThisInternalName.Contains("sword_")) return PluginItemTypes.Sword;

			if (sThisInternalName.Contains("wand_")) return PluginItemTypes.Wand;

			if (sThisInternalName.Contains("xbow_")) return PluginItemTypes.TwoHandCrossbow;
			if (sThisInternalName.Contains("bow_")) return PluginItemTypes.TwoHandBow;
			if (sThisInternalName.Contains("combatstaff_")) return PluginItemTypes.TwoHandDaibo;


			if (sThisInternalName.Contains("mightyweapon_2h_")) return PluginItemTypes.TwoHandMighty;
			if (sThisInternalName.Contains("polearm_")) return PluginItemTypes.TwoHandPolearm;
			if (sThisInternalName.Contains("staff_")) return PluginItemTypes.TwoHandStaff;

			if (sThisInternalName.Contains("staffofcow")) return PluginItemTypes.StaffOfHerding;
			if (sThisInternalName.Contains("mojo_")) return PluginItemTypes.Mojo;
			if (sThisInternalName.Contains("orb_")) return PluginItemTypes.Source;
			if (sThisInternalName.Contains("quiver_")) return PluginItemTypes.Quiver;

			if (sThisInternalName.Contains("crushield_")) return PluginItemTypes.CrusaderShield;
			if (sThisInternalName.Contains("shield_")) return PluginItemTypes.Shield;

			if (sThisInternalName.Contains("amulet_")) return PluginItemTypes.Amulet;
			if (sThisInternalName.Contains("ring_")) return PluginItemTypes.Ring;
			if (sThisInternalName.Contains("boots_")) return PluginItemTypes.Boots;
			if (sThisInternalName.Contains("bracers_")) return PluginItemTypes.Bracers;
			if (sThisInternalName.Contains("cloak_")) return PluginItemTypes.Cloak;
			if (sThisInternalName.Contains("gloves_")) return PluginItemTypes.Gloves;
			if (sThisInternalName.Contains("pants_")) return PluginItemTypes.Pants;
			if (sThisInternalName.Contains("barbbelt_")) return PluginItemTypes.MightyBelt;
			if (sThisInternalName.Contains("shoulderpads_")) return PluginItemTypes.Shoulders;
			if (sThisInternalName.Contains("spiritstone_")) return PluginItemTypes.SpiritStone;
			if (sThisInternalName.Contains("voodoomask_")) return PluginItemTypes.VoodooMask;
			if (sThisInternalName.Contains("wizardhat_")) return PluginItemTypes.WizardHat;
			if (sThisInternalName.Contains("lore_book_")) return PluginItemTypes.MiscBook;
			if (sThisInternalName.Contains("page_of_")) return PluginItemTypes.CraftTome;
			if (sThisInternalName.Contains("blacksmithstome")) return PluginItemTypes.CraftTome;

			if (sThisInternalName.Contains("healthpotion")) return PluginItemTypes.HealthPotion;
			if (sThisInternalName.Contains("followeritem_enchantress_")) return PluginItemTypes.FollowerEnchantress;
			if (sThisInternalName.Contains("followeritem_scoundrel_")) return PluginItemTypes.FollowerScoundrel;
			if (sThisInternalName.Contains("followeritem_templar_")) return PluginItemTypes.FollowerTemplar;
			if (sThisInternalName.Contains("craftingplan_")) return PluginItemTypes.CraftingPlan;
			if (sThisInternalName.Contains("dye_")) return PluginItemTypes.Dye;
			if (sThisInternalName.Contains("a1_")) return PluginItemTypes.SpecialItem;
			if (sThisInternalName.Contains("healthglobe")) return PluginItemTypes.HealthGlobe;


			// Follower item types
			if (sThisInternalName.Contains("jewelbox_") || DBItemType == ItemType.FollowerSpecial)
			{
				if (dbFollowerType == FollowerType.Scoundrel)
					return PluginItemTypes.FollowerScoundrel;
				if (dbFollowerType == FollowerType.Templar)
					return PluginItemTypes.FollowerTemplar;
				if (dbFollowerType == FollowerType.Enchantress)
					return PluginItemTypes.FollowerEnchantress;
			}

			if (sThisInternalName.Contains("chestarmor_"))
			{
				if (DBItemType == ItemType.Cloak) return PluginItemTypes.Cloak;
				return PluginItemTypes.Chest;
			}
			if (sThisInternalName.Contains("helm_"))
			{
				if (DBItemType == ItemType.SpiritStone) return PluginItemTypes.SpiritStone;
				if (DBItemType == ItemType.VoodooMask) return PluginItemTypes.VoodooMask;
				if (DBItemType == ItemType.WizardHat) return PluginItemTypes.WizardHat;
				return PluginItemTypes.Helm;
			}
			if (sThisInternalName.Contains("helmcloth_"))
			{
				if (DBItemType == ItemType.SpiritStone) return PluginItemTypes.SpiritStone;
				if (DBItemType == ItemType.VoodooMask) return PluginItemTypes.VoodooMask;
				if (DBItemType == ItemType.WizardHat) return PluginItemTypes.WizardHat;
				return PluginItemTypes.Helm;
			}
			if (sThisInternalName.Contains("belt_"))
			{
				if (DBItemType == ItemType.MightyBelt) return PluginItemTypes.MightyBelt;
				return PluginItemTypes.Belt;
			}


			return PluginItemTypes.Unknown;
		}
		//public static bool DetermineItemTypeUsingSNO(int snoid, out PluginItemTypes types)
		//{
		//	types = PluginItemTypes.Unknown;

		//	if (ItemSnoCache.GEMS_AmethystSNOIds.Contains(snoid))
		//	{
		//		types = PluginItemTypes.Amethyst;
		//		return true;
		//	}
		//	if (ItemSnoCache.GEMS_RubySNOIds.Contains(snoid))
		//	{
		//		types = PluginItemTypes.Ruby;
		//		return true;
		//	}
		//	if (ItemSnoCache.GEMS_TopazSNOIds.Contains(snoid))
		//	{
		//		types = PluginItemTypes.Topaz;
		//		return true;
		//	}
		//	if (ItemSnoCache.GEMS_EmeraldSNOIds.Contains(snoid))
		//	{
		//		types = PluginItemTypes.Emerald;
		//		return true;
		//	}
		//	if (ItemSnoCache.GEMS_DiamondSNOIds.Contains(snoid))
		//	{
		//		types = PluginItemTypes.Diamond;
		//		return true;
		//	}
		//	if (ItemSnoCache.HealthPotionSNOIds.Contains(snoid))
		//	{
		//		types = PluginItemTypes.HealthPotion;
		//		return true;
		//	}
		//	if (ItemSnoCache.CraftingMaterialSNOIds.Contains(snoid))
		//	{
		//		types = PluginItemTypes.CraftingMaterial;
		//		return true;
		//	}
		//	if (ItemSnoCache.InfernalKeySNOIds.Contains(snoid))
		//	{
		//		types = PluginItemTypes.InfernalKey;
		//		return true;
		//	}
		//	if (ItemSnoCache.MiscItemSNOIds.Contains(snoid))
		//	{
		//		types = PluginItemTypes.CraftingMaterial;
		//		return true;
		//	}
		//	if (ItemSnoCache.DyesSNOIds.Contains(snoid))
		//	{
		//		types = PluginItemTypes.Dye;
		//		return true;
		//	}
		//	if (ItemSnoCache.HoradricCacheSNOIds.Contains(snoid))
		//	{
		//		types=PluginItemTypes.HoradricCache;
		//		return true;
		//	}

		//	return types != PluginItemTypes.Unknown;
		//}
		public static PluginBaseItemTypes DetermineBaseType(PluginItemTypes thisPluginItemTypes)
		{
			PluginBaseItemTypes thisGilesBaseTypes = PluginBaseItemTypes.Unknown;
			if (thisPluginItemTypes == PluginItemTypes.Axe || thisPluginItemTypes == PluginItemTypes.CeremonialKnife || thisPluginItemTypes == PluginItemTypes.Dagger ||
				 thisPluginItemTypes == PluginItemTypes.FistWeapon || thisPluginItemTypes == PluginItemTypes.Mace || thisPluginItemTypes == PluginItemTypes.MightyWeapon ||
				 thisPluginItemTypes == PluginItemTypes.Spear || thisPluginItemTypes == PluginItemTypes.Sword || thisPluginItemTypes == PluginItemTypes.Wand || thisPluginItemTypes == PluginItemTypes.Flail)
			{
				thisGilesBaseTypes = PluginBaseItemTypes.WeaponOneHand;
			}
			else if (thisPluginItemTypes == PluginItemTypes.TwoHandDaibo || thisPluginItemTypes == PluginItemTypes.TwoHandMace ||
				 thisPluginItemTypes == PluginItemTypes.TwoHandMighty || thisPluginItemTypes == PluginItemTypes.TwoHandPolearm || thisPluginItemTypes == PluginItemTypes.TwoHandStaff ||
				 thisPluginItemTypes == PluginItemTypes.TwoHandSword || thisPluginItemTypes == PluginItemTypes.TwoHandAxe || thisPluginItemTypes == PluginItemTypes.TwoHandFlail)
			{
				thisGilesBaseTypes = PluginBaseItemTypes.WeaponTwoHand;
			}
			else if (thisPluginItemTypes == PluginItemTypes.TwoHandCrossbow || thisPluginItemTypes == PluginItemTypes.HandCrossbow || thisPluginItemTypes == PluginItemTypes.TwoHandBow)
			{
				thisGilesBaseTypes = PluginBaseItemTypes.WeaponRange;
			}
			else if (thisPluginItemTypes == PluginItemTypes.Mojo || thisPluginItemTypes == PluginItemTypes.Source ||
				 thisPluginItemTypes == PluginItemTypes.Quiver || thisPluginItemTypes == PluginItemTypes.Shield || thisPluginItemTypes == PluginItemTypes.CrusaderShield)
			{
				thisGilesBaseTypes = PluginBaseItemTypes.Offhand;
			}
			else if (thisPluginItemTypes == PluginItemTypes.Boots || thisPluginItemTypes == PluginItemTypes.Bracers || thisPluginItemTypes == PluginItemTypes.Chest ||
				 thisPluginItemTypes == PluginItemTypes.Cloak || thisPluginItemTypes == PluginItemTypes.Gloves || thisPluginItemTypes == PluginItemTypes.Helm ||
				 thisPluginItemTypes == PluginItemTypes.Pants || thisPluginItemTypes == PluginItemTypes.Shoulders || thisPluginItemTypes == PluginItemTypes.SpiritStone ||
				 thisPluginItemTypes == PluginItemTypes.VoodooMask || thisPluginItemTypes == PluginItemTypes.WizardHat || thisPluginItemTypes == PluginItemTypes.Belt ||
				 thisPluginItemTypes == PluginItemTypes.MightyBelt)
			{
				thisGilesBaseTypes = PluginBaseItemTypes.Armor;
			}
			else if (thisPluginItemTypes == PluginItemTypes.Amulet || thisPluginItemTypes == PluginItemTypes.Ring)
			{
				thisGilesBaseTypes = PluginBaseItemTypes.Jewelry;
			}
			else if (thisPluginItemTypes == PluginItemTypes.FollowerEnchantress || thisPluginItemTypes == PluginItemTypes.FollowerScoundrel ||
				 thisPluginItemTypes == PluginItemTypes.FollowerTemplar)
			{
				thisGilesBaseTypes = PluginBaseItemTypes.FollowerItem;
			}
			else if (thisPluginItemTypes == PluginItemTypes.CraftingMaterial || thisPluginItemTypes == PluginItemTypes.CraftTome || thisPluginItemTypes == PluginItemTypes.MiscBook ||
				 thisPluginItemTypes == PluginItemTypes.SpecialItem || thisPluginItemTypes == PluginItemTypes.CraftingPlan || thisPluginItemTypes == PluginItemTypes.HealthPotion ||
				 thisPluginItemTypes == PluginItemTypes.Dye || thisPluginItemTypes == PluginItemTypes.StaffOfHerding || thisPluginItemTypes == PluginItemTypes.InfernalKey ||
				thisPluginItemTypes == PluginItemTypes.KeyStone || thisPluginItemTypes == PluginItemTypes.HoradricCache || thisPluginItemTypes == PluginItemTypes.BloodShard)
			{
				thisGilesBaseTypes = PluginBaseItemTypes.Misc;
			}
			else if (thisPluginItemTypes == PluginItemTypes.Ruby || thisPluginItemTypes == PluginItemTypes.Emerald || thisPluginItemTypes == PluginItemTypes.Topaz ||
				 thisPluginItemTypes == PluginItemTypes.Amethyst || thisPluginItemTypes == PluginItemTypes.Diamond)
			{
				thisGilesBaseTypes = PluginBaseItemTypes.Gem;
			}
			else if (thisPluginItemTypes == PluginItemTypes.HealthGlobe)
			{
				thisGilesBaseTypes = PluginBaseItemTypes.HealthGlobe;
			}
			return thisGilesBaseTypes;
		}
		public static bool DetermineIsStackable(PluginItemTypes thisPluginItemTypes)
		{
			bool bIsStackable = thisPluginItemTypes == PluginItemTypes.CraftingMaterial || thisPluginItemTypes == PluginItemTypes.CraftTome || thisPluginItemTypes == PluginItemTypes.Ruby ||
									  thisPluginItemTypes == PluginItemTypes.Diamond || thisPluginItemTypes == PluginItemTypes.Emerald || thisPluginItemTypes == PluginItemTypes.Topaz || thisPluginItemTypes == PluginItemTypes.Amethyst ||
									  thisPluginItemTypes == PluginItemTypes.HealthPotion || thisPluginItemTypes == PluginItemTypes.CraftingPlan || thisPluginItemTypes == PluginItemTypes.Dye ||
									  thisPluginItemTypes == PluginItemTypes.InfernalKey || thisPluginItemTypes == PluginItemTypes.KeyStone;
			return bIsStackable;
		}
		public static bool DetermineIsStackable(CacheACDItem item)
		{
			PluginItemTypes thisPluginItemTypes = DetermineItemType(item);
			bool bIsStackable = thisPluginItemTypes == PluginItemTypes.CraftingMaterial || thisPluginItemTypes == PluginItemTypes.CraftTome || thisPluginItemTypes == PluginItemTypes.Ruby ||
									  thisPluginItemTypes == PluginItemTypes.Diamond || thisPluginItemTypes == PluginItemTypes.Emerald || thisPluginItemTypes == PluginItemTypes.Topaz || thisPluginItemTypes == PluginItemTypes.Amethyst ||
									  thisPluginItemTypes == PluginItemTypes.HealthPotion || thisPluginItemTypes == PluginItemTypes.CraftingPlan || thisPluginItemTypes == PluginItemTypes.Dye ||
									  thisPluginItemTypes == PluginItemTypes.InfernalKey || thisPluginItemTypes == PluginItemTypes.KeyStone;
			return bIsStackable;
		}
		public static bool DetermineIsTwoSlot(PluginItemTypes thisPluginItemTypes)
		{
			if (thisPluginItemTypes == PluginItemTypes.Axe || thisPluginItemTypes == PluginItemTypes.CeremonialKnife || thisPluginItemTypes == PluginItemTypes.Dagger ||
				 thisPluginItemTypes == PluginItemTypes.FistWeapon || thisPluginItemTypes == PluginItemTypes.Mace || thisPluginItemTypes == PluginItemTypes.MightyWeapon ||
				 thisPluginItemTypes == PluginItemTypes.Spear || thisPluginItemTypes == PluginItemTypes.Sword || thisPluginItemTypes == PluginItemTypes.Wand ||
				 thisPluginItemTypes == PluginItemTypes.TwoHandDaibo || thisPluginItemTypes == PluginItemTypes.TwoHandCrossbow || thisPluginItemTypes == PluginItemTypes.TwoHandMace ||
				 thisPluginItemTypes == PluginItemTypes.TwoHandMighty || thisPluginItemTypes == PluginItemTypes.TwoHandPolearm || thisPluginItemTypes == PluginItemTypes.TwoHandStaff ||
				 thisPluginItemTypes == PluginItemTypes.TwoHandSword || thisPluginItemTypes == PluginItemTypes.TwoHandAxe || thisPluginItemTypes == PluginItemTypes.HandCrossbow ||
				 thisPluginItemTypes == PluginItemTypes.TwoHandBow || thisPluginItemTypes == PluginItemTypes.Mojo || thisPluginItemTypes == PluginItemTypes.Source ||
				 thisPluginItemTypes == PluginItemTypes.Quiver || thisPluginItemTypes == PluginItemTypes.Shield || thisPluginItemTypes == PluginItemTypes.Boots ||
				 thisPluginItemTypes == PluginItemTypes.Bracers || thisPluginItemTypes == PluginItemTypes.Chest || thisPluginItemTypes == PluginItemTypes.Cloak ||
				 thisPluginItemTypes == PluginItemTypes.Gloves || thisPluginItemTypes == PluginItemTypes.Helm || thisPluginItemTypes == PluginItemTypes.Pants ||
				 thisPluginItemTypes == PluginItemTypes.Shoulders || thisPluginItemTypes == PluginItemTypes.SpiritStone ||
				 thisPluginItemTypes == PluginItemTypes.VoodooMask || thisPluginItemTypes == PluginItemTypes.WizardHat || thisPluginItemTypes == PluginItemTypes.StaffOfHerding ||
				 thisPluginItemTypes == PluginItemTypes.Flail || thisPluginItemTypes == PluginItemTypes.TwoHandFlail || thisPluginItemTypes == PluginItemTypes.CrusaderShield || thisPluginItemTypes == PluginItemTypes.HoradricCache)
				return true;
			return false;
		}
		public static ItemType PluginItemTypeToDBItemType(PluginItemTypes thisPluginItemTypes)
		{
			switch (thisPluginItemTypes)
			{
				case PluginItemTypes.Axe: return ItemType.Axe;
				case PluginItemTypes.CeremonialKnife: return ItemType.CeremonialDagger;
				case PluginItemTypes.HandCrossbow: return ItemType.HandCrossbow;
				case PluginItemTypes.Dagger: return ItemType.Dagger;
				case PluginItemTypes.FistWeapon: return ItemType.FistWeapon;

				case PluginItemTypes.Flail: return ItemType.Flail;
				case PluginItemTypes.TwoHandFlail: return ItemType.Flail;
				case PluginItemTypes.CrusaderShield: return ItemType.CrusaderShield;

				case PluginItemTypes.Mace: return ItemType.Mace;
				case PluginItemTypes.MightyWeapon: return ItemType.MightyWeapon;
				case PluginItemTypes.Spear: return ItemType.Spear;
				case PluginItemTypes.Sword: return ItemType.Sword;
				case PluginItemTypes.Wand: return ItemType.Wand;
				case PluginItemTypes.TwoHandAxe: return ItemType.Axe;
				case PluginItemTypes.TwoHandBow: return ItemType.Bow;
				case PluginItemTypes.TwoHandDaibo: return ItemType.Daibo;
				case PluginItemTypes.TwoHandCrossbow: return ItemType.Crossbow;
				case PluginItemTypes.TwoHandMace: return ItemType.Mace;
				case PluginItemTypes.TwoHandMighty: return ItemType.MightyWeapon;
				case PluginItemTypes.TwoHandPolearm: return ItemType.Polearm;
				case PluginItemTypes.TwoHandStaff: return ItemType.Staff;
				case PluginItemTypes.TwoHandSword: return ItemType.Sword;
				case PluginItemTypes.StaffOfHerding: return ItemType.Staff;
				case PluginItemTypes.Mojo: return ItemType.Mojo;
				case PluginItemTypes.Source: return ItemType.Orb;
				case PluginItemTypes.Quiver: return ItemType.Quiver;
				case PluginItemTypes.Shield: return ItemType.Shield;
				case PluginItemTypes.Amulet: return ItemType.Amulet;
				case PluginItemTypes.Ring: return ItemType.Ring;
				case PluginItemTypes.Belt: return ItemType.Belt;
				case PluginItemTypes.Boots: return ItemType.Boots;
				case PluginItemTypes.Bracers: return ItemType.Bracer;
				case PluginItemTypes.Chest: return ItemType.Chest;
				case PluginItemTypes.Cloak: return ItemType.Cloak;
				case PluginItemTypes.Gloves: return ItemType.Gloves;
				case PluginItemTypes.Helm: return ItemType.Helm;
				case PluginItemTypes.Pants: return ItemType.Legs;
				case PluginItemTypes.MightyBelt: return ItemType.MightyBelt;
				case PluginItemTypes.Shoulders: return ItemType.Shoulder;
				case PluginItemTypes.SpiritStone: return ItemType.SpiritStone;
				case PluginItemTypes.VoodooMask: return ItemType.VoodooMask;
				case PluginItemTypes.WizardHat: return ItemType.WizardHat;
				case PluginItemTypes.FollowerEnchantress: return ItemType.FollowerSpecial;
				case PluginItemTypes.FollowerScoundrel: return ItemType.FollowerSpecial;
				case PluginItemTypes.FollowerTemplar: return ItemType.FollowerSpecial;
				case PluginItemTypes.CraftingMaterial: return ItemType.CraftingReagent;
				case PluginItemTypes.CraftTome: return ItemType.CraftingPage;
				case PluginItemTypes.Ruby: return ItemType.Gem;
				case PluginItemTypes.Emerald: return ItemType.Gem;
				case PluginItemTypes.Topaz: return ItemType.Gem;
				case PluginItemTypes.Amethyst: return ItemType.Gem;
				case PluginItemTypes.Diamond: return ItemType.Gem;
				case PluginItemTypes.SpecialItem: return ItemType.Unknown;
				case PluginItemTypes.CraftingPlan: return ItemType.CraftingPlan;
				case PluginItemTypes.HealthPotion: return ItemType.Potion;
				case PluginItemTypes.Dye: return ItemType.Unknown;
				case PluginItemTypes.InfernalKey: return ItemType.CraftingReagent;
				case PluginItemTypes.MiscBook: return ItemType.CraftingPage;
				case PluginItemTypes.KeyStone: return ItemType.KeystoneFragment;
				case PluginItemTypes.HoradricCache: return ItemType.HoradricCache;
				
			}
			return ItemType.Unknown;
		}
		public static PluginItemTypes DBItemTypeToPluginItemType(ItemType type)
		{
			switch (type)
			{
				case ItemType.Axe:
					return PluginItemTypes.Axe;
				case ItemType.Sword:
					return PluginItemTypes.Sword;
				case ItemType.Mace:
					return PluginItemTypes.Mace;
				case ItemType.Dagger:
					return PluginItemTypes.Dagger;
				case ItemType.Flail:
					return PluginItemTypes.Flail;
				case ItemType.Bow:
					return PluginItemTypes.TwoHandBow;
				case ItemType.Crossbow:
					return PluginItemTypes.TwoHandCrossbow;
				case ItemType.Staff:
					return PluginItemTypes.TwoHandStaff;
				case ItemType.Spear:
					return PluginItemTypes.Spear;
				case ItemType.Shield:
					return PluginItemTypes.Shield;
				case ItemType.CrusaderShield:
					return PluginItemTypes.CrusaderShield;
				case ItemType.Gloves:
					return PluginItemTypes.Gloves;
				case ItemType.Boots:
					return PluginItemTypes.Boots;
				case ItemType.Chest:
					return PluginItemTypes.Chest;
				case ItemType.Ring:
					return PluginItemTypes.Ring;
				case ItemType.Amulet:
					return PluginItemTypes.Amulet;
				case ItemType.Quiver:
					return PluginItemTypes.Quiver;
				case ItemType.Shoulder:
					return PluginItemTypes.Shoulders;
				case ItemType.Legs:
					return PluginItemTypes.Pants;
				case ItemType.FistWeapon:
					return PluginItemTypes.FistWeapon;
				case ItemType.Mojo:
					return PluginItemTypes.Mojo;
				case ItemType.CeremonialDagger:
					return PluginItemTypes.CeremonialKnife;
				case ItemType.WizardHat:
					return PluginItemTypes.WizardHat;
				case ItemType.Helm:
					return PluginItemTypes.Helm;
				case ItemType.Belt:
					return PluginItemTypes.Belt;
				case ItemType.Bracer:
					return PluginItemTypes.Bracers;
				case ItemType.Orb:
					return PluginItemTypes.Source;
				case ItemType.MightyWeapon:
					return PluginItemTypes.MightyWeapon;
				case ItemType.MightyBelt:
					return PluginItemTypes.MightyBelt;
				case ItemType.Polearm:
					return PluginItemTypes.TwoHandPolearm;
				case ItemType.Cloak:
					return PluginItemTypes.Cloak;
				case ItemType.Wand:
					return PluginItemTypes.Wand;
				case ItemType.SpiritStone:
					return PluginItemTypes.SpiritStone;
				case ItemType.Daibo:
					return PluginItemTypes.TwoHandDaibo;
				case ItemType.HandCrossbow:
					return PluginItemTypes.HandCrossbow;
				case ItemType.VoodooMask:
					return PluginItemTypes.VoodooMask;
				case ItemType.Potion:
					return PluginItemTypes.HealthPotion;
				case ItemType.CraftingReagent:
					return PluginItemTypes.CraftingMaterial;
				case ItemType.CraftingPage:
					return PluginItemTypes.CraftingMaterial;
				case ItemType.CraftingPlan:
					return PluginItemTypes.CraftingPlan;
				case ItemType.HoradricCache:
					return PluginItemTypes.HoradricCache;
				case ItemType.KeystoneFragment:
					return PluginItemTypes.KeyStone;
			}

			return PluginItemTypes.Unknown;
		}
		public static GemQualityTypes ReturnGemQualityType(int snoid, int itemLevel)
		{

			if (TheCache.ObjectIDCache.ItemGemEntries.ContainsKey(snoid))
			{
				var entry = TheCache.ObjectIDCache.ItemGemEntries[snoid];
				return entry.Quality;
			}

			if (itemLevel == 14) return GemQualityTypes.Chipped;
			if (itemLevel == 22) return GemQualityTypes.Flawed;
			if (itemLevel == 30) return GemQualityTypes.Normal;
			if (itemLevel == 36) return GemQualityTypes.Flawless;
			if (itemLevel == 42) return GemQualityTypes.Perfect;
			if (itemLevel == 48) return GemQualityTypes.Radiant;
			if (itemLevel == 54) return GemQualityTypes.Square;
			if (itemLevel == 60) return GemQualityTypes.FlawlessSquare;

			return GemQualityTypes.Unknown;
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
		public static FollowerType ReturnFollowerType(PluginItemTypes types)
		{
			if (types==PluginItemTypes.FollowerEnchantress)
				return FollowerType.Enchantress;
			if (types == PluginItemTypes.FollowerTemplar)
				return FollowerType.Templar;
			if (types == PluginItemTypes.FollowerScoundrel)
				return FollowerType.Scoundrel;

			return FollowerType.None;
		}

		public static bool IsUberKey(int SNOID)
		{
			return SNOID == 364694 || SNOID == 364697 || SNOID == 364695 || SNOID == 364696;
		}
	}
}
