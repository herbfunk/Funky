using System;
using System.Linq;
using FunkyBot.Cache.Enums;
using FunkyBot.Movement;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;
using Zeta.TreeSharp;

namespace FunkyBot.Cache.Objects
{
	public class CacheDestructable : CacheGizmo
	{

		public CacheDestructable(CacheObject baseobj)
			: base(baseobj)
		{
		}

		private bool? lastIntersectionTestResult = null;

		public override bool ObjectIsValidForTargeting
		{
			get
			{
				if (!base.ObjectIsValidForTargeting) return false;

				//Update SNOAnim
				//if (this.Gizmotype.Value == GizmoType.Destructible)
				//{
				//	try
				//	{
				//		Logging.Write("Updating Animation");
				//		AnimState = (this.ref_Gizmo.CommonData.AnimationInfo.State);
				//		SnoAnim = (this.ref_Gizmo.CommonData.CurrentAnimation);
				//	}
				//	catch
				//	{
				//		if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Cache))
				//			Logger.Write(LogLevel.Cache, "Exception occured attempting to update AnimState for object {0}", InternalName);
				//		//AnimState=AnimationState.Invalid;
				//	}
				//}

				////Get current animation state! (Idle = Untouched, Dead = Destroyed)
				UpdateAnimationState();
				AnimationState currentAnimState = AnimState;
				if (currentAnimState != AnimationState.Idle)
				{
					this.NeedsRemoved = true;
					this.BlacklistFlag = BlacklistType.Permanent;
					Logger.Write(LogLevel.Cache, "Removing destructible {0} due to invalid AnimationState of {1} -- SNOAnim {2}", InternalName, currentAnimState.ToString(), SnoAnim.ToString());
					return false;
				}

				// No physics mesh? Ignore this destructible altogether
				if (this.PhysicsSNO.HasValue && this.PhysicsSNO.Value <= 0)
				{
					Logger.Write(LogLevel.Cache, "Removing destructible {0} due to invalid PhysicsSNO", InternalName);

					// No physics mesh on a destructible, probably bugged
					this.NeedsRemoved = true;
					this.BlacklistFlag = BlacklistType.Permanent;
					return false;
				}

				if (this.RequiresLOSCheck && !this.IgnoresLOSCheck)
				{
					//Preform Test every 2500ms on normal objects, 1250ms on special objects.
					double lastLOSCheckMS = base.LineOfSight.LastLOSCheckMS;
					if (lastLOSCheckMS < 1250)
						return false;
					else if (lastLOSCheckMS < 2500 && this.CentreDistance > 20f)
						return false;

					if (!base.LineOfSight.LOSTest(Bot.Character.Data.Position, true, false) && (Funky.PlayerMover.iTotalAntiStuckAttempts == 0 || this.RadiusDistance > 12f))
					{
						return false;
					}

					this.RequiresLOSCheck = false;
				}

				//Ignore Destructibles Setting
				if (!Zeta.CommonBot.Settings.CharacterSettings.Instance.DestroyEnvironment
					&& this.Gizmotype.Value == GizmoType.Destructible
					&& this.PriorityCounter == 0
					&& !this.IsBarricade.Value)
				{
					//ignore from being a targeted right now..
					this.BlacklistLoops = 15;
					//We dont want to complete ignore this since the object may pose a navigational obstacle.
					return false;
				}



				float radiusDistance = this.RadiusDistance;
				//Barricade and path intersects the actorsphere radius..
				//Some barricades may be lower than ourself, or our destination is high enough to raycast past the object. So we add a little to the Z of the obstacle.
				//The best method would be to get the hight of the object and compare it to our current Z-height if we are nearly within radius distance of the object.
				if ((this.targetType.Value == TargetType.Barricade || this.IsBarricade.HasValue && this.IsBarricade.Value) &&
					(!Funky.PlayerMover.ShouldHandleObstacleObject  //Have special flag from unstucker to destroy nearby barricade.
					 && this.PriorityCounter == 0
					 && radiusDistance > 0f))
				{
					Vector3 BarricadeTest = this.Position;
					if (radiusDistance > 1f)
					{
						if (radiusDistance < 10f)
						{
							BarricadeTest = MathEx.GetPointAt(this.Position, 10f, Navigation.FindDirection(Bot.Character.Data.Position, this.Position, true));
							BarricadeTest.Z = Navigation.MGP.GetHeight(BarricadeTest.ToVector2());

							this.lastIntersectionTestResult = MathEx.IntersectsPath(this.Position, this.CollisionRadius.Value, Bot.Character.Data.Position, BarricadeTest);
							if (!this.lastIntersectionTestResult.Value)
							{
								return false;
							}
						}
					}
				}


				if (radiusDistance > (Bot.Settings.Ranges.DestructibleRange + 5f))
				{
					return false;
				}

				return true;
			}
		}

