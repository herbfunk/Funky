using System;
using System.Linq;
using Zeta.Common;
using Zeta;
using Zeta.TreeSharp;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Zeta.Navigation;
using Zeta.Pathfinding;
using Zeta.CommonBot;
using Zeta.Common.Plugins;
using System.IO;
using System.Threading;
using Decorator=Zeta.TreeSharp.Decorator;
using Action=Zeta.TreeSharp.Action;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using Zeta.Internals.Actors;

namespace FunkyTrinity
{
	 public partial class Funky : IPlugin
	 {
		  public Version Version { get { return new Version(2, 4, 0, 3); } }
		  public string Author { get { return "Herbfunk"; } }
		  public string Description
		  {
				get
				{
					 return "FunkyTrinity version "+Version;
				}
		  }
		  public string Name { get { return "FunkyTrinity"; } }
		  public bool Equals(IPlugin other) { return (other.Name==Name)&&(other.Version==Version); }

		 
		  public void OnInitialize()
		  {
				bool BotWasRunning=BotMain.IsRunning;
				Demonbuddy.SplitButton FunkyButton=null;


				BotMain.OnStop+=new BotEvent(FunkyBotStop);
				BotMain.OnStart+=new BotEvent(FunkyBotStart);


				bool FunkyCombatRoutineCurrent=Zeta.CommonBot.RoutineManager.Current!=null&&
					 !String.IsNullOrEmpty(Zeta.CommonBot.RoutineManager.Current.Name)&&Zeta.CommonBot.RoutineManager.Current.Name=="Funky";

				if (FunkyCombatRoutineCurrent)
				{
					 #region FunkyButtonHandlerHook
					 try
					 {

						  Window mainWindow=Demonbuddy.App.Current.MainWindow;
						  var tab=mainWindow.FindName("tabControlMain") as TabControl;
						  if (tab==null) return;
						  var infoDumpTab=tab.Items[0] as TabItem;
						  if (infoDumpTab==null) return;
						  var grid=infoDumpTab.Content as Grid;
						  if (grid==null) return;
						  FunkyButton=grid.FindName("Funky") as Demonbuddy.SplitButton;
						  if (FunkyButton!=null)
						  {
								Logging.WriteDiagnostic("Funky Button handler added");
						  }
						  else
						  {
								Demonbuddy.SplitButton[] splitbuttons=grid.Children.OfType<Demonbuddy.SplitButton>().ToArray();
								if (splitbuttons.Any())
								{

									 foreach (var item in splitbuttons)
									 {
										  if (item.Name.Contains("Funky"))
										  {
												FunkyButton=item;
												break;
										  }
									 }
								}
						  }
						  initFunkyButton=true;
					 } catch
					 {
						  Logging.Write("Could not find Funky Button! Safely handled exception.");
						  initFunkyButton=false;
					 }
					 #endregion
				}

				string sRoutinePath=FolderPaths.sDemonBuddyPath+@"\Routines\Funky\";
				string sPluginRoutineFolder=FolderPaths.sDemonBuddyPath+@"\Plugins\FunkyTrinity\CombatRoutine\";

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

					 FunkyButton=null;
					 FolderPaths.sDemonBuddyPath=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
					 Logging.WriteDiagnostic("Reloading combat routine..");

					 //Create Folder
					 if (!System.IO.Directory.Exists(sRoutinePath))
						  System.IO.Directory.CreateDirectory(sRoutinePath);

					 //Copy Files
					 System.IO.File.Copy(sPluginRoutineFolder+"CombatRoutine", sRoutinePath+"CombatRoutine.cs", true);
					 System.IO.File.Copy(sPluginRoutineFolder+"RoutineDebug", sRoutinePath+"RoutineDebug.cs", true);


					 //Recompile Routine
					 //Zeta.Common.Compiler.CodeCompiler FunkyRoutineCode=new Zeta.Common.Compiler.CodeCompiler(sRoutinePath);
					 //FunkyRoutineCode.ParseFilesForCompilerOptions();
					 //FunkyRoutineCode.Compile();
					 //Logging.WriteDiagnostic(FunkyRoutineCode.CompiledToLocation);

					 //Reload Routines
					 Zeta.CommonBot.RoutineManager.Reload();

					 //remove
					 System.IO.File.Delete(sRoutinePath+"RoutineDebug.cs");
					 System.IO.File.Delete(sRoutinePath+"CombatRoutine.cs");
					 System.IO.Directory.Delete(sRoutinePath);

					 //Search again..
					 bool funkyRoutine=Zeta.CommonBot.RoutineManager.Routines.Any(r => r.Name=="Funky");
					 if (funkyRoutine)
					 {
						  Logging.WriteDiagnostic("Setting Combat Routine to Funky");
						  Zeta.CommonBot.RoutineManager.Current=Zeta.CommonBot.RoutineManager.Routines.First(r => r.Name=="Funky");
						  Zeta.CommonBot.RoutineManager.SetCurrentCombatRoutine();

						  #region FunkyButtonHandlerHook
						  try
						  {

								Window mainWindow=Demonbuddy.App.Current.MainWindow;
								var tab=mainWindow.FindName("tabControlMain") as TabControl;
								if (tab==null) return;
								var infoDumpTab=tab.Items[0] as TabItem;
								if (infoDumpTab==null) return;
								var grid=infoDumpTab.Content as Grid;
								if (grid==null) return;
								FunkyButton=grid.FindName("Funky") as Demonbuddy.SplitButton;
								if (FunkyButton!=null)
								{
									 Logging.WriteDiagnostic("Funky Button handler added");
								}
								else
								{
									 Demonbuddy.SplitButton[] splitbuttons=grid.Children.OfType<Demonbuddy.SplitButton>().ToArray();
									 if (splitbuttons.Any())
									 {

										  foreach (var item in splitbuttons)
										  {
												if (item.Name.Contains("Funky"))
												{
													 FunkyButton=item;
													 break;
												}
										  }
									 }
								}
								initFunkyButton=true;
						  } catch (Exception ex)
						  {
								Logging.Write("Could not find Funky Button! Safely handled exception.");
								initFunkyButton=false;
								Logging.WriteVerbose(ex.Message+"\r\n"+ex.StackTrace+"\r\n"+ex.Source);
						  }
						  #endregion

					 }
					 else
					 {
						  //Failed to find the routine..
						  Logging.Write("Failure to set Funky Routine because Routines cannot be reloaded again! Closing Demonbuddy...");
						  Process.GetCurrentProcess().CloseMainWindow();
						  return;
					 }
				}



