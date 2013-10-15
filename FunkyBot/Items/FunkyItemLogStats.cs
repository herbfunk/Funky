using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using Zeta.Common;
using Zeta.Internals.Actors;

namespace FunkyBot
{
    public partial class Funky
    {
		  public enum LootIndex
		  {
				Misc=0,
				Magical=1,
				Rare=2,
				Legendary=3,
				Gem=4,
				Crafting=5,
				Key=6,
		  }

        private static void LootedItemLog(GilesItemType thisgilesitemtype, GilesBaseItemType thisgilesbasetype, ItemQuality itemQuality)
        {
            if (thisgilesitemtype == GilesItemType.HealthPotion)
                return;

            //No profile set.. because new game?
				if (Bot.BotStatistics.ProfileStats.CurrentProfile==null)
                return;

            switch (thisgilesbasetype)
            {
                case GilesBaseItemType.WeaponOneHand:
                case GilesBaseItemType.WeaponTwoHand:
                case GilesBaseItemType.WeaponRange:
                case GilesBaseItemType.Offhand:
                case GilesBaseItemType.Armor:
                case GilesBaseItemType.Jewelry:
                case GilesBaseItemType.FollowerItem:
                    if (itemQuality > ItemQuality.Rare6)
                    {
								Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[3]++;
                        //Statistics.ItemStats.CurrentGame.lootedItemTotals[3]++;
                    }
                    else if (itemQuality > ItemQuality.Magic3)
                    {
								Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[2]++;
                    }
                    else
								Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[1]++;
                    break;

                case GilesBaseItemType.Unknown:
                case GilesBaseItemType.Misc:
						  if (thisgilesitemtype== GilesItemType.CraftingMaterial||thisgilesitemtype== GilesItemType.CraftingPlan||thisgilesitemtype== GilesItemType.CraftTome)
								Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[(int)LootIndex.Crafting]++;
						  else if (thisgilesitemtype==GilesItemType.InfernalKey)
								Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[(int)LootIndex.Key]++;
						  else
								Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[0]++;
						  break;
                case GilesBaseItemType.Gem:
						  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[(int)LootIndex.Gem]++;
                    break;
            }

            
        }

		  private static void StashedItemLog(CacheACDItem i)
        {
				if (Bot.BotStatistics.ProfileStats.CurrentProfile==null)
                return;

				GilesItemType thisGilesItemType=DetermineItemType(i.ThisInternalName, i.ThisDBItemType, i.ThisFollowerType);
				if (thisGilesItemType== GilesItemType.InfernalKey)
				{
					 Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[(int)LootIndex.Key]++;
					 return;
				}
					

            switch (i.ACDItem.ItemType)
            {
                case ItemType.CraftingPage:
                case ItemType.CraftingPlan:
					 case ItemType.CraftingReagent:
						  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[(int)LootIndex.Crafting]++;
                    break;
                case ItemType.Gem:
						  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[(int)LootIndex.Gem]++;
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
								Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[3]++;
                    else if (i.ThisQuality > ItemQuality.Magic3)
								Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[2]++;
                    else
								Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[1]++;
                    break;
            }
        }


        internal static string ReturnLogOutputString()
        {
				System.TimeSpan diff1=Bot.BotStatistics.ItemStats.TotalTimeTracked();
            int[] stashedCounts, lootedCounts;
				stashedCounts=Bot.BotStatistics.ItemStats.stashedTotals();
				lootedCounts=Bot.BotStatistics.ItemStats.lootedTotals();
            int TotalStashed = 0;
            int TotalLooted = 0;

            for (int i = 0; i < 7; i++)
            {
                TotalStashed += stashedCounts[i];
                TotalLooted += lootedCounts[i];
            }

            double itemPerMin = Math.Round(TotalLooted / diff1.TotalMinutes, 1);
            double totalTime = Math.Round(diff1.TotalMinutes, 1);

            string ReturnStr = "\r\n" + "============================================" + "\r\n" +
                "Total Looted (" + TotalLooted + ") / Stashed (" + TotalStashed + ")" + "\r\n" +
                "Legendaries Looted (" + lootedCounts[3] + ") / Stashed (" + stashedCounts[3] + ")" + "\r\n" +
                "Rares Looted (" + lootedCounts[2] + ") / Stashed (" + stashedCounts[2] + ")" + "\r\n" +
                "Magical Looted (" + lootedCounts[1] + ") / Stashed (" + stashedCounts[1] + ")" + "\r\n" +
                "Misc Looted (" + lootedCounts[0] + ") / Stashed (" + stashedCounts[0] + ")" + "\r\n" +
					 "Gems Looted ("+lootedCounts[(int)LootIndex.Gem]+") / Stashed ("+stashedCounts[(int)LootIndex.Gem]+")"+"\r\n"+
					 "Craft Looted ("+lootedCounts[(int)LootIndex.Crafting]+") / Stashed ("+stashedCounts[(int)LootIndex.Crafting]+")"+"\r\n"+
					 "Keys Looted ("+lootedCounts[(int)LootIndex.Key]+") / Stashed ("+stashedCounts[(int)LootIndex.Key]+")"+"\r\n"+
					 "Items looted per minute "+itemPerMin.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) +"\r\n"+
                "============================================"+"\r\n"+
					  "Total time running "+totalTime.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)+" minutes"+"\r\n"+
					  "Total Game Count "+Bot.BotStatistics.GameStats.TotalGames.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)+"\r\n"+
					  "Total Death Count "+Bot.BotStatistics.GameStats.TotalDeaths.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

            return ReturnStr;
        }

    }
}