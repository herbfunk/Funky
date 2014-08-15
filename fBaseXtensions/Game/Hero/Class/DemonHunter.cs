using System;
using System.Collections.Generic;
using System.Linq;
using fBaseXtensions.Game.Hero.Skills;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Game.Hero.Skills.SkillObjects;
using fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter;
using fBaseXtensions.Items.Enums;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.Game.Hero.Class
{

	internal class DemonHunter : PlayerClass
	{
		public DemonHunter()
		{
			Logger.DBLog.DebugFormat("[Funky] Using DemonHunter Player Class");
		}

		//Base class for each individual class!
		public override ActorClass AC { get { return ActorClass.DemonHunter; } }
		private readonly HashSet<SNOAnim> knockbackanims_Male = new HashSet<SNOAnim>
		{
					 SNOAnim.Demonhunter_Male_XBow_knockback_land_01,
					 SNOAnim.Demonhunter_Male_HTH_knockback_land_01,
					 SNOAnim.Demonhunter_Male_1HS_knockback_land_01,
					 SNOAnim.Demonhunter_Male_Bow_knockback_land_01,
					 SNOAnim.Demonhunter_Male_1HXBow_knockback_land_01,
					 SNOAnim.Demonhunter_Male_DW_XBow_knockback_land_01,
		};
		private readonly HashSet<SNOAnim> knockbackanims_Female = new HashSet<SNOAnim>
		{
					 SNOAnim.Demonhunter_Female_HTH_knockback_land_01,
					 SNOAnim.Demonhunter_Female_Bow_knockback_land_01,
					 SNOAnim.Demonhunter_Female_1HS_knockback_land_01,
					 SNOAnim.Demonhunter_Female_1HXBow_knockback_land_01,
		};

		internal override HashSet<SNOAnim> KnockbackLandAnims
		{
			get
			{
				return FunkyGame.Hero.SnoActor == SNOActor.Demonhunter_Female ? knockbackanims_Female : knockbackanims_Male;
			}
		}
		internal override Skill DefaultAttack
		{
			get { return new ProjectileRangeAttack(); }
		}
		internal override int MainPetCount
		{
			get
			{
				return FunkyGame.Targeting.Cache.Environment.HeroPets.DemonHunterPet;
			}
		}
		internal override bool IsMeleeClass
		{
			get
			{
				return false;
			}
		}
		internal override bool ShouldGenerateNewZigZagPath()
		{
			return (DateTime.Now.Subtract(FunkyGame.Navigation.lastChangedZigZag).TotalMilliseconds >= 1500f ||
					   (FunkyGame.Navigation.vPositionLastZigZagCheck != Vector3.Zero && FunkyGame.Hero.Position == FunkyGame.Navigation.vPositionLastZigZagCheck && DateTime.Now.Subtract(FunkyGame.Navigation.lastChangedZigZag).TotalMilliseconds >= 1200) ||
					   Vector3.Distance(FunkyGame.Hero.Position, FunkyGame.Navigation.vSideToSideTarget) <= 6f ||
					   FunkyGame.Targeting.Cache.CurrentTarget != null && FunkyGame.Targeting.Cache.CurrentTarget.AcdGuid.HasValue && FunkyGame.Targeting.Cache.CurrentTarget.AcdGuid.Value != FunkyGame.Navigation.iACDGUIDLastWhirlwind);
		}
		internal override void GenerateNewZigZagPath()
		{
			if (FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_30] >= 6 || FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_30] >= 3)
				FunkyGame.Navigation.vSideToSideTarget = FunkyGame.Navigation.FindZigZagTargetLocation(FunkyGame.Targeting.Cache.CurrentTarget.Position, 25f, false, true);
			else
				FunkyGame.Navigation.vSideToSideTarget = FunkyGame.Navigation.FindZigZagTargetLocation(FunkyGame.Targeting.Cache.CurrentTarget.Position, 25f);

			FunkyGame.Navigation.iACDGUIDLastWhirlwind = FunkyGame.Targeting.Cache.CurrentTarget.AcdGuid.HasValue ? FunkyGame.Targeting.Cache.CurrentTarget.AcdGuid.Value : -1;
			FunkyGame.Navigation.lastChangedZigZag = DateTime.Now;
		}

		internal override Skill CreateAbility(SNOPower Power)
		{
			DemonHunterActiveSkills power = (DemonHunterActiveSkills)Enum.ToObject(typeof(DemonHunterActiveSkills), (int)Power);

			switch (power)
			{
				case DemonHunterActiveSkills.DemonHunter_SpikeTrap:
					return new SpikeTrap();
				case DemonHunterActiveSkills.DemonHunter_EntanglingShot:
					return new EntanglingShot();
				case DemonHunterActiveSkills.DemonHunter_FanOfKnives:
					return new FanOfKnives();
				case DemonHunterActiveSkills.DemonHunter_BolaShot:
					return new BolaShot();
				case DemonHunterActiveSkills.DemonHunter_Multishot:
					return new Multishot();
				case DemonHunterActiveSkills.DemonHunter_Grenades:
					return new Grenades();
				case DemonHunterActiveSkills.DemonHunter_Vault:
					return new Vault();
				case DemonHunterActiveSkills.DemonHunter_Preparation:
					return new Preparation();
				case DemonHunterActiveSkills.DemonHunter_Chakram:
					return new Chakram();
				case DemonHunterActiveSkills.DemonHunter_ClusterArrow:
					return new ClusterArrow();
				case DemonHunterActiveSkills.DemonHunter_HungeringArrow:
					return new HungeringArrow();
				case DemonHunterActiveSkills.DemonHunter_Caltrops:
					return new Caltrops();
				case DemonHunterActiveSkills.DemonHunter_Sentry:
					return new Sentry();
				case DemonHunterActiveSkills.DemonHunter_SmokeScreen:
					return new SmokeScreen();
				case DemonHunterActiveSkills.DemonHunter_MarkedForDeath:
					return new MarkedForDeath();
				case DemonHunterActiveSkills.DemonHunter_ShadowPower:
					return new ShadowPower();
				case DemonHunterActiveSkills.DemonHunter_RainOfVengeance:
					return new RainOfVengeance();
				case DemonHunterActiveSkills.DemonHunter_RapidFire:
					return new RapidFire();
				case DemonHunterActiveSkills.DemonHunter_ElementalArrow:
					return new ElementalArrow();
				case DemonHunterActiveSkills.DemonHunter_Impale:
					return new Impale();
				case DemonHunterActiveSkills.DemonHunter_Companion:
					return new Companion();
				case DemonHunterActiveSkills.DemonHunter_Strafe:
					return new Strafe();
				case DemonHunterActiveSkills.DemonHunter_EvasiveFire:
					return new EvasiveFire();
				case DemonHunterActiveSkills.DemonHunter_Vengeance:
					return new Vengeance();
				default:
					return DefaultAttack;
			}
		}

		enum DemonHunterActiveSkills
		{
			DemonHunter_SpikeTrap = 75301,
			DemonHunter_EntanglingShot = 361936,
			DemonHunter_FanOfKnives = 77546,
			DemonHunter_BolaShot = 77552,
			/*
								 DemonHunter_MoltenArrow=77601,
			*/
			DemonHunter_Multishot = 77649,
			DemonHunter_Grenades = 86610,
			DemonHunter_Vault = 111215,
			DemonHunter_Preparation = 129212,
			DemonHunter_Chakram = 129213,
			DemonHunter_ClusterArrow = 129214,
			DemonHunter_HungeringArrow = 129215,
			DemonHunter_Caltrops = 129216,
			DemonHunter_Sentry = 129217,
			DemonHunter_SmokeScreen = 130695,
			DemonHunter_MarkedForDeath = 130738,
			DemonHunter_ShadowPower = 130830,
			DemonHunter_RainOfVengeance = 130831,
			DemonHunter_RapidFire = 131192,
			DemonHunter_ElementalArrow = 131325,
			DemonHunter_Impale = 131366,
			DemonHunter_Companion = 365311,
			DemonHunter_Strafe = 134030,
			DemonHunter_EvasiveFire = 377450,
			DemonHunter_Vengeance = 302846,

		}
		/*
						enum DemonHunterPassiveSkills
						{
							 DemonHunter_Passive_NightStalker=218350,
							 DemonHunter_Passive_TacticalAdvantage=218385,
							 DemonHunter_Passive_NumbingTraps=218398,
							 DemonHunter_Passive_Archery=209734,
							 DemonHunter_Passive_Brooding=210801,
							 DemonHunter_Passive_ThrillOfTheHunt=211225,
							 DemonHunter_Passive_Grenadier=208779,
							 DemonHunter_Passive_CustomEngineering=208610,
							 DemonHunter_Passive_SteadyAim=164363,
							 DemonHunter_Passive_Vengeance=155714,
							 DemonHunter_Passive_Sharpshooter=155715,
							 DemonHunter_Passive_CullTheWeak=155721,
							 DemonHunter_Passive_Perfectionist=155722,
							 DemonHunter_Passive_Ballistics=155723,
							 DemonHunter_Passive_HotPursuit=155725,

						}
		*/
	}

}