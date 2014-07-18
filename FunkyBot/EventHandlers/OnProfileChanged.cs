using System;
using fBaseXtensions.Helpers;
using FunkyBot.Config.Settings;
using FunkyBot.Game;
using FunkyBot.Misc;
using Zeta.Bot;

namespace FunkyBot.EventHandlers
{
	public partial class EventHandlers
	{
		// Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
		internal static void FunkyOnProfileChanged(object src, EventArgs mea)
		{
			string sThisProfile = ProfileManager.CurrentProfile.Path;

			Logger.Write(LogLevel.Event, "Profile Changed to {0}", sThisProfile);

			//Clear Custom Cluster Settings
			SettingCluster.ClusterSettingsTag = Bot.Settings.Cluster;
			Bot.Game.QuestMode = false;
			SettingLOSMovement.LOSSettingsTag = Bot.Settings.LOSMovement;
			SettingAdventureMode.AdventureModeSettingsTag = Bot.Settings.AdventureMode;
			//ProfileCache.LineOfSightSNOIds.Clear();

			//Update Tracker
			//Bot.Game.CurrentGameStats.ProfileChanged(sThisProfile);
		}
	}
}