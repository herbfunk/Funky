using System;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using fBaseXtensions.Navigation.Clustering;

namespace fBaseXtensions.Targeting.Behaviors
{
	public class TBGrouping : TargetBehavior
	{
		/*
		  Grouping Behavior
		  -Setting Enabled, Bot Health is at least the Minimum Set and current target is an unit.
		 -We use a list of units we found to be out of targeting range but within grouping range.
		 -With this list we generate clusters and then check the properties for any that has flags we desire.
		 -
		*/
		public TBGrouping() : base() { }

		public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Grouping; } }
		public override bool BehavioralCondition
		{
			get
			{
				return !FunkyGame.IsInNonCombatBehavior &&
						   !FunkyGame.Targeting.Cache.Environment.bAnyLootableItemsNearby &&
						   FunkyBaseExtension.Settings.Grouping.AttemptGroupingMovements &&
						   FunkyBaseExtension.Settings.Grouping.GroupingMinimumBotHealth <= FunkyGame.Hero.dCurrentHealthPct;
			}
		}
		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			{
				//Final Possible Target Check
				if (obj != null)
				{
					if (obj.targetType.Equals(TargetType.Unit))
					{
						//Update CurrentUnitTarget Variable.
						CacheUnit unitObj = (CacheUnit)obj;

						//Grouping Movements
						if (FunkyBaseExtension.Settings.Grouping.AttemptGroupingMovements
							 && unitObj.CurrentHealthPct.Value < 1d //only after we engaged the target.
							 && !unitObj.BeingIgnoredDueToClusterLogic && !unitObj.IsClusterException //we only want a cluster target!
							 && DateTime.Compare(DateTime.Now, FunkyGame.Navigation.groupingSuspendedDate) > 0
							 && !FunkyGame.Targeting.Cache.Environment.bAnyTreasureGoblinsPresent || FunkyBaseExtension.Settings.Targeting.GoblinPriority < 2)
						{
							FunkyGame.Targeting.Cache.Clusters.UpdateGroupClusteringVariables();

							if (FunkyGame.Targeting.Cache.Clusters.CurrentGroupClusters.Count > 0)
							{

								foreach (UnitCluster cluster in FunkyGame.Targeting.Cache.Clusters.CurrentGroupClusters)
								{
									//Validate the cluster is something worthy..
									if (!CheckCluster(cluster)) continue;


									//Validate that our target will not intersect avoidances?
									CacheUnit groupUnit = cluster.GetNearestUnitToCenteroid();
									if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(groupUnit.Position))
									{
										groupUnit = cluster.ListUnits[0];
										if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(groupUnit.Position)) continue;
									}


									Logger.Write(LogLevel.Grouping, "Starting Grouping Behavior");

									//Activate Behavior
									FunkyGame.Navigation.groupRunningBehavior = true;
									FunkyGame.Navigation.groupingOrginUnit = (CacheUnit)ObjectCache.Objects[obj.RAGUID];

									Logger.Write(LogLevel.Grouping, "Group Cluster Propeties {0}", cluster.Info.Properties.ToString());

									//Find initial grouping target..
									obj = groupUnit;
									FunkyGame.Navigation.groupingCurrentUnit = groupUnit;
									FunkyGame.Navigation.groupingUnitCluster = cluster;
									return true;
								}

							}
						}
					}
				}
				return false;
			};
		}

		private delegate bool CheckCurrentCluster(UnitCluster cluster);
		private readonly CheckCurrentCluster CheckCluster = (UnitCluster cluster) =>
		{
			//if (FunkyBaseExtension.SettingsFunky.LogGroupingOutput)
			//    Logger.Write(LogLevel.Grouping, "Current Unit Cluster Propeties [{0}]", cluster.Info.Properties.ToString());

			return ((!FunkyBaseExtension.Settings.Targeting.IgnoreAboveAverageMobs
							&& (FunkyBaseExtension.Settings.Grouping.GroupingClusterProperties.HasFlag(ClusterProperties.Elites) && cluster.Info.Properties.HasFlag(ClusterProperties.Elites) ||
							   (FunkyBaseExtension.Settings.Grouping.GroupingClusterProperties.HasFlag(ClusterProperties.Boss) && cluster.Info.Properties.HasFlag(ClusterProperties.Boss)))) ||
						(FunkyBaseExtension.Settings.Grouping.GroupingClusterProperties.HasFlag(ClusterProperties.Large) && cluster.Info.Properties.HasFlag(ClusterProperties.Large)) ||
						(FunkyBaseExtension.Settings.Grouping.GroupingClusterProperties.HasFlag(ClusterProperties.Strong) && cluster.Info.Properties.HasFlag(ClusterProperties.Strong)) ||
						(FunkyBaseExtension.Settings.Grouping.GroupingClusterProperties.HasFlag(ClusterProperties.Fast) && cluster.Info.Properties.HasFlag(ClusterProperties.Fast)));
		};

	}



}
