using System;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta.CommonBot.Profile;
using Zeta.CommonBot.Profile.Composites;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyTrinity.XMLTags
{
	///<summary>
	///Is set to an ID when using CheckScences and a valid scence was found.
	///</summary>
	[ComVisible(false)]
	[XmlElement("Scene")]
	public class SceneTag : ComplexNodeTag
	{
		private bool? bComplexDoneCheck;
		private bool? bAlreadyCompleted;
		private Func<bool> funcConditionalProcess;
		private static Func<ProfileBehavior, bool> funcBehaviorProcess;
		private int iUniqueID;

		protected override Composite CreateBehavior()
		{
			PrioritySelector decorated=new PrioritySelector(new Composite[0]);

			foreach (ProfileBehavior behavior in base.GetNodes())
			{
				decorated.AddChild(behavior.Behavior);
			}
			// Logging.Write("Count: ");

			return new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(CheckNotAlreadyDone), decorated);
		}

		public bool GetConditionExec()
		{
			if (FunkyTrinity.Funky.ScenceCheck==iUniqueID)
			{
				return true;
			}
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
				return iUniqueID;
			}
			set
			{
				iUniqueID=value;
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