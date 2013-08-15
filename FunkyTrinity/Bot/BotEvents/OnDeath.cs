using System;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  private void FunkyOnDeath(object src, EventArgs mea)
		  {
				if (DateTime.Now.Subtract(Bot.Stats.lastDied).TotalSeconds>10)
				{
					 Bot.Stats.lastDied=DateTime.Now;
					 Bot.Stats.iTotalDeaths++;
					 Bot.Stats.iDeathsThisRun++;

					 //Herbfunk Stats
					 Bot.BotStatistics.GameStats.CurrentGame.Deaths++;
					 Bot.BotStatistics.ProfileStats.CurrentProfile.DeathCount++;

					 ResetBot();

	
					 // Does Trinity need to handle deaths?
					 if (Bot.Stats.iMaxDeathsAllowed>0)
					 {
						  if (Bot.Stats.iDeathsThisRun>=Bot.Stats.iMaxDeathsAllowed)
						  {
								Logging.Write("[Funky] You have died too many times. Now restarting the game.");

								string sUseProfile=Bot.Stats.sFirstProfileSeen;
								ProfileManager.Load(!string.IsNullOrEmpty(sUseProfile)
																?sUseProfile
																:Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile);
								Thread.Sleep(1000);
								ResetGame();
								ZetaDia.Service.Party.LeaveGame();
								Thread.Sleep(10000);
						  }
						  else
						  {
								Logging.Write("[Funky] I'm sorry, but I seem to have let you die :( Now restarting the current profile.");
								ProfileManager.Load(Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile);
								Thread.Sleep(3500);
						  }
					 }

				}
		  }
	 }
}