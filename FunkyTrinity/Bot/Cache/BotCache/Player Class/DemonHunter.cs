﻿using System;
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
		  internal class DemonHunter : Player
		  {
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
								(Bot.Combat.vPositionLastZigZagCheck!=vNullLocation&&Bot.Character.Position==Bot.Combat.vPositionLastZigZagCheck&&DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=1200)||
								Vector3.Distance(Bot.Character.Position, Bot.Combat.vSideToSideTarget)<=6f||
								Bot.Target.CurrentTarget.AcdGuid.Value!=Bot.Combat.iACDGUIDLastWhirlwind);
				}
				public override void GenerateNewZigZagPath()
				{
					 if (Bot.Combat.bCheckGround)
						  Bot.Combat.vSideToSideTarget=FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, 25f, false, true, true);
					 else if (Bot.Combat.iAnythingWithinRange[RANGE_30]>=6||Bot.Combat.iElitesWithinRange[RANGE_30]>=3)
						  Bot.Combat.vSideToSideTarget=FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, 25f, false, true);
					 else
						  Bot.Combat.vSideToSideTarget=FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, 25f);
					 Bot.Combat.powerLastSnoPowerUsed=SNOPower.None;
					 Bot.Combat.iACDGUIDLastWhirlwind=Bot.Target.CurrentTarget.AcdGuid.Value;
					 Bot.Combat.lastChangedZigZag=DateTime.Now;
				}
				public override void RecreateAbilities()
				{
					 base.Abilities=new Dictionary<SNOPower, Ability>();

					 //Create the abilities
					 foreach (var item in base.HotbarAbilities)
					 {
						  base.Abilities.Add(item, this.CreateAbility(item));
					 }
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=14,
								SecondaryEnergy=true,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Character.dCurrentHealthPct<=0.99d||Bot.Character.bIsRooted||Bot.Combat.iElitesWithinRange[RANGE_25]>=1||Bot.Combat.iAnythingWithinRange[RANGE_15]>=3);

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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=28,
								SecondaryEnergy=true,
								Range=0,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								//PreCastConditions=,
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (!HasBuff(SNOPower.DemonHunter_ShadowPower)||Bot.Character.bIsIncapacitated)
												&&(Bot.Character.dDiscipline>=28||(Bot.Character.dDiscipline>=14&&Bot.Combat.IsKiting))
												&&(Bot.Character.dCurrentHealthPct<=0.90||Bot.Character.bIsRooted||Bot.Combat.iElitesWithinRange[RANGE_20]>=1||Bot.Combat.iAnythingWithinRange[RANGE_15]>=3||Bot.Character.bIsIncapacitated);
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckCanCast),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Character.dDisciplinePct<0.50d&&Bot.Character.dCurrentHealthPct<0.75d;
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=0,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iAnythingWithinRange[RANGE_20]>=1||Bot.Target.CurrentTarget.RadiusDistance<=30f);
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(2, 1, true),
								Cost=10,
								SecondaryEnergy=true,
								Counter=1,
								Range=0,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPetCount|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=30,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Combat.powerLastSnoPowerUsed!=SNOPower.DemonHunter_Sentry&&
												(Bot.KiteDistance>0&&Bot.Combat.KitedLastTarget||DateTime.Now.Subtract(Bot.Combat.LastKiteAction).TotalMilliseconds<1000)||
												(Bot.Combat.iElitesWithinRange[RANGE_40]>=1||Bot.Combat.iAnythingWithinRange[RANGE_40]>=2);
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=3,
								SecondaryEnergy=true,
								Range=40,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iElitesWithinRange[RANGE_40]>=1||Bot.Combat.iAnythingWithinRange[RANGE_40]>=3||((Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.ObjectIsSpecial)&&Bot.Target.CurrentTarget.RadiusDistance<=40f));
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Cost=8,
								SecondaryEnergy=true,
								Range=20,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Target.CurrentTarget.RadiusDistance<=10f||Bot.Combat.iAnythingWithinRange[RANGE_6]>=1);
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=0,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckCanCast),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iAnythingWithinRange[RANGE_25]>=7||Bot.Combat.iElitesWithinRange[RANGE_25]>=1);
								}),
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
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=50,
								Range=50,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iElitesWithinRange[RANGE_50]>=1||Bot.Combat.iAnythingWithinRange[RANGE_50]>=5||((Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.ObjectIsSpecial)&&Bot.Target.CurrentTarget.RadiusDistance<=69f));
								}),
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
								ClusterConditions=new Tuple<double, float, int, bool>(5d, 40, 1, true),
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=30,
								Range=55,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return Clusters(5d,40,2,true).Count>0||(Bot.Combat.iAnythingWithinRange[RANGE_40]>2||Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.ObjectIsSpecial);
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=20,
								Range=0,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iAnythingWithinRange[RANGE_15]>=4||Bot.Combat.iElitesWithinRange[RANGE_15]>=1);
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=15,
								Range=25,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Target.CurrentTarget.RadiusDistance<=15f;
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=30,
								Range=40,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Combat.powerLastSnoPowerUsed!=SNOPower.DemonHunter_SpikeTrap&&
											 (Bot.Combat.iElitesWithinRange[RANGE_30]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>4||((Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.ObjectIsSpecial)&&Bot.Target.CurrentTarget.RadiusDistance<=35f));
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=6,
								SecondaryEnergy=true,
								Range=0,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iAnythingWithinRange[RANGE_30]>=2||Bot.Combat.iElitesWithinRange[RANGE_40]>=1);
								}),
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
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=10,
								Range=48,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
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
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=10,
								Range=50,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return ((!Bot.Class.HotbarAbilities.Contains(SNOPower.DemonHunter_ClusterArrow))||
												DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.DemonHunter_Chakram]).TotalMilliseconds>=110000);
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=20,
								Range=50,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return true;
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=25,
								Range=12,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return ((Bot.Character.dCurrentEnergy>=25&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount)&&
												 Bot.Target.CurrentTarget.RadiusDistance<=12f;
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=0,
								Range=50,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=0,
								Range=50,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
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
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=0,
								Range=50,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
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
								ClusterConditions=new Tuple<double, float, int, bool>(6d, 40f, 1, true),
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=0,
								Range=40,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion

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
}