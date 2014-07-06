using System;
using fItemPlugin.Items;
using FunkyBot.Cache.Objects;
using Zeta.Game.Internals.Actors;


namespace FunkyBot.Game.ProfileTracking
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
		public int Dropped { get; set; }

		public LootStats()
		{
			Looted = 0;
			Dropped = 0;
		}

		public void Merge(LootStats other)
		{
			Looted += other.Looted;
			Dropped += other.Dropped;
		}

		public override string ToString()
		{
			return String.Format("{0} \t {1}",
				Dropped, Looted);
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
				case LootStatTypes.Dropped:
					return Magical.Dropped + Rare.Dropped + Legendary.Dropped + Crafting.Dropped + Keys.Dropped + Gems.Dropped + KeyStoneFragments.Dropped + HoradricCache.Dropped;
			}

			return 0;
		}

		public void LootedItemLog(CacheItem thisCacheItem)
		{
			PluginItemType thisgilesitemtype = ItemFunc.DetermineItemType(thisCacheItem.InternalName, thisCacheItem.BalanceData.thisItemType, thisCacheItem.BalanceData.thisFollowerType);
			PluginBaseItemType thisgilesbasetype = ItemFunc.DetermineBaseType(thisgilesitemtype);
			ItemQuality itemQuality = thisCacheItem.Itemquality.HasValue ? thisCacheItem.Itemquality.Value : ItemQuality.Invalid;

			if (thisgilesitemtype == PluginItemType.HealthPotion)
				return;


			switch (thisgilesbasetype)
			{
				case PluginBaseItemType.WeaponOneHand:
				case PluginBaseItemType.WeaponTwoHand:
				case PluginBaseItemType.WeaponRange:
				case PluginBaseItemType.Offhand:
				case PluginBaseItemType.Armor:
				case PluginBaseItemType.Jewelry:
				case PluginBaseItemType.FollowerItem:
					if (itemQuality > ItemQuality.Rare6)
					{
						Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Legendary.Looted++;
					}
					else if (itemQuality > ItemQuality.Magic3)
					{
						Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Rare.Looted++;
					}
					else
					{
						Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Magical.Looted++;
					}
					break;

				case PluginBaseItemType.Unknown:
				case PluginBaseItemType.Misc:
					if (thisgilesitemtype == PluginItemType.CraftingMaterial || thisgilesitemtype == PluginItemType.CraftingPlan || thisgilesitemtype == PluginItemType.CraftTome)
					{
						Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Crafting.Looted++;
					}
					else if (thisgilesitemtype == PluginItemType.InfernalKey)
					{
						Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Keys.Looted++;
					}
					else if(thisgilesitemtype == PluginItemType.KeyStone)
					{
						Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.KeyStoneFragments.Looted++;
					}
					break;
				case PluginBaseItemType.Gem:
					Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Gems.Looted++;
					break;
			}


		}

		public void DroppedItemLog(CacheItem i)
		{
			CacheBalance thisBalanceData = i.BalanceData;
			PluginItemType thisGilesItemType = ItemFunc.DetermineItemType(i.InternalName, thisBalanceData.thisItemType, thisBalanceData.thisFollowerType);
			if (thisGilesItemType == PluginItemType.InfernalKey)
			{
				Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Keys.Dropped++;
				//	 Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[(int)LootIndex.Key]++;
				return;
			}

			switch (thisBalanceData.thisItemType)
			{
				case ItemType.CraftingPage:
				case ItemType.CraftingPlan:
				case ItemType.CraftingReagent:
					Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Crafting.Dropped++;
					//  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[(int)LootIndex.Crafting]++;
					break;
				case ItemType.Gem:
					Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Gems.Dropped++;
					// Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[(int)LootIndex.Gem]++;
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
					if (i.Itemquality.Value == ItemQuality.Legendary)
					{
						//Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[3]++;
						Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Legendary.Dropped++;
					}
					else if (i.Itemquality.Value > ItemQuality.Magic3)
					{
						// Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[2]++;
						Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Rare.Dropped++;
					}
					else
					{
						//  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[1]++;
						Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.Magical.Dropped++;
					}
					break;
			}
		}

		public override string ToString()
		{
			return String.Format("Type       \t Drop \t Loot\r\n" +
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
