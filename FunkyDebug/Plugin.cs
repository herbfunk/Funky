using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Demonbuddy;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Common.Compiler;
using Zeta.Common.Plugins;

namespace FunkyDebug
{
	 public partial class FunkyDebugger : IPlugin
	 {
		 internal static readonly log4net.ILog DBLog = Zeta.Common.Logger.GetLoggerInstanceForType();
		 private static PluginContainer lastSelectedPC = null;
		 private static ContextMenu advanceMenu = new ContextMenu();
		 private static readonly string sDemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		 private static bool initFunkyButton;

		  #region IPlugin Members

		  public string Author
		  {
				get { return "HerbFunk"; }
		  }

		  public string Description
		  {
				get { return "Debugging Tools"; }
		  }

		  public Window DisplayWindow
		  {
				get { return null; }
		  }

		  public string Name
		  {
				get { return "FunkyDebug"; }
		  }

		  public void OnDisabled()
		  {
				BotMain.OnStart-=FunkyBotStart;
				BotMain.OnStop-=FunkyBotStop;

				Button funkyButton;
				bool found = FindFunkyButton(out funkyButton);
				if (found)
				{
					funkyButton.Click -= lblFunky_Click;
				}
		  }

		  public void OnEnabled()
		  {
				BotMain.OnStart+=FunkyBotStart;
				BotMain.OnStop+=FunkyBotStop;
		  }

		  public void OnInitialize()
		  {
			  Button FunkyButton;
			  initFunkyButton = FindFunkyButton(out FunkyButton);
			  if (initFunkyButton && FunkyButton != null)
			  {
				  DBLog.DebugFormat("Funky Debug Button Click Handler Added");
				  FunkyButton.Click += lblFunky_Click;
			  }
		  }
		  static void lblFunky_Click(object sender, EventArgs e)
		  {
			  try
			  {
				  FormDebug debug = new FormDebug();
				  debug.ShowDialog();
				  //BotMain.CurrentBot.ConfigWindow.Show();
			  }
			  catch
			  {

			  }

		  }

		  private static bool FindFunkyButton(out Button funkyButton)
		  {
			  funkyButton = null;

			  Window mainWindow = App.Current.MainWindow;
			  var tab = mainWindow.FindName("tabControlMain") as TabControl;
			  if (tab == null) return false;
			  var infoDumpTab = tab.Items[0] as TabItem;
			  if (infoDumpTab == null) return false;
			  var grid = infoDumpTab.Content as Grid;
			  if (grid == null) return false;
			  funkyButton = grid.FindName("FunkyDebug") as Button;
			  if (funkyButton != null)
			  {
				  DBLog.DebugFormat("Funky Debug handler added");
				  return true;
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
							  funkyButton = item;
							  return true;
						  }
					  }
				  }
			  }

			  return false;
		  }

		  public void OnPulse()
		  {

		  }

		  public void OnShutdown()
		  {
			  Button funkyButton;
			  bool found = FindFunkyButton(out funkyButton);
			  if (found)
			  {
				  funkyButton.Click -= lblFunky_Click;
			  }
		  }

		  public Version Version
		  {
				get { return new Version(0, 0, 1); }
		  }

		  #endregion

		  #region IEquatable<IPlugin> Members

		  public bool Equals(IPlugin other) { return (other.Name==Name)&&(other.Version==Version); }

		  #endregion

		  private void FunkyBotStart(IBot bot)
		  {

		  }
		  private void FunkyBotStop(IBot bot)
		  {

		  }
	 }
}
