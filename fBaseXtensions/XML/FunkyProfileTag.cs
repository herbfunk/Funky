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
    [XmlElement("FunkyProfile")]
    public class FunkyProfileTag : ProfileBehavior
	{
        [XmlElement("Cluster")]
        public ClusterSettings Cluster { get; set; }

        [XmlElement("ObjectWeighting")]
        public List<ObjectWeight> ObjectWeightList { get; set; } 


		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
			    if (Cluster != null)
			    {
                    SettingCluster.ClusterSettingsTag = new SettingCluster
                    {
                        EnableClusteringTargetLogic = Cluster.Enabled,
                        ClusterDistance = Cluster.Radius,
                        ClusterMaxDistance = Cluster.Range,
                        ClusterMinimumUnitCount = Cluster.Units,
                        ExceptionSNOs = Cluster.SNOs.Select(s => s.SNO).ToList(),
                    };

                    FunkyGame.Targeting.Cache.Clusters = new Clustering();

                    Logger.DBLog.Info("[Funky] Using Custom Cluster Settings!");
			    }


			    if (ObjectWeightList != null)
			    {
                    foreach (var entry in ObjectWeightList)
                    {
                        if (FunkyGame.Game.ObjectCustomWeights.ContainsKey(entry.Sno))
                            FunkyGame.Game.ObjectCustomWeights[entry.Sno] = entry.Weight;
                        else
                            FunkyGame.Game.ObjectCustomWeights.Add(entry.Sno, entry.Weight);
                    }

                    Logger.DBLog.InfoFormat("[Funky] Using Custom Weights for {0} objects", ObjectWeightList.Count);
			    }


				m_IsDone=true;
			});
		}





		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

	    public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}



        [XmlElement("ObjectWeight")]
        public class ObjectWeight
        {
            [XmlAttribute("sno")]
            [XmlAttribute("SNO")]
            [XmlAttribute("Sno")]
            public int Sno { get; set; }

            [XmlAttribute("weight")]
            [XmlAttribute("Weight")]
            public int Weight { get; set; }

            public ObjectWeight(int sno, int weight)
            {
                Sno = sno;
                Weight = weight;
            }

            public ObjectWeight()
            {
                Sno = -1;
                Weight = 0;
            }

            public override string ToString()
            {
                return Sno.ToString();
            }
        }

        [XmlElement("Clustering")]
        public class ClusterSettings
        {
            [XmlElement("ExceptionList")]
            public List<ClusterException> SNOs
            {
                get { return _snos; }
                set { _snos = value; }
            }
            private List<ClusterException> _snos = new List<ClusterException>();

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

            public ClusterSettings()
            {
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


   
}