using System;
using System.Windows;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace fBaseXtensions
{
	public class FunkyCombatRoutine : CombatRoutine
    {
        private static readonly log4net.ILog Log = Logger.GetLoggerInstanceForType();
		public Version Version { get { return new Version(3, 0, 0, 0); } }

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

		}


        public override void Initialize()
        {
            foreach (PluginContainer plugin in PluginManager.Plugins)
            {
                if (plugin.Plugin.Name == "fBaseXtensions" && !plugin.Enabled)
                {
                    plugin.Enabled = true;
                }
            }
        }

		public override string Name { get { return "Funky"; } }

		public override Window ConfigWindow
		{
			get
			{
                try
                {
                    foreach (PluginContainer plugin in PluginManager.Plugins)
                    {
                        if (plugin.Plugin.Name == "fBaseXtensions")
                        {
                            return plugin.Plugin.DisplayWindow;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("[Funky] Error Opening Plugin Config window!");
                    Log.Error("[Funky] {0}", ex);
                }
                Log.Error("[Funky] Unable to open Plugin Config window!");
                return null;
			}
		}

		public override ActorClass Class
		{
			get
			{

                if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld)
                {
                    // Return none if we are oog to make sure we can start the bot anytime.
                    return ActorClass.Invalid;
                }

                return ZetaDia.Me.ActorClass;
			}
		}

		public override SNOPower DestroyObjectPower
		{
			get
			{
				return SNOPower.None;
			}
		}
		public override float DestroyObjectDistance { get { return 15; } }
		public override Composite Combat { get { return new PrioritySelector(); } }
        public override Composite Buff { get { return new Action(); } }

		public bool Equals(IPlugin other) { return (other.Name == Name) && (other.Version == Version); }
		#endregion
	}
}
