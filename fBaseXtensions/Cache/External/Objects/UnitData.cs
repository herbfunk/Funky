using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Cache.External.Objects
{
	public class UnitData
	{
		public HashSet<UnitEntry> UnitEntries { get; set; }
		public HashSet<UnitPetEntry> UnitPetEntries { get; set; } 
		public UnitData()
		{

			#region Pets
			UnitPetEntries = new HashSet<UnitPetEntry>
			{
				new UnitPetEntry(158941, PetTypes.DEMONHUNTER_SpikeTrap),
				new UnitPetEntry(111330, PetTypes.DEMONHUNTER_SpikeTrap),

				new UnitPetEntry(169123, PetTypes.MONK_MysticAlly),
				new UnitPetEntry(123885, PetTypes.MONK_MysticAlly),
				new UnitPetEntry(169890, PetTypes.MONK_MysticAlly),
				new UnitPetEntry(168878, PetTypes.MONK_MysticAlly),
				new UnitPetEntry(169891, PetTypes.MONK_MysticAlly),
				new UnitPetEntry(169077, PetTypes.MONK_MysticAlly),
				new UnitPetEntry(169904, PetTypes.MONK_MysticAlly),
				new UnitPetEntry(169907, PetTypes.MONK_MysticAlly),
				new UnitPetEntry(169906, PetTypes.MONK_MysticAlly),
				new UnitPetEntry(169908, PetTypes.MONK_MysticAlly),
				new UnitPetEntry(169905, PetTypes.MONK_MysticAlly),
				new UnitPetEntry(169909, PetTypes.MONK_MysticAlly),

				new UnitPetEntry(179780, PetTypes.WITCHDOCTOR_Gargantuan),
				new UnitPetEntry(179778, PetTypes.WITCHDOCTOR_Gargantuan),
				new UnitPetEntry(179772, PetTypes.WITCHDOCTOR_Gargantuan),
				new UnitPetEntry(179779, PetTypes.WITCHDOCTOR_Gargantuan),
				new UnitPetEntry(179776, PetTypes.WITCHDOCTOR_Gargantuan),
				new UnitPetEntry(122305, PetTypes.WITCHDOCTOR_Gargantuan),

				new UnitPetEntry(110959, PetTypes.WITCHDOCTOR_ZombieDogs),
				new UnitPetEntry(103235, PetTypes.WITCHDOCTOR_ZombieDogs),
				new UnitPetEntry(103215, PetTypes.WITCHDOCTOR_ZombieDogs),
				new UnitPetEntry(105763, PetTypes.WITCHDOCTOR_ZombieDogs),
				new UnitPetEntry(103217, PetTypes.WITCHDOCTOR_ZombieDogs),
				new UnitPetEntry(51353, PetTypes.WITCHDOCTOR_ZombieDogs),

				new UnitPetEntry(178664, PetTypes.DEMONHUNTER_Pet),
				new UnitPetEntry(173827, PetTypes.DEMONHUNTER_Pet),
				new UnitPetEntry(133741, PetTypes.DEMONHUNTER_Pet),
				new UnitPetEntry(159144, PetTypes.DEMONHUNTER_Pet),
				new UnitPetEntry(181748, PetTypes.DEMONHUNTER_Pet),
				new UnitPetEntry(159098, PetTypes.DEMONHUNTER_Pet),

				new UnitPetEntry(80745, PetTypes.WIZARD_Hydra),
				new UnitPetEntry(81515, PetTypes.WIZARD_Hydra),
				new UnitPetEntry(82111, PetTypes.WIZARD_Hydra),
				new UnitPetEntry(82972, PetTypes.WIZARD_Hydra),
				new UnitPetEntry(82109, PetTypes.WIZARD_Hydra),
				new UnitPetEntry(83959, PetTypes.WIZARD_Hydra),
				new UnitPetEntry(325807, PetTypes.WIZARD_Hydra),

				new UnitPetEntry(150027, PetTypes.DEMONHUNTER_Sentry),
				new UnitPetEntry(150026, PetTypes.DEMONHUNTER_Sentry),
				new UnitPetEntry(168815, PetTypes.DEMONHUNTER_Sentry),
				new UnitPetEntry(150024, PetTypes.DEMONHUNTER_Sentry),
				new UnitPetEntry(150025, PetTypes.DEMONHUNTER_Sentry),
				new UnitPetEntry(141402, PetTypes.DEMONHUNTER_Sentry),

				new UnitPetEntry(87189, PetTypes.WITCHDOCTOR_Fetish),
				new UnitPetEntry(89933, PetTypes.WITCHDOCTOR_Fetish),
				new UnitPetEntry(89934, PetTypes.WITCHDOCTOR_Fetish),
				new UnitPetEntry(90072, PetTypes.WITCHDOCTOR_Fetish),
			};
			
			#endregion

			UnitEntries = new HashSet<UnitEntry>
			{
				new UnitEntry(5984, UnitFlags.TreasureGoblin, "treasureGoblin_A-12185"),
				new UnitEntry(5985, UnitFlags.TreasureGoblin, "treasureGoblin_B-16978"),
				new UnitEntry(5987, UnitFlags.TreasureGoblin, "treasureGoblin_C-8189"),
				new UnitEntry(5988, UnitFlags.TreasureGoblin, "treasureGoblin_D-6951"),

				// ================ ACT ONE ====================
				#region Act One
		
				new UnitEntry(86624, UnitFlags.Boss, "Adventurer_D_TemplarIntroUnique-17287"),
				new UnitEntry(3526, UnitFlags.Boss, "Butcher-14105"),
				new UnitEntry(51341, UnitFlags.Boss, "SpiderQueen-9649"),
				new UnitEntry(5350, UnitFlags.Boss, "SkeletonKing"),
				new UnitEntry(98879, UnitFlags.Boss | UnitFlags.Fast, "graveDigger_Warden-18500"),

				new UnitEntry(218364, UnitFlags.Rare, "Skeleton_A_Unique_03-1788"),
				new UnitEntry(189906, UnitFlags.Rare, "TriuneVesselActivated_A_Unique_Tower_Of_Power-20073"),
				new UnitEntry(361349, UnitFlags.Rare, "Unique_CaptainDaltyn_AdventureMode-3477"),
				new UnitEntry(210591, UnitFlags.Normal, "a2dun_Spider_EggSack_Clusters_E-9340"),
				new UnitEntry(171193, UnitFlags.Normal, "a2dun_Spider_EggSack_Clusters-9339"),
				new UnitEntry(218458, UnitFlags.Rare, "Spider_Poison_A_Unique_01-8192"),
				new UnitEntry(176054, UnitFlags.Normal, "zombieCrawler_Spawner_B-4613"),
				new UnitEntry(76856, UnitFlags.Normal, "ZombieCrawler_Custom_A-5374"),
				new UnitEntry(4308, UnitFlags.Normal, "GoatWarrior_piece_spear-3220"),
				new UnitEntry(81982, UnitFlags.Normal, "FleshPitFlyerSpawner_B_Event_FarmAmbush-6604"),
				new UnitEntry(81954, UnitFlags.Normal | UnitFlags.Flying, "FleshPitFlyer_B_Event_Ambusher-6670"),
				new UnitEntry(260231, UnitFlags.Unique | UnitFlags.Fast, "FleshPitFlyer_B_FarmhouseAmbush_Unique-7236"),
				new UnitEntry(365425, UnitFlags.Unique, "Skeleton_B_Unique_01-7935"),
				new UnitEntry(218307, UnitFlags.Unique, "Corpulent_A_Unique_01-1428"),
				new UnitEntry(169533, UnitFlags.Unique, "Goatman_Shaman_B_Unique_MysticWagon-6395"),
				new UnitEntry(129222, UnitFlags.Normal, "Goatman_Shaman_Highlands_Barricade-5657"),
				new UnitEntry(230832, UnitFlags.Normal, "Shield_Skeleton_Jail-15038"),
				new UnitEntry(230834, UnitFlags.Normal | UnitFlags.Ranged, "SkeletonArcher_Jail-15043"),
				new UnitEntry(4203, UnitFlags.Normal, "Ghoul_C-15058"),
				new UnitEntry(434, UnitFlags.Rare, "skeleton_twoHander_B-15280"),
				new UnitEntry(5389, UnitFlags.Normal | UnitFlags.Ranged | UnitFlags.Summoner, "SkeletonSummoner_C-15686"),
				new UnitEntry(3849, UnitFlags.Normal | UnitFlags.Grotesque, "Corpulent_C-15764"),
				new UnitEntry(202856, UnitFlags.Normal, "electricEel_B-16203"),
				new UnitEntry(218536, UnitFlags.AdventureModeBoss, "Beast_A_Unique_02-12159"),
				new UnitEntry(365335, UnitFlags.AdventureModeBoss, "Beast_A_Unique_03-4661"),
				new UnitEntry(3337, UnitFlags.Rare, "Beast_A-4362"),
				new UnitEntry(3338, UnitFlags.Rare, "Beast_B-19030"),
				new UnitEntry(218308, UnitFlags.Grotesque | UnitFlags.AdventureModeBoss, "Corpulent_A_Unique_02-2709"),
				new UnitEntry(365450, UnitFlags.Grotesque | UnitFlags.AdventureModeBoss, "Corpulent_A_Unique_03-9354"),
				new UnitEntry(3847, UnitFlags.Normal | UnitFlags.Grotesque, "Corpulent_A-457"),
				new UnitEntry(218405, UnitFlags.Grotesque | UnitFlags.AdventureModeBoss, "Corpulent_B_Unique_01-1470"),
				new UnitEntry(3848, UnitFlags.Normal | UnitFlags.Grotesque, "Corpulent_B-1087"),
				new UnitEntry(77090, UnitFlags.Fast | UnitFlags.AdventureModeBoss, "CryptChild_A_FamilyTree_Son-5518"),
				new UnitEntry(3893, UnitFlags.Normal, "CryptChild_A-8219"),
				new UnitEntry(3894, UnitFlags.Normal, "CryptChild_B-1792"),
				new UnitEntry(218362, UnitFlags.Unique | UnitFlags.Flying, "FleshPitFlyer_A_Unique_02-985"),
				new UnitEntry(4156, UnitFlags.Rare | UnitFlags.Flying, "FleshPitFlyer_A-536"),
				new UnitEntry(4157, UnitFlags.Normal | UnitFlags.Flying, "FleshPitFlyer_B-4355"),
				new UnitEntry(195747, UnitFlags.Normal | UnitFlags.Flying, "FleshPitFlyer_E-18877"),
				new UnitEntry(104424, UnitFlags.Normal | UnitFlags.Flying, "FleshPitFlyer_Leoric_Inferno-10916"),
				new UnitEntry(4152, UnitFlags.Normal | UnitFlags.Summoner, "FleshPitFlyerSpawner_A-6429"),
				new UnitEntry(4153, UnitFlags.Normal | UnitFlags.Summoner, "FleshPitFlyerSpawner_B-4352"),
				new UnitEntry(4154, UnitFlags.Normal | UnitFlags.Summoner, "FleshPitFlyerSpawner_C-12440"),
				new UnitEntry(136943, UnitFlags.Normal | UnitFlags.Flying, "Ghost_A_NoRun-5601"),
				new UnitEntry(218441, UnitFlags.Unique | UnitFlags.Flying, "Ghost_A_Unique_02-5704"),
				new UnitEntry(156353, UnitFlags.Unique | UnitFlags.Flying, "Ghost_A_Unique_Chancellor-12860"),
				new UnitEntry(85971, UnitFlags.Unique | UnitFlags.Fast | UnitFlags.Flying, "Ghost_A_Unique_House1000Undead-3742"),
				new UnitEntry(370, UnitFlags.Rare | UnitFlags.Fast | UnitFlags.Flying, "Ghost_A-3368"),
				new UnitEntry(4201, UnitFlags.Normal, "Ghoul_A-7213"),
				new UnitEntry(129000, UnitFlags.Rare | UnitFlags.Fast, "Goatman_Melee_A_Ghost-8170"),
				new UnitEntry(365330, UnitFlags.Unique, "Goatman_Melee_A_Unique_03-5431"),
				new UnitEntry(4282, UnitFlags.Normal, "Goatman_Melee_A-4378"),
				new UnitEntry(129012, UnitFlags.Rare, "Goatman_Melee_B_Ghost-9549"),
				new UnitEntry(218469, UnitFlags.Fast | UnitFlags.AdventureModeBoss, "Goatman_Melee_B_Unique_01-4436"),
				new UnitEntry(4283, UnitFlags.Normal, "Goatman_Melee_B-8932"),
				new UnitEntry(4286, UnitFlags.Normal | UnitFlags.Ranged, "Goatman_Ranged_A-4379"),
				new UnitEntry(4287, UnitFlags.Normal | UnitFlags.Ranged, "Goatman_Ranged_B-8930"),
				new UnitEntry(218508, UnitFlags.Unique, "Goatman_Shaman_A_Unique_01-7630"),
				new UnitEntry(4290, UnitFlags.Normal | UnitFlags.Ranged, "Goatman_Shaman_A-9279"),
				new UnitEntry(76676, UnitFlags.Unique, "Goatman_Shaman_B_Unique-11733"),
				new UnitEntry(375, UnitFlags.Normal | UnitFlags.Ranged, "Goatman_Shaman_B-4749"),
				new UnitEntry(366975, UnitFlags.AdventureModeBoss, "Goatman_Shaman_C_Unique_01-11475"),
				new UnitEntry(366981, UnitFlags.AdventureModeBoss, "Goatman_Shaman_C_Unique_02-10983"),
				new UnitEntry(255704, UnitFlags.Unique | UnitFlags.Fast, "GoatMutant_Ranged_A_Unique_Uber-3308"),
				new UnitEntry(218206, UnitFlags.Fast | UnitFlags.AdventureModeBoss, "graveDigger_B_Ghost_Unique-9675"),
				new UnitEntry(131278, UnitFlags.Normal, "graveDigger_B_Ghost-8312"),
				new UnitEntry(131280, UnitFlags.Normal, "graveRobber_A_Ghost-8239"),
				new UnitEntry(4372, UnitFlags.Rare, "graveRobber_A-11397"),
				new UnitEntry(4373, UnitFlags.Rare, "graveRobber_B-22477"),
				new UnitEntry(174013, UnitFlags.Rare | UnitFlags.Fast, "graveRobber_C_Nigel-23714"),
				new UnitEntry(4376, UnitFlags.Normal, "graveRobber_C-23602"),
				new UnitEntry(4377, UnitFlags.Normal, "graveRobber_D-23646"),
				new UnitEntry(4564, UnitFlags.Normal, "Lamprey_A-684"),
				new UnitEntry(4982, UnitFlags.Rare | UnitFlags.Ranged, "QuillDemon_A-8892"),
				new UnitEntry(4983, UnitFlags.Normal | UnitFlags.Ranged, "QuillDemon_B-6082"),
				new UnitEntry(364563, UnitFlags.Normal | UnitFlags.Ranged, "QuillDemon_FastAttack_A-5555"),
				new UnitEntry(5235, UnitFlags.Normal | UnitFlags.Fast | UnitFlags.Burrowing, "Scavenger_A-5765"),
				new UnitEntry(5236, UnitFlags.Normal | UnitFlags.Fast | UnitFlags.Burrowing, "Scavenger_B-5021"),
				new UnitEntry(5275, UnitFlags.Normal, "Shield_Skeleton_A-3100"),
				new UnitEntry(5276, UnitFlags.Rare, "Shield_Skeleton_B-4932"),
				new UnitEntry(112134, UnitFlags.Rare, "Shield_Skeleton_NephChamp-5788"),
				new UnitEntry(115403, UnitFlags.Unique, "Skeleton_A_Cain_Unique-7172"),
				new UnitEntry(87012, UnitFlags.Normal, "Skeleton_A_Cain-7076"),
				new UnitEntry(105863, UnitFlags.Normal, "Skeleton_A_TemplarIntro_NoWander-16176"),
				new UnitEntry(104725, UnitFlags.Normal, "Skeleton_A_TemplarIntro-16340"),
				new UnitEntry(5393, UnitFlags.Normal, "Skeleton_A-3088"),
				new UnitEntry(5395, UnitFlags.Normal, "Skeleton_B-8092"),
				new UnitEntry(80652, UnitFlags.Normal, "Skeleton_Cain-7031"),
				new UnitEntry(5411, UnitFlags.Normal, "skeleton_twoHander_A-982"),
				new UnitEntry(218400, UnitFlags.Ranged | UnitFlags.AdventureModeBoss, "SkeletonArcher_A_Unique_01-2825"),
				new UnitEntry(5346, UnitFlags.Normal | UnitFlags.Ranged, "SkeletonArcher_A-3093"),
				new UnitEntry(5347, UnitFlags.Normal | UnitFlags.Ranged, "SkeletonArcher_B-5230"),
				new UnitEntry(179343, UnitFlags.Normal | UnitFlags.Ranged, "SkeletonArcher_F-4928"),
				new UnitEntry(104728, UnitFlags.Normal | UnitFlags.Ranged, "SkeletonSummoner_A_TemplarIntro-16466"),
				new UnitEntry(5387, UnitFlags.Normal | UnitFlags.Ranged | UnitFlags.Summoner, "SkeletonSummoner_A-3074"),
				new UnitEntry(5388, UnitFlags.Normal | UnitFlags.Ranged | UnitFlags.Summoner, "SkeletonSummoner_B-24350"),
				new UnitEntry(100956, UnitFlags.Normal, "Spawner_Leor_Iron_Maiden-14866"),
				new UnitEntry(122367, UnitFlags.Normal, "Spider_A_SpiderQueen_Minion-9756"),
				new UnitEntry(218448, UnitFlags.Fast | UnitFlags.AdventureModeBoss, "Spider_A_Unique_01-8699"),
				new UnitEntry(5474, UnitFlags.Normal, "Spider_A-7259"),
				new UnitEntry(218462, UnitFlags.Unique | UnitFlags.Fast, "Spider_Poison_A_Unique_02-7479"),
				new UnitEntry(166726, UnitFlags.Normal, "Spider_Poison_A-7111"),
				new UnitEntry(370696, UnitFlags.Normal, "Spiderling_A_CursedChest-17137"),
				new UnitEntry(218456, UnitFlags.Unique | UnitFlags.Fast, "Spiderling_A_Unique_01-6619"),
				new UnitEntry(5467, UnitFlags.Normal, "Spiderling_A-7140"),
				new UnitEntry(176907, UnitFlags.Normal, "trDun_Cath_Skeleton_SummoningMachine-1008"),
				new UnitEntry(6023, UnitFlags.Normal, "trist_Urn_Tall-2629"),
				new UnitEntry(218674, UnitFlags.Unique | UnitFlags.Fast, "Triune_Berserker_A_Unique_02-13348"),
				new UnitEntry(6052, UnitFlags.Normal, "Triune_Berserker_A-12360"),
				new UnitEntry(6059, UnitFlags.Normal, "Triune_Summonable_A-10084"),
				new UnitEntry(145745, UnitFlags.Normal, "TriuneCultist_A_Templar-16276"),
				new UnitEntry(366998, UnitFlags.AdventureModeBoss, "TriuneCultist_A_Unique_03-12855"),
				new UnitEntry(260237, UnitFlags.Unique, "TriuneCultist_A_VendorRescue_Unique-12735"),
				new UnitEntry(6024, UnitFlags.Normal, "TriuneCultist_A-9509"),
				new UnitEntry(366990, UnitFlags.Unique, "TriuneCultist_B_Unique_01-10375"),
				new UnitEntry(105959, UnitFlags.Rare, "TriuneCultist_C_TortureLeader-14148"),
				new UnitEntry(178213, UnitFlags.Normal, "TriuneCultist_E-9420"),
				new UnitEntry(218662, UnitFlags.Ranged | UnitFlags.AdventureModeBoss, "TriuneSummoner_A_Unique_01-23267"),
				new UnitEntry(131131, UnitFlags.Unique | UnitFlags.Ranged | UnitFlags.Fast, "TriuneSummoner_A_Unique_SwordOfJustice-22011"),
				new UnitEntry(6035, UnitFlags.Normal | UnitFlags.Ranged | UnitFlags.Summoner, "TriuneSummoner_A-10080"),
				new UnitEntry(6046, UnitFlags.Normal, "TriuneVessel_A-12145"),
				new UnitEntry(6042, UnitFlags.Normal, "TriuneVesselActivated_A-13288"),
				new UnitEntry(166452, UnitFlags.Normal, "trOut_Highlands_Goatmen_SummoningMachine_A_Node-10023"),
				new UnitEntry(76953, UnitFlags.Unique, "Unburied_A_Unique-12652"),
				new UnitEntry(6356, UnitFlags.Normal, "Unburied_A-3007"),
				new UnitEntry(365906, UnitFlags.AdventureModeBoss, "Unburied_C_Unique_01-3328"),
				new UnitEntry(6359, UnitFlags.Normal, "Unburied_C-12299"),
				new UnitEntry(99556, UnitFlags.Normal, "WitherMoth_A_Hidden-8944"),
				new UnitEntry(6500, UnitFlags.Normal, "WitherMoth_A-6229"),
				new UnitEntry(6572, UnitFlags.Normal | UnitFlags.AvoidanceSummoner, "WoodWraith_A_01-4841"),
				new UnitEntry(139454, UnitFlags.Normal | UnitFlags.AvoidanceSummoner, "WoodWraith_A_02-4412"),
				new UnitEntry(139456, UnitFlags.Normal | UnitFlags.AvoidanceSummoner, "WoodWraith_A_03-3352"),
				new UnitEntry(170324, UnitFlags.Normal | UnitFlags.AvoidanceSummoner, "WoodWraith_B_01-9529"),
				new UnitEntry(170325, UnitFlags.Normal | UnitFlags.AvoidanceSummoner, "WoodWraith_B_02-10029"),
				new UnitEntry(495, UnitFlags.Normal | UnitFlags.AvoidanceSummoner, "WoodWraith_B_03-9685"),
				new UnitEntry(332432, UnitFlags.Ranged | UnitFlags.AdventureModeBoss, "x1_devilshand_unique_SkeletonSummoner_B-8145"),
				new UnitEntry(294664, UnitFlags.Normal, "x1_Ghoul_A_Challenge-1856"),
				new UnitEntry(365632, UnitFlags.AdventureModeBoss, "x1_SpeedKill_SkeletonKing-776"),
				new UnitEntry(77085, UnitFlags.AdventureModeBoss, "Zombie_A_FamilyTree_Father-5481"),
				new UnitEntry(6652, UnitFlags.Normal, "Zombie_A-477"),
				new UnitEntry(6653, UnitFlags.Normal, "Zombie_B-5575"),
				new UnitEntry(6654, UnitFlags.Normal, "Zombie_C-10516"),
				new UnitEntry(204256, UnitFlags.Normal, "Zombie_E-4755"),
				new UnitEntry(90453, UnitFlags.Normal, "Zombie_Inferno_C-14953"),
				new UnitEntry(6632, UnitFlags.Normal, "ZombieCrawler_A-560"),
				new UnitEntry(6633, UnitFlags.Normal, "ZombieCrawler_B-8390"),
				new UnitEntry(218367, UnitFlags.Normal, "ZombieCrawler_Barricade_A-647"),
				new UnitEntry(6634, UnitFlags.Normal, "ZombieCrawler_C-11088"),
				new UnitEntry(123160, UnitFlags.Normal, "ZombieCrawler_Custom_B-5786"),
				new UnitEntry(113949, UnitFlags.Normal, "ZombieCrawler_Custom_C-13275"),
				new UnitEntry(218813, UnitFlags.Normal, "ZombieCrawler_E-4982"),
				new UnitEntry(85900, UnitFlags.Unique | UnitFlags.Fast | UnitFlags.Summoner, "ZombieFemale_A_BlacksmithA-8082"),
				new UnitEntry(77087, UnitFlags.AdventureModeBoss, "ZombieFemale_A_FamilyTree_Mother-5494"),
				new UnitEntry(219725, UnitFlags.Normal, "ZombieFemale_A_TristramQuest_Unique-1359"),
				new UnitEntry(108444, UnitFlags.Normal, "ZombieFemale_A_TristramQuest-1604"),
				new UnitEntry(6639, UnitFlags.Normal | UnitFlags.Summoner, "ZombieFemale_B-5768"),
				new UnitEntry(6640, UnitFlags.Normal | UnitFlags.Summoner, "ZombieFemale_C-4599"),
				new UnitEntry(364508, UnitFlags.Normal, "ZombieFemale_Spitter_A-5560"),
				new UnitEntry(176889, UnitFlags.Unique, "ZombieFemale_Unique_WretchedQueen-2822"),
				new UnitEntry(203121, UnitFlags.Normal, "ZombieSkinny_A_LeahInn-980"),
				new UnitEntry(209608, UnitFlags.Fast | UnitFlags.AdventureModeBoss, "ZombieSkinny_A_Unique_01-1337"),
				new UnitEntry(6644, UnitFlags.Normal, "ZombieSkinny_A-481"),
				new UnitEntry(6646, UnitFlags.Normal, "ZombieSkinny_B-459"),
				new UnitEntry(6647, UnitFlags.Normal, "ZombieSkinny_C-11485"),
				new UnitEntry(218339, UnitFlags.Normal, "ZombieSkinny_Custom_A-280"),
				new UnitEntry(6651, UnitFlags.Normal, "ZombieSkinny_D-4597"),
				new UnitEntry(139757, UnitFlags.Unique, "Nephalem_Ghost_A_DrownedTemple_Martyr_Skeleton-26979"),
				new UnitEntry(139715, UnitFlags.Unique | UnitFlags.Fast, "Nephalem_Ghost_A_DrownedTemple_Martyr2_Skeleton-27128"),
				new UnitEntry(139756, UnitFlags.Unique | UnitFlags.Fast, "Nephalem_Ghost_A_DrownedTemple_Martyr3_Skeleton-27130"),
				new UnitEntry(139713, UnitFlags.Unique, "Nephalem_Ghost_A_DrownedTemple_Martyr1_Skeleton-27140"),
				new UnitEntry(90008, UnitFlags.Normal, "TownAttackCultistMelee-27395"),
				new UnitEntry(90367, UnitFlags.Normal | UnitFlags.Ranged, "TownAttack_Cultist-28100"),
				new UnitEntry(178619, UnitFlags.Unique | UnitFlags.Ranged, "TownAttack_Summoner_Unique-28361"),
				new UnitEntry(178300, UnitFlags.Rare | UnitFlags.Fast, "TownAttack_Berserker-28517"),
				new UnitEntry(178297, UnitFlags.Normal | UnitFlags.Ranged, "TownAttack_Summoner-28528"),
				new UnitEntry(186039, UnitFlags.Normal | UnitFlags.Ranged, "TriuneSummoner_A_CainEvent-29198"),

	#endregion
				
				// ================ ACT TWO ====================
				new UnitEntry(218947, UnitFlags.Unique | UnitFlags.Burrowing | UnitFlags.Worm, ""),
				new UnitEntry(144400, UnitFlags.Unique | UnitFlags.Burrowing | UnitFlags.Worm, ""),
				new UnitEntry(5088, UnitFlags.Normal | UnitFlags.Burrowing | UnitFlags.Worm, "Rockworm_A"),
				new UnitEntry(5090, UnitFlags.Normal | UnitFlags.Burrowing | UnitFlags.Worm, "Rockworm_D"),
				new UnitEntry(203048, UnitFlags.Normal | UnitFlags.Burrowing | UnitFlags.Worm, "Rockworm_A3_Crater"),
				new UnitEntry(220777, UnitFlags.Unique | UnitFlags.Burrowing | UnitFlags.Worm, "Rockworm_A3_Crater_Unique_01"),
				new UnitEntry(220851, UnitFlags.Unique | UnitFlags.Burrowing | UnitFlags.Worm, "Rockworm_A3_Crater_Unique_02"),

				new UnitEntry(3384, UnitFlags.Normal | UnitFlags.Burrowing | UnitFlags.Flying, "Bloodhawk_A"),
				new UnitEntry(222011, UnitFlags.Unique | UnitFlags.Burrowing | UnitFlags.Flying, "Bloodhawk_A_Unique_01"),
				new UnitEntry(222385, UnitFlags.Unique | UnitFlags.Burrowing | UnitFlags.Flying, "Bloodhawk_A_Unique_02"),
				new UnitEntry(3385, UnitFlags.Normal | UnitFlags.Burrowing | UnitFlags.Flying, "Bloodhawk_B"),

				new UnitEntry(3980, UnitFlags.Normal | UnitFlags.ReflectiveMissle | UnitFlags.Flying, "DuneDervish_A"),
				new UnitEntry(140947, UnitFlags.Unique | UnitFlags.ReflectiveMissle | UnitFlags.Flying, "DuneDervish_A_DyingManMine"),
				new UnitEntry(3981, UnitFlags.Normal | UnitFlags.ReflectiveMissle | UnitFlags.Flying, "DuneDervish_B"),
				new UnitEntry(256000, UnitFlags.Unique | UnitFlags.ReflectiveMissle | UnitFlags.Flying, "DuneDervish_B_Unique_Uber"),
				new UnitEntry(3982, UnitFlags.Normal | UnitFlags.ReflectiveMissle | UnitFlags.Flying, "DuneDervish_C"),

				new UnitEntry(5191, UnitFlags.Normal | UnitFlags.ReflectiveMissle | UnitFlags.Burrowing, "SandMonster_A"),
				new UnitEntry(226849, UnitFlags.Unique | UnitFlags.ReflectiveMissle | UnitFlags.Burrowing, "SandMonster_A_Eternal_Guardian_ZoltBoss"),
				new UnitEntry(116299, UnitFlags.Normal | UnitFlags.ReflectiveMissle | UnitFlags.Burrowing, "SandMonster_A_Gauntlet"),
				new UnitEntry(164502, UnitFlags.Normal | UnitFlags.ReflectiveMissle | UnitFlags.Burrowing, "SandMonster_A_Head_Guardian"),
				new UnitEntry(219832, UnitFlags.Unique | UnitFlags.ReflectiveMissle | UnitFlags.Burrowing, "sandMonster_A_PortalRoulette"),
				new UnitEntry(5192, UnitFlags.Normal | UnitFlags.ReflectiveMissle | UnitFlags.Burrowing, "SandMonster_B"),
				new UnitEntry(222413, UnitFlags.Unique | UnitFlags.ReflectiveMissle | UnitFlags.Burrowing, "SandMonster_B_Unique_01"),
				new UnitEntry(5193, UnitFlags.Normal | UnitFlags.ReflectiveMissle | UnitFlags.Burrowing, "SandMonster_C"),
				new UnitEntry(222523, UnitFlags.Unique | UnitFlags.ReflectiveMissle | UnitFlags.Burrowing, "SandMonster_C_Unique_01"),
				new UnitEntry(5194, UnitFlags.Normal | UnitFlags.ReflectiveMissle | UnitFlags.Burrowing, "SandMonster_D"),

				new UnitEntry(5187, UnitFlags.Normal | UnitFlags.Burrowing | UnitFlags.Fast, "Sandling_A"),
				new UnitEntry(5188, UnitFlags.Normal | UnitFlags.Burrowing | UnitFlags.Fast, "Sandling_B"),
				new UnitEntry(5189, UnitFlags.Normal | UnitFlags.Burrowing | UnitFlags.Fast, "Sandling_C"),

				new UnitEntry(5199, UnitFlags.Normal | UnitFlags.Burrowing, "SandShark_A"),
				new UnitEntry(221402, UnitFlags.Unique | UnitFlags.Burrowing, "SandShark_A_Unique_01"),
				new UnitEntry(156738, UnitFlags.Unique | UnitFlags.Burrowing, "SandShark_B_SewerSharkEvent"),

				new UnitEntry(5432, UnitFlags.Normal | UnitFlags.Stealthable, "SnakeMan_Melee_A"),
				new UnitEntry(5433, UnitFlags.Normal | UnitFlags.Stealthable, "SnakeMan_Melee_B"),
				new UnitEntry(5434, UnitFlags.Normal | UnitFlags.Stealthable, "SnakeMan_Melee_C"),
				new UnitEntry(213842, UnitFlags.Normal | UnitFlags.Stealthable, "SnakeMan_Melee_A_AdriaRescue"),
				new UnitEntry(160525, UnitFlags.Normal | UnitFlags.Stealthable, "SnakeMan_Melee_A_EscapeFromCaldeum"),
				new UnitEntry(60816, UnitFlags.Normal | UnitFlags.Stealthable, "SnakeMan_Melee_C_Khamsin"),

				//
				// ================ ACT THREE ====================
				new UnitEntry(4093, UnitFlags.Normal|UnitFlags.SucideBomber, "FallenLunatic_A"),
				new UnitEntry(4094, UnitFlags.Normal|UnitFlags.SucideBomber, "FallenLunatic_A"),
				new UnitEntry(4095, UnitFlags.Normal|UnitFlags.SucideBomber, "FallenLunatic_A"),
				new UnitEntry(231356, UnitFlags.Normal|UnitFlags.SucideBomber, "FallenLunatic_A"),

				// ================ ACT FOUR ====================
				new UnitEntry(343767, UnitFlags.AdventureModeBoss|UnitFlags.MalletLord, "X1_LR_Boss_MalletDemon"),
				new UnitEntry(219751, UnitFlags.Unique|UnitFlags.MalletLord, "MalletDemon_A_Unique"),
				new UnitEntry(219736, UnitFlags.Unique|UnitFlags.MalletLord, "MalletDemon_A_Unique_01"),
				new UnitEntry(106709, UnitFlags.Normal|UnitFlags.MalletLord, "MalletDemon_A_Unique_01"),

				new UnitEntry(106714, UnitFlags.Normal|UnitFlags.Debuffing, "TerrorDemon_A"),
				new UnitEntry(196102, UnitFlags.Unique|UnitFlags.Debuffing, "TerrorDemon_A_Unique_1000Monster"),
				new UnitEntry(256034, UnitFlags.Unique|UnitFlags.Debuffing, "TerrorDemon_A_Unique_Uber"),

				new UnitEntry(5508, UnitFlags.Normal|UnitFlags.Debuffing|UnitFlags.Flying, "Succubus_A"),
				new UnitEntry(209596, UnitFlags.Unique|UnitFlags.Debuffing|UnitFlags.Flying, "Succubus_A_Unique_01"),
				new UnitEntry(152679, UnitFlags.Normal|UnitFlags.Debuffing|UnitFlags.Flying, "Succubus_B"),
				new UnitEntry(219673, UnitFlags.Unique|UnitFlags.Debuffing|UnitFlags.Flying, "Succubus_C_Unique_01"),
				new UnitEntry(152535, UnitFlags.Normal|UnitFlags.Debuffing|UnitFlags.Flying, "Succubus_DaughterOfPain"),

				// ================ ACT FIVE ====================

				new UnitEntry(4080, UnitFlags.Normal|UnitFlags.Revivable|UnitFlags.Fast, "FallenGrunt_A"),
				new UnitEntry(4083, UnitFlags.Normal|UnitFlags.Revivable|UnitFlags.Fast, "FallenGrunt_B"),
				new UnitEntry(4084, UnitFlags.Normal|UnitFlags.Revivable|UnitFlags.Fast, "FallenGrunt_C"),
				new UnitEntry(4085, UnitFlags.Normal|UnitFlags.Revivable|UnitFlags.Fast, "FallenGrunt_D"),


			};

		}

		public void ClearCollections()
		{
			UnitEntries.Clear();
			UnitPetEntries.Clear();

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
