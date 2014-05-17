using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.DBHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player
{
	internal class Backpack
	{
		public Backpack()
		{
			CacheItemList = new Dictionary<int, CacheACDItem>();
			NewItemList = new List<CacheACDItem>();
		}
		public Dictionary<int, CacheACDItem> CacheItemList { get; set; }
		internal List<CacheACDItem> NewItemList { get; set; } 

		public ACDItem BestPotionToUse { get; set; }

		public int CurrentPotionACDGUID = -1;

		//Sets List to current backpack contents
		public void Update()
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
						//Stackable item that increased
						if (CacheItemList[ACDGuid].IsStackableItem
							&& CacheItemList[ACDGuid].ThisItemStackQuantity != thisitem.ItemStackQuantity)
						{
							//Update it..
							CacheItemList[ACDGuid].ThisItemStackQuantity = thisitem.ItemStackQuantity;

							//Add it to new item list..
							NewItemList.Add(new CacheACDItem(thisitem));
						}

						continue;
					}

					var thiscacheditem = new CacheACDItem(thisitem);
					CacheItemList.Add(thiscacheditem.ACDGUID, thiscacheditem);

					//Add it to new item list..
					NewItemList.Add(thiscacheditem);
				}
			}

			//Trim away items missing..
			var UnseenACDGuids = CacheItemList.Keys.Where(k => !SeenACDGuid.Contains(k)).ToList();
			foreach (var unseenAcdguiD in UnseenACDGuids)
			{
				CacheItemList.Remove(unseenAcdguiD);
			}
		}

		public bool ContainsItem(CacheItem item)
		{
			//Update Item List!
			Update();

			//Logger.DBLog.Info("New Items Count: " + NewItemList.Count);

			//Use Gamebalance ID to narrow down the items.
			foreach (var i in NewItemList.Where(i => i.ThisBalanceID == item.BalanceID))
			{
				//and matching Item Quality..
				if (i.ThisQuality == item.Itemquality)
					return true;
			}

			return false;
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


			var Potions = CacheItemList.Values.Where(i => i.ThisDBItemType== ItemType.Potion);
			if (!Potions.Any())
			{
				Logger.DBLog.InfoFormat("No Potions Found In Backpack. Total Items Enumerated {0}", CacheItemList.Count);
				return null;
			}

			Potions = Potions.OrderByDescending(i => i.ThisLevel).ThenByDescending(i => i.ThisItemStackQuantity);
			//Set Best Potion to use..
			CurrentPotionACDGUID = Potions.FirstOrDefault().ACDGUID;
			//Find best potion to use based upon stack
			BestPotionToUse = Potions.OrderBy(i => i.ThisItemStackQuantity).FirstOrDefault().ACDItem;
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
						Logger.DBLog.DebugFormat("[Funky] Safetly Handled Exception: occured checking of item unidentified flag");
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

		public int GetBloodShardCount()
		{
			int _count = -1;
			try
			{
				_count=ZetaDia.CPlayer.BloodshardCount;
			}
			catch
			{

			}

			return _count;
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
		public List<CacheACDItem> ReturnCurrentBackpackItems()
		{
			List<CacheACDItem> returnItems = new List<CacheACDItem>();
			try
			{
				using (ZetaDia.Memory.AcquireFrame())
				{
					foreach (ACDItem item in ZetaDia.Me.Inventory.Backpack)
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

		public int ReturnFreeBackpackSlots()
		{
			Update();

			int OccupiedSlots = 0;
			foreach (var item in CacheItemList.Values)
			{
				if (item.IsTwoSlot)
					OccupiedSlots += 2;
				else
					OccupiedSlots++;
			}

			return 60-OccupiedSlots;
		}

	}
}