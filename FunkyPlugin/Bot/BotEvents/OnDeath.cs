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
				if (DateTime.Now.Subtract(Bot.lastDied).TotalSeconds>10)
				{
					 Bot.lastDied=DateTime.Now;
					 Bot.iTotalDeaths++;
					 Bot.iDeathsThisRun++;

					 //Herbfunk Stats
					 Statistics.GameStats.CurrentGame.Deaths++;
					 Statistics.ProfileStats.CurrentProfile.DeathCount++;

					 ResetBot();

	
					 // Does Trinity need to handle deaths?
					 if (Bot.iMaxDeathsAllowed>0)
					 {
						  if (Bot.iDeathsThisRun>=Bot.iMaxDeathsAllowed)
						  {
								Logging.Write("[Funky] You have died too many times. Now restarting the game.");

								string sUseProfile=Funky.sFirstProfileSeen;
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