using System;
using Zeta;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.Navigation;
using Zeta.Internals.Actors;
using Zeta.CommonBot;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Zeta.Internals.SNO;

using FunkyTrinity.Cache;
using FunkyTrinity.Cache.Enums;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal static partial class TownRunManager
		  {
				private static bool PotionCheck=false;

				// **********************************************************************************************
				// *****  Sell Overlord - determines if we should visit the vendor for repairs or selling   *****
				// **********************************************************************************************
				internal static bool GilesSellOverlord(object ret)
				{
					 Bot.Character.BackPack.townRunCache.hashGilesCachedSellItems.Clear();




					 //Get new list of current backpack
					 Bot.Character.BackPack.Update();
					 //Setup any extra potions to sell.
					 List<ACDItem> Potions=Bot.Character.BackPack.ReturnCurrentPotions();

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
									 if (thisitem.IsPotion&&thisitem.ACDGUID!=Bot.Character.BackPack.CurrentPotionACDGUID)
									 {
										  Bot.Character.BackPack.townRunCache.hashGilesCachedSellItems.Add(thisitem);
										  continue;
									 }

									 if (Bot.SettingsFunky.ItemRules.ItemRulesSalvaging)
										  if (ItemRulesEval.checkSalvageItem(thisitem.ACDItem)==Interpreter.InterpreterAction.SALVAGE)
												continue;


									 if (Bot.SettingsFunky.ItemRules.UseItemRules)
									 {
										  Interpreter.InterpreterAction action=ItemRulesEval.checkItem(thisitem.ACDItem, Zeta.CommonBot.ItemEvaluationType.Keep);
										  switch (action)
										  {
												case Interpreter.InterpreterAction.TRASH:
													 Bot.Character.BackPack.townRunCache.hashGilesCachedSellItems.Add(thisitem);
													 continue;
										  }
									 }



									 //Log("GilesTrinityScoring == "+Bot.SettingsFunky.ItemRules.ItemRuleGilesScoring.ToString());

									 bool bShouldSellThis=Bot.SettingsFunky.ItemRules.ItemRuleGilesScoring==true?GilesSellValidation(thisitem.ThisInternalName, thisitem.ThisLevel, thisitem.ThisQuality, thisitem.ThisDBItemType, thisitem.ThisFollowerType):ItemManager.Current.ShouldSellItem(thisitem.ACDItem);

									 if (bShouldSellThis)
									 {
										  Bot.Character.BackPack.townRunCache.hashGilesCachedSellItems.Add(thisitem);
									 }

								}
						  }
						  else
						  {
								Log("GSError: Diablo 3 memory read error, or item became invalid [StashOver-1]", true);
						  }
					 }

					 bool bShouldVisitVendor=Bot.Character.BackPack.townRunCache.hashGilesCachedSellItems.Count>0;

					 // Check durability percentages
					 bNeedsEquipmentRepairs=Bot.Character.BackPack.ShouldRepairItems();


					 if (bShouldVisitVendor)
					 {
						  Bot.Character.BackPack.townRunCache.sortSellList();
					 }

					 if (!bShouldVisitVendor) bShouldVisitVendor=bNeedsEquipmentRepairs;

					 return bShouldVisitVendor;
				}

				// **********************************************************************************************
				// *****             Pre Sell sets everything up ready for running to vendor                *****
				// **********************************************************************************************

				internal static RunStatus GilesOptimisedPreSell(object ret)
				{
					 if (Bot.SettingsFunky.Debug.DebugStatusBar)
						  BotMain.StatusText="Town run: Sell routine started";
					 Log("GSDebug: Sell routine started.", true);
					 if (ZetaDia.Actors.Me==null)
					 {
						  Log("GSError: Diablo 3 memory read error, or item became invalid [PreSell-1]", true);
						  return RunStatus.Failure;
					 }
					 bLoggedJunkThisStash=false;
					 bCurrentlyMoving=false;
					 PotionCheck=false;
					 iCurrentItemLoops=0;
					 RandomizeTheTimer();
					 TownRunManager.bFailedToLootLastItem=false;

					 List<ACDItem> potions=Bot.Character.BackPack.ReturnCurrentPotions();
					 Bot.Character.iTotalPotions=potions.Any()?potions.Sum(p => p.ItemStackQuantity):0;
					 return RunStatus.Success;
				}

				// **********************************************************************************************
				// *****    Sell Routine replacement for smooth one-at-a-time item selling and handling     *****
				// **********************************************************************************************

				internal static RunStatus GilesOptimisedSell(object ret)
				{
					 string sVendorName="";
					 switch (ZetaDia.CurrentAct)
					 {
						  case Act.A1:
								sVendorName="a1_uniquevendor_miner"; break;
						  case Act.A2:
								sVendorName="a2_uniquevendor_peddler"; break;
						  case Act.A3:
								sVendorName="a3_uniquevendor_collector"; break;
						  case Act.A4:
								sVendorName="a4_uniquevendor_collector"; break;
					 }

					 #region Navigation
					 DiaUnit objSellNavigation=ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).FirstOrDefault<DiaUnit>(u => u.Name.ToLower().StartsWith(sVendorName));
					 Vector3 vectorPlayerPosition=ZetaDia.Me.Position;
					 Vector3 vectorSellLocation=new Vector3(0f, 0f, 0f);

					 if (objSellNavigation==null)
					 {
						  switch (ZetaDia.CurrentAct)
						  {
								case Act.A1:
									 vectorSellLocation=new Vector3(2912.775f, 2803.896f, 24.04533f); break;
								case Act.A2:
									 vectorSellLocation=new Vector3(295.2101f, 265.1436f, 0.1000002f); break;
								case Act.A3:
								case Act.A4:
									 vectorSellLocation=new Vector3(410.6073f, 355.8762f, 0.1000005f); break;
						  }
					 }
					 else
						  vectorSellLocation=objSellNavigation.Position;


					 float iDistanceFromSell=Vector3.Distance(vectorPlayerPosition, vectorSellLocation);
					 //Out-Of-Range...
					 if (objSellNavigation==null)
						  //!GilesCanRayCast(vectorPlayerPosition, vectorSellLocation, NavCellFlags.AllowWalk))
					 {
						  Logging.WriteVerbose("Vendor Obj is Null or Raycast Failed.. using Navigator to move!");
						  Navigator.PlayerMover.MoveTowards(vectorSellLocation);
						  return RunStatus.Running;
					 }
					 else
					 {
						  if (iDistanceFromSell>40f)
						  {
								ZetaDia.Me.UsePower(SNOPower.Walk, vectorSellLocation, ZetaDia.Me.WorldDynamicId);
								return RunStatus.Running;
						  }
						  else if (iDistanceFromSell>7.5f&&!Zeta.Internals.UIElements.VendorWindow.IsValid)
						  {
								//Use our click movement
								Bot.NavigationCache.RefreshMovementCache();

								//Wait until we are not moving to send click again..
								if (Bot.NavigationCache.IsMoving) 
									 return RunStatus.Running;

								objSellNavigation.Interact();
								//ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, vectorSellLocation, ZetaDia.Me.WorldDynamicId, objSellNavigation.ACDGuid);
								return RunStatus.Running;
						  }
					 }

					 if (objSellNavigation==null)
						  return RunStatus.Failure;

					 if (!Zeta.Internals.UIElements.VendorWindow.IsVisible)
					 {
						  objSellNavigation.Interact();
						  return RunStatus.Running;
					 }

					 if (!Zeta.Internals.UIElements.InventoryWindow.IsVisible)
					 {
						  Bot.Character.BackPack.InventoryBackPackToggle(true);
						  return RunStatus.Running;
					 }
					 #endregion

					 #region SellItem
					 if (Bot.Character.BackPack.townRunCache.hashGilesCachedSellItems.Count>0)
					 {
						  iCurrentItemLoops++;
						  if (iCurrentItemLoops<iItemDelayLoopLimit)
								return RunStatus.Running;
						  iCurrentItemLoops=0;
						  RandomizeTheTimer();

						  CacheACDItem thisitem=Bot.Character.BackPack.townRunCache.hashGilesCachedSellItems.FirstOrDefault();
						  // Item log for cool stuff sold
						  if (thisitem!=null)
						  {
								GilesItemType OriginalGilesItemType=DetermineItemType(thisitem.ThisInternalName, thisitem.ThisDBItemType, thisitem.ThisFollowerType);
								GilesBaseItemType thisGilesBaseType=DetermineBaseType(OriginalGilesItemType);
								if (thisGilesBaseType==GilesBaseItemType.WeaponTwoHand||thisGilesBaseType==GilesBaseItemType.WeaponOneHand||thisGilesBaseType==GilesBaseItemType.WeaponRange||
									 thisGilesBaseType==GilesBaseItemType.Armor||thisGilesBaseType==GilesBaseItemType.Jewelry||thisGilesBaseType==GilesBaseItemType.Offhand||
									 thisGilesBaseType==GilesBaseItemType.FollowerItem)
								{
									 double iThisItemValue=ValueThisItem(thisitem, OriginalGilesItemType);
									 double iNeededValue=ScoreNeeded(OriginalGilesItemType);
									 LogJunkItems(thisitem, thisGilesBaseType, OriginalGilesItemType, iThisItemValue);
								}
								ZetaDia.Me.Inventory.SellItem(thisitem.ACDItem);
						  }
						  if (thisitem!=null)
								Bot.Character.BackPack.townRunCache.hashGilesCachedSellItems.Remove(thisitem);
						  if (Bot.Character.BackPack.townRunCache.hashGilesCachedSellItems.Count>0)
								return RunStatus.Running;
					 }
					 #endregion

					 #region BuyPotion
					 //Check if settings for potion buy is enabled, with less than 99 potions existing!
					 if (Bot.SettingsFunky.BuyPotionsDuringTownRun&&Bot.Character.iTotalPotions<Bot.SettingsFunky.Loot.MaximumHealthPotions&&
						  !PotionCheck)
					 {
						  //Obey the timer, so we don't buy 100 potions in 3 seconds.
						  iCurrentItemLoops++;
						  if (iCurrentItemLoops<iItemDelayLoopLimit)
								return RunStatus.Running;
						  iCurrentItemLoops=0;
						  RandomizeTheTimer();

						  //Buy Potions
						  int BestPotionID=0;
						  int LastHPValue=0;
						  foreach (ACDItem item in ZetaDia.Me.Inventory.MerchantItems)
						  {
								if (item.IsPotion&&item.HitpointsGranted>LastHPValue
									 &&item.RequiredLevel<=Bot.Character.iMyLevel
									 &&item.Gold<ZetaDia.Me.Inventory.Coinage)
								{
									 LastHPValue=item.HitpointsGranted;
									 BestPotionID=item.DynamicId;
								}
						  }
						  if (BestPotionID!=0)
						  {
								ZetaDia.Me.Inventory.BuyItem(BestPotionID);
								//Update counter
								Bot.Character.iTotalPotions++;
								return RunStatus.Running;
						  }
						  else
								PotionCheck=true;
					 }
					 else
						  PotionCheck=true;
					 #endregion

					 if (bNeedsEquipmentRepairs)
					 {
						  iCurrentItemLoops++;
						  if (iCurrentItemLoops<iItemDelayLoopLimit)
								return RunStatus.Running;
						  iCurrentItemLoops=0;
						  RandomizeTheTimer();

						  if (ZetaDia.Me.Inventory.Coinage<40000)
						  {
								Log("Emergency Stop: You need repairs but don't have enough money. Stopping the bot to prevent infinite death loop.");
								BotMain.Stop(false, "Not enough gold to repair item(s)!");
						  }

						  ZetaDia.Me.Inventory.RepairEquippedItems();
						  bNeedsEquipmentRepairs=false;
					 }


					 bCurrentlyMoving=false;
					 bReachedSafety=false;
					 return RunStatus.Success;
				}

				// **********************************************************************************************
				// *****        Post Sell tidies everything up and signs off junk log after selling         *****
				// **********************************************************************************************



				internal static RunStatus GilesOptimisedPostSell(object ret)
				{
					 iCurrentItemLoops++;
					 if (iCurrentItemLoops<iItemDelayLoopLimit)
						  return RunStatus.Running;

					 Log("GSDebug: Sell routine ending sequence...", true);


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

					 // See if we can close the inventory window
					 if (Zeta.Internals.UIElement.IsValidElement(0x368FF8C552241695))
					 {
						  try
						  {
								var el=Zeta.Internals.UIElement.FromHash(0x368FF8C552241695);
								if (el!=null&&el.IsValid&&el.IsVisible&&el.IsEnabled)
									 el.Click();
						  } catch
						  {
								// Do nothing if it fails, just catching to prevent any big errors/plugin crashes from this
						  }
					 }

					 iLastDistance=0f;
					 iCurrentItemLoops=0;
					 RandomizeTheTimer();
					 Log("GSDebug: Sell routine finished.", true);
					 Bot.Character.lastPreformedNonCombatAction=DateTime.Now;
					 return RunStatus.Success;
				}

		  }
	 }
}