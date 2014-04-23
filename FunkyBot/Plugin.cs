using System;
using System.Globalization;
using System.Linq;
using Demonbuddy;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Misc;
using FunkyBot.Settings;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Common;
using System.Windows;
using System.Windows.Controls;
using Zeta.Common.Plugins;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace FunkyBot
{
	public partial class Funky : IPlugin
	{
		public Version Version { get { return new Version(2, 9, 1, 0); } }
		public string Author { get { return "Herbfunk"; } }
		public string Description
		{
			get
			{
				return "FunkyBot version " + Version;
			}
		}
		public string Name { get { return "FunkyBot"; } }
		public bool Equals(IPlugin other) { return (other.Name == Name) && (other.Version == Version); }

		public void OnInitialize()
		{
			bool BotWasRunning = BotMain.IsRunning;
			SplitButton FunkyButton = null;


			bool FunkyCombatRoutineCurrent = RoutineManager.Current != null &&
				 !String.IsNullOrEmpty(RoutineManager.Current.Name) && RoutineManager.Current.Name == "Funky";

			if (FunkyCombatRoutineCurrent)
			{
				#region FunkyButtonHandlerHook
				try
				{

					Window mainWindow = App.Current.MainWindow;
					var tab = mainWindow.FindName("tabControlMain") as TabControl;
					if (tab == null) return;
					var infoDumpTab = tab.Items[0] as TabItem;
					if (infoDumpTab == null) return;
					var grid = infoDumpTab.Content as Grid;
					if (grid == null) return;
					FunkyButton = grid.FindName("Funky") as SplitButton;
					if (FunkyButton != null)
					{
						Logger.DBLog.DebugFormat("Funky Button handler added");
					}
					else
					{
						SplitButton[] splitbuttons = grid.Children.OfType<SplitButton>().ToArray();
						if (splitbuttons.Any())
						{

							foreach (var item in splitbuttons)
							{
								if (item.Name.Contains("Funky"))
								{
									FunkyButton = item;
									break;
								}
							}
						}
					}
					initFunkyButton = true;
				}
				catch
				{
					Logger.DBLog.InfoFormat("Could not find Funky Button! Safely handled exception.");
					initFunkyButton = false;
				}
				#endregion
			}

			string sRoutinePath = FolderPaths.sDemonBuddyPath + @"\Routines\Funky\";
			string sPluginRoutineFolder = FolderPaths.sDemonBuddyPath + @"\Plugins\FunkyBot\CombatRoutine\";
		
			//DateTime RoutineCombatDate=System.IO.File.GetLastWriteTime(sRoutinePath+"CombatRoutine.cs");
			//DateTime RoutineDebugDate=System.IO.File.GetLastWriteTime(sRoutinePath+"RoutineDebug.cs");
			//DateTime PluginCombatDate=System.IO.File.GetLastWriteTime(sPluginRoutineFolder+"CombatRoutine");
			//DateTime PluginDebugDate=System.IO.File.GetLastWriteTime(sPluginRoutineFolder+"RoutineDebug");
			//bool RoutineFilesMismatch=RoutineCombatDate.CompareTo(PluginCombatDate)>0||RoutineDebugDate.CompareTo(PluginDebugDate)>0;
			if (!initFunkyButton)
			{
				//stop bot
				if (BotWasRunning)
				{
					// Zeta.CommonBot.BotMain.Stop();
				}

				FunkyButton = null;
				FolderPaths.sDemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Logger.DBLog.DebugFormat("Reloading combat routine..");

				//Create Folder
				if (!Directory.Exists(sRoutinePath))
					Directory.CreateDirectory(sRoutinePath);

				//Copy Files
				File.Copy(sPluginRoutineFolder + "CombatRoutine", sRoutinePath + "CombatRoutine.cs", true);
				File.Copy(sPluginRoutineFolder + "RoutineDebug", sRoutinePath + "RoutineDebug.cs", true);


				//Recompile Routine
				//Zeta.Common.Compiler.CodeCompiler FunkyRoutineCode=new Zeta.Common.Compiler.CodeCompiler(sRoutinePath);
				//FunkyRoutineCode.ParseFilesForCompilerOptions();
				//FunkyRoutineCode.Compile();
				//Logger.DBLog.DebugFormat(FunkyRoutineCode.CompiledToLocation);

				//Reload Routines
				RoutineManager.Reload();

				//remove
				File.Delete(sRoutinePath + "RoutineDebug.cs");
				File.Delete(sRoutinePath + "CombatRoutine.cs");
				Directory.Delete(sRoutinePath);

				//Search again..
				bool funkyRoutine = RoutineManager.Routines.Any(r => r.Name == "Funky");
				if (funkyRoutine)
				{
					Logger.DBLog.DebugFormat("Setting Combat Routine to Funky");
					RoutineManager.Current = RoutineManager.Routines.First(r => r.Name == "Funky");

					#region FunkyButtonHandlerHook
					try
					{

						FunkyButton = FindFunkyButton();
						initFunkyButton = true;
					}
					catch (Exception ex)
					{
						Logger.DBLog.InfoFormat("Could not find Funky Button! Safely handled exception.");
						initFunkyButton = false;
						Logger.DBLog.InfoFormat(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Source);
					}
					#endregion

				}
				else
				{
					//Failed to find the routine..
					Logger.DBLog.InfoFormat("Failure to set Funky Routine because Routines cannot be reloaded again! Closing Demonbuddy...");
					Process.GetCurrentProcess().CloseMainWindow();
					return;
				}
			}



			if (initFunkyButton && FunkyButton != null)
			{
				Logger.DBLog.DebugFormat("Funky Split Button Click Handler Added");
				FunkyButton.Click += FunkyWindow.buttonFunkySettingDB_Click;
			}

			ObjectCache.SnoUnitPropertyCache = CacheUnitIDs.DeserializeFromXML();
			ObjectCache.SnoObjectPropertyCache = CacheObjectIDs.DeserializeFromXML();

			ObjectCache.FakeCacheObject = new CacheObject(Vector3.Zero, TargetType.None, 0d, "Fake Target", 1f);
			
			//Update Account Details..
			Bot.Character.Account.UpdateCurrentAccountDetails();
			Bot.Settings = new Settings_Funky();

			Logger.DBLogFile = DateTime.Now.ToString("yyyy-MM-dd hh.mm") + ".txt";
			Logger.Write(LogLevel.User, "Init Logger Completed!");

			//Generate Checksum for Update Check
			//Process.Start(FolderPaths.sTrinityPluginPath + @"\CheckSum.exe");
			//if (Updater.UpdateAvailable())
			//{
			//	string dbPathString = Assembly.GetEntryAssembly().Location;
			//	string dbExePath = Path.GetFullPath(dbPathString);
			//	Process.GetCurrentProcess().CloseMainWindow();
			//	Process.Start(dbExePath);
			//	return;
			//}
		}

		public void OnPulse()
		{
		}

		public void OnEnabled()
		{
			try
			{
				Logger.CleanLogs();
			}
			catch (Exception ex)
			{

				Logger.DBLog.InfoFormat("failure to clean logs! __ " + ex.Message + "\r" +
					   ex.StackTrace);
			}




			if (!Directory.Exists(FolderPaths.sTrinityPluginPath))
			{
				Logger.DBLog.InfoFormat("Fatal Error - cannot enable plugin. Invalid path: " + FolderPaths.sTrinityPluginPath);
				Logger.DBLog.InfoFormat("Please check you have installed the plugin to the correct location, and then restart DemonBuddy and re-enable the plugin.");
				Logger.DBLog.InfoFormat(@"Plugin should be installed to \<DemonBuddyFolder>\Plugins\FunkyBot\");
			}
			else
			{
				bPluginEnabled = true;

				// Safety check incase DB "OnStart" event didn't fire properly
				if (BotMain.IsRunning)
					EventHandlers.FunkyBotStart(null);

				// Carguy's ticks-per-second feature
				//if (settings.bEnableTPS)
				//  BotMain.TicksPerSecond=(int)settings.iTPSAmount;

				FileInfo PluginInfo = new FileInfo(FolderPaths.sDemonBuddyPath + @"\Plugins\FunkyBot\");
				//
				string CompileDateString = PluginInfo.LastWriteTime.ToString("MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture);
				Logger.DBLog.InfoFormat("************************************");
				Logger.DBLog.InfoFormat("Funky Bot Plugin -- Enabled!");
				Logger.DBLog.InfoFormat(" -- Version -- " + Version);
				Logger.DBLog.InfoFormat("Modified: " + CompileDateString);
				Logger.DBLog.InfoFormat("************************************");

				//string profile=Zeta.CommonBot.ProfileManager.CurrentProfile!=null?Zeta.CommonBot.ProfileManager.CurrentProfile.Name:Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile;
				//Logger.DBLog.InfoFormat("Loaded Profile "+profile);

				//CheckUpdate();

				iDemonbuddyMonsterPowerLevel = CharacterSettings.Instance.MonsterPowerLevel;
				BotMain.OnStop += EventHandlers.FunkyBotStop;
				BotMain.OnStart += EventHandlers.FunkyBotStart;

			}
		}
		public Window DisplayWindow
		{
			get
			{
				string settingsFolder = FolderPaths.sDemonBuddyPath + @"\Settings\FunkyBot\" + Bot.Character.Account.CurrentAccountName;
				if (!Directory.Exists(settingsFolder))
					Directory.CreateDirectory(settingsFolder);
				try
				{
					FunkyWindow.funkyConfigWindow = new FunkyWindow();

				}
				catch (Exception ex)
				{
					Logger.DBLog.InfoFormat("Failure to initilize Funky Setting Window! \r\n {0} \r\n {1} \r\n {2}", ex.Message, ex.Source, ex.StackTrace);
				}

				return FunkyWindow.funkyConfigWindow;
			}
		}
		public void OnDisabled()
		{
			bPluginEnabled = false;
			BotMain.OnStop -= EventHandlers.FunkyBotStop;
			BotMain.OnStart -= EventHandlers.FunkyBotStart;
			RemoveHandlers();

			if (Funky.initTreeHooks) ResetTreehooks();

			#region FunkyButtonHandlerRemove
			Window mainWindow = App.Current.MainWindow;
			var tab = mainWindow.FindName("tabControlMain") as TabControl;
			if (tab == null) return;
			var infoDumpTab = tab.Items[0] as TabItem;
			if (infoDumpTab == null) return;
			var grid = infoDumpTab.Content as Grid;
			if (grid == null) return;

			SplitButton[] splitbuttons = grid.Children.OfType<SplitButton>().ToArray();
			if (splitbuttons.Any())
			{

				foreach (var item in splitbuttons)
				{
					if (item.Name.Contains("Funky"))
					{
						Logger.DBLog.DebugFormat("Funky Split Button Click Handler Removed");
						item.Click -= FunkyWindow.buttonFunkySettingDB_Click;
						break;
					}
				}
			}
			#endregion

			ResetGame();

			Logger.DBLog.InfoFormat("DISABLED: FunkyPlugin has shut down...");
		}

		public void OnShutdown()
		{
			BotMain.OnStart -= EventHandlers.FunkyBotStart;
			BotMain.OnStop -= EventHandlers.FunkyBotStop;
			RemoveHandlers();
			ResetTreehooks();

			#region FunkyButtonHandlerRemove
			Window mainWindow = App.Current.MainWindow;
			var tab = mainWindow.FindName("tabControlMain") as TabControl;
			if (tab == null) return;
			var infoDumpTab = tab.Items[0] as TabItem;
			if (infoDumpTab == null) return;
			var grid = infoDumpTab.Content as Grid;
			if (grid == null) return;

			SplitButton[] splitbuttons = grid.Children.OfType<SplitButton>().ToArray();
			if (splitbuttons.Any())
			{

				foreach (var item in splitbuttons)
				{
					if (item.Name.Contains("Funky"))
					{
						Logger.DBLog.DebugFormat("Funky Split Button Click Handler Removed");
						item.Click -= FunkyWindow.buttonFunkySettingDB_Click;
						break;
					}
				}
			}
			#endregion

			ResetGame();
		}
		internal static Window MainWindow
		{
			get
			{
				return Application.Current.MainWindow;
			}
		}
		internal static void RemoveHandlers()
		{
			GameEvents.OnPlayerDied -= EventHandlers.FunkyOnDeath;
			GameEvents.OnGameJoined -= EventHandlers.FunkyOnJoinGame;
			GameEvents.OnGameLeft -= EventHandlers.FunkyOnLeaveGame;
			GameEvents.OnGameChanged -= EventHandlers.FunkyOnGameChanged;
			ProfileManager.OnProfileLoaded -= EventHandlers.FunkyOnProfileChanged;
		}

		internal static void ResetTreehooks()
		{
			Navigator.PlayerMover = new DefaultPlayerMover();
			Navigator.StuckHandler = new DefaultStuckHandler();
			CombatTargeting.Instance.Provider = new DefaultCombatTargetingProvider();
			LootTargeting.Instance.Provider = new DefaultLootTargetingProvider();
			ObstacleTargeting.Instance.Provider = new DefaultObstacleTargetingProvider();

			BrainBehavior.CreateCombatLogic();
			BrainBehavior.CreateLootBehavior();
			BrainBehavior.CreateVendorBehavior();
		}
		internal static float Difference(float A, float B)
		{
			if (A > B)
				return A - B;

			return B - A;
		}

	}
}