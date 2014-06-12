using System;
using System.Linq;
using FunkyBot.Cache.Enums;
using FunkyBot.Movement;
using FunkyBot.Player.HotBar.Skills;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Logger = FunkyBot.Misc.Logger;
using LogLevel = FunkyBot.Misc.LogLevel;

namespace FunkyBot.Cache.Objects
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
				return Bot.Settings.Ranges.DestructibleRange + 1;
			}
		}

		public override bool ObjectIsValidForTargeting
		{
			get
			{
				if (!base.ObjectIsValidForTargeting) return false;


				////Get current animation state! (Idle = Untouched, Dead = Destroyed)
				UpdateAnimationState();
				AnimationState currentAnimState = AnimState;
				if (currentAnimState != AnimationState.Idle)
				{
					NeedsRemoved = true;
					BlacklistFlag = BlacklistType.Permanent;
					Logger.Write(LogLevel.Cache, "Removing destructible {0} due to invalid AnimationState of {1} -- SNOAnim {2}", InternalName, currentAnimState.ToString(), SnoAnim.ToString());
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

					if (!base.LineOfSight.LOSTest(Bot.Character.Data.Position, true, ServerObjectIntersection: false) && (PlayerMover.iTotalAntiStuckAttempts == 0 || RadiusDistance > 12f))
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



				float radiusDistance = RadiusDistance;
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
							BarricadeTest = MathEx.GetPointAt(Position, 10f, Navigation.FindDirection(Bot.Character.Data.Position, Position, true));
							BarricadeTest.Z = Navigation.MGP.GetHeight(BarricadeTest.ToVector2());

							lastIntersectionTestResult = MathEx.IntersectsPath(Position, CollisionRadius.Value, Bot.Character.Data.Position, BarricadeTest);
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
			if (Equals(Bot.Targeting.Cache.LastCachedTarget) && centreDistance <= 25f)
				Weight += 400;
			// Close destructibles get a weight increase
			if (centreDistance <= 16f)
				Weight += 1500d;
			Vector3 BotPosition = Bot.Character.Data.Position;
			// If there's a monster in the path-line to the item, reduce the weight by 50%
			if (RadiusDistance > 0f && !Bot.Character.Data.equipment.NoMonsterCollision && ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
				Weight *= 0.5;
			// Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
			if (Bot.Character.Data.bIsRooted)
				Weight = 19200d - (Math.Floor(centreDistance) * 200d);
			// Very close destructibles get a final weight increase
			if (centreDistance <= 12f)
				Weight += 2000d;

		}

		public override bool IsZDifferenceValid
		{
			get
			{
				float fThisHeightDifference = Funky.Difference(Bot.Character.Data.Position.Z, Position.Z);
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
			Skill.SetupAbilityForUse(ref Bot.Character.Class.PowerPrime, true);

			if (Bot.Character.Class.PowerPrime.Power != SNOPower.None)
			{
				// Wait while animating before an attack
				if (Bot.Character.Class.PowerPrime.WaitWhileAnimating)
					Bot.Character.Data.WaitWhileAnimating(9);

				if (targetType.Value == TargetType.Barricade)
					Logger.DBLog.DebugFormat("[Funky] Barricade: Name=" + InternalName + ". SNO=" + SNOID.ToString() +
											", Range=" + CentreDistance.ToString() + ". Needed range=" + Bot.Character.Class.PowerPrime.MinimumRange.ToString() + ". Radius=" +
											Radius.ToString() + ". SphereRadius=" + ActorSphereRadius.Value.ToString() + ". Type=" + targetType.ToString() + ". Using power=" + Bot.Character.Class.PowerPrime.Power.ToString());
				else
					Logger.DBLog.DebugFormat("[Funky] Destructible: Name=" + InternalName + ". SNO=" + SNOID.ToString() +
											", Range=" + CentreDistance.ToString() + ". Needed range=" + Bot.Character.Class.PowerPrime.MinimumRange.ToString() + ". Radius=" +
											Radius.ToString() + ". SphereRadius=" + ActorSphereRadius.Value.ToString() + ". Type=" + targetType.ToString() + ". Using power=" + Bot.Character.Class.PowerPrime.Power.ToString());

				Skill.UsePower(ref Bot.Character.Class.PowerPrime);

				if (Bot.Character.Class.PowerPrime.SuccessUsed.HasValue && Bot.Character.Class.PowerPrime.SuccessUsed.Value)
				{
					//Logger.DBLog.InfoFormat(powerPrime.powerThis.ToString() + " used successfully");
					Bot.Character.Class.PowerPrime.OnSuccessfullyUsed();
					BlacklistLoops = 10;
				}
				else
				{
					PowerCacheLookup.dictAbilityLastFailed[Bot.Character.Class.PowerPrime.Power] = DateTime.Now;
				}

				InteractionAttempts++;
				// If we've tried interacting too many times, blacklist this for a while
				if (InteractionAttempts > 2)
				{
					BlacklistLoops = 10;
					InteractionAttempts = 0;
				}


				Bot.Character.Data.WaitWhileAnimating(9, true);
			}

			//Get current animation state! (Idle = Untouched, Dead = Destroyed)
			UpdateAnimationState();
			AnimationState currentAnimState = AnimState;
			if (currentAnimState != AnimationState.Idle || !IsStillValid())
			{
				// Now tell Trinity to get a new target!
				Bot.Targeting.Cache.bForceTargetUpdate = true;
				Bot.NavigationCache.lastChangedZigZag = DateTime.Today;
				Bot.NavigationCache.vPositionLastZigZagCheck = Vector3.Zero;
				PlayerMover.ShouldHandleObstacleObject = false;

				//Blacklist all destructibles surrounding this obj
				//ObjectCache.Objects.BlacklistObjectsSurroundingObject<CacheDestructable>(this, 10f, 15);
			}

			return RunStatus.Running;
		}

		public override bool WithinInteractionRange()
		{
			float fRangeRequired = Bot.Character.Class.PowerPrime.MinimumRange;

			float distance = RadiusDistance;
			if (Bot.Targeting.Cache.LastCachedTarget.Equals(this))
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