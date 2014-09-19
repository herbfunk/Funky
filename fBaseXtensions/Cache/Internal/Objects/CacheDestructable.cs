using System;
using System.Linq;
using fBaseXtensions.Cache.Internal.Blacklist;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Game.Hero.Skills;
using fBaseXtensions.Navigation;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Cache.Internal.Objects
{
	public class CacheDestructable : CacheGizmo
	{

		public CacheDestructable(CacheObject baseobj)
			: base(baseobj)
		{
		}

		private bool? lastIntersectionTestResult = null;

		public override int InteractionRange
		{
			get
			{
				return FunkyBaseExtension.Settings.Ranges.DestructibleRange + 1;
			}
		}

		public override bool ObjectIsValidForTargeting
		{
			get
			{
				if (!base.ObjectIsValidForTargeting) return false;

				float radiusDistance = RadiusDistance;

				//Ignore Destructibles that are not nearby..
				if (CentreDistance>40f && radiusDistance>InteractionRange)
				{
					IgnoredType = TargetingIgnoreTypes.DistanceFailure;
					BlacklistLoops = 10;
					return false;
				}

				////Get current animation state! (Idle = Untouched, Dead = Destroyed)
				UpdateAnimationState();
				AnimationState currentAnimState = AnimState;
				if (currentAnimState != AnimationState.Idle)
				{
					NeedsRemoved = true;
					BlacklistFlag = BlacklistType.Permanent;
					//Logger.Write(LogLevel.Cache, "Removing destructible {0} due to invalid AnimationState of {1} -- SNOAnim {2}", InternalName, currentAnimState.ToString(), SnoAnim.ToString());
					return false;
				}


				if (RequiresLOSCheck && !IgnoresLOSCheck)
				{
					//Preform Test every 2500ms on normal objects, 1250ms on special objects.
					double lastLOSCheckMS = base.LineOfSight.LastLOSCheckMS;
					if ((lastLOSCheckMS < 1250) || (lastLOSCheckMS < 2500 && CentreDistance > 20f))
					{
						IgnoredType = TargetingIgnoreTypes.LineOfSightFailure;
						return false;
					}

					if (!base.LineOfSight.LOSTest(FunkyGame.Hero.Position, true, ServerObjectIntersection: false) && (PlayerMover.iTotalAntiStuckAttempts == 0 || RadiusDistance > 12f))
					{
						IgnoredType = TargetingIgnoreTypes.LineOfSightFailure;
						return false;
					}

					RequiresLOSCheck = false;
				}

				//Ignore Destructibles Setting
				if (!CharacterSettings.Instance.DestroyEnvironment
					&& Gizmotype.Value == GizmoType.DestroyableObject
					&& PriorityCounter == 0
					&& !IsBarricade.Value)
				{
					IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
					//ignore from being a targeted right now..
					BlacklistLoops = 15;
					//We dont want to complete ignore this since the object may pose a navigational obstacle.
					return false;
				}



				
				//Barricade and path intersects the actorsphere radius..
				//Some barricades may be lower than ourself, or our destination is high enough to raycast past the object. So we add a little to the Z of the obstacle.
				//The best method would be to get the hight of the object and compare it to our current Z-height if we are nearly within radius distance of the object.
				if ((targetType.Value == TargetType.Barricade || IsBarricade.HasValue && IsBarricade.Value) &&
					(!PlayerMover.ShouldHandleObstacleObject  //Have special flag from unstucker to destroy nearby barricade.
					 && PriorityCounter == 0
					 && radiusDistance > 0f))
				{
					Vector3 BarricadeTest = Position;
					if (radiusDistance > 1f)
					{
						if (radiusDistance < 10f)
						{
							BarricadeTest = MathEx.GetPointAt(Position, 10f, Navigation.Navigation.FindDirection(FunkyGame.Hero.Position, Position, true));
							BarricadeTest.Z = Navigation.Navigation.MGP.GetHeight(BarricadeTest.ToVector2());

							lastIntersectionTestResult = MathEx.IntersectsPath(Position, CollisionRadius.Value, FunkyGame.Hero.Position, BarricadeTest);
							if (!lastIntersectionTestResult.Value)
							{
								IgnoredType = TargetingIgnoreTypes.IntersectionFailed;
								return false;
							}
						}
					}
				}


				if (radiusDistance > InteractionRange && (!QuestMonster || radiusDistance > 100f))
				{
					IgnoredType = TargetingIgnoreTypes.DistanceFailure;
					return false;
				}

				return true;
			}
		}

		public override void UpdateWeight()
		{
			base.UpdateWeight();

			float centreDistance = CentreDistance;
			Weight = 12000d - (Math.Floor(centreDistance) * 175d);
			// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
			if (Equals(FunkyGame.Targeting.Cache.LastCachedTarget) && centreDistance <= 25f)
				Weight += 400;
			// Close destructibles get a weight increase
			if (centreDistance <= 16f)
				Weight += 1500d;
			Vector3 BotPosition = FunkyGame.Hero.Position;
			// If there's a monster in the path-line to the item, reduce the weight by 50%
			if (RadiusDistance > 0f && !Equipment.NoMonsterCollision && ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
				Weight *= 0.5;
			// Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
			if (FunkyGame.Hero.bIsRooted)
				Weight = 19200d - (Math.Floor(centreDistance) * 200d);
			// Very close destructibles get a final weight increase
			if (centreDistance <= 12f)
				Weight += 2000d;

		}

		public override bool IsZDifferenceValid
		{
			get
			{
				float fThisHeightDifference = FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, Position.Z);
				if (fThisHeightDifference >= 15f)
				{
					if (!PlayerMover.ShouldHandleObstacleObject)
						return false;
					else if (fThisHeightDifference >= 20f)//Was stuck.. so give extra range..
						return false;
				}
				return base.IsZDifferenceValid;
			}
		}

		public override bool ObjectShouldBeRecreated
		{
			get
			{
				return false;
			}
		}

		public override RunStatus Interact()
		{
			Skill.SetupAbilityForUse(ref FunkyGame.Hero.Class.PowerPrime, true);

			if (FunkyGame.Hero.Class.PowerPrime.Power != SNOPower.None)
			{
				// Wait while animating before an attack
				if (FunkyGame.Hero.Class.PowerPrime.WaitWhileAnimating)
					FunkyGame.Hero.WaitWhileAnimating(9);

				//if (targetType.Value == TargetType.Barricade)
				//	Logger.DBLog.DebugFormat("[Funky] Barricade: Name=" + InternalName + ". SNO=" + SNOID.ToString() +
				//							", Range=" + CentreDistance.ToString() + ". Needed range=" + FunkyGame.Hero.Class.PowerPrime.MinimumRange.ToString() + ". Radius=" +
				//							Radius.ToString() + ". SphereRadius=" + ActorSphereRadius.Value.ToString() + ". Type=" + targetType.ToString() + ". Using power=" + FunkyGame.Hero.Class.PowerPrime.Power.ToString());
				//else
				//	Logger.DBLog.DebugFormat("[Funky] Destructible: Name=" + InternalName + ". SNO=" + SNOID.ToString() +
				//							", Range=" + CentreDistance.ToString() + ". Needed range=" + FunkyGame.Hero.Class.PowerPrime.MinimumRange.ToString() + ". Radius=" +
				//							Radius.ToString() + ". SphereRadius=" + ActorSphereRadius.Value.ToString() + ". Type=" + targetType.ToString() + ". Using power=" + FunkyGame.Hero.Class.PowerPrime.Power.ToString());

				Skill.UsePower(ref FunkyGame.Hero.Class.PowerPrime);

				if (FunkyGame.Hero.Class.PowerPrime.SuccessUsed.HasValue && FunkyGame.Hero.Class.PowerPrime.SuccessUsed.Value)
				{
					//Logger.DBLog.InfoFormat(powerPrime.powerThis.ToString() + " used successfully");
					FunkyGame.Hero.Class.PowerPrime.OnSuccessfullyUsed();
					BlacklistLoops = 10;
				}
				else
				{
					PowerCacheLookup.dictAbilityLastFailed[FunkyGame.Hero.Class.PowerPrime.Power] = DateTime.Now;
				}

				InteractionAttempts++;
				// If we've tried interacting too many times, blacklist this for a while
				if (InteractionAttempts > 2)
				{
					BlacklistLoops = 10;
					InteractionAttempts = 0;
				}


				FunkyGame.Hero.WaitWhileAnimating(9, true);
			}

			//Get current animation state! (Idle = Untouched, Dead = Destroyed)
			UpdateAnimationState();
			AnimationState currentAnimState = AnimState;
			if (currentAnimState != AnimationState.Idle || !IsStillValid())
			{
				// Now tell Trinity to get a new target!
				FunkyGame.Targeting.Cache.bForceTargetUpdate = true;
				FunkyGame.Navigation.lastChangedZigZag = DateTime.Today;
				FunkyGame.Navigation.vPositionLastZigZagCheck = Vector3.Zero;
				PlayerMover.ShouldHandleObstacleObject = false;

				//Blacklist all destructibles surrounding this obj
				//ObjectCache.Objects.BlacklistObjectsSurroundingObject<CacheDestructable>(this, 10f, 15);
			}

			return RunStatus.Running;
		}

		public override bool WithinInteractionRange()
		{
			float fRangeRequired = FunkyGame.Hero.Class.PowerPrime.MinimumRange;

			float distance = RadiusDistance;
			if (FunkyGame.Targeting.Cache.LastCachedTarget.Equals(this))
			{
				// distance+=(this.CollisionRadius.Value*0.25f);
			}

			base.DistanceFromTarget = distance;

			return (base.DistanceFromTarget <= fRangeRequired);
		}

		public override string DebugString
		{

			get
			{
				return String.Format("{0}\r\nLastIntersectionResult={1}",
					base.DebugString,
					lastIntersectionTestResult.HasValue ? lastIntersectionTestResult.Value.ToString() : "NULL");
			}
		}
	}
}