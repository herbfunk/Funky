using System.Collections.Generic;
using FunkyBot.AbilityFunky;
using FunkyBot.Cache;

namespace FunkyBot.Movement.Clustering
{
	internal class UnitCluster:cluster
	{
		public List<CacheUnit> ListUnits { get; protected set; }
		public ClusterInfo Info { get; set; }


		//internal int UnitMobileCounter { get; set; }
		//internal double UnitsMovementRatio
		//{
		//   get
		//   {
		//      return UnitMobileCounter/ListUnits.Count;
		//   }
		//}

		public float NearestMonsterDistance { get; set; }

		public void UpdateUnitPointLists(ClusterConditions CC)
		{
			 if (ListUnits.Count==0) return;

			List<int> RemovalIndexList=new List<int>();
			bool changeOccured=false;
			foreach (var item in ListUnits)
			{
				if (!item.IsStillValid()||(!CC.IgnoreNonTargetable||!item.IsTargetable.Value))
				{
					RemovalIndexList.Add(ListUnits.IndexOf(item));
					RAGUIDS.Remove(item.RAGUID);
					changeOccured=true;
				}
			}


			if (changeOccured)
			{
				RemovalIndexList.Sort();
				RemovalIndexList.Reverse();
				foreach (var item in RemovalIndexList)
				{
					//ListCacheObjects.RemoveAt(item);
					ListUnits.RemoveAt(item);
					ListPoints.RemoveAt(item);
				}

				if (ListUnits.Count>1)
				{
					//Logging.WriteVerbose("Updating Cluster");

					//Reset Vars
					Info=new ClusterInfo();

					NearestMonsterDistance=-1f;

					//Set default using First Unit
					CacheUnit firstUnit=ListUnits[0];
					MidPoint=firstUnit.PointPosition;
					RAGUIDS.Add(firstUnit.RAGUID);
					NearestMonsterDistance=firstUnit.CentreDistance;
					Info.Update(ref firstUnit);


					//Iterate thru the remaining
					for (int i=1; i<ListUnits.Count-1; i++)
					{
						this.UpdateProperties(ListUnits[i]);
					}
				}

			}
		}



		protected UnitCluster():base()
		{
			ListUnits=new List<CacheUnit>();
			//UnitMobileCounter=0;
			NearestMonsterDistance=-1f;
			Info=new ClusterInfo();

		}  // of parameterless constructor

		protected UnitCluster(double p_Dist)
			: base(p_Dist)
		{
			Dist=p_Dist;

		}  // of overloaded constructor

		public UnitCluster(double p_Dist, CacheUnit unit)
			: base(p_Dist, unit)
		{
			ListUnits=new List<CacheUnit>();
			ListUnits.Add(unit);
			NearestMonsterDistance=unit.CentreDistance;
			Info=new ClusterInfo();
			Info.Update(ref unit);

		}  // of overloaded constructor

		private bool ContainsUnit(CacheUnit unit)
		{
			bool u_Exists=false;

			if (base.RAGUIDS.Contains(unit.RAGUID))
				u_Exists=true;

			return u_Exists;
		}

		private void UpdateProperties(CacheUnit unit)
		{
			float distance=unit.CentreDistance;
			if (distance<this.NearestMonsterDistance)
				this.NearestMonsterDistance=distance;

			Info.Update(ref unit, true);
		}

		/// <summary>
		/// Adds point to this cluster only if it is "reachable"
		/// (if point is inside a circle of radius Dist of any cluster's points )
		/// </summary>
		/// <param name="p_Point">The point to be added to this cluster</param>
		/// <returns>false if point can't be added (that is either already in cluster
		/// or it is unreachable from any of the cluster's points)</returns>
		internal override bool  AddObject(CacheObject obj)
		{
			bool l_bSuccess=base.AddObject(obj);

			if (l_bSuccess)//&&Bot.Targeting.Environment.UnitRAGUIDs.Contains(unit.RAGUID))
			{
					 CacheUnit unitobj=(CacheUnit)obj;
					 ListUnits.Add(unitobj);
					 this.UpdateProperties(unitobj);
				}
				else
					l_bSuccess=false;

			return l_bSuccess;

		}  // of AddPoint()


		/// <summary>
		/// Incorporates all points from given cluster to this cluster
		/// </summary>
		/// <param name="p_Cluster"></param>
		/// <returns>true always</returns>
		public override bool AnnexCluster(cluster p_Cluster)
		{
			base.AnnexCluster(p_Cluster);

			//Unit specific
			UnitCluster u_Cluster=(UnitCluster)p_Cluster;
			ListUnits.AddRange(u_Cluster.ListUnits);
			if (this.NearestMonsterDistance>u_Cluster.NearestMonsterDistance)
				this.NearestMonsterDistance=u_Cluster.NearestMonsterDistance;
			Info.Merge(u_Cluster.Info);


			return true;

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

		 public CacheUnit GetClosestUnitToPosition(GridPoint loc)
		 {
			  double minimumDistance=0.0;
			  int nearestPointIndex=-1;
			
			  foreach (GridPoint p in this.ListPoints)
			  {
					double distance=GridPoint.GetDistanceBetweenPoints(p, loc);

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


		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj==null||this.GetType()!=obj.GetType())
			{
				return false;
			}
			else
			{
				UnitCluster p=(UnitCluster)obj;
				return this.Midpoint.Equals(p.Midpoint);
			}
		}
		public override int GetHashCode()
		{
			return this.Midpoint.GetHashCode();
		}

	}
}