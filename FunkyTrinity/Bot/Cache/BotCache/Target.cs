﻿using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.TreeSharp;
using System.Collections.Generic;
using Zeta.Internals.SNO;
using Zeta.CommonBot;
using System.Globalization;

namespace FunkyTrinity
{
	 public partial class Funky
	 {

		  internal class TargetHandler
		  {
				//Constructor
				public TargetHandler()
				{
					 CurrentState=RunStatus.Running;
					 CurrentTarget=null;
				}


				///<summary>
				///This method handles the current object target.
				///</summary>
				public RunStatus HandleThis()
				{

					 //Prechecks
					 bool Continue=PreChecks();


					 //Refresh
					 if (!Continue)
						  return CurrentState;
					 else
						  Continue=Refresh();


					 //Combat logic
					 if (!Continue)
						  return CurrentState;
					 else
						  Continue=CombatLogic();


					 //Movement
					 if (!Continue)
						  return CurrentState;
					 else
						  Continue=Movement();


					 //Interaction
					 if (!Continue)
						  return CurrentState;
					 else
						  Continue=ObjectInteraction();


					 //Return status
					 return CurrentState;
				}

				//The current state which is used to return from the handler
				public RunStatus CurrentState { get; set; }

				//This is the object that is our target which is the data used in methods.
				//This data is continiously updated by RefreshDiaObjects.
				///<summary>
				///This should reference Cached Data.
				///Used throughout the code as the Bot.CurrentTarget data.
				///This must be set to a valid cacheobject in order to properly handle it.
				///</summary>
				public CacheObject CurrentTarget;

				///<summary>
				///Update our current object data ("Current Target")
				///</summary>
				public bool UpdateTarget()
				{
					 //Generate a vaild object list using our cached collection!
					 Bot.ValidObjects=ObjectCache.Objects.Values
														  .Where(obj => obj.ObjectIsValidForTargeting).ToList();

					 //Check avoidance requirement still valid
					 if (Bot.Combat.RequiresAvoidance&&Bot.Combat.TriggeringAvoidances.Count==0)
						  Bot.Combat.RequiresAvoidance=false;

					 Vector3 LOS=vNullLocation;

					 //Check if we require avoidance movement.
					 #region AvodianceMovementCheck
					 // Note that if treasure goblin level is set to kamikaze, even avoidance moves are disabled to reach the goblin!
					 if (Bot.Combat.RequiresAvoidance&&(!Bot.Combat.bAnyTreasureGoblinsPresent||Bot.SettingsFunky.GoblinPriority<2)&&
						  DateTime.Now.Subtract(Bot.Combat.timeCancelledEmergencyMove).TotalMilliseconds>=Bot.Combat.iMillisecondsCancelledEmergencyMoveFor)
					 {//Bot requires avoidance movement.. (Note: we have not done the weighting of our targeting list yet..)
						  Vector3 vAnySafePoint;

						  bool SafeMovementFound=false;

						  if (CurrentTarget!=null&&CurrentTarget.targetType.HasValue&&TargetType.ServerObjects.HasFlag(CurrentTarget.targetType.Value))
								LOS=CurrentTarget.Position;
						  else
								LOS=vNullLocation;


						  SafeMovementFound=GridPointAreaCache.AttemptFindSafeSpot(out vAnySafePoint, LOS);
						  if (SafeMovementFound)
						  {
								float distance=vAnySafePoint.Distance(Bot.Character.Position);

								float losdistance=0f;
								if (LOS!=vNullLocation)
									 losdistance=LOS.Distance(Bot.Character.Position);

								string los=LOS!=vNullLocation?("\r\n using LOS location "+LOS.ToString()+" distance "+losdistance.ToString()):" ";

								Logging.WriteDiagnostic("Safespot found at {0} with {1} distance{2}", vAnySafePoint.ToString(), distance, los);
								//bFoundSafeSpot = true;

								//setup avoidance target
								if (CurrentTarget!=null)
									 Bot.Combat.LastCachedTarget=CurrentTarget.Clone();

								CurrentTarget=new CacheObject(vAnySafePoint, TargetType.Avoidance, 20000f, "SafeAvoid", 2.5f, -1);

								//Set timer here until next we try... since we've already attempted at least 9 GPCs!
								Bot.Combat.iMillisecondsCancelledEmergencyMoveFor=(int)(Bot.Character.dCurrentHealthPct*Bot.SettingsFunky.AvoidanceRecheckMaximumRate);
								Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
								//Bot.Combat.LastAvoidanceMovement=DateTime.Now;
								return true;
						  }

						  Bot.UpdateAvoidKiteRates();
					 }
					 #endregion

					 Bot.Combat.bStayPutDuringAvoidance=false;

					 //update cluster targeting valid selection list
					 if (Bot.SettingsFunky.EnableClusteringTargetLogic
							 &&(!Bot.SettingsFunky.IgnoreClusteringWhenLowHP||Bot.Character.dCurrentHealthPct>Bot.SettingsFunky.IgnoreClusterLowHPValue)
							 &&(DateTime.Now.Subtract(Bot.Combat.dateSincePickedTarget).TotalMilliseconds>500||DateTime.Now.Subtract(Bot.Combat.LastClusterTargetLogicRefresh).TotalMilliseconds>200))
					 {
						  Bot.Combat.UpdateTargetClusteringVariables();
					 }


					 //Standard weighting of valid objects -- updates current target.
					 this.WeightEvaluationObjList();

					 //check kiting
					 #region Kiting
					 if (Bot.KiteDistance>0
						  &&(!Bot.Combat.bAnyTreasureGoblinsPresent||Bot.SettingsFunky.GoblinPriority<2)
						  &&DateTime.Now.Subtract(Bot.Combat.timeCancelledKiteMove).TotalMilliseconds>=Bot.Combat.iMillisecondsCancelledKiteMoveFor
						  &&(Bot.Class.AC!=ActorClass.Wizard||(Bot.Class.AC==ActorClass.Wizard&&(!Bot.SettingsFunky.Class.bKiteOnlyArchon||HasBuff(SNOPower.Wizard_Archon)))))
					 {

						  ////Find any units that we should kite, sorted by distance.
						  //var nearbyUnits=Bot.ValidObjects.OfType<CacheUnit>().Where(unit => unit.ShouldBeKited
						  //                                                                 &&unit.RadiusDistance<Bot.KiteDistance);

						  if (Bot.Combat.NearbyKitingUnits.Count>0)
						  {
								if (CurrentTarget!=null&&CurrentTarget.targetType.HasValue&&TargetType.ServerObjects.HasFlag(CurrentTarget.targetType.Value))
									 LOS=CurrentTarget.Position;
								else
									 LOS=vNullLocation;

								Vector3 vAnySafePoint;
								if (GridPointAreaCache.AttemptFindSafeSpot(out vAnySafePoint, LOS, true))
								{
									 Logging.WriteDiagnostic("Kitespot found at {0} with {1} distance from our current position!", vAnySafePoint.ToString(), vAnySafePoint.Distance(Bot.Character.Position));
									 Bot.Combat.IsKiting=true;

									 if (CurrentTarget!=null)
										  Bot.Character.LastCachedTarget=CurrentTarget;

									 CurrentTarget=new CacheObject(vAnySafePoint, TargetType.Avoidance, 20000f, "Kitespot", 2.5f, -1);
									 Bot.Combat.iMillisecondsCancelledKiteMoveFor=(int)(Bot.Character.dCurrentHealthPct*Bot.SettingsFunky.KitingRecheckMaximumRate);
									 Bot.Combat.timeCancelledKiteMove=DateTime.Now;
									 return true;
								}
								Bot.UpdateAvoidKiteRates();
						  }
					 }

					 //If we have a cached kite target.. and no current target, lets swap back!
					 if (Bot.Combat.KitedLastTarget&&CurrentTarget==null
								  &&Bot.Character.LastCachedTarget!=null
								  &&ObjectCache.Objects.ContainsKey(Bot.Character.LastCachedTarget.RAGUID))
					 {
						  //Swap back to our orginal "kite" target
						  CurrentTarget=ObjectCache.Objects[Bot.Character.LastCachedTarget.RAGUID];
						  Logging.WriteVerbose("Swapping back to unit {0} after kiting!", CurrentTarget.InternalName);
						  return true;
					 }
					 #endregion

					 // No valid targets but we were told to stay put?
					 if (CurrentTarget==null&&Bot.Combat.bStayPutDuringAvoidance)
					 {
						  if (Bot.Combat.TriggeringAvoidances.Count==0)
						  {
								CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "StayPutPoint", 2.5f, -1);
								return true;
						  }
						  else
								Bot.Combat.iMillisecondsCancelledEmergencyMoveFor=0; //reset wait time
					 }

