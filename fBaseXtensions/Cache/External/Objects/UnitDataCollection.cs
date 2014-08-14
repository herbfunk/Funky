using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Objects
{
	public class UnitDataCollection
	{
		public HashSet<int> SpawnerUnits { get; set; }
		public HashSet<int> FastUnits { get; set; }
		public HashSet<int> BurrowableUnits { get; set; }
		public HashSet<int> GrotesqueUnits { get; set; }
		public HashSet<int> SucideBomberUnits { get; set; }
		public HashSet<int> StealthUnits { get; set; }
		public HashSet<int> ReflectiveMissleUnits { get; set; }
		public HashSet<int> RangedUnits { get; set; }
		public HashSet<int> FlyingUnits { get; set; }
		public HashSet<int> RevivableUnits { get; set; }
		public HashSet<int> GoblinUnits { get; set; }
		public HashSet<int> BossUnits { get; set; }
		public HashSet<UnitPriority> UnitPriorities { get; set; }

		public UnitDataCollection()
		{
			SpawnerUnits = new HashSet<int>();
			FastUnits = new HashSet<int>();
			BurrowableUnits = new HashSet<int>();
			GrotesqueUnits = new HashSet<int>();
			SucideBomberUnits = new HashSet<int>();
			StealthUnits = new HashSet<int>();
			ReflectiveMissleUnits = new HashSet<int>();
			RangedUnits = new HashSet<int>();
			FlyingUnits = new HashSet<int>();
			RevivableUnits = new HashSet<int>();
			GoblinUnits = new HashSet<int>();
			BossUnits = new HashSet<int>();
			UnitPriorities = new HashSet<UnitPriority>();


		}

		public void ClearCollections()
		{
			SpawnerUnits.Clear();
			FastUnits.Clear();

			BurrowableUnits.Clear();
			GrotesqueUnits.Clear();
			SucideBomberUnits.Clear();
			StealthUnits.Clear();
			ReflectiveMissleUnits.Clear();
			RangedUnits.Clear();
			FlyingUnits.Clear();
			RevivableUnits.Clear();
			GoblinUnits.Clear();
			BossUnits.Clear();
			UnitPriorities.Clear();
		}

		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "External", "Dictionaries", "Cache_Units.xml");
		internal static UnitDataCollection DeserializeFromXML()
		{
			var deserializer = new XmlSerializer(typeof(UnitDataCollection));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (UnitDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		internal static void SerializeToXML(UnitDataCollection settings)
		{
			var serializer = new XmlSerializer(typeof(UnitDataCollection));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}

	}
}
