using System;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using Zeta.Common;
using Zeta.Internals.Actors;

namespace FunkyBot.Targeting.Behaviors
{
	 public class TBFleeing : TargetBehavior
	 {
         private DateTime FleeRetryDate = DateTime.Today;
		 private int iSecondsFleeMoveFor = 0;

		  public TBFleeing() : base() { }

		  public override bool BehavioralCondition
		  {
				get
				{
					 return 
                         Bot.Settings.Fleeing.EnableFleeingBehavior&&
                         DateTime.Now.CompareTo(FleeRetryDate)>0&&
                         Bot.Character.dCurrentHealthPct<=Bot.Settings.Fleeing.FleeBotMinimumHealthPercent&&
                         Bot.Targeting.Environment.FleeTriggeringUnits.Count>0&&
                         (!Bot.Targeting.Environment.bAnyTreasureGoblinsPresent||Bot.Settings.Targeting.GoblinPriority<2)&&
                         (Bot.Class.AC!=ActorClass.Wizard||(!Bot.Class.HotBar.HasBuff(SNOPower.Wizard_Archon)||!Bot.Settings.Class.bKiteOnlyArchon));
				}
		  }
		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Fleeing; } }

          public override void Initialize()
          {
              this.Test = (ref CacheObject obj) =>
              {

                  //Resuse last safespot until timer expires!
                  if (DateTime.Now.Subtract(Bot.Targeting.LastFleeAction).TotalSeconds < this.iSecondsFleeMoveFor)
                  {
                      Vector3 reuseV3 = Bot.NavigationCache.AttemptToReuseLastLocationFound();
                      if (reuseV3 != Vector3.Zero)
                      {
                          if (!ObjectCache.Objects.IsPointNearbyMonsters(reuseV3, Bot.Settings.Fleeing.FleeMaxMonsterDistance)
                              &&!ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(reuseV3))
                          {
                              obj = new CacheObject(reuseV3, TargetType.Fleeing, 20000f, "ReuseFleeSpot", 2.5f, -1);
                              return true;

                          }
                      }
                  }

                  Vector3 vAnySafePoint;

                  //Setup Line of Sight for last target if its a unit and still valid..
                  Vector3 LineOfSight = 
                        Bot.Targeting.LastCachedTarget.targetType.HasValue &&
                        Bot.Targeting.LastCachedTarget.targetType.Value == TargetType.Unit &&
                        Bot.Targeting.LastCachedTarget.ObjectIsValidForTargeting ? Bot.Targeting.LastCachedTarget.Position 
                                                                                 : Vector3.Zero;

                  if (Bot.NavigationCache.AttemptFindSafeSpot(out vAnySafePoint, LineOfSight, Bot.Settings.Plugin.FleeingFlags))
                  {
                      float distance = vAnySafePoint.Distance(Bot.Character.Position);

                      if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
                        Logging.WriteDiagnostic("Flee Movement found AT {0} with {1} Distance", vAnySafePoint.ToString(), distance.ToString());
                      

                      obj = new CacheObject(vAnySafePoint, TargetType.Fleeing, 20000f, "FleeSpot", 2.5f, -1);
					  this.iSecondsFleeMoveFor = 1 + (int)(distance / 5f);
                      return true;
                  }
                  else
                  {//Failed to find any location..

                      //Set the future date we must wait for to retry..
                      FleeRetryDate = DateTime.Now.AddMilliseconds(Bot.Settings.Fleeing.FailureRetryMilliseconds);

                  }

                  return false;
              };
          }
	 }
}
