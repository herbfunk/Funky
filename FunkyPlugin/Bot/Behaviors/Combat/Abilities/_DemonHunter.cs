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
		  internal static cacheSNOPower DemonHunterAbility(bool bCurrentlyAvoiding=false, bool bOOCBuff=false, bool bDestructiblePower=false)
		  {
				#region DemonHunter

				// Pick the best destructible power available
				if (bDestructiblePower)
				{
					 SNOPower destructiblePower=Bot.Class.DestructiblePower();
					 if (destructiblePower==SNOPower.DemonHunter_Grenades)
					 {
						  return new cacheSNOPower(SNOPower.DemonHunter_Grenades, 12f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
					 }

					 return new cacheSNOPower(destructiblePower, 20f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
				}
				CacheUnit thisCacheUnitObj;

				if (!bOOCBuff&&Bot.Target.ObjectData!=null&&Bot.Target.ObjectData is CacheUnit)
					 thisCacheUnitObj=(CacheUnit)Bot.Target.ObjectData;
				else
					 thisCacheUnitObj=null;

				iWaitingReservedAmount=70;
				// Shadow Power
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_ShadowPower)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dDiscipline>=14&&
					(Bot.Character.dCurrentHealthPct<=0.99||Bot.Character.bIsRooted||Bot.Combat.iElitesWithinRange[RANGE_25]>=1||Bot.Combat.iAnythingWithinRange[RANGE_15]>=3)&&
					AbilityUseTimer(SNOPower.DemonHunter_ShadowPower))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_ShadowPower, 0f, vNullLocation, iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// Smoke Screen
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_SmokeScreen)
					&&(!HasBuff(SNOPower.DemonHunter_ShadowPower)||Bot.Character.bIsIncapacitated)
					&&Bot.Character.dDiscipline>=14
					&&(Bot.Character.dCurrentHealthPct<=0.90||Bot.Character.bIsRooted||Bot.Combat.iElitesWithinRange[RANGE_20]>=1||Bot.Combat.iAnythingWithinRange[RANGE_15]>=3||Bot.Character.bIsIncapacitated)
					&&AbilityUseTimer(SNOPower.DemonHunter_SmokeScreen))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_SmokeScreen, 0f, vNullLocation, iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// Preparation
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Preparation)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dDiscipline<=9&&Bot.Combat.iAnythingWithinRange[RANGE_40]>=1&&
					AbilityUseTimer(SNOPower.DemonHunter_Preparation)&&PowerManager.CanCast(SNOPower.DemonHunter_Preparation))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_Preparation, 0f, vNullLocation, iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// Evasive Fire
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_EvasiveFire)&&!Bot.Character.bIsIncapacitated&&
					(Bot.Combat.iAnythingWithinRange[RANGE_20]>=1||Bot.Target.ObjectData.RadiusDistance<=30f)&&
					AbilityUseTimer(SNOPower.DemonHunter_EvasiveFire)&&
					((Bot.Class.RuneIndexCache[SNOPower.DemonHunter_EvasiveFire]!=0||DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.DemonHunter_EvasiveFire]).TotalMilliseconds>2750)||Bot.Character.dCurrentEnergyPct<0.25))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_EvasiveFire, 0f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 1, 1, USE_SLOWLY);
				}
				// Companion
				if (!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Companion)&&Bot.Character.PetData.DemonHunterPet==0&&
					Bot.Character.dDiscipline>=10&&AbilityUseTimer(SNOPower.DemonHunter_Companion))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_Companion, 0f, vNullLocation, iCurrentWorldID, -1, 2, 1, USE_SLOWLY);
				}
				// Sentry Turret
				if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Sentry)&&
					Bot.Combat.powerLastSnoPowerUsed!=SNOPower.DemonHunter_Sentry&&
					(Bot.Class.KiteDistance>0&&Bot.Combat.KitedLastTarget||DateTime.Now.Subtract(Bot.Combat.LastKiteAction).TotalMilliseconds<1000)||
					(Bot.Combat.iElitesWithinRange[RANGE_40]>=1||Bot.Combat.iAnythingWithinRange[RANGE_40]>=2)&&
					Bot.Character.dCurrentEnergy>=30&&AbilityUseTimer(SNOPower.DemonHunter_Sentry))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_Sentry, 0f, ZetaDia.Me.Position, iCurrentWorldID, -1, 0, 0, false);
				}
				// Marked for Death
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_MarkedForDeath)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dDiscipline>=3&&
					(Bot.Combat.iElitesWithinRange[RANGE_40]>=1||Bot.Combat.iAnythingWithinRange[RANGE_40]>=3||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.ObjectData.IsTreasureGoblin||Bot.Target.ObjectData.IsBoss)&&Bot.Target.ObjectData.RadiusDistance<=40f))&&
					AbilityUseTimer(SNOPower.DemonHunter_MarkedForDeath))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_MarkedForDeath, 40f, vNullLocation, iCurrentWorldID, Bot.Target.ObjectData.AcdGuid.Value, 1, 1, USE_SLOWLY);
				}
				// Vault
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Vault)&&!Bot.Character.bIsRooted&&!Bot.Character.bIsIncapacitated&&
					 // Only use vault to retreat if < level 60, or if in inferno difficulty for level 60's
					(Bot.Character.iMyLevel<60||iCurrentGameDifficulty==GameDifficulty.Inferno)&&
					(Bot.Target.ObjectData.RadiusDistance<=10f||Bot.Combat.iAnythingWithinRange[RANGE_6]>=1)&&
					Bot.Character.dDiscipline>=8&&AbilityUseTimer(SNOPower.DemonHunter_Vault)&&PowerManager.CanCast(SNOPower.DemonHunter_Vault))
				{
					 Vector3 vNewTarget=MathEx.CalculatePointFrom(Bot.Target.ObjectData.Position, Bot.Character.Position, -15f);
					 return new cacheSNOPower(SNOPower.DemonHunter_Vault, 20f, vNewTarget, iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
				}
				// Rain of Vengeance
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_RainOfVengeance)&&!Bot.Character.bIsIncapacitated&&
					(Bot.Combat.iAnythingWithinRange[RANGE_25]>=7||Bot.Combat.iElitesWithinRange[RANGE_25]>=1)&&
					AbilityUseTimer(SNOPower.DemonHunter_RainOfVengeance)&&PowerManager.CanCast(SNOPower.DemonHunter_RainOfVengeance))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_RainOfVengeance, 0f, vNullLocation, iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// Cluster Arrow
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_ClusterArrow)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dCurrentEnergy>=50&&
					(Bot.Combat.iElitesWithinRange[RANGE_50]>=1||Bot.Combat.iAnythingWithinRange[RANGE_50]>=5||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.ObjectData.IsTreasureGoblin||Bot.Target.ObjectData.IsBoss)&&Bot.Target.ObjectData.RadiusDistance<=69f))&&
					AbilityUseTimer(SNOPower.DemonHunter_ClusterArrow))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_ClusterArrow, 69f, vNullLocation, iCurrentWorldID, Bot.Target.ObjectData.AcdGuid.Value, 1, 1, USE_SLOWLY);
				}
				// Multi Shot
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Multishot)
					 &&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=30)
				{
					 //Bot.Target.ObjectData.CanReflectMissiles||Bot.Combat.iElitesWithinRange[RANGE_50]>=1||Bot.Combat.iAnythingWithinRange[RANGE_50]>=2||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.ObjectData.IsTreasureGoblin||Bot.Target.ObjectData.IsBoss)&&Bot.Target.ObjectData.RadiusDistance<=69f))

					 //reflective target..
					 if (thisCacheUnitObj!=null&&thisCacheUnitObj.IsMissileReflecting)
						  return new cacheSNOPower(SNOPower.DemonHunter_Multishot, 55f, vNullLocation, iCurrentWorldID, thisCacheUnitObj.AcdGuid.Value, 1, 1, USE_SLOWLY);

					 //cluster target..
					 if (ObjectCache.Objects.Clusters(8d, 45f, 2).Count>0)
						  return new cacheSNOPower(SNOPower.DemonHunter_Multishot, 55f, ObjectCache.Objects.Clusters(MinUnitCount: 2)[0].CurrentValidUnit.Position, iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
					 
				}
				// Fan of Knives
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_FanOfKnives)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dCurrentEnergy>=20&&
					(Bot.Combat.iAnythingWithinRange[RANGE_15]>=4||Bot.Combat.iElitesWithinRange[RANGE_15]>=1)&&
					AbilityUseTimer(SNOPower.DemonHunter_FanOfKnives))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_FanOfKnives, 0f, vNullLocation, iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// Strafe spam - similar to barbarian whirlwind routine
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Strafe)&&!Bot.Character.bIsIncapacitated&&!Bot.Character.bIsRooted&&
					 // Only if within 25 foot of main target
					Bot.Target.ObjectData.RadiusDistance<=15f&&
					 // Check for energy reservation amounts
					((Bot.Character.dCurrentEnergy>=15&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=iWaitingReservedAmount))
				{
					 bool bGenerateNewZigZag=(DateTime.Now.Subtract(lastChangedZigZag).TotalMilliseconds>=1500f||
						  (vPositionLastZigZagCheck!=vNullLocation&&Bot.Character.Position==vPositionLastZigZagCheck&&DateTime.Now.Subtract(lastChangedZigZag).TotalMilliseconds>=1200)||
						  Vector3.Distance(Bot.Character.Position, vSideToSideTarget)<=6f||
						  Bot.Target.ObjectData.AcdGuid.Value!=iACDGUIDLastWhirlwind);
					 vPositionLastZigZagCheck=Bot.Character.Position;
					 if (bGenerateNewZigZag)
					 {
						  if (bCheckGround)
								vSideToSideTarget=FindZigZagTargetLocation(Bot.Target.ObjectData.Position, 25f, false, true, true);
						  else if (Bot.Combat.iAnythingWithinRange[RANGE_30]>=6||Bot.Combat.iElitesWithinRange[RANGE_30]>=3)
								vSideToSideTarget=FindZigZagTargetLocation(Bot.Target.ObjectData.Position, 25f, false, true);
						  else
								vSideToSideTarget=FindZigZagTargetLocation(Bot.Target.ObjectData.Position, 25f);
						  Bot.Combat.powerLastSnoPowerUsed=SNOPower.None;
						  iACDGUIDLastWhirlwind=Bot.Target.ObjectData.AcdGuid.Value;
						  lastChangedZigZag=DateTime.Now;
					 }
					 return new cacheSNOPower(SNOPower.DemonHunter_Strafe, 25f, vSideToSideTarget, iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
				}
				// Spike Trap
				if (!bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_SpikeTrap)&&
					Bot.Combat.powerLastSnoPowerUsed!=SNOPower.DemonHunter_SpikeTrap&&
					(Bot.Combat.iElitesWithinRange[RANGE_30]>=1||Bot.Combat.iAnythingWithinRange[RANGE_25]>4||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.ObjectData.IsTreasureGoblin||Bot.Target.ObjectData.IsBoss)&&Bot.Target.ObjectData.RadiusDistance<=35f))&&
					Bot.Character.dCurrentEnergy>=30&&AbilityUseTimer(SNOPower.DemonHunter_SpikeTrap))
				{
					 // For distant monsters, try to target a little bit in-front of them (as they run towards us), if it's not a treasure goblin
					 float fExtraDistance=0f;
					 if (Bot.Target.ObjectData.CentreDistance>17f&&!Bot.Target.ObjectData.IsTreasureGoblin)
					 {
						  fExtraDistance=Bot.Target.ObjectData.CentreDistance-17f;
						  if (fExtraDistance>5f)
								fExtraDistance=5f;
						  if (Bot.Target.ObjectData.CentreDistance-fExtraDistance<15f)
								fExtraDistance-=2;
					 }
					 Vector3 vNewTarget=MathEx.CalculatePointFrom(Bot.Target.ObjectData.Position, Bot.Character.Position, Bot.Target.ObjectData.CentreDistance-fExtraDistance);
					 return new cacheSNOPower(SNOPower.DemonHunter_SpikeTrap, 40f, vNewTarget, iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// Caltrops
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Caltrops)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dDiscipline>=6&&(Bot.Combat.iAnythingWithinRange[RANGE_30]>=2||Bot.Combat.iElitesWithinRange[RANGE_40]>=1)&&
					AbilityUseTimer(SNOPower.DemonHunter_Caltrops))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_Caltrops, 0f, vNullLocation, iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// Elemental Arrow
				if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Target.ObjectData.IsTreasureGoblin&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_ElementalArrow)&&!Bot.Character.bIsIncapacitated&&
					((Bot.Character.dCurrentEnergy>=10&&!Bot.Character.bWaitingForReserveEnergy&&(Bot.Target.ObjectData.SNOID!=5208&&Bot.Target.ObjectData.SNOID!=5209&&
					Bot.Target.ObjectData.SNOID!=5210))||Bot.Character.dCurrentEnergy>=iWaitingReservedAmount))
				{

					 // Players with grenades *AND* elemental arrow should spam grenades at close-range instead
					 if (HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Grenades)&&Bot.Target.ObjectData.RadiusDistance<=18f)
						  return new cacheSNOPower(SNOPower.DemonHunter_Grenades, 18f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 0, 1, USE_SLOWLY);
					 // Now return elemental arrow, if not sending grenades instead
					 return new cacheSNOPower(SNOPower.DemonHunter_ElementalArrow, 48f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 0, 1, USE_SLOWLY);
				}
				// Chakram
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Chakram)&&!Bot.Character.bIsIncapacitated&&
					 // If we have elemental arrow or rapid fire, then use chakram as a 110 second buff, instead
					((!HotbarAbilitiesContainsPower(SNOPower.DemonHunter_ClusterArrow))||
					DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.DemonHunter_Chakram]).TotalMilliseconds>=110000)&&
					((Bot.Character.dCurrentEnergy>=10&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=iWaitingReservedAmount))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_Chakram, 69f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 0, 1, USE_SLOWLY);
				}
				// Rapid Fire
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_RapidFire)&&!Bot.Character.bIsIncapacitated&&
					((Bot.Character.dCurrentEnergy>=20&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=iWaitingReservedAmount))
				{
					 // Players with grenades *AND* rapid fire should spam grenades at close-range instead
					 if (HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Grenades)&&Bot.Target.ObjectData.RadiusDistance<=18f)
						  return new cacheSNOPower(SNOPower.DemonHunter_Grenades, 18f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 0, 0, USE_SLOWLY);
					 // Now return rapid fire, if not sending grenades instead
					 return new cacheSNOPower(SNOPower.DemonHunter_RapidFire, 69f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 0, 0, SIGNATURE_SPAM);
				}
				// Impale
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Impale)&&!Bot.Character.bIsIncapacitated&&
					((Bot.Character.dCurrentEnergy>=25&&!Bot.Character.bWaitingForReserveEnergy)||Bot.Character.dCurrentEnergy>=iWaitingReservedAmount)&&
					Bot.Target.ObjectData.RadiusDistance<=12f)
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_Impale, 12f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 0, 1, USE_SLOWLY);
				}
				// Hungering Arrow
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_HungeringArrow)&&!Bot.Character.bIsIncapacitated)
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_HungeringArrow, 48f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 0, 0, USE_SLOWLY);
				}
				// Entangling shot
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_EntanglingShot)&&!Bot.Character.bIsIncapacitated&&(!Bot.Target.ObjectData.IsMissileReflecting||Bot.Character.dCurrentEnergy<30))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_EntanglingShot, 50f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 0, 0, USE_SLOWLY);
				}
				// Bola Shot
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_BolaShot)&&!Bot.Character.bIsIncapacitated&&(!Bot.Target.ObjectData.IsMissileReflecting||Bot.Character.dCurrentEnergy<30))
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_BolaShot, 50f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 0, 1, USE_SLOWLY);
				}
				// Grenades
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Grenades)&&!Bot.Character.bIsIncapacitated)
				{
					 return new cacheSNOPower(SNOPower.DemonHunter_Grenades, 40f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 0, 1, USE_SLOWLY);
				}
				// Default attacks
				if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated)
				{
					 return new cacheSNOPower(SNOPower.Weapon_Ranged_Projectile, 40f, vNullLocation, -1, Bot.Target.ObjectData.AcdGuid.Value, 1, 1, USE_SLOWLY);
				}
				return new cacheSNOPower(SNOPower.None, 0, vNullLocation, -1, -1, 0, 0, false);
				#endregion
		  }
	 }
}