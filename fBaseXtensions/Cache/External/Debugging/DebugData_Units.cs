using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Debugging.EntryObjects;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Debugging
{
	public class DebugData_Units
	{
		public DebugData_Units()
		{
			Entries = new HashSet<DebugUnitEntry>();
		}
		public HashSet<DebugUnitEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_Units.xml";

		internal static void SerializeToXML(DebugData_Units settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_Units));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_Units DeserializeFromXML()
		{
			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_Units.xml");
				var debugData = new DebugData_Units();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_Units));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_Units)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}