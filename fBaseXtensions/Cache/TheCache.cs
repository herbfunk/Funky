using System.Collections.Generic;
using fBaseXtensions.Cache.External;
using fBaseXtensions.Cache.External.Objects;
using Zeta.Game;

namespace fBaseXtensions.Cache
{
	public static class TheCache
	{
		public static IDCache ObjectIDCache;
		public static Dictionary<int, SnoEntry> CacheEntries = new Dictionary<int, SnoEntry>();
		public static Dictionary<int, ItemDataEntry> ItemDataEntries = new Dictionary<int, ItemDataEntry>(); 

		internal static void LoadCache()
		{
			ObjectIDCache = new IDCache();
			
			//AvoidanceDataCollection.SerializeToXML(TheCache.ObjectIDCache.Avoidance);
			//ItemDataCollection.SerializeToXML(TheCache.ObjectIDCache.Items);
			//GizmoDataCollection.SerializeToXML(TheCache.ObjectIDCache.Gizmos);
			//UnitDataCollection.SerializeToXML(ObjectIDCache.Units);
			//UnitData.SerializeToXML(ObjectIDCache.UnitData);

			CacheEntries.Clear();
			foreach (var entry in ObjectIDCache.Items.DroppedItemCache)
			{
				CacheEntries.Add(entry.SnoId, entry);
			}
			foreach (var entry in ObjectIDCache.Gizmos.GizmoCache)
			{
				CacheEntries.Add(entry.SnoId, entry);
			}
			foreach (var entry in ObjectIDCache.UnitData.UnitEntries)
			{
				CacheEntries.Add(entry.SnoId, entry);
			}

			ItemDataEntries.Clear();
			foreach (var entry in ObjectIDCache.Items.ItemDataCache)
			{
				ItemDataEntries.Add(entry.SnoId, entry);
			}
		}
		public static readonly List<int> riftWorldIds = new List<int>()
        {
			288454,
			288685,
			288687,
			288798,
			288800,
			288802,
			288804,
			288806,
			288810,
			288814,
			288816,
        };

		///<summary>
		///To Find Town Areas
		///</summary>
		public static Act FindActByLevelID(int ID)
		{
			switch (ID)
			{
				case 332339:
					return Act.A1;
				case 168314:
					return Act.A2;
				case 92945:
					return Act.A3;
				case 270011:
					return Act.A5;
			}

			return Act.Invalid;
		}
	}
}