				if (initFunkyButton&&FunkyButton!=null)
				{
					 Logging.WriteDiagnostic("Funky Split Button Click Handler Added");
					 FunkyButton.Click+=buttonFunkySettingDB_Click;
				}

				DBLogFile=Zeta.Common.Logging.LogFilePath;
				Logger.Write(LogLevel.User, "Init Logger Completed! DB Log Path Set {0}", DBLogFile);
				//if (BotWasRunning) BotMain.Start();


		  }

		  public void OnPulse()
		  {
		  }

		  public void OnEnabled()
		  {
				try
				{
					 CleanLogs();
				} catch (Exception ex)
				{

					 Logging.Write("failure to clean logs! __ "+ex.Message+"\r"+
							ex.StackTrace);
				}




				if (!Directory.Exists(FolderPaths.sTrinityPluginPath))
				{
					 Log("Fatal Error - cannot enable plugin. Invalid path: "+FolderPaths.sTrinityPluginPath);
					 Log("Please check you have installed the plugin to the correct location, and then restart DemonBuddy and re-enable the plugin.");
					 Log(@"Plugin should be installed to \<DemonBuddyFolder>\Plugins\FunkyTrinity\");
				}
				else
				{
					 bPluginEnabled=true;

					 try
					 {
						  MainWindow.Title="DB - "+ZetaDia.Service.CurrentHero.BattleTagName;
					 } catch{}


					 // Safety check incase DB "OnStart" event didn't fire properly
					 if (BotMain.IsRunning) 
						  FunkyBotStart(null);


					 
					 
					 // Carguy's ticks-per-second feature
					 //if (settings.bEnableTPS)
					 //  BotMain.TicksPerSecond=(int)settings.iTPSAmount;


					 ErrorClickerThread=new Thread(ErrorClickerWorker);
					 ErrorClickerThread.IsBackground=true;
					 ErrorClickerThread.Start();
					 System.IO.FileInfo PluginInfo=new FileInfo(FolderPaths.sDemonBuddyPath+@"\Plugins\FunkyTrinity\");
					 //
					 string CompileDateString=PluginInfo.LastWriteTime.ToString("MM/dd hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
					 Log("************************************");
					 Log("ENABLED: Funky Trinity Plugin");
					 Log(" -- Version -- "+Version);
					 Log("\tModified: "+CompileDateString);
					 Log("************************************");

					 string profile=Zeta.CommonBot.ProfileManager.CurrentProfile!=null?Zeta.CommonBot.ProfileManager.CurrentProfile.Name:Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile;
					 Logging.Write("Loaded Profile "+profile);

					 CheckUpdate();

					 iDemonbuddyMonsterPowerLevel=Zeta.CommonBot.Settings.CharacterSettings.Instance.MonsterPowerLevel;
				}
		  }
		  public Window DisplayWindow
		  {
				get
				{
					 Bot.UpdateCurrentAccountDetails();
					 string settingsFolder=FolderPaths.sDemonBuddyPath+@"\Settings\FunkyTrinity\"+Bot.CurrentAccountName;
					 if (!Directory.Exists(settingsFolder))
						  Directory.CreateDirectory(settingsFolder);
					 try
					 {
						  funkyConfigWindow=new FunkyWindow();

					 } catch (Exception ex)
					 {
						  Logging.WriteVerbose("Failure to initilize Funky Setting Window! \r\n {0} \r\n {1} \r\n {2}", ex.Message, ex.Source, ex.StackTrace);
					 }

					 return funkyConfigWindow;
				}
		  }
		  public void OnDisabled()
		  {
				bPluginEnabled=false;

				if (ErrorClickerThread!=null&&ErrorClickerThread.ThreadState==System.Threading.ThreadState.Background)
				{
					 Logging.WriteDiagnostic("Error Dialog Thread Aborting..");
					 ErrorClickerThread.Abort();
				}

				Log("DISABLED: FunkyPlugin has shut down...");
		  }

		  public void OnShutdown()
		  {
				BotMain.OnStart-=FunkyBotStart;
				BotMain.OnStop-=FunkyBotStop;
				RemoveHandlers();
				ResetTreehooks();

				if (ErrorClickerThread!=null&&ErrorClickerThread.ThreadState==System.Threading.ThreadState.Background)
				{
					 Logging.WriteDiagnostic("Error Dialog Thread Aborting..");
					 ErrorClickerThread.Abort();
				}


				#region FunkyButtonHandlerRemove
				Window mainWindow=Demonbuddy.App.Current.MainWindow;
				var tab=mainWindow.FindName("tabControlMain") as TabControl;
				if (tab==null) return;
				var infoDumpTab=tab.Items[0] as TabItem;
				if (infoDumpTab==null) return;
				var grid=infoDumpTab.Content as Grid;
				if (grid==null) return;

				Demonbuddy.SplitButton[] splitbuttons=grid.Children.OfType<Demonbuddy.SplitButton>().ToArray();
				if (splitbuttons.Any())
				{

					 foreach (var item in splitbuttons)
					 {
						  if (item.Name.Contains("Funky"))
						  {
								Logging.WriteDiagnostic("Funky Split Button Click Handler Removed");
								item.Click-=buttonFunkySettingDB_Click;
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
					 return System.Windows.Application.Current.MainWindow;
				}
		  }
		  internal void RemoveHandlers()
		  {
				GameEvents.OnLevelUp-=OnPlayerLevelUp;
				GameEvents.OnPlayerDied-=FunkyOnDeath;
				GameEvents.OnGameJoined-=FunkyOnJoinGame;
				GameEvents.OnGameLeft-=FunkyOnLeaveGame;
				GameEvents.OnGameChanged-=FunkyOnGameChanged;
		  }

		  internal void ResetTreehooks()
		  {
				Navigator.PlayerMover=new DefaultPlayerMover();
				Navigator.StuckHandler=new DefaultStuckHandler();
				CombatTargeting.Instance.Provider=new DefaultCombatTargetingProvider();
				LootTargeting.Instance.Provider=new DefaultLootTargetingProvider();
				ObstacleTargeting.Instance.Provider=new DefaultObstacleTargetingProvider();

				Zeta.CommonBot.Logic.BrainBehavior.CreateCombatLogic();
				Zeta.CommonBot.Logic.BrainBehavior.CreateLootBehavior();
				Zeta.CommonBot.Logic.BrainBehavior.CreateVendorBehavior();
		  }






	 }
}