using System;
using System.Linq;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.DBHandlers;
using FunkyBot.Movement;
using FunkyBot.Targeting.Behaviors;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Common;
using System.Collections.Generic;

namespace FunkyBot.Targeting
{
	public class TargetingCache
	{
		#region Target Changed

		internal class TargetChangedArgs : EventArgs
		{
			public CacheObject newObject { get; set; }
			public TargetBehavioralTypes targetBehaviorUsed { get; set; }

			public TargetChangedArgs(CacheObject newobj, TargetBehavioralTypes sendingtype)
			{
				newObject = newobj;
				targetBehaviorUsed = sendingtype;
			}
		}
		internal delegate void TargetChangeHandler(object cacheobj, TargetChangedArgs args);
		internal TargetChangeHandler TargetChanged;
		internal void OnTargetChanged(TargetChangedArgs e)
		{
			Logger.Write(LogLevel.Target, "Changed Object: {0}", MakeStringSingleLine(e.newObject.DebugString));



			LastChangeOfTarget = DateTime.Now;
			if (Bot.Settings.EnableWaitAfterContainers && CurrentTarget.targetType == TargetType.Container)
			{
				//Herbfunks delay for container loot.
				lastHadContainerAsTarget = DateTime.Now;

				if (CurrentTarget.IsResplendantChest)
					lastHadRareChestAsTarget = DateTime.Now;
			}

			// We're sticking to the same target, so update the target's health cache to check for stucks
			if (CurrentTarget.targetType == TargetType.Unit)
			{
				CurrentUnitTarget = (CacheUnit)CurrentTarget;
				//Used to pause after no targets found.
				lastHadUnitInSights = DateTime.Now;

				// And record when we last saw any form of elite
				if (CurrentUnitTarget.IsBoss || CurrentUnitTarget.IsEliteRareUnique || CurrentUnitTarget.IsTreasureGoblin)
					lastHadEliteUnitInSights = DateTime.Now;
			}

			bWholeNewTarget = true;
			bPickNewAbilities = true;

			TargetChangeHandler handler = TargetChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}
		private Char CHARnewLine = '\x000A';
		private Char CHARspace = '\x0009';
		private string MakeStringSingleLine(string str)
		{
			return str.Replace(CHARnewLine, CHARspace);
		}
		#endregion



		///<summary>
		///Refreshes Cache and updates current target
		///</summary>
		public void Refresh()
		{
			//Update Character (just incase it wasnt called before..)
			Bot.Character.Data.Update(false, true);

			//Reset key targeting vars always!
			InitObjectRefresh();

			//Update object cache collection
			ObjectCache.UpdateCacheObjectCollection();

			//Update last Refresh Time
			lastRefreshedObjects = DateTime.Now;

			//Refresh Obstacles
			ObjectCache.Obstacles.Values.ForEach(obj => obj.RefreshObject());

			//Check avoidance requirement still valid
			if (RequiresAvoidance)
			{
				if (!AvoidanceLastTarget &&
					 DateTime.Now.Subtract(Bot.Targeting.Movement.LastMovementAttempted).TotalMilliseconds < 300 &&//We are moving..? 
					 !ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(Bot.Targeting.Movement.CurrentTargetLocation) &&
					 !ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(Bot.Character.Data.Position, Bot.Targeting.Movement.CurrentTargetLocation))
				{
					RequiresAvoidance = false;
				}
				else if (AvoidanceLastTarget && LastCachedTarget.CentreDistance <= 2.5f)
					RequiresAvoidance = false;
				else if (Environment.TriggeringAvoidances.Count == 0)
					RequiresAvoidance = false;
			}

			//This is our list of objects we consider to be valid for targeting.
			ValidObjects = ObjectCache.Objects.Values.Where(o => o.ObjectIsValidForTargeting).ToList();


			//Update Prioritize Flag
			bPrioritizeCloseRangeUnits = (Bot.Settings.Targeting.PrioritizeCloseRangeUnits && Bot.Settings.Targeting.PrioritizeCloseRangeMinimumUnits <= Environment.SurroundingUnits);


			// Still no target, let's end it all!
			if (!RefreshTargetBehaviors())
			{
				StartingLocation = Vector3.Zero;
				Bot.NavigationCache.PrioritizedRAGUIDs.Clear();
			}
			else
			{

				if (ExitGame.ShouldExitGame)
				{
					//Have not started exit behavior.. global overlord will begin so lets exit!
					if (!ExitGame.BehaviorEngaged)
					{
						CurrentTarget = null;
					}
					else if (DateTime.Now.Subtract(ExitGame.BehaviorEngagedTime).TotalSeconds > 60)
					{//We started behavior over a minute ago.. lets just exit already!!
						Logger.DBLog.InfoFormat("[Funky] Forcing Exiting behavior after one minute!");
						CurrentTarget = null;
					}
				}
			}
		}

