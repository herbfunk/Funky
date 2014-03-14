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
	[XmlElement("TrinityUseOnce")]
	public class TrinityUseOnceTag : ComplexNodeTag
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
			// See if we've EVER hit this ID before
			 if (ProfileCache.hashUseOnceID.Contains(ID))
			{
				// See if we've hit it more than or equal to the max times before
				 if (ProfileCache.dictUseOnceID[ID]>=Max||ProfileCache.dictUseOnceID[ID]<0)
					return false;
				// Add 1 to our hit count, and let it run this time
				 ProfileCache.dictUseOnceID[ID]++;
				return true;
			}
			// Never hit this before, so create the entry and let it run
			// First see if we should disable all other ID's currently hit to prevent them ever being run again this run
			if (DisablePrevious!=null&&DisablePrevious.ToLower()=="true")
			{
				 foreach (var thisid in ProfileCache.hashUseOnceID)
				{
					if (thisid!=ID)
					{
						 ProfileCache.dictUseOnceID[thisid]=-1;
					}
				}
			}
			// Now store the fact we have hit this ID and set up the dictionary entry for it
			ProfileCache.hashUseOnceID.Add(ID);
			ProfileCache.dictUseOnceID.Add(ID, 1);
			return true;
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

		[XmlAttribute("disableprevious")]
		public string DisablePrevious { get; set; }

		[XmlAttribute("max")]
		public int Max { get; set; }

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