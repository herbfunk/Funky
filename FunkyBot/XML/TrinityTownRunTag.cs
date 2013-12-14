using System.Runtime.InteropServices;
using Zeta.CommonBot.Logic;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityTownRun")]
	public class TrinityTownRunTag : ProfileBehavior
	{
		 [XmlAttribute("waitTime")]
		 [XmlAttribute("wait")]
		 public static int WaitTime { get; set; }
		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
				BrainBehavior.ForceTownrun("Town-run request received, will town-run at next possible moment.");
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