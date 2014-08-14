using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Objects
{
	public class GizmoStringDataCollection
	{
		public string[] Barricades { get; set; }
		public string[] Doors { get; set; }
		public string[] Containers { get; set; }

		public GizmoStringDataCollection()
		{
			Barricades = new[]
			{
				"door_destructable","breakables",
				"cart_a_breakable",
			};
			Containers = new[]
			{
				"chest", "loottype", "weaponrack", "armorrack",
			};
		}

		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "External", "Dictionaries", "SNOId_Cache_Gizmos.xml");
		internal static GizmoStringDataCollection DeserializeFromXML()
		{
			var deserializer = new XmlSerializer(typeof(GizmoStringDataCollection));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (GizmoStringDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		internal static void SerializeToXML(GizmoStringDataCollection settings)
		{
			var serializer = new XmlSerializer(typeof(GizmoStringDataCollection));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
	}
}
