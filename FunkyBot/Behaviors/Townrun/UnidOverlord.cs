using System;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using Zeta;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.Navigation;
using Zeta.Internals.Actors;
using Zeta.CommonBot;
using System.Linq;
using System.IO;
using Zeta.Internals.SNO;
using Zeta.Internals;
using Zeta.Internals.Actors.Gizmos;
using FunkyBot.Game;

namespace FunkyBot
{
	 public partial class Funky
	 {
		  internal static partial class TownRunManager
		  {
				// **********************************************************************************************
				// *****         TownRunCheckOverlord - determine if we should do a town-run or not         *****
				// **********************************************************************************************
				internal static bool UnidItemOverlord(object ret)
				{
					 if (bCheckUnidItems)
					 {
						  Bot.Character.BackPack.townRunCache.hashGilesCachedUnidStashItems.Clear();
						  Bot.Character.BackPack.Update();
						  bCheckUnidItems=false;
						  foreach (var thisitem in Bot.Character.BackPack.CacheItemList.Values.Where(i => i.IsUnidentified))
						  {
								if (thisitem.ACDItem.BaseAddress!=IntPtr.Zero)
								{
									 // Find out if this item's in a protected bag slot
									 if (!ItemManager.Current.ItemIsProtected(thisitem.ACDItem))
									 {
										  if (Bot.Settings.ItemRules.ItemRulesUnidStashing)
										  {
												if (Bot.ItemRulesEval.checkUnidStashItem(thisitem.ACDItem)==Interpreter.InterpreterAction.KEEP)
												{
													 Bot.Character.BackPack.townRunCache.hashGilesCachedUnidStashItems.Add(thisitem);
													 continue;
												}
										  }
									 }
								}
								else
								{
									 Log("GSError: Diablo 3 memory read error, or item became invalid [StashOver-1]", true);
								}
						  }
					 }

					 return (Bot.Character.BackPack.townRunCache.hashGilesCachedUnidStashItems.Count>0);
				}

				internal static RunStatus UnidStashBehavior(object ret)
				{
					 if (ZetaDia.Actors.Me==null)
					 {
						  Log("GSError: Diablo 3 memory read error, or item became invalid [CoreStash-1]", true);
						  return RunStatus.Failure;
					 }
					 Vector3 vectorPlayerPosition=ZetaDia.Me.Position;
					 Vector3 vectorStashLocation=new Vector3(0f, 0f, 0f);
					 DiaObject objPlayStash=ZetaDia.Actors.GetActorsOfType<GizmoPlayerSharedStash>(true).FirstOrDefault<GizmoPlayerSharedStash>();
					 if (objPlayStash!=null)
						  vectorStashLocation=objPlayStash.Position;
					 else if (!ZetaDia.Me.IsInTown)
						  return RunStatus.Failure;
					 else
					 {
						  //Setup vector for movement
						  switch (ZetaDia.CurrentAct)
						  {
								case Act.A1:
									 vectorStashLocation=new Vector3(2971.285f, 2798.801f, 24.04533f); break;
								case Act.A2:
									 vectorStashLocation=new Vector3(323.4543f, 228.5806f, 0.1f); break;
								case Act.A3:
								case Act.A4:
									 vectorStashLocation=new Vector3(389.3798f, 390.7143f, 0.3321428f); break;
						  }
					 }

					 float iDistanceFromStash=Vector3.Distance(vectorPlayerPosition, vectorStashLocation);
					 if (iDistanceFromStash>120f)
						  return RunStatus.Failure;

					 //Out-Of-Range...
					 if (objPlayStash==null)
					 {

						  Navigator.PlayerMover.MoveTowards(vectorStashLocation);
						  return RunStatus.Running;
					 }
					 else
					 {
						  if (iDistanceFromStash>40f)
						  {
								ZetaDia.Me.UsePower(SNOPower.Walk, vectorStashLocation, ZetaDia.Me.WorldDynamicId);
								return RunStatus.Running;
						  }
						  else if (iDistanceFromStash>7.5f&&!UIElements.StashWindow.IsVisible)
						  {
								//Use our click movement
								Bot.NavigationCache.RefreshMovementCache();

								//Wait until we are not moving to send click again..
								if (Bot.NavigationCache.IsMoving) return RunStatus.Running;

								ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, vectorStashLocation, ZetaDia.Me.WorldDynamicId, objPlayStash.ACDGuid);
								return RunStatus.Running;
						  }
					 }

					 if (objPlayStash==null)
						  return RunStatus.Failure;
					 if (!UIElements.StashWindow.IsVisible)
					 {
						  objPlayStash.Interact();
						  return RunStatus.Running;
					 }

