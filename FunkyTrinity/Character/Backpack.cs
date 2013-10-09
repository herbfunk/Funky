using System;
using System.Collections.Generic;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot.Settings;
using Zeta.Internals;
using Zeta.Internals.Actors;

namespace FunkyBot.Cache
{
	internal class Backpack
	{
		public Backpack()
		{
			townRunCache=new TownRunCache();
			BPItems=new List<CacheBPItem>();
			CacheItemList=new Dictionary<int, CacheACDItem>();
		}
		public List<CacheBPItem> BPItems { get; set; }
		public Dictionary<int,CacheACDItem> CacheItemList { get; set; }

		public ACDItem BestPotionToUse { get; set; }

		public int CurrentPotionACDGUID=-1;

		public TownRunCache townRunCache { get; set; }


		//Sets List to current backpack contents
		public void Update()
		{
			 List<int> SeenACDGUIDs=new List<int>();
			 using (ZetaDia.Memory.AcquireFrame())
			 {
				  foreach (var thisitem in ZetaDia.Me.Inventory.Backpack)
				  {
						int ACDGUID=thisitem.ACDGuid;
						SeenACDGUIDs.Add(ACDGUID);
						if (CacheItemList.ContainsKey(ACDGUID)) continue;

						CacheACDItem thiscacheditem=new CacheACDItem(thisitem);
						CacheItemList.Add(thiscacheditem.ACDGUID, thiscacheditem);
				  }
			 }

			 List<int> UnseenACDGUIDs=CacheItemList.Keys.Where(k => !SeenACDGUIDs.Contains(k)).ToList();
			 foreach (var unseenAcdguiD in UnseenACDGUIDs)
			 {
				  CacheItemList.Remove(unseenAcdguiD);
			 }


			//We refresh our BPItem Cache whenever we are checking for looted items!
			if (Bot.Targeting.ShouldCheckItemLooted)
			{
				 //Get a list of current BP Cached ACDItems
				 List<int> BPItemsACDItemList=(from backpackItems in BPItems
														 select backpackItems.ACDGUID).ToList();

				 //Now get items that are not currently in the BPItems List.
				 foreach (var item in CacheItemList.Values.Where(I => !BPItemsACDItemList.Contains(I.ACDGUID)))
				 {
					  BPItems.Add(new CacheBPItem(item.ACDGUID, item.ACDItem));
				 }
			}
		}

		//Used to check if backpack is visible
		public bool InventoryBackpackVisible()
		{
			bool InvVisible=false;
			try
			{
				InvVisible=UIElements.InventoryWindow.IsVisible;
			} catch
			{
			}

			return InvVisible;
		}

		//Used to toggle current backpack
		public void InventoryBackPackToggle(bool show)
		{
			bool InvVisible=InventoryBackpackVisible();

			if (InvVisible&&!show)
				UIElements.BackgroundScreenPCButtonInventory.Click();
			else if (!InvVisible&&show)
				UIElements.BackgroundScreenPCButtonInventory.Click();
		}

		public List<ACDItem> ReturnCurrentPotions()
		{
			//Always update!
			Update();
			BestPotionToUse=null;
			using (ZetaDia.Memory.AcquireFrame())
			{
				 var Potions=ZetaDia.Me.Inventory.Backpack.Where<ACDItem>(i => i.IsPotion);
				 if (Potions.Count()<=0) return null;
				 Potions=Potions.OrderByDescending(i => i.HitpointsGranted).ThenByDescending(i => i.ItemStackQuantity);
				 //Set Best Potion to use..
				 CurrentPotionACDGUID=Potions.FirstOrDefault().ACDGuid;
				 int balanceID=Potions.FirstOrDefault().GameBalanceId;
				 //Find best potion to use based upon stack
				 BestPotionToUse=Potions.Where<ACDItem>(i => i.GameBalanceId==balanceID).OrderBy(i => i.ItemStackQuantity).FirstOrDefault();
				 return Potions.ToList();
			}
		}

		public Queue<ACDItem> ReturnUnidenifiedItems()
		{
			Queue<ACDItem> returnQueue=new Queue<ACDItem>();

			Update();


				var filteredItems=ZetaDia.Me.Inventory.Backpack.Where<ACDItem>(i =>
					i.IsValid&&!i.IsMiscItem);

				if (filteredItems.Count()>0)
				{
					foreach (ACDItem item in filteredItems)
					{
						try
						{
							if (item.IsUnidentified)
								returnQueue.Enqueue(item);
						} catch
						{
							Logging.WriteDiagnostic("[Funky] Safetly Handled Exception: occured checking of item unidentified flag");
						}
					}
				}
			

			return returnQueue;
		}

