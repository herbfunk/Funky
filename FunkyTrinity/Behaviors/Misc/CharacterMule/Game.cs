﻿using System;
using System.Linq;
using Zeta;
using Zeta.TreeSharp;
using Zeta.CommonBot.Profile;
using Zeta.CommonBot;
using Zeta.Internals.Actors.Gizmos;
using Zeta.Common;
using Zeta.Internals.Actors;
using System.Collections.Generic;

namespace FunkyBot
{
	 public partial class Funky
	 {
		  public static class NewMuleGame
		  {
				private static DateTime LastActionTaken=DateTime.Today;
				private static int RandomWaitTimeMilliseconds=1000;

				private static void RandomizeWaitTime(bool longwait=false)
				{
					 Random R=new Random(DateTime.Now.Millisecond);
					 if (!longwait)
						  RandomWaitTimeMilliseconds=R.Next(1000, 2250);
					 else
						  RandomWaitTimeMilliseconds=R.Next(3050, 5880);
				}

				internal static int BotHeroIndex=-1;
				internal static string LastProfile=null;
				internal static string BotHeroName=null;
				internal static int LastHandicap=0;

				public static RunStatus BeginNewGameProfile()
				{
					 if (DateTime.Now.Subtract(LastActionTaken).TotalMilliseconds>RandomWaitTimeMilliseconds)
					 {
						  string NewGameProfile=FolderPaths.sTrinityPluginPath+@"Behaviors\Misc\CharacterMule\NewGame.xml";
						  if (ProfileManager.CurrentProfile.Path!=NewGameProfile)
						  {
								if (System.IO.File.Exists(NewGameProfile))
								{
									 Logger.Write(LogLevel.OutOfGame, "Loading NewGame profile");
									 ProfileManager.Load(NewGameProfile, true);
									 Zeta.CommonBot.Settings.CharacterSettings.Instance.MonsterPowerLevel=0;
								}
						  }
						  else
								return RunStatus.Success;

						  LastActionTaken=DateTime.Now;

					 }
					 return RunStatus.Running;
				}

				private static GizmoPlayerSharedStash CurrentStashObject=null;
				private static Vector3 StashV3=new Vector3(2971.285f, 2798.801f, 24.04533f);
				private static Queue<ACDItem> SortedStashItems=new Queue<ACDItem>();


				public static RunStatus StashTransfer()
				{
					 if (CurrentStashObject==null)
					 {
						  //Find the stash object
						  ZetaDia.Actors.Update();
						  var objs=ZetaDia.Actors.GetActorsOfType<GizmoPlayerSharedStash>(true, true);
						  if (objs.Any())
						  {
								CurrentStashObject=objs.First();
						  }
						  else
						  {
								Log("Failed to find stash.. Moving To Stash Vector");
								Zeta.Navigation.Navigator.MoveTo(StashV3, "Stash");
						  }
					 }
					 else if (!Zeta.Internals.UIElements.StashWindow.IsVisible)
					 {
						  if (CurrentStashObject.Distance>7.5f)
						  {
								Bot.NavigationCache.RefreshMovementCache();

								if (!Bot.NavigationCache.IsMoving)
								{
									 if (CurrentStashObject.Distance>50f)
										  ZetaDia.Me.UsePower(SNOPower.Walk, CurrentStashObject.Position, ZetaDia.Me.WorldDynamicId);
									 else
									 {
										  //Use our click movement
										  Bot.NavigationCache.RefreshMovementCache();
										  //Wait until we are not moving to send click again..
										  if (Bot.NavigationCache.IsMoving)
												return RunStatus.Running;

										  ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, CurrentStashObject.Position, ZetaDia.Me.WorldDynamicId, CurrentStashObject.ACDGuid);
									 }
								}
						  }
						  else
								CurrentStashObject.Interact();
					 }
					 else
					 {
						  if (SortedStashItems.Count==0)
						  {
								int itemSlotsFilled=0;
								var sortedItems=ZetaDia.Me.Inventory.StashItems.Where(item => !ItemManager.Current.ItemIsProtected(item)).OrderBy(item => !item.IsTwoSquareItem).ThenByDescending(item => item.InventoryRow).ThenByDescending(item => item.InventoryColumn);
								foreach (var item in sortedItems)
								{
									 SortedStashItems.Enqueue(item);
									 itemSlotsFilled+=item.IsTwoSquareItem?2:1;
									 if (itemSlotsFilled>59)
										  break;
								}

						  }
						  else
						  {
								if (ZetaDia.Me.Inventory.NumFreeBackpackSlots>1)
								{
									 if (DateTime.Now.Subtract(LastActionTaken).TotalMilliseconds>RandomWaitTimeMilliseconds)
									 {
										  // ACDItem currentItem=SortedStashItems[0];
										  ZetaDia.Me.Inventory.QuickWithdraw(SortedStashItems.Dequeue());
										  LastActionTaken=DateTime.Now;
										  RandomWaitTime();
									 }
								}
								else
								{
									 CurrentStashObject=null;
									 SortedStashItems.Clear();
									 LastActionTaken=DateTime.Today;
									 TransferedGear=true;
									 Bot.UpdateCurrentAccountDetails();
									 //Delete settings
									 string sFunkyCharacterFolder=System.IO.Path.Combine(FolderPaths.sDemonBuddyPath, "Settings", "FunkyTrinity", Bot.CurrentAccountName);
									 if (System.IO.Directory.Exists(sFunkyCharacterFolder))
									 {
										  string sFunkyCharacterConfigFile=System.IO.Path.Combine(sFunkyCharacterFolder, Bot.CurrentHeroName+".cfg");
										  if (System.IO.File.Exists(sFunkyCharacterConfigFile))
												System.IO.File.Delete(sFunkyCharacterConfigFile);
									 }

									 D3Character.NewCharacterName=null;

									 ZetaDia.Service.Party.LeaveGame(true);
									 return RunStatus.Running;
								}
						  }

					 }


					 return RunStatus.Running;
				}


				public static RunStatus FinishMuleBehavior()
				{
					 if (DateTime.Now.Subtract(LastActionTaken).TotalMilliseconds>RandomWaitTimeMilliseconds)
					 {
						  if (ZetaDia.IsInGame)
						  {
								ZetaDia.Service.Party.LeaveGame(true);
								RandomizeWaitTime(true);
						  }
						  else if (ZetaDia.Service.CurrentHero.Name!=BotHeroName)
						  {
								ZetaDia.Service.GameAccount.SwitchHero(BotHeroIndex);
								RandomizeWaitTime(true);
								BotHeroIndex++;
						  }
						  else if (Zeta.CommonBot.ProfileManager.CurrentProfile.Path!=LastProfile)
						  {
								Zeta.CommonBot.ProfileManager.Load(LastProfile);
								RandomizeWaitTime();
								Zeta.CommonBot.Settings.CharacterSettings.Instance.MonsterPowerLevel=NewMuleGame.LastHandicap;
						  }
						  else
						  {
								return RunStatus.Success;
						  }
						  LastActionTaken=DateTime.Now;

					 }

					 return RunStatus.Running;
				}
		  }
	 }
}