using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Demonbuddy;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Common.Plugins;
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
		internal static PluginContainer lastSelectedPC = null;
		private static ContextMenu advanceMenu = new ContextMenu();



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

							if (FunkyButton == null)
								grid.Children.Add(btnSplit_Funky);



							var debugButton = grid.FindName("FunkyDebug") as Button;
							if (debugButton != null)
							{
								DBLog.DebugFormat("Funky Debug handler added");
							}
							else
							{
								Button[] splitbuttons = grid.Children.OfType<Button>().ToArray();
								if (splitbuttons.Any())
								{

									foreach (var item in splitbuttons)
									{
										if (item.Name.Contains("FunkyDebug"))
										{
											debugButton = item;
											break;
										}
									}
								}
							}



							if (debugButton == null)
							{
								debugButton = new Button
								{
									Width = 125,
									Height = 20,
									HorizontalAlignment = HorizontalAlignment.Left,
									VerticalAlignment = VerticalAlignment.Top,
									Margin = new Thickness(425, 35, 0, 0),
									IsEnabled = true,
									Visibility = Visibility.Hidden,
									Content = "Debug",
									Name = "FunkyDebug",
								};

								grid.Children.Add(debugButton);
							}

						}
					}

					Button CompileMenuItem = new Button
					{
						Content = "Recompile",
						Width = 150,
						Height = 30,
					};
					CompileMenuItem.Click += FunkyDebug.RecompileSelectedPlugin;
					advanceMenu.Items.Add(CompileMenuItem);

					var PluginsDumpTab = tab.Items[2] as TabItem;
					if (PluginsDumpTab == null) return;
					var plugingrid = PluginsDumpTab.Content as Grid;
					if (plugingrid == null) return;
					var listboxPlugins = plugingrid.Children.OfType<ListBox>().First();
					if (listboxPlugins == null) return;

					listboxPlugins.SelectionChanged += OnPluginItemChanged;
					listboxPlugins.ContextMenu = advanceMenu;
				}
			}));
		}


		private static void OnPluginItemChanged(object sender, EventArgs e)
		{
			ListBox sender_ = (ListBox)sender;
			var SelectedItem = sender_.SelectedItem;
			if (sender != null && SelectedItem is Zeta.Common.Plugins.PluginContainer)
			{
				lastSelectedPC = SelectedItem as Zeta.Common.Plugins.PluginContainer;
			}
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
				{
					var btnfunky = grid.FindName("Funky") as SplitButton;
					if (btnfunky != null) grid.Children.Remove(btnfunky);

					var btnfunkydebug = grid.FindName("FunkyDebug") as Button;
					if (btnfunkydebug != null) grid.Children.Remove(btnfunkydebug);
				}

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
