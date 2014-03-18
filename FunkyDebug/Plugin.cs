using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Demonbuddy;
using Zeta.Bot;
using Zeta.Common.Plugins;

namespace FunkyDebug
{
	 public partial class FunkyDebugger : IPlugin
	 {
		 internal static readonly log4net.ILog DBLog = Zeta.Common.Logger.GetLoggerInstanceForType();

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
		  }

		  public void OnEnabled()
		  {
				BotMain.OnStart+=FunkyBotStart;
				BotMain.OnStop+=FunkyBotStop;
		  }

		  public void OnInitialize()
		  {
			  Application.Current.Dispatcher.Invoke(
			  new Action(
			  () =>
			  {
				  Window mainWindow = App.Current.MainWindow;
				  Button FunkyButton;
				  var tab = mainWindow.FindName("tabControlMain") as TabControl;
				  if (tab != null)
				  {
					  var infoDumpTab = tab.Items[0] as TabItem;
					  if (infoDumpTab != null)
					  {
						  var grid = infoDumpTab.Content as Grid;
						  if (grid != null)
						  {
							  FunkyButton = grid.FindName("FunkyDebug") as Button;
							  if (FunkyButton != null)
							  {
								  DBLog.DebugFormat("Funky Button handler added");
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
											  FunkyButton = item;
											  break;
										  }
									  }
								  }
							  }



							  if (FunkyButton != null)
								  return;
							  else
							  {
								  FunkyButton = new Button
								  {
									  Width = 125,
									  Height = 20,
									  HorizontalAlignment = HorizontalAlignment.Left,
									  VerticalAlignment = VerticalAlignment.Top,
									  Margin = new Thickness(425, 35, 0, 0),
									  IsEnabled = true,
									  Content = "Debug",
									  Name = "FunkyDebug",
								  };
								  FunkyButton.Click += lblFunky_Click;
								  grid.Children.Add(FunkyButton);
							  }
						  }
					  }
				  }
			  }));
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

		  public void OnPulse()
		  {

		  }

		  public void OnShutdown()
		  {

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
