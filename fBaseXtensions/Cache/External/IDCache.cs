using fBaseXtensions.Cache.External.Objects;

namespace fBaseXtensions.Cache.External
{
	public class IDCache
	{
		public AvoidanceDataCollection Avoidance { get; set; }
		public UnitDataCollection Units { get; set; }
		public ItemDataCollection Items { get; set; }
		public GizmoDataCollection Gizmos { get; set; }
		public UnitData UnitData { get; set; }

		public IDCache()
		{
			Avoidance = AvoidanceDataCollection.DeserializeFromXML(); //new AvoidanceDataCollection(); //
			Gizmos = GizmoDataCollection.DeserializeFromXML();//new GizmoDataCollection(); //
			Units = UnitDataCollection.DeserializeFromXML();//new UnitDataCollection(); //
			Items = ItemDataCollection.DeserializeFromXML();//new ItemDataCollection();//
			UnitData = UnitData.DeserializeFromXML(); //new UnitData();//
		}
	}
}
