using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Demonbuddy;
using FunkyBot.Cache;
using FunkyBot.Cache.Dictionaries.Objects;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Config;
using FunkyBot.Config.Settings;
using FunkyBot.Config.UI;
using FunkyBot.DBHandlers;
using FunkyBot.DBHandlers.Townrun;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Decorator = Zeta.TreeSharp.Decorator;
using Action = Zeta.TreeSharp.Action;
using Logger = FunkyBot.Misc.Logger;

namespace FunkyBot
{
	public partial class Funky : CombatRoutine
	{

		public Funky()
		{
			 Instance = this;
		}
        public static Funky Instance { get; private set; }




		internal static float Difference(float A, float B)
		{
			if (A > B)
				return A - B;

			return B - A;
		}

		// Status text for DB main window status
		internal static string sStatusText = "";
		// Do we need to reset the debug bar after combat handling?
		internal static bool bResetStatusText = false;

		

		internal static bool initTreeHooks;
		internal static void HookBehaviorTree()
		{
			Logger.DBLog.InfoFormat("[Funky] Treehooks..");
			#region TreeHooks
			foreach (var hook in TreeHooks.Instance.Hooks)
			{
				#region Combat

				// Replace the combat behavior tree, as that happens first and so gets done quicker!
				if (hook.Key.Contains("Combat"))
				{
					Logger.DBLog.DebugFormat("Combat...");

			
						// Replace the pause just after identify stuff to ensure we wait before trying to run to vendor etc.
						CanRunDecoratorDelegate canRunDelegateCombatTargetCheck = CombatHandler.GlobalOverlord;
						ActionDelegate actionDelegateCoreTarget = CombatHandler.HandleTarget;
						Sequence sequencecombat = new Sequence(
								new Action(actionDelegateCoreTarget)
								);
						hook.Value[0] = new Decorator(canRunDelegateCombatTargetCheck, sequencecombat);
						Logger.DBLog.DebugFormat("Combat Tree replaced...");
					

				}

				#endregion

				#region VendorRun

				// Wipe the vendorrun and loot behavior trees, since we no longer want them
				if (hook.Key.Contains("VendorRun"))
				{
					Logger.DBLog.DebugFormat("VendorRun...");

					Decorator GilesDecorator = hook.Value[0] as Decorator;
					PrioritySelector CompositeReplacement = GilesDecorator.Children[0] as PrioritySelector;
					PrioritySelector GilesReplacement=CompositeReplacement;

					bool usingReplacedTownRun = false;
					if (CompositeReplacement.Children[0] is PrioritySelector)
					{
						GilesReplacement = CompositeReplacement.Children[0] as PrioritySelector;
						usingReplacedTownRun = true;
					}
					 
					
					#region TownPortal

					//[1] == Return to town
	
						CanRunDecoratorDelegate canRunDelegateReturnToTown = TownPortalBehavior.FunkyTPOverlord;
						ActionDelegate actionDelegateReturnTown = TownPortalBehavior.FunkyTPBehavior;
						ActionDelegate actionDelegateTownPortalFinish = TownPortalBehavior.FunkyTownPortalTownRun;
						Sequence sequenceReturnTown = new Sequence(
							new Action(actionDelegateReturnTown),
							new Action(actionDelegateTownPortalFinish)
							);
						GilesReplacement.Children[1] = new Decorator(canRunDelegateReturnToTown, sequenceReturnTown);
						Logger.DBLog.DebugFormat("Town Run - Town Portal - hooked...");
					


					#endregion

					//Normally there are 9 children -- so if there is additional children due to townrun inserts we adjust our index!
					int insertIndex = 5 + (GilesReplacement.Children.Count-9);
					
					#region Interaction Behavior
					CanRunDecoratorDelegate canRunDelegateInteraction = TownRunManager.InteractionOverlord;
					ActionDelegate actionDelegateInteractionMovementhBehavior = TownRunManager.InteractionMovement;
					ActionDelegate actionDelegateInteractionClickBehaviorBehavior = TownRunManager.InteractionClickBehavior;
					ActionDelegate actionDelegateInteractionLootingBehaviorBehavior = TownRunManager.InteractionLootingBehavior;
					ActionDelegate actionDelegateInteractionFinishBehaviorBehavior = TownRunManager.InteractionFinishBehavior;

					Sequence sequenceInteraction = new Sequence(
							new Action(actionDelegateInteractionFinishBehaviorBehavior),
							new Action(actionDelegateInteractionMovementhBehavior),
							new Action(actionDelegateInteractionClickBehaviorBehavior),
							new Action(actionDelegateInteractionLootingBehaviorBehavior),
							new Action(actionDelegateInteractionFinishBehaviorBehavior)
						);
					GilesReplacement.InsertChild(insertIndex, new Decorator(canRunDelegateInteraction, sequenceInteraction));
					Logger.DBLog.DebugFormat("Town Run - Interaction Behavior - Inserted...");

					#endregion

					CanRunDecoratorDelegate canRunDelegateStats = TownRunManager.StatsOverlord;
					ActionDelegate actionDelegateStatsBehavior = TownRunManager.StatsBehavior;
					Sequence sequenceStats = new Sequence(new Action(actionDelegateStatsBehavior));
					GilesReplacement.InsertChild(insertIndex+2, new Decorator(canRunDelegateStats, sequenceStats));

					#region Finish Behavior

					ActionDelegate actionDelegateFinishBehavior = TownRunManager.FinishBehavior;
					Sequence actionFinish = GilesReplacement.Children[insertIndex+5] as Sequence;
					if (actionFinish!=null)
					{
						actionFinish.InsertChild(0, new Action(actionDelegateFinishBehavior));
						Logger.DBLog.DebugFormat("Town Run - Finish Behavior - Inserted...");
						GilesReplacement.Children[insertIndex + 5] = actionFinish;
					}
					#endregion


					//CanRunDecoratorDelegate canRunDelegateGilesTownRunCheck = TownRunManager.GilesTownRunCheckOverlord;
					//hook.Value[0] = new Decorator(canRunDelegateGilesTownRunCheck, new PrioritySelector(GilesReplacement));

					Logger.DBLog.DebugFormat("Vendor Run tree hooked...");
				} // Vendor run hook


				#endregion

				#region Loot

				if (hook.Key.Contains("Loot"))
				{
					Logger.DBLog.DebugFormat("Loot...");

		
						// Replace the loot behavior tree with a blank one, as we no longer need it
						CanRunDecoratorDelegate canRunDelegateBlank = BlankDecorator;
						ActionDelegate actionDelegateBlank = BlankAction;
						Sequence sequenceblank = new Sequence(
								new Action(actionDelegateBlank)
								);
						hook.Value[0] = new Decorator(canRunDelegateBlank, sequenceblank);
						Logger.DBLog.DebugFormat("Loot tree replaced...");
					
				}

				#endregion

				#region OutOfGame


				if (hook.Key.Contains("OutOfGame"))
				{
					Logger.DBLog.DebugFormat("OutOfGame...");

					PrioritySelector CompositeReplacement = hook.Value[0] as PrioritySelector;

					CanRunDecoratorDelegate shouldPreformOutOfGameBehavior = OutOfGame.OutOfGameOverlord;
					ActionDelegate actionDelgateOOGBehavior = OutOfGame.OutOfGameBehavior;
					Sequence sequenceOOG = new Sequence(
							new Action(actionDelgateOOGBehavior)
					);
					CompositeReplacement.Children.Insert(0, new Decorator(shouldPreformOutOfGameBehavior, sequenceOOG));
					hook.Value[0] = CompositeReplacement;

					Logger.DBLog.DebugFormat("Out of game tree hooked");
				}

				#endregion

				#region Death


				if (hook.Key.Contains("Death"))
				{
					Logger.DBLog.DebugFormat("Death...");


					Decorator deathDecorator = hook.Value[0] as Decorator;

					PrioritySelector DeathPrioritySelector = deathDecorator.Children[0] as PrioritySelector;

					//Insert Death Tally Counter At Beginning!
					CanRunDecoratorDelegate deathTallyDecoratorDelegate = EventHandlers.EventHandlers.TallyDeathCanRunDecorator;
					ActionDelegate actionDelegatedeathTallyAction = EventHandlers.EventHandlers.TallyDeathAction;
					Action deathTallyAction = new Action(actionDelegatedeathTallyAction);
					Decorator deathTallyDecorator = new Decorator(deathTallyDecoratorDelegate, deathTallyAction);
					DeathPrioritySelector.InsertChild(0, deathTallyDecorator);


					//Death Wait..
					CanRunDecoratorDelegate deathWaitDecoratorDelegate = EventHandlers.EventHandlers.DeathShouldWait;
					ActionDelegate deathWaitActionDelegate = EventHandlers.EventHandlers.DeathWaitAction;
					Action deathWaitAction = new Action(deathWaitActionDelegate);
					Decorator deathWaitDecorator = new Decorator(deathWaitDecoratorDelegate, deathWaitAction);
					DeathPrioritySelector.InsertChild(0, deathWaitDecorator);

					//Insert Death Tally Reset at End!
					Action deathTallyActionReset = new Action(ret => EventHandlers.EventHandlers.TallyedDeathCounter = false);
					DeathPrioritySelector.InsertChild(DeathPrioritySelector.Children.Count-1, deathTallyActionReset);
					Logger.DBLog.DebugFormat("Death tree hooked");



					//foreach (var item in DeathPrioritySelector.Children)
					//{
					//	Logger.DBLog.InfoFormat(item.GetType().ToString());
					//}


					/*
					 *  Zeta.TreeSharp.Decorator
						Zeta.TreeSharp.Decorator
						Zeta.TreeSharp.Decorator
						Zeta.TreeSharp.Decorator
						Zeta.TreeSharp.Decorator
						Zeta.TreeSharp.Decorator
						Zeta.TreeSharp.Action
					 * 
					*/


				}


				#endregion
			}
			#endregion
			initTreeHooks = true;
		}
		internal static void RemoveHandlers()
		{
			GameEvents.OnPlayerDied -= EventHandlers.EventHandlers.FunkyOnDeath;
			GameEvents.OnGameJoined -= EventHandlers.EventHandlers.FunkyOnJoinGame;
			GameEvents.OnWorldChanged -= EventHandlers.EventHandlers.FunkyOnWorldChange;
			GameEvents.OnGameLeft -= EventHandlers.EventHandlers.FunkyOnLeaveGame;
			GameEvents.OnGameChanged -= EventHandlers.EventHandlers.FunkyOnGameChanged;
			ProfileManager.OnProfileLoaded -= EventHandlers.EventHandlers.FunkyOnProfileChanged;
			GameEvents.OnLevelUp -= EventHandlers.EventHandlers.FunkyOnLevelUp;
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
		}
		private static bool BlankDecorator(object ret)
		{
			return false;
		}
		private static RunStatus BlankAction(object ret)
		{
			return RunStatus.Success;
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
				if (!Bot.Targeting.Cache.DontMove)
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

		
		public string Author { get { return "Herbfunk"; } }
		public string Description
		{
			get
			{
				return "FunkyBot version " + Version;
			}
		}

		public override void Dispose()
		{
			SplitButton btnSplit_Funky = UIControl.FindFunkyButton();
			if (btnSplit_Funky != null)
			{
				btnSplit_Funky.Click -= UIControl.buttonFunkySettingDB_Click;
				Grid dbGrid = UIControl.GetDemonbuddyMainGrid();
				if (dbGrid != null)
				{
					Logger.DBLog.DebugFormat("Funky Split Button Removed!");
					dbGrid.Children.Remove(btnSplit_Funky);
				}
			}

			if (RoutineManager.Current.Name != "Funky") return;

			BotMain.OnStop -= EventHandlers.EventHandlers.FunkyBotStop;
			BotMain.OnStart -= EventHandlers.EventHandlers.FunkyBotStart;
			RemoveHandlers();

			if (initTreeHooks) ResetTreehooks();

			Bot.ResetGame();

			Logger.DBLog.InfoFormat("FunkyBot has been Disposed!");
		}

		public override void Initialize()
		{
			SplitButton btnSplit_Funky = UIControl.FindFunkyButton();
			if (btnSplit_Funky == null)
			{
				UIControl.initDebugLabels(out btnSplit_Funky);
				UIControl.AddButtonToDemonbuddyMainTab(ref btnSplit_Funky);

				Logger.DBLog.DebugFormat("Funky Split Button Click Handler Added");
				btnSplit_Funky.Click += UIControl.buttonFunkySettingDB_Click;
			}

			ObjectCache.SNOCache = new SnoIDCache();
			ObjectCache.FakeCacheObject = new CacheObject(Vector3.Zero, TargetType.None, 0d, "Fake Target", 1f);

			//Update Account Details..
			Bot.Character.Account.UpdateCurrentAccountDetails();
			Bot.Settings = new Settings_Funky();
			BotMain.OnStop += EventHandlers.EventHandlers.FunkyBotStop;
			BotMain.OnStart += EventHandlers.EventHandlers.FunkyBotStart;

			Logger.Write("Init Logger Completed!");

			Logger.DBLog.InfoFormat("FunkyBot has been Initalized!");
		}

		public override string Name { get { return "Funky"; } }

		public override Window ConfigWindow
		{
			get
			{
				UIControl.FrmSettings = new SettingsForm();
				Window w = new Window();
				w.Loaded += (sender, args) =>
				{
					UIControl.buttonFunkySettingDB_Click(null, null);
					w.Close();
				};

				return w;
			} 
		}

		public override ActorClass Class
		{
			get 
			{
				if (Bot.Character.Account.ActorClass == ActorClass.Invalid)
					Bot.Character.Account.UpdateCurrentAccountDetails();

				return Bot.Character.Account.ActorClass;
			}
		}

		public override SNOPower DestroyObjectPower
		{
			get
			{
				if (ZetaDia.IsInGame)
					return ZetaDia.CPlayer.GetPowerForSlot(HotbarSlot.HotbarMouseLeft);

				return SNOPower.None;
			}
		}
		public override float DestroyObjectDistance { get { return 15; } }
		public override Composite Combat { get { return new PrioritySelector(); } }
		public override Composite Buff { get { return new PrioritySelector(); } }

		public bool Equals(IPlugin other) { return (other.Name == Name) && (other.Version == Version); }

	}
}
