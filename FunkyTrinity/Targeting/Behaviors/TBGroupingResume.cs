using System;

namespace FunkyTrinity.Targeting.Behaviors
{
	 public class TBGroupingResume : TargetBehavior
	 {
		  public TBGroupingResume() : base() { }


		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Grouping; } }
		  public override bool BehavioralCondition
		  {
				get
				{
					 return (FunkyTrinity.Bot.NavigationCache.groupRunningBehavior);
				}
		  }
		  public override void Initialize()
		  {
				base.Test=(ref Cache.CacheObject obj) =>
				{
					 if (!FunkyTrinity.Bot.NavigationCache.groupReturningToOrgin)
					 {
						  FunkyTrinity.Bot.Combat.UpdateGroupClusteringVariables();

						  bool EndBehavior=false;
						  if (!FunkyTrinity.Bot.NavigationCache.groupingCurrentUnit.ObjectIsValidForTargeting)
						  {
								if (FunkyTrinity.Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
									 Logger.Write(LogLevel.Grouping, "Target is no longer valid. Starting return to Orgin.");

								EndBehavior=true;
						  }
						  else if (FunkyTrinity.Bot.NavigationCache.groupingCurrentUnit.CurrentHealthPct.Value<1d
								&&FunkyTrinity.Bot.NavigationCache.groupingCurrentUnit.IsMoving)
						  {
								if (FunkyTrinity.Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
									 Logger.Write(LogLevel.Grouping, "Target has been engaged. Starting return to Orgin.");

								EndBehavior=true;
						  }

						  if (!EndBehavior)
						  {
								obj=FunkyTrinity.Bot.NavigationCache.groupingCurrentUnit;
						  }
						  else
						  {
								FunkyTrinity.Bot.NavigationCache.groupingCurrentUnit=null;
								FunkyTrinity.Bot.NavigationCache.groupReturningToOrgin=true;
								obj=FunkyTrinity.Bot.NavigationCache.groupingOrginUnit;
						  }
						  return true;

					 }
					 else
					 {
						  bool endBehavior=false;

						  //Returning to Orgin Unit..
						  if (!FunkyTrinity.Bot.NavigationCache.groupingOrginUnit.ObjectIsValidForTargeting)
						  {
								endBehavior=true;

								if (FunkyTrinity.Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
									 Logger.Write(LogLevel.Grouping, "Orgin Target is no longer valid for targeting.");
						  }
						  else if (FunkyTrinity.Bot.NavigationCache.groupingOrginUnit.CentreDistance<(FunkyTrinity.Bot.Class.IsMeleeClass?25f:45f))
						  {
								if (FunkyTrinity.Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
									 Logger.Write(LogLevel.Grouping, "Orgin Target is within {0}f of the bot.", (FunkyTrinity.Bot.Class.IsMeleeClass?25f:45f).ToString());

								endBehavior=true;
						  }

						  if (!endBehavior)
						  {
								obj=FunkyTrinity.Bot.NavigationCache.groupingOrginUnit;
								return true;
						  }
						  else
								FunkyTrinity.Bot.NavigationCache.GroupingFinishBehavior();
					 }



					 return false;
				};
		  }
	 }
}
