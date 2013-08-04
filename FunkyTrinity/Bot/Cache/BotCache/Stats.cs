using System;
using System.Collections.Generic;
using Zeta;

namespace FunkyTrinity
{

		  public static partial class Bot
		  {
				public class BotStatistics
				{
					 public BotStatistics()
					 {
						  iMaxDeathsAllowed=0;
						  iDeathsThisRun=0;
						  lastDied=DateTime.Today;
						  iTotalDeaths=0;

						  iTotalJoinGames=0;
						  iTotalLeaveGames=0;
						  iTotalProfileRecycles=0;

						  listProfilesLoaded=new List<string>();
						  sLastProfileSeen=String.Empty;
						  sFirstProfileSeen=String.Empty;
						  lastProfileCheck=DateTime.Today;

						  LastJoinedGame=DateTime.MinValue;
					 }

					 public DateTime LastJoinedGame { get; set; }
					 // Death counts
					 public int iMaxDeathsAllowed { get; set; }
					 public int iDeathsThisRun { get; set; }
					 // On death, clear the timers for all abilities
					 public DateTime lastDied { get; set; }
					 public int iTotalDeaths { get; set; }
					 // How many total leave games, for stat-tracking?
					 public int iTotalJoinGames { get; set; }
					 // How many total leave games, for stat-tracking?
					 public int iTotalLeaveGames { get; set; }
					 public int iTotalProfileRecycles { get; set; }

					 // Related to the profile reloaded when restarting games, to pick the FIRST profile.
					 // Also storing a list of all profiles, for experimental reasons/incase I want to use them down the line
					 public List<string> listProfilesLoaded { get; set; }
					 public string sLastProfileSeen { get; set; }
					 public string sFirstProfileSeen { get; set; }
					 public DateTime lastProfileCheck { get; set; }
				}
		  }
    
}