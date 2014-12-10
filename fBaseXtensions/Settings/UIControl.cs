using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Demonbuddy;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Common.Compiler;
using Zeta.Common.Plugins;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.Settings
{
	internal static class UIControl
	{
		internal static SettingsForm FrmSettings;
		internal static void InstallSettingsButton()
		{
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

						SplitButton FunkyButton = grid.FindName("Funky") as SplitButton;
						if (FunkyButton != null)
						{
							Logger.DBLog.DebugFormat("Funky Button handler added");
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

						if (FunkyButton==null)
						{
							SplitButton btn = new SplitButton
							{
								Width = 125,
								Height = 20,
								HorizontalAlignment = HorizontalAlignment.Left,
								VerticalAlignment = VerticalAlignment.Top,
								Margin = new Thickness(425, 10, 0, 0),
								IsEnabled = true,
								Content = "Funky",
								Name = "Funky",
							};
							btn.Click += lblFunky_Click;

							lblDebug_OpenLog = new Label
							{
								Content = "Open DB LogFile",
								Width = 100,
								Height = 25,
								HorizontalAlignment = HorizontalAlignment.Stretch,
							};
							lblDebug_OpenLog.MouseDown += lblDebug_OpenDBLog;

							lblDebug_FunkyLog = new Label
							{
								Content = "Open Funky LogFile",
								Width = 100,
								Height = 25,
								HorizontalAlignment = HorizontalAlignment.Stretch,
							};
							lblDebug_FunkyLog.MouseDown += lblDebug_OpenFunkyLog;

							Label OpenTrinityFolder = new Label
							{
								Content = "Open Funky Folder",
								Width = 100,
								Height = 25,
								HorizontalAlignment = HorizontalAlignment.Stretch,

							};
							OpenTrinityFolder.MouseDown += lblDebug_OpenTrinityFolder;

							Label Recompile = new Label
							{
								Content = "Recompile Funky",
								Width = 100,
								Height = 25,
								HorizontalAlignment = HorizontalAlignment.Stretch,

							};
							Recompile.MouseDown += lblCompile_Click;




							menuItem_Debug = new MenuItem
							{
								Header = "Debuging",
								Width = 125
							};
							menuItem_Debug.Items.Add(lblDebug_OpenLog);
							menuItem_Debug.Items.Add(lblDebug_FunkyLog);
							menuItem_Debug.Items.Add(OpenTrinityFolder);
							menuItem_Debug.Items.Add(Recompile);
							btn.ButtonMenuItemsSource.Add(menuItem_Debug);
							btn.Click += buttonFunkySettingDB_Click;
							grid.Children.Add(btn);
						}
					}
				)
			);
		}
		internal static void UninstallSettingsButton()
		{
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

						SplitButton FunkyButton = grid.FindName("Funky") as SplitButton;
						if (FunkyButton != null)
						{
							Logger.DBLog.DebugFormat("Funky Button handler added");
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

						if (FunkyButton!=null)
						{
							grid.Children.Remove(FunkyButton);
						}
					}
				)
			);
		
		}
		internal static SplitButton FindFunkyButton()
		{
			try
			{
				

				
			}
			catch(Exception ex)
			{
				Logger.DBLog.Debug("fBaseXtensions: Exception during FindFunkyButton", ex);
				throw;
			}
			return null;
		}

		internal static void AddButtonToDemonbuddyMainTab(SplitButton Button)
		{
			var grid = GetDemonbuddyMainGrid();
			if (grid == null) return;

			grid.Children.Add(Button);
		}
		internal static void AddButtonToDemonbuddyMainTab(Button Button)
		{
			var grid = GetDemonbuddyMainGrid();
			if (grid == null) return;

			grid.Children.Add(Button);
		}

		internal static Grid GetDemonbuddyMainGrid()
		{
			try
			{
				Window mainWindow = App.Current.MainWindow;
				var tab = mainWindow.FindName("tabControlMain") as TabControl;
				if (tab == null) return null;
				var infoDumpTab = tab.Items[0] as TabItem;
				if (infoDumpTab == null) return null;
				var grid = infoDumpTab.Content as Grid;
				if (grid == null) return null;

				return grid;
			}
			catch (Exception ex)
			{
				Logger.DBLog.Debug("fBaseXtensions: Exception during GetDemonbuddyMainGrid", ex);
			}

			return null;
		}

		private static Label lblDebug_OpenLog,  lblDebug_FunkyLog;
		private static MenuItem menuItem_Debug;
		private static readonly log4net.ILog DBLog = Zeta.Common.Logger.GetLoggerInstanceForType();

		public static SplitButton initDebugLabels()
		{
			SplitButton btn = new SplitButton
			{
				Width = 125,
				Height = 20,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Margin = new Thickness(425, 10, 0, 0),
				IsEnabled = true,
				Content = "Funky",
				Name = "Funky",
			};
			btn.Click += lblFunky_Click;

			lblDebug_OpenLog = new Label
			{
				Content = "Open DB LogFile",
				Width = 100,
				Height = 25,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};
			lblDebug_OpenLog.MouseDown += lblDebug_OpenDBLog;

			lblDebug_FunkyLog = new Label
			{
				Content = "Open Funky LogFile",
				Width = 100,
				Height = 25,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};
			lblDebug_FunkyLog.MouseDown += lblDebug_OpenFunkyLog;

			Label OpenTrinityFolder = new Label
			{
				Content = "Open Funky Folder",
				Width = 100,
				Height = 25,
				HorizontalAlignment = HorizontalAlignment.Stretch,

			};
			OpenTrinityFolder.MouseDown += lblDebug_OpenTrinityFolder;

			Label Recompile = new Label
			{
				Content = "Recompile Funky",
				Width = 100,
				Height = 25,
				HorizontalAlignment = HorizontalAlignment.Stretch,

			};
			Recompile.MouseDown += lblCompile_Click;




			menuItem_Debug = new MenuItem
			{
				Header = "Debuging",
				Width = 125
			};
			menuItem_Debug.Items.Add(lblDebug_OpenLog);
			menuItem_Debug.Items.Add(lblDebug_FunkyLog);
			menuItem_Debug.Items.Add(OpenTrinityFolder);
			menuItem_Debug.Items.Add(Recompile);
			btn.ButtonMenuItemsSource.Add(menuItem_Debug);

			return btn;
		}

		static void lblFunky_Click(object sender, EventArgs e)
		{
			try
			{
				BotMain.CurrentBot.ConfigWindow.Show();
			}
			catch
			{

			}

		}
		static void lblCompile_Click(object sender, EventArgs e)
		{
			RecompilePlugins();
		}

		private static void RecompilePlugins()
		{
			if (BotMain.IsRunning)
			{
				BotMain.Stop(false, "Recompiling Plugin!");
				while (BotMain.BotThread.IsAlive)
					Thread.Sleep(0);
			}

			var EnabledPlugins = PluginManager.GetEnabledPlugins().ToArray();

			foreach (var p in PluginManager.Plugins)
			{
				p.Enabled = false;
			}

			PluginManager.ShutdownAllPlugins();

			Logger.DBLog.DebugFormat("Disposing All Routines");
			foreach (var r in RoutineManager.Routines)
			{
				r.Dispose();
			}
			


			string sDemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			string sTrinityPluginPath = FolderPaths.PluginPath;

			CodeCompiler FunkyCode = new CodeCompiler(sTrinityPluginPath);
			//FunkyCode.ParseFilesForCompilerOptions();
			Logger.DBLog.DebugFormat("Recompiling Funky Bot");
			FunkyCode.Compile();
			Logger.DBLog.DebugFormat(FunkyCode.CompiledToLocation);

			Logger.DBLog.DebugFormat("Clearing all treehooks");
			TreeHooks.Instance.ClearAll();

			Logger.DBLog.DebugFormat("Disposing of current bot");
			BotMain.CurrentBot.Dispose();

			Logger.DBLog.DebugFormat("Removing old Assemblies");
			CodeCompiler.DeleteOldAssemblies();

			BrainBehavior.CreateBrain();

			Logger.DBLog.DebugFormat("Reloading Plugins");
			PluginManager.ReloadAllPlugins(sDemonBuddyPath + @"\Plugins\");

			Logger.DBLog.DebugFormat("Enabling Plugins");
			PluginManager.SetEnabledPlugins(EnabledPlugins);

			Logger.DBLog.DebugFormat("Reloading Routines");
			RoutineManager.Reload();
		}
		internal static void RecompileSelectedPlugin(Object sender, EventArgs e)
		{
			var FBD = new System.Windows.Forms.FolderBrowserDialog();
			string sDemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			FBD.SelectedPath = sDemonBuddyPath + @"\Plugins\";
			System.Windows.Forms.DialogResult PluginLocationResult = FBD.ShowDialog();

			if (PluginLocationResult == System.Windows.Forms.DialogResult.OK && !String.IsNullOrEmpty(FBD.SelectedPath))
			{
				Logger.DBLog.DebugFormat(FBD.SelectedPath);

				if (BotMain.IsRunning)
				{
					BotMain.Stop(false, "Recompiling Plugin!");
					while (BotMain.BotThread.IsAlive)
						Thread.Sleep(0);
				}

				var EnabledPlugins = PluginManager.GetEnabledPlugins().ToArray();

				foreach (var p in PluginManager.Plugins)
				{
					p.Enabled = false;
				}

				PluginManager.ShutdownAllPlugins();

				//FunkyBot.Logger.DBLog.DebugFormat("Removing {0} from plugins", FunkyRoutine.lastSelectedPC.Plugin.Name);
				//while (PluginManager.Plugins.Any(p => p.Plugin.Name == FunkyRoutine.lastSelectedPC.Plugin.Name))
				//{
				//	PluginManager.Plugins.Remove(PluginManager.Plugins.First(p => p.Plugin.Name == FunkyRoutine.lastSelectedPC.Plugin.Name));
				//}

				Logger.DBLog.DebugFormat("Clearing all treehooks");
				TreeHooks.Instance.ClearAll();

				Logger.DBLog.DebugFormat("Disposing of current bot");
				BotMain.CurrentBot.Dispose();

				Logger.DBLog.DebugFormat("Removing old Assemblies");
				CodeCompiler.DeleteOldAssemblies();



				CodeCompiler FunkyCode = new CodeCompiler(FBD.SelectedPath);
				//FunkyCode.ParseFilesForCompilerOptions();
				Logger.DBLog.DebugFormat("Recompiling Plugin");
				FunkyCode.Compile();
				Logger.DBLog.DebugFormat(FunkyCode.CompiledToLocation);



				TreeHooks.Instance.ClearAll();
				BrainBehavior.CreateBrain();

				Logger.DBLog.DebugFormat("Reloading Plugins");
				PluginManager.ReloadAllPlugins(sDemonBuddyPath + @"\Plugins\");

				Logger.DBLog.DebugFormat("Enabling Plugins");
				PluginManager.SetEnabledPlugins(EnabledPlugins);
			}
		}
		static void lblDebug_OpenDBLog(object sender, EventArgs e)
		{
			try
			{
				string sDemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

				FileInfo demonbuddyLogFolder = new FileInfo(sDemonBuddyPath + @"\Logs\");
				if (demonbuddyLogFolder.Directory != null && !demonbuddyLogFolder.Directory.GetFiles().Any())
					return;
				int pid = Process.GetCurrentProcess().Id;
				var newestfile = demonbuddyLogFolder.Directory.GetFiles().First(f => f.Name.Contains(pid.ToString()));
				try
				{
					Process.Start(newestfile.FullName);
				}
				catch (Exception)
				{

				}
			}
			catch (Exception)
			{

			}
		}
		static void lblDebug_OpenFunkyLog(object sender, EventArgs e)
		{
			string sDemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			FileInfo demonbuddyLogFolder = new FileInfo(sDemonBuddyPath + @"\Logs\");
			if (demonbuddyLogFolder.Directory != null && !demonbuddyLogFolder.Directory.GetFiles().Any())
				return;

			var newestfile = demonbuddyLogFolder.Directory.GetFiles().Where(f => f.Name.Contains("FunkyLog")).OrderByDescending(file => file.LastWriteTime).First();
			try
			{
				Process.Start(newestfile.FullName);
			}
			catch (Exception)
			{

			}
		}

		static void lblDebug_OpenTrinityFolder(object sender, EventArgs e)
		{
			string sDemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			Process.Start(sDemonBuddyPath + @"\Plugins\FunkyBot\");
		}

		internal static void buttonFunkySettingDB_Click(object sender, RoutedEventArgs e)
		{
			//Update Account Details when bot is not running!
			//if (!BotMain.IsRunning)
				//Bot.Character.Account.UpdateCurrentAccountDetails();

			string settingsFolder = FolderPaths.DemonBuddyPath + @"\Settings\FunkyBot\" + FunkyGame.CurrentAccountName;
			if (!Directory.Exists(settingsFolder)) Directory.CreateDirectory(settingsFolder);

			try
			{
				PluginSettings.LoadSettings();
				FrmSettings = new SettingsForm();
				FrmSettings.Show();
			}
			catch (Exception ex)
			{
				Logger.DBLog.InfoFormat("Failure to initilize Funky Setting Window! \r\n {0} \r\n {1} \r\n {2}",
					ex.Message, ex.Source, ex.StackTrace);
				if (ex.InnerException != null)
				{
					Logger.DBLog.InfoFormat("Inner Exception: {0}\r\n{1}\r\n{2}", ex.InnerException.Message, ex.InnerException.Source, ex.InnerException.StackTrace);
				}
			}


		}
	}
}
