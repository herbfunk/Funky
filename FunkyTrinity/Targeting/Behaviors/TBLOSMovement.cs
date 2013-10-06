using System;
using System.Linq;
using System.Collections.Generic;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;
using FunkyTrinity.Movement.Clustering;
using FunkyTrinity.Cache.Enums;
using Zeta.Common;
using Zeta.Navigation;
namespace FunkyTrinity.Targeting.Behaviors
{
	 public class TBLOSMovement : TargetBehavior
	 {
		  public TBLOSMovement() : base() { }
		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.LineOfSight; } }
		  public override bool BehavioralCondition
		  {
				get
				{
					 //Check objects added for LOS movement
					 return Bot.Combat.LoSMovementUnits.Count>0;
				}
		  }

		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				{
					 /*LOS Movement
					  -->Validate last used LOS Unit (if any)
					  -->No Previous Unit Set?
							-->Create clusters of all units in the collection. (so we can eliminate most targets!)
							-->Sort by distance, select closest.
					  Return Any unit set?
					 */

					 if (obj==null)
					 {
						  if (Bot.NavigationCache.LOSmovementUnit==null||!Bot.NavigationCache.LOSmovementUnit.IsStillValid())
						  {
								Navigation.NP.Clear();
								Bot.NavigationCache.LOSVector=Vector3.Zero;
								Bot.NavigationCache.LOSmovementUnit=null;
						  }
							

						  if (Bot.NavigationCache.LOSmovementUnit==null)
						  {
								//New LOS Movement Selection.
								List<UnitCluster> ValidLOSUnitClusters=new List<UnitCluster>();
								ValidLOSUnitClusters=UnitCluster.RunKmeans(Bot.Combat.LoSMovementUnits, 20).OrderBy(cluster => cluster.NearestMonsterDistance).ToList();
								foreach (var validLosUnitCluster in ValidLOSUnitClusters)
								{
									 foreach (var unit in validLosUnitCluster.ListUnits)
									 {
										  if (unit.CurrentHealthPct.Value>0.75d)
										  {
												if (!Navigation.NP.CanFullyClientPathTo(unit.Position)) continue;

												if (FunkyTrinity.Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
													 Logger.Write(LogLevel.Movement, "Line of Sight Started for object {0}", unit.InternalName);

												Bot.NavigationCache.LOSmovementUnit=(CacheUnit)ObjectCache.Objects[unit.RAGUID];
												
												break;
										  }
									 }
									 if (Bot.NavigationCache.LOSmovementUnit!=null)
										  break;
								}
						  }

						  if (Bot.NavigationCache.LOSmovementUnit!=null)
						  {
								CacheUnit NavUnit=Bot.NavigationCache.LOSmovementUnit;
								if (Bot.NavigationCache.LOSVector==Vector3.Zero)
									 Bot.NavigationCache.LOSVector=Bot.NavigationCache.LOSmovementUnit.BotMeleeVector;

								obj=new CacheObject(Bot.NavigationCache.LOSVector, TargetType.LineOfSight, 1d, "Line Of Sight", 5f, NavUnit.RAGUID);
								Navigation.NP.MoveTo(Bot.NavigationCache.LOSVector, "LineOfSight");
								return true;
						  }
					 }

					 return false;
				};
		  }
	 }
}