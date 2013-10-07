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
		  /*
			Line of Sight Movement Behavior
		   --Units that are "special" that fail the Line of Sight Check during ObjectIsValidForTargeting will be added to a special list of units that we check here.
		   --We use the list of units to compute clusters
 		   --We iterate the units searching for any that has <=75% HP and CanFullyClientPathTo. 
		   --Once we find a valid unit, we generate the path and let the target handler begin the movement.
		   

		   Note: This behavior only activates when no targets are found during refresh.
				(Excluding Non-Movement targets)


		  */

		  public TBLOSMovement() : base() { }
		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.LineOfSight; } }
		  public override bool BehavioralCondition
		  {
				get
				{
					 //Check objects added for LOS movement
					 return Bot.Combat.LoSMovementUnits.Count>0||Bot.NavigationCache.LOSmovementUnit!=null;
				}
		  }

		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				{
					 if (obj==null)
					 {
						  if (Bot.NavigationCache.LOSmovementUnit==null||!Bot.NavigationCache.LOSmovementUnit.IsStillValid())
						  {
								if (FunkyTrinity.Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
								{
									 if (Bot.NavigationCache.LOSmovementUnit==null)
										  Logger.Write(LogLevel.Movement, "LOS Unit is Null -- Reseting.");
									 else
									 	 Logger.Write(LogLevel.Movement, "LOS Unit is No Longer Valid -- Reseting.");
								}
									

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
												{
													 Logger.Write(LogLevel.Movement, "Line of Sight Started for object {0} -- with {1} vectors", unit.InternalName, Navigation.NP.CurrentPath.Count);
												}
													 

												Bot.NavigationCache.LOSmovementUnit=unit;
												
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
								{
									 Bot.NavigationCache.LOSVector=Bot.NavigationCache.LOSmovementUnit.Position;
								}
								Navigation.NP.MoveTo(Bot.NavigationCache.LOSVector, "LineOfSightMoveTo", true);
									

								obj=new CacheObject(Bot.NavigationCache.LOSVector, TargetType.LineOfSight, 1d, "Line Of Sight", 5f, NavUnit.RAGUID);
								return true;
						  }
					 }

					 return false;
				};
		  }
	 }
}