					 //Final Possible Target Check
					 if (CurrentTarget==null)
					 {
						  // See if we should wait for milliseconds for possible loot drops before continuing run
						  if (DateTime.Now.Subtract(Bot.Combat.lastHadUnitInSights).TotalMilliseconds<=Bot.SettingsFunky.AfterCombatDelay&&DateTime.Now.Subtract(Bot.Combat.lastHadEliteUnitInSights).TotalMilliseconds<=10000||
								//Cut the delay time in half for non-elite monsters!
								DateTime.Now.Subtract(Bot.Combat.lastHadUnitInSights).TotalMilliseconds<=Bot.SettingsFunky.AfterCombatDelay)
						  {
								CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "WaitForLootDrops", 2f, -1);
								return true;
						  }
						  //Herbfunks wait after loot containers are opened. 3s for rare chests, half the settings delay for everything else.
						  if ((DateTime.Now.Subtract(Bot.Combat.lastHadRareChestAsTarget).TotalMilliseconds<=3750)||
								(DateTime.Now.Subtract(Bot.Combat.lastHadContainerAsTarget).TotalMilliseconds<=(Bot.SettingsFunky.AfterCombatDelay*1.25)))
						  {
								CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "ContainerLootDropsWait", 2f, -1);
								return true;
						  }

