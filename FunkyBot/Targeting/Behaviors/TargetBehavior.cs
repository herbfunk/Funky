using FunkyBot.Cache;
using FunkyBot.Cache.Objects;

namespace FunkyBot.Targeting.Behaviors
{
	 /// <summary>
	 /// Contains delegate that passes ref of current target, and describes which test was conducted.
	 /// </summary>
	public abstract class TargetBehavior
	{
		 public virtual TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.None; } }
		 public virtual bool BehavioralCondition { get { return true; } }

		 internal delegate bool TargetTests(ref CacheObject obj);
		 internal TargetTests Test { get; set; }

		 public TargetBehavior() { Initialize(); }
		 public virtual void Initialize(){}


	}
}
