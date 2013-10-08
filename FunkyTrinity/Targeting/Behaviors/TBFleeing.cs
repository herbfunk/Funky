using System;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using Zeta.Common;
using Zeta.Internals.Actors;

namespace FunkyBot.Targeting.Behaviors
{
	 public class TBFleeing : TargetBehavior
	 {
		  public TBFleeing() : base() { }

		  public override bool BehavioralCondition
		  {
				get
				{
					 return Bot.Settings.Fleeing.EnableFleeingBehavior;
				}
		  }
		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Fleeing; } }

		  public override void Initialize()
		  {
				this.Test=(ref CacheObject obj) =>
				{
					 if (Bot.Settings.Fleeing.EnableFleeingBehavior&&Bot.Character.dCurrentHealthPct<=Bot.Settings.Fleeing.FleeBotMinimumHealthPercent&&Bot.Combat.FleeTriggeringUnits.Count>0
								&&(DateTime.Now.Subtract(Bot.Combat.timeCancelledFleeMove).TotalMilliseconds>Bot.Combat.iMillisecondsCancelledFleeMoveFor)
								&&(!Bot.Combat.bAnyTreasureGoblinsPresent||Bot.Settings.Targeting.GoblinPriority<2)
								&&(Bot.Class.AC!=ActorClass.Wizard||(Bot.Class.AC==ActorClass.Wizard&&(!Bot.Class.HasBuff(SNOPower.Wizard_Archon)||!Bot.Settings.Class.bKiteOnlyArchon))))
					 {
						  //Resuse last safespot until timer expires!
						  if (DateTime.Now.Subtract(Bot.Targeting.LastFleeAction).TotalSeconds<Bot.Combat.iSecondsFleeMoveFor)
						  {
								Vector3 reuseV3=Bot.NavigationCache.AttemptToReuseLastLocationFound();
								if (reuseV3!=Vector3.Zero)
								{
									 obj=new CacheObject(reuseV3, TargetType.Avoidance, 20000f, "FleeSpot", 2.5f, -1);
									 return true;
								}
						  }

						  Vector3 vAnySafePoint;
						  if (Bot.NavigationCache.AttemptFindSafeSpot(out vAnySafePoint, Vector3.Zero, true))
						  {

								Logging.WriteDiagnostic("Flee Movement found AT {0} with {1} Distance", vAnySafePoint.ToString(), vAnySafePoint.Distance(Bot.Character.Position));
								Bot.Combat.IsFleeing=true;

								//if (obj!=null)FunkyTrinity.Bot.Character.LastCachedTarget=obj;

								obj=new CacheObject(vAnySafePoint, TargetType.Avoidance, 20000f, "FleeSpot", 2.5f, -1);
								Bot.Combat.iSecondsFleeMoveFor=1+(int)(Vector3.Distance(Bot.Character.Position, vAnySafePoint)/25f);
								return true;
						  }
						  Avoidances.AvoidanceCache.UpdateAvoidKiteRates();

					 }

					 ////If we have a cached kite target.. and no current target, lets swap back!
					 //if (FunkyTrinity.Bot.Combat.FleeingLastTarget&&obj==null
					 //			 &&FunkyTrinity.Bot.Character.LastCachedTarget!=null
					 //			 &&ObjectCache.Objects.ContainsKey(FunkyTrinity.Bot.Character.LastCachedTarget.RAGUID))
					 //{
					 //	 //Swap back to our orginal "kite" target
					 //	 obj=ObjectCache.Objects[FunkyTrinity.Bot.Character.LastCachedTarget.RAGUID];
					 //	 Logging.WriteVerbose("Swapping back to unit {0} after fleeing", obj.InternalName);
					 //	 return true;

					 //}

					 return false;
				};
		  }
	 }
}
