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

		  internal class Barbarian : Player
		  {
				 enum BarbarianActiveSkills
				 {
						Barbarian_AncientSpear=69979,
						Barbarian_Rend=70472,
						Barbarian_Frenzy=78548,
						Barbarian_Sprint=78551,
						Barbarian_BattleRage=79076,
						Barbarian_ThreateningShout=79077,
						Barbarian_Bash=79242,
						Barbarian_GroundStomp=79446,
						Barbarian_IgnorePain=79528,
						Barbarian_WrathOfTheBerserker=79607,
						Barbarian_HammerOfTheAncients=80028,
						Barbarian_CallOfTheAncients=80049,
						Barbarian_Cleave=80263,
						Barbarian_WarCry=81612,
						Barbarian_SeismicSlam=86989,
						Barbarian_Leap=93409,
						Barbarian_WeaponThrow=93885,
						Barbarian_Whirlwind=96296,
						Barbarian_FuriousCharge=97435,
						Barbarian_Earthquake=98878,
						Barbarian_Revenge=109342,
						Barbarian_Overpower=159169,
				 }
				 enum BarbarianPassiveSkills
				 {
						Barbarian_Passive_BoonOfBulKathos=204603,
						Barbarian_Passive_NoEscape=204725,
						Barbarian_Passive_Brawler=205133,
						Barbarian_Passive_Ruthless=205175,
						Barbarian_Passive_BerserkerRage=205187,
						Barbarian_Passive_PoundOfFlesh=205205,
						Barbarian_Passive_Bloodthirst=205217,
						Barbarian_Passive_Animosity=205228,
						Barbarian_Passive_Unforgiving=205300,
						Barbarian_Passive_Relentless=205398,
						Barbarian_Passive_Superstition=205491,
						Barbarian_Passive_InspiringPresence=205546,
						Barbarian_Passive_Juggernaut=205707,
						Barbarian_Passive_ToughAsNails=205848,
						Barbarian_Passive_WeaponsMaster=206147,
				 }
				//Base class for each individual class!
				public Barbarian(ActorClass a)
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

					 //No default rage generation ability.. then we add the Instant Melee Ability.
					 if (!this.HotbarContainsAPrimaryAbility())
					 {
						  base.Abilities.Add(SNOPower.Weapon_Melee_Instant, Ability.Instant_Melee_Attack);
						  base.RuneIndexCache.Add(SNOPower.Weapon_Melee_Instant, -1);
					 }

					 if (base.Abilities.ContainsKey(SNOPower.Barbarian_WeaponThrow))
						  UsingWeaponThrowAbility=true;

					 //Sort Abilities
					 base.SortedAbilities=base.Abilities.Values.OrderByDescending(a => a.Priority).ThenBy(a => a.Range).ToList();
				}

				private bool UsingWeaponThrowAbility=false;
				public override bool IsMeleeClass
				{
					 get
					 {
						  return !UsingWeaponThrowAbility;
					 }
				}


				public override bool ShouldGenerateNewZigZagPath()
				{
					 return (DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=2000f||
								(Bot.Combat.vPositionLastZigZagCheck!=Vector3.Zero&&Bot.Character.Position==Bot.Combat.vPositionLastZigZagCheck&&DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=1200)||
								Vector3.Distance(Bot.Character.Position, Bot.Combat.vSideToSideTarget)<=5f||
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

				public override bool HotbarContainsAPrimaryAbility()
				{
					 return (this.HotbarPowers.Contains(SNOPower.Barbarian_Bash)||this.HotbarPowers.Contains(SNOPower.Barbarian_Cleave)||this.HotbarPowers.Contains(SNOPower.Barbarian_WeaponThrow)||this.HotbarPowers.Contains(SNOPower.Barbarian_Frenzy));
				}

				public override Ability DestructibleAbility()
				{
					 SNOPower destructiblePower=this.DestructiblePower();
					 Ability returnAbility=this.Abilities[destructiblePower];
					 if (destructiblePower==SNOPower.Barbarian_WeaponThrow)
					 {
						  if (Bot.Character.dCurrentEnergy>=10)
						  {
								
								returnAbility.SetupAbilityForUse();
								return returnAbility;
						  }
						  else
						  {
								return Ability.Instant_Melee_Attack;
						  }
					 }

					 
					 returnAbility.SetupAbilityForUse();
					 return returnAbility;
				}

				public override Ability CreateAbility(SNOPower Power)
				{
					 Ability returnAbility=null;

					 #region IgnorePain
					 // Ignore Pain when low on health
					 if (Power.Equals(SNOPower.Barbarian_IgnorePain))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(0, 0, true),
								Cost=0,
								UseAvoiding=true,
								UseOOCBuff=false,
								IsSpecialAbility=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckCanCast),
								
								Fcriteria=new Func<bool>(() => { return Bot.Character.dCurrentHealthPct<=0.45; }),
						  };
					 } 
					 #endregion
					 #region Earthquake
					 // Earthquake, elites close-range only
					 if (Power.Equals(SNOPower.Barbarian_Earthquake))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(4, 4, true),
								Cost=0,
								UseAvoiding=false,
								UseOOCBuff=false,
								IsSpecialAbility=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy|AbilityConditions.CheckExisitingBuff|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 13),
						  };

					 }
					 #endregion
					 #region Wrath of the berserker
					 // Wrath of the berserker, elites only (wrath of berserker)
					 if (Power.Equals(SNOPower.Barbarian_WrathOfTheBerserker))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(4, 4, true),
								Cost=50,
								UseAvoiding=true,
								UseOOCBuff=false,
								IsSpecialAbility=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckExisitingBuff|AbilityConditions.CheckCanCast),
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Combat.bAnyChampionsPresent
                        || (Bot.SettingsFunky.Class.bBarbUseWOTBAlways && Bot.Combat.bAnyMobsInCloseRange)
                        || (Bot.SettingsFunky.Class.bGoblinWrath && Bot.Target.CurrentTarget.IsTreasureGoblin);
								}),
						  };
					 }
					 #endregion
					 #region Call of the ancients
					 // Call of the ancients, elites only
					 if (Power.Equals(SNOPower.Barbarian_CallOfTheAncients))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(4, 4, true),
								Cost=50,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 25),
						  };
					 }
					 #endregion


					 #region BattleRage
					 if (Power.Equals(SNOPower.Barbarian_BattleRage))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=20,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckPlayerIncapacitated),
								//TestCustomCombatConditionAlways=true,
								Fcriteria=new Func<bool>(() =>
								{
									 return !HasBuff(SNOPower.Barbarian_BattleRage)||
												//Only if we cannot spam sprint..
												(!Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Sprint)&&
													 ((Bot.SettingsFunky.Class.bFuryDumpWrath&&Bot.Character.dCurrentEnergyPct>=0.98&&HasBuff(SNOPower.Barbarian_WrathOfTheBerserker))||
													 (Bot.SettingsFunky.Class.bFuryDumpAlways&&Bot.Character.dCurrentEnergyPct>=0.98)));
								}),

						  };
					 }
					 #endregion
					 #region Warcry
					 // war cry OOC 
					 if (Power.Equals(SNOPower.Barbarian_WarCry))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=0,
								Range=0,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								//TestCustomCombatConditionAlways=true,
								Fcriteria=new Func<bool>(() =>
								{
									 return (!HasBuff(SNOPower.Barbarian_WarCry)
												||(Bot.Class.PassivePowers.Contains(SNOPower.Barbarian_Passive_InspiringPresence)&&DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastUse[SNOPower.Barbarian_WarCry]).TotalSeconds>59
												||Bot.Character.dCurrentEnergyPct<0.10));
								}),
						  };
					 }
					 #endregion
					 #region Sprint
					 // Sprint buff, if same suitable targets as elites, keep maintained for WW users
					 if (Power.Equals(SNOPower.Barbarian_Sprint))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=20,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								Fcriteria=new Func<bool>(() =>
								{
									 return (!HasBuff(SNOPower.Barbarian_Sprint)&&Bot.SettingsFunky.OutOfCombatMovement)||
										  (((Bot.SettingsFunky.Class.bFuryDumpWrath&&Bot.Character.dCurrentEnergyPct>=0.95&&HasBuff(SNOPower.Barbarian_WrathOfTheBerserker))||
										  (Bot.SettingsFunky.Class.bFuryDumpAlways&&Bot.Character.dCurrentEnergyPct>=0.95)||
										  ((Bot.Class.AbilityUseTimer(SNOPower.Barbarian_Sprint)&&!HasBuff(SNOPower.Barbarian_Sprint))&&
										  // Always keep up if we are whirlwinding, or if the target is a goblin
										  (Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Whirlwind)||Bot.Target.CurrentTarget.IsTreasureGoblin)))&&
										  (!Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)||(Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)&&HasBuff(SNOPower.Barbarian_BattleRage))));
								}),

						  };
					 }
					 #endregion


					 #region Threatening Shout
					 // Threatening shout
					 if (Power.Equals(SNOPower.Barbarian_ThreateningShout))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=20,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								Fcriteria=new Func<bool>(() =>
								{
									 return (
										  Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_20]>1||(Bot.Target.CurrentTarget.IsBoss&&Bot.Target.CurrentTarget.RadiusDistance<=20)||
										  (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_20]>2&&!Bot.Combat.bAnyBossesInRange&&(Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_50]==0||Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_SeismicSlam)))||
										  Bot.Character.dCurrentHealthPct<=0.75
										  );
								}),
						  };
					 }
					 #endregion
					 #region Ground Stomp
					 // Ground Stomp
					 if (Power.Equals(SNOPower.Barbarian_GroundStomp))
					 {
						  //
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 2, true),
								Cost=20,
								Range=16,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals,int>(RangeIntervals.Range_15,4),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals,int>(RangeIntervals.Range_15,1),
								
								
						  };
					 }
					 #endregion
					 #region Leap
					 if (Power.Equals(SNOPower.Barbarian_Leap))
					 {

						  return new Ability
						  {
								Power=Power,
								
								WaitVars=new WaitLoops(2, 2, true),
								UsageType=AbilityUseType.ClusterLocation|AbilityUseType.Location,
								Range=35,
								Priority=AbilityPriority.Low,
								UseAvoiding=false,
								UseOOCBuff=false,
								IsNavigationSpecial=true,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckCanCast),
								ClusterConditions=new ClusterConditions(5d, 30, 2, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, falseConditionalFlags: TargetProperties.Fast, MinimumRadiusDistance: 30),

						  };
					 } 
					 #endregion

					 #region Revenge
					 // Revenge used off-cooldown
					 if (Power.Equals(SNOPower.Barbarian_Revenge))
					 {
						  //
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(4, 4, true),
								Cost=0,
								UseAvoiding=false,
								UseOOCBuff=true,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								
								UnitsWithinRangeConditions=new Tuple<RangeIntervals,int>(RangeIntervals.Range_6,1),
						  };
					 }
					 #endregion
					 #region Furious Charge
					 // Furious charge
					 if (Power.Equals(SNOPower.Barbarian_FuriousCharge))
					 {
						  //TODO:: Make Cluster Location --
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 2, true),
								Cost=20,
								Range=35,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15,3),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 15),

								
								
							
						  };
					 }
					 #endregion
					 #region Rend
					 // Rend spam
					 if (Power.Equals(SNOPower.Barbarian_Rend))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(3, 3, true),
								Cost=20,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								
								Fcriteria=new Func<bool>(() =>
								{
									 return 
										 !this.bWaitingForSpecial&&
										  //Only if 2 non-elite targets OR 1 elite target is within 6feet
									  (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_6]>1||Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_6]>0)&&
										  // Don't use against goblins (they run too quick!) Or any mobs added to the fast list unless elite.                                                                  
									  (!Bot.Target.CurrentTarget.IsTreasureGoblin&&(!CacheIDLookup.hashActorSNOFastMobs.Contains(Bot.Target.CurrentTarget.SNOID)||Bot.Target.CurrentUnitTarget.IsEliteRareUnique)||Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_6]>3)&&
										  //Non-WW users
									 (!Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Whirlwind)&&((Bot.Class.AbilityUseTimer(SNOPower.Barbarian_Rend)||(Bot.Combat.iNonRendedTargets_6>2)))
										  // This segment is for people who *DO* have whirlwind
										  ||(Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Whirlwind)&&
										  // See if it's off-cooldown and at least 40 fury, or use as a fury dump
										  ((Bot.SettingsFunky.Class.bFuryDumpWrath&&Bot.Character.dCurrentEnergyPct>=0.92&&HasBuff(SNOPower.Barbarian_WrathOfTheBerserker))||
										  (Bot.SettingsFunky.Class.bFuryDumpAlways&&Bot.Character.dCurrentEnergyPct>=0.92)||
											(DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastUse[SNOPower.Barbarian_Rend]).TotalMilliseconds>=2800))&&
										  // Max once every 1.2 seconds even if fury dumping, so sprint can be fury dumped too
										  // DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Barbarian_Rend]).TotalMilliseconds >= 1200 &&
										  // 3+ mobs of any kind at close range *OR* one elite/boss/special at close range
									  ((Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3&&Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_12]>=1)||
									  (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3&&Bot.Target.CurrentTarget.IsTreasureGoblin&&Bot.Target.CurrentTarget.RadiusDistance<=6f)||
											Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=5||
									  ((Bot.Target.CurrentUnitTarget.IsEliteRareUnique)&&Bot.Target.CurrentTarget.RadiusDistance<=6f&&Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3))));
								}),
						  };
					 }
					 #endregion
					 #region Overpower
					 // Overpower used off-cooldown
					 if (Power.Equals(SNOPower.Barbarian_Overpower))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(4, 4, true),
								Cost=20,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_6]>=2||(Bot.Character.dCurrentHealthPct<=0.85&&Bot.Target.CurrentTarget.RadiusDistance<=5f)||
										  (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_6]>=1&&
										  ((Bot.Target.CurrentUnitTarget.IsEliteRareUnique)||HasBuff(SNOPower.Barbarian_WrathOfTheBerserker)||
										  Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_SeismicSlam)));
								}),
						  };
					 }
					 #endregion
					 #region Seismic Slam
					 // Seismic slam enemies within close range
					 if (Power.Equals(SNOPower.Barbarian_SeismicSlam))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.ClusterLocation|AbilityUseType.Location,
								WaitVars=new WaitLoops(2, 2, true),
								Cost=this.RuneIndexCache[SNOPower.Barbarian_SeismicSlam]==3?15:30,
								Range=40,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								ClusterConditions=new ClusterConditions(this.RuneIndexCache[Power]==4?4d:6d, 40f, 2, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, falseConditionalFlags: TargetProperties.TreasureGoblin|TargetProperties.Fast),

								Fcriteria=new Func<bool>(()=>{return !this.bWaitingForSpecial;}),
								
						  };
					 }
					 #endregion
					 #region Ancient Spear
					 // Ancient spear 
					 if (Power.Equals(SNOPower.Barbarian_AncientSpear))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(2, 2, true),
								Range=35,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.Ranged,25,0.50d),
								
								//TestCustomCombatConditionAlways=true,
								Fcriteria=new Func<bool>(()=>
								{
									return Bot.Target.CurrentUnitTarget.Monstersize.Value==MonsterSize.Ranged || Bot.Character.dCurrentEnergyPct<0.5d;
								}),
						  };
					 }
					 #endregion

					 #region WhirlWind
					 // Whirlwind spam as long as necessary pre-buffs are up
					 if (Power.Equals(SNOPower.Barbarian_Whirlwind))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.ZigZagPathing,
								WaitVars=new WaitLoops(0, 0, true),
								Cost=10,
								Range=15,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,

								PreCastConditions=(AbilityConditions.CheckEnergy|AbilityConditions.CheckPlayerIncapacitated),
								ClusterConditions=new ClusterConditions(10d,30f,2,true),
								
								Fcriteria=new Func<bool>(() =>
								{
									 return !this.bWaitingForSpecial&&
										  (!Bot.SettingsFunky.Class.bSelectiveWhirlwind||Bot.Combat.bAnyNonWWIgnoreMobsInRange||!CacheIDLookup.hashActorSNOWhirlwindIgnore.Contains(Bot.Target.CurrentTarget.SNOID))&&
										  // If they have battle-rage, make sure it's up
										  (!Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)||(Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)&&HasBuff(SNOPower.Barbarian_BattleRage)));
								}),

						  };
					 }
					 #endregion

					 #region Hammer of the ancients
					 // Hammer of the ancients spam-attacks - never use if waiting for special
					 if (Power.Equals(SNOPower.Barbarian_HammerOfTheAncients))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 2, true),
								Cost=20,
								Range=this.RuneIndexCache[Power]==0?13:this.RuneIndexCache[Power]==1?20:16,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								ClusterConditions=new ClusterConditions(6d, 20f, 1, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial,20),
								Fcriteria=new Func<bool>(() => { return !this.bWaitingForSpecial; }),
								
						  };
					 }
					 #endregion
					 #region Weapon Throw
					 // Weapon throw
					 if (Power.Equals(SNOPower.Barbarian_WeaponThrow))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=10,
								Range=44,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								
						  };
					 }
					 #endregion
					 #region Frenzy
					 // Frenzy rapid-attacks
					 if (Power.Equals(SNOPower.Barbarian_Frenzy))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 0, true),
								Cost=0,
								Range=10,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								
						  };
					 }
					 #endregion
					 #region Bash
					 // Bash fast-attacks
					 if (Power.Equals(SNOPower.Barbarian_Bash))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=0,
								Range=10,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckRecastTimer|AbilityConditions.CheckCanCast|AbilityConditions.CheckPlayerIncapacitated),
								
						  };
					 }
					 #endregion
					 #region Cleave
					 // Cleave fast-attacks
					 if (Power.Equals(SNOPower.Barbarian_Cleave))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target|AbilityUseType.ClusterTarget,
								WaitVars=new WaitLoops(0, 2, true),
								Cost=0,
								Range=10,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								ClusterConditions=new ClusterConditions(4d,10f,2,true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None),
						  };
					 }
					 #endregion

					 if (Power==SNOPower.Weapon_Melee_Instant)
						  returnAbility=Ability.Instant_Melee_Attack;

					 return returnAbility;
				}

				public override Ability AbilitySelector(bool bCurrentlyAvoiding=false, bool bOOCBuff=false)
				{
					 return base.AbilitySelector(bCurrentlyAvoiding, bOOCBuff);
				}
		  }
	 
}