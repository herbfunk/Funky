using System;
using System.Linq;
using FunkyTrinity.Cache;
using FunkyTrinity.Enums;
using FunkyTrinity.Movement;
using FunkyTrinity.Movement.Clustering;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity
{
	 public class TLA_Finalize : TargetLogicAction
	{
		 public override void Initialize()
		 {
			  base.Test=(ref CacheObject obj) =>
			  {
					//Final Possible Target Check
					if (obj==null)
					{
						 // No valid targets but we were told to stay put?
						 if (FunkyTrinity.Bot.Combat.bStayPutDuringAvoidance)
						 {
							  if (FunkyTrinity.Bot.Combat.TriggeringAvoidances.Count==0)
							  {
									obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.Avoidance, 20000, "StayPutPoint", 2.5f, -1);
									return true;
							  }
							  else
									FunkyTrinity.Bot.Combat.iMillisecondsCancelledEmergencyMoveFor=0; //reset wait time
						 }

						 // See if we should wait for milliseconds for possible loot drops before continuing run
						 if (DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.lastHadUnitInSights).TotalMilliseconds<=FunkyTrinity.Bot.SettingsFunky.AfterCombatDelay&&DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.lastHadEliteUnitInSights).TotalMilliseconds<=10000||
							  //Cut the delay time in half for non-elite monsters!
							 DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.lastHadUnitInSights).TotalMilliseconds<=FunkyTrinity.Bot.SettingsFunky.AfterCombatDelay)
						 {
							  obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.Avoidance, 20000, "WaitForLootDrops", 2f, -1);
							  return true;

						 }
						 //Herbfunks wait after loot containers are opened. 3s for rare chests, half the settings delay for everything else.
						 if ((DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.lastHadRareChestAsTarget).TotalMilliseconds<=3750)||
							 (DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.lastHadContainerAsTarget).TotalMilliseconds<=(FunkyTrinity.Bot.SettingsFunky.AfterCombatDelay*1.25)))
						 {
							  obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.Avoidance, 20000, "ContainerLootDropsWait", 2f, -1);
							  return true;
						 }

						 // Finally, a special check for waiting for wrath of the berserker cooldown before engaging Azmodan
						 if (FunkyTrinity.Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_WrathOfTheBerserker)&&FunkyTrinity.Bot.SettingsFunky.Class.bWaitForWrath&&!FunkyTrinity.Bot.Class.AbilityUseTimer(SNOPower.Barbarian_WrathOfTheBerserker)&&
							 ZetaDia.CurrentWorldId==121214&&
							 (Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						 {
							  Logging.Write("[Funky] Waiting for Wrath Of The Berserker cooldown before continuing to Azmodan.");
							  obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.Avoidance, 20000, "GilesWaitForWrath", 0f, -1);
							  InactivityDetector.Reset();
							  return true;
						 }
						 // And a special check for wizard archon
						 if (FunkyTrinity.Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_Archon)&&!FunkyTrinity.Bot.Class.AbilityUseTimer(SNOPower.Wizard_Archon)&&FunkyTrinity.Bot.SettingsFunky.Class.bWaitForArchon&&ZetaDia.CurrentWorldId==121214&&
							 (Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						 {
							  Logging.Write("[Funky] Waiting for Wizard Archon cooldown before continuing to Azmodan.");
							  obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.Avoidance, 20000, "GilesWaitForArchon", 0f, -1);
							  InactivityDetector.Reset();
							  return true;
						 }
						 // And a very sexy special check for WD BigBadVoodoo
						 if (FunkyTrinity.Bot.Class.HotbarPowers.Contains(SNOPower.Witchdoctor_BigBadVoodoo)&&!PowerManager.CanCast(SNOPower.Witchdoctor_BigBadVoodoo)&&ZetaDia.CurrentWorldId==121214&&
							 (Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						 {
							  Logging.Write("[Funky] Waiting for WD BigBadVoodoo cooldown before continuing to Azmodan.");
							  obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.Avoidance, 20000, "GilesWaitForVoodooo", 0f, -1);
							  InactivityDetector.Reset();
							  return true;
						 }


						 //Check if our current path intersects avoidances. (When not in town, and not currently inside avoidance)
						 if (!FunkyTrinity.Bot.Character.bIsInTown&&(FunkyTrinity.Bot.SettingsFunky.AttemptAvoidanceMovements||FunkyTrinity.Bot.Combat.CriticalAvoidance)
								 &&Navigation.NP.CurrentPath.Count>0
								 &&FunkyTrinity.Bot.Combat.TriggeringAvoidances.Count==0)
						 {
							  Vector3 curpos=FunkyTrinity.Bot.Character.Position;
							  IndexedList<Vector3> curpath=Navigation.NP.CurrentPath;

							  var CurrentNearbyPath=curpath.Where(v => curpos.Distance(v)<=40f);
							  if (CurrentNearbyPath!=null&&CurrentNearbyPath.Any())
							  {
									CurrentNearbyPath.OrderBy(v => curpath.IndexOf(v));

									Vector3 lastV3=Vector3.Zero;
									foreach (var item in CurrentNearbyPath)
									{
										 if (lastV3==Vector3.Zero)
											  lastV3=item;
										 else if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(item, lastV3))
										 {
											  obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.Avoidance, 20000, "AvoidanceIntersection", 2.5f, -1);
											  return true;


										 }
									}
							  }
						 }
					}
					
					
					return false;
			  };
		 }
	}
}
