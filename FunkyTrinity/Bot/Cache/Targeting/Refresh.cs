using System;
using System.Linq;
using FunkyTrinity.ability;
using FunkyTrinity.Cache;
using FunkyTrinity.Enums;
using FunkyTrinity.Movement;
using FunkyTrinity.Movement.Clustering;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using FunkyTrinity.Targeting.Behaviors;

namespace FunkyTrinity.Targeting
{
	 public partial class TargetHandler
	 {
		  //Order is important! -- we test from start to finish.
		  internal readonly TargetBehavior[] TargetBehaviors=new TargetBehavior[]
		  {
			  new TB_Refresh(), 
			  new TB_GroupingResume(), 
			  new TB_Avoidance(), 
			  new TB_Fleeing(), 
			  new TB_UpdateTarget(), 
			  new TB_Grouping(), 
			  new TB_End(),
		  };

		 ///<summary>
		  ///Update our current object data ("Current Target")
		  ///</summary>
		  public bool UpdateTarget()
		  {
				bool conditionTest=false;
				TargetBehavioralTypes lastBehavioralType=TargetBehavioralTypes.None;
				foreach (var TLA in TargetBehaviors)
				{
					 if (!TLA.BehavioralCondition) continue;

					 conditionTest=TLA.Test.Invoke(ref CurrentTarget);
					 if (conditionTest)
					 {
						  lastBehavioralType=TLA.TargetBehavioralTypeType;
						  break;
					 }
				}

				return conditionTest;
		  }



	 }
}
