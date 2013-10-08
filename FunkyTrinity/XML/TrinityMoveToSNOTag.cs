using System;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta;
using Zeta.CommonBot.Profile;
using Zeta.Internals.Actors;
using Zeta.Navigation;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityMoveToSNO")]
	public class TrinityMoveToSNOTag : ProfileBehavior
	{
		private bool m_IsDone;
		private float fPathPrecision;
		private int iSNOID;
		private string sDestinationName;

		protected override Composite CreateBehavior()
		{
			Composite[] children=new Composite[2];
			Composite[] compositeArray=new Composite[2];
			compositeArray[0]=new Zeta.TreeSharp.Action(new ActionSucceedDelegate(FlagTagAsCompleted));
			children[0]=new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(CheckDistanceWithinPathPrecision), new Sequence(compositeArray));
			ActionDelegate actionDelegateMove=new ActionDelegate(GilesMoveToLocation);
			Sequence sequenceblank=new Sequence(
				new Zeta.TreeSharp.Action(actionDelegateMove)
				);
			children[1]=sequenceblank;
			return new PrioritySelector(children);
		}

		private RunStatus GilesMoveToLocation(object ret)
		{
			DiaObject tempObject=ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID);
			if (tempObject!=null)
			{
				Navigator.PlayerMover.MoveTowards(tempObject.Position);
				return RunStatus.Success;
			}
			return RunStatus.Success;
		}

		private bool CheckDistanceWithinPathPrecision(object object_0)
		{
			DiaObject tempObject=ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID);
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
		public string Name
		{
			get
			{
				return sDestinationName;
			}
			set
			{
				sDestinationName=value;
			}
		}

		[XmlAttribute("pathPrecision")]
		public float PathPrecision
		{
			get
			{
				return fPathPrecision;
			}
			set
			{
				fPathPrecision=value;
			}
		}

		[XmlAttribute("snoid")]
		public int SNOID
		{
			get
			{
				return iSNOID;
			}
			set
			{
				iSNOID=value;
			}
		}
	}
}