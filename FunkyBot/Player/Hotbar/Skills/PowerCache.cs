using System;
using System.Collections.Generic;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Cache
{
	public class PowerCacheLookup
	{
		internal static readonly HashSet<int> PowerStackImportant = new HashSet<int>
				{
					 (int)SNOPower.Witchdoctor_SoulHarvest,
					 (int)SNOPower.Wizard_EnergyTwister
				};

		internal static readonly HashSet<SNOPower> AbilitiesDestructiblePriority = new HashSet<SNOPower>
		  {
				SNOPower.Barbarian_Frenzy, SNOPower.Barbarian_Bash,SNOPower.Barbarian_Cleave,SNOPower.X1_Barbarian_WeaponThrow, SNOPower.Barbarian_Rend,
				SNOPower.DemonHunter_HungeringArrow, SNOPower.X1_DemonHunter_EntanglingShot, SNOPower.DemonHunter_Bolas, SNOPower.DemonHunter_Grenades, SNOPower.DemonHunter_ElementalArrow, SNOPower.DemonHunter_RapidFire, SNOPower.DemonHunter_Chakram,
				SNOPower.Monk_FistsofThunder,SNOPower.Monk_DeadlyReach,SNOPower.Monk_CripplingWave,SNOPower.Monk_WayOfTheHundredFists,
				SNOPower.Witchdoctor_Firebomb, SNOPower.Witchdoctor_PoisonDart, SNOPower.Witchdoctor_ZombieCharger, SNOPower.Witchdoctor_CorpseSpider, SNOPower.Witchdoctor_PlagueOfToads,
				SNOPower.Wizard_EnergyTwister, SNOPower.Wizard_MagicMissile,SNOPower.Wizard_ShockPulse,SNOPower.Wizard_SpectralBlade, SNOPower.Wizard_Electrocute, SNOPower.Wizard_Archon_DisintegrationWave,
		  };

		internal static readonly HashSet<SNOPower> SpecialMovementAbilities = new HashSet<SNOPower>
		  {
				SNOPower.Barbarian_FuriousCharge,SNOPower.Barbarian_Sprint,SNOPower.Barbarian_Whirlwind,
				SNOPower.DemonHunter_Vault,SNOPower.DemonHunter_Strafe,
				SNOPower.Monk_TempestRush,
		  };

		internal static readonly HashSet<SNOPower> PrimaryAbilities = new HashSet<SNOPower>
		  {
				 SNOPower.Barbarian_Bash,SNOPower.Barbarian_Cleave,SNOPower.X1_Barbarian_WeaponThrow,SNOPower.Barbarian_Frenzy,
				 SNOPower.DemonHunter_HungeringArrow,SNOPower.X1_DemonHunter_EntanglingShot,SNOPower.DemonHunter_Bolas,SNOPower.DemonHunter_Grenades,
				 SNOPower.Monk_FistsofThunder,SNOPower.Monk_DeadlyReach,SNOPower.Monk_CripplingWave,SNOPower.Monk_WayOfTheHundredFists,
				 SNOPower.Witchdoctor_PoisonDart,SNOPower.Witchdoctor_CorpseSpider,SNOPower.Witchdoctor_PlagueOfToads,SNOPower.Witchdoctor_Firebomb,
				 SNOPower.Wizard_MagicMissile,SNOPower.Wizard_ShockPulse,SNOPower.Wizard_SpectralBlade,SNOPower.Wizard_Electrocute,SNOPower.Wizard_Archon_DisintegrationWave,
		  };

		internal static readonly HashSet<SNOPower> PassiveAbiltiesReduceRepeatTime = new HashSet<SNOPower>
		  {
				//barb
				SNOPower.Barbarian_Passive_BoonOfBulKathos,
				SNOPower.Monk_Passive_BeaconOfYtar,
				SNOPower.Witchdoctor_Passive_SpiritVessel,
				SNOPower.Witchdoctor_Passive_TribalRites,
				SNOPower.Witchdoctor_Passive_GraveInjustice,
				SNOPower.Wizard_Passive_Evocation,
		  };
		internal static readonly Dictionary<SNOPower, int> dictAbilityRepeatDefaults = new Dictionary<SNOPower, int>
            {
                #region Barb
		{SNOPower.DrinkHealthPotion, 30000}, 
                {SNOPower.Weapon_Melee_Instant, 5}, 
                {SNOPower.Weapon_Ranged_Instant, 5}, 
                {SNOPower.Barbarian_Bash, 5}, 
                {SNOPower.Barbarian_Cleave, 5}, 
                {SNOPower.Barbarian_Frenzy, 5}, 
                {SNOPower.Barbarian_HammerOfTheAncients, 150}, 
                {SNOPower.Barbarian_Rend, 3500}, 
                {SNOPower.Barbarian_SeismicSlam, 200}, 
                {SNOPower.Barbarian_Whirlwind, 5}, 
                {SNOPower.Barbarian_GroundStomp, 12200}, 
                {SNOPower.Barbarian_Leap, 10200}, 
                {SNOPower.Barbarian_Sprint, 2900}, 
                {SNOPower.Barbarian_IgnorePain, 30200},
                {SNOPower.X1_Barbarian_AncientSpear, 300}, // Has a rune that resets cooldown from 10 seconds to 0 on crit
                {SNOPower.Barbarian_Revenge, 600}, 
                {SNOPower.Barbarian_FuriousCharge, 500}, // Need to be able to check skill-rune for the dynamic cooldown - set to 10 always except for the skill rune :(
                {SNOPower.Barbarian_Overpower, 200}, 
                {SNOPower.X1_Barbarian_WeaponThrow, 5}, 
                {SNOPower.Barbarian_ThreateningShout, 10200}, 
                {SNOPower.Barbarian_BattleRage, 118000}, 
                {SNOPower.X1_Barbarian_WarCry_v2, 20500}, 
                {SNOPower.Barbarian_Earthquake, 120500},  // Need to be able to check skill-run for dynamic cooldown, and passive for extra cooldown
                {SNOPower.Barbarian_CallOfTheAncients, 120500}, // Need to be able to check passive for cooldown
                {SNOPower.Barbarian_WrathOfTheBerserker, 120500}, // Need to be able to check passive for cooldown 
	#endregion
                #region Monk
		// Monk skills
                {SNOPower.Monk_FistsofThunder, 5},
                {SNOPower.Monk_DeadlyReach, 5},
                {SNOPower.Monk_CripplingWave, 5},
                {SNOPower.Monk_WayOfTheHundredFists, 5},
                {SNOPower.Monk_LashingTailKick, 250},
                {SNOPower.Monk_TempestRush, 150},
                {SNOPower.Monk_WaveOfLight, 250},
                {SNOPower.Monk_BlindingFlash, 15200}, 
                {SNOPower.Monk_BreathOfHeaven, 15200}, 
                {SNOPower.Monk_Serenity, 20200}, 
                {SNOPower.X1_Monk_InnerSanctuary, 20200}, 
                {SNOPower.Monk_DashingStrike, 1000}, 
                {SNOPower.Monk_ExplodingPalm, 5000}, 
                {SNOPower.Monk_SweepingWind, 6000}, 
                {SNOPower.Monk_CycloneStrike, 10000}, 
                {SNOPower.Monk_SevenSidedStrike, 30200}, 
                {SNOPower.X1_Monk_MysticAlly_v2, 30000}, 
                {SNOPower.X1_Monk_MantraOfEvasion_v2, 3300}, 
                {SNOPower.X1_Monk_MantraOfRetribution_v2, 3300},
                {SNOPower.X1_Monk_MantraOfHealing_v2, 3300},  
                {SNOPower.X1_Monk_MantraOfConviction_v2, 3300},  
	#endregion
                #region Wizard
		// Wizard skills
                {SNOPower.Wizard_MagicMissile, 5},
                {SNOPower.Wizard_ShockPulse, 5},
                {SNOPower.Wizard_SpectralBlade, 5},
                {SNOPower.Wizard_Electrocute, 5},
                {SNOPower.Wizard_RayOfFrost, 5},
                {SNOPower.Wizard_ArcaneOrb, 500},
                {SNOPower.Wizard_ArcaneTorrent, 5},
                {SNOPower.Wizard_Disintegrate, 5},
                {SNOPower.Wizard_FrostNova, 9000},
                {SNOPower.Wizard_DiamondSkin, 15000},
                {SNOPower.Wizard_SlowTime, 16000}, // Is actually 20 seconds, with a rune that changes to 16 seconds
                {SNOPower.Wizard_Teleport, 16000}, // Need to be able to check rune that let's us spam this 3 times in a row then cooldown
                {SNOPower.Wizard_WaveOfForce, 12000}, // normally 15/16 seconds, but a certain rune can allow 12 seconds :(
                {SNOPower.Wizard_EnergyTwister, 5},
                {SNOPower.Wizard_Hydra, 1500},
                {SNOPower.Wizard_Meteor, 1000},
                {SNOPower.Wizard_Blizzard, 2500}, // Effect lasts for 6 seconds, actual cooldown is 0...
                {SNOPower.Wizard_IceArmor, 115000},
                {SNOPower.Wizard_StormArmor, 115000},
                {SNOPower.Wizard_MagicWeapon, 60000},
                {SNOPower.Wizard_Familiar, 60000},
                {SNOPower.Wizard_EnergyArmor, 115000},
                {SNOPower.Wizard_ExplosiveBlast, 6000},
                {SNOPower.Wizard_MirrorImage, 5000},
                {SNOPower.Wizard_Archon, 100000}, // Actually 120 seconds, but 100 seconds with a rune
                {SNOPower.Wizard_Archon_ArcaneBlast, 5000},
                {SNOPower.Wizard_Archon_ArcaneStrike, 200},
                {SNOPower.Wizard_Archon_DisintegrationWave, 5},
                {SNOPower.Wizard_Archon_SlowTime, 16000},
                {SNOPower.Wizard_Archon_Teleport, 10000}, 
	#endregion
                #region WitchDoctor
		// Witch Doctor skills 
                {SNOPower.Witchdoctor_PoisonDart, 5},
                {SNOPower.Witchdoctor_CorpseSpider, 5},
                {SNOPower.Witchdoctor_PlagueOfToads, 5},
                {SNOPower.Witchdoctor_Firebomb, 5},	
				{SNOPower.Witchdoctor_GraspOfTheDead, 6000},
                {SNOPower.Witchdoctor_Firebats, 5},
                {SNOPower.Witchdoctor_Haunt, 12000},
                {SNOPower.Witchdoctor_Locust_Swarm, 8000},
                {SNOPower.Witchdoctor_SummonZombieDog, 25000},
                {SNOPower.Witchdoctor_Horrify, 16200},
                {SNOPower.Witchdoctor_SpiritWalk, 15200},
                {SNOPower.Witchdoctor_Hex, 15200},
                {SNOPower.Witchdoctor_SoulHarvest, 15000},	
                {SNOPower.Witchdoctor_Sacrifice, 1000},	
                {SNOPower.Witchdoctor_MassConfusion, 45200},		
                {SNOPower.Witchdoctor_ZombieCharger, 5},	
                {SNOPower.Witchdoctor_SpiritBarrage, 15000},
				{SNOPower.Witchdoctor_AcidCloud, 1500},		
                {SNOPower.Witchdoctor_WallOfZombies, 25200},
                {SNOPower.Witchdoctor_Gargantuan, 25000},
                {SNOPower.Witchdoctor_BigBadVoodoo, 120000},
                {SNOPower.Witchdoctor_FetishArmy, 90000}, 
	#endregion
                #region DemonHunter
		// Demon Hunter skills
                {SNOPower.DemonHunter_HungeringArrow, 5},
                {SNOPower.X1_DemonHunter_EntanglingShot, 5},
                {SNOPower.DemonHunter_Bolas, 5},
                {SNOPower.DemonHunter_Grenades, 5},	
				{SNOPower.DemonHunter_Impale, 5},
                {SNOPower.DemonHunter_RapidFire, 5},
                {SNOPower.DemonHunter_Chakram, 5},
                {SNOPower.DemonHunter_ElementalArrow, 5},
                {SNOPower.DemonHunter_Caltrops, 6000},
                {SNOPower.DemonHunter_SmokeScreen, 3000},
                {SNOPower.DemonHunter_ShadowPower, 5000},
                {SNOPower.DemonHunter_Vault, 400},
                {SNOPower.DemonHunter_Preparation, 5000},	
                {SNOPower.X1_DemonHunter_Companion, 30000},	
                {SNOPower.DemonHunter_MarkedForDeath, 10000},		
                {SNOPower.X1_DemonHunter_EvasiveFire, 1500},	
                {SNOPower.DemonHunter_FanOfKnives, 10000},
				{SNOPower.DemonHunter_SpikeTrap, 1000},		
                {SNOPower.DemonHunter_Sentry, 12000},
                {SNOPower.DemonHunter_Strafe, 5},
                {SNOPower.DemonHunter_Multishot, 5},
                {SNOPower.DemonHunter_ClusterArrow, 150},
                {SNOPower.DemonHunter_RainOfVengeance, 10000}, 
	#endregion
            };

		public static Dictionary<SNOPower, int> dictAbilityRepeatDelay = new Dictionary<SNOPower, int>(dictAbilityRepeatDefaults);
		// Last used-timers for all abilities to prevent spamming D3 memory for cancast checks too often
		// These should NEVER need manually editing
		// But you do need to make sure every skill used by Trinity is listed in here once!
		internal static Dictionary<SNOPower, DateTime> dictAbilityLastUseDefaults = new Dictionary<SNOPower, DateTime>
            {
                {SNOPower.None, DateTime.Today},{SNOPower.DrinkHealthPotion, DateTime.Today},

					 {SNOPower.Weapon_Melee_Instant, DateTime.Today},{SNOPower.Weapon_Ranged_Instant, DateTime.Today},
					 {SNOPower.Weapon_Ranged_Projectile, DateTime.Today},{SNOPower.Weapon_Ranged_Wand, DateTime.Today},
 
                // Barbarian last-used timers
                #region Barb
		        {SNOPower.Barbarian_Bash, DateTime.Today},{SNOPower.Barbarian_Cleave, DateTime.Today},{SNOPower.Barbarian_Frenzy, DateTime.Today}, 
                {SNOPower.Barbarian_HammerOfTheAncients, DateTime.Today},{SNOPower.Barbarian_Rend, DateTime.Today},{SNOPower.Barbarian_SeismicSlam, DateTime.Today}, 
                {SNOPower.Barbarian_Whirlwind, DateTime.Today},{SNOPower.Barbarian_GroundStomp, DateTime.Today},{SNOPower.Barbarian_Leap, DateTime.Today}, 
                {SNOPower.Barbarian_Sprint, DateTime.Today},{SNOPower.Barbarian_IgnorePain, DateTime.Today},{SNOPower.X1_Barbarian_AncientSpear, DateTime.Today},
                {SNOPower.Barbarian_Revenge, DateTime.Today},{SNOPower.Barbarian_FuriousCharge, DateTime.Today},{SNOPower.Barbarian_Overpower, DateTime.Today}, 
                {SNOPower.X1_Barbarian_WeaponThrow, DateTime.Today},{SNOPower.Barbarian_ThreateningShout, DateTime.Today},{SNOPower.Barbarian_BattleRage, DateTime.Today},
                {SNOPower.X1_Barbarian_WarCry_v2, DateTime.Today},{SNOPower.Barbarian_Earthquake, DateTime.Today},{SNOPower.Barbarian_CallOfTheAncients, DateTime.Today}, 
                {SNOPower.Barbarian_WrathOfTheBerserker, DateTime.Today }, 
	            #endregion 
                // Monk last-used timers
                #region Monk
		        {SNOPower.Monk_FistsofThunder, DateTime.Today},{SNOPower.Monk_DeadlyReach, DateTime.Today},{SNOPower.Monk_CripplingWave, DateTime.Today},
                {SNOPower.Monk_WayOfTheHundredFists, DateTime.Today},{SNOPower.Monk_LashingTailKick, DateTime.Today},{SNOPower.Monk_TempestRush, DateTime.Today},
                {SNOPower.Monk_WaveOfLight, DateTime.Today},{SNOPower.Monk_BlindingFlash, DateTime.Today},{SNOPower.Monk_BreathOfHeaven, DateTime.Today}, 
                {SNOPower.Monk_Serenity, DateTime.Today}, {SNOPower.X1_Monk_InnerSanctuary, DateTime.Today},{SNOPower.Monk_DashingStrike, DateTime.Today}, 
                {SNOPower.Monk_ExplodingPalm, DateTime.Today},{SNOPower.Monk_SweepingWind, DateTime.Today},{SNOPower.Monk_CycloneStrike, DateTime.Today}, 
                {SNOPower.Monk_SevenSidedStrike, DateTime.Today},{SNOPower.X1_Monk_MysticAlly_v2, DateTime.Today},{SNOPower.X1_Monk_MantraOfEvasion_v2, DateTime.Today}, 
                {SNOPower.X1_Monk_MantraOfRetribution_v2, DateTime.Today},{SNOPower.X1_Monk_MantraOfHealing_v2, DateTime.Today}, {SNOPower.X1_Monk_MantraOfConviction_v2, DateTime.Today}, 
	            #endregion
                // Wizard last-used timers
                #region Wizard
		        {SNOPower.Wizard_MagicMissile, DateTime.Today},{SNOPower.Wizard_ShockPulse, DateTime.Today},{SNOPower.Wizard_SpectralBlade, DateTime.Today},
                {SNOPower.Wizard_Electrocute, DateTime.Today},{SNOPower.Wizard_RayOfFrost, DateTime.Today},{SNOPower.Wizard_ArcaneOrb, DateTime.Today},
                {SNOPower.Wizard_ArcaneTorrent, DateTime.Today},{SNOPower.Wizard_Disintegrate, DateTime.Today},{SNOPower.Wizard_FrostNova, DateTime.Today},
                {SNOPower.Wizard_DiamondSkin, DateTime.Today},{SNOPower.Wizard_SlowTime, DateTime.Today},{SNOPower.Wizard_Teleport, DateTime.Today},
                {SNOPower.Wizard_WaveOfForce, DateTime.Today},{SNOPower.Wizard_EnergyTwister, DateTime.Today},{SNOPower.Wizard_Hydra, DateTime.Today},
                {SNOPower.Wizard_Meteor, DateTime.Today},{SNOPower.Wizard_Blizzard, DateTime.Today},{SNOPower.Wizard_IceArmor, DateTime.Today},
                {SNOPower.Wizard_StormArmor, DateTime.Today},{SNOPower.Wizard_MagicWeapon, DateTime.Today},{SNOPower.Wizard_Familiar, DateTime.Today},
                {SNOPower.Wizard_EnergyArmor, DateTime.Today},{SNOPower.Wizard_ExplosiveBlast, DateTime.Today},{SNOPower.Wizard_MirrorImage, DateTime.Today},
                {SNOPower.Wizard_Archon, DateTime.Today},{SNOPower.Wizard_Archon_ArcaneBlast, DateTime.Today},{SNOPower.Wizard_Archon_ArcaneStrike, DateTime.Today},
                {SNOPower.Wizard_Archon_DisintegrationWave, DateTime.Today},{SNOPower.Wizard_Archon_SlowTime, DateTime.Today},{SNOPower.Wizard_Archon_Teleport, DateTime.Today}, 
	            #endregion
                // Witch Doctor last-used timers 
                #region WitchDoctor
		        {SNOPower.Witchdoctor_PoisonDart, DateTime.Today},{SNOPower.Witchdoctor_CorpseSpider, DateTime.Today},{SNOPower.Witchdoctor_PlagueOfToads, DateTime.Today},
                {SNOPower.Witchdoctor_Firebomb, DateTime.Today},{SNOPower.Witchdoctor_GraspOfTheDead, DateTime.Today},{SNOPower.Witchdoctor_Firebats, DateTime.Today},
                {SNOPower.Witchdoctor_Haunt, DateTime.Today},{SNOPower.Witchdoctor_Locust_Swarm, DateTime.Today},{SNOPower.Witchdoctor_SummonZombieDog, DateTime.Today},
                {SNOPower.Witchdoctor_Horrify, DateTime.Today},{SNOPower.Witchdoctor_SpiritWalk, DateTime.Today},{SNOPower.Witchdoctor_Hex, DateTime.Today},
                {SNOPower.Witchdoctor_SoulHarvest, DateTime.Today},{SNOPower.Witchdoctor_Sacrifice, DateTime.Today},{SNOPower.Witchdoctor_MassConfusion, DateTime.Today},		
                {SNOPower.Witchdoctor_ZombieCharger, DateTime.Today},{SNOPower.Witchdoctor_SpiritBarrage, DateTime.Today},{SNOPower.Witchdoctor_AcidCloud, DateTime.Today},		
                {SNOPower.Witchdoctor_WallOfZombies, DateTime.Today},{SNOPower.Witchdoctor_Gargantuan, DateTime.Today},{SNOPower.Witchdoctor_BigBadVoodoo, DateTime.Today},
                {SNOPower.Witchdoctor_FetishArmy, DateTime.Today},
                 
	            #endregion
                // Demon Hunter last-used timers 
                #region DemonHunter
		        {SNOPower.DemonHunter_HungeringArrow, DateTime.Today},{SNOPower.X1_DemonHunter_EntanglingShot, DateTime.Today},{SNOPower.DemonHunter_Bolas, DateTime.Today},
                {SNOPower.DemonHunter_Grenades, DateTime.Today},{SNOPower.DemonHunter_Impale, DateTime.Today},{SNOPower.DemonHunter_RapidFire, DateTime.Today},
                {SNOPower.DemonHunter_Chakram, DateTime.Today},{SNOPower.DemonHunter_ElementalArrow, DateTime.Today},{SNOPower.DemonHunter_Caltrops, DateTime.Today},
                {SNOPower.DemonHunter_SmokeScreen, DateTime.Today},{SNOPower.DemonHunter_ShadowPower, DateTime.Today},{SNOPower.DemonHunter_Vault, DateTime.Today},
                {SNOPower.DemonHunter_Preparation, DateTime.Today},{SNOPower.X1_DemonHunter_Companion, DateTime.Today},{SNOPower.DemonHunter_MarkedForDeath, DateTime.Today},		
                {SNOPower.X1_DemonHunter_EvasiveFire, DateTime.Today},{SNOPower.DemonHunter_FanOfKnives, DateTime.Today},{SNOPower.DemonHunter_SpikeTrap, DateTime.Today},		
                {SNOPower.DemonHunter_Sentry, DateTime.Today},{SNOPower.DemonHunter_Strafe, DateTime.Today},{SNOPower.DemonHunter_Multishot, DateTime.Today},
                {SNOPower.DemonHunter_ClusterArrow, DateTime.Today},{SNOPower.DemonHunter_RainOfVengeance, DateTime.Today},
	            #endregion
            };
		// And this is to avoid using certain long-cooldown skills immediately after a fail
		public static Dictionary<SNOPower, DateTime> dictAbilityLastFailed = new Dictionary<SNOPower, DateTime>(dictAbilityLastUseDefaults);
		// And a "global cooldown" to prevent non-signature-spells being used too fast
		public static DateTime lastGlobalCooldownUse = DateTime.Today;



	}
}