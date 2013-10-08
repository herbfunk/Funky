using System;

namespace FunkyBot.Targeting.Behaviors
{
	 public class TBGroupingResume : TargetBehavior
	 {
		  public TBGroupingResume() : base() { }


		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Grouping; } }
		  public override bool BehavioralCondition
		  {
				get
				{
					 return (Bot.NavigationCache.groupRunningBehavior);
				}
		  }
		  public override void Initialize()
		  {
				base.Test=(ref Cache.CacheObject obj) =>
				{
					 if (!Bot.NavigationCache.groupReturningToOrgin)
					 {
						  Bot.Combat.UpdateGroupClusteringVariables();

						  bool EndBehavior=false;
						  if (!Bot.NavigationCache.groupingCurrentUnit.ObjectIsValidForTargeting)
						  {
								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
									 Logger.Write(LogLevel.Grouping, "Target is no longer valid. Starting return to Orgin.");

								EndBehavior=true;
						  }
						  else if (Bot.NavigationCache.groupingCurrentUnit.CurrentHealthPct.Value<1d
								&&Bot.NavigationCache.groupingCurrentUnit.IsMoving)
						  {
								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
									 Logger.Write(LogLevel.Grouping, "Target has been engaged. Starting return to Orgin.");

								EndBehavior=true;
						  }

						  if (!EndBehavior)
						  {
								obj=Bot.NavigationCache.groupingCurrentUnit;
						  }
						  else
						  {
								Bot.NavigationCache.groupingCurrentUnit=null;
								Bot.NavigationCache.groupReturningToOrgin=true;
								obj=Bot.NavigationCache.groupingOrginUnit;
						  }
						  return true;

					 }
					 else
					 {
						  bool endBehavior=false;

						  //Returning to Orgin Unit..
						  if (!Bot.NavigationCache.groupingOrginUnit.ObjectIsValidForTargeting)
						  {
								endBehavior=true;

								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
									 Logger.Write(LogLevel.Grouping, "Orgin Target is no longer valid for targeting.");
						  }
						  else if (Bot.NavigationCache.groupingOrginUnit.CentreDistance<(Bot.Class.IsMeleeClass?25f:45f))
						  {
								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
									 Logger.Write(LogLevel.Grouping, "Orgin Target is within {0}f of the bot.", (Bot.Class.IsMeleeClass?25f:45f).ToString());

								endBehavior=true;
						  }

						  if (!endBehavior)
						  {
								obj=Bot.NavigationCache.groupingOrginUnit;
								return true;
						  }
						  else
								Bot.NavigationCache.GroupingFinishBehavior();
					 }



					 return false;
				};
		  }
	 }
}