						  // Finally, a special check for waiting for wrath of the berserker cooldown before engaging Azmodan
						  if (HotbarAbilitiesContainsPower(SNOPower.Barbarian_WrathOfTheBerserker)&&Bot.SettingsFunky.Class.bWaitForWrath&&!AbilityUseTimer(SNOPower.Barbarian_WrathOfTheBerserker)&&
								ZetaDia.CurrentWorldId==121214&&
								(Vector3.Distance(Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for Wrath Of The Berserker cooldown before continuing to Azmodan.");
								CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "GilesWaitForWrath", 0f, -1);
								return true;
						  }
						  // And a special check for wizard archon
						  if (HotbarAbilitiesContainsPower(SNOPower.Wizard_Archon)&&!AbilityUseTimer(SNOPower.Wizard_Archon)&&Bot.SettingsFunky.Class.bWaitForArchon&&ZetaDia.CurrentWorldId==121214&&
								(Vector3.Distance(Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for Wizard Archon cooldown before continuing to Azmodan.");
								CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "GilesWaitForArchon", 0f, -1);
								return true;
						  }
						  // And a very sexy special check for WD BigBadVoodoo
						  if (HotbarAbilitiesContainsPower(SNOPower.Witchdoctor_BigBadVoodoo)&&!PowerManager.CanCast(SNOPower.Witchdoctor_BigBadVoodoo)&&ZetaDia.CurrentWorldId==121214&&
								(Vector3.Distance(Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for WD BigBadVoodoo cooldown before continuing to Azmodan.");
								CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "GilesWaitForVoodooo", 0f, -1);
								return true;
						  }


						  //Check if our current path intersects avoidances. (When not in town, and not currently inside avoidance)
						  if (!Bot.Character.bIsInTown&&(Bot.SettingsFunky.AttemptAvoidanceMovements||Bot.Combat.CriticalAvoidance)
								  &&NP.CurrentPath.Count>0
								  &&Bot.Combat.TriggeringAvoidances.Count==0)
						  {
								Vector3 curpos=Bot.Character.Position;
								IndexedList<Vector3> curpath=NP.CurrentPath;

								var CurrentNearbyPath=curpath.Where(v => curpos.Distance(v)<=40f);
								if (CurrentNearbyPath!=null&&CurrentNearbyPath.Any())
								{
									 CurrentNearbyPath.OrderBy(v => curpath.IndexOf(v));

									 Vector3 lastV3=vNullLocation;
									 foreach (var item in CurrentNearbyPath)
									 {
										  if (lastV3==vNullLocation)
												lastV3=item;
										  else if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(item, lastV3))
										  {
												CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "AvoidanceIntersection", 2.5f, -1);
												return true;
										  }
									 }
								}
						  }
					 }
					 else
						  return true;

					 return false;
				}
				///<summary>
				///Iterates through Usable objects and sets the Bot.CurrentTarget to the highest weighted object found inside the given list.
				///</summary>
				private void WeightEvaluationObjList()
				{
					 // Store if we are ignoring all units this cycle or not
					 bool bIgnoreAllUnits=!Bot.Combat.bAnyChampionsPresent&&!Bot.Combat.bAnyMobsInCloseRange&&((!Bot.Combat.bAnyTreasureGoblinsPresent&&Bot.SettingsFunky.GoblinPriority>=2)||Bot.SettingsFunky.GoblinPriority<2)&&
										  Bot.Character.dCurrentHealthPct>=0.85d;

					 //clear our last "avoid" list..
					 ObjectCache.Objects.objectsIgnoredDueToAvoidance.Clear();
					 bool bPrioritizeCloseRange=(Bot.Combat.bForceCloseRangeTarget||Bot.Character.bIsRooted);
					 bool bIsBerserked=HasBuff(SNOPower.Barbarian_WrathOfTheBerserker);
					 double iHighestWeightFound=0;


					 foreach (CacheObject thisobj in Bot.ValidObjects)
					 {
						  thisobj.UpdateWeight();

						  //Avoidance (Melee Only) Attempt to find a location where we can attack!
						  if (ObjectCache.Objects.objectsIgnoredDueToAvoidance.Contains(thisobj))
						  {
								Vector3 SafeLOSMovement;
								if (thisobj.Weight>iHighestWeightFound)
								{//Only if we don't have a higher priority already..

									 if (thisobj.GPRect.TryFindSafeSpot(out SafeLOSMovement, Bot.Character.Position, Bot.KiteDistance>0f, true))
									 {
										  CurrentTarget=new CacheObject(SafeLOSMovement, TargetType.Avoidance, 20000, "SafetyMovement", 2.5f, -1);
										  iHighestWeightFound=thisobj.Weight;
									 }
								}

								continue;
						  }


						  if (thisobj.Weight==1)
						  {// Force the character to stay where it is if there is nothing available that is out of avoidance stuff and we aren't already in avoidance stuff
								thisobj.Weight=0;

								if (!Bot.Combat.RequiresAvoidance)
									 Bot.Combat.bStayPutDuringAvoidance=true;

								continue;
						  }

						  // Is the weight of this one higher than the current-highest weight? Then make this the new primary target!
						  if (thisobj.Weight>iHighestWeightFound&&thisobj.Weight>0)
						  {
								//Check combat looting (Demonbuddy Setting)
								if (iHighestWeightFound>0
													 &&thisobj.targetType.Value==TargetType.Item
													 &&!Zeta.CommonBot.Settings.CharacterSettings.Instance.CombatLooting
													 &&CurrentTarget.targetType.Value==TargetType.Unit) continue;


								CacheObject curTargetObj=CurrentTarget!=null?CurrentTarget:null;
								//Set our current target to this object!
								CurrentTarget=ObjectCache.Objects[thisobj.RAGUID];
								//Check for Range Classes and Unit Targets
								if (!Bot.Class.IsMeleeClass&&CurrentTarget.targetType.Value==TargetType.Unit&&(Bot.Combat.NearbyAvoidances.Count>0||Bot.Combat.NearbyKitingUnits.Count>0))
								{
									 Ability nextAbility=GilesAbilitySelector();
									 if (nextAbility.CurrentBotRange>nextAbility.MinimumRange)
									 {
										  if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(nextAbility.DestinationVector))
										  {
												if (thisobj.ObjectIsSpecial)
													 Bot.Combat.bStayPutDuringAvoidance=true;

												CurrentTarget=curTargetObj;
												continue;
										  }
									 }
								}


								iHighestWeightFound=thisobj.Weight;
						  }

					 } // Loop through all the objects and give them a weight


