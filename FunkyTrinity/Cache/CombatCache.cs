using System;
using System.Linq;
using FunkyTrinity.Targeting;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Internals;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.SNO;
using Zeta.CommonBot;
using FunkyTrinity.Ability;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;
using FunkyTrinity.Movement.Clustering;

namespace FunkyTrinity.Cache
{
		  ///<summary>
		  ///Cache of current combat variables
		  ///</summary>
		  public class CombatCache
		  {

				public CombatCache()
				{
					 bWholeNewTarget=false;
					 bPickNewAbilities=false;
					 bWaitingAfterPower=false;
					 bWaitingForPower=false;
					 bWaitingForPotion=false;
					 bForceTargetUpdate=false;
					 bWasRootedLastTick=false;

					 ShouldCheckItemLooted=false;
					 reCheckedFinished=false;
					 recheckCount=0;

					 iElitesWithinRange=new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
					 iAnythingWithinRange=new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
					 iNonRendedTargets_6=0;
					 bAnyBossesInRange=false;
					 bAnyChampionsPresent=false;
					 bAnyTreasureGoblinsPresent=false;
					 bAnyMobsInCloseRange=false;
					 bAnyNonWWIgnoreMobsInRange=false;
					 RequiresAvoidance=false;
					 TravellingAvoidance=false;
					 bForceCloseRangeTarget=false;
					 lastHadUnitInSights=DateTime.Today;
					 lastHadEliteUnitInSights=DateTime.Today;
					 lastHadContainerAsTarget=DateTime.Today;
					 lastHadRareChestAsTarget=DateTime.Today;
					 dateSincePickedTarget=DateTime.Today;
					 LastAvoidanceMovement=DateTime.Today;
					 SurroundingUnits=0;
					 DontMove=false;
					 CriticalAvoidance=false;
					 FleeingLastTarget=false;
					 AvoidanceLastTarget=false;
					 UsesDOTDPSAbility=false;
					 TargetClusterCollection=new ClusterTargetCollection(TargetClusterConditions);
				}

				internal CacheObject LastCachedTarget { get; set; }
				internal List<int> PrioritizedRAGUIDs=new List<int>();
				internal List<CacheServerObject> NearbyObstacleObjects=new List<CacheServerObject>();
				internal List<int> NearbyAvoidances=new List<int>();
				internal List<CacheUnit> FleeTriggeringUnits=new List<CacheUnit>();
				internal List<CacheAvoidance> TriggeringAvoidances=new List<CacheAvoidance>();
				internal List<int> UnitRAGUIDs=new List<int>();
				internal List<int> ValidClusterUnits=new List<int>();
				internal List<CacheUnit> DistantUnits=new List<CacheUnit>();
				internal List<CacheUnit> LoSMovementUnits=new List<CacheUnit>();
				private ClusterConditions TargetClusterConditions=new ClusterConditions(Bot.SettingsFunky.Cluster.ClusterDistance, 100f, Bot.SettingsFunky.Cluster.ClusterMinimumUnitCount, false);
				internal ClusterTargetCollection TargetClusterCollection { get; set; }

				internal List<UnitCluster> CurrentGroupClusters=new List<UnitCluster>();

				///<summary>
				///Tracks Lists of Clusters for specific Conditions used during ability Clustering.
				///</summary>
				internal Dictionary<ClusterConditions, ClusterCollection> AbilityClusters=new Dictionary<ClusterConditions, ClusterCollection>();

				internal DateTime LastClusterTargetLogicRefresh=DateTime.MinValue;
				internal DateTime LastClusterGroupingLogicRefresh=DateTime.MinValue;


				//Cache last filtered list generated
				internal List<UnitCluster> LastClusterList=new List<UnitCluster>();
				private DateTime lastClusterComputed=DateTime.Today;
				internal List<UnitCluster> Clusters(ClusterConditions CC)
				{
					 if (!AbilityClusters.ContainsKey(CC))
					 {
						  Logger.Write(LogLevel.Cluster, "Creating new entry for ClusterConditions -- {0}", CC.ToString());
						  AbilityClusters.Add(CC, new ClusterCollection(CC, 400,200));
					 }

					 if (AbilityClusters[CC].ShouldUpdateClusters)
						  AbilityClusters[CC].UpdateClusters();
					 else
						  AbilityClusters[CC].RefreshClusters();

					 //Logging.WriteVerbose("ability Clusters Found {0}", AbilityClusters[CC].Count.ToString());
					 return AbilityClusters[CC].CurrentClusters;
				}


