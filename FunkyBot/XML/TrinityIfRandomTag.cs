using System;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Composites;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using FunkyBot.Game;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityIfRandom")]
	public class TrinityIfRandomTag : ComplexNodeTag
	{
		private bool? bComplexDoneCheck;
		private bool? bAlreadyCompleted;
		private static Func<ProfileBehavior, bool> funcBehaviorProcess;

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

		[XmlAttribute("id")]
		public int ID { get; set; }

		[XmlAttribute("result")]
		public int Result { get; set; }

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