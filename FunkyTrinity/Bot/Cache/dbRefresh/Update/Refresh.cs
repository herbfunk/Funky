using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using Zeta.Internals.SNO;
using Zeta.CommonBot;

namespace FunkyTrinity
{
	 public partial class Funky
	 {

		  public static partial class dbRefresh
		  {
				private static int RefreshRateMilliseconds=150;
				public static bool ShouldRefreshObjectList
				{
					 get
					 {
						  return DateTime.Now.Subtract(lastRefreshedObjects).TotalMilliseconds>=RefreshRateMilliseconds;
					 }
				}
				///<summary>
				///Tracks the current Level ID
				///</summary>
				private static int LastLevelID=-1;
				///<summary>
				///Used to flag when Init should iterate and remove the objects
				///</summary>
				internal static bool RemovalCheck=false;



				///<summary>
				///Resets/Updates cache and misc vars
				///</summary>
				private static void InitObjectRefresh()
				{
					 //Cache last target only if current target is not avoidance (Movement).
					 if (!Bot.Target.Equals(null)&&Bot.Target.CurrentTarget.targetType.HasValue&&Bot.Target.CurrentTarget.targetType.Value!=TargetType.Avoidance)
						  Bot.Character.LastCachedTarget=Bot.Target.CurrentTarget!=null?Bot.Target.CurrentTarget.Clone():FakeCacheObject;

					 if (!Bot.Target.Equals(null)&&Bot.Target.CurrentTarget.targetType.HasValue&&Bot.Target.CurrentTarget.targetType.Value==TargetType.Avoidance
						  &&!String.IsNullOrEmpty(Bot.Target.CurrentTarget.InternalName)&&Bot.Target.CurrentTarget.InternalName.Contains("Kitespot"))
					 {
						  Bot.Combat.LastKiteAction=DateTime.Now;
						  Bot.Combat.KitedLastTarget=true;
					 }
					 else
						  Bot.Combat.KitedLastTarget=false;


					 Bot.Target.CurrentTarget=null;

					 //Kill Loot Radius Update
					 Bot.Combat.UpdateKillLootRadiusValues();

					 // Refresh buffs (so we can check for wrath being up to ignore ice balls and anything else like that)
					 Bot.Class.RefreshCurrentBuffs();

					 // Clear forcing close-range priority on mobs after XX period of time
					 if (Bot.Combat.bForceCloseRangeTarget&&DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>Bot.Combat.iMillisecondsForceCloseRange)
					 {
						  Bot.Combat.bForceCloseRangeTarget=false;
					 }

					 // Bunch of variables used throughout
					 Bot.Character.PetData.Reset();
					 // Reset the counters for monsters at various ranges
					 Bot.Combat.Reset();


					 //clear GPCache (Only melee uses this)
					 GridPointAreaCache.CheckClearedSearchRefresh();

					 //Check if we should trim our SNO cache..
					 if (DateTime.Now.Subtract(ObjectCache.cacheSnoCollection.lastTrimming).TotalMinutes>3)
						  ObjectCache.cacheSnoCollection.TrimOldUnusedEntries();


					 //Check Level ID changes and clear cache objects.
					 if (ZetaDia.CurrentLevelAreaId!=LastLevelID&&
								(!ZetaDia.Me.IsInTown))
					 {
						  LastLevelID=ZetaDia.CurrentLevelAreaId;

						  //Clear our current collection since we changed levels.
						  ObjectCache.Objects.Clear();
						  RemovalCheck=false;

						  //Reset Playermover Backtrack Positions
						  GridPointAreaCache.cacheMovementGPRs.Clear();

						  //Reset Skip Ahead Cache
						  CacheMovementTracking.ClearCache();

						  //This is the only time we should call this. MGP only needs updated every level change!
						  UpdateSearchGridProvider(true);
					 }



					 //Check Cached Object Removal flag
					 if (RemovalCheck)
					 {
						  //Remove flagged objects
						  List<int> RemovalObjs=(from objs in ObjectCache.Objects.Values
														 where objs.NeedsRemoved
														 select objs.RAGUID).ToList();

						  if (RemovalObjs.Count>0)
						  {
								List<int> removalList=RemovalObjs.ToList();

								for (int i=0; i<removalList.Count; i++)
								{
									 CacheObject thisObj=ObjectCache.Objects[removalList[i]];

                                    //remove prioritized raguid
                                     if(Bot.Combat.PrioritizedRAGUIDs.Contains(removalList[i]))
                                         Bot.Combat.PrioritizedRAGUIDs.Remove(removalList[i]);

									 //Blacklist flag check
									 if (thisObj.BlacklistFlag!=BlacklistType.None)
										  AddObjectToBlacklist(thisObj.RAGUID, thisObj.BlacklistFlag);

									 ObjectCache.Objects.Remove(thisObj.RAGUID);
								}
						  }

						  RemovalCheck=false;
					 }

					 //Increase counter, clear entries if overdue.
					 ObjectCache.Obstacles.AttemptToClearEntries();

					 //Non-Combat behavior we reset temp blacklist so we don't get killed by "ignored" units..
					 if (Bot.Combat.IsInNonCombatBehavior&&DateTime.Now.Subtract(dateSinceTemporaryBlacklistClear).TotalSeconds>10)
					 {
						  dateSinceTemporaryBlacklistClear=DateTime.Now;
						  hashRGUIDTemporaryIgnoreBlacklist=new HashSet<int>();
					 }
				}

				///<summary>
				///Refreshes Cache and updates current target
				///</summary>
				public static void RefreshDiaObjects()
				{
					 //Update Character (just incase it wasnt called before..)
					 Bot.Character.Update(false, true);

					 //Reset key targeting vars always!
					 InitObjectRefresh();

					 //Update object cache collection
					 UpdateCacheObjectCollection();

					 //Generate a vaild object list using our cached collection!
					 Bot.Target.ValidObjects=ObjectCache.Objects.Values
														  .Where(obj => obj.ObjectIsValidForTargeting).ToList();

					 //Update last Refresh Time
					 lastRefreshedObjects=DateTime.Now;

					 //Check avoidance requirement still valid
					 if (Bot.Combat.RequiresAvoidance&&Bot.Combat.TriggeringAvoidances.Count==0)
						  Bot.Combat.RequiresAvoidance=false;

					 // Still no target, let's end it all!
					 if (!Bot.Target.UpdateTarget())
					 {
						  //clear all prioritzed objects.
						  Bot.Combat.PrioritizedRAGUIDs.Clear();
						  return;
					 }

					 if (SettingsFunky.EnableWaitAfterContainers&&Bot.Target.CurrentTarget.targetType==TargetType.Container)
					 {
						  //Herbfunks delay for container loot.
						  Bot.Combat.lastHadContainerAsTarget=DateTime.Now;

						  if (Bot.Target.CurrentTarget.IsResplendantChest)
								Bot.Combat.lastHadRareChestAsTarget=DateTime.Now;
					 }
					 // Record the last time our target changed etc.
					 if (Bot.Target.CurrentTarget!=Bot.Character.LastCachedTarget)
					 {
						  Bot.Combat.dateSincePickedTarget=DateTime.Now;
						  Bot.Combat.LastHealthChange=DateTime.Now;
					 }
					 else
					 {
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
					 }
				}
		  }
	 }
}