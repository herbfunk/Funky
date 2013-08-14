namespace FunkyTrinity.ability
{
	 public class ClusterConditions
	 {
			public readonly double ClusterDistance;
			public readonly float MaximumDistance;
			public readonly int MinimumUnits;
			public readonly bool IgnoreNonTargetable;
			public readonly double DOTDPSRatio;
			private readonly System.Guid GUID;
			public ClusterConditions(double clusterRadius, float MaxDistanceFromBot, int MinimumUnitCount, bool IgnoreNonTargetableUnits, double DotDPSRatio=0.00)
			{
				 ClusterDistance=clusterRadius;
				 MaximumDistance=MaxDistanceFromBot;
				 MinimumUnits=MinimumUnitCount;
				 IgnoreNonTargetable=IgnoreNonTargetableUnits;
				 DOTDPSRatio=DotDPSRatio;
				 GUID=System.Guid.NewGuid();
			}

			public override int GetHashCode()
			{
				 return this.GUID.GetHashCode();
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
					  ClusterConditions p=(ClusterConditions)obj;
					  return ClusterDistance==p.ClusterDistance
								&&MaximumDistance==p.MaximumDistance
								&&MinimumUnits==p.MinimumUnits
								&&IgnoreNonTargetable==p.IgnoreNonTargetable
								&&DOTDPSRatio==p.DOTDPSRatio;
				 }
			}

			public string ToString()
			{
				 return string.Format("ClusterDistance {0} -- MaximumDistance {1} -- MinimumUnits {2} -- IgnoreNonTargetable {3} -- DotDPSRatio {4}",
												ClusterDistance.ToString(), MaximumDistance.ToString(), MinimumUnits.ToString(), IgnoreNonTargetable.ToString(), DOTDPSRatio.ToString());
			}
	 }
}