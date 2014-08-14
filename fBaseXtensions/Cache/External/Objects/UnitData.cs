using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Objects
{
	public class UnitData
	{
		public HashSet<UnitEntry> UnitEntries { get; set; } 

		public UnitData()
		{
			UnitEntries = new HashSet<UnitEntry>
			{
				new UnitEntry(5984, UnitFlags.TreasureGoblin, "treasureGoblin_A-12185"),
				new UnitEntry(5987, UnitFlags.TreasureGoblin, "treasureGoblin_C-8189"),
				new UnitEntry(5988, UnitFlags.TreasureGoblin, "treasureGoblin_D-6951"),

				// ================ ACT ONE ====================
				new UnitEntry(5350, UnitFlags.Boss, "SkeletonKing"),
				
				new UnitEntry(218462, UnitFlags.Rare | UnitFlags.Fast, "Spider_Poison_A_Unique_02-7479"),
				new UnitEntry(129012, UnitFlags.Rare, "Goatman_Melee_B_Ghost-9549"),
				new UnitEntry(218674, UnitFlags.Rare | UnitFlags.Fast, "Triune_Berserker_A_Unique_02-13348"),
				new UnitEntry(105959, UnitFlags.Rare, "TriuneCultist_C_TortureLeader-14148"),
				new UnitEntry(260237, UnitFlags.Rare, "TriuneCultist_A_VendorRescue_Unique-12735"),
				new UnitEntry(3337, UnitFlags.Rare, "Beast_A-4362"),
				new UnitEntry(365330, UnitFlags.Rare, "Goatman_Melee_A_Unique_03-5431"),
				new UnitEntry(370, UnitFlags.Rare | UnitFlags.Fast | UnitFlags.Flying, "Ghost_A-3368"),

				new UnitEntry(294664, UnitFlags.Normal, "x1_Ghoul_A_Challenge-1856"),
				new UnitEntry(375, UnitFlags.Normal | UnitFlags.Ranged, "Goatman_Shaman_B-4749"),
				new UnitEntry(5395, UnitFlags.Normal, "Skeleton_B-8092"),
				new UnitEntry(5387, UnitFlags.Normal | UnitFlags.Ranged | UnitFlags.Summoner, "SkeletonSummoner_A-3074"),
				new UnitEntry(5393, UnitFlags.Normal, "Skeleton_A-3088"),
				new UnitEntry(5346, UnitFlags.Normal | UnitFlags.Ranged, "SkeletonArcher_A-3093"),
				new UnitEntry(5275, UnitFlags.Normal, "Shield_Skeleton_A-3100"),
				new UnitEntry(6356, UnitFlags.Normal, "Unburied_A-3007"),
				new UnitEntry(4286, UnitFlags.Normal | UnitFlags.Ranged, "Goatman_Ranged_A-4379"),
				new UnitEntry(4282, UnitFlags.Normal, "Goatman_Melee_A-4378"),
				new UnitEntry(4157, UnitFlags.Normal | UnitFlags.Flying, "FleshPitFlyer_B-4355"),
				new UnitEntry(4153, UnitFlags.Normal | UnitFlags.Summoner, "FleshPitFlyerSpawner_B-4352"),
				new UnitEntry(139454, UnitFlags.Normal, "WoodWraith_A_02-4412"),
				new UnitEntry(5236, UnitFlags.Normal | UnitFlags.Fast | UnitFlags.Burrowing, "Scavenger_B-5021"),
				new UnitEntry(4983, UnitFlags.Normal | UnitFlags.Ranged, "QuillDemon_B-6082"),
				new UnitEntry(166726, UnitFlags.Normal, "Spider_Poison_A-7111"),
				new UnitEntry(5467, UnitFlags.Normal, "Spiderling_A-7140"),
				new UnitEntry(4201, UnitFlags.Normal, "Ghoul_A-7213"),
				new UnitEntry(5474, UnitFlags.Normal, "Spider_A-7259"),
				new UnitEntry(4283, UnitFlags.Normal, "Goatman_Melee_B-8932"),
				new UnitEntry(4287, UnitFlags.Normal | UnitFlags.Ranged, "Goatman_Ranged_B-8930"),
				new UnitEntry(99556, UnitFlags.Normal, "WitherMoth_A_Hidden-8944"),
				new UnitEntry(4290, UnitFlags.Normal | UnitFlags.Ranged, "Goatman_Shaman_A-9279"),
				new UnitEntry(6024, UnitFlags.Normal, "TriuneCultist_A-9509"),
				new UnitEntry(170324, UnitFlags.Normal, "WoodWraith_B_01-9529"),
				new UnitEntry(495, UnitFlags.Normal, "WoodWraith_B_03-9685"),
				new UnitEntry(166452, UnitFlags.Normal, "trOut_Highlands_Goatmen_SummoningMachine_A_Node-10023"),
				new UnitEntry(170325, UnitFlags.Normal, "WoodWraith_B_02-10029"),
				new UnitEntry(6035, UnitFlags.Normal | UnitFlags.Ranged, "TriuneSummoner_A-10080"),
				new UnitEntry(6059, UnitFlags.Normal, "Triune_Summonable_A-10084"),
				new UnitEntry(6046, UnitFlags.Normal, "TriuneVessel_A-12145"),
				new UnitEntry(6359, UnitFlags.Normal, "Unburied_C-12299"),
				new UnitEntry(6052, UnitFlags.Normal, "Triune_Berserker_A-12360"),
				new UnitEntry(113949, UnitFlags.Normal, "ZombieCrawler_Custom_C-13275"),
				new UnitEntry(6042, UnitFlags.Normal, "TriuneVesselActivated_A-13288"),
				new UnitEntry(100956, UnitFlags.Normal, "Spawner_Leor_Iron_Maiden-14866"),
				new UnitEntry(90453, UnitFlags.Normal, "Zombie_Inferno_C-14953"),
				new UnitEntry(3847, UnitFlags.Normal | UnitFlags.Grotesque, "Corpulent_A-457"),
				new UnitEntry(6646, UnitFlags.Normal, "ZombieSkinny_B-459"),
				new UnitEntry(6644, UnitFlags.Normal, "ZombieSkinny_A-481"),
				new UnitEntry(6652, UnitFlags.Normal, "Zombie_A-477"),
				new UnitEntry(4156, UnitFlags.Rare | UnitFlags.Flying, "FleshPitFlyer_A-536"),
				new UnitEntry(6632, UnitFlags.Normal, "ZombieCrawler_A-560"),
				new UnitEntry(4564, UnitFlags.Normal, "Lamprey_A-684"),
				new UnitEntry(6023, UnitFlags.Normal, "trist_Urn_Tall-2629"),
				new UnitEntry(364563, UnitFlags.Normal | UnitFlags.Ranged, "QuillDemon_FastAttack_A-5555"),
				new UnitEntry(364508, UnitFlags.Normal, "ZombieFemale_Spitter_A-5560"),
				new UnitEntry(6653, UnitFlags.Normal, "Zombie_B-5575"),
				new UnitEntry(6639, UnitFlags.Normal | UnitFlags.Summoner, "ZombieFemale_B-5768"),
				new UnitEntry(5235, UnitFlags.Normal | UnitFlags.Fast | UnitFlags.Burrowing, "Scavenger_A-5765"),
				new UnitEntry(123160, UnitFlags.Normal, "ZombieCrawler_Custom_B-5786"),
				new UnitEntry(4152, UnitFlags.Normal | UnitFlags.Summoner, "FleshPitFlyerSpawner_A-6429"),
				new UnitEntry(3893, UnitFlags.Normal, "CryptChild_A-8219"),
				new UnitEntry(131280, UnitFlags.Normal, "graveRobber_A_Ghost-8239"),
				new UnitEntry(131278, UnitFlags.Normal, "graveDigger_B_Ghost-8312"),
				new UnitEntry(6633, UnitFlags.Normal, "ZombieCrawler_B-8390"),

				new UnitEntry(218448, UnitFlags.AdventureModeBoss | UnitFlags.Fast, "Spider_A_Unique_01-8699"),
				new UnitEntry(218206, UnitFlags.AdventureModeBoss | UnitFlags.Fast, "graveDigger_B_Ghost_Unique-9675"),
				new UnitEntry(365906, UnitFlags.AdventureModeBoss, "Unburied_C_Unique_01-3328"),
				new UnitEntry(332432, UnitFlags.AdventureModeBoss | UnitFlags.Ranged, "x1_devilshand_unique_SkeletonSummoner_B-8145"),

			};

		}

		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "External", "Dictionaries", "Cache_UnitData.xml");
		internal static UnitData DeserializeFromXML()
		{
			var deserializer = new XmlSerializer(typeof(UnitData));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (UnitData)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		internal static void SerializeToXML(UnitData settings)
		{
			var serializer = new XmlSerializer(typeof(UnitData));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
	}
}
