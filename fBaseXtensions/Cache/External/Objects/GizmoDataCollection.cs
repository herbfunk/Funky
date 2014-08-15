using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Enums;
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
				new GizmoStringEntry("door_destructable", PluginGizmoType.BreakableDoor),
				new GizmoStringEntry("cart_a_breakable", PluginGizmoType.DestroyableObject),
				new GizmoStringEntry("breakables", PluginGizmoType.DestroyableObject),
				new GizmoStringEntry("chest", PluginGizmoType.Chest),
				new GizmoStringEntry("loottype", PluginGizmoType.Chest),
				new GizmoStringEntry("corpse", PluginGizmoType.Chest),
				new GizmoStringEntry("weaponrack", PluginGizmoType.Chest),
				new GizmoStringEntry("armorrack", PluginGizmoType.Chest),
				new GizmoStringEntry("corpse", PluginGizmoType.Chest),
			};

			GizmoCache = new HashSet<GizmoEntry>
			{
				//============== DOORS ==============//
				new GizmoEntry(5854, PluginGizmoType.Door),
				new GizmoEntry(454, PluginGizmoType.Door),
				new GizmoEntry(5767, PluginGizmoType.Door),
				new GizmoEntry(230324, PluginGizmoType.Door),
				new GizmoEntry(308241, PluginGizmoType.Door),
				new GizmoEntry(273323, PluginGizmoType.Door),
				new GizmoEntry(308241, PluginGizmoType.Door, "X1_Westm_Door_Giant_Lowering_Wolf-9346"),
				new GizmoEntry(308376, PluginGizmoType.Door, "X1_Westm_Door_Giant_Iron_Bars_Arched-18634"),
				new GizmoEntry(90419, PluginGizmoType.Door, "trOut_NewTristram_Gate_Town-67"),
				new GizmoEntry(309812, PluginGizmoType.Door, "X1_Westm_Door_Giant_Iron-5188"),
				new GizmoEntry(152772, PluginGizmoType.Door, "caOut_Oasis_aqd_door-8258"),
				new GizmoEntry(258595, PluginGizmoType.Door, "x1_Catacombs_Door_A-8887"),
				new GizmoEntry(5763, PluginGizmoType.Door, "trDun_Cath_Gate_A-93193"),
				new GizmoEntry(5766, PluginGizmoType.Door, "trDun_Cath_Gate_C-9372"),
				new GizmoEntry(105361, PluginGizmoType.Door, "a1dun_Leor_Gate_A-17181"),
				new GizmoEntry(95571, PluginGizmoType.Door, "a1dun_Leor_Jail_Door_A-10528"),
				new GizmoEntry(5765, PluginGizmoType.Door, "trDun_Cath_Gate_B_SkeletonKing-3656"),
				new GizmoEntry(104888, PluginGizmoType.Door, "a1dunLeor_Interactive_Wooden_Door_A-6720"),
				new GizmoEntry(74476, PluginGizmoType.Door, "CellarDoor_trOut_TristramField_Door-7824"),
				new GizmoEntry(168804, PluginGizmoType.Door, "trDun_Cath_Gate_A_StrangerGate-20884"),
				new GizmoEntry(89665, PluginGizmoType.Door, "trOut_Highlands_Manor_Cain_Door-9032"),
				new GizmoEntry(99304, PluginGizmoType.Door, "a1dun_Leor_Manor_DeathofCain_Door-8987"),
				new GizmoEntry(177439, PluginGizmoType.Door, "a1dun_caves_DrownedTemple_WallDoor-26916"),
				new GizmoEntry(170913, PluginGizmoType.Door, "trOut_TristramField_Field_Gate-23591"),
				new GizmoEntry(100862, PluginGizmoType.Door, "a1dun_Leor_Jail_Door_A_Exit-18422"),
				new GizmoEntry(167289, PluginGizmoType.Door, "trdun_Cath_CathedralDoorExterior-4148"),

				//============== Resplendant Chests ==============//
				new GizmoEntry(62873, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(95011, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(81424, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(108230, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(111808, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(111809, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(199583, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(109264, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(101500, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(96993, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(62866, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(108230, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(211861, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(62860, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(96993, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(112182, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(363725, PluginGizmoType.Chest, "", GizmoTargetTypes.Resplendant),
				new GizmoEntry(357509, PluginGizmoType.Chest, "a1dun_Leor_Chest_Rare_Garrach-16708", GizmoTargetTypes.Resplendant),
				new GizmoEntry(289248, PluginGizmoType.Chest, "x1_Westm_Chest_Rare-9700", GizmoTargetTypes.Resplendant),
				new GizmoEntry(179866, PluginGizmoType.Chest, "a3dun_Crater_ST_Chest_Rare-15899", GizmoTargetTypes.Resplendant),

				//============== Chests ==============//
				new GizmoEntry(62865, PluginGizmoType.Chest, "TrOut_Highlands_Chest-18282", GizmoTargetTypes.Chest),
				new GizmoEntry(62859, PluginGizmoType.Chest, "TrOut_Fields_Chest-9087", GizmoTargetTypes.Chest),
				new GizmoEntry(79319, PluginGizmoType.Chest, "trOut_Highlands_chest_Bloody-9602", GizmoTargetTypes.Chest),
				new GizmoEntry(94708, PluginGizmoType.Chest, "a1dun_Leor_Chest-14472", GizmoTargetTypes.Chest),
				new GizmoEntry(108792, PluginGizmoType.Chest, "trOut_Wilderness_HangingTree_GraveChest-1883", GizmoTargetTypes.Chest),
				new GizmoEntry(5822, PluginGizmoType.Chest, "trDun_Crypt_Chest_01-2841", GizmoTargetTypes.Chest),
				new GizmoEntry(70534, PluginGizmoType.Chest, "a2dun_Spider_Chest-7959", GizmoTargetTypes.Chest),
				new GizmoEntry(96522, PluginGizmoType.Chest, "a1dun_Cath_chest-3168", GizmoTargetTypes.Chest),
				new GizmoEntry(364559, PluginGizmoType.Chest, "x1_Global_Chest_CursedChest-1801", GizmoTargetTypes.Chest),
				new GizmoEntry(357331, PluginGizmoType.Chest, "x1_Global_Chest-2959", GizmoTargetTypes.Chest),
				new GizmoEntry(375539, PluginGizmoType.Chest, "x1_Global_Chest_SpeedKill_Elite-8331", GizmoTargetTypes.Chest),
				new GizmoEntry(78790, PluginGizmoType.Chest, "trOut_wilderness_chest-9346", GizmoTargetTypes.Chest),
				new GizmoEntry(375540, PluginGizmoType.Chest, "x1_Global_Chest_SpeedKill_Boss-881", GizmoTargetTypes.Chest),
				new GizmoEntry(137189, PluginGizmoType.Chest, "DrownedTemple_Chest-26930", GizmoTargetTypes.Chest),
				new GizmoEntry(215512, PluginGizmoType.Chest, "a1dun_Caves_Nephalem Altar_A_Chest_03_B-24663", GizmoTargetTypes.Chest),
				new GizmoEntry(215434, PluginGizmoType.Chest, "a1dun_Caves_Nephalem Altar_A_Chest_03-26244", GizmoTargetTypes.Chest),
				new GizmoEntry(111870, PluginGizmoType.Chest, "A3_Battlefield_Chest_Snowy-5333", GizmoTargetTypes.Chest),
				new GizmoEntry(82796, PluginGizmoType.Chest, "a2dun_Aqd_Chest-6018", GizmoTargetTypes.Chest),
				new GizmoEntry(197642, PluginGizmoType.Chest, "a2dun_Swr_Chest-11659", GizmoTargetTypes.Chest),
				new GizmoEntry(182309, PluginGizmoType.Chest, "A4dun_Garden_Chest-3815", GizmoTargetTypes.Chest),
				new GizmoEntry(99892, PluginGizmoType.Chest, "caOut_Boneyards_chest-7552", GizmoTargetTypes.Chest),
				new GizmoEntry(187106, PluginGizmoType.Chest, "A4dun_Spire_Chest-2062", GizmoTargetTypes.Chest),
				new GizmoEntry(289247, PluginGizmoType.Chest, "x1_Westm_Chest-13789", GizmoTargetTypes.Chest),
				new GizmoEntry(289786, PluginGizmoType.Chest, "x1_westm_Graveyard_Chest-20848", GizmoTargetTypes.Chest),
				new GizmoEntry(289757, PluginGizmoType.Chest, "x1_Abattoir_Chest-39194", GizmoTargetTypes.Chest),
				new GizmoEntry(289807, PluginGizmoType.Chest, "x1_Pand_HexMaze_Chest-12836", GizmoTargetTypes.Chest),
				new GizmoEntry(179865, PluginGizmoType.Chest, "a3dun_Crater_ST_Chest-8347", GizmoTargetTypes.Chest),
				new GizmoEntry(289796, PluginGizmoType.Chest, "X1_PandExt_Chest-1937", GizmoTargetTypes.Chest),
				new GizmoEntry(260404, PluginGizmoType.Chest, "x1_Bog_Chest-23698", GizmoTargetTypes.Chest),
				new GizmoEntry(249360, PluginGizmoType.Chest, "x1_BogCave_Chest-2649", GizmoTargetTypes.Chest),
				new GizmoEntry(130170, PluginGizmoType.Chest, "a3dun_Crater_Chest-8191", GizmoTargetTypes.Chest),
				new GizmoEntry(62872, PluginGizmoType.Chest, "CaOut_Oasis_Chest-2327", GizmoTargetTypes.Chest),
				new GizmoEntry(2976, PluginGizmoType.Chest, "a2dun_Zolt_Chest-1553", GizmoTargetTypes.Chest),
				new GizmoEntry(261135, PluginGizmoType.Chest, "x1_Catacombs_chest-11622", GizmoTargetTypes.Chest),
				new GizmoEntry(51300, PluginGizmoType.Chest, "a3dun_Keep_Chest_A-8177", GizmoTargetTypes.Chest),
				new GizmoEntry(111947, PluginGizmoType.Chest, "A3_Battlefield_Chest_Frosty-1287", GizmoTargetTypes.Chest),

				//============== Floor Switches ==============//
				new GizmoEntry(5759, PluginGizmoType.Chest, "trDun_Cath_FloorSpawner_02-670", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(116063, PluginGizmoType.Chest, "a2dun_Spider_Ground_Spawner-13310", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(5758, PluginGizmoType.Chest, "trDun_Cath_FloorSpawner_01-2966", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(261139, PluginGizmoType.Chest, "x1_Catacombs_Ground_Clicky-11367", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(289762, PluginGizmoType.Chest, "x1_Abattoir_Ground_Clicky-10782", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(251867, PluginGizmoType.Chest, "x1_Bog_Ground_Clicky-5500", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(289784, PluginGizmoType.Chest, "x1_westm_Graveyard_Ground_Clicky-14846", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(285382, PluginGizmoType.Chest, "x1_Pand_Ext_Ground_Clicky-1947", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(289805, PluginGizmoType.Chest, "x1_Pand_HexMaze_Ground_Clicky-12403", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(289250, PluginGizmoType.Chest, "x1_Westm_Ground_Clicky-14727", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(249365, PluginGizmoType.Chest, "x1_BogCave_GroundTile-15025", GizmoTargetTypes.MiscContainer),

				//============== Corpse ==============//
				new GizmoEntry(187374, PluginGizmoType.Chest, "LootType2_Adventurer_D_Corpse_01-17329", GizmoTargetTypes.Corpse),
				new GizmoEntry(187372, PluginGizmoType.Chest, "LootType2_Adventurer_C_Corpse_01-17368", GizmoTargetTypes.Corpse),
				new GizmoEntry(187376, PluginGizmoType.Chest, "LootType2_Adventurer_D_Corpse_02-18039", GizmoTargetTypes.Corpse),
				new GizmoEntry(187423, PluginGizmoType.Chest, "LootType3_TristramGuard_Corpse_06-6737", GizmoTargetTypes.Corpse),
				new GizmoEntry(187369, PluginGizmoType.Chest, "LootType2_Adventurer_A_Corpse_02-6854", GizmoTargetTypes.Corpse),
				new GizmoEntry(188119, PluginGizmoType.Chest, "LootType2_FesteringSkeleton_A_Corpse-9853", GizmoTargetTypes.Corpse),
				new GizmoEntry(188120, PluginGizmoType.Chest, "LootType2_FesteringSkeleton_B_Corpse-9851", GizmoTargetTypes.Corpse),
				new GizmoEntry(187435, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_B_Corpse_05-401", GizmoTargetTypes.Corpse),
				new GizmoEntry(187434, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_B_Corpse_03-554", GizmoTargetTypes.Corpse),
				new GizmoEntry(187371, PluginGizmoType.Chest, "LootType2_Adventurer_B_Corpse_02-705", GizmoTargetTypes.Corpse),
				new GizmoEntry(187367, PluginGizmoType.Chest, "LootType2_Adventurer_A_Corpse_01-1076", GizmoTargetTypes.Corpse),
				new GizmoEntry(187370, PluginGizmoType.Chest, "LootType2_Adventurer_B_Corpse_01-19454", GizmoTargetTypes.Corpse),
				new GizmoEntry(187373, PluginGizmoType.Chest, "LootType2_Adventurer_C_Corpse_02-20187", GizmoTargetTypes.Corpse),
				new GizmoEntry(187428, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_A_Corpse_01-4310", GizmoTargetTypes.Corpse),
				new GizmoEntry(187429, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_A_Corpse_02-5245", GizmoTargetTypes.Corpse),
				new GizmoEntry(187418, PluginGizmoType.Chest, "LootType3_TristramGuard_Corpse_01-5268", GizmoTargetTypes.Corpse),
				new GizmoEntry(188128, PluginGizmoType.Chest, "LootType3_CaldeumGuard_Cleaver_D_Corpse_05-14642", GizmoTargetTypes.Corpse),
				new GizmoEntry(188000, PluginGizmoType.Chest, "TristramGuard_Corpse_02_DarkRitualEvent-697", GizmoTargetTypes.Corpse),
				new GizmoEntry(187438, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_C_Corpse_03-3097", GizmoTargetTypes.Corpse),
				new GizmoEntry(187432, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_B_Corpse_01-3187", GizmoTargetTypes.Corpse),
				new GizmoEntry(187431, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_A_Corpse_04-3196", GizmoTargetTypes.Corpse),
				new GizmoEntry(187436, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_C_Corpse_01-3508", GizmoTargetTypes.Corpse),
				new GizmoEntry(185159, PluginGizmoType.Chest, "Cow_corpse_clickable_01-5683", GizmoTargetTypes.Corpse),
				new GizmoEntry(132551, PluginGizmoType.Chest, "Blacksmith_Apprentice_Corpse-668", GizmoTargetTypes.Corpse),
				new GizmoEntry(139486, PluginGizmoType.Chest, "trOut_TristramFields_DenofEvil_FallenShaman-11661", GizmoTargetTypes.Corpse),
				new GizmoEntry(77843, PluginGizmoType.Chest, "Goatman_Tree_Knot_trOut_Goatmen-18023", GizmoTargetTypes.Corpse),
				new GizmoEntry(187437, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_C_Corpse_02-408", GizmoTargetTypes.Corpse),
				new GizmoEntry(187422, PluginGizmoType.Chest, "LootType3_TristramGuard_Corpse_05-409", GizmoTargetTypes.Corpse),
				new GizmoEntry(187433, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_B_Corpse_02-427", GizmoTargetTypes.Corpse),
				new GizmoEntry(187439, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_C_Corpse_06-10499", GizmoTargetTypes.Corpse),
				new GizmoEntry(187419, PluginGizmoType.Chest, "LootType3_TristramGuard_Corpse_02-540", GizmoTargetTypes.Corpse),
				new GizmoEntry(188228, PluginGizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_02-705", GizmoTargetTypes.Corpse),
				new GizmoEntry(188232, PluginGizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_06-717", GizmoTargetTypes.Corpse),
				new GizmoEntry(188133, PluginGizmoType.Chest, "LootType3_CaldeumGuard_Spear_D_Corpse_03-10835", GizmoTargetTypes.Corpse),
				new GizmoEntry(187430, PluginGizmoType.Chest, "LootType2_tristramVillager_Male_A_Corpse_03-9136", GizmoTargetTypes.Corpse),
				new GizmoEntry(225782, PluginGizmoType.Chest, "trOut_TristramFields_DenofEvil_FallenShaman_Special-2084", GizmoTargetTypes.Corpse),
				new GizmoEntry(193023, PluginGizmoType.Chest, "LootType3_GraveGuard_C_Corpse_03-4911", GizmoTargetTypes.Corpse),
				new GizmoEntry(187420, PluginGizmoType.Chest, "LootType3_TristramGuard_Corpse_03-913", GizmoTargetTypes.Corpse),
				new GizmoEntry(188130, PluginGizmoType.Chest, "LootType3_CaldeumGuard_Spear_D_Corpse_01-4575", GizmoTargetTypes.Corpse),
				new GizmoEntry(188131, PluginGizmoType.Chest, "LootType3_CaldeumGuard_Spear_D_Corpse_02-7461", GizmoTargetTypes.Corpse),
				new GizmoEntry(188229, PluginGizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_03-1579", GizmoTargetTypes.Corpse),
				new GizmoEntry(188230, PluginGizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_04-2570", GizmoTargetTypes.Corpse),
				new GizmoEntry(188227, PluginGizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_01-2760", GizmoTargetTypes.Corpse),
				new GizmoEntry(188231, PluginGizmoType.Chest, "LootType2_OldTristram_Guard_Corpse_05-3783", GizmoTargetTypes.Corpse),
				new GizmoEntry(193022, PluginGizmoType.Chest, "LootType3_GraveGuard_C_Corpse_02-26949", GizmoTargetTypes.Corpse),
				new GizmoEntry(188367, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_C_Corpse_05-14542", GizmoTargetTypes.Corpse),
				new GizmoEntry(193015, PluginGizmoType.Chest, "LootType3_GraveGuard_B_Corpse_01-14680", GizmoTargetTypes.Corpse),
				new GizmoEntry(193025, PluginGizmoType.Chest, "LootType3_GraveGuard_C_Corpse_05-14701", GizmoTargetTypes.Corpse),
				new GizmoEntry(174891, PluginGizmoType.Chest, "Adventurer_A_Corpse_01_Chapel-28759", GizmoTargetTypes.Corpse),
				new GizmoEntry(200232, PluginGizmoType.Chest, "LootType2_BastionsKeepGuard_Frosty_Ranged_Corpse_04-6095", GizmoTargetTypes.Corpse),
				new GizmoEntry(200229, PluginGizmoType.Chest, "LootType2_BastionsKeepGuard_Frosty_Ranged_Corpse_02-6882", GizmoTargetTypes.Corpse),
				new GizmoEntry(200223, PluginGizmoType.Chest, "LootType2_BastionsKeepGuard_Frosty_Melee_Corpse_03-10469", GizmoTargetTypes.Corpse),
				new GizmoEntry(200230, PluginGizmoType.Chest, "LootType2_BastionsKeepGuard_Frosty_Ranged_Corpse_03-10770", GizmoTargetTypes.Corpse),
				new GizmoEntry(200225, PluginGizmoType.Chest, "LootType2_BastionsKeepGuard_Frosty_Melee_Corpse_04-13158", GizmoTargetTypes.Corpse),
				new GizmoEntry(200227, PluginGizmoType.Chest, "LootType2_BastionsKeepGuard_Frosty_Melee_Corpse_06-15737", GizmoTargetTypes.Corpse),
				new GizmoEntry(193029, PluginGizmoType.Chest, "LootType3_GraveGuard_D_Corpse_02-588", GizmoTargetTypes.Corpse),
				new GizmoEntry(188392, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_G_Corpse_01-834", GizmoTargetTypes.Corpse),
				new GizmoEntry(188370, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_D_Corpse_02-2152", GizmoTargetTypes.Corpse),
				new GizmoEntry(188372, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_D_Corpse_04-14885", GizmoTargetTypes.Corpse),
				new GizmoEntry(193030, PluginGizmoType.Chest, "LootType3_GraveGuard_D_Corpse_03-16055", GizmoTargetTypes.Corpse),
				new GizmoEntry(309397, PluginGizmoType.Chest, "x1_Westm_Corpse_C_05-2331", GizmoTargetTypes.Corpse),
				new GizmoEntry(309385, PluginGizmoType.Chest, "x1_Westm_Corpse_A_03-2823", GizmoTargetTypes.Corpse),
				new GizmoEntry(187388, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Ranged_Corpse_05-1634", GizmoTargetTypes.Corpse),
				new GizmoEntry(187382, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Melee_Corpse_06-1711", GizmoTargetTypes.Corpse),
				new GizmoEntry(187384, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Ranged_Corpse_01-2175", GizmoTargetTypes.Corpse),
				new GizmoEntry(187386, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Ranged_Corpse_03-3458", GizmoTargetTypes.Corpse),
				new GizmoEntry(187381, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Melee_Corpse_05-3984", GizmoTargetTypes.Corpse),
				new GizmoEntry(187380, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Melee_Corpse_04-6826", GizmoTargetTypes.Corpse),
				new GizmoEntry(187379, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Melee_Corpse_03-13521", GizmoTargetTypes.Corpse),
				new GizmoEntry(187389, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Ranged_Corpse_06-7714", GizmoTargetTypes.Corpse),
				new GizmoEntry(187385, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Ranged_Corpse_02-7698", GizmoTargetTypes.Corpse),
				new GizmoEntry(187378, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Melee_Corpse_02-7747", GizmoTargetTypes.Corpse),
				new GizmoEntry(187377, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Melee_Corpse_01-8150", GizmoTargetTypes.Corpse),
				new GizmoEntry(188366, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_C_Corpse_04-811", GizmoTargetTypes.Corpse),
				new GizmoEntry(188352, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_A_Corpse_02-1499", GizmoTargetTypes.Corpse),
				new GizmoEntry(190275, PluginGizmoType.Chest, "LootType3_CaldeumGuard_Cleaver_A_Corpse_04-20366", GizmoTargetTypes.Corpse),
				new GizmoEntry(190277, PluginGizmoType.Chest, "LootType3_CaldeumGuard_Cleaver_A_Corpse_06-19176", GizmoTargetTypes.Corpse),
				new GizmoEntry(187392, PluginGizmoType.Chest, "LootType3_CaldeumGuard_Cleaver_A_Corpse_03-14052", GizmoTargetTypes.Corpse),
				new GizmoEntry(187391, PluginGizmoType.Chest, "LootType3_CaldeumGuard_Cleaver_A_Corpse_02-2545", GizmoTargetTypes.Corpse),
				new GizmoEntry(187390, PluginGizmoType.Chest, "LootType3_CaldeumGuard_Cleaver_A_Corpse_01-6750", GizmoTargetTypes.Corpse),
				new GizmoEntry(331189, PluginGizmoType.Chest, "x1_Abattoir_Corpse_B-10256", GizmoTargetTypes.Corpse),
				new GizmoEntry(331190, PluginGizmoType.Chest, "x1_Abattoir_Corpse_C-10258", GizmoTargetTypes.Corpse),
				new GizmoEntry(331188, PluginGizmoType.Chest, "x1_Abattoir_Corpse_A-10264", GizmoTargetTypes.Corpse),
				new GizmoEntry(187421, PluginGizmoType.Chest, "LootType3_TristramGuard_Corpse_04-1613", GizmoTargetTypes.Corpse),
				new GizmoEntry(188369, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_D_Corpse_01-5423", GizmoTargetTypes.Corpse),
				new GizmoEntry(249388, PluginGizmoType.Chest, "x1_BogCave_Corpse-859", GizmoTargetTypes.Corpse),
				new GizmoEntry(362668, PluginGizmoType.Chest, "x1_Bog_Bogit_Corpse-5507", GizmoTargetTypes.Corpse),
				new GizmoEntry(289804, PluginGizmoType.Chest, "x1_Pand_HexMaze_Corpse-12718", GizmoTargetTypes.Corpse),
				new GizmoEntry(187387, PluginGizmoType.Chest, "LootType3_BastionsKeepGuard_Ranged_Corpse_04-57725", GizmoTargetTypes.Corpse),
				new GizmoEntry(309398, PluginGizmoType.Chest, "x1_Westm_Corpse_C_06-8349", GizmoTargetTypes.Corpse),
				new GizmoEntry(359196, PluginGizmoType.Chest, "x1_Adventurer_Female_Corpse_C_01-10315", GizmoTargetTypes.Corpse),
				new GizmoEntry(309387, PluginGizmoType.Chest, "x1_Westm_Corpse_A_05-12778", GizmoTargetTypes.Corpse),
				new GizmoEntry(309392, PluginGizmoType.Chest, "x1_Westm_Corpse_B_05-13666", GizmoTargetTypes.Corpse),
				new GizmoEntry(309391, PluginGizmoType.Chest, "x1_Westm_Corpse_B_04-13753", GizmoTargetTypes.Corpse),
				new GizmoEntry(309408, PluginGizmoType.Chest, "x1_Westm_Corpse_E_04-14683", GizmoTargetTypes.Corpse),
				new GizmoEntry(359192, PluginGizmoType.Chest, "x1_Adventurer_Female_Corpse_C_03-12283", GizmoTargetTypes.Corpse),
				new GizmoEntry(309402, PluginGizmoType.Chest, "x1_Westm_Corpse_D_04-13365", GizmoTargetTypes.Corpse),
				new GizmoEntry(309381, PluginGizmoType.Chest, "x1_Westm_Corpse_C_01-14849", GizmoTargetTypes.Corpse),
				new GizmoEntry(312137, PluginGizmoType.Chest, "x1_westmarchGuard_Melee_Corpse_03-15606", GizmoTargetTypes.Corpse),
				new GizmoEntry(309380, PluginGizmoType.Chest, "x1_Westm_Corpse_B_01-16010", GizmoTargetTypes.Corpse),
				new GizmoEntry(359197, PluginGizmoType.Chest, "x1_Adventurer_Female_Corpse_D_03-3091", GizmoTargetTypes.Corpse),
				new GizmoEntry(200250, PluginGizmoType.Chest, "Clicky_LootType2_DemonFlyer_B_Frosty_Corpse_01-15526", GizmoTargetTypes.Corpse),
				new GizmoEntry(289783, PluginGizmoType.Chest, "x1_westm_Graveyard_Corpse-14914", GizmoTargetTypes.Corpse),
				new GizmoEntry(261137, PluginGizmoType.Chest, "x1_Catacombs_Corpse_01-11441", GizmoTargetTypes.Corpse),

				//============== Weapon Rack ==============//
				new GizmoEntry(464, PluginGizmoType.Chest, "trDun_WeaponRack-791", GizmoTargetTypes.ItemRack),
				new GizmoEntry(58317, PluginGizmoType.Chest, "a1dun_Leor_Tool_Rack_A_01-15551", GizmoTargetTypes.ItemRack),
				new GizmoEntry(77354, PluginGizmoType.Chest, "Goatman_Weapon_Rack_trOut_Highlands-17316", GizmoTargetTypes.ItemRack),
				new GizmoEntry(192466, PluginGizmoType.Chest, "A3_Battlefield_Weaponrack_A-16802", GizmoTargetTypes.ItemRack),
				new GizmoEntry(198012, PluginGizmoType.Chest, "a2dun_zolt_WeaponRack_A-18302", GizmoTargetTypes.ItemRack),
				new GizmoEntry(289246, PluginGizmoType.Chest, "x1_Westm_weaponRack-9780", GizmoTargetTypes.ItemRack),
				new GizmoEntry(289763, PluginGizmoType.Chest, "x1_Abattoir_weaponRack-1181", GizmoTargetTypes.ItemRack),
				new GizmoEntry(167520, PluginGizmoType.Chest, "a4dunGarden_Props_Weaponrack_A-8221", GizmoTargetTypes.ItemRack),

				//============== Armor Rack ==============//
				new GizmoEntry(5671, PluginGizmoType.Chest, "trDun_ArmorRack-1451", GizmoTargetTypes.ItemRack),
				new GizmoEntry(289794, PluginGizmoType.Chest, "X1_PandExt_ArmorRack-12750", GizmoTargetTypes.ItemRack),
				new GizmoEntry(340114, PluginGizmoType.Chest, "x1_Bog_ArmorRack_A-15606", GizmoTargetTypes.ItemRack),
				new GizmoEntry(289756, PluginGizmoType.Chest, "x1_Abattoir_ArmorRack-3055", GizmoTargetTypes.ItemRack),
				new GizmoEntry(289244, PluginGizmoType.Chest, "x1_Westm_ArmorRack-9628", GizmoTargetTypes.ItemRack),

				//============== Misc Containers ==============//
				new GizmoEntry(5727, PluginGizmoType.Chest, "trDun_Cath_BookcaseShelves_B-394", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(5724, PluginGizmoType.Chest, "trDun_Cath_BookcaseShelves_A-1786", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(178151, PluginGizmoType.Chest, "trOut_Highlands_Mystic_Wagon-4999", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(5673, PluginGizmoType.Chest, "trDun_book_pile_a-822", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(5730, PluginGizmoType.Chest, "trDun_Cath_BookcaseShelves_Wide-2385", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(219334, PluginGizmoType.Chest, "a1dun_Crypts_Jar_of_Souls_02-6412", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(73396, PluginGizmoType.Chest, "a2dun_Zolt_Books_Full_Wall_A-625", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(71543, PluginGizmoType.Chest, "a2dun_Zolt_Desk_Scrolls_A-634", GizmoTargetTypes.MiscContainer),

				//============== Special Interactables ==============//
				new GizmoEntry(174754, PluginGizmoType.Switch),
				new GizmoEntry(174753, PluginGizmoType.Switch),
				new GizmoEntry(102079, PluginGizmoType.Switch),
				new GizmoEntry(105754, PluginGizmoType.Switch),
				new GizmoEntry(203608, PluginGizmoType.Switch),
				new GizmoEntry(93713, PluginGizmoType.Switch),
				new GizmoEntry(301177, PluginGizmoType.Switch),
				new GizmoEntry(102927, PluginGizmoType.Switch, "Ghost_Jail_Prisoner"), //Warden Skeleton Remains
				new GizmoEntry(213490, PluginGizmoType.Switch, "a2dun_Spider_Venom_Pool-4568"),
				new GizmoEntry(461, PluginGizmoType.Switch, "trDun_SkeletonKing_Bridge_Active-3253"),
				new GizmoEntry(5747, PluginGizmoType.Switch, "trDun_Cath_Chandelier_Trap_Switch2-383"),
				new GizmoEntry(221574, PluginGizmoType.Switch, "a1dun_Leoric_IronMaiden_Event-14441"),

				new GizmoEntry(138989, PluginGizmoType.HealingWell, "", GizmoTargetTypes.Healthwell),
				new GizmoEntry(373463, PluginGizmoType.PoolOfReflection, "PoolOfReflection", GizmoTargetTypes.PoolOfReflection),
				 

				new GizmoEntry(176075, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(176077, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(176074, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(176076, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(260331, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(260330, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(330697, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(330699, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(330698, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(330695, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(330696, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
			};
		}

		public void ClearCollections()
		{
			GizmoCache.Clear();
			GizmoStringCache.Clear();
		}

		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "External", "Dictionaries", "Cache_Gizmos.xml");
		public static GizmoDataCollection DeserializeFromXML()
		{
			var deserializer = new XmlSerializer(typeof(GizmoDataCollection));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (GizmoDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		public static GizmoDataCollection DeserializeFromXML(string FolderPath)
		{
			if (!Directory.Exists(FolderPath))
				return null;

			string FilePath = Path.Combine(FolderPath, "Cache_Gizmos.xml");

			var deserializer = new XmlSerializer(typeof(GizmoDataCollection));
			TextReader textReader = new StreamReader(FilePath);
			var settings = (GizmoDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}

		public static void SerializeToXML(GizmoDataCollection settings)
		{
			var serializer = new XmlSerializer(typeof(GizmoDataCollection));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}
		public static void SerializeToXML(GizmoDataCollection settings, string FolderPath)
		{
			string FilePath = Path.Combine(FolderPath, "Cache_Gizmos.xml");
			var serializer = new XmlSerializer(typeof(GizmoDataCollection));
			var textWriter = new StreamWriter(FilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}

	}
}
