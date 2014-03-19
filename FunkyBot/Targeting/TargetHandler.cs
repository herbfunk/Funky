using System;
using FunkyBot.Cache.Objects;
using FunkyBot.DBHandlers;
using FunkyBot.Player.HotBar.Skills;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Movement;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;

namespace FunkyBot.Targeting
{
	public class TargetingHandler
	{
		//Constructor
		public TargetingHandler()
		{
			CurrentState = RunStatus.Running;
		}


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
			Bot.Character.Class.SecondaryHotbarBuffPresent();


			// Special pausing *AFTER* using certain powers
			#region PauseCheck
			if (Bot.Targeting.Cache.bWaitingAfterPower && Bot.Character.Class.PowerPrime.WaitLoopsAfter >= 1)
			{
				if (Bot.Character.Class.PowerPrime.WaitLoopsAfter >= 1) Bot.Character.Class.PowerPrime.WaitLoopsAfter--;
				if (Bot.Character.Class.PowerPrime.WaitLoopsAfter <= 0) Bot.Targeting.Cache.bWaitingAfterPower = false;

				CurrentState = RunStatus.Running;
				return false;
			}
			#endregion

			// Update player-data cache -- Special combat call
			Bot.Character.Data.Update(true);

			// Check for death / player being dead
			#region DeadCheck
			if (Bot.Character.Data.dCurrentHealthPct <= 0)
			{
				//Disable OOC IDing behavior if dead!
				if (ItemIdentifyBehavior.shouldPreformOOCItemIDing) ItemIdentifyBehavior.shouldPreformOOCItemIDing = false;

				CurrentState = RunStatus.Success;
				return false;
			}
			#endregion

