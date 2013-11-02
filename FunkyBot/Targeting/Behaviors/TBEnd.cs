using System;
using System.Linq;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Movement;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;
using Zeta.Navigation;

namespace FunkyBot.Targeting.Behaviors
{
	 public class TBEnd : TargetBehavior
	 {
		  public TBEnd() : base() { }
		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Target; } }


		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				{
					 if (obj==null)
					 {
						  // See if we should wait for milliseconds for possible loot drops before continuing run
						  if (DateTime.Now.Subtract(Bot.Targeting.lastHadUnitInSights).TotalMilliseconds<=Bot.Settings.AfterCombatDelay&&DateTime.Now.Subtract(Bot.Targeting.lastHadEliteUnitInSights).TotalMilliseconds<=10000||
								//Cut the delay time in half for non-elite monsters!
							  DateTime.Now.Subtract(Bot.Targeting.lastHadUnitInSights).TotalMilliseconds<=Bot.Settings.AfterCombatDelay)
						  {
								obj=new CacheObject(Bot.Character.Position, TargetType.NoMovement, 20000, "WaitForLootDrops", 2f, -1);
								return true;

						  }
						  //Herbfunks wait after loot containers are opened. 3s for rare chests, half the settings delay for everything else.
						  if ((DateTime.Now.Subtract(Bot.Targeting.lastHadRareChestAsTarget).TotalMilliseconds<=3750)||
							  (DateTime.Now.Subtract(Bot.Targeting.lastHadContainerAsTarget).TotalMilliseconds<=(Bot.Settings.AfterCombatDelay*1.25)))
						  {
								obj=new CacheObject(Bot.Character.Position, TargetType.NoMovement, 20000, "ContainerLootDropsWait", 2f, -1);
								return true;
						  }
						  
						  // Finally, a special check for waiting for wrath of the berserker cooldown before engaging Azmodan
						  if (Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_WrathOfTheBerserker)&&Bot.Settings.Class.bWaitForWrath&&!Bot.Class.Abilities[SNOPower.Barbarian_WrathOfTheBerserker].AbilityUseTimer()&&
							  Bot.Character.iCurrentWorldID==121214&&
							  (Vector3.Distance(Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for Wrath Of The Berserker cooldown before continuing to Azmodan.");
								obj=new CacheObject(Bot.Character.Position, TargetType.NoMovement, 20000, "GilesWaitForWrath", 0f, -1);
								InactivityDetector.Reset();
								return true;
						  }
						  // And a special check for wizard archon
						  if (Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_Archon)&&!Bot.Class.Abilities[SNOPower.Wizard_Archon].AbilityUseTimer()&&Bot.Settings.Class.bWaitForArchon&&ZetaDia.CurrentWorldId==121214&&
							  (Vector3.Distance(Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for Wizard Archon cooldown before continuing to Azmodan.");
								obj=new CacheObject(Bot.Character.Position, TargetType.NoMovement, 20000, "GilesWaitForArchon", 0f, -1);
								InactivityDetector.Reset();
								return true;
						  }
						  // And a very sexy special check for WD BigBadVoodoo
						  if (Bot.Class.HotbarPowers.Contains(SNOPower.Witchdoctor_BigBadVoodoo)&&!PowerManager.CanCast(SNOPower.Witchdoctor_BigBadVoodoo)&&ZetaDia.CurrentWorldId==121214&&
							  (Vector3.Distance(Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for WD BigBadVoodoo cooldown before continuing to Azmodan.");
								obj=new CacheObject(Bot.Character.Position, TargetType.NoMovement, 20000, "GilesWaitForVoodooo", 0f, -1);
								InactivityDetector.Reset();
								return true;
						  }

						  //Check if we engaged in combat..
						  if (Bot.Targeting.LastCachedTarget != ObjectCache.FakeCacheObject)
						  {
							  //Currently preforming an interactive profile behavior
							  if (Bot.Profile.IsRunningOOCBehavior && Bot.Profile.ProfileBehaviorIsOOCInteractive && Bot.Profile.OOCBehaviorStartVector.Distance2D(Bot.Character.Position) > 10f)
							  {
								  if (Bot.Targeting.LastCachedTarget.Position != Bot.Profile.OOCBehaviorStartVector)
									  Navigator.Clear();

								  //Generate the path here so we can start moving..
								  Navigation.NP.MoveTo(Bot.Profile.OOCBehaviorStartVector, "ReturnToOOCLoc", true);

								  //Setup a temp target that the handler will use
								  obj = new CacheObject(Bot.Profile.OOCBehaviorStartVector, TargetType.LineOfSight, 1d, "ReturnToOOCLoc", 10f);
								  return true;
							  }
							  //lets see how far we are from our starting location.
							  else if (Bot.Character.Position.Distance(Bot.Targeting.StartingLocation) > 20f &&
									!Navigation.CanRayCast(Bot.Character.Position, Funky.PlayerMover.vLastMoveTo, UseSearchGridProvider: true))
							  {
								  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
									  Logger.Write(LogLevel.Movement, "Updating Navigator in Target Refresh");

								  SkipAheadCache.ClearCache();
								  Navigator.Clear();
								  Navigator.MoveTo(Funky.PlayerMover.vLastMoveTo, "original destination", true);
							  }
						  }

						  //Check if our current path intersects avoidances. (When not in town, and not currently inside avoidance)
                          if (!Bot.Character.bIsInTown && (Bot.Settings.Avoidance.AttemptAvoidanceMovements || Bot.Character.CriticalAvoidance)
                                  && Navigation.NP.CurrentPath.Count > 0
                                  && Bot.Combat.TriggeringAvoidances.Count == 0)
                          {
                              //Vector3 curpos=Bot.Character.Position;
                              //IndexedList<Vector3> curpath=Navigation.NP.CurrentPath;

                              //var CurrentNearbyPath=curpath.Where(v => curpos.Distance(v)<=40f);
                              //if (CurrentNearbyPath!=null&&CurrentNearbyPath.Any())
                              //{
                              //Vector3 lastV3=Vector3.Zero;
                              //foreach (var item in CurrentNearbyPath.OrderBy(v => curpath.IndexOf(v)))
                              //{
                              //if (lastV3==Vector3.Zero)
                              //lastV3=curpos;

                              if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(Bot.Character.Position, Navigation.NP.CurrentPath.Current))
                              {
                                  obj = new CacheObject(Bot.Character.Position, TargetType.NoMovement, 20000, "AvoidanceIntersection", 2.5f, -1);
                                  return true;
                              }

                              //lastV3=item;
                              //}
                              //}
                          }
					 }

					 return obj!=null;
				};
		  }
	 }
}
