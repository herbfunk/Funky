using System;
using FunkyBot.DBHandlers;

namespace FunkyBot.Game
{
	internal class GoldInactivity
	{
		public bool TimeoutTripped { get; set; }
		public DateTime LastCoinageUpdate
		{
			get { return _lastcoinageupdate; }
			set
			{
				_lastcoinageupdate = value;
				TimeoutTripped = false;
			}
		}
		private DateTime _lastcoinageupdate;

		public GoldInactivity()
		{
			_lastcoinageupdate = DateTime.Now;
			TimeoutTripped = false;
		}


		public void CheckTimeoutTripped()
		{
			if (Bot.Settings.Plugin.GoldInactivityTimeoutSeconds == 0) return;

			double lastCoinageChange = DateTime.Now.Subtract(LastCoinageUpdate).TotalSeconds;
			if (lastCoinageChange > 5)
			{
				TimeoutTripped = lastCoinageChange >= Bot.Settings.Plugin.GoldInactivityTimeoutSeconds;
				if (TimeoutTripped)
				{
					Logger.DBLog.Info("[Funky] Gold Timeout Breached.. enabling exit behavior!");
					ExitGame.ShouldExitGame = true;
				}
			}
		}
	}
}
