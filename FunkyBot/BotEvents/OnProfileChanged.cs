using System;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;
using Zeta.CommonBot.Settings;
using FunkyBot.Cache;
using FunkyBot.ProfileTracking;

namespace FunkyBot
{
    public partial class Funky
    {
        // Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
        internal static void FunkyOnProfileChanged(object src, EventArgs mea)
        {
            string sThisProfile = ProfileManager.CurrentProfile.Path;

            if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.User))
                Logger.Write(LogLevel.User, "Profile Changed to {0}", sThisProfile);

            //Update Tracker
            Bot.TrackingStats.ProfileChanged(sThisProfile);
        }
    }
}