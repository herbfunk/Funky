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

		  private static Zeta.CommonBot.Profile.ProfileBehavior CurrentProfileBehavior=null;
		  private static bool IsRunningTownPortalBehavior=false;
        private static bool initFunkyButton = false;
        private static bool initTreeHooks = false;

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

		  private static GilesSettings settings=new GilesSettings();

        // Darkfriend's Looting Rule
		  private static Interpreter ItemRulesEval;

        // A null location, may shave off the tiniest fraction of CPU time, but probably not. Still, I like using this variable! :D
        private static readonly Vector3 vNullLocation = Vector3.Zero;

        // Used to force-refresh dia objects at least once every XX milliseconds 
        private static DateTime lastRefreshedObjects = DateTime.Today;

        // Status text for DB main window status
        private static string sStatusText = "";

        // A "fake" object to send to target provider for stuck handlers etc.
		  internal static DiaObject thisFakeObject;

		  
        // Related to the profile reloaded when restarting games, to pick the FIRST profile.
        // Also storing a list of all profiles, for experimental reasons/incase I want to use them down the line
		  private static List<string> listProfilesLoaded=new List<string>();
		  private static string sLastProfileSeen="";
		  private static string sFirstProfileSeen="";
		  private static DateTime lastProfileCheck=DateTime.Today;


        // Do we need to reset the debug bar after combat handling?
        private static bool bResetStatusText = false;

        // A list of "useonceonly" tags that have been triggered this xml profile
		  internal static HashSet<int> hashUseOnceID=new HashSet<int>();
		  internal static Dictionary<int, int> dictUseOnceID=new Dictionary<int, int>();
        // For the random ID tag
		  internal static Dictionary<int, int> dictRandomID=new Dictionary<int, int>();

        // Death counts
		  internal static int iMaxDeathsAllowed=0;
		  internal static int iDeathsThisRun=0;

		  // On death, clear the timers for all abilities
		  private static DateTime lastDied=DateTime.Today;
		  private static int iTotalDeaths=0;
		  // How many total leave games, for stat-tracking?
		  private static int iTotalJoinGames=0;
		  // How many total leave games, for stat-tracking?
		  private static int iTotalLeaveGames=0;
		  private static int iTotalProfileRecycles=0;


		  private static List<int> ValidClusterUnits=new List<int>();
		  private static DateTime LastClusterTargetLogicRefresh=DateTime.Today;

		  
        // Total main loops so we can update things every XX loops
        private static int iCombatLoops = 0;

        // The number of loops to extend kill range for after a fight to try to maximize kill bonus exp etc.
		  private static int iKeepKillRadiusExtendedFor=0;
        // The number of loops to extend loot range for after a fight to try to stop missing loot
        private static int iKeepLootRadiusExtendedFor = 0;

        // How many follower items were ignored, purely for item stat tracking
        private static int iTotalFollowerItemsIgnored = 0;

        // Random variables used during item handling and town-runs
        private static int iItemDelayLoopLimit = 0;
        private static int iCurrentItemLoops = 0;
        private static bool bLoggedAnythingThisStash = false;
        private static bool bUpdatedStashMap = false;
        private static bool bLoggedJunkThisStash = false;
        private static string sValueItemStatString = "";
        private static string sJunkItemStatString = "";
        private static bool bTestingBackpack = false;


		  // This dictionary stores attempted stash counts on items, to help detect any stash stucks on the same item etc.
		  private static Dictionary<int, int> _dictItemStashAttempted=new Dictionary<int, int>();

        // Stash mapper - it's an array representing every slot in your stash, true or false dictating if the slot is free or not
        private static bool[,] GilesStashSlotBlocked = new bool[7, 30];

        private static bool bOutputItemScores = false;

        // Target provider and core routine variables

        private static string sDemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string sTrinityPluginPath = sDemonBuddyPath + @"\Plugins\FunkyTrinity\";
        private static string sTrinityLogPath = sDemonBuddyPath + @"\Plugins\FunkyTrinity\Log\";
		  private static string sTrinityConfigFile=sDemonBuddyPath+@"\Settings\FunkyTrinity\FunkyTrinity_GilesSettings.cfg";
		  private static string sFunkyConfigFile=sDemonBuddyPath+@"\Settings\FunkyTrinity\FunkyTrinity_FunkySettings.cfg";

        private static bool bSavingConfig = false;

        private static readonly string[] sQualityString = new string[4] { "White", "Magic", "Rare", "Legendary" };
        private static readonly string[] sGemString = new string[4] { "Ruby", "Topaz", "Amethyst", "Emerald" };
        private static DateTime ItemStatsLastPostedReport = DateTime.Now;
        private static DateTime ItemStatsWhenStartedBot = DateTime.Now;
        private static bool bMaintainStatTracking = false;

        // Store items already logged by item-stats, to make sure no stats get doubled up by accident
        private static HashSet<int> _hashsetItemStatsLookedAt = new HashSet<int>();
        private static HashSet<int> _hashsetItemPicksLookedAt = new HashSet<int>();
        private static HashSet<int> _hashsetItemFollowersIgnored = new HashSet<int>();


        // These objects are instances of my stats class above, holding identical types of data for two different things - one holds item DROP stats, one holds item PICKUP stats
        private static GilesItemStats ItemsDroppedStats = new GilesItemStats(0, new double[4], new double[64], new double[4, 64], 0, new double[64], 0, new double[4], new double[64], new double[4, 64], 0);
        private static GilesItemStats ItemsPickedStats = new GilesItemStats(0, new double[4], new double[64], new double[4, 64], 0, new double[64], 0, new double[4], new double[64], new double[4, 64], 0);



        // Readable names of the above stats that get output into the trash/stash log files
        private static readonly string[] StatNames = new string[29] { 
            "Dexterity", "Intelligence", "Strength", "Vitality", 
            "Life %", "Life On Hit", "Life Steal %", "Life Regen", 
            "Magic Find %", "Gold Find   %", "Movement Speed %", "Pickup Radius", "Sockets", 
            "Crit Chance %", "Crit Damage %", "Attack Speed %", "+Min Damage", "+Max Damage",
            "Total Block %", "Thorns", "+All Resist", "+Highest Single Resist", "DPS", "Armor", "Max Disc.", "Max Mana", "Arcane-On-Crit", "Mana Regen", "Globe Bonus"};
        // Stores the apparent maximums of each stat for each item slot
        // Note that while these SHOULD be *actual* maximums for most stats - for things like DPS, these can just be more sort of "what a best-in-slot DPS would be"
        //												             Dex  Int  Str  Vit  Life%     LOH Steal%  LPS Magic% Gold% MSPD Rad. Sox Crit% CDam% ASPD Min+ Max+ Block% Thorn Allres Res   DPS ARMOR Disc.Mana Arc. Regen  Globes
        #region ItemAttributeWeights

        //Weapons/Offhand
        private static double[] iMaxWeaponOneHand = new double[29] { 
            320, 320, 320, 320, 
            0, 850, 3, 0,
            0, 0, 0, 0, 1,
            0, 100, 0, 0, 0,
            0, 0, 0, 0, 1429, 0, 10, 150, 10, 14, 0 };
        private static double[] iMaxWeaponTwoHand = new double[29] { 
            530, 530, 530, 530,
            0, 1800, 6, 0,
            0, 0, 0, 0, 1,
            0, 200, 0, 0, 0,
            0, 0, 0, 0, 1680, 0, 10, 119, 10, 14, 0 };
        private static double[] iMaxWeaponRanged = new double[29] {  
            320, 320, 320, 320,
            0, 850, 3, 0,
            0, 0, 0, 0, 1,
            0, 100, 0, 0, 0,
            0, 0, 0, 0, 1618, 0, 0, 0, 0, 14, 0 };
        private static double[] iMaxOffHand = new double[29] {       
            300, 300, 300, 300,
            9, 0, 0, 234,
            18, 20, 0, 0, 1,
            8.5, 0, 15, 110, 402,
            0, 979, 0, 0, 0, 0, 10, 119, 10, 11, 12794 };
        private static double[] iMaxShield = new double[29] {        
            330, 330, 330, 330,
            16, 0, 0, 342,
            20, 25, 0, 0, 1,
            10, 0, 0, 0, 0,
            30, 2544, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };

        //Ring/Ammy
        private static double[] iMaxRing = new double[29] {          
            200, 200, 200, 200,
            12, 479, 0, 340,
            20, 25, 0, 0, 1,
            6, 50, 9, 36, 100,
            0, 979, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
        private static double[] iMaxAmulet = new double[29] {        
            350, 350, 350, 350,
            16, 959, 0, 599,
            45, 50, 0, 0, 1,
            10, 100, 9, 36, 100,
            0, 1712, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };

        //Armor
        private static double[] iMaxShoulders = new double[29] {     
            200, 200, 300, 200,
            12, 0, 0, 342,
            20, 25, 0, 7, 0,
            0, 0, 0, 0, 0,
            0, 2544, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
        private static double[] iMaxHelm = new double[29] {          
            200, 300, 200, 200,
            12, 0, 0, 342,
            20, 25, 0, 7, 1,
            6, 0, 0, 0, 0,
            0, 1454, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };
        private static double[] iMaxPants = new double[29] {         
            200, 200, 200, 300,
            0, 0, 0, 342,
            20, 25, 0, 7, 2,
            0, 0, 0, 0, 0,
            0, 1454, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };
        private static double[] iMaxGloves = new double[29] {        
            300, 300, 200, 200,
            0, 0, 0, 342,
            20, 25, 0, 7, 0,
            10, 50, 9, 0, 0,
            0, 1454, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
        private static double[] iMaxChest = new double[29] {        
            200, 200, 200, 300,
            12, 0, 0, 599,
            20, 25, 0, 7, 3,
            0, 0, 0, 0, 0,
            0, 2544, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };
        private static double[] iMaxBracer = new double[29] {        
            200, 200, 200, 200,
            0, 0, 0, 342,
            20, 25, 0, 7, 0,
            6, 0, 0, 0, 0,
            0, 1454, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
        private static double[] iMaxBoots = new double[29] {         
            300, 200, 200, 200,
            0, 0, 0, 342,
            20, 25, 12, 7, 0,
            0, 0, 0, 0, 0,
            0, 1454, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
        private static double[] iMaxBelt = new double[29] {          
            200, 200, 300, 200,
            12, 0, 0, 342,
            20, 25, 0, 7, 0,
            0, 0, 0, 0, 0,
            0, 2544, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };

        private static double[] iMaxCloak = new double[29] { 200, 200, 200, 300, 12, 0, 0, 410, 20, 25, 0, 7, 3, 0, 0, 0, 0, 0, 0, 2544, 70, 50, 0, 397, 10, 0, 0, 0, 12794 };
        private static double[] iMaxMightyBelt = new double[29] { 200, 200, 300, 200, 12, 0, 3, 342, 20, 25, 0, 7, 0, 0, 0, 0, 0, 0, 0, 2544, 70, 50, 0, 265, 0, 0, 0, 0, 12794 };
        private static double[] iMaxSpiritStone = new double[29] { 200, 300, 200, 200, 12, 0, 0, 342, 20, 25, 0, 7, 1, 6, 0, 0, 0, 0, 0, 1454, 70, 50, 0, 397, 0, 0, 0, 0, 12794 };
        private static double[] iMaxVoodooMask = new double[29] { 200, 300, 200, 200, 12, 0, 0, 342, 20, 25, 0, 7, 1, 6, 0, 0, 0, 0, 0, 1454, 70, 50, 0, 397, 0, 119, 0, 11, 12794 };
        private static double[] iMaxWizardHat = new double[29] { 200, 300, 200, 200, 12, 0, 0, 342, 20, 25, 0, 7, 1, 6, 0, 0, 0, 0, 0, 1454, 70, 50, 0, 397, 0, 0, 10, 0, 12794 };

        private static double[] iMaxFollower = new double[29] { 200, 200, 200, 200, 0, 300, 0, 234, 0, 0, 0, 0, 0, 0, 55, 0, 0, 0, 0, 0, 50, 40, 0, 0, 0, 0, 0, 0, 0 };

        // Stores the total points this stat is worth at the above % point of maximum
        // Note that these values get all sorts of bonuses, multipliers, and extra things applied in the actual scoring routine. These values are more of a "base" value.
        //                                                              Dex    Int    Str    Vit    Life%  LOH    Steal% LPS   Magic%  Gold%  MSPD   Rad  Sox    Crit%  CDam%  ASPD   Min+  Max+ Block% Thorn Allres Res   DPS    ARMOR  Disc.  Mana  Arc.  Regen  Globes
        private static double[] iWeaponPointsAtMax = new double[29] { 14000, 14000, 14000, 14000, 13000, 20000, 7000, 1000, 6000, 6000, 6000, 500, 16000, 15000, 15000, 0, 0, 0, 0, 1000, 11000, 0, 64000, 0, 10000, 8500, 8500, 10000, 8000 };
        //                                                              Dex    Int    Str    Vit    Life%  LOH    Steal% LPS   Magic%  Gold%  MSPD   Rad. Sox    Crit%  CDam%  ASPD   Min+  Max+ Block% Thorn Allres Res   DPS    ARMOR  Disc.  Mana  Arc.  Regen  Globes
        private static double[] iArmorPointsAtMax = new double[29] { 11000, 11000, 11000, 9500, 9000, 10000, 4000, 1200, 3000, 3000, 3500, 1000, 4300, 9000, 6100, 7000, 3000, 3000, 5000, 1200, 7500, 1500, 0, 5000, 4000, 3000, 3000, 6000, 5000 };
        private static double[] iJewelryPointsAtMax = new double[29] { 11500, 11000, 11000, 10000, 8000, 11000, 4000, 1200, 4500, 4500, 3500, 1000, 3500, 7500, 6300, 6800, 800, 800, 5000, 1200, 7500, 1500, 0, 4500, 4000, 3000, 3000, 6000, 5000 };

        // Some special values for score calculations
        // BonusThreshold is a percentage of the "max-stat possible", that the stat starts to get a multiplier on it's score. 1 means it has to be above 100% of the "max-stat" to get a multiplier (so only possible if the max-stat isn't ACTUALLY the max possible)
        // MinimumThreshold is a percentage of the "max-stat possible", that the stat will simply be ignored for being too low. eg if set to .5 - then anything less than 50% of the max-stat will be ignored.
        // MinimumPrimary is used for some stats only - and means that at least ONE primary stat has to be above that level, to get score. Eg magic-find has .5 - meaning any item without at least 50% of a max-stat primary, will ignore magic-find scoring.
        //                                                             Dex  Int  Str  Vit  Life%  LOH  Steal%   LPS Magic% Gold% MSPD Radi  Sox  Crit% CDam% ASPD  Min+  Max+  Block%  Thorn  Allres  Res   DPS  ARMOR   Disc. Mana  Arc. Regen  Globes
        private static double[] iBonusThreshold = new double[29] { .75, .75, .75, .75, .80, .80, .9, 1, 1, 1, .95, 1, 1, .70, .90, 1, .9, .9, .83, 1, .85, .95, .90, .90, 1, 1, 1, .9, 1 };
        private static double[] iMinimumThreshold = new double[29] { .40, .40, .40, .30, .60, .45, .7, .7, .64, .64, .75, .8, .4, .40, .60, .40, .2, .2, .65, .6, .40, .55, .40, .80, .7, .7, .7, .7, .8 };
        private static double[] iStatMinimumPrimary = new double[29] { 0, 0, 0, 0, 0, 0, 0, .2, .50, .50, .30, 0, 0, 0, 0, 0, .40, .40, .40, .40, .40, .40, 0, .40, .40, .40, .40, .4, .4 };

        #endregion

        // Whether to try forcing a vendor-run for custom reasons
        private static bool bWantToTownRun = false;
        private static bool bLastTownRunCheckResult = false;
        private static bool bReachedSafety = false;
        // DateTime check to prevent inventory-check spam when looking for repairs being needed
        private static DateTime TimeLastCheckedForTownRun = DateTime.Now;
        private static bool bCurrentlyMoving = false;
        private static bool bNeedsEquipmentRepairs = false;




		  private static bool bPluginEnabled=false;
    }
}