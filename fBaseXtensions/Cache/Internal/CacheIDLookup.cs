using System.Collections.Generic;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Cache.Internal
{

	internal static class CacheIDLookup
	{
		internal static ShrineTypes FindShrineType(int SNOID)
		{
			switch (SNOID)
			{
				case 176075:
					return ShrineTypes.Enlightenment;
				case 176077:
					return ShrineTypes.Frenzy;
				case 176074:
					return ShrineTypes.Protection;
				case 176076:
					return ShrineTypes.Fortune;
				case 260331:
					return ShrineTypes.Fleeting;
				case 260330:
					return ShrineTypes.Empowered;


				case 330697:
					return ShrineTypes.Channeling;
				case 330699:
					return ShrineTypes.Speed;
				case 330698:
					return ShrineTypes.Shield;
				case 330695:
					return ShrineTypes.Power;
				default://case 330696:
					return ShrineTypes.Conduit;

			}
		}


		internal static Dictionary<int, CacheBalance> dictGameBalanceCache = new Dictionary<int, CacheBalance>();

		#region SNO Priority Values
		// Dictionary for priorities, like the skeleton summoner cos it keeps bringing new stuff
		public static readonly Dictionary<int, int> dictActorSNOPriority = new Dictionary<int, int> { 
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
         };
		#endregion


		/// <summary>
		/// Units that are considered NPC that we should check/update is hostile.
		/// </summary>
		public static readonly HashSet<int> hashSnoNpcNoIgnore = new HashSet<int>
		{
			357018,
		};
		// NOTE: you don't NEED interactable SNO's listed here. But if they are listed here, *THIS* is the range at which your character will try to walk to within the object 
		// BEFORE trying to actually "click it". Certain objects need you to get very close, so it's worth having them listed with low interact ranges
		public static readonly Dictionary<int, int> dictInteractableRange = new Dictionary<int, int> { 
            {56686, 4}, {52685, 4}, {54850, 14},  {54882, 40}, {54908, 4},
         };
		// 174900 = fire-spewers (demonic forge) in Act 3, 54908 = iron gates  58379 = a2_desolate_large_bones
		// 3048 = a2 zolt dungeon "sand wall" door, 200872 = a3dunRmpt_Interactives_signal_fire_A_Prop
		public static HashSet<int> hashSNONavigationObstacles = new HashSet<int> {
            174900, 191459, 58379, 204168, 3341, 185391, 123325, 196211, 3048, 200872,60870,60671,60665 ,104632,171663,
        };

		// Destructible things that need targeting by a location instead of an ACDGUID (stuff you can't "click on" to destroy in-game)
		public static readonly HashSet<int> hashDestructableLocationTarget = new HashSet<int> { 
            170657, 116409, 121586, 3016, 121586,80231,58559,58962,62562,54477,54191,53957,54446,54477,53999,54191,54025,
				98910,
         };

		//Overrides the collision radius value!
		public static readonly Dictionary<int, float> dictFixedCollisionRadius = new Dictionary<int, float>
			  {
                    //a3 demonic forges
					{174900, 20f},{185391, 20f},
                    //a3 Siege Monster
                    {230725, 45f},
                    //monster_affix waller
                    //{226808, 12.75f},
			  };



		// A special list of things *NOT* to use whirlwind on (eg because they move too quick/WW is ineffective on)
		// 4304 = shielder fat dudes in act 3 hell zones
		public static readonly HashSet<int> hashActorSNOWhirlwindIgnore = new HashSet<int>
				{ 
            4304, 5984, 5985, 5987, 5988,
         };


		// A list of crappy "summoned mobs" we should always ignore unless they are very close to us, eg "grunts", summoned skeletons etc.
		public static readonly HashSet<int> hashActorSNOShortRangeOnly = new HashSet<int>
				{ 
				4085,4084,4083,4080, 5395, 144315,
         };

		

		// A list of SNO's to *FORCE* to type: Item. (BE CAREFUL WITH THIS!).
		// 166943 = infernal key
		
		// Interactable whitelist - things that need interacting with like special wheels, levers - they will be blacklisted for 30 seconds after one-use
		public static readonly HashSet<int> hashSNOInteractWhitelist = new HashSet<int>
				{ 
            54908, 56686, 54850, 454, 211999, 52685, 54882, 89665, 
         };





		
		




	}

}