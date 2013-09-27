using System;
using System.Linq;
using FunkyTrinity.Ability;
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

namespace FunkyTrinity.Targeting
{
	 public partial class TargetHandler
	 {
		  //TODO:: Added Line of Sight Movement as a behavior.

		  //Order is important! -- we test from start to finish.
		  internal readonly TargetBehavior[] TargetBehaviors=new TargetBehavior[]
		  {
			  new TBGroupingResume(), 
			  new TBAvoidance(), 
			  new TBFleeing(), 
			  new TBUpdateTarget(), 
			  new TBGrouping(), 
			 // new TBLOSMovement(),
			  new TBEnd(),
		  };

		  internal TargetBehavioralTypes lastBehavioralType=TargetBehavioralTypes.None;
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

				if (Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
					 Logger.Write(LogLevel.Target, "Changed Object: {0}", MakeStringSingleLine(e.newObject.DebugString));


				
				if (handler!=null)
				{
					 handler(this, e);
				}
		  }

		 ///<summary>
		  ///Update our current object data ("Current Target")
		  ///</summary>
		  private bool RefreshTargetBehaviors()
		  {
				bool conditionTest=false;
				lastBehavioralType=TargetBehavioralTypes.None;
				foreach (var TLA in TargetBehaviors)
				{
					 if (!TLA.BehavioralCondition) continue;

					 conditionTest=TLA.Test.Invoke(ref CurrentTarget);
					 if (conditionTest)
					 {
						  if (!LastCachedTarget.Equals(CurrentTarget))
						  {
								LastHealthChange=DateTime.Today;
								LastHealthDropPct=0d;

								TargetChangedArgs TargetChangedInfo= new TargetChangedArgs(CurrentTarget, lastBehavioralType);
								OnTargetChanged(TargetChangedInfo);
						  }

						  lastBehavioralType=TLA.TargetBehavioralTypeType;
						  break;
					 }
				}


					
				return conditionTest;
		  }

		  // Used to force-refresh dia objects at least once every XX milliseconds 
		  internal DateTime lastRefreshedObjects=DateTime.Today;
		  private int RefreshRateMilliseconds=150;
		  public bool ShouldRefreshObjectList
		  {
				get
				{
					 return DateTime.Now.Subtract(lastRefreshedObjects).TotalMilliseconds>=RefreshRateMilliseconds;
				}
		  }
		  ///<summary>
		  ///Tracks the current Level ID
		  ///</summary>
		  private int LastLevelID=-1;
		  private DateTime LastCheckedLevelID=DateTime.Today;

		  ///<summary>
		  ///Used to flag when Init should iterate and remove the objects
		  ///</summary>
		  internal bool RemovalCheck=false;



		  ///<summary>
		  ///Resets/Updates cache and misc vars
		  ///</summary>
		  private void InitObjectRefresh()
		  {
				//Cache last target only if current target is not avoidance (Movement).
				LastCachedTarget=Bot.Target.CurrentTarget!=null?Bot.Target.CurrentTarget:Funky.FakeCacheObject;

				if (!Bot.Target.Equals(null)&&Bot.Target.CurrentTarget.targetType.HasValue&&Bot.Target.CurrentTarget.targetType.Value==TargetType.Avoidance
					 &&!String.IsNullOrEmpty(Bot.Target.CurrentTarget.InternalName))
				{
					 string internalname=Bot.Target.CurrentTarget.InternalName;
					 if (internalname.Contains("FleeSpot"))
					 {
						  Bot.Combat.LastFleeAction=DateTime.Now;
						  Bot.Combat.FleeingLastTarget=true;
					 }
					 else if (internalname.Contains("AvoidanceIntersection")||internalname.Contains("StayPutPoint")||internalname.Contains("SafeAvoid")||internalname.Contains("SafeReuseAvoid"))
					 {
						  Bot.Combat.LastAvoidanceMovement=DateTime.Now;
						  Bot.Combat.AvoidanceLastTarget=true;
					 }
				}
				else
				{
					 Bot.Combat.FleeingLastTarget=false;
					 Bot.Combat.AvoidanceLastTarget=false;
				}

				Bot.Target.CurrentTarget=null;
				Bot.Target.CurrentUnitTarget=null;

				//Kill Loot Radius Update
				Bot.UpdateKillLootRadiusValues();

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
				if (DateTime.Now.Subtract(ObjectCache.cacheSnoCollection.lastTrimming).TotalMinutes>3)
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
				{
					 BlacklistCache.CheckRefreshBlacklists(10);
				}
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
					 //Nothing "Triggering".. 
					 if (Bot.Combat.TriggeringAvoidances.Count==0)
					 {
						  //No flee behavior.. check if bot has at least 25% hp..
						  if (!Bot.SettingsFunky.Fleeing.EnableFleeingBehavior||Bot.Character.dCurrentHealthPct>0.25d)
								Bot.Combat.RequiresAvoidance=false;
					 }
				}

				//This is our list of objects we consider to be valid for targeting.
				Bot.ValidObjects=ObjectCache.Objects.Values.Where(o => o.ObjectIsValidForTargeting).ToList();


				// Still no target, let's end it all!
				if (!RefreshTargetBehaviors())
				{
					 //clear all prioritzed objects.
					 Bot.Combat.PrioritizedRAGUIDs.Clear();
					 return;
				}

				if (!Bot.Target.CurrentTarget.Equals(LastCachedTarget))
					 TargetMovement.NewTargetResetVars();

				if (Bot.SettingsFunky.EnableWaitAfterContainers&&Bot.Target.CurrentTarget.targetType==TargetType.Container)
				{
					 //Herbfunks delay for container loot.
					 Bot.Combat.lastHadContainerAsTarget=DateTime.Now;

					 if (Bot.Target.CurrentTarget.IsResplendantChest)
						  Bot.Combat.lastHadRareChestAsTarget=DateTime.Now;
				}

				// We're sticking to the same target, so update the target's health cache to check for stucks
				if (Bot.Target.CurrentTarget.targetType==TargetType.Unit)
				{
					 CacheUnit thisUnitObj=(CacheUnit)Bot.Target.CurrentTarget;
					 //Used to pause after no targets found.
					 Bot.Combat.lastHadUnitInSights=DateTime.Now;

					 // And record when we last saw any form of elite
					 if (Bot.Target.CurrentTarget.IsBoss||thisUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin)
						  Bot.Combat.lastHadEliteUnitInSights=DateTime.Now;
				}

				// Record the last time our target changed etc.
				if (Bot.Target.CurrentTarget!=LastCachedTarget)
				{
					 Bot.Combat.dateSincePickedTarget=DateTime.Now;
					 LastHealthChange=DateTime.Now;
				}
		  }










		  private Char CHARnewLine='\x000A';
		  private Char CHARspace='\x0009';
		  private string MakeStringSingleLine(string str)
		  {
				return str.Replace(CHARnewLine, CHARspace);
		  }

		  internal CacheObject LastCachedTarget { get; set; }
		 internal DateTime LastHealthChange { get; set; }
		 internal double LastHealthDropPct { get; set; }
	 }
}
