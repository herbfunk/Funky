using System;
using System.Linq;
using System.Collections.Generic;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;
using FunkyTrinity.Movement.Clustering;
using FunkyTrinity.Cache.Enums;
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

					 if (Bot.NavigationCache.LOSmovementUnit==null||!Bot.NavigationCache.LOSmovementUnit.IsStillValid())
						  Bot.NavigationCache.LOSmovementUnit=null;

					 if (Bot.NavigationCache.LOSmovementUnit==null)
					 {
						  //New LOS Movement Selection.
						  List<UnitCluster> ValidLOSUnitClusters=new List<UnitCluster>();
						  ValidLOSUnitClusters=UnitCluster.RunKmeans(Bot.Combat.LoSMovementUnits, 20).OrderBy(cluster => cluster.NearestMonsterDistance).ToList();
						  foreach (var validLosUnitCluster in ValidLOSUnitClusters)
						  {
								CacheUnit nearestUnit=validLosUnitCluster.GetNearestUnitToCenteroid();
								if (nearestUnit.CurrentHealthPct.Value>0.50d&&nearestUnit.IsTargetableAndAttackable)
								{
									 if (FunkyTrinity.Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
										  Logger.Write(LogLevel.Movement, "Line of Sight Started for object {0}", nearestUnit.InternalName);

									 Navigation.NP.Clear();
									 Zeta.Navigation.Navigator.SearchGridProvider.Update();
									 Bot.NavigationCache.LOSmovementUnit=(CacheUnit)ObjectCache.Objects[nearestUnit.RAGUID];
									 break;
								}
						  }
					 }

					 if (Bot.NavigationCache.LOSmovementUnit!=null)
					 {
						  obj=new CacheObject(Bot.NavigationCache.LOSmovementUnit.Position, TargetType.LineOfSight, 20000, "LOSMovement", 10f);
						  return true;
					 }

					 return false;
				};
		  }
	 }
}