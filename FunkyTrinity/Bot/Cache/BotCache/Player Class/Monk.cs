using System;
using System.Linq;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.CommonBot;
using Zeta.Internals.SNO;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal class Monk : Player
		  {
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
					 foreach (var item in base.HotbarAbilities)
					 {
						  base.Abilities.Add(item, this.CreateAbility(item));
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
							  (Bot.Combat.vPositionLastZigZagCheck!=vNullLocation&&Bot.Character.Position==Bot.Combat.vPositionLastZigZagCheck&&DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=200)||
							  Vector3.Distance(Bot.Character.Position, Bot.Combat.vSideToSideTarget)<=4f||
							  Bot.Target.CurrentTarget.AcdGuid.Value!=Bot.Combat.iACDGUIDLastWhirlwind);
				}
				public override void GenerateNewZigZagPath()
				{
					 float fExtraDistance=Bot.Target.CurrentTarget.CentreDistance<=20f?5f:1f;
					 Bot.Combat.vSideToSideTarget=FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, Bot.Target.CurrentTarget.CentreDistance+fExtraDistance);
					 // Resetting this to ensure the "no-spam" is reset since we changed our target location
					 Bot.Combat.powerLastSnoPowerUsed=SNOPower.None;
					 Bot.Combat.iACDGUIDLastWhirlwind=Bot.Target.CurrentTarget.AcdGuid.Value;
					 Bot.Combat.lastChangedZigZag=DateTime.Now;
				}
				public override Ability DestructibleAbility()
				{
					 //Tempest Rush used recently..
					 if (this.HotbarAbilities.Contains(SNOPower.Monk_TempestRush))
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=50,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return !HasBuff(Power)||Bot.SettingsFunky.Class.bMonkSpamMantra&&Bot.Target.CurrentTarget!=null&&(Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_20]>=2||(Bot.Combat.iAnythingWithinRange[RANGE_20]>=1&&Bot.SettingsFunky.Class.bMonkInnaSet)||(Bot.Target.CurrentTarget.ObjectIsSpecial||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=25f)&&
										  // Check if either we don't have blinding flash, or we do and it's been cast in the last 6000ms
										  //DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Monk_BlindingFlash]).TotalMilliseconds <= 6000)) &&
										  (!Bot.Class.HotbarAbilities.Contains(SNOPower.Monk_BlindingFlash)||
										  (Bot.Class.HotbarAbilities.Contains(SNOPower.Monk_BlindingFlash)&&
										  ((!Bot.SettingsFunky.Class.bMonkInnaSet&&Bot.Combat.iElitesWithinRange[RANGE_50]==0&&(Bot.Target.CurrentTarget.ObjectIsSpecial&&!Bot.Target.CurrentTarget.IsBoss)||HasBuff(SNOPower.Monk_BlindingFlash))))&&
										  // Check our mantras, if we have them, are up first
										  (!Bot.Class.HotbarAbilities.Contains(SNOPower.Monk_MantraOfEvasion)||(Bot.Class.HotbarAbilities.Contains(SNOPower.Monk_MantraOfEvasion)&&HasBuff(SNOPower.Monk_MantraOfEvasion)))&&
										  (!Bot.Class.HotbarAbilities.Contains(SNOPower.Monk_MantraOfConviction)||(Bot.Class.HotbarAbilities.Contains(SNOPower.Monk_MantraOfConviction)&&HasBuff(SNOPower.Monk_MantraOfConviction)))&&
										  (!Bot.Class.HotbarAbilities.Contains(SNOPower.Monk_MantraOfRetribution)||(Bot.Class.HotbarAbilities.Contains(SNOPower.Monk_MantraOfRetribution)&&HasBuff(SNOPower.Monk_MantraOfRetribution))));
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
								AbilityWaitVars=new Tuple<int, int, bool>(2, 2, true),
								Cost=25,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								Counter=1,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPetCount),
								RuneIndex=this.RuneIndexCache[Power],
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=30,
								UseAvoiding=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=10,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() => { return Bot.Character.dCurrentHealthPct<=0.50; }),
						  };
					 }
					 #endregion
					 #region Breath of heaven
					 // Breath of heaven when needing healing or the buff
					 if (Power.Equals(SNOPower.Monk_BreathOfHeaven))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=25,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=10,
								UseAvoiding=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return
										  Bot.Combat.iElitesWithinRange[RANGE_15]>=1||Bot.Character.dCurrentHealthPct<=0.4||
										  (Bot.Combat.iAnythingWithinRange[RANGE_20]>=5&&Bot.Combat.iElitesWithinRange[RANGE_50]==0)||
										  (Bot.Combat.iAnythingWithinRange[RANGE_15]>=3&&Bot.Character.dCurrentEnergyPct<=0.5)||
										  (Bot.Target.CurrentTarget.IsBoss&&Bot.Target.CurrentTarget.RadiusDistance<=15f)||
										  (Bot.SettingsFunky.Class.bMonkInnaSet&&Bot.Combat.iAnythingWithinRange[RANGE_15]>=1&&this.HotbarAbilities.Contains(SNOPower.Monk_SweepingWind)&&!HasBuff(SNOPower.Monk_SweepingWind))
										  &&
										  // Check if we don't have breath of heaven
										  (!this.HotbarAbilities.Contains(SNOPower.Monk_BreathOfHeaven)||
										  (this.HotbarAbilities.Contains(SNOPower.Monk_BreathOfHeaven)&&(!Bot.SettingsFunky.Class.bMonkInnaSet||
										  HasBuff(SNOPower.Monk_BreathOfHeaven))))&&
										  // Check if either we don't have sweeping winds, or we do and it's ready to cast in a moment
										  (!this.HotbarAbilities.Contains(SNOPower.Monk_SweepingWind)||
										  (this.HotbarAbilities.Contains(SNOPower.Monk_SweepingWind)&&(Bot.Character.dCurrentEnergy>=95||
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=Bot.SettingsFunky.Class.bMonkInnaSet?5:75,
								Priority=AbilityPriority.High,
								UseAvoiding=true,

								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckExisitingBuff),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals,int>(RangeIntervals.Range_20,1),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals,int>(RangeIntervals.Range_20,Bot.SettingsFunky.Class.bMonkInnaSet?1:2),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 25),

								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return
										  // Check if either we don't have blinding flash, or we do and it's been cast in the last 6000ms
										  //DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Monk_BlindingFlash]).TotalMilliseconds <= 6000)) &&
										  (!this.HotbarAbilities.Contains(SNOPower.Monk_BlindingFlash)||
										  (this.HotbarAbilities.Contains(SNOPower.Monk_BlindingFlash)&&
										  ((!Bot.SettingsFunky.Class.bMonkInnaSet&&Bot.Combat.iElitesWithinRange[RANGE_50]==0&&Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.ObjectIsSpecial&&!Bot.Target.CurrentTarget.IsBoss)||HasBuff(SNOPower.Monk_BlindingFlash))))&&
										  // Check our mantras, if we have them, are up first
										  (!this.HotbarAbilities.Contains(SNOPower.Monk_MantraOfEvasion)||(this.HotbarAbilities.Contains(SNOPower.Monk_MantraOfEvasion)&&HasBuff(SNOPower.Monk_MantraOfEvasion)))&&
										  (!this.HotbarAbilities.Contains(SNOPower.Monk_MantraOfConviction)||(this.HotbarAbilities.Contains(SNOPower.Monk_MantraOfConviction)&&HasBuff(SNOPower.Monk_MantraOfConviction)))&&
										  (!this.HotbarAbilities.Contains(SNOPower.Monk_MantraOfRetribution)||(this.HotbarAbilities.Contains(SNOPower.Monk_MantraOfRetribution)&&HasBuff(SNOPower.Monk_MantraOfRetribution)));
								}),

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
								AbilityWaitVars=new Tuple<int, int, bool>(2, 3, true),
								Cost=50,
								Range=16,
								Priority=AbilityPriority.Low,
								
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals,int>(RangeIntervals.Range_25,1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 15),
								
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return !Bot.Character.bWaitingForReserveEnergy||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount;
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=40,
								Range=14,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),

								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 3),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 14),
								
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (!Bot.Character.bWaitingForReserveEnergy||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount);
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
								AbilityWaitVars=new Tuple<int, int, bool>(2, 2, true),
								Cost=50,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 2),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 18),
								
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (!Bot.Character.bWaitingForReserveEnergy||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount);
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=30,
								Range=10,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 4),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 10),
								
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return 
										  // Either doesn't have sweeping wind, or does but the buff is already up
										  (!this.HotbarAbilities.Contains(SNOPower.Monk_SweepingWind)||(this.HotbarAbilities.Contains(SNOPower.Monk_SweepingWind)&&HasBuff(SNOPower.Monk_SweepingWind)))&&
										  (!Bot.Character.bWaitingForReserveEnergy||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount);
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
								UsageType=AbilityUseType.ClusterLocation| AbilityUseType.Location,
								AbilityWaitVars=new Tuple<int, int, bool>(2, 2, true),
								Cost=this.RuneIndexCache[SNOPower.Monk_WaveOfLight]==3?40:75,
								Range=16,
								Priority=AbilityPriority.Low,

							
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								ClusterConditions=Tuple.Create<double, float, int, bool>(7d, 35f, 3, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial,20),

								RuneIndex=this.RuneIndexCache[Power],
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=15,
								Range=23,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (AbilityLastUseMS(SNOPower.Monk_TempestRush)<350&&Bot.Combat.iAnythingWithinRange[RANGE_50]>0&&Bot.Target.CurrentTarget!=null)
										  ||((Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=14f)||Bot.Combat.iAnythingWithinRange[RANGE_15]>2)&&
										  (!Bot.Character.bWaitingForReserveEnergy||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount);
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=25,
								Range=30,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial,14),

								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (!Bot.Character.bWaitingForReserveEnergy||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount);
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
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, false),
								
								Priority=AbilityPriority.None,
								Range=this.RuneIndexCache[SNOPower.Monk_FistsofThunder]==0?25:12,

								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								ClusterConditions=new Tuple<double, float, int, bool>(5d, 20f, 1, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None),

								RuneIndex=this.RuneIndexCache[Power],
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Priority=AbilityPriority.None,
								Range=16,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Priority=AbilityPriority.None,
								Range=14,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, false),
								Priority=AbilityPriority.None,
								Range=14,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion

					 return returnAbility;
				}
				public override Ability AbilitySelector(bool bCurrentlyAvoiding=false, bool bOOCBuff=false)
				{
					 // Monks need 80 for special spam like tempest rushing
					 this.iWaitingReservedAmount=80;
					 return base.AbilitySelector(bCurrentlyAvoiding, bOOCBuff);
				}
		  }
	 }
}