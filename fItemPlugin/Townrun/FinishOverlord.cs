using System;
using fBaseXtensions.Behaviors;
using fBaseXtensions.Game;
using fBaseXtensions.Monitor;
using Zeta.Bot.Dungeons;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fItemPlugin.Townrun
{

	internal static partial class TownRunManager
	{
		

		private static bool ResetCache = false;
		public static RunStatus FinishBehavior(object ret)
		{
			if (FunkyGame.GameIsInvalid)
			{
				Logger.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			

			//If not in town.. lets check if there are any valid targets we could handle!
			if (!ZetaDia.IsInTown)
			{
				if (!ResetCache)
				{
					ZetaDia.Memory.ClearCache();
					ZetaDia.Actors.Update();
					GridSegmentation.Update();
					Navigator.SearchGridProvider.Update();
					ResetCache = true;
				}
				FunkyGame.Game.ForceOutOfCombatBehavior = true;
				if (FunkyGame.Targeting.CheckHandleTarget() == RunStatus.Running) return RunStatus.Running;

				float distanceFromStart=FunkyGame.Hero.Position.Distance(TownPortalBehavior.StartingPosition);

				//Backtrack to our starting location!
				if (TownPortalBehavior.StartingPosition != Vector3.Zero && distanceFromStart > 25f)
				{
					Logger.DBLog.InfoFormat("Returning to Start Location -- Distance of {0}", distanceFromStart);
					if (FunkyGame.Hero.IsMoving) return RunStatus.Running;
					Navigator.MoveTo(TownPortalBehavior.StartingPosition, "TownPortalStart");
					return RunStatus.Running;
				}

				TownPortalBehavior.StartingPosition = Vector3.Zero;
			}
			
			GoldInactivity.LastCoinageUpdate = DateTime.Now;
			fBaseXtensions.Navigation.Navigation.MGP.Update();
			FunkyGame.Game.ForceOutOfCombatBehavior = false;
			ResetCache = false;
			FunkyGame.Targeting.Cache.bFailedToLootLastItem = false;

			Logger.DBLog.Info("Finished Town Run Behavior!");
			return RunStatus.Success;
		}


	}

}