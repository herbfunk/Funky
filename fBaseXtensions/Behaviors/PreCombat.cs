using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero.Class;
using fBaseXtensions.Helpers;
using FunkyBot.DBHandlers.CharacterMule;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.TreeSharp;

namespace fBaseXtensions.Behaviors
{
	public static class PreCombat
	{
		public static bool PreCombatOverlord(object ret)
		{
			if (FunkyGame.GameIsInvalid) return false;

			FunkyGame.Targeting.CheckPrecombat();

			//Check for game prohibiting ui elements (Achievements, Skills, Waypoint Map, etc)
			var uie = UI.FindGameProhibitingElements();
			if (uie != null)
			{
				UI.ClosingUIElements = true;
				return true;
			}


			if (OutOfGame.MuleBehavior)
			{
				if (BotMain.StatusText.Contains("Game Finished"))
				{
					if (ZetaDia.Actors.GetActorsOfType<GizmoPlayerSharedStash>(true, true).Any())
					{
						return true;
					}
				}
			}
			else if (ExitGame.ShouldExitGame)
			{
				ExitGame.BehaviorEngaged = true;
				return true;
			}

			return false;
		}

		private static DateTime WaitTime = DateTime.Now;
		public static async Task<bool> _PreCombatOverlord()
		{
			if (FunkyGame.GameIsInvalid) return false;

			FunkyGame.Targeting.CheckPrecombat();

			//Check for game prohibiting ui elements (Achievements, Skills, Waypoint Map, etc)
			if (await UI._CloseGameProhibitingElements())
				return true;

			if (OutOfGame.MuleBehavior)
			{
				if (BotMain.StatusText.Contains("Game Finished"))
				{
					if (ZetaDia.Actors.GetActorsOfType<GizmoPlayerSharedStash>(true, true).Any())
					{
						return true;
					}
				}
			}
			else if (ExitGame.ShouldExitGame)
			{
				ExitGame.BehaviorEngaged = true;
				return true;
			}

			return false;
		}
		public static RunStatus HandleTarget(object ret)
		{
			if (UI.ClosingUIElements)
				return UI.CloseGameProhibitingElements();

			if (OutOfGame.MuleBehavior)
			{
				if (!OutOfGame.TransferedGear)
				{
					return NewMuleGame.StashTransfer();
				}

				return NewMuleGame.FinishMuleBehavior();
			}

			//Exit Game!!
			if (ExitGame.BehaviorEngaged)
			{
				return ExitGame.Behavior();
			}

			return RunStatus.Success;
		}
	}
}
