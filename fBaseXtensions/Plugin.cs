using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Demonbuddy;
using fBaseXtensions.Behaviors;
using fBaseXtensions.Cache;
using fBaseXtensions.Cache.External;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Helpers;
using fBaseXtensions.Monitor;
using fBaseXtensions.Settings;
using fBaseXtensions.XML;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions
{
    public class FunkyBaseExtension : IPlugin
    {
        public Version Version
        {
            get { return new Version(1, 1, 6, 0); }
        }

		public static PluginSettings Settings { get; set; }


	    public void OnPulse()
	    {
			if (!HookHandler.CheckCombatHook())
			{
				Logger.DBLog.Info("Hooking Combat!");
				HookHandler.HookCombat();
			}

			if (FunkyGame.GameIsInvalid) 
				return;


			//in-game monitoring
			FunkyGame.Profile.CheckCurrentProfileBehavior();
			GoldInactivity.CheckTimeoutTripped();
			Hotbar.CheckSkills();
			Equipment.CheckEquippment();
			

			if (FunkyGame.AdventureMode)
				FunkyGame.Bounty.CheckActiveBounty();
	    }

	    public void OnInitialize()
	    {
			Logger.DBLog.DebugFormat("fBaseXtensions OnInitialize Started");
			
			if (File.Exists(FolderPaths.PluginPath + @"\CombatRoutine.cs"))
			{
				try
				{

					if (File.Exists(FolderPaths.RoutinePath + @"\CombatRoutine.cs"))
						File.Delete(FolderPaths.RoutinePath + @"\CombatRoutine.cs");

					File.Copy(FolderPaths.PluginPath + @"\CombatRoutine.cs", FolderPaths.RoutinePath + @"\CombatRoutine.cs");
					Logger.DBLog.DebugFormat("fBaseXtensions Copied Combat Routine");

				}
				catch (Exception ex)
				{
					Logger.DBLog.DebugFormat("fBaseXtensions Copy Combat Routine Threw Exception", ex);
				}
			}

		    Settings=new PluginSettings();
			PluginSettings.LoadSettings();
			TheCache.ObjectIDCache = new IDCache();
	        CharacterControl.OrginalGameDifficultySetting = CharacterSettings.Instance.GameDifficulty;
			BotMain.OnStart += EventHandling.OnBotStart;
			BotMain.OnStop += EventHandling.OnBotStop;
	        CustomConditions.Initialize();
			ObjectCache.FakeCacheObject = new CacheObject(Vector3.Zero, TargetType.None, 0d, "Fake Target", 1f);
	        CharacterSettings.Instance.PropertyChanged += CharacterControl.OnDBCharacterSettingPropertyChanged;
			Logger.Write("Init Logger Completed!");
			Logger.DBLog.DebugFormat("fBaseXtensions OnInitialize Finished");
	    }

	    public void OnShutdown()
	    {
			Logger.DBLog.DebugFormat("fBaseXtensions OnShutdown Started");
			

			Logger.DBLog.DebugFormat("fBaseXtensions OnShutdown Finished");
	    }

		public static bool PluginIsEnabled
		{
		    get
		    {
		        var plugin= PluginManager.Plugins.First(p => p.Plugin.Name == "fBaseXtensions");
		        if (plugin != null && plugin.Enabled)
		        {
		            return true;
		        }
                return _pluginenabled; //?
		        
		    }
		}

        private static bool _pluginenabled = false;
		
	    public void OnEnabled()
	    {
			UIControl.InstallSettingsButton();
            _pluginenabled = true;
			Logger.DBLog.InfoFormat("fBaseXtensions v{0} has been enabled!", Version.ToString());
	        
			if (BotMain.IsRunning) EventHandling.OnBotStart(null);
	    }

	    public void OnDisabled()
	    {
			UIControl.UninstallSettingsButton();
            _pluginenabled = false;
			Logger.DBLog.InfoFormat("fBaseXtensions v{0} has been disabled!", Version.ToString());
	    }

		
		public string Author
		{
			get { return "HerbFunk"; }
		}
		public string Description
		{
			get { return "Base Extension for Additional Functionality"; }
		}
		SettingsForm settingsWindow;
		public Window DisplayWindow
		{
			get
			{
				PluginSettings.LoadSettings();
				settingsWindow = new SettingsForm();

				Window fakeWindow = new Window
				{
					Width = 0,
					Height = 0,
					WindowStartupLocation = WindowStartupLocation.Manual,
				};
				fakeWindow.Initialized += (sender, args) =>
				{
					settingsWindow.ShowDialog();
				};
				fakeWindow.Loaded += (sender, args) =>
				{
					fakeWindow.Close();
				};

				return fakeWindow;
			}
		}

		public string Name
		{
			get { return "fBaseXtensions"; }
		}



	    public bool Equals(IPlugin other) { return (other.Name == Name) && (other.Version == Version); }


		internal static float Difference(float A, float B)
		{
			if (A > B)
				return A - B;

			return B - A;
		}
    }
}
