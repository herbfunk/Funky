using System;
using System.Linq;
using FunkyTrinity.AbilityFunky;
using FunkyTrinity.Avoidances;
using FunkyTrinity.Cache;
using FunkyTrinity.Cache.Enums;
using FunkyTrinity.Movement;
using FunkyTrinity.Movement.Clustering;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using FunkyTrinity.Targeting.Behaviors;
using Zeta.Internals.SNO;
using Zeta.Navigation;
using Zeta.CommonBot.Settings;

namespace FunkyTrinity.Targeting
{
	 public partial class TargetHandler
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
				TargetChangeHandler handler=TargetChanged;

				if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
					 Logger.Write(LogLevel.Target, "Changed Object: {0}", MakeStringSingleLine(e.newObject.DebugString));

				this.LastChangeOfTarget=DateTime.Now;
				TargetMovement.NewTargetResetVars();

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
						  if (!LastCachedTarget.Equals(CurrentTarget))
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
				Bot.Character.Update(false, true);

				//Reset key targeting vars always!
				InitObjectRefresh();

				//Update object cache collection
				ObjectCache.UpdateCacheObjectCollection();

				//Update last Refresh Time
				lastRefreshedObjects=DateTime.Now;


				//Check avoidance requirement still valid
				if (Bot.Combat.RequiresAvoidance)
				{
					 if (!this.AvoidanceLastTarget&&DateTime.Now.Subtract(TargetMovement.LastMovementCommand).TotalMilliseconds<550) //We are moving..? 
					 {
						  Bot.Combat.RequiresAvoidance=false;
					 }
					 else if (Bot.Combat.TriggeringAvoidances.Count==0)
						  Bot.Combat.RequiresAvoidance=false;
				}

				//Update Search Grid Provider?
				if (Bot.NavigationCache.ShouldUpdateSearchGrid&&ObjectCache.Objects.Count>0&&!Bot.IsInNonCombatBehavior)
				{
					 Bot.NavigationCache.ShouldUpdateSearchGrid=false;
					 Zeta.Navigation.Navigator.SearchGridProvider.Update();
				}

				//This is our list of objects we consider to be valid for targeting.
				ObjectCache.ValidObjects=ObjectCache.Objects.Values.Where(o => o.ObjectIsValidForTargeting).ToList();


				// Still no target, let's end it all!
				if (!RefreshTargetBehaviors())
				{
					 //clear all prioritzed objects.
					 Bot.Combat.PrioritizedRAGUIDs.Clear();
					 return;
				}


				if (Bot.Settings.EnableWaitAfterContainers&&Bot.Targeting.CurrentTarget.targetType==TargetType.Container)
				{
					 //Herbfunks delay for container loot.
					this.lastHadContainerAsTarget=DateTime.Now;

					 if (Bot.Targeting.CurrentTarget.IsResplendantChest)
						  this.lastHadRareChestAsTarget=DateTime.Now;
				}

				// We're sticking to the same target, so update the target's health cache to check for stucks
				if (Bot.Targeting.CurrentTarget.targetType==TargetType.Unit)
				{
					 CacheUnit thisUnitObj=(CacheUnit)Bot.Targeting.CurrentTarget;
					 //Used to pause after no targets found.
					 this.lastHadUnitInSights=DateTime.Now;

					 // And record when we last saw any form of elite
					 if (Bot.Targeting.CurrentTarget.IsBoss||thisUnitObj.IsEliteRareUnique||Bot.Targeting.CurrentTarget.IsTreasureGoblin)
						  this.lastHadEliteUnitInSights=DateTime.Now;
				}

				// Record the last time our target changed etc.
				if (Bot.Targeting.CurrentTarget!=LastCachedTarget)
				{
					 this.LastChangeOfTarget=DateTime.Now;
				}
		  }

		  ///<summary>
		  ///Resets/Updates cache and misc vars
		  ///</summary>
		  private void InitObjectRefresh()
		  {
				//Cache last target only if current target is not avoidance (Movement).
				LastCachedTarget=Bot.Targeting.CurrentTarget!=null?Bot.Targeting.CurrentTarget.Clone():ObjectCache.FakeCacheObject;

				if (!Bot.Targeting.Equals(null)&&Bot.Targeting.CurrentTarget.targetType.HasValue&&Bot.Targeting.CurrentTarget.targetType.Value==TargetType.Avoidance
					 &&!String.IsNullOrEmpty(Bot.Targeting.CurrentTarget.InternalName))
				{
					 string internalname=Bot.Targeting.CurrentTarget.InternalName;
					 if (internalname.Contains("FleeSpot"))
					 {
						  this.LastFleeAction=DateTime.Now;
						  this.FleeingLastTarget=true;
					 }
					 else if (internalname.Contains("AvoidanceIntersection")||internalname.Contains("StayPutPoint")||internalname.Contains("SafeAvoid")||internalname.Contains("SafeReuseAvoid"))
					 {
						  this.LastAvoidanceMovement=DateTime.Now;
						  this.AvoidanceLastTarget=true;
					 }
				}
				else
				{
					 this.FleeingLastTarget=false;
					 this.AvoidanceLastTarget=false;
				}

				//Reset target
				Bot.Targeting.CurrentTarget=null;
				Bot.Targeting.CurrentUnitTarget=null;

				//Kill Loot Radius Update
				Bot.Targeting.UpdateKillLootRadiusValues();

				// Refresh buffs (so we can check for wrath being up to ignore ice balls and anything else like that)
				Bot.Class.RefreshCurrentBuffs();
				Bot.Class.RefreshCurrentDebuffs();

				// Clear forcing close-range priority on mobs after XX period of time
				if (Bot.Combat.bForceCloseRangeTarget&&DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>Bot.Combat.iMillisecondsForceCloseRange)
				{
					 Bot.Combat.bForceCloseRangeTarget=false;
				}

				// Bunch of variables used throughout
				Bot.Character.PetData.Reset();
				// Reset the counters for monsters at various ranges
				Bot.Combat.Reset();


				//Check if we should trim our SNO cache..
				if (DateTime.Now.Subtract(ObjectCache.cacheSnoCollection.lastTrimming).TotalMilliseconds>Funky.Settings.UnusedSNORemovalRate)
					 ObjectCache.cacheSnoCollection.TrimOldUnusedEntries();


				//Check Level ID changes and clear cache objects.
				if (ZetaDia.CurrentLevelAreaId!=LastLevelID&&(!ZetaDia.Me.IsInTown))
				{
					 //Grace Peroid of 5 Seconds before updating.
					 if (DateTime.Now.Subtract(LastCheckedLevelID).TotalSeconds>5)
					 {
						  LastCheckedLevelID=DateTime.Now;
						  LastLevelID=ZetaDia.CurrentLevelAreaId;

						  //Clear our current collection since we changed levels.
						  ObjectCache.Objects.Clear();
						  ObjectCache.cacheSnoCollection.ClearDictionaryCacheEntries();
						  RemovalCheck=false;

						  //Reset Playermover Backtrack Positions
						  BackTrackCache.cacheMovementGPRs.Clear();

						  //Reset Skip Ahead Cache
						  SkipAheadCache.ClearCache();
					 }
				}

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
						  if (Bot.Combat.PrioritizedRAGUIDs.Contains(item))
								Bot.Combat.PrioritizedRAGUIDs.Remove(item);

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

	 }
}
