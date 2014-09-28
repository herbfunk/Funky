using System;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Blacklist;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Game.Hero.Skills;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using fBaseXtensions.Navigation;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Targeting
{
	public partial class TargetingClass
	{
		///<summary>
		///This method handles the current object target.
		///</summary>
		public RunStatus HandleThis()
		{
			//Prechecks
			bool Continue = PreChecks();


			//Refresh
			if (!Continue)
				return CurrentState;
			Continue = Refresh();


			//Combat logic
			if (!Continue)
				return CurrentState;
			Continue = CombatLogic();


			//Movement
			if (!Continue)
				return CurrentState;
			Continue = Movement();


			//Interaction
			if (!Continue)
				return CurrentState;
			ObjectInteraction();


			//Return status
			return CurrentState;
		}

		//The current state which is used to return from the handler
		public RunStatus CurrentState { get; set; }

		//Prechecks are things prior to target checks and actual target handling.. This is always called first.
		public virtual bool PreChecks()
		{
			// If we aren't in the game of a world is loading, don't do anything yet
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld)
			{
				CurrentState = RunStatus.Success;
				return false;
			}

			// See if we should update hotbar abilities
			FunkyGame.Hero.Class.SecondaryHotbarBuffPresent();


			// Special pausing *AFTER* using certain powers
			#region PauseCheck
			if (Cache.bWaitingAfterPower && FunkyGame.Hero.Class.PowerPrime.WaitLoopsAfter >= 1)
			{
				if (FunkyGame.Hero.Class.PowerPrime.WaitLoopsAfter >= 1) FunkyGame.Hero.Class.PowerPrime.WaitLoopsAfter--;
				if (FunkyGame.Hero.Class.PowerPrime.WaitLoopsAfter <= 0) Cache.bWaitingAfterPower = false;

				CurrentState = RunStatus.Running;
				return false;
			}
			#endregion

			// Update player-data cache -- Special combat call
			FunkyGame.Hero.Update(true);

			// Check for death / player being dead
			#region DeadCheck
			if (FunkyGame.Hero.dCurrentHealthPct <= 0)
			{
				//Disable OOC IDing behavior if dead!
				//if (ItemIdentifyBehavior.shouldPreformOOCItemIDing) ItemIdentifyBehavior.shouldPreformOOCItemIDing = false;

				CurrentState = RunStatus.Success;
				return false;
			}
			#endregion

			//Herbfunk
			//Confirmation of item looted
			#region ItemLootedConfirmationCheck
			if (Cache.ShouldCheckItemLooted)
			{
				//Reset?
				if (Cache.CurrentTarget == null || Cache.CurrentTarget.targetType.HasValue && Cache.CurrentTarget.targetType.Value != TargetType.Item)
				{
					Cache.ShouldCheckItemLooted = false;
					return false;
				}

				//Vendor Behavior ignore checking (Unless told that we shouldn't from town run manager)
				if (BrainBehavior.IsVendoring && !Cache.IgnoreVendoring)
				{
					CurrentState = RunStatus.Success;
					return false;
				}
				if (FunkyGame.Hero.bIsIncapacitated)
				{
					CurrentState = RunStatus.Running;
					return false;
				}

				//Count each attempt to confirm.
				Cache.recheckCount++;
				string statusText = "[Item Confirmation] Current recheck count " + Cache.recheckCount;

				CacheItem thisCacheItem = (CacheItem)Cache.CurrentTarget;
				bool LootedSuccess = Backpack.ContainsItem(thisCacheItem.BalanceID.Value, thisCacheItem.Itemquality.Value);

				statusText += " [ItemFound=" + LootedSuccess + "]";
				if (LootedSuccess)
				{
					//Logger.DBLog.Info("Item Looted Successfully!");
					GameEvents.FireItemLooted(Cache.CurrentTarget.AcdGuid.Value);

					if (FunkyBaseExtension.Settings.Debugging.DebugStatusBar) BotMain.StatusText = statusText;

					//This is where we should manipulate information of both what dropped and what was looted.
					//Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.LootedItemLog(thisCacheItem);
					PluginItemTypes itemType = ItemFunc.DetermineItemType(thisCacheItem.InternalName, thisCacheItem.BalanceData.thisItemType, thisCacheItem.BalanceData.thisFollowerType);
					PluginBaseItemTypes itembaseType = ItemFunc.DetermineBaseType(itemType);

					if (FunkyGame.CurrentGameStats != null)
						FunkyGame.CurrentGameStats.CurrentProfile.LootTracker.LootedItemLog(itemType, itembaseType, thisCacheItem.Itemquality.Value);

					//Remove item from cache..
					Cache.CurrentTarget.NeedsRemoved = true;

					//Update backpack again!
					Backpack.UpdateItemList();
				}
				else
				{
					CacheItem thisObjItem = (CacheItem)Cache.CurrentTarget;

					statusText += " [Quality";
					//Quality of the item determines the recheck attempts.
					ItemQuality curQuality = thisObjItem.Itemquality.Value;
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
							statusText += "<=Magical]";
							//Non-Quality items get skipped quickly.
							if (Cache.recheckCount > 2)
								Cache.reCheckedFinished = true;
							break;

						case ItemQuality.Rare4:
						case ItemQuality.Rare5:
						case ItemQuality.Rare6:
							statusText += "=Rare]";
							if (Cache.recheckCount > 3)
								Cache.reCheckedFinished = true;
							//else
							//bItemForcedMovement = true;

							break;

						case ItemQuality.Legendary:
							statusText += "=Legendary]";
							if (Cache.recheckCount > 4)
								Cache.reCheckedFinished = true;
							//else
							//bItemForcedMovement = true;

							break;
					}
					#endregion

					//If we are still rechecking then use the waitAfter (powerprime Ability related) to wait a few loops.
					if (!Cache.reCheckedFinished)
					{
						statusText += " RECHECKING";
						if (FunkyBaseExtension.Settings.Debugging.DebugStatusBar)
						{
							BotMain.StatusText = statusText;
						}
						Cache.bWaitingAfterPower = true;
						FunkyGame.Hero.Class.PowerPrime.WaitLoopsAfter = 3;
						CurrentState = RunStatus.Running;
						return false;
					}

					//We Rechecked Max Confirmation Checking Count, now we check if we want to retry confirmation, or simply try once more then ignore for a few.
					bool stackableItem = ItemFunc.DetermineIsStackable(thisObjItem.BalanceData.GetGItemType(thisObjItem.InternalName));
					if (thisObjItem.Itemquality.Value > ItemQuality.Magic3 || stackableItem)
					{
						//Items above rare quality don't get blacklisted, just ignored for a few loops.
						//This will force a movement if stuck.. but 5 loops is only 750ms

						if (!Cache.IgnoreVendoring) //Exclude town run manager checking!
							Cache.CurrentTarget.BlacklistLoops = 5;
					}
					else
					{
						//Blacklist items below rare quality!
						Cache.CurrentTarget.BlacklistFlag = BlacklistType.Temporary;
						Cache.CurrentTarget.NeedsRemoved = true;
					}
				}

				// Now tell Trinity to get a new target!
				Cache.bForceTargetUpdate = true;

				//Reset flag, and continue..
				Cache.ShouldCheckItemLooted = false;
			}
			#endregion


			// See if we have been "newly rooted", to force target updates
			if (FunkyGame.Hero.bIsRooted && !Cache.bWasRootedLastTick)
			{
				Cache.bWasRootedLastTick = true;
				Cache.bForceTargetUpdate = true;
			}

			if (!FunkyGame.Hero.bIsRooted) Cache.bWasRootedLastTick = false;

			return true;
		}

		//This is the 2nd step in handling.. we recheck target, get a new Ability if needed, and check potion/special movement avoidance here.
		public virtual bool Refresh()
		{
			// Make sure we reset unstucker stuff here
			PlayerMover.iTimesReachedStuckPoint = 0;
			PlayerMover.vSafeMovementLocation = Vector3.Zero;
			PlayerMover.timeLastRecordedPosition = DateTime.Now;

			// Let's calculate whether or not we want a new target list...
			#region NewtargetChecks
			// Whether we should refresh the target list or not
			bool bShouldRefreshDiaObjects = false;

			if (!Cache.bWholeNewTarget && !Cache.bWaitingForPower && !Cache.bWaitingForPotion)
			{
				// Update targets at least once every 80 milliseconds
				if (Cache.bForceTargetUpdate
					|| Cache.TravellingAvoidance
					|| ((DateTime.Now.Subtract(Cache.lastRefreshedObjects).TotalMilliseconds >= 80 && !ObjectCache.CheckFlag(Cache.CurrentTarget.targetType.Value, TargetType.AvoidanceMovements | TargetType.NoMovement))
					|| DateTime.Now.Subtract(Cache.lastRefreshedObjects).TotalMilliseconds >= 1200))
				{
					bShouldRefreshDiaObjects = true;
				}

				// If we AREN'T getting new targets - find out if we SHOULD because the current unit has died etc.
				if (!bShouldRefreshDiaObjects && Cache.CurrentTarget.targetType.Value == TargetType.Unit && !Cache.CurrentTarget.IsStillValid())
					bShouldRefreshDiaObjects = true;

			}

			// So, after all that, do we actually want a new target list?
			if (!Cache.bWholeNewTarget && !Cache.bWaitingForPower && !Cache.bWaitingForPotion)
			{
				// If we *DO* want a new target list, do this... 
				if (bShouldRefreshDiaObjects)
				{
					// Now call the function that refreshes targets
					Cache.Refresh();

					// No target, return success
					if (Cache.CurrentTarget == null)
					{
						CurrentState = RunStatus.Success;
						return false;
					}

					// Been trying to handle the same target for more than 30 seconds without damaging/reaching it? Blacklist it!
					// Note: The time since target picked updates every time the current target loses health, if it's a monster-target
					if (!ObjectCache.CheckFlag(Cache.CurrentTarget.targetType.Value, TargetType.AvoidanceMovements | TargetType.NoMovement | TargetType.LineOfSight | TargetType.Backtrack)
						  && ((Cache.CurrentTarget.targetType.Value != TargetType.Unit && DateTime.Now.Subtract(Cache.LastChangeOfTarget).TotalSeconds > 12)
						  || (Cache.CurrentTarget.targetType.Value == TargetType.Unit && !Cache.CurrentTarget.IsBoss && DateTime.Now.Subtract(Cache.LastChangeOfTarget).TotalSeconds > 40)))
					{
						// NOTE: This only blacklists if it's remained the PRIMARY TARGET that we are trying to actually directly attack!
						// So it won't blacklist a monster "on the edge of the screen" who isn't even being targetted
						// Don't blacklist monsters on <= 50% health though, as they can't be in a stuck location... can they!? Maybe give them some extra time!
						bool bBlacklistThis = true;
						// PREVENT blacklisting a monster on less than 90% health unless we haven't damaged it for more than 2 minutes
						if (Cache.CurrentTarget.targetType.Value == TargetType.Unit)
						{
							if (Cache.CurrentTarget.IsTreasureGoblin && FunkyBaseExtension.Settings.Targeting.GoblinPriority >= 3) bBlacklistThis = false;
							if (DateTime.Now.Subtract(Cache.LastChangeOfTarget).TotalSeconds <= 120) bBlacklistThis = false;
						}

						if (bBlacklistThis)
						{
							if (Cache.CurrentTarget.targetType.Value == TargetType.Unit)
							{
								//Logger.DBLog.DebugFormat("[Funky] Blacklisting a monster because of possible stuck issues. Monster="+ObjectData.InternalName+" {"+
								//ObjectData.SNOID.ToString()+"}. Range="+ObjectData.CentreDistance.ToString()+", health %="+ObjectData.CurrentHealthPct.ToString());
							}

							Cache.CurrentTarget.NeedsRemoved = true;
							Cache.CurrentTarget.BlacklistFlag = BlacklistType.Temporary;
						}
					}
					// Make sure we start trying to move again should we need to!
					Cache.bPickNewAbilities = true;

					cMovement.NewTargetResetVars();
				}
				// Ok we didn't want a new target list, should we at least update the position of the current target, if it's a monster?
				else if (Cache.CurrentTarget.targetType.Value == TargetType.Unit && Cache.CurrentTarget.IsStillValid())
				{
					Cache.CurrentTarget.UpdatePosition();
				}
			}
			#endregion

			// This variable just prevents an instant 2-target update after coming here from the main decorator function above
			Cache.bWholeNewTarget = false;


			//Update CurrentUnitTarget
			if (Cache.CurrentTarget.targetType.Value == TargetType.Unit)
			{
				//Update CurrentUnitTarget Variable.
				if (Cache.CurrentUnitTarget == null) Cache.CurrentUnitTarget = (CacheUnit)Cache.CurrentTarget;
			}


			//Make sure we are not incapacitated..
			if (FunkyGame.Hero.bIsIncapacitated)
			{
				CurrentState = RunStatus.Running;
				return false;
			}

			//We are ready for the specific object type interaction
			return true;
		}

		public virtual bool CombatLogic()
		{
			//Check if we can cast any combat buff-type abilities while channeling
			if (FunkyGame.Hero.Class.LastUsedAbility.IsChanneling)
			{
				Skill buff;
				if (FunkyGame.Hero.Class.FindCombatBuffPower(out buff))
				{
					Skill.UsePower(ref buff);
					buff.OnSuccessfullyUsed(false);
				}
			}

			// Find a valid skill
			#region AbilityPick
			if (Cache.bPickNewAbilities && !Cache.bWaitingForPower && !Cache.bWaitingForPotion)
			{
				Cache.bPickNewAbilities = false;
				if (Cache.CurrentTarget.targetType.Value == TargetType.Unit && Cache.CurrentTarget.AcdGuid.HasValue)
				{
					// Pick an Ability		
					Skill nextAbility = FunkyGame.Hero.Class.AbilitySelector(Cache.CurrentUnitTarget);

					// Did we get default attack?
					if (nextAbility.Equals(FunkyGame.Hero.Class.DefaultAttack) && !FunkyGame.Hero.Class.CanUseDefaultAttack && !FunkyBaseExtension.Settings.Combat.AllowDefaultAttackAlways)
					{//TODO:: Fix issue when nothing keeps returning (possibly due to bad ability setup)
						Helpers.Logger.Write(Helpers.LogLevel.Ability, "Default Attack not usable -- Failed to find a valid Ability to use -- Target: {0}", Cache.CurrentTarget.InternalName);
						Cache.bForceTargetUpdate = true;
						CurrentState = RunStatus.Running;
						Cache.CurrentTarget.BlacklistLoops = 10;
						return false;
					}

					FunkyGame.Hero.Class.PowerPrime = nextAbility;
				}

				// Select an Ability for destroying a destructible with in advance
				if (Cache.CurrentTarget.targetType.Value == TargetType.Destructible || Cache.CurrentTarget.targetType == TargetType.Barricade)
				{
					Skill nextAbility = FunkyGame.Hero.Class.DestructibleAbility();
					if (nextAbility.Equals(FunkyGame.Hero.Class.DefaultAttack) && !FunkyGame.Hero.Class.CanUseDefaultAttack && !FunkyBaseExtension.Settings.Combat.AllowDefaultAttackAlways)
					{
						Helpers.Logger.Write(Helpers.LogLevel.Ability, "Default Attack not usable -- Failed to find a valid Ability to use -- Target: {0}", Cache.CurrentTarget.InternalName);
						Cache.bForceTargetUpdate = true;
						CurrentState = RunStatus.Running;
						Cache.CurrentTarget.BlacklistLoops = 10;
						return false;
					}

					FunkyGame.Hero.Class.PowerPrime = FunkyGame.Hero.Class.DestructibleAbility();
				}

				//Interactables (for pre and post waits)
				if (ObjectCache.CheckFlag(Cache.CurrentTarget.targetType.Value, TargetType.Item | TargetType.Interactables | TargetType.Interaction))
				{
					Skill.SetupAbilityForUse(ref Cache.InteractionSkill);
					FunkyGame.Hero.Class.PowerPrime = Cache.InteractionSkill;
				}
			}
			#endregion

			#region PotionCheck
			if (FunkyGame.Hero.dCurrentHealthPct <= FunkyBaseExtension.Settings.Combat.PotionHealthPercent
				 && !Cache.bWaitingForPower
				 && !Cache.bWaitingForPotion
				 && !FunkyGame.Hero.bIsIncapacitated
				 && FunkyGame.Hero.Class.HealthPotionAbility.AbilityUseTimer())
			{
				Cache.bWaitingForPotion = true;
				CurrentState = RunStatus.Running;
				return false;
			}
			if (Cache.bWaitingForPotion)
			{
				Cache.bWaitingForPotion = false;
				if (FunkyGame.Hero.Class.HealthPotionAbility.CheckCustomCombatMethod())
				{

					FunkyGame.Hero.Class.HealthPotionAbility.AttemptToUseHealthPotion();
					CurrentState = RunStatus.Running;
					return false;
				}
			}
			#endregion

			// See if we can use any special buffs etc. while in avoidance
			if (ObjectCache.CheckFlag(Cache.CurrentTarget.targetType.Value, TargetType.Globe | TargetType.AvoidanceMovements))
			{
				Skill buff;
				if (FunkyGame.Hero.Class.FindBuffPower(out buff))
				{
					Skill.UsePower(ref buff);
					buff.OnSuccessfullyUsed();
				}
			}


			return true;
		}

		public virtual bool Movement()
		{
			//Set the target location for the Target Movement class..
			cMovement.CurrentTargetLocation = Cache.CurrentTarget.Position;


			//Instead of using target position we use the navigator pathing as CurrentTargetLocation
			if (ObjectCache.CheckFlag(Cache.CurrentTarget.targetType.Value, TargetType.LineOfSight | TargetType.Backtrack))
			{

				//Navigation.NP.MoveTo(FunkyGame.Navigation.LOSmovementObject.Position, "02 LOS:" + FunkyGame.Navigation.LOSmovementObject.InternalName, true);

				if (Navigation.Navigation.NP.CurrentPath.Count > 0)
				{
					//No more points to navigate..
					if (Navigation.Navigation.NP.CurrentPath.Count == 1 && FunkyGame.Hero.Position.Distance(Navigation.Navigation.NP.CurrentPath.Current) <= Cache.CurrentTarget.Radius)
					{
						Helpers.Logger.Write(LogLevel.LineOfSight, "Ending Line of Sight Movement");
						if (Cache.CurrentTarget.targetType.Value == TargetType.LineOfSight)
						{
							FunkyGame.Navigation.LOSmovementObject = null;
						}
						else
						{
							//Ending backtracking behavior!
							Cache.Backtracking = false;
							Cache.StartingLocation = Vector3.Zero;
						}
					}
					else
					{
						//Skip to next location if within 2.5f distance!
						if (Navigation.Navigation.NP.CurrentPath.Count > 1 && FunkyGame.Hero.Position.Distance2D(Navigation.Navigation.NP.CurrentPath.Current) <= 5f)
						{
							Helpers.Logger.DBLog.Debug("LOS: Skipping to next vector");
							Navigation.Navigation.NP.CurrentPath.Next();
						}

						cMovement.CurrentTargetLocation = Navigation.Navigation.NP.CurrentPath.Current;
					}

					CurrentState = cMovement.TargetMoveTo(Cache.CurrentTarget);
					return false;
				}
			}



			//Check if we are in range for interaction..
			if (Cache.CurrentTarget.WithinInteractionRange()) return true;

			//Movement required..
			CurrentState = cMovement.TargetMoveTo(Cache.CurrentTarget);

			return false;
		}

		//This is the final step in handling.. here we actually switch to a specific method based upon the target object we are handling.
		public virtual bool ObjectInteraction()
		{

			#region DebugInfo
			if (FunkyBaseExtension.Settings.Debugging.DebugStatusBar)
			{
				FunkyGame.sStatusText = "[Interact- ";
				switch (Cache.CurrentTarget.targetType.Value)
				{
					case TargetType.Avoidance:
						FunkyGame.sStatusText += "Avoid] ";
						break;
					case TargetType.Fleeing:
						FunkyGame.sStatusText += "Flee] ";
						break;
					case TargetType.NoMovement:
						FunkyGame.sStatusText += "NoMovement] ";
						break;
					case TargetType.Unit:
						FunkyGame.sStatusText += "Combat] ";
						break;
					case TargetType.Item:
					case TargetType.Gold:
					case TargetType.Globe:
						FunkyGame.sStatusText += "Pickup] ";
						break;
					case TargetType.Interactable:
						FunkyGame.sStatusText += "Interact] ";
						break;
					case TargetType.Container:
						FunkyGame.sStatusText += "Open] ";
						break;
					case TargetType.Destructible:
					case TargetType.Barricade:
						FunkyGame.sStatusText += "Destroy] ";
						break;
					case TargetType.Shrine:
						FunkyGame.sStatusText += "Click] ";
						break;
					case TargetType.LineOfSight:
						FunkyGame.sStatusText += "LOS] ";
						break;
				}
				FunkyGame.sStatusText += "Target=" + Cache.CurrentTarget.InternalName + " C-Dist=" + Math.Round(Cache.CurrentTarget.CentreDistance, 2) + ". " +
						"R-Dist=" + Math.Round(Cache.CurrentTarget.RadiusDistance, 2) + ". ";

				if (Cache.CurrentTarget.targetType.Value == TargetType.Unit && FunkyGame.Hero.Class.PowerPrime.Power != SNOPower.None)
					FunkyGame.sStatusText += "Power=" + FunkyGame.Hero.Class.PowerPrime.Power + " (range " + FunkyGame.Hero.Class.PowerPrime.MinimumRange + ") ";


				FunkyGame.sStatusText += "Weight=" + Cache.CurrentTarget.Weight;
				BotMain.StatusText = FunkyGame.sStatusText;
				FunkyGame.bResetStatusText = true;
			}
			#endregion

			switch (Cache.CurrentTarget.targetType.Value)
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
				case TargetType.CursedShrine:
				case TargetType.CursedChest:
					CurrentState = Cache.CurrentTarget.Interact();
					break;
				case TargetType.AvoidanceMovements:
					CurrentState = RunStatus.Running;
					break;
				case TargetType.Backtrack:
					//Last position.. since we are interacting, we are within range.
					if (Navigation.Navigation.NP.CurrentPath.Count <= 1)
					{
						Cache.Backtracking = false;
						Cache.StartingLocation = Vector3.Zero;
					}
					CurrentState = RunStatus.Running;
					break;
				case TargetType.NoMovement:
					CurrentState = RunStatus.Running;
					break;
				case TargetType.LineOfSight:
					//Last position.. since we are interacting, we are within range.
					//if (Navigation.NP.CurrentPath.Count <= 1)
					//{
					Helpers.Logger.DBLog.InfoFormat("Ending LOS Movement from Interaction");
					Navigation.Navigation.NP.Clear();
					FunkyGame.Navigation.LOSmovementObject = null;
					//}
					CurrentState = RunStatus.Running;
					break;
				case TargetType.Interaction:
					Logger.DBLog.InfoFormat("Interacting with obj {0}", Cache.CurrentTarget.DebugStringSimple);
					Cache.CurrentTarget.ref_DiaObject.Interact();
					Cache.bForceTargetUpdate = true;
					break;
			}

			// Now tell Trinity to get a new target!
			FunkyGame.Navigation.lastChangedZigZag = DateTime.Today;
			FunkyGame.Navigation.vPositionLastZigZagCheck = Vector3.Zero;
			//bForceTargetUpdate=true;

			return false;
		}


	}
}
