using System;
using FunkyTrinity.Cache;
using FunkyTrinity.Enums;
using Zeta.Common;

namespace FunkyTrinity.Targeting.Behaviors
{
	 public class TBAvoidance : TargetBehavior
	 {
		  public TBAvoidance() : base() { }

		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Avoidance; } }
		  public override bool BehavioralCondition
		  {
				get
				{
					 return (FunkyTrinity.Bot.Combat.RequiresAvoidance&&(!FunkyTrinity.Bot.Combat.bAnyTreasureGoblinsPresent||FunkyTrinity.Bot.SettingsFunky.Targeting.GoblinPriority<2)
							&&(DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.timeCancelledEmergencyMove).TotalMilliseconds>FunkyTrinity.Bot.Combat.iMillisecondsCancelledEmergencyMoveFor));
				}
		  }
		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				 {
					  if (Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
					  {
							string avoidances="";
							Bot.Combat.TriggeringAvoidances.ForEach(a => avoidances = avoidances + a.AvoidanceType.ToString() + ", ");
							Logger.Write(LogLevel.Movement, "Avoidances Triggering: {0}", avoidances);
					  }
					  //Reuse the last generated safe spot...
					  if (DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.LastAvoidanceMovement).TotalMilliseconds<FunkyTrinity.Bot.Combat.iSecondsEmergencyMoveFor)
					  {
							Vector3 reuseV3=FunkyTrinity.Bot.NavigationCache.AttemptToReuseLastLocationFound();
							if (reuseV3!=Vector3.Zero)
							{
								 obj=new CacheObject(reuseV3, TargetType.Avoidance, 20000f, "SafeReuseAvoid", 2.5f, -1);
								 return true;
							}
					  }

					  Vector3 vAnySafePoint;
					  if (FunkyTrinity.Bot.NavigationCache.AttemptFindSafeSpot(out vAnySafePoint, Vector3.Zero, FunkyTrinity.Bot.Character.ShouldFlee))
					  {
							float distance=vAnySafePoint.Distance(FunkyTrinity.Bot.Character.Position);

							Logging.WriteDiagnostic("Avoid Movement found AT {0} with {1} Distance", vAnySafePoint.ToString(), distance);

							//setup avoidance target
							obj=new CacheObject(vAnySafePoint, TargetType.Avoidance, 20000f, "SafeAvoid", 2.5f, -1);

							//Estimate time we will be reusing this movement vector3
							FunkyTrinity.Bot.Combat.iSecondsEmergencyMoveFor=1+(int)(distance/5f);

							//Avoidance takes priority over kiting..
							FunkyTrinity.Bot.Combat.timeCancelledFleeMove=DateTime.Now;
							FunkyTrinity.Bot.Combat.iMillisecondsCancelledFleeMoveFor=((FunkyTrinity.Bot.Combat.iSecondsEmergencyMoveFor+1)*1000);
							return true;
					  }
					  FunkyTrinity.Bot.UpdateAvoidKiteRates();


					  return false;
				 };
		  }
	 }
}
