using System;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta;
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
		private static Func<ProfileBehavior, bool> funcBehaviorProcess;
		private string sType;

		protected override Composite CreateBehavior()
		{
			var decorated=new PrioritySelector(new Composite[0]);
			foreach (var behavior in base.GetNodes())
			{
				decorated.AddChild(behavior.Behavior);
			}
			return new Decorator(CheckNotAlreadyDone, decorated);
		}

		public bool GetConditionExec()
		{
			bool flag;
			var vMyLocation=ZetaDia.Me.Position;
			if (sType!=null&&sType=="reverse")
				flag=ZetaDia.Actors.GetActorsOfType<DiaObject>(true).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID&&a.Position.Distance(vMyLocation)<=Range)==null;
			else
				flag=(ZetaDia.Actors.GetActorsOfType<DiaObject>(true).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID&&a.Position.Distance(vMyLocation)<=Range)!=null);
			return flag;
		}

		private bool CheckNotAlreadyDone(object object_0)
		{
			return !IsDone;
		}

		public override void ResetCachedDone()
		{
			foreach (var behavior in Body)
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
		public int SNOID { get; set; }

		[XmlAttribute("range")]
		public float Range { get; set; }

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

		public Func<bool> Conditional { get; set; }

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
					bComplexDoneCheck=GetConditionExec();
				}
				if (bComplexDoneCheck==false)
				{
					return true;
				}
				if (funcBehaviorProcess==null)
				{
					funcBehaviorProcess=CheckBehaviorIsDone;
				}
				var bAllChildrenDone=Body.All(funcBehaviorProcess);
				if (bAllChildrenDone)
				{
					bAlreadyCompleted=true;
				}
				return bAllChildrenDone;
			}
		}
	}
}