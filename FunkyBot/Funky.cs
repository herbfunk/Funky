using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Demonbuddy;
using fBaseXtensions;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Helpers;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Decorator = Zeta.TreeSharp.Decorator;
using Action = Zeta.TreeSharp.Action;
using Logger = fBaseXtensions.Helpers.Logger;

namespace FunkyBot
{
	public partial class Funky : CombatRoutine
	{
		public Version Version { get { return new Version(3, 0, 0, 0); } }

		public Funky()
		{
			Instance = this;
		}
		public static Funky Instance { get; private set; }

		internal static string RoutinePath
		{
			get
			{
				if (Directory.Exists(FolderPaths.DemonBuddyPath + @"\Plugins\FunkyBot\"))
					return FolderPaths.DemonBuddyPath + @"\Plugins\FunkyBot\";
				else if (Directory.Exists(FolderPaths.DemonBuddyPath + @"\Routines\FunkyBot\"))
					return FolderPaths.DemonBuddyPath + @"\Routines\FunkyBot\";

				return null;
			}
		}


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

			Logger.DBLog.DebugFormat("Combat...");


			// Replace the pause just after identify stuff to ensure we wait before trying to run to vendor etc.
			//var OverlordCoRoutine=new ActionRunCoroutine(ctx => fBaseXtensions.Behaviors.CombatHandler.Overlord());
			CanRunDecoratorDelegate canRunDelegateCombatTargetCheck = fBaseXtensions.Behaviors.CombatHandler.GlobalOverlord;
			ActionDelegate actionDelegateCoreTarget = fBaseXtensions.Behaviors.CombatHandler.HandleTarget;
			Sequence sequencecombat = new Sequence
			(
				new Action(actionDelegateCoreTarget)
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
					new Action(actionDelegateBlank)
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

			BotMain.OnStop -= EventHandlers.EventHandlers.FunkyBotStop;
			BotMain.OnStart -= EventHandlers.EventHandlers.FunkyBotStart;

			if (initTreeHooks) ResetTreehooks();

			Logger.DBLog.InfoFormat("FunkyBot has been Disposed!");
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

			BotMain.OnStop += EventHandlers.EventHandlers.FunkyBotStop;
			BotMain.OnStart += EventHandlers.EventHandlers.FunkyBotStart;
			Logger.DBLog.InfoFormat("FunkyBot has been Initalized!");
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

	}
}
