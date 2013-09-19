using System.Collections.Generic;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot.Settings;
using Zeta.Internals;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Cache
{
	internal class Backpack
	{
		public Backpack()
		{
			oocItemCache=new OOCItemCache();
			townRunCache=new TownRunCache();
			BPItems=new List<CacheBPItem>();
			CacheItemList=new Dictionary<int, CacheACDItem>();
		}
		public List<CacheBPItem> BPItems { get; set; }
		public Dictionary<int,CacheACDItem> CacheItemList { get; set; }

		public ACDItem BestPotionToUse { get; set; }

		public int CurrentPotionACDGUID=-1;

		public TownRunCache townRunCache { get; set; }

		public OOCItemCache oocItemCache { get; set; }


		//Sets List to current backpack contents
		public void Update()
		{

			int CurrentItemCount=ZetaDia.Me.Inventory.Backpack.Count();
			if (CurrentItemCount!=CacheItemList.Count||ZetaDia.Me.Inventory.Backpack.Any(i => !CacheItemList.ContainsKey(i.ACDGuid)))
			{
				CacheItemList=new Dictionary<int, CacheACDItem>();
				foreach (var thisitem in ZetaDia.Me.Inventory.Backpack)
				{
					using (ZetaDia.Memory.AcquireFrame())
					{
						//CacheACDItem thiscacheditem=new CacheACDItem(thisitem.InternalName, thisitem.Name, thisitem.Level, thisitem.ItemQualityLevel, thisitem.Gold, thisitem.GameBalanceId,
						//            thisitem.DynamicId, thisitem.Stats.WeaponDamagePerSecond, thisitem.IsOneHand, thisitem.DyeType, thisitem.ItemType, thisitem.FollowerSpecialType,
						//            thisitem.IsUnidentified, thisitem.ItemStackQuantity, thisitem.Stats, thisitem, thisitem.InventoryRow, thisitem.InventoryColumn, thisitem.IsPotion, thisitem.ACDGuid);
						CacheACDItem thiscacheditem=new CacheACDItem(thisitem);
						CacheItemList.Add(thiscacheditem.ACDGUID,thiscacheditem);
					}
				}
			}



			//We refresh our BPItem Cache whenever we are checking for looted items!
			if (Bot.Combat.ShouldCheckItemLooted)
			{
				//Get a list of current BP Cached ACDItems
				List<ACDItem> BPItemsACDItemList=(from backpackItems in BPItems
					select backpackItems.Ref_ACDItem).ToList();
				var NewItems=ZetaDia.Me.Inventory.Backpack.Where<ACDItem>(I => !BPItemsACDItemList.Contains(I));
				if (NewItems.Count()==0) return;

				//Now get items that are not currently in the BPItems List.
				using (ZetaDia.Memory.AcquireFrame())
				{
					foreach (var item in NewItems)
					{
						BPItems.Add(new CacheBPItem(item.ACDGuid, item));
					}
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

			using (ZetaDia.Memory.AcquireFrame())
			{
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
			using (ZetaDia.Memory.AcquireFrame())
			{
				var filteredItems=ZetaDia.Me.Inventory.Backpack.Where<ACDItem>(i =>
					i.IsValid&&!i.IsMiscItem&&i.IsUnidentified);
				if (Backwards)
					return filteredItems.OrderByDescending(o => InventoryRowCombine(o.InventoryRow)).ThenByDescending(o => o.InventoryColumn).ToList();
				else
					return filteredItems.OrderBy(o => InventoryRowCombine(o.InventoryRow)).ThenBy(o => o.InventoryColumn).ToList();
			}
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
				bool intown=ZetaDia.Me.IsInTown;
				List<float> repairPct=ZetaDia.Me.Inventory.Equipped.Select(o => o.DurabilityPercent).ToList();
				using (ZetaDia.Memory.AcquireFrame())
				{
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

		//Used to hold OOC ID behavior data
		public class OOCItemCache
		{
			public OOCItemCache()
			{
			}

			//Vars used in actual town runs so we don't have to recheck.
			public List<CacheItem> HerbfunkOOCKeepItems=new List<CacheItem>();
			public HashSet<int> HerbfunkOOCcheckedItemDynamicIDs=new HashSet<int>();

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