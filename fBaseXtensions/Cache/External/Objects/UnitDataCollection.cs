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

		public HashSet<UnitEntry> UnitEntries { get; set; } 

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
			UnitEntries = new HashSet<UnitEntry>
			{
				new UnitEntry(5350, UnitFlags.Boss, "SkeletonKing"),
				new UnitEntry(5387, UnitFlags.Normal | UnitFlags.Ranged | UnitFlags.Summoner, "SkeletonSummoner_A-3074"),
				new UnitEntry(5393, UnitFlags.Normal, "Skeleton_A-3088"),
				new UnitEntry(5346, UnitFlags.Normal | UnitFlags.Ranged, "SkeletonArcher_A-3093"),
				new UnitEntry(5275, UnitFlags.Normal, "Shield_Skeleton_A-3100"),
				new UnitEntry(6356, UnitFlags.Normal, "Unburied_A-3007"),
				new UnitEntry(4286, UnitFlags.Normal | UnitFlags.Ranged, "Goatman_Ranged_A-4379"),
				new UnitEntry(4282, UnitFlags.Normal, "Goatman_Melee_A-4378"),
				new UnitEntry(3337, UnitFlags.Rare, "Beast_A-4362"),
				new UnitEntry(4157, UnitFlags.Normal | UnitFlags.Flying, "FleshPitFlyer_B-4355"),
				new UnitEntry(4153, UnitFlags.Normal | UnitFlags.Summoner, "FleshPitFlyerSpawner_B-4352"),
				new UnitEntry(139454, UnitFlags.Normal, "WoodWraith_A_02-4412"),
				new UnitEntry(5236, UnitFlags.Normal | UnitFlags.Fast, "Scavenger_B-5021"),
				new UnitEntry(365330, UnitFlags.Rare, "Goatman_Melee_A_Unique_03-5431"),
				new UnitEntry(4983, UnitFlags.Normal | UnitFlags.Ranged, "QuillDemon_B-6082"),
				new UnitEntry(166726, UnitFlags.Normal, "Spider_Poison_A-7111"),
				new UnitEntry(5467, UnitFlags.Normal, "Spiderling_A-7140"),
				new UnitEntry(4201, UnitFlags.Normal, "Ghoul_A-7213"),
				new UnitEntry(5474, UnitFlags.Normal, "Spider_A-7259"),
				new UnitEntry(218462, UnitFlags.Rare | UnitFlags.Fast, "Spider_Poison_A_Unique_02-7479"),
				new UnitEntry(5987, UnitFlags.TreasureGoblin, "treasureGoblin_C-8189"),

			};

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
