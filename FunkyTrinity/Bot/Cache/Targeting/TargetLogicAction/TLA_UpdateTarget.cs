using System;

namespace FunkyTrinity
{
	public class TLA_UpdateTarget:TargetLogicAction
	{
		 public override TargetActions TargetActionType
		 {
			  get
			  {
					return TargetActions.Target;
			  }
		 }
		 public override void Initialize()
		 {
			  base.Test=(ref Cache.CacheObject obj) =>
			  {
					FunkyTrinity.Bot.Combat.bStayPutDuringAvoidance=false;

					//cluster update
					FunkyTrinity.Bot.Combat.UpdateTargetClusteringVariables();

					//Standard weighting of valid objects -- updates current target.
					Bot.Target.WeightEvaluationObjList(ref obj);

					return false;
			  };
		 }
	}
}
