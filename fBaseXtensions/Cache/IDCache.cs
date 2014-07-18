using fBaseXtensions.Cache.Objects;

namespace fBaseXtensions.Cache
{
	public class IDCache
	{
		public GizmoSnoDataCollection GizmosSno { get; set; }
		public UnitSnoDataCollection UnitsSno { get; set; }
		public ItemSnoDataCollection ItemsSno { get; set; }
		public ItemStringDataCollection ItemsString { get; set; }

		public IDCache()
		{
			GizmosSno = GizmoSnoDataCollection.DeserializeFromXML();
			UnitsSno = UnitSnoDataCollection.DeserializeFromXML();
			ItemsSno = ItemSnoDataCollection.DeserializeFromXML();//new ItemSnoDataCollection();//
			ItemsString = ItemStringDataCollection.DeserializeFromXML(); //new ItemStringDataCollection();
		}
	}
}
