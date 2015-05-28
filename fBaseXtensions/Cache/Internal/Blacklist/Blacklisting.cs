﻿using System;
using System.Collections.Generic;
using fBaseXtensions.Cache.Internal.Objects;
using Zeta.Bot;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.Internal.Blacklist
{
	public static class BlacklistCache
	{
		// IGNORE LIST / BLACKLIST - for units / monsters / npcs
		// Special blacklist for things like ravens, templar/scoundrel/enchantress in town, witch-doctor summons, tornado-animations etc. etc. that should never be attacked
		// Note: This is a MONSTER blacklist - so only stuff that needs to be ignored by the combat-engine. An "object" blacklist is further down!
		internal static readonly HashSet<int> BlacklistSnoIDs = new HashSet<int> { 
            111456, 5013, 5014, 205756, 205746, 4182, 4183, 4644, 4062, 4538, 52693, 162575, 2928, 51291, 51292, 
            96132, 90958, 90959, 80980, 51292, 51291, 2928, 3546,164195, 129345, 81857, 138428, 81857, 60583, 170038, 174854, 190390, 
            194263, 107031, 106584, 186130, 187265, 201426, 201242, 200969, 201423, 
            201438, 201464, 201454, 108012, 103279, 89578, 74004, 84531, 84538, 89579, 190492, 209133, 6318, 107705, 105681, 
            182276, 117574, 182271, 182283, 182278, 128895, 81980, 81226, 81227, 107067, 106749,
            107107, 107112, 106731, 107752, 107829, 90321, 107828, 121327, 249320, 81232, 81231, 81239, 210433, 195414,
            80758, 80757, 81229, 81230, 83024, 83025, 249190, 251396, 138472, 118260, 200226, 192654, 245828, 
            215103, 132951, 217508, 199998, 199997, 114527, 245910, 
            80140, 
            206569, 200706, 5895, 5896, 5897, 5899, 4686, 87037, 85843, 103919, 249338, 
            251416, 249192, 80812, 196899,196900,196903,223333,220636,218951,206559,166133,114304,212231,
            181563,181857,181858,5215,175482,3901,152126,80447,425,3609,58568, 210087, 164057,220160, 87534, 144405, 181176, 181177,62522,220114,
				108882,210419,60108,245919,5898,5901,5582,146502,2120941707,2033647664,2033975334,2033975334,-1443233779, 129519,
				316079,349943,369795,330082,308143,350072,330019,//crusader related
				248990,249358,237062,
				321855, 347223,
				//hydras?
				325815,325813,
				54862,
				163449, 78030, 2909, 58283, 58299, 58309, 58321, 87809, 88005, 90150, 91600, 97023, 97350, 97381, 72689, 121327, 54952, 54515, 3340, 122076, 123640, 
				60665, 60844, 78554, 86400, 86428, 81699, 86266, 86400, 110769, 211456, 6190, 80002, 104596, 58836, 104827, 74909, 6155, 6156, 6158, 6159, 75132,
				181504, 91688, 3007, 3011, 3014, 130858, 131573, 214396, 182730, 226087, 141639, 206569, 15119, 54413, 54926, 2979, 56416, 53802, 5776, 3949, 
				108490, 52833, 3341, 4482, 188129, 188127, 55259, 54693, 3689, 131494, 3609, 225589, 171635, 3948,5739, 185949, 182697, 200371,
				75023,54972,73260,172810,225567,225565, 225566, 
				206461,61459,63114,53853,54331,53957,54379,199337,5900,5744,5902,85690,
				182526,460,108266,56598,89503,118223,5906,330629,
				363709,348143,
				 /*
             * A5
             */

            // Pandemonium Fortress
            357297, // X1_Fortress_Rack_C
            //374196, // X1_Fortress_Rack_C_Stump
            357295, // X1_Fortress_Rack_B
            //374195, // X1_Fortress_Rack_B_Stump
            357299, // X1_Fortress_Rack_D
            //374197, // X1_Fortress_Rack_D_Stump
            357301, // X1_Fortress_Rack_E
            //374198, // X1_Fortress_Rack_E_Stump
            357306, // X1_Fortress_Rack_F
            //374199, // X1_Fortress_Rack_F_Stump
            365503, // X1_Fortress_FloatRubble_A
            365739, // X1_Fortress_FloatRubble_F
            365580, // X1_Fortress_FloatRubble_C
            365611, // X1_Fortress_FloatRubble_E

            284713, // x1_westmarch_rat_A
            355365, // x1_Abattoir_furnaceWall

            304313, // x1_abattoir_furnace_01 
            375383, // x1_Abattoir_furnaceSpinner_Event_Phase2 -- this is a rotating avoidance, with a fire "beam" about 45f in length

            265637, // x1_Catacombs_Weapon_Rack_Raise

            321479, // x1_Westm_HeroWorship03_VO

            328008, // X1_Westm_Door_Giant_Closed
            312441, // X1_Westm_Door_Giant_Opening_Event

            328942, // x1_Pand_Ext_ImperiusCharge_Barricade 
            324867, // x1_Westm_DeathOrb_Caster_TEST
            313302, // X1_Westm_Breakable_Wolf_Head_A

            368268, // x1_Urzael_SoundSpawner
            368626, // x1_Urzael_SoundSpawner
            368599, // x1_Urzael_SoundSpawner
            368621, // x1_Urzael_SoundSpawner

            377253, // x1_Fortress_Crystal_Prison_Shield
            316221, // X1_WarpToPortal 
            370187, // x1_Malthael_Boss_Orb_Collapse
            328830, // x1_Fortress_Portal_Switch

            5503,//Start_Location_Team_0
			5466,//SphereTrigger
			4589, //levelUp_glowSphere
            4893, //ProximityTrigger

            258064, //Uber_BossPortal_Door
			376350, //x1_global_chest_shield_sphere
			195171,//PT_Blacksmith_ForgeWeaponShortcut
			85816,//healthGlobe_swipe
			308862,//Console_powerGlobe_castBuff_geo
			367978,//x1_healthGlobe_playerIsHealed_attract
			52685,//a3dun_Keep_Bridge (requires switch to open -- not operatable)
			89880, //g_LightGlow_Orange
			134394, //Dungeon_Stone_Runes_FX_geo
			217285, //trOut_Leor_painting
			167311, //trOut_Highlands_Goatmen_Chokepoint_Gate
			167185, //trOut_Cultists_Summoning_Portal_B
			175181, //trDun_Crypt_Skeleton_King_Throne_Parts
			201680, //a3dun_crater_st_Demon_ChainPylon_Fire_MistressOfPain
			84919, //Shield_Skeleton_SkeletonKing
			6575, //woodWraith_explosion
			178657, //TemplarIntro_Stash
			154103, //g_LightGlow_Red-18216
			101441, //witchdoctor_Blowgun
			295051, //powerUpGlobe_swipe
			105606, //WD_zombieDogRune_poison_swipes_02
			159631, //barbarian_frenzyRune_blood_swipe
			181195, //a2dun_Cald_Belial_Acid_Attack
			205459, //NoSpawnActor75feet
            95263, //Cow_B-5389
            249334, //DemonChains_ItemPassive-15012

            //Destructibles (non-essential)
            334973,//x1_Pand_Ext_Ledge_Breakable_Large_A
            334996,//x1_Pand_Ext_Ledge_Breakable_Top_Pillar_Small
            245047,//(x1_BogCave_Breakable_Stalagmites_A-45399)
            245120,//(x1_BogCave_Breakable_Stalagmites_B-45385)
            245448,//(x1_BogCave_Breakable_Stalagmites_C-45404)
            252580,//(x1_BogCave_Breakable_Stalagmites_E-45392)
            255791,//(x1_BogCave_Breakable_Stalagmites_G-45390)            254851,//(x1_BogCave_Stalagmite_Group_A-45397)            254854,//(x1_BogCave_Stalagmite_Group_B-45405)            254857,//(x1_BogCave_Stalagmite_Group_C-45394)            254859,//(x1_BogCave_Stalagmite_Group_D-45387)            255245,//(x1_BogCave_Stalagmite_Fungus_A-80949)
            255247,//(x1_BogCave_Stalagmite_Fungus_B-80917)
            255252,//(x1_BogCave_Stalagmite_Fungus_C-80843)
            255250,//(x1_BogCave_Stalagmite_Fungus_D-80913)
            255254,//(x1_BogCave_Stalagmite_Fungus_E-80830)
            255257,//(x1_BogCave_Stalagmite_Fungus_F-82345)
            358104,//(x1_Catacombs_Breakable_Corner_Wall
            53628, //a2dun_Swr_Act_Iron_Railing_A_01
            2965, //a2dun_Swr_Breakable_Wall_A
            326935, //X1_Westm_Scaffolding_E_Breakable
            115908, //Wall_Feilds_MainC1_TrOut
            434971, //px_Bounty_Camp_Pinger
            435630, //px_Bounty_Camp_Pinger_450-3771
            433403, //px_Bounty_Camp_Hellportals_Pool
            434361, //px_Highlands_Camp_ResurgentCult_PortalSpawner
            434340, //px_Spire_Camp_HellPortals_PortalSpawner
        };


		// When did we last clear the temporary blacklist?
		private static DateTime dateSinceTemporaryBlacklistClear = DateTime.Today;
	    private static DateTime dateSincePermBlacklistClear = DateTime.Today;
	    private const int PermBlacklistClearMinutes = 5;
	    private const int TempBlacklistClearSeconds = 90;

		// And the full blacklist?
		// These are blacklists used to blacklist objects/monsters/items either for the rest of the current game, or for a shorter period
		private static HashSet<int> hashRGUIDTemporaryIgnoreBlacklist = new HashSet<int>();
		private static HashSet<int> hashRGUIDIgnoreBlacklist = new HashSet<int>();
		internal static HashSet<int> hashSNOTargetBlacklist = new HashSet<int>();
		internal static HashSet<int> hashProfileSNOTargetBlacklist = new HashSet<int>();

		internal static void UpdateProfileBlacklist()
		{
			//Refresh Profile Target Blacklist 
			hashProfileSNOTargetBlacklist = new HashSet<int>();
			foreach (var item in ProfileManager.CurrentProfile.TargetBlacklists)
			{
				hashProfileSNOTargetBlacklist.Add(item.ActorId);
			}
		}
		internal static void ClearBlacklistCollections()
		{
			hashRGUIDIgnoreBlacklist.Clear();
			hashRGUIDTemporaryIgnoreBlacklist.Clear();

            dateSinceTemporaryBlacklistClear = DateTime.Today;
            dateSincePermBlacklistClear = DateTime.Today;
		}

		internal static readonly HashSet<ActorType> IgnoredActorTypes = new HashSet<ActorType>
			  {
				  ActorType.AxeSymbol,
				  ActorType.ClientEffect,
				  ActorType.Critter,
				  ActorType.Environment,
				 // ActorType.CustomBrain,
				  ActorType.Invalid
			  };

		internal static void CheckRefreshBlacklists(int minimumSeconds = TempBlacklistClearSeconds)
		{
			// Clear the temporary blacklist every 90 seconds
			if (DateTime.Now.Subtract(dateSinceTemporaryBlacklistClear).TotalSeconds > minimumSeconds)
			{
				dateSinceTemporaryBlacklistClear = DateTime.Now;
				hashRGUIDTemporaryIgnoreBlacklist.Clear();
			}
		}

	    internal static void CheckPermBlacklist()
	    {
            // Clear the perm blacklist every 10 minutes
            if (DateTime.Now.Subtract(dateSincePermBlacklistClear).TotalMinutes > PermBlacklistClearMinutes)
            {
                dateSincePermBlacklistClear = DateTime.Now;
                hashRGUIDIgnoreBlacklist.Clear();
            }
	    }


		internal static void AddObjectToBlacklist(int RAGUID, BlacklistType blacklist)
		{
			if (blacklist == BlacklistType.Permanent)
			{
				if (!hashRGUIDIgnoreBlacklist.Contains(RAGUID))
					hashRGUIDIgnoreBlacklist.Add(RAGUID);

			}
			else if (blacklist == BlacklistType.Temporary)
			{
				if (!hashRGUIDTemporaryIgnoreBlacklist.Contains(RAGUID))
				{
					hashRGUIDTemporaryIgnoreBlacklist.Add(RAGUID);
					dateSinceTemporaryBlacklistClear = DateTime.Now;
				}
			}
		}

		//Updates RActorGUID and returns if we should continue Cache Process by checking blacklists.
		internal static bool IsRAGUIDBlacklisted(int ractorGUID)
		{
			// See if it's on our temporary blacklist (from being stuck targeting it), as long as it's distance is not extremely close
			if (hashRGUIDTemporaryIgnoreBlacklist.Contains(ractorGUID))
			{
				return true;
			}
			// Or on our more permanent "per-game" blacklist
			if (hashRGUIDIgnoreBlacklist.Contains(ractorGUID))
			{
				return true;
			}

			return false;
		}
		internal static bool IsSNOIDBlacklisted(int snoID)
		{
			if (BlacklistSnoIDs.Contains(snoID))
				return true;

			if (hashSNOTargetBlacklist.Contains(snoID))
				return true;

			return false;
		}

		internal static void IgnoreThisObject(CachedSNOEntry snoObj, int RAGUID, bool removal = true, bool blacklistSNOID = true)
		{
			//Logger.DBLog.InfoFormat("[Blacklist] -- RAGUID {0} SNOID {1} ({2})", snoObj.SNOID, RAGUID, snoObj.InternalName);

			int sno, raguid;
			sno = snoObj.SNOID;
			raguid = RAGUID;

			//Add to our blacklist so we don't create it again..
			hashRGUIDIgnoreBlacklist.Add(raguid);

			if (blacklistSNOID)
				//Blacklist SNO so we don't create it ever again!
				BlacklistSnoIDs.Add(sno);

			if (removal)
			{
				//Clear SNO cache entries..
				ObjectCache.cacheSnoCollection.Remove(snoObj.SNOID);
				//Clear previous cache entries..
				if (ObjectCache.Objects.ContainsKey(raguid))
					ObjectCache.Objects.Remove(raguid);
			}


		}
		internal static void IgnoreThisObject(CacheObject thisobj, bool removal = true, bool blacklistSNOID = true)
		{
			//Logger.DBLog.InfoFormat("[Blacklist] Obj RAGUID {0} SNOID {1} ({2})", thisobj.RAGUID, thisobj.SNOID, thisobj.InternalName);

			int sno, raguid;
			sno = thisobj.SNOID;
			raguid = thisobj.RAGUID;

			//Add to our blacklist so we don't create it again..
			hashRGUIDIgnoreBlacklist.Add(raguid);

			if (blacklistSNOID)
				//Blacklist SNO so we don't create it ever again!
				BlacklistSnoIDs.Add(sno);

			if (removal)
			{
				//Clear SNO cache entries..
				ObjectCache.cacheSnoCollection.Remove(sno);

				if (ObjectCache.Objects.ContainsKey(raguid))
					ObjectCache.Objects.Remove(raguid);


			}
		}
	}
}
