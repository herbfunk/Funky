using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyMonitor.Common;
using Zeta;
using Zeta.CommonBot;
using Zeta.TreeSharp;

namespace FunkyBot.DBHandlers
{
	internal class ExitGame
	{
		private static bool _behaviorEngaged = false;
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
		public static DateTime BehaviorEngagedTime=DateTime.MaxValue;

		///<summary>
		///Exiting Game Behavior
		///</summary>
		public static RunStatus Behavior()
		{
			if (TownPortalBehavior.FunkyTPOverlord(null))
			{
				TownPortalBehavior.FunkyTPBehavior(null);
				return RunStatus.Running;
			}

			string profile = Bot.Game.CurrentGameStats.Profiles.Count > 0 ? Bot.Game.CurrentGameStats.Profiles.First().ProfileName :
							Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile;

			ProfileManager.Load(profile);

			Logging.Write("[Funky] Exiting game and reloading profile {0}", profile);
			ZetaDia.Service.Party.LeaveGame();
			//ZetaDia.Service.Games.LeaveGame();
			EventHandlers.FunkyOnLeaveGame(null, null);

			//Finally disable this..
			BehaviorEngaged = false;

			return RunStatus.Success;
		}
	}
}
