using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using FunkyBot.XMLTags;
using FunkyBot.Cache;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Game;

namespace FunkyBot
{
	public partial class EventHandlers
	{
		internal static bool DumpedDeathInfo = false;

		internal static void FunkyOnDeath(object src, EventArgs mea)
		{
			if (!DumpedDeathInfo)
			{
				Logger.Write(LogLevel.User, "Death Info Dump \r\n ProfileBehavior {0} \r\n Last Target Behavior {1} \r\n" +
							"{2} \r\n" +
							"Triggering Avoidances={3} -- RequiredAvoidance={4} -- LastAvoidAction={5} \r\n" +
							"Nearby Flee Triggering Units={6} -- LastFleeAction={7} \r\n",
							Bot.Game.Profile.CurrentProfileBehavior.GetType().ToString(), Bot.Targeting.lastBehavioralType.ToString(),
							ObjectCache.Objects.DumpDebugInfo(),
							Bot.Targeting.Environment.TriggeringAvoidances.Count, Bot.Targeting.RequiresAvoidance, Bot.Targeting.LastAvoidanceMovement.ToString(CultureInfo.InvariantCulture),
							Bot.Targeting.Environment.FleeTriggeringUnits.Count, Bot.Targeting.LastFleeAction.ToString(CultureInfo.InvariantCulture));

				DumpedDeathInfo = true;
			}

			//if (DateTime.Now.Subtract(Bot.Stats.lastDied).TotalSeconds>10)
			//{

			//Bot.Stats.lastDied=DateTime.Now;
			//Bot.Stats.iTotalDeaths++;
			//Bot.Stats.iDeathsThisRun++;
			//Bot.BotStatistics.GameStats.TotalDeaths++;

			Funky.ResetBot();


			// Does Trinity need to handle deaths?
			if (TrinityMaxDeathsTag.MaxDeathsAllowed > 0)
			{
				if (Bot.Game.CurrentGameStats.TotalDeaths > TrinityMaxDeathsTag.MaxDeathsAllowed)
				{
					Logger.DBLog.InfoFormat("[Funky] You have died too many times. Now restarting the game.");

					string profile = GlobalSettings.Instance.LastProfile;

					if (Bot.Game.CurrentGameStats.Profiles.Count > 0)
						profile = Bot.Game.CurrentGameStats.Profiles.First().ProfileName;

					ProfileManager.Load(profile);
					Thread.Sleep(1000);
					Funky.ResetGame();
					ZetaDia.Service.Party.LeaveGame();
					Thread.Sleep(10000);
				}
				else
				{
					Logger.DBLog.InfoFormat("[Funky] I'm sorry, but I seem to have let you die :( Now restarting the current profile.");
					ProfileManager.Load(GlobalSettings.Instance.LastProfile);
					Thread.Sleep(3500);
				}
			}

			//}
		}
	}
}