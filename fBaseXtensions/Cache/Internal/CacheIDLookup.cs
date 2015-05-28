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
            {56686, 4}, {54850, 14},  {54882, 40}, {54908, 4},
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
                    {91111, 45f},{214240, 45f},{220471, 45f},{316254, 45f},{180734, 45f},{410366, 45f},
                    //monster_affix waller
                    //{226808, 12.75f},
			  };



		// A special list of things *NOT* to use whirlwind on (eg because they move too quick/WW is ineffective on)
		// 4304 = shielder fat dudes in act 3 hell zones
		public static readonly HashSet<int> hashActorSNOWhirlwindIgnore = new HashSet<int>
				{ 
            4304, 5984, 5985, 5987, 5988,
         };


		
		internal static readonly HashSet<int> hashSNOSkipCommonDataCheck = new HashSet<int>()
		{
			75726, //Wizard Arcane Orb Orbiter Orbs
		};

		
		




	}

}