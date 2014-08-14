using System;
using fBaseXtensions.Cache.Internal.Blacklist;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.Game.Internals.SNO;

namespace fBaseXtensions.Cache.Internal.Objects
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
		public bool? GizmoDisabledByScript
		{
			get;
			set;
		}
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

				//Disabled by script?
				if (GizmoDisabledByScript.HasValue && GizmoDisabledByScript.Value)
				{
					IgnoredType = TargetingIgnoreTypes.GizmoDisabledByScript;
					return false;
				}

				if (this.InternalName.ToLower().StartsWith("minimapicon"))
				{
					// Minimap icons caused a few problems in the past, so this force-blacklists them
					this.BlacklistFlag = BlacklistType.Permanent;
					this.NeedsRemoved = true;
					BlacklistCache.BlacklistSnoIDs.Add(this.SNOID); //SNO blacklist.
					return false;
				}


				//Z-Height FunkyBaseExtension.Difference Check
				if (!IsZDifferenceValid)
				{
					IgnoredType = TargetingIgnoreTypes.ZDifferenceFailure;
					BlacklistLoops = 3;
					return false;
				}


				return true;
			}
		}

		public override bool UpdateData()
		{

			if (ref_Gizmo == null)
			{
				try
				{
					ref_Gizmo = (DiaGizmo)ref_DiaObject;
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Failure to convert obj to DiaGizmo {0}", DebugStringSimple);


					NeedsRemoved = true;
					return false;
				}
			}

			//Destructibles are not important unless they are close.. 40f is minimum range!
			//if ((this.targetType.Value == TargetType.Destructible || this.targetType.Value == TargetType.Barricade) && this.CentreDistance > 40f)
			//{
			//	if (FunkyBaseExtension.Settings.Debugging.FunkyLogFlags.HasFlag(LogLevel.Cache))
			//		Logger.Write(LogLevel.Cache, "Removing Destructible/Barricade {0} out of range!", InternalName);

			//	this.BlacklistLoops = 12;
			//	return false;
			//}

			if ((ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Interactables))
				 && (!this.GizmoHasBeenUsed.HasValue || !this.GizmoHasBeenUsed.Value))
			{
				try
				{

					if (base.Gizmotype.Value == GizmoType.PowerUp)
					{
						//this.HandleAsAvoidanceObject = true;
						GizmoShrine gizmoShrine = this.ref_Gizmo as GizmoShrine;
						this.GizmoHasBeenUsed = gizmoShrine.GizmoState == 1;
					}
					else if (base.Gizmotype.Value == GizmoType.HealingWell)
					{
						//this.HandleAsAvoidanceObject = true;
						GizmoHealthwell gizmoHealthWell = this.ref_Gizmo as GizmoHealthwell;
						this.GizmoHasBeenUsed = gizmoHealthWell.HasBeenOperated;
					}
					else if (base.Gizmotype.Value == GizmoType.Door)
					{
						GizmoDoor gizmoDoor = this.ref_Gizmo as GizmoDoor;
						this.GizmoHasBeenUsed = gizmoDoor.HasBeenOperated;
						//this.HandleAsAvoidanceObject = true;
					}
					else if (base.Gizmotype.Value == GizmoType.Chest)
					{
						//if (this.IsChestContainer)
						//this.HandleAsAvoidanceObject = true;

						GizmoLootContainer gizmoContainer = this.ref_Gizmo as GizmoLootContainer;
						this.GizmoHasBeenUsed = gizmoContainer.IsOpen;
					}
					else if (Gizmotype.Value == GizmoType.Switch)
					{
						this.GizmoHasBeenUsed = this.ref_Gizmo.HasBeenOperated;
					}
					else if (Gizmotype.Value == GizmoType.PoolOfReflection)
					{
						this.GizmoHasBeenUsed = this.ref_Gizmo.HasBeenOperated;
					}
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Exception GizmoHasBeenOperated {0}", DebugStringSimple);
					BlacklistFlag = BlacklistType.Temporary;
					BlacklistLoops = 100;
					NeedsRemoved = true;
					return false;
				}

				//Used Doors we remove from obstacle cache!
				if (Gizmotype.Value == GizmoType.Door && GizmoHasBeenUsed.HasValue && GizmoHasBeenUsed.Value && ObjectCache.Obstacles.ContainsKey(RAGUID))
				{
					ObjectCache.Obstacles.Remove(RAGUID);
					Obstacletype = ObstacleType.None;
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
					Logger.Write(LogLevel.Cache, "Exception occured attempting to update AnimState for object {0}", DebugStringSimple);
					//AnimState=AnimationState.Invalid;
				}
			}


			//DisabledByScript Check
			if ((!GizmoDisabledByScript.HasValue || GizmoDisabledByScript.Value) && ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Interactables))
			{
				try
				{
					GizmoDisabledByScript = ref_Gizmo.CommonData.GetAttribute<int>(ActorAttributeType.GizmoDisabledByScript) > 0;
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Handled GizmoDisabledByScript for gizmo {0}", DebugStringSimple);
					GizmoDisabledByScript = false;
				}
			}
			
			//Update Quest Monster?
			if (FunkyGame.Targeting.Cache.UpdateQuestMonsterProperty || FunkyGame.Game.QuestMode)
			{
				try
				{
					QuestMonster = ref_Gizmo.CommonData.GetAttribute<int>(ActorAttributeType.QuestMonster) != 0;
				}
				catch (Exception)
				{
					QuestMonster = false;
				}
			}

			if (ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Destructible | TargetType.Barricade | TargetType.Interactable))
			{
				if (this.IsBarricade.HasValue && this.IsBarricade.Value && !this.targetType.Value.HasFlag(TargetType.Barricade))
				{

					//Logger.DBLog.InfoFormat("Changing Gizmo {0} target type from {1} to Barricade!", this.InternalName, this.targetType.Value.ToString());
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
				return String.Format("{0}InteractionAttempts[{1}]" +
				                     "\r\nGizmoType[{2}]",
					  base.DebugString,
					  this.InteractionAttempts.ToString(),
					  base.Gizmotype.HasValue ? base.Gizmotype.Value.ToString() : "?");
			}
		}
	}
}