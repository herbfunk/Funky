using System;
using System.Collections.Generic;
using System.Linq;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero
{
    public class Backpack
    {


        private static List<CacheACDItem> NewItemList = new List<CacheACDItem>();

        //Used to toggle current backpack
        public static void InventoryBackPackToggle(bool show)
        {
            bool InvVisible = InventoryBackpackVisible();

            if (InvVisible && !show)
                UIElements.BackgroundScreenPCButtonInventory.Click();
            else if (!InvVisible && show)
                UIElements.BackgroundScreenPCButtonInventory.Click();
        }

        public static bool InventoryBackpackVisible()
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

        /*
         * During Update of backpack
         *      -Record occupied slot count
         *      -Record SNOs of stackable items
         *      -Record backpack occupied array
         *      
         */
        public static Dictionary<int, CacheACDItem> CacheItemList = new Dictionary<int, CacheACDItem>();
        private static int _occupiedSlots = 0;
        private static readonly List<int> _stackableItems = new List<int>();
        private static readonly bool[,] _backpackslotblocked = new bool[10, 6];

        internal static string DebugString
        {
            get
            {
                string stackableItemString = _stackableItems.Aggregate("",
                    (current, item) => current + "SNOID: " + item + "\r\n");

                string backpackSlots = "";
                for (int iRow = 0; iRow <= 5; iRow++)
                {
                    backpackSlots = backpackSlots + "Row: " + iRow;
                    for (int iColumn = 0; iColumn <= 9; iColumn++)
                    {
                        backpackSlots = backpackSlots + "    [" + _backpackslotblocked[iColumn, iRow] + "]";
                    }
                    backpackSlots = backpackSlots + "\r\n";
                }

                return String.Format("Backpack Debug String\r\n" +
                                     "Occupied Slots {0}\r\n" +
                                     "Stackable Items {1}\r\n" +
                                     "Backpack Slots\r\n" +
                                     "{3}" +
                                     "\r\n" +
                                     "Cache Item List (Total {2})\r\n",
                    _occupiedSlots, stackableItemString, CacheItemList.Count, backpackSlots);
            }
        }
        public static void ClearBackpackItemCache()
        {
            CacheItemList.Clear();
            _occupiedSlots = 0;
            _stackableItems.Clear();
            
            for (int iRow = 0; iRow <= 5; iRow++)
                for (int iColumn = 0; iColumn <= 9; iColumn++)
                    _backpackslotblocked[iColumn, iRow] = false;

            if (FunkyGame.GameIsInvalid)
                return;

            UpdateItemList();
        }

        public static void UpdateItemList()
        {
            List<int> SeenACDGuid = new List<int>();

            //Clear New Item List
            NewItemList.Clear();

            using (ZetaDia.Memory.AcquireFrame())
            {
                foreach (var thisitem in ZetaDia.Me.Inventory.Backpack)
                {
                    var ACDGuid = thisitem.ACDGuid;
                    SeenACDGuid.Add(ACDGuid);

                    if (CacheItemList.ContainsKey(ACDGuid))
                    {
                        var cachedItem = CacheItemList[ACDGuid];

                        //Stackable item that increased
                        if (CacheItemList[ACDGuid].IsStackableItem)
                        {
                            try
                            {
                                if (cachedItem.ThisItemStackQuantity != thisitem.ItemStackQuantity)
                                {
                                    //Update it..
                                    cachedItem.ThisItemStackQuantity = thisitem.ItemStackQuantity;

                                    //Check if stack quanity is maxed to remove it from our list
                                    if (cachedItem.ThisItemStackQuantity >= cachedItem.MaxStackQuanity &&
                                        _stackableItems.Contains(cachedItem.SNO))
                                        _stackableItems.Remove(cachedItem.SNO);

                                    //Add it to new item list..
                                    NewItemList.Add(CacheItemList[ACDGuid]);
                                }
                            }
                            catch (Exception ex)
                            {
                                //Logger.DBLog.DebugFormat("Item {0} stack quanity threw exception {1}",
                                    //CacheItemList[ACDGuid].ThisInternalName, ex.Message);
                            }
                        }

                        continue;
                    }

                    var thiscacheditem = new CacheACDItem(thisitem);
                    CacheItemList.Add(thiscacheditem.ACDGUID, thiscacheditem);

                    //Add it to new item list..
                    NewItemList.Add(thiscacheditem);

                    //
                    _occupiedSlots += thiscacheditem.IsTwoSlot ? 2 : 1;
                    if (thiscacheditem.IsStackableItem &&
                        thiscacheditem.ThisItemStackQuantity < thiscacheditem.MaxStackQuanity &&
                        !_stackableItems.Contains(thiscacheditem.SNO))
                        _stackableItems.Add(thiscacheditem.SNO);

                    _backpackslotblocked[thiscacheditem.invCol, thiscacheditem.invRow] = true;
                    if (thiscacheditem.IsTwoSlot)
                        _backpackslotblocked[thiscacheditem.invCol, thiscacheditem.invRow + 1] = true;
                }
            }

            //Trim away items missing..
            var UnseenACDGuids = CacheItemList.Keys.Where(k => !SeenACDGuid.Contains(k)).ToList();
            foreach (var unseenAcdguiD in UnseenACDGuids)
            {
                var item = CacheItemList[unseenAcdguiD];

                if (item.IsStackableItem && _stackableItems.Contains(item.SNO))
                    _stackableItems.Remove(item.SNO);

                _occupiedSlots -= item.IsTwoSlot ? 2 : 1;
                _backpackslotblocked[item.invCol, item.invRow] = false;
                if (item.IsTwoSlot)
                    _backpackslotblocked[item.invCol, item.invRow + 1] = false;


                CacheItemList.Remove(unseenAcdguiD);
            }
        }

        public static bool ContainsItem(int balanceID, ItemQuality quality)
        {
            //Update Item List!
            UpdateItemList();

            //Logger.DBLog.Info("New Items Count: " + NewItemList.Count);

            //Use Gamebalance ID to narrow down the items.
            foreach (var i in NewItemList.Where(i => i.ThisBalanceID == balanceID))
            {
                //and matching Item Quality..
                if (i.ThisQuality == quality)
                    return true;
            }

            return false;
        }

        public static bool ContainsItem(CacheItem item)
        {
            UpdateItemList();

            if (item.BalanceData == null)
            {
                if (item.ItemDropType.HasValue)
                {
                    var pluginBase = ItemFunc.DetermineBaseItemType(item.ItemDropType.Value);
                    return NewItemList.Any(i => i.BaseItemType == pluginBase);
                }
            }
            else
            {
                return NewItemList.Any(i => i.ThisBalanceID == item.BalanceData.BalanceID);
            }

            return NewItemList.Any(i => i.SNO == item.SNOID);
        }

        public static bool ContainsItem(int SNOID)
        {
            //Update Item List!
            UpdateItemList();
            return NewItemList.Any(i => i.SNO == SNOID);
        }

        public static Queue<ACDItem> ReturnUnidenifiedItems()
        {
            Queue<ACDItem> returnQueue = new Queue<ACDItem>();

            UpdateItemList();


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
                        Logger.DBLog.DebugFormat(
                            "[Funky] Safetly Handled Exception: occured checking of item unidentified flag");
                    }
                }
            }


            return returnQueue;
        }

        public static Queue<ACDItem> ReturnUnidenifiedItemsSorted(bool backwards = false)
        {
            Queue<ACDItem> returnQueue = new Queue<ACDItem>();
            foreach (var item in GetUnidenifiedItemsSorted(backwards))
            {
                returnQueue.Enqueue(item);
            }
            return returnQueue;
        }

        //Combines inventory rows into 3 groupings
        private static int InventoryRowCombine(int i)
        {
            if ((i & 1) == 0)
                return i;
            return i - 1;
        }

        private static List<ACDItem> GetUnidenifiedItemsSorted(bool Backwards = false)
        {
            UpdateItemList();

            var filteredItems = ZetaDia.Me.Inventory.Backpack.Where(i =>
                i.IsValid && !i.IsMiscItem && i.IsUnidentified);
            if (Backwards)
                return
                    filteredItems.OrderByDescending(o => InventoryRowCombine(o.InventoryRow))
                        .ThenByDescending(o => o.InventoryColumn)
                        .ToList();
            return
                filteredItems.OrderBy(o => InventoryRowCombine(o.InventoryRow)).ThenBy(o => o.InventoryColumn).ToList();
        }

        public static int CurrentPotionACDGUID = -1;
        public static int CurrentPotionCount = 0;

        public static List<CacheACDItem> ReturnRegularPotions()
        {
            var retList = new List<CacheACDItem>();

            //Always update!
            UpdateItemList();
            CurrentPotionACDGUID = -1;
            CurrentPotionCount = 0;

            var Potions =
                CacheItemList.Values.Where(i => i.ItemType == PluginItemTypes.HealthPotion && i.IsRegularPotion);
            if (!Potions.Any())
            {
                return retList;
            }

            retList = Potions.OrderByDescending(i => i.ThisItemStackQuantity).ToList();
            return retList;
        }

        public static ACDItem ReturnBestPotionToUse()
        {
            //Always update!
            UpdateItemList();
            var Potions = CacheItemList.Values.Where(i => i.IsPotion).ToList();
            if (Potions.Count > 0)
            {
                //Check for any Bottomless Pots First!
                var BottomlessPotions =
                    Potions.Where(i => i.PotionType != PotionTypes.Regular).ToList();
                if (BottomlessPotions.Count > 0)
                {
                    //TODO:: Base selection on current events!
                    return BottomlessPotions.First().ACDItem;
                }

                //Return normal potion
                return Potions.OrderByDescending(i => i.ThisItemStackQuantity).First().ACDItem;
            }

            return null;
        }



        public static bool ShouldRepairItems(float minimumPercent)
        {
            try
            {
                float repairVar = minimumPercent;
                bool ShouldRepair = false;
                using (ZetaDia.Memory.AcquireFrame())
                {
                    bool intown = ZetaDia.IsInTown;
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

        public static int GetBloodShardCount()
        {
            int _count = -1;
            try
            {
                _count = ZetaDia.CPlayer.BloodshardCount;
            }
            catch
            {

            }

            return _count;
        }

        public static int GetNumberFreeBackpackSlots()
        {
            UpdateItemList();
            return 60 - _occupiedSlots;
        }

        public static bool CanPickupItem(bool TwoSlotItem)
        {
            int freebackpackslots = GetNumberFreeBackpackSlots();

            // If it's a 2-square item, find a double-slot free
            if (TwoSlotItem)
            {
                if (freebackpackslots<=1) return false;

                for (int iRow = 0; iRow <= 4; iRow++)
                {
                    for (int iColumn = 0; iColumn <= 9; iColumn++)
                    {
                        if (!_backpackslotblocked[iColumn, iRow] && !_backpackslotblocked[iColumn, iRow + 1])
                        {
                            return true;
                        }
                    }
                }
            } // 2 slot item?
                // Now deal with any leftover 1-slot items
            else
            {
                if (freebackpackslots==0) return false;

                // First we try and find somewhere "sensible"
                for (int iRow = 0; iRow <= 5; iRow++)
                {
                    for (int iColumn = 0; iColumn <= 9; iColumn++)
                    {
                        // Nothing in this slot
                        if (!_backpackslotblocked[iColumn, iRow])
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

    }
}
