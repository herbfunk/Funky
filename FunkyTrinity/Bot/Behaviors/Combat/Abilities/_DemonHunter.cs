using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.CommonBot;


namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal static Ability DemonHunterAbility(bool bCurrentlyAvoiding=false, bool bOOCBuff=false, bool bDestructiblePower=false)
		  {

				// Pick the best destructible power available
				if (bDestructiblePower)
				{
					 SNOPower destructiblePower=Bot.Class.DestructiblePower();
					 if (destructiblePower==SNOPower.DemonHunter_Grenades)
					 {
						  return new Ability(SNOPower.DemonHunter_Grenades, 16f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
					 }

					 return new Ability(destructiblePower, 22f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
				}
				CacheUnit thisCacheUnitObj;

				if (!bOOCBuff&&Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget is CacheUnit)
					 thisCacheUnitObj=(CacheUnit)Bot.Target.CurrentTarget;
				else
					 thisCacheUnitObj=null;

				Bot.Class.iWaitingReservedAmount=70;
				#region Shadow Power
				// Shadow Power
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_ShadowPower)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dDiscipline>=14&&
					(Bot.Character.dCurrentHealthPct<=0.99d||Bot.Character.bIsRooted||Bot.Combat.iElitesWithinRange[RANGE_25]>=1||Bot.Combat.iAnythingWithinRange[RANGE_15]>=3)&&
					(AbilityUseTimer(SNOPower.DemonHunter_ShadowPower)||Bot.Character.dCurrentHealthPct<0.25d&&PowerManager.CanCast(SNOPower.DemonHunter_ShadowPower)))
				{
					 return new Ability(SNOPower.DemonHunter_ShadowPower, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Smoke Screen
				// Smoke Screen
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_SmokeScreen)
					&&(!HasBuff(SNOPower.DemonHunter_ShadowPower)||Bot.Character.bIsIncapacitated)
					&&(Bot.Character.dDiscipline>=28||(Bot.Character.dDiscipline>=14&&Bot.Combat.IsKiting))
					&&(Bot.Character.dCurrentHealthPct<=0.90||Bot.Character.bIsRooted||Bot.Combat.iElitesWithinRange[RANGE_20]>=1||Bot.Combat.iAnythingWithinRange[RANGE_15]>=3||Bot.Character.bIsIncapacitated)
					&&AbilityUseTimer(SNOPower.DemonHunter_SmokeScreen))
				{
					 return new Ability(SNOPower.DemonHunter_SmokeScreen, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Preparation
				// Preparation
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Preparation)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dDiscipline<=9&&Bot.Combat.iAnythingWithinRange[RANGE_40]>=1&&
					AbilityUseTimer(SNOPower.DemonHunter_Preparation)&&PowerManager.CanCast(SNOPower.DemonHunter_Preparation))
				{
					 return new Ability(SNOPower.DemonHunter_Preparation, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Evasive Fire
				// Evasive Fire
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_EvasiveFire)&&!Bot.Character.bIsIncapacitated&&
					(Bot.Combat.iAnythingWithinRange[RANGE_20]>=1||Bot.Target.CurrentTarget.RadiusDistance<=30f)&&
					AbilityUseTimer(SNOPower.DemonHunter_EvasiveFire)&&
					((Bot.Class.RuneIndexCache[SNOPower.DemonHunter_EvasiveFire]!=0||DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.DemonHunter_EvasiveFire]).TotalMilliseconds>2750)||Bot.Character.dCurrentEnergyPct<0.25))
				{
					 return new Ability(SNOPower.DemonHunter_EvasiveFire, 0f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Companion
				// Companion
				if (!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Companion)&&Bot.Character.PetData.DemonHunterPet==0&&
					Bot.Character.dDiscipline>=10&&AbilityUseTimer(SNOPower.DemonHunter_Companion))
				{
					 return new Ability(SNOPower.DemonHunter_Companion, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 2, 1, USE_SLOWLY);
				} 
				#endregion
				#region Sentry Turret
				// Sentry Turret
				if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Sentry)&&
					Bot.Combat.powerLastSnoPowerUsed!=SNOPower.DemonHunter_Sentry&&
					(Bot.Class.KiteDistance>0&&Bot.Combat.KitedLastTarget||DateTime.Now.Subtract(Bot.Combat.LastKiteAction).TotalMilliseconds<1000)||
					(Bot.Combat.iElitesWithinRange[RANGE_40]>=1||Bot.Combat.iAnythingWithinRange[RANGE_40]>=2)&&
					Bot.Character.dCurrentEnergy>=30&&AbilityUseTimer(SNOPower.DemonHunter_Sentry))
				{
					 return new Ability(SNOPower.DemonHunter_Sentry, 0f, ZetaDia.Me.Position, Bot.Character.iCurrentWorldID, -1, 0, 0, false);
				} 
				#endregion
				#region Marked for Death
				// Marked for Death
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_MarkedForDeath)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dDiscipline>=3&&
					(Bot.Combat.iElitesWithinRange[RANGE_40]>=1||Bot.Combat.iAnythingWithinRange[RANGE_40]>=3||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=40f))&&
					AbilityUseTimer(SNOPower.DemonHunter_MarkedForDeath))
				{
					 return new Ability(SNOPower.DemonHunter_MarkedForDeath, 40f, vNullLocation, Bot.Character.iCurrentWorldID, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Vault
				// Vault
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Vault)&&!Bot.Character.bIsRooted&&!Bot.Character.bIsIncapacitated&&
					 // Only use vault to retreat if < level 60, or if in inferno difficulty for level 60's
					(Bot.Character.iMyLevel<60||Bot.Character.iCurrentGameDifficulty==GameDifficulty.Inferno)&&
					(Bot.Target.CurrentTarget.RadiusDistance<=10f||Bot.Combat.iAnythingWithinRange[RANGE_6]>=1)&&
					Bot.Character.dDiscipline>=8&&AbilityUseTimer(SNOPower.DemonHunter_Vault)&&PowerManager.CanCast(SNOPower.DemonHunter_Vault))
				{
					 Vector3 vNewTarget=MathEx.CalculatePointFrom(Bot.Target.CurrentTarget.Position, Bot.Character.Position, -15f);
					 return new Ability(SNOPower.DemonHunter_Vault, 20f, vNewTarget, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
				} 
				#endregion
				#region Rain of Vengeance
				// Rain of Vengeance
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_RainOfVengeance)&&!Bot.Character.bIsIncapacitated&&
					(Bot.Combat.iAnythingWithinRange[RANGE_25]>=7||Bot.Combat.iElitesWithinRange[RANGE_25]>=1)&&
					AbilityUseTimer(SNOPower.DemonHunter_RainOfVengeance)&&PowerManager.CanCast(SNOPower.DemonHunter_RainOfVengeance))
				{
					 return new Ability(SNOPower.DemonHunter_RainOfVengeance, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Cluster Arrow
				// Cluster Arrow
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_ClusterArrow)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dCurrentEnergy>=50&&
					(Bot.Combat.iElitesWithinRange[RANGE_50]>=1||Bot.Combat.iAnythingWithinRange[RANGE_50]>=5||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=69f))&&
					AbilityUseTimer(SNOPower.DemonHunter_ClusterArrow))
				{
					 return new Ability(SNOPower.DemonHunter_ClusterArrow, 69f, vNullLocation, Bot.Character.iCurrentWorldID, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Multi Shot
				// Multi Shot
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Multishot)
					 &&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=30&&(thisCacheUnitObj==null||!thisCacheUnitObj.IsMissileReflecting))
				{
					 //Bot.Target.ObjectData.CanReflectMissiles||Bot.Combat.iElitesWithinRange[RANGE_50]>=1||Bot.Combat.iAnythingWithinRange[RANGE_50]>=2||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.ObjectData.IsTreasureGoblin||Bot.Target.ObjectData.IsBoss)&&Bot.Target.ObjectData.RadiusDistance<=69f))

					 //reflective target..
					 if (thisCacheUnitObj!=null&&thisCacheUnitObj.IsMissileReflecting)
						  return new Ability(SNOPower.DemonHunter_Multishot, 55f, vNullLocation, Bot.Character.iCurrentWorldID, thisCacheUnitObj.AcdGuid.Value, 1, 1, USE_SLOWLY);

					 //cluster target..
					 System.Collections.Generic.List<Cluster> clusters=ObjectCache.Objects.Clusters(7d, 45f, 2);
					 if (clusters.Count>0)
					 {

						  Vector3 pos=clusters[0].ListUnits[0].Position;
						  return new Ability(SNOPower.DemonHunter_Multishot, 55f, pos, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);

					 }
				} 
				#endregion
				#region Fan of Knives
				// Fan of Knives
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_FanOfKnives)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dCurrentEnergy>=20&&
					(Bot.Combat.iAnythingWithinRange[RANGE_15]>=4||Bot.Combat.iElitesWithinRange[RANGE_15]>=1)&&
					AbilityUseTimer(SNOPower.DemonHunter_FanOfKnives))
				{
					 return new Ability(SNOPower.DemonHunter_FanOfKnives, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Strafe
				// Strafe spam - similar to barbarian whirlwind routine
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Strafe)&&!Bot.Character.bIsIncapacitated&&!Bot.Character.bIsRooted&&
					 // Only if within 25 foot of main target
					Bot.Target.CurrentTarget.RadiusDistance<=15f&&
					 // Check for energy reservation amounts
					((Bot.Character.dCurrentEnergy>=15&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount))
				{
					 bool bGenerateNewZigZag=(DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=1500f||
						  (Bot.Combat.vPositionLastZigZagCheck!=vNullLocation&&Bot.Character.Position==Bot.Combat.vPositionLastZigZagCheck&&DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=1200)||
						  Vector3.Distance(Bot.Character.Position, Bot.Combat.vSideToSideTarget)<=6f||
						  Bot.Target.CurrentTarget.AcdGuid.Value!=Bot.Combat.iACDGUIDLastWhirlwind);
					 Bot.Combat.vPositionLastZigZagCheck=Bot.Character.Position;
					 if (bGenerateNewZigZag)
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
					 return new Ability(SNOPower.DemonHunter_Strafe, 25f, Bot.Combat.vSideToSideTarget, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
				} 
				#endregion
				#region Spike Trap
				// Spike Trap
				if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_SpikeTrap)&&
					Bot.Combat.powerLastSnoPowerUsed!=SNOPower.DemonHunter_SpikeTrap&&
					(Bot.Combat.iElitesWithinRange[RANGE_30]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>4||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=35f))&&
					Bot.Character.dCurrentEnergy>=30&&AbilityUseTimer(SNOPower.DemonHunter_SpikeTrap))
				{
					 // For distant monsters, try to target a little bit in-front of them (as they run towards us), if it's not a treasure goblin
					 float fExtraDistance=0f;
					 if (Bot.Target.CurrentTarget.CentreDistance>17f&&!Bot.Target.CurrentTarget.IsTreasureGoblin)
					 {
						  fExtraDistance=Bot.Target.CurrentTarget.CentreDistance-17f;
						  if (fExtraDistance>5f)
								fExtraDistance=5f;
						  if (Bot.Target.CurrentTarget.CentreDistance-fExtraDistance<15f)
								fExtraDistance-=2;
					 }
					 Vector3 vNewTarget=MathEx.CalculatePointFrom(Bot.Target.CurrentTarget.Position, Bot.Character.Position, Bot.Target.CurrentTarget.CentreDistance-fExtraDistance);
					 return new Ability(SNOPower.DemonHunter_SpikeTrap, 40f, vNewTarget, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Caltrops
				// Caltrops
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Caltrops)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dDiscipline>=6&&(Bot.Combat.iAnythingWithinRange[RANGE_30]>=2||Bot.Combat.iElitesWithinRange[RANGE_40]>=1)&&
					AbilityUseTimer(SNOPower.DemonHunter_Caltrops))
				{
					 return new Ability(SNOPower.DemonHunter_Caltrops, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Elemental Arrow
				// Elemental Arrow
				if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Target.CurrentTarget.IsTreasureGoblin&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_ElementalArrow)&&!Bot.Character.bIsIncapacitated&&
					((Bot.Character.dCurrentEnergy>=10&&!Bot.Character.bWaitingForReserveEnergy&&(Bot.Target.CurrentTarget.SNOID!=5208&&Bot.Target.CurrentTarget.SNOID!=5209&&
					Bot.Target.CurrentTarget.SNOID!=5210))||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount))
				{

					 // Players with grenades *AND* elemental arrow should spam grenades at close-range instead
					 if (HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Grenades)&&Bot.Target.CurrentTarget.RadiusDistance<=18f)
						  return new Ability(SNOPower.DemonHunter_Grenades, 18f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
					 // Now return elemental arrow, if not sending grenades instead
					 return new Ability(SNOPower.DemonHunter_ElementalArrow, 48f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
				} 
				#endregion
				#region Chakram
				// Chakram
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Chakram)&&!Bot.Character.bIsIncapacitated&&
					 // If we have elemental arrow or rapid fire, then use chakram as a 110 second buff, instead
					((!HotbarAbilitiesContainsPower(SNOPower.DemonHunter_ClusterArrow))||
					DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.DemonHunter_Chakram]).TotalMilliseconds>=110000)&&
					((Bot.Character.dCurrentEnergy>=10&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount))
				{
					 return new Ability(SNOPower.DemonHunter_Chakram, 69f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
				} 
				#endregion
				#region Rapid Fire
				// Rapid Fire
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_RapidFire)&&!Bot.Character.bIsIncapacitated&&
					((Bot.Character.dCurrentEnergy>=20&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount))
				{
					 // Players with grenades *AND* rapid fire should spam grenades at close-range instead
					 if (HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Grenades)&&Bot.Target.CurrentTarget.RadiusDistance<=18f)
						  return new Ability(SNOPower.DemonHunter_Grenades, 18f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, USE_SLOWLY);
					 // Now return rapid fire, if not sending grenades instead
					 return new Ability(SNOPower.DemonHunter_RapidFire, 69f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, SIGNATURE_SPAM);
				} 
				#endregion
				#region Impale
				// Impale
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Impale)&&!Bot.Character.bIsIncapacitated&&
					((Bot.Character.dCurrentEnergy>=25&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount)&&
					Bot.Target.CurrentTarget.RadiusDistance<=12f)
				{
					 return new Ability(SNOPower.DemonHunter_Impale, 12f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
				} 
				#endregion
				#region Hungering Arrow
				// Hungering Arrow
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_HungeringArrow)&&!Bot.Character.bIsIncapacitated&&(!Bot.Target.CurrentTarget.IsMissileReflecting||Bot.Character.dCurrentEnergy<10))
				{
					 return new Ability(SNOPower.DemonHunter_HungeringArrow, 48f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, USE_SLOWLY);
				} 
				#endregion
				#region Entangling shot
				// Entangling shot
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_EntanglingShot)&&!Bot.Character.bIsIncapacitated&&(!Bot.Target.CurrentTarget.IsMissileReflecting||Bot.Character.dCurrentEnergy<10))
				{
					 return new Ability(SNOPower.DemonHunter_EntanglingShot, 50f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, USE_SLOWLY);
				} 
				#endregion
				#region Bola Shot
				// Bola Shot
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_BolaShot)&&!Bot.Character.bIsIncapacitated&&(!Bot.Target.CurrentTarget.IsMissileReflecting||Bot.Character.dCurrentEnergy<10))
				{
					 return new Ability(SNOPower.DemonHunter_BolaShot, 50f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
				} 
				#endregion
				#region Grenades
				// Grenades
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Grenades)&&!Bot.Character.bIsIncapacitated)
				{
					 int acdguid=Bot.Target.CurrentTarget.AcdGuid.Value;
					 if (ObjectCache.Objects.Clusters(6d, 40f, 1, true).Count>0)
					 {
						  acdguid=ObjectCache.Objects.Clusters()[0].ListUnits[0].AcdGuid.Value;
					 }
					 return new Ability(SNOPower.DemonHunter_Grenades, 40f, vNullLocation, -1, acdguid, 0, 1, USE_SLOWLY);
				} 
				#endregion
				#region Default
				// Default attacks
				if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated)
				{
					 return new Ability(SNOPower.Weapon_Ranged_Projectile, 40f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
				} 
				#endregion
				return new Ability(SNOPower.None, 0, vNullLocation, -1, -1, 0, 0, false);

		  }
	 }
}