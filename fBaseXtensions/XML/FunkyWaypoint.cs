using System;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace fBaseXtensions.XML
{
	//

	[XmlElement("FunkyWaypoint")]
	class FunkyWaypoint : ProfileBehavior
	{
		[XmlAttribute("ID")]
		[XmlAttribute("waypointNumber")]
		[XmlAttribute("waypoint")]
		public int waypointID
		{
			get;
			set;
		}

		private bool m_IsDone = false;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new PrioritySelector(
				//Init: Current Position / Current Level ID / Navigator Cleared
				 new Decorator(ret => !InitDone,
					  new Action(ret => Init())
				 ),
				 new Decorator(ret => FunkyGame.Targeting.Cache.ShouldRefreshObjectList && !ShouldAttemptInteraction(),
					  new Action(ret => FunkyGame.Targeting.Cache.Refresh())
				 ),
				 new Decorator(ret => FunkyGame.Targeting.Cache.CurrentTarget != null,
					  new Action(ret => Combat())
				 ),
				//Attempt to Update Actor, Failure == DONE! -- Check if level area has changed already!
				 new Decorator(ret => LevelAreaChanged(),
					  new PrioritySelector(
							new Decorator(ret => ZetaDia.IsLoadingWorld,
								 new Sleep(1000)),
							new Decorator(ret => !ZetaDia.IsLoadingWorld,
								 new Action(ret => m_IsDone = true)))
				 ),
				 //Toggle Waypoint Map!
				 new Decorator(ret => !UIElements.WaypointMap.IsVisible,
					 new Action(ret => UIManager.ToggleWaypointMap())),
				//Check if Correct UI is showing!
				new Decorator(ret => !UI.ValidateUIElement(UI.WaypointMap.GetWaypointUIByWaypointID(waypointID)),
					new PrioritySelector(
						new Decorator(ret => !UI.ValidateUIElement(UI.WaypointMap.WaypointMap_ActOne),
							new Action(ret => UI.WaypointMap.WaypointMap_ZoomOut.Click())),
						new Decorator(ret => UI.WaypointMap.GetWaypointActUIByWaypointID(waypointID).IsVisible,
							new Action(ret => UI.WaypointMap.GetWaypointActUIByWaypointID(waypointID).Click()))
							)),
				//Check if in range and interaction should occur again!
				 new Decorator(ret => ShouldAttemptInteraction() && !LevelAreaChanged(),
					  new Action(ret => UseWaypoint())
				 )
			);
		}

		private int CurrentLevelID;
		private int CurrentWorldID;

		private bool InitDone = false;
		private void Init()
		{
			try
			{
				CurrentLevelID = ZetaDia.CurrentLevelAreaId;
				CurrentWorldID = ZetaDia.CurrentWorldId;
				InitDone = true;
			}
			catch (Exception)
			{

			}
		}


		private bool LevelAreaChanged()
		{
			int CurrentID = -1;
			int curWorldID = -1;
			try
			{
				CurrentID = ZetaDia.CurrentLevelAreaId;
				curWorldID = ZetaDia.CurrentWorldId;
			}
			catch (Exception)
			{
				// return false;


			}

			if ((curWorldID == -1 && CurrentID == -1) || (CurrentID == CurrentLevelID && CurrentWorldID == curWorldID))
				return false;
			if (!(CurrentID == -1 && curWorldID == -1) && (CurrentLevelID != CurrentID || curWorldID != CurrentWorldID))
				return true;

			return true;
		}

		private DateTime LastInteraction = DateTime.MinValue;
		private bool ShouldAttemptInteraction()
		{
			return DateTime.Now.Subtract(LastInteraction).TotalSeconds > 8;
		}

		private void UseWaypoint()
		{
			try
			{
				Logger.DBLog.Info("Using Waypoint -- WaypointID: " + waypointID);
				new DwordDataMessage(Opcode.DWordDataMessage21, waypointID).Send();
				LastInteraction = DateTime.Now;

			}
			catch (Exception)
			{

			}

		}

		private RunStatus Combat()
		{

			FunkyGame.Targeting.Movement.RestartTracking();

			//Directly Handle Target..
			RunStatus targetHandler = FunkyGame.Targeting.Handler.HandleThis();

			//Only return failure if handling failed..
			if (targetHandler == RunStatus.Failure)
			{
				return RunStatus.Success;
			}
			if (targetHandler == RunStatus.Success)
			{
				FunkyGame.Targeting.ResetTargetHandling();
				return RunStatus.Failure;
			}

			return RunStatus.Running;
		}


	}
}
