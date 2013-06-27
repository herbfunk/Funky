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

				}
				public override bool IsMeleeClass
				{
					 get
					 {
						  return false;
					 }
				}
				public override Ability AbilitySelector(bool bCurrentlyAvoiding=false, bool bOOCBuff=false, bool bDestructiblePower=false)
				{

					 // Pick the best destructible power available
					 if (bDestructiblePower)
					 {
						  if (!this.HasBuff(SNOPower.Wizard_Archon))
						  {
								SNOPower destructiblePower=this.DestructiblePower();
								if (destructiblePower==SNOPower.Wizard_EnergyTwister)
								{
									 if (Bot.Character.dCurrentEnergy>=35)
										  return new Ability(SNOPower.Wizard_EnergyTwister, 9f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
									 else
										  return new Ability(SNOPower.Weapon_Melee_Instant, 10f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
								}
								return new Ability(destructiblePower, 10f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
						  }
						  else
						  {
								if (Bot.Target.CurrentTarget.RadiusDistance<=10f)
									 return new Ability(SNOPower.Wizard_Archon_ArcaneStrike, 20f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, USE_SLOWLY);
								return new Ability(SNOPower.Wizard_Archon_DisintegrationWave, 19f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, USE_SLOWLY);
						  }
					 }

					 // Wizards want to save up to a reserve of 65+ energy
					 this.iWaitingReservedAmount=65;
					 if (!this.HasBuff(SNOPower.Wizard_Archon))
					 {

						  if (!bOOCBuff)
						  {

								#region Teleport
								// Teleport in combat for critical-mass wizards
								if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Teleport)&&Bot.SettingsFunky.Class.bEnableCriticalMass&&
									Bot.Combat.powerLastSnoPowerUsed!=SNOPower.Wizard_Teleport&&
									Bot.Character.dCurrentEnergy>=15&&Bot.Target.CurrentTarget.CentreDistance<=35f&&
									PowerManager.CanCast(SNOPower.Wizard_Teleport))
								{
									 Bot.Combat.vSideToSideTarget=FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, Bot.Target.CurrentTarget.CentreDistance, true);
									 return new Ability(SNOPower.Wizard_Teleport, 35f, Bot.Combat.vSideToSideTarget, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
								}
								#endregion
								#region Archon
								// Archon
								if (!bOOCBuff&&!bCurrentlyAvoiding&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Archon)&&
									(Bot.Combat.iElitesWithinRange[RANGE_30]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>=1||Bot.Character.dCurrentHealthPct<=0.6||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=30f))&&
									Bot.Character.dCurrentEnergy>=25&&Bot.Character.dCurrentHealthPct>=0.10&&
									PowerManager.CanCast(SNOPower.Wizard_Archon))
								{
									 // Familiar has been removed for now. Uncomment the three comments below relating to familiars to force re-buffing them
									 bool bHasBuffAbilities=(Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_MagicWeapon)|| //hashPowerHotbarAbilities.Contains(SNOPower.Wizard_Familiar) ||
										 Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_EnergyArmor)||Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_IceArmor)||
										 Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_StormArmor));

									 int iExtraEnergyNeeded=25;

									 if (Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_MagicWeapon)) iExtraEnergyNeeded+=25;

									 if (Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_EnergyArmor)||Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_IceArmor)||
										 Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_StormArmor)) iExtraEnergyNeeded+=25;

									 if (!bHasBuffAbilities||Bot.Character.dCurrentEnergy<=iExtraEnergyNeeded)
										  this.bWaitingForSpecial=true;

									 if (!this.bWaitingForSpecial)
									 {
										  dictAbilityLastUse[SNOPower.Wizard_MagicWeapon]=DateTime.Today;
										  dictAbilityLastUse[SNOPower.Wizard_EnergyArmor]=DateTime.Today;
										  dictAbilityLastUse[SNOPower.Wizard_IceArmor]=DateTime.Today;
										  dictAbilityLastUse[SNOPower.Wizard_StormArmor]=DateTime.Today;
										  this.bWaitingForSpecial=true;
									 }
									 else
									 {
										  this.bWaitingForSpecial=false;
										  return new Ability(SNOPower.Wizard_Archon, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 4, 5, USE_SLOWLY);
									 }
								}
								#endregion
								#region Explosive Blast
								// Explosive Blast SPAM when enough AP, blow erry thing up, nah mean
								if (!bOOCBuff&&!bCurrentlyAvoiding&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_ExplosiveBlast)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=20&&
									((Bot.Combat.iElitesWithinRange[RANGE_25]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>=1||Bot.Character.dCurrentHealthPct<=0.7)&&Bot.Target.CurrentTarget.RadiusDistance<=12f)&&
									PowerManager.CanCast(SNOPower.Wizard_ExplosiveBlast))
								{
									 float fThisRange=11f;
									 if (Bot.SettingsFunky.Class.bEnableCriticalMass)
										  fThisRange=9f;
									 return new Ability(SNOPower.Wizard_ExplosiveBlast, fThisRange, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
								}
								#endregion

								// Check to see if we have a signature spell on our hotbar, for energy twister check
								bool bHasSignatureSpell=(Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_MagicMissile)||Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_ShockPulse)||
									Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_SpectralBlade)||Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Electrocute));

								#region Energy Twister
								// Energy Twister SPAMS whenever 35 or more ap to generate Arcane Power
								if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_EnergyTwister)&&
									 // If using storm chaser, then force a signature spell every 1 stack of the buff, if we have a signature spell
									(!bHasSignatureSpell||this.GetBuffStacks(SNOPower.Wizard_EnergyTwister)<1)&&
									(Bot.Combat.iElitesWithinRange[RANGE_30]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>=1||Bot.Target.CurrentTarget.RadiusDistance<=12f)&&
									(!Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Electrocute)||!SnoCacheLookup.hashActorSNOFastMobs.Contains(Bot.Target.CurrentTarget.SNOID))&&
									((Bot.SettingsFunky.Class.bEnableCriticalMass&&(!bHasSignatureSpell||Bot.Character.dCurrentEnergy>=35))||(!Bot.SettingsFunky.Class.bEnableCriticalMass&&Bot.Character.dCurrentEnergy>=35)))
								{
									 float fThisRange=28f;
									 if (Bot.SettingsFunky.Class.bEnableCriticalMass)
										  fThisRange=9f;
									 return new Ability(SNOPower.Wizard_EnergyTwister, fThisRange, new Vector3(Bot.Target.CurrentTarget.Position.X, Bot.Target.CurrentTarget.Position.Y, Bot.Target.CurrentTarget.Position.Z), Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
								}
								#endregion
								#region Disintegrate
								// Disintegrate
								if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Disintegrate)&&Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.targetType.Value==TargetType.Unit&&
									((Bot.Character.dCurrentEnergy>=20&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount))
								{
									 float fThisRange=35f;
									 if (Bot.SettingsFunky.Class.bEnableCriticalMass)
										  fThisRange=20f;
									 return new Ability(SNOPower.Wizard_Disintegrate, fThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, SIGNATURE_SPAM);
								}
								#endregion
								#region Arcane Orb
								// Arcane Orb
								if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_ArcaneOrb)&&
									((Bot.Character.dCurrentEnergy>=35&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount)&&
									Bot.Class.AbilityUseTimer(SNOPower.Wizard_ArcaneOrb))
								{
									 float fThisRange=40f;
									 if (Bot.SettingsFunky.Class.bEnableCriticalMass)
										  fThisRange=20f;

									 if (Clusters(4d, fThisRange, 2, true).Count>0)
									 {
										  int ACD=Clusters()[0].ListUnits[0].AcdGuid.Value;
										  return new Ability(SNOPower.Wizard_ArcaneOrb, fThisRange, vNullLocation, -1, ACD, 1, 1, USE_SLOWLY);
									 }
									 else if (Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.ObjectIsSpecial)
									 {
										  return new Ability(SNOPower.Wizard_ArcaneOrb, fThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
									 }
								}
								#endregion
								#region Arcane Torrent
								// Arcane Torrent
								if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_ArcaneTorrent)&&Bot.Target.CurrentTarget!=null&&
									((Bot.Character.dCurrentEnergy>=16&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=this.iWaitingReservedAmount)&&
									Bot.Class.AbilityUseTimer(SNOPower.Wizard_ArcaneTorrent))
								{
									 float fThisRange=40f;
									 /*if (settings.bEnableCriticalMass)
										 fThisRange = 20f;*/
									 return new Ability(SNOPower.Wizard_ArcaneTorrent, fThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, USE_SLOWLY);
								}
								#endregion
								#region Ray of Frost
								// Ray of Frost
								if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_RayOfFrost)&&Bot.Target.CurrentTarget!=null&&
									Bot.Character.dCurrentEnergy>=12)
								{
									 float fThisRange=35f;
									 if (Bot.SettingsFunky.Class.bEnableCriticalMass)
										  fThisRange=20f;
									 return new Ability(SNOPower.Wizard_RayOfFrost, fThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, SIGNATURE_SPAM);
								}
								#endregion
								#region Slow time
								// Slow Time for in combat
								if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_SlowTime)&&
									(Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>1||Bot.Character.dCurrentHealthPct<=0.7||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=35f))&&
									PowerManager.CanCast(SNOPower.Wizard_SlowTime))
								{
									 return new Ability(SNOPower.Wizard_SlowTime, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
								}
								#endregion
								#region Wave of force
								// Wave of force
								if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=25&&
									(
									 // Check this isn't a critical mass wizard, cos they won't want to use this except for low health unless they don't have nova/blast in which case go for it
									(Bot.SettingsFunky.Class.bEnableCriticalMass&&((!Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_FrostNova)&&!Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_ExplosiveBlast))||
										(Bot.Character.dCurrentHealthPct<=0.7&&(Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>0||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=23f)))))
									 // Else normal wizard in which case check standard stuff
									||(!Bot.SettingsFunky.Class.bEnableCriticalMass&&Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>3||Bot.Character.dCurrentHealthPct<=0.7||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=23f))
									)&&
									Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_WaveOfForce)&&
									Bot.Class.AbilityUseTimer(SNOPower.Wizard_WaveOfForce, true)&&PowerManager.CanCast(SNOPower.Wizard_WaveOfForce))
								{
									 return new Ability(SNOPower.Wizard_WaveOfForce, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
								}
								#endregion
								#region Blizzard
								// Blizzard
								if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Blizzard)&&
									Bot.Combat.powerLastSnoPowerUsed!=SNOPower.Wizard_Blizzard&&
									Bot.Character.dCurrentEnergy>=40&&Bot.Class.AbilityUseTimer(SNOPower.Wizard_Blizzard))
								{
									 if (Clusters(4d, 45f, 4, true).Count>0||(Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.ObjectIsSpecial&&Clusters(4d, 45f, 2).Count>0))
									 {
										  Vector3 Location=Clusters()[0].ListUnits[0].Position;
										  return new Ability(SNOPower.Wizard_Blizzard, 45f, Location, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
									 }
								}
								#endregion
								#region Meteor
								// Meteor
								if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Meteor)&&
									 !Bot.Target.CurrentTarget.IsTreasureGoblin&&
									 Bot.Character.dCurrentEnergy>=50&&PowerManager.CanCast(SNOPower.Wizard_Meteor))
								{
									 if (Clusters(4d, 45f, 3, true).Count>0)
									 {
										  Vector3 Location=Clusters()[0].ListUnits[0].Position;
										  return new Ability(SNOPower.Wizard_Meteor, 45f, Location, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
									 }

								}
								#endregion
								#region Hydra
								// Hydra
								if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&
									Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Hydra)&& //(Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>4||Bot.Character.dCurrentHealthPct<=0.7||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss||Bot.Target.CurrentTarget.IsTreasureGoblin)&&Bot.Target.CurrentTarget.RadiusDistance<=15f))&&
									Bot.Character.PetData.WizardHydra==0&&
									Bot.Character.dCurrentEnergy>=15&&Bot.Class.AbilityUseTimer(SNOPower.Wizard_Hydra)
									&&!Bot.Target.CurrentTarget.IsTreasureGoblin)
								{
									 if (Clusters(10d, 45f, 3, true).Count>0)
									 {
										  Vector3 Location=Clusters()[0].ListUnits[0].Position;
										  return new Ability(SNOPower.Wizard_Hydra, 45f, Location, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
									 }
									 else if (Bot.Target.CurrentTarget.ObjectIsSpecial)
									 {
										  return new Ability(SNOPower.Wizard_Hydra, 45f, Bot.Target.CurrentTarget.Position, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
									 }
								}
								#endregion
								#region Mirror Image
								// Mirror Image  @ half health or 5+ monsters or rooted/incapacitated or last elite left @25% health
								if (!bOOCBuff&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_MirrorImage)&&
									(Bot.Character.dCurrentHealthPct<=0.50||Bot.Combat.iAnythingWithinRange[RANGE_30]>=5||Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted||Bot.Target.CurrentTarget.ObjectIsSpecial)&&
									PowerManager.CanCast(SNOPower.Wizard_MirrorImage))
								{
									 return new Ability(SNOPower.Wizard_MirrorImage, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
								}
								#endregion
								#region Frost Nova
								// Frost Nova SPAM
								if (!bOOCBuff&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_FrostNova)&&!Bot.Character.bIsIncapacitated&&
									((Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>0||Bot.Character.dCurrentHealthPct<=0.7)&&Bot.Target.CurrentTarget.RadiusDistance<=12f)&&
									PowerManager.CanCast(SNOPower.Wizard_FrostNova))
								{
									 float fThisRange=14f;
									 if (Bot.SettingsFunky.Class.bEnableCriticalMass)
										  fThisRange=9f;
									 return new Ability(SNOPower.Wizard_FrostNova, fThisRange, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
								}
								#endregion


								#region Magic Missile
								// Magic Missile
								if (!bOOCBuff&&!bCurrentlyAvoiding&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_MagicMissile)&&Bot.Target.CurrentTarget!=null&&(!Bot.Target.CurrentTarget.IsMissileReflecting||Bot.Character.dCurrentEnergy<30))
								{
									 float fThisRange=35f;
									 return new Ability(SNOPower.Wizard_MagicMissile, fThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, USE_SLOWLY);
								}
								#endregion
								#region Shock Pulse
								// Shock Pulse
								if (!bOOCBuff&&!bCurrentlyAvoiding&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_ShockPulse)&&Bot.Target.CurrentTarget!=null)
								{
									 float range=15f;
									 //rune index
									 //1 == walking
									 //2 == piercing orb
									 int runeIndex=this.RuneIndexCache[SNOPower.Wizard_ShockPulse];
									 if (runeIndex==2)
										  range=40f;
									 else if (runeIndex==1)
										  range=35f;

									 return new Ability(SNOPower.Wizard_ShockPulse, range, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
								}
								#endregion
								#region Spectral Blade
								// Spectral Blade
								if (!bOOCBuff&&!bCurrentlyAvoiding&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_SpectralBlade)&&Bot.Target.CurrentTarget!=null)
								{
									 return new Ability(SNOPower.Wizard_SpectralBlade, 14f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
								}
								#endregion
								#region Electrocute
								// Electrocute
								if (!bOOCBuff&&!bCurrentlyAvoiding&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Electrocute)&&!Bot.Character.bIsIncapacitated)
								{
									 float Range=40f;
									 if (this.RuneIndexCache[SNOPower.Wizard_Electrocute]==2)
										  Range=15f;

									 return new Ability(SNOPower.Wizard_Electrocute, Range, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, USE_SLOWLY);
								}
								#endregion
								#region Default attacks
								// Default attacks
								if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated)
								{
									 return new Ability(SNOPower.Weapon_Melee_Instant, 10f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
								}
								#endregion
						  }


						  #region Diamond Skin
						  // Diamond Skin SPAM
						  if (Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_DiamondSkin)&&Bot.Combat.powerLastSnoPowerUsed!=SNOPower.Wizard_DiamondSkin&&
							  (Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>0||Bot.Character.dCurrentHealthPct<=0.90||Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted||(!bOOCBuff&&Bot.Target.CurrentTarget.RadiusDistance<=40f))&&
							  ((Bot.SettingsFunky.Class.bEnableCriticalMass&&!bOOCBuff)||!HasBuff(SNOPower.Wizard_DiamondSkin))&&
							  PowerManager.CanCast(SNOPower.Wizard_DiamondSkin))
						  {
								return new Ability(SNOPower.Wizard_DiamondSkin, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 1, USE_SLOWLY);
						  }
						  #endregion
						  #region wizard armors
						  // The three wizard armors, done in an else-if loop so it doesn't keep replacing one with the other
						  if (!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=25)
						  {
								// Energy armor as priority cast if available and not buffed
								if (Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_EnergyArmor))
								{
									 if (!this.HasBuff(SNOPower.Wizard_EnergyArmor)&&PowerManager.CanCast(SNOPower.Wizard_EnergyArmor))
									 {
										  return new Ability(SNOPower.Wizard_EnergyArmor, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
									 }
								}
								// Ice Armor
								else if (Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_IceArmor))
								{
									 if (!this.HasBuff(SNOPower.Wizard_IceArmor)&&PowerManager.CanCast(SNOPower.Wizard_IceArmor))
									 {
										  return new Ability(SNOPower.Wizard_IceArmor, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
									 }
								}
								// Storm Armor
								else if (Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_StormArmor))
								{
									 if (!this.HasBuff(SNOPower.Wizard_StormArmor)&&PowerManager.CanCast(SNOPower.Wizard_StormArmor))
									 {
										  return new Ability(SNOPower.Wizard_StormArmor, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
									 }
								}
						  }
						  #endregion
						  #region Magic Weapon
						  // Magic Weapon
						  if (!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_MagicWeapon)&&
							  Bot.Character.dCurrentEnergy>=25&&(Bot.Class.AbilityUseTimer(SNOPower.Wizard_MagicWeapon)||!HasBuff(SNOPower.Wizard_MagicWeapon)))
						  {
								return new Ability(SNOPower.Wizard_MagicWeapon, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
						  }
						  #endregion
						  #region Familiar
						  // Familiar
						  if (!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Familiar)&&
							  Bot.Character.dCurrentEnergy>=25&&Bot.Class.AbilityUseTimer(SNOPower.Wizard_Familiar))
						  {
								return new Ability(SNOPower.Wizard_Familiar, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
						  }
						  #endregion
					 }
					 else
					 {
						  if (!bOOCBuff&&!bCurrentlyAvoiding)
						  {
								// Archon form
								#region Slow Time
								// Archon Slow Time for in combat
								if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&
									(Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>1||Bot.Character.dCurrentHealthPct<=0.7||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=35f))&&
									Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Archon_SlowTime)&&
									Bot.Class.AbilityUseTimer(SNOPower.Wizard_Archon_SlowTime, true)&&PowerManager.CanCast(SNOPower.Wizard_Archon_SlowTime))
								{
									 return new Ability(SNOPower.Wizard_Archon_SlowTime, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
								}
								#endregion
								#region Teleport
								// Archon Teleport in combat
								if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Archon_Teleport)&&
									 // Try and teleport-retreat from 1 elite or 3+ greys or a boss at 15 foot range
									(Bot.Combat.iElitesWithinRange[RANGE_15]>=1||Bot.Combat.iAnythingWithinRange[RANGE_15]>=3||(Bot.Target.CurrentTarget.IsBoss&&Bot.Target.CurrentTarget.RadiusDistance<=15f))&&
									Bot.Class.AbilityUseTimer(SNOPower.Wizard_Archon_Teleport)&&PowerManager.CanCast(SNOPower.Wizard_Archon_Teleport))
								{
									 Vector3 vNewTarget=MathEx.CalculatePointFrom(Bot.Target.CurrentTarget.Position, Bot.Character.Position, -20f);
									 return new Ability(SNOPower.Wizard_Archon_Teleport, 35f, vNewTarget, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
								}
								#endregion
								#region Arcane Blast
								// Arcane Blast
								if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Archon_ArcaneBlast)&&
									Bot.Class.AbilityUseTimer(SNOPower.Wizard_Archon_ArcaneBlast)&&PowerManager.CanCast(SNOPower.Wizard_Archon_ArcaneBlast))
								{
									 if (Bot.Combat.iAnythingWithinRange[RANGE_6]>2||Bot.Combat.iElitesWithinRange[RANGE_6]>0||(Bot.Target.CurrentTarget.ObjectIsSpecial&&Bot.Target.CurrentTarget.RadiusDistance<=8f))
									 {
										  return new Ability(SNOPower.Wizard_Archon_ArcaneBlast, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
									 }
								}
								#endregion
								#region Arcane Strike
								// Arcane Strike (Arcane Strike) Rapid Spam at close-range only
								if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Archon_ArcaneStrike)&&Bot.Combat.iAnythingWithinRange[RANGE_12]>2&&Bot.Target.CurrentTarget!=null)
								{
									 return new Ability(SNOPower.Wizard_Archon_ArcaneStrike, 11f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
								}
								#endregion
								#region Disintegrate
								// Disintegrate
								if (!bOOCBuff&&!bCurrentlyAvoiding&&Bot.Class.HotbarAbilities.Contains(SNOPower.Wizard_Archon_DisintegrationWave)&&!Bot.Character.bIsIncapacitated&&Bot.Target.CurrentTarget!=null)
								{
									 return new Ability(SNOPower.Wizard_Archon_DisintegrationWave, 49f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, SIGNATURE_SPAM);
								}
								#endregion
						  }

					 }
					 return new Ability(SNOPower.None, 0, vNullLocation, -1, -1, 0, 0, false);
				}
		  }
	 }
}