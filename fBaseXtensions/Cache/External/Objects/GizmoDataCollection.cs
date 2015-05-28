﻿using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Helpers;

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
	
				// 
				new GizmoEntry(113960, PluginGizmoType.BreakableDoor, "caOut_Oasis_Rakanishu_SideStone_A-10174", GizmoTargetTypes.SpecialDestructible),
				new GizmoEntry(113845, PluginGizmoType.BreakableDoor, "caOut_Oasis_Rakanishu_CenterStone_A-7493", GizmoTargetTypes.SpecialDestructible),

				//============== Barricades ==============//
				new GizmoEntry(5719, PluginGizmoType.BreakableDoor, "trDun_Cath_Barricade_B"),
				new GizmoEntry(379048, PluginGizmoType.BreakableDoor, "p1_Cesspools_Door_Breakable"),
				new GizmoEntry(379037, PluginGizmoType.BreakableDoor, "p1_Cesspools_Barricade_Breakable"),
				new GizmoEntry(95481, PluginGizmoType.BreakableDoor, "a1dun_Leor_Jail_Door_Breakable_A-67620"),
				new GizmoEntry(181228, PluginGizmoType.BreakableDoor, "caOut_StingingWinds_Barricade_A-713"),
				new GizmoEntry(62653, PluginGizmoType.BreakableDoor, "a2dun_Aqd_Act_Barricade_A_01-13064"),
				new GizmoEntry(118384, PluginGizmoType.BreakableDoor, "CaOut_Oasis_Gear_Box-14874"),
				new GizmoEntry(5718, PluginGizmoType.BreakableDoor, "trDun_Cath_Barricade_A-17018"),
				new GizmoEntry(5792, PluginGizmoType.BreakableDoor, "trDun_Cath_WoodDoor_A_Barricaded-970"),
				new GizmoEntry(5823, PluginGizmoType.BreakableDoor, "trDun_Crypt_Door-2084"),
				new GizmoEntry(195108, PluginGizmoType.BreakableDoor, "Barricade_Doube_Breakable_Snow_A-3269"),
				new GizmoEntry(193932, PluginGizmoType.BreakableDoor, "a3_Battlefield_Barricade_Double_Breakable_charred-8320"),
				new GizmoEntry(193963, PluginGizmoType.BreakableDoor, "a3_Battlefield_Barricade_Breakable_charred-8359"),
				new GizmoEntry(159066, PluginGizmoType.BreakableDoor, "a3dun_Bridge_Barricade_A-8366"),
				new GizmoEntry(159117, PluginGizmoType.BreakableDoor, "a3dun_Bridge_Barricade_B-8415"),
				new GizmoEntry(159561, PluginGizmoType.BreakableDoor, "a3dun_Bridge_Barricade_C-9841"),
				new GizmoEntry(78935, PluginGizmoType.BreakableDoor, "a1dun_Caves_Goat_Barricade_B-24006"),
				new GizmoEntry(55325, PluginGizmoType.BreakableDoor, "a3dun_Keep_Door_Destructable-13854"),
				new GizmoEntry(380987, PluginGizmoType.BreakableDoor, "p1_Cesspools_Barricade_Breakable_Scaffolding-30687"),
				new GizmoEntry(291776, PluginGizmoType.BreakableDoor, "x1_fortress_Barricade_Breakable-110680"),
				new GizmoEntry(291382, PluginGizmoType.BreakableDoor, "x1_Westm_Barricade_Round-154093"),
				new GizmoEntry(108194, PluginGizmoType.BreakableDoor, "a2dunSwr_Breakables_Barricade_B-163977"),
				new GizmoEntry(291744, PluginGizmoType.BreakableDoor, "x1_Catacombs_Barricade_Round-38867"),
				new GizmoEntry(291743, PluginGizmoType.BreakableDoor, "x1_Catacombs_Barricade_Breakable-55384"),
				new GizmoEntry(199412, PluginGizmoType.BreakableDoor, "trDun_TinkerDoor_Breakable-8333"),
				new GizmoEntry(103316, PluginGizmoType.BreakableDoor, "TrOut_Highlands_Manor_Front_Gate-18784"),
				new GizmoEntry(195101, PluginGizmoType.BreakableDoor, "Barricade_Breakable_Snow_A-457233"),
				new GizmoEntry(291372, PluginGizmoType.BreakableDoor, "x1_Westm_Barricade_Breakable-518640"),


				//============== DOORS ==============//
				new GizmoEntry(54882, PluginGizmoType.Door, "a3dun_Keep_Door_Wooden_A", GizmoTargetTypes.None, 40),
				new GizmoEntry(54850, PluginGizmoType.Door, "a3dun_Keep_SiegeTowerDoor", GizmoTargetTypes.None, 14),
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
				new GizmoEntry(454, PluginGizmoType.Door, "trDun_Cath_WoodDoor_A-814"),

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
				new GizmoEntry(106165, PluginGizmoType.Chest, "caOut_Boneyards_chest_Rare-16905", GizmoTargetTypes.Resplendant),
				new GizmoEntry(356805, PluginGizmoType.Chest, "x1_Catacombs_chest_rare_GardenEvent-89365", GizmoTargetTypes.Resplendant),
				new GizmoEntry(261136, PluginGizmoType.Chest, "x1_Catacombs_chest_rare-11124", GizmoTargetTypes.Resplendant),
				new GizmoEntry(129476, PluginGizmoType.Chest, "a3dun_Crater_Chest_Rare-17045", GizmoTargetTypes.Resplendant),
				new GizmoEntry(187107, PluginGizmoType.Chest, "a4dun_Spire_Chest_Rare-58008", GizmoTargetTypes.Resplendant),
				new GizmoEntry(289797, PluginGizmoType.Chest, "X1_PandExt_Chest_Rare-70672", GizmoTargetTypes.Resplendant),
				new GizmoEntry(190708, PluginGizmoType.Chest, "a2dun_Aqd_Chest_Rare_FacePuzzleSmall-14922", GizmoTargetTypes.Resplendant),
                new GizmoEntry(249362, PluginGizmoType.Chest, "x1_BogCave_Chest_Rare-42256", GizmoTargetTypes.Resplendant),
                new GizmoEntry(289856, PluginGizmoType.Chest, "x1_Fortress_Chest_Rare-35353", GizmoTargetTypes.Resplendant),
                new GizmoEntry(197655, PluginGizmoType.Chest, "a2dun_Swr_Chest_Rare-19521", GizmoTargetTypes.Resplendant),

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
				new GizmoEntry(106329, PluginGizmoType.Chest, "caOut_Oasis_Well_Lift_Chest-8787", GizmoTargetTypes.Chest),
				new GizmoEntry(79016, PluginGizmoType.Chest, "trOut_Tristram_chest-4447", GizmoTargetTypes.Chest),
				new GizmoEntry(3018, PluginGizmoType.Chest, "a2dun_Zolt_Random_Chest-6138", GizmoTargetTypes.Chest),
				new GizmoEntry(212491, PluginGizmoType.Chest, "a1dun_Random_Cloud-16051", GizmoTargetTypes.Chest),
				new GizmoEntry(210422, PluginGizmoType.Chest, "a1dun_random_pot_of_gold_A-16933", GizmoTargetTypes.Chest),
				new GizmoEntry(338905, PluginGizmoType.Chest, "X1_Fortress_Chest-66175", GizmoTargetTypes.Chest),
				new GizmoEntry(213820, PluginGizmoType.Chest, "a2dun_Zolt_Blood_Container-46882", GizmoTargetTypes.Chest),
				new GizmoEntry(213859, PluginGizmoType.Chest, "a2dun_Zolt_Blood_Container_02-50993", GizmoTargetTypes.Chest),
				new GizmoEntry(213447, PluginGizmoType.Chest, "Lore_AzmodanChest3-42024", GizmoTargetTypes.Chest),
				new GizmoEntry(213446, PluginGizmoType.Chest, "Lore_AzmodanChest2-43183", GizmoTargetTypes.Chest),
				new GizmoEntry(199584, PluginGizmoType.Chest, "CaOut_BoneYard_WormCave_Chest-45908", GizmoTargetTypes.Chest),
				new GizmoEntry(108122, PluginGizmoType.Chest, "caOut_StingingWinds_Chest-21069", GizmoTargetTypes.Chest),
				new GizmoEntry(191734, PluginGizmoType.Chest, "caOut_StingingWinds_Chest_CultistCamp-13499", GizmoTargetTypes.Chest),
                new GizmoEntry(197018, PluginGizmoType.Chest, "a2dun_Cave_EggSack__Chest-54543", GizmoTargetTypes.Chest),

				//============== Floor Switches ==============//
				new GizmoEntry(5759, PluginGizmoType.Chest, "trDun_Cath_FloorSpawner_02-670", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(116063, PluginGizmoType.Chest, "a2dun_Spider_Ground_Spawner-13310", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(5758, PluginGizmoType.Chest, "trDun_Cath_FloorSpawner_01-2966", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(261139, PluginGizmoType.Chest, "x1_Catacombs_Ground_Clicky-11367", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(289762, PluginGizmoType.Chest, "x1_Abattoir_Ground_Clicky-10782", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(251867, PluginGizmoType.Chest, "x1_Bog_Ground_Clicky-5500", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(289784, PluginGizmoType.Chest, "x1_westm_Graveyard_Ground_Clicky-14846", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(285382, PluginGizmoType.Chest, "x1_Pand_Ext_Ground_Clicky-1947", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(289805, PluginGizmoType.Chest, "x1_Pand_HexMaze_Ground_Clicky-12403", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(289250, PluginGizmoType.Chest, "x1_Westm_Ground_Clicky-14727", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(249365, PluginGizmoType.Chest, "x1_BogCave_GroundTile-15025", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(379074, PluginGizmoType.Chest, "p1_Cesspools_Ground_Clicky-39539", GizmoTargetTypes.FloorContainer),
				new GizmoEntry(289859, PluginGizmoType.Chest, "x1_Fortress_Ground_Clicky-66204", GizmoTargetTypes.FloorContainer),

				//============== Corpse ==============//
				#region Corpses
				new GizmoEntry(122930, PluginGizmoType.Chest, "caOut_Boneyard_BanishedSkeleton_B-12533", GizmoTargetTypes.Corpse),
				new GizmoEntry(122904, PluginGizmoType.Chest, "caOut_Boneyard_BanishedSkeleton_A-13073", GizmoTargetTypes.Corpse),
				new GizmoEntry(122932, PluginGizmoType.Chest, "caOut_Boneyard_BanishedSkeleton_C-24391", GizmoTargetTypes.Corpse),
				new GizmoEntry(311234, PluginGizmoType.Chest, "x1_Westm_Female_Corpse_A_01-40455", GizmoTargetTypes.Corpse),
				new GizmoEntry(193021, PluginGizmoType.Chest, "LootType3_GraveGuard_C_Corpse_01-19181", GizmoTargetTypes.Corpse),
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
				new GizmoEntry(188388, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_G_Corpse_05-32847", GizmoTargetTypes.Corpse),
				new GizmoEntry(187972, PluginGizmoType.Chest, "LootType2_caldeumVillager_Male_A_Corpse_03-1449", GizmoTargetTypes.Corpse),
				new GizmoEntry(187975, PluginGizmoType.Chest, "LootType2_caldeumVillager_Male_A_Corpse_06-1892", GizmoTargetTypes.Corpse),
				new GizmoEntry(309407, PluginGizmoType.Chest, "x1_Westm_Corpse_E_03-19796", GizmoTargetTypes.Corpse),
				new GizmoEntry(193024, PluginGizmoType.Chest, "LootType3_GraveGuard_C_Corpse_04-5062", GizmoTargetTypes.Corpse),
				new GizmoEntry(193017, PluginGizmoType.Chest, "LootType3_GraveGuard_B_Corpse_03-5763", GizmoTargetTypes.Corpse),
				new GizmoEntry(188391, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_G_Corpse_02-5849", GizmoTargetTypes.Corpse),
				new GizmoEntry(188063, PluginGizmoType.Chest, "LootType2_skeleton_A_Corpse_01-2187", GizmoTargetTypes.Corpse),
				new GizmoEntry(193028, PluginGizmoType.Chest, "LootType3_GraveGuard_D_Corpse_01-5132", GizmoTargetTypes.Corpse),
				new GizmoEntry(309390, PluginGizmoType.Chest, "x1_Westm_Corpse_B_03-321", GizmoTargetTypes.Corpse),
				new GizmoEntry(289249, PluginGizmoType.Chest, "x1_Westm_Corpse_A_01-340", GizmoTargetTypes.Corpse),
				new GizmoEntry(309396, PluginGizmoType.Chest, "x1_Westm_Corpse_C_04-341", GizmoTargetTypes.Corpse),
				new GizmoEntry(312138, PluginGizmoType.Chest, "x1_westmarchGuard_Melee_Corpse_04-24119", GizmoTargetTypes.Corpse),
				new GizmoEntry(312139, PluginGizmoType.Chest, "x1_westmarchGuard_Melee_Corpse_05-24635", GizmoTargetTypes.Corpse),
				new GizmoEntry(312136, PluginGizmoType.Chest, "x1_westmarchGuard_Melee_Corpse_02-25092", GizmoTargetTypes.Corpse),
				new GizmoEntry(312146, PluginGizmoType.Chest, "x1_westmarchGuard_Ranged_Corpse_06-25505", GizmoTargetTypes.Corpse),
				new GizmoEntry(312144, PluginGizmoType.Chest, "x1_westmarchGuard_Ranged_Corpse_04-26246", GizmoTargetTypes.Corpse),
				new GizmoEntry(359193, PluginGizmoType.Chest, "x1_Adventurer_Female_Corpse_D_04-26411", GizmoTargetTypes.Corpse),
				new GizmoEntry(312143, PluginGizmoType.Chest, "x1_westmarchGuard_Ranged_Corpse_03-27968", GizmoTargetTypes.Corpse),
				new GizmoEntry(260417, PluginGizmoType.Chest, "x1_Bog_Corpse_01-30455", GizmoTargetTypes.Corpse),
				new GizmoEntry(359571, PluginGizmoType.Chest, "x1_BogBlight_Corpse-33821", GizmoTargetTypes.Corpse),
				new GizmoEntry(312142, PluginGizmoType.Chest, "x1_westmarchGuard_Ranged_Corpse_02-5729", GizmoTargetTypes.Corpse),
				new GizmoEntry(312135, PluginGizmoType.Chest, "x1_westmarchGuard_Melee_Corpse_01-5743", GizmoTargetTypes.Corpse),
				new GizmoEntry(309388, PluginGizmoType.Chest, "x1_Westm_Corpse_A_06-5770", GizmoTargetTypes.Corpse),
				new GizmoEntry(309386, PluginGizmoType.Chest, "x1_Westm_Corpse_A_04-5784", GizmoTargetTypes.Corpse),
				new GizmoEntry(312141, PluginGizmoType.Chest, "x1_westmarchGuard_Ranged_Corpse_01-6040", GizmoTargetTypes.Corpse),
				new GizmoEntry(359194, PluginGizmoType.Chest, "x1_Adventurer_Female_Corpse_A_03-6278", GizmoTargetTypes.Corpse),
				new GizmoEntry(309394, PluginGizmoType.Chest, "x1_Westm_Corpse_C_02-7561", GizmoTargetTypes.Corpse),
				new GizmoEntry(309410, PluginGizmoType.Chest, "x1_Westm_Corpse_E_06-7641", GizmoTargetTypes.Corpse),
				new GizmoEntry(309409, PluginGizmoType.Chest, "x1_Westm_Corpse_E_05-7678", GizmoTargetTypes.Corpse),
				new GizmoEntry(359195, PluginGizmoType.Chest, "x1_Adventurer_Female_Corpse_B_04-7949", GizmoTargetTypes.Corpse),
				new GizmoEntry(309382, PluginGizmoType.Chest, "x1_Westm_Corpse_D_01-8862", GizmoTargetTypes.Corpse),
				new GizmoEntry(309389, PluginGizmoType.Chest, "x1_Westm_Corpse_B_02-8865", GizmoTargetTypes.Corpse),
				new GizmoEntry(312145, PluginGizmoType.Chest, "x1_westmarchGuard_Ranged_Corpse_05-8965", GizmoTargetTypes.Corpse),
				new GizmoEntry(312140, PluginGizmoType.Chest, "x1_westmarchGuard_Melee_Corpse_06-10310", GizmoTargetTypes.Corpse),
				new GizmoEntry(360303, PluginGizmoType.Chest, "x1_Westm_MysticCorpsePile-12175", GizmoTargetTypes.Corpse),
				new GizmoEntry(309395, PluginGizmoType.Chest, "x1_Westm_Corpse_C_03-20339", GizmoTargetTypes.Corpse),
				new GizmoEntry(188383, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_F_Corpse_04-9852", GizmoTargetTypes.Corpse),
				new GizmoEntry(188386, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_F_Corpse_01-9866", GizmoTargetTypes.Corpse),
				new GizmoEntry(188379, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_E_Corpse_04-9865", GizmoTargetTypes.Corpse),
				new GizmoEntry(188362, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_B_Corpse_06-9873", GizmoTargetTypes.Corpse),
				new GizmoEntry(188355, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_A_Corpse_05-9891", GizmoTargetTypes.Corpse),
				new GizmoEntry(188360, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_B_Corpse_04-9888", GizmoTargetTypes.Corpse),
				new GizmoEntry(188381, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_F_Corpse_06-9898", GizmoTargetTypes.Corpse),
				new GizmoEntry(188377, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_E_Corpse_02-9904", GizmoTargetTypes.Corpse),
				new GizmoEntry(193012, PluginGizmoType.Chest, "LootType3_GraveGuard_A_Corpse_04-15087", GizmoTargetTypes.Corpse),
				new GizmoEntry(193018, PluginGizmoType.Chest, "LootType3_GraveGuard_B_Corpse_04-15167", GizmoTargetTypes.Corpse),
				new GizmoEntry(119801, PluginGizmoType.Chest, "Adventurer_A_Corpse_NephalemCave-6056", GizmoTargetTypes.Corpse),
				new GizmoEntry(188363, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_C_Corpse_01-12550", GizmoTargetTypes.Corpse),
				new GizmoEntry(156653, PluginGizmoType.Chest, "TristramGuard_Corpse_03_DescentEvent-2063", GizmoTargetTypes.Corpse),
				new GizmoEntry(193026, PluginGizmoType.Chest, "LootType3_GraveGuard_C_Corpse_06-23822", GizmoTargetTypes.Corpse),
				new GizmoEntry(193009, PluginGizmoType.Chest, "LootType3_GraveGuard_A_Corpse_01-6330", GizmoTargetTypes.Corpse),
				new GizmoEntry(200222, PluginGizmoType.Chest, "LootType2_BastionsKeepGuard_Frosty_Melee_Corpse_02-10677", GizmoTargetTypes.Corpse),
				new GizmoEntry(200252, PluginGizmoType.Chest, "Clicky_LootType2_DemonTrooper_B_Frosty_Corpse_01-13367", GizmoTargetTypes.Corpse),
				new GizmoEntry(200221, PluginGizmoType.Chest, "LootType2_BastionsKeepGuard_Frosty_Melee_Corpse_01-15048", GizmoTargetTypes.Corpse),
				new GizmoEntry(188384, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_F_Corpse_03-17401", GizmoTargetTypes.Corpse),
				new GizmoEntry(187974, PluginGizmoType.Chest, "LootType2_caldeumVillager_Male_A_Corpse_05-595", GizmoTargetTypes.Corpse),
				new GizmoEntry(187960, PluginGizmoType.Chest, "LootType2_caldeumVillager_Male_A_Corpse_01-601", GizmoTargetTypes.Corpse),
				new GizmoEntry(187973, PluginGizmoType.Chest, "LootType2_caldeumVillager_Male_A_Corpse_04-1149", GizmoTargetTypes.Corpse),
				new GizmoEntry(187963, PluginGizmoType.Chest, "LootType2_caldeumVillager_Male_A_Corpse_02-3396", GizmoTargetTypes.Corpse),
				new GizmoEntry(359191, PluginGizmoType.Chest, "x1_Adventurer_Female_Corpse_B_02-8488", GizmoTargetTypes.Corpse),
				new GizmoEntry(309403, PluginGizmoType.Chest, "x1_Westm_Corpse_D_05-1640", GizmoTargetTypes.Corpse),
				new GizmoEntry(309400, PluginGizmoType.Chest, "x1_Westm_Corpse_D_02-2898", GizmoTargetTypes.Corpse),
				new GizmoEntry(359190, PluginGizmoType.Chest, "x1_Adventurer_Female_Corpse_A_01-3766", GizmoTargetTypes.Corpse),
				new GizmoEntry(309406, PluginGizmoType.Chest, "x1_Westm_Corpse_E_02-7290", GizmoTargetTypes.Corpse),
				new GizmoEntry(188374, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_D_Corpse_06-27369", GizmoTargetTypes.Corpse),
				new GizmoEntry(188382, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_F_Corpse_05-29262", GizmoTargetTypes.Corpse),
				new GizmoEntry(309393, PluginGizmoType.Chest, "x1_Westm_Corpse_B_06-18437", GizmoTargetTypes.Corpse),
				new GizmoEntry(385818, PluginGizmoType.Chest, "LootType3_DeadEndCorpse_CaldeumGuard_Cleaver_A_Corpse_02-87136", GizmoTargetTypes.Corpse),
				new GizmoEntry(385820, PluginGizmoType.Chest, "LootType3_DeadEndCorpse_TristramGuard_Corpse_02-56997", GizmoTargetTypes.Corpse),
				new GizmoEntry(380516, PluginGizmoType.Chest, "p1_Cesspools_Corpse_Bloated-40161", GizmoTargetTypes.Corpse),
				new GizmoEntry(380367, PluginGizmoType.Chest, "p1_Cesspools_Corpse_B-40709", GizmoTargetTypes.Corpse),
				new GizmoEntry(379071, PluginGizmoType.Chest, "p1_Cesspools_Corpse-42087", GizmoTargetTypes.Corpse),
				new GizmoEntry(289860, PluginGizmoType.Chest, "x1_Fortress_Corpse-66233", GizmoTargetTypes.Corpse),
				new GizmoEntry(385817, PluginGizmoType.Chest, "LootType3_DeadEndCorpse_CaldeumGuard_Cleaver_A_Corpse_01-12191", GizmoTargetTypes.Corpse),
				new GizmoEntry(309404, PluginGizmoType.Chest, "x1_Westm_Corpse_D_06-58048", GizmoTargetTypes.Corpse),
				new GizmoEntry(309384, PluginGizmoType.Chest, "x1_Westm_Corpse_A_02-36773", GizmoTargetTypes.Corpse),
				new GizmoEntry(385940, PluginGizmoType.Chest, "x1_Catacombs_Corpse_01_DeadEndReward-78256", GizmoTargetTypes.Corpse),
				new GizmoEntry(362421, PluginGizmoType.Chest, "x1_Westm_Corpse2-92884", GizmoTargetTypes.Corpse),
				new GizmoEntry(289795, PluginGizmoType.Chest, "X1_PandExt_Corpse-94007", GizmoTargetTypes.Corpse),
				new GizmoEntry(385937, PluginGizmoType.Chest, "x1_Fortress_Corpse_DeadEndReward-109792", GizmoTargetTypes.Corpse),
				new GizmoEntry(352429, PluginGizmoType.Chest, "x1_Pand_demonFlyer_B_clickable_corpse_01-93948", GizmoTargetTypes.Corpse),
				new GizmoEntry(188390, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_G_Corpse_03-6509", GizmoTargetTypes.Corpse),
				new GizmoEntry(188365, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_C_Corpse_03-849", GizmoTargetTypes.Corpse),
				new GizmoEntry(188358, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_B_Corpse_02-10805", GizmoTargetTypes.Corpse),
				new GizmoEntry(188359, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_B_Corpse_03-36505", GizmoTargetTypes.Corpse),
				new GizmoEntry(385819, PluginGizmoType.Chest, "LootType3_DeadEndCorpse_TristramGuard_Corpse_01-13295", GizmoTargetTypes.Corpse),
				new GizmoEntry(193013, PluginGizmoType.Chest, "LootType3_GraveGuard_A_Corpse_05-26450", GizmoTargetTypes.Corpse),
				new GizmoEntry(193010, PluginGizmoType.Chest, "LootType3_GraveGuard_A_Corpse_02-7132", GizmoTargetTypes.Corpse),
				new GizmoEntry(123930, PluginGizmoType.Chest, "sunBleachedCorpse_A_01-21203", GizmoTargetTypes.Corpse),
				new GizmoEntry(188065, PluginGizmoType.Chest, "LootType2_skeleton_A_Corpse_03-44934", GizmoTargetTypes.Corpse),
				new GizmoEntry(188357, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_B_Corpse_01-45747", GizmoTargetTypes.Corpse),
				new GizmoEntry(188067, PluginGizmoType.Chest, "LootType2_skeleton_A_Corpse_05-45850", GizmoTargetTypes.Corpse),
				new GizmoEntry(188387, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_G_Corpse_06-45969", GizmoTargetTypes.Corpse),
				new GizmoEntry(188368, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_C_Corpse_06-46144", GizmoTargetTypes.Corpse),
				new GizmoEntry(188389, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_G_Corpse_04-46262", GizmoTargetTypes.Corpse),
				new GizmoEntry(188376, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_E_Corpse_01-19127", GizmoTargetTypes.Corpse),
				new GizmoEntry(188371, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_D_Corpse_03-23459", GizmoTargetTypes.Corpse),
				new GizmoEntry(309383, PluginGizmoType.Chest, "x1_Westm_Corpse_E_01-16388", GizmoTargetTypes.Corpse),
				new GizmoEntry(192774, PluginGizmoType.Chest, "BastionsKeepGuard_Melee_Corpse_Morgan-29969", GizmoTargetTypes.Corpse),
				new GizmoEntry(204724, PluginGizmoType.Chest, "BastionsKeepGuard_Corpse_Jonathan_L-30929", GizmoTargetTypes.Corpse),
				new GizmoEntry(385814, PluginGizmoType.Chest, "LootType3_DeadEndCorpse_BastionsKeepGuard_Melee_Corpse_02-37466", GizmoTargetTypes.Corpse),
				new GizmoEntry(141420, PluginGizmoType.Chest, "demonTrooper_A_clickable_corpse_01-39140", GizmoTargetTypes.Corpse),
				new GizmoEntry(200228, PluginGizmoType.Chest, "LootType2_BastionsKeepGuard_Frosty_Ranged_Corpse_01-41821", GizmoTargetTypes.Corpse),
				new GizmoEntry(200251, PluginGizmoType.Chest, "Clicky_LootType2_DemonTrooper_A_Frosty_Corpse_01-41864", GizmoTargetTypes.Corpse),
				new GizmoEntry(200233, PluginGizmoType.Chest, "LootType2_BastionsKeepGuard_Frosty_Ranged_Corpse_06-41902", GizmoTargetTypes.Corpse),
				new GizmoEntry(230768, PluginGizmoType.Chest, "demonTrooper_B_clickable_corpse_01-42003", GizmoTargetTypes.Corpse),
				new GizmoEntry(200249, PluginGizmoType.Chest, "Clicky_LootType2_DemonFlyer_A_Frosty_Corpse_01-43762", GizmoTargetTypes.Corpse),
				new GizmoEntry(200231, PluginGizmoType.Chest, "LootType2_BastionsKeepGuard_Frosty_Ranged_Corpse_05-44897", GizmoTargetTypes.Corpse),
				new GizmoEntry(188364, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_C_Corpse_02-1953", GizmoTargetTypes.Corpse),
				new GizmoEntry(188361, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_B_Corpse_05-21751", GizmoTargetTypes.Corpse),
				new GizmoEntry(188356, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_A_Corpse_06-5361", GizmoTargetTypes.Corpse),
				new GizmoEntry(385813, PluginGizmoType.Chest, "LootType3_DeadEndCorpse_BastionsKeepGuard_Melee_Corpse_01-6345", GizmoTargetTypes.Corpse),
				new GizmoEntry(385816, PluginGizmoType.Chest, "LootType3_DeadEndCorpse_BastionsKeepGuard_Ranged_Corpse_02-20452", GizmoTargetTypes.Corpse),
				new GizmoEntry(188010, PluginGizmoType.Chest, "demonFlyer_B_clickable_corpse_01-23271", GizmoTargetTypes.Corpse),
				new GizmoEntry(385815, PluginGizmoType.Chest, "LootType3_DeadEndCorpse_BastionsKeepGuard_Ranged_Corpse_01-13185", GizmoTargetTypes.Corpse),
				new GizmoEntry(188351, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_A_Corpse_01-1623", GizmoTargetTypes.Corpse),
				new GizmoEntry(188353, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_A_Corpse_03-2019", GizmoTargetTypes.Corpse),
				new GizmoEntry(190276, PluginGizmoType.Chest, "LootType3_CaldeumGuard_Cleaver_A_Corpse_05-6325", GizmoTargetTypes.Corpse),
				new GizmoEntry(188066, PluginGizmoType.Chest, "LootType2_skeleton_A_Corpse_04-702", GizmoTargetTypes.Corpse),
				new GizmoEntry(193016, PluginGizmoType.Chest, "LootType3_GraveGuard_B_Corpse_02-29580", GizmoTargetTypes.Corpse),
				new GizmoEntry(188385, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_F_Corpse_02-45508", GizmoTargetTypes.Corpse),
				new GizmoEntry(193027, PluginGizmoType.Chest, "LootType3_GraveGuard_D_Corpse_06-45500", GizmoTargetTypes.Corpse),
				new GizmoEntry(188373, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_D_Corpse_05-1501", GizmoTargetTypes.Corpse),
				new GizmoEntry(188354, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_A_Corpse_04-7762", GizmoTargetTypes.Corpse),
				new GizmoEntry(188375, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_E_Corpse_06-13184", GizmoTargetTypes.Corpse),
				new GizmoEntry(188378, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_E_Corpse_03-15081", GizmoTargetTypes.Corpse),
				new GizmoEntry(188380, PluginGizmoType.Chest, "LootType2_caldeumTortured_Male_E_Corpse_05-3925", GizmoTargetTypes.Corpse),
				new GizmoEntry(188068, PluginGizmoType.Chest, "LootType2_skeleton_A_Corpse_06-33635", GizmoTargetTypes.Corpse),
				new GizmoEntry(188064, PluginGizmoType.Chest, "LootType2_skeleton_A_Corpse_02-35332", GizmoTargetTypes.Corpse),
				new GizmoEntry(193011, PluginGizmoType.Chest, "LootType3_GraveGuard_A_Corpse_03-13730", GizmoTargetTypes.Corpse),
				new GizmoEntry(193032, PluginGizmoType.Chest, "LootType3_GraveGuard_D_Corpse_05-55169", GizmoTargetTypes.Corpse),
				new GizmoEntry(309401, PluginGizmoType.Chest, "x1_Westm_Corpse_D_03-17426", GizmoTargetTypes.Corpse),
				new GizmoEntry(193019, PluginGizmoType.Chest, "LootType3_GraveGuard_B_Corpse_05-50003", GizmoTargetTypes.Corpse),
				new GizmoEntry(193031, PluginGizmoType.Chest, "LootType3_GraveGuard_D_Corpse_04-12723", GizmoTargetTypes.Corpse),
                new GizmoEntry(193020, PluginGizmoType.Chest, "LootType3_GraveGuard_B_Corpse_06-21296", GizmoTargetTypes.Corpse),
				#endregion

				//============== Weapon Rack ==============//
				new GizmoEntry(464, PluginGizmoType.Chest, "trDun_WeaponRack-791", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(58317, PluginGizmoType.Chest, "a1dun_Leor_Tool_Rack_A_01-15551", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(77354, PluginGizmoType.Chest, "Goatman_Weapon_Rack_trOut_Highlands-17316", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(192466, PluginGizmoType.Chest, "A3_Battlefield_Weaponrack_A-16802", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(198012, PluginGizmoType.Chest, "a2dun_zolt_WeaponRack_A-18302", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(289246, PluginGizmoType.Chest, "x1_Westm_weaponRack-9780", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(289763, PluginGizmoType.Chest, "x1_Abattoir_weaponRack-1181", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(167520, PluginGizmoType.Chest, "a4dunGarden_Props_Weaponrack_A-8221", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(289763, PluginGizmoType.Chest, "x1_Abattoir_weaponRack-5023", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(307432, PluginGizmoType.Chest, "x1_westm_Int_WeaponRack-6628", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(301981, PluginGizmoType.Chest, "x1_westm_Tub_Tools_A-7975", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(289858, PluginGizmoType.Chest, "x1_Fortress_WeaponRack-66101", GizmoTargetTypes.WeaponRack),
				new GizmoEntry(379076, PluginGizmoType.Chest, "p1_Cesspools_weaponRack-39531", GizmoTargetTypes.WeaponRack),

				//============== Armor Rack ==============//
				new GizmoEntry(5671, PluginGizmoType.Chest, "trDun_ArmorRack-1451", GizmoTargetTypes.ArmorRack),
				new GizmoEntry(289794, PluginGizmoType.Chest, "X1_PandExt_ArmorRack-12750", GizmoTargetTypes.ArmorRack),
				new GizmoEntry(340114, PluginGizmoType.Chest, "x1_Bog_ArmorRack_A-15606", GizmoTargetTypes.ArmorRack),
				new GizmoEntry(289756, PluginGizmoType.Chest, "x1_Abattoir_ArmorRack-3055", GizmoTargetTypes.ArmorRack),
				new GizmoEntry(289244, PluginGizmoType.Chest, "x1_Westm_ArmorRack-9628", GizmoTargetTypes.ArmorRack),
				new GizmoEntry(346654, PluginGizmoType.Chest, "X1_Westm_Graveyard_Armor_Rack-13280", GizmoTargetTypes.ArmorRack),
				new GizmoEntry(289777, PluginGizmoType.Chest, "x1_Bog_ArmorRack-5965", GizmoTargetTypes.ArmorRack),
				new GizmoEntry(196509, PluginGizmoType.Chest, "a3dun_Bridge_Armor_Rack-42001", GizmoTargetTypes.ArmorRack),
				new GizmoEntry(307431, PluginGizmoType.Chest, "x1_westm_Int_ArmorRack-26984", GizmoTargetTypes.ArmorRack),
				new GizmoEntry(53253, PluginGizmoType.Chest, "a3dun_Keep_Armor_Rack-17043", GizmoTargetTypes.ArmorRack),
                new GizmoEntry(96594, PluginGizmoType.Chest, "trOut_Highlands_ChiefGoatmenMummyRack_A-8866", GizmoTargetTypes.ArmorRack),

				//============== Misc Containers ==============//
				new GizmoEntry(5727, PluginGizmoType.Chest, "trDun_Cath_BookcaseShelves_B-394", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(5724, PluginGizmoType.Chest, "trDun_Cath_BookcaseShelves_A-1786", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(178151, PluginGizmoType.Chest, "trOut_Highlands_Mystic_Wagon-4999", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(5673, PluginGizmoType.Chest, "trDun_book_pile_a-822", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(5730, PluginGizmoType.Chest, "trDun_Cath_BookcaseShelves_Wide-2385", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(219334, PluginGizmoType.Chest, "a1dun_Crypts_Jar_of_Souls_02-6412", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(73396, PluginGizmoType.Chest, "a2dun_Zolt_Books_Full_Wall_A-625", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(71543, PluginGizmoType.Chest, "a2dun_Zolt_Desk_Scrolls_A-634", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(230712, PluginGizmoType.Chest, "Lore_WaterPuzzle_Satchel-7158", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(313989, PluginGizmoType.Chest, "x1_westm_Book_shelf-27498", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(170233, PluginGizmoType.Chest, "LoreChest_GuardCaptainJournal-1949", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(220152, PluginGizmoType.Chest, "Lore_Satchel_Chest_FacePuzzleLarge-42045", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(220153, PluginGizmoType.Chest, "Lore_Satchel_Chest_FacePuzzleSmall-14914", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(207706, PluginGizmoType.Chest, "CaOut_Oasis_Chest_Rare_MapVendorCave-12710", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(3028, PluginGizmoType.Chest, "a2dun_Zolt_Random_GoldChest-11759", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(166661, PluginGizmoType.Chest, "Lore_UriksJournal-13083", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(230240, PluginGizmoType.Chest, "Lore_HighlandsChest-10452", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(190915, PluginGizmoType.Chest, "a4dun_spire_CorruptedWallAngel_Column_1-34318", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(193154, PluginGizmoType.Chest, "a4dun_spire_CorruptedWallAngel_Column_2-37722", GizmoTargetTypes.MiscContainer),
				new GizmoEntry(193165, PluginGizmoType.Chest, "a4dun_spire_CorruptedWallAngel_Column_3-39228", GizmoTargetTypes.MiscContainer),
                new GizmoEntry(403041, PluginGizmoType.Chest, "p1_Tgoblin_Greed_Bait-8859", GizmoTargetTypes.MiscContainer),
				//============== Special Interactables ==============//
				//
				
				new GizmoEntry(211999, PluginGizmoType.Switch),
				new GizmoEntry(56686, PluginGizmoType.Switch, "", GizmoTargetTypes.None, 4),
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
				new GizmoEntry(117344, PluginGizmoType.Switch, "a3dun_rmpt_Demon_Elevator_A-3153"),
				new GizmoEntry(341214, PluginGizmoType.Switch, "x1_Bog_CatacombsPortal_BeaconLoc-46725"),
				new GizmoEntry(361364, PluginGizmoType.Switch, "X1_PandExt_SiegeRune-95859"),
				new GizmoEntry(3707, PluginGizmoType.Switch, "caOut_Totem_A"), //Act 2 Restless Sands Totem Switches
				new GizmoEntry(430733, PluginGizmoType.Switch, "px_Wilderness_Camp_TemplarPrisoners-10975", GizmoTargetTypes.Bounty),
                new GizmoEntry(433124, PluginGizmoType.Switch, "px_Bounty_Camp_TrappedAngels-23348", GizmoTargetTypes.Bounty),
                new GizmoEntry(432259, PluginGizmoType.Switch, "px_Highlands_Camp_ResurgentCult_Totem-7343", GizmoTargetTypes.Bounty),
                new GizmoEntry(433402, PluginGizmoType.Switch, "px_Bounty_Camp_Hellportals_Frame-16815", GizmoTargetTypes.Bounty),
                new GizmoEntry(432770, PluginGizmoType.Switch, "px_SpiderCaves_Camp_Cocoon-3772", GizmoTargetTypes.Bounty),

				//
				new GizmoEntry(138989, PluginGizmoType.HealingWell, "", GizmoTargetTypes.Healthwell),
				new GizmoEntry(116807, PluginGizmoType.HealingWell, "a1dun_Leor_BloodWell_A", GizmoTargetTypes.Healthwell),
				new GizmoEntry(373463, PluginGizmoType.PoolOfReflection, "PoolOfReflection", GizmoTargetTypes.PoolOfReflection),
				 
				
				new GizmoEntry(176075, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(176077, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(176074, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(176076, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(260331, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),
				new GizmoEntry(260330, PluginGizmoType.PowerUp, "", GizmoTargetTypes.Shrine),

				new GizmoEntry(330697, PluginGizmoType.PowerUp, "", GizmoTargetTypes.PylonShrine),//Channeling
				new GizmoEntry(330699, PluginGizmoType.PowerUp, "", GizmoTargetTypes.PylonShrine),//Speed
				new GizmoEntry(330698, PluginGizmoType.PowerUp, "", GizmoTargetTypes.PylonShrine),//Shield
				new GizmoEntry(330695, PluginGizmoType.PowerUp, "", GizmoTargetTypes.PylonShrine),//Power
				new GizmoEntry(330696, PluginGizmoType.PowerUp, "", GizmoTargetTypes.PylonShrine),//Conduit


                //obstacles
                new GizmoEntry(105478, PluginGizmoType.Switch, "a1dun_Leor_Spike_Spawner_Switch", GizmoTargetTypes.Obstacle),//Conduit
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
