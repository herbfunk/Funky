using System;
using System.Collections.Generic;
using System.Linq;
using fItemPlugin.Items;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace fItemPlugin.Player
{

	/// <summary>
	/// Equipped Items Manager
	/// </summary>
	public static class Equipment
	{
		public static double TotalSkillResourceCostReductionPercent = 0;
		public static double TotalSkillCooldownReductionPercent = 0;

		public static bool GlobesRestoreResource { get; set; }
		public static bool ImmuneToDescratorMoltenPlaguedAvoidances { get; set; }
		public static bool ImmuneToArcane { get; set; }
		public static bool ImmuneToPoison { get; set; }
		public static bool ImmuneToFrozen { get; set; }
		public static bool RingOfGrandeur { get; set; }
		public static bool NoMonsterCollision { get; set; }

		public static List<CacheACDItem> EquippedItems = new List<CacheACDItem>();
		public static List<LegendaryItemTypes> LegendaryEquippedItems = new List<LegendaryItemTypes>();

		private static DateTime _lastCheckedItemIDs = DateTime.MinValue;
		private const int _itemIDRecheckSeconds = 10;
		private static List<int> _equippedItemIDs = new List<int>();
		private static bool ShouldRecheckItemIDs
		{
			get
			{
				return DateTime.Now.Subtract(_lastCheckedItemIDs).TotalSeconds >= _itemIDRecheckSeconds;
			}
		}
		internal static void CheckEquippment()
		{
			if (OnEquippedItemsChanged == null) return;

			if (ShouldRecheckItemIDs)
			{
				_lastCheckedItemIDs = DateTime.Now;
				List<int> compareList = ReturnEquippedItemIDs();

				//Check for any new IDs..
				if (compareList.Except(_equippedItemIDs).Any())
				{
					_equippedItemIDs = new List<int>(compareList);
					RefreshEquippedItemsList();
					OnEquippedItemsChanged();
				}
			}
		}
		public delegate void EquippedItemsChanged();
		public static event EquippedItemsChanged OnEquippedItemsChanged;


		private static List<int> ReturnEquippedItemIDs()
		{
			var returnItems = new List<int>();

			try
			{
				using (ZetaDia.Memory.AcquireFrame())
				{
					foreach (ACDItem item in ZetaDia.Me.Inventory.Equipped)
					{
						try
						{
							var thisItemID = item.DynamicId;
							returnItems.Add(thisItemID);
						}
						catch{}
					}
				}

			}
			catch
			{

			}

			return returnItems;
		}

		public static void RefreshEquippedItemsList()
		{
			if (Character.GameIsInvalid())
				return;

			FunkyTownRunPlugin.DBLog.DebugFormat("Refreshing Item List..");

			EquippedItems.Clear();
			EquippedItems = ReturnCurrentEquippedItems();
			LegendaryEquippedItems = EquippedItems.Where(i => i.LegendaryItemType != LegendaryItemTypes.None).Select(i => i.LegendaryItemType).ToList();


			var ids = EquippedItems.Select(i => i.ThisDynamicID).ToList();
			_equippedItemIDs = new List<int>(ids);

			//Countess Julias Cameo
			ImmuneToArcane = LegendaryEquippedItems.Any(i => i == LegendaryItemTypes.CountessJuliasCameo);
			ImmuneToPoison = LegendaryEquippedItems.Any(i => i == LegendaryItemTypes.MarasKaleidoscope);
			ImmuneToFrozen = LegendaryEquippedItems.Any(i => i == LegendaryItemTypes.IceClimbers);

			//Repear Wraps
			if (LegendaryEquippedItems.Any(i => i== LegendaryItemTypes.ReapersWraps))
			{
				GlobesRestoreResource = true;
				FunkyTownRunPlugin.DBLog.DebugFormat("Reaper's Wraps is equipped.. globes will be targeted on low resource!");
			}
			else
				GlobesRestoreResource = false;


			//Illusionary Boots
			if (LegendaryEquippedItems.Any(i => i == LegendaryItemTypes.IllusionaryBoots))
			{
				NoMonsterCollision = true;
				FunkyTownRunPlugin.DBLog.DebugFormat("Illusionary Boots Found -- No monster collision!");
			}
			else
				NoMonsterCollision = false;


			//Ring of Royal Grandeur
			RingOfGrandeur = LegendaryEquippedItems.Any(i => i == LegendaryItemTypes.RoyalGrandeur);


			//Blackthorns Set
			int BlackThornSetCount = LegendaryEquippedItems.Count(i => i == LegendaryItemTypes.Blackthornes);
			if (BlackThornSetCount == 4 || BlackThornSetCount == 3 && RingOfGrandeur)
			{
				ImmuneToDescratorMoltenPlaguedAvoidances = true;
				FunkyTownRunPlugin.DBLog.DebugFormat("Blackthorne's Avoidance Immune is equipped");
			}
			else
				ImmuneToDescratorMoltenPlaguedAvoidances = false;
			
			TotalSkillResourceCostReductionPercent = 0;
			TotalSkillCooldownReductionPercent = 0;
			foreach (var item in EquippedItems.Where(i => i.ItemStatProperties!=null))
			{
				TotalSkillResourceCostReductionPercent += Math.Round(item.ItemStatProperties.ResourceCostReductionPercent, MidpointRounding.AwayFromZero);
				TotalSkillCooldownReductionPercent += Math.Round(item.ItemStatProperties.PowerCooldownReductionPercent, MidpointRounding.AwayFromZero);
			}

			FunkyTownRunPlugin.DBLog.InfoFormat("Total resource cost reduction {0}", TotalSkillResourceCostReductionPercent);
			FunkyTownRunPlugin.DBLog.InfoFormat("Total skill cool down reduction {0}", TotalSkillCooldownReductionPercent);

			
		}

		private static List<CacheACDItem> ReturnCurrentEquippedItems()
		{
			var returnItems = new List<CacheACDItem>();
			try
			{
				using (ZetaDia.Memory.AcquireFrame())
				{
					foreach (ACDItem item in ZetaDia.Me.Inventory.Equipped)
					{
						var thiscacheditem = new CacheACDItem(item);
						returnItems.Add(thiscacheditem);
					}
				}

			}
			catch (Exception)
			{

			}
			return returnItems;
		}

	}
}
