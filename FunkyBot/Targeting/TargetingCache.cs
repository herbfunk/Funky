using System;
using FunkyBot.Cache;
using FunkyBot.Movement;
using FunkyBot.Targeting.Behaviors;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Settings;
using System.Collections.Generic;

namespace FunkyBot.Targeting
{
	 public partial class TargetingHandler
	{
		 //Order is important! -- we test from start to finish.
		 internal readonly TargetBehavior[] TargetBehaviors=new TargetBehavior[]
		  {
			  new TBGroupingResume(), 
			  new TBAvoidance(), 
			  new TBFleeing(), 
			  new TBUpdateTarget(), 
			  new TBGrouping(), 
			  new TBLOSMovement(),
			  new TBEnd(),
		  };
		 internal TargetBehavioralTypes lastBehavioralType=TargetBehavioralTypes.None;
		 internal Clustering Clusters = new Clustering();
		 internal Environment Environment = new Environment();
		 internal float iCurrentMaxKillRadius=0f;
		 internal float iCurrentMaxLootRadius=0f;
		 internal void UpdateKillLootRadiusValues()
		 {
			  iCurrentMaxKillRadius=CharacterSettings.Instance.KillRadius;
			  iCurrentMaxLootRadius=CharacterSettings.Instance.LootRadius;
			  // Not allowed to kill monsters due to profile/routine/combat targeting settings - just set the kill range to a third
			  if (!ProfileManager.CurrentProfile.KillMonsters) iCurrentMaxKillRadius/=3;

			  // Always have a minimum kill radius, so we're never getting whacked without retaliating
			  if (iCurrentMaxKillRadius<10||Bot.Settings.Ranges.IgnoreCombatRange) iCurrentMaxKillRadius=10;

			  //Non-Combat Behavior we set minimum kill radius
			  if (Bot.IsInNonCombatBehavior) iCurrentMaxKillRadius=50;

			  // Not allowed to loots due to profile/routine/loot targeting settings - just set range to a quarter
			  if (!ProfileManager.CurrentProfile.PickupLoot) iCurrentMaxLootRadius/=4;


			  //Ignore Loot Range Setting
			  if (Bot.Settings.Ranges.IgnoreLootRange) iCurrentMaxLootRadius=10;
		 }
		 internal bool bPrioritizeCloseRangeUnits { get; set; }
		 internal bool DontMove { get; set; }
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
		 internal void ResetTargetHandling()
		 {
			  this.CurrentTarget=null;

			  TargetMovement.ResetTargetMovementVars();

			  bWaitingForPower=false;
			  bWaitingAfterPower=false;
			  bWaitingForPotion=false;
			  bWasRootedLastTick=false;
			  recheckCount=0;
			  reCheckedFinished=false;

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
		 internal DateTime LastFleeAction=DateTime.Today;
		 internal DateTime LastChangeOfTarget=DateTime.Today;
		 // Last had any mob in range, for loot-waiting
		 internal DateTime lastHadUnitInSights { get; set; }
		 // When we last saw a boss/elite etc.
		 internal DateTime lastHadEliteUnitInSights { get; set; }
		 //Last time we had a container, for loot-waiting
		 internal DateTime lastHadContainerAsTarget { get; set; }
		 //When we last saw a "rare" chest
		 internal DateTime lastHadRareChestAsTarget { get; set; }
		 // Store the date-time when we *FIRST* picked this target, so we can blacklist after X period of time targeting

		 internal int iTotalNumberGoblins=0;
		 internal DateTime lastGoblinTime=DateTime.Today;

	    //Store the location of bot position when target handling is engaged.
		 internal Vector3 StartingLocation=Vector3.Zero;
		 ///<summary>
		 ///Used to flag when Init should iterate and remove the objects
		 ///</summary>
		 internal bool RemovalCheck=false;
		 // Used to force-refresh dia objects at least once every XX milliseconds 
		 internal DateTime lastRefreshedObjects=DateTime.Today;
		 public bool ShouldRefreshObjectList
		 {
			  get
			  {
					return DateTime.Now.Subtract(lastRefreshedObjects).TotalMilliseconds>=Bot.Settings.Plugin.CacheObjectRefreshRate;
			  }
		 }
	}
}
