using System;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using System.Linq;
using System.IO;
using FunkyBot.Player;


namespace FunkyBot.DBHandlers
{

	internal static partial class TownRunManager
	{
		// **********************************************************************************************
		// *****         Salvage Overlord determines if we should visit the blacksmith or not       *****
		// **********************************************************************************************
		internal static bool GilesSalvageOverlord(object ret)
		{
			townRunItemCache.SalvageItems.Clear();


			//Get new list of current backpack
			Bot.Character.Data.BackPack.Update();

			//Refresh item manager if we are not using item rules nor giles scoring.
			if (!Bot.Settings.ItemRules.UseItemRules && !Bot.Settings.ItemRules.ItemRuleGilesScoring)
				ItemManager.Current.Refresh();

			foreach (var thisitem in Bot.Character.Data.BackPack.CacheItemList.Values)
			{
				if (thisitem.ACDItem.BaseAddress != IntPtr.Zero)
				{
					// Find out if this item's in a protected bag slot
					if (!ItemManager.Current.ItemIsProtected(thisitem.ACDItem))
					{
						if (thisitem.ThisDBItemType == ItemType.Potion || thisitem.IsVendorBought) continue;

						if (Bot.Settings.ItemRules.ItemRulesSalvaging)
						{
							if (Bot.Character.ItemRulesEval.checkSalvageItem(thisitem.ACDItem) == Interpreter.InterpreterAction.SALVAGE)
							{
								townRunItemCache.SalvageItems.Add(thisitem);
								continue;
							}
						}



						//Logger.DBLog.InfoFormat("GilesTrinityScoring == "+Bot.SettingsFunky.ItemRules.ItemRuleGilesScoring.ToString());

						bool bShouldVisitSalvage = ItemManager.Current.ShouldStashItem(thisitem.ACDItem);

						if (bShouldVisitSalvage)
							townRunItemCache.SalvageItems.Add(thisitem);

					}
				}
				else
				{
					Logger.DBLog.DebugFormat("GSError: Diablo 3 memory read error, or item became invalid [StashOver-1]");
				}
			}

			if (townRunItemCache.SalvageItems.Count > 0)
			{
				townRunItemCache.sortSalvagelist();
				return true;
			}

			return false;
		}

		// **********************************************************************************************
		// *****             Pre Salvage sets everything up ready for our blacksmith run            *****
		// **********************************************************************************************

		internal static RunStatus GilesOptimisedPreSalvage(object ret)
		{
			if (Bot.Settings.Debug.DebugStatusBar)
				BotMain.StatusText = "Town run: Salvage routine started";
			Logger.DBLog.DebugFormat("GSDebug: Salvage routine started.");

			if (ZetaDia.Actors.Me == null)
			{
				Logger.DBLog.DebugFormat("GSError: Diablo 3 memory read error, or item became invalid [PreSalvage-1]");
				return RunStatus.Failure;
			}

			bLoggedJunkThisStash = false;
			bCurrentlyMoving = false;
			iCurrentItemLoops = 0;
			RandomizeTheTimer();
			return RunStatus.Success;
		}

		// **********************************************************************************************
		// *****                 Nice smooth one-at-a-time salvaging replacement                    *****
		// **********************************************************************************************
		internal static RunStatus GilesOptimisedSalvage(object ret)
		{
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null)
			{
				Logger.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			DiaUnit objBlacksmith = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).FirstOrDefault<DiaUnit>(u => u.IsSalvageShortcut);
			Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
			Vector3 vectorSalvageLocation = new Vector3(0f, 0f, 0f);

			Act curAct = ZetaDia.CurrentAct;
			if (curAct == Act.Invalid || curAct == Act.OpenWorld || curAct == Act.Test) curAct = Character.FindActByLevelID(Bot.Character.Data.iCurrentLevelID);

			//Normal distance we use to move to specific location before moving to NPC
			float _distanceRequired=curAct!=Act.A5?50f:14f; //Act 5 we want short range only!

			if (objBlacksmith == null || objBlacksmith.Distance > _distanceRequired)
			{
				vectorSalvageLocation = ReturnMovementVector(TownRunBehavior.Salvage, curAct);
			}
			else
				vectorSalvageLocation = objBlacksmith.Position;

			if (vectorSalvageLocation == Vector3.Zero)
				Character.FindActByLevelID(Bot.Character.Data.CurrentWorldDynamicID);

			Bot.NavigationCache.RefreshMovementCache();
			//Wait until we are not moving
			if (Bot.NavigationCache.IsMoving)
				return RunStatus.Running;


			float iDistanceFromSell = Vector3.Distance(vectorPlayerPosition, vectorSalvageLocation);
			//Out-Of-Range...
			if (objBlacksmith == null || iDistanceFromSell > 12f)//|| !GilesCanRayCast(vectorPlayerPosition, vectorSalvageLocation, Zeta.Internals.SNO.NavCellFlags.AllowWalk))
			{
				//Use our click movement
				Bot.NavigationCache.RefreshMovementCache();

				//Wait until we are not moving to send click again..
				if (Bot.NavigationCache.IsMoving)
					return RunStatus.Running;

				//if (iDistanceFromSell > 50f)
				Navigator.PlayerMover.MoveTowards(vectorSalvageLocation);
				//else
					//ZetaDia.Me.UsePower(SNOPower.Walk, vectorSalvageLocation);

				return RunStatus.Running;
			}


