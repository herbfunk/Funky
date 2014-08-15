using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Debugging.EntryObjects;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Debugging
{
	public class DebugData_Containers
	{
		public DebugData_Containers()
		{
			Entries = new HashSet<DebugEntry>();
		}
		public HashSet<DebugEntry> Entries { get; set; }
		private static readonly string DefaultFolderPath = Path.Combine(FolderPaths.PluginPath, "Log");
		private static readonly string DefaultFilePath = DefaultFolderPath + @"\DebugData_Containers.xml";

		internal static void SerializeToXML(DebugData_Containers settings)
		{
			FolderPaths.CheckFolderExists(DefaultFolderPath);

			var serializer = new XmlSerializer(typeof(DebugData_Containers));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		internal static DebugData_Containers DeserializeFromXML()
		{
			if (!File.Exists(DefaultFilePath))
			{
				Logger.DBLog.DebugFormat("Could not load Data Debugging File! {0}", "DebugData_Containers.xml");
				var debugData = new DebugData_Containers();
				SerializeToXML(debugData);
				return debugData;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_Containers));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (DebugData_Containers)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}

		internal static DebugData_Containers DeserializeFromXML(string FilePath)
		{
			if (!File.Exists(FilePath))
			{
				return null;
			}

			var deserializer = new XmlSerializer(typeof(DebugData_Containers));
			TextReader textReader = new StreamReader(FilePath);
			var settings = (DebugData_Containers)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}