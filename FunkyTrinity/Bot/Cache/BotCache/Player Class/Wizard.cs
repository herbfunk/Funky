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
		  internal class Wizard : Player
		  {
				//Base class for each individual class!
				public Wizard(ActorClass a)
					 : base(a)
				{
					 this.RecreateAbilities();
					 HasSignatureAbility=(this.HotbarAbilities.Contains(SNOPower.Wizard_MagicMissile)||this.HotbarAbilities.Contains(SNOPower.Wizard_ShockPulse)||
									this.HotbarAbilities.Contains(SNOPower.Wizard_SpectralBlade)||this.HotbarAbilities.Contains(SNOPower.Wizard_Electrocute));
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
				private bool HasSignatureAbility=false;

				public override void GenerateNewZigZagPath()
				{
					 Vector3 loc;
					 //Low HP -- Flee Teleport
					 if (Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP&&Bot.Character.dCurrentHealthPct<0.5d&&
								(GridPointAreaCache.AttemptFindSafeSpot(out loc, Bot.Target.CurrentTarget.Position, true)))
						  Bot.Combat.vSideToSideTarget=loc;
					 else
						  Bot.Combat.vSideToSideTarget=FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, Bot.Target.CurrentTarget.CentreDistance, true);
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
					 HashSet<SNOPower> abilities_=HasBuff(SNOPower.Wizard_Archon)?base.CachedAbilities:base.HotbarAbilities;

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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.ClusterLocation|AbilityUseType.ZigZagPathing,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=15,
								Range=35,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast|AbilityConditions.CheckEnergy),
								ClusterConditions=new Tuple<double, float, int, bool>(5d, 48f, 2, false),
								TestCustomCombatConditionAlways=true,
								Fcriteria=new Func<bool>(() =>
								{
									 return ((Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP&&Bot.Character.dCurrentHealthPct<0.5d)
											||(Bot.SettingsFunky.Class.bTeleportIntoGrouping&&Clusters(5d, 48f, 2, false).Count>0&&Clusters()[0].Midpoint.Distance(Bot.Character.PointPosition)>15f)
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(4, 5, true),
								Cost=25,
								UseAvoiding=false,
								UseOOCBuff=false,
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Location,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=35,
								Range=base.bUsingCriticalMassPassive?9:28,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast),
								
								Fcriteria=new Func<bool>(() =>
								{
									 return (!HasSignatureAbility||this.GetBuffStacks(SNOPower.Wizard_EnergyTwister)<1)&&
												(Bot.Combat.iElitesWithinRange[RANGE_30]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>=1||Bot.Target.CurrentTarget.RadiusDistance<=12f)&&
												(!Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Electrocute)||!SnoCacheLookup.hashActorSNOFastMobs.Contains(Bot.Target.CurrentTarget.SNOID))&&
												((base.bUsingCriticalMassPassive&&(!HasSignatureAbility||Bot.Character.dCurrentEnergy>=35))||(!base.bUsingCriticalMassPassive&&Bot.Character.dCurrentEnergy>=35));
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=20,
								Range=base.bUsingCriticalMassPassive?20:35,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy),
								
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=35,
								Range=base.bUsingCriticalMassPassive?20:40,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckEnergy),
								ClusterConditions=new Tuple<double, float, int, bool>(5d, base.bUsingCriticalMassPassive?20:40, 1, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial),

								
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
								RuneIndex=this.RuneIndexCache[Power],

								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=16,
								Range=40,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
							
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2),
								ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 35),

								RuneIndex=this.RuneIndexCache[Power],
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Cost=25,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckCanCast|AbilityConditions.CheckRecastTimer),
								
								Fcriteria=new Func<bool>(() =>
								{
									 return
										  // Check this isn't a critical mass wizard, cos they won't want to use this except for low health unless they don't have nova/blast in which case go for it
										  ((base.bUsingCriticalMassPassive&&((!Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_FrostNova)&&!Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_ExplosiveBlast))||
										  (Bot.Character.dCurrentHealthPct<=0.7&&(Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>0||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=23f)))))
										  // Else normal wizard in which case check standard stuff
										  ||(!base.bUsingCriticalMassPassive&&Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>3||Bot.Character.dCurrentHealthPct<=0.7||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=23f)));
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Cost=40,
								Range=50,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial),
								ClusterConditions=new Tuple<double, float, int, bool>(5d, 50f, 1, true),

								
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.ClusterTarget|AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Cost=50,
								Range=50,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								ClusterConditions=new Tuple<double, float, int, bool>(4d, 50f, 1, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial),
								
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.ClusterLocation|AbilityUseType.Location,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Counter=1,
								Cost=15,
								Range=50,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckPetCount),
								ClusterConditions=new Tuple<double, float, int, bool>(7d, 50f, 1, true),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial),
								
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=10,
								Range=48,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckCanCast),
							
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Character.dCurrentHealthPct<=0.50||Bot.Combat.iAnythingWithinRange[RANGE_30]>=5||Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted||Bot.Target.CurrentTarget.ObjectIsSpecial);
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Range=(this.RuneIndexCache[SNOPower.Wizard_Electrocute]==2?15:40),
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=10,
								Counter=1,
								Range=0,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckCanCast|AbilityConditions.CheckExisitingBuff),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>0||Bot.Character.dCurrentHealthPct<=0.90||Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted||(Bot.Target.CurrentTarget.RadiusDistance<=40f));
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.ClusterLocation|AbilityUseType.ZigZagPathing,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Range=48,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|AbilityConditions.CheckRecastTimer|AbilityConditions.CheckCanCast),
								//UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 3),
								//ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1),
								//TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.Boss, 15),
								ClusterConditions=new Tuple<double,float,int,bool>(5d, 48f, 2, false),
								TestCustomCombatConditionAlways=true,
								Fcriteria=new Func<bool>(()=>
								{
									 return ((Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP&&Bot.Character.dCurrentHealthPct<0.5d)
												||(Bot.SettingsFunky.Class.bTeleportIntoGrouping&&Clusters(5d, 48f, 2, false).Count>0&&Clusters()[0].Midpoint.Distance(Bot.Character.PointPosition)>15f)
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
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
									 return Bot.Combat.iAnythingWithinRange[RANGE_12]>0;
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Range=48,
								IsRanged=true,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.TargetableAndAttackable),
								Fcriteria=new Func<bool>(()=>
								{
									 return Bot.Combat.iAnythingWithinRange[RANGE_6]==0;
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
								RuneIndex=this.RuneIndexCache[Power],
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Range=15,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_12, 1),
								TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None, 6),

								Fcriteria=new Func<bool>(() =>
								{//We only want to use this if there are nearby units!
									 return Bot.Combat.iAnythingWithinRange[RANGE_12]>0;
								}),
						  };
					 }
					 #endregion

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
								return Cancel_Archon_Buff;
						  }
					 }

					 // Wizards want to save up to a reserve of 65+ energy
					 this.iWaitingReservedAmount=65;


					 return base.AbilitySelector(bCurrentlyAvoiding, bOOCBuff);
				}
		  }
	 }
}