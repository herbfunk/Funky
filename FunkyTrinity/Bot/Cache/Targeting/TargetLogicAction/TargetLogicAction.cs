using System;
using FunkyTrinity.Cache;

namespace FunkyTrinity
{
	 /// <summary>
	 /// Contains delegate that passes ref of current target, and describes which test was conducted.
	 /// </summary>
	public class TargetLogicAction
	{
		 public virtual TargetActions TargetActionType { get { return TargetActions.None; } }

		 internal delegate bool TargetTests(ref CacheObject obj);
		 internal TargetTests Test { get; set; }

		 public TargetLogicAction() { Initialize(); }
		 public virtual void Initialize(){}
	}
}
