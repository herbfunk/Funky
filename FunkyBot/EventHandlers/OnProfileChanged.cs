using System;
using Zeta.CommonBot;

namespace FunkyBot
{
    public partial class EventHandlers
    {
        // Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
        internal static void FunkyOnProfileChanged(object src, EventArgs mea)
        {
            string sThisProfile = ProfileManager.CurrentProfile.Path;

			if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Event))
				Logger.Write(LogLevel.Event, "Profile Changed to {0}", sThisProfile);

            //Update Tracker
            Bot.Game.CurrentGameStats.ProfileChanged(sThisProfile);
        }
    }
}