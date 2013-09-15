using System.Diagnostics;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action=Zeta.TreeSharp.Action;

namespace FunkyTrinity.XMLTags 
{
	 // TrinityTownRun forces a town-run request
	 [XmlElement("TrinityTownPortal")]
	 public class TrinityTownPortal : ProfileBehavior
	 {
		  [XmlAttribute("waitTime")]
		  [XmlAttribute("wait")]
		  public static int WaitTime { get; set; }
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