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

namespace FunkyTrinity
{

		  internal class Monk : Player
		  {
				 enum MonkActiveSkills
				 {
						Monk_BreathOfHeaven=69130,
						Monk_MantraOfRetribution=69484,
						Monk_MantraOfHealing=69490,
						Monk_MantraOfConviction=95572,
						Monk_FistsofThunder=95940,
						Monk_DeadlyReach=96019,
						Monk_WaveOfLight=96033,
						Monk_SweepingWind=96090,
						Monk_DashingStrike=96203,
						Monk_Serenity=96215,
						Monk_CripplingWave=96311,
						Monk_SevenSidedStrike=96694,
						Monk_WayOfTheHundredFists=97110,
						Monk_InnerSanctuary=97222,
						Monk_ExplodingPalm=97328,
						Monk_LashingTailKick=111676,
						Monk_TempestRush=121442,
						Monk_MysticAlly=123208,
						Monk_BlindingFlash=136954,
						Monk_MantraOfEvasion=192405,
						Monk_CycloneStrike=223473,
				 }
				 enum MonkPassiveSkills
				 {
						Monk_Passive_CombinationStrike=218415,
						Monk_Passive_Resolve=211581,
						Monk_Passive_TheGuardiansPath=209812,
						Monk_Passive_Pacifism=209813,
						Monk_Passive_SixthSense=209622,
						Monk_Passive_SeizeTheInitiative=209628,
						Monk_Passive_OneWithEverything=209656,
						Monk_Passive_Transcendence=209250,
						Monk_Passive_BeaconOfYtar=209104,
						Monk_Passive_ExaltedSoul=209027,
						Monk_Passive_FleetFooted=209029,
						Monk_Passive_ChantOfResonance=156467,
						Monk_Passive_NearDeathExperience=156484,
						Monk_Passive_GuidingLight=156492,

				 }

				//Base class for each individual class!
				public Monk(ActorClass a)
					 : base(a)
				{
					 this.RecreateAbilities();
				}
				public override void RecreateAbilities()
				{
					 base.Abilities=new Dictionary<SNOPower, Ability>();

					 //Create the abilities
					 foreach (var item in base.HotbarPowers)
					 {
						  base.Abilities.Add(item, this.CreateAbility(item));
					 }

					 //No default generation ability
					 if (!this.HotbarContainsAPrimaryAbility())
					 {
						  base.Abilities.Add(SNOPower.Weapon_Melee_Instant, Ability.Instant_Melee_Attack);
						  base.RuneIndexCache.Add(SNOPower.Weapon_Melee_Instant, -1);
					 }

					 //Sort Abilities
					 base.SortedAbilities=base.Abilities.Values.OrderByDescending(a => a.Priority).ThenBy(a => a.Range).ToList();
				}
				public override int MainPetCount
				{
					 get
					 {
						  return Bot.Character.PetData.MysticAlly;
					 }
				}
				public override bool IsMeleeClass
				{
					 get
					 {
						  return true;
					 }
				}
				public override bool ShouldGenerateNewZigZagPath()
				{
					 return (DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=1500||
							  (Bot.Combat.vPositionLastZigZagCheck!=Vector3.Zero&&Bot.Character.Position==Bot.Combat.vPositionLastZigZagCheck&&DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=200)||
							  Vector3.Distance(Bot.Character.Position, Bot.Combat.vSideToSideTarget)<=4f||
							  Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.AcdGuid.HasValue&&Bot.Target.CurrentTarget.AcdGuid.Value!=Bot.Combat.iACDGUIDLastWhirlwind);
				}
				public override void GenerateNewZigZagPath()
				{
					 float fExtraDistance=Bot.Target.CurrentTarget.CentreDistance<=20f?5f:1f;
					 Bot.Combat.vSideToSideTarget=Bot.NavigationCache.FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, Bot.Target.CurrentTarget.CentreDistance+fExtraDistance);
					 // Resetting this to ensure the "no-spam" is reset since we changed our target location
					 Bot.Combat.powerLastSnoPowerUsed=SNOPower.None;
					 Bot.Combat.iACDGUIDLastWhirlwind=Bot.Target.CurrentTarget.AcdGuid.HasValue?Bot.Target.CurrentTarget.AcdGuid.Value:-1;
					 Bot.Combat.lastChangedZigZag=DateTime.Now;
				}
				public override Ability DestructibleAbility()
				{
					 //Tempest Rush used recently..
					 if (this.HotbarPowers.Contains(SNOPower.Monk_TempestRush))
					 {
						  //Check if we are still using..
						  Bot.Character.UpdateAnimationState(false, true);
						  if (Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run|SNOAnim.Monk_Male_HTH_Hobble_Run))
								return this.Abilities[SNOPower.Monk_TempestRush];
					 }

