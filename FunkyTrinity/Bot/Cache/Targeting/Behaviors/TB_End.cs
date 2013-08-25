using FunkyTrinity.Cache;
namespace FunkyTrinity.Targeting.Behaviors
{
	 public class TB_End : TargetBehavior
	 {
		  public TB_End() : base() { }
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
