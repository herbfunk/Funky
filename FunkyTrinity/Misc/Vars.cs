using System;
using System.Linq;
using System.Collections.Generic;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Pathfinding;
using Zeta.Internals.SNO;
using Zeta;
using System.Reflection;
using Zeta.Navigation;
using System.IO;

namespace FunkyTrinity
{

    public partial class Funky
    {
		  internal static readonly CacheObject FakeCacheObject=new CacheObject(vNullLocation, TargetType.None, 0d, "Fake Target", 1f, -1);

		  private static bool bPluginEnabled=false;
		  private static bool initFunkyButton=false;
		  private static bool initTreeHooks=false;
		  private static bool bMaintainStatTracking=false;

        // **********************************************************************************************
        // *****   A few special variables, mainly for Giles use, just at the top for easy access   *****
        // **********************************************************************************************

        #region Constants

        private const int RANGE_50 = 0;
        private const int RANGE_40 = 1;
        private const int RANGE_30 = 2;
        private const int RANGE_25 = 3;
        private const int RANGE_20 = 4;
        private const int RANGE_15 = 5;
        private const int RANGE_12 = 6;
        private const int RANGE_6 = 7;

        private const bool USE_COMBAT_ONLY = false;
        private const bool USE_ANY_TIME = true;
        private const bool SIGNATURE_SPAM = false;
        private const bool USE_SLOWLY = true;
        // These constants are used for item scoring and stashing
        private const int DEXTERITY = 0;
        private const int INTELLIGENCE = 1;
        private const int STRENGTH = 2;
        private const int VITALITY = 3;
        private const int LIFEPERCENT = 4;
        private const int LIFEONHIT = 5;
        private const int LIFESTEAL = 6;
        private const int LIFEREGEN = 7;
        private const int MAGICFIND = 8;
        private const int GOLDFIND = 9;
        private const int MOVEMENTSPEED = 10;
        private const int PICKUPRADIUS = 11;
        private const int SOCKETS = 12;
        private const int CRITCHANCE = 13;
        private const int CRITDAMAGE = 14;
        private const int ATTACKSPEED = 15;
        private const int MINDAMAGE = 16;
        private const int MAXDAMAGE = 17;
        private const int BLOCKCHANCE = 18;
        private const int THORNS = 19;
        private const int ALLRESIST = 20;
        private const int RANDOMRESIST = 21;
        private const int TOTALDPS = 22;
        private const int ARMOR = 23;
        private const int MAXDISCIPLINE = 24;
        private const int MAXMANA = 25;
        private const int ARCANECRIT = 26;
        private const int MANAREGEN = 27;
        private const int GLOBEBONUS = 28;
        private const int TOTALSTATS = 29; // starts at 0, remember... 0-26 = 1-27!

        private const int QUALITYWHITE = 0;
        private const int QUALITYBLUE = 1;
        private const int QUALITYYELLOW = 2;
        private const int QUALITYORANGE = 3;
        private const int GEMRUBY = 0;
        private const int GEMTOPAZ = 1;
        private const int GEMAMETHYST = 2;
        private const int GEMEMERALD = 3;
        #endregion

        // Darkfriend's Looting Rule
		  private static Interpreter ItemRulesEval;

        // A null location, may shave off the tiniest fraction of CPU time, but probably not. Still, I like using this variable! :D
        private static readonly Vector3 vNullLocation = Vector3.Zero;

        // Used to force-refresh dia objects at least once every XX milliseconds 
        private static DateTime lastRefreshedObjects = DateTime.Today;

        // Status text for DB main window status
        private static string sStatusText = "";
		  // Do we need to reset the debug bar after combat handling?
		  private static bool bResetStatusText=false;

		  
        // Related to the profile reloaded when restarting games, to pick the FIRST profile.
        // Also storing a list of all profiles, for experimental reasons/incase I want to use them down the line
		  private static List<string> listProfilesLoaded=new List<string>();
		  private static string sLastProfileSeen="";
		  private static string sFirstProfileSeen="";
		  private static DateTime lastProfileCheck=DateTime.Today;




        // A list of "useonceonly" tags that have been triggered this xml profile
		  internal static HashSet<int> hashUseOnceID=new HashSet<int>();
		  internal static Dictionary<int, int> dictUseOnceID=new Dictionary<int, int>();
        // For the random ID tag
		  internal static Dictionary<int, int> dictRandomID=new Dictionary<int, int>();



        // The number of loops to extend kill range for after a fight to try to maximize kill bonus exp etc.
		  private static int iKeepKillRadiusExtendedFor=0;
        // The number of loops to extend loot range for after a fight to try to stop missing loot
        private static int iKeepLootRadiusExtendedFor = 0;



		  public static class FolderPaths
		  {
				internal static string sDemonBuddyPath=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				internal static string sTrinityPluginPath=sDemonBuddyPath+@"\Plugins\FunkyTrinity\";
				internal static string sTrinityLogPath=sDemonBuddyPath+@"\Plugins\FunkyTrinity\Log\";
				
		  }
    }
}