using System;
using System.Collections.Generic;
using System.Linq;
using FunkyBot.Cache.Objects;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player
{
	internal class Equipment
	{
		public bool GlobesRestoreResource { get; set; }
		public bool ImmuneToDescratorMoltenPlaguedAvoidances { get; set; }
		public bool RingOfGrandeur { get; set; }

		public List<CacheACDItem> EquippedItems { get; set; }

		public Equipment()
		{
			EquippedItems = new List<CacheACDItem>();
			GlobesRestoreResource = false;
			ImmuneToDescratorMoltenPlaguedAvoidances = false;
			RingOfGrandeur = false;
		}

		public void RefreshEquippedItemsList()
		{
			EquippedItems.Clear();
			EquippedItems = ReturnCurrentEquippedItems();


			//Repear Wraps
			if (EquippedItems.Any(i => i.ThisRealName.Contains("Reaper's Wraps")))
			{
				GlobesRestoreResource = true;
				Logger.DBLog.DebugFormat("Reaper's Wraps is equipped.. globes will be targeted on low resource!");
			}
			else
				GlobesRestoreResource = false;


			//Ring of Royal Grandeur
			RingOfGrandeur = EquippedItems.Any(i => i.ThisRealName.Contains("Ring of Royal Grandeur"));


			//Blackthorns Set
			int BlackThornSetCount = EquippedItems.Count(i => i.ThisRealName.Contains("Blackthorne's"));
			if (BlackThornSetCount == 4 || BlackThornSetCount == 3 && RingOfGrandeur)
			{
				ImmuneToDescratorMoltenPlaguedAvoidances = true;
				Logger.DBLog.DebugFormat("Blackthorne's Avoidance Immune is equipped");
			}
			else
				ImmuneToDescratorMoltenPlaguedAvoidances = false;
			
		}

		public List<CacheACDItem> ReturnCurrentEquippedItems()
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
