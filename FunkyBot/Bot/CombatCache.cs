using System;
using System.Linq;
using FunkyBot.AbilityFunky;
using FunkyBot.Movement.Clustering;
using FunkyBot.Targeting;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Internals;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.SNO;
using Zeta.CommonBot;
using FunkyBot.Cache;
using FunkyBot.Movement;

namespace FunkyBot.Cache
{
	 ///<summary>
	 ///Cache of current combat variables
	 ///</summary>
	 public class CombatCache
	 {

		  public CombatCache()
		  {
				bAnyLootableItemsNearby=false;


				iElitesWithinRange=new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
				iAnythingWithinRange=new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
				iNonRendedTargets_6=0;
				bAnyBossesInRange=false;
				bAnyChampionsPresent=false;
				bAnyTreasureGoblinsPresent=false;
				bAnyNonWWIgnoreMobsInRange=false;
				SurroundingUnits=0;
				UsesDOTDPSAbility=false;
				TargetClusterCollection=new ClusterTargetCollection(TargetClusterConditions);
		  }

		  internal List<int> PrioritizedRAGUIDs=new List<int>();
		  internal List<CacheServerObject> NearbyObstacleObjects=new List<CacheServerObject>();
		  internal List<int> NearbyAvoidances=new List<int>();
		  internal List<CacheUnit> FleeTriggeringUnits=new List<CacheUnit>();
		  internal List<CacheAvoidance> TriggeringAvoidances=new List<CacheAvoidance>();
          internal List<int> TriggeringAvoidanceRAGUIDs = new List<int>();

		  internal List<int> ValidClusterUnits=new List<int>();
		  internal List<int> UnitRAGUIDs=new List<int>();
		  internal List<CacheUnit> DistantUnits=new List<CacheUnit>();
		  internal List<CacheObject> LoSMovementObjects=new List<CacheObject>();
		  private ClusterConditions TargetClusterConditions=new ClusterConditions(Bot.Settings.Cluster.ClusterDistance, 100f, Bot.Settings.Cluster.ClusterMinimumUnitCount, false);
		  internal ClusterTargetCollection TargetClusterCollection { get; set; }

		  internal List<UnitCluster> CurrentGroupClusters=new List<UnitCluster>();

		  ///<summary>
		  ///Tracks Lists of Clusters for specific Conditions used during Ability Clustering.
		  ///</summary>
		  internal Dictionary<ClusterConditions, ClusterCollection> AbilityClusters=new Dictionary<ClusterConditions, ClusterCollection>();

		  internal DateTime LastClusterTargetLogicRefresh=DateTime.MinValue;
		  internal DateTime LastClusterGroupingLogicRefresh=DateTime.MinValue;


		  //Cache last filtered list generated
		  internal List<UnitCluster> Clusters(ClusterConditions CC)
		  {
				if (!AbilityClusters.ContainsKey(CC))
				{
					 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Cluster))
						  Logger.Write(LogLevel.Cluster, "Creating new entry for ClusterConditions -- {0}", CC.ToString());
					 AbilityClusters.Add(CC, new ClusterCollection(CC, 400, 200));
				}

				if (AbilityClusters[CC].ShouldUpdateClusters)
					 AbilityClusters[CC].UpdateClusters();
				else
					 AbilityClusters[CC].RefreshClusters();

				//Logging.WriteVerbose("Ability Clusters Found {0}", AbilityClusters[CC].Count.ToString());
				return AbilityClusters[CC].CurrentClusters;
		  }


		  internal void UpdateGroupClusteringVariables()
		  {
				//grouping clustering
				if (Bot.Settings.Grouping.AttemptGroupingMovements)
				{
					 if ((CurrentGroupClusters.Count==0&&DateTime.Compare(LastClusterGroupingLogicRefresh, DateTime.Now)<0)
						  ||(CurrentGroupClusters.Count>0&&DateTime.Now.Subtract(LastClusterGroupingLogicRefresh).TotalMilliseconds>1250))
					 {
						  //Clear Clusters and Unit collection
						  CurrentGroupClusters=new List<UnitCluster>();

						  //Check if there are enough units present currently..
						  if (DistantUnits.Count>Bot.Settings.Grouping.GroupingMinimumUnitsInCluster)
						  {

								//Update UnitCluster Collection
								CurrentGroupClusters=UnitCluster.RunKmeans(DistantUnits, Bot.Settings.Grouping.GroupingClusterRadiusDistance)
									 .Where(cluster => cluster.ListUnits.Count>=Bot.Settings.Grouping.GroupingMinimumUnitsInCluster&&cluster.NearestMonsterDistance<=Bot.Settings.Grouping.GroupingMaximumDistanceAllowed)
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
				if (Bot.Settings.Cluster.EnableClusteringTargetLogic&&
					 (!Bot.Settings.Cluster.IgnoreClusteringWhenLowHP||Bot.Character.dCurrentHealthPct>Bot.Settings.Cluster.IgnoreClusterLowHPValue))
				{
					 if (UnitRAGUIDs.Count>=Bot.Settings.Cluster.ClusterMinimumUnitCount)
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



		  //Kiting
		  internal bool IsFleeing { get; set; }
		  //Duration: Seconds of the kite movement
		  internal int iSecondsFleeMoveFor=0;



		  //Duration: Seconds of the avoid movement
		  internal int iSecondsEmergencyMoveFor=0;

		  // This lets us know if there is a target but it's in avoidance so we can just "stay put" until avoidance goes
		  internal bool bStayPutDuringAvoidance=false;


		  #endregion






		  // Variables relating to quick-reference of monsters within sepcific ranges (if anyone has suggestion for similar functionality with reduced CPU use, lemme know, but this is fast atm!)
		  internal int[] iElitesWithinRange;
		  internal int[] iAnythingWithinRange;

		  internal int iNonRendedTargets_6 { get; set; }
		  internal bool UsesDOTDPSAbility { get; set; }
		  internal int SurroundingUnits { get; set; }

		  internal bool bAnyLootableItemsNearby { get; set; }
		  internal bool bAnyChampionsPresent { get; set; }
		  internal bool bAnyTreasureGoblinsPresent { get; set; }
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

					 string strDebug_ClusterAbility=string.Format("Total Ability UnitCluster Entries {0}", this.AbilityClusters.Keys.ToString());



					 return String.Format("Combat Cache\r\n {0} \r\n {1} \r\n {2} \r\n {3}", "", strDebug_ClusterTarget, strDebug_ClusterGroup, strDebug_ClusterAbility);
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

				bAnyNonWWIgnoreMobsInRange=false;
				bAnyLootableItemsNearby=false;
				bCheckGround=false;
				UsesDOTDPSAbility=false;
				IsFleeing=false;

				UnitRAGUIDs=new List<int>();
				SurroundingUnits=0;
				TriggeringAvoidances.Clear();
                TriggeringAvoidanceRAGUIDs.Clear();
				NearbyAvoidances.Clear();
				NearbyObstacleObjects.Clear();
				FleeTriggeringUnits.Clear();
				DistantUnits.Clear();
				LoSMovementObjects.Clear();
		  }
	 }



}