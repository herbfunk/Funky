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

		private DateTime lastCheckedTimeout = DateTime.MinValue;
		public void CheckTimeoutTripped()
		{
			if (DateTime.Now.Subtract(lastCheckedTimeout).TotalMilliseconds > 5000)
			{
				lastCheckedTimeout = DateTime.Now;
				double lastCoinageChange = DateTime.Now.Subtract(LastCoinageUpdate).TotalMilliseconds;
				TimeoutTripped = lastCoinageChange >= Bot.Settings.Plugin.GoldInactivityTimeoutMilliseconds;
				if (TimeoutTripped) ExitGame.ShouldExitGame = true;
			}
		}
	}
}
