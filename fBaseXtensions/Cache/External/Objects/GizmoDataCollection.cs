using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Helpers;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.External.Objects
{
	public class GizmoDataCollection
	{
		//GizmoType: Door Name: X1_Westm_Door_Giant_Lowering_Wolf-9346 ActorSNO: 308241 Distance: 29.25269 Position: <596.4537, 1140.521, 0.1612549> Barracade: False Radius: 21.95018

		public HashSet<GizmoEntry> GizmoCache { get; set; }
		public HashSet<GizmoStringEntry> GizmoStringCache { get; set; }
		public HashSet<int> ResplendantChests { get; set; }
 

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
				new GizmoEntry(62873, GizmoType.Chest),
				new GizmoEntry(95011, GizmoType.Chest),
				new GizmoEntry(81424, GizmoType.Chest),
				new GizmoEntry(108230, GizmoType.Chest),
				new GizmoEntry(111808, GizmoType.Chest),
				new GizmoEntry(111809, GizmoType.Chest),
				new GizmoEntry(199583, GizmoType.Chest),
				new GizmoEntry(109264, GizmoType.Chest),
				new GizmoEntry(101500, GizmoType.Chest),
				new GizmoEntry(96993, GizmoType.Chest),
				new GizmoEntry(62866, GizmoType.Chest),
				new GizmoEntry(108230, GizmoType.Chest),
				new GizmoEntry(211861, GizmoType.Chest),
				new GizmoEntry(62860, GizmoType.Chest),
				new GizmoEntry(96993, GizmoType.Chest),
				new GizmoEntry(112182, GizmoType.Chest),
				new GizmoEntry(363725, GizmoType.Chest),

				//============== Chests ==============//
				new GizmoEntry(62865, GizmoType.Chest, "TrOut_Highlands_Chest-18282"),
				new GizmoEntry(62859, GizmoType.Chest, "TrOut_Fields_Chest-9087"),
				new GizmoEntry(79319, GizmoType.Chest, "trOut_Highlands_chest_Bloody-9602"),
				new GizmoEntry(94708, GizmoType.Chest, "a1dun_Leor_Chest-14472"),
				new GizmoEntry(108792, GizmoType.Chest, "trOut_Wilderness_HangingTree_GraveChest-1883"),
				new GizmoEntry(5822, GizmoType.Chest, "trDun_Crypt_Chest_01-2841"),
				new GizmoEntry(70534, GizmoType.Chest, "a2dun_Spider_Chest-7959"),
				new GizmoEntry(96522, GizmoType.Chest, "a1dun_Cath_chest-3168"),
				new GizmoEntry(364559, GizmoType.Chest, "x1_Global_Chest_CursedChest-1801"),
				new GizmoEntry(357331, GizmoType.Chest, "x1_Global_Chest-2959"),
				new GizmoEntry(375539, GizmoType.Chest, "x1_Global_Chest_SpeedKill_Elite-8331"),

				//============== Floor Switches ==============//
				new GizmoEntry(5759, GizmoType.Chest, "trDun_Cath_FloorSpawner_02-670"),
				new GizmoEntry(116063, GizmoType.Chest, "a2dun_Spider_Ground_Spawner-13310"),
				new GizmoEntry(5758, GizmoType.Chest, "trDun_Cath_FloorSpawner_01-2966"),

				//============== Corpse ==============//
				new GizmoEntry(187374, GizmoType.Chest, "LootType2_Adventurer_D_Corpse_01-17329"),
				new GizmoEntry(187372, GizmoType.Chest, "LootType2_Adventurer_C_Corpse_01-17368"),
				new GizmoEntry(187376, GizmoType.Chest, "LootType2_Adventurer_D_Corpse_02-18039"),
				new GizmoEntry(187423, GizmoType.Chest, "LootType3_TristramGuard_Corpse_06-6737"),
				new GizmoEntry(187369, GizmoType.Chest, "LootType2_Adventurer_A_Corpse_02-6854"),
				new GizmoEntry(188119, GizmoType.Chest, "LootType2_FesteringSkeleton_A_Corpse-9853"),
				new GizmoEntry(188120, GizmoType.Chest, "LootType2_FesteringSkeleton_B_Corpse-9851"),
				new GizmoEntry(187435, GizmoType.Chest, "LootType2_tristramVillager_Male_B_Corpse_05-401"),
				new GizmoEntry(187434, GizmoType.Chest, "LootType2_tristramVillager_Male_B_Corpse_03-554"),
				new GizmoEntry(187371, GizmoType.Chest, "LootType2_Adventurer_B_Corpse_02-705"),
				new GizmoEntry(187367, GizmoType.Chest, "LootType2_Adventurer_A_Corpse_01-1076"),
				new GizmoEntry(187370, GizmoType.Chest, "LootType2_Adventurer_B_Corpse_01-19454"),
				new GizmoEntry(187373, GizmoType.Chest, "LootType2_Adventurer_C_Corpse_02-20187"),
				new GizmoEntry(187428, GizmoType.Chest, "LootType2_tristramVillager_Male_A_Corpse_01-4310"),
				new GizmoEntry(187429, GizmoType.Chest, "LootType2_tristramVillager_Male_A_Corpse_02-5245"),
				new GizmoEntry(187418, GizmoType.Chest, "LootType3_TristramGuard_Corpse_01-5268"),
				new GizmoEntry(188128, GizmoType.Chest, "LootType3_CaldeumGuard_Cleaver_D_Corpse_05-14642"),
				new GizmoEntry(188000, GizmoType.Chest, "TristramGuard_Corpse_02_DarkRitualEvent-697"),
				new GizmoEntry(187438, GizmoType.Chest, "LootType2_tristramVillager_Male_C_Corpse_03-3097"),
				new GizmoEntry(187432, GizmoType.Chest, "LootType2_tristramVillager_Male_B_Corpse_01-3187"),
				new GizmoEntry(187431, GizmoType.Chest, "LootType2_tristramVillager_Male_A_Corpse_04-3196"),
				new GizmoEntry(187436, GizmoType.Chest, "LootType2_tristramVillager_Male_C_Corpse_01-3508"),
				new GizmoEntry(185159, GizmoType.Chest, "Cow_corpse_clickable_01-5683"),
				new GizmoEntry(132551, GizmoType.Chest, "Blacksmith_Apprentice_Corpse-668"),
				new GizmoEntry(139486, GizmoType.Chest, "trOut_TristramFields_DenofEvil_FallenShaman-11661"),
				new GizmoEntry(77843, GizmoType.Chest, "Goatman_Tree_Knot_trOut_Goatmen-18023"),

				//============== Weapon Rack ==============//
				new GizmoEntry(464, GizmoType.Chest, "trDun_WeaponRack-791"),
				new GizmoEntry(58317, GizmoType.Chest, "a1dun_Leor_Tool_Rack_A_01-15551"),
				new GizmoEntry(77354, GizmoType.Chest, "Goatman_Weapon_Rack_trOut_Highlands-17316"),

				//============== Armor Rack ==============//
				new GizmoEntry(5671, GizmoType.Chest, "trDun_ArmorRack-1451"),

				//============== Misc Containers ==============//
				new GizmoEntry(5727, GizmoType.Chest, "trDun_Cath_BookcaseShelves_B-394"),
				new GizmoEntry(5724, GizmoType.Chest, "trDun_Cath_BookcaseShelves_A-1786"),
				new GizmoEntry(178151, GizmoType.Chest, "trOut_Highlands_Mystic_Wagon-4999"),
				new GizmoEntry(5673, GizmoType.Chest, "trDun_book_pile_a-822"),

				//============== Special Interactables ==============//
				new GizmoEntry(174754, GizmoType.Switch),
				new GizmoEntry(174753, GizmoType.Switch),
				new GizmoEntry(102079, GizmoType.Switch),
				new GizmoEntry(105754, GizmoType.Switch),
				new GizmoEntry(203608, GizmoType.Switch),
				new GizmoEntry(93713, GizmoType.Switch),
				new GizmoEntry(301177, GizmoType.Switch),
			};
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
