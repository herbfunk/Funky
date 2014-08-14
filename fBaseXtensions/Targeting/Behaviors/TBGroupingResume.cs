using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Targeting.Behaviors
{
	public class TBGroupingResume : TargetBehavior
	{
		public TBGroupingResume() : base() { }


		public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Grouping; } }
		public override bool BehavioralCondition
		{
			get
			{
				return (FunkyGame.Navigation.groupRunningBehavior);
			}
		}
		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			{
				if (!FunkyGame.Navigation.groupReturningToOrgin)
				{//Not returning to Orgin Target

					FunkyGame.Targeting.Cache.Clusters.UpdateGroupClusteringVariables();

					bool EndBehavior = false;
					if (!FunkyGame.Navigation.groupingCurrentUnit.ObjectIsValidForTargeting)
					{
						Logger.Write(LogLevel.Grouping, "Target is no longer valid. Starting return to Orgin.");

						EndBehavior = true;
					}
					else if (FunkyGame.Navigation.groupingCurrentUnit.CurrentHealthPct.Value < 1d
						  && FunkyGame.Navigation.groupingCurrentUnit.IsMoving)
					{
						Logger.Write(LogLevel.Grouping, "Target has been engaged. Starting return to Orgin.");

						EndBehavior = true;
					}

					if (!EndBehavior)
					{
						obj = FunkyGame.Navigation.groupingCurrentUnit;
					}
					else
					{
						FunkyGame.Navigation.groupingCurrentUnit = null;
						FunkyGame.Navigation.groupReturningToOrgin = true;
						obj = FunkyGame.Navigation.groupingOrginUnit;
					}
					return true;

				}
				else
				{//Returning to Orgin Unit..
					bool endBehavior = false;
					if (!FunkyGame.Navigation.groupingOrginUnit.ObjectIsValidForTargeting)
					{
						endBehavior = true;

						Logger.Write(LogLevel.Grouping, "Orgin Target is no longer valid for targeting.");
					}
					else if (FunkyGame.Navigation.groupingOrginUnit.CentreDistance < (FunkyGame.Hero.Class.IsMeleeClass ? 25f : 45f))
					{
						Logger.Write(LogLevel.Grouping, "Orgin Target is within {0}f of the bot.", (FunkyGame.Hero.Class.IsMeleeClass ? 25f : 45f).ToString());

						endBehavior = true;
					}

					if (!endBehavior)
					{
						obj = FunkyGame.Navigation.groupingOrginUnit;
						return true;
					}
					else
					{
						FunkyGame.Navigation.GroupingFinishBehavior();
					}
				}



				return false;
			};
		}
	}
}
