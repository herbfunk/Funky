using System;
using System.IO;
using System.Windows;
using fItemPlugin.ItemRules;
using fItemPlugin.Items;
using fItemPlugin.Player;
using fItemPlugin.Townrun;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace fItemPlugin
{
    public partial class FunkyTownRunPlugin : IPlugin
    {
		public string Name { get { return "fItemPlugin"; } }
		public string Author { get { return "HerbFunk"; } }
		public string Description
		{
			get { return "An Item Plugin - with vendor behavior replacement!"; }
		}
		public Window DisplayWindow
		{
			get
			{
				Settings.LoadSettings();

				try
				{
					FunkyWindow.funkyConfigWindow = new FunkyWindow();

				}
				catch (Exception ex)
				{
					DBLog.DebugFormat("Failure to initilize Funky Setting Window! \r\n {0} \r\n {1} \r\n {2}", ex.Message, ex.Source, ex.StackTrace);
				}

				return FunkyWindow.funkyConfigWindow;
			}
		}

		public void OnDisabled()
		{
			BotMain.OnStart -= FunkyBotStart;
			BotMain.OnStop -= FunkyBotStop;
		}

		public void OnEnabled()
		{
			BotMain.OnStart += FunkyBotStart;
			BotMain.OnStop += FunkyBotStop;
			if (BotMain.IsRunning) FunkyBotStart(null);
		}

		public void OnInitialize()
		{
			ItemSnoCache.LoadItemIds();
		}

		public void OnPulse()
		{
			if (initTreeHooks && !trinityCheck)
			{
				trinityCheck = true;

				if (RunningTrinity)
				{
					var vendorDecorator = TreeHooks.Instance.Hooks["VendorRun"][0] as Decorator;
					var replacementvendorDecorator = new Decorator(VendorCanRunDelegate, VendorPrioritySelector);
					TreeHooks.Instance.ReplaceHook("VendorRun", replacementvendorDecorator);
					DBLog.DebugFormat("Replaced Trinity Townrun Replacement!?");
				}

				
				//if (vendorDecorator.DecoratedChild
			}

			Equipment.CheckEquippment();
		}

		public void OnShutdown()
		{
			
		}
		
		public bool Equals(IPlugin other) { return (other.Name == Name) && (other.Version == Version); }

	
		public static Settings PluginSettings = new Settings();
		public static Interpreter ItemRulesEval;

		internal static readonly log4net.ILog DBLog = Zeta.Common.Logger.GetLoggerInstanceForType();

		internal static void LogGoodItems(CacheACDItem thisgooditem, PluginBaseItemType thisPluginBaseItemType, PluginItemType thisPluginItemType)
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
				string outputPath = FolderPaths.LoggingFolderPath + @"\" + Character.CurrentHeroName + " -- StashLog.log";

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
						switch (thisPluginBaseItemType)
						{
							case PluginBaseItemType.WeaponOneHand:
							case PluginBaseItemType.WeaponRange:
							case PluginBaseItemType.WeaponTwoHand:
								//if (ithisitemvalue >= settings.iNeedPointsToNotifyWeapon)
								//  bShouldNotify = true;
								break;
							case PluginBaseItemType.Armor:
							case PluginBaseItemType.Offhand:
								//if (ithisitemvalue >= settings.iNeedPointsToNotifyArmor)
								//bShouldNotify = true;
								break;
							case PluginBaseItemType.Jewelry:
								//if (ithisitemvalue >= settings.iNeedPointsToNotifyJewelry)
								//bShouldNotify = true;
								break;
						}
						//if (bShouldNotify)
							//Prowl.AddNotificationToQueue(thisgooditem.ThisRealName + " [" + thisPluginItemType.ToString() + "] (Score=" + iThisItemValue.ToString() + ". " + TownRunManager.sValueItemStatString + ")", ZetaDia.Service.Hero.Name + " new item!", Prowl.ProwlNotificationPriority.Emergency);
					}
					if (!thisgooditem.IsUnidentified)
					{
						LogWriter.WriteLine(thisPluginBaseItemType.ToString() + " - " + thisPluginItemType.ToString() + " '" + thisgooditem.ThisRealName + sLegendaryString);
						LogWriter.WriteLine("  " + thisgooditem.ItemStatString);
						LogWriter.WriteLine("");
					}
					else
					{
						LogWriter.WriteLine(thisPluginBaseItemType.ToString() + " - " + thisPluginItemType.ToString() + " '" + sLegendaryString);
						LogWriter.WriteLine("  " + thisgooditem.ThisLevel.ToString());
						LogWriter.WriteLine("");
					}
				}

			}
			catch (IOException)
			{
				DBLog.Info("Fatal Error: File access error for stash log file.");
			}
		}

		internal static void LogJunkItems(CacheACDItem thisgooditem, PluginBaseItemType thisPluginBaseItemType, PluginItemType thisPluginItemType)
		{
			FileStream LogStream = null;
			string outputPath = FolderPaths.LoggingFolderPath + @"\" + Character.CurrentHeroName + " -- JunkLog.log";

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
					LogWriter.WriteLine(thisPluginBaseItemType.ToString() + " - " + thisPluginItemType.ToString() + " '" + thisgooditem.ThisRealName + sLegendaryString);
					LogWriter.WriteLine("  (no scorable attributes)");
					LogWriter.WriteLine("");
				}

			}
			catch (IOException)
			{
				DBLog.Info("Fatal Error: File access error for junk log file.");
			}
		}

		internal static void LogTownRunStats()
		{
			FileStream LogStream = null;
			string outputPath = FolderPaths.LoggingFolderPath + @"\" + Character.CurrentHeroName + " -- TownRunStats.log";
			try
			{
				LogStream = File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read);
				using (StreamWriter LogWriter = new StreamWriter(LogStream))
				{
					LogWriter.Write(TownRunStats.GenerateOutputString());
				}
			}
			catch (IOException)
			{
				DBLog.Info("Fatal Error: File access error for town run stats log file.");
			}
		}

		internal static Stats TownRunStats = new Stats();

		internal static bool initTreeHooks;
		internal static bool trinityCheck;
		internal static PrioritySelector VendorPrioritySelector;
		internal static CanRunDecoratorDelegate VendorCanRunDelegate;
		internal static bool RunningTrinity = false;

		internal static void HookBehaviorTree()
		{

			bool idenify = true, unidfystash = true, stash = true, vendor = true, salvage = true;

			DBLog.InfoFormat("[Funky] Treehooks..");
			#region TreeHooks
			foreach (var hook in TreeHooks.Instance.Hooks)
			{
				#region VendorRun

				// Wipe the vendorrun and loot behavior trees, since we no longer want them
				if (hook.Key.Contains("VendorRun"))
				{
					DBLog.DebugFormat("VendorRun...");

					Decorator GilesDecorator = hook.Value[0] as Decorator;
					//PrioritySelector GilesReplacement = GilesDecorator.Children[0] as PrioritySelector;
					PrioritySelector GilesReplacement = GilesDecorator.Children[0] as PrioritySelector;

					ActionDelegate actionDelegatePrePause = TownRunManager.GilesStashPrePause;
					ActionDelegate actionDelegatePause = TownRunManager.GilesStashPause;

					#region Idenify


					if (idenify)
					{
						CanRunDecoratorDelegate canRunDelegateFunkyIDManual = TownRunManager.IdenifyItemManualOverlord;
						ActionDelegate actionDelegateIDManual = TownRunManager.IdenifyItemManualBehavior;
						ActionDelegate actionDelegateIDFinish = TownRunManager.IdenifyItemManualFinishBehavior;
						Sequence sequenceIDManual = new Sequence(
							new Action(actionDelegateIDManual),
							new Action(actionDelegateIDFinish)
						);

						CanRunDecoratorDelegate canRunDelegateFunkyIDBookOfCain = TownRunManager.IdenifyItemBookOfCainOverlord;
						ActionDelegate actionDelegateIDBookOfCainMovement = TownRunManager.IdenifyItemBookOfCainMovementBehavior;
						ActionDelegate actionDelegateIDBookOfCainInteraction = TownRunManager.IdenifyItemBookOfCainInteractionBehavior;
						Sequence sequenceIDBookOfCain = new Sequence(
							new Action(actionDelegateIDBookOfCainMovement),
							new Action(actionDelegateIDBookOfCainInteraction),
							new Action(actionDelegateIDFinish)
						);


						PrioritySelector priorityIDItems = new PrioritySelector(
							new Decorator(canRunDelegateFunkyIDManual, sequenceIDManual),
							new Decorator(canRunDelegateFunkyIDBookOfCain, sequenceIDBookOfCain)
						);

						CanRunDecoratorDelegate canRunDelegateFunkyIDOverlord = TownRunManager.IdenifyItemOverlord;
						GilesReplacement.Children[2] = new Decorator(canRunDelegateFunkyIDOverlord, priorityIDItems);

						DBLog.DebugFormat("Town Run - Idenify Items - hooked...");
					}


					#endregion

					// Replace the pause just after identify stuff to ensure we wait before trying to run to vendor etc.
					CanRunDecoratorDelegate canRunDelegateEvaluateAction = TownRunManager.ActionsEvaluatedOverlord;
					ActionDelegate actionDelegateEvaluateAction = TownRunManager.ActionsEvaluatedBehavior;

					Sequence sequenceEvaluate = new Sequence(
							    new Action(actionDelegatePrePause),
							    new Action(actionDelegatePause),
								new Action(actionDelegateEvaluateAction)
							);

					GilesReplacement.Children[3] = new Decorator(canRunDelegateEvaluateAction, sequenceEvaluate);



					#region Salvage
					if (salvage)
					{
						// Replace DB salvaging behavior tree with my optimized & "one-at-a-time" version
						CanRunDecoratorDelegate canRunDelegateSalvageGilesOverlord = TownRunManager.GilesSalvageOverlord;
						ActionDelegate actionDelegatePreSalvage = TownRunManager.GilesOptimisedPreSalvage;
						ActionDelegate actionDelegateSalvage = TownRunManager.GilesOptimisedSalvage;
						ActionDelegate actionDelegatePostSalvage = TownRunManager.GilesOptimisedPostSalvage;
						Sequence sequenceSalvage = new Sequence(
								new Action(actionDelegatePreSalvage),
								new Action(actionDelegateSalvage),
								new Action(actionDelegatePostSalvage),
								new Sequence(
								new Action(actionDelegatePrePause),
								new Action(actionDelegatePause)
								)
								);
						GilesReplacement.Children[4] = new Decorator(canRunDelegateSalvageGilesOverlord, sequenceSalvage);
						DBLog.DebugFormat("Town Run - Salvage - hooked...");
					}
					#endregion

					#region Stash
					if (stash)
					{
						// Replace DB stashing behavior tree with my optimized version with loot rule replacement
						CanRunDecoratorDelegate canRunDelegateStashGilesOverlord = TownRunManager.StashOverlord;
						ActionDelegate actionDelegatePreStash = TownRunManager.PreStash;
						ActionDelegate actionDelegatePostStash = TownRunManager.PostStash;

						ActionDelegate actionDelegateStashMovement = TownRunManager.StashMovement;
						ActionDelegate actionDelegateStashUpdate = TownRunManager.StashUpdate;
						ActionDelegate actionDelegateStashItems = TownRunManager.StashItems;

						Sequence sequencestash = new Sequence(
								new Action(actionDelegatePreStash),
								new Action(actionDelegateStashMovement),
								new Action(actionDelegateStashUpdate),
								new Action(actionDelegateStashItems),
								new Action(actionDelegatePostStash),
								new Sequence(
								new Action(actionDelegatePrePause),
								new Action(actionDelegatePause)
								)
								);
						GilesReplacement.Children[5] = new Decorator(canRunDelegateStashGilesOverlord, sequencestash);
						DBLog.DebugFormat("Town Run - Stash - hooked...");
					}
					#endregion

					#region Vendor
					if (vendor)
					{
						// Replace DB vendoring behavior tree with my optimized & "one-at-a-time" version
						CanRunDecoratorDelegate canRunDelegateSellGilesOverlord = TownRunManager.GilesSellOverlord;
						ActionDelegate actionDelegatePreSell = TownRunManager.GilesOptimisedPreSell;
						ActionDelegate actionDelegateMovement = TownRunManager.VendorMovement;
						ActionDelegate actionDelegateSell = TownRunManager.GilesOptimisedSell;
						ActionDelegate actionDelegatePostSell = TownRunManager.GilesOptimisedPostSell;
						Sequence sequenceSell = new Sequence(
								new Action(actionDelegatePreSell),
								new Action(actionDelegateMovement),
								new Action(actionDelegateSell),
								new Action(actionDelegatePostSell),
								new Sequence(
								new Action(actionDelegatePrePause),
								new Action(actionDelegatePause)
								)
								);
						GilesReplacement.Children[6] = new Decorator(canRunDelegateSellGilesOverlord, sequenceSell);
						DBLog.DebugFormat("Town Run - Vendor - hooked...");
					}

					#endregion



					#region UnidfyStash

					//if (unidfystash)
					//{
					//	CanRunDecoratorDelegate canRunUnidBehavior = TownRunManager.UnidItemOverlord;
					//	ActionDelegate actionDelegatePreUnidStash = TownRunManager.PreStash;
					//	ActionDelegate actionDelegatePostUnidStash = TownRunManager.PostStash;

					//	ActionDelegate actionDelegateStashMovement = TownRunManager.StashMovement;
					//	ActionDelegate actionDelegateStashUpdate = TownRunManager.StashUpdate;
					//	ActionDelegate actionDelegateStashItems = TownRunManager.StashItems;

					//	Sequence sequenceUnidStash = new Sequence(
					//			new Action(actionDelegatePreUnidStash),
					//			new Action(actionDelegateStashMovement),
					//			new Action(actionDelegateStashUpdate),
					//			new Action(actionDelegateStashItems),
					//			new Action(actionDelegatePostUnidStash),
					//			new Sequence(
					//			new Action(actionDelegatePrePause),
					//			new Action(actionDelegatePause)
					//			)
					//		  );

					//	//Insert Before Item ID Step
					//	GilesReplacement.InsertChild(2, new Decorator(canRunUnidBehavior, sequenceUnidStash));
					//	DBLog.DebugFormat("Town Run - Undify Stash - inserted...");
					//}
					#endregion



					#region Gambling Behavior

					CanRunDecoratorDelegate canRunDelegateGambling = TownRunManager.GamblingRunOverlord;
					ActionDelegate actionDelegateGamblingMovementBehavior = TownRunManager.GamblingMovement;
					ActionDelegate actionDelegateGamblingInteractionBehavior = TownRunManager.GamblingInteraction;
					ActionDelegate actionDelegateGamblingStartBehavior = TownRunManager.GamblingStart;
					ActionDelegate actionDelegateGamblingFinishBehavior = TownRunManager.GamblingFinish;

					Sequence sequenceGambling = new Sequence(
							new Action(actionDelegateGamblingStartBehavior),
							new Action(actionDelegateGamblingMovementBehavior),
							new Action(actionDelegateGamblingInteractionBehavior),
							new Action(actionDelegateGamblingFinishBehavior)
						);
					GilesReplacement.InsertChild(7, new Decorator(canRunDelegateGambling, sequenceGambling));
					DBLog.DebugFormat("Town Run - Gambling Behavior - Inserted...");

					#endregion

					#region Finish Behavior

					ActionDelegate actionDelegateFinishBehavior = TownRunManager.ActionsEvaluatedEndingBehavior;
					Action actionFinish = GilesReplacement.Children[9] as Action;
					Sequence sequenceFinish = new Sequence(
							new Action(actionDelegateFinishBehavior),
							actionFinish
						);
					DBLog.DebugFormat("Town Run - Finish Behavior - Inserted...");
					GilesReplacement.Children[9] = sequenceFinish;
					#endregion
					

					CanRunDecoratorDelegate canRunDelegateGilesTownRunCheck = TownRunManager.TownRunCheckOverlord;
					VendorCanRunDelegate = canRunDelegateGilesTownRunCheck;
					VendorPrioritySelector = GilesReplacement;

					hook.Value[0] = new Decorator(canRunDelegateGilesTownRunCheck, GilesReplacement);
					DBLog.DebugFormat("Vendor Run tree hooked...");
				} // Vendor run hook


				#endregion
			}
			#endregion
			initTreeHooks = true;
		}



		private void FunkyBotStart(IBot bot)
		{
			DBLog.Info("===================");
			DBLog.Info("Funky Town Run Plugin");
			DBLog.InfoFormat("Version {0}", Version.ToString());
			DBLog.Info("===================");

			if (!initTreeHooks)
				HookBehaviorTree();

			RunningTrinity = RoutineManager.Current.Name == "Trinity";

			Settings.LoadSettings();

			if (PluginSettings.UseItemRules)
				ItemRulesEval = new Interpreter();
		}
		private void FunkyBotStop(IBot bot)
		{
			RunningTrinity = false;
			initTreeHooks = false;
			trinityCheck = false;
			BrainBehavior.CreateVendorBehavior();
		}
	}
}
