using System.Collections.Generic;
using fBaseXtensions.Cache.External.Enums;

namespace fBaseXtensions.Settings
{
	public class SettingCluster
	{
		public double ClusterDistance { get; set; }
		public int ClusterMinimumUnitCount { get; set; }
		public bool EnableClusteringTargetLogic { get; set; }
		public double IgnoreClusterLowHPValue { get; set; }
		public float ClusterMaxDistance { get; set; }
		public bool UnitException_RareElites { get; set; }
		public UnitFlags UnitExceptions { get; set; }

		public SettingCluster(bool enabled=true)
		{
			EnableClusteringTargetLogic=enabled;
			ClusterDistance=7d;
			ClusterMinimumUnitCount=3;
			IgnoreClusterLowHPValue=0.55d;
			ClusterMaxDistance = 100;
			UnitException_RareElites = true;
			UnitExceptions = UnitFlags.SucideBomber | UnitFlags.Unique | UnitFlags.Boss | UnitFlags.AdventureModeBoss | UnitFlags.TreasureGoblin | UnitFlags.AvoidanceSummoner;
		}
		public SettingCluster()
		{
			EnableClusteringTargetLogic=true;
			ClusterDistance=10d;
			ClusterMinimumUnitCount=2;
			IgnoreClusterLowHPValue=0.55d;
			ClusterMaxDistance = 100;
			UnitException_RareElites = true;
			UnitExceptions = UnitFlags.SucideBomber | UnitFlags.Unique | UnitFlags.Boss | UnitFlags.AdventureModeBoss | UnitFlags.TreasureGoblin | UnitFlags.AvoidanceSummoner;
		}

		internal List<int> ExceptionSNOs = new List<int>();
 
		public static readonly SettingCluster DisabledClustering = new SettingCluster
		{
			EnableClusteringTargetLogic = false,
			ClusterDistance = 20d,
			ClusterMaxDistance = 100f,
			ClusterMinimumUnitCount = 0
		};

		private static SettingCluster clusterSettingsTag = new SettingCluster();
		internal static SettingCluster ClusterSettingsTag
		{
			get { return clusterSettingsTag; }
			set { clusterSettingsTag = value; }
		}
	}
}