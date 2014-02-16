using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyMonitor.Common;
using FunkyBot.DBHandlers;
using Zeta;
using Zeta.CommonBot;
using Zeta.TreeSharp;

namespace FunkyBot.Game
{
	internal class GoldInactivity
	{
		public bool BehaviorEngaged = false;
		public bool TimeoutTripped { get; set; }
		public DateTime LastCoinageUpdate
		{
			get { return _lastcoinageupdate;}
			set 
			{ 
				_lastcoinageupdate=value;
				TimeoutTripped = false;
			}
		}
		private DateTime _lastcoinageupdate;

		public GoldInactivity()
		{
			_lastcoinageupdate = DateTime.Now;
			TimeoutTripped = false;
		}

		private DateTime lastCheckedTimeout=DateTime.MinValue;
		public void CheckTimeoutTripped()
		{
			if (DateTime.Now.Subtract(lastCheckedTimeout).TotalMilliseconds > 5000)
			{
				lastCheckedTimeout=DateTime.Now;
				double lastCoinageChange = DateTime.Now.Subtract(LastCoinageUpdate).TotalMilliseconds;
				TimeoutTripped = lastCoinageChange >= Bot.Settings.Plugin.GoldInactivityTimeoutMilliseconds;
				//if (TimeoutTripped)
				//{
				//	Logging.Write("Gold Inactivity Timer has exceede the maximum value allowed!");
				//}
			}
		}

		public RunStatus ExitGame()
		{
			

			if (TownPortalBehavior.FunkyTPOverlord(null))
			{
				TownPortalBehavior.FunkyTPBehavior(null);
				return RunStatus.Running;
			}

			string profile = Bot.Game.CurrentGameStats.Profiles.Count > 0 ? Bot.Game.CurrentGameStats.Profiles.First().ProfileName:
							Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile;

			ProfileManager.Load(profile);

			Logging.Write("[Funky] Exiting game Due To Gold Inactivity Timeout.. Reloading First Profile.");
			ZetaDia.Service.Party.LeaveGame();
			//ZetaDia.Service.Games.LeaveGame();
			EventHandlers.FunkyOnLeaveGame(null, null);

			BehaviorEngaged = false;

			return RunStatus.Success;
		}
	}
}
