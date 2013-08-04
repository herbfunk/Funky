using System;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot.Profile;
using Zeta.Internals.Actors.Gizmos;
using Zeta.Navigation;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyTrinity.XMLTags
{
	[ComVisible(false)]
	[XmlElement("UseWaypointFunk")]
	public class UseWaypointFunkTag : ProfileBehavior
	{
		private bool m_IsDone=false;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Zeta.TreeSharp.PrioritySelector(
				//Init: Current Position / Current Level ID / Navigator Cleared
				new Zeta.TreeSharp.Decorator(ret => !InitDone,
					new Zeta.TreeSharp.Action(ret => Init())
					),
				//Attempt to Update Actor, Failure == DONE! -- Check if level area has changed already!
				new Zeta.TreeSharp.Decorator(ret => !SafeActorUpdate()||LevelAreaChanged(),
					new Zeta.TreeSharp.PrioritySelector(
						new Zeta.TreeSharp.Decorator(ret => ZetaDia.IsLoadingWorld,
							new Zeta.TreeSharp.Sleep(1000)),
						new Zeta.TreeSharp.Decorator(ret => !ZetaDia.IsLoadingWorld,
							new Zeta.TreeSharp.Action(ret => m_IsDone=true)))
					),
				//Check if we have not already made our movements to get into interact range. (Prevents moving during transition)
				new Zeta.TreeSharp.Decorator(ret => !MovementToWaypointCompleted,
					new Zeta.TreeSharp.PrioritySelector(
						//Check if out of range and not moving
						new Zeta.TreeSharp.Decorator(ret => !WithinRangeOfInteraction()&&ShouldMove(),
							new Zeta.TreeSharp.Action(ret => MoveToActor())
							),
						//Check if in range but still moving
						new Zeta.TreeSharp.Decorator(ret => WithinRangeOfInteraction()&&!ShouldMove(),
							new Zeta.TreeSharp.Action(ret => Navigator.PlayerMover.MoveStop())
							),
						//Within Range and Not Moving, update our movement var so we don't move anymore again.
						new Zeta.TreeSharp.Decorator(ret => WithinRangeOfInteraction()&&ShouldMove(),
							new Zeta.TreeSharp.Action(ret => MovementToWaypointCompleted=true))
						)
					),
				//Check if in range and interaction should occur again!
				new Zeta.TreeSharp.Decorator(ret => WithinRangeOfInteraction()&&ShouldAttemptInteraction()&&!LevelAreaChanged(),
					new Zeta.TreeSharp.Action(ret => DoInteraction())
					)
				);
		}

//		  private Vector3 CurrentPosition=Vector3.Zero;
		private int CurrentLevelID;
		private int CurrentWorldID;

		private bool InitDone=false;
		private void Init()
		{
			Navigator.Clear();

			try
			{
				CurrentLevelID=ZetaDia.CurrentLevelAreaId;
				CurrentWorldID=ZetaDia.CurrentWorldId;
					 
				InitDone=true;
			} catch (Exception)
			{

			}
		}

		private GizmoWaypoint WaypointGizmo;
		private bool SafeActorUpdate()
		{
			var units=ZetaDia.Actors.GetActorsOfType<GizmoWaypoint>(false, false)
				.Where(o => o.IsValid&&o.InLineOfSight)
				.OrderBy(o => o.Distance);

			if (units.Count()==0)
				return false;

			try
			{
				WaypointGizmo=units.First();
				return true;
			} catch (Exception)
			{

				return false;
			}

		}

		private bool MovementToWaypointCompleted=false;


		private bool WithinRangeOfInteraction()
		{
			try
			{
				if (WaypointGizmo.Distance>5f)
					return false;
				else
					return true;
			} catch (Exception)
			{
				return false;
			}
		}

		private bool LevelAreaChanged()
		{
			int CurrentID=-1;
			int curWorldID=-1;
			try
			{
				CurrentID=ZetaDia.CurrentLevelAreaId;
				curWorldID=ZetaDia.CurrentWorldId;
			} catch (Exception)
			{
				// return false;


			}

			if ((curWorldID==-1&&CurrentID==-1)||(CurrentID==CurrentLevelID&&CurrentWorldID==curWorldID))
				return false;
			if (!(CurrentID==-1&&curWorldID==-1)&&(CurrentLevelID!=CurrentID||curWorldID!=CurrentWorldID))
				return true;

			return true;
		}

		private bool ShouldMove()
		{
			try
			{
				return !ZetaDia.Me.Movement.IsMoving;
			} catch (Exception)
			{

				return false;
			}
		}

//		  private MoveResult LastMoveResult;
		private void MoveToActor()
		{

			Navigator.MoveTo(WaypointGizmo.Position);
		}

		private DateTime LastInteraction=DateTime.MinValue;
		private bool ShouldAttemptInteraction()
		{
			if (DateTime.Now.Subtract(LastInteraction).TotalSeconds>5)
				return true;
			else
				return false;
		}

		private void DoInteraction()
		{
			try
			{
				Logging.Write("Using Waypoint -- WaypointID: "+waypointID);
				WaypointGizmo.UseWaypoint(waypointID);
				LastInteraction=DateTime.Now;

			} catch (Exception)
			{

			}

		}


		[XmlAttribute("waypointNumber")]
		[XmlAttribute("waypoint")]
		public int waypointID
		{
			get;
			set;
		}
	}
}