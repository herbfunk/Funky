using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Debugging.EntryObjects;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Debugging
{
	public class DebugData_Doors
	{
		public DebugData_Doors()
		{
			Entries = new HashSet<DebugEntry>();
		}
		public HashSet<DebugEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_Doors.xml";
		internal static void SerializeToXML(DebugData_Doors settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_Doors));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_Doors DeserializeFromXML()
		{

			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_Doors.xml");
				var debugData = new DebugData_Doors();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_Doors));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_Doors)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}