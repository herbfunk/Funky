using System;
using Zeta;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.Navigation;
using Zeta.Internals.Actors;
using Zeta.CommonBot;
using System.Linq;
using System.IO;

using FunkyTrinity.Cache;
using FunkyTrinity.Cache.Enums;


namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal static partial class TownRunManager
		  {
				// **********************************************************************************************
				// *****         Salvage Overlord determines if we should visit the blacksmith or not       *****
				// **********************************************************************************************
				internal static bool GilesSalvageOverlord(object ret)
				{
					 Bot.Character.BackPack.townRunCache.hashGilesCachedSalvageItems.Clear();


					 bool bShouldVisitSalvage=false;

					 //Get new list of current backpack
					 Bot.Character.BackPack.Update();

					 //Refresh item manager if we are not using item rules nor giles scoring.
					 if (!Bot.SettingsFunky.ItemRules.UseItemRules&&!Bot.SettingsFunky.ItemRules.ItemRuleGilesScoring)
						  ItemManager.Current.Refresh();

					 foreach (var thisitem in Bot.Character.BackPack.CacheItemList.Values)
					 {
						  if (thisitem.ACDItem.BaseAddress!=IntPtr.Zero)
						  {
								// Find out if this item's in a protected bag slot
								if (!ItemManager.Current.ItemIsProtected(thisitem.ACDItem))
								{

									 if (Bot.SettingsFunky.ItemRules.ItemRulesSalvaging)
									 {
										  if (ItemRulesEval.checkSalvageItem(thisitem.ACDItem)==Interpreter.InterpreterAction.SALVAGE)
										  {
												Bot.Character.BackPack.townRunCache.hashGilesCachedSalvageItems.Add(thisitem);
												continue;
										  }
									 }



									 //Log("GilesTrinityScoring == "+Bot.SettingsFunky.ItemRules.ItemRuleGilesScoring.ToString());

									 bShouldVisitSalvage=ItemManager.Current.ShouldStashItem(thisitem.ACDItem);

									 if (bShouldVisitSalvage)
										  Bot.Character.BackPack.townRunCache.hashGilesCachedSalvageItems.Add(thisitem);

								}
						  }
						  else
						  {
								Log("GSError: Diablo 3 memory read error, or item became invalid [StashOver-1]", true);
						  }
					 }

					 if (Bot.Character.BackPack.townRunCache.hashGilesCachedSalvageItems.Count>0)
					 {
						  Bot.Character.BackPack.townRunCache.sortSalvagelist();
						  return true;
					 }

					 return false;
				}

				// **********************************************************************************************
				// *****             Pre Salvage sets everything up ready for our blacksmith run            *****
				// **********************************************************************************************

				internal static RunStatus GilesOptimisedPreSalvage(object ret)
				{
					 if (Bot.SettingsFunky.Debug.DebugStatusBar)
						  BotMain.StatusText="Town run: Salvage routine started";
					 Log("GSDebug: Salvage routine started.", true);

					 if (ZetaDia.Actors.Me==null)
					 {
						  Log("GSError: Diablo 3 memory read error, or item became invalid [PreSalvage-1]", true);
						  return RunStatus.Failure;
					 }

					 TownRunManager.bFailedToLootLastItem=false;
					 bLoggedJunkThisStash=false;
					 bCurrentlyMoving=false;
					 iCurrentItemLoops=0;
					 RandomizeTheTimer();
					 return RunStatus.Success;
				}

				// **********************************************************************************************
				// *****                 Nice smooth one-at-a-time salvaging replacement                    *****
				// **********************************************************************************************
				internal static RunStatus GilesOptimisedSalvage(object ret)
				{
					 DiaUnit objBlacksmith=ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).FirstOrDefault<DiaUnit>(u => u.IsSalvageShortcut);
					 Vector3 vectorPlayerPosition=ZetaDia.Me.Position;
					 Vector3 vectorSalvageLocation=new Vector3(0f, 0f, 0f);

					 if (objBlacksmith==null||objBlacksmith.Distance>20f)
					 {
						  switch (ZetaDia.CurrentAct)
						  {
								case Act.A1:
									 vectorSalvageLocation=new Vector3(2949.626f, 2815.065f, 24.04389f); break;
								case Act.A2:
									 vectorSalvageLocation=new Vector3(289.6358f, 232.1146f, 0.1f); break;
								case Act.A3:
								case Act.A4:
									 vectorSalvageLocation=new Vector3(379.6096f, 415.6198f, 0.3321424f); break;
						  }
					 }
					 else
						  vectorSalvageLocation=objBlacksmith.Position;


					 Bot.NavigationCache.RefreshMovementCache();
					 //Wait until we are not moving
					 if (Bot.NavigationCache.IsMoving)
						  return RunStatus.Running;


					 float iDistanceFromSell=Vector3.Distance(vectorPlayerPosition, vectorSalvageLocation);
					 //Out-Of-Range...
					 if (objBlacksmith==null||iDistanceFromSell>12f)//|| !GilesCanRayCast(vectorPlayerPosition, vectorSalvageLocation, Zeta.Internals.SNO.NavCellFlags.AllowWalk))
					 {
						  //Use our click movement
						  Bot.NavigationCache.RefreshMovementCache();

						  //Wait until we are not moving to send click again..
						  if (Bot.NavigationCache.IsMoving)
								return RunStatus.Running;

						  Navigator.PlayerMover.MoveTowards(vectorSalvageLocation);
						  return RunStatus.Running;
					 }


					 if (objBlacksmith==null)
						  return RunStatus.Failure;

					 if (!Zeta.Internals.UIElements.SalvageWindow.IsVisible)
					 {
						  objBlacksmith.Interact();
						  return RunStatus.Running;
					 }

					 if (!Zeta.Internals.UIElements.InventoryWindow.IsVisible)
					 {
						  Bot.Character.BackPack.InventoryBackPackToggle(true);
						  return RunStatus.Running;
					 }


					 iCurrentItemLoops++;
					 if (iCurrentItemLoops<iItemDelayLoopLimit*1.15)
						  return RunStatus.Running;

					 iCurrentItemLoops=0;
					 RandomizeTheTimer();

					 if (Bot.Character.BackPack.townRunCache.hashGilesCachedSalvageItems.Count>0)
					 {
						  CacheACDItem thisitem=Bot.Character.BackPack.townRunCache.hashGilesCachedSalvageItems.FirstOrDefault();
						  if (thisitem!=null)
						  {
								// Item log for cool stuff stashed
								GilesItemType OriginalGilesItemType=DetermineItemType(thisitem.ThisInternalName, thisitem.ThisDBItemType, thisitem.ThisFollowerType);
								GilesBaseItemType thisGilesBaseType=DetermineBaseType(OriginalGilesItemType);
								if (thisGilesBaseType==GilesBaseItemType.WeaponTwoHand||thisGilesBaseType==GilesBaseItemType.WeaponOneHand||thisGilesBaseType==GilesBaseItemType.WeaponRange||
									 thisGilesBaseType==GilesBaseItemType.Armor||thisGilesBaseType==GilesBaseItemType.Jewelry||thisGilesBaseType==GilesBaseItemType.Offhand||
									 thisGilesBaseType==GilesBaseItemType.FollowerItem)
								{
									 double iThisItemValue=ValueThisItem(thisitem, OriginalGilesItemType);
									 LogJunkItems(thisitem, thisGilesBaseType, OriginalGilesItemType, iThisItemValue);
								}
								ZetaDia.Me.Inventory.SalvageItem(thisitem.ThisDynamicID);

						  }
						  Bot.Character.BackPack.townRunCache.hashGilesCachedSalvageItems.Remove(thisitem);
						  if (Bot.Character.BackPack.townRunCache.hashGilesCachedSalvageItems.Count>0)
						  {
								thisitem=Bot.Character.BackPack.townRunCache.hashGilesCachedSalvageItems.FirstOrDefault();
								if (thisitem!=null)
									 return RunStatus.Running;
						  }
						  else
						  {
								iCurrentItemLoops=0;
								return RunStatus.Running;
						  }
					 }
					 bReachedSafety=false;
					 bCurrentlyMoving=false;
					 return RunStatus.Success;
				}

				// **********************************************************************************************
				// *****         Post salvage cleans up and signs off junk log file after salvaging         *****
				// **********************************************************************************************

				internal static RunStatus GilesOptimisedPostSalvage(object ret)
				{
					 Log("GSDebug: Salvage routine ending sequence...", true);
					 if (bLoggedJunkThisStash)
					 {
						  FileStream LogStream=null;
						  try
						  {
								string sLogFileName=LoggingPrefixString+" -- JunkLog.log";
								LogStream=File.Open(LoggingFolderPath+sLogFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
								using (StreamWriter LogWriter=new StreamWriter(LogStream))
									 LogWriter.WriteLine("");
								//LogStream.Close();
						  } catch (IOException)
						  {
								Log("Fatal Error: File access error for signing off the junk log file.");
						  }
						  bLoggedJunkThisStash=false;
					 }


					 if (!bReachedSafety&&ZetaDia.CurrentAct==Act.A3)
					 {
						  Vector3 vectorPlayerPosition=ZetaDia.Me.Position;
						  Vector3 vectorSafeLocation=new Vector3(379.6096f, 415.6198f, 0.3321424f);
						  float iDistanceFromSafety=Vector3.Distance(vectorPlayerPosition, vectorSafeLocation);
						  if (bCurrentlyMoving)
						  {
								if (iDistanceFromSafety<=8f)
								{
									 bCurrentlyMoving=false;
								}
								else if (iLastDistance==iDistanceFromSafety)
								{
									 ZetaDia.Me.UsePower(SNOPower.Walk, vectorSafeLocation, ZetaDia.Me.WorldDynamicId);
								}
								return RunStatus.Running;
						  }
						  iLastDistance=iDistanceFromSafety;

						  if (iDistanceFromSafety>120f)
								return RunStatus.Failure;
						  if (iDistanceFromSafety>8f)
						  {
								ZetaDia.Me.UsePower(SNOPower.Walk, vectorSafeLocation, ZetaDia.Me.WorldDynamicId);
								bCurrentlyMoving=true;
								return RunStatus.Running;
						  }
						  bCurrentlyMoving=false;
						  bReachedSafety=true;
					 }

					 iLastDistance=0f;
					 Log("GSDebug: Salvage routine finished.", true);
					 return RunStatus.Success;
				}
		  }
	 }
}