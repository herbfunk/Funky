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
		  internal class WitchDoctor : Player
		  {
				//Base class for each individual class!
				public WitchDoctor(ActorClass a)
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
					 base.SortedAbilities=base.Abilities.Values.OrderByDescending(a => a.Priority).ThenByDescending(a => a.Range).ToList();
				}
				public override int MainPetCount
				{
					 get
					 {
						  return Bot.Character.PetData.Gargantuan;
					 }
				}
				public override bool IsMeleeClass
				{
					 get
					 {
						  return false;
					 }
				}
				public override Ability CreateAbility(SNOPower Power)
				{
					 Ability returnAbility=null;
					 #region Spirit Walk
					 // Spirit Walk Cast on 65% health or while avoiding anything but molten core or incapacitated or Chasing Goblins
					 if (Power.Equals(SNOPower.Witchdoctor_SpiritWalk))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=49,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy| AbilityConditions.CheckCanCast),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Character.dCurrentHealthPct<=0.65||(Bot.Combat.IsKiting&&Bot.Combat.iAnythingWithinRange[RANGE_15]>1)||Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted||(Bot.SettingsFunky.OutOfCombatMovement)||
												(Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentUnitTarget.IsEliteRareUnique&&Bot.Target.CurrentTarget.RadiusDistance<=40f));
								}),
						  };
					 }
					 #endregion
					 #region Soul Harvest
					 // Soul Harvest Any Elites or 2+ Norms and baby it's harvest season
					 if (Power.Equals(SNOPower.Witchdoctor_SoulHarvest))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=59,
								Counter=5,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,

								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast | AbilityConditions.CheckExisitingBuff | AbilityConditions.CheckEnergy),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_12, 3),

								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion
					 #region Sacrifice
					 // Sacrifice AKA Zombie Dog Jihad, use on Elites Only or to try and Save yourself
					 if (Power.Equals(SNOPower.Witchdoctor_Sacrifice))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 0, true),
								Cost=10,
								Range=48,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial,15),

								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion
					 #region Gargantuan
					 // Gargantuan, Recast on 1+ Elites or Bosses to trigger Restless Giant
					 if (Power.Equals(SNOPower.Witchdoctor_Gargantuan))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(2, 1, true),
								Cost=147,
								Counter=1,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast | AbilityConditions.CheckEnergy | AbilityConditions.CheckPetCount),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (this.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]==0&&(Bot.Combat.iElitesWithinRange[RANGE_15]>=1||(Bot.Target.CurrentUnitTarget.IsEliteRareUnique&&Bot.Target.CurrentTarget.RadiusDistance<=15f))
												||this.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]!=0&&Bot.Character.PetData.Gargantuan==0);
								}),
						  };
					 }
					 #endregion
					 #region Zombie dogs
					 // Zombie dogs Woof Woof, good for being blown up, cast when less than or equal to 1 Dog or Not Blowing them up and cast when less than 4
					 if (Power.Equals(SNOPower.Witchdoctor_SummonZombieDog))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=49,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckCanCast|AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Character.PetData.ZombieDogs<(this.PassiveAbilities.Contains(SNOPower.Witchdoctor_Passive_ZombieHandler)?4:3);
								}),
						  };
					 }
					 #endregion
					 #region Hex
					 // Hex Spam Cast on ANYTHING in range, mmm pork and chicken
					 if (Power.Equals(SNOPower.Witchdoctor_Hex))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Self,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=49,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy | AbilityConditions.CheckCanCast),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_12, 1),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_12, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial,18),

								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion
					 #region Mass Confuse
					 // Mass Confuse, elites only or big mobs or to escape on low health
					 if (Power.Equals(SNOPower.Witchdoctor_MassConfusion))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=74,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_12, 6),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_12, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 12),

								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion
					 #region Big Bad Voodoo
					 // Big Bad Voodoo, elites and bosses only
					 if (Power.Equals(SNOPower.Witchdoctor_BigBadVoodoo))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Self,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast),
								
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_6, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 12),

								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion
					 #region Grasp of the Dead
					 // Grasp of the Dead, look below, droping globes and dogs when using it on elites and 3 norms
					 if (Power.Equals(SNOPower.Witchdoctor_GraspOfTheDead))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTarget | AbilityUseType.Target,
								
								AbilityWaitVars=new Tuple<int, int, bool>(0, 3, true),
								Cost=122,
								Range=45,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast | AbilityConditions.CheckEnergy),
								ClusterConditions=new Tuple<double, float, int, bool>(4d, 45f, 2, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 45),

								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion
					 #region Horrify
					 // Horrify Buff at 60% health
					 if (Power.Equals(SNOPower.Witchdoctor_Horrify))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Self,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=37,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast|AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Character.dCurrentHealthPct<=0.60;
								}),
						  };
					 }
					 #endregion
					 #region Fetish Army
					 // Fetish Army, elites only
					 if (Power.Equals(SNOPower.Witchdoctor_FetishArmy))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Self,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 16),

								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion
					 #region Spirit Barrage
					 // Spirit Barrage
					 if (Power.Equals(SNOPower.Witchdoctor_SpiritBarrage))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTarget| AbilityUseType.Target,
								ClusterConditions=new Tuple<double,float,int,bool>(5d,20f,1,true),
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=108,
								Range=21,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast | AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return true;
								}),
						  };
					 }
					 #endregion
					 #region Haunt
					 // Haunt the shit out of monster and maybe they will give you treats
					 if (Power.Equals(SNOPower.Witchdoctor_Haunt))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTarget| AbilityUseType.Target,
								ClusterConditions=new Tuple<double, float, int, bool>(5d, 20f, 1, true),
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=98,
								Range=21,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return Clusters().Count>0&&Clusters().Any(c => c.DotDPSRatio<0.25);
								}),
						  };
					 }
					 #endregion
					 #region Locust
					 // Locust
					 if (Power.Equals(SNOPower.Witchdoctor_Locust_Swarm))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTarget| AbilityUseType.Target,
								ClusterConditions=new Tuple<double, float, int, bool>(5d, 20f, 1, true),
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=196,
								Range=21,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return Clusters().Count>0&&Clusters().Any(c => c.DotDPSRatio<0.25);
								}),
						  };
					 }
					 #endregion

					 #region Wall of Zombies
					 // Wall of Zombies
					 if (Power.Equals(SNOPower.Witchdoctor_WallOfZombies))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Location,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=103,
								Range=25,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy | AbilityConditions.CheckCanCast ),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 3),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 25),

								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion
					 #region Zombie Charger
					 // Zombie Charger aka Zombie bears Spams Bears @ Everything from 11feet away
					 if (Power.Equals(SNOPower.Witchdoctor_ZombieCharger))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=134,
								Range=11,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast),
								ClusterConditions=new Tuple<double, float, int, bool>(5d, 20f, 1, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial),



								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion
					 #region Acid Cloud
					 // Acid Cloud
					 if (Power.Equals(SNOPower.Witchdoctor_AcidCloud))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTarget| AbilityUseType.Target,
							
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=250,
								Range=this.RuneIndexCache[Power]==4?20:40,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial),
								ClusterConditions=new Tuple<double, float, int, bool>(4d, this.RuneIndexCache[Power]==4?20f:40f, 1, true),



								RuneIndex=this.RuneIndexCache[Power],
						  };
					 }
					 #endregion
					 #region Fire Bats
					 // Fire Bats fast-attack
					 if (Power.Equals(SNOPower.Witchdoctor_Firebats))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterLocation|AbilityUseType.Target,
								
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Range=this.RuneIndexCache[Power]==0?0:this.RuneIndexCache[Power]==4?14:25,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial),
								ClusterConditions=new Tuple<double, float, int, bool>(5d, this.RuneIndexCache[Power]==4?12f:20f, 1, true),


								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Character.dCurrentEnergy>=551||(Bot.Character.dCurrentEnergy>66
												&&Bot.Character.CurrentAnimationState==AnimationState.Channeling&&Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.WitchDoctor_Female_1HT_spell_channel|SNOAnim.WitchDoctor_Female_2HT_spell_channel|SNOAnim.WitchDoctor_Female_HTH_spell_channel|SNOAnim.WitchDoctor_Male_1HT_Spell_Channel|SNOAnim.WitchDoctor_Male_HTH_Spell_Channel)));
								}),
						  };
					 }
					 #endregion


					 #region Poison Darts
					 // Poison Darts fast-attack Spams Darts when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
					 if (Power.Equals(SNOPower.Witchdoctor_PoisonDart))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=10,
								Range=48,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return true;
								}),
						  };
					 }
					 #endregion
					 #region Corpse Spiders
					 // Corpse Spiders fast-attacks Spams Spiders when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
					 if (Power.Equals(SNOPower.Witchdoctor_CorpseSpider))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=10,
								Range=40,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return true;
								}),
						  };
					 }
					 #endregion
					 #region Toads
					 // Toads fast-attacks Spams Toads when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
					 if (Power.Equals(SNOPower.Witchdoctor_PlagueOfToads))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=10,
								Range=30,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return true;
								}),
						  };
					 }
					 #endregion
					 #region Fire Bomb
					 // Fire Bomb fast-attacks Spams Bomb when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
					 if (Power.Equals(SNOPower.Witchdoctor_Firebomb))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Range=35,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return true;
								}),
						  };
					 }
					 #endregion

					 if (Power==SNOPower.Weapon_Melee_Instant)
						  returnAbility=Instant_Melee_Attack;

					 return returnAbility;
				}
				public override Ability DestructibleAbility()
				{
					 SNOPower destructiblePower=this.DestructiblePower();
					 if (destructiblePower==SNOPower.Witchdoctor_ZombieCharger)
					 {
						  if (Bot.Character.dCurrentEnergy<140)
								return Instant_Melee_Attack;
					 }

					 Ability returnAbility=this.Abilities[destructiblePower];
					 returnAbility.SetupAbilityForUse();
					 returnAbility.MinimumRange=25f;
					 return returnAbility;
					
				}
				public override Ability AbilitySelector(bool bCurrentlyAvoiding=false, bool bOOCBuff=false)
				{
					 // Witch doctors have no reserve requirements?
					 this.iWaitingReservedAmount=0;

					 return base.AbilitySelector(bCurrentlyAvoiding, bOOCBuff);
				}
		  }
	 }
}