			if (!UIElements.SalvageWindow.IsVisible)
			{
				objBlacksmith.Interact();
				return RunStatus.Running;
			}

			if (!UIElements.InventoryWindow.IsVisible)
			{
				Bot.Character.Data.BackPack.InventoryBackPackToggle(true);
				return RunStatus.Running;
			}

			if (!TownRunItemLoopsTest(1.15)) return RunStatus.Running;


			if (townRunItemCache.SalvageItems.Count > 0)
			{
				CacheACDItem thisitem = townRunItemCache.SalvageItems.FirstOrDefault();
				if (thisitem != null)
				{
					// Item log for cool stuff stashed
					GilesItemType OriginalGilesItemType = ItemFunc.DetermineItemType(thisitem.ThisInternalName, thisitem.ThisDBItemType, thisitem.ThisFollowerType);
					GilesBaseItemType thisGilesBaseType = ItemFunc.DetermineBaseType(OriginalGilesItemType);
					if (thisGilesBaseType == GilesBaseItemType.WeaponTwoHand || thisGilesBaseType == GilesBaseItemType.WeaponOneHand || thisGilesBaseType == GilesBaseItemType.WeaponRange ||
						 thisGilesBaseType == GilesBaseItemType.Armor || thisGilesBaseType == GilesBaseItemType.Jewelry || thisGilesBaseType == GilesBaseItemType.Offhand ||
						 thisGilesBaseType == GilesBaseItemType.FollowerItem)
					{
						double iThisItemValue = ItemFunc.ValueThisItem(thisitem, OriginalGilesItemType);
						Logger.LogJunkItems(thisitem, thisGilesBaseType, OriginalGilesItemType, iThisItemValue);
					}
					Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.SalvagedItemLog(thisitem);
					ZetaDia.Me.Inventory.SalvageItem(thisitem.ThisDynamicID);

				}
				townRunItemCache.SalvageItems.Remove(thisitem);
				if (townRunItemCache.SalvageItems.Count > 0)
				{
					thisitem = townRunItemCache.SalvageItems.FirstOrDefault();
					if (thisitem != null)
						return RunStatus.Running;
				}
				else
				{
					iCurrentItemLoops = 0;
					return RunStatus.Running;
				}
			}
			bReachedSafety = false;
			bCurrentlyMoving = false;
			return RunStatus.Success;
		}

		// **********************************************************************************************
		// *****         Post salvage cleans up and signs off junk log file after salvaging         *****
		// **********************************************************************************************

		internal static RunStatus GilesOptimisedPostSalvage(object ret)
		{
			Logger.DBLog.DebugFormat("GSDebug: Salvage routine ending sequence...");
			if (bLoggedJunkThisStash)
			{
				FileStream LogStream;
				try
				{
					string sLogFileName = Logger.LoggingPrefixString + " -- JunkLog.log";
					LogStream = File.Open(FolderPaths.LoggingFolderPath + sLogFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
					using (StreamWriter LogWriter = new StreamWriter(LogStream))
						LogWriter.WriteLine("");
					//LogStream.Close();
				}
				catch (IOException)
				{
					Logger.DBLog.DebugFormat("Fatal Error: File access error for signing off the junk log file.");
				}
				bLoggedJunkThisStash = false;
			}


			if (!bReachedSafety && ZetaDia.CurrentAct == Act.A3)
			{
				Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
				Vector3 vectorSafeLocation = new Vector3(379.6096f, 415.6198f, 0.3321424f);
				float iDistanceFromSafety = Vector3.Distance(vectorPlayerPosition, vectorSafeLocation);
				if (bCurrentlyMoving)
				{
					if (iDistanceFromSafety <= 8f)
					{
						bCurrentlyMoving = false;
					}
					else if (iLastDistance == iDistanceFromSafety)
					{
						ZetaDia.Me.UsePower(SNOPower.Walk, vectorSafeLocation, ZetaDia.Me.WorldDynamicId);
					}
					return RunStatus.Running;
				}
				iLastDistance = iDistanceFromSafety;

				if (iDistanceFromSafety > 120f)
					return RunStatus.Failure;
				if (iDistanceFromSafety > 8f)
				{
					ZetaDia.Me.UsePower(SNOPower.Walk, vectorSafeLocation, ZetaDia.Me.WorldDynamicId);
					bCurrentlyMoving = true;
					return RunStatus.Running;
				}
				bCurrentlyMoving = false;
				bReachedSafety = true;
			}
			bFailedToLootLastItem = false;
			iLastDistance = 0f;
			Logger.DBLog.DebugFormat("GSDebug: Salvage routine finished.");
			return RunStatus.Success;
		}
	}

}