				internal void UpdateGroupClusteringVariables()
				{
					 //grouping clustering
					 if (Bot.SettingsFunky.Grouping.AttemptGroupingMovements)
					 {
						  if ((CurrentGroupClusters.Count==0&&DateTime.Compare(LastClusterGroupingLogicRefresh, DateTime.Now)<0)
								||(CurrentGroupClusters.Count>0&&DateTime.Now.Subtract(LastClusterGroupingLogicRefresh).TotalMilliseconds>1250))
						  {
								//Clear Clusters and Unit collection
								CurrentGroupClusters=new List<UnitCluster>();

								//Check if there are enough units present currently..
								if (DistantUnits.Count>Bot.SettingsFunky.Grouping.GroupingMinimumUnitsInCluster)
								{

									 //Update UnitCluster Collection
									 CurrentGroupClusters=UnitCluster.RunKmeans(DistantUnits, Bot.SettingsFunky.Grouping.GroupingClusterRadiusDistance)
										  .Where(cluster => cluster.ListUnits.Count>=Bot.SettingsFunky.Grouping.GroupingMinimumUnitsInCluster&&cluster.NearestMonsterDistance<=Bot.SettingsFunky.Grouping.GroupingMaximumDistanceAllowed)
										  .OrderByDescending(cluster => cluster.NearestMonsterDistance).ToList();

									 //if (Bot.SettingsFunky.LogGroupingOutput)
									 //    Logger.Write(LogLevel.Cluster, "Updated Group Clusters. Count={0}", CurrentGroupClusters.Count.ToString());

									 LastClusterGroupingLogicRefresh=DateTime.Now;
								}

						  }
					 }
				}

				///<summary>
				///The main cluster refresh -- creates general clustering to be used in more specific clustering checks.
				///</summary>
				internal void UpdateTargetClusteringVariables()
				{
					 
					 //normal clustering
					 if (Bot.SettingsFunky.Cluster.EnableClusteringTargetLogic&&
						  (!Bot.SettingsFunky.Cluster.IgnoreClusteringWhenLowHP||Bot.Character.dCurrentHealthPct>Bot.SettingsFunky.Cluster.IgnoreClusterLowHPValue))
					 {
						  if (UnitRAGUIDs.Count>=Bot.SettingsFunky.Cluster.ClusterMinimumUnitCount)
						  {
								if (TargetClusterCollection.ShouldUpdateClusters)
									 TargetClusterCollection.UpdateClusters();
						  }
						  else
								TargetClusterCollection.RefreshClusters();

						  List<CacheUnit> units=TargetClusterCollection.RetrieveAllUnits();

						  if (units.Count>0)
								ValidClusterUnits=units.Select(u => u.RAGUID).ToList();
					 }
				}

			  #region Kite & Avoid

				///<summary>
				///Tracks if kiting was used last loop.
				///</summary>
				internal bool FleeingLastTarget { get; set; }
				internal bool AvoidanceLastTarget { get; set; }
				//Kiting
				internal bool IsFleeing { get; set; }
				// Prevent spam-kiting too much - allow fighting between each kite movement
				internal DateTime timeCancelledFleeMove=DateTime.Today;
				internal int iMillisecondsCancelledFleeMoveFor=0;
				//Duration: Seconds of the kite movement
				internal int iSecondsFleeMoveFor=0;

				internal DateTime LastFleeAction=DateTime.Today;
				//Avoidance Related
				internal bool RequiresAvoidance { get; set; }
				internal bool TravellingAvoidance { get; set; }
				internal bool DontMove { get; set; }
				internal bool CriticalAvoidance { get; set; }
				internal DateTime LastAvoidanceMovement { get; set; }

				// This force-prevents avoidance for XX loops incase we get stuck trying to avoid stuff
				internal DateTime timeCancelledEmergencyMove=DateTime.Today;
				internal int iMillisecondsCancelledEmergencyMoveFor=0;
				//Duration: Seconds of the avoid movement
				internal int iSecondsEmergencyMoveFor=0;

				// This lets us know if there is a target but it's in avoidance so we can just "stay put" until avoidance goes
				internal bool bStayPutDuringAvoidance=false;


				#endregion


				// Variables used to actually hold powers the power-selector has picked to use, for buffing and main power use
			  internal SNOPower powerLastSnoPowerUsed { get; set; }

				//Loot Check
				internal bool ShouldCheckItemLooted { get; set; }
				internal int recheckCount { get; set; }
				internal bool reCheckedFinished { get; set; }


				#region HandlerFlags

				// A flag to indicate whether we have a new target from the overlord (decorator) or not, in which case don't refresh targets again this first loop
				internal bool bWholeNewTarget { get; set; }
				// A flag to indicate if we should pick a new power/ability to use or not
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
				// This holds whether or not we want to prioritize a close-target, used when we might be body-blocked by monsters
				internal bool bForceCloseRangeTarget { get; set; }
				#endregion

				// how long to force close-range targets for
				internal int iMillisecondsForceCloseRange=0;
				// Date time we were last told to stick to close range targets
				internal DateTime lastForcedKeepCloseRange=DateTime.Today;


				// Variables relating to quick-reference of monsters within sepcific ranges (if anyone has suggestion for similar functionality with reduced CPU use, lemme know, but this is fast atm!)
				internal int[] iElitesWithinRange;
				internal int[] iAnythingWithinRange;

				internal int iNonRendedTargets_6 { get; set; }
				internal bool UsesDOTDPSAbility { get; set; }
				internal int SurroundingUnits { get; set; }


