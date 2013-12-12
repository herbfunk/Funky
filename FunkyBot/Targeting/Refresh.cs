using System;
using System.Linq;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Movement;
using FunkyBot.Targeting.Behaviors;
using Zeta.Common;
using Zeta.CommonBot.Logic;
using Zeta.Navigation;

namespace FunkyBot.Targeting
{
	 public partial class TargetingHandler
	 {
		  #region Target Changed

		  internal class TargetChangedArgs : EventArgs
		  {
				public CacheObject newObject { get; set; }
				public TargetBehavioralTypes targetBehaviorUsed { get; set; }

				public TargetChangedArgs(CacheObject newobj, TargetBehavioralTypes sendingtype)
				{
					 newObject=newobj;
					 targetBehaviorUsed=sendingtype;
				}
		  }
		  internal delegate void TargetChangeHandler(object cacheobj, TargetChangedArgs args);
		  internal TargetChangeHandler TargetChanged;
		  internal void OnTargetChanged(TargetChangedArgs e)
		  {
				if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
					 Logger.Write(LogLevel.Target, "Changed Object: {0}", MakeStringSingleLine(e.newObject.DebugString));

				

				LastChangeOfTarget=DateTime.Now;
				if (Bot.Settings.EnableWaitAfterContainers&&CurrentTarget.targetType==TargetType.Container)
				{
					 //Herbfunks delay for container loot.
					 lastHadContainerAsTarget=DateTime.Now;

					 if (CurrentTarget.IsResplendantChest)
						  lastHadRareChestAsTarget=DateTime.Now;
				}

				// We're sticking to the same target, so update the target's health cache to check for stucks
				if (CurrentTarget.targetType==TargetType.Unit)
				{
					 CurrentUnitTarget=(CacheUnit)CurrentTarget;
					 //Used to pause after no targets found.
					 lastHadUnitInSights=DateTime.Now;

					 // And record when we last saw any form of elite
					 if (CurrentUnitTarget.IsBoss||CurrentUnitTarget.IsEliteRareUnique||CurrentUnitTarget.IsTreasureGoblin)
						  lastHadEliteUnitInSights=DateTime.Now;
				}

				Bot.Targeting.TargetMover.NewTargetResetVars();
				Bot.Targeting.bWholeNewTarget=true;
				Bot.Targeting.bPickNewAbilities=true;

				TargetChangeHandler handler=TargetChanged;
				if (handler!=null)
				{
					 handler(this, e);
				}
		  }
		  private Char CHARnewLine='\x000A';
		  private Char CHARspace='\x0009';
		  private string MakeStringSingleLine(string str)
		  {
				return str.Replace(CHARnewLine, CHARspace);
		  }
		  #endregion

		 ///<summary>
		  ///Update our current object data ("Current Target")
		  ///</summary>
		  private bool RefreshTargetBehaviors()
		  {
				bool conditionTest=false;
				lastBehavioralType=TargetBehavioralTypes.None;
				foreach (var TLA in TargetBehaviors)
				{
					 //Check each behavior "pre-condition"
					 if (!TLA.BehavioralCondition) continue;

					 //Now test the behavior
					 conditionTest=TLA.Test.Invoke(ref CurrentTarget);
					 if (conditionTest)
					 {
						  if (!LastCachedTarget.RAGUID.Equals(CurrentTarget.RAGUID))
						  {
								TargetChangedArgs TargetChangedInfo= new TargetChangedArgs(CurrentTarget, lastBehavioralType);
								OnTargetChanged(TargetChangedInfo);
						  }

						  lastBehavioralType=TLA.TargetBehavioralTypeType;
						  break;
					 }
				}


					
				return conditionTest;
		  }

		  ///<summary>
		  ///Refreshes Cache and updates current target
		  ///</summary>
		  public void RefreshDiaObjects()
		  {
				//Update Character (just incase it wasnt called before..)
				Bot.Character.Data.Update(false, true);

				//Reset key targeting vars always!
				InitObjectRefresh();

				//Update object cache collection
				ObjectCache.UpdateCacheObjectCollection();

				//Update last Refresh Time
				lastRefreshedObjects=DateTime.Now;

			    //Refresh Obstacles
				ObjectCache.Obstacles.Values.ForEach(obj => obj.RefreshObject());

				//Check avoidance requirement still valid
				if (Bot.Targeting.RequiresAvoidance)
				{
					 if (!AvoidanceLastTarget&&
						  DateTime.Now.Subtract(Bot.Targeting.TargetMover.LastMovementAttempted).TotalMilliseconds < 300 &&//We are moving..? 
						  !ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(Bot.Targeting.TargetMover.CurrentTargetLocation) &&
						  !ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(Bot.Character.Data.Position, Bot.Targeting.TargetMover.CurrentTargetLocation)) 
					 {
						  RequiresAvoidance=false;
					 }
					 else if (AvoidanceLastTarget&&LastCachedTarget.CentreDistance<=2.5f)
						 RequiresAvoidance = false;
					 else if (Environment.TriggeringAvoidances.Count == 0)
						 RequiresAvoidance = false;
				}

				//This is our list of objects we consider to be valid for targeting.
				ValidObjects=ObjectCache.Objects.Values.Where(o => o.ObjectIsValidForTargeting).ToList();


				//Update Prioritize Flag
				bPrioritizeCloseRangeUnits = (Bot.Settings.Targeting.PrioritizeCloseRangeUnits && Bot.Settings.Targeting.PrioritizeCloseRangeMinimumUnits <= Environment.SurroundingUnits);


				// Still no target, let's end it all!
				if (!RefreshTargetBehaviors())
				{
					 StartingLocation=Vector3.Zero;
					 Bot.NavigationCache.PrioritizedRAGUIDs.Clear();
				}
		  }

