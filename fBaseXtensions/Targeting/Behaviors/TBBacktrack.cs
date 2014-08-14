using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using Zeta.Common;

namespace fBaseXtensions.Targeting.Behaviors
{
	public class TBBacktrack : TargetBehavior
	{
		public TBBacktrack() : base() { }

		public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Backtrack; } }
		public override bool BehavioralCondition
		{
			get
			{
				return
					(FunkyGame.Targeting.Cache.Backtracking && FunkyGame.Targeting.Cache.StartingLocation != Vector3.Zero);
			}
		}
		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			{
				if (FunkyGame.Hero.Position.Distance(FunkyGame.Targeting.Cache.StartingLocation) > FunkyBaseExtension.Settings.Backtracking.MinimumDistanceFromStart)
				{
					//Generate the path here so we can start moving..
					Navigation.Navigation.NP.MoveTo(FunkyGame.Targeting.Cache.StartingLocation, "Backtracking", true);

					//Setup a temp target that the handler will use
					obj = new CacheObject(FunkyGame.Targeting.Cache.StartingLocation, TargetType.Backtrack, 20000, "Backtracking", FunkyBaseExtension.Settings.Backtracking.MinimumDistanceFromStart);
					return true;
				}

				FunkyGame.Targeting.Cache.Backtracking = false;
				return false;

			};
		}
	}
}
