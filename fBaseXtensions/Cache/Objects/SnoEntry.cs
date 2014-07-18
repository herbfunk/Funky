using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.Objects
{
	public enum EntryType
	{
		None=0,
		Item,
		Gizmo,
		Unit
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

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType))]
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

	public class ItemEntry:SnoEntry
	{
		public override EntryType EntryType { get { return EntryType.Item; } }
		public override ActorType ActorType { get { return ActorType.Item; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType))]
		public override object ObjectType
		{
			get { return _objectType; }
			set { _objectType = value; }
		}
		 private object _objectType=PluginDroppedItemTypes.Unknown;


		public ItemEntry() : base() { }
		public ItemEntry(int snoID, PluginDroppedItemTypes objectType, string internalname="")
			:base(snoID)
		{
			InternalName=internalname;
			_objectType = objectType;
		}
	}

	public class GizmoEntry : SnoEntry
	{
		public override EntryType EntryType { get { return EntryType.Gizmo; } }
		public override ActorType ActorType { get { return ActorType.Gizmo; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(GizmoType))]
		public override object ObjectType
		{
			get { return _objectType; }
			set { _objectType = value; }
		}
		private object _objectType = GizmoType.None;


		public GizmoEntry() : base() { }
		public GizmoEntry(int snoID, GizmoType objectType, string internalname = "")
			:base(snoID)
		{
			InternalName=internalname;
			_objectType = objectType;
		}
	}
}