		  ///<summary>
		  ///Resets/Updates cache and misc vars
		  ///</summary>
		  private void InitObjectRefresh()
		  {
				//Cache last target only if current target is not avoidance (Movement).
				LastCachedTarget=CurrentTarget!=null?CurrentTarget:ObjectCache.FakeCacheObject;

                if (CurrentTarget != null && CurrentTarget.targetType.HasValue && ObjectCache.CheckTargetTypeFlag(CurrentTarget.targetType.Value,TargetType.AvoidanceMovements))
				{
					 if (CurrentTarget.targetType.Value==TargetType.Fleeing)
					 {
						  LastFleeAction=DateTime.Now;
						  FleeingLastTarget=true;
					 }
					 else 
					 {
						  LastAvoidanceMovement=DateTime.Now;
						  AvoidanceLastTarget=true;
					 }
				}
				else
				{
					 FleeingLastTarget=false;
					 AvoidanceLastTarget=false;
				}

				//Traveling Flag Reset
				TravellingAvoidance=false;

				//Reset target
				CurrentTarget=null;
				CurrentUnitTarget=null;

				//Kill Loot Radius Update
				UpdateKillLootRadiusValues();

				// Refresh buffs (so we can check for wrath being up to ignore ice balls and anything else like that)
				Bot.Character.Class.HotBar.RefreshHotbarBuffs();


				// Bunch of variables used throughout
				Bot.Character.Data.PetData.Reset();
				// Reset the counters for monsters at various ranges
				Environment.Reset();
			


				//Check if we should trim our SNO cache..
				if (DateTime.Now.Subtract(ObjectCache.cacheSnoCollection.lastTrimming).TotalMilliseconds>Bot.Settings.Plugin.UnusedSNORemovalRate)
					 ObjectCache.cacheSnoCollection.TrimOldUnusedEntries();


				//Check Cached Object Removal flag
				if (RemovalCheck)
				{
					 //Remove flagged objects
					 var RemovalObjs=(from objs in ObjectCache.Objects.Values
											where objs.NeedsRemoved
											select objs.RAGUID).ToList();

					 foreach (var item in RemovalObjs)
					 {
						  CacheObject thisObj=ObjectCache.Objects[item];

						  //remove prioritized raguid
						  if (Bot.NavigationCache.PrioritizedRAGUIDs.Contains(item))
							  Bot.NavigationCache.PrioritizedRAGUIDs.Remove(item);

						  //Blacklist flag check
						  if (thisObj.BlacklistFlag!=BlacklistType.None)
								BlacklistCache.AddObjectToBlacklist(thisObj.RAGUID, thisObj.BlacklistFlag);

						  ObjectCache.Objects.Remove(thisObj.RAGUID);
					 }

					 RemovalCheck=false;
				}


				//Increase counter, clear entries if overdue.
				ObjectCache.Obstacles.AttemptToClearEntries();

				//Non-Combat behavior we reset temp blacklist so we don't get killed by "ignored" units..
				if (Bot.IsInNonCombatBehavior)
					 BlacklistCache.CheckRefreshBlacklists(10);
		  }

		  private int LastWorldID = -1;
		  private bool LastLevelIDChangeWasTownRun;
		  private void LevelAreaIDChangeHandler(int ID)
		  {
			  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Event))
				  Logger.Write(LogLevel.Event, "Level Area ID has Changed");

			  if (!BrainBehavior.IsVendoring)
			  {
				  //Check for World ID change!
				  if (Bot.Character.Data.iCurrentWorldID != LastWorldID)
				  {
					  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Event))
						  Logger.Write(LogLevel.Event, "World ID changed.. clearing Profile Interactable Cache.");
					  LastWorldID = Bot.Character.Data.iCurrentWorldID;
					  Bot.Game.Profile.InteractableObjectCache.Clear();
					  Navigator.SearchGridProvider.Update();
				  }

				  if (!LastLevelIDChangeWasTownRun)
				  {//Do full clear..
					  //Reset Playermover Backtrack Positions
					  BackTrackCache.cacheMovementGPRs.Clear();
					  Bot.NavigationCache.LOSBlacklistedRAGUIDs.Clear();
					  Bot.Game.Profile.InteractableCachedObject = null;
				  }

				  //Clear the object cache!
				  ObjectCache.Objects.Clear();
				  //ObjectCache.cacheSnoCollection.ClearDictionaryCacheEntries();
				  RemovalCheck = false;

				  //Reset Skip Ahead Cache
				  SkipAheadCache.ClearCache();

				  Bot.Character.Data.UpdateCoinage = true;

				  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
					  Logger.Write(LogLevel.Movement, "Updating Search Grid Provider.");

				  Navigator.SearchGridProvider.Update();

				  LastLevelIDChangeWasTownRun = false;
			  }
			  else if (Bot.Character.Data.bIsInTown)
			  {
				  LastLevelIDChangeWasTownRun = true;
			  }
		  }

	 }
}
