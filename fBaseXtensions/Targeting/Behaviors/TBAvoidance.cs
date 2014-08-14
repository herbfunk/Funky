using System;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Navigation.Gridpoint;
using Zeta.Common;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Targeting.Behaviors
{
	public class TBAvoidance : TargetBehavior
	{
		private DateTime AvoidRetryDate = DateTime.Today;
		private int iSecondsAvoidMoveFor = 0;
		public TBAvoidance() : base() { }

		public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Avoidance; } }
		public override bool BehavioralCondition
		{
			get
			{
				return
					(FunkyGame.Targeting.Cache.RequiresAvoidance &&
					 DateTime.Now.CompareTo(AvoidRetryDate) > 0 &&
					(!FunkyGame.Targeting.Cache.Environment.bAnyTreasureGoblinsPresent || FunkyBaseExtension.Settings.Targeting.GoblinPriority < 2));
			}
		}
		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			 {
				 if (fBaseXtensions.FunkyBaseExtension.Settings.Logging.LogFlags.HasFlag(LogLevel.Movement))
				 {
					 string avoidances = "";
					 FunkyGame.Targeting.Cache.Environment.TriggeringAvoidances.ForEach(a => avoidances = avoidances + a.AvoidanceType.ToString() + ", ");
					 Logger.Write(LogLevel.Movement, "Avoidances Triggering: {0}", avoidances);
				 }

				 //Reuse the last generated safe spot...
				 if (DateTime.Now.Subtract(FunkyGame.Targeting.Cache.LastAvoidanceMovement).TotalSeconds < this.iSecondsAvoidMoveFor)
				 {
					 Vector3 reuseV3 = FunkyGame.Navigation.AttemptToReuseLastLocationFound();
					 if (reuseV3 != Vector3.Zero)
					 {
						 if (!ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(reuseV3))
						 {
							 obj = new CacheObject(reuseV3, TargetType.Avoidance, 20000f, "SafeReuseAvoid", 2.5f, reuseV3.GetHashCode());
							 return true;
						 }
					 }
				 }

				 Vector3 vAnySafePoint;

				 //Check if we have any movement abilities we can cast.. if so lets not check avoidance intersections.
				 PointCheckingFlags flags = FunkyBaseExtension.Settings.Plugin.AvoidanceFlags;
				 if (FunkyGame.Hero.Class.HasCastableMovementAbility())
					 flags &= ~(PointCheckingFlags.AvoidanceIntersection | PointCheckingFlags.BlockedDirection);

				 Vector3 losVector3=Vector3.Zero;
				 if (FunkyGame.Targeting.Cache.LastCachedTarget.targetType != null && FunkyGame.Targeting.Cache.LastCachedTarget.targetType.Value == TargetType.Unit)
				 {
					 losVector3=FunkyGame.Targeting.Cache.LastCachedTarget.Position;
				 }

				 if (FunkyGame.Navigation.AttemptFindSafeSpot(out vAnySafePoint, losVector3, flags))
				 {
					 float distance = vAnySafePoint.Distance(FunkyGame.Hero.Position);

					 Logger.DBLog.DebugFormat("Avoid Movement found AT {0} with {1} Distance", vAnySafePoint.ToString(), distance);

					 //setup avoidance target
					 obj = new CacheObject(vAnySafePoint, TargetType.Avoidance, 20000f, "SafeAvoid", 2.5f, vAnySafePoint.GetHashCode());

					 //Estimate time we will be reusing this movement vector3
					 this.iSecondsAvoidMoveFor = 1 + (int)(distance / 5f);
					 return true;
				 }
				 else
				 {//Failed to find any location..

					 //Set the future date we must wait for to retry..
					 AvoidRetryDate = DateTime.Now.AddMilliseconds(FunkyBaseExtension.Settings.Avoidance.FailureRetryMilliseconds);
				 }

				 return false;
			 };
		}
	}
}
