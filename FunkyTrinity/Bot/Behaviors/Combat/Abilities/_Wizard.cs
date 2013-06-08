using System;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.CommonBot;


namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal static Ability WizardAbility(bool bCurrentlyAvoiding=false, bool bOOCBuff=false, bool bDestructiblePower=false)
		  {

				// Pick the best destructible power available
				if (bDestructiblePower)
				{
					 if (!HasBuff(SNOPower.Wizard_Archon))
					 {
						  SNOPower destructiblePower=Bot.Class.DestructiblePower();
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
				CacheUnit thisCacheUnitObj;

				if (!bOOCBuff&&Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.targetType.Value==TargetType.Unit)
					 thisCacheUnitObj=(CacheUnit)Bot.Target.CurrentTarget;
				else
					 thisCacheUnitObj=null;

				// Wizards want to save up to a reserve of 65+ energy
				Bot.Class.iWaitingReservedAmount=65;
				if (!HasBuff(SNOPower.Wizard_Archon))
				{
					 #region Slow time
					 // Slow time, for if being followed
					 if (bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_SlowTime)&&
						 AbilityUseTimer(SNOPower.Wizard_SlowTime, true)&&PowerManager.CanCast(SNOPower.Wizard_SlowTime))
					 {
						  return new Ability(SNOPower.Wizard_SlowTime, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
					 }
					 #endregion
					 #region Slow time
					 // Slow Time for in combat
					 if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_SlowTime)&&
						 (Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>1||Bot.Character.dCurrentHealthPct<=0.7||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=35f))&&
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
						 (SettingsFunky.Class.bEnableCriticalMass&&((!HotbarAbilitiesContainsPower(SNOPower.Wizard_FrostNova)&&!HotbarAbilitiesContainsPower(SNOPower.Wizard_ExplosiveBlast))||
							 (Bot.Character.dCurrentHealthPct<=0.7&&(Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>0||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=23f)))))
						  // Else normal wizard in which case check standard stuff
						 ||(!SettingsFunky.Class.bEnableCriticalMass&&Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>3||Bot.Character.dCurrentHealthPct<=0.7||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=23f))
						 )&&
						 HotbarAbilitiesContainsPower(SNOPower.Wizard_WaveOfForce)&&
						 AbilityUseTimer(SNOPower.Wizard_WaveOfForce, true)&&PowerManager.CanCast(SNOPower.Wizard_WaveOfForce))
					 {
						  return new Ability(SNOPower.Wizard_WaveOfForce, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
					 }
					 #endregion
					 #region Blizzard
					 // Blizzard
					 if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_Blizzard)&&
						 Bot.Combat.powerLastSnoPowerUsed!=SNOPower.Wizard_Blizzard&&
						 (Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>2||thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Target.CurrentTarget.IsBoss||Bot.Character.dCurrentHealthPct<=0.7)&&
						 Bot.Character.dCurrentEnergy>=40&&AbilityUseTimer(SNOPower.Wizard_Blizzard))
					 {
						  return new Ability(SNOPower.Wizard_Blizzard, 40f, new Vector3(Bot.Target.CurrentTarget.Position.X, Bot.Target.CurrentTarget.Position.Y, Bot.Target.CurrentTarget.Position.Z), Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
					 }
					 #endregion
					 #region Meteor
					 // Meteor
					 if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_Meteor)&&
						  !Bot.Target.CurrentTarget.IsTreasureGoblin&&
						  Bot.Character.dCurrentEnergy>=50&&PowerManager.CanCast(SNOPower.Wizard_Meteor))
					 {
						  if (ObjectCache.Objects.Clusters(4d, 45f, 3, true).Count>0)
						  {
								Vector3 Location=ObjectCache.Objects.Clusters()[0].ListUnits[0].Position;
								return new Ability(SNOPower.Wizard_Meteor, 45f, Location, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
						  }
						  
					 }
					 #endregion
					 #region Teleport
					 // Teleport in combat for critical-mass wizards
					 if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_Teleport)&&SettingsFunky.Class.bEnableCriticalMass&&
						 Bot.Combat.powerLastSnoPowerUsed!=SNOPower.Wizard_Teleport&&
						 Bot.Character.dCurrentEnergy>=15&&Bot.Target.CurrentTarget.CentreDistance<=35f&&
						 PowerManager.CanCast(SNOPower.Wizard_Teleport))
					 {
						  Bot.Combat.vSideToSideTarget=FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, Bot.Target.CurrentTarget.CentreDistance, true);
						  return new Ability(SNOPower.Wizard_Teleport, 35f, Bot.Combat.vSideToSideTarget, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
					 }
					 #endregion
					 #region Diamond Skin
					 // Diamond Skin SPAM
					 if (HotbarAbilitiesContainsPower(SNOPower.Wizard_DiamondSkin)&&Bot.Combat.powerLastSnoPowerUsed!=SNOPower.Wizard_DiamondSkin&&
						 (Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>0||Bot.Character.dCurrentHealthPct<=0.90||Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted||(!bOOCBuff&&Bot.Target.CurrentTarget.RadiusDistance<=40f))&&
						 ((SettingsFunky.Class.bEnableCriticalMass&&!bOOCBuff)||!HasBuff(SNOPower.Wizard_DiamondSkin))&&
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
						  if (HotbarAbilitiesContainsPower(SNOPower.Wizard_EnergyArmor))
						  {
								if (!HasBuff(SNOPower.Wizard_EnergyArmor)&&PowerManager.CanCast(SNOPower.Wizard_EnergyArmor))
								{
									 return new Ability(SNOPower.Wizard_EnergyArmor, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
								}
						  }
						  // Ice Armor
						  else if (HotbarAbilitiesContainsPower(SNOPower.Wizard_IceArmor))
						  {
								if (!HasBuff(SNOPower.Wizard_IceArmor)&&PowerManager.CanCast(SNOPower.Wizard_IceArmor))
								{
									 return new Ability(SNOPower.Wizard_IceArmor, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
								}
						  }
						  // Storm Armor
						  else if (HotbarAbilitiesContainsPower(SNOPower.Wizard_StormArmor))
						  {
								if (!HasBuff(SNOPower.Wizard_StormArmor)&&PowerManager.CanCast(SNOPower.Wizard_StormArmor))
								{
									 return new Ability(SNOPower.Wizard_StormArmor, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
								}
						  }
					 }
					 #endregion
					 #region Magic Weapon
					 // Magic Weapon
					 if (!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_MagicWeapon)&&
						 Bot.Character.dCurrentEnergy>=25&&(AbilityUseTimer(SNOPower.Wizard_MagicWeapon)||!HasBuff(SNOPower.Wizard_MagicWeapon)))
					 {
						  return new Ability(SNOPower.Wizard_MagicWeapon, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
					 }
					 #endregion
					 #region Familiar
					 // Familiar
					 if (!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_Familiar)&&
						 Bot.Character.dCurrentEnergy>=25&&AbilityUseTimer(SNOPower.Wizard_Familiar))
					 {
						  return new Ability(SNOPower.Wizard_Familiar, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
					 }
					 #endregion
					 #region Hydra
					 // Hydra
					 if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&
						 HotbarAbilitiesContainsPower(SNOPower.Wizard_Hydra)&& //(Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>4||Bot.Character.dCurrentHealthPct<=0.7||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss||Bot.Target.CurrentTarget.IsTreasureGoblin)&&Bot.Target.CurrentTarget.RadiusDistance<=15f))&&
						 Bot.Character.PetData.WizardHydra==0&&
						 Bot.Character.dCurrentEnergy>=15&&AbilityUseTimer(SNOPower.Wizard_Hydra)
						 &&!Bot.Target.CurrentTarget.IsTreasureGoblin)
					 {
						  if (ObjectCache.Objects.Clusters(10d, 45f, 3, true).Count>0)
						  {
								Vector3 Location=ObjectCache.Objects.Clusters()[0].ListUnits[0].Position;
								return new Ability(SNOPower.Wizard_Hydra, 45f, Location, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
						  }
					 }
					 #endregion
					 #region Mirror Image
					 // Mirror Image  @ half health or 5+ monsters or rooted/incapacitated or last elite left @25% health
					 if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Wizard_MirrorImage)&&
						 (Bot.Character.dCurrentHealthPct<=0.50||Bot.Combat.iAnythingWithinRange[RANGE_30]>=5||Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted||(thisCacheUnitObj!=null&&Bot.Combat.iElitesWithinRange[RANGE_30]==1&&thisCacheUnitObj.IsEliteRareUnique&&!Bot.Target.CurrentTarget.IsBoss&&thisCacheUnitObj.CurrentHealthPct<=0.35))&&
						 PowerManager.CanCast(SNOPower.Wizard_MirrorImage))
					 {
						  return new Ability(SNOPower.Wizard_MirrorImage, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
					 }
					 #endregion
					 #region Archon
					 // Archon
					 if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Wizard_Archon)&&
						 (Bot.Combat.iElitesWithinRange[RANGE_30]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>=1||Bot.Character.dCurrentHealthPct<=0.6||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=30f))&&
						 Bot.Character.dCurrentEnergy>=25&&Bot.Character.dCurrentHealthPct>=0.10&&
						 PowerManager.CanCast(SNOPower.Wizard_Archon))
					 {
						  // Familiar has been removed for now. Uncomment the three comments below relating to familiars to force re-buffing them
						  bool bHasBuffAbilities=(HotbarAbilitiesContainsPower(SNOPower.Wizard_MagicWeapon)|| //hashPowerHotbarAbilities.Contains(SNOPower.Wizard_Familiar) ||
							  HotbarAbilitiesContainsPower(SNOPower.Wizard_EnergyArmor)||HotbarAbilitiesContainsPower(SNOPower.Wizard_IceArmor)||
							  HotbarAbilitiesContainsPower(SNOPower.Wizard_StormArmor));
						  int iExtraEnergyNeeded=25;
						  if (HotbarAbilitiesContainsPower(SNOPower.Wizard_MagicWeapon)) iExtraEnergyNeeded+=25;
						  //if (hashPowerHotbarAbilities.Contains(SNOPower.Wizard_Familiar)) iExtraEnergyNeeded += 25;
						  if (HotbarAbilitiesContainsPower(SNOPower.Wizard_EnergyArmor)||HotbarAbilitiesContainsPower(SNOPower.Wizard_IceArmor)||
							  HotbarAbilitiesContainsPower(SNOPower.Wizard_StormArmor)) iExtraEnergyNeeded+=25;
						  if (!bHasBuffAbilities||Bot.Character.dCurrentEnergy<=iExtraEnergyNeeded)
								Bot.Class.bWaitingForSpecial=true;
						  if (!Bot.Class.bWaitingForSpecial)
						  {
								dictAbilityLastUse[SNOPower.Wizard_MagicWeapon]=DateTime.Today;
								//dictAbilityLastUse[SNOPower.Wizard_Familiar] = DateTime.Today;
								dictAbilityLastUse[SNOPower.Wizard_EnergyArmor]=DateTime.Today;
								dictAbilityLastUse[SNOPower.Wizard_IceArmor]=DateTime.Today;
								dictAbilityLastUse[SNOPower.Wizard_StormArmor]=DateTime.Today;
								Bot.Class.bWaitingForSpecial=true;
						  }
						  else
						  {
								Bot.Class.bWaitingForSpecial=false;
								return new Ability(SNOPower.Wizard_Archon, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 4, 5, USE_SLOWLY);
						  }
					 }
					 #endregion
					 #region Frost Nova
					 // Frost Nova SPAM
					 if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Wizard_FrostNova)&&!Bot.Character.bIsIncapacitated&&
						 ((Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>0||Bot.Character.dCurrentHealthPct<=0.7)&&Bot.Target.CurrentTarget.RadiusDistance<=12f)&&
						 PowerManager.CanCast(SNOPower.Wizard_FrostNova))
					 {
						  float fThisRange=14f;
						  if (SettingsFunky.Class.bEnableCriticalMass)
								fThisRange=9f;
						  return new Ability(SNOPower.Wizard_FrostNova, fThisRange, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
					 }
					 #endregion
					 #region Explosive Blast
					 // Explosive Blast SPAM when enough AP, blow erry thing up, nah mean
					 if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Wizard_ExplosiveBlast)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=20&&
						 ((Bot.Combat.iElitesWithinRange[RANGE_25]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>=1||Bot.Character.dCurrentHealthPct<=0.7)&&Bot.Target.CurrentTarget.RadiusDistance<=12f)&&
						 PowerManager.CanCast(SNOPower.Wizard_ExplosiveBlast))
					 {
						  float fThisRange=11f;
						  if (SettingsFunky.Class.bEnableCriticalMass)
								fThisRange=9f;
						  return new Ability(SNOPower.Wizard_ExplosiveBlast, fThisRange, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
					 }
					 #endregion
					 // Check to see if we have a signature spell on our hotbar, for energy twister check
					 bool bHasSignatureSpell=(HotbarAbilitiesContainsPower(SNOPower.Wizard_MagicMissile)||HotbarAbilitiesContainsPower(SNOPower.Wizard_ShockPulse)||
						 HotbarAbilitiesContainsPower(SNOPower.Wizard_SpectralBlade)||HotbarAbilitiesContainsPower(SNOPower.Wizard_Electrocute));
					 #region Energy Twister
					 // Energy Twister SPAMS whenever 35 or more ap to generate Arcane Power
					 if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_EnergyTwister)&&
						  // If using storm chaser, then force a signature spell every 1 stack of the buff, if we have a signature spell
						 (!bHasSignatureSpell||GetBuffStacks(SNOPower.Wizard_EnergyTwister)<1)&&
						 (Bot.Combat.iElitesWithinRange[RANGE_30]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>=1||Bot.Target.CurrentTarget.RadiusDistance<=12f)&&
						 (!HotbarAbilitiesContainsPower(SNOPower.Wizard_Electrocute)||!SnoCacheLookup.hashActorSNOFastMobs.Contains(Bot.Target.CurrentTarget.SNOID))&&
						 ((SettingsFunky.Class.bEnableCriticalMass&&(!bHasSignatureSpell||Bot.Character.dCurrentEnergy>=35))||(!SettingsFunky.Class.bEnableCriticalMass&&Bot.Character.dCurrentEnergy>=35)))
					 {
						  float fThisRange=28f;
						  if (SettingsFunky.Class.bEnableCriticalMass)
								fThisRange=9f;
						  return new Ability(SNOPower.Wizard_EnergyTwister, fThisRange, new Vector3(Bot.Target.CurrentTarget.Position.X, Bot.Target.CurrentTarget.Position.Y, Bot.Target.CurrentTarget.Position.Z), Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
					 }
					 #endregion
					 #region Disintegrate
					 // Disintegrate
					 if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_Disintegrate)&&thisCacheUnitObj!=null&&
						 ((Bot.Character.dCurrentEnergy>=20&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount))
					 {
						  float fThisRange=35f;
						  if (SettingsFunky.Class.bEnableCriticalMass)
								fThisRange=20f;
						  return new Ability(SNOPower.Wizard_Disintegrate, fThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, SIGNATURE_SPAM);
					 }
					 #endregion
					 #region Arcane Orb
					 // Arcane Orb
					 if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_ArcaneOrb)&&
						 ((Bot.Character.dCurrentEnergy>=35&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount)&&
						 AbilityUseTimer(SNOPower.Wizard_ArcaneOrb))
					 {
						  float fThisRange=40f;
						  if (SettingsFunky.Class.bEnableCriticalMass)
								fThisRange=20f;

						  if (ObjectCache.Objects.Clusters(4d, fThisRange, 2, true).Count>0)
						  {
								int ACD=ObjectCache.Objects.Clusters()[0].ListUnits[0].AcdGuid.Value;
								return new Ability(SNOPower.Wizard_ArcaneOrb, fThisRange, vNullLocation, -1, ACD, 1, 1, USE_SLOWLY);
						  }
					 }
					 #endregion
					 #region Arcane Torrent
					 // Arcane Torrent
					 if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_ArcaneTorrent)&&thisCacheUnitObj!=null&&
						 ((Bot.Character.dCurrentEnergy>=16&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount)&&
						 AbilityUseTimer(SNOPower.Wizard_ArcaneTorrent))
					 {
						  float fThisRange=40f;
						  /*if (settings.bEnableCriticalMass)
							  fThisRange = 20f;*/
						  return new Ability(SNOPower.Wizard_ArcaneTorrent, fThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, USE_SLOWLY);
					 }
					 #endregion
					 #region Ray of Frost
					 // Ray of Frost
					 if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_RayOfFrost)&&thisCacheUnitObj!=null&&
						 Bot.Character.dCurrentEnergy>=12)
					 {
						  float fThisRange=35f;
						  if (SettingsFunky.Class.bEnableCriticalMass)
								fThisRange=20f;
						  return new Ability(SNOPower.Wizard_RayOfFrost, fThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, SIGNATURE_SPAM);
					 }
					 #endregion
					 #region Magic Missile
					 // Magic Missile
					 if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Wizard_MagicMissile)&&Bot.Target.CurrentTarget!=null&&(!Bot.Target.CurrentTarget.IsMissileReflecting||Bot.Character.dCurrentEnergy<30))
					 {
						  float fThisRange=35f;
						  if (SettingsFunky.Class.bEnableCriticalMass)
								fThisRange=20f;
						  return new Ability(SNOPower.Wizard_MagicMissile, fThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, USE_SLOWLY);
					 }
					 #endregion
					 #region Shock Pulse
					 // Shock Pulse
					 if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Wizard_ShockPulse)&&Bot.Target.CurrentTarget!=null)
					 {
						  return new Ability(SNOPower.Wizard_ShockPulse, 15f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
					 }
					 #endregion
					 #region Spectral Blade
					 // Spectral Blade
					 if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Wizard_SpectralBlade)&&Bot.Target.CurrentTarget!=null)
					 {
						  return new Ability(SNOPower.Wizard_SpectralBlade, 14f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
					 }
					 #endregion
					 #region Electrocute
					 // Electrocute
					 if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Wizard_Electrocute)&&Bot.Target.CurrentTarget!=null)
					 {
						  return new Ability(SNOPower.Wizard_Electrocute, 18f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, USE_SLOWLY);
					 }
					 #endregion
					 #region Default attacks
					 // Default attacks
					 if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&Bot.Target.CurrentTarget!=null)
					 {
						  return new Ability(SNOPower.Weapon_Melee_Instant, 10f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
					 }
					 #endregion
				}
				else
				{
					 // Archon form
					 #region Slow Time
					 // Archon Slow Time for in combat
					 if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&
						 (Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_25]>1||Bot.Character.dCurrentHealthPct<=0.7||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=35f))&&
						 HotbarAbilitiesContainsPower(SNOPower.Wizard_Archon_SlowTime)&&
						 AbilityUseTimer(SNOPower.Wizard_Archon_SlowTime, true)&&PowerManager.CanCast(SNOPower.Wizard_Archon_SlowTime))
					 {
						  return new Ability(SNOPower.Wizard_Archon_SlowTime, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
					 }
					 #endregion
					 #region Teleport
					 // Archon Teleport in combat
					 if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Wizard_Archon_Teleport)&&
						  // Try and teleport-retreat from 1 elite or 3+ greys or a boss at 15 foot range
						 (Bot.Combat.iElitesWithinRange[RANGE_15]>=1||Bot.Combat.iAnythingWithinRange[RANGE_15]>=3||(Bot.Target.CurrentTarget.IsBoss&&Bot.Target.CurrentTarget.RadiusDistance<=15f))&&
						 AbilityUseTimer(SNOPower.Wizard_Archon_Teleport)&&PowerManager.CanCast(SNOPower.Wizard_Archon_Teleport))
					 {
						  Vector3 vNewTarget=MathEx.CalculatePointFrom(Bot.Target.CurrentTarget.Position, Bot.Character.Position, -20f);
						  return new Ability(SNOPower.Wizard_Archon_Teleport, 35f, vNewTarget, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
					 }
					 #endregion
					 #region Arcane Blast
					 // Arcane Blast
					 if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&
						 (Bot.Combat.iElitesWithinRange[RANGE_15]>=1||Bot.Combat.iAnythingWithinRange[RANGE_15]>=1||
						  ((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=15f))&&
						 AbilityUseTimer(SNOPower.Wizard_Archon_ArcaneBlast)&&PowerManager.CanCast(SNOPower.Wizard_Archon_ArcaneBlast))
					 {
						  return new Ability(SNOPower.Wizard_Archon_ArcaneBlast, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
					 }
					 #endregion
					 #region Arcane Strike
					 // Arcane Strike (Arcane Strike) Rapid Spam at close-range only
					 if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&Bot.Target.CurrentTarget.RadiusDistance<=13f&&
						 (thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss))
					 {
						  return new Ability(SNOPower.Wizard_Archon_ArcaneStrike, 11f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
					 }
					 #endregion
					 #region Disintegrate
					 // Disintegrate
					 if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&thisCacheUnitObj!=null)
					 {
						  return new Ability(SNOPower.Wizard_Archon_DisintegrationWave, 49f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, SIGNATURE_SPAM);
					 }
					 #endregion

				}
				return new Ability(SNOPower.None, 0, vNullLocation, -1, -1, 0, 0, false);

		  }
	 }
}