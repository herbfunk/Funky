using System.Runtime.InteropServices;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyTrinity.XMLTags
{
	[ComVisible(false)]
	[XmlElement("FunkyCastTP")]
	public class FunkyCastTPTag : ProfileBehavior
	{
		private bool m_IsDone=false;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			Composite[] children=new Composite[2];

			ActionDelegate actionDelegateCast=new ActionDelegate(Funky.FunkyTPBehavior);
			Sequence sequenceblank=new Sequence(
				new Zeta.TreeSharp.Action(actionDelegateCast)
				);

			children[0]=new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(Funky.FunkyTPOverlord), sequenceblank);
			children[1]=new Zeta.TreeSharp.Action(new ActionSucceedDelegate(FlagTagAsCompleted));

			return new PrioritySelector(children);
		}

		private void FlagTagAsCompleted(object object_0)
		{
			m_IsDone=true;
		}

		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}

	}
}