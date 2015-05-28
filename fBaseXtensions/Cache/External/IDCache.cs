﻿using System;
using System.Collections.Generic;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.External.Objects;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.External
{
	public class IDCache
	{
		public Dictionary<int, CacheUnitEntry> UnitEntries = new Dictionary<int, CacheUnitEntry>();
		public Dictionary<int, CacheGizmoEntry> GizmoEntries = new Dictionary<int, CacheGizmoEntry>();
		public Dictionary<int, ItemDataEntry> ItemDataEntries = new Dictionary<int, ItemDataEntry>();
		public List<ItemStringEntry> ItemDroppedInternalNames = new List<ItemStringEntry>();
		public Dictionary<int, CacheDroppedItemEntry> ItemDroppedEntries = new Dictionary<int, CacheDroppedItemEntry>();
		public Dictionary<int, CacheItemGemEntry> ItemGemEntries = new Dictionary<int, CacheItemGemEntry>();
		public Dictionary<int, CacheUnitPetEntry> UnitPetEntries = new Dictionary<int, CacheUnitPetEntry>();
		public Dictionary<int, CacheAvoidanceEntry> AvoidanceEntries = new Dictionary<int, CacheAvoidanceEntry>();

		public BountyDataCollection BountyEntries
		{
			get
			{
				if (_bountycache == null)
					_bountycache = BountyDataCollection.DeserializeFromXML();

				return _bountycache;
			}
		}
		private BountyDataCollection _bountycache;

		public IDCache()
		{
			Logger.DBLog.Info("[fBaseXtensions] Loading The External Cache...");

			var unitdata = UnitDataCollection.DeserializeFromXML();
			//var unitdata = new UnitDataCollection(); UnitDataCollection.SerializeToXML(unitdata);

			UnitEntries.Clear();
			foreach (var entry in unitdata.UnitEntries)
			{
				UnitEntries.Add(entry.SnoId, new CacheUnitEntry(entry));
			}
            Logger.DBLog.DebugFormat("[fBaseXtensions] Loaded {0} Unit Entries", UnitEntries.Count);

			UnitPetEntries.Clear();
			foreach (var entry in unitdata.UnitPetEntries)
			{
				UnitPetEntries.Add(entry.SnoId, new CacheUnitPetEntry(entry));
			}
            Logger.DBLog.DebugFormat("[fBaseXtensions] Loaded {0} Pet Entries", UnitPetEntries.Count);

			var Items = ItemDataCollection.DeserializeFromXML();
			//var Items = new ItemDataCollection(); ItemDataCollection.SerializeToXML(Items);

			ItemDroppedEntries.Clear();
			foreach (var entry in Items.DroppedItemCache)
			{
				ItemDroppedEntries.Add(entry.SnoId, new CacheDroppedItemEntry(entry));
			}
            Logger.DBLog.DebugFormat("[fBaseXtensions] Loaded {0} Dropped Item Entries", ItemDroppedEntries.Count);

			ItemDataEntries.Clear();
			foreach (var entry in Items.ItemDataCache)
			{
				ItemDataEntries.Add(entry.SnoId, entry);
			}
            Logger.DBLog.DebugFormat("[fBaseXtensions] Loaded {0} Item Entries", ItemDataEntries.Count);

			ItemGemEntries.Clear();
			foreach (var entry in Items.GemCache)
			{
				ItemGemEntries.Add(entry.SnoId, new CacheItemGemEntry(entry));
			}
            Logger.DBLog.DebugFormat("[fBaseXtensions] Loaded {0} Gem Entries", ItemGemEntries.Count);

			ItemDroppedInternalNames.Clear();
			foreach (var entry in Items.DroppedItemInternalNames)
			{
				ItemDroppedInternalNames.Add(entry);
			}
            //Logger.DBLog.InfoFormat("[fBaseXtensions] Loaded {0} Item Name Entries", ItemDroppedInternalNames.Count);


			var Gizmos = GizmoDataCollection.DeserializeFromXML();
			//var Gizmos = new GizmoDataCollection(); GizmoDataCollection.SerializeToXML(Gizmos);

			GizmoEntries.Clear();
			foreach (var entry in Gizmos.GizmoCache)
			{
                GizmoEntries.Add(entry.SnoId, new CacheGizmoEntry(entry));
			}
            Logger.DBLog.DebugFormat("[fBaseXtensions] Loaded {0} Gizmo Entries", GizmoEntries.Count);


			var Avoidance = AvoidanceDataCollection.DeserializeFromXML();
			//var Avoidance = new AvoidanceDataCollection(); AvoidanceDataCollection.SerializeToXML(Avoidance);

			AvoidanceEntries.Clear();
			foreach (var entry in Avoidance.AvoidanceCache)
			{
                
				AvoidanceEntries.Add(entry.SnoId, new CacheAvoidanceEntry(entry));
			}
            Logger.DBLog.DebugFormat("[fBaseXtensions] Loaded {0} Avoidance Entries", AvoidanceEntries.Count);

            Logger.DBLog.InfoFormat("[fBaseXtensions] Finished Loading The External Cache!");

		}

		internal bool TryGetCacheValue(int snoid, out CacheEntry value)
		{
			value = null;

			if (UnitEntries.ContainsKey(snoid))
			{
				value = UnitEntries[snoid];
				return true;
			}
			if (GizmoEntries.ContainsKey(snoid))
			{
				value = GizmoEntries[snoid];
				return true;
			}
			if (ItemDroppedEntries.ContainsKey(snoid))
			{
				value = ItemDroppedEntries[snoid];
				return true;
			}
			if (AvoidanceEntries.ContainsKey(snoid))
			{
				value = AvoidanceEntries[snoid];
				return true;
			}
			if (UnitPetEntries.ContainsKey(snoid))
			{
				value = UnitPetEntries[snoid];
				return true;
			}

			return false;
		}

		internal bool CacheValueContainsKey(int snoid)
		{
			if (UnitEntries.ContainsKey(snoid))
			{
				return true;
			}
			if (GizmoEntries.ContainsKey(snoid))
			{
				return true;
			}
			if (ItemDroppedEntries.ContainsKey(snoid))
			{
				return true;
			}
			if (AvoidanceEntries.ContainsKey(snoid))
			{
				return true;
			}

			return false;
		}

		//public void ClearCache()
		//{
		//	Avoidance.ClearCollections();
		//	//Units.ClearCollections();
		//	Items.ClearCollections();
		//	Gizmos.ClearCollections();
		//	UnitData.ClearCollections();
		//}
	}
}
