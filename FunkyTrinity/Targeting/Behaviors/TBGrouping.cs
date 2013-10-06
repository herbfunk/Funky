using System;
using System.Collections.Generic;
using System.Linq;
using FunkyTrinity.Cache;
using FunkyTrinity.Cache.Enums;
using FunkyTrinity.Movement;
using FunkyTrinity.Movement.Clustering;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Targeting.Behaviors
{
	 public class TBGrouping : TargetBehavior
	 {
		  public TBGrouping() : base() { }

		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Grouping; } }
		  public override bool BehavioralCondition
		  {
				get
				{
					 return !Bot.IsInNonCombatBehavior&&
								!Bot.Combat.bAnyLootableItemsNearby&&
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
								if (FunkyTrinity.Bot.Settings.Grouping.AttemptGroupingMovements
									 &&unitObj.CurrentHealthPct.Value<1d //only after we engaged the target.
									 &&!unitObj.BeingIgnoredDueToClusterLogic&&!unitObj.IsClusterException //we only want a cluster target!
									 &&DateTime.Compare(DateTime.Now, FunkyTrinity.Bot.NavigationCache.groupingSuspendedDate)>0
									 &&!Bot.Combat.bAnyTreasureGoblinsPresent||FunkyTrinity.Bot.Settings.Targeting.GoblinPriority<2)
								{
									 FunkyTrinity.Bot.Combat.UpdateGroupClusteringVariables();

									 if (Bot.Combat.CurrentGroupClusters.Count>0)
									 {

										  foreach (UnitCluster cluster in Bot.Combat.CurrentGroupClusters)
										  {
												//Validate the cluster is something worthy..
												if (!CheckCluster(cluster)) continue;

												//Validate that our target will not intersect avoidances?
												CacheUnit groupUnit=cluster.ListUnits[0];
												if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(obj.Position)) continue;

												if (FunkyTrinity.Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
													 Logger.Write(LogLevel.Grouping, "Starting Grouping Behavior");

												//Activate Behavior
												FunkyTrinity.Bot.NavigationCache.groupRunningBehavior=true;
												FunkyTrinity.Bot.NavigationCache.groupingOrginUnit=(CacheUnit)ObjectCache.Objects[obj.RAGUID];

												if (FunkyTrinity.Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Grouping))
													 Logger.Write(LogLevel.Grouping, "Group Cluster Propeties {0}", cluster.Info.Properties.ToString());

												//Find initial grouping target..
												obj=cluster.ListUnits[0];
												Bot.Targeting.CurrentUnitTarget=(CacheUnit)obj;
												FunkyTrinity.Bot.NavigationCache.groupingCurrentUnit=Bot.Targeting.CurrentUnitTarget;
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

				return ((!FunkyTrinity.Bot.Settings.Targeting.IgnoreAboveAverageMobs&&(cluster.Info.Properties.HasFlag(ClusterProperties.Elites)||cluster.Info.Properties.HasFlag(ClusterProperties.Boss)))||
																					 cluster.Info.Properties.HasFlag(ClusterProperties.Large)||
																					 cluster.Info.Properties.HasFlag(ClusterProperties.Strong)||
																					 cluster.Info.Properties.HasFlag(ClusterProperties.Fast));
		  };

		  private delegate UnitCluster CheckGroups(List<UnitCluster> clusters);
		  private CheckGroups GroupingTest=(List<UnitCluster> clusters) =>
		  {
				var PossibleGroups=clusters
						  .Where(c =>
								(!FunkyTrinity.Bot.Settings.Targeting.IgnoreAboveAverageMobs&&(c.Info.Properties.HasFlag(ClusterProperties.Elites))||c.Info.Properties.HasFlag(ClusterProperties.Boss))||
								c.Info.Properties.HasFlag(ClusterProperties.Large)||
								c.Info.Properties.HasFlag(ClusterProperties.Strong)||
								c.Info.Properties.HasFlag(ClusterProperties.Ranged)||
								c.Info.Properties.HasFlag(ClusterProperties.Fast));

				if (PossibleGroups.Any())
				{
					 return PossibleGroups.First();
				}

				return null;
		  };
	 }



}
