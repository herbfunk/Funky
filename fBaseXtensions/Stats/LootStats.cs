using System;
using fBaseXtensions.Game;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Stats
{
	///<summary>
	///Loot Stat Actions
	///</summary>
	public enum LootStatTypes
	{
		Looted,
		Stashed,
		Salvaged,
		Vendored,
		Dropped
	}
	///<summary>
	///Tracking of Loot Specific actions taken
	///</summary>
	public class LootStats
	{
		public int Looted { get; set; }
		public int Stashed { get; set; }
		public int Salvaged { get; set; }
		public int Vendored { get; set; }
		public int Dropped { get; set; }

		public LootStats()
		{
			Looted = 0;
			Stashed = 0;
			Salvaged = 0;
			Vendored = 0;
			Dropped = 0;
		}

		public void Merge(LootStats other)
		{
			Looted += other.Looted;
			Stashed += other.Stashed;
			Salvaged += other.Salvaged;
			Vendored += other.Vendored;
			Dropped += other.Dropped;
		}

		public override string ToString()
		{
			return String.Format("{0} \t {1} \t {2} \t {3} \t {4}",
				Dropped, Looted, Stashed, Vendored, Salvaged);
		}
	}

	///<summary>
	///Container for multiple individual LootStats (Summary)
	///</summary>
	public class LootTracking
	{
		public LootStats Magical { get; set; }
		public LootStats Rare { get; set; }
		public LootStats Legendary { get; set; }
		public LootStats Gems { get; set; }
		public LootStats Crafting { get; set; }
		public LootStats Keys { get; set; }

		public LootStats KeyStoneFragments { get; set; }
		public LootStats HoradricCache { get; set; }

		public LootTracking()
		{
			Magical = new LootStats();
			Rare = new LootStats();
			Legendary = new LootStats();
			Gems = new LootStats();
			Crafting = new LootStats();
			Keys = new LootStats();
			KeyStoneFragments = new LootStats();
			HoradricCache = new LootStats();
		}

		public void Merge(LootTracking other)
		{
			Magical.Merge(other.Magical);
			Rare.Merge(other.Rare);
			Legendary.Merge(other.Legendary);
			Gems.Merge(other.Gems);
			Crafting.Merge(other.Crafting);
			Keys.Merge(other.Keys);
			KeyStoneFragments.Merge(other.KeyStoneFragments);
			HoradricCache.Merge(other.HoradricCache);
		}

		public int GetTotalLootStatCount(LootStatTypes statType)
		{
			switch (statType)
			{
				case LootStatTypes.Looted:
					return Magical.Looted + Rare.Looted + Legendary.Looted + Crafting.Looted + Keys.Looted + Gems.Looted + KeyStoneFragments.Looted + HoradricCache.Looted;
				case LootStatTypes.Stashed:
					return Magical.Stashed + Rare.Stashed + Legendary.Stashed + Crafting.Stashed + Keys.Stashed + Gems.Stashed + KeyStoneFragments.Stashed + HoradricCache.Stashed;
				case LootStatTypes.Salvaged:
					return Magical.Salvaged + Rare.Salvaged + Legendary.Salvaged + Crafting.Salvaged + Keys.Salvaged + Gems.Salvaged + KeyStoneFragments.Salvaged + HoradricCache.Salvaged;
				case LootStatTypes.Vendored:
					return Magical.Vendored + Rare.Vendored + Legendary.Vendored + Crafting.Vendored + Keys.Vendored + Gems.Vendored + KeyStoneFragments.Vendored + HoradricCache.Vendored;
				case LootStatTypes.Dropped:
					return Magical.Dropped + Rare.Dropped + Legendary.Dropped + Crafting.Dropped + Keys.Dropped + Gems.Dropped + KeyStoneFragments.Dropped + HoradricCache.Dropped;
			}

			return 0;
		}

		public void LootedItemLog(PluginItemTypes itemType, PluginBaseItemTypes baseitemType, ItemQuality itemQuality)
		{
			if (itemType == PluginItemTypes.HealthPotion)
				return;


			switch (baseitemType)
			{
				case PluginBaseItemTypes.WeaponOneHand:
				case PluginBaseItemTypes.WeaponTwoHand:
				case PluginBaseItemTypes.WeaponRange:
				case PluginBaseItemTypes.Offhand:
				case PluginBaseItemTypes.Armor:
				case PluginBaseItemTypes.Jewelry:
				case PluginBaseItemTypes.FollowerItem:
					if (itemQuality > ItemQuality.Rare6)
					{
                        FunkyGame.CurrentStats.CurrentProfile.LootTracker.Legendary.Looted++;
					}
					else if (itemQuality > ItemQuality.Magic3)
					{
                        FunkyGame.CurrentStats.CurrentProfile.LootTracker.Rare.Looted++;
					}
					else
					{
                        FunkyGame.CurrentStats.CurrentProfile.LootTracker.Magical.Looted++;
					}
					break;

				case PluginBaseItemTypes.Unknown:
				case PluginBaseItemTypes.Misc:
					if (itemType == PluginItemTypes.CraftingMaterial || itemType == PluginItemTypes.CraftingPlan || itemType == PluginItemTypes.CraftTome)
					{
                        FunkyGame.CurrentStats.CurrentProfile.LootTracker.Crafting.Looted++;
					}
					else if (itemType == PluginItemTypes.InfernalKey)
					{
                        FunkyGame.CurrentStats.CurrentProfile.LootTracker.Keys.Looted++;
					}
					else if (itemType == PluginItemTypes.KeyStone)
					{
                        FunkyGame.CurrentStats.CurrentProfile.LootTracker.KeyStoneFragments.Looted++;
					}
					break;
				case PluginBaseItemTypes.Gem:
                    FunkyGame.CurrentStats.CurrentProfile.LootTracker.Gems.Looted++;
					break;
			}


		}

		public void DroppedItemLog(PluginItemTypes itemType, ItemQuality itemQuality)
		{
			switch (itemType)
			{
				case PluginItemTypes.CraftingMaterial:
				case PluginItemTypes.CraftingPlan:
				case PluginItemTypes.LegendaryCraftingMaterial:
				case PluginItemTypes.CraftTome:
					FunkyGame.CurrentStats.CurrentProfile.LootTracker.Crafting.Dropped++;
					break;

				case PluginItemTypes.Ruby:
				case PluginItemTypes.Emerald:
				case PluginItemTypes.Topaz:
				case PluginItemTypes.Amethyst:
				case PluginItemTypes.Diamond:
					FunkyGame.CurrentStats.CurrentProfile.LootTracker.Gems.Dropped++;
					break;


				case PluginItemTypes.Axe:
				case PluginItemTypes.CeremonialKnife:
				case PluginItemTypes.HandCrossbow:
				case PluginItemTypes.Dagger:
				case PluginItemTypes.FistWeapon:
				case PluginItemTypes.Flail:
				case PluginItemTypes.Mace:
				case PluginItemTypes.MightyWeapon:
				case PluginItemTypes.Spear:
				case PluginItemTypes.Sword:
				case PluginItemTypes.Wand:
				case PluginItemTypes.TwoHandAxe:
				case PluginItemTypes.TwoHandBow:
				case PluginItemTypes.TwoHandDaibo:
				case PluginItemTypes.TwoHandCrossbow:
				case PluginItemTypes.TwoHandFlail:
				case PluginItemTypes.TwoHandMace:
				case PluginItemTypes.TwoHandMighty:
				case PluginItemTypes.TwoHandPolearm:
				case PluginItemTypes.TwoHandStaff:
				case PluginItemTypes.TwoHandSword:
				case PluginItemTypes.Mojo:
				case PluginItemTypes.Source:
				case PluginItemTypes.Quiver:
				case PluginItemTypes.Shield:
				case PluginItemTypes.CrusaderShield:
				case PluginItemTypes.Amulet:
				case PluginItemTypes.Ring:
				case PluginItemTypes.Belt:
				case PluginItemTypes.Boots:
				case PluginItemTypes.Bracers:
				case PluginItemTypes.Chest:
				case PluginItemTypes.Cloak:
				case PluginItemTypes.Gloves:
				case PluginItemTypes.Helm:
				case PluginItemTypes.Pants:
				case PluginItemTypes.MightyBelt:
				case PluginItemTypes.Shoulders:
				case PluginItemTypes.SpiritStone:
				case PluginItemTypes.VoodooMask:
				case PluginItemTypes.WizardHat:
					if (itemQuality == ItemQuality.Legendary)
					{
						FunkyGame.CurrentStats.CurrentProfile.LootTracker.Legendary.Dropped++;
					}
					else if (itemQuality > ItemQuality.Magic3)
					{
						FunkyGame.CurrentStats.CurrentProfile.LootTracker.Rare.Dropped++;
					}
					else
					{
						FunkyGame.CurrentStats.CurrentProfile.LootTracker.Magical.Dropped++;
					}
					break;

				case PluginItemTypes.FollowerEnchantress:
				case PluginItemTypes.FollowerScoundrel:
				case PluginItemTypes.FollowerTemplar:
					break;


				case PluginItemTypes.SpecialItem:
					break;

				case PluginItemTypes.HealthPotion:
					break;

				case PluginItemTypes.HealthGlobe:
					break;

				case PluginItemTypes.InfernalKey:
					FunkyGame.CurrentStats.CurrentProfile.LootTracker.Keys.Dropped++;
					break;
				case PluginItemTypes.KeyStone:
					break;
				case PluginItemTypes.BloodShard:
					break;
			}
		}

		public void StashedItemLog(CacheACDItem i)
		{
			PluginItemTypes thisPluginItemType = ItemFunc.DetermineItemType(i);
			if (thisPluginItemType == PluginItemTypes.InfernalKey)
			{
				Keys.Stashed++;
				return;
			}
			if (thisPluginItemType == PluginItemTypes.HoradricCache)
			{
				HoradricCache.Stashed++;
			}
			if (thisPluginItemType == PluginItemTypes.KeyStone)
			{
				KeyStoneFragments.Stashed += (int)i.ThisItemStackQuantity;
			}

			switch (i.ACDItem.ItemType)
			{
				case ItemType.CraftingPage:
				case ItemType.CraftingPlan:
				case ItemType.CraftingReagent:
					Crafting.Stashed++;
					break;
				case ItemType.Gem:
					Gems.Stashed++;
					break;
				case ItemType.Amulet:
				case ItemType.Axe:
				case ItemType.Belt:
				case ItemType.Boots:
				case ItemType.Bow:
				case ItemType.Bracer:
				case ItemType.CeremonialDagger:
				case ItemType.Chest:
				case ItemType.Cloak:
				case ItemType.Crossbow:
				case ItemType.Dagger:
				case ItemType.Daibo:
				case ItemType.FistWeapon:
				case ItemType.FollowerSpecial:
				case ItemType.Flail:
				case ItemType.CrusaderShield:
				case ItemType.Gloves:
				case ItemType.HandCrossbow:
				case ItemType.Helm:
				case ItemType.Legs:
				case ItemType.Mace:
				case ItemType.MightyBelt:
				case ItemType.MightyWeapon:
				case ItemType.Mojo:
				case ItemType.Orb:
				case ItemType.Polearm:
				case ItemType.Quiver:
				case ItemType.Ring:
				case ItemType.Shield:
				case ItemType.Shoulder:
				case ItemType.Spear:
				case ItemType.SpiritStone:
				case ItemType.Staff:
				case ItemType.Sword:
				case ItemType.VoodooMask:
				case ItemType.Wand:
				case ItemType.WizardHat:
					if (i.ThisQuality == ItemQuality.Legendary)
					{
						Legendary.Stashed++;
					}
					else if (i.ThisQuality > ItemQuality.Magic3)
					{
						Rare.Stashed++;
					}
					else
					{
						Magical.Stashed++;
					}
					break;
			}
		}
		public void SalvagedItemLog(CacheACDItem i)
		{
			if (i.ThisQuality == ItemQuality.Legendary)
			{
				Legendary.Salvaged++;
			}
			else if (i.ThisQuality > ItemQuality.Magic3)
			{
				Rare.Salvaged++;
			}
			else
			{
				Magical.Salvaged++;
			}
		}
		public void VendoredItemLog(CacheACDItem i)
		{
			switch (i.ACDItem.ItemType)
			{
				case ItemType.CraftingPage:
				case ItemType.CraftingPlan:
				case ItemType.CraftingReagent:
					Crafting.Vendored++;
					break;
				case ItemType.Gem:
					Gems.Vendored++;
					break;
				case ItemType.Amulet:
				case ItemType.Axe:
				case ItemType.Belt:
				case ItemType.Boots:
				case ItemType.Bow:
				case ItemType.Bracer:
				case ItemType.CeremonialDagger:
				case ItemType.Chest:
				case ItemType.Cloak:
				case ItemType.Crossbow:
				case ItemType.Dagger:
				case ItemType.Daibo:
				case ItemType.FistWeapon:
				case ItemType.FollowerSpecial:
				case ItemType.Gloves:
				case ItemType.HandCrossbow:
				case ItemType.Helm:
				case ItemType.Legs:
				case ItemType.Mace:
				case ItemType.MightyBelt:
				case ItemType.MightyWeapon:
				case ItemType.Mojo:
				case ItemType.Orb:
				case ItemType.Polearm:
				case ItemType.Quiver:
				case ItemType.Ring:
				case ItemType.Shield:
				case ItemType.Shoulder:
				case ItemType.Spear:
				case ItemType.SpiritStone:
				case ItemType.Staff:
				case ItemType.Sword:
				case ItemType.VoodooMask:
				case ItemType.Wand:
				case ItemType.WizardHat:
					if (i.ThisQuality == ItemQuality.Legendary)
					{
						Legendary.Vendored++;
					}
					else if (i.ThisQuality > ItemQuality.Magic3)
					{
						Rare.Vendored++;
					}
					else
					{
						Magical.Vendored++;
					}
					break;
			}
		}


		public override string ToString()
		{
			return String.Format("Type       \t Drop \t Loot \t Stash \t Sold \t Salvaged\r\n" +
								 "Magical:    \t {0} \r\n" +
								 "Rare:      \t {1} \r\n" +
								 "Legendary: \t {2} \r\n" +
								 "Gems:      \t {3} \r\n" +
								 "Crafting: \t {4} \r\n" +
								 "Keys:      \t {5} \r\n" +
								 "KeyFrags: \t {6} \r\n" +
								 "Cache:     \t {7} \r\n",
								 Magical, Rare, Legendary, Gems, Crafting, Keys, KeyStoneFragments, HoradricCache);
		}
	}
}