				#region Last seen objects
				// Last had any mob in range, for loot-waiting
				internal DateTime lastHadUnitInSights { get; set; }
				// When we last saw a boss/elite etc.
				internal DateTime lastHadEliteUnitInSights { get; set; }
				//Last time we had a container, for loot-waiting
				internal DateTime lastHadContainerAsTarget { get; set; }
				//When we last saw a "rare" chest
				internal DateTime lastHadRareChestAsTarget { get; set; }
				// Store the date-time when we *FIRST* picked this target, so we can blacklist after X period of time targeting
				internal DateTime dateSincePickedTarget { get; set; }
				internal int iTotalNumberGoblins=0;
				internal DateTime lastGoblinTime=DateTime.Today;
				#endregion


				internal int iACDGUIDLastWhirlwind=0;
				internal Vector3 vSideToSideTarget=Vector3.Zero;
				internal DateTime lastChangedZigZag=DateTime.Today;
				internal Vector3 vPositionLastZigZagCheck=Vector3.Zero;

				internal bool bAnyChampionsPresent { get; set; }
				internal bool bAnyTreasureGoblinsPresent { get; set; }
				internal bool bAnyMobsInCloseRange { get; set; }
				internal bool bAnyBossesInRange { get; set; }
				// A flag to say whether any NONE-hashActorSNOWhirlwindIgnore things are around
				internal bool bAnyNonWWIgnoreMobsInRange { get; set; }
				/// <summary>
				/// Check LoS if waller avoidance detected
				/// </summary>
				internal bool bCheckGround=false;


				internal string DebugString
				{
					 get
					 {
						  string strDebug_LAST=string.Format("LastPickedTarget {0} -- LastHealthChanged {1} // HealthDrop {2}\r\n"+
																		  "lastHadUnitInSights {3} -- lastHadEliteUnitInSights {4} -- lastHadContainerAsTarget {5} -- lastHadRareChestAsTarget {6}\r\n"+
																		  "LastAvoidanceMovement {7}",
																		  this.dateSincePickedTarget.ToString(), "", "",
																		  this.lastHadUnitInSights.ToString(), this.lastHadEliteUnitInSights.ToString(), this.lastHadContainerAsTarget.ToString(), this.lastHadRareChestAsTarget.ToString(), this.LastAvoidanceMovement.ToString());

						  string strDebug_ClusterTarget=string.Format("LastClusterTargetLogicRefresh {0}\r\n"+
																					 "CurrentTargetClusters Count {1} -- ValidClusterUnits Count {2}",
																					 this.LastClusterTargetLogicRefresh.ToString(), this.TargetClusterCollection.CurrentClusters.Count, this.ValidClusterUnits.Count);
						  string currentgroupclusterinfo=String.Empty;
						  if (this.CurrentGroupClusters.Count>0)
						  {
								foreach (var item in CurrentGroupClusters)
							  {
									currentgroupclusterinfo=currentgroupclusterinfo+"Units=["+item.ListUnits.Count.ToString()+"] -- Flags: "+item.Info.Properties.ToString()+"\r\n";
							  }
						  }
						  string strDebug_ClusterGroup=string.Format("LastClusterGroupingLogicRefresh {0}\r\n"+
																					 "CurrentGroupClusters Count {1} -- DistantUnits Count {2} \r\n {3}",
																					 this.LastClusterGroupingLogicRefresh.ToString(), this.CurrentGroupClusters.Count, this.DistantUnits.Count, currentgroupclusterinfo);

						  string strDebug_ClusterAbility=string.Format("Total ability UnitCluster Entries {0}", this.AbilityClusters.Keys.ToString());



						  return String.Format("Combat Cache\r\n {0} \r\n {1} \r\n {2} \r\n {3}", strDebug_LAST, strDebug_ClusterTarget, strDebug_ClusterGroup, strDebug_ClusterAbility);
					 }
				}






				internal void Reset()
				{
					 iElitesWithinRange=new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
					 iAnythingWithinRange=new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
					 iNonRendedTargets_6=0;
					 bAnyBossesInRange=false;
					 bAnyChampionsPresent=false;
					 bAnyTreasureGoblinsPresent=false;
					 bAnyMobsInCloseRange=false;
					 bAnyNonWWIgnoreMobsInRange=false;
					 TravellingAvoidance=false;
					 UnitRAGUIDs=new List<int>();
					 SurroundingUnits=0;
					 TriggeringAvoidances.Clear();
					 IsFleeing=false;
					 UsesDOTDPSAbility=false;
					 bCheckGround=false;
					 NearbyAvoidances.Clear();
					 NearbyObstacleObjects.Clear();
					 FleeTriggeringUnits.Clear();
					 DistantUnits.Clear();
					 LoSMovementUnits.Clear();
				}
				internal void ResetTargetHandling()
				{
					 Bot.Target.CurrentTarget=null;
					 //Bot.NavigationCache.ResetPathing();
					 FunkyTrinity.Movement.TargetMovement.ResetTargetMovementVars();


					 bWaitingForPower=false;
					 bWaitingAfterPower=false;
					 bWaitingForPotion=false;
					 bWasRootedLastTick=false;
					 recheckCount=0;
					 reCheckedFinished=false;

				}
		  }

	 

}