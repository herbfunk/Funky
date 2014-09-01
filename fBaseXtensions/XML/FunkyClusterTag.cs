using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using fBaseXtensions.Settings;
using fBaseXtensions.Targeting;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace fBaseXtensions.XML
{
	[ComVisible(false)]
	[XmlElement("FunkyClustering")]
	public class FunkyClusteringTag : ProfileBehavior
	{
		[XmlElement("ExceptionList")]
		public List<ClusterException> SNOs
		{
			get { return _snos; }
			set { _snos = value; }
		}
		private List<ClusterException> _snos = new List<ClusterException>(); 

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		[XmlAttribute("enabled")]
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}
		private bool _enabled = FunkyBaseExtension.Settings.Cluster.EnableClusteringTargetLogic;
		[XmlAttribute("radius")]
		public double Radius
		{
			get { return _radius; }
			set { _radius = value; }
		}
		private double _radius = FunkyBaseExtension.Settings.Cluster.ClusterDistance;
		[XmlAttribute("range")]
		public float Range
		{
			get { return _range; }
			set { _range = value; }
		}
		private float _range = FunkyBaseExtension.Settings.Cluster.ClusterMaxDistance;
		[XmlAttribute("units")]
		public int Units
		{
			get { return _units; }
			set { _units = value; }
		}
		private int _units = FunkyBaseExtension.Settings.Cluster.ClusterMinimumUnitCount;

		

		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
				SettingCluster.ClusterSettingsTag = new SettingCluster 
				{
					EnableClusteringTargetLogic=_enabled,
					ClusterDistance = _radius,
					ClusterMaxDistance = _range,
					ClusterMinimumUnitCount = _units,
					ExceptionSNOs = SNOs.Select(s => s.SNO).ToList()
				};
				FunkyGame.Targeting.Cache.Clusters = new Clustering();
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

	[XmlElement("ClusterException")]
	public class ClusterException
	{
		[XmlAttribute("sno")]
		[XmlAttribute("SNO")]
		[XmlAttribute("Sno")]
		public int SNO { get; set; }

		public ClusterException(int sno)
		{
			SNO = sno;
		}

		public ClusterException()
		{
			SNO = -1;
		}

		public override string ToString()
		{
			return SNO.ToString();
		}
	}
}