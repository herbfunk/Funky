using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Debugging.EntryObjects;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Debugging
{
	public class DebugData_Barricades
	{
		public DebugData_Barricades()
		{
			Entries = new HashSet<DebugEntry>();
		}
		public HashSet<DebugEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_Barricades.xml";

		internal static void SerializeToXML(DebugData_Barricades settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_Barricades));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_Barricades DeserializeFromXML()
		{
			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_Barricades.xml");
				var debugData = new DebugData_Barricades();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_Barricades));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_Barricades)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}