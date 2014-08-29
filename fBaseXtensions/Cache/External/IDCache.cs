using System.Collections.Generic;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.External.Objects;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items.Enums;

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
		public Dictionary<int, PetTypes> UnitPetEntries = new Dictionary<int, PetTypes>();
		public Dictionary<int, CacheAvoidanceEntry> AvoidanceEntries = new Dictionary<int, CacheAvoidanceEntry>(); 


		public IDCache()
		{
			Logger.DBLog.Info("[fBaseXtensions] Loading External Cache..");

			var unitdata = UnitDataCollection.DeserializeFromXML();
			//var unitdata = new UnitDataCollection(); UnitDataCollection.SerializeToXML(unitdata);

			UnitEntries.Clear();
			foreach (var entry in unitdata.UnitEntries)
			{
				UnitEntries.Add(entry.SnoId, new CacheUnitEntry(entry.SnoId, (UnitFlags)entry.ObjectType, entry.InternalName));
			}
			UnitPetEntries.Clear();
			foreach (var entry in unitdata.UnitPetEntries)
			{
				UnitPetEntries.Add(entry.SnoId, (PetTypes)entry.ObjectType);
			}


			var Items = ItemDataCollection.DeserializeFromXML();
			//var Items = new ItemDataCollection(); ItemDataCollection.SerializeToXML(Items);

			ItemDroppedEntries.Clear();
			foreach (var entry in Items.DroppedItemCache)
			{
				ItemDroppedEntries.Add(entry.SnoId, new CacheDroppedItemEntry(entry.SnoId, (PluginDroppedItemTypes)entry.ObjectType, entry.InternalName));
			}
			ItemDataEntries.Clear();
			foreach (var entry in Items.ItemDataCache)
			{
				ItemDataEntries.Add(entry.SnoId, entry);
			}
			ItemGemEntries.Clear();
			foreach (var entry in Items.GemCache)
			{
				ItemGemEntries.Add(entry.SnoId, new CacheItemGemEntry(entry));
			}
			ItemDroppedInternalNames.Clear();
			foreach (var entry in Items.DroppedItemInternalNames)
			{
				ItemDroppedInternalNames.Add(entry);
			}


			var Gizmos = GizmoDataCollection.DeserializeFromXML();
			//var Gizmos = new GizmoDataCollection(); GizmoDataCollection.SerializeToXML(Gizmos);

			GizmoEntries.Clear();
			foreach (var entry in Gizmos.GizmoCache)
			{
				GizmoEntries.Add(entry.SnoId, new CacheGizmoEntry(entry));
			}


			var Avoidance = AvoidanceDataCollection.DeserializeFromXML();
			//var Avoidance = new AvoidanceDataCollection(); AvoidanceDataCollection.SerializeToXML(Avoidance);

			AvoidanceEntries.Clear();
			foreach (var entry in Avoidance.AvoidanceCache)
			{
				AvoidanceEntries.Add(entry.SnoId, new CacheAvoidanceEntry(entry.SnoId, (AvoidanceType)entry.ObjectType, entry.InternalName));
			}
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