			//Herbfunk
			//Confirmation of item looted
			#region ItemLootedConfirmationCheck
			if (Bot.Targeting.Cache.ShouldCheckItemLooted)
			{
				//Reset?
				if (Bot.Targeting.Cache.CurrentTarget == null || Bot.Targeting.Cache.CurrentTarget.targetType.HasValue && Bot.Targeting.Cache.CurrentTarget.targetType.Value != TargetType.Item)
				{
					Bot.Targeting.Cache.ShouldCheckItemLooted = false;
					return false;
				}

				//Vendor Behavior
				if (BrainBehavior.IsVendoring)
				{
					CurrentState = RunStatus.Success;
					return false;
				}
				if (Bot.Character.Data.bIsIncapacitated)
				{
					CurrentState = RunStatus.Running;
					return false;
				}

				//Count each attempt to confirm.
				Bot.Targeting.Cache.recheckCount++;
				string statusText = "[Item Confirmation] Current recheck count " + Bot.Targeting.Cache.recheckCount;
				bool LootedSuccess = Bot.Character.Data.BackPack.ContainsItem(Bot.Targeting.Cache.CurrentTarget.AcdGuid.Value, Bot.Targeting.Cache.CheckItemLootStackCount);
				//Verify item is non-stackable!

				statusText += " [ItemFound=" + LootedSuccess + "]";
				if (LootedSuccess)
				{
					GameEvents.FireItemLooted(Bot.Targeting.Cache.CurrentTarget.AcdGuid.Value);

					if (Bot.Settings.Debug.DebugStatusBar) BotMain.StatusText = statusText;

					//This is where we should manipulate information of both what dropped and what was looted.
					Logger.LogItemInformation();

					//Reset if we reach here..
					Bot.Targeting.Cache.reCheckedFinished = false;
					Bot.Targeting.Cache.recheckCount = 0;
					Bot.Targeting.Cache.CheckItemLootStackCount = 0;
					Bot.Targeting.Cache.ShouldCheckItemLooted = false;
					Bot.Targeting.Cache.bForceTargetUpdate = true;

					//Remove..
					Bot.Targeting.Cache.CurrentTarget.NeedsRemoved = true;
				}
				else
				{
					CacheItem thisObjItem = (CacheItem)Bot.Targeting.Cache.CurrentTarget;

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
							if (Bot.Targeting.Cache.recheckCount > 1)
								Bot.Targeting.Cache.reCheckedFinished = true;
							break;

						case ItemQuality.Rare4:
						case ItemQuality.Rare5:
						case ItemQuality.Rare6:
							statusText += "=Rare]";
							if (Bot.Targeting.Cache.recheckCount > 2)
								Bot.Targeting.Cache.reCheckedFinished = true;
							//else
							//bItemForcedMovement = true;

							break;

						case ItemQuality.Legendary:
							statusText += "=Legendary]";
							if (Bot.Targeting.Cache.recheckCount > 3)
								Bot.Targeting.Cache.reCheckedFinished = true;
							//else
							//bItemForcedMovement = true;

							break;
					}
					#endregion

					//If we are still rechecking then use the waitAfter (powerprime Ability related) to wait a few loops.
					if (!Bot.Targeting.Cache.reCheckedFinished)
					{
						statusText += " RECHECKING";
						if (Bot.Settings.Debug.DebugStatusBar)
						{
							BotMain.StatusText = statusText;
						}
						Bot.Targeting.Cache.bWaitingAfterPower = true;
						Bot.Character.Class.PowerPrime.WaitLoopsAfter = 3;
						CurrentState = RunStatus.Running;
						return false;
					}
					//We Rechecked Max Confirmation Checking Count, now we check if we want to retry confirmation, or simply try once more then ignore for a few.
					bool stackableItem = (ItemType.Potion | ItemType.CraftingPage | ItemType.CraftingPlan | ItemType.CraftingReagent).HasFlag(thisObjItem.BalanceData.thisItemType);
					if (thisObjItem.Itemquality.Value > ItemQuality.Magic3 || stackableItem)
					{
						//Items above rare quality don't get blacklisted, just ignored for a few loops.
						//This will force a movement if stuck.. but 5 loops is only 750ms
						Bot.Targeting.Cache.CurrentTarget.BlacklistLoops = 5;
					}
					else
					{
						//Blacklist items below rare quality!
						Bot.Targeting.Cache.CurrentTarget.BlacklistFlag = BlacklistType.Temporary;
						Bot.Targeting.Cache.CurrentTarget.NeedsRemoved = true;
					}

					// Now tell Trinity to get a new target!
					Bot.Targeting.Cache.bForceTargetUpdate = true;
				}

				//Reset flag, and continue..
				Bot.Targeting.Cache.ShouldCheckItemLooted = false;
			}
			#endregion


			// See if we have been "newly rooted", to force target updates
			if (Bot.Character.Data.bIsRooted && !Bot.Targeting.Cache.bWasRootedLastTick)
			{
				Bot.Targeting.Cache.bWasRootedLastTick = true;
				Bot.Targeting.Cache.bForceTargetUpdate = true;
			}

			if (!Bot.Character.Data.bIsRooted) Bot.Targeting.Cache.bWasRootedLastTick = false;

