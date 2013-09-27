using System;
using System.Linq;
using FunkyTrinity.Cache.Enums;

using FunkyTrinity.Movement;
using FunkyTrinity.Targeting;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;
using Zeta.TreeSharp;

namespace FunkyTrinity.Cache
{
	public class CacheDestructable : CacheGizmo
	{

		public CacheDestructable(CacheObject baseobj)
			: base(baseobj)
		{
		}

		private bool? lastIntersectionTestResult=null;

		public override bool ObjectIsValidForTargeting
		{
			get
			{
				if (!base.ObjectIsValidForTargeting) return false;

				//Get current animation state! (Idle = Untouched, Dead = Destroyed)
				AnimationState currentAnimState=this.AnimState;
				if (currentAnimState!=AnimationState.Idle)
				{
					this.NeedsRemoved=true;
					this.BlacklistFlag=BlacklistType.Permanent;
					return false;
				}

				// No physics mesh? Ignore this destructible altogether
				if (this.PhysicsSNO.HasValue&&this.PhysicsSNO.Value<=0)
				{
					// No physics mesh on a destructible, probably bugged
					this.NeedsRemoved=true;
					this.BlacklistFlag=BlacklistType.Permanent;
					return false;
				}

				//We don't cache unless its 40f, so if its out of range we remove it!
				if (base.CentreDistance>75f)
				{
					this.NeedsRemoved=true;
					return false;
				}


				if (this.RequiresLOSCheck&&!this.IgnoresLOSCheck)
				{
					//Preform Test every 2500ms on normal objects, 1250ms on special objects.
					double lastLOSCheckMS=base.LineOfSight.LastLOSCheckMS;
					if (lastLOSCheckMS<1250)
						return false;
					else if (lastLOSCheckMS<2500&&this.CentreDistance>20f)
						return false;

					if (!base.LineOfSight.LOSTest(Bot.Character.Position, true, false)&&(Funky.PlayerMover.iTotalAntiStuckAttempts==0||this.RadiusDistance>12f))
					{
						return false;
					}

					this.RequiresLOSCheck=false;
				}

				//Ignore Destructibles Setting
				if (!Zeta.CommonBot.Settings.CharacterSettings.Instance.DestroyEnvironment
				    &&this.Gizmotype.Value==GizmoType.Destructible
				    &&this.PriorityCounter==0
				    &&!this.IsBarricade.Value)
				{
					//ignore from being a targeted right now..
					this.BlacklistLoops=15;
					//We dont want to complete ignore this since the object may pose a navigational obstacle.
					return false;
				}



				float radiusDistance=this.RadiusDistance;
				//Barricade and path intersects the actorsphere radius..
				//Some barricades may be lower than ourself, or our destination is high enough to raycast past the object. So we add a little to the Z of the obstacle.
				//The best method would be to get the hight of the object and compare it to our current Z-height if we are nearly within radius distance of the object.
				if ((this.targetType.Value==TargetType.Barricade||this.IsBarricade.HasValue&&this.IsBarricade.Value)&&
				    (!Funky.PlayerMover.ShouldHandleObstacleObject  //Have special flag from unstucker to destroy nearby barricade.
				     &&this.PriorityCounter==0
				     &&radiusDistance>0f))
				{
					Vector3 BarricadeTest=this.Position;
					if (radiusDistance>1f)
					{
						if (radiusDistance<10f)
						{
							BarricadeTest=MathEx.GetPointAt(this.Position, 10f, Navigation.FindDirection(Bot.Character.Position, this.Position, true));
							BarricadeTest.Z=Navigation.MGP.GetHeight(BarricadeTest.ToVector2());

							this.lastIntersectionTestResult=MathEx.IntersectsPath(this.Position, this.CollisionRadius.Value, Bot.Character.Position, BarricadeTest);
							if (!this.lastIntersectionTestResult.Value)
							{
								return false;
							}
						}
					}
				}


				if (radiusDistance>(Bot.DestructibleRange+5f))
				{
					return false;
				}

				return true;
			}
		}