		///<summary>
		///Resets/Updates cache and misc vars
		///</summary>
		private void InitObjectRefresh()
		{
			//Cache last target only if current target is not avoidance (Movement).
			LastCachedTarget = CurrentTarget != null ? CurrentTarget : ObjectCache.FakeCacheObject;

			if (CurrentTarget != null && CurrentTarget.targetType.HasValue && ObjectCache.CheckTargetTypeFlag(CurrentTarget.targetType.Value, TargetType.AvoidanceMovements))
			{
				if (CurrentTarget.targetType.Value == TargetType.Fleeing)
				{
					LastFleeAction = DateTime.Now;
					FleeingLastTarget = true;
				}
				else
				{
					LastAvoidanceMovement = DateTime.Now;
					AvoidanceLastTarget = true;
				}
			}
			else
			{
				FleeingLastTarget = false;
				AvoidanceLastTarget = false;
			}

			//Traveling Flag Reset
			TravellingAvoidance = false;

			//Reset target
			CurrentTarget = null;
			CurrentUnitTarget = null;



			//Kill Loot Radius Update
			UpdateKillLootRadiusValues();

			// Refresh buffs (so we can check for wrath being up to ignore ice balls and anything else like that)
			Bot.Character.Class.HotBar.RefreshHotbarBuffs();


			// Bunch of variables used throughout
			Bot.Character.Data.PetData.Reset();
			// Reset the counters for monsters at various ranges
			Environment.Reset();



			//Check if we should trim our SNO cache..
			if (DateTime.Now.Subtract(ObjectCache.cacheSnoCollection.lastTrimming).TotalMilliseconds > Bot.Settings.Plugin.UnusedSNORemovalRate)
				ObjectCache.cacheSnoCollection.TrimOldUnusedEntries();


			//Check Cached Object Removal flag
			if (RemovalCheck)
			{
				//Remove flagged objects
				var RemovalObjs = (from objs in ObjectCache.Objects.Values
								   where objs.NeedsRemoved
								   select objs.RAGUID).ToList();

				foreach (var item in RemovalObjs)
				{
					CacheObject thisObj = ObjectCache.Objects[item];

					//remove prioritized raguid
					if (Bot.NavigationCache.PrioritizedRAGUIDs.Contains(item))
						Bot.NavigationCache.PrioritizedRAGUIDs.Remove(item);

					//Blacklist flag check
					if (thisObj.BlacklistFlag != BlacklistType.None)
						BlacklistCache.AddObjectToBlacklist(thisObj.RAGUID, thisObj.BlacklistFlag);

					ObjectCache.Objects.Remove(thisObj.RAGUID);
				}

				RemovalCheck = false;
			}


			//Increase counter, clear entries if overdue.
			ObjectCache.Obstacles.AttemptToClearEntries();

			//Non-Combat behavior we reset temp blacklist so we don't get killed by "ignored" units..
			if (Bot.IsInNonCombatBehavior)
				BlacklistCache.CheckRefreshBlacklists(10);

			//Check Gold Inactivity
			Bot.Game.GoldTimeoutChecker.CheckTimeoutTripped();
		}

		///<summary>
		///Update our current object data ("Current Target")
		///</summary>
		private bool RefreshTargetBehaviors()
		{
			bool conditionTest = false;
			lastBehavioralType = TargetBehavioralTypes.None;
			foreach (var TLA in TargetBehaviors)
			{
				//Check each behavior "pre-condition"
				if (!TLA.BehavioralCondition) continue;

				//Now test the behavior
				conditionTest = TLA.Test.Invoke(ref CurrentTarget);
				if (conditionTest)
				{
					if (!LastCachedTarget.RAGUID.Equals(CurrentTarget.RAGUID))
					{
						TargetChangedArgs TargetChangedInfo = new TargetChangedArgs(CurrentTarget, lastBehavioralType);
						OnTargetChanged(TargetChangedInfo);
					}

					lastBehavioralType = TLA.TargetBehavioralTypeType;
					break;
				}
			}



			return conditionTest;
		}


		//Targeting Specific Properties
		internal bool Backtracking { get; set; }
		// A flag to indicate whether we have a new target from the overlord (decorator) or not, in which case don't refresh targets again this first loop
		internal bool bWholeNewTarget { get; set; }
		// A flag to indicate if we should pick a new power/Ability to use or not
		internal bool bPickNewAbilities { get; set; }
		// Flag used to indicate if we are simply waiting for a power to go off - so don't do any new target checking or anything
		internal bool bWaitingForPower { get; set; }
		// And a special post-use pause
		internal bool bWaitingAfterPower { get; set; }
		// If we are waiting before popping a potion
		internal bool bWaitingForPotion { get; set; }
		// Force a target update after certain interactions
		internal bool bForceTargetUpdate { get; set; }
		// Variable to let us force new target creations immediately after a root
		internal bool bWasRootedLastTick { get; set; }

