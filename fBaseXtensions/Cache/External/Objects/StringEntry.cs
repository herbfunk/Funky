using System;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.External.Objects
{
	public abstract class StringEntry
	{
		public string ID { get; set; }
		public virtual PluginActorType ActorType { get; set; }
		public virtual EntryType EntryType { get; set; }

		[XmlIgnore]
		public virtual Object ObjectType { get; set; }

		protected StringEntry() { ID = ""; }
		protected StringEntry(string Name)
		{
			ID = Name;
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var p = obj as StringEntry;
			if (p == null)
				return false;
			return (String.Equals(ID, p.ID));
		}
	}

	public class ItemStringEntry : StringEntry
	{
		public override EntryType EntryType { get { return EntryType.Item; } }
		public override PluginActorType ActorType { get { return PluginActorType.Item; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(PluginGizmoType))]
		public new Object ObjectType
		{
			get { return _objectType; }
			set { _objectType = value; }
		}
		private Object _objectType = PluginDroppedItemTypes.Unknown;


		public ItemStringEntry() : base() { }
		public ItemStringEntry(string name, PluginDroppedItemTypes objectType)
			: base(name)
		{
			_objectType = objectType;
		}
	}

	public class GizmoStringEntry : StringEntry
	{
		public override EntryType EntryType { get { return EntryType.Gizmo; } }
		public override PluginActorType ActorType { get { return PluginActorType.Gizmo; } }

		[XmlElement(Type = typeof(PluginDroppedItemTypes)),
		XmlElement(Type = typeof(PluginGizmoType))]
		public new Object ObjectType
		{
			get { return _objectType; }
			set { _objectType = value; }
		}
		private Object _objectType = PluginGizmoType.None;


		public GizmoStringEntry() : base() { }
		public GizmoStringEntry(string name, PluginGizmoType objectType)
			: base(name)
		{
			_objectType = objectType;
		}
	}
}