					 if (!bUpdatedStashMap)
					 {
						  // Array for what blocks are or are not blocked
						  for (int iRow=0; iRow<=29; iRow++)
								for (int iColumn=0; iColumn<=6; iColumn++)
									 GilesStashSlotBlocked[iColumn, iRow]=false;
						  // Block off the entire of any "protected stash pages"
						  foreach (int iProtPage in Zeta.CommonBot.Settings.CharacterSettings.Instance.ProtectedStashPages)
								for (int iProtRow=0; iProtRow<=9; iProtRow++)
									 for (int iProtColumn=0; iProtColumn<=6; iProtColumn++)
										  GilesStashSlotBlocked[iProtColumn, iProtRow+(iProtPage*10)]=true;
						  // Remove rows we don't have
						  for (int iRow=(ZetaDia.Me.NumSharedStashSlots/7); iRow<=29; iRow++)
								for (int iColumn=0; iColumn<=6; iColumn++)
									 GilesStashSlotBlocked[iColumn, iRow]=true;
						  // Map out all the items already in the stash
						  foreach (ACDItem tempitem in ZetaDia.Me.Inventory.StashItems)
						  {
								if (tempitem.BaseAddress!=IntPtr.Zero)
								{
									 int inventoryRow=tempitem.InventoryRow;
									 int inventoryColumn=tempitem.InventoryColumn;
									 // Mark this slot as not-free
									 GilesStashSlotBlocked[inventoryColumn, inventoryRow]=true;
									 // Try and reliably find out if this is a two slot item or not
									 GilesItemType tempItemType=DetermineItemType(tempitem.InternalName, tempitem.ItemType, tempitem.FollowerSpecialType);
									 if (DetermineIsTwoSlot(tempItemType)&&inventoryRow!=19&&inventoryRow!=9&&inventoryRow!=29)
									 {
										  GilesStashSlotBlocked[inventoryColumn, inventoryRow+1]=true;
									 }
									 else if (DetermineIsTwoSlot(tempItemType)&&(inventoryRow==19||inventoryRow==9||inventoryRow==29))
									 {
										  Log("GSError: DemonBuddy thinks this item is 2 slot even though it's at bottom row of a stash page: "+tempitem.Name+" ["+tempitem.InternalName+
												"] type="+tempItemType.ToString()+" @ slot "+(inventoryRow+1).ToString()+"/"+
												(inventoryColumn+1).ToString(), true);
									 }
								}
						  } // Loop through all stash items
						  bUpdatedStashMap=true;
					 } // Need to update the stash map?


					 if (Bot.Character.BackPack.townRunCache.hashGilesCachedUnidStashItems.Count>0)
					 {
						  iCurrentItemLoops++;
						  if (iCurrentItemLoops<iItemDelayLoopLimit)
								return RunStatus.Running;
						  iCurrentItemLoops=0;
						  RandomizeTheTimer();
						  CacheACDItem thisitem=Bot.Character.BackPack.townRunCache.hashGilesCachedUnidStashItems.FirstOrDefault();
						  if (LastStashPoint[0]<0&&LastStashPoint[1]<0&&LastStashPage<0)
						  {
								bool bDidStashSucceed=GilesStashAttempt(thisitem, out LastStashPoint, out LastStashPage);
								if (!bDidStashSucceed)
								{
									 Log("There was an unknown error stashing an item.", true);
									 if (MuleBehavior)
										  return RunStatus.Success;
								}
								else
									 return RunStatus.Running;
						  }
						  else
						  {
								//We have a valid place to stash.. so lets check if stash page is currently open
								if (ZetaDia.Me.Inventory.CurrentStashPage==LastStashPage)
								{
									 //Herbfunk: Current Game Stats
                                     TotalStats.CurrentTrackingProfile.LootTracker.StashedItemLog(thisitem);

									 ZetaDia.Me.Inventory.MoveItem(thisitem.ThisDynamicID, ZetaDia.Me.CommonData.DynamicId, InventorySlot.PlayerSharedStash, LastStashPoint[0], LastStashPoint[1]);
									 LastStashPoint=new int[] { -1, -1 };
									 LastStashPage=-1;

									 if (thisitem!=null)
										  Bot.Character.BackPack.townRunCache.hashGilesCachedUnidStashItems.Remove(thisitem);
									 if (Bot.Character.BackPack.townRunCache.hashGilesCachedUnidStashItems.Count>0)
										  return RunStatus.Running;
								}
								else
								{
									 //Lets switch the current page..
									 ZetaDia.Me.Inventory.SwitchStashPage(LastStashPage);
									 return RunStatus.Running;
								}
						  }
					 }
					 return RunStatus.Success;
				}

				internal static RunStatus UnidStashOptimisedPreStash(object ret)
				{
					 if (Bot.Settings.Debug.DebugStatusBar)
						  BotMain.StatusText="Town run: Unid Item Stash routine started";
					 Log("GSDebug: Unid Stash routine started.", true);
					 bLoggedAnythingThisStash=false;
					 bUpdatedStashMap=false;
					 iCurrentItemLoops=0;
					 RandomizeTheTimer();
					 TownRunManager.bFailedToLootLastItem=false;
					 return RunStatus.Success;
				}

				// **********************************************************************************************
				// *****            Post Stash tidies up and signs off log file after a stash run           *****
				// **********************************************************************************************
				internal static RunStatus UnidStashOptimisedPostStash(object ret)
				{
					 Log("GSDebug: Unid Stash routine ending sequence...", true);
					 // See if there's any legendary items we should send Prowl notifications about
					 while (pushQueue.Count>0) { SendNotification(pushQueue.Dequeue()); }
					 /*
					  if (bLoggedAnythingThisStash)
					  {
							FileStream LogStream = null;
							try
							{
								 LogStream = File.Open(sTrinityPluginPath + ZetaDia.Service.CurrentHero.BattleTagName + " - StashLog - " + ZetaDia.Actors.Me.ActorClass.ToString() + ".log", FileMode.Append, FileAccess.Write, FileShare.Read);
								 using (StreamWriter LogWriter = new StreamWriter(LogStream))
									  LogWriter.WriteLine("");
								 LogStream.Close();
							}
							catch (IOException)
							{
								 Log("Fatal Error: File access error for signing off the stash log file.");
								 if (LogStream != null)
									  LogStream.Close();
							}
							bLoggedAnythingThisStash = false;
					  }
						*/
					 Bot.Character.lastPreformedNonCombatAction=DateTime.Now;


					 Log("GSDebug: Unid Stash routine finished.", true);
					 return RunStatus.Success;
				}
		  }
	 }
}