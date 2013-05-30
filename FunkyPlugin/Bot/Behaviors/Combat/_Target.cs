using System;
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

		  public class TargetHandler
		  {
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

				public override bool Equals(object obj)
				{
					 //Check for null and compare run-time types. 
					 if (obj==null)
					 {
						  if (this.ObjectData!=null)
								return false;
						  else
								return true;
					 }
					 else
					 {
						  Type ta=obj.GetType();
						  Type tb=this.ObjectData!=null?this.ObjectData.GetType():this.GetType();

						  if (ta.Equals(tb))
						  {
								return ((CacheObject)obj)==(this.ObjectData);
						  }
						  else
								return false;
					 }
				}
				public override int GetHashCode()
				{
					 return this.ObjectData!=null?this.ObjectData.GetHashCode():base.GetHashCode();
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
				public CacheObject ObjectData;

				//We only actually call this the first time, during the class BOT initilizing
				public TargetHandler()
				{
					 CurrentState=RunStatus.Running;
					 ObjectData=null;
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
					 if (Bot.Combat.bWaitingAfterPower&&Bot.Combat.powerPrime.iForceWaitLoopsAfter>=1)
					 {
						  if (Bot.Combat.powerPrime.iForceWaitLoopsAfter>=1)
								Bot.Combat.powerPrime.iForceWaitLoopsAfter--;
						  if (Bot.Combat.powerPrime.iForceWaitLoopsAfter<=0)
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
						  if (ObjectData==null||ObjectData.targetType.HasValue&&ObjectData.targetType.Value!=TargetType.Item)
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
						  bool LootedSuccess=Bot.Character.BackPack.ContainsItem(ObjectData.AcdGuid.Value);
						  statusText+=" [ItemFound="+LootedSuccess+"]";
						  if (LootedSuccess)
						  {
								Zeta.CommonBot.GameEvents.FireItemLooted(ObjectData.AcdGuid.Value);

								if (SettingsFunky.DebugStatusBar) BotMain.StatusText=statusText;

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
								CacheItem thisObjItem=(CacheItem)ObjectData;

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
									 if (SettingsFunky.DebugStatusBar)
									 {
										  BotMain.StatusText=statusText;
									 }
									 Bot.Combat.bWaitingAfterPower=true;
									 Bot.Combat.powerPrime.iForceWaitLoopsAfter=3;
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
										  ObjectData.BlacklistLoops=5;
									 }
									 else
									 {
										  //Blacklist items below rare quality!
										  ObjectData.BlacklistFlag=BlacklistType.Temporary;
										  ObjectData.NeedsRemoved=true;
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
							  ((DateTime.Now.Subtract(lastRefreshedObjects).TotalMilliseconds>=80&&ObjectData.targetType.Value!=TargetType.Avoidance)||
								 DateTime.Now.Subtract(lastRefreshedObjects).TotalMilliseconds>=800))
						  {
								bShouldRefreshDiaObjects=true;
						  }

						  // If we AREN'T getting new targets - find out if we SHOULD because the current unit has died etc.
						  if (!bShouldRefreshDiaObjects&&ObjectData.targetType.Value==TargetType.Unit)
						  {
								if (!ObjectData.IsStillValid())
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
								dbRefresh.RefreshDiaObjects();

								// No target, return success
								if (ObjectData==null)
								{
									 CurrentState=RunStatus.Success;
									 return false;
								}
								else if (Bot.Character.LastCachedTarget!=null&&
									 Bot.Character.LastCachedTarget.RAGUID!=ObjectData.RAGUID&&ObjectData.targetType.Value==TargetType.Item)
								{
									 //Reset Item Vars
									 Bot.Combat.recheckCount=0;
									 Bot.Combat.reCheckedFinished=false;
								}

								// Been trying to handle the same target for more than 30 seconds without damaging/reaching it? Blacklist it!
								// Note: The time since target picked updates every time the current target loses health, if it's a monster-target
								if (ObjectData.targetType.Value!=TargetType.Avoidance&&
									((ObjectData.targetType.Value!=TargetType.Unit&&DateTime.Now.Subtract(Bot.Combat.dateSincePickedTarget).TotalSeconds>12)||
									 (ObjectData.targetType.Value==TargetType.Unit&&!ObjectData.IsBoss&&DateTime.Now.Subtract(Bot.Combat.dateSincePickedTarget).TotalSeconds>40))
									)
								{
									 // NOTE: This only blacklists if it's remained the PRIMARY TARGET that we are trying to actually directly attack!
									 // So it won't blacklist a monster "on the edge of the screen" who isn't even being targetted
									 // Don't blacklist monsters on <= 50% health though, as they can't be in a stuck location... can they!? Maybe give them some extra time!
									 bool bBlacklistThis=true;
									 // PREVENT blacklisting a monster on less than 90% health unless we haven't damaged it for more than 2 minutes
									 if (ObjectData.targetType.Value==TargetType.Unit)
									 {
										  if (ObjectData.IsTreasureGoblin&&SettingsFunky.GoblinPriority>=3)
												bBlacklistThis=false;
										  if (DateTime.Now.Subtract(Bot.Combat.dateSincePickedTarget).TotalSeconds<=120)
												bBlacklistThis=false;
									 }
									 if (bBlacklistThis)
									 {
										  if (ObjectData.targetType.Value==TargetType.Unit)
										  {
												//Logging.WriteDiagnostic("[Funky] Blacklisting a monster because of possible stuck issues. Monster="+ObjectData.InternalName+" {"+
												//ObjectData.SNOID.ToString()+"}. Range="+ObjectData.CentreDistance.ToString()+", health %="+ObjectData.CurrentHealthPct.ToString());
										  }
										  ObjectData.NeedsRemoved=true;
										  ObjectData.BlacklistFlag=BlacklistType.Temporary;
									 }
								}
								// Make sure we start trying to move again should we need to!
								Bot.Combat.bAlreadyMoving=false;
								Bot.Combat.lastMovementCommand=DateTime.Today;
								Bot.Combat.bPickNewAbilities=true;
						  }
						  // Ok we didn't want a new target list, should we at least update the position of the current target, if it's a monster?
						  else if (ObjectData.targetType.Value==TargetType.Unit&&ObjectData.IsStillValid())
						  {
								try
								{
									 ObjectData.UpdatePosition();
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

					 // Find a valid ability if the target is a monster
					 #region AbilityPick
					 if (Bot.Combat.bPickNewAbilities&&!Bot.Combat.bWaitingForPower&&!Bot.Combat.bWaitingForPotion)
					 {
						  Bot.Combat.bPickNewAbilities=false;
						  if (ObjectData.targetType.Value==TargetType.Unit&&ObjectData.AcdGuid.HasValue)
						  {
								//ToDo: Check clustering..
								// Pick a suitable ability								Shielded units: Find destructible power instead.
								Bot.Combat.powerPrime=GilesAbilitySelector(false, false, !ObjectData.CanInteract());

								//Check LOS still valid...
								#region LOSUpdate
								if (!ObjectData.IgnoresLOSCheck&&ObjectData.RequiresLOSCheck&&ObjectData.LastLOSSearchMS>1800)
								{
									 if (!ObjectData.LOSTest(Bot.Character.Position, true, (!Bot.Class.IsMeleeClass), (Bot.Class.IsMeleeClass)))
									 {
										  //LOS failed.. now we should decide if we want to find a spot for this target, or just ignore it.
										  if (ObjectData.ObjectIsSpecial)
										  {
												if (ObjectData.FindLOSLocation)
												{
													 Logging.WriteVerbose("Using LOS Vector at {0} to move to", ObjectData.LOSV3.ToString());
                                                     ObjectData.RequiresLOSCheck = false;
													 Bot.Combat.bWholeNewTarget=true;
													 CurrentState=RunStatus.Running;
													 return false;
												}
												else
												{
													 ObjectData.BlacklistLoops=10;
												}
										  }

										  //We could not find a LOS Locaiton or did not find a reason to try.. so we reset LOS check, temp ignore it, and force new target.
										  Logging.WriteVerbose("LOS Request for object {0} due to raycast failure!", ObjectData.InternalName);
										  Bot.Combat.bForceTargetUpdate=true;
										  CurrentState=RunStatus.Running;
										  return false;
									 }
									 else
                                         ObjectData.RequiresLOSCheck = false;
								}
								#endregion
						  }

						  // Select an ability for destroying a destructible with in advance
						  if (ObjectData.targetType.Value==TargetType.Destructible||ObjectData.targetType==TargetType.Barricade)
								Bot.Combat.powerPrime=GilesAbilitySelector(false, false, true);
					 }
					 #endregion

					 // Pop a potion when necessary
					 // Note that we force a single-loop pause first, to help potion popping "go off"
					 #region PotionCheck
					 if (Bot.Character.dCurrentHealthPct<=Bot.Class.EmergencyHealthPotionLimit
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
					 if (ObjectData.targetType.Value==TargetType.Avoidance)
					 {
						  Bot.Combat.powerBuff=GilesAbilitySelector(true, false, false);
						  if (Bot.Combat.powerBuff.Power!=SNOPower.None)
						  {
								ZetaDia.Me.UsePower(Bot.Combat.powerBuff.Power, Bot.Combat.powerBuff.TargetPosition, Bot.Combat.powerBuff.WorldID, Bot.Combat.powerBuff.TargetRaGuid);
								Bot.Combat.powerLastSnoPowerUsed=Bot.Combat.powerBuff.Power;
								dictAbilityLastUse[Bot.Combat.powerBuff.Power]=DateTime.Now;
						  }
					 }
					 #endregion

					 //We are ready for the specific object type interaction
					 return true;
				}

				//3rd Step..
				public virtual bool Movement()
				{
					 // Set current destination to our current target's destination
					 Bot.Combat.vCurrentDestination=ObjectData.Position;
					 bool LOSMoving=ObjectData.LOSV3!=vNullLocation;

					 if (LOSMoving)
					 {
						  Bot.Combat.vCurrentDestination=ObjectData.LOSV3;
						  if (Bot.Character.Position.Distance(ObjectData.LOSV3)>10f)
						  {
								CurrentState=ObjectData.MoveTowards();
								return false;
						  }
						  else
						  {
								ObjectData.LOSV3=vNullLocation;
								Bot.Combat.bPickNewAbilities=true;
								return false;
						  }
					 }

					 //Check if we used AutoMovement (Melee Targeting)
					 #region AutoMovement
					 if (Bot.Combat.UsedAutoMovementCommand)
					 {
						  //Waiting for movement to occur..
						  if (DateTime.Now.Subtract(Bot.Combat.lastMovementCommand).TotalMilliseconds<50)
								return false;

						  //Force update to get accurate read
						  Bot.Character.UpdateMovementData();


						  if (SettingsFunky.DebugStatusBar)
						  {
								BotMain.StatusText=("[Funky] Movement Command, Movement State "+Bot.Character.currentMovementState.ToString()+
										 ", IsMoving: "+Bot.Character.isMoving.ToString()+", MovementTarget Match "+(ObjectData.AcdGuid.Value==Bot.Character.iCurrentMovementTargetGUID).ToString());
						  }

						  if (Bot.Character.isMoving)
						  {//We are successfully moving..
								if (Bot.Character.currentMovementState==MovementState.WalkingInPlace)
								{//Check if we are stuck moving in place..
									 Logging.WriteVerbose("Movement State returned {0} , Blacklist Bot.CurrentTarget!", Bot.Character.currentMovementState.ToString());
									 Bot.Combat.bForceTargetUpdate=true;
									 ObjectData.BlacklistLoops=5;
								}
								return false;
						  }

						  //If we make it here its because.. We are not moving OR we are moving but target IDs do not match!
						  //Turn off automovement, force target update, and switch off wait.
						  Bot.Combat.UsedAutoMovementCommand=false;
						  Bot.Combat.bForceTargetUpdate=true;
						  Bot.Combat.bWaitingForPower=false;
						  Bot.Combat.bPickNewAbilities=true;
					 }
					 #endregion

					 // Maintain an area list of all zones we pass through/near while moving, for our custom navigation handler
					 //PlayerMover.RecordSkipAheadCachePoint();

					 //Check if we are in range for interaction..
					 if (ObjectData.WithinInteractionRange())
						  return true;
					 else
					 {//Movement required..
						  CurrentState=ObjectData.MoveTowards();
						  return false;
					 }
				}

				//This is the final step in handling.. here we actually switch to a specific method based upon the target object we are handling.
				public virtual bool ObjectInteraction()
				{

					 #region DebugInfo
					 if (SettingsFunky.DebugStatusBar)
					 {
						  sStatusText="[Interact- ";
						  switch (ObjectData.targetType.Value)
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
						  sStatusText+="Target="+ObjectData.InternalName+" C-Dist="+Math.Round(ObjectData.CentreDistance, 2).ToString()+". "+
								  "R-Dist="+Math.Round(ObjectData.RadiusDistance, 2).ToString()+". ";

						  if (ObjectData.targetType.Value==TargetType.Unit&&Bot.Combat.powerPrime.Power!=SNOPower.None)
								sStatusText+="Power="+Bot.Combat.powerPrime.Power.ToString()+" (range "+Bot.Combat.powerPrime.iMinimumRange.ToString()+") ";


						  sStatusText+="Weight="+ObjectData.Weight.ToString();
						  BotMain.StatusText=sStatusText;
						  bResetStatusText=true;
					 }
					 #endregion

					 switch (ObjectData.targetType.Value)
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
								CurrentState=ObjectData.Interact();
								break;
						  case TargetType.Avoidance:
								//No avoidances remain so we need to reset this here.. else it wont reset since no cache won't have any avoidances to check.
								if (!ObjectCache.Obstacles.Avoidances.Any(avoid => avoid.Obstacletype.Value==ObstacleType.StaticAvoidance))
									 Bot.Combat.RequiresAvoidance=false;

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

					 sStatusText+="Target="+ObjectData.InternalName+" C-Dist="+Math.Round(ObjectData.CentreDistance, 2).ToString()+". "+
						  "R-Dist="+Math.Round(ObjectData.RadiusDistance, 2).ToString()+". ";

					 if (ObjectData.targetType.Value==TargetType.Unit&&Bot.Combat.powerPrime.Power!=SNOPower.None)
						  sStatusText+="Power="+Bot.Combat.powerPrime.Power.ToString()+" (range "+Bot.Combat.powerPrime.iMinimumRange.ToString()+") ";

					 sStatusText+="Weight="+ObjectData.Weight.ToString();
					 BotMain.StatusText=sStatusText;
					 bResetStatusText=true;
				}




		  }
	 }
}