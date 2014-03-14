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
			double lastCoinageChange = DateTime.Now.Subtract(LastCoinageUpdate).TotalMilliseconds;
			if (lastCoinageChange > 5000)
			{
				TimeoutTripped = lastCoinageChange >= Bot.Settings.Plugin.GoldInactivityTimeoutMilliseconds;
				if (TimeoutTripped) ExitGame.ShouldExitGame = true;
			}
		}
	}
}