		private Queue<ACDItem> ReturnUnidenifiedItemsSorted(bool backwards=false)
		{
			Queue<ACDItem> returnQueue=new Queue<ACDItem>();
			foreach (var item in GetUnidenifiedItemsSorted(backwards))
			{
				returnQueue.Enqueue(item);
			}
			return returnQueue;
		}

		//Combines inventory rows into 3 groupings
		private int InventoryRowCombine(int i)
		{
			if ((i&1)==0)
				return i;
			else
				return i-1;
		}
		private List<ACDItem> GetUnidenifiedItemsSorted(bool Backwards=false)
		{
			List<ACDItem> returnList=new List<ACDItem>();

			Update();

				var filteredItems=ZetaDia.Me.Inventory.Backpack.Where<ACDItem>(i =>
					i.IsValid&&!i.IsMiscItem&&i.IsUnidentified);
				if (Backwards)
					return filteredItems.OrderByDescending(o => InventoryRowCombine(o.InventoryRow)).ThenByDescending(o => o.InventoryColumn).ToList();
				else
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
			bool backwards=((int)MathEx.Random(0, 1)==0);
			Queue<ACDItem> returnQueue=new Queue<ACDItem>();
			List<ACDItem> SortedItems=GetUnidenifiedItemsSorted(backwards);

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
					return ReturnUnidenifiedItemsSorted(false);
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
				float repairVar=CharacterSettings.Instance.RepairWhenDurabilityBelow;
				bool ShouldRepair=false;
				using (ZetaDia.Memory.AcquireFrame())
				{
					 bool intown=ZetaDia.Me.IsInTown;
					 List<float> repairPct=ZetaDia.Me.Inventory.Equipped.Select(o => o.DurabilityPercent).ToList();

					 //Already in town? Have gear with 50% or less durability?
					 ShouldRepair=(repairPct.Any(o => o<=repairVar)||intown&&repairPct.Any(o => o<=50));
				}

				return ShouldRepair;
			} catch
			{
				return false;
			}

		}

		public bool ContainsItem(int ACDGUID)
		{
			//Update Item List
			Update();
			bool found=(from backpackItems in BPItems
				where backpackItems.ACDGUID==ACDGUID
				select backpackItems).Any();
			return found;
		}

		 public List<CacheACDItem> ReturnCurrentEquippedItems()
		 {
			  List<CacheACDItem> returnItems=new List<CacheACDItem>();
			  try
			  {
				  using (ZetaDia.Memory.AcquireFrame())
				  {
					  foreach (ACDItem item in ZetaDia.Me.Inventory.Equipped)
					  {
						  CacheACDItem thiscacheditem=new CacheACDItem(item);
						  returnItems.Add(thiscacheditem);
					  }
				  }

			  } catch (Exception)
			  {

			  }
			  return returnItems;
		 }


		//Used to hold Town Run Data
		public class TownRunCache
		{
			private int InventoryRowCombine(int i)
			{
				if ((i&1)==0)
					return i;
				else
					return i-1;
			}

			public TownRunCache()
			{
			}

			// These three lists are used to cache item data from the backpack when handling sales, salvaging and stashing
			// It completely minimized D3 <-> DB memory access, to reduce any random bugs/crashes etc.
			public HashSet<CacheACDItem> hashGilesCachedKeepItems=new HashSet<CacheACDItem>();
			public HashSet<CacheACDItem> hashGilesCachedSalvageItems=new HashSet<CacheACDItem>();
			public HashSet<CacheACDItem> hashGilesCachedSellItems=new HashSet<CacheACDItem>();
			public HashSet<CacheACDItem> hashGilesCachedUnidStashItems=new HashSet<CacheACDItem>();

			public void sortSellList()
			{
				List<CacheACDItem> sortedList=hashGilesCachedSellItems.OrderBy(o => InventoryRowCombine(o.invRow)).ThenBy(o => o.invCol).ToList();
				HashSet<CacheACDItem> newSortedHashSet=new HashSet<CacheACDItem>();
				foreach (CacheACDItem item in sortedList)
				{
					newSortedHashSet.Add(item);
				}

				hashGilesCachedSellItems=newSortedHashSet;

			}

			public void sortSalvagelist()
			{
				List<CacheACDItem> sortedList=hashGilesCachedSalvageItems.OrderBy(o => InventoryRowCombine(o.invRow)).ThenBy(o => o.invCol).ToList();
				HashSet<CacheACDItem> newSortedHashSet=new HashSet<CacheACDItem>();
				foreach (CacheACDItem item in sortedList)
				{
					newSortedHashSet.Add(item);
				}

				hashGilesCachedSalvageItems=newSortedHashSet;

			}
		}

	}
}