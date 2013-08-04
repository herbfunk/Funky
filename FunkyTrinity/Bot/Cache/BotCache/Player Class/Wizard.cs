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

		  internal class Wizard : Player
		  {
				 enum WizardActiveSkills
				 {
						Wizard_Electrocute=1765,
						Wizard_SlowTime=1769,
						Wizard_ArcaneOrb=30668,
						Wizard_Blizzard=30680,
						Wizard_EnergyShield=30708,
						Wizard_FrostNova=30718,
						Wizard_Hydra=30725,
						Wizard_MagicMissile=30744,
						Wizard_ShockPulse=30783,
						Wizard_WaveOfForce=30796,
						Wizard_Meteor=69190,
						Wizard_SpectralBlade=71548,
						Wizard_IceArmor=73223,
						Wizard_StormArmor=74499,
						Wizard_DiamondSkin=75599,
						Wizard_MagicWeapon=76108,
						Wizard_EnergyTwister=77113,
						Wizard_EnergyArmor=86991,
						Wizard_ExplosiveBlast=87525,
						Wizard_Disintegrate=91549,
						Wizard_RayOfFrost=93395,
						Wizard_MirrorImage=98027,
						Wizard_Familiar=99120,
						Wizard_ArcaneTorrent=134456,
						Wizard_Archon=134872,
						Wizard_Archon_ArcaneStrike=135166,
						Wizard_Archon_DisintegrationWave=135238,
						Wizard_Archon_SlowTime=135663,
						Wizard_Archon_ArcaneBlast=167355,
						Wizard_Archon_Teleport=167648,
						Wizard_Teleport=168344,

				 }
				 enum WizardPassiveSkills
				 {
						Wizard_Passive_ColdBlooded=226301,
						Wizard_Passive_Paralysis=226348,
						Wizard_Passive_Conflagration=218044,
						Wizard_Passive_CriticalMass=218153,
						Wizard_Passive_ArcaneDynamo=208823,
						Wizard_Passive_GalvanizingWard=208541,
						Wizard_Passive_Illusionist=208547,
						Wizard_Passive_Blur=208468,
						Wizard_Passive_GlassCannon=208471,
						Wizard_Passive_AstralPresence=208472,
						Wizard_Passive_Evocation=208473,
						Wizard_Passive_UnstableAnomaly=208474,
						Wizard_Passive_TemporalFlux=208477,
						Wizard_Passive_PowerHungry=208478,
						Wizard_Passive_Prodigy=208493,

				 }

				internal bool bUsingCriticalMassPassive=false;

				//Base class for each individual class!
				public Wizard(ActorClass a)
					 : base(a)
				{
					 this.RecreateAbilities();
					 HasSignatureAbility=(this.HotbarPowers.Contains(SNOPower.Wizard_MagicMissile)||this.HotbarPowers.Contains(SNOPower.Wizard_ShockPulse)||
									this.HotbarPowers.Contains(SNOPower.Wizard_SpectralBlade)||this.HotbarPowers.Contains(SNOPower.Wizard_Electrocute));

					 //Check passive critical mass
					 this.bUsingCriticalMassPassive=base.PassivePowers.Contains(SNOPower.Wizard_Passive_CriticalMass);
				}
				public override void RecreateAbilities()
				{
					 base.Abilities=new Dictionary<SNOPower, Ability>();

					 //Create the abilities
					 foreach (var item in base.HotbarPowers)
					 {
						  base.Abilities.Add(item, this.CreateAbility(item));
					 }

					 //Sort Abilities
					 base.SortedAbilities=base.Abilities.Values.OrderByDescending(a => a.Priority).ThenByDescending(a => a.Range).ToList();
				}
				private bool HasSignatureAbility=false;

				public override void GenerateNewZigZagPath()
				{
					 Vector3 loc=Vector3.Zero;
					 //Low HP -- Flee Teleport
					 if (Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP&&Bot.Character.dCurrentHealthPct<0.5d&&(Bot.NavigationCache.AttemptFindSafeSpot(out loc, Bot.Target.CurrentTarget.Position, true)))
						  Bot.Combat.vSideToSideTarget=loc;
					 else
						  Bot.Combat.vSideToSideTarget=Bot.NavigationCache.FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, Bot.Target.CurrentTarget.CentreDistance, true);
				}
				public override int MainPetCount
				{
					 get
					 {
						  return Bot.Character.PetData.WizardHydra;
					 }
				}
				public override bool IsMeleeClass
				{
					 get
					 {
						  //bool LastUsedCloseRangeAbility=(SNOPower.Wizard_Archon_ArcaneStrike|SNOPower.Wizard_Archon_ArcaneBlast).HasFlag(Bot.Combat.powerLastSnoPowerUsed);
						  return false;
					 }
				}

				private bool MissingBuffs()
				{
					 HashSet<SNOPower> abilities_=HasBuff(SNOPower.Wizard_Archon)?base.CachedPowers:base.HotbarPowers;

					 if ((abilities_.Contains(SNOPower.Wizard_EnergyArmor)&&!HasBuff(SNOPower.Wizard_EnergyArmor))||(abilities_.Contains(SNOPower.Wizard_IceArmor)&&!HasBuff(SNOPower.Wizard_IceArmor))||(abilities_.Contains(SNOPower.Wizard_StormArmor)&&!HasBuff(SNOPower.Wizard_StormArmor)))
						  return true;

					 if (abilities_.Contains(SNOPower.Wizard_MagicWeapon)&&!HasBuff(SNOPower.Wizard_MagicWeapon))
						  return true;

					 return false;
						  
				}
				public override Ability CreateAbility(SNOPower Power)
				{
					 Ability returnAbility=null;
					 #region Teleport
					 // Teleport in combat for critical-mass wizards
					 if (Power.Equals(SNOPower.Wizard_Teleport))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.ClusterLocation|AbilityUseType.ZigZagPathing,
								WaitVars=new WaitLoops(0,1,true),
								Cost=15,
								Range=35,
								UseAvoiding=false,
								UseOOCBuff=false,
								IsNavigationSpecial=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast|AbilityConditions.CheckEnergy),
								ClusterConditions=new ClusterConditions(5d, 48f, 2, false),
								TestCustomCombatConditionAlways=true,
								Fcriteria=new Func<bool>(() =>
								{
									 return ((Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP&&Bot.Character.dCurrentHealthPct<0.5d)
											||(Bot.SettingsFunky.Class.bTeleportIntoGrouping&&Funky.Clusters(5d, 48f, 2, false).Count>0&&Funky.Clusters()[0].Midpoint.Distance(Bot.Character.PointPosition)>15f)
											||(!Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP&&!Bot.SettingsFunky.Class.bTeleportIntoGrouping));
								}),
						  };
					 }
					 #endregion
					 #region Archon
					 // Archon
					 if (Power.Equals(SNOPower.Wizard_Archon))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(4, 5, true),
								Cost=25,
								UseAvoiding=false,
								UseOOCBuff=false,
								IsSpecialAbility=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast|AbilityConditions.CheckEnergy),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_30, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 30),
								Fcriteria=new Func<bool>(()=>
								{
									 bool missingBuffs=this.MissingBuffs();
									 if (missingBuffs)
										  base.bWaitingForSpecial=true;

									 return !missingBuffs;
								}),
						  };
					 }
					 #endregion
					 #region Explosive Blast
					 // Explosive Blast SPAM when enough AP, blow erry thing up, nah mean
					 if (Power.Equals(SNOPower.Wizard_ExplosiveBlast))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(0, 0, true),
								Cost=20,
								Range=10,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast|AbilityConditions.CheckEnergy),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 12),
						  };
					 }
					 #endregion

					 #region Energy Twister
					 // Energy Twister SPAMS whenever 35 or more ap to generate Arcane Power
					 if (Power.Equals(SNOPower.Wizard_EnergyTwister))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Location,
								WaitVars=new WaitLoops(0, 0, true),
								Cost=35,
								Range=this.bUsingCriticalMassPassive?9:28,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast),
								
								Fcriteria=new Func<bool>(() =>
								{
									 return (!HasSignatureAbility||this.GetBuffStacks(SNOPower.Wizard_EnergyTwister)<1)&&
												(Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_30]>=1||Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_25]>=1||Bot.Target.CurrentTarget.RadiusDistance<=12f)&&
												(!Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_Electrocute)||!CacheIDLookup.hashActorSNOFastMobs.Contains(Bot.Target.CurrentTarget.SNOID))&&
												((this.bUsingCriticalMassPassive&&(!HasSignatureAbility||Bot.Character.dCurrentEnergy>=35))||(!this.bUsingCriticalMassPassive&&Bot.Character.dCurrentEnergy>=35));
								}),
						  };
					 }
					 #endregion
					 #region Disintegrate
					 // Disintegrate
					 if (Power.Equals(SNOPower.Wizard_Disintegrate))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 0, true),
								Cost=20,
								Range=this.bUsingCriticalMassPassive?20:35,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								Fcriteria=new Func<bool>(() => { return !this.bWaitingForSpecial; }),
								
						  };
					 }
					 #endregion
					 #region Arcane Orb
					 // Arcane Orb
					 if (Power.Equals(SNOPower.Wizard_ArcaneOrb))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=35,
								Range=this.bUsingCriticalMassPassive?20:40,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy),
								ClusterConditions=new ClusterConditions(5d, this.bUsingCriticalMassPassive?20:40, 1, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial),
								Fcriteria=new Func<bool>(() => { return !this.bWaitingForSpecial; }),
								
								
						  };
					 }
					 #endregion
					 #region Arcane Torrent
					 // Arcane Torrent
					 if (Power.Equals(SNOPower.Wizard_ArcaneTorrent))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 0, true),
								Cost=16,
								Range=40,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								Fcriteria=new Func<bool>(() => { return !this.bWaitingForSpecial; }),
								
						  };
					 }
					 #endregion
					 #region Ray of Frost
					 // Ray of Frost
					 if (Power.Equals(SNOPower.Wizard_RayOfFrost))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 0, true),
								Cost=35,
								Range=48,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
							
						  };
					 }
					 #endregion
					 #region Slow time
					 // Slow Time for in combat
					 if (Power.Equals(SNOPower.Wizard_SlowTime))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 35),

								
						  };
					 }
					 #endregion
					 #region Wave of force
					 // Wave of force
					 if (Power.Equals(SNOPower.Wizard_WaveOfForce))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 2, true),
								Cost=25,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer),
								
								Fcriteria=new Func<bool>(() =>
								{
									 return
										  // Check this isn't a critical mass wizard, cos they won't want to use this except for low health unless they don't have nova/blast in which case go for it
										  ((this.bUsingCriticalMassPassive&&((!Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_FrostNova)&&!Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_ExplosiveBlast))||
										  (Bot.Character.dCurrentHealthPct<=0.7&&(Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_15]>0||Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>0||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=23f)))))
										  // Else normal wizard in which case check standard stuff
										  ||(!this.bUsingCriticalMassPassive&&Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_15]>0||Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>3||Bot.Character.dCurrentHealthPct<=0.7||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=23f)));
								}),
						  };
					 }
					 #endregion
					 #region Blizzard
					 // Blizzard
					 if (Power.Equals(SNOPower.Wizard_Blizzard))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 2, true),
								Cost=40,
								Range=50,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial),
								ClusterConditions=new ClusterConditions(5d, 50f, 2, true),
								Fcriteria=new Func<bool>(() => { return !this.bWaitingForSpecial; }),
								
								
						  };
					 }
					 #endregion
					 #region Meteor
					 // Meteor
					 if (Power.Equals(SNOPower.Wizard_Meteor))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 2, true),
								Cost=50,
								Range=50,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								ClusterConditions=new ClusterConditions(4d, 50f, 2, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, falseConditionalFlags:TargetProperties.Fast),
								Fcriteria=new Func<bool>(() => { return !this.bWaitingForSpecial; }),
								
						  };
					 }
					 #endregion
					 #region Hydra
					 // Hydra
					 if (Power.Equals(SNOPower.Wizard_Hydra))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.ClusterLocation|AbilityUseType.Location,
								WaitVars=new WaitLoops(1, 2, true),
								Counter=1,
								Cost=15,
								Range=50,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPetCount),
								ClusterConditions=new ClusterConditions(7d, 50f, 2, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, falseConditionalFlags: TargetProperties.Fast),
								
						  };
					 }
					 #endregion
					 #region Mirror Image
					 // Mirror Image  @ half health or 5+ monsters or rooted/incapacitated or last elite left @25% health
					 if (Power.Equals(SNOPower.Wizard_MirrorImage))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Cost=10,
								Range=48,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckCanCast),
							
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Character.dCurrentHealthPct<=0.50||Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_30]>=5||Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted||Bot.Target.CurrentTarget.ObjectIsSpecial);
								}),
						  };
					 }
					 #endregion
					 #region Frost Nova
					 // Frost Nova SPAM
					 if (Power.Equals(SNOPower.Wizard_FrostNova))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 0, true),
								Range=14,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None, 12),
						  };
					 }
					 #endregion


					 #region Magic Missile
					 // Magic Missile
					 if (Power.Equals(SNOPower.Wizard_MagicMissile))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Range=40,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								
						  };
					 }
					 #endregion
					 #region Shock Pulse
					 // Shock Pulse
					 if (Power.Equals(SNOPower.Wizard_ShockPulse))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Range=this.RuneIndexCache[SNOPower.Wizard_ShockPulse]==2?40:this.RuneIndexCache[SNOPower.Wizard_ShockPulse]==1?35:15,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
						  };
					 }
					 #endregion
					 #region Spectral Blade
					 // Spectral Blade
					 if (Power.Equals(SNOPower.Wizard_SpectralBlade))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 1, true),
								Range=14,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
						  };
					 }
					 #endregion
					 #region Electrocute
					 // Electrocute
					 if (Power.Equals(SNOPower.Wizard_Electrocute))
					 {
						  return new Ability
						  {
								Power=Power,

								UsageType=AbilityUseType.Target|AbilityUseType.ClusterTarget,
								WaitVars=new WaitLoops(0, 0, true),
								Range=(this.RuneIndexCache[SNOPower.Wizard_Electrocute]==2?15:40),
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),

								//Aim for cluster with 2 units very close together.
								ClusterConditions=new ClusterConditions(3d, this.RuneIndexCache[SNOPower.Wizard_Electrocute]==2?15:40, 2, true),
								//No conditions for a single target.
								TargetUnitConditionFlags=new UnitTargetConditions(),

						  };
					 }
					 #endregion




					 #region Diamond Skin
					 // Diamond Skin SPAM
					 if (Power.Equals(SNOPower.Wizard_DiamondSkin))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(0, 1, true),
								Cost=10,
								Counter=1,
								Range=0,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast|AbilityConditions.CheckExisitingBuff),
								
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_25]>0||Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_25]>0||Bot.Character.dCurrentHealthPct<=0.90||Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted||(Bot.Target.CurrentTarget.RadiusDistance<=40f));
								}),
						  };
					 }
					 #endregion
					 #region wizard armors

					 // Energy armor as priority cast if available and not buffed
					 if (Power.Equals(SNOPower.Wizard_EnergyArmor)||Power.Equals(SNOPower.Wizard_IceArmor)||Power.Equals(SNOPower.Wizard_StormArmor))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 2, true),
								Cost=25,
								Counter=1,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckExisitingBuff),
						  };
					 }

					 #endregion
					 #region Magic Weapon
					 // Magic Weapon
					 if (Power.Equals(SNOPower.Wizard_MagicWeapon))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 2, true),
								Cost=25,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckExisitingBuff),
						  };
					 }
					 #endregion
					 #region Familiar
					 // Familiar
					 if (Power.Equals(SNOPower.Wizard_Familiar))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 2, true),
								Cost=25,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								
						  };
					 }
					 #endregion


					 // Archon form
					 #region Slow Time
					 // Archon Slow Time for in combat
					 if (Power.Equals(SNOPower.Wizard_Archon_SlowTime))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 1, true),
								Range=48,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckCanCast),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 35),

						  };
					 }
					 #endregion
					 #region Teleport
					 // Archon Teleport in combat
					 if (Power.Equals(SNOPower.Wizard_Archon_Teleport))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.ClusterLocation|AbilityUseType.ZigZagPathing,
								WaitVars=new WaitLoops(1, 1, true),
								Range=48,
								UseAvoiding=true,
								UseOOCBuff=false,
								IsNavigationSpecial=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast),
								//UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 3),
								//ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1),
								//TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.Boss, 15),
								ClusterConditions=new ClusterConditions(5d, 48f, 2, false),
								TestCustomCombatConditionAlways=true,
								Fcriteria=new Func<bool>(()=>
								{
									 return ((Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP&&Bot.Character.dCurrentHealthPct<0.5d)
												||(Bot.SettingsFunky.Class.bTeleportIntoGrouping&&Funky.Clusters(5d, 48f, 2, false).Count>0&&Funky.Clusters()[0].Midpoint.Distance(Bot.Character.PointPosition)>15f)
												||(!Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP&&!Bot.SettingsFunky.Class.bTeleportIntoGrouping));

								}),
						  };
					 }
					 #endregion
					 #region Arcane Blast
					 // Arcane Blast
					 if (Power.Equals(SNOPower.Wizard_Archon_ArcaneBlast))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Buff,
								WaitVars=new WaitLoops(1, 1, true),
								Range=15,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast),
								
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_6, 2),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_6, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 8),


								Fcriteria=new Func<bool>(()=>
								{//We only want to use this if there are nearby units!
									 return Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_12]>0;
								}),

						  };
					 }
					 #endregion
					 #region Disintegrate
					 // Disintegrate
					 if (Power.Equals(SNOPower.Wizard_Archon_DisintegrationWave))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(0, 0, true),
								Range=48,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.TargetableAndAttackable),
								Fcriteria=new Func<bool>(()=>
								{
									 return Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_6]==0;
								}),
						  };
					 }
					 #endregion
					 #region Arcane Strike
					 // Arcane Strike (Arcane Strike) Rapid Spam at close-range only
					 if (Power.Equals(SNOPower.Wizard_Archon_ArcaneStrike))
					 {
						  return new Ability
						  {
								Power=Power,
								
								UsageType=AbilityUseType.Target,
								WaitVars=new WaitLoops(1, 1, true),
								Range=15,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_12, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None, 6),

								Fcriteria=new Func<bool>(() =>
								{//We only want to use this if there are nearby units!
									 return Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_12]>0;
								}),
						  };
					 }
					 #endregion


					 if (Power==SNOPower.Weapon_Ranged_Wand) returnAbility=Ability.Wand_Range_Attack;

					 return returnAbility;
				}
				public override Ability DestructibleAbility()
				{
					 SNOPower destructiblePower=this.DestructiblePower();
					 Ability returnAbility=this.Abilities[destructiblePower];

					 if (destructiblePower==SNOPower.Wizard_EnergyTwister&&Bot.Character.dCurrentEnergy<35)
					 {
						  destructiblePower=this.DestructiblePower(destructiblePower);
					 }

					 returnAbility.SetupAbilityForUse();
					 return returnAbility;
				}

				public override Ability AbilitySelector(bool bCurrentlyAvoiding=false, bool bOOCBuff=false)
				{
					 if ((bCurrentlyAvoiding||bOOCBuff)
						  &&(Bot.SettingsFunky.Class.bCancelArchonRebuff&&HasBuff(SNOPower.Wizard_Archon)))
					 {
						  if (MissingBuffs())
						  {//We are missing buffs.. time to cancel archon!
								return Ability.Cancel_Archon_Buff;
						  }
					 }

					 // Wizards want to save up to a reserve of 65+ energy
					 this.iWaitingReservedAmount=65;


					 return base.AbilitySelector(bCurrentlyAvoiding, bOOCBuff);
				}
		  }
	 
}