		public override void UpdateWeight()
		{
			base.UpdateWeight();

			float centreDistance=this.CentreDistance;
			this.Weight=12000d-(Math.Floor(centreDistance)*175d);
			// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
			if (this==Bot.Target.LastCachedTarget&&centreDistance<=25f)
				this.Weight+=400;
			// Close destructibles get a weight increase
			if (centreDistance<=16f)
				this.Weight+=1500d;
			Vector3 BotPosition=Bot.Character.Position;
			// If there's a monster in the path-line to the item, reduce the weight by 50%
			if (this.RadiusDistance>0f&&ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
				this.Weight*=0.5;
			// Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
			if ((Bot.Combat.bForceCloseRangeTarget||Bot.Character.bIsRooted))
				this.Weight=19200d-(Math.Floor(centreDistance)*200d);
			// Very close destructibles get a final weight increase
			if (centreDistance<=12f)
				this.Weight+=2000d;

		}

		public override bool IsZDifferenceValid
		{
			get
			{
				float fThisHeightDifference=Funky.Difference(Bot.Character.Position.Z, this.Position.Z);
				if (fThisHeightDifference>=15f)
				{
					if (!Funky.PlayerMover.ShouldHandleObstacleObject)
						return false;
					else if (fThisHeightDifference>=20f)//Was stuck.. so give extra range..
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
			Ability.ability.SetupAbilityForUse(ref Bot.Class.PowerPrime, true);

			if (Bot.Class.PowerPrime.Power!=SNOPower.None)
			{
				// Wait while animating before an attack
				if (Bot.Class.PowerPrime.WaitWhileAnimating)
					Bot.Character.WaitWhileAnimating(9);

				if (this.targetType.Value==TargetType.Barricade)
					Logging.WriteDiagnostic("[Funky] Barricade: Name="+this.InternalName+". SNO="+this.SNOID.ToString()+
					                        ", Range="+this.CentreDistance.ToString()+". Needed range="+Bot.Class.PowerPrime.MinimumRange.ToString()+". Radius="+
					                        this.Radius.ToString()+". SphereRadius="+this.ActorSphereRadius.Value.ToString()+". Type="+this.targetType.ToString()+". Using power="+Bot.Class.PowerPrime.Power.ToString());
				else
					Logging.WriteDiagnostic("[Funky] Destructible: Name="+this.InternalName+". SNO="+this.SNOID.ToString()+
					                        ", Range="+this.CentreDistance.ToString()+". Needed range="+Bot.Class.PowerPrime.MinimumRange.ToString()+". Radius="+
					                        this.Radius.ToString()+". SphereRadius="+this.ActorSphereRadius.Value.ToString()+". Type="+this.targetType.ToString()+". Using power="+Bot.Class.PowerPrime.Power.ToString());

				Ability.ability.UsePower(ref Bot.Class.PowerPrime);

				if (Bot.Class.PowerPrime.SuccessUsed.HasValue&&Bot.Class.PowerPrime.SuccessUsed.Value)
				{
					//Logging.Write(powerPrime.powerThis.ToString() + " used successfully");
					Bot.Class.PowerPrime.SuccessfullyUsed();
					this.InteractionAttempts++;
				}
				else
				{
					PowerCacheLookup.dictAbilityLastFailed[Bot.Class.PowerPrime.Power]=DateTime.Now;
				}

						 
				// If we've tried interacting too many times, blacklist this for a while
				if (this.InteractionAttempts>5)
				{
					this.BlacklistLoops=20;
					this.InteractionAttempts=0;
				}

							
				Bot.Character.WaitWhileAnimating(9, true);
			}

			//Get current animation state! (Idle = Untouched, Dead = Destroyed)
			AnimationState currentAnimState=this.AnimState;
			if (currentAnimState!=AnimationState.Idle)
			{
				// Now tell Trinity to get a new target!
				Bot.Combat.bForceTargetUpdate=true;
				Bot.Combat.lastChangedZigZag=DateTime.Today;
				Bot.Combat.vPositionLastZigZagCheck=Vector3.Zero;
				Funky.PlayerMover.ShouldHandleObstacleObject=false;

				//Blacklist all destructibles surrounding this obj
				//ObjectCache.Objects.BlacklistObjectsSurroundingObject<CacheDestructable>(this, 10f, 15);
			}

			return RunStatus.Running;
		}

		public override bool WithinInteractionRange()
		{
			float fRangeRequired=Bot.Class.PowerPrime.MinimumRange;

			float distance=this.RadiusDistance;
			if (Bot.Target.LastCachedTarget.Equals(this))
			{
				 distance+=(this.CollisionRadius.Value*0.25f);
			}

			base.DistanceFromTarget=distance;

			return (base.DistanceFromTarget<=fRangeRequired);
		}

		public override string DebugString
		{

			get
			{
				return String.Format("{0} \r\n LastIntersectionResult={1}",
					base.DebugString,
					this.lastIntersectionTestResult.HasValue?this.lastIntersectionTestResult.Value.ToString():"NULL");
			}
		}
	}
}