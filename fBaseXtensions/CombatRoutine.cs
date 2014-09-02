using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;

namespace fBaseXtensions
{
	public class FunkyCombatRoutine : CombatRoutine
	{
		public Version Version { get { return new Version(3, 0, 0, 0); } }

		public FunkyCombatRoutine()
		{
			Instance = this;
		}
		public static FunkyCombatRoutine Instance { get; private set; }

		#region Combat Routine Implementation
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
			if (RoutineManager.Current.Name != "Funky") return;

			BotMain.OnStop -= FunkyBotStop;
			BotMain.OnStart -= FunkyBotStart;

			if (initTreeHooks) ResetTreehooks();

			Logger.DBLog.InfoFormat("Funky Combat Routine has been Disposed!");
		}

		public override void Initialize()
		{
			var basePlugin = PluginManager.Plugins.First(p => p.Plugin.Name == "fBaseXtensions");
			if (basePlugin != null)
			{
				if (!basePlugin.Enabled)
				{
					Logger.DBLog.Warn("FunkyBot requires fBaseXtensions to be enabled! -- Enabling it automatically.");
					basePlugin.Enabled = true;
				}
			}

			BotMain.OnStop += FunkyBotStop;
			BotMain.OnStart += FunkyBotStart;
			Logger.DBLog.InfoFormat("Funky Combat Routine has been Initalized!");
		}

		public override string Name { get { return "Funky"; } }

		public override Window ConfigWindow
		{
			get
			{
				return new Window();
			}
		}

		public override ActorClass Class
		{
			get
			{

				return FunkyGame.CurrentActorClass;
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
		#endregion

		internal static void FunkyBotStart(IBot bot)
		{
			Navigator.PlayerMover = new fBaseXtensions.Navigation.PlayerMover();
			Navigator.StuckHandler = new fBaseXtensions.Navigation.PluginStuckHandler();
			ITargetingProvider newCombatTargetingProvider = new PluginCombatTargeting();
			CombatTargeting.Instance.Provider = newCombatTargetingProvider;
			ITargetingProvider newLootTargetingProvider = new PluginLootTargeting();
			LootTargeting.Instance.Provider = newLootTargetingProvider;
			ITargetingProvider newObstacleTargetingProvider = new PluginObstacleTargeting();
			ObstacleTargeting.Instance.Provider = newObstacleTargetingProvider;

			if (!initTreeHooks)
			{
				HookBehaviorTree();
			}

			Navigator.SearchGridProvider.Update();
		}
		// When the bot stops, output a final item-stats report so it is as up-to-date as can be
		internal static void FunkyBotStop(IBot bot)
		{
			Navigator.PlayerMover = new DefaultPlayerMover();
			Navigator.StuckHandler = new DefaultStuckHandler();
			ResetTreehooks();
			//Bot.Reset();

		}


		internal static bool initTreeHooks;
		internal static void HookBehaviorTree()
		{
			Logger.DBLog.InfoFormat("[Funky] Treehooks..");

			Logger.DBLog.DebugFormat("Combat...");


			// Replace the pause just after identify stuff to ensure we wait before trying to run to vendor etc.
			//var OverlordCoRoutine=new ActionRunCoroutine(ctx => fBaseXtensions.Behaviors.CombatHandler.Overlord());
			CanRunDecoratorDelegate canRunDelegateCombatTargetCheck = fBaseXtensions.Behaviors.CombatHandler.GlobalOverlord;
			ActionDelegate actionDelegateCoreTarget = fBaseXtensions.Behaviors.CombatHandler.HandleTarget;
			Sequence sequencecombat = new Sequence
			(
				new Zeta.TreeSharp.Action(actionDelegateCoreTarget)
			);
			var NewCombatComposite = new Decorator(canRunDelegateCombatTargetCheck, sequencecombat);
			HookHandler.SetHookValue(HookHandler.HookType.Combat, 0, NewCombatComposite);

			//var Coroutine = new ActionRunCoroutine(ctx => fBaseXtensions.Behaviors.CombatHandler.CombatBehavior());
			//HookHandler.SetHookValue(HookHandler.HookType.Combat, 0, Coroutine);
			Logger.DBLog.DebugFormat("Combat Tree replaced...");


			Logger.DBLog.DebugFormat("Loot...");
			// Replace the loot behavior tree with a blank one, as we no longer need it
			CanRunDecoratorDelegate canRunDelegateBlank = BlankDecorator;
			ActionDelegate actionDelegateBlank = BlankAction;
			Sequence sequenceblank = new Sequence(
					new Zeta.TreeSharp.Action(actionDelegateBlank)
					);

			var NewLootComposite = new Decorator(canRunDelegateBlank, sequenceblank);
			HookHandler.SetHookValue(HookHandler.HookType.Loot, 0, NewLootComposite);
			Logger.DBLog.DebugFormat("Loot tree replaced...");


			initTreeHooks = true;
		}
		internal static void ResetTreehooks()
		{
			initTreeHooks = false;
			CombatTargeting.Instance.Provider = new DefaultCombatTargetingProvider();
			LootTargeting.Instance.Provider = new DefaultLootTargetingProvider();
			ObstacleTargeting.Instance.Provider = new DefaultObstacleTargetingProvider();
			HookHandler.RestoreHook(HookHandler.HookType.Combat);
			HookHandler.RestoreHook(HookHandler.HookType.Loot);
		}

		private static bool BlankDecorator(object ret) { return false; }
		private static RunStatus BlankAction(object ret) { return RunStatus.Success; }

		public class PluginCombatTargeting : ITargetingProvider
		{
			private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
			public List<DiaObject> GetObjectsByWeight()
			{
				if (!FunkyGame.Targeting.Cache.DontMove)
					return listEmptyList;
				List<DiaObject> listFakeList = new List<DiaObject>();
				listFakeList.Add(null);
				return listFakeList;
			}
		}
		public class PluginLootTargeting : ITargetingProvider
		{
			private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
			public List<DiaObject> GetObjectsByWeight()
			{
				return listEmptyList;
			}
		}
		public class PluginObstacleTargeting : ITargetingProvider
		{
			private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
			public List<DiaObject> GetObjectsByWeight()
			{
				return listEmptyList;
			}
		}
	}
}
