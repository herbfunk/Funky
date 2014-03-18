using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using Zeta.Common;
using FunkyBot.Movement;

namespace FunkyBot.Targeting.Behaviors
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
					(Bot.Targeting.Cache.Backtracking && Bot.Targeting.Cache.StartingLocation != Vector3.Zero);
			}
		}
		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			{
				if (Bot.Character.Data.Position.Distance(Bot.Targeting.Cache.StartingLocation) > Bot.Settings.Backtracking.MinimumDistanceFromStart)
				{
					//Generate the path here so we can start moving..
					Navigation.NP.MoveTo(Bot.Targeting.Cache.StartingLocation, "Backtracking", true);

					//Setup a temp target that the handler will use
					obj = new CacheObject(Bot.Targeting.Cache.StartingLocation, TargetType.Backtrack, 20000, "Backtracking", Bot.Settings.Backtracking.MinimumDistanceFromStart);
					return true;
				}

				Bot.Targeting.Cache.Backtracking = false;
				return false;

			};
		}
	}
}
