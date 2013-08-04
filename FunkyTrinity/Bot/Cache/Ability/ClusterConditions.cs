namespace FunkyTrinity.ability
{
	 public class ClusterConditions
	 {
			public readonly double ClusterDistance;
			public readonly float MaximumDistance;
			public readonly int MinimumUnits;
			public readonly bool IgnoreNonTargetable;

			public ClusterConditions(double clusterRadius, float MaxDistanceFromBot, int MinimumUnitCount, bool IgnoreNonTargetableUnits)
			{
				 ClusterDistance=clusterRadius;
				 MaximumDistance=MaxDistanceFromBot;
				 MinimumUnits=MinimumUnitCount;
				 IgnoreNonTargetable=IgnoreNonTargetableUnits;
			}
	 }
}