using System.Runtime.InteropServices;
using fBaseXtensions.Helpers;
using fBaseXtensions.Settings;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace fBaseXtensions.XML
{
	[ComVisible(false)]
	[XmlElement("FunkyClustering")]
	public class FunkyClusteringTag : ProfileBehavior
	{
		[XmlAttribute("enabled")]
		public bool Enabled { get; set; }
		[XmlAttribute("radius")]
		public double Radius { get; set; }
		[XmlAttribute("units")]
		public int Units { get; set; }
		[XmlAttribute("range")]
		public float Range { get; set; }

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
				SettingCluster.ClusterSettingsTag = new SettingCluster 
				{
					EnableClusteringTargetLogic=Enabled,
					ClusterDistance = Radius,
					ClusterMaxDistance = Range,
					ClusterMinimumUnitCount = Units
				};
				Logger.DBLog.Info("[Funky] Using Custom Cluster Settings!");
				m_IsDone=true;
			});
		}


		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}