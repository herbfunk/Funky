using System.Collections.Generic;
using fBaseXtensions.Cache.External.Objects;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External
{
	public class IDCache
	{
		//public AvoidanceDataCollection Avoidance { get; set; }
		public UnitDataCollection Units { get; set; }
		//public ItemDataCollection Items { get; set; }
		//public GizmoDataCollection Gizmos { get; set; }
		//public UnitData UnitData { get; set; }


		public Dictionary<int, UnitEntry> UnitEntries = new Dictionary<int, UnitEntry>();
		public Dictionary<int, GizmoEntry> GizmoEntries = new Dictionary<int, GizmoEntry>();
		public Dictionary<int, ItemDataEntry> ItemDataEntries = new Dictionary<int, ItemDataEntry>();
		public List<ItemStringEntry> ItemDroppedInternalNames = new List<ItemStringEntry>();
		public Dictionary<int, DroppedItemEntry> ItemDroppedEntries = new Dictionary<int, DroppedItemEntry>();
		public Dictionary<int, ItemGemEntry> ItemGemEntries = new Dictionary<int, ItemGemEntry>();
		public Dictionary<int, PetTypes> UnitPetEntries = new Dictionary<int, PetTypes>();
		public Dictionary<int, AvoidanceEntry> AvoidanceEntries = new Dictionary<int, AvoidanceEntry>(); 


		public IDCache()
		{
			Logger.DBLog.Info("[fBaseXtensions] Loading External Cache..");

			Units = UnitDataCollection.DeserializeFromXML();//new UnitDataCollection(); //
			

			var unitdata = UnitData.DeserializeFromXML();
			//var unitdata = new UnitData();
			//UnitData.SerializeToXML(unitdata);

			UnitEntries.Clear();
			foreach (var entry in unitdata.UnitEntries)
			{
				UnitEntries.Add(entry.SnoId, entry);
			}
			UnitPetEntries.Clear();
			foreach (var entry in unitdata.UnitPetEntries)
			{
				UnitPetEntries.Add(entry.SnoId, (PetTypes)entry.ObjectType);
			}


			var Items = ItemDataCollection.DeserializeFromXML();
			//var Items = new ItemDataCollection();
			//ItemDataCollection.SerializeToXML(Items);

			ItemDroppedEntries.Clear();
			foreach (var entry in Items.DroppedItemCache)
			{
				ItemDroppedEntries.Add(entry.SnoId, entry);
			}
			ItemDataEntries.Clear();
			foreach (var entry in Items.ItemDataCache)
			{
				ItemDataEntries.Add(entry.SnoId, entry);
			}
			ItemGemEntries.Clear();
			foreach (var entry in Items.GemCache)
			{
				ItemGemEntries.Add(entry.SnoId, entry);
			}
			ItemDroppedInternalNames.Clear();
			foreach (var entry in Items.DroppedItemInternalNames)
			{
				ItemDroppedInternalNames.Add(entry);
			}


			var Gizmos = GizmoDataCollection.DeserializeFromXML();
			//var Gizmos = new GizmoDataCollection();
			//GizmoDataCollection.SerializeToXML(Gizmos);

			GizmoEntries.Clear();
			foreach (var entry in Gizmos.GizmoCache)
			{
				GizmoEntries.Add(entry.SnoId, entry);
			}


			var Avoidance = AvoidanceDataCollection.DeserializeFromXML();
			//var Avoidance = new AvoidanceDataCollection();
			AvoidanceEntries.Clear();
			foreach (var entry in Avoidance.AvoidanceCache)
			{
				AvoidanceEntries.Add(entry.SnoId, entry);
			}
		}

		internal bool TryGetCacheValue(int snoid, out SnoEntry value)
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
