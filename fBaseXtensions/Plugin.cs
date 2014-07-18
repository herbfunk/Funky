using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using fBaseXtensions.Cache;
using fBaseXtensions.Cache.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Monitor;
using fBaseXtensions.Settings;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Common.Plugins;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions
{
    public class FunkyBaseExtension : IPlugin
    {
		public static PluginSettings Settings { get; set; }

	    public void OnPulse()
	    {
			
			if (FunkyGame.GameIsInvalid) return;

			//in-game monitoring
			FunkyGame.Profile.CheckCurrentProfileBehavior();
			GoldInactivity.CheckTimeoutTripped();
			Hotbar.CheckSkills();
			Equipment.CheckEquippment();
	    }

	    public void OnInitialize()
	    {
		    Settings=new PluginSettings();
			PluginSettings.LoadSettings();

			TheCache.ObjectIDCache=new IDCache();
			//ItemSnoDataCollection.SerializeToXML(TheCache.ObjectIDCache.ItemsSno);
			//ItemStringDataCollection.SerializeToXML(SNOCache.IdCollections.ItemsString);
	    }

	    public void OnShutdown()
	    {
		   
	    }

	    public void OnEnabled()
	    {
			Logger.DBLog.InfoFormat("fBaseXtensions has been enabled!");
			BotMain.OnStart += EventHandling.OnBotStart;
			BotMain.OnStop += EventHandling.OnBotStop;
			if (BotMain.IsRunning) EventHandling.OnBotStart(null);
	    }

	    public void OnDisabled()
	    {
			Logger.DBLog.InfoFormat("fBaseXtensions has been disabled!");
	    }

		public Version Version
		{
			get { return new Version(0, 0, 1); }
		}
		public string Author
		{
			get { return "HerbFunk"; }
		}
		public string Description
		{
			get { return "Base Extension for Additional Functionality"; }
		}
		SettingsWindow settingsWindow;
		public Window DisplayWindow
		{
			get
			{
				PluginSettings.LoadSettings();
				settingsWindow = new SettingsWindow();

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
    }
}
