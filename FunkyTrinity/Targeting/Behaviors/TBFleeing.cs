using System;
using FunkyTrinity.Cache;
using FunkyTrinity.Cache.Enums;
using Zeta.Common;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Targeting.Behaviors
{
	 public class TBFleeing : TargetBehavior
	 {
		  public TBFleeing() : base() { }

		  public override bool BehavioralCondition
		  {
				get
				{
					 return Bot.SettingsFunky.Fleeing.EnableFleeingBehavior;
				}
		  }
		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Fleeing; } }

		  public override void Initialize()
		  {
				this.Test=(ref CacheObject obj) =>
				{
					 if (FunkyTrinity.Bot.SettingsFunky.Fleeing.EnableFleeingBehavior&&FunkyTrinity.Bot.Character.dCurrentHealthPct<=FunkyTrinity.Bot.SettingsFunky.Fleeing.FleeBotMinimumHealthPercent&&FunkyTrinity.Bot.Combat.FleeTriggeringUnits.Count>0
								&&(DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.timeCancelledFleeMove).TotalMilliseconds>FunkyTrinity.Bot.Combat.iMillisecondsCancelledFleeMoveFor)
								&&(!FunkyTrinity.Bot.Combat.bAnyTreasureGoblinsPresent||FunkyTrinity.Bot.SettingsFunky.Targeting.GoblinPriority<2)
								&&(FunkyTrinity.Bot.Class.AC!=ActorClass.Wizard||(FunkyTrinity.Bot.Class.AC==ActorClass.Wizard&&(!FunkyTrinity.Bot.Class.HasBuff(SNOPower.Wizard_Archon)||!FunkyTrinity.Bot.SettingsFunky.Class.bKiteOnlyArchon))))
					 {
						  //Resuse last safespot until timer expires!
						  if (DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.LastFleeAction).TotalSeconds<FunkyTrinity.Bot.Combat.iSecondsFleeMoveFor)
						  {
								Vector3 reuseV3=FunkyTrinity.Bot.NavigationCache.AttemptToReuseLastLocationFound();
								if (reuseV3!=Vector3.Zero)
								{
									 obj=new CacheObject(reuseV3, TargetType.Avoidance, 20000f, "FleeSpot", 2.5f, -1);
									 return true;
								}
						  }

						  Vector3 vAnySafePoint;
						  if (FunkyTrinity.Bot.NavigationCache.AttemptFindSafeSpot(out vAnySafePoint, Vector3.Zero, true))
						  {

								Logging.WriteDiagnostic("Flee Movement found AT {0} with {1} Distance", vAnySafePoint.ToString(), vAnySafePoint.Distance(FunkyTrinity.Bot.Character.Position));
								FunkyTrinity.Bot.Combat.IsFleeing=true;

								//if (obj!=null)FunkyTrinity.Bot.Character.LastCachedTarget=obj;

								obj=new CacheObject(vAnySafePoint, TargetType.Avoidance, 20000f, "FleeSpot", 2.5f, -1);
								FunkyTrinity.Bot.Combat.iSecondsFleeMoveFor=1+(int)(Vector3.Distance(FunkyTrinity.Bot.Character.Position, vAnySafePoint)/25f);
								return true;
						  }
						  FunkyTrinity.Bot.UpdateAvoidKiteRates();

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