					 SNOPower destructiblePower=this.DestructiblePower();
					 Ability returnAbility=this.Abilities[destructiblePower];
					 returnAbility.SetupAbilityForUse();
					 return returnAbility;
				}

				private bool HasMantraBuff()
				{
					if (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_MantraOfConviction))
					{
						 return HasBuff(SNOPower.Monk_MantraOfConviction);
					}
					else if (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_MantraOfEvasion))
					{
						 return HasBuff(SNOPower.Monk_MantraOfEvasion);
					}
					else if (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_MantraOfHealing))
					{
						 return HasBuff(SNOPower.Monk_MantraOfHealing);
					}
					else if (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_MantraOfRetribution))
					{
						 return HasBuff(SNOPower.Monk_MantraOfRetribution);
					}

					 //No Mantra on Hotbar.. we return true since we cannot cast it.
					return true;
				}


				public override bool HotbarContainsAPrimaryAbility()
				{
					 return (this.HotbarPowers.Contains(SNOPower.Monk_FistsofThunder)||this.HotbarPowers.Contains(SNOPower.Monk_DeadlyReach)||this.HotbarPowers.Contains(SNOPower.Monk_CripplingWave)||this.HotbarPowers.Contains(SNOPower.Monk_WayOfTheHundredFists));
				}

				public override Ability CreateAbility(SNOPower Power)
				{
					 Ability returnAbility=null;

					 #region Mantras
					 if (Power.Equals(SNOPower.Monk_MantraOfEvasion)||Power.Equals(SNOPower.Monk_MantraOfConviction)||Power.Equals(SNOPower.Monk_MantraOfHealing)||Power.Equals(SNOPower.Monk_MantraOfRetribution))
					 {
						  return new Ability
						  {
								Power=Power,


								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=50,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy),
								IsSpecialAbility=true,
								Fbuff=new Func<bool>(() =>
								{
									 return !HasBuff(Power);
								}),

								Fcriteria=new Func<bool>(() =>
								{

									 return
										  !HasBuff(Power)
										  ||Bot.SettingsFunky.Class.bMonkSpamMantra&&Bot.Target.CurrentTarget!=null&&(Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_25]>0||Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=2||(Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=1&&Bot.SettingsFunky.Class.bMonkInnaSet)||(Bot.Target.CurrentUnitTarget.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=25f)&&
										  // Check if either we don't have blinding flash, or we do and it's been cast in the last 6000ms
										  //DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Monk_BlindingFlash]).TotalMilliseconds <= 6000)) &&
										  (!Bot.Class.HotbarPowers.Contains(SNOPower.Monk_BlindingFlash)||
										  (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_BlindingFlash)&&(HasBuff(SNOPower.Monk_BlindingFlash)))&&
										  // Check our mantras, if we have them, are up first
										  this.HasMantraBuff());
								}),
						  };
					 }
					 #endregion
					 #region Mystic ally
					 // Mystic ally
					 if (Power.Equals(SNOPower.Monk_MysticAlly))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(2, 2, true),
								Cost=25,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								IsSpecialAbility=true,
								Counter=1,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPetCount),
								Fcriteria=new Func<bool>(() => { return true; }),
						  };
					 }
					 #endregion
					 #region InnerSanctuary
					 // InnerSanctuary
					 if (Power.Equals(SNOPower.Monk_InnerSanctuary))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=30,
								UseAvoiding=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer),

								Fcriteria=new Func<bool>(() => { return Bot.Character.dCurrentHealthPct<=0.45; }),
						  };
					 }
					 #endregion
					 #region Serenity
					 // Serenity if health is low
					 if (Power.Equals(SNOPower.Monk_Serenity))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=10,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer),

								Fcriteria=new Func<bool>(() => { return Bot.Character.dCurrentHealthPct<=0.50; }),
						  };
					 }
					 #endregion
					 #region Breath of heaven
					 // Breath of heaven when needing healing or the buff
					 if (Power.Equals(SNOPower.Monk_BreathOfHeaven))
					 {
						  //Add RuneIndex for Buff
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=25,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer),
								Fbuff=new Func<bool>(() => { return !HasBuff(SNOPower.Monk_BreathOfHeaven); }),
								Fcriteria=new Func<bool>(() => { return (Bot.Character.dCurrentHealthPct<=0.5||!HasBuff(SNOPower.Monk_BreathOfHeaven)); }),
						  };
					 }
					 #endregion

					 #region Blinding Flash
					 // Blinding Flash
					 if (Power.Equals(SNOPower.Monk_BlindingFlash))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=10,
								UseAvoiding=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer),

								Fcriteria=new Func<bool>(() =>
								{
									 return
										  Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_15]>=1||Bot.Character.dCurrentHealthPct<=0.4||
										  (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=5&&Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_50]==0)||
										  (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3&&Bot.Character.dCurrentEnergyPct<=0.5)||
										  (Bot.Target.CurrentTarget.IsBoss&&Bot.Target.CurrentTarget.RadiusDistance<=15f)||
										  (Bot.SettingsFunky.Class.bMonkInnaSet&&Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=1&&this.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&!HasBuff(SNOPower.Monk_SweepingWind))
										  &&
										  // Check if we don't have breath of heaven
										  (!this.HotbarPowers.Contains(SNOPower.Monk_BreathOfHeaven)||
										  (this.HotbarPowers.Contains(SNOPower.Monk_BreathOfHeaven)&&(!Bot.SettingsFunky.Class.bMonkInnaSet||
										  HasBuff(SNOPower.Monk_BreathOfHeaven))))&&
										  // Check if either we don't have sweeping winds, or we do and it's ready to cast in a moment
										  (!this.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)||
										  (this.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&(Bot.Character.dCurrentEnergy>=95||
										  (Bot.SettingsFunky.Class.bMonkInnaSet&&Bot.Character.dCurrentEnergy>=25)||HasBuff(SNOPower.Monk_SweepingWind)))||
										  Bot.Character.dCurrentHealthPct<=0.4);
								}),
						  };
					 }
					 #endregion
					 #region Sweeping wind
					 // Sweeping wind
					 //intell -- inna
					 if (Power.Equals(SNOPower.Monk_SweepingWind))
					 {
						  return new Ability
						  {
								Power=Power,

								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=Bot.SettingsFunky.Class.bMonkInnaSet?5:75,
								Priority=AbilityPriority.High,
								UseOOCBuff=false,
								UseAvoiding=true,
								IsSpecialAbility=true,

								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckExisitingBuff),
								
								ClusterConditions=new ClusterConditions(7d,35f,2,false),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 25),

						  };
					 }
					 #endregion
					 #region Seven Sided Strike
					 // Seven Sided Strike
					 if (Power.Equals(SNOPower.Monk_SevenSidedStrike))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Location,
								WaitVars=new WaitLoops(2, 3, true),
								Cost=50,
								Range=16,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 15),


								Fcriteria=new Func<bool>(() =>
								{
									 return !this.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount;
								}),
						  };
					 }
					 #endregion

					 #region Exploding Palm
					 // Exploding Palm
					 if (Power.Equals(SNOPower.Monk_ExplodingPalm))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=40,
								Range=14,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),

								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 3),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 14),


								Fcriteria=new Func<bool>(() =>
								{
									 return (!this.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount);
								}),
						  };
					 }
					 #endregion
					 #region Cyclone Strike
					 // Cyclone Strike
					 if (Power.Equals(SNOPower.Monk_CycloneStrike))
					 {
						  //TODO:: ADD RUNE REDUCING COST
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(2, 2, true),
								Cost=50,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),

								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 2),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 18),


								Fcriteria=new Func<bool>(() =>
								{
									 return (!this.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount);
								}),
						  };
					 }
					 #endregion

					 #region Lashing Tail Kick
					 // Lashing Tail Kick
					 if (Power.Equals(SNOPower.Monk_LashingTailKick))
					 {
						  return new Ability
						  {
								Power=Power,

								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=30,
								Range=10,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								ClusterConditions=new ClusterConditions(4d, 18f, 3, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 10),


								Fcriteria=new Func<bool>(() =>
								{
									 return
										  // Either doesn't have sweeping wind, or does but the buff is already up
										  (!this.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)||(this.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&HasBuff(SNOPower.Monk_SweepingWind)))&&
										  (!this.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount);
								}),

						  };
					 }
					 #endregion
					 #region Wave of light
					 // Wave of light
					 if (Power.Equals(SNOPower.Monk_WaveOfLight))
					 {
						  return new Ability
						  {
								Power=Power,

								//RuneIndex of 1 (Explosive Light) is self cast.
								UsageType=this.RuneIndexCache[SNOPower.Monk_WaveOfLight]==1?AbilityUseType.Self:AbilityUseType.ClusterLocation|AbilityUseType.Location,
								WaitVars=new WaitLoops(2, 2, true),
								Cost=this.RuneIndexCache[SNOPower.Monk_WaveOfLight]==3?40:75,
								Range=16,
								Priority=AbilityPriority.Low,


								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								ClusterConditions=new ClusterConditions(6d, 35f, 3, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 20),

								Fcriteria=new Func<bool>(() => { return !this.bWaitingForSpecial && this.HasMantraBuff(); }),

						  };
					 }
					 #endregion
					 #region tempest rush
					 // For tempest rush re-use
					 if (Power.Equals(SNOPower.Monk_TempestRush))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ZigZagPathing,
								WaitVars=new WaitLoops(0, 0, true),
								Cost=15,
								Range=23,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),


								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								TargetUnitConditionFlags=new UnitTargetConditions
								{
									 TrueConditionFlags=TargetProperties.RareElite|TargetProperties.Unique,
									 Distance=15,
								},

								Fcriteria=new Func<bool>(() =>
								{
									 bool isChanneling=(this.IsHobbling||AbilityLastUseMS(SNOPower.Monk_TempestRush)<350);
									 int channelingCost=this.RuneIndexCache[Power]==3?8:10;

									 //If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
									 return (isChanneling&&Bot.Character.dCurrentEnergy>channelingCost)||(Bot.Character.dCurrentEnergy>40)
											  &&(!this.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount);
								}),
						  };
					 }

					 #endregion
					 #region Dashing Strike
					 // Dashing Strike
					 if (Power.Equals(SNOPower.Monk_DashingStrike))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=25,
								Range=30,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.Ranged, 20),


								Fcriteria=new Func<bool>(() =>
								{
									 return (!this.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount);
								}),
						  };
					 }
					 #endregion

					 #region Fists of thunder
					 // Fists of thunder as the primary, repeatable attack
					 if (Power.Equals(SNOPower.Monk_FistsofThunder))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTargetNearest|AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, false),

								Priority=AbilityPriority.None,
								Range=this.RuneIndexCache[SNOPower.Monk_FistsofThunder]==0?25:12,

								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								ClusterConditions=new ClusterConditions(5d, 20f, 1, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None),


						  };

					 }
					 #endregion
					 #region Deadly reach
					 // Deadly reach
					 if (Power.Equals(SNOPower.Monk_DeadlyReach))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Priority=AbilityPriority.None,
								Range=16,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),

						  };
					 }
					 #endregion
					 #region Crippling wave
					 // Crippling wave
					 if (Power.Equals(SNOPower.Monk_CripplingWave))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Priority=AbilityPriority.None,
								Range=14,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),

						  };
					 }
					 #endregion
					 #region Way of hundred fists
					 // Way of hundred fists
					 if (Power.Equals(SNOPower.Monk_WayOfTheHundredFists))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, false),
								Priority=AbilityPriority.None,
								Range=14,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),

						  };
					 }
					 #endregion

					 if (Power==SNOPower.Weapon_Melee_Instant)
						  returnAbility=Ability.Instant_Melee_Attack;

					 return returnAbility;
				}
				public override Ability AbilitySelector(bool bCurrentlyAvoiding=false, bool bOOCBuff=false)
				{
					 // Monks need 80 for special spam like tempest rushing
					 this.iWaitingReservedAmount=80;
					 return base.AbilitySelector(bCurrentlyAvoiding, bOOCBuff);
				}
				
				private bool IsHobbling
				{
					 get
					 {
						  Bot.Character.UpdateAnimationState(false);
						  return Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run|SNOAnim.Monk_Male_HTH_Hobble_Run);
					 }
				}
		  }
	 
}