using System.Collections.Generic;
using System.Linq;
using FunkyBot.Cache.Objects;

namespace FunkyBot.Movement.Clustering
{
	internal abstract class cluster
	{
		public List<GridPoint> ListPoints { get; protected set; }
		public double Dist { get; protected set; }
		internal List<CacheObject> ListCacheObjects { get; set; }
		internal List<int> RAGUIDS { get; set; }
		protected GridPoint MidPoint;

		public GridPoint Midpoint
		{
			get
			{
				return MidPoint / ListPoints.Count;
			}
		}

		#region Constructors

		protected cluster()
		{
			RAGUIDS = new List<int>();
			ListPoints = new List<GridPoint>();
			ListCacheObjects = new List<CacheObject>();

		}  // of parameterless constructor

		protected cluster(double p_Dist)
			: this()
		{
			Dist = p_Dist;

		}  // of overloaded constructor

		public cluster(double p_Dist, CacheObject obj)
			: this(p_Dist)
		{
			ListPoints.Add(obj.PointPosition);
			MidPoint = obj.PointPosition;

		}  // of overloaded constructor


		#endregion

		private bool ContainsObject(CacheObject obj)
		{
			bool u_Exists = false;

			if (RAGUIDS.Contains(obj.RAGUID))
				u_Exists = true;

			return u_Exists;
		}


		/// <summary>
		/// Adds point to this cluster only if it is "reachable"
		/// (if point is inside a circle of radius Dist of any cluster's points )
		/// </summary>
		/// <returns>false if point can't be added (that is either already in cluster
		/// or it is unreachable from any of the cluster's points)</returns>
		internal virtual bool AddObject(CacheObject obj)
		{
			bool l_bSuccess = true;

			if (!ContainsObject(obj) && IsPointReachable(obj.PointPosition))
			{
				ListCacheObjects.Add(obj);
				ListPoints.Add(obj.PointPosition);
				RAGUIDS.Add(obj.RAGUID);
				MidPoint += obj.PointPosition;
			}
			else
				l_bSuccess = false;

			return l_bSuccess;

		}  // of AddPoint()

		/// <summary>
		/// is point inside a circle of radius Dist of any of the cluster's points?
		/// </summary>
		/// <param name="p_Point"></param>
		/// <returns>true if point is inside a circle of radius Dist of any of the cluster's points</returns>
		public bool IsPointReachable(GridPoint p_Point)
		{
			if (ListPoints.FindAll(x => x.Distance(p_Point) <= Dist).Count > 0)
				return true;
			else
				return false;

		}  // of IsPointReachable()

		/// <summary>
		/// Incorporates all points from given cluster to this cluster
		/// </summary>
		/// <param name="p_Cluster"></param>
		/// <returns>true always</returns>
		public virtual bool AnnexCluster(cluster p_Cluster)
		{
			MidPoint += p_Cluster.MidPoint;
			ListPoints.AddRange(p_Cluster.ListPoints);
			RAGUIDS.AddRange(p_Cluster.RAGUIDS);
			return true;

		}  // of AnnexCluster()

		public static UnitCluster MergeClusters(UnitCluster p_C1, UnitCluster p_C2)
		{
			if (p_C2.ListPoints.Count > 0)
				p_C1.AnnexCluster(p_C2);

			return p_C1;

		}  // of MergeClusters()

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			cluster p = (cluster)obj;
			return Midpoint.Equals(p.Midpoint);
		}
		public override int GetHashCode()
		{
			return Midpoint.GetHashCode();
		}

		public CacheObject GetNearestObjectToCenteroid()
		{
			double minimumDistance = 0.0;
			int nearestPointIndex = -1;
			GridPoint centeroid = Midpoint;

			foreach (GridPoint p in ListPoints)
			{
				double distance = GridPoint.GetDistanceBetweenPoints(p, centeroid);

				if (ListPoints.IndexOf(p) == 0)
				{
					minimumDistance = distance;
					nearestPointIndex = ListPoints.IndexOf(p);
				}
				else
				{
					if (minimumDistance > distance)
					{
						minimumDistance = distance;
						nearestPointIndex = ListPoints.IndexOf(p);
					}
				}
			}

			return (ListCacheObjects[nearestPointIndex]);
		}

		public static List<UnitCluster> RunKmeans<T>(List<T> objList, double distance) where T : CacheObject
		{
			List<UnitCluster> LC_ = new List<UnitCluster>();

			if (objList.Count == 0)
				return LC_;

			List<CacheObject> l_ListUnits = new List<CacheObject>(objList.ToArray());

			if (l_ListUnits.Count == 0)
				return LC_;



			// for starters, take a point to create one cluster
			CacheUnit l_P1 = (CacheUnit)l_ListUnits[0];

			l_ListUnits.RemoveAt(0);

			// so far, we have a one-point cluster
			LC_.Add(new UnitCluster(distance, l_P1));

			#region Main Loop
			// the algorithm is inside this loop
			List<UnitCluster> l_ListAttainableClusters;
			UnitCluster l_c;
			foreach (CacheUnit p in l_ListUnits)
			{
				l_ListAttainableClusters = new List<UnitCluster>();
				l_ListAttainableClusters = LC_.FindAll(x => x.IsPointReachable(p.PointPosition));
				LC_.RemoveAll(x => x.IsPointReachable(p.PointPosition));
				l_c = new UnitCluster(distance, p);
				// merge point's "reachable" clusters
				if (l_ListAttainableClusters.Count > 0)
					l_c.AnnexCluster(l_ListAttainableClusters.Aggregate((c, x) =>
					  c = MergeClusters(x, c)));
				LC_.Add(l_c);
				//Logger.DBLog.InfoFormat("Cluster Found: Total Points {0} with Centeroid {1}", l_c.ListPoints.Count, l_c.Centeroid.ToString());
				l_ListAttainableClusters = null;
				l_c = null;
			}  // of loop over candidate points

			//LC_=LC_.OrderByDescending(o => o.ListPoints.Count).ToList();
			#endregion

			return LC_;
		}

	}

	internal class Cluster : cluster
	{
		public Cluster(double Dist) : base(Dist) { }
	}
}  // of namespace BasicLibrary
