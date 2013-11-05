using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;
using System.Windows;
using System.Windows.Controls;
using Zeta.Navigation;

namespace FunkyBot
{
    public partial class Funky
    {
		  private static void FunkyOnGameChanged(object sender, EventArgs e)
		  {
			  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Event))
				  Logger.Write(LogLevel.Event, "OnGameChanged Event");

			  Navigator.SearchGridProvider.Update();
			  ResetGame();
			  Bot.Game.RefreshGameID();
		  }
    }
}