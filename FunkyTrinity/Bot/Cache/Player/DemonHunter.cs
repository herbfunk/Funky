using System;
using System.Linq;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.CommonBot;
using Zeta.Internals.SNO;
using FunkyTrinity.Enums;
using FunkyTrinity.ability;
using FunkyTrinity.Cache;
using FunkyTrinity.ability.Abilities;
using FunkyTrinity.ability.Abilities.DemonHunter;
namespace FunkyTrinity
{

		  internal class DemonHunter : Player
		  {
				 enum DemonHunterActiveSkills
				 {
						DemonHunter_SpikeTrap=75301,
						DemonHunter_EntanglingShot=75873,
						DemonHunter_FanOfKnives=77546,
						DemonHunter_BolaShot=77552,
						DemonHunter_MoltenArrow=77601,
						DemonHunter_Multishot=77649,
						DemonHunter_Grenades=86610,
						DemonHunter_Vault=111215,
						DemonHunter_Preparation=129212,
						DemonHunter_Chakram=129213,
						DemonHunter_ClusterArrow=129214,
						DemonHunter_HungeringArrow=129215,
						DemonHunter_Caltrops=129216,
						DemonHunter_Sentry=129217,
						DemonHunter_SmokeScreen=130695,
						DemonHunter_MarkedForDeath=130738,
						DemonHunter_ShadowPower=130830,
						DemonHunter_RainOfVengeance=130831,
						DemonHunter_RapidFire=131192,
						DemonHunter_ElementalArrow=131325,
						DemonHunter_Impale=131366,
						DemonHunter_Companion=133695,
						DemonHunter_Strafe=134030,
						DemonHunter_EvasiveFire=134209,

				 }
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
				//Base class for each individual class!
				public DemonHunter(ActorClass a)
					 : base(a)
				{

				}
				public override Ability DefaultAttack
				{
					 get { return new ProjectileRangeAttack(); }
				}
				public override int MainPetCount
				{
					 get
					 {
						  return Bot.Character.PetData.DemonHunterPet;
					 }
				}
				public override bool IsMeleeClass
				{
					 get
					 {
						  return false;
					 }
				}
				public override bool ShouldGenerateNewZigZagPath()
				{
					 return (DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=1500f||
								(Bot.Combat.vPositionLastZigZagCheck!=Vector3.Zero&&Bot.Character.Position==Bot.Combat.vPositionLastZigZagCheck&&DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=1200)||
								Vector3.Distance(Bot.Character.Position, Bot.Combat.vSideToSideTarget)<=6f||
								Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.AcdGuid.HasValue&&Bot.Target.CurrentTarget.AcdGuid.Value!=Bot.Combat.iACDGUIDLastWhirlwind);
				}
				public override void GenerateNewZigZagPath()
				{
					 if (Bot.Combat.bCheckGround)
						  Bot.Combat.vSideToSideTarget=Bot.NavigationCache.FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, 25f, false, true, true);
					 else if (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_30]>=6||Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_30]>=3)
						  Bot.Combat.vSideToSideTarget=Bot.NavigationCache.FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, 25f, false, true);
					 else
						  Bot.Combat.vSideToSideTarget=Bot.NavigationCache.FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, 25f);
					 Bot.Combat.powerLastSnoPowerUsed=SNOPower.None;
					 Bot.Combat.iACDGUIDLastWhirlwind=Bot.Target.CurrentTarget.AcdGuid.HasValue?Bot.Target.CurrentTarget.AcdGuid.Value:-1;
					 Bot.Combat.lastChangedZigZag=DateTime.Now;
				}

				public override void RecreateAbilities()
				{
					 Abilities=new Dictionary<SNOPower, Ability>();

					 //Create the abilities
					 foreach (var item in HotbarPowers)
					 {
							Abilities.Add(item, this.CreateAbility(item));
					 }

					 //No default rage generation ability.. then we add the Instant Melee Ability.
					 if (!HotbarContainsAPrimaryAbility())
					 {
							Ability defaultAbility=this.DefaultAttack;
							Abilities.Add(defaultAbility.Power, defaultAbility);
							RuneIndexCache.Add(defaultAbility.Power, -1);
					 }

					 //Sort Abilities
					 SortedAbilities=Abilities.Values.OrderByDescending(a => a.Priority).ThenBy(a => a.Range).ToList();
					 
					 //Update LOS conditions
					 base.UpdateLOSConditions();

				}

				public override Ability CreateAbility(SNOPower Power)
				{
					DemonHunterActiveSkills power=(DemonHunterActiveSkills)Enum.ToObject(typeof(DemonHunterActiveSkills), (int)Power);

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
						 case DemonHunterActiveSkills.DemonHunter_MoltenArrow:
							return new MoltenArrow();
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
						 default:
							return this.DefaultAttack;
					}
				}

		  }
	 
}