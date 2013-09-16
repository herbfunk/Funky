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
using FunkyTrinity.Enums;
using FunkyTrinity.Cache;
using System.Diagnostics;
using System.Windows;

namespace FunkyTrinity
{

    public partial class Funky
    {
		  internal static readonly CacheObject FakeCacheObject=new CacheObject(Vector3.Zero, TargetType.None, 0d, "Fake Target", 1f, -1);

		  private static bool bPluginEnabled=false;
		  private static bool initFunkyButton=false;
		  private static bool initTreeHooks=false;
		  private static bool bMaintainStatTracking=false;

        // **********************************************************************************************
        // *****   A few special variables, mainly for Giles use, just at the top for easy access   *****
        // **********************************************************************************************

        #region Constants

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
	    internal static Interpreter ItemRulesEval;

		 internal static string DBLogFile;

        // Status text for DB main window status
	    internal static string sStatusText = "";
		  // Do we need to reset the debug bar after combat handling?
	    internal static bool bResetStatusText=false;

		 private static void buttonFunkySettingDB_Click(object sender, RoutedEventArgs e)
		 {
			  Bot.UpdateCurrentAccountDetails();

			  string settingsFolder=FolderPaths.sDemonBuddyPath+@"\Settings\FunkyTrinity\"+Bot.CurrentAccountName;
			  if (!Directory.Exists(settingsFolder)) Directory.CreateDirectory(settingsFolder);

			  try
			  {
					funkyConfigWindow=new FunkyWindow();
					funkyConfigWindow.Show();
			  } catch (Exception ex)
			  {
					Logging.WriteVerbose("Failure to initilize Funky Setting Window! \r\n {0} \r\n {1} \r\n {2}", ex.Message, ex.Source, ex.StackTrace);
			  }
		 }

		 internal static FunkyWindow funkyConfigWindow;

        // A list of "useonceonly" tags that have been triggered this xml profile
		  internal static HashSet<int> hashUseOnceID=new HashSet<int>();
		  internal static Dictionary<int, int> dictUseOnceID=new Dictionary<int, int>();
        // For the random ID tag
		  internal static Dictionary<int, int> dictRandomID=new Dictionary<int, int>();

		  internal static IntPtr D3Handle
		  {
				get
				{
						
					 Process p=Process.GetProcessById(ZetaDia.Memory.Process.Id);
					 return p.MainWindowHandle;
				}
		  }

		  public static class FolderPaths
		  {
				internal static string sDemonBuddyPath=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				internal static string sTrinityPluginPath=sDemonBuddyPath+@"\Plugins\FunkyTrinity\";
				internal static string sTrinityLogPath=sDemonBuddyPath+@"\Plugins\FunkyTrinity\Log\";
				internal static string SettingsDefaultPath=Path.Combine(Funky.FolderPaths.sTrinityPluginPath, "Config", "Defaults");
				internal static string sTrinityLogScreenShotPath
				{
					 get
					 {
						  string path=Path.Combine(sTrinityLogPath, @"ScreenShots\");
						  if (!System.IO.Directory.Exists(path))
								System.IO.Directory.CreateDirectory(path);

						  return path;
					 }
				}

				internal static string sFunkySettingsPath
				{
					 get
					 {
						  if (Bot.CurrentAccountName==null) 
								Bot.UpdateCurrentAccountDetails();

						  string sFunkyCharacterFolder=Path.Combine(FolderPaths.sDemonBuddyPath, "Settings", "FunkyTrinity", Bot.CurrentAccountName);
						  if (!System.IO.Directory.Exists(sFunkyCharacterFolder))
						  {
								Logging.WriteDiagnostic("Creating Funky Settings Folder @ {0}", sFunkyCharacterFolder);
								System.IO.Directory.CreateDirectory(sFunkyCharacterFolder);
						  }

						  return sFunkyCharacterFolder;
					 }
				}
				internal static string sFunkySettingsCurrentPath
				{
					 get
					 {
						  if (Bot.CurrentHeroName==null)
								Bot.UpdateCurrentAccountDetails();

						  return Path.Combine(sFunkySettingsPath, Bot.CurrentHeroName+".xml");

					 }
				}
		  }
    }
}