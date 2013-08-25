using System;
using FunkyTrinity.Cache;
using FunkyTrinity.Enums;
using Zeta.Common;

namespace FunkyTrinity
{
	 public class TLA_Avoidance : TargetLogicAction
	 {
		  public override TargetActions TargetActionType { get { return TargetActions.Avoidance; } }

		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				 {
					  if (FunkyTrinity.Bot.Combat.RequiresAvoidance&&(!FunkyTrinity.Bot.Combat.bAnyTreasureGoblinsPresent||FunkyTrinity.Bot.SettingsFunky.GoblinPriority<2)
							&&(DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.timeCancelledEmergencyMove).TotalMilliseconds>FunkyTrinity.Bot.Combat.iMillisecondsCancelledEmergencyMoveFor))
					  {
							//Reuse the last generated safe spot...
							if (DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.LastAvoidanceMovement).TotalMilliseconds>=
									FunkyTrinity.Bot.Combat.iSecondsEmergencyMoveFor)
							{
								 Vector3 reuseV3=FunkyTrinity.Bot.NavigationCache.AttemptToReuseLastLocationFound();
								 if (reuseV3!=Vector3.Zero)
								 {
									  obj=new CacheObject(reuseV3, TargetType.Avoidance, 20000f, "SafeAvoid", 2.5f, -1);
									  return true;
								 }
							}

							Vector3 vAnySafePoint;
							Vector3 LOS=Vector3.Zero;
							if (FunkyTrinity.Bot.NavigationCache.AttemptFindSafeSpot(out vAnySafePoint, LOS, FunkyTrinity.Bot.Character.ShouldFlee))
							{
								 float distance=vAnySafePoint.Distance(FunkyTrinity.Bot.Character.Position);

								 float losdistance=0f;
								 if (LOS!=Vector3.Zero) losdistance=LOS.Distance(FunkyTrinity.Bot.Character.Position);

								 string los=LOS!=Vector3.Zero?("\r\n using LOS location "+LOS.ToString()+" distance "+losdistance.ToString()):" ";

								 Logging.WriteDiagnostic("Avoid Movement found AT {0} with {1} Distance", vAnySafePoint.ToString(), distance);
								 //bFoundSafeSpot = true;

								 //setup avoidance target
								 if (obj!=null) FunkyTrinity.Bot.Combat.LastCachedTarget=obj.Clone();
								 obj=new CacheObject(vAnySafePoint, TargetType.Avoidance, 20000f, "SafeAvoid", 2.5f, -1);

								 //Estimate time we will be reusing this movement vector3
								 FunkyTrinity.Bot.Combat.iSecondsEmergencyMoveFor=1+(int)(distance/25f);

								 //Avoidance takes priority over kiting..
								 FunkyTrinity.Bot.Combat.timeCancelledFleeMove=DateTime.Now;
								 FunkyTrinity.Bot.Combat.iMillisecondsCancelledFleeMoveFor=((FunkyTrinity.Bot.Combat.iSecondsEmergencyMoveFor+1)*1000);
								 return true;
							}
							FunkyTrinity.Bot.UpdateAvoidKiteRates();
					  }

					  return false;
				 };
		  }
	 }
}
