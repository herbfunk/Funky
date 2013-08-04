using System.Runtime.InteropServices;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyTrinity.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityTownRun")]
	public class TrinityTownRunTag : ProfileBehavior
	{
		private bool m_IsDone=false;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Zeta.TreeSharp.Action(ret =>
			{
				Zeta.CommonBot.Logic.BrainBehavior.ForceTownrun("Town-run request received, will town-run at next possible moment.");
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