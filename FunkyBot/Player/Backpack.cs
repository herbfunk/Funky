using fItemPlugin.Items;
using FunkyBot.Cache.Objects;
using System.Collections.Generic;
using System.Linq;
using Zeta.Game;

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

	}
}