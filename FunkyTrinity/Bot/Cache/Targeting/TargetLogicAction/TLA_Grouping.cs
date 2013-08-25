using System;
using System.Linq;
using FunkyTrinity.Cache;
using FunkyTrinity.Enums;
using FunkyTrinity.Movement;
using FunkyTrinity.Movement.Clustering;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity
{
	 public class TLA_Grouping : TargetLogicAction
	 {
		  public override TargetActions TargetActionType { get { return TargetActions.Grouping; } }

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
								if (Bot.Target.CurrentUnitTarget==null) Bot.Target.CurrentUnitTarget=(CacheUnit)obj;

								//Grouping Movements
								if (FunkyTrinity.Bot.SettingsFunky.AttemptGroupingMovements
									 &&Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value<1d
									 &&DateTime.Compare(DateTime.Now, FunkyTrinity.Bot.NavigationCache.groupingSuspendedDate)>0
									 &&!Bot.Target.CurrentUnitTarget.IsTreasureGoblin||FunkyTrinity.Bot.SettingsFunky.GoblinPriority<2) //only after we engaged the target.
								{
									 FunkyTrinity.Bot.Combat.UpdateGroupClusteringVariables();



									 if (FunkyTrinity.Bot.Combat.CurrentGroupClusters.Count>0)
									 {
										  UnitCluster currentTargetCluster=Bot.Target.CurrentUnitTarget.CurrentTargetCluster;
										  if (currentTargetCluster!=null)
										  {
												bool ShouldTriggerBehavior=(!FunkyTrinity.Bot.SettingsFunky.IgnoreAboveAverageMobs&&currentTargetCluster.Info.Properties.HasFlag(ClusterProperties.Elites)||
																					 currentTargetCluster.Info.Properties.HasFlag(ClusterProperties.Large)||
																					 currentTargetCluster.Info.Properties.HasFlag(ClusterProperties.Strong)||
																					 currentTargetCluster.Info.Properties.HasFlag(ClusterProperties.Fast));

												//Trigger for grouping..
												if (ShouldTriggerBehavior)
												{
													 var PossibleGroups=FunkyTrinity.Bot.Combat.CurrentGroupClusters
																.Where(c =>
																	 (!FunkyTrinity.Bot.SettingsFunky.IgnoreAboveAverageMobs&&c.Info.Properties.HasFlag(ClusterProperties.Elites))||
																	 c.Info.Properties.HasFlag(ClusterProperties.Large)||
																	 c.Info.Properties.HasFlag(ClusterProperties.Strong));


													 if (PossibleGroups.Any())
													 {
														  if (FunkyTrinity.Bot.SettingsFunky.LogGroupingOutput)
																Logger.Write(LogLevel.Grouping, "Starting Grouping Behavior");

														  //Activate Behavior
														  FunkyTrinity.Bot.NavigationCache.groupRunningBehavior=true;
														  FunkyTrinity.Bot.NavigationCache.groupingOrginUnit=(CacheUnit)ObjectCache.Objects[obj.RAGUID];

														  //Get Cluster
														  UnitCluster groupCluster=PossibleGroups.First();
														  FunkyTrinity.Bot.NavigationCache.groupingCurrentCluster=groupCluster;

														  if (FunkyTrinity.Bot.SettingsFunky.LogGroupingOutput)
																Logger.Write(LogLevel.Grouping, "Group Cluster Propeties {0}", groupCluster.Info.Properties.ToString());

														  //Find initial grouping target..
														  obj=groupCluster.ListUnits[0];
														  Bot.Target.CurrentUnitTarget=(CacheUnit)obj;
														  FunkyTrinity.Bot.NavigationCache.groupingCurrentUnit=Bot.Target.CurrentUnitTarget;
													 }
												}
										  }
									 }
								}

						  }
						  return true;
					 }
					 return false;
				};
		  }
	 }
}