		//Loot Check
		internal bool ShouldCheckItemLooted { get; set; }
		internal int recheckCount { get; set; }
		internal bool reCheckedFinished { get; set; }
		internal int CheckItemLootStackCount { get; set; }


		//Order is important! -- we test from start to finish.
		internal readonly TargetBehavior[] TargetBehaviors =
		 {
			 new TBGroupingResume(), 
			 new TBAvoidance(), 
			 new TBFleeing(), 
			 new TBUpdateTarget(), 
			 new TBGrouping(), 
			 new TBLOSMovement(),
			 new TBBacktrack(),
			 new TBEnd()
		 };
		internal TargetBehavioralTypes lastBehavioralType = TargetBehavioralTypes.None;
		internal Clustering Clusters = new Clustering();
		internal Environment Environment = new Environment();
		internal float iCurrentMaxKillRadius = 0f;
		internal float iCurrentMaxLootRadius = 0f;
		internal void UpdateKillLootRadiusValues()
		{
			iCurrentMaxKillRadius = CharacterSettings.Instance.KillRadius;
			iCurrentMaxLootRadius = CharacterSettings.Instance.LootRadius;
			// Not allowed to kill monsters due to profile/routine/combat targeting settings - just set the kill range to a third
			if (!ProfileManager.CurrentProfile.KillMonsters) iCurrentMaxKillRadius /= 3;

			// Always have a minimum kill radius, so we're never getting whacked without retaliating
			if (iCurrentMaxKillRadius < 10 || Bot.Settings.Ranges.IgnoreCombatRange) iCurrentMaxKillRadius = 10;

			//Non-Combat Behavior we set minimum kill radius
			if (Bot.IsInNonCombatBehavior) iCurrentMaxKillRadius = 50;

			// Not allowed to loots due to profile/routine/loot targeting settings - just set range to a quarter
			if (!ProfileManager.CurrentProfile.PickupLoot) iCurrentMaxLootRadius /= 4;


			//Ignore Loot Range Setting
			if (Bot.Settings.Ranges.IgnoreLootRange) iCurrentMaxLootRadius = 10;
		}
		internal bool bPrioritizeCloseRangeUnits { get; set; }

		internal bool DontMove { get; set; }



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


		internal void ResetTargetHandling()
		{
			CurrentTarget = null;
			StartingLocation = Vector3.Zero;
			bWaitingForPower = false;
			bWaitingAfterPower = false;
			bWaitingForPotion = false;
			bWasRootedLastTick = false;
			recheckCount = 0;
			reCheckedFinished = false;
			CheckItemLootStackCount = 0;
			Backtracking = false;
		}
		//Avoidance Related
		internal bool RequiresAvoidance { get; set; }
		internal bool TravellingAvoidance { get; set; }
		///<summary>
		///Usable Objects -- refresh inside Target.UpdateTarget
		///</summary>
		internal List<CacheObject> ValidObjects = new List<CacheObject>();
		///<summary>
		///Adds the specific object to a list due to the object being avoided due to avoidance.
		///</summary>
		internal List<CacheObject> objectsIgnoredDueToAvoidance = new List<CacheObject>();

		internal CacheObject LastCachedTarget { get; set; }
		internal bool FleeingLastTarget { get; set; }
		internal bool AvoidanceLastTarget { get; set; }
		internal DateTime LastAvoidanceMovement { get; set; }
		internal DateTime LastFleeAction = DateTime.Today;
		internal DateTime LastChangeOfTarget = DateTime.Today;
		// Last had any mob in range, for loot-waiting
		internal DateTime lastHadUnitInSights { get; set; }
		// When we last saw a boss/elite etc.
		internal DateTime lastHadEliteUnitInSights { get; set; }
		//Last time we had a container, for loot-waiting
		internal DateTime lastHadContainerAsTarget { get; set; }
		//Cursed Shrine Used but still valid
		internal DateTime lastSeenCursedShrine { get; set; }
		//Update QuestMonster property on units
		internal bool UpdateQuestMonsterProperty { get; set; }
		//When we last saw a "rare" chest
		internal DateTime lastHadRareChestAsTarget { get; set; }
		// Store the date-time when we *FIRST* picked this target, so we can blacklist after X period of time targeting

		internal int iTotalNumberGoblins = 0;
		internal DateTime lastGoblinTime = DateTime.Today;

		//Store the location of bot position when target handling is engaged.
		internal Vector3 StartingLocation = Vector3.Zero;
		///<summary>
		///Used to flag when Init should iterate and remove the objects
		///</summary>
		internal bool RemovalCheck = false;
		// Used to force-refresh dia objects at least once every XX milliseconds 
		internal DateTime lastRefreshedObjects = DateTime.Today;
		public bool ShouldRefreshObjectList
		{
			get
			{
				return DateTime.Now.Subtract(lastRefreshedObjects).TotalMilliseconds >= Bot.Settings.Plugin.CacheObjectRefreshRate;
			}
		}
	}
}
