using System;
using FunkyBot.Cache.Enums;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Internals.Actors.Gizmos;

namespace FunkyBot.Cache.Objects
{

	public class CacheGizmo : CacheObject
	{

		public CacheGizmo(CacheObject baseobj)
			: base(baseobj)
		{

		}

		public DiaGizmo ref_Gizmo { get; set; }

		///<summary>
		///For Shrines/Healthwells the value is set to (GizmoHasBeenOperated). For Containers the value is set to (ChestOpen).
		///</summary>
		public bool? GizmoHasBeenUsed { get; set; }
		public int? PhysicsSNO { get; set; }
		//public bool? IsEnviromentalActor { get; set; }

		internal bool? HandleAsObstacle { get; set; }




		public override void UpdateWeight()
		{
			base.UpdateWeight();
		}

		public override bool ObjectIsValidForTargeting
		{
			get
			{
				if (!base.ObjectIsValidForTargeting)
					return false;


				if (this.InternalName.ToLower().StartsWith("minimapicon"))
				{
					// Minimap icons caused a few problems in the past, so this force-blacklists them
					this.BlacklistFlag = BlacklistType.Permanent;
					this.NeedsRemoved = true;
					BlacklistCache.hashActorSNOIgnoreBlacklist.Add(this.SNOID); //SNO blacklist.
					return false;
				}


				//Z-Height Funky.Difference Check
				if (!this.IsZDifferenceValid)
				{
					this.BlacklistLoops = 3;
					return false;
				}


				// Check the primary object blacklist
				if (BlacklistCache.hashSNOIgnoreBlacklist.Contains(this.SNOID))
				{
					this.NeedsRemoved = true;
					this.BlacklistFlag = BlacklistType.Permanent;
					return false;
				}


				return true;
			}
		}

		public override bool UpdateData()
		{

			if (this.ref_Gizmo == null)
			{
				try
				{
					this.ref_Gizmo = (DiaGizmo)base.ref_DiaObject;
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Failure to convert obj to DiaItem!");


					NeedsRemoved = true;
					return false;
				}
			}

			//Destructibles are not important unless they are close.. 40f is minimum range!
			//if ((this.targetType.Value == TargetType.Destructible || this.targetType.Value == TargetType.Barricade) && this.CentreDistance > 40f)
			//{
			//	if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Cache))
			//		Logger.Write(LogLevel.Cache, "Removing Destructible/Barricade {0} out of range!", InternalName);

			//	this.BlacklistLoops = 12;
			//	return false;
			//}

			if ((ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Interactables))
				 && (!this.GizmoHasBeenUsed.HasValue || !this.GizmoHasBeenUsed.Value))
			{
				try
				{
					if (base.Gizmotype.Value == Zeta.Internals.SNO.GizmoType.Shrine)
					{
						this.HandleAsObstacle = true;
						GizmoShrine gizmoShrine = this.ref_Gizmo as GizmoShrine;
						this.GizmoHasBeenUsed = gizmoShrine.GizmoState == 1;
					}
					else if (base.Gizmotype.Value == Zeta.Internals.SNO.GizmoType.Healthwell)
					{
						this.HandleAsObstacle = true;
						GizmoHealthwell gizmoHealthWell = this.ref_Gizmo as GizmoHealthwell;
						this.GizmoHasBeenUsed = gizmoHealthWell.HasBeenOperated;
					}
					else if (base.Gizmotype.Value == Zeta.Internals.SNO.GizmoType.Door)
					{
						GizmoDoor gizmoDoor = this.ref_Gizmo as GizmoDoor;
						this.GizmoHasBeenUsed = gizmoDoor.HasBeenOperated;
						this.HandleAsObstacle = true;
					}
					else if (base.Gizmotype.Value == Zeta.Internals.SNO.GizmoType.LootContainer)
					{
						if (this.IsChestContainer)
							this.HandleAsObstacle = true;

						GizmoLootContainer gizmoContainer = this.ref_Gizmo as GizmoLootContainer;
						this.GizmoHasBeenUsed = gizmoContainer.IsOpen;
					}
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Safely handled getting attribute GizmoHasBeenOperated gizmo {0}", this.InternalName);
					return false;
				}

				//Blacklist used gizmos.
				if (this.GizmoHasBeenUsed.HasValue && this.GizmoHasBeenUsed.Value)
				{
					Logger.Write(LogLevel.Cache, "Removing {0} Has Been Used!", InternalName);

					this.BlacklistFlag = BlacklistType.Permanent;
					this.NeedsRemoved = true;
					return false;
				}
			}

			//only shrines and "chests" would have set this value true.. so if no value than we set it false!
			if (!this.HandleAsObstacle.HasValue)
				this.HandleAsObstacle = false;
			else if (this.HandleAsObstacle.Value)
				base.Obstacletype = ObstacleType.ServerObject;

			//PhysicsSNO -- (continiously updated) excluding shrines/interactables
			if (ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Destructible | TargetType.Barricade | TargetType.Container))
			{
				try
				{
					this.PhysicsSNO = base.ref_DiaObject.PhysicsSNO;
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Safely handled exception getting physics SNO for object " + this.InternalName + " [" + this.SNOID + "]");
					return false;
				}
			}

			////Update SNOAnim
			if (ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Destructible | TargetType.Barricade))
			{
				try
				{
					UpdateAnimationState();
					UpdateSNOAnim();
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Exception occured attempting to update AnimState for object {0}", InternalName);
					//AnimState=AnimationState.Invalid;
				}
			}

			if (ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Destructible | TargetType.Barricade | TargetType.Interactable))
			{
				if (this.IsBarricade.HasValue && this.IsBarricade.Value && !this.targetType.Value.HasFlag(TargetType.Barricade))
				{
					Logging.WriteVerbose("Changing Gizmo {0} target type from {1} to Barricade!", this.InternalName, this.targetType.Value.ToString());
					//Change "barricade" attribute gizmos into barricade targeting!
					this.targetType = TargetType.Barricade;
				}
			}

			if (this.Radius == 0f)
			{
				if (base.ActorSphereRadius.HasValue)
				{
					this.Radius = base.ActorSphereRadius.Value;

					this.Radius *= 0.80f;

					if (this.Radius < 0f)
						this.Radius = 0f;
				}
			}

			return true;
		}

		public override bool IsStillValid()
		{
			if (ref_Gizmo == null || !ref_Gizmo.IsValid || ref_Gizmo.BaseAddress == IntPtr.Zero)
				return false;

			return base.IsStillValid();
		}

		public override string DebugString
		{

			get
			{
				return String.Format("{0} --  InteractionAttempts[{1}] \r\n GizmoType[{2}]",
					  base.DebugString,
					  this.InteractionAttempts.ToString(),
					  base.Gizmotype.HasValue ? base.Gizmotype.Value.ToString() : "?");
			}
		}
	}
}