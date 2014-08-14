using System;
using System.Collections.Generic;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills
{
	public class PowerCacheLookup
	{
		internal static readonly HashSet<int> PowerStackImportant = new HashSet<int>
				{
					 (int)SNOPower.Witchdoctor_SoulHarvest,
					 (int)SNOPower.Wizard_EnergyTwister
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
                {SNOPower.Barbarian_WrathOfTheBerserker, DateTime.Today }, {SNOPower.X1_Barbarian_Avalanche_v2,DateTime.Today},
	            #endregion 

				#region Crusader
		        {SNOPower.X1_Crusader_AkaratsChampion, DateTime.Today},{SNOPower.X1_Crusader_BlessedHammer, DateTime.Today},{SNOPower.X1_Crusader_BlessedShield, DateTime.Today}, 
                {SNOPower.X1_Crusader_Bombardment, DateTime.Today},{SNOPower.X1_Crusader_Condemn, DateTime.Today},{SNOPower.X1_Crusader_Consecration, DateTime.Today}, 
                {SNOPower.X1_Crusader_CrushingResolve, DateTime.Today},{SNOPower.X1_Crusader_FallingSword, DateTime.Today},{SNOPower.X1_Crusader_FistOfTheHeavens, DateTime.Today}, 
                {SNOPower.X1_Crusader_HeavensFury3, DateTime.Today},{SNOPower.X1_Crusader_IronSkin, DateTime.Today},{SNOPower.X1_Crusader_Judgment, DateTime.Today},
                {SNOPower.X1_Crusader_Justice, DateTime.Today},{SNOPower.X1_Crusader_LawsOfJustice2, DateTime.Today},{SNOPower.X1_Crusader_LawsOfHope2, DateTime.Today}, 
                {SNOPower.X1_Crusader_LawsOfValor2, DateTime.Today},{SNOPower.X1_Crusader_Provoke, DateTime.Today},{SNOPower.X1_Crusader_Punish, DateTime.Today},
                {SNOPower.X1_Crusader_ShieldGlare, DateTime.Today},{SNOPower.X1_Crusader_ShieldBash2, DateTime.Today},{SNOPower.X1_Crusader_Slash, DateTime.Today}, 
                {SNOPower.X1_Crusader_Smite, DateTime.Today },  {SNOPower.X1_Crusader_SteedCharge, DateTime.Today }, {SNOPower.X1_Crusader_SweepAttack, DateTime.Today },
	            #endregion 

                // Monk last-used timers
                #region Monk
		        {SNOPower.Monk_FistsofThunder, DateTime.Today},{SNOPower.Monk_DeadlyReach, DateTime.Today},{SNOPower.Monk_CripplingWave, DateTime.Today},
                {SNOPower.Monk_WayOfTheHundredFists, DateTime.Today},{SNOPower.Monk_LashingTailKick, DateTime.Today},{SNOPower.Monk_TempestRush, DateTime.Today},
                {SNOPower.Monk_WaveOfLight, DateTime.Today},{SNOPower.Monk_BlindingFlash, DateTime.Today},{SNOPower.Monk_BreathOfHeaven, DateTime.Today}, 
                {SNOPower.Monk_Serenity, DateTime.Today}, {SNOPower.X1_Monk_InnerSanctuary, DateTime.Today},{SNOPower.X1_Monk_DashingStrike, DateTime.Today}, 
                {SNOPower.Monk_ExplodingPalm, DateTime.Today},{SNOPower.Monk_SweepingWind, DateTime.Today},{SNOPower.Monk_CycloneStrike, DateTime.Today}, 
                {SNOPower.Monk_SevenSidedStrike, DateTime.Today},{SNOPower.X1_Monk_MysticAlly_v2, DateTime.Today},{SNOPower.X1_Monk_MantraOfEvasion_v2, DateTime.Today}, 
                {SNOPower.X1_Monk_MantraOfRetribution_v2, DateTime.Today},{SNOPower.X1_Monk_MantraOfHealing_v2, DateTime.Today}, {SNOPower.X1_Monk_MantraOfConviction_v2, DateTime.Today}, 
				{SNOPower.X1_Monk_Epiphany, DateTime.Today},
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
                {SNOPower.Wizard_Archon_DisintegrationWave, DateTime.Today},{SNOPower.Wizard_Archon_SlowTime, DateTime.Today},
				{SNOPower.Wizard_Archon_Teleport, DateTime.Today}, {SNOPower.X1_Wizard_Wormhole, DateTime.Today},

				{SNOPower.Wizard_Archon_DisintegrationWave_Cold, DateTime.Today},{SNOPower.Wizard_Archon_DisintegrationWave_Fire, DateTime.Today},{SNOPower.Wizard_Archon_DisintegrationWave_Lightning, DateTime.Today},
				{SNOPower.Wizard_Archon_ArcaneBlast_Cold, DateTime.Today},{SNOPower.Wizard_Archon_ArcaneBlast_Fire, DateTime.Today},{SNOPower.Wizard_Archon_ArcaneBlast_Lightning, DateTime.Today},
				{SNOPower.Wizard_Archon_ArcaneStrike_Cold, DateTime.Today},{SNOPower.Wizard_Archon_ArcaneStrike_Lightning, DateTime.Today},{SNOPower.Wizard_Archon_ArcaneStrike_Fire, DateTime.Today},
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
                {SNOPower.Witchdoctor_FetishArmy, DateTime.Today},{SNOPower.Witchdoctor_Piranhas,DateTime.Today},
                 
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
                {SNOPower.DemonHunter_ClusterArrow, DateTime.Today},{SNOPower.DemonHunter_RainOfVengeance, DateTime.Today},{SNOPower.X1_DemonHunter_Vengeance, DateTime.Today},
	            #endregion
            };
		// And this is to avoid using certain long-cooldown skills immediately after a fail
		public static Dictionary<SNOPower, DateTime> dictAbilityLastFailed = new Dictionary<SNOPower, DateTime>(dictAbilityLastUseDefaults);
		// And a "global cooldown" to prevent non-signature-spells being used too fast
		public static DateTime lastGlobalCooldownUse = DateTime.Today;



	}
}