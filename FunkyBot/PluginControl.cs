using System;
using System.Linq;
using System.Windows.Controls;
using Demonbuddy;
using FunkyBot.Cache;
using FunkyBot.Movement;
using System.Collections.Generic;
using FunkyBot.Player.Class;
using FunkyBot.XMLTags;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Logic;
using Zeta.Navigation;
using Zeta.TreeSharp;
using Zeta.Internals.Actors;
using System.Xml;
using System.Windows;
using Decorator = Zeta.TreeSharp.Decorator;
using FunkyBot.Game;
using Action = Zeta.TreeSharp.Action;
using FunkyBot.DBHandlers;

namespace FunkyBot
{
	public partial class Funky
	{
		internal static bool bPluginEnabled;
		private static bool initFunkyButton;
		internal static bool initTreeHooks;
		internal static int iDemonbuddyMonsterPowerLevel = 0;

		// Status text for DB main window status
		internal static string sStatusText = "";
		// Do we need to reset the debug bar after combat handling?
		internal static bool bResetStatusText = false;


		public static void ResetBot()
		{

			Logging.Write("Preforming reset of bot data...");
			BlacklistCache.ClearBlacklistCollections();

			PlayerMover.iTotalAntiStuckAttempts = 1;
			PlayerMover.vSafeMovementLocation = Vector3.Zero;
			PlayerMover.vOldPosition = Vector3.Zero;
			PlayerMover.iTimesReachedStuckPoint = 0;
			PlayerMover.timeLastRecordedPosition = DateTime.Today;
			PlayerMover.timeStartedUnstuckMeasure = DateTime.Today;
			PlayerMover.iTimesReachedMaxUnstucks = 0;
			PlayerMover.iCancelUnstuckerForSeconds = 0;
			PlayerMover.timeCancelledUnstuckerFor = DateTime.Today;

			//Reset all data with bot (Playerdata, Combat Data)
			Bot.Reset();
			PlayerClass.CreateBotClass();
			//Update character info!
			Bot.Character.Data.Update();

			//OOC ID Flags
			Bot.Targeting.ShouldCheckItemLooted = false;
			Bot.Targeting.CheckItemLootStackCount = 0;
			ItemIdentifyBehavior.shouldPreformOOCItemIDing = false;

			//TP Behavior Reset
			TownPortalBehavior.ResetTPBehavior();

			//Sno Trim Timer Reset
			ObjectCache.cacheSnoCollection.ResetTrimTimer();
			//clear obstacles
			ObjectCache.Obstacles.Clear();
			ObjectCache.Objects.Clear();
			EventHandlers.DumpedDeathInfo = false;
		}
		public static void ResetGame()
		{
			ProfileCache.hashUseOnceID = new HashSet<int>();
			ProfileCache.dictUseOnceID = new Dictionary<int, int>();
			ProfileCache.dictRandomID = new Dictionary<int, int>();
			SkipAheadCache.ClearCache();
			TownRunManager.TownrunStartedInTown = true;
			TrinityMaxDeathsTag.MaxDeathsAllowed = 0;
			TownRunManager._dictItemStashAttempted = new Dictionary<int, int>();
		}

