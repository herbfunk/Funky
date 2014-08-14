using System;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Navigation.Gridpoint;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.Targeting.Behaviors
{
	public class TBFleeing : TargetBehavior
	{
		private DateTime FleeRetryDate = DateTime.Today;
		private int iSecondsFleeMoveFor = 0;

		public TBFleeing() : base() { }

		public override bool BehavioralCondition
		{
			get
			{
				return
					FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior &&
					DateTime.Now.CompareTo(FleeRetryDate) > 0 &&
					FunkyGame.Hero.dCurrentHealthPct <= FunkyBaseExtension.Settings.Fleeing.FleeBotMinimumHealthPercent &&
					FunkyGame.Targeting.Cache.Environment.FleeTriggeringUnits.Count > 0 &&
					(!FunkyGame.Targeting.Cache.Environment.bAnyTreasureGoblinsPresent || FunkyBaseExtension.Settings.Targeting.GoblinPriority < 2) &&
					(FunkyGame.Hero.Class.AC != ActorClass.Wizard || (!Hotbar.HasBuff(SNOPower.Wizard_Archon) || !FunkyBaseExtension.Settings.Wizard.bKiteOnlyArchon));
			}
		}
		public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Fleeing; } }

		public override void Initialize()
		{
			this.Test = (ref CacheObject obj) =>
			{

				//Resuse last safespot until timer expires!
				if (DateTime.Now.Subtract(FunkyGame.Targeting.Cache.LastFleeAction).TotalSeconds < this.iSecondsFleeMoveFor)
				{
					Vector3 reuseV3 = FunkyGame.Navigation.AttemptToReuseLastLocationFound();
					if (reuseV3 != Vector3.Zero)
					{
						if (!ObjectCache.Objects.IsPointNearbyMonsters(reuseV3, FunkyBaseExtension.Settings.Fleeing.FleeMaxMonsterDistance)
							&& !ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(reuseV3))
						{
							obj = new CacheObject(reuseV3, TargetType.Fleeing, 20000f, "ReuseFleeSpot", 2.5f, -1);
							return true;

						}
					}
				}

				Vector3 vAnySafePoint;

				//Setup Line of Sight for last target if its a unit and still valid..
				Vector3 LineOfSight =
					  FunkyGame.Targeting.Cache.LastCachedTarget.targetType.HasValue &&
					  FunkyGame.Targeting.Cache.LastCachedTarget.targetType.Value == TargetType.Unit &&
					  FunkyGame.Targeting.Cache.LastCachedTarget.ObjectIsValidForTargeting ? FunkyGame.Targeting.Cache.LastCachedTarget.Position
																			   : Vector3.Zero;
				PointCheckingFlags flags = FunkyBaseExtension.Settings.Plugin.FleeingFlags;
				if (FunkyGame.Hero.Class.HasCastableMovementAbility())
					flags &= ~(PointCheckingFlags.AvoidanceIntersection | PointCheckingFlags.BlockedDirection);

				if (FunkyGame.Navigation.AttemptFindSafeSpot(out vAnySafePoint, LineOfSight, flags))
				{
					float distance = vAnySafePoint.Distance(FunkyGame.Hero.Position);

					Logger.DBLog.DebugFormat("Flee Movement found AT {0} with {1} Distance", vAnySafePoint.ToString(), distance.ToString());


					obj = new CacheObject(vAnySafePoint, TargetType.Fleeing, 20000f, "FleeSpot", 2.5f, -1);
					this.iSecondsFleeMoveFor = 1 + (int)(distance / 5f);
					return true;
				}
				else
				{//Failed to find any location..

					//Set the future date we must wait for to retry..
					FleeRetryDate = DateTime.Now.AddMilliseconds(FunkyBaseExtension.Settings.Fleeing.FailureRetryMilliseconds);

				}

				return false;
			};
		}

		internal static bool ShouldFlee
		{
			get
			{
				bool flee = FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior && FunkyGame.Hero.dCurrentHealthPct <= FunkyBaseExtension.Settings.Fleeing.FleeBotMinimumHealthPercent;
				return flee;
			}
		}
	}
}
