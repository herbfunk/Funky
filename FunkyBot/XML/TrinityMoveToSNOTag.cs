using System;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta;
using Zeta.CommonBot.Profile;
using Zeta.Internals.Actors;
using Zeta.Navigation;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityMoveToSNO")]
	public class TrinityMoveToSNOTag : ProfileBehavior
	{
		private bool m_IsDone;

		protected override Composite CreateBehavior()
		{
			var children=new Composite[2];
			var compositeArray=new Composite[2];
			compositeArray[0]=new Action(new ActionSucceedDelegate(FlagTagAsCompleted));
			children[0]=new Decorator(CheckDistanceWithinPathPrecision, new Sequence(compositeArray));
			ActionDelegate actionDelegateMove=GilesMoveToLocation;
			var sequenceblank=new Sequence(
				new Action(actionDelegateMove)
				);
			children[1]=sequenceblank;
			return new PrioritySelector(children);
		}

		private RunStatus GilesMoveToLocation(object ret)
		{
			var tempObject=ZetaDia.Actors.GetActorsOfType<DiaObject>(true).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID);
			if (tempObject!=null)
			{
				Navigator.PlayerMover.MoveTowards(tempObject.Position);
				return RunStatus.Success;
			}
			return RunStatus.Success;
		}

		private bool CheckDistanceWithinPathPrecision(object object_0)
		{
			var tempObject=ZetaDia.Actors.GetActorsOfType<DiaObject>(true).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID);
			if (tempObject!=null)
			{
				return (ZetaDia.Me.Position.Distance(tempObject.Position)<=Math.Max(PathPrecision, Navigator.PathPrecision));
			}
			return false;
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


		public override bool IsDone
		{
			get
			{
				if (IsActiveQuestStep)
				{
					return m_IsDone;
				}
				return true;
			}
		}

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("pathPrecision")]
		public float PathPrecision { get; set; }

		[XmlAttribute("snoid")]
		public int SNOID { get; set; }
	}
}