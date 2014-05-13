using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace FunkyBot.Misc
{
	public class DebugData
	{
		//public HashSet<DebugEntry> Objects { get; set; }

		public HashSet<DebugEntry> Doors { get; set; }
		public HashSet<DebugEntry> Containers { get; set; }
		public HashSet<DebugEntry> Destructibles { get; set; }
		public HashSet<DebugEntry> Barricades { get; set; }
		
		public DebugData()
		{
			Doors = new HashSet<DebugEntry>();
			Containers = new HashSet<DebugEntry>();
			Destructibles = new HashSet<DebugEntry>();
			Barricades = new HashSet<DebugEntry>();
		}

		public void CheckEntry(CachedSNOEntry entry)
		{
			var d = new DebugEntry(entry);
			if (d.TargetType==TargetType.Door)
			{
				if (Doors.Contains(d)) return;
				Doors.Add(d);
			}
			else if (d.TargetType == TargetType.Container)
			{
				if (Containers.Contains(d)) return;
				Containers.Add(d);
			}
			else if(d.TargetType == TargetType.Destructible)
			{
				if (Destructibles.Contains(d)) return;
				Destructibles.Add(d);
			}
			else if(d.TargetType == TargetType.Barricade)
			{
				if (Barricades.Contains(d)) return;
				Barricades.Add(d);
			}
			else
			{
				return;
			}

			SerializeToXML(this);
		}

		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static void SerializeToXML(DebugData settings)
		{
			FolderPaths.CheckFolderExists(DefaultFilePath);

			var serializer = new XmlSerializer(typeof(DebugData));
			var textWriter = new StreamWriter(DefaultFilePath + @"\DataDebug.xml");
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData DeserializeFromXML()
		{
			string filePath = DefaultFilePath + @"\DataDebug.xml";
			if (!File.Exists(filePath))
			{
				Logger.DBLog.Debug("Could not load Data Debugging File!");
				return new DebugData();
			}

			var deserializer = new XmlSerializer(typeof(DebugData));
			TextReader textReader = new StreamReader(filePath);
			var settings = (DebugData)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}


		public class DebugEntry
		{
			public int SNOID { get; set; }
			public string Name { get; set; }
			public ActorType ActorType { get; set; }
			public GizmoType GizmoType { get; set; }
			public TargetType TargetType { get; set; }
			

			public DebugEntry()
			{
				SNOID = 0;
				Name = String.Empty;
				ActorType= ActorType.Invalid;
				GizmoType= GizmoType.None;
				TargetType = TargetType.None;
				
			}
			public DebugEntry(CachedSNOEntry entry)
			{
				SNOID=entry.SNOID;
				Name = entry.InternalName;
				ActorType=entry.Actortype.HasValue ? entry.Actortype.Value : ActorType.Invalid;
				GizmoType=entry.Gizmotype.HasValue ? entry.Gizmotype.Value : GizmoType.None;
				TargetType=entry.targetType.HasValue ? entry.targetType.Value : TargetType.None;
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
}
