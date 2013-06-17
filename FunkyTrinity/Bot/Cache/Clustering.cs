using System;
using System.Linq;
using Zeta;
using System.Collections.Generic;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  private static DateTime lastClusterComputed=DateTime.Today;
		  private static List<Cluster> LC_=new List<Cluster>();

		  private static List<Cluster> RunKmeans<T>(List<T> unitList, double distance) where T : CacheObject
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
		 


		  //Cache last filtered list generated
		  internal static List<Cluster> LastClusterList=new List<Cluster>();
		  internal static List<Cluster> Clusters(double Distance=12d, float MaxDistanceFromBot=50f, int MinUnitCount=1, bool ignoreFullyNonAttackable=true)
		  {
				//only run Kmeans every 200ms or when cache is empty!
				if (DateTime.Now.Subtract(lastClusterComputed).TotalMilliseconds>200)
				{
					 lastClusterComputed=DateTime.Now;
					 LastClusterList=Bot.Combat.RunKMeans(MinUnitCount, Distance, MaxDistanceFromBot);
					 //Sort by distance -- reverse to get nearest unit First
					 if (LastClusterList.Count>0)
					 {
						  LastClusterList=LastClusterList.OrderBy(o => o.NearestMonsterDistance).ToList();
						  LastClusterList.First().ListUnits.Sort();
						  LastClusterList.First().ListUnits.Reverse();
					 }
				}
				return LastClusterList.Where(c => c.ListUnits.Count>=MinUnitCount).ToList();
		  }

    }
}