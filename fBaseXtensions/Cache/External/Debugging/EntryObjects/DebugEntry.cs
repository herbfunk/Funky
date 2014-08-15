using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.External.Debugging.EntryObjects
{
	public class DebugEntry
	{
		public int SNOID { get; set; }
		public string Name { get; set; }
		public PluginActorType ActorType { get; set; }
		public PluginGizmoType GizmoType { get; set; }
		public TargetType TargetType { get; set; }
		public PluginDroppedItemTypes DroppedItemType { get; set; }

		public DebugEntry()
		{
			SNOID = 0;
			Name = String.Empty;
			ActorType = PluginActorType.Invalid;
			GizmoType = PluginGizmoType.None;
			TargetType = TargetType.None;
			DroppedItemType = PluginDroppedItemTypes.Unknown;
		}
		public DebugEntry(CachedSNOEntry entry)
		{
			SNOID = entry.SNOID;
			Name = entry.InternalName;
			ActorType = entry.Actortype.HasValue ? (PluginActorType)Enum.Parse(typeof(PluginActorType), entry.Actortype.Value.ToString()) : PluginActorType.Invalid;
			GizmoType = entry.Gizmotype.HasValue ? (PluginGizmoType)Enum.Parse(typeof(PluginGizmoType), entry.Gizmotype.Value.ToString()) : PluginGizmoType.None;
			TargetType = entry.targetType.HasValue ? entry.targetType.Value : TargetType.None;

			if (ActorType == PluginActorType.Item)
				DroppedItemType = ItemFunc.DetermineDroppedItemType(Name, SNOID);
		}
		public DebugEntry(int snoid, string name, PluginActorType actortype, PluginGizmoType gizmotype = PluginGizmoType.None)
		{
			SNOID = snoid;
			Name = name;
			ActorType = actortype;
			GizmoType = gizmotype;

			if (ActorType == PluginActorType.Item)
				DroppedItemType = ItemFunc.DetermineDroppedItemType(Name, SNOID);
		}

		public string ReturnCacheEntryString()
		{
			string returnString = "";
			if (ActorType == PluginActorType.Item)
			{
				returnString = "new DroppedItemEntry(" + SNOID + ", ";
				returnString = returnString + "PluginDroppedItemTypes." + DroppedItemType.ToString() + @", """ + Name + @"""),";
			}
			else if (ActorType == PluginActorType.Gizmo)
			{
				returnString = "new GizmoEntry(" + SNOID + ", ";
				returnString = returnString + "PluginGizmoType." + GizmoType.ToString() + @", """ + Name + @"""),";
			}

			return returnString;
		}

		public override int GetHashCode()
		{
			return SNOID;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var p = obj as DebugEntry;
			if (p == null)
				return false;
			return (SNOID == p.SNOID);
		}
	}
}
