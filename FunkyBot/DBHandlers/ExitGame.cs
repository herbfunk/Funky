using System;
using System.Linq;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.TreeSharp;

namespace FunkyBot.DBHandlers
{
	internal class ExitGame
	{
		public static bool ShouldExitGame = false;

		private static bool _behaviorEngaged;
		public static bool BehaviorEngaged
		{
			get { return _behaviorEngaged; }
			set
			{
				_behaviorEngaged = value;
				//true we reset behavior starting time
				BehaviorEngagedTime = value ? DateTime.Now : DateTime.MaxValue;
			}
		}
		public static DateTime BehaviorEngagedTime = DateTime.MaxValue;

		///<summary>
		///Exiting Game Behavior
		///</summary>
		public static RunStatus Behavior()
		{

			//Run Town Portal Behavior.. 
			if (TownPortalBehavior.FunkyTPOverlord(null))
			{
				TownPortalBehavior.FunkyTPBehavior(null);
				return RunStatus.Running;
			}

			//Exit Game..
			if (ZetaDia.IsInGame)
			{
				if (DateTime.Now.Subtract(_lastExitAttempt).TotalSeconds > 4)
				{
					Logger.DBLog.InfoFormat("[Funky] Exiting game..");
					ZetaDia.Service.Party.LeaveGame();
					_lastExitAttempt = DateTime.Now;
				}
				return RunStatus.Running;
			}

			//Get First or Last Used Profile..
			string profile = Bot.Game.CurrentGameStats.Profiles.Count > 0 ? Bot.Game.CurrentGameStats.Profiles.First().ProfileName :
							GlobalSettings.Instance.LastProfile;

			//Load Profile and Fire our left game handler
			ProfileManager.Load(profile);
			EventHandlers.FunkyOnLeaveGame(null, null);

			//Finally disable this..
			BehaviorEngaged = false;
			ShouldExitGame = false;

			return RunStatus.Success;
		}
		private static DateTime _lastExitAttempt = DateTime.MinValue;
	}
}
