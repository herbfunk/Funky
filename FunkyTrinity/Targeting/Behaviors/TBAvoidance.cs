using System;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using Zeta.Common;

namespace FunkyBot.Targeting.Behaviors
{
	 public class TBAvoidance : TargetBehavior
	 {
		  public TBAvoidance() : base() { }

		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Avoidance; } }
		  public override bool BehavioralCondition
		  {
				get
				{
					 return (Bot.Targeting.RequiresAvoidance&&(!Bot.Combat.bAnyTreasureGoblinsPresent||Bot.Settings.Targeting.GoblinPriority<2)
							&&(DateTime.Now.Subtract(Bot.Combat.timeCancelledEmergencyMove).TotalMilliseconds>Bot.Combat.iMillisecondsCancelledEmergencyMoveFor));
				}
		  }
		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				 {
					  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
					  {
							string avoidances="";
							Bot.Combat.TriggeringAvoidances.ForEach(a => avoidances = avoidances + a.AvoidanceType.ToString() + ", ");
							if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
								Logger.Write(LogLevel.Movement, "Avoidances Triggering: {0}", avoidances);
					  }
					  //Reuse the last generated safe spot...
					  if (DateTime.Now.Subtract(Bot.Targeting.LastAvoidanceMovement).TotalMilliseconds<Bot.Combat.iSecondsEmergencyMoveFor)
					  {
							Vector3 reuseV3=Bot.NavigationCache.AttemptToReuseLastLocationFound();
							if (reuseV3!=Vector3.Zero)
							{
								 obj=new CacheObject(reuseV3, TargetType.Avoidance, 20000f, "SafeReuseAvoid", 2.5f, -1);
								 return true;
							}
					  }

					  Vector3 vAnySafePoint;
					  if (Bot.NavigationCache.AttemptFindSafeSpot(out vAnySafePoint, Vector3.Zero, Bot.Character.ShouldFlee))
					  {
							float distance=vAnySafePoint.Distance(Bot.Character.Position);

							Logging.WriteDiagnostic("Avoid Movement found AT {0} with {1} Distance", vAnySafePoint.ToString(), distance);

							//setup avoidance target
							obj=new CacheObject(vAnySafePoint, TargetType.Avoidance, 20000f, "SafeAvoid", 2.5f, -1);

							//Estimate time we will be reusing this movement vector3
							Bot.Combat.iSecondsEmergencyMoveFor=1+(int)(distance/5f);

							//Avoidance takes priority over kiting..
							Bot.Combat.timeCancelledFleeMove=DateTime.Now;
							Bot.Combat.iMillisecondsCancelledFleeMoveFor=((Bot.Combat.iSecondsEmergencyMoveFor+1)*1000);
							return true;
					  }
					  Avoidances.AvoidanceCache.UpdateAvoidKiteRates();


					  return false;
				 };
		  }
	 }
}
