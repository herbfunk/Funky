using System;
using fItemPlugin.Items;
using Zeta.Game.Internals.Actors;

namespace fItemPlugin
{
	class Stats
	{
		public int ItemsGambled { get; set; }
		public int TownRuns { get; set; }

		//Normal, Magic, Rare, Legendary, Gems, Crafting Mats, Horadric Caches, Misc (keys)

		public LootStats Magical { get; set; }
		public LootStats Rare { get; set; }
		public LootStats Legendary { get; set; }
		public LootStats Gems { get; set; }
		public LootStats Crafting { get; set; }
		public LootStats Keys { get; set; }
		public LootStats KeyStoneFragments { get; set; }
		public LootStats HoradricCache { get; set; }

		public Stats()
		{
			ItemsGambled = 0;
			TownRuns = 0;
			Magical = new LootStats();
			Rare = new LootStats();
			Legendary = new LootStats();
			Gems = new LootStats();
			Crafting = new LootStats();
			Keys = new LootStats();
			KeyStoneFragments = new LootStats();
			HoradricCache = new LootStats();
		}


		public void StashedItemLog(CacheACDItem i)
		{
			PluginItemType thisPluginItemType = ItemFunc.DetermineItemType(i);
			if (thisPluginItemType == PluginItemType.InfernalKey)
			{
				Keys.Stashed++;
				return;
			}
			if (thisPluginItemType == PluginItemType.HoradricCache)
			{
				HoradricCache.Stashed++;
			}
			if (thisPluginItemType == PluginItemType.KeyStone)
			{
				KeyStoneFragments.Stashed += i.ThisItemStackQuantity;
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

		private string GenerateItemString()
		{
			return String.Format("Type       \t Stash \t Sold \t Salvaged\r\n" +
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

		internal string GenerateOutputString()
		{
			return String.Format("Town Runs: {0}\r\n" +
			                     "Items Gambled: {1}\r\n" +
			                     "{2}",
							 TownRuns,
							 ItemsGambled,
							 GenerateItemString());
		}

		public enum LootStatTypes
		{
			Stashed,
			Salvaged,
			Vendored,
		}

		public class LootStats
		{
			public int Stashed { get; set; }
			public int Salvaged { get; set; }
			public int Vendored { get; set; }
		

			public LootStats()
			{
				Stashed = 0;
				Salvaged = 0;
				Vendored = 0;
			}


			public override string ToString()
			{
				return String.Format("{0} \t {1} \t {2}",
									Stashed, Vendored, Salvaged);
			}
		}

	}
}
