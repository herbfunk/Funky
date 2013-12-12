using FunkyBot.Movement;
using Zeta;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.Internals.Actors;


namespace FunkyBot.DBHandlers
{

	internal static partial class TownRunManager
	{
		//This is used prior to Demonbuddy takes control.. to reduce the lag caused by the pathing towards the townportal.



		internal static bool FinishTownRunOverlord(object ret)
		{//Should we preform town run finishing behavior?

			//Did Vendor Run Start in Town?
			if (!TownrunStartedInTown)
			{
				return true;
			}

			TownrunStartedInTown = true;
			return false;
		}

		internal static RunStatus TownRunFinishBehavior(object ret)
		{//Return to the town portal

			//Check if town portal object is present
			if (!FoundTownPortal)
			{
				var portalObjects = ZetaDia.Actors.GetActorsOfType<DiaGizmo>();
				foreach (DiaGizmo portalObject in portalObjects)
				{
					if (portalObject.IsTownPortal)
					{
						Logging.Write("Found Townportal Object");
						TownPortalObj = portalObject;
						FoundTownPortal = true;
						break;
					}
				}

				//Check if Townportal object is null..
				if (TownPortalObj == null)
				{
					switch (ZetaDia.CurrentAct)
					{
						case Act.A1:
							TownportalMovementVector3 = new Vector3(2984.8f, 2794.428f, 24.04532f);
							break;
						case Act.A2:
							TownportalMovementVector3 = new Vector3(308.9394f, 274.3232f, 0.1000006f);
							break;
						case Act.A3:
						case Act.A4:
							TownportalMovementVector3 = new Vector3(376.0297f, 390.4622f, 0.4707576f);
							break;
					}
				}
				else
				{
					TownportalMovementVector3 = TownPortalObj.Position;
				}

				Navigation.NP.MoveTo(TownportalMovementVector3, "TownPortal", true);
			}
			Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
			if (FoundTownPortal && TownPortalObj.Distance < 3f)
			{
				//We are within interaction distance -- reset and finish!
				FoundTownPortal = false;
				TownPortalObj = null;
				TownportalMovementVector3 = Vector3.Zero;
				TownrunStartedInTown = true;
				Navigation.NP.Clear();
				return RunStatus.Success;
			}
			if (Navigation.NP.CurrentPath.Count > 0 && vectorPlayerPosition.Distance(Navigation.NP.CurrentPath.Current) <= Navigation.NP.PathPrecision)
				Navigation.NP.MoveTo(TownportalMovementVector3, "TownPortal", true);

			//Use our click movement
			Bot.NavigationCache.RefreshMovementCache();

			//Wait until we are not moving to send click again..
			if (Bot.NavigationCache.IsMoving)
				return RunStatus.Running;

			ZetaDia.Me.UsePower(SNOPower.Walk, Navigation.NP.CurrentPath.Current, Bot.Character.Data.iCurrentWorldID);

			return RunStatus.Success;
		}

	}

}
