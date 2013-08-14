using System;
using System.Linq;
using Zeta;
using System.Collections.Generic;
using FunkyTrinity.ability;
using FunkyTrinity.Cache;

namespace FunkyTrinity.Movement
{
	 internal class ClusterCollection
	 {
		  public ClusterConditions clusterConditions { get; set; }
		  public List<Cluster> CurrentClusters { get; set; }
		  private DateTime lastClusterComputed { get; set; }
		  private DateTime lastClusterRefresh { get; set; }

		  public bool ShouldRefreshClusters
		  {
				get
				{
					 return DateTime.Now.Subtract(lastClusterComputed).TotalMilliseconds>200;
				}
		  }

		  ///<summary>
		  ///Iterates thru the cluster list and calls the update method on each.
		  ///</summary>
		  public void RefreshClusters()
		  {
				if (DateTime.Now.Subtract(lastClusterRefresh).TotalMilliseconds<100)
					 return;

				
				if (CurrentClusters.Count>0)
				{
					 foreach (var item in CurrentClusters)
					 {
						  item.UpdateUnitPointLists(clusterConditions);
					 }

					CurrentClusters=CurrentClusters.Where(c => c.ListUnits.Count>=clusterConditions.MinimumUnits&&(clusterConditions.DOTDPSRatio==0.00d||c.DotDPSRatio<=clusterConditions.DOTDPSRatio)).ToList();
				}

				lastClusterRefresh=DateTime.Now;
		  }

		  ///<summary>
		  ///Updates the Cluster List!
		  ///</summary>
		  public void UpdateClusters()
		  {
				CurrentClusters.Clear();

				//Get unit objects only!
				List<CacheUnit> listObjectUnits=Bot.ValidObjects.OfType<CacheUnit>().Where(u =>
					 Bot.Combat.UnitRAGUIDs.Contains(u.RAGUID)
					 &&u.CentreDistance<=clusterConditions.MaximumDistance
					 &&(!clusterConditions.IgnoreNonTargetable||u.IsTargetable.HasValue&&u.IsTargetable.Value)).ToList();


				//Logging.WriteVerbose("Total Units {0}", listObjectUnits.Count.ToString());
				if (listObjectUnits.Count>0)
				{
					 CurrentClusters=Cluster.RunKmeans(listObjectUnits, clusterConditions.ClusterDistance).Where(c => c.ListUnits.Count>=clusterConditions.MinimumUnits&&(clusterConditions.DOTDPSRatio==0.00d||c.DotDPSRatio<=clusterConditions.DOTDPSRatio)).ToList();

					 //Sort by distance -- reverse to get nearest unit First
					 if (CurrentClusters.Count>0)
					 {
						  CurrentClusters=CurrentClusters.OrderBy(o => o.NearestMonsterDistance).ToList();
						  CurrentClusters.First().ListUnits.Sort();
						  CurrentClusters.First().ListUnits.Reverse();
					 }
				}

				lastClusterComputed=DateTime.Now;
		  }

		  public ClusterCollection(ClusterConditions CC)
		  {
				CurrentClusters=new List<Cluster>();
				lastClusterComputed=DateTime.Today;
				lastClusterRefresh=DateTime.Today;
				clusterConditions=CC;
		  }
    }
}