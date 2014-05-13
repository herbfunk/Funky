namespace FunkyBot.Cache.Dictionaries.Objects
{
	public class SnoIDCache
	{
		public GizmoDataCollection Gizmos { get; set; }
		public UnitDataCollection Units { get; set; }

		public SnoIDCache()
		{
			Gizmos = GizmoDataCollection.DeserializeFromXML();
			Units = UnitDataCollection.DeserializeFromXML();
		}
	}
}
