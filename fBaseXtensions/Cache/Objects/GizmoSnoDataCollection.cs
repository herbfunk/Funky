using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.Objects
{
	public class GizmoSnoDataCollection
	{
		public HashSet<int> Doors { get; set; }
		public HashSet<int> SpecialInteractables { get; set; }
		public HashSet<int> ResplendantChests { get; set; }

		public GizmoSnoDataCollection()
		{
			Doors = new HashSet<int>();
			ResplendantChests = new HashSet<int>();
			SpecialInteractables = new HashSet<int>();
		}

		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "Dictionaries", "SNOId_Cache_Gizmos.xml");
		internal static GizmoSnoDataCollection DeserializeFromXML()
		{
			var deserializer = new XmlSerializer(typeof(GizmoSnoDataCollection));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (GizmoSnoDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		internal static void SerializeToXML(GizmoSnoDataCollection settings)
		{
			var serializer = new XmlSerializer(typeof(GizmoSnoDataCollection));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
	}
}
