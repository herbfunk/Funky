using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Debugging.EntryObjects;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Debugging
{
	public class DebugData_DroppedItems
	{
		public DebugData_DroppedItems()
		{
			Entries = new HashSet<DebugEntry>();
		}
		public HashSet<DebugEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_DroppedItems.xml";

		internal static void SerializeToXML(DebugData_DroppedItems settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_DroppedItems));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_DroppedItems DeserializeFromXML()
		{
			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_DroppedItems.xml");
				var debugData = new DebugData_DroppedItems();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_DroppedItems));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_DroppedItems)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}