			return true;
		}

		//This is the 2nd step in handling.. we recheck target, get a new Ability if needed, and check potion/special movement avoidance here.
		public virtual bool Refresh()
		{
			// Make sure we reset unstucker stuff here
			Funky.PlayerMover.iTimesReachedStuckPoint = 0;
			Funky.PlayerMover.vSafeMovementLocation = Vector3.Zero;
			Funky.PlayerMover.timeLastRecordedPosition = DateTime.Now;

			// Let's calculate whether or not we want a new target list...
			#region NewtargetChecks
			// Whether we should refresh the target list or not
			bool bShouldRefreshDiaObjects = false;

			if (!Bot.Targeting.Cache.bWholeNewTarget && !Bot.Targeting.Cache.bWaitingForPower && !Bot.Targeting.Cache.bWaitingForPotion)
			{
				// Update targets at least once every 80 milliseconds
				if (Bot.Targeting.Cache.bForceTargetUpdate
					|| Bot.Targeting.Cache.TravellingAvoidance
					|| ((DateTime.Now.Subtract(Bot.Targeting.Cache.lastRefreshedObjects).TotalMilliseconds >= 80 && !ObjectCache.CheckTargetTypeFlag(Bot.Targeting.Cache.CurrentTarget.targetType.Value, TargetType.AvoidanceMovements | TargetType.NoMovement))
					|| DateTime.Now.Subtract(Bot.Targeting.Cache.lastRefreshedObjects).TotalMilliseconds >= 1200))
				{
					bShouldRefreshDiaObjects = true;
				}

				// If we AREN'T getting new targets - find out if we SHOULD because the current unit has died etc.
				if (!bShouldRefreshDiaObjects && Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Unit && !Bot.Targeting.Cache.CurrentTarget.IsStillValid())
					bShouldRefreshDiaObjects = true;

			}

			// So, after all that, do we actually want a new target list?
			if (!Bot.Targeting.Cache.bWholeNewTarget && !Bot.Targeting.Cache.bWaitingForPower && !Bot.Targeting.Cache.bWaitingForPotion)
			{
				// If we *DO* want a new target list, do this... 
				if (bShouldRefreshDiaObjects)
				{
					// Now call the function that refreshes targets
					Bot.Targeting.Cache.Refresh();

					// No target, return success
					if (Bot.Targeting.Cache.CurrentTarget == null)
					{
						CurrentState = RunStatus.Success;
						return false;
					}
					else if (Bot.Targeting.Cache.LastCachedTarget != null &&
						  Bot.Targeting.Cache.LastCachedTarget.RAGUID != Bot.Targeting.Cache.CurrentTarget.RAGUID && Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Item)
					{
						//Reset Item Vars
						Bot.Targeting.Cache.recheckCount = 0;
						Bot.Targeting.Cache.reCheckedFinished = false;
						Bot.Targeting.Cache.CheckItemLootStackCount = 0;
					}

					// Been trying to handle the same target for more than 30 seconds without damaging/reaching it? Blacklist it!
					// Note: The time since target picked updates every time the current target loses health, if it's a monster-target
					if (!ObjectCache.CheckTargetTypeFlag(Bot.Targeting.Cache.CurrentTarget.targetType.Value, TargetType.AvoidanceMovements | TargetType.NoMovement | TargetType.LineOfSight | TargetType.Backtrack)
						  && ((Bot.Targeting.Cache.CurrentTarget.targetType.Value != TargetType.Unit && DateTime.Now.Subtract(Bot.Targeting.Cache.LastChangeOfTarget).TotalSeconds > 12)
						  || (Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Unit && !Bot.Targeting.Cache.CurrentTarget.IsBoss && DateTime.Now.Subtract(Bot.Targeting.Cache.LastChangeOfTarget).TotalSeconds > 40)))
					{
						// NOTE: This only blacklists if it's remained the PRIMARY TARGET that we are trying to actually directly attack!
						// So it won't blacklist a monster "on the edge of the screen" who isn't even being targetted
						// Don't blacklist monsters on <= 50% health though, as they can't be in a stuck location... can they!? Maybe give them some extra time!
						bool bBlacklistThis = true;
						// PREVENT blacklisting a monster on less than 90% health unless we haven't damaged it for more than 2 minutes
						if (Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Unit)
						{
							if (Bot.Targeting.Cache.CurrentTarget.IsTreasureGoblin && Bot.Settings.Targeting.GoblinPriority >= 3) bBlacklistThis = false;
							if (DateTime.Now.Subtract(Bot.Targeting.Cache.LastChangeOfTarget).TotalSeconds <= 120) bBlacklistThis = false;
						}

						if (bBlacklistThis)
						{
							if (Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Unit)
							{
								//Logger.DBLog.DebugFormat("[Funky] Blacklisting a monster because of possible stuck issues. Monster="+ObjectData.InternalName+" {"+
								//ObjectData.SNOID.ToString()+"}. Range="+ObjectData.CentreDistance.ToString()+", health %="+ObjectData.CurrentHealthPct.ToString());
							}

							Bot.Targeting.Cache.CurrentTarget.NeedsRemoved = true;
							Bot.Targeting.Cache.CurrentTarget.BlacklistFlag = BlacklistType.Temporary;
						}
					}
					// Make sure we start trying to move again should we need to!
					Bot.Targeting.Cache.bPickNewAbilities = true;

					Bot.Targeting.Movement.NewTargetResetVars();
				}
				// Ok we didn't want a new target list, should we at least update the position of the current target, if it's a monster?
				else if (Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Unit && Bot.Targeting.Cache.CurrentTarget.IsStillValid())
				{
					Bot.Targeting.Cache.CurrentTarget.UpdatePosition();
				}
			}
			#endregion

			// This variable just prevents an instant 2-target update after coming here from the main decorator function above
			Bot.Targeting.Cache.bWholeNewTarget = false;


			//Update CurrentUnitTarget
			if (Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Unit)
			{
				//Update CurrentUnitTarget Variable.
				if (Bot.Targeting.Cache.CurrentUnitTarget == null) Bot.Targeting.Cache.CurrentUnitTarget = (CacheUnit)Bot.Targeting.Cache.CurrentTarget;
			}


			//Make sure we are not incapacitated..
			if (Bot.Character.Data.bIsIncapacitated)
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
			if (Bot.Character.Class.LastUsedAbility.IsChanneling)
			{
				Skill buff;
				if (Bot.Character.Class.FindCombatBuffPower(out buff))
				{
					Skill.UsePower(ref buff);
					buff.OnSuccessfullyUsed(false);
				}
			}

			// Find a valid Ability if the target is a monster
			#region AbilityPick
			if (Bot.Targeting.Cache.bPickNewAbilities && !Bot.Targeting.Cache.bWaitingForPower && !Bot.Targeting.Cache.bWaitingForPotion)
			{
				Bot.Targeting.Cache.bPickNewAbilities = false;
				if (Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Unit && Bot.Targeting.Cache.CurrentTarget.AcdGuid.HasValue)
				{
					// Pick an Ability		
					Skill nextAbility = Bot.Character.Class.AbilitySelector(Bot.Targeting.Cache.CurrentUnitTarget);

					// Did we get default attack?
					if (nextAbility.Equals(Bot.Character.Class.DefaultAttack) && !Bot.Character.Class.CanUseDefaultAttack && !Bot.Settings.Class.AllowDefaultAttackAlways)
					{//TODO:: Fix issue when nothing keeps returning (possibly due to bad ability setup)
						Logger.Write(LogLevel.Ability, "Default Attack not usable -- Failed to find a valid Ability to use -- Target: {0}", Bot.Targeting.Cache.CurrentTarget.InternalName);
						Bot.Targeting.Cache.bForceTargetUpdate = true;
						CurrentState = RunStatus.Running;
						Bot.Targeting.Cache.CurrentTarget.BlacklistLoops = 10;
						return false;
					}

					Bot.Character.Class.PowerPrime = nextAbility;
				}

				// Select an Ability for destroying a destructible with in advance
				if (Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Destructible || Bot.Targeting.Cache.CurrentTarget.targetType == TargetType.Barricade)
					Bot.Character.Class.PowerPrime = Bot.Character.Class.DestructibleAbility();
			}
			#endregion

			#region PotionCheck
			if (Bot.Character.Data.dCurrentHealthPct <= Bot.Settings.Combat.PotionHealthPercent
				 && !Bot.Targeting.Cache.bWaitingForPower
				 && !Bot.Targeting.Cache.bWaitingForPotion
				 && !Bot.Character.Data.bIsIncapacitated
				 && Bot.Character.Class.HealthPotionAbility.AbilityUseTimer())
			{
				Bot.Targeting.Cache.bWaitingForPotion = true;
				CurrentState = RunStatus.Running;
				return false;
			}
			if (Bot.Targeting.Cache.bWaitingForPotion)
			{
				Bot.Targeting.Cache.bWaitingForPotion = false;
				if (Bot.Character.Class.HealthPotionAbility.CheckCustomCombatMethod())
				{
					Bot.Character.Class.HealthPotionAbility.AttemptToUseHealthPotion();
					CurrentState = RunStatus.Running;
					return false;
				}
			}
			#endregion

			// See if we can use any special buffs etc. while in avoidance
			if (ObjectCache.CheckTargetTypeFlag(Bot.Targeting.Cache.CurrentTarget.targetType.Value, TargetType.Gold | TargetType.Globe | TargetType.AvoidanceMovements | TargetType.NoMovement))
			{
				Skill buff;
				if (Bot.Character.Class.FindBuffPower(out buff))
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
			Bot.Targeting.Movement.CurrentTargetLocation = Bot.Targeting.Cache.CurrentTarget.Position;


			//Instead of using target position we use the navigator pathing as CurrentTargetLocation
			if (ObjectCache.CheckTargetTypeFlag(Bot.Targeting.Cache.CurrentTarget.targetType.Value, TargetType.LineOfSight | TargetType.Backtrack))
			{

				//Navigation.NP.MoveTo(Bot.NavigationCache.LOSmovementObject.Position, "02 LOS:" + Bot.NavigationCache.LOSmovementObject.InternalName, true);

				if (Navigation.NP.CurrentPath.Count > 0)
				{
					//No more points to navigate..
					if (Navigation.NP.CurrentPath.Count == 1 && Bot.Character.Data.Position.Distance(Navigation.NP.CurrentPath.Current) <= Bot.Targeting.Cache.CurrentTarget.Radius)
					{
						Logger.Write(LogLevel.Movement, "Ending Line of Sight Movement");
						if (Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.LineOfSight)
						{
							Bot.NavigationCache.LOSmovementObject = null;
						}
						else
						{
							//Ending backtracking behavior!
							Bot.Targeting.Cache.Backtracking = false;
							Bot.Targeting.Cache.StartingLocation = Vector3.Zero;
						}
					}
					else
					{
						Bot.Targeting.Movement.CurrentTargetLocation = Navigation.NP.CurrentPath.Current;
					}

					CurrentState = Bot.Targeting.Movement.TargetMoveTo(Bot.Targeting.Cache.CurrentTarget);
					return false;
				}
			}



			//Check if we are in range for interaction..
			if (Bot.Targeting.Cache.CurrentTarget.WithinInteractionRange())
				return true;
			//Movement required..
			CurrentState = Bot.Targeting.Movement.TargetMoveTo(Bot.Targeting.Cache.CurrentTarget);
			return false;
		}

		//This is the final step in handling.. here we actually switch to a specific method based upon the target object we are handling.
		public virtual bool ObjectInteraction()
		{

			#region DebugInfo
			if (Bot.Settings.Debug.DebugStatusBar)
			{
				Funky.sStatusText = "[Interact- ";
				switch (Bot.Targeting.Cache.CurrentTarget.targetType.Value)
				{
					case TargetType.Avoidance:
						Funky.sStatusText += "Avoid] ";
						break;
					case TargetType.Fleeing:
						Funky.sStatusText += "Flee] ";
						break;
					case TargetType.NoMovement:
						Funky.sStatusText += "NoMovement] ";
						break;
					case TargetType.Unit:
						Funky.sStatusText += "Combat] ";
						break;
					case TargetType.Item:
					case TargetType.Gold:
					case TargetType.Globe:
						Funky.sStatusText += "Pickup] ";
						break;
					case TargetType.Interactable:
						Funky.sStatusText += "Interact] ";
						break;
					case TargetType.Container:
						Funky.sStatusText += "Open] ";
						break;
					case TargetType.Destructible:
					case TargetType.Barricade:
						Funky.sStatusText += "Destroy] ";
						break;
					case TargetType.Shrine:
						Funky.sStatusText += "Click] ";
						break;
					case TargetType.LineOfSight:
						Funky.sStatusText += "LOS] ";
						break;
				}
				Funky.sStatusText += "Target=" + Bot.Targeting.Cache.CurrentTarget.InternalName + " C-Dist=" + Math.Round(Bot.Targeting.Cache.CurrentTarget.CentreDistance, 2) + ". " +
						"R-Dist=" + Math.Round(Bot.Targeting.Cache.CurrentTarget.RadiusDistance, 2) + ". ";

				if (Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Unit && Bot.Character.Class.PowerPrime.Power != SNOPower.None)
					Funky.sStatusText += "Power=" + Bot.Character.Class.PowerPrime.Power + " (range " + Bot.Character.Class.PowerPrime.MinimumRange + ") ";


				Funky.sStatusText += "Weight=" + Bot.Targeting.Cache.CurrentTarget.Weight;
				BotMain.StatusText = Funky.sStatusText;
				Funky.bResetStatusText = true;
			}
			#endregion

			switch (Bot.Targeting.Cache.CurrentTarget.targetType.Value)
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
					CurrentState = Bot.Targeting.Cache.CurrentTarget.Interact();
					break;
				case TargetType.AvoidanceMovements:
					CurrentState = RunStatus.Running;
					break;
				case TargetType.Backtrack:
					//Last position.. since we are interacting, we are within range.
					if (Navigation.NP.CurrentPath.Count <= 1)
					{
						Bot.Targeting.Cache.Backtracking = false;
						Bot.Targeting.Cache.StartingLocation = Vector3.Zero;
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
					Logger.DBLog.InfoFormat("Ending LOS Movement from Interaction");
					Navigation.NP.Clear();
					Bot.NavigationCache.LOSmovementObject = null;
					//}
					CurrentState = RunStatus.Running;
					break;
			}

			// Now tell Trinity to get a new target!
			Bot.NavigationCache.lastChangedZigZag = DateTime.Today;
			Bot.NavigationCache.vPositionLastZigZagCheck = Vector3.Zero;
			//Bot.Targeting.bForceTargetUpdate=true;

			return false;
		}

		internal void UpdateStatusText(string Action)
		{
			Funky.sStatusText = Action + " ";

			Funky.sStatusText += "Target=" + Bot.Targeting.Cache.CurrentTarget.InternalName + " C-Dist=" + Math.Round(Bot.Targeting.Cache.CurrentTarget.CentreDistance, 2) + ". " +
				 "R-Dist=" + Math.Round(Bot.Targeting.Cache.CurrentTarget.RadiusDistance, 2) + ". ";

			if (Bot.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Unit && Bot.Character.Class.PowerPrime.Power != SNOPower.None)
				Funky.sStatusText += "Power=" + Bot.Character.Class.PowerPrime.Power + " (range " + Bot.Character.Class.PowerPrime.MinimumRange + ") ";

			Funky.sStatusText += "Weight=" + Bot.Targeting.Cache.CurrentTarget.Weight;
			BotMain.StatusText = Funky.sStatusText;
			Funky.bResetStatusText = true;
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null)
			{
				if (Bot.Targeting.Cache.CurrentTarget != null)
					return false;
				return true;
			}
			Type ta = obj.GetType();
			Type tb = Bot.Targeting.Cache.CurrentTarget != null ? Bot.Targeting.Cache.CurrentTarget.GetType() : GetType();

			if (ta.Equals(tb))
			{
				return ((CacheObject)obj) == (Bot.Targeting.Cache.CurrentTarget);
			}
			return false;
		}
		public override int GetHashCode()
		{
			return Bot.Targeting.Cache.CurrentTarget != null ? Bot.Targeting.Cache.CurrentTarget.GetHashCode() : base.GetHashCode();
		}


	}

}