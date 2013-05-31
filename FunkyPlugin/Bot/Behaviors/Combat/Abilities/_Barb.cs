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
		  internal static Ability BarbAbility(bool bCurrentlyAvoiding=false, bool bOOCBuff=false, bool bDestructiblePower=false)
		  {

				#region Barb
				// Pick the best destructible power available
				#region DestructiblePower
				if (bDestructiblePower)
				{
					 SNOPower destructiblePower=Bot.Class.DestructiblePower();
					 if (destructiblePower==SNOPower.Barbarian_WeaponThrow)
					 {
						  if (Bot.Character.dCurrentEnergy>=20)
						  {
								return new Ability(SNOPower.Barbarian_WeaponThrow, 15f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
						  }
						  else
						  {
								return new Ability(SNOPower.Weapon_Melee_Instant, 10f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
						  }
					 }

					 return new Ability(destructiblePower, 10f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
				}
				#endregion

				// Barbarians need 56 reserve for special spam like WW
				Bot.Class.iWaitingReservedAmount=56;
				// Ignore Pain when low on health
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_IgnorePain)&&Bot.Character.dCurrentHealthPct<=0.45&&
					AbilityUseTimer(SNOPower.Barbarian_IgnorePain, true)&&PowerManager.CanCast(SNOPower.Barbarian_IgnorePain))
				{
					 return new Ability(SNOPower.Barbarian_IgnorePain, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
				}

				CacheUnit thisCacheUnitObj;

				if (!bOOCBuff&&Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.targetType.Value==TargetType.Unit)
					 thisCacheUnitObj=(CacheUnit)Bot.Target.CurrentTarget;
				else
					 thisCacheUnitObj=null;

				// Flag up a variable to see if we should reserve 50 fury for special abilities
				Bot.Class.bWaitingForSpecial=false;
				//Only check if we are not already 100%
				if (Bot.Character.dCurrentEnergyPct<1)
				{
					 if (thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique&&thisCacheUnitObj.MonsterShielding&&!thisCacheUnitObj.IsAttackable.Value)
						  Bot.Class.bWaitingForSpecial=true;

					 if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Earthquake)&&
						 Bot.Combat.iElitesWithinRange[RANGE_25]>=1&&AbilityUseTimer(SNOPower.Barbarian_Earthquake))
					 {
						  Bot.Class.bWaitingForSpecial=true;
					 }
					 if (!bOOCBuff&&!bCurrentlyAvoiding
						  &&HotbarAbilitiesContainsPower(SNOPower.Barbarian_WrathOfTheBerserker)
						  &&AbilityUseTimer(SNOPower.Barbarian_WrathOfTheBerserker)
						  &&Bot.Combat.iElitesWithinRange[RANGE_50]>2)
					 {
						  Bot.Class.bWaitingForSpecial=true;
					 }
					 if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_CallOfTheAncients)&&
						 Bot.Combat.iElitesWithinRange[RANGE_15]>=3&&AbilityUseTimer(SNOPower.Barbarian_CallOfTheAncients))
					 {
						  Bot.Class.bWaitingForSpecial=true;
					 }
				}
				// Earthquake, elites close-range only
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Earthquake)&&!Bot.Character.bIsIncapacitated&&
					(Bot.Combat.iElitesWithinRange[RANGE_15]>0||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=13f))&&
					AbilityUseTimer(SNOPower.Barbarian_Earthquake, true)&&
					PowerManager.CanCast(SNOPower.Barbarian_Earthquake))
				{
					 if (Bot.Character.dCurrentEnergy>=50)
						  return new Ability(SNOPower.Barbarian_Earthquake, 13f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 4, 4, USE_SLOWLY);
					 Bot.Class.bWaitingForSpecial=true;
				}

				// Wrath of the berserker, elites only (wrath of berserker)
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_WrathOfTheBerserker)&&
					 // Not on heart of sin after Cydaea
					Bot.Target.CurrentTarget.SNOID!=193077&&
					 // Make sure we are allowed to use wrath on goblins, else make sure this isn't a goblin
					(SettingsFunky.Class.bGoblinWrath||!Bot.Target.CurrentTarget.IsTreasureGoblin)&&
					 // If on a boss, only when injured
					((Bot.Target.CurrentTarget.IsBoss&&thisCacheUnitObj!=null&&thisCacheUnitObj.CurrentHealthPct.Value<=0.99&&!HotbarAbilitiesContainsPower(SNOPower.Barbarian_Whirlwind))||
					 // If *NOT* on a boss, and definitely no boss in range, then judge based on any elites at all within 30 feet
					 ((!Bot.Target.CurrentTarget.IsBoss||
						HotbarAbilitiesContainsPower(SNOPower.Barbarian_Whirlwind))&&
						(!Bot.Combat.bAnyBossesInRange||
						HotbarAbilitiesContainsPower(SNOPower.Barbarian_Whirlwind))
								&&((ObjectCache.Objects.Clusters(10d, 30f, 3).Any(c => c.EliteCount>2))))) //find any with at least 3.)))&&
					 // Don't still have the buff
					&&!HasBuff(SNOPower.Barbarian_WrathOfTheBerserker)&&
					AbilityUseTimer(SNOPower.Barbarian_WrathOfTheBerserker, true)&&
					PowerManager.CanCast(SNOPower.Barbarian_WrathOfTheBerserker))
				{
					 if (Bot.Character.dCurrentEnergy>=50)
						  return new Ability(SNOPower.Barbarian_WrathOfTheBerserker, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 4, 4, USE_SLOWLY);
					 Bot.Class.bWaitingForSpecial=true;
				}
				// Call of the ancients, elites only
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_CallOfTheAncients)&&!Bot.Character.bIsIncapacitated&&
					(Bot.Combat.iElitesWithinRange[RANGE_25]>0||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=25f))&&
					AbilityUseTimer(SNOPower.Barbarian_CallOfTheAncients, true)&&
					PowerManager.CanCast(SNOPower.Barbarian_CallOfTheAncients))
				{
					 if (Bot.Character.dCurrentEnergy>=50)
						  return new Ability(SNOPower.Barbarian_CallOfTheAncients, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 4, 4, USE_SLOWLY);
					 Bot.Class.bWaitingForSpecial=true;
				}
				// Battle rage, for if being followed and before we do sprint
				if (bOOCBuff&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_BattleRage)&&
					(AbilityUseTimer(SNOPower.Barbarian_BattleRage)||!HasBuff(SNOPower.Barbarian_BattleRage))&&
					Bot.Character.dCurrentEnergy>=20&&PowerManager.CanCast(SNOPower.Barbarian_BattleRage))
				{
					 return new Ability(SNOPower.Barbarian_BattleRage, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// Special segment for sprint as an out-of-combat only
				if (bOOCBuff&&(SettingsFunky.OutOfCombatMovement||HasBuff(SNOPower.Barbarian_WrathOfTheBerserker))&&
					!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Sprint)&&
					!HasBuff(SNOPower.Barbarian_Sprint)&&
					Bot.Character.dCurrentEnergy>=20&&AbilityUseTimer(SNOPower.Barbarian_Sprint)&&PowerManager.CanCast(SNOPower.Barbarian_Sprint))
				{
					 return new Ability(SNOPower.Barbarian_Sprint, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
				}
				// War cry, constantly maintain
				if (!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_WarCry)&&
					(PowerManager.CanCast(SNOPower.Barbarian_WarCry)&&
					(!HasBuff(SNOPower.Barbarian_WarCry)
						||(PassiveSkillsContainsPower(SNOPower.Barbarian_Passive_InspiringPresence)&&DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Barbarian_WarCry]).TotalSeconds>59
						||Bot.Character.dCurrentEnergyPct<0.10)
					)))
				{
					 return new Ability(SNOPower.Barbarian_WarCry, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// war cry OOC 
				if (bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_WarCry)&&
					(PowerManager.CanCast(SNOPower.Barbarian_WarCry)&&Bot.Combat.iAnythingWithinRange[RANGE_25]>1)
					&&Bot.Character.dCurrentEnergyPct<0.9)
				{
					 return new Ability(SNOPower.Barbarian_WarCry, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}

				// Threatening shout
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_ThreateningShout)&&!Bot.Character.bIsIncapacitated&&
					(
						Bot.Combat.iElitesWithinRange[RANGE_20]>1||(Bot.Target.CurrentTarget.IsBoss&&Bot.Target.CurrentTarget.RadiusDistance<=20)||
						(Bot.Combat.iAnythingWithinRange[RANGE_20]>2&&!Bot.Combat.bAnyBossesInRange&&(Bot.Combat.iElitesWithinRange[RANGE_50]==0||HotbarAbilitiesContainsPower(SNOPower.Barbarian_SeismicSlam)))||
						Bot.Character.dCurrentHealthPct<=0.75
					)&&
					AbilityUseTimer(SNOPower.Barbarian_ThreateningShout, true)&&PowerManager.CanCast(SNOPower.Barbarian_ThreateningShout))
				{
					 return new Ability(SNOPower.Barbarian_ThreateningShout, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// Ground Stomp
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_GroundStomp)&&!Bot.Character.bIsIncapacitated&&
					(Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>4||Bot.Character.dCurrentHealthPct<=0.7)&&
					AbilityUseTimer(SNOPower.Barbarian_GroundStomp, true)&&
					PowerManager.CanCast(SNOPower.Barbarian_GroundStomp))
				{
					 return new Ability(SNOPower.Barbarian_GroundStomp, 16f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
				}
				// Revenge used off-cooldown
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Revenge)&&!Bot.Character.bIsIncapacitated&&
					 // Don't use revenge on goblins, too slow!
					(!Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Combat.iAnythingWithinRange[RANGE_12]>=5)&&
					 // Doesn't need CURRENT target to be in range, just needs ANYTHING to be within 9 foot, since it's an AOE!
					(Bot.Combat.iAnythingWithinRange[RANGE_6]>0||Bot.Target.CurrentTarget.RadiusDistance<=6f)&&
					AbilityUseTimer(SNOPower.Barbarian_Revenge)&&PowerManager.CanCast(SNOPower.Barbarian_Revenge))
				{
					 // Note - we have LONGER animation times for whirlwind-users
					 // Since whirlwind seems to interrupt rend so easily
					 int iPreDelay=3;
					 int iPostDelay=3;
					 if (HotbarAbilitiesContainsPower(SNOPower.Barbarian_Whirlwind))
					 {
						  if (Bot.Combat.powerLastSnoPowerUsed==SNOPower.Barbarian_Whirlwind)
						  {
								iPreDelay=5;
								iPostDelay=5;
						  }
					 }
					 return new Ability(SNOPower.Barbarian_Revenge, 0f, Bot.Character.Position, Bot.Character.iCurrentWorldID, -1, iPreDelay, iPostDelay, USE_SLOWLY);
				}
				// Furious charge
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_FuriousCharge)&&!Bot.Character.bIsRooted&&!Bot.Character.bIsIncapacitated&&
					(Bot.Combat.iElitesWithinRange[RANGE_25]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>3||Bot.Character.dCurrentHealthPct<=0.7||Bot.Target.CurrentTarget.CentreDistance>=15f||thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Target.CurrentTarget.IsBoss)&&
					AbilityUseTimer(SNOPower.Barbarian_FuriousCharge)&&
					PowerManager.CanCast(SNOPower.Barbarian_FuriousCharge))
				{
					 float fExtraDistance;
					 if (Bot.Target.CurrentTarget.CentreDistance<=15)
						  fExtraDistance=10;
					 else
						  fExtraDistance=(25-Bot.Target.CurrentTarget.CentreDistance);
					 if (fExtraDistance<5f)
						  fExtraDistance=5f;
					 Vector3 vNewTarget=MathEx.CalculatePointFrom(Bot.Target.CurrentTarget.Position, Bot.Character.Position, Bot.Target.CurrentTarget.CentreDistance+fExtraDistance);
					 return new Ability(SNOPower.Barbarian_FuriousCharge, 32f, vNewTarget, Bot.Character.iCurrentWorldID, -1, 1, 2, USE_SLOWLY);
				}
				// Leap used when off-cooldown, or when out-of-range
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Leap)&&!Bot.Character.bIsIncapacitated&&
					 // Less than 90% health *OR* target >= 18 feet away *OR* an elite within 30 feet of us
					(Bot.Character.dCurrentHealthPct<=0.75||Bot.Target.CurrentTarget.CentreDistance>=18f||thisCacheUnitObj!=null&&thisCacheUnitObj.ForceLeap||Bot.Combat.iElitesWithinRange[RANGE_30]>=1)&&
					AbilityUseTimer(SNOPower.Barbarian_Leap, true)&&
					PowerManager.CanCast(SNOPower.Barbarian_Leap))
				{
					 // For close-by monsters, try to leap a little further than their centre-point
					 float fExtraDistance=Bot.Target.CurrentTarget.Radius;
					 if (fExtraDistance<=4f)
						  fExtraDistance=4f;
					 if (Bot.Target.CurrentTarget.CentreDistance+fExtraDistance>35f)
						  fExtraDistance=35-Bot.Target.CurrentTarget.CentreDistance;
					 Vector3 vNewTarget=MathEx.CalculatePointFrom(Bot.Target.CurrentTarget.Position, Bot.Character.Position, Bot.Target.CurrentTarget.CentreDistance+fExtraDistance);
					 return new Ability(SNOPower.Barbarian_Leap, 35f, vNewTarget, Bot.Character.iCurrentWorldID, -1, 2, 2, USE_SLOWLY);
				}
				// Rend spam
				if (!bOOCBuff&&!Bot.Class.bWaitingForSpecial&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Rend)&&!Bot.Combat.UsedAutoMovementCommand&&!Bot.Character.bIsIncapacitated&&
					 //Only if 2 non-elite targets OR 1 elite target is within 6feet
					(Bot.Combat.iAnythingWithinRange[RANGE_6]>1||Bot.Combat.iElitesWithinRange[RANGE_6]>0)&&

					// Don't use against goblins (they run too quick!) Or any mobs added to the fast list unless elite.                                                                  
					(!Bot.Target.CurrentTarget.IsTreasureGoblin&&(!SnoCacheLookup.hashActorSNOFastMobs.Contains(Bot.Target.CurrentTarget.SNOID)||thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique)||Bot.Combat.iAnythingWithinRange[RANGE_6]>3)
					&&
					 //Non-WW users
					(!HotbarAbilitiesContainsPower(SNOPower.Barbarian_Whirlwind)&&
							(
					 //We use rend every 3.5s, Or if there are non-rended targets, or our current target is not rended
							(AbilityUseTimer(SNOPower.Barbarian_Rend)||(Bot.Combat.iNonRendedTargets_6>1||(thisCacheUnitObj!=null&&thisCacheUnitObj.HasDOTdps.HasValue&&!thisCacheUnitObj.HasDOTdps.Value)))
							)

					// This segment is for people who *DO* have whirlwind
						 ||(HotbarAbilitiesContainsPower(SNOPower.Barbarian_Whirlwind)&&
					 // See if it's off-cooldown and at least 40 fury, or use as a fury dump
							 (
								(SettingsFunky.Class.bFuryDumpWrath&&Bot.Character.dCurrentEnergyPct>=0.92&&HasBuff(SNOPower.Barbarian_WrathOfTheBerserker))||
								(SettingsFunky.Class.bFuryDumpAlways&&Bot.Character.dCurrentEnergyPct>=0.92)||
								(DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Barbarian_Rend]).TotalMilliseconds>=2800)
							 )&&
					 // Max once every 1.2 seconds even if fury dumping, so sprint can be fury dumped too
					 // DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Barbarian_Rend]).TotalMilliseconds >= 1200 &&
					 // 3+ mobs of any kind at close range *OR* one elite/boss/special at close range
							 (
								(Bot.Combat.iAnythingWithinRange[RANGE_15]>=3&&Bot.Combat.iElitesWithinRange[RANGE_12]>=1)||
								(Bot.Combat.iAnythingWithinRange[RANGE_15]>=3&&Bot.Target.CurrentTarget.IsTreasureGoblin&&Bot.Target.CurrentTarget.RadiusDistance<=6f)||
								Bot.Combat.iAnythingWithinRange[RANGE_15]>=5||
								((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=6f&&Bot.Combat.iAnythingWithinRange[RANGE_15]>=3)
							 )
						 )
					)&&
					 // And finally, got at least 20 energy
					Bot.Character.dCurrentEnergy>=20)
				{
					 //iACDGUIDLastRend = Bot.CurrentTarget.ObjectData.AcdGuid.Value;
					 // Note - we have LONGER animation times for whirlwind-users
					 // Since whirlwind seems to interrupt rend so easily
					 int iPreDelay=1;
					 int iPostDelay=1;
					 if (HotbarAbilitiesContainsPower(SNOPower.Barbarian_Whirlwind))
					 {
						  if (Bot.Combat.powerLastSnoPowerUsed==SNOPower.Barbarian_Whirlwind||Bot.Combat.powerLastSnoPowerUsed==SNOPower.Barbarian_HammerOfTheAncients||Bot.Combat.powerLastSnoPowerUsed==SNOPower.None)
						  {
								iPreDelay=5;
								iPostDelay=5;
						  }
					 }
					 return new Ability(SNOPower.Barbarian_Rend, 0f, Bot.Character.Position, Bot.Character.iCurrentWorldID, Bot.Target.CurrentTarget.AcdGuid.Value, iPreDelay, iPostDelay, USE_SLOWLY);
				}
				// Overpower used off-cooldown
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Overpower)&&!Bot.Character.bIsIncapacitated&&
					 // Doesn't need CURRENT target to be in range, just needs ANYTHING to be within 9 foot, since it's an AOE!
					 //(BOT.DATA_Combat.iAnythingWithinRange[RANGE_5] > 0 || Bot.CurrentTarget.ObjectData.RadiusDistance <= 6f) &&
					 //intell
					(
						Bot.Combat.iAnythingWithinRange[RANGE_6]>=2||
						(Bot.Character.dCurrentHealthPct<=0.85&&Bot.Target.CurrentTarget.RadiusDistance<=5f)||
						(
							Bot.Combat.iAnythingWithinRange[RANGE_6]>=1&&
							((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique)||Bot.Target.CurrentTarget.IsBoss||HasBuff(SNOPower.Barbarian_WrathOfTheBerserker)||
							 HotbarAbilitiesContainsPower(SNOPower.Barbarian_SeismicSlam))
						)
					)&&
					AbilityUseTimer(SNOPower.Barbarian_Overpower)&&PowerManager.CanCast(SNOPower.Barbarian_Overpower))
				{
					 // Note - we have LONGER animation times for whirlwind-users
					 // Since whirlwind seems to interrupt rend so easily
					 int iPreDelay=3;
					 int iPostDelay=3;
					 if (HotbarAbilitiesContainsPower(SNOPower.Barbarian_Whirlwind))
					 {
						  if (Bot.Combat.powerLastSnoPowerUsed==SNOPower.Barbarian_Whirlwind||Bot.Combat.powerLastSnoPowerUsed==SNOPower.None)
						  {
								iPreDelay=5;
								iPostDelay=5;
						  }
					 }
					 return new Ability(SNOPower.Barbarian_Overpower, 0f, Bot.Character.Position, Bot.Character.iCurrentWorldID, -1, iPreDelay, iPostDelay, USE_SLOWLY);
				}
				// Seismic slam enemies within close range
				if ((!bOOCBuff&&!Bot.Class.bWaitingForSpecial&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_SeismicSlam)
					 &&!Bot.Character.bIsIncapacitated&&PowerManager.CanCast(SNOPower.Barbarian_SeismicSlam))
					 &&(Bot.Character.dCurrentEnergy>30||Bot.Class.RuneIndexCache[SNOPower.Barbarian_SeismicSlam]==3&&Bot.Character.dCurrentEnergy>15))
				{
					 int RuneIndex=Bot.Class.RuneIndexCache[SNOPower.Barbarian_SeismicSlam];
					 //Reduce cluster for Crackling Rift
					 double ClusterDist=RuneIndex==4?4d:6d;
					 int ACDGuid=-1;

					 //Goblins ignore clustering..
					 if (thisCacheUnitObj!=null&&thisCacheUnitObj.IsTreasureGoblin&&thisCacheUnitObj.CurrentHealthPct<0.25)
					 {
						  return new Ability(SNOPower.Barbarian_SeismicSlam, 40f, vNullLocation, -1, thisCacheUnitObj.AcdGuid.Value, 2, 2, USE_SLOWLY);
					 }
					 else if (ObjectCache.Objects.Clusters(ClusterDist, 40f, 2).Count>0)
					 {
						  ACDGuid=ObjectCache.Objects.Clusters(MinUnitCount: 2)[0].CurrentValidUnit.AcdGuid.Value;
						  return new Ability(SNOPower.Barbarian_SeismicSlam, 40f, vNullLocation, -1, ACDGuid, 2, 2, USE_SLOWLY);
					 }
				}
				// Ancient spear 
				if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_AncientSpear)&&
					 // Don't use if target < 20% health
					(thisCacheUnitObj!=null&&thisCacheUnitObj.CurrentHealthPct>=0.2)&&
					AbilityUseTimer(SNOPower.Barbarian_AncientSpear)&&
					PowerManager.CanCast(SNOPower.Barbarian_AncientSpear))
				{
					 // For close-by monsters, try to leap a little further than their centre-point
					 float fExtraDistance=Bot.Target.CurrentTarget.Radius;
					 if (fExtraDistance<=4f)
						  fExtraDistance=4f;
					 if (Bot.Target.CurrentTarget.CentreDistance+fExtraDistance>30f)
						  fExtraDistance=30-Bot.Target.CurrentTarget.CentreDistance;
					 if (fExtraDistance<5f)
						  fExtraDistance=5f;
					 Vector3 vNewTarget=MathEx.CalculatePointFrom(Bot.Target.CurrentTarget.Position, Bot.Character.Position, Bot.Target.CurrentTarget.CentreDistance+fExtraDistance);
					 return new Ability(SNOPower.Barbarian_AncientSpear, 35f, vNewTarget, Bot.Character.iCurrentWorldID, -1, 2, 2, USE_SLOWLY);
				}
				// Sprint buff, if same suitable targets as elites, keep maintained for WW users
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Sprint)
					&&!Bot.Character.bIsIncapacitated&&!HasBuff(SNOPower.Barbarian_Sprint)
					&&
					 // Fury Dump Options for sprint: use at max energy constantly, or on a timer
					(
						(SettingsFunky.Class.bFuryDumpWrath&&Bot.Character.dCurrentEnergyPct>=0.95&&HasBuff(SNOPower.Barbarian_WrathOfTheBerserker))||
						(SettingsFunky.Class.bFuryDumpAlways&&Bot.Character.dCurrentEnergyPct>=0.95)||
						((AbilityUseTimer(SNOPower.Barbarian_Sprint)&&!HasBuff(SNOPower.Barbarian_Sprint))&&
					 // Always keep up if we are whirlwinding, or if the target is a goblin
							(HotbarAbilitiesContainsPower(SNOPower.Barbarian_Whirlwind)||Bot.Target.CurrentTarget.IsTreasureGoblin))
					)&&
					 // Or if the target is >16 feet away and we have 50+ fury
					 //(Bot.CurrentTarget.ObjectData.CentreDistance >= 16f && BOT.DATA_Character.dCurrentEnergy >= 50)
					 // If they have battle-rage, make sure it's up
					(!HotbarAbilitiesContainsPower(SNOPower.Barbarian_BattleRage)||(HotbarAbilitiesContainsPower(SNOPower.Barbarian_BattleRage)&&HasBuff(SNOPower.Barbarian_BattleRage)))&&
					 // Check for reserved-energy waiting or not
					 //((BOT.DATA_Character.dCurrentEnergy >= 40 && !BOT.DATA_Character.bWaitingForReserveEnergy) || BOT.DATA_Character.dCurrentEnergy >= Bot.Class.iWaitingReservedAmount) &&
					Bot.Character.dCurrentEnergy>=20)
				{
					 return new Ability(SNOPower.Barbarian_Sprint, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// Whirlwind spam as long as necessary pre-buffs are up
				if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Class.bWaitingForSpecial&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Whirlwind)&&!Bot.Character.bIsIncapacitated&&!Bot.Character.bIsRooted
					 // Don't WW against goblins, units in the special SNO list
					&&(!SettingsFunky.Class.bSelectiveWhirlwind||Bot.Combat.bAnyNonWWIgnoreMobsInRange||!SnoCacheLookup.hashActorSNOWhirlwindIgnore.Contains(Bot.Target.CurrentTarget.SNOID))&&
					 // Only if within 15 foot of main target
					((Bot.Target.CurrentTarget.RadiusDistance<=20f||Bot.Combat.iAnythingWithinRange[RANGE_25]>=1)&&
					(Bot.Combat.iAnythingWithinRange[RANGE_50]>=2||(thisCacheUnitObj!=null&&thisCacheUnitObj.CurrentHealthPct>=0.30)||Bot.Target.CurrentTarget.IsBoss||(thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique)||Bot.Character.dCurrentHealthPct<=0.60))&&
					 // Check for energy reservation amounts
					 //((BOT.DATA_Character.dCurrentEnergy >= 20 && !BOT.DATA_Character.bWaitingForReserveEnergy) || BOT.DATA_Character.dCurrentEnergy >= Bot.Class.iWaitingReservedAmount) &&
					Bot.Character.dCurrentEnergy>=10&&
					 // If they have battle-rage, make sure it's up
					(!HotbarAbilitiesContainsPower(SNOPower.Barbarian_BattleRage)||(HotbarAbilitiesContainsPower(SNOPower.Barbarian_BattleRage)&&HasBuff(SNOPower.Barbarian_BattleRage))))
				// If they have sprint, make sure it's up
				//(!hashPowerHotbarAbilities.Contains(SNOPower.Barbarian_Sprint) || (hashPowerHotbarAbilities.Contains(SNOPower.Barbarian_Sprint) && GilesHasBuff(SNOPower.Barbarian_Sprint))))
				{
					 bool bGenerateNewZigZag=(DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=2000f||
						  (Bot.Combat.vPositionLastZigZagCheck!=vNullLocation&&Bot.Character.Position==Bot.Combat.vPositionLastZigZagCheck&&DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=1200)||
						  Vector3.Distance(Bot.Character.Position, Bot.Combat.vSideToSideTarget)<=5f||
						  Bot.Target.CurrentTarget.AcdGuid.Value!=Bot.Combat.iACDGUIDLastWhirlwind);
					 Bot.Combat.vPositionLastZigZagCheck=Bot.Character.Position;
					 if (bGenerateNewZigZag)
					 {
						  //float fExtraDistance = targetCurrent.fCentreDistance+(targetCurrent.fCentreDistance <= 16f ? 16f : 8f);
						  //Bot.Combat.vSideToSideTarget = FindZigZagTargetLocation(Bot.CurrentTarget.Position, Bot.CurrentTarget.CentreDistance + 25f);
						  // Resetting this to ensure the "no-spam" is reset since we changed our target location
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
					 return new Ability(SNOPower.Barbarian_Whirlwind, 15f, Bot.Combat.vSideToSideTarget, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
				}
				// Battle rage, constantly maintain
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_BattleRage)&&!Bot.Character.bIsIncapacitated&&
					 // Fury Dump Options for battle rage IF they don't have sprint 
					(
					 (SettingsFunky.Class.bFuryDumpWrath&&Bot.Character.dCurrentEnergyPct>=0.99&&HasBuff(SNOPower.Barbarian_WrathOfTheBerserker))||
					 (SettingsFunky.Class.bFuryDumpAlways&&Bot.Character.dCurrentEnergyPct>=0.99)||!HasBuff(SNOPower.Barbarian_BattleRage)
					)&&
					Bot.Character.dCurrentEnergy>=20&&PowerManager.CanCast(SNOPower.Barbarian_BattleRage))
				{
					 return new Ability(SNOPower.Barbarian_BattleRage, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				}
				// Hammer of the ancients spam-attacks - never use if waiting for special
				if (!bOOCBuff&&!bCurrentlyAvoiding
					 &&HotbarAbilitiesContainsPower(SNOPower.Barbarian_HammerOfTheAncients)
					 &&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=20&&PowerManager.CanCast(SNOPower.Barbarian_HammerOfTheAncients)&&!Bot.Class.bWaitingForSpecial)
				{
					 int RuneIndex=Bot.Class.RuneIndexCache[SNOPower.Barbarian_HammerOfTheAncients];
					 float Range=16f;
					 double ClusterDist=5d;
					 int ACDGuid=-1;

					 if (RuneIndex==0)
						  Range=13f;
					 else if (RuneIndex==1)
					 {
						  Range=20f;
						  ClusterDist=6d;
					 }
					 //0 == 12f, 4d
					 //1 == 24f, 7d
					 //	  16f, 6d

					 if (Bot.Target.CurrentTarget.IsTreasureGoblin)
						  return new Ability(SNOPower.Barbarian_HammerOfTheAncients, 12f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 3, USE_SLOWLY);
					 else
					 {

						  if (ObjectCache.Objects.Clusters(ClusterDist, Range, 2).Count>0)
						  {
								ACDGuid=ObjectCache.Objects.Clusters(MinUnitCount: 2)[0].CurrentValidUnit.AcdGuid.Value;

								return new Ability(SNOPower.Barbarian_HammerOfTheAncients, 0f, vNullLocation, Bot.Character.iCurrentWorldID, ACDGuid, 1, 2, USE_SLOWLY);
						  }
						  else if (thisCacheUnitObj!=null&&thisCacheUnitObj.ObjectIsSpecial)
						  {
								ACDGuid=Bot.Target.CurrentTarget.AcdGuid.Value;
								return new Ability(SNOPower.Barbarian_HammerOfTheAncients, Range, vNullLocation, -1, ACDGuid, 1, 2, USE_SLOWLY);
						  }
					 }
				}
				// Weapon throw
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_WeaponThrow)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dCurrentEnergy>=10)
				{
					 return new Ability(SNOPower.Barbarian_WeaponThrow, 44f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
				}
				// Frenzy rapid-attacks
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Frenzy))
				{
					 return new Ability(SNOPower.Barbarian_Frenzy, 10f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 0, SIGNATURE_SPAM);
				}
				// Bash fast-attacks
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Bash))
				{
					 return new Ability(SNOPower.Barbarian_Bash, 10f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
				}
				// Cleave fast-attacks
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Cleave))
				{
					 return new Ability(SNOPower.Barbarian_Cleave, 10f, vNullLocation, Bot.Character.iCurrentWorldID, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 2, USE_SLOWLY);
				}
				// Default attacks
				if (!bOOCBuff&&!bCurrentlyAvoiding)
				{
					 return new Ability(SNOPower.Weapon_Melee_Instant, 10f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
				}

				return new Ability(SNOPower.None, 0, vNullLocation, -1, -1, 0, 0, false);
				#endregion

		  }
	 }
}