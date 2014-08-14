using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Helpers;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.External.Objects
{
	public class GizmoDataCollection
	{
		//GizmoType: Door Name: X1_Westm_Door_Giant_Lowering_Wolf-9346 ActorSNO: 308241 Distance: 29.25269 Position: <596.4537, 1140.521, 0.1612549> Barracade: False Radius: 21.95018

		public HashSet<GizmoEntry> GizmoCache { get; set; }
		public HashSet<GizmoStringEntry> GizmoStringCache { get; set; }

 

		public GizmoDataCollection()
		{
			GizmoStringCache = new HashSet<GizmoStringEntry>
			{
				new GizmoStringEntry("door_destructable", GizmoType.BreakableDoor),
				new GizmoStringEntry("cart_a_breakable", GizmoType.DestroyableObject),
				new GizmoStringEntry("breakables", GizmoType.DestroyableObject),
				new GizmoStringEntry("chest", GizmoType.Chest),
				new GizmoStringEntry("loottype", GizmoType.Chest),
				new GizmoStringEntry("corpse", GizmoType.Chest),
				new GizmoStringEntry("weaponrack", GizmoType.Chest),
				new GizmoStringEntry("armorrack", GizmoType.Chest),
				new GizmoStringEntry("corpse", GizmoType.Chest),
			};

			GizmoCache = new HashSet<GizmoEntry>
			{
				//============== DOORS ==============//
				new GizmoEntry(5854, GizmoType.Door),
				new GizmoEntry(454, GizmoType.Door),
				new GizmoEntry(5767, GizmoType.Door),
				new GizmoEntry(230324, GizmoType.Door),
				new GizmoEntry(308241, GizmoType.Door),
				new GizmoEntry(273323, GizmoType.Door),
				new GizmoEntry(308241, GizmoType.Door, "X1_Westm_Door_Giant_Lowering_Wolf-9346"),
				new GizmoEntry(308376, GizmoType.Door, "X1_Westm_Door_Giant_Iron_Bars_Arched-18634"),
				new GizmoEntry(90419, GizmoType.Door, "trOut_NewTristram_Gate_Town-67"),
				new GizmoEntry(309812, GizmoType.Door, "X1_Westm_Door_Giant_Iron-5188"),
				new GizmoEntry(152772, GizmoType.Door, "caOut_Oasis_aqd_door-8258"),
				new GizmoEntry(258595, GizmoType.Door, "x1_Catacombs_Door_A-8887"),
				new GizmoEntry(5763, GizmoType.Door, "trDun_Cath_Gate_A-93193"),
				new GizmoEntry(5766, GizmoType.Door, "trDun_Cath_Gate_C-9372"),
				new GizmoEntry(105361, GizmoType.Door, "a1dun_Leor_Gate_A-17181"),
				new GizmoEntry(95571, GizmoType.Door, "a1dun_Leor_Jail_Door_A-10528"),
				new GizmoEntry(5765, GizmoType.Door, "trDun_Cath_Gate_B_SkeletonKing-3656"),
				new GizmoEntry(104888, GizmoType.Door, "a1dunLeor_Interactive_Wooden_Door_A-6720"),

				//============== Resplendant Chests ==============//
				new GizmoEntry(62873, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(95011, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(81424, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(108230, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(111808, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(111809, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(199583, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(109264, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(101500, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(96993, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(62866, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(108230, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(211861, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(62860, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(96993, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(112182, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(363725, GizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(357509, GizmoType.Chest, "a1dun_Leor_Chest_Rare_Garrach-16708", GizmoTargetTypes.Resplendant),

				//============== Chests ==============//
				new GizmoEntry(62865, GizmoType.Chest, "TrOut_Highlands_Chest-18282", GizmoTargetTypes.Chest),
				new GizmoEntry(62859, GizmoType.Chest, "TrOut_Fields_Chest-9087", GizmoTargetTypes.Chest),
				new GizmoEntry(79319, GizmoType.Chest, "trOut_Highlands_chest_Bloody-9602", GizmoTargetTypes.Chest),
				new GizmoEntry(94708, GizmoType.Chest, "a1dun_Leor_Chest-14472", GizmoTargetTypes.Chest),
				new GizmoEntry(108792, GizmoType.Chest, "trOut_Wilderness_HangingTree_GraveChest-1883", GizmoTargetTypes.Chest),
				new GizmoEntry(5822, GizmoType.Chest, "trDun_Crypt_Chest_01-2841", GizmoTargetTypes.Chest),
				new GizmoEntry(70534, GizmoType.Chest, "a2dun_Spider_Chest-7959", GizmoTargetTypes.Chest),
				new GizmoEntry(96522, GizmoType.Chest, "a1dun_Cath_chest-3168", GizmoTargetTypes.Chest),
				new GizmoEntry(364559, GizmoType.Chest, "x1_Global_Chest_CursedChest-1801", GizmoTargetTypes.Chest),
				new GizmoEntry(357331, GizmoType.Chest, "x1_Global_Chest-2959", GizmoTargetTypes.Chest),
				new GizmoEntry(375539, GizmoType.Chest, "x1_Global_Chest_SpeedKill_Elite-8331", GizmoTargetTypes.Chest),
				new GizmoEntry(78790, GizmoType.Chest, "trOut_wilderness_chest-9346", GizmoTargetTypes.Chest),
				new GizmoEntry(375540, GizmoType.Chest, "x1_Global_Chest_SpeedKill_Boss-881", GizmoTargetTypes.Chest),
				new GizmoEntry(137189, GizmoType.Chest, "DrownedTemple_Chest-26930", GizmoTargetTypes.Chest),
				new GizmoEntry(215512, GizmoType.Chest, "a1dun_Caves_Nephalem Altar_A_Chest_03_B-24663", GizmoTargetTypes.Chest),
				new GizmoEntry(215434, GizmoType.Chest, "a1dun_Caves_Nephalem Altar_A_Chest_03-26244", GizmoTargetTypes.Chest),

				//============== Floor Switches ==============//
				new GizmoEntry(5759, GizmoType.Chest, "trDun_Cath_FloorSpawner_02-670", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(116063, GizmoType.Chest, "a2dun_Spider_Ground_Spawner-13310", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(5758, GizmoType.Chest, "trDun_Cath_FloorSpawner_01-2966", GizmoTargetTypes.MiscContainer),

				//============== Corpse ==============//
				new GizmoEntry(187374, GizmoType.Chest, "LootType2_Adventurer_D_Corpse_01-17329", GizmoTargetTypes.Corpse),
				new GizmoEntry(187372, GizmoType.Chest, "LootType2_Adventurer_C_Corpse_01-17368", GizmoTargetTypes.Corpse),
				new GizmoEntry(187376, GizmoType.Chest, "LootType2_Adventurer_D_Corpse_02-18039", GizmoTargetTypes.Corpse),
				new GizmoEntry(187423, GizmoType.Chest, "LootType3_TristramGuard_Corpse_06-6737", GizmoTargetTypes.Corpse),
				new GizmoEntry(187369, GizmoType.Chest, "LootType2_Adventurer_A_Corpse_02-6854", GizmoTargetTypes.Corpse),
				new GizmoEntry(188119, GizmoType.Chest, "LootType2_FesteringSkeleton_A_Corpse-9853", GizmoTargetTypes.Corpse),
				new GizmoEntry(188120, GizmoType.Chest, "LootType2_FesteringSkeleton_B_Corpse-9851", GizmoTargetTypes.Corpse),
				new GizmoEntry(187435, GizmoType.Chest, "LootType2_tristramVillager_Male_B_Corpse_05-401", GizmoTargetTypes.Corpse),
				new GizmoEntry(187434, GizmoType.Chest, "LootType2_tristramVillager_Male_B_Corpse_03-554", GizmoTargetTypes.Corpse),
				new GizmoEntry(187371, GizmoType.Chest, "LootType2_Adventurer_B_Corpse_02-705", GizmoTargetTypes.Corpse),
				new GizmoEntry(187367, GizmoType.Chest, "LootType2_Adventurer_A_Corpse_01-1076", GizmoTargetTypes.Corpse),
				new GizmoEntry(187370, GizmoType.Chest, "LootType2_Adventurer_B_Corpse_01-19454", GizmoTargetTypes.Corpse),
				new GizmoEntry(187373, GizmoType.Chest, "LootType2_Adventurer_C_Corpse_02-20187", GizmoTargetTypes.Corpse),
				new GizmoEntry(187428, GizmoType.Chest, "LootType2_tristramVillager_Male_A_Corpse_01-4310", GizmoTargetTypes.Corpse),
				new GizmoEntry(187429, GizmoType.Chest, "LootType2_tristramVillager_Male_A_Corpse_02-5245", GizmoTargetTypes.Corpse),
				new GizmoEntry(187418, GizmoType.Chest, "LootType3_TristramGuard_Corpse_01-5268", GizmoTargetTypes.Corpse),
				new GizmoEntry(188128, GizmoType.Chest, "LootType3_CaldeumGuard_Cleaver_D_Corpse_05-14642", GizmoTargetTypes.Corpse),
				new GizmoEntry(188000, GizmoType.Chest, "TristramGuard_Corpse_02_DarkRitualEvent-697", GizmoTargetTypes.Corpse),
				new GizmoEntry(187438, GizmoType.Chest, "LootType2_tristramVillager_Male_C_Corpse_03-3097", GizmoTargetTypes.Corpse),
				new GizmoEntry(187432, GizmoType.Chest, "LootType2_tristramVillager_Male_B_Corpse_01-3187", GizmoTargetTypes.Corpse),
				new GizmoEntry(187431, GizmoType.Chest, "LootType2_tristramVillager_Male_A_Corpse_04-3196", GizmoTargetTypes.Corpse),
				new GizmoEntry(187436, GizmoType.Chest, "LootType2_tristramVillager_Male_C_Corpse_01-3508", GizmoTargetTypes.Corpse),
				new GizmoEntry(185159, GizmoType.Chest, "Cow_corpse_clickable_01-5683", GizmoTargetTypes.Corpse),
				new GizmoEntry(132551, GizmoType.Chest, "Blacksmith_Apprentice_Corpse-668", GizmoTargetTypes.Corpse),
				new GizmoEntry(139486, GizmoType.Chest, "trOut_TristramFields_DenofEvil_FallenShaman-11661", GizmoTargetTypes.Corpse),
				new GizmoEntry(77843, GizmoType.Chest, "Goatman_Tree_Knot_trOut_Goatmen-18023", GizmoTargetTypes.Corpse),
				new GizmoEntry(187437, GizmoType.Chest, "LootType2_tristramVillager_Male_C_Corpse_02-408", GizmoTargetTypes.Corpse),
				new GizmoEntry(187422, GizmoType.Chest, "LootType3_TristramGuard_Corpse_05-409", GizmoTargetTypes.Corpse),
				new GizmoEntry(187433, GizmoType.Chest, "LootType2_tristramVillager_Male_B_Corpse_02-427", GizmoTargetTypes.Corpse),
				new GizmoEntry(187439, GizmoType.Chest, "LootType2_tristramVillager_Male_C_Corpse_06-10499", GizmoTargetTypes.Corpse),
				new GizmoEntry(187419, GizmoType.Chest, "LootType3_TristramGuard_Corpse_02-540", GizmoTargetTypes.Corpse),
				new GizmoEntry(188228, GizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_02-705", GizmoTargetTypes.Corpse),
				new GizmoEntry(188232, GizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_06-717", GizmoTargetTypes.Corpse),
				new GizmoEntry(188133, GizmoType.Chest, "LootType3_CaldeumGuard_Spear_D_Corpse_03-10835", GizmoTargetTypes.Corpse),
				new GizmoEntry(187430, GizmoType.Chest, "LootType2_tristramVillager_Male_A_Corpse_03-9136", GizmoTargetTypes.Corpse),
				new GizmoEntry(225782, GizmoType.Chest, "trOut_TristramFields_DenofEvil_FallenShaman_Special-2084", GizmoTargetTypes.Corpse),
				new GizmoEntry(193023, GizmoType.Chest, "LootType3_GraveGuard_C_Corpse_03-4911", GizmoTargetTypes.Corpse),
				new GizmoEntry(187420, GizmoType.Chest, "LootType3_TristramGuard_Corpse_03-913", GizmoTargetTypes.Corpse),
				new GizmoEntry(188130, GizmoType.Chest, "LootType3_CaldeumGuard_Spear_D_Corpse_01-4575", GizmoTargetTypes.Corpse),
				new GizmoEntry(188131, GizmoType.Chest, "LootType3_CaldeumGuard_Spear_D_Corpse_02-7461", GizmoTargetTypes.Corpse),
				new GizmoEntry(188229, GizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_03-1579", GizmoTargetTypes.Corpse),
				new GizmoEntry(188230, GizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_04-2570", GizmoTargetTypes.Corpse),
				new GizmoEntry(188227, GizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_01-2760", GizmoTargetTypes.Corpse),
				new GizmoEntry(188231, GizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_05-3783", GizmoTargetTypes.Corpse),
				new GizmoEntry(193022, GizmoType.Chest, "LootType3_GraveGuard_C_Corpse_02-26949", GizmoTargetTypes.Corpse),
				new GizmoEntry(188367, GizmoType.Chest, "LootType2_caldeumTortured_Male_C_Corpse_05-14542", GizmoTargetTypes.Corpse),
				new GizmoEntry(193015, GizmoType.Chest, "LootType3_GraveGuard_B_Corpse_01-14680", GizmoTargetTypes.Corpse),
				new GizmoEntry(193025, GizmoType.Chest, "LootType3_GraveGuard_C_Corpse_05-14701", GizmoTargetTypes.Corpse),
				new GizmoEntry(174891, GizmoType.Chest, "Adventurer_A_Corpse_01_Chapel-28759", GizmoTargetTypes.Corpse),

				//============== Weapon Rack ==============//
				new GizmoEntry(464, GizmoType.Chest, "trDun_WeaponRack-791", GizmoTargetTypes.ItemRack),
				new GizmoEntry(58317, GizmoType.Chest, "a1dun_Leor_Tool_Rack_A_01-15551", GizmoTargetTypes.ItemRack),
				new GizmoEntry(77354, GizmoType.Chest, "Goatman_Weapon_Rack_trOut_Highlands-17316", GizmoTargetTypes.ItemRack),

				//============== Armor Rack ==============//
				new GizmoEntry(5671, GizmoType.Chest, "trDun_ArmorRack-1451", GizmoTargetTypes.ItemRack),

				//============== Misc Containers ==============//
				new GizmoEntry(5727, GizmoType.Chest, "trDun_Cath_BookcaseShelves_B-394", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(5724, GizmoType.Chest, "trDun_Cath_BookcaseShelves_A-1786", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(178151, GizmoType.Chest, "trOut_Highlands_Mystic_Wagon-4999", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(5673, GizmoType.Chest, "trDun_book_pile_a-822", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(5730, GizmoType.Chest, "trDun_Cath_BookcaseShelves_Wide-2385", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(219334, GizmoType.Chest, "a1dun_Crypts_Jar_of_Souls_02-6412", GizmoTargetTypes.MiscContainer),

				//============== Special Interactables ==============//
				new GizmoEntry(174754, GizmoType.Switch),
				new GizmoEntry(174753, GizmoType.Switch),
				new GizmoEntry(102079, GizmoType.Switch),
				new GizmoEntry(105754, GizmoType.Switch),
				new GizmoEntry(203608, GizmoType.Switch),
				new GizmoEntry(93713, GizmoType.Switch),
				new GizmoEntry(301177, GizmoType.Switch),
				new GizmoEntry(102927, GizmoType.Switch, "Ghost_Jail_Prisoner"), //Warden Skeleton Remains
				new GizmoEntry(213490, GizmoType.Switch, "a2dun_Spider_Venom_Pool-4568"),

				new GizmoEntry(138989, GizmoType.HealingWell, "", GizmoTargetTypes.Healthwell),

				new GizmoEntry(176075, GizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(176077, GizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(176074, GizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(176076, GizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(260331, GizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(260330, GizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(330697, GizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(330699, GizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(330698, GizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(330695, GizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(330696, GizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
			};
		}

		public void ClearCollections()
		{
			GizmoCache.Clear();
			GizmoStringCache.Clear();
		}

		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "External", "Dictionaries", "Cache_Gizmos.xml");
		internal static GizmoDataCollection DeserializeFromXML()
		{
			var deserializer = new XmlSerializer(typeof(GizmoDataCollection));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (GizmoDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		internal static void SerializeToXML(GizmoDataCollection settings)
		{
			var serializer = new XmlSerializer(typeof(GizmoDataCollection));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
	}
}
