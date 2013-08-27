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
			  new TBGroupingResume(), 
			  new TBAvoidance(), 
			  new TBFleeing(), 
			  new TBUpdateTarget(), 
			  new TBGrouping(), 
			  new TBEnd(),
		  };

		  internal TargetBehavioralTypes lastBehavioralType=TargetBehavioralTypes.None;
		  internal class TargetChangedArgs : EventArgs
		  {
				public CacheObject newObject { get; set; }
				public TargetBehavioralTypes targetBehaviorUsed { get; set; }

				public TargetChangedArgs(CacheObject newobj, TargetBehavioralTypes sendingtype)
				{
					 newObject=newobj;
					 targetBehaviorUsed=sendingtype;
				}
		  }
		  internal delegate void TargetChangeHandler(object cacheobj, TargetChangedArgs timeInformation);

		  internal TargetChangeHandler TargetChanged;

		 ///<summary>
		  ///Update our current object data ("Current Target")
		  ///</summary>
		  public bool UpdateTarget()
		  {
				bool conditionTest=false;
				lastBehavioralType=TargetBehavioralTypes.None;
				foreach (var TLA in TargetBehaviors)
				{
					 if (!TLA.BehavioralCondition) continue;

					 conditionTest=TLA.Test.Invoke(ref CurrentTarget);
					 if (conditionTest)
					 {
						  if (!Bot.Character.LastCachedTarget.Equals(CurrentTarget))
						  {
								if (Bot.SettingsFunky.FunkyLogFlags.HasFlag(LogLevel.Target))
									 Logger.Write(LogLevel.Target, "Changed Object: {0}", MakeStringSingleLine(CurrentTarget.DebugString));

								TargetChangedArgs TargetChangedInfo=
									 new TargetChangedArgs(CurrentTarget, lastBehavioralType);

								// if anyone has subscribed, notify them
								if (TargetChanged!=null)
								{
									 TargetChanged(CurrentTarget, TargetChangedInfo);
								}
						  }

						  lastBehavioralType=TLA.TargetBehavioralTypeType;
						  break;
					 }
				}


					
				return conditionTest;
		  }
		  private Char CHARnewLine='\x000A';
		  private Char CHARspace='\x0009';
		  private string MakeStringSingleLine(string str)
		  {
				return str.Replace(CHARnewLine, CHARspace);
		  }


	 }
}
