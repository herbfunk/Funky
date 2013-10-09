using System;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Profile;
using Zeta.CommonBot.Profile.Composites;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityIf")]
	public class TrinityIfTag : ComplexNodeTag, IPythonExecutable
	{
		private bool? bComplexDoneCheck;
		private bool? bAlreadyCompleted;
		private Func<bool> funcConditionalProcess;
		private static Func<ProfileBehavior, bool> funcBehaviorProcess;
		private string sConditionString;

		public void CompilePython()
		{

			ScriptManager.GetCondition(Condition);
		}

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
			try
			{
				if (Conditional==null)
				{
					Conditional=ScriptManager.GetCondition(Condition);
				}
				flag=Conditional();
			} catch (Exception exception)
			{
				Logging.WriteDiagnostic(ScriptManager.FormatSyntaxErrorException(exception));
				BotMain.Stop(false, "");
				throw;
			}
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

		[XmlAttribute("condition", true)]
		public string Condition
		{
			get
			{
				return sConditionString;
			}
			set
			{
				sConditionString=value;
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
				// Make sure we've not already completed this if
				if (bAlreadyCompleted.HasValue&&bAlreadyCompleted==true)
				{
					return true;
				}
				// First check the actual "if" conditions if we haven't already
				if (!bComplexDoneCheck.HasValue)
				{
					bComplexDoneCheck=new bool?(GetConditionExec());
				}
				if (bComplexDoneCheck==false)
				{
					return true;
				}
				// Ok we've already checked that and it was false FIRST check, so now go purely on behavior-done flag
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