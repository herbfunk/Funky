using System;
using System.Collections.Generic;
using System.Linq;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using fBaseXtensions.Navigation.Clustering;
using fBaseXtensions.Settings;

namespace fBaseXtensions.Targeting
{
	public class Clustering
	{
		public Clustering()
		{
			TargetClusterCollection = new ClusterTargetCollection(TargetClusterConditions);
		}


		private readonly ClusterConditions TargetClusterConditions = new ClusterConditions(SettingCluster.ClusterSettingsTag.ClusterDistance, SettingCluster.ClusterSettingsTag.ClusterMaxDistance, SettingCluster.ClusterSettingsTag.ClusterMinimumUnitCount, false);
		private readonly ClusterTargetCollection TargetClusterCollection;
		internal List<int> ValidClusterUnits = new List<int>();
		internal List<UnitCluster> CurrentGroupClusters = new List<UnitCluster>();




		private DateTime LastClusterGroupingLogicRefresh = DateTime.MinValue;

		private Dictionary<ClusterConditions, ClusterCollection> AbilityClusters = new Dictionary<ClusterConditions, ClusterCollection>();
		///<summary>
		///Tracks Lists of Clusters for specific Conditions used during Ability Clustering.
		///</summary>
		internal List<UnitCluster> AbilityClusterCache(ClusterConditions CC)
		{
			if (!AbilityClusters.ContainsKey(CC))
			{
				Logger.Write(LogLevel.Cluster, "Creating new entry for ClusterConditions -- {0}", CC.ToString());
				AbilityClusters.Add(CC, new ClusterCollection(CC, 400, 200));
			}

			if (AbilityClusters[CC].ShouldUpdateClusters)
				AbilityClusters[CC].UpdateClusters();
			else
				AbilityClusters[CC].RefreshClusters();

			//Logger.DBLog.InfoFormat("Ability Clusters Found {0}", AbilityClusters[CC].Count.ToString());
			return AbilityClusters[CC].CurrentClusters;
		}


		internal void UpdateGroupClusteringVariables()
		{
			//grouping clustering
			if (FunkyBaseExtension.Settings.Grouping.AttemptGroupingMovements)
			{
				if ((CurrentGroupClusters.Count == 0 && DateTime.Compare(LastClusterGroupingLogicRefresh, DateTime.Now) < 0)
					 || (CurrentGroupClusters.Count > 0 && DateTime.Now.Subtract(LastClusterGroupingLogicRefresh).TotalMilliseconds > 1250))
				{
					//Clear Clusters and Unit collection
					CurrentGroupClusters = new List<UnitCluster>();

					//Check if there are enough units present currently..
					if (FunkyGame.Targeting.Cache.Environment.DistantUnits.Count > FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitsInCluster)
					{

						//Update UnitCluster Collection
						CurrentGroupClusters = UnitCluster.RunKmeans(FunkyGame.Targeting.Cache.Environment.DistantUnits, FunkyBaseExtension.Settings.Grouping.GroupingClusterRadiusDistance)
							 .Where(cluster => cluster.ListUnits.Count >= FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitsInCluster && cluster.NearestMonsterDistance <= FunkyBaseExtension.Settings.Grouping.GroupingMaximumDistanceAllowed)
							 .OrderByDescending(cluster => cluster.NearestMonsterDistance).ToList();

						//if (FunkyBaseExtension.SettingsFunky.LogGroupingOutput)
						//    Logger.Write(LogLevel.Cluster, "Updated Group Clusters. Count={0}", CurrentGroupClusters.Count.ToString());

						LastClusterGroupingLogicRefresh = DateTime.Now;
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
			if (SettingCluster.ClusterSettingsTag.EnableClusteringTargetLogic &&
				 (FunkyGame.Hero.dCurrentHealthPct > SettingCluster.ClusterSettingsTag.IgnoreClusterLowHPValue))
			{
				if (FunkyGame.Targeting.Cache.Environment.UnitRAGUIDs.Count >= SettingCluster.ClusterSettingsTag.ClusterMinimumUnitCount)
				{
					if (TargetClusterCollection.ShouldUpdateClusters)
						TargetClusterCollection.UpdateClusters();
				}
				else
					TargetClusterCollection.RefreshClusters();

				List<CacheUnit> units = TargetClusterCollection.RetrieveAllUnits();

				if (units.Count > 0)
					ValidClusterUnits = units.Select(u => u.RAGUID).ToList();
			}
		}


	}
}
