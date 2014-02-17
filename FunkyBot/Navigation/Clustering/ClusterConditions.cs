namespace FunkyBot.Movement.Clustering
{
	public class ClusterConditions
	{
		public readonly double ClusterDistance;
		public readonly float MaximumDistance;
		public readonly float MinimumDistance;
		public readonly int MinimumUnits;
		public readonly bool IgnoreNonTargetable;
		public readonly double DOTDPSRatio;
		public readonly ClusterProperties ClusterFlags;
		public readonly bool UseRadiusDistance;
		private readonly System.Guid GUID;

		public ClusterConditions(double clusterRadius, float MaxDistanceFromBot, int MinimumUnitCount, bool IgnoreNonTargetableUnits, double DotDPSRatio = 0.00, ClusterProperties clusterflags = ClusterProperties.None, float minDistance = 0f, bool useRadiusDistance = false)
		{
			ClusterDistance = clusterRadius;
			MaximumDistance = MaxDistanceFromBot;
			MinimumUnits = MinimumUnitCount;
			IgnoreNonTargetable = IgnoreNonTargetableUnits;
			DOTDPSRatio = DotDPSRatio;
			ClusterFlags = clusterflags;
			MinimumDistance = minDistance;
			UseRadiusDistance = useRadiusDistance;
			GUID = System.Guid.NewGuid();
		}

		public override int GetHashCode()
		{
			return GUID.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			ClusterConditions p = (ClusterConditions)obj;
			return ClusterDistance == p.ClusterDistance
				   && MaximumDistance == p.MaximumDistance
				   && MinimumUnits == p.MinimumUnits
				   && IgnoreNonTargetable == p.IgnoreNonTargetable
				   && DOTDPSRatio == p.DOTDPSRatio;
		}

		public string ToString()
		{
			return string.Format("ClusterDistance {0} -- MaximumDistance {1} -- MinimumUnits {2} -- IgnoreNonTargetable {3} -- DotDPSRatio {4}",
										   ClusterDistance.ToString(), MaximumDistance.ToString(), MinimumUnits.ToString(), IgnoreNonTargetable.ToString(), DOTDPSRatio.ToString());
		}
	}
}