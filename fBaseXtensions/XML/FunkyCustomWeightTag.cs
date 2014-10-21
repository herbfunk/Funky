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
    [XmlElement("FunkyCustomWeight")]
    public class FunkyCustomWeightTag : ProfileBehavior
	{
        [XmlElement("WeightList")]
        public List<Weight> WeightList
		{
			get { return _weightlist; }
			set { _weightlist = value; }
		}
        private List<Weight> _weightlist = new List<Weight>(); 

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		

		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
                foreach (var entry in WeightList)
                {
                    if (FunkyGame.Game.ObjectCustomWeights.ContainsKey(entry.SNO))
                        FunkyGame.Game.ObjectCustomWeights[entry.SNO] = entry.WEIGHT;
                    else
                        FunkyGame.Game.ObjectCustomWeights.Add(entry.SNO, entry.WEIGHT);
                }

                Logger.DBLog.InfoFormat("[Funky] Using Custom Weights for {0} objects", WeightList.Count);
				m_IsDone=true;
			});
		}


		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}

	[XmlElement("Weight")]
	public class Weight
	{
		[XmlAttribute("sno")]
		[XmlAttribute("SNO")]
		[XmlAttribute("Sno")]
		public int SNO { get; set; }

        [XmlAttribute("weight")]
        [XmlAttribute("Weight")]
        public int WEIGHT { get; set; }

		public Weight(int sno, int weight)
		{
			SNO = sno;
		    WEIGHT = weight;
		}

        public Weight()
		{
			SNO = -1;
            WEIGHT = 0;
		}

		public override string ToString()
		{
			return SNO.ToString();
		}
	}
}