		public override void UpdateWeight()
		{
			base.UpdateWeight();

			float centreDistance = this.CentreDistance;
			this.Weight = 12000d - (Math.Floor(centreDistance) * 175d);
			// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
			if (this == Bot.Targeting.LastCachedTarget && centreDistance <= 25f)
				this.Weight += 400;
			// Close destructibles get a weight increase
			if (centreDistance <= 16f)
				this.Weight += 1500d;
			Vector3 BotPosition = Bot.Character.Data.Position;
			// If there's a monster in the path-line to the item, reduce the weight by 50%
			if (this.RadiusDistance > 0f && ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
				this.Weight *= 0.5;
			// Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
			if (Bot.Character.Data.bIsRooted)
				this.Weight = 19200d - (Math.Floor(centreDistance) * 200d);
			// Very close destructibles get a final weight increase
			if (centreDistance <= 12f)
				this.Weight += 2000d;

		}

		public override bool IsZDifferenceValid
		{
			get
			{
				float fThisHeightDifference = Funky.Difference(Bot.Character.Data.Position.Z, this.Position.Z);
				if (fThisHeightDifference >= 15f)
				{
					if (!Funky.PlayerMover.ShouldHandleObstacleObject)
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
			Player.HotBar.Skills.Skill.SetupAbilityForUse(ref Bot.Character.Class.PowerPrime, true);

			if (Bot.Character.Class.PowerPrime.Power != SNOPower.None)
			{
				// Wait while animating before an attack
				if (Bot.Character.Class.PowerPrime.WaitWhileAnimating)
					Bot.Character.Data.WaitWhileAnimating(9);

				if (this.targetType.Value == TargetType.Barricade)
					Logging.WriteDiagnostic("[Funky] Barricade: Name=" + this.InternalName + ". SNO=" + this.SNOID.ToString() +
											", Range=" + this.CentreDistance.ToString() + ". Needed range=" + Bot.Character.Class.PowerPrime.MinimumRange.ToString() + ". Radius=" +
											this.Radius.ToString() + ". SphereRadius=" + this.ActorSphereRadius.Value.ToString() + ". Type=" + this.targetType.ToString() + ". Using power=" + Bot.Character.Class.PowerPrime.Power.ToString());
				else
					Logging.WriteDiagnostic("[Funky] Destructible: Name=" + this.InternalName + ". SNO=" + this.SNOID.ToString() +
											", Range=" + this.CentreDistance.ToString() + ". Needed range=" + Bot.Character.Class.PowerPrime.MinimumRange.ToString() + ". Radius=" +
											this.Radius.ToString() + ". SphereRadius=" + this.ActorSphereRadius.Value.ToString() + ". Type=" + this.targetType.ToString() + ". Using power=" + Bot.Character.Class.PowerPrime.Power.ToString());

				Player.HotBar.Skills.Skill.UsePower(ref Bot.Character.Class.PowerPrime);

				if (Bot.Character.Class.PowerPrime.SuccessUsed.HasValue && Bot.Character.Class.PowerPrime.SuccessUsed.Value)
				{
					//Logging.Write(powerPrime.powerThis.ToString() + " used successfully");
					Bot.Character.Class.PowerPrime.OnSuccessfullyUsed();
					this.InteractionAttempts++;
				}
				else
				{
					PowerCacheLookup.dictAbilityLastFailed[Bot.Character.Class.PowerPrime.Power] = DateTime.Now;
				}


				// If we've tried interacting too many times, blacklist this for a while
				if (this.InteractionAttempts > 1)
				{
					this.BlacklistLoops = 10;
					this.InteractionAttempts = 0;
				}


				Bot.Character.Data.WaitWhileAnimating(9, true);
			}

			//Get current animation state! (Idle = Untouched, Dead = Destroyed)
			UpdateAnimationState();
			AnimationState currentAnimState = this.AnimState;
			if (currentAnimState != AnimationState.Idle || !IsStillValid())
			{
				// Now tell Trinity to get a new target!
				Bot.Targeting.bForceTargetUpdate = true;
				Bot.NavigationCache.lastChangedZigZag = DateTime.Today;
				Bot.NavigationCache.vPositionLastZigZagCheck = Vector3.Zero;
				Funky.PlayerMover.ShouldHandleObstacleObject = false;

				//Blacklist all destructibles surrounding this obj
				//ObjectCache.Objects.BlacklistObjectsSurroundingObject<CacheDestructable>(this, 10f, 15);
			}

			return RunStatus.Running;
		}

		public override bool WithinInteractionRange()
		{
			float fRangeRequired = Bot.Character.Class.PowerPrime.MinimumRange;

			float distance = this.RadiusDistance;
			if (Bot.Targeting.LastCachedTarget.Equals(this))
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
				return String.Format("{0} \r\n LastIntersectionResult={1}",
					base.DebugString,
					this.lastIntersectionTestResult.HasValue ? this.lastIntersectionTestResult.Value.ToString() : "NULL");
			}
		}
	}
}