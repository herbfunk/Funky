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

	}

	public class GizmoEntry : SnoEntry
	{
		public override EntryType EntryType { get { return EntryType.Gizmo; } }
		public override ActorType ActorType { get { return ActorType.Gizmo; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType)),
		XmlElement(Type = typeof(AvoidanceType)),
		XmlElement(Type = typeof(UnitFlags))]
		public new Object ObjectType
		{
			get { return _objectType; }
			set { _objectType = value; }
		}
		private Object _objectType = GizmoType.None;


		public GizmoEntry() : base() { }
		public GizmoEntry(int snoID, GizmoType objectType, string internalname = "")
			: base(snoID)
		{
			InternalName = internalname;
			_objectType = objectType;
		}
	}

	public class DroppedItemEntry:SnoEntry
	{
		public override EntryType EntryType { get { return EntryType.Item; } }
		public override ActorType ActorType { get { return ActorType.Item; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType)),
		XmlElement(Type = typeof(AvoidanceType)),
		XmlElement(Type = typeof(UnitFlags))]
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
	}

	public class AvoidanceEntry : SnoEntry
	{
		public override EntryType EntryType { get { return EntryType.Avoidance; } }
		public override ActorType ActorType { get { return ActorType.ServerProp; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType)),
		XmlElement(Type = typeof(AvoidanceType)),
		XmlElement(Type = typeof(UnitFlags))]
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
	}

	public class UnitEntry : SnoEntry
	{
		public override EntryType EntryType { get { return EntryType.Unit; } }
		public override ActorType ActorType { get { return ActorType.Monster; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType)),
		XmlElement(Type = typeof(AvoidanceType)),
		XmlElement(Type = typeof(UnitFlags))]
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





}
