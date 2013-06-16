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
		  internal static Ability WitchDoctorAbility(bool bCurrentlyAvoiding=false, bool bOOCBuff=false, bool bDestructiblePower=false)
		  {
				// Extra height thingy, not REALLY used as it was originally going to be, will probably get phased out...
				float iThisHeight=2f;

				// Pick the best destructible power available
				if (bDestructiblePower)
				{
					 SNOPower destructiblePower=Bot.Class.DestructiblePower();
					 if (destructiblePower==SNOPower.Witchdoctor_ZombieCharger)
					 {
						  if (Bot.Character.dCurrentEnergy>=140)
								return new Ability(SNOPower.Witchdoctor_ZombieCharger, 12f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
						  else
								return new Ability(SNOPower.Weapon_Melee_Instant, 10f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
					 }


					 return new Ability(destructiblePower, 12f, vNullLocation, -1, -1, 0, 0, USE_SLOWLY);
				}
				CacheUnit thisCacheUnitObj;

				if (!bOOCBuff&&!Bot.Target.Equals(null)&&Bot.Target.CurrentTarget.targetType.Value==TargetType.Unit)
					 thisCacheUnitObj=(CacheUnit)Bot.Target.CurrentTarget;
				else
					 thisCacheUnitObj=null;

				// Witch doctors have no reserve requirements?
				Bot.Class.iWaitingReservedAmount=0;

				#region Spirit Walk
				// Spirit Walk Cast on 65% health or while avoiding anything but molten core or incapacitated or Chasing Goblins
				if (HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_SpiritWalk)&&Bot.Character.dCurrentEnergy>=49&&
					(Bot.Character.dCurrentHealthPct<=0.65||(Bot.Combat.IsKiting&&Bot.Combat.iAnythingWithinRange[RANGE_15]>1)||Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted||(Bot.SettingsFunky.OutOfCombatMovement&&bOOCBuff)||
					 (!bOOCBuff&&Bot.Target.CurrentTarget.IsTreasureGoblin&&thisCacheUnitObj!=null&&thisCacheUnitObj.CurrentHealthPct<0.90&&Bot.Target.CurrentTarget.RadiusDistance<=40f))
					 &&PowerManager.CanCast(SNOPower.Witchdoctor_SpiritWalk))
				{
					 return new Ability(SNOPower.Witchdoctor_SpiritWalk, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
				} 
				#endregion

				#region Soul Harvest
				// Soul Harvest Any Elites or 2+ Norms and baby it's harvest season
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_SoulHarvest)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=59&&GetBuffStacks(SNOPower.Witchdoctor_SoulHarvest)<4&&PowerManager.CanCast(SNOPower.Witchdoctor_SoulHarvest))
				{
					 System.Collections.Generic.List<Cluster> clusters=Clusters(2d, 4f, 3, true);
					 if (clusters.Count>0)
					 {
						  return new Ability(SNOPower.Witchdoctor_SoulHarvest, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 1, USE_SLOWLY);
					 }

				} 
				#endregion
				#region Sacrifice
				// Sacrifice AKA Zombie Dog Jihad, use on Elites Only or to try and Save yourself
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_Sacrifice)&&
					(Bot.Combat.iElitesWithinRange[RANGE_15]>0||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss||Bot.Target.CurrentTarget.IsTreasureGoblin)&&Bot.Target.CurrentTarget.RadiusDistance<=15f))&&
					PowerManager.CanCast(SNOPower.Witchdoctor_Sacrifice))
				{
					 return new Ability(SNOPower.Witchdoctor_Sacrifice, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 0, USE_SLOWLY);
				} 
				#endregion
				#region Gargantuan
				// Gargantuan, Recast on 1+ Elites or Bosses to trigger Restless Giant
				if (HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_Gargantuan)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=147&&PowerManager.CanCast(SNOPower.Witchdoctor_Gargantuan)&&
					 (Bot.Class.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]==0&&(Bot.Combat.iElitesWithinRange[RANGE_15]>=1||(thisCacheUnitObj!=null&&((thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=15f)))
					 ||Bot.Class.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]!=0&&Bot.Character.PetData.Gargantuan==0))
				{
					 return new Ability(SNOPower.Witchdoctor_Gargantuan, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 2, 1, USE_SLOWLY);
				} 
				#endregion
				#region Zombie dogs
				// Zombie dogs Woof Woof, good for being blown up, cast when less than or equal to 1 Dog or Not Blowing them up and cast when less than 4
				if (HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_SummonZombieDog)
					 &&(Bot.Class.PassiveAbilities.Contains(SNOPower.Witchdoctor_Passive_ZombieHandler)&&Bot.Character.PetData.ZombieDogs<4||Bot.Character.PetData.ZombieDogs<3)
					 &&PowerManager.CanCast(SNOPower.Witchdoctor_SummonZombieDog))
				{
					 return new Ability(SNOPower.Witchdoctor_SummonZombieDog, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
				} 
				#endregion
				#region Hex
				// Hex Spam Cast on ANYTHING in range, mmm pork and chicken
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_Hex)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=49&&
					(Bot.Combat.iElitesWithinRange[RANGE_12]>=1||Bot.Combat.iAnythingWithinRange[RANGE_12]>=1||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=18f))&&
					PowerManager.CanCast(SNOPower.Witchdoctor_Hex))
				{
					 return new Ability(SNOPower.Witchdoctor_Hex, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
				} 
				#endregion
				#region Mass Confuse
				// Mass Confuse, elites only or big mobs or to escape on low health
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_MassConfusion)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=74&&
					(Bot.Combat.iElitesWithinRange[RANGE_12]>=1||Bot.Combat.iAnythingWithinRange[RANGE_12]>=6||Bot.Character.dCurrentHealthPct<=0.25||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=12f))&&
					!Bot.Target.CurrentTarget.IsTreasureGoblin&&PowerManager.CanCast(SNOPower.Witchdoctor_MassConfusion))
				{
					 return new Ability(SNOPower.Witchdoctor_MassConfusion, 0f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Big Bad Voodoo
				// Big Bad Voodoo, elites and bosses only
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_BigBadVoodoo)&&!Bot.Character.bIsIncapacitated&&
					!Bot.Target.CurrentTarget.IsTreasureGoblin&&
					(Bot.Combat.iElitesWithinRange[RANGE_6]>0||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=12f))&&
					PowerManager.CanCast(SNOPower.Witchdoctor_BigBadVoodoo))
				{
					 return new Ability(SNOPower.Witchdoctor_BigBadVoodoo, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
				} 
				#endregion
				#region Grasp of the Dead
				// Grasp of the Dead, look below, droping globes and dogs when using it on elites and 3 norms
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_GraspOfTheDead)&&!Bot.Character.bIsIncapacitated&&
					Bot.Character.dCurrentEnergy>=122&&PowerManager.CanCast(SNOPower.Witchdoctor_GraspOfTheDead))
				{
					 if (Clusters(5d, 35f, 2).Count>0)
					 {
						  Vector3 Center=Clusters()[0].ListUnits[0].Position;
						  return new Ability(SNOPower.Witchdoctor_GraspOfTheDead, 35f, Center, Bot.Character.iCurrentWorldID, -1, 0, 3, USE_SLOWLY);
					 }
				} 
				#endregion
				#region Horrify
				// Horrify Buff at 60% health
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_Horrify)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=37&&
					Bot.Character.dCurrentHealthPct<=0.60&&
					PowerManager.CanCast(SNOPower.Witchdoctor_Horrify))
				{
					 return new Ability(SNOPower.Witchdoctor_Horrify, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
				} 
				#endregion
				#region Fetish Army
				// Fetish Army, elites only
				if (!bOOCBuff&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_FetishArmy)&&!Bot.Character.bIsIncapacitated&&
					(Bot.Combat.iElitesWithinRange[RANGE_25]>0||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=16f))&&
					PowerManager.CanCast(SNOPower.Witchdoctor_FetishArmy))
				{
					 return new Ability(SNOPower.Witchdoctor_FetishArmy, 0f, vNullLocation, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Spirit Barrage
				// Spirit Barrage
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_SpiritBarrage)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=108&&
					PowerManager.CanCast(SNOPower.Witchdoctor_SpiritBarrage))
				{
					 return new Ability(SNOPower.Witchdoctor_SpiritBarrage, 21f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Haunt
				// Haunt the shit out of monster and maybe they will give you treats
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_Haunt)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=98&&
					PowerManager.CanCast(SNOPower.Witchdoctor_Haunt)&&AbilityLastUseMS(SNOPower.Witchdoctor_Haunt)>1000
					 &&!Bot.Combat.bAnyTreasureGoblinsPresent||Bot.SettingsFunky.GoblinPriority<2)
				{
					 //We want to only haunt clusters of at least 2 units with no current haunt.

					 if (Clusters(5d, 30f, 2).Count>0)
					 {
						  if (Clusters().Any(c => c.DotDPSRatio<0.25))
						  {
								Cluster clust=Clusters().First(c => c.DotDPSRatio<0.25);
								if (clust.ListUnits.Any(u => !u.HasDOTdps.HasValue||!u.HasDOTdps.Value))
								{
									 int acdguid=clust.ListUnits.First(u => !u.HasDOTdps.HasValue||!u.HasDOTdps.Value).AcdGuid.Value;
									 return new Ability(SNOPower.Witchdoctor_Haunt, 21f, vNullLocation, Bot.Character.iCurrentWorldID, acdguid, 1, 1, USE_SLOWLY);
								}
						  }
					 }


				} 
				#endregion
				#region Locust
				// Locust
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_Locust_Swarm)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=196&&
					PowerManager.CanCast(SNOPower.Witchdoctor_Locust_Swarm)&&AbilityLastUseMS(SNOPower.Witchdoctor_Locust_Swarm)>1000
					 &&!Bot.Combat.bAnyTreasureGoblinsPresent||Bot.SettingsFunky.GoblinPriority<2)
				{
					 if (Clusters(5d, 30f, 2).Count>0)
					 {
						  if (Clusters().Any(c => c.DotDPSRatio<0.25))
						  {
								Cluster clust=Clusters().First(c => c.DotDPSRatio<0.25);
								if (clust.ListUnits.Any(u => !u.HasDOTdps.HasValue||!u.HasDOTdps.Value))
								{
									 int acdguid=clust.ListUnits.First(u => !u.HasDOTdps.HasValue||!u.HasDOTdps.Value).AcdGuid.Value;
									 return new Ability(SNOPower.Witchdoctor_Locust_Swarm, 21f, vNullLocation, Bot.Character.iCurrentWorldID, acdguid, 1, 1, USE_SLOWLY);
								}
						  }
					 }
				} 
				#endregion

				#region Wall of Zombies
				// Wall of Zombies
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_WallOfZombies)&&!Bot.Character.bIsIncapacitated&&
					(Bot.Combat.iElitesWithinRange[RANGE_15]>0||Bot.Combat.iAnythingWithinRange[RANGE_15]>3||((thisCacheUnitObj!=null&&thisCacheUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin||Bot.Target.CurrentTarget.IsBoss)&&Bot.Target.CurrentTarget.RadiusDistance<=25f))&&
					Bot.Character.dCurrentEnergy>=103&&PowerManager.CanCast(SNOPower.Witchdoctor_WallOfZombies))
				{
					 return new Ability(SNOPower.Witchdoctor_WallOfZombies, 25f, Bot.Target.CurrentTarget.Position, Bot.Character.iCurrentWorldID, -1, 1, 1, USE_SLOWLY);
				} 
				#endregion
				#region Zombie Charger
				// Zombie Charger aka Zombie bears Spams Bears @ Everything from 11feet away
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_ZombieCharger)&&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=134&&
					(Bot.Combat.iAnythingWithinRange[RANGE_12]>1||(thisCacheUnitObj!=null&&thisCacheUnitObj.ObjectIsSpecial))&&
					PowerManager.CanCast(SNOPower.Witchdoctor_ZombieCharger))
				{
					 if (thisCacheUnitObj!=null&&thisCacheUnitObj.ObjectIsSpecial)
						  return new Ability(SNOPower.Witchdoctor_ZombieCharger, 11f, new Vector3(Bot.Target.CurrentTarget.Position.X, Bot.Target.CurrentTarget.Position.Y, Bot.Target.CurrentTarget.Position.Z+iThisHeight), Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
					 else
					 {
						  //Cluster tests
						  if (Clusters(6d, 25f, 2, true).Count>0)
						  {
								Vector3 Centeroid=Clusters()[0].ListUnits[0].BotMeleeVector;
								return new Ability(SNOPower.Witchdoctor_ZombieCharger, 0f, Centeroid, Bot.Character.iCurrentWorldID, -1, 0, 0, USE_SLOWLY);
						  }
					 }
				} 
				#endregion
				#region Acid Cloud
				// Acid Cloud
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_AcidCloud)&&AbilityUseTimer(SNOPower.Witchdoctor_AcidCloud)
					 &&!Bot.Character.bIsIncapacitated&&Bot.Character.dCurrentEnergy>=250
					 &&PowerManager.CanCast(SNOPower.Witchdoctor_AcidCloud))
				{
					 bool ConditionalTestResult=false;
					 int ACDGuid=-1;
					 Vector3 Location=vNullLocation;
					 float range=0f;
					 int RuneIndex=Bot.Class.RuneIndexCache[SNOPower.Witchdoctor_AcidCloud];
					 //1 == Larger Radius (25f)
					 //4 == Kiss Of Death (Directional AOE 20f)

					 if (thisCacheUnitObj!=null&&thisCacheUnitObj.IsMissileReflecting)
					 {
						  ConditionalTestResult=true;
						  ACDGuid=thisCacheUnitObj.AcdGuid.Value;
					 }
					 else
					 {

						  if (RuneIndex==4)
						  {
								//Only tight cluster
								if (Clusters(6d, 30f, 2).Count>0)
								{
									 ConditionalTestResult=true;
									 Location=Clusters()[0].ListUnits[0].Position;
								}
						  }
						  else
						  {
								//If we using the 24f rune then we allow larger distance for cluster
								double distance=RuneIndex==1?6d:4d;

								if (Clusters(distance, 45f, 2).Count>0)
								{
									 ConditionalTestResult=true;
									 ACDGuid=Clusters()[0].ListUnits[0].AcdGuid.Value;
									 
								}
						  }
					 }

					 //Setup range
					 if (ConditionalTestResult)
					 {
						  if (RuneIndex==4)
								range=20f;
						  else
								range=40f;
					 }

					 if (ConditionalTestResult)
						  return new Ability(SNOPower.Witchdoctor_AcidCloud, range, Location, Bot.Character.iCurrentWorldID, ACDGuid, 1, 1, USE_SLOWLY);

				} 
				#endregion
				#region Fire Bats
				// Fire Bats fast-attack
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_Firebats)&&!Bot.Character.bIsIncapacitated)
				{
					 //Check our mana first!
					 if (Bot.Character.dCurrentEnergy>=221||(Bot.Character.dCurrentEnergy>66
						  &&Bot.Character.CurrentAnimationState==AnimationState.Channeling&&Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.WitchDoctor_Female_1HT_spell_channel|SNOAnim.WitchDoctor_Female_2HT_spell_channel|SNOAnim.WitchDoctor_Female_HTH_spell_channel|SNOAnim.WitchDoctor_Male_1HT_Spell_Channel|SNOAnim.WitchDoctor_Male_HTH_Spell_Channel)))
					 {
						  bool ConditionalTestResult=false;
						  int ACDGuid=-1;
						  Vector3 Location=vNullLocation;
						  float range=0f;
						  int RuneIndex=Bot.Class.RuneIndexCache[SNOPower.Witchdoctor_Firebats];

						  //Reflective units ignore normal tests.
						  if (thisCacheUnitObj!=null&&thisCacheUnitObj.IsMissileReflecting)
						  {
								ACDGuid=thisCacheUnitObj.AcdGuid.Value;
								ConditionalTestResult=true;
						  }
						  else
						  {
								System.Collections.Generic.List<Cluster> clusters;
								//Dire Bats
								if (RuneIndex==0)
								{
									 clusters=Clusters(5d, 25f, 2);
									 //We want a cluster that is tight with at least 2 units!
									 if (clusters.Count>0)
									 {
										  ConditionalTestResult=true;
										  Location=clusters[0].ListUnits[0].Position;
									 }

								}
								else if (RuneIndex==4)
								{//Cloud -- only 12f range!

									 clusters=Clusters(6d, 12f, 2);
									 if (clusters.Count>0)
									 {
										  ConditionalTestResult=true;
										  Location=Bot.Character.Position;
									 }
								}
								else
								{//Posion/Vampire/HungryBats/NoRune (Small AOE Range)
									 clusters=Clusters(4d, 18f, 2);
									 //we want cluster that is semi-tight with at least 2 units!
									 if (clusters.Count>0)
									 {
										  ConditionalTestResult=true;
										  Location=clusters[0].ListUnits[0].Position;
									 }
								}
						  }

						  //Setup range
						  if (ConditionalTestResult)
						  {
								if (RuneIndex==0)
									 range=0f;
								else if (RuneIndex==4)
									 range=14f;
								else
									 range=25f;
						  }

						  if (ConditionalTestResult)
								return new Ability(SNOPower.Witchdoctor_Firebats, range, Location, Bot.Character.iCurrentWorldID, ACDGuid, 1, 2, USE_SLOWLY);
					 }
				} 
				#endregion


				#region Poison Darts
				// Poison Darts fast-attack Spams Darts when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_PoisonDart)&&!Bot.Character.bIsIncapacitated)
				{
					 return new Ability(SNOPower.Witchdoctor_PoisonDart, 44f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
				} 
				#endregion
				#region Corpse Spiders
				// Corpse Spiders fast-attacks Spams Spiders when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_CorpseSpider)&&!Bot.Character.bIsIncapacitated)
				{
					 float fUseThisRange=35f;
					 if (HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_ZombieCharger)&&Bot.Character.dCurrentEnergy>=150)
						  fUseThisRange=30f;
					 return new Ability(SNOPower.Witchdoctor_CorpseSpider, fUseThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
				} 
				#endregion
				#region Toads
				// Toads fast-attacks Spams Toads when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_PlagueOfToads)&&!Bot.Character.bIsIncapacitated)
				{
					 float fUseThisRange=35f;
					 if (HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_AcidCloud)&&Bot.Character.dCurrentEnergy>=225)
						  fUseThisRange=30f;
					 return new Ability(SNOPower.Witchdoctor_PlagueOfToads, fUseThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 0, 1, USE_SLOWLY);
				} 
				#endregion
				#region Fire Bomb
				// Fire Bomb fast-attacks Spams Bomb when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
				if (!bOOCBuff&&!bCurrentlyAvoiding&&HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_Firebomb)&&!Bot.Character.bIsIncapacitated)
				{
					 float fUseThisRange=35f;
					 if (HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_ZombieCharger)&&Bot.Character.dCurrentEnergy>=150)
						  fUseThisRange=30f;
					 return new Ability(SNOPower.Witchdoctor_Firebomb, fUseThisRange, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 3, USE_SLOWLY);
				} 
				#endregion
				#region Default attacks
				// Default attacks
				if (!bOOCBuff&&!bCurrentlyAvoiding&&!Bot.Character.bIsIncapacitated)
				{
					 return new Ability(SNOPower.Weapon_Melee_Instant, 11f, vNullLocation, -1, Bot.Target.CurrentTarget.AcdGuid.Value, 1, 1, USE_SLOWLY);
				} 
				#endregion
				return new Ability(SNOPower.None, 0, vNullLocation, -1, -1, 0, 0, false);
	
		  }
	 }
}