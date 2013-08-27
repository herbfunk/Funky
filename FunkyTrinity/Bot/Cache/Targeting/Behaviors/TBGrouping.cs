using System;
using System.Collections.Generic;
using System.Linq;
using FunkyTrinity.Cache;
using FunkyTrinity.Enums;
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
					 return FunkyTrinity.Bot.SettingsFunky.AttemptGroupingMovements;
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
								if (FunkyTrinity.Bot.SettingsFunky.AttemptGroupingMovements
									 &&unitObj.CurrentHealthPct.Value<1d //only after we engaged the target.
									 &&!unitObj.BeingIgnoredDueToClusterLogic && !unitObj.IsClusterException //we only want a cluster target!
									 &&DateTime.Compare(DateTime.Now, FunkyTrinity.Bot.NavigationCache.groupingSuspendedDate)>0
									 &&!unitObj.IsTreasureGoblin||FunkyTrinity.Bot.SettingsFunky.GoblinPriority<2) 
								{
									 FunkyTrinity.Bot.Combat.UpdateGroupClusteringVariables();

									 if (FunkyTrinity.Bot.Combat.CurrentGroupClusters.Count>0)
									 {
										  if (Bot.Combat.TargetClusterCollection.CurrentClusters.Count>0)
										  {
												//Trigger for grouping..
												if (CheckCluster(Bot.Combat.TargetClusterCollection.CurrentClusters.First()))
												{
													 UnitCluster groupCluster=GroupingTest(FunkyTrinity.Bot.Combat.CurrentGroupClusters);

													 if (groupCluster==null) return false;

													 if (FunkyTrinity.Bot.SettingsFunky.FunkyLogFlags.HasFlag(LogLevel.Grouping))
														  Logger.Write(LogLevel.Grouping, "Starting Grouping Behavior");

													 //Activate Behavior
													 FunkyTrinity.Bot.NavigationCache.groupRunningBehavior=true;
													 FunkyTrinity.Bot.NavigationCache.groupingOrginUnit=(CacheUnit)ObjectCache.Objects[obj.RAGUID];

													 //Get Cluster
													 FunkyTrinity.Bot.NavigationCache.groupingCurrentCluster=groupCluster;

													 if (FunkyTrinity.Bot.SettingsFunky.FunkyLogFlags.HasFlag(LogLevel.Grouping))
														  Logger.Write(LogLevel.Grouping, "Group Cluster Propeties {0}", groupCluster.Info.Properties.ToString());

													 //Find initial grouping target..
													 obj=groupCluster.ListUnits[0];
													 Bot.Target.CurrentUnitTarget=(CacheUnit)obj;
													 FunkyTrinity.Bot.NavigationCache.groupingCurrentUnit=Bot.Target.CurrentUnitTarget;
													 return true;

												}
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

				return ((!FunkyTrinity.Bot.SettingsFunky.IgnoreAboveAverageMobs&&(cluster.Info.Properties.HasFlag(ClusterProperties.Elites)||cluster.Info.Properties.HasFlag(ClusterProperties.Boss)))||
																					 cluster.Info.Properties.HasFlag(ClusterProperties.Large)||
																					 cluster.Info.Properties.HasFlag(ClusterProperties.Strong)||
																					 cluster.Info.Properties.HasFlag(ClusterProperties.Fast));
		  };

		  private delegate UnitCluster CheckGroups(List<UnitCluster> clusters);
		  private CheckGroups GroupingTest=(List<UnitCluster> clusters) =>
		  {
				var PossibleGroups=clusters
						  .Where(c =>
								(!FunkyTrinity.Bot.SettingsFunky.IgnoreAboveAverageMobs&&(c.Info.Properties.HasFlag(ClusterProperties.Elites))||c.Info.Properties.HasFlag(ClusterProperties.Boss))||
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
