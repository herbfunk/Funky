using System;
using System.Collections.Generic;
using System.Linq;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Movement.Clustering;
using FunkyBot.Movement;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.Targeting.Behaviors
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
					 return !Bot.IsInNonCombatBehavior&&
								!Bot.Targeting.Environment.bAnyLootableItemsNearby&&
								Bot.Settings.Grouping.AttemptGroupingMovements&&
								Bot.Settings.Grouping.GroupingMinimumBotHealth<=Bot.Character.dCurrentHealthPct;
				}
		  }
		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				{
					 //Final Possible Target Check
					 if (obj!=null)
					 {
						  if (obj.targetType.Equals(TargetType.Unit))
						  {
								//Update CurrentUnitTarget Variable.
								CacheUnit unitObj=(CacheUnit)obj;

								//Grouping Movements
								if (Bot.Settings.Grouping.AttemptGroupingMovements
									 &&unitObj.CurrentHealthPct.Value<1d //only after we engaged the target.
									 &&!unitObj.BeingIgnoredDueToClusterLogic&&!unitObj.IsClusterException //we only want a cluster target!
									 &&DateTime.Compare(DateTime.Now, Bot.NavigationCache.groupingSuspendedDate)>0
									 &&!Bot.Targeting.Environment.bAnyTreasureGoblinsPresent||Bot.Settings.Targeting.GoblinPriority<2)
								{
									Bot.Targeting.Clusters.UpdateGroupClusteringVariables();

									 if (Bot.Targeting.Clusters.CurrentGroupClusters.Count > 0)
									 {

										 foreach (UnitCluster cluster in Bot.Targeting.Clusters.CurrentGroupClusters)
										  {
												//Validate the cluster is something worthy..
												if (!CheckCluster(cluster)) continue;

												//Validate that our target will not intersect avoidances?
												CacheUnit groupUnit=cluster.ListUnits[0];
												if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(obj.Position)) continue;

												if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
													 Logger.Write(LogLevel.Grouping, "Starting Grouping Behavior");

												//Activate Behavior
												Bot.NavigationCache.groupRunningBehavior=true;
												Bot.NavigationCache.groupingOrginUnit=(CacheUnit)ObjectCache.Objects[obj.RAGUID];

												if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
													 Logger.Write(LogLevel.Grouping, "Group Cluster Propeties {0}", cluster.Info.Properties.ToString());

												//Find initial grouping target..
												CacheUnit unitobj=cluster.ListUnits[0];
												obj=cluster.ListUnits[0];
												Bot.NavigationCache.groupingCurrentUnit=unitobj;
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
		  private CheckCurrentCluster CheckCluster=(UnitCluster cluster) =>
		  {
				//if (Bot.SettingsFunky.LogGroupingOutput)
				//    Logger.Write(LogLevel.Grouping, "Current Unit Cluster Propeties [{0}]", cluster.Info.Properties.ToString());

				return ((!Bot.Settings.Targeting.IgnoreAboveAverageMobs
                                && (Bot.Settings.Grouping.GroupingClusterProperties.HasFlag(ClusterProperties.Elites) && cluster.Info.Properties.HasFlag(ClusterProperties.Elites) ||
								   (Bot.Settings.Grouping.GroupingClusterProperties.HasFlag(ClusterProperties.Boss)&&cluster.Info.Properties.HasFlag(ClusterProperties.Boss))))||
                            (Bot.Settings.Grouping.GroupingClusterProperties.HasFlag(ClusterProperties.Large) && cluster.Info.Properties.HasFlag(ClusterProperties.Large)) ||
                            (Bot.Settings.Grouping.GroupingClusterProperties.HasFlag(ClusterProperties.Strong) && cluster.Info.Properties.HasFlag(ClusterProperties.Strong)) ||
                            (Bot.Settings.Grouping.GroupingClusterProperties.HasFlag(ClusterProperties.Fast) && cluster.Info.Properties.HasFlag(ClusterProperties.Fast)));
		  };

	 }



}
