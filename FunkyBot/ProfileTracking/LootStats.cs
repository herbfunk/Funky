using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zeta.Internals.Actors;

namespace FunkyBot.ProfileTracking
{
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
            this.Looted += other.Looted;
            this.Stashed += other.Stashed;
            this.Salvaged += other.Salvaged;
            this.Vendored += other.Vendored;
            this.Dropped += other.Dropped;
        }

        public override string ToString()
        {
            return String.Format("Dropped {0} / Looted {1} / Stashed {2} / Vendored {3} / Salvaged {4}", 
                this.Dropped, this.Looted, this.Stashed, this.Vendored, this.Salvaged);
        }
    }
    public class LootTracking
    {
        public LootStats Magical { get; set; }
        public LootStats Rare { get; set; }
        public LootStats Legendary { get; set; }
        public LootStats Gems { get; set; }
        public LootStats Crafting { get; set; }
        public LootStats Keys { get; set; }

        public LootTracking()
        {
            Magical = new LootStats();
            Rare = new LootStats();
            Legendary = new LootStats();
            Gems = new LootStats();
            Crafting = new LootStats();
            Keys = new LootStats();
        }

        public void Merge(LootTracking other)
        {
            Magical.Merge(other.Magical);
            Rare.Merge(other.Rare);
            Legendary.Merge(other.Legendary);
            Gems.Merge(other.Gems);
            Crafting.Merge(other.Crafting);
            Keys.Merge(other.Keys);
        }

        public void LootedItemLog(GilesItemType thisgilesitemtype, GilesBaseItemType thisgilesbasetype, ItemQuality itemQuality)
        {
            if (thisgilesitemtype == GilesItemType.HealthPotion)
                return;

            //No profile set.. because new game?
            //if (Bot.BotStatistics.ProfileStats.CurrentProfile == null)
            //    return;

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
                        //Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[3]++;
                        //Statistics.ItemStats.CurrentGame.lootedItemTotals[3]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Legendary.Looted++;
                    }
                    else if (itemQuality > ItemQuality.Magic3)
                    {
                        //Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[2]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Rare.Looted++;
                    }
                    else
                    {
                        //Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[1]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Magical.Looted++;
                    }
                    break;

                case GilesBaseItemType.Unknown:
                case GilesBaseItemType.Misc:
                    if (thisgilesitemtype == GilesItemType.CraftingMaterial || thisgilesitemtype == GilesItemType.CraftingPlan || thisgilesitemtype == GilesItemType.CraftTome)
                    {
                        //   Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[(int)LootIndex.Crafting]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Crafting.Looted++;
                    }
                    else if (thisgilesitemtype == GilesItemType.InfernalKey)
                    {
                        // Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[(int)LootIndex.Key]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Keys.Looted++;
                    }
                    else
                    {
                        // Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[0]++;
                    }
                    break;
                case GilesBaseItemType.Gem:
                    // Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[(int)LootIndex.Gem]++;
                    ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Gems.Looted++;
                    break;
            }


        }

        public void StashedItemLog(CacheACDItem i)
        {
            //if (Bot.BotStatistics.ProfileStats.CurrentProfile==null)
            //return;

            GilesItemType thisGilesItemType = Funky.DetermineItemType(i.ThisInternalName, i.ThisDBItemType, i.ThisFollowerType);
            if (thisGilesItemType == GilesItemType.InfernalKey)
            {
                ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Keys.Stashed++;
                //	 Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[(int)LootIndex.Key]++;
                return;
            }


            switch (i.ACDItem.ItemType)
            {
                case ItemType.CraftingPage:
                case ItemType.CraftingPlan:
                case ItemType.CraftingReagent:
                    ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Crafting.Stashed++;
                    //  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[(int)LootIndex.Crafting]++;
                    break;
                case ItemType.Gem:
                    ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Gems.Stashed++;
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
                    if (i.ThisQuality == ItemQuality.Legendary)
                    {
                        //Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[3]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Legendary.Stashed++;
                    }
                    else if (i.ThisQuality > ItemQuality.Magic3)
                    {
                        // Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[2]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Rare.Stashed++;
                    }
                    else
                    {
                        //  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[1]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Magical.Stashed++;
                    }
                    break;
            }
        }

        public void SalvagedItemLog(CacheACDItem i)
        {
            if (i.ThisQuality == ItemQuality.Legendary)
            {
                //Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[3]++;
                ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Legendary.Salvaged++;
            }
            else if (i.ThisQuality > ItemQuality.Magic3)
            {
                // Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[2]++;
                ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Rare.Salvaged++;
            }
            else
            {
                //  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[1]++;
                ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Magical.Salvaged++;
            }
            return;
        }

        public void VendoredItemLog(CacheACDItem i)
        {
            //if (Bot.BotStatistics.ProfileStats.CurrentProfile==null)
            //return;

            switch (i.ACDItem.ItemType)
            {
                case ItemType.CraftingPage:
                case ItemType.CraftingPlan:
                case ItemType.CraftingReagent:
                    ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Crafting.Vendored++;
                    //  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[(int)LootIndex.Crafting]++;
                    break;
                case ItemType.Gem:
                    ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Gems.Vendored++;
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
                    if (i.ThisQuality == ItemQuality.Legendary)
                    {
                        //Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[3]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Legendary.Vendored++;
                    }
                    else if (i.ThisQuality > ItemQuality.Magic3)
                    {
                        // Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[2]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Rare.Vendored++;
                    }
                    else
                    {
                        //  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[1]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Magical.Vendored++;
                    }
                    break;
            }
        }

        public void DroppedItemLog(CacheItem i)
        {
            CacheBalance thisBalanceData = i.BalanceData;
            GilesItemType thisGilesItemType = Funky.DetermineItemType(i.InternalName, thisBalanceData.thisItemType, thisBalanceData.thisFollowerType);
            if (thisGilesItemType == GilesItemType.InfernalKey)
            {
                ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Keys.Dropped++;
                //	 Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[(int)LootIndex.Key]++;
                return;
            }

            switch (thisBalanceData.thisItemType)
            {
                case ItemType.CraftingPage:
                case ItemType.CraftingPlan:
                case ItemType.CraftingReagent:
                    ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Crafting.Dropped++;
                    //  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[(int)LootIndex.Crafting]++;
                    break;
                case ItemType.Gem:
                    ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Gems.Dropped++;
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
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Legendary.Dropped++;
                    }
                    else if (i.Itemquality.Value > ItemQuality.Magic3)
                    {
                        // Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[2]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Rare.Dropped++;
                    }
                    else
                    {
                        //  Bot.BotStatistics.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[1]++;
                        ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.Magical.Dropped++;
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return String.Format("Magical:    \t {0} \r\n" +
                                 "Rare:      \t {1} \r\n" +
                                 "Legendary: \t {2} \r\n" +
                                 "Gems:      \t {3} \r\n" +
                                 "Crafting: \t {4} \r\n" +
                                 "Keys:      \t {5} \r\n",
                                 this.Magical.ToString(), this.Rare.ToString(), this.Legendary.ToString(), this.Gems.ToString(), this.Crafting.ToString(), this.Keys.ToString());
        }
    }
}
