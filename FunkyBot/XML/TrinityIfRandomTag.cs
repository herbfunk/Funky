using System;
using System.Linq;
using System.Runtime.InteropServices;
using FunkyBot.Cache;
using Zeta.CommonBot.Profile;
using Zeta.CommonBot.Profile.Composites;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityIfRandom")]
	public class TrinityIfRandomTag : ComplexNodeTag
	{
		private bool? bComplexDoneCheck;
		private bool? bAlreadyCompleted;
		private Func<bool> funcConditionalProcess;
		private static Func<ProfileBehavior, bool> funcBehaviorProcess;
		private int iID;
		private int iResult;

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
			int iOldValue;
			// If the dictionary value doesn't even exist, FAIL!
			if (!ProfileCache.dictRandomID.TryGetValue(ID, out iOldValue)&&Result!=-1)
				return false;
			// Ok, do the results match up what we want? then SUCCESS!
			if (iOldValue==Result||Result==-1)
				return true;
			// No? Fail!
			return false;
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

		[XmlAttribute("id")]
		public int ID
		{
			get
			{
				return iID;
			}
			set
			{
				iID=value;
			}
		}

		[XmlAttribute("result")]
		public int Result
		{
			get
			{
				return iResult;
			}
			set
			{
				iResult=value;
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