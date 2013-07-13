using System;
using System.Linq;
using Zeta;
using System.Collections.Generic;
using Zeta.Common;
using Zeta.Internals.SNO;
using Zeta.Internals.Actors;
using System.Windows;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  #region SNO Priority Values
		  // Dictionary for priorities, like the skeleton summoner cos it keeps bringing new stuff
		  private static readonly Dictionary<int, int> dictActorSNOPriority=new Dictionary<int, int> { 
            // Wood wraiths all this line (495 & 496 & 6572 & 139454 & 139456 & 170324 & 170325)
            {495, 901}, {496, 901}, {6572, 901}, {139454, 901}, {139456, 901}, {170324, 901}, {170325, 901},
            // Fallen Shaman prophets goblin Summoners (365 & 4100)
            {365, 1901}, {4100, 1901}, {4409, 1901}, {4098, 1901}, {4099,1901},
            // The annoying grunts summoned by the above
            {4084, -401}, {4083, -401}, {4080, -401}, {4085, -401},
            // Fallen Champions (Big Guys who SMASH!)
            {4070, 501}, {4071, 501},
            //A2 Foul Conjurer
            {6038,501},
            //Dervish (Spinning AoE monsters)
            {3980, 501}, {3981, 501}, {3982,501},
            //Sand Sharks
            {5199, -401},
            //A2 Birds (Attacking but is still in the air!)
            {3384, -401}, {3385, -401},
            // Wretched mothers that summon zombies in act 1 (6639)
            {6639, 951}, 
            // Fallen lunatic (4095)
            {4095, 2999},
            // Pestilence hands (4738)
            {4738, 1901}, 
             // Maghda and her minions
            {6031, 801}, {178512, 901},
            // Cydaea boss (95250)
            {95250, 1501},
            //Cydaea Spiderlings (137139)
            //{137139, -301},
            // GoatMutantshaman Elite (4304)
            //{4304, 999},
            // GoatMutantshaman (4300)
            //{4300, 901},
            // Succubus (5508)
            //{5508, 801},
            // skeleton summoners (5387, 5388, 5389)
            {5387, 951}, {5388, 951}, {5389, 951}, 
            // Weak skeletons summoned by the above
            {5395, -401},
            // Wasp/Bees - Act 2 annoying flyers (5212) //5208,5209,5210
            {5212, 1501}, {5208,1501}, {5209,1501}, {5210,1501},
            // Act 2 Construct Fire Mage
            {5372, 1501},
            // Act 2 Construct Ice Mage
            {5368, 501},
            // Dark summoner - summons the helion dogs (6035)
            {6035, 501}, 
            // Dark berserkers - has the huge damaging slow hit (6052)
            {6052, 501}, 
            // The giant undead fat grotesques that explode in act 1 (3847)
            {3847, 401}, 
            // Hive pods that summon flyers in act 1 (4152, 4153, 4154)
            {4152, 901}, {4153, 901}, {4154, 901}, 
            // Totems in act 1 that summon the ranged goatmen (166452)
            {166452, 901}, 
            // Totems in act 1 dungeons that summon skeletons (176907)
            {176907, 901},
            //A2 Summoning Towers
            //Telsa
            {208824,501},
            //Construct Summoner (A2 imp respawner)
            {3037, 901},
            //Weak Skeletons
            {5397, -201},
            //Weak Archer Skeletons
            {5349, -101},
				//A2 Slime Gyser
				//{218228, 
         }; 
		  #endregion


		  // NOTE: you don't NEED interactable SNO's listed here. But if they are listed here, *THIS* is the range at which your character will try to walk to within the object 
		  // BEFORE trying to actually "click it". Certain objects need you to get very close, so it's worth having them listed with low interact ranges
		  private static readonly Dictionary<int, int> dictInteractableRange=new Dictionary<int, int> { 
            {56686, 4}, {52685, 4}, {54850, 14},  {54882, 40}, {54908, 4},
         };
		  // 174900 = fire-spewers (demonic forge) in Act 3, 54908 = iron gates  58379 = a2_desolate_large_bones
		  // Navigation obstacles for standard navigation down dungeons etc. to help DB movement
		  // MAKE SURE you add the *SAME* SNO to the "size" dictionary below, and include a reasonable size (keep it smaller rather than larger) for the SNO.
		  private static HashSet<int> hashSNONavigationObstacles=new HashSet<int> {
            174900, 191459, 58379, 204168, 3341, 185391, 123325, 
        };
		  // Destructible things that are very large and need breaking at a bigger distance - eg logstacks, large crates, carts, etc.
		  private static readonly Dictionary<int, int> dictSNOExtendedDestructRange=new Dictionary<int, int> { 
				{195108, 25},{129031, 20}, {210418, 25},{116409, 18},{211959, 25},{197514, 18}, {218228,15},
         }; 
		  // Destructible things that need targeting by a location instead of an ACDGUID (stuff you can't "click on" to destroy in-game)
		  private static readonly HashSet<int> hashDestructableLocationTarget=new HashSet<int> { 
            170657, 116409, 121586, 3016, 121586,80231,58559,58962,62562
         };







		  internal static class SnoCacheLookup
		  {
				// A special list of things *NOT* to use whirlwind on (eg because they move too quick/WW is ineffective on)
				// 4304 = shielder fat dudes in act 3 hell zones
				public static readonly HashSet<int> hashActorSNOWhirlwindIgnore=new HashSet<int> { 
            4304, 5984, 5985, 5987, 5988,
         };

				// Very fast moving mobs (eg wasps), for special skill-selection decisions
				// 5212 = act 2 wasps
				public static readonly HashSet<int> hashActorSNOFastMobs=new HashSet<int> { 
            5212,5208,5209,5210,
            //a2 little grunts
            4085,4084,4083,4080,
         };

				public static readonly HashSet<int> hashActorSNOBurrowableUnits=new HashSet<int>
				{
					 //worms
					 5088,5090,144400,203048,
					 //a2 birds
					 3384,3385,
					 //Sand shark (normal and uniques)
					 5199,221402,156738,
					 //a2 sand imps
					 5189,5188,5187,
					 //a2 sand dwellers (big guys who can reflect range missles)
					 5191, 5192, 5193, 5194,
				};
				public static readonly HashSet<int> hashActorSNOCorpulent=new HashSet<int>
				{
					 //grotesque Exploding Guys
					 3847,3848,3849,3850,218308,218405,113994,195639,

				};
				public static readonly HashSet<int> hashActorSNOSucideBomberUnits=new HashSet<int>
				{
					 //Lunitics
					 4093,4094,4095

				};
				public static readonly HashSet<int> hashActorSNOStealthUnits=new HashSet<int>
				{
					 //A2 Snakemen
					 5432,5433,5434,
					 //A4 Terror Demons
					 106714,

				};
				public static readonly HashSet<int> hashActorSNOReflectiveMissleUnits=new HashSet<int>
				{
					 //a2 Keywarden
					 256000,
					 //a2 Dune Dervish
					 3981,3980,3982,
					 //a2 Sand Monster
					 5191, 5192, 5193, 5194,
				};

				// A list of crappy "summoned mobs" we should always ignore unless they are very close to us, eg "grunts", summoned skeletons etc.
				public static readonly HashSet<int> hashActorSNOShortRangeOnly=new HashSet<int> { 
				4085,4084,4083,4080, 5395, 144315,
         };

				// Special list of "non-unit" SNOs that should be considered a Unit.
				public static readonly HashSet<int> hashActorSNOForceTargetUnit=new HashSet<int> { 
				//a2dun_Cave_SlimeGeyser
				218228,
         };
				public static readonly HashSet<int> hashActorSNOIgnoreLOSCheck=new HashSet<int>
				{
					 //worm bosses
					 218947,144400,
					 //heart of sin
					 193077,
					 //a2 cave slime geyser
					 218228,
				// Siegebreaker (96192), Azmodan (89690), Cydea (95250), Heart-thing (193077), Kulle (80509), Small Belial (220160), Big Belial (3349), Diablo 1 (114917), terror Diablo (133562), burrowed horrow worm (219847)
            96192, 89690, 95250, 193077, 80509, 3349, 114917, 133562,
             // Uber_ZoltunKulle, Uber_SkeletonKingRed,  Uber_SiegebreakerDemon, Uber_Maghda, Uber_Despair, Uber_Gluttony
            256508, 255929, 256187, 256189, 256711, 256709,
				};
				//Special list of "fallen" which is used to skip blacklisting
				public static readonly HashSet<int> hashActorSNOSummonedUnit=new HashSet<int>
        {
            4084, 4083, 4080, 4085,
        };

				// A list of all known SNO's of treasure goblins/bandits etc.
				public static readonly HashSet<int> hashActorSNOGoblins=new HashSet<int> { 
            5984, 5985, 5987, 5988
         };
				// A list of ranged mobs that should be attacked even if they are outside of the routines current kill radius
				//365, 4100 = fallen; 4300, 4304 = goat shaman; 4738 = pestilence; 4299 = goat ranged; 62736, 130794 = demon flyer; 5508 = succubus
				public static readonly HashSet<int> hashActorSNORanged=new HashSet<int> { 
            365, 4100, 4304, 4300, 4738, 4299, 62736, 130794, 5508, 4409, 4099, 4098,
         };
				// A list of bosses in the game, just to make CERTAIN they are treated as elites
				public static readonly HashSet<int> hashBossSNO=new HashSet<int> { 
            // Siegebreaker (96192), Azmodan (89690), Cydea (95250), Heart-thing (193077), Kulle (80509), Small Belial (220160), Big Belial (3349), Diablo 1 (114917), terror Diablo (133562), burrowed horrow worm (219847)
            96192, 89690, 95250, 193077, 80509, 220160, 3349, 114917, 133562, 218947,
            // Diablo shadow clones (needs all of them, there is a male & female version of each class!)
            144001, 144003, 143996, 143994, 
            // Jondar (act 1 dungeons)
            86624, 
             // Uber_ZoltunKulle, Uber_SkeletonKingRed,  Uber_SiegebreakerDemon, Uber_Maghda, Uber_Despair, Uber_Gluttony
            256508, 255929, 256187, 256189, 256711, 256709,
            //A2 KeyWarden,
            256000,
         };
				// Resplendent chest SNO list
				public static readonly HashSet<int> hashSNOContainerResplendant=new HashSet<int> { 
            62873, 95011, 81424, 108230, 111808, 111809, 199583, 109264,101500,
         };
				// Chests/average-level containers that deserve a bit of extra radius (ie - they are more worthwhile to loot than "random" misc/junk containers)
				public static readonly HashSet<int> hashSNOContainerWhitelist=new HashSet<int> { 
            62859, 62865, 62872, 78790, 79016, 94708, 96522, 130170, 108122, 111870, 111947, 213447, 213446, 51300, 179865, 199584, 109264, 212491, 210422, 106165, 
         };
				// A list of SNO's to *FORCE* to type: Item. (BE CAREFUL WITH THIS!).
				// 166943 = infernal key
				public static readonly HashSet<int> hashForceSNOToItemList=new HashSet<int> { 
            166943, 
         };
				// Interactable whitelist - things that need interacting with like special wheels, levers - they will be blacklisted for 30 seconds after one-use
				public static readonly HashSet<int> hashSNOInteractWhitelist=new HashSet<int> { 
            54908, 56686, 54850, 454, 211999, 52685, 54882, 89665, 
         };
				// Three special lists used purely for checking for the existance of a player's summoned mystic ally, gargantuan, or zombie dog
				public static readonly HashSet<int> hashMysticAlly=new HashSet<int> { 169123, 123885, 169890, 168878, 169891, 169077, 169904, 169907, 169906, 169908, 169905, 169909 };
				public static readonly HashSet<int> hashGargantuan=new HashSet<int> { 179780, 179778, 179772, 179779, 179776, 122305 };
				public static readonly HashSet<int> hashZombie=new HashSet<int> { 110959, 103235, 103215, 105763, 103217, 51353 };
				public static readonly HashSet<int> hashDHPets=new HashSet<int> { 178664, 173827, 133741, 159144, 181748, 159098 };
				public static readonly HashSet<int> hashWizHydras=new HashSet<int> { 80745, 81515, 82111, 82972, 82109, 83959 }; //IDs taken from d3lexicon
				//Quick reference to all possible pets
				public static readonly HashSet<int> hashSummonedPets=new HashSet<int> { 169123, 123885, 169890, 168878, 169891, 169077, 169904, 169907, 169906, 169908, 169905, 169909,
                                                                                    179780, 179778, 179772, 179779, 179776, 122305,
                                                                                     110959, 103235, 103215, 105763, 103217, 51353,
                                                                                      178664, 173827, 133741, 159144, 181748, 159098,
																													80745, 81515, 82111, 82972, 82109, 83959, };
		  }
	 }
}