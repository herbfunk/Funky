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
				}
				private bool HasSignatureAbility=false;

				public override void GenerateNewZigZagPath()
				{
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
						  return false;
					 }
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
								UsageType=AbilityUseType.ZigZagPathing,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Cost=15,
								Range=35,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast | AbilityConditions.CheckEnergy ),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return Bot.Combat.powerLastSnoPowerUsed!=SNOPower.Wizard_Teleport&&
									Bot.Character.dCurrentEnergy>=15&&Bot.Target.CurrentTarget.CentreDistance<=35f;
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
								AbilityWaitVars=new Tuple<int, int, bool>(4, 5, true),
								Cost=25,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated|  AbilityConditions.CheckCanCast | AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iElitesWithinRange[RANGE_30]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>=1||Bot.Character.dCurrentHealthPct<=0.6||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=30f));
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=20,
								Range=10,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast | AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return ((Bot.Combat.iElitesWithinRange[RANGE_25]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>=1||Bot.Character.dCurrentHealthPct<=0.7)&&Bot.Target.CurrentTarget.RadiusDistance<=12f);
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=35,
								Range=Bot.SettingsFunky.Class.bEnableCriticalMass?9:28,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy | AbilityConditions.CheckCanCast),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (!HasSignatureAbility||this.GetBuffStacks(SNOPower.Wizard_EnergyTwister)<1)&&
												(Bot.Combat.iElitesWithinRange[RANGE_30]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>=1||Bot.Target.CurrentTarget.RadiusDistance<=12f)&&
												(!Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Electrocute)||!SnoCacheLookup.hashActorSNOFastMobs.Contains(Bot.Target.CurrentTarget.SNOID))&&
												((Bot.SettingsFunky.Class.bEnableCriticalMass&&(!HasSignatureAbility||Bot.Character.dCurrentEnergy>=35))||(!Bot.SettingsFunky.Class.bEnableCriticalMass&&Bot.Character.dCurrentEnergy>=35));
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=20,
								Range=Bot.SettingsFunky.Class.bEnableCriticalMass?20:35,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return true;
								}),
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
								ClusterConditions=new Tuple<double,float,int,bool>(5d,Bot.SettingsFunky.Class.bEnableCriticalMass?20:40,1,true),
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=35,
								Range=Bot.SettingsFunky.Class.bEnableCriticalMass?20:40,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckRecastTimer | AbilityConditions.CheckEnergy),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return Clusters(5d,Bot.SettingsFunky.Class.bEnableCriticalMass?20:40,2,true).Count>0||(Bot.Target.CurrentTarget.ObjectIsSpecial);
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=16,
								Range=40,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy | AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return true;
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Cost=35,
								Range=48,
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
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>1||Bot.Character.dCurrentHealthPct<=0.7||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=35f));
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Cost=25,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy| AbilityConditions.CheckCanCast | AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return 
										  // Check this isn't a critical mass wizard, cos they won't want to use this except for low health unless they don't have nova/blast in which case go for it
										  ((Bot.SettingsFunky.Class.bEnableCriticalMass&&((!Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_FrostNova)&&!Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_ExplosiveBlast))||
										  (Bot.Character.dCurrentHealthPct<=0.7&&(Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>0||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=23f)))))
										  // Else normal wizard in which case check standard stuff
										  ||(!Bot.SettingsFunky.Class.bEnableCriticalMass&&Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>3||Bot.Character.dCurrentHealthPct<=0.7||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=23f)));
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
								UsageType= AbilityUseType.ClusterTarget|AbilityUseType.Target,
								ClusterConditions=new Tuple<double,float,int,bool>(5d,45,1,true),
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Cost=40,
								Range=45,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Clusters(4d, 45f, 4, true).Count>0)||(Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.ObjectIsSpecial);
								}),
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
								UsageType= AbilityUseType.ClusterTarget|AbilityUseType.Target,
								ClusterConditions=new Tuple<double,float,int,bool>(4d,45,1,true),
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Cost=40,
								Range=45,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy|AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Clusters(4d, 45f, 3, true).Count>0)||(Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.ObjectIsSpecial);
								}),
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
								UsageType= AbilityUseType.ClusterLocation|AbilityUseType.Location,
								ClusterConditions=new Tuple<double,float,int,bool>(8d, 45f, 1, true),
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Counter=1,
								Cost=15,
								Range=45,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy | AbilityConditions.CheckRecastTimer| AbilityConditions.CheckPetCount),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Clusters(8d, 45f, 3, true).Count>0) || (Bot.Target.CurrentTarget.ObjectIsSpecial);
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Cost=10,
								Range=48,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckCanCast),
								RuneIndex=this.RuneIndexCache[Power],
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
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Range=14,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return ((Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>0||Bot.Character.dCurrentHealthPct<=0.7)&&Bot.Target.CurrentTarget.RadiusDistance<=12f);
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Range=40,
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
					 #region Shock Pulse
					 // Shock Pulse
					 if (Power.Equals(SNOPower.Wizard_ShockPulse))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Range=this.RuneIndexCache[SNOPower.Wizard_ShockPulse]==2?40:this.RuneIndexCache[SNOPower.Wizard_ShockPulse]==1?35:15,
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
					 #region Spectral Blade
					 // Spectral Blade
					 if (Power.Equals(SNOPower.Wizard_SpectralBlade))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 1, true),
								Range=14,
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
					 #region Electrocute
					 // Electrocute
					 if (Power.Equals(SNOPower.Wizard_Electrocute))
					 {
						  return new Ability
						  {
								Power=Power,
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Range=(this.RuneIndexCache[SNOPower.Wizard_Electrocute]==2?15:40),
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
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckCanCast | AbilityConditions.CheckExisitingBuff),
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
								UsageType=AbilityUseType.Buff,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Cost=25,
								Counter=1,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy| AbilityConditions.CheckCanCast | AbilityConditions.CheckExisitingBuff),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return true;
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Cost=25,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy | AbilityConditions.CheckExisitingBuff | AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return true;
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 2, true),
								Cost=25,
								UseAvoiding=true,
								UseOOCBuff=true,
								Priority=AbilityPriority.High,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckEnergy | AbilityConditions.CheckRecastTimer),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return true;
								}),
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Range=48,
								UseAvoiding=true,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckRecastTimer | AbilityConditions.CheckCanCast),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>1||Bot.Character.dCurrentHealthPct<=0.7||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=35f));
								}),
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
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Range=48,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckRecastTimer | AbilityConditions.CheckCanCast),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iElitesWithinRange[RANGE_15]>=1||Bot.Combat.iAnythingWithinRange[RANGE_15]>=3||(Bot.Target.CurrentTarget.IsBoss&&Bot.Target.CurrentTarget.RadiusDistance<=15f));
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.Low,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated| AbilityConditions.CheckRecastTimer | AbilityConditions.CheckCanCast),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
									 return (Bot.Combat.iAnythingWithinRange[RANGE_6]>2||Bot.Combat.iElitesWithinRange[RANGE_6]>0||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=8f));
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
								AbilityWaitVars=new Tuple<int, int, bool>(1, 1, true),
								Range=12,
								UseAvoiding=false,
								UseOOCBuff=false,
								Priority=AbilityPriority.None,
								PreCastConditions=(AbilityConditions.CheckPlayerIncapacitated),
								RuneIndex=this.RuneIndexCache[Power],
								Fcriteria=new Func<bool>(() =>
								{
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
								UsageType=AbilityUseType.Target,
								AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
								Range=48,
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
					 return returnAbility;
				}
				public override Ability DestructibleAbility()
				{
					 SNOPower destructiblePower=this.DestructiblePower();
					 Ability returnAbility=this.Abilities[destructiblePower];

					 if (!this.HasBuff(SNOPower.Wizard_Archon))
					 {
						  if (destructiblePower==SNOPower.Wizard_EnergyTwister)
						  {
								if (Bot.Character.dCurrentEnergy<35)
									 return Instant_Melee_Attack;
						  }
						  returnAbility.SetupAbilityForUse();
						  returnAbility.MinimumRange=25f;
						  return returnAbility;
					 }
					 else
					 {
						  if (Bot.Target.CurrentTarget.RadiusDistance<=10f)
								returnAbility=this.Abilities[SNOPower.Wizard_Archon_ArcaneStrike];
						  else
								returnAbility=this.Abilities[SNOPower.Wizard_Archon_DisintegrationWave];

						  returnAbility.SetupAbilityForUse();
						  returnAbility.MinimumRange=25f;
						  return returnAbility;
					 }

					 
					 
				}
				public override Ability AbilitySelector(bool bCurrentlyAvoiding=false, bool bOOCBuff=false)
				{
					 // Wizards want to save up to a reserve of 65+ energy
					 this.iWaitingReservedAmount=65;


					 return base.AbilitySelector(bCurrentlyAvoiding, bOOCBuff);
				}
		  }
	 }
}