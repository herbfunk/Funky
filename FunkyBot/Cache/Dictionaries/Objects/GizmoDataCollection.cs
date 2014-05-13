using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Cache.Dictionaries.Objects
{
	public class GizmoDataCollection
	{
		public HashSet<int> Doors { get; set; }
		public HashSet<int> SpecialInteractables { get; set; }
		public HashSet<int> ResplendantChests { get; set; }

		public GizmoDataCollection()
		{
			Doors = new HashSet<int>();
			ResplendantChests = new HashSet<int>();
			SpecialInteractables = new HashSet<int>();
		}

		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "Dictionaries", "SNOId_Cache_Gizmos.xml");
		internal static GizmoDataCollection DeserializeFromXML()
		{
			var deserializer = new XmlSerializer(typeof(GizmoDataCollection));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (GizmoDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		internal static void SerializeToXML(GizmoDataCollection settings)
		{
			var serializer = new XmlSerializer(typeof(GizmoDataCollection));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
	}
}
