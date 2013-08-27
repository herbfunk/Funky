using FunkyTrinity.Cache;
namespace FunkyTrinity.Targeting.Behaviors
{
	 public class TBEnd : TargetBehavior
	 {
		  public TBEnd() : base() { }
		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Target; } }


		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				{
					 return obj!=null;
				};
		  }
	 }
}