					 #region RangeClassTargetUnit
					 /*
					 if (CurrentTarget!=null&&CurrentTarget.targetType.Value==TargetType.Unit&&!Bot.Class.IsMeleeClass)
					 {
						  Ability tmpSNOPowerAbility=GilesAbilitySelector(false, false, false);
						  float range=Math.Min(CurrentTarget.RadiusDistance, tmpSNOPowerAbility.iMinimumRange);
						  Vector3 abilityPosition=MathEx.GetPointAt(Bot.Character.Position, range, FindDirection(Bot.Character.Position, CurrentTarget.Position, true));

						  if (range>0f)
						  {
								Bot.Combat.bForceCloseRangeTarget=true;

								if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(Bot.Character.Position, abilityPosition))
									 CurrentTarget.BlacklistLoops=1;

								if (ObjectCache.Objects.IsPointNearbyMonsters(abilityPosition, Bot.Class.KiteDistance)&&ObjectCache.Objects.Values.OfType<CacheUnit>().Any(unit => unit.Position.Distance(abilityPosition)+unit.Radius<Bot.Class.KiteDistance))
									 CurrentTarget=ObjectCache.Objects.Values.OfType<CacheUnit>().First(unit => unit.Position.Distance(abilityPosition)+unit.Radius<Bot.Class.KiteDistance);
						  }
					 }

					 */
					 #endregion

				}


				//Prechecks are things prior to target checks and actual target handling.. This is always called first.
				public virtual bool PreChecks()
				{
					 // If we aren't in the game of a world is loading, don't do anything yet
					 if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld)
					 {
						  CurrentState=RunStatus.Success;
						  return false;
					 }

					 // See if we should update hotbar abilities
					 Bot.Class.SecondaryHotbarBuffPresent();

					 // Special pausing *AFTER* using certain powers
					 #region PauseCheck
					 if (Bot.Combat.bWaitingAfterPower&&Bot.Combat.powerPrime.WaitLoopsAfter>=1)
					 {
						  if (Bot.Combat.powerPrime.WaitLoopsAfter>=1)
								Bot.Combat.powerPrime.WaitLoopsAfter--;
						  if (Bot.Combat.powerPrime.WaitLoopsAfter<=0)
								Bot.Combat.bWaitingAfterPower=false;
						  CurrentState=RunStatus.Running;
						  return false;
					 }
					 #endregion

					 // Update player-data cache -- Special combat call
					 Bot.Character.Update(true);

					 // Check for death / player being dead
					 #region DeadCheck
					 if (Bot.Character.dCurrentHealthPct<=0)
					 {
						  //Disable OOC IDing behavior if dead!
						  if (shouldPreformOOCItemIDing)
								shouldPreformOOCItemIDing=false;

						  CurrentState=RunStatus.Success;
						  return false;
					 }
					 #endregion

					 //Herbfunk
					 //Confirmation of item looted
					 #region ItemLootedConfirmationCheck
					 if (Bot.Combat.ShouldCheckItemLooted)
					 {
						  //Reset?
						  if (CurrentTarget==null||CurrentTarget.targetType.HasValue&&CurrentTarget.targetType.Value!=TargetType.Item)
						  {
								Bot.Combat.ShouldCheckItemLooted=false;
								return false;
						  }

						  //Vendor Behavior
						  if (Zeta.CommonBot.Logic.BrainBehavior.IsVendoring)
						  {
								CurrentState=RunStatus.Success;
								return false;
						  }
						  else if (Bot.Character.bIsIncapacitated)
						  {
								CurrentState=RunStatus.Running;
								return false;
						  }

						  //Count each attempt to confirm.
						  Bot.Combat.recheckCount++;
						  string statusText="[Item Confirmation] Current recheck count "+Bot.Combat.recheckCount;
						  bool LootedSuccess=Bot.Character.BackPack.ContainsItem(CurrentTarget.AcdGuid.Value);
						  statusText+=" [ItemFound="+LootedSuccess+"]";
						  if (LootedSuccess)
						  {
								Zeta.CommonBot.GameEvents.FireItemLooted(CurrentTarget.AcdGuid.Value);

								if (Bot.SettingsFunky.DebugStatusBar) BotMain.StatusText=statusText;

								//This is where we should manipulate information of both what dropped and what was looted.
								LogItemInformation();

								//Reset if we reach here..
								Bot.Combat.reCheckedFinished=false;
								Bot.Combat.recheckCount=0;
								Bot.Combat.ShouldCheckItemLooted=false;
								Bot.Combat.bForceTargetUpdate=true;
						  }
						  else
						  {
								CacheItem thisObjItem=(CacheItem)CurrentTarget;

								statusText+=" [Quality";
								//Quality of the item determines the recheck attempts.
								ItemQuality curQuality=thisObjItem.Itemquality.Value;
								#region QualityRecheckSwitch
								switch (curQuality)
								{
									 case ItemQuality.Inferior:
									 case ItemQuality.Invalid:
									 case ItemQuality.Special:
									 case ItemQuality.Superior:
									 case ItemQuality.Normal:
									 case ItemQuality.Magic1:
									 case ItemQuality.Magic2:
									 case ItemQuality.Magic3:
										  statusText+="<=Magical]";
										  //Non-Quality items get skipped quickly.
										  if (Bot.Combat.recheckCount>1)
												Bot.Combat.reCheckedFinished=true;
										  break;

									 case ItemQuality.Rare4:
									 case ItemQuality.Rare5:
									 case ItemQuality.Rare6:
										  statusText+="=Rare]";
										  if (Bot.Combat.recheckCount>2)
												Bot.Combat.reCheckedFinished=true;
										  //else
										  //bItemForcedMovement = true;

										  break;

									 case ItemQuality.Legendary:
										  statusText+="=Legendary]";
										  if (Bot.Combat.recheckCount>3)
												Bot.Combat.reCheckedFinished=true;
										  //else
										  //bItemForcedMovement = true;

										  break;
								}
								#endregion

								//If we are still rechecking then use the waitAfter (powerprime ability related) to wait a few loops.
								if (!Bot.Combat.reCheckedFinished)
								{
									 statusText+=" RECHECKING";
									 if (Bot.SettingsFunky.DebugStatusBar)
									 {
										  BotMain.StatusText=statusText;
									 }
									 Bot.Combat.bWaitingAfterPower=true;
									 Bot.Combat.powerPrime.WaitLoopsAfter=3;
									 CurrentState=RunStatus.Running;
									 return false;
								}
								else
								{
									 //We Rechecked Max Confirmation Checking Count, now we check if we want to retry confirmation, or simply try once more then ignore for a few.


									 if (thisObjItem.Itemquality.Value>ItemQuality.Magic3)
									 {
										  //Items above rare quality don't get blacklisted, just ignored for a few loops.
										  //This will force a movement if stuck.. but 5 loops is only 750ms
										  CurrentTarget.BlacklistLoops=5;
									 }
									 else
									 {
										  //Blacklist items below rare quality!
										  CurrentTarget.BlacklistFlag=BlacklistType.Temporary;
										  CurrentTarget.NeedsRemoved=true;
									 }

									 // Now tell Trinity to get a new target!
									 Bot.Combat.bForceTargetUpdate=true;
								}
						  }

						  //Reset flag, and continue..
						  Bot.Combat.ShouldCheckItemLooted=false;
					 }
					 #endregion


					 // See if we have been "newly rooted", to force target updates
					 if (Bot.Character.bIsRooted&&!Bot.Combat.bWasRootedLastTick)
					 {
						  Bot.Combat.bWasRootedLastTick=true;
						  Bot.Combat.bForceTargetUpdate=true;
					 }
					 if (!Bot.Character.bIsRooted)
						  Bot.Combat.bWasRootedLastTick=false;

					 return true;
				}

				//This is the 2nd step in handling.. we recheck target, get a new ability if needed, and check potion/special movement avoidance here.
				public virtual bool Refresh()
				{
					 // Make sure we reset unstucker stuff here
					 Funky.PlayerMover.iTimesReachedStuckPoint=0;
					 Funky.PlayerMover.vSafeMovementLocation=Vector3.Zero;
					 Funky.PlayerMover.timeLastRecordedPosition=DateTime.Now;

					 // Let's calculate whether or not we want a new target list...
					 #region NewtargetChecks
					 // Whether we should refresh the target list or not
					 bool bShouldRefreshDiaObjects=false;

					 if (!Bot.Combat.bWholeNewTarget&&!Bot.Combat.bWaitingForPower&&!Bot.Combat.bWaitingForPotion)
					 {
						  // Update targets at least once every 80 milliseconds
						  if (Bot.Combat.bForceTargetUpdate||Bot.Combat.TravellingAvoidance||
							  ((DateTime.Now.Subtract(Bot.lastRefreshedObjects).TotalMilliseconds>=80&&CurrentTarget.targetType.Value!=TargetType.Avoidance)||
								 DateTime.Now.Subtract(Bot.lastRefreshedObjects).TotalMilliseconds>=800))
						  {
								bShouldRefreshDiaObjects=true;
						  }

						  // If we AREN'T getting new targets - find out if we SHOULD because the current unit has died etc.
						  if (!bShouldRefreshDiaObjects&&CurrentTarget.targetType.Value==TargetType.Unit)
						  {
								if (!CurrentTarget.IsStillValid())
								{
									 bShouldRefreshDiaObjects=true;
								}
						  }
					 }

					 // So, after all that, do we actually want a new target list?
					 if (!Bot.Combat.bWholeNewTarget&&!Bot.Combat.bWaitingForPower&&!Bot.Combat.bWaitingForPotion)
					 {
						  // If we *DO* want a new target list, do this... 
						  if (bShouldRefreshDiaObjects)
						  {
								// Now call the function that refreshes targets
								Bot.RefreshDiaObjects();

								// No target, return success
								if (CurrentTarget==null)
								{
									 CurrentState=RunStatus.Success;
									 return false;
								}
								else if (Bot.Character.LastCachedTarget!=null&&
									 Bot.Character.LastCachedTarget.RAGUID!=CurrentTarget.RAGUID&&CurrentTarget.targetType.Value==TargetType.Item)
								{
									 //Reset Item Vars
									 Bot.Combat.recheckCount=0;
									 Bot.Combat.reCheckedFinished=false;
								}

								// Been trying to handle the same target for more than 30 seconds without damaging/reaching it? Blacklist it!
								// Note: The time since target picked updates every time the current target loses health, if it's a monster-target
								if (CurrentTarget.targetType.Value!=TargetType.Avoidance&&
									((CurrentTarget.targetType.Value!=TargetType.Unit&&DateTime.Now.Subtract(Bot.Combat.dateSincePickedTarget).TotalSeconds>12)||
									 (CurrentTarget.targetType.Value==TargetType.Unit&&!CurrentTarget.IsBoss&&DateTime.Now.Subtract(Bot.Combat.dateSincePickedTarget).TotalSeconds>40))
									)
								{
									 // NOTE: This only blacklists if it's remained the PRIMARY TARGET that we are trying to actually directly attack!
									 // So it won't blacklist a monster "on the edge of the screen" who isn't even being targetted
									 // Don't blacklist monsters on <= 50% health though, as they can't be in a stuck location... can they!? Maybe give them some extra time!
									 bool bBlacklistThis=true;
									 // PREVENT blacklisting a monster on less than 90% health unless we haven't damaged it for more than 2 minutes
									 if (CurrentTarget.targetType.Value==TargetType.Unit)
									 {
										  if (CurrentTarget.IsTreasureGoblin&&Bot.SettingsFunky.GoblinPriority>=3)
												bBlacklistThis=false;
										  if (DateTime.Now.Subtract(Bot.Combat.dateSincePickedTarget).TotalSeconds<=120)
												bBlacklistThis=false;
									 }
									 if (bBlacklistThis)
									 {
										  if (CurrentTarget.targetType.Value==TargetType.Unit)
										  {
												//Logging.WriteDiagnostic("[Funky] Blacklisting a monster because of possible stuck issues. Monster="+ObjectData.InternalName+" {"+
												//ObjectData.SNOID.ToString()+"}. Range="+ObjectData.CentreDistance.ToString()+", health %="+ObjectData.CurrentHealthPct.ToString());
										  }
										  CurrentTarget.NeedsRemoved=true;
										  CurrentTarget.BlacklistFlag=BlacklistType.Temporary;
									 }
								}
								// Make sure we start trying to move again should we need to!
								Bot.Combat.bAlreadyMoving=false;
								Bot.Combat.lastMovementCommand=DateTime.Today;
								Bot.Combat.bPickNewAbilities=true;
								Bot.Combat.fLastDistanceFromTarget=-1f;
						  }
						  // Ok we didn't want a new target list, should we at least update the position of the current target, if it's a monster?
						  else if (CurrentTarget.targetType.Value==TargetType.Unit&&CurrentTarget.IsStillValid())
						  {
								try
								{
									 CurrentTarget.UpdatePosition();
								} catch
								{
									 // Keep going anyway if we failed to get a new position from DemonBuddy
									 Logging.WriteDiagnostic("GSDEBUG: Caught position read failure in main target handler loop.");
								}
						  }
					 }
					 #endregion

					 // This variable just prevents an instant 2-target update after coming here from the main decorator function above
					 Bot.Combat.bWholeNewTarget=false;


					 //Health Change Timer
					 if (CurrentTarget.targetType.Value==TargetType.Unit&&
						  DateTime.Now.Subtract(Bot.Combat.LastHealthChange).TotalMilliseconds>3000)
					 {
						  Logging.WriteVerbose("Health change has not occured within 3 seconds for unit {0}", CurrentTarget.InternalName);
						  CurrentTarget.NeedsRemoved=true;
						  return false;
					 }


					 //We are ready for the specific object type interaction
					 return true;
				}

				public virtual bool CombatLogic()
				{
					 // Find a valid ability if the target is a monster
					 #region AbilityPick
					 if (Bot.Combat.bPickNewAbilities&&!Bot.Combat.bWaitingForPower&&!Bot.Combat.bWaitingForPotion)
					 {
						  Bot.Combat.bPickNewAbilities=false;
						  if (CurrentTarget.targetType.Value==TargetType.Unit&&CurrentTarget.AcdGuid.HasValue)
						  {
								//ToDo: Check clustering..
								// Pick a suitable ability								Shielded units: Find destructible power instead.
								Bot.Combat.powerPrime=GilesAbilitySelector(false, false,false);

								//Check LOS still valid...
								#region LOSUpdate
								if (!CurrentTarget.RequiresLOSCheck&&CurrentTarget.LastLOSCheckMS>3000)
								{
									 if (!CurrentTarget.IgnoresLOSCheck)
									 {
										  NavCellFlags LOSNavFlags=NavCellFlags.None;
										  if (Bot.Class.IsMeleeClass||!CurrentTarget.WithinInteractionRange())
										  {
												if (!Bot.Class.IsMeleeClass) //Add Projectile Testing!
													 LOSNavFlags=NavCellFlags.AllowWalk|NavCellFlags.AllowProjectile;
												else
													 LOSNavFlags=NavCellFlags.AllowWalk;
										  }

										  if (!CurrentTarget.LOSTest(Bot.Character.Position, true, (!Bot.Class.IsMeleeClass), LOSNavFlags))
										  {
												//LOS failed.. now we should decide if we want to find a spot for this target, or just ignore it.
												if (CurrentTarget.ObjectIsSpecial&&CurrentTarget.LastLOSSearchMS>2500)
												{
													 CurrentTarget.LastLOSSearch=DateTime.Now;

													 GridPointAreaCache.GPRectangle TargetGPRect=CurrentTarget.GPRect;
													 //Expand GPRect into 5x5, 7x7 for ranged!
													 TargetGPRect.FullyExpand();
													 if (!Bot.Class.IsMeleeClass)
														  TargetGPRect.FullyExpand();

													 Vector3 LOSV3;
													 if (TargetGPRect.TryFindSafeSpot(out LOSV3, CurrentTarget.BotMeleeVector))
													 {
														  CurrentTarget.LOSV3=LOSV3;
														  Logging.WriteVerbose("Using LOS Vector at {0} to move to", LOSV3.ToString());
														  CurrentTarget.RequiresLOSCheck=false;
														  Bot.Combat.bWholeNewTarget=true;
														  CurrentState=RunStatus.Running;
														  return false;
													 }
												}

												//We could not find a LOS Locaiton or did not find a reason to try.. so we reset LOS check, temp ignore it, and force new target.
												Logging.WriteVerbose("LOS Request for object {0} due to raycast failure!", CurrentTarget.InternalName);
												Bot.Combat.bForceTargetUpdate=true;
												CurrentState=RunStatus.Running;
												return false;
										  }
										  else
												CurrentTarget.RequiresLOSCheck=false;
									 }
								}
								#endregion
						  }

						  // Select an ability for destroying a destructible with in advance
						  if (CurrentTarget.targetType.Value==TargetType.Destructible||CurrentTarget.targetType==TargetType.Barricade)
								Bot.Combat.powerPrime=GilesAbilitySelector(false, false, true);
					 }
					 #endregion

					 // Pop a potion when necessary
					 // Note that we force a single-loop pause first, to help potion popping "go off"
					 #region PotionCheck
					 if (Bot.Character.dCurrentHealthPct<=Bot.EmergencyHealthPotionLimit
							&&!Bot.Combat.bWaitingForPower&&!Bot.Combat.bWaitingForPotion&&!Bot.Character.bIsIncapacitated&&AbilityUseTimer(SNOPower.DrinkHealthPotion))
					 {
						  Bot.Combat.bWaitingForPotion=true;
						  CurrentState=RunStatus.Running;
						  return false;
					 }
					 if (Bot.Combat.bWaitingForPotion)
					 {
						  Bot.Combat.bWaitingForPotion=false;
						  if (!Bot.Character.bIsIncapacitated&&AbilityUseTimer(SNOPower.DrinkHealthPotion))
						  {
								Bot.AttemptToUseHealthPotion();
						  }
					 }
					 #endregion

					 // See if we can use any special buffs etc. while in avoidance
					 #region AvoidanceSpecialAbilityCheck
					 if (CurrentTarget.targetType.Value==TargetType.Avoidance)
					 {
						  Bot.Combat.powerBuff=GilesAbilitySelector(true, false, false);
						  if (Bot.Combat.powerBuff.Power!=SNOPower.None)
						  {
								ZetaDia.Me.UsePower(Bot.Combat.powerBuff.Power, Bot.Combat.powerBuff.TargetPosition, Bot.Combat.powerBuff.WorldID, Bot.Combat.powerBuff.TargetRAGUID);
								Bot.Combat.powerLastSnoPowerUsed=Bot.Combat.powerBuff.Power;
								dictAbilityLastUse[Bot.Combat.powerBuff.Power]=DateTime.Now;
						  }
					 }
					 #endregion

					 return true;
				}


				public virtual bool Movement()
				{
					 // Set current destination to our current target's destination
					 Bot.Combat.vCurrentDestination=CurrentTarget.Position;
					 if (CurrentTarget.LOSV3!=vNullLocation)
					 {
						  //Recheck LOS every second
						  if (CurrentTarget.LastLOSCheckMS>2500)
						  {
								NavCellFlags LOSNavFlags=NavCellFlags.None;
								if (Bot.Class.IsMeleeClass||!CurrentTarget.WithinInteractionRange())
								{
									 if (!Bot.Class.IsMeleeClass) //Add Projectile Testing!
										  LOSNavFlags=NavCellFlags.AllowWalk|NavCellFlags.AllowProjectile;
									 else
										  LOSNavFlags=NavCellFlags.AllowWalk;
								}

								if (CurrentTarget.LOSTest(Bot.Character.Position, true, (!Bot.Class.IsMeleeClass), LOSNavFlags))
								{
									 //Los Passed!
									 CurrentTarget.LOSV3=vNullLocation;
									 Bot.Combat.bWholeNewTarget=true;
									 return false;
								}
						  }

						  Bot.Combat.vCurrentDestination=CurrentTarget.LOSV3;
						  if (Bot.Character.Position.Distance(CurrentTarget.LOSV3)>2.5f)
						  {
								CurrentState=CurrentTarget.MoveTowards();
								return false;
						  }
					 }



					 //Check if we are in range for interaction..
					 if (CurrentTarget.WithinInteractionRange())
						  return true;
					 else
					 {//Movement required..
						  CurrentState=CurrentTarget.MoveTowards();
						  return false;
					 }
				}

				//This is the final step in handling.. here we actually switch to a specific method based upon the target object we are handling.
				public virtual bool ObjectInteraction()
				{

					 #region DebugInfo
					 if (Bot.SettingsFunky.DebugStatusBar)
					 {
						  sStatusText="[Interact- ";
						  switch (CurrentTarget.targetType.Value)
						  {
								case TargetType.Avoidance:
									 sStatusText+="Avoid] ";
									 break;
								case TargetType.Unit:
									 sStatusText+="Combat] ";
									 break;
								case TargetType.Item:
								case TargetType.Gold:
								case TargetType.Globe:
									 sStatusText+="Pickup] ";
									 break;
								case TargetType.Interactable:
									 sStatusText+="Interact] ";
									 break;
								case TargetType.Container:
									 sStatusText+="Open] ";
									 break;
								case TargetType.Destructible:
								case TargetType.Barricade:
									 sStatusText+="Destroy] ";
									 break;
								case TargetType.Shrine:
									 sStatusText+="Click] ";
									 break;
						  }
						  sStatusText+="Target="+CurrentTarget.InternalName+" C-Dist="+Math.Round(CurrentTarget.CentreDistance, 2).ToString()+". "+
								  "R-Dist="+Math.Round(CurrentTarget.RadiusDistance, 2).ToString()+". ";

						  if (CurrentTarget.targetType.Value==TargetType.Unit&&Bot.Combat.powerPrime.Power!=SNOPower.None)
								sStatusText+="Power="+Bot.Combat.powerPrime.Power.ToString()+" (range "+Bot.Combat.powerPrime.MinimumRange.ToString()+") ";


						  sStatusText+="Weight="+CurrentTarget.Weight.ToString();
						  BotMain.StatusText=sStatusText;
						  bResetStatusText=true;
					 }
					 #endregion

					 switch (CurrentTarget.targetType.Value)
					 {
						  case TargetType.Unit:
						  case TargetType.Item:
						  case TargetType.Gold:
						  case TargetType.Globe:
						  case TargetType.Shrine:
						  case TargetType.Interactable:
						  case TargetType.Container:
						  case TargetType.Door:
						  case TargetType.Destructible:
						  case TargetType.Barricade:
								CurrentState=CurrentTarget.Interact();
								break;
						  case TargetType.Avoidance:
								CurrentState=RunStatus.Running;
								break;
					 }

					 // Now tell Trinity to get a new target!
					 Bot.Combat.lastChangedZigZag=DateTime.Today;
					 Bot.Combat.vPositionLastZigZagCheck=Vector3.Zero;
					 Bot.Combat.bForceTargetUpdate=true;

					 return false;
				}

				internal void UpdateStatusText(string Action)
				{
					 sStatusText=Action+" ";

					 sStatusText+="Target="+CurrentTarget.InternalName+" C-Dist="+Math.Round(CurrentTarget.CentreDistance, 2).ToString()+". "+
						  "R-Dist="+Math.Round(CurrentTarget.RadiusDistance, 2).ToString()+". ";

					 if (CurrentTarget.targetType.Value==TargetType.Unit&&Bot.Combat.powerPrime.Power!=SNOPower.None)
						  sStatusText+="Power="+Bot.Combat.powerPrime.Power.ToString()+" (range "+Bot.Combat.powerPrime.MinimumRange.ToString()+") ";

					 sStatusText+="Weight="+CurrentTarget.Weight.ToString();
					 BotMain.StatusText=sStatusText;
					 bResetStatusText=true;
				}

				public override bool Equals(object obj)
				{
					 //Check for null and compare run-time types. 
					 if (obj==null)
					 {
						  if (this.CurrentTarget!=null)
								return false;
						  else
								return true;
					 }
					 else
					 {
						  Type ta=obj.GetType();
						  Type tb=this.CurrentTarget!=null?this.CurrentTarget.GetType():this.GetType();

						  if (ta.Equals(tb))
						  {
								return ((CacheObject)obj)==(this.CurrentTarget);
						  }
						  else
								return false;
					 }
				}
				public override int GetHashCode()
				{
					 return this.CurrentTarget!=null?this.CurrentTarget.GetHashCode():base.GetHashCode();
				}
		  }
	 }
}