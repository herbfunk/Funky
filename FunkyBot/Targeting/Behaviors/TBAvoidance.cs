using System;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Movement;
using FunkyBot.Player.HotBar.Skills;
using Zeta.Common;

namespace FunkyBot.Targeting.Behaviors
{
	 public class TBAvoidance : TargetBehavior
	 {
         private DateTime AvoidRetryDate = DateTime.Today;
		 private int iSecondsAvoidMoveFor = 0;
		  public TBAvoidance() : base() { }

		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Avoidance; } }
		  public override bool BehavioralCondition
		  {
				get
				{
					 return 
                         (Bot.Targeting.RequiresAvoidance&&
                          DateTime.Now.CompareTo(AvoidRetryDate)>0&&
                         (!Bot.Targeting.Environment.bAnyTreasureGoblinsPresent||Bot.Settings.Targeting.GoblinPriority<2));
				}
		  }
		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				 {
					  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
					  {
							string avoidances="";
							Bot.Targeting.Environment.TriggeringAvoidances.ForEach(a => avoidances = avoidances + a.AvoidanceType.ToString() + ", ");
							Logger.Write(LogLevel.Movement, "Avoidances Triggering: {0}", avoidances);
					  }

					  //Reuse the last generated safe spot...
					  if (DateTime.Now.Subtract(Bot.Targeting.LastAvoidanceMovement).TotalSeconds<this.iSecondsAvoidMoveFor)
					  {
							Vector3 reuseV3=Bot.NavigationCache.AttemptToReuseLastLocationFound();
							if (reuseV3!=Vector3.Zero)
							{
                                if (!ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(reuseV3))
                                {
                                    obj = new CacheObject(reuseV3, TargetType.Avoidance, 20000f, "SafeReuseAvoid", 2.5f, -1);
                                    return true;
                                }
							}
					  }

					  Vector3 vAnySafePoint;

					  //Check if we have any movement abilities we can cast.. if so lets not check avoidance intersections.
					  PointCheckingFlags flags=Bot.Settings.Plugin.AvoidanceFlags;
					  if (Bot.Character.Class.HasCastableMovementAbility())
						  flags &= ~(PointCheckingFlags.AvoidanceIntersection|PointCheckingFlags.BlockedDirection);

					  if (Bot.NavigationCache.AttemptFindSafeSpot(out vAnySafePoint, Vector3.Zero, flags))
					  {
							float distance=vAnySafePoint.Distance(Bot.Character.Data.Position);

							Logging.WriteDiagnostic("Avoid Movement found AT {0} with {1} Distance", vAnySafePoint.ToString(), distance);

							//setup avoidance target
							obj=new CacheObject(vAnySafePoint, TargetType.Avoidance, 20000f, "SafeAvoid", 2.5f, -1);

							//Estimate time we will be reusing this movement vector3
							this.iSecondsAvoidMoveFor = 1 + (int)(distance / 5f);
							return true;
					  }
                      else
                      {//Failed to find any location..

                          //Set the future date we must wait for to retry..
                          AvoidRetryDate = DateTime.Now.AddMilliseconds(Bot.Settings.Avoidance.FailureRetryMilliseconds);
                      }

					  return false;
				 };
		  }
	 }
}