		private static SplitButton FindFunkyButton()
		{
			Window mainWindow = App.Current.MainWindow;
			var tab = mainWindow.FindName("tabControlMain") as TabControl;
			if (tab == null) return null;
			var infoDumpTab = tab.Items[0] as TabItem;
			if (infoDumpTab == null) return null;
			var grid = infoDumpTab.Content as Grid;
			if (grid == null) return null;
			SplitButton FunkyButton = grid.FindName("Funky") as SplitButton;
			if (FunkyButton != null)
			{
				Logging.WriteDiagnostic("Funky Button handler added");
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

			return FunkyButton;
		}
		internal static void HookBehaviorTree()
		{

			bool townportal = false, idenify = false, stash = false, vendor = false, salvage = false, looting = true, combat = true;

			using (XmlReader reader = XmlReader.Create(FolderPaths.sTrinityPluginPath + "Treehooks.xml"))
			{

				// Parse the XML document.  ReadString is used to 
				// read the text content of the elements.
				reader.Read();
				reader.ReadStartElement("Treehooks");
				reader.Read();
				while (reader.LocalName != "Treehooks")
				{
					switch (reader.LocalName)
					{
						case "Townportal":
							townportal = Convert.ToBoolean(reader.ReadInnerXml());
							break;
						case "Identification":
							idenify = Convert.ToBoolean(reader.ReadInnerXml());
							break;
						case "Stash":
							stash = Convert.ToBoolean(reader.ReadInnerXml());
							break;
						case "Vendor":
							vendor = Convert.ToBoolean(reader.ReadInnerXml());
							break;
						case "Salvage":
							salvage = Convert.ToBoolean(reader.ReadInnerXml());
							break;
						case "Looting":
							looting = Convert.ToBoolean(reader.ReadInnerXml());
							break;
						case "Combat":
							combat = Convert.ToBoolean(reader.ReadInnerXml());
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
						CanRunDecoratorDelegate canRunDelegateCombatTargetCheck = GlobalOverlord;
						ActionDelegate actionDelegateCoreTarget = HandleTarget;
						Sequence sequencecombat = new Sequence(
								new Action(actionDelegateCoreTarget)
								);
						hook.Value[0] = new Decorator(canRunDelegateCombatTargetCheck, sequencecombat);
						Logging.WriteDiagnostic("Combat Tree replaced...");
					}

				} // Vendor run hook
				// Wipe the vendorrun and loot behavior trees, since we no longer want them
				if (hook.Key.Contains("VendorRun"))
				{
					Decorator GilesDecorator = hook.Value[0] as Decorator;
					//PrioritySelector GilesReplacement = GilesDecorator.Children[0] as PrioritySelector;
					PrioritySelector GilesReplacement = GilesDecorator.Children[0] as PrioritySelector;

					//[1] == Return to town
					if (townportal)
					{
						CanRunDecoratorDelegate canRunDelegateReturnToTown = TownPortalBehavior.FunkyTPOverlord;
						ActionDelegate actionDelegateReturnTown = TownPortalBehavior.FunkyTPBehavior;
						ActionDelegate actionDelegateTownPortalFinish = TownPortalBehavior.FunkyTownPortalTownRun;
						Sequence sequenceReturnTown = new Sequence(
							new Action(actionDelegateReturnTown),
							new Action(actionDelegateTownPortalFinish)
							);
						GilesReplacement.Children[1] = new Decorator(canRunDelegateReturnToTown, sequenceReturnTown);
						Logging.WriteDiagnostic("Town Run - Town Portal - hooked...");
					}

					ActionDelegate actionDelegatePrePause = TownRunManager.GilesStashPrePause;
					ActionDelegate actionDelegatePause = TownRunManager.GilesStashPause;

					if (idenify)
					{
						//[2] == IDing items in inventory
						CanRunDecoratorDelegate canRunDelegateFunkyIDBehavior = ItemIdentifyBehavior.FunkyIDOverlord;
						ActionDelegate actionDelegateID = ItemIdentifyBehavior.FunkyIDBehavior;
						Sequence sequenceIDItems = new Sequence(
								new Action(actionDelegateID),
								new Sequence(
								new Action(actionDelegatePrePause),
								new Action(actionDelegatePause)
								)
								);
						GilesReplacement.Children[2] = new Decorator(canRunDelegateFunkyIDBehavior, sequenceIDItems);
						Logging.WriteDiagnostic("Town Run - Idenify Items - hooked...");
					}


					// Replace the pause just after identify stuff to ensure we wait before trying to run to vendor etc.
					CanRunDecoratorDelegate canRunDelegateStashGilesPreStashPause = TownRunManager.GilesPreStashPauseOverlord;
					Sequence sequencepause = new Sequence(
							new Action(actionDelegatePrePause),
							new Action(actionDelegatePause)
							);

					GilesReplacement.Children[3] = new Decorator(canRunDelegateStashGilesPreStashPause, sequencepause);


					if (stash)
					{
						// Replace DB stashing behavior tree with my optimized version with loot rule replacement
						CanRunDecoratorDelegate canRunDelegateStashGilesOverlord = TownRunManager.GilesStashOverlord;
						ActionDelegate actionDelegatePreStash = TownRunManager.GilesOptimisedPreStash;
						ActionDelegate actionDelegateStashing = TownRunManager.GilesOptimisedStash;
						ActionDelegate actionDelegatePostStash = TownRunManager.GilesOptimisedPostStash;
						Sequence sequencestash = new Sequence(
								new Action(actionDelegatePreStash),
								new Action(actionDelegateStashing),
								new Action(actionDelegatePostStash),
								new Sequence(
								new Action(actionDelegatePrePause),
								new Action(actionDelegatePause)
								)
								);
						GilesReplacement.Children[4] = new Decorator(canRunDelegateStashGilesOverlord, sequencestash);
						Logging.WriteDiagnostic("Town Run - Stash - hooked...");
					}

					if (vendor)
					{
						// Replace DB vendoring behavior tree with my optimized & "one-at-a-time" version
						CanRunDecoratorDelegate canRunDelegateSellGilesOverlord = TownRunManager.GilesSellOverlord;
						ActionDelegate actionDelegatePreSell = TownRunManager.GilesOptimisedPreSell;
						ActionDelegate actionDelegateSell = TownRunManager.GilesOptimisedSell;
						ActionDelegate actionDelegatePostSell = TownRunManager.GilesOptimisedPostSell;
						Sequence sequenceSell = new Sequence(
								new Action(actionDelegatePreSell),
								new Action(actionDelegateSell),
								new Action(actionDelegatePostSell),
								new Sequence(
								new Action(actionDelegatePrePause),
								new Action(actionDelegatePause)
								)
								);
						GilesReplacement.Children[5] = new Decorator(canRunDelegateSellGilesOverlord, sequenceSell);
						Logging.WriteDiagnostic("Town Run - Vendor - hooked...");
					}

					if (salvage)
					{
						// Replace DB salvaging behavior tree with my optimized & "one-at-a-time" version
						CanRunDecoratorDelegate canRunDelegateSalvageGilesOverlord = TownRunManager.GilesSalvageOverlord;
						ActionDelegate actionDelegatePreSalvage = TownRunManager.GilesOptimisedPreSalvage;
						ActionDelegate actionDelegateSalvage = TownRunManager.GilesOptimisedSalvage;
						ActionDelegate actionDelegatePostSalvage = TownRunManager.GilesOptimisedPostSalvage;
						Sequence sequenceSalvage = new Sequence(
								new Action(actionDelegatePreSalvage),
								new Action(actionDelegateSalvage),
								new Action(actionDelegatePostSalvage),
								new Sequence(
								new Action(actionDelegatePrePause),
								new Action(actionDelegatePause)
								)
								);
						GilesReplacement.Children[6] = new Decorator(canRunDelegateSalvageGilesOverlord, sequenceSalvage);
						Logging.WriteDiagnostic("Town Run - Salvage - hooked...");
					}


					//Decorator FinishTownRun=GilesReplacement.Children[7] as Decorator;
					//Decorator FinishTownRunCheck=FinishTownRun.Children[0] as Decorator;
					//PrioritySelector FinishTownRunPrioritySelector=FinishTownRunCheck.DecoratedChild as PrioritySelector;
					//Decorator FinishTownRunPrioritySelectorDecorator=FinishTownRunPrioritySelector.Children[0] as Decorator;
					//Zeta.TreeSharp.Action FinishTownRunAction=FinishTownRunPrioritySelectorDecorator.Children[0] as Zeta.TreeSharp.Action;

					//[7] == Return to Townportal if there is one..

					//Setup our movement back to townportal
					CanRunDecoratorDelegate canRunDelegateUseTownPortalReturn = TownRunManager.FinishTownRunOverlord;
					ActionDelegate actionDelegateFinishTownRun = TownRunManager.TownRunFinishBehavior;
					Sequence sequenceFinishTownRun = new Sequence(
					   new Action(actionDelegateFinishTownRun)
					);

					//We insert this before the demonbuddy townportal finishing behavior.. 
					GilesReplacement.InsertChild(7, new Decorator(canRunDelegateUseTownPortalReturn, sequenceFinishTownRun));



					CanRunDecoratorDelegate canRunUnidBehavior = TownRunManager.UnidItemOverlord;
					ActionDelegate actionDelegatePreUnidStash = TownRunManager.UnidStashOptimisedPreStash;
					ActionDelegate actionDelegatePostUnidStash = TownRunManager.UnidStashOptimisedPostStash;
					ActionDelegate actionDelegateUnidBehavior = TownRunManager.UnidStashBehavior;
					Sequence sequenceUnidStash = new Sequence(
							new Action(actionDelegatePreUnidStash),
							new Action(actionDelegateUnidBehavior),
							new Action(actionDelegatePostUnidStash),
							new Sequence(
							new Action(actionDelegatePrePause),
							new Action(actionDelegatePause)
							)
						  );
					GilesReplacement.InsertChild(2, new Decorator(canRunUnidBehavior, sequenceUnidStash));


					CanRunDecoratorDelegate canRunDelegateGilesTownRunCheck = TownRunManager.GilesTownRunCheckOverlord;
					hook.Value[0] = new Decorator(canRunDelegateGilesTownRunCheck, new PrioritySelector(GilesReplacement));

					Logging.WriteDiagnostic("Vendor Run tree hooked...");
				} // Vendor run hook
				if (hook.Key.Contains("Loot"))
				{
					if (looting)
					{
						// Replace the loot behavior tree with a blank one, as we no longer need it
						CanRunDecoratorDelegate canRunDelegateBlank = BlankDecorator;
						ActionDelegate actionDelegateBlank = BlankAction;
						Sequence sequenceblank = new Sequence(
								new Action(actionDelegateBlank)
								);
						hook.Value[0] = new Decorator(canRunDelegateBlank, sequenceblank);
						Logging.WriteDiagnostic("Loot tree replaced...");
					}
					else
					{
						CanRunDecoratorDelegate canRunDelegateBlank = BlankDecorator;
						hook.Value[0] = new Decorator(canRunDelegateBlank, BrainBehavior.CreateLootBehavior());
					}
				} // Vendor run hook
				if (hook.Key.Contains("OutOfGame"))
				{
					PrioritySelector CompositeReplacement = hook.Value[0] as PrioritySelector;

					CanRunDecoratorDelegate shouldPreformOutOfGameBehavior = OutOfGame.OutOfGameOverlord;
					ActionDelegate actionDelgateOOGBehavior = OutOfGame.OutOfGameBehavior;
					Sequence sequenceOOG = new Sequence(
							new Action(actionDelgateOOGBehavior)
					);
					CompositeReplacement.Children.Insert(0, new Decorator(shouldPreformOutOfGameBehavior, sequenceOOG));
					hook.Value[0] = CompositeReplacement;

					Logging.WriteDiagnostic("Out of game tree hooked");
				}
				if (hook.Key.Contains("Death"))
				{
					PrioritySelector DeathPrioritySelector = hook.Value[0] as PrioritySelector;

					//CanRunDecoratorDelegate canRunDeathBehavior=new CanRunDecoratorDelegate(Death.DeathOverlord);

					//Sequence sequenceDeath=new Sequence(
					//	 new Zeta.TreeSharp.Action(actionDelgateDeath)
					//);
					//DeathPrioritySelector.Children[0]=new Zeta.TreeSharp.Decorator(canRunDeathBehavior, sequenceDeath);

					Decorator DeathDecorator = DeathPrioritySelector.Children[0] as Decorator;
					Sequence DeathSequence = DeathDecorator.Children[0] as Sequence;
					ActionDelegate actionDelgateDeath = EventHandlers.DeathHandler;
					DeathSequence.InsertChild(0, new Action(actionDelgateDeath));
					/*
					 17:10:24.548 N] Zeta.TreeSharp.Action
					[17:10:24.548 N] Zeta.TreeSharp.DecoratorContinue
					[17:10:24.548 N] Zeta.TreeSharp.Action
					[17:10:24.548 N] Zeta.TreeSharp.Sleep
					[17:10:24.548 N] Zeta.TreeSharp.Action
					*/
					// foreach (var item in DeathSequence.Children)
					//{
					//	 Logging.Write(item.GetType().ToString());
					//}

				}
			}
			#endregion
			initTreeHooks = true;
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

	public class TrinityStuckHandler : IStuckHandler
	{
		public Vector3 GetUnstuckPos()
		{
			return Vector3.Zero;
		}

		public bool IsStuck
		{
			get
			{
				return false;
			}
		}
	}
	public class TrinityCombatTargetingReplacer : ITargetingProvider
	{
		private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
		public List<DiaObject> GetObjectsByWeight()
		{
			if (!Bot.Targeting.DontMove)
				return listEmptyList;
			List<DiaObject> listFakeList = new List<DiaObject>();
			listFakeList.Add(null);
			return listFakeList;
		}
	}
	public class TrinityLootTargetingProvider : ITargetingProvider
	{
		private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
		public List<DiaObject> GetObjectsByWeight()
		{
			return listEmptyList;
		}
	}
	public class TrinityObstacleTargetingProvider : ITargetingProvider
	{
		private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
		public List<DiaObject> GetObjectsByWeight()
		{
			return listEmptyList;
		}
	}
}