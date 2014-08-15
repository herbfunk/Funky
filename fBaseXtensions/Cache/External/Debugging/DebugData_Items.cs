using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Debugging.EntryObjects;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Debugging
{
	public class DebugData_Items
	{
		public DebugData_Items()
		{
			Entries = new HashSet<DebugItemDataEntry>();
		}
		public HashSet<DebugItemDataEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_Items.xml";

		internal static void SerializeToXML(DebugData_Items settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_Items));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_Items DeserializeFromXML()
		{
			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_Items.xml");
				var debugData = new DebugData_Items();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_Items));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_Items)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}