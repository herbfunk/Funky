using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Demonbuddy;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using System.Windows.Controls;
using System.IO;
using System.Reflection;
using Action = System.Action;

namespace GilesBlankCombatRoutine
{
	[ComVisible(false)]
	public class FunkyRoutine : CombatRoutine
	{
		private static string sDemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		private static string sTrinityPluginPath = sDemonBuddyPath + @"\Plugins\FunkyBot\";
		private static readonly log4net.ILog DBLog = Zeta.Common.Logger.GetLoggerInstanceForType();




		public override void Initialize()
		{
			// Set up the pause button
			Application.Current.Dispatcher.Invoke(
			new Action(
			() =>
			{
				Window mainWindow = App.Current.MainWindow;
				SplitButton FunkyButton;
				var tab = mainWindow.FindName("tabControlMain") as TabControl;
				if (tab != null)
				{
					var infoDumpTab = tab.Items[0] as TabItem;
					if (infoDumpTab != null)
					{
						var grid = infoDumpTab.Content as Grid;
						if (grid != null)
						{
							FunkyButton = grid.FindName("Funky") as SplitButton;
							if (FunkyButton != null)
							{
								DBLog.DebugFormat("Funky Button handler added");
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

							SplitButton btnSplit_Funky;
							FunkyDebug.initDebugLabels(out btnSplit_Funky);

							if (FunkyButton != null)
								return;
							else
								grid.Children.Add(btnSplit_Funky);
						}
					}
				}
			}));
		}


		public sealed override void Dispose()
		{
			// Set up the pause button
			Application.Current.Dispatcher.Invoke(
			new Action(
			() =>
			{
				Window mainWindow = Application.Current.MainWindow;
				var tab = mainWindow.FindName("tabControlMain") as TabControl;
				if (tab == null) return;
				var infoDumpTab = tab.Items[0] as TabItem;
				if (infoDumpTab == null) return;
				var grid = infoDumpTab.Content as Grid;
				if (grid == null) return;
				SplitButton btnfunky = grid.FindName("Funky") as SplitButton;
				if (btnfunky == null) return;
				grid.Children.Remove(btnfunky);
			}));

			GC.SuppressFinalize(this);
			return;
		}

		public override string Name { get { return "Funky"; } }

		public override Window ConfigWindow { get { return new Window(); } }

		public override ActorClass Class { get { return ZetaDia.Me.ActorClass; } }

		public override SNOPower DestroyObjectPower
		{
			get
			{
				if (ZetaDia.IsInGame)
					return ZetaDia.CPlayer.GetPowerForSlot(HotbarSlot.HotbarMouseLeft);
				else
					return SNOPower.None;
			}
		}

		public override float DestroyObjectDistance { get { return 15; } }

		/*private Composite _combat;
		private Composite _buff;*/
		public override Composite Combat { get { return new PrioritySelector(); } }
		public override Composite Buff { get { return new PrioritySelector(); } }

	}
}
