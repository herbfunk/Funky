using System.Collections.Generic;
using System.Linq;
using fItemPlugin.Items;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace fItemPlugin.Player
{
	public class Backpack
	{

		public static Dictionary<int, CacheACDItem> CacheItemList = new Dictionary<int, CacheACDItem>();
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
		public static void UpdateItemList()
		{
			List<int> SeenACDGuid = new List<int>();

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
						}

						continue;
					}

					var thiscacheditem = new CacheACDItem(thisitem);
					CacheItemList.Add(thiscacheditem.ACDGUID, thiscacheditem);
				}
			}

			//Trim away items missing..
			var UnseenACDGuids = CacheItemList.Keys.Where(k => !SeenACDGuid.Contains(k)).ToList();
			foreach (var unseenAcdguiD in UnseenACDGuids)
			{
				CacheItemList.Remove(unseenAcdguiD);
			}
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
						FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Safetly Handled Exception: occured checking of item unidentified flag");
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
				return filteredItems.OrderByDescending(o => InventoryRowCombine(o.InventoryRow)).ThenByDescending(o => o.InventoryColumn).ToList();
			return filteredItems.OrderBy(o => InventoryRowCombine(o.InventoryRow)).ThenBy(o => o.InventoryColumn).ToList();
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

			var Potions = CacheItemList.Values.Where(i => i.ItemType == PluginItemType.HealthPotion && i.IsRegularPotion);
			if (!Potions.Any())
			{
				//FunkyTownRunPlugin.DBLog.InfoFormat("No Potions Found In Backpack. Total Items Enumerated {0}", CacheItemList.Count);
				return retList;
			}

			retList = Potions.OrderByDescending(i => i.ThisItemStackQuantity).ToList();

			if (FunkyTownRunPlugin.PluginSettings.PotionsCount > 0)
				CurrentPotionACDGUID = retList[0].ACDGUID;

			if (FunkyTownRunPlugin.PluginSettings.BuyPotionsDuringTownRun)
				CurrentPotionCount = retList.Count;

			return retList;
		}

		public static ACDItem ReturnBestPotionToUse()
		{
			//Always update!
			UpdateItemList();
			var Potions = CacheItemList.Values.Where(i => i.ItemType == PluginItemType.HealthPotion).ToList();
			if (Potions.Count > 0)
			{
				//Check for any Bottomless Pots First!
				var BottomlessPotions = Potions.Where(i => i.PotionType != PotionTypes.None && i.PotionType != PotionTypes.Regular).ToList();
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



		public static bool ShouldRepairItems()
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

	}
}
