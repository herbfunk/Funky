using System;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot.Profile;
using Zeta.CommonBot.Profile.Composites;
using Zeta.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityIfSNOInRange")]
	public class IfSNOInRangeTag : ComplexNodeTag
	{
		private bool? bComplexDoneCheck;
		private bool? bAlreadyCompleted;
		private Func<bool> funcConditionalProcess;
		private static Func<ProfileBehavior, bool> funcBehaviorProcess;
		private int iSNOID;
		private float fRadius;
		private string sType;

		protected override Composite CreateBehavior()
		{
			PrioritySelector decorated=new PrioritySelector(new Composite[0]);
			foreach (ProfileBehavior behavior in base.GetNodes())
			{
				decorated.AddChild(behavior.Behavior);
			}
			return new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(CheckNotAlreadyDone), decorated);
		}

		public bool GetConditionExec()
		{
			bool flag;
			Vector3 vMyLocation=ZetaDia.Me.Position;
			if (sType!=null&&sType=="reverse")
				flag=ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID&&a.Position.Distance(vMyLocation)<=Range)==null;
			else
				flag=(ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID&&a.Position.Distance(vMyLocation)<=Range)!=null);
			return flag;
		}

		private bool CheckNotAlreadyDone(object object_0)
		{
			return !IsDone;
		}

		public override void ResetCachedDone()
		{
			foreach (ProfileBehavior behavior in Body)
			{
				behavior.ResetCachedDone();
			}
			bComplexDoneCheck=null;
		}

		private static bool CheckBehaviorIsDone(ProfileBehavior profileBehavior)
		{
			return profileBehavior.IsDone;
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

		[XmlAttribute("range")]
		public float Range
		{
			get
			{
				return fRadius;
			}
			set
			{
				fRadius=value;
			}
		}

		[XmlAttribute("type")]
		public string Type
		{
			get
			{
				return sType;
			}
			set
			{
				sType=value;
			}
		}

		public Func<bool> Conditional
		{
			get
			{
				return funcConditionalProcess;
			}
			set
			{
				funcConditionalProcess=value;
			}
		}

		public override bool IsDone
		{
			get
			{
				// Make sure we've not already completed this tag
				if (bAlreadyCompleted.HasValue&&bAlreadyCompleted==true)
				{
					return true;
				}
				if (!bComplexDoneCheck.HasValue)
				{
					bComplexDoneCheck=new bool?(GetConditionExec());
				}
				if (bComplexDoneCheck==false)
				{
					return true;
				}
				if (funcBehaviorProcess==null)
				{
					funcBehaviorProcess=new Func<ProfileBehavior, bool>(CheckBehaviorIsDone);
				}
				bool bAllChildrenDone=Body.All<ProfileBehavior>(funcBehaviorProcess);
				if (bAllChildrenDone)
				{
					bAlreadyCompleted=true;
				}
				return bAllChildrenDone;
			}
		}
	}
}