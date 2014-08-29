using System;
using System.Collections.Generic;
using System.Linq;
using fBaseXtensions.Items;

namespace fItemPlugin.Townrun
{
	public class ItemCache
	{
		public ItemCache()
		{
			KeepItems = new HashSet<CacheACDItem>();
			SalvageItems = new HashSet<CacheACDItem>();
			SellItems = new HashSet<CacheACDItem>();
		}
		
		public HashSet<CacheACDItem> KeepItems { get; set; }
		public HashSet<CacheACDItem> SalvageItems { get; set; }
		public HashSet<CacheACDItem> SellItems { get; set; }

		private int InventoryRowCombine(int i)
		{
			if ((i & 1) == 0)
				return i;

			return i - 1;
		}
		public void sortSellList()
		{
			List<CacheACDItem> sortedList = SellItems.OrderBy(o => InventoryRowCombine(o.invRow)).ThenBy(o => o.invCol).ToList();
			var newSortedHashSet = new HashSet<CacheACDItem>();
			foreach (var item in sortedList)
			{
				newSortedHashSet.Add(item);
			}

			SellItems = newSortedHashSet;

		}
		public void sortSalvagelist()
		{
			List<CacheACDItem> sortedList = SalvageItems.OrderBy(o => InventoryRowCombine(o.invRow)).ThenBy(o => o.invCol).ToList();
			var newSortedHashSet = new HashSet<CacheACDItem>();
			foreach (var item in sortedList)
			{
				newSortedHashSet.Add(item);
			}

			SalvageItems = newSortedHashSet;

		}

		public string GetListString()
		{
			return String.Format("KeepItems: {0} SalvageItems: {1} SellItems: {2}", KeepItems.Count, SalvageItems.Count, SellItems.Count);
		}
	}
}
