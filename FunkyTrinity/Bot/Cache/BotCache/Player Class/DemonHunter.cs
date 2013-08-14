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
						DemonHunter_Sentry_TurretAttack=129661,
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
					 this.RecreateAbilities();
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
					 base.Abilities=new Dictionary<SNOPower, Ability>();

					 //Create the abilities
					 foreach (var item in base.HotbarPowers)
					 {
						  base.Abilities.Add(item, this.CreateAbility(item));
					 }

					 //No default generation ability.
					 if (!this.HotbarContainsAPrimaryAbility())
					 {
						  base.Abilities.Add(SNOPower.Weapon_Ranged_Projectile, Ability.Projectile_Range_Attack);
						  base.RuneIndexCache.Add(SNOPower.Weapon_Ranged_Projectile, -1);
					 }

					 //Sort Abilities
					 base.SortedAbilities=base.Abilities.Values.OrderByDescending(a => a.Priority).ThenByDescending(a => a.Range).ToList();
				}

				public override bool HotbarContainsAPrimaryAbility()
				{
					 return (this.HotbarPowers.Contains(SNOPower.DemonHunter_HungeringArrow)||this.HotbarPowers.Contains(SNOPower.DemonHunter_EntanglingShot)||this.HotbarPowers.Contains(SNOPower.DemonHunter_BolaShot)||this.HotbarPowers.Contains(SNOPower.DemonHunter_Grenades));
				}

				public override Ability CreateAbility(SNOPower Power)
				{
					 Ability returnAbility=null;

					 #region Shadow Power
					 // Shadow Power
					 if (Power.Equals(SNOPower.DemonHunter_ShadowPower))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=14,
								SecondaryEnergy=true,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy),
								
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Character.dCurrentHealthPct<=0.99d||Bot.Character.bIsRooted||Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_25]>=1||Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3);

								}),
						  };
					 }
					 #endregion
					 #region Smoke Screen
					 // Smoke Screen
					 if (Power.Equals(SNOPower.DemonHunter_SmokeScreen))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=28,
								SecondaryEnergy=true,
								Range=0,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								//PreCastConditions=,
								
								Fcriteria=new Func<bool>(() =>
								{
									 return (!HasBuff(SNOPower.DemonHunter_ShadowPower)||Bot.Character.bIsIncapacitated)
												&&(Bot.Character.dDiscipline>=28||(Bot.Character.dDiscipline>=14&&Bot.Combat.IsFleeing))
												&&(Bot.Character.dCurrentHealthPct<=0.90||Bot.Character.bIsRooted||Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_20]>=1||Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3||Bot.Character.bIsIncapacitated);
								}),
						  };
					 }
					 #endregion
					 #region Preparation
					 // Preparation
					 if (Power.Equals(SNOPower.DemonHunter_Preparation))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckCanCast),
								Cost=this.RuneIndexCache[SNOPower.DemonHunter_Preparation]==0?25:0,
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Character.dDisciplinePct<0.25d
										  //Rune: Punishment (Restores all Hatered for 25 disc)
										  ||(this.RuneIndexCache[Power]==0&&Bot.Character.dCurrentEnergyPct<0.20d);
								}),
						  };
					 }
					 #endregion


					 #region Evasive Fire
					 // Evasive Fire
					 if (Power.Equals(SNOPower.DemonHunter_EvasiveFire))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=0,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals,int>(RangeIntervals.Range_20,1),

								
						  };
					 }
					 #endregion
					 #region Companion
					 // Companion
					 if (Power.Equals(SNOPower.DemonHunter_Companion))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(2, 1, true),
								Cost=10,
								SecondaryEnergy=true,
								Counter=1,
								Range=0,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPetCount|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								
						  };
					 }
					 #endregion
					 #region Sentry Turret
					 // Sentry Turret
					 if (Power.Equals(SNOPower.DemonHunter_Sentry))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Self,
								WaitVars=new WaitLoops(0, 0, true),
								Cost=30,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Combat.powerLastSnoPowerUsed!=SNOPower.DemonHunter_Sentry&&
												(Bot.Combat.FleeingLastTarget||DateTime.Now.Subtract(Bot.Combat.LastFleeAction).TotalMilliseconds<1000)||
												(Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_40]>=1||Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_40]>=2);
								}),
						  };
					 }
					 #endregion
					 #region Marked for Death
					 // Marked for Death
					 if (Power.Equals(SNOPower.DemonHunter_MarkedForDeath))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=3,
								SecondaryEnergy=true,
								Range=40,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_40, 3),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_40, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 40),
								
								
						  };
					 }
					 #endregion
					 #region Vault
					 // Vault
					 if (Power.Equals(SNOPower.DemonHunter_Vault))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Location,
								WaitVars=new WaitLoops(1, 2, true),
								Cost=8,
								SecondaryEnergy=true,
								Range=20,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None,10),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals,int>(RangeIntervals.Range_6,1),
								
								
						  };
					 }
					 #endregion
					 #region Rain of Vengeance
					 // Rain of Vengeance
					 if (Power.Equals(SNOPower.DemonHunter_RainOfVengeance))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=0,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckCanCast),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 7),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals,int>(RangeIntervals.Range_25, 1),

								
						  };
					 }
					 #endregion
					 #region Cluster Arrow
					 // Cluster Arrow
					 if (Power.Equals(SNOPower.DemonHunter_ClusterArrow))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Location|AbilityUseType.ClusterLocation,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=50,
								Range=50,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),

								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_50, 3),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_50, 1),
								//TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial,69),
								ClusterConditions=new ClusterConditions(5d, 50, 1, true),

								
						  };
					 }
					 #endregion
					 #region Multi Shot
					 // Multi Shot
					 if (Power.Equals(SNOPower.DemonHunter_Multishot))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTarget| AbilityUseType.Target,
								
								WaitVars=new WaitLoops(1, 1, true),
								Cost=30,
								Range=55,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								ClusterConditions=new ClusterConditions(5d, 40, 3, true),

								
						  };
					 }
					 #endregion
					 #region Fan of Knives
					 // Fan of Knives
					 if (Power.Equals(SNOPower.DemonHunter_FanOfKnives))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=20,
								Range=0,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 4),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1),

								
						  };
					 }
					 #endregion
					 #region Strafe
					 // Strafe spam - similar to barbarian whirlwind routine
					 if (Power.Equals(SNOPower.DemonHunter_Strafe))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ZigZagPathing,
								WaitVars=new WaitLoops(0, 0, true),
								Cost=15,
								Range=25,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None,15),

								
						  };
					 }
					 #endregion
					 #region Spike Trap
					 // Spike Trap
					 if (Power.Equals(SNOPower.DemonHunter_SpikeTrap))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Location,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=30,
								Range=40,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 4),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_30, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial,35),

								
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Combat.powerLastSnoPowerUsed!=SNOPower.DemonHunter_SpikeTrap;
										
								}),
						  };
					 }
					 #endregion
					 #region Caltrops
					 // Caltrops
					 if (Power.Equals(SNOPower.DemonHunter_Caltrops))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=6,
								SecondaryEnergy=true,
								Range=0,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_30, 2),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_40, 1),

								
						  };

					 }
					 #endregion
					 #region Elemental Arrow
					 // Elemental Arrow
					 if (Power.Equals(SNOPower.DemonHunter_ElementalArrow))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=10,
								Range=48,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								ClusterConditions=new ClusterConditions(4d, 40, 2, true),
								Fcriteria=new Func<bool>(() =>
								{
									 return (!Bot.Target.CurrentTarget.IsTreasureGoblin&&
												Bot.Target.CurrentTarget.SNOID!=5208&&Bot.Target.CurrentTarget.SNOID!=5209&&
												Bot.Target.CurrentTarget.SNOID!=5210);
								}),
						  };
					 }
					 #endregion
					 #region Chakram
					 // Chakram
					 if (Power.Equals(SNOPower.DemonHunter_Chakram))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=10,
								Range=50,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								
								ClusterConditions=new ClusterConditions(4d, 40, 2, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial),

								Fcriteria=new Func<bool>(() =>
								{
									 return ((!Bot.Class.HotbarPowers.Contains(SNOPower.DemonHunter_ClusterArrow))||
												DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastUse[SNOPower.DemonHunter_Chakram]).TotalMilliseconds>=110000);
								}),
						  };
					 }
					 #endregion
					 #region Rapid Fire
					 // Rapid Fire
					 if (Power.Equals(SNOPower.DemonHunter_RapidFire))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=20,
								Range=50,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								
						  };
					 }
					 #endregion
					 #region Impale
					 // Impale
					 if (Power.Equals(SNOPower.DemonHunter_Impale))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=25,
								Range=12,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None,12),

								
								Fcriteria=new Func<bool>(() =>
								{
									 return ((Bot.Character.dCurrentEnergy>=25&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount);
								}),
						  };
					 }
					 #endregion
					 #region Hungering Arrow
					 // Hungering Arrow
					 if (Power.Equals(SNOPower.DemonHunter_HungeringArrow))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=0,
								Range=50,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								
						  };
					 }
					 #endregion
					 #region Entangling shot
					 // Entangling shot
					 if (Power.Equals(SNOPower.DemonHunter_EntanglingShot))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=0,
								Range=50,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								
						  };
					 }
					 #endregion
					 #region Bola Shot
					 // Bola Shot
					 if (Power.Equals(SNOPower.DemonHunter_BolaShot))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=0,
								Range=50,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
							
								ClusterConditions=new ClusterConditions(5d, 49f, 2, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None, 49),

								
						  };
					 }
					 #endregion
					 #region Grenades
					 // Grenades
					 if (Power.Equals(SNOPower.DemonHunter_Grenades))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTarget| AbilityUseType.Target,
								ClusterConditions=new ClusterConditions(6d, 40f, 1, true),
								WaitVars=new WaitLoops(0, 1, true),
								Cost=0,
								Range=40,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								
						  };
					 }
					 #endregion

					 if (Power==SNOPower.Weapon_Ranged_Projectile)
						  returnAbility=Ability.Projectile_Range_Attack;

					 return returnAbility;
				}
				public override Ability DestructibleAbility()
				{
					 SNOPower destructiblePower=this.DestructiblePower();
					 Ability returnAbility=this.Abilities[destructiblePower];
					
					 returnAbility.SetupAbilityForUse();
					 returnAbility.MinimumRange=25f;

					 return returnAbility;
				}
				public override Ability AbilitySelector(bool bCurrentlyAvoiding=false, bool bOOCBuff=false)
				{
					 this.iWaitingReservedAmount=70;


					 return base.AbilitySelector(bCurrentlyAvoiding, bOOCBuff);
				}

		  }
	 
}