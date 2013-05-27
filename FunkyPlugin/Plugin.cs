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

[assembly: AssemblyVersionAttribute("1.0.20.*")]
namespace FunkyTrinity
{
	 public partial class Funky : IPlugin
	 {
		  public Version Version { get { return new Version(1, 6, 3, 4); } }
		  public Version SubVersion { get { return new Version(0, 1, 0, 20); } }

		  public string Author { get { return "Herbfunk"; } }
		  public string Description
		  {
				get
				{
					 return "FunkyTrinity version "+SubVersion+"\t\r\n"+"Extension of GilesTrinity v1.6.3.4";
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

				string sRoutinePath=sDemonBuddyPath+@"\Routines\Funky\";
				string sPluginRoutineFolder=sDemonBuddyPath+@"\Plugins\FunkyTrinity\CombatRoutine\";

				DateTime RoutineCombatDate=System.IO.File.GetLastWriteTime(sRoutinePath+"CombatRoutine.cs");
				DateTime RoutineDebugDate=System.IO.File.GetLastWriteTime(sRoutinePath+"RoutineDebug.cs");
				DateTime PluginCombatDate=System.IO.File.GetLastWriteTime(sPluginRoutineFolder+"CombatRoutine");
				DateTime PluginDebugDate=System.IO.File.GetLastWriteTime(sPluginRoutineFolder+"RoutineDebug");
				bool RoutineFilesMismatch=RoutineCombatDate.CompareTo(PluginCombatDate)>0||RoutineDebugDate.CompareTo(PluginDebugDate)>0;
				if (RoutineFilesMismatch||!initFunkyButton)
				{
					 //stop bot
					 if (BotWasRunning)
					 {
						  // Zeta.CommonBot.BotMain.Stop();
					 }

					 FunkyButton=null;
					 sDemonBuddyPath=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
					 Logging.WriteDiagnostic("Reloading combat routine..");

					 //Create Folder
					 if (!System.IO.Directory.Exists(sRoutinePath))
						  System.IO.Directory.CreateDirectory(sRoutinePath);

					 //Copy Files
					 System.IO.File.Copy(sPluginRoutineFolder+"CombatRoutine", sRoutinePath+"CombatRoutine.cs", true);
					 System.IO.File.Copy(sPluginRoutineFolder+"RoutineDebug", sRoutinePath+"RoutineDebug.cs", true);


					 //Recompile Routine
					 Zeta.Common.Compiler.CodeCompiler FunkyRoutineCode=new Zeta.Common.Compiler.CodeCompiler(sRoutinePath);
					 FunkyRoutineCode.ParseFilesForCompilerOptions();
					 FunkyRoutineCode.Compile();
					 Logging.WriteDiagnostic(FunkyRoutineCode.CompiledToLocation);


					 //remove
					 //System.IO.File.Delete(sRoutinePath+"RoutineDebug.cs");
					 //System.IO.File.Delete(sRoutinePath+"CombatRoutine.cs");
					 //System.IO.Directory.Delete(sRoutinePath);


					 //Reload Routines
					 Zeta.CommonBot.RoutineManager.Reload();

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




				if (!Directory.Exists(sTrinityPluginPath))
				{
					 Log("Fatal Error - cannot enable plugin. Invalid path: "+sTrinityPluginPath);
					 Log("Please check you have installed the plugin to the correct location, and then restart DemonBuddy and re-enable the plugin.");
					 Log(@"Plugin should be installed to \<DemonBuddyFolder>\Plugins\FunkyTrinity\");
				}
				else
				{
					 bPluginEnabled=true;

					 // Safety check incase DB "OnStart" event didn't fire properly
					 if (BotMain.IsRunning)
					 {
						  FunkyBotStart(null);
					 }


					 // Carguy's ticks-per-second feature
					 if (settings.bEnableTPS)
					 {
						  BotMain.TicksPerSecond=(int)settings.iTPSAmount;
					 }

					 ErrorClickerThread=new Thread(ErrorClickerWorker);
					 ErrorClickerThread.IsBackground=true;
					 ErrorClickerThread.Start();
					 System.IO.FileInfo PluginInfo=new FileInfo(sDemonBuddyPath+@"\Plugins\FunkyTrinity\");
					 //
					 string CompileDateString=PluginInfo.LastWriteTime.ToString("MM/dd hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
					 Log("************************************");
					 Log("ENABLED: Funky Trinity Plugin");
					 Log(" -- Version -- "+SubVersion);
					 Log("\tModified: "+CompileDateString);
					 Log("************************************");

					 string profile=Zeta.CommonBot.ProfileManager.CurrentProfile!=null?Zeta.CommonBot.ProfileManager.CurrentProfile.Name:Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile;
					 Logging.Write("Loaded Profile "+profile);
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


		  private static void HookBehaviorTree()
		  {

				bool townportal=false, idenify=false, stash=false, vendor=false, salvage=false, looting=true, combat=true;

				using (XmlReader reader=XmlReader.Create(sTrinityPluginPath+"Treehooks.xml"))
				{

					 // Parse the XML document.  ReadString is used to 
					 // read the text content of the elements.
					 reader.Read();
					 reader.ReadStartElement("Treehooks");
					 reader.Read();
					 while (reader.LocalName!="Treehooks")
					 {
						  switch (reader.LocalName)
						  {
								case "Townportal":
									 townportal=Convert.ToBoolean(reader.ReadInnerXml());
									 break;
								case "Identification":
									 idenify=Convert.ToBoolean(reader.ReadInnerXml());
									 break;
								case "Stash":
									 stash=Convert.ToBoolean(reader.ReadInnerXml());
									 break;
								case "Vendor":
									 vendor=Convert.ToBoolean(reader.ReadInnerXml());
									 break;
								case "Salvage":
									 salvage=Convert.ToBoolean(reader.ReadInnerXml());
									 break;
								case "Looting":
									 looting=Convert.ToBoolean(reader.ReadInnerXml());
									 break;
								case "Combat":
									 combat=Convert.ToBoolean(reader.ReadInnerXml());
									 break;
						  }

						  reader.Read();
					 }
				}

				Logging.WriteVerbose("[Funky] Replacing Treehooks..");
				#region TreeHooks
				foreach (var hook in TreeHooks.Instance.Hooks)
				{
					 // Replace the combat behavior tree, as that happens first and so gets done quicker!
					 if (hook.Key.Contains("Combat"))
					 {
						  if (combat)
						  {
								// Replace the pause just after identify stuff to ensure we wait before trying to run to vendor etc.
								CanRunDecoratorDelegate canRunDelegateCombatTargetCheck=new CanRunDecoratorDelegate(GlobalOverlord);
								ActionDelegate actionDelegateCoreTarget=new ActionDelegate(HandleTarget);
								Sequence sequencecombat=new Sequence(
										new Zeta.TreeSharp.Action(actionDelegateCoreTarget)
										);
								hook.Value[0]=new Zeta.TreeSharp.Decorator(canRunDelegateCombatTargetCheck, sequencecombat);
								Logging.WriteDiagnostic("Combat Tree replaced...");
						  }

					 } // Vendor run hook
					 // Wipe the vendorrun and loot behavior trees, since we no longer want them
					 if (hook.Key.Contains("VendorRun"))
					 {
						  Zeta.TreeSharp.Decorator GilesDecorator=hook.Value[0] as Zeta.TreeSharp.Decorator;
						  //PrioritySelector GilesReplacement = GilesDecorator.Children[0] as PrioritySelector;
						  PrioritySelector GilesReplacement=GilesDecorator.Children[0] as PrioritySelector;

						  //[1] == Return to town
						  if (townportal)
						  {
								CanRunDecoratorDelegate canRunDelegateReturnToTown=new CanRunDecoratorDelegate(FunkyTPOverlord);
								ActionDelegate actionDelegateReturnTown=new ActionDelegate(FunkyTPBehavior);
								Sequence sequenceReturnTown=new Sequence(
									new Zeta.TreeSharp.Action(actionDelegateReturnTown)
									);
								GilesReplacement.Children[1]=new Zeta.TreeSharp.Decorator(canRunDelegateReturnToTown, sequenceReturnTown);
								Logging.WriteDiagnostic("Town Run - Town Portal - hooked...");
						  }

						  ActionDelegate actionDelegatePrePause=new ActionDelegate(GilesStashPrePause);
						  ActionDelegate actionDelegatePause=new ActionDelegate(GilesStashPause);

						  if (idenify)
						  {
								//[2] == IDing items in inventory
								CanRunDecoratorDelegate canRunDelegateFunkyIDBehavior=new CanRunDecoratorDelegate(FunkyIDOverlord);
								ActionDelegate actionDelegateID=new ActionDelegate(FunkyIDBehavior);
								Sequence sequenceIDItems=new Sequence(
										new Zeta.TreeSharp.Action(actionDelegateID),
										new Sequence(
										new Zeta.TreeSharp.Action(actionDelegatePrePause),
										new Zeta.TreeSharp.Action(actionDelegatePause)
										)
										);
								GilesReplacement.Children[2]=new Zeta.TreeSharp.Decorator(canRunDelegateFunkyIDBehavior, sequenceIDItems);
								Logging.WriteDiagnostic("Town Run - Idenify Items - hooked...");
						  }


						  // Replace the pause just after identify stuff to ensure we wait before trying to run to vendor etc.
						  CanRunDecoratorDelegate canRunDelegateStashGilesPreStashPause=new CanRunDecoratorDelegate(GilesPreStashPauseOverlord);
						  Sequence sequencepause=new Sequence(
								  new Zeta.TreeSharp.Action(actionDelegatePrePause),
								  new Zeta.TreeSharp.Action(actionDelegatePause)
								  );

						  GilesReplacement.Children[3]=new Zeta.TreeSharp.Decorator(canRunDelegateStashGilesPreStashPause, sequencepause);
						  

						  if (stash)
						  {
								// Replace DB stashing behavior tree with my optimized version with loot rule replacement
								CanRunDecoratorDelegate canRunDelegateStashGilesOverlord=new CanRunDecoratorDelegate(GilesStashOverlord);
								ActionDelegate actionDelegatePreStash=new ActionDelegate(GilesOptimisedPreStash);
								ActionDelegate actionDelegateStashing=new ActionDelegate(GilesOptimisedStash);
								ActionDelegate actionDelegatePostStash=new ActionDelegate(GilesOptimisedPostStash);
								Sequence sequencestash=new Sequence(
										new Zeta.TreeSharp.Action(actionDelegatePreStash),
										new Zeta.TreeSharp.Action(actionDelegateStashing),
										new Zeta.TreeSharp.Action(actionDelegatePostStash),
										new Sequence(
										new Zeta.TreeSharp.Action(actionDelegatePrePause),
										new Zeta.TreeSharp.Action(actionDelegatePause)
										)
										);
								GilesReplacement.Children[4]=new Zeta.TreeSharp.Decorator(canRunDelegateStashGilesOverlord, sequencestash);
								Logging.WriteDiagnostic("Town Run - Stash - hooked...");
						  }

						  if (vendor)
						  {
								// Replace DB vendoring behavior tree with my optimized & "one-at-a-time" version
								CanRunDecoratorDelegate canRunDelegateSellGilesOverlord=new CanRunDecoratorDelegate(GilesSellOverlord);
								ActionDelegate actionDelegatePreSell=new ActionDelegate(GilesOptimisedPreSell);
								ActionDelegate actionDelegateSell=new ActionDelegate(GilesOptimisedSell);
								ActionDelegate actionDelegatePostSell=new ActionDelegate(GilesOptimisedPostSell);
								Sequence sequenceSell=new Sequence(
										new Zeta.TreeSharp.Action(actionDelegatePreSell),
										new Zeta.TreeSharp.Action(actionDelegateSell),
										new Zeta.TreeSharp.Action(actionDelegatePostSell),
										new Sequence(
										new Zeta.TreeSharp.Action(actionDelegatePrePause),
										new Zeta.TreeSharp.Action(actionDelegatePause)
										)
										);
								GilesReplacement.Children[5]=new Zeta.TreeSharp.Decorator(canRunDelegateSellGilesOverlord, sequenceSell);
								Logging.WriteDiagnostic("Town Run - Vendor - hooked...");
						  }

						  if (salvage)
						  {
								// Replace DB salvaging behavior tree with my optimized & "one-at-a-time" version
								CanRunDecoratorDelegate canRunDelegateSalvageGilesOverlord=new CanRunDecoratorDelegate(GilesSalvageOverlord);
								ActionDelegate actionDelegatePreSalvage=new ActionDelegate(GilesOptimisedPreSalvage);
								ActionDelegate actionDelegateSalvage=new ActionDelegate(GilesOptimisedSalvage);
								ActionDelegate actionDelegatePostSalvage=new ActionDelegate(GilesOptimisedPostSalvage);
								Sequence sequenceSalvage=new Sequence(
										new Zeta.TreeSharp.Action(actionDelegatePreSalvage),
										new Zeta.TreeSharp.Action(actionDelegateSalvage),
										new Zeta.TreeSharp.Action(actionDelegatePostSalvage),
										new Sequence(
										new Zeta.TreeSharp.Action(actionDelegatePrePause),
										new Zeta.TreeSharp.Action(actionDelegatePause)
										)
										);
								GilesReplacement.Children[6]=new Zeta.TreeSharp.Decorator(canRunDelegateSalvageGilesOverlord, sequenceSalvage);
								Logging.WriteDiagnostic("Town Run - Salvage - hooked...");
						  }

						  //[7] == Return to Townportal if there is one..
						  //CanRunDecoratorDelegate canRunDelegateUseTownPortalReturn = new CanRunDecoratorDelegate(

						  CanRunDecoratorDelegate canRunUnidBehavior=new CanRunDecoratorDelegate(UnidItemOverlord);
						  ActionDelegate actionDelegatePreUnidStash=new ActionDelegate(GilesOptimisedPreStash);
						  ActionDelegate actionDelegatePostUnidStash=new ActionDelegate(GilesOptimisedPostStash);
						  ActionDelegate actionDelegateUnidBehavior=new ActionDelegate(UnidStashBehavior);
						  Sequence sequenceUnidStash=new Sequence(
								  new Zeta.TreeSharp.Action(actionDelegatePreUnidStash),
								  new Zeta.TreeSharp.Action(actionDelegateUnidBehavior),
								  new Zeta.TreeSharp.Action(actionDelegatePostUnidStash),
								  new Sequence(
								  new Zeta.TreeSharp.Action(actionDelegatePrePause),
								  new Zeta.TreeSharp.Action(actionDelegatePause)
								  )
								);
						  GilesReplacement.InsertChild(2, new Zeta.TreeSharp.Decorator(canRunUnidBehavior, sequenceUnidStash));


						  CanRunDecoratorDelegate canRunDelegateGilesTownRunCheck=new CanRunDecoratorDelegate(GilesTownRunCheckOverlord);
						  hook.Value[0]=new Zeta.TreeSharp.Decorator(canRunDelegateGilesTownRunCheck, new PrioritySelector(GilesReplacement));

						  Logging.WriteDiagnostic("Vendor Run tree hooked...");
					 } // Vendor run hook
					 if (hook.Key.Contains("Loot"))
					 {
						  if (looting)
						  {
								// Replace the loot behavior tree with a blank one, as we no longer need it
								CanRunDecoratorDelegate canRunDelegateBlank=new CanRunDecoratorDelegate(BlankDecorator);
								ActionDelegate actionDelegateBlank=new ActionDelegate(BlankAction);
								Sequence sequenceblank=new Sequence(
										new Zeta.TreeSharp.Action(actionDelegateBlank)
										);
								hook.Value[0]=new Zeta.TreeSharp.Decorator(canRunDelegateBlank, sequenceblank);
								Logging.WriteDiagnostic("Loot tree replaced...");
						  }
						  else
						  {
						      CanRunDecoratorDelegate canRunDelegateBlank=new CanRunDecoratorDelegate(BlankDecorator);
								ActionDelegate actionDelegateBlank=new ActionDelegate(BlankAction);
								Sequence sequenceblank=new Sequence(
										new Zeta.TreeSharp.Action(actionDelegateBlank)
										);
								hook.Value[0]=new Zeta.TreeSharp.Decorator(canRunDelegateBlank, Zeta.CommonBot.Logic.BrainBehavior.CreateLootBehavior());
						  }
					 } // Vendor run hook
					 if (hook.Key.Contains("OutOfGame"))
					 {
						  Zeta.TreeSharp.PrioritySelector CompositeReplacement=hook.Value[0] as Zeta.TreeSharp.PrioritySelector;

						  CanRunDecoratorDelegate shouldPreformOutOfGameBehavior=new CanRunDecoratorDelegate(OutOfGameOverlord);
						  ActionDelegate actionDelgateOOGBehavior=new ActionDelegate(OutOfGameBehavior);
						  Sequence sequenceOOG=new Sequence(
								  new Zeta.TreeSharp.Action(actionDelgateOOGBehavior)
						  );
						  CompositeReplacement.Children.Insert(0, new Zeta.TreeSharp.Decorator(shouldPreformOutOfGameBehavior, sequenceOOG));
						  hook.Value[0]=CompositeReplacement;

						  Logging.WriteDiagnostic("Out of game tree hooked");
					 }
				}
				#endregion
				initTreeHooks=true;
		  }


		  private static bool BlankDecorator(object ret)
		  {
				return false;
		  }
		  private static RunStatus BlankAction(object ret)
		  {
				return RunStatus.Success;
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
		  internal static void ResetBot()
		  {
				Log("Preforming reset of bot data...", true);
				hashRGUIDIgnoreBlacklist=new HashSet<int>();
				hashRGUIDTemporaryIgnoreBlacklist=new HashSet<int>();
				dictAbilityLastUse=new Dictionary<SNOPower, DateTime>(dictAbilityLastUseDefaults);

				PlayerMover.iTotalAntiStuckAttempts=1;
				PlayerMover.vSafeMovementLocation=Vector3.Zero;
				PlayerMover.vOldPosition=Vector3.Zero;
				PlayerMover.iTimesReachedStuckPoint=0;
				PlayerMover.timeLastRecordedPosition=DateTime.Today;
				PlayerMover.timeStartedUnstuckMeasure=DateTime.Today;
				PlayerMover.iTimesReachedMaxUnstucks=0;
				PlayerMover.iCancelUnstuckerForSeconds=0;
				PlayerMover.timeCancelledUnstuckerFor=DateTime.Today;

				//Reset all data with bot (Playerdata, Combat Data)
				Bot.Reset();

				//OOC ID Flags
				Bot.Combat.ShouldCheckItemLooted=false;
				shouldPreformOOCItemIDing=false;

				//TP Behavior Reset
				ResetTPBehavior();

				//Sno Trim Timer Reset
				ObjectCache.cacheSnoCollection.ResetTrimTimer();
				//clear obstacles
				ObjectCache.Obstacles.Clear();
				ObjectCache.Objects.Clear();

				UpdateSearchGridProvider();
		  }
		  internal static void ResetGame()
		  {
				ResetBot();

				hashUseOnceID=new HashSet<int>();
				dictUseOnceID=new Dictionary<int, int>();
				dictRandomID=new Dictionary<int, int>();
				CacheMovementTracking.ClearCache();

				iMaxDeathsAllowed=0;
				iDeathsThisRun=0;
				_hashsetItemStatsLookedAt=new HashSet<int>();
				_hashsetItemPicksLookedAt=new HashSet<int>();
				_hashsetItemFollowersIgnored=new HashSet<int>();
				_dictItemStashAttempted=new Dictionary<int, int>();

				listProfilesLoaded=new List<string>();
				sLastProfileSeen="";
				sFirstProfileSeen="";
				
		  }

		  private static void Log(string message, bool bIsDiagnostic=false)
		  {
				string totalMessage=String.Format("[Funky] {0}", message);
				if (!bIsDiagnostic)
					 Logging.Write(totalMessage);
				else
					 Logging.WriteDiagnostic(totalMessage);
		  }


	 }
}