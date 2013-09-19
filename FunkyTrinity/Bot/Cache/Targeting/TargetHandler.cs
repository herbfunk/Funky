using System;
using System.Linq;
using FunkyTrinity.ability;
using FunkyTrinity.Cache;
using FunkyTrinity.Enums;
using FunkyTrinity.Movement;
using FunkyTrinity.Movement.Clustering;
using FunkyTrinity.Targeting.Behaviors;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;
using Zeta.TreeSharp;

namespace FunkyTrinity.Targeting
{
	 public partial class TargetHandler
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

		  //Used to reduce additional unboxing when target is an unit.
		  internal CacheUnit CurrentUnitTarget;



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
				FunkyTrinity.Bot.Class.SecondaryHotbarBuffPresent();


				// Special pausing *AFTER* using certain powers
				#region PauseCheck
				if (FunkyTrinity.Bot.Combat.bWaitingAfterPower&&FunkyTrinity.Bot.Combat.powerPrime.WaitLoopsAfter>=1)
				{
					 if (FunkyTrinity.Bot.Combat.powerPrime.WaitLoopsAfter>=1) FunkyTrinity.Bot.Combat.powerPrime.WaitLoopsAfter--;
					 if (FunkyTrinity.Bot.Combat.powerPrime.WaitLoopsAfter<=0) FunkyTrinity.Bot.Combat.bWaitingAfterPower=false;

					 CurrentState=RunStatus.Running;
					 return false;
				}
				#endregion

				// Update player-data cache -- Special combat call
				FunkyTrinity.Bot.Character.Update(true);

				// Check for death / player being dead
				#region DeadCheck
				if (FunkyTrinity.Bot.Character.dCurrentHealthPct<=0)
				{
					 //Disable OOC IDing behavior if dead!
					 if (Funky.shouldPreformOOCItemIDing) Funky.shouldPreformOOCItemIDing=false;

					 CurrentState=RunStatus.Success;
					 return false;
				}
				#endregion

				//Herbfunk
				//Confirmation of item looted
				#region ItemLootedConfirmationCheck
				if (FunkyTrinity.Bot.Combat.ShouldCheckItemLooted)
				{
					 //Reset?
					 if (CurrentTarget==null||CurrentTarget.targetType.HasValue&&CurrentTarget.targetType.Value!=TargetType.Item)
					 {
						  FunkyTrinity.Bot.Combat.ShouldCheckItemLooted=false;
						  return false;
					 }

					 //Vendor Behavior
					 if (Zeta.CommonBot.Logic.BrainBehavior.IsVendoring)
					 {
						  CurrentState=RunStatus.Success;
						  return false;
					 }
					 else if (FunkyTrinity.Bot.Character.bIsIncapacitated)
					 {
						  CurrentState=RunStatus.Running;
						  return false;
					 }

					 //Count each attempt to confirm.
					 FunkyTrinity.Bot.Combat.recheckCount++;
					 string statusText="[Item Confirmation] Current recheck count "+FunkyTrinity.Bot.Combat.recheckCount;
					 bool LootedSuccess=FunkyTrinity.Bot.Character.BackPack.ContainsItem(CurrentTarget.AcdGuid.Value);
					 //Verify item is non-stackable!

					 statusText+=" [ItemFound="+LootedSuccess+"]";
					 if (LootedSuccess)
					 {
						  Zeta.CommonBot.GameEvents.FireItemLooted(CurrentTarget.AcdGuid.Value);

						  if (FunkyTrinity.Bot.SettingsFunky.Debug.DebugStatusBar) BotMain.StatusText=statusText;

						  //This is where we should manipulate information of both what dropped and what was looted.
						  Funky.LogItemInformation();

						  //Reset if we reach here..
						  FunkyTrinity.Bot.Combat.reCheckedFinished=false;
						  FunkyTrinity.Bot.Combat.recheckCount=0;
						  FunkyTrinity.Bot.Combat.ShouldCheckItemLooted=false;
						  FunkyTrinity.Bot.Combat.bForceTargetUpdate=true;
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
									 if (FunkyTrinity.Bot.Combat.recheckCount>1)
										  FunkyTrinity.Bot.Combat.reCheckedFinished=true;
									 break;

								case ItemQuality.Rare4:
								case ItemQuality.Rare5:
								case ItemQuality.Rare6:
									 statusText+="=Rare]";
									 if (FunkyTrinity.Bot.Combat.recheckCount>2)
										  FunkyTrinity.Bot.Combat.reCheckedFinished=true;
									 //else
									 //bItemForcedMovement = true;

									 break;

								case ItemQuality.Legendary:
									 statusText+="=Legendary]";
									 if (FunkyTrinity.Bot.Combat.recheckCount>3)
										  FunkyTrinity.Bot.Combat.reCheckedFinished=true;
									 //else
									 //bItemForcedMovement = true;

									 break;
						  }
						  #endregion

						  //If we are still rechecking then use the waitAfter (powerprime Ability related) to wait a few loops.
						  if (!FunkyTrinity.Bot.Combat.reCheckedFinished)
						  {
								statusText+=" RECHECKING";
								if (FunkyTrinity.Bot.SettingsFunky.Debug.DebugStatusBar)
								{
									 BotMain.StatusText=statusText;
								}
								FunkyTrinity.Bot.Combat.bWaitingAfterPower=true;
								FunkyTrinity.Bot.Combat.powerPrime.WaitLoopsAfter=3;
								CurrentState=RunStatus.Running;
								return false;
						  }
						  else
						  {
								//We Rechecked Max Confirmation Checking Count, now we check if we want to retry confirmation, or simply try once more then ignore for a few.
								bool stackableItem=(ItemType.Potion|ItemType.CraftingPage|ItemType.CraftingPlan|ItemType.CraftingReagent).HasFlag(thisObjItem.BalanceData.thisItemType);
								if (thisObjItem.Itemquality.Value>ItemQuality.Magic3||stackableItem)
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
								FunkyTrinity.Bot.Combat.bForceTargetUpdate=true;
						  }
					 }

					 //Reset flag, and continue..
					 FunkyTrinity.Bot.Combat.ShouldCheckItemLooted=false;
				}
				#endregion


				// See if we have been "newly rooted", to force target updates
				if (FunkyTrinity.Bot.Character.bIsRooted&&!FunkyTrinity.Bot.Combat.bWasRootedLastTick)
				{
					 FunkyTrinity.Bot.Combat.bWasRootedLastTick=true;
					 FunkyTrinity.Bot.Combat.bForceTargetUpdate=true;
				}

				if (!FunkyTrinity.Bot.Character.bIsRooted) FunkyTrinity.Bot.Combat.bWasRootedLastTick=false;

				return true;
		  }

		  //This is the 2nd step in handling.. we recheck target, get a new Ability if needed, and check potion/special movement avoidance here.
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

				if (!FunkyTrinity.Bot.Combat.bWholeNewTarget&&!FunkyTrinity.Bot.Combat.bWaitingForPower&&!FunkyTrinity.Bot.Combat.bWaitingForPotion)
				{
					 // Update targets at least once every 80 milliseconds
					 if (FunkyTrinity.Bot.Combat.bForceTargetUpdate
						 ||FunkyTrinity.Bot.Combat.TravellingAvoidance
						 ||((DateTime.Now.Subtract(FunkyTrinity.Bot.Refresh.lastRefreshedObjects).TotalMilliseconds>=80&&CurrentTarget.targetType.Value!=TargetType.Avoidance)
						 ||DateTime.Now.Subtract(FunkyTrinity.Bot.Refresh.lastRefreshedObjects).TotalMilliseconds>=1200))
					 {
						  bShouldRefreshDiaObjects=true;
					 }

					 // If we AREN'T getting new targets - find out if we SHOULD because the current unit has died etc.
					 if (!bShouldRefreshDiaObjects&&CurrentTarget.targetType.Value==TargetType.Unit&&!CurrentTarget.IsStillValid())
						  bShouldRefreshDiaObjects=true;

				}

				// So, after all that, do we actually want a new target list?
				if (!FunkyTrinity.Bot.Combat.bWholeNewTarget&&!FunkyTrinity.Bot.Combat.bWaitingForPower&&!FunkyTrinity.Bot.Combat.bWaitingForPotion)
				{
					 // If we *DO* want a new target list, do this... 
					 if (bShouldRefreshDiaObjects)
					 {
						  // Now call the function that refreshes targets
						  FunkyTrinity.Bot.Refresh.RefreshDiaObjects();

						  // No target, return success
						  if (CurrentTarget==null)
						  {
								CurrentState=RunStatus.Success;
								return false;
						  }
						  else if (FunkyTrinity.Bot.Character.LastCachedTarget!=null&&
								FunkyTrinity.Bot.Character.LastCachedTarget.RAGUID!=CurrentTarget.RAGUID&&CurrentTarget.targetType.Value==TargetType.Item)
						  {
								//Reset Item Vars
								FunkyTrinity.Bot.Combat.recheckCount=0;
								FunkyTrinity.Bot.Combat.reCheckedFinished=false;
						  }

						  // Been trying to handle the same target for more than 30 seconds without damaging/reaching it? Blacklist it!
						  // Note: The time since target picked updates every time the current target loses health, if it's a monster-target
						  if (CurrentTarget.targetType.Value!=TargetType.Avoidance
								&&((CurrentTarget.targetType.Value!=TargetType.Unit&&DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.dateSincePickedTarget).TotalSeconds>12)
								||(CurrentTarget.targetType.Value==TargetType.Unit&&!CurrentTarget.IsBoss&&DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.dateSincePickedTarget).TotalSeconds>40)))
						  {
								// NOTE: This only blacklists if it's remained the PRIMARY TARGET that we are trying to actually directly attack!
								// So it won't blacklist a monster "on the edge of the screen" who isn't even being targetted
								// Don't blacklist monsters on <= 50% health though, as they can't be in a stuck location... can they!? Maybe give them some extra time!
								bool bBlacklistThis=true;
								// PREVENT blacklisting a monster on less than 90% health unless we haven't damaged it for more than 2 minutes
								if (CurrentTarget.targetType.Value==TargetType.Unit)
								{
									 if (CurrentTarget.IsTreasureGoblin&&FunkyTrinity.Bot.SettingsFunky.Targeting.GoblinPriority>=3) bBlacklistThis=false;
									 if (DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.dateSincePickedTarget).TotalSeconds<=120) bBlacklistThis=false;
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
						  FunkyTrinity.Bot.Combat.bPickNewAbilities=true;

						  TargetMovement.NewTargetResetVars();
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
				FunkyTrinity.Bot.Combat.bWholeNewTarget=false;


				//Health Change Timer
				if (CurrentTarget.targetType.Value==TargetType.Unit)
				{
					 //Update CurrentUnitTarget Variable.
					 if (CurrentUnitTarget==null) CurrentUnitTarget=(CacheUnit)CurrentTarget;

					 

					 //double HealthChangeMS=DateTime.Now.Subtract(FunkyTrinity.Bot.Combat.LastHealthChange).TotalMilliseconds;

					 //if (HealthChangeMS>3000&&!CurrentTarget.ObjectIsSpecial||HealthChangeMS>6000)
					 //{
					 //	 Logger.Write(LogLevel.Target, "Health change has not occured within 3 seconds for unit {0}", CurrentTarget.InternalName);
					 //	 Bot.Combat.bForceTargetUpdate=true;
					 //	 CurrentState=RunStatus.Running;
					 //	 CurrentTarget.BlacklistLoops=10;
					 //	 return false;
					 //}
				}


				//We are ready for the specific object type interaction
				return true;
		  }

		  public virtual bool CombatLogic()
		  {
				// Find a valid Ability if the target is a monster
				#region AbilityPick
				if (FunkyTrinity.Bot.Combat.bPickNewAbilities&&!FunkyTrinity.Bot.Combat.bWaitingForPower&&!FunkyTrinity.Bot.Combat.bWaitingForPotion)
				{
					 FunkyTrinity.Bot.Combat.bPickNewAbilities=false;
					 if (CurrentTarget.targetType.Value==TargetType.Unit&&CurrentTarget.AcdGuid.HasValue)
					 {
						  // Pick an Ability		
						  Ability nextAbility=FunkyTrinity.Bot.Class.AbilitySelector(CurrentUnitTarget);

						  // Did we get default attack?
						  if (nextAbility.Equals(Bot.Class.DefaultAttack)&&!Bot.Class.CanUseDefaultAttack)
						  {
								Logger.Write(LogLevel.Ability, "Failed to find a valid ability to use -- Target: {0}", Bot.Target.CurrentTarget.InternalName);
								FunkyTrinity.Bot.Combat.bForceTargetUpdate=true;
								CurrentState=RunStatus.Running;
								CurrentTarget.BlacklistLoops=10;
								return false;
						  }

						  FunkyTrinity.Bot.Combat.powerPrime=nextAbility;
					 }

					 // Select an Ability for destroying a destructible with in advance
					 if (CurrentTarget.targetType.Value==TargetType.Destructible||CurrentTarget.targetType==TargetType.Barricade)
						  FunkyTrinity.Bot.Combat.powerPrime=FunkyTrinity.Bot.Class.DestructibleAbility();
				}
				#endregion

				// Pop a potion when necessary
				// Note that we force a single-loop pause first, to help potion popping "go off"
				#region PotionCheck
				if (FunkyTrinity.Bot.Character.dCurrentHealthPct<=FunkyTrinity.Bot.EmergencyHealthPotionLimit
					 &&!FunkyTrinity.Bot.Combat.bWaitingForPower&&!FunkyTrinity.Bot.Combat.bWaitingForPotion&&!FunkyTrinity.Bot.Character.bIsIncapacitated&&FunkyTrinity.Bot.Class.AbilityUseTimer(SNOPower.DrinkHealthPotion))
				{
					 FunkyTrinity.Bot.Combat.bWaitingForPotion=true;
					 CurrentState=RunStatus.Running;
					 return false;
				}
				if (FunkyTrinity.Bot.Combat.bWaitingForPotion)
				{
					 FunkyTrinity.Bot.Combat.bWaitingForPotion=false;
					 if (!FunkyTrinity.Bot.Character.bIsIncapacitated&&FunkyTrinity.Bot.Class.AbilityUseTimer(SNOPower.DrinkHealthPotion))
					 {
						  FunkyTrinity.Bot.AttemptToUseHealthPotion();
					 }
				}
				#endregion

				// See if we can use any special buffs etc. while in avoidance
				#region AvoidanceSpecialAbilityCheck
				if (TargetType.Avoidance.HasFlag(CurrentTarget.targetType.Value))
				{
					 Ability movement;
					 if (FunkyTrinity.Bot.Class.FindSpecialMovementPower(out movement))
					 {
						  FunkyTrinity.ability.Ability.SetupAbilityForUse(ref movement);
						  FunkyTrinity.ability.Ability.UsePower(ref movement);
						  movement.SuccessfullyUsed();
					 }
				}

				//if ((TargetType.Gold|TargetType.Globe|TargetType.Gizmo|TargetType.Item).HasFlag(CurrentTarget.targetType.Value))
				//{
				//    Bot.Combat.powerBuff=Bot.Class.AbilitySelector(true, false);
				//    if (Bot.Combat.powerBuff.Power!=SNOPower.None)
				//    {
				//         Bot.Combat.powerBuff.UsePower();
				//         //	ZetaDia.Me.UsePower(Bot.Combat.powerBuff.Power, Bot.Combat.powerBuff.TargetPosition, Bot.Combat.powerBuff.WorldID, Bot.Combat.powerBuff.TargetRAGUID);
				//         Bot.Combat.powerBuff.SuccessfullyUsed();
				//    }
				//}
				#endregion

				return true;
		  }


		  public virtual bool Movement()
		  {
				
				if (CurrentTarget.targetType.Value==TargetType.LineOfSight)
				{

					 //Validate LOS movement
					 if (CurrentTarget.LastLOSMoveResult.HasFlag(Zeta.Navigation.MoveResult.Failed|Zeta.Navigation.MoveResult.UnstuckAttempt|Zeta.Navigation.MoveResult.PathGenerationFailed|Zeta.Navigation.MoveResult.ReachedDestination))
					 {
						  if (Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
								Logger.Write(LogLevel.Movement, "LOS Ended Due to MoveResult {0} for Object {1}",CurrentTarget.LastLOSMoveResult.ToString(), CurrentTarget.InternalName);

						  //CurrentTarget.LOSV3=Vector3.Zero;
						  Bot.NavigationCache.LOSmovementUnit=null;
						  FunkyTrinity.Bot.Combat.bWholeNewTarget=true;
						  return false;
					 }

					 if (CurrentTarget.LastLOSMoveResult.HasFlag(Zeta.Navigation.MoveResult.Moved))
					 {
						  //CurrentState=TargetMovement.TargetMoveTo(CurrentTarget);
						  CurrentState=RunStatus.Running;
						  CurrentTarget.LastLOSMoveResult=Zeta.Navigation.Navigator.MoveTo(CurrentTarget.LOSV3, "LOS Movement", true);
						  return false;
					 }
				}

				// Set current destination to our current target's destination
				TargetMovement.CurrentTargetLocation=CurrentTarget.Position;

				//Check if we are in range for interaction..
				if (CurrentTarget.WithinInteractionRange())
					 return true;
				else
				{//Movement required..
					 CurrentState=TargetMovement.TargetMoveTo(CurrentTarget);
					 return false;
				}
		  }

		  //This is the final step in handling.. here we actually switch to a specific method based upon the target object we are handling.
		  public virtual bool ObjectInteraction()
		  {

				#region DebugInfo
				if (FunkyTrinity.Bot.SettingsFunky.Debug.DebugStatusBar)
				{
					 Funky.sStatusText="[Interact- ";
					 switch (CurrentTarget.targetType.Value)
					 {
						  case TargetType.Avoidance:
								Funky.sStatusText+="Avoid] ";
								break;
						  case TargetType.Unit:
								Funky.sStatusText+="Combat] ";
								break;
						  case TargetType.Item:
						  case TargetType.Gold:
						  case TargetType.Globe:
								Funky.sStatusText+="Pickup] ";
								break;
						  case TargetType.Interactable:
								Funky.sStatusText+="Interact] ";
								break;
						  case TargetType.Container:
								Funky.sStatusText+="Open] ";
								break;
						  case TargetType.Destructible:
						  case TargetType.Barricade:
								Funky.sStatusText+="Destroy] ";
								break;
						  case TargetType.Shrine:
								Funky.sStatusText+="Click] ";
								break;
					 }
					 Funky.sStatusText+="Target="+CurrentTarget.InternalName+" C-Dist="+Math.Round(CurrentTarget.CentreDistance, 2).ToString()+". "+
							 "R-Dist="+Math.Round(CurrentTarget.RadiusDistance, 2).ToString()+". ";

					 if (CurrentTarget.targetType.Value==TargetType.Unit&&FunkyTrinity.Bot.Combat.powerPrime.Power!=SNOPower.None)
						  Funky.sStatusText+="Power="+FunkyTrinity.Bot.Combat.powerPrime.Power.ToString()+" (range "+FunkyTrinity.Bot.Combat.powerPrime.MinimumRange.ToString()+") ";


					 Funky.sStatusText+="Weight="+CurrentTarget.Weight.ToString();
					 BotMain.StatusText=Funky.sStatusText;
					 Funky.bResetStatusText=true;
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
						  FunkyTrinity.Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
						  FunkyTrinity.Bot.Combat.timeCancelledFleeMove=DateTime.Now;
						  CurrentState=RunStatus.Running;
						  break;
				}

				// Now tell Trinity to get a new target!
				FunkyTrinity.Bot.Combat.lastChangedZigZag=DateTime.Today;
				FunkyTrinity.Bot.Combat.vPositionLastZigZagCheck=Vector3.Zero;
				//Bot.Combat.bForceTargetUpdate=true;

				return false;
		  }

		  internal void UpdateStatusText(string Action)
		  {
				Funky.sStatusText=Action+" ";

				Funky.sStatusText+="Target="+CurrentTarget.InternalName+" C-Dist="+Math.Round(CurrentTarget.CentreDistance, 2).ToString()+". "+
					 "R-Dist="+Math.Round(CurrentTarget.RadiusDistance, 2).ToString()+". ";

				if (CurrentTarget.targetType.Value==TargetType.Unit&&FunkyTrinity.Bot.Combat.powerPrime.Power!=SNOPower.None)
					 Funky.sStatusText+="Power="+FunkyTrinity.Bot.Combat.powerPrime.Power.ToString()+" (range "+FunkyTrinity.Bot.Combat.powerPrime.MinimumRange.ToString()+") ";

				Funky.sStatusText+="Weight="+CurrentTarget.Weight.ToString();
				BotMain.StatusText=Funky.sStatusText;
				Funky.bResetStatusText=true;
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