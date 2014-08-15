using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.External.Objects
{
	public abstract class CacheEntry
	{
		public int SnoId { get; set; }
		public string InternalName { get; set; }
		public virtual ActorType ActorType { get; set; }
		public virtual EntryType EntryType { get; set; }

		[XmlIgnore]
		public virtual Object ObjectType { get; set; }

		protected CacheEntry() { SnoId = -1; }
		protected CacheEntry(int snoID)
		{
			SnoId = snoID;
			InternalName = String.Empty;
		}

		public override int GetHashCode()
		{
			return SnoId;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var p = obj as CacheEntry;
			if (p == null)
				return false;
			return (SnoId == p.SnoId);
		}

		public override string ToString()
		{
			return String.Format("SnoID[{0}] ActorType[{1}] EntryType[{2}] Name[{3}]", SnoId, ActorType.ToString(), EntryType.ToString(), InternalName);
		}
	}

	public class CacheGizmoEntry : CacheEntry
	{
		public override EntryType EntryType { get { return EntryType.Gizmo; } }
		public override ActorType ActorType { get { return ActorType.Gizmo; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType)),
		XmlElement(Type = typeof(AvoidanceType)),
		XmlElement(Type = typeof(UnitFlags)),
		XmlElement(Type = typeof(PetTypes))]
		public new Object ObjectType
		{
			get { return _objectType; }
			set { _objectType = value; }
		}
		private Object _objectType = GizmoType.None;

		public GizmoTargetTypes GizmotargetType
		{
			get { return _gizmotargetType; }
			set { _gizmotargetType = value; }
		}
		private GizmoTargetTypes _gizmotargetType = GizmoTargetTypes.None;





		public CacheGizmoEntry() : base() { }
		public CacheGizmoEntry(int snoID, GizmoType objectType, string internalname = "", GizmoTargetTypes targettype = GizmoTargetTypes.None)
			: base(snoID)
		{
			InternalName = internalname;
			_objectType = objectType;
			GizmotargetType = targettype;
		}
		public CacheGizmoEntry(GizmoEntry entry)
		{
			InternalName = entry.InternalName;
			var pluginGizmoType = (PluginGizmoType)entry.ObjectType;
			_objectType = (GizmoType)Enum.Parse(typeof(GizmoType), pluginGizmoType.ToString());
			GizmotargetType = entry.GizmotargetType;
		}

		public override string ToString()
		{
			return base.ToString() + " " + String.Format("GizmoType[{0}] GizmoTargetType[{1}]", ((GizmoType)_objectType).ToString(), _gizmotargetType);
		}
	}

	public class CacheDroppedItemEntry : CacheEntry
	{
		public override EntryType EntryType { get { return EntryType.Item; } }
		public override ActorType ActorType { get { return ActorType.Item; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType)),
		XmlElement(Type = typeof(AvoidanceType)),
		XmlElement(Type = typeof(UnitFlags)),
		XmlElement(Type = typeof(PetTypes))]
		public new Object ObjectType
		{
			get { return _objectType; }
			set { _objectType = value; }
		}
		private Object _objectType = PluginDroppedItemTypes.Unknown;


		public CacheDroppedItemEntry() : base() { }
		public CacheDroppedItemEntry(int snoID, PluginDroppedItemTypes objectType, string internalname = "")
			: base(snoID)
		{
			InternalName = internalname;
			_objectType = objectType;
		}

		public override string ToString()
		{
			return base.ToString() + " " + String.Format("PluginDroppedItemTypes[{0}]", ((PluginDroppedItemTypes)_objectType).ToString());
		}
	}

	public class CacheAvoidanceEntry : CacheEntry
	{
		public override EntryType EntryType { get { return EntryType.Avoidance; } }
		public override ActorType ActorType { get { return ActorType.ServerProp; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType)),
		XmlElement(Type = typeof(AvoidanceType)),
		XmlElement(Type = typeof(UnitFlags)),
		XmlElement(Type = typeof(PetTypes))]
		public new Object ObjectType
		{
			get { return _objectType; }
			set { _objectType = value; }
		}
		private Object _objectType = AvoidanceType.None;


		public CacheAvoidanceEntry() : base() { }
		public CacheAvoidanceEntry(int snoID, AvoidanceType objectType, string internalname = "")
			: base(snoID)
		{
			InternalName = internalname;
			_objectType = objectType;
		}

		public override string ToString()
		{
			return base.ToString() + " " + String.Format("AvoidanceType[{0}]", ((AvoidanceType)_objectType).ToString());
		}
	}

	public class CacheUnitEntry : CacheEntry
	{
		public override EntryType EntryType { get { return EntryType.Unit; } }
		public override ActorType ActorType { get { return ActorType.Monster; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType)),
		XmlElement(Type = typeof(AvoidanceType)),
		XmlElement(Type = typeof(UnitFlags)),
		XmlElement(Type = typeof(PetTypes))]
		public new Object ObjectType
		{
			get { return _objectType; }
			set { _objectType = value; }
		}
		private Object _objectType = UnitFlags.None;


		public CacheUnitEntry() : base() { }
		public CacheUnitEntry(int snoID, UnitFlags flags, string internalname = "")
			: base(snoID)
		{
			InternalName = internalname;
			_objectType = flags;
		}

		public string ReturnCacheEntryString()
		{
			//new UnitEntry(5984, UnitFlags.TreasureGoblin, "treasureGoblin_A-12185"),
			string sUnitFlags = "UnitFlags." + ((UnitFlags)ObjectType).ToString().Replace(", ", " | UnitFlags.");
			return String.Format("new UnitEntry({0}, {1}, {3}{2}{3}),", SnoId, sUnitFlags, InternalName, @"""");
		}

		public override string ToString()
		{
			return base.ToString() + " " + String.Format("UnitFlags[{0}]", ((UnitFlags)_objectType).ToString());
		}
	}

	public class CacheUnitPetEntry : CacheEntry
	{
		public override EntryType EntryType { get { return EntryType.Unit; } }
		public override ActorType ActorType { get { return ActorType.Monster; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType)),
		XmlElement(Type = typeof(AvoidanceType)),
		XmlElement(Type = typeof(UnitFlags)),
		XmlElement(Type = typeof(PetTypes))]
		public new Object ObjectType
		{
			get { return _objectType; }
			set { _objectType = value; }
		}
		private Object _objectType = PetTypes.None;


		public CacheUnitPetEntry() : base() { }
		public CacheUnitPetEntry(int snoID, PetTypes type, string internalname = "")
			: base(snoID)
		{
			InternalName = internalname;
			_objectType = type;
		}

		public override string ToString()
		{
			return base.ToString() + " " + String.Format("PetTypes[{0}]", ((PetTypes)_objectType).ToString());
		}
	}

	public class CacheItemGemEntry
	{
		public int SnoId { get; set; }
		public GemType Type { get; set; }
		public GemQualityTypes Quality { get; set; }

		public CacheItemGemEntry()
		{
			SnoId = -1;
			Type = GemType.Amethyst;
			Quality = GemQualityTypes.Unknown;
		}
		public CacheItemGemEntry(int snoid, GemType type, GemQualityTypes quality)
		{
			SnoId = snoid;
			Type = type;
			Quality = quality;
		}
		public CacheItemGemEntry(ItemGemEntry entry)
		{
			SnoId = entry.SnoId;
			var pluginGemType = (PluginGemType)entry.Type;
			Type=(GemType)Enum.Parse(typeof(GemType), pluginGemType.ToString());
			Quality = entry.Quality;
		}

		public override int GetHashCode()
		{
			return SnoId;
		}
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var p = obj as ItemGemEntry;
			if (p == null)
				return false;
			return (SnoId == p.SnoId);
		}
	}
}
