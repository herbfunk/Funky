using System;
using Zeta;
using System.Collections.Generic;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.Internals.Actors;
using System.Xml;
using FunkyTrinity.Cache;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal static void Log(string message, bool bIsDiagnostic=false)
		  {
				string totalMessage=String.Format("[Funky] {0}", message);
				if (!bIsDiagnostic)
					 Logging.Write(totalMessage);
				else
					 Logging.WriteDiagnostic(totalMessage);
		  }

		  public static void ResetBot()
		  {
				Log("Preforming reset of bot data...", true);
			  ObjectCache.ClearBlacklistCollections();
				PowerCacheLookup.dictAbilityLastUse=new Dictionary<SNOPower, DateTime>(PowerCacheLookup.dictAbilityLastUseDefaults);

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

				//Bot.NavigationCache.UpdateSearchGridProvider();

				DumpedDeathInfo=false;
		  }
		  public static void ResetGame()
		  {
				ResetBot();

				hashUseOnceID=new HashSet<int>();
				dictUseOnceID=new Dictionary<int, int>();
				dictRandomID=new Dictionary<int, int>();
				FunkyTrinity.Movement.SkipAheadCache.ClearCache();

				Bot.Stats.iMaxDeathsAllowed=0;
				Bot.Stats.iDeathsThisRun=0;
				_hashsetItemStatsLookedAt=new HashSet<int>();
				_hashsetItemPicksLookedAt=new HashSet<int>();
				_hashsetItemFollowersIgnored=new HashSet<int>();
				TownRunManager._dictItemStashAttempted=new Dictionary<int, int>();

				Bot.Stats.listProfilesLoaded=new List<string>();
				Bot.Stats.sLastProfileSeen="";
				Bot.Stats.sFirstProfileSeen="";

		  }


		  private static void HookBehaviorTree()
		  {

				bool townportal=false, idenify=false, stash=false, vendor=false, salvage=false, looting=true, combat=true;

				using (XmlReader reader=XmlReader.Create(FolderPaths.sTrinityPluginPath+"Treehooks.xml"))
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

						  ActionDelegate actionDelegatePrePause=new ActionDelegate(TownRunManager.GilesStashPrePause);
						  ActionDelegate actionDelegatePause=new ActionDelegate(TownRunManager.GilesStashPause);

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
						  CanRunDecoratorDelegate canRunDelegateStashGilesPreStashPause=new CanRunDecoratorDelegate(TownRunManager.GilesPreStashPauseOverlord);
						  Sequence sequencepause=new Sequence(
								  new Zeta.TreeSharp.Action(actionDelegatePrePause),
								  new Zeta.TreeSharp.Action(actionDelegatePause)
								  );

						  GilesReplacement.Children[3]=new Zeta.TreeSharp.Decorator(canRunDelegateStashGilesPreStashPause, sequencepause);


						  if (stash)
						  {
								// Replace DB stashing behavior tree with my optimized version with loot rule replacement
								CanRunDecoratorDelegate canRunDelegateStashGilesOverlord=new CanRunDecoratorDelegate(TownRunManager.GilesStashOverlord);
								ActionDelegate actionDelegatePreStash=new ActionDelegate(TownRunManager.GilesOptimisedPreStash);
								ActionDelegate actionDelegateStashing=new ActionDelegate(TownRunManager.GilesOptimisedStash);
								ActionDelegate actionDelegatePostStash=new ActionDelegate(TownRunManager.GilesOptimisedPostStash);
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
								CanRunDecoratorDelegate canRunDelegateSellGilesOverlord=new CanRunDecoratorDelegate(TownRunManager.GilesSellOverlord);
								ActionDelegate actionDelegatePreSell=new ActionDelegate(TownRunManager.GilesOptimisedPreSell);
								ActionDelegate actionDelegateSell=new ActionDelegate(TownRunManager.GilesOptimisedSell);
								ActionDelegate actionDelegatePostSell=new ActionDelegate(TownRunManager.GilesOptimisedPostSell);
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
								CanRunDecoratorDelegate canRunDelegateSalvageGilesOverlord=new CanRunDecoratorDelegate(TownRunManager.GilesSalvageOverlord);
								ActionDelegate actionDelegatePreSalvage=new ActionDelegate(TownRunManager.GilesOptimisedPreSalvage);
								ActionDelegate actionDelegateSalvage=new ActionDelegate(TownRunManager.GilesOptimisedSalvage);
								ActionDelegate actionDelegatePostSalvage=new ActionDelegate(TownRunManager.GilesOptimisedPostSalvage);
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

						  CanRunDecoratorDelegate canRunUnidBehavior=new CanRunDecoratorDelegate(TownRunManager.UnidItemOverlord);
						  ActionDelegate actionDelegatePreUnidStash=new ActionDelegate(TownRunManager.UnidStashOptimisedPreStash);
						  ActionDelegate actionDelegatePostUnidStash=new ActionDelegate(TownRunManager.UnidStashOptimisedPostStash);
						  ActionDelegate actionDelegateUnidBehavior=new ActionDelegate(TownRunManager.UnidStashBehavior);
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


						  CanRunDecoratorDelegate canRunDelegateGilesTownRunCheck=new CanRunDecoratorDelegate(TownRunManager.GilesTownRunCheckOverlord);
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
    }
}