using System;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;

namespace fBaseXtensions.Cache.External.Debugging.EntryObjects
{
	public class DebugUnitEntry
	{
		public int SNOID { get; set; }
		public string Name { get; set; }
		public PluginActorType ActorType { get; set; }
		public UnitFlags UnitFlags { get; set; }
		public TargetType TargetType { get; set; }


		public DebugUnitEntry()
		{
			SNOID = 0;
			Name = String.Empty;
			ActorType = PluginActorType.Invalid;
			UnitFlags = UnitFlags.None;
			TargetType = TargetType.None;

		}
		public DebugUnitEntry(CacheUnit entry)
		{
			SNOID = entry.SNOID;
			Name = entry.InternalName;
			ActorType = PluginActorType.Monster;
			UnitFlags = entry.UnitPropertyFlags.HasValue ? entry.UnitPropertyFlags.Value : UnitFlags.None;
			TargetType = TargetType.Unit;
		}
		public DebugUnitEntry(int snoid, string name, UnitFlags flags)
		{
			SNOID = snoid;
			Name = name;
			ActorType = PluginActorType.Monster;
			UnitFlags = flags;
			TargetType = TargetType.Unit;
		}

		public string ReturnCacheEntryString()
		{
			string sUnitFlags = "UnitFlags." + UnitFlags.ToString().Replace(", ", " | UnitFlags.");
			return "new UnitEntry(" + SNOID + ", UnitFlags." + sUnitFlags + @", """ + Name + @"""),";
		}

		public override int GetHashCode()
		{
			return SNOID;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var p = obj as DebugUnitEntry;
			if (p == null)
				return false;
			return (SNOID == p.SNOID);
		}
	}
}
