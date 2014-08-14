using System.Linq;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common.Plugins;
using Zeta.Game;
using Logger = fBaseXtensions.Helpers.Logger;

namespace FunkyBot.EventHandlers
{
	public partial class EventHandlers
	{
		internal static void FunkyBotStart(IBot bot)
		{
			Navigator.PlayerMover = new fBaseXtensions.Navigation.PlayerMover();
			Navigator.StuckHandler = new fBaseXtensions.Navigation.PluginStuckHandler();
			ITargetingProvider newCombatTargetingProvider = new Funky.PluginCombatTargeting();
			CombatTargeting.Instance.Provider = newCombatTargetingProvider;
			ITargetingProvider newLootTargetingProvider = new Funky.PluginLootTargeting();
			LootTargeting.Instance.Provider = newLootTargetingProvider;
			ITargetingProvider newObstacleTargetingProvider = new Funky.PluginObstacleTargeting();
			ObstacleTargeting.Instance.Provider = newObstacleTargetingProvider;

			if (!Funky.initTreeHooks)
			{
				Funky.HookBehaviorTree();
			}

			Navigator.SearchGridProvider.Update();
		}
	}
}