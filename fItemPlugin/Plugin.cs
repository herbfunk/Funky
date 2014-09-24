using System;
using System.IO;
using System.Windows;
using fBaseXtensions;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using fItemPlugin.ItemRules;
using fItemPlugin.Townrun;
using Zeta.Bot;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fItemPlugin
{
	public partial class FunkyTownRunPlugin : IPlugin
	{
		public Version Version { get { return new Version(1, 2, 5); } }
		public string Name { get { return "fItemPlugin"; } }
		public string Author { get { return "HerbFunk"; } }
		public string Description
		{
			get { return "An Item Plugin - with vendor behavior replacement!"; }
		}
		formSettings settingsWindow;
		public Window DisplayWindow
		{
			get
			{
				Settings.LoadSettings();

				settingsWindow = new formSettings();

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

		public void OnDisabled()
		{
			BotMain.OnStart -= FunkyBotStart;
			BotMain.OnStop -= FunkyBotStop;
		}

		public void OnEnabled()
		{
			//var basePlugin = PluginManager.Plugins.First(p => p.Plugin.Name == "fBaseXtensions");
			//if (basePlugin != null)
			//{
			//	if (!basePlugin.Enabled)
			//	{
			//		DBLog.Warn("FunkyTownRun requires fBaseXtensions to be enabled! -- Enabling it automatically.");
			//		basePlugin.Enabled = true;
			//	}
			//}

			BotMain.OnStart += FunkyBotStart;
			BotMain.OnStop += FunkyBotStop;
			if (BotMain.IsRunning) FunkyBotStart(null);
		}

		public void OnInitialize()
		{

		}

		public void OnPulse()
		{
			if (TreehookHandling.initTreeHooks && !TreehookHandling.CheckVendorHooked())
			{
				Logger.DBLog.InfoFormat("Replacing Vendor Hook!");
				HookHandler.RestoreHook(HookHandler.HookType.VendorRun);
				TreehookHandling.HookBehaviorTree();
			}
		}

		public void OnShutdown()
		{

		}

		public bool Equals(IPlugin other) { return (other.Name == Name) && (other.Version == Version); }


		public static Settings PluginSettings = new Settings();
		public static Interpreter ItemRulesEval;

		internal static readonly log4net.ILog DBLog = Zeta.Common.Logger.GetLoggerInstanceForType();

		internal static void LogGoodItems(CacheACDItem thisgooditem, PluginBaseItemTypes thisPluginBaseItemTypes, PluginItemTypes thisPluginItemType)
		{

			try
			{
				//Update this item
				using (ZetaDia.Memory.AcquireFrame())
				{
					thisgooditem = new CacheACDItem(thisgooditem.ACDItem);
				}
			}
			catch
			{
				DBLog.DebugFormat("Failure to update CacheACDItem during Logging");
			}
			//double iThisItemValue = ItemFunc.ValueThisItem(thisgooditem, thisPluginItemType);

			FileStream LogStream = null;
			try
			{
				string outputPath = FolderPaths.LoggingFolderPath + @"\StashLog.log";

				LogStream = File.Open(outputPath, FileMode.Append, FileAccess.Write, FileShare.Read);
				using (StreamWriter LogWriter = new StreamWriter(LogStream))
				{
					if (!TownRunManager.bLoggedAnythingThisStash)
					{
						TownRunManager.bLoggedAnythingThisStash = true;
						LogWriter.WriteLine(DateTime.Now.ToString() + ":");
						LogWriter.WriteLine("====================");
					}
					string sLegendaryString = "";
					if (thisgooditem.ThisQuality >= ItemQuality.Legendary)
					{
						if (!thisgooditem.IsUnidentified)
						{
							//Prowl.AddNotificationToQueue(thisgooditem.ThisRealName + " [" + thisPluginItemType.ToString() + "] (Score=" + iThisItemValue.ToString() + ". " + TownRunManager.sValueItemStatString + ")", ZetaDia.Service.Hero.Name + " new legendary!", Prowl.ProwlNotificationPriority.Emergency);
							sLegendaryString = " {legendary item}";
							// Change made by bombastic
							DBLog.Info("+=+=+=+=+=+=+=+=+ LEGENDARY FOUND +=+=+=+=+=+=+=+=+");
							DBLog.Info("+  Name:       " + thisgooditem.ThisRealName + " (" + thisPluginItemType.ToString() + ")");
							//DBLog.Info("+  Score:       " + Math.Round(iThisItemValue).ToString());
							DBLog.Info("+  Attributes: " + thisgooditem.ItemStatString);
							DBLog.Info("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+");
						}
						else
						{
							DBLog.Info("+=+=+=+=+=+=+=+=+ LEGENDARY FOUND +=+=+=+=+=+=+=+=+");
							DBLog.Info("+  Unid:       " + thisPluginItemType.ToString());
							DBLog.Info("+  Level:       " + thisgooditem.ThisLevel.ToString());
							DBLog.Info("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+");
						}


					}
					else
					{
						// Check for non-legendary notifications
						bool bShouldNotify = false;
						switch (thisPluginBaseItemTypes)
						{
							case PluginBaseItemTypes.WeaponOneHand:
							case PluginBaseItemTypes.WeaponRange:
							case PluginBaseItemTypes.WeaponTwoHand:
								//if (ithisitemvalue >= settings.iNeedPointsToNotifyWeapon)
								//  bShouldNotify = true;
								break;
							case PluginBaseItemTypes.Armor:
							case PluginBaseItemTypes.Offhand:
								//if (ithisitemvalue >= settings.iNeedPointsToNotifyArmor)
								//bShouldNotify = true;
								break;
							case PluginBaseItemTypes.Jewelry:
								//if (ithisitemvalue >= settings.iNeedPointsToNotifyJewelry)
								//bShouldNotify = true;
								break;
						}
						//if (bShouldNotify)
						//Prowl.AddNotificationToQueue(thisgooditem.ThisRealName + " [" + thisPluginItemType.ToString() + "] (Score=" + iThisItemValue.ToString() + ". " + TownRunManager.sValueItemStatString + ")", ZetaDia.Service.Hero.Name + " new item!", Prowl.ProwlNotificationPriority.Emergency);
					}
					if (!thisgooditem.IsUnidentified)
					{

						LogWriter.WriteLine(thisgooditem.ThisQuality.ToString() + "  " + thisPluginItemType.ToString() + " '" + thisgooditem.ThisRealName + sLegendaryString);
						LogWriter.WriteLine("  " + thisgooditem.ItemStatString);
						LogWriter.WriteLine("");
					}
					else
					{
						LogWriter.WriteLine(thisgooditem.ThisQuality.ToString() + "  " + thisPluginItemType.ToString() + " '" + sLegendaryString);
						LogWriter.WriteLine("iLevel " + thisgooditem.ThisLevel.ToString());
						LogWriter.WriteLine("");
					}
				}

			}
			catch (IOException)
			{
				DBLog.Info("Fatal Error: File access error for stash log file.");
			}
		}

		internal static void LogJunkItems(CacheACDItem thisgooditem, PluginBaseItemTypes thisPluginBaseItemTypes, PluginItemTypes thisPluginItemType)
		{
			FileStream LogStream = null;
			string outputPath = FolderPaths.LoggingFolderPath + @"\JunkLog.log";

			try
			{
				LogStream = File.Open(outputPath, FileMode.Append, FileAccess.Write, FileShare.Read);
				using (StreamWriter LogWriter = new StreamWriter(LogStream))
				{
					if (!TownRunManager.bLoggedJunkThisStash)
					{
						TownRunManager.bLoggedJunkThisStash = true;
						LogWriter.WriteLine(DateTime.Now.ToString() + ":");
						LogWriter.WriteLine("====================");
					}
					string sLegendaryString = "";
					if (thisgooditem.ThisQuality >= ItemQuality.Legendary)
						sLegendaryString = " {legendary item}";
					LogWriter.WriteLine(thisgooditem.ThisQuality.ToString() + " " + thisPluginItemType.ToString() + " '" + thisgooditem.ThisRealName + sLegendaryString);
					LogWriter.Write(thisgooditem.ItemStatProperties.ReturnPrimaryStatString());
					LogWriter.WriteLine("");
				}

			}
			catch (IOException)
			{
				DBLog.Info("Fatal Error: File access error for junk log file.");
			}
		}

		internal static bool RunningTrinity = false;

		private void FunkyBotStart(IBot bot)
		{
			DBLog.Info("===================");
			DBLog.Info("Funky Town Run Plugin");
			DBLog.InfoFormat("Version {0}", Version.ToString());
			DBLog.Info("===================");

			if (!TreehookHandling.initTreeHooks)
				TreehookHandling.HookBehaviorTree();

			RunningTrinity = RoutineManager.Current.Name == "Trinity";

			Settings.LoadSettings();

			if (PluginSettings.UseItemRules)
				ItemRulesEval = new Interpreter();
		}
		private void FunkyBotStop(IBot bot)
		{
			RunningTrinity = false;
			TreehookHandling.initTreeHooks = false;
			HookHandler.RestoreHook(HookHandler.HookType.VendorRun);
		}
	}
}
