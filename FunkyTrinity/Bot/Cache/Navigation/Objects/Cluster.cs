using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;

namespace FunkyTrinity.Movement
{
	 internal partial class Cluster
	 {

		  public List<CacheUnit> ListUnits { get; protected set; }
		  public List<GridPoint> ListPoints { get; protected set; }
		 
		  private CacheUnit currentValidUnit=null;
		  public CacheUnit CurrentValidUnit
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

		  internal int UnitMobileCounter { get; set; }
		  internal double UnitsMovementRatio
		  {
				get
				{
					 return UnitMobileCounter/ListUnits.Count;
				}
		  }

		  public double Dist { get; protected set; }

		  public float NearestMonsterDistance { get; set; }


		  private GridPoint MidPoint;
		  public GridPoint Midpoint
		  {
				get
				{
					 return MidPoint/ListUnits.Count;
				}
		  }

		  protected Cluster()
		  {
				ListPoints=new List<GridPoint>();
				ListUnits=new List<CacheUnit>();
				EliteCount=0;
				DotDPSCount=0;
				UnitMobileCounter=0;
				NearestMonsterDistance=-1f;
				RAGUIDS=new List<int>();

		  }  // of parameterless constructor

		  protected Cluster(double p_Dist)
				: this()
		  {
				Dist=p_Dist;

		  }  // of overloaded constructor

		  public Cluster(double p_Dist, CacheUnit unit)
				: this(p_Dist)
		  {
				ListUnits.Add(unit);
				ListPoints.Add(unit.PointPosition);
				MidPoint=unit.PointPosition;

				RAGUIDS.Add(unit.RAGUID);
				NearestMonsterDistance=unit.CentreDistance;
				if (unit.MonsterElite)
					 EliteCount++;
				if (Bot.Combat.UsesDOTDPSAbility&&unit.HasDOTdps.HasValue&&unit.HasDOTdps.Value)
					 DotDPSCount++;
				if (unit.IsMoving)
					 UnitMobileCounter++;

		  }  // of overloaded constructor

		  private bool ContainsUnit(CacheUnit unit)
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
		  private bool AddUnit(CacheUnit unit)
		  {
				bool l_bSuccess=true;

				if (!ContainsUnit(unit)&&Bot.Combat.UnitRAGUIDs.Contains(unit.RAGUID))
					 if (IsPointReachable(unit.PointPosition))
					 {
						  ListUnits.Add(unit);
						  ListPoints.Add(unit.PointPosition);
						  RAGUIDS.Add(unit.RAGUID);
						  MidPoint+=unit.PointPosition;

						  float distance=unit.CentreDistance;
						  if (distance<this.NearestMonsterDistance)
								this.NearestMonsterDistance=distance;

						  if (unit.MonsterElite)
								EliteCount++;

						  if (Bot.Combat.UsesDOTDPSAbility&&unit.HasDOTdps.HasValue&&unit.HasDOTdps.Value)
								DotDPSCount++;

						  if (unit.IsMoving)
								UnitMobileCounter++;
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
		  public bool IsPointReachable(GridPoint p_Point)
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

				MidPoint+=p_Cluster.MidPoint;
				ListUnits.AddRange(p_Cluster.ListUnits);
				ListPoints.AddRange(p_Cluster.ListPoints);
				RAGUIDS.AddRange(p_Cluster.RAGUIDS);

				EliteCount+=p_Cluster.EliteCount;
				DotDPSCount+=p_Cluster.DotDPSCount;
				UnitMobileCounter+=p_Cluster.UnitMobileCounter;

				if (this.NearestMonsterDistance>p_Cluster.NearestMonsterDistance)
					 this.NearestMonsterDistance=p_Cluster.NearestMonsterDistance;

				return l_bSuccess;

		  }  // of AnnexCluster()

		  public CacheUnit GetNearestUnitToCenteroid()
		  {
				double minimumDistance=0.0;
				int nearestPointIndex=-1;
				GridPoint centeroid=this.Midpoint;

				foreach (GridPoint p in this.ListPoints)
				{
					 double distance=GridPoint.GetDistanceBetweenPoints(p, centeroid);

					 if (this.ListPoints.IndexOf(p)==0)
					 {
						  minimumDistance=distance;
						  nearestPointIndex=this.ListPoints.IndexOf(p);
					 }
					 else
					 {
						  if (minimumDistance>distance)
						  {
								minimumDistance=distance;
								nearestPointIndex=this.ListPoints.IndexOf(p);
						  }
					 }
				}

				return (this.ListUnits[nearestPointIndex]);
		  }

		  public static Cluster MergeClusters(Cluster p_C1, Cluster p_C2)
		  {
				if (p_C2.ListPoints.Count>0)
					 p_C1.AnnexCluster(p_C2);

				return p_C1;

		  }  // of MergeClusters()

		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||this.GetType()!=obj.GetType())
				{
					 return false;
				}
				else
				{
					 Cluster p=(Cluster)obj;
					 return this.Midpoint.Equals(p.Midpoint);
				}
		  }
		  public override int GetHashCode()
		  {
				return this.Midpoint.GetHashCode();
		  }

			
		 public static List<Cluster> RunKmeans<T>(List<T> unitList, double distance) where T : CacheObject
			{
				 List<Cluster> LC_=new List<Cluster>();

				 if (unitList.Count==0)
						return LC_;

				 List<CacheObject> l_ListUnits=new List<CacheObject>(unitList.ToArray());

				 if (l_ListUnits.Count==0)
						return LC_;



				 // for starters, take a point to create one cluster
				 CacheUnit l_P1=(CacheUnit)l_ListUnits[0];

				 l_ListUnits.RemoveAt(0);

				 // so far, we have a one-point cluster
				 LC_.Add(new Cluster(distance, l_P1));

				 #region Main Loop
				 // the algorithm is inside this loop
				 List<Cluster> l_ListAttainableClusters;
				 Cluster l_c;
				 foreach (CacheUnit p in l_ListUnits)
				 {
						l_ListAttainableClusters=new List<Cluster>();
						l_ListAttainableClusters=LC_.FindAll(x => x.IsPointReachable(p.PointPosition));
						LC_.RemoveAll(x => x.IsPointReachable(p.PointPosition));
						l_c=new Cluster(distance, p);
						// merge point's "reachable" clusters
						if (l_ListAttainableClusters.Count>0)
							 l_c.AnnexCluster(l_ListAttainableClusters.Aggregate((c, x) =>
								c=Cluster.MergeClusters(x, c)));
						LC_.Add(l_c);
						//Logging.WriteVerbose("Cluster Found: Total Points {0} with Centeroid {1}", l_c.ListPoints.Count, l_c.Centeroid.ToString());
						l_ListAttainableClusters=null;
						l_c=null;
				 }  // of loop over candidate points

				 //LC_=LC_.OrderByDescending(o => o.ListPoints.Count).ToList();
				 #endregion

				 return LC_;
			}


	 }  // of partial class Cluster

}  // of namespace BasicLibrary
