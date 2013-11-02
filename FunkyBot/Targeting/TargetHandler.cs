using System;
using System.Linq;
using FunkyBot.AbilityFunky;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Movement;
using FunkyBot.Movement.Clustering;
using FunkyBot.Targeting.Behaviors;
using JetBrains.Annotations;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Logic;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;
using Zeta.Navigation;
using Zeta.TreeSharp;

namespace FunkyBot.Targeting
{
	 public partial class TargetingHandler
	 {
		  //Constructor
		  public TargetingHandler()
		  {
				CurrentState=RunStatus.Running;
				CurrentTarget=null;
				Bot.Character.OnLevelAreaIDChanged+=this.LevelAreaIDChangeHandler;
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
				Bot.Class.SecondaryHotbarBuffPresent();


				// Special pausing *AFTER* using certain powers
				#region PauseCheck
				if (Bot.Targeting.bWaitingAfterPower&&Bot.Class.PowerPrime.WaitLoopsAfter>=1)
				{
					 if (Bot.Class.PowerPrime.WaitLoopsAfter>=1) Bot.Class.PowerPrime.WaitLoopsAfter--;
					 if (Bot.Class.PowerPrime.WaitLoopsAfter<=0) Bot.Targeting.bWaitingAfterPower=false;

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
					 if (Funky.shouldPreformOOCItemIDing) Funky.shouldPreformOOCItemIDing=false;

					 CurrentState=RunStatus.Success;
					 return false;
				}
				#endregion

				//Herbfunk
				//Confirmation of item looted
				#region ItemLootedConfirmationCheck
				if (Bot.Targeting.ShouldCheckItemLooted)
				{
					 //Reset?
					 if (CurrentTarget==null||CurrentTarget.targetType.HasValue&&CurrentTarget.targetType.Value!=TargetType.Item)
					 {
						  Bot.Targeting.ShouldCheckItemLooted=false;
						  return false;
					 }

					 //Vendor Behavior
					 if (BrainBehavior.IsVendoring)
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
					 Bot.Targeting.recheckCount++;
					 string statusText="[Item Confirmation] Current recheck count "+Bot.Targeting.recheckCount;
					 bool LootedSuccess=Bot.Character.BackPack.ContainsItem(CurrentTarget.AcdGuid.Value);
					 //Verify item is non-stackable!

					 statusText+=" [ItemFound="+LootedSuccess+"]";
					 if (LootedSuccess)
					 {
						  GameEvents.FireItemLooted(CurrentTarget.AcdGuid.Value);

						  if (Bot.Settings.Debug.DebugStatusBar) BotMain.StatusText=statusText;

						  //This is where we should manipulate information of both what dropped and what was looted.
						  Funky.LogItemInformation();

						  //Reset if we reach here..
						  Bot.Targeting.reCheckedFinished=false;
						  Bot.Targeting.recheckCount=0;
						  Bot.Targeting.ShouldCheckItemLooted=false;
						  Bot.Targeting.bForceTargetUpdate=true;
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
									 if (Bot.Targeting.recheckCount>1)
										  Bot.Targeting.reCheckedFinished=true;
									 break;

								case ItemQuality.Rare4:
								case ItemQuality.Rare5:
								case ItemQuality.Rare6:
									 statusText+="=Rare]";
									 if (Bot.Targeting.recheckCount>2)
										  Bot.Targeting.reCheckedFinished=true;
									 //else
									 //bItemForcedMovement = true;

									 break;

								case ItemQuality.Legendary:
									 statusText+="=Legendary]";
									 if (Bot.Targeting.recheckCount>3)
										  Bot.Targeting.reCheckedFinished=true;
									 //else
									 //bItemForcedMovement = true;

									 break;
						  }
						  #endregion

						  //If we are still rechecking then use the waitAfter (powerprime Ability related) to wait a few loops.
						  if (!Bot.Targeting.reCheckedFinished)
						  {
								statusText+=" RECHECKING";
								if (Bot.Settings.Debug.DebugStatusBar)
								{
									 BotMain.StatusText=statusText;
								}
								Bot.Targeting.bWaitingAfterPower=true;
								Bot.Class.PowerPrime.WaitLoopsAfter=3;
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
								Bot.Targeting.bForceTargetUpdate=true;
						  }
					 }

					 //Reset flag, and continue..
					 Bot.Targeting.ShouldCheckItemLooted=false;
				}
				#endregion


