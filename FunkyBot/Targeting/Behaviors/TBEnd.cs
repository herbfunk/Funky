using System;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
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
								obj=new CacheObject(Bot.Character.Data.Position, TargetType.NoMovement, 20000, "WaitForLootDrops", 2f, -1);
								return true;

						  }
						  //Herbfunks wait after loot containers are opened. 3s for rare chests, half the settings delay for everything else.
						  if ((DateTime.Now.Subtract(Bot.Targeting.lastHadRareChestAsTarget).TotalMilliseconds<=3750)||
							  (DateTime.Now.Subtract(Bot.Targeting.lastHadContainerAsTarget).TotalMilliseconds<=(Bot.Settings.AfterCombatDelay*1.25)))
						  {
								obj=new CacheObject(Bot.Character.Data.Position, TargetType.NoMovement, 20000, "ContainerLootDropsWait", 2f, -1);
								return true;
						  }
						  
						  // Finally, a special check for waiting for wrath of the berserker cooldown before engaging Azmodan
						  if (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_WrathOfTheBerserker)&&Bot.Settings.Class.bWaitForWrath&&!Bot.Character.Class.Abilities[SNOPower.Barbarian_WrathOfTheBerserker].AbilityUseTimer()&&
							  Bot.Character.Data.iCurrentWorldID==121214&&
							  (Vector3.Distance(Bot.Character.Data.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Data.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for Wrath Of The Berserker cooldown before continuing to Azmodan.");
								obj=new CacheObject(Bot.Character.Data.Position, TargetType.NoMovement, 20000, "GilesWaitForWrath", 0f, -1);
								InactivityDetector.Reset();
								return true;
						  }
						  // And a special check for wizard archon
						  if (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_Archon)&&!Bot.Character.Class.Abilities[SNOPower.Wizard_Archon].AbilityUseTimer()&&Bot.Settings.Class.bWaitForArchon&&ZetaDia.CurrentWorldId==121214&&
							  (Vector3.Distance(Bot.Character.Data.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Data.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for Wizard Archon cooldown before continuing to Azmodan.");
								obj=new CacheObject(Bot.Character.Data.Position, TargetType.NoMovement, 20000, "GilesWaitForArchon", 0f, -1);
								InactivityDetector.Reset();
								return true;
						  }
						  // And a very sexy special check for WD BigBadVoodoo
						  if (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Witchdoctor_BigBadVoodoo)&&!PowerManager.CanCast(SNOPower.Witchdoctor_BigBadVoodoo)&&ZetaDia.CurrentWorldId==121214&&
							  (Vector3.Distance(Bot.Character.Data.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Data.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for WD BigBadVoodoo cooldown before continuing to Azmodan.");
								obj=new CacheObject(Bot.Character.Data.Position, TargetType.NoMovement, 20000, "GilesWaitForVoodooo", 0f, -1);
								InactivityDetector.Reset();
								return true;
						  }

						  //Currently preforming an interactive profile behavior (check if in town and not vendoring)
						  if (Bot.Game.Profile.PreformingInteractiveBehavior && (!Bot.Character.Data.bIsInTown||!Zeta.CommonBot.Logic.BrainBehavior.IsVendoring))
						  {
							  if (Bot.Game.Profile.InteractableCachedObject.Position.Distance(Bot.Character.Data.Position) > 50f)
							  {
								  if (Bot.Targeting.LastCachedTarget.Position != Bot.Game.Profile.InteractableCachedObject.Position)
									  Navigator.Clear();

								  //Generate the path here so we can start moving..
								  Navigation.NP.MoveTo(Bot.Game.Profile.InteractableCachedObject.Position, "ReturnToOOCLoc", true);

								  //Setup a temp target that the handler will use
								  obj = new CacheObject(Bot.Game.Profile.InteractableCachedObject.Position, TargetType.LineOfSight, 1d, "ReturnToOOCLoc", 10f, Bot.Game.Profile.InteractableCachedObject.RAGUID);
								  return true;
							  }
						  }

						  //Check if we engaged in combat..
						  bool EngagedInCombat = false;
						 float distanceFromStart=0f;
						 if (Bot.Targeting.LastCachedTarget != ObjectCache.FakeCacheObject && !Bot.Targeting.Backtracking && Bot.Targeting.StartingLocation!= Vector3.Zero)
						  {
							  EngagedInCombat = true;
							  distanceFromStart=Bot.Character.Data.Position.Distance(Bot.Targeting.StartingLocation);
							  //lets see how far we are from our starting location.
							  if (distanceFromStart > 20f &&
									!Navigation.CanRayCast(Bot.Character.Data.Position, Funky.PlayerMover.vLastMoveTo, UseSearchGridProvider: true))
							  {
								  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
									  Logger.Write(LogLevel.Movement, "Updating Navigator in Target Refresh");

								  SkipAheadCache.ClearCache();
								  Navigator.Clear();
								  //Navigator.MoveTo(Funky.PlayerMover.vLastMoveTo, "original destination", true);
							  }
						  }

						  //Check if our current path intersects avoidances. (When not in town, and not currently inside avoidance)
                          if (!Bot.Character.Data.bIsInTown && (Bot.Settings.Avoidance.AttemptAvoidanceMovements || Bot.Character.Data.CriticalAvoidance)
                                  && Navigation.NP.CurrentPath.Count > 0
                                  && Bot.Targeting.Environment.TriggeringAvoidances.Count == 0)
                          {
                              if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(Bot.Character.Data.Position, Navigation.NP.CurrentPath.Current))
                              {
                                  obj = new CacheObject(Bot.Character.Data.Position, TargetType.NoMovement, 20000, "AvoidanceIntersection", 2.5f, -1);
                                  return true;
                              }
                          }

						 //Backtracking Check..
						 if(EngagedInCombat && Bot.Settings.Backtracking.EnableBacktracking && distanceFromStart>=Bot.Settings.Backtracking.MinimumDistanceFromStart)
						 {
							 Bot.Targeting.Backtracking = true;
							 obj = new CacheObject(Bot.Targeting.StartingLocation, TargetType.Backtrack, 20000, "Backtracking", 2.5f);
							 return true;
						 }
					 }

					 return obj!=null;
				};
		  }
	 }
}
