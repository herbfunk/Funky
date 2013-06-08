using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunkyTrinity
{
	 internal partial class Cluster
	 {
		  public string DebugString
		  {
				get
				{
					 return "Total Points Contained "+ListPoints.Count.ToString()+" with Centeroid at "+Centeroid.ToString();
				}
		  }

		  public List<Funky.CacheUnit> ListUnits { get; protected set; }
		  public List<Funky.GridPoint> ListPoints { get; protected set; }
		 
		  private Funky.CacheUnit currentValidUnit=null;
		  public Funky.CacheUnit CurrentValidUnit
		  {
				get
				{
					 if (currentValidUnit!=null&&!currentValidUnit.ObjectIsValidForTargeting)
					 {
						  ListUnits.Remove(currentValidUnit);
						  currentValidUnit=null;
					 }
					 else if (currentValidUnit==null)
					 {
						  if (ListUnits.Count>0)
						  {
								foreach (var item in ListUnits)
								{
									 if (item.ObjectIsValidForTargeting)
									 {
										  currentValidUnit=item;
										  break;
									 }
								}
						  }
					 }

					 return currentValidUnit;
				}
		  }
		  public bool ContainsNoTagetableUnits
		  {
				get
				{
					 if (ListUnits.Count>0)
						  return ListUnits.Any(unit => unit.IsTargetableAndAttackable);

					 return true;
				}
		  }
		  internal List<int> RAGUIDS { get; set; }
		  internal int EliteCount { get; set; }
		  internal int DotDPSCount { get; set; }
		  internal double DotDPSRatio
		  {
				get
				{
					 return DotDPSCount/ListUnits.Count;
				}
		  }

		  public double Dist { get; protected set; }

		  public float NearestMonsterDistance { get; set; }

		  public Funky.GridPoint Centeroid
		  {
				get
				{
					 int TotalCount=ListUnits.Count;
					 Funky.GridPoint SumPoint=ListUnits[0].PointPosition.Clone();
					 for (int i=1; i<ListUnits.Count-1; i++)
					 {
						  SumPoint+=ListUnits[i].PointPosition;
					 }
					 return SumPoint/TotalCount;
				}
		  }

		  public double AccumulatedWeight
		  {
				get
				{
					 return ListUnits.Sum(u => u.Weight);
				}
		  }

		  protected Cluster()
		  {
				ListPoints=new List<Funky.GridPoint>();
				ListUnits=new List<Funky.CacheUnit>();
				EliteCount=0;
				DotDPSCount=0;
				NearestMonsterDistance=-1f;
				RAGUIDS=new List<int>();

		  }  // of parameterless constructor

		  protected Cluster(double p_Dist)
				: this()
		  {
				Dist=p_Dist;

		  }  // of overloaded constructor

		  public Cluster(double p_Dist, Funky.CacheUnit unit)
				: this(p_Dist)
		  {
				ListUnits.Add(unit);
				ListPoints.Add(unit.PointPosition);
				RAGUIDS.Add(unit.RAGUID);
				NearestMonsterDistance=unit.CentreDistance;
				if (unit.MonsterElite)
					 EliteCount++;
				if (Funky.Bot.Combat.UsesDOTDPSAbility&&unit.HasDOTdps.HasValue&&unit.HasDOTdps.Value)
					 DotDPSCount++;

		  }  // of overloaded constructor

		  public bool ContainsPoint(Funky.GridPoint p_Point)
		  {
				bool l_Exists=false;

				if (ListPoints.Contains((p_Point), new PointComparer()))
					 l_Exists=true;

				return l_Exists;

		  }  // of ContainsPoint()

		  public bool ContainsUnit(Funky.CacheUnit unit)
		  {
				bool u_Exists=false;

				if (RAGUIDS.Contains(unit.RAGUID))
					 u_Exists=true;

				return u_Exists;
		  }

		  /// <summary>
		  /// Adds point to this cluster only if it is "reachable"
		  /// (if point is inside a circle of radius Dist of any cluster's points )
		  /// </summary>
		  /// <param name="p_Point">The point to be added to this cluster</param>
		  /// <returns>false if point can't be added (that is either already in cluster
		  /// or it is unreachable from any of the cluster's points)</returns>
		  public bool AddUnit(Funky.CacheUnit unit)
		  {
				bool l_bSuccess=true;

				if (!ContainsUnit(unit)&&Funky.Bot.Combat.UnitRAGUIDs.Contains(unit.RAGUID))
					 if (IsPointReachable(unit.PointPosition))
					 {
						  ListUnits.Add(unit);
						  ListPoints.Add(unit.PointPosition);
						  RAGUIDS.Add(unit.RAGUID);

						  float distance=unit.CentreDistance;
						  if (distance<this.NearestMonsterDistance)
								this.NearestMonsterDistance=distance;

						  if (unit.MonsterElite)
								EliteCount++;

						  if (Funky.Bot.Combat.UsesDOTDPSAbility&&unit.HasDOTdps.HasValue&&unit.HasDOTdps.Value)
								DotDPSCount++;
					 }
					 else
						  l_bSuccess=false;

				return l_bSuccess;

		  }  // of AddPoint()

		  /// <summary>
		  /// is point inside a circle of radius Dist of any of the cluster's points?
		  /// </summary>
		  /// <param name="p_Point"></param>
		  /// <returns>true if point is inside a circle of radius Dist of any of the cluster's points</returns>
		  public bool IsPointReachable(Funky.GridPoint p_Point)
		  {
				if (ListPoints.FindAll(x => x.Distance(p_Point)<=Dist).Count>0)
					 return true;
				else
					 return false;

		  }  // of IsPointReachable()

		  /// <summary>
		  /// Incorporates all points from given cluster to this cluster
		  /// </summary>
		  /// <param name="p_Cluster"></param>
		  /// <returns>true always</returns>
		  public bool AnnexCluster(Cluster p_Cluster)
		  {
				bool l_bSuccess=true;

				ListUnits.AddRange(p_Cluster.ListUnits);
				ListPoints.AddRange(p_Cluster.ListPoints);
				RAGUIDS.AddRange(p_Cluster.RAGUIDS);
				EliteCount+=p_Cluster.EliteCount;
				DotDPSCount+=p_Cluster.DotDPSCount;
				if (this.NearestMonsterDistance>p_Cluster.NearestMonsterDistance)
					 this.NearestMonsterDistance=p_Cluster.NearestMonsterDistance;

				return l_bSuccess;

		  }  // of AnnexCluster()

		  public static Cluster MergeClusters(Cluster p_C1, Cluster p_C2)
		  {
				if (p_C2.ListPoints.Count>0)
					 p_C1.AnnexCluster(p_C2);

				return p_C1;

		  }  // of MergeClusters()

	 }  // of partial class Cluster

}  // of namespace BasicLibrary