				// See if we have been "newly rooted", to force target updates
				if (Bot.Character.bIsRooted&&!Bot.Targeting.bWasRootedLastTick)
				{
					 Bot.Targeting.bWasRootedLastTick=true;
					 Bot.Targeting.bForceTargetUpdate=true;
				}

				if (!Bot.Character.bIsRooted) Bot.Targeting.bWasRootedLastTick=false;

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

				if (!Bot.Targeting.bWholeNewTarget&&!Bot.Targeting.bWaitingForPower&&!Bot.Targeting.bWaitingForPotion)
				{
					 // Update targets at least once every 80 milliseconds
					 if (Bot.Targeting.bForceTargetUpdate
						 ||Bot.Targeting.TravellingAvoidance
						 ||((DateTime.Now.Subtract(Bot.Targeting.lastRefreshedObjects).TotalMilliseconds>=80&&!ObjectCache.CheckTargetTypeFlag(CurrentTarget.targetType.Value, TargetType.AvoidanceMovements|TargetType.NoMovement))
						 ||DateTime.Now.Subtract(Bot.Targeting.lastRefreshedObjects).TotalMilliseconds>=1200))
					 {
						  bShouldRefreshDiaObjects=true;
					 }

					 // If we AREN'T getting new targets - find out if we SHOULD because the current unit has died etc.
					 if (!bShouldRefreshDiaObjects&&CurrentTarget.targetType.Value==TargetType.Unit&&!CurrentTarget.IsStillValid())
						  bShouldRefreshDiaObjects=true;

				}

				// So, after all that, do we actually want a new target list?
				if (!Bot.Targeting.bWholeNewTarget&&!Bot.Targeting.bWaitingForPower&&!Bot.Targeting.bWaitingForPotion)
				{
					 // If we *DO* want a new target list, do this... 
					 if (bShouldRefreshDiaObjects)
					 {
						  // Now call the function that refreshes targets
						  Bot.Targeting.RefreshDiaObjects();

						  // No target, return success
						  if (CurrentTarget==null)
						  {
								CurrentState=RunStatus.Success;
								return false;
						  }
						  else if (LastCachedTarget!=null&&
								LastCachedTarget.RAGUID!=CurrentTarget.RAGUID&&CurrentTarget.targetType.Value==TargetType.Item)
						  {
								//Reset Item Vars
								Bot.Targeting.recheckCount=0;
								Bot.Targeting.reCheckedFinished=false;
						  }

						  // Been trying to handle the same target for more than 30 seconds without damaging/reaching it? Blacklist it!
						  // Note: The time since target picked updates every time the current target loses health, if it's a monster-target
						  if (!ObjectCache.CheckTargetTypeFlag(CurrentTarget.targetType.Value,TargetType.AvoidanceMovements| TargetType.NoMovement| TargetType.LineOfSight)
								&&((CurrentTarget.targetType.Value!=TargetType.Unit&&DateTime.Now.Subtract(Bot.Targeting.LastChangeOfTarget).TotalSeconds>12)
								||(CurrentTarget.targetType.Value==TargetType.Unit&&!CurrentTarget.IsBoss&&DateTime.Now.Subtract(Bot.Targeting.LastChangeOfTarget).TotalSeconds>40)))
						  {
								// NOTE: This only blacklists if it's remained the PRIMARY TARGET that we are trying to actually directly attack!
								// So it won't blacklist a monster "on the edge of the screen" who isn't even being targetted
								// Don't blacklist monsters on <= 50% health though, as they can't be in a stuck location... can they!? Maybe give them some extra time!
								bool bBlacklistThis=true;
								// PREVENT blacklisting a monster on less than 90% health unless we haven't damaged it for more than 2 minutes
								if (CurrentTarget.targetType.Value==TargetType.Unit)
								{
									 if (CurrentTarget.IsTreasureGoblin&&Bot.Settings.Targeting.GoblinPriority>=3) bBlacklistThis=false;
									 if (DateTime.Now.Subtract(Bot.Targeting.LastChangeOfTarget).TotalSeconds<=120) bBlacklistThis=false;
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
						  Bot.Targeting.bPickNewAbilities=true;

						  TargetMovement.NewTargetResetVars();
					 }
					 // Ok we didn't want a new target list, should we at least update the position of the current target, if it's a monster?
					 else if (CurrentTarget.targetType.Value==TargetType.Unit&&CurrentTarget.IsStillValid())
					 {
						  CurrentTarget.UpdatePosition();
					 }
				}
				#endregion

				// This variable just prevents an instant 2-target update after coming here from the main decorator function above
				Bot.Targeting.bWholeNewTarget=false;


				//Update CurrentUnitTarget
				if (CurrentTarget.targetType.Value==TargetType.Unit)
				{
					 //Update CurrentUnitTarget Variable.
					 if (CurrentUnitTarget==null) CurrentUnitTarget=(CacheUnit)CurrentTarget;
				}


                //Make sure we are not incapacitated..
              if (Bot.Character.bIsIncapacitated)
              {
                  CurrentState = RunStatus.Running;
                  return false;
              }

				//We are ready for the specific object type interaction
				return true;
		  }

         //TODO:: Add wait during Incapitated!

		  public virtual bool CombatLogic()
		  {
				//Check if we can cast any combat buff-type abilities while channeling
				if (Bot.Class.LastUsedAbility.IsChanneling)
				{
					 Ability buff;
					 if (Bot.Class.FindCombatBuffPower(out buff))
					 {
						  Ability.UsePower(ref buff);
						  buff.OnSuccessfullyUsed(false);
					 }
				}

				// Find a valid Ability if the target is a monster
				#region AbilityPick
				if (Bot.Targeting.bPickNewAbilities&&!Bot.Targeting.bWaitingForPower&&!Bot.Targeting.bWaitingForPotion)
				{
					 Bot.Targeting.bPickNewAbilities=false;
					 if (CurrentTarget.targetType.Value==TargetType.Unit&&CurrentTarget.AcdGuid.HasValue)
					 {
						  // Pick an Ability		
						  Ability nextAbility=Bot.Class.AbilitySelector(CurrentUnitTarget);

						  // Did we get default attack?
						  if (nextAbility.Equals(Bot.Class.DefaultAttack)&&!Bot.Class.CanUseDefaultAttack)
						  {
								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Ability))
									 Logger.Write(LogLevel.Ability, "Failed to find a valid Ability to use -- Target: {0}", Bot.Targeting.CurrentTarget.InternalName);
								Bot.Targeting.bForceTargetUpdate=true;
								CurrentState=RunStatus.Running;
								CurrentTarget.BlacklistLoops=10;
								return false;
						  }

						  Bot.Class.PowerPrime=nextAbility;
					 }

					 // Select an Ability for destroying a destructible with in advance
					 if (CurrentTarget.targetType.Value==TargetType.Destructible||CurrentTarget.targetType==TargetType.Barricade)
						  Bot.Class.PowerPrime=Bot.Class.DestructibleAbility();
				}
				#endregion

				#region PotionCheck
				if (Bot.Character.dCurrentHealthPct<=Bot.Settings.Combat.PotionHealthPercent
					 &&!Bot.Targeting.bWaitingForPower
					 &&!Bot.Targeting.bWaitingForPotion
					 &&!Bot.Character.bIsIncapacitated
					 &&Bot.Class.HealthPotionAbility.AbilityUseTimer())
				{
					 Bot.Targeting.bWaitingForPotion=true;
					 CurrentState=RunStatus.Running;
					 return false;
				}
				if (Bot.Targeting.bWaitingForPotion)
				{
					 Bot.Targeting.bWaitingForPotion=false;
					 if (Bot.Class.HealthPotionAbility.CheckCustomCombatMethod())
					 {
						  Bot.Class.HealthPotionAbility.AttemptToUseHealthPotion();
						  CurrentState=RunStatus.Running;
						  return false;
					 }
				}
				#endregion

				// See if we can use any special buffs etc. while in avoidance
				if (ObjectCache.CheckTargetTypeFlag(CurrentTarget.targetType.Value,TargetType.Gold|TargetType.Globe|TargetType.AvoidanceMovements|TargetType.NoMovement))
				{
					  Ability buff;
					 if (Bot.Class.FindBuffPower(out buff))
					 {
						  Ability.UsePower(ref buff);
						  buff.OnSuccessfullyUsed();
					 }
				}


				return true;
		  }


		  public virtual bool Movement()
		  {

				if (CurrentTarget.targetType.Value==TargetType.LineOfSight)
				{
					//Since we only update our path during target refresh.. we should check if we are within range already!
					 if (Navigation.NP.CurrentPath.Count>0&&Bot.Character.Position.Distance(Navigation.NP.CurrentPath.Current)<=Navigation.NP.PathPrecision)
						  Navigation.NP.MoveTo(CurrentTarget.Position, "LineOfSightMoveTo", true);

					 if (Navigation.NP.CurrentPath.Count==0)
					 {
						  Bot.NavigationCache.LOSVector=Vector3.Zero;
						  Bot.NavigationCache.LOSmovementObject=null;
						  this.bForceTargetUpdate=true;
						  return false;
					 }
				}

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
				if (Bot.Settings.Debug.DebugStatusBar)
				{
					 Funky.sStatusText="[Interact- ";
					 switch (CurrentTarget.targetType.Value)
					 {
						  case TargetType.Avoidance:
								Funky.sStatusText+="Avoid] ";
								break;
						  case TargetType.Fleeing:
								Funky.sStatusText+="Flee] ";
								break;
						  case TargetType.NoMovement:
								Funky.sStatusText+="NoMovement] ";
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

					 if (CurrentTarget.targetType.Value==TargetType.Unit&&Bot.Class.PowerPrime.Power!=SNOPower.None)
						  Funky.sStatusText+="Power="+Bot.Class.PowerPrime.Power.ToString()+" (range "+Bot.Class.PowerPrime.MinimumRange.ToString()+") ";


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
					 case TargetType.AvoidanceMovements:
						  CurrentState=RunStatus.Running;
						  break;
					 case TargetType.NoMovement:
					 case TargetType.LineOfSight:
						  CurrentState=RunStatus.Running;
						  break;
				}

				// Now tell Trinity to get a new target!
				Bot.NavigationCache.lastChangedZigZag=DateTime.Today;
				Bot.NavigationCache.vPositionLastZigZagCheck=Vector3.Zero;
				//Bot.Targeting.bForceTargetUpdate=true;

				return false;
		  }

		  internal void UpdateStatusText(string Action)
		  {
				Funky.sStatusText=Action+" ";

				Funky.sStatusText+="Target="+CurrentTarget.InternalName+" C-Dist="+Math.Round(CurrentTarget.CentreDistance, 2).ToString()+". "+
					 "R-Dist="+Math.Round(CurrentTarget.RadiusDistance, 2).ToString()+". ";

				if (CurrentTarget.targetType.Value==TargetType.Unit&&Bot.Class.PowerPrime.Power!=SNOPower.None)
					 Funky.sStatusText+="Power="+Bot.Class.PowerPrime.Power.ToString()+" (range "+Bot.Class.PowerPrime.MinimumRange.ToString()+") ";

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