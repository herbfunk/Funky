using System;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;

namespace FunkyBot.EventHandlers
{
	public partial class EventHandlers
	{
		// When the bot stops, output a final item-stats report so it is as up-to-date as can be
		internal static void FunkyBotStop(IBot bot)
		{
			Navigator.PlayerMover = new DefaultPlayerMover();
			Navigator.StuckHandler = new DefaultStuckHandler();
			Funky.ResetTreehooks();
			//Bot.Reset();

		}
	}
}