using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using fBaseXtensions.Settings;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.External.Objects
{
	public enum EntryType
	{
		None=0,
		Item,
		Gizmo,
		Unit,
		Avoidance
	}

	/// <summary>
	/// Holds information to help ID objects
	/// SNO is the main property, Internal Name is optional.
	/// ActorType and EntryType declare what type of entry the object is.
	/// ObjectType holds an important data to help with further idenification of the object. (Such as GizmoType or ItemType)
	/// </summary>
	public abstract class SnoEntry
	{
		public int SnoId { get; set; }
		public string InternalName { get; set; }
		public virtual ActorType ActorType { get; set; }
		public virtual EntryType EntryType { get; set; }

		[XmlIgnore]
		public virtual Object ObjectType { get; set; }

		protected SnoEntry() { SnoId = -1; }
		protected SnoEntry(int snoID)
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

			var p = obj as SnoEntry;
			if (p == null)
				return false;
			return (SnoId == p.SnoId);
		}

		public override string ToString()
		{
			return String.Format("SnoID[{0}] ActorType[{1}] EntryType[{2}] Name[{3}]", SnoId, ActorType.ToString(), EntryType.ToString(), InternalName);
		}
	}

	public class GizmoEntry : SnoEntry
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





		public GizmoEntry() : base() { }
		public GizmoEntry(int snoID, GizmoType objectType, string internalname = "", GizmoTargetTypes targettype= GizmoTargetTypes.None)
			: base(snoID)
		{
			InternalName = internalname;
			_objectType = objectType;
			GizmotargetType = targettype;
		}

		public override string ToString()
		{
			return base.ToString() + " " + String.Format("GizmoType[{0}] GizmoTargetType[{1}]", ((GizmoType)_objectType).ToString(), _gizmotargetType);
		}
	}

	public class DroppedItemEntry:SnoEntry
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


		public DroppedItemEntry() : base() { }
		public DroppedItemEntry(int snoID, PluginDroppedItemTypes objectType, string internalname="")
			:base(snoID)
		{
			InternalName=internalname;
			_objectType = objectType;
		}

		public override string ToString()
		{
			return base.ToString() + " " + String.Format("PluginDroppedItemTypes[{0}]", ((PluginDroppedItemTypes)_objectType).ToString());
		}
	}

	public class AvoidanceEntry : SnoEntry
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


		public AvoidanceEntry() : base() { }
		public AvoidanceEntry(int snoID, AvoidanceType objectType, string internalname = "")
			:base(snoID)
		{
			InternalName=internalname;
			_objectType = objectType;
		}

		public override string ToString()
		{
			return base.ToString() + " " + String.Format("AvoidanceType[{0}]", ((AvoidanceType)_objectType).ToString());
		}
	}

	public class UnitEntry : SnoEntry
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


		public UnitEntry() : base() { }
		public UnitEntry(int snoID, UnitFlags flags, string internalname = "")
			:base(snoID)
		{
			InternalName=internalname;
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

	public class UnitPetEntry : SnoEntry
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


		public UnitPetEntry() : base() { }
		public UnitPetEntry(int snoID, PetTypes type, string internalname = "")
			:base(snoID)
		{
			InternalName=internalname;
			_objectType = type;
		}

		public override string ToString()
		{
			return base.ToString() + " " + String.Format("PetTypes[{0}]", ((PetTypes)_objectType).ToString());
		}
	}

	public class ItemDataEntry
	{
		public int SnoId { get; set; }
		public PluginItemTypes ItemType { get; set; }
		public LegendaryItemTypes LegendaryType { get; set; }

		public ItemDataEntry()
		{
			SnoId = -1;
			ItemType = PluginItemTypes.Unknown;
			LegendaryType = LegendaryItemTypes.None;
		}
		public ItemDataEntry(int snoid, PluginItemTypes type, LegendaryItemTypes legendarytype = LegendaryItemTypes.None)
		{
			SnoId = snoid;
			ItemType = type;
			LegendaryType = legendarytype;
		}


		public override int GetHashCode()
		{
			return SnoId;
		}
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var p = obj as ItemDataEntry;
			if (p == null)
				return false;
			return (SnoId == p.SnoId);
		}
	}

	public class ItemGemEntry
	{
		public int SnoId { get; set; }
		public GemType Type { get; set; }
		public GemQualityTypes Quality { get; set; }

		public ItemGemEntry()
		{
			SnoId = -1;
			Type = GemType.Amethyst;
			Quality = GemQualityTypes.Unknown;
		}
		public ItemGemEntry(int snoid, GemType type, GemQualityTypes quality)
		{
			SnoId = snoid;
			Type = type;
			Quality=quality;
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
