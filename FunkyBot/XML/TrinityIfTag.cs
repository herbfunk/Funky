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
		private static Func<ProfileBehavior, bool> funcBehaviorProcess;

		public void CompilePython()
		{

			ScriptManager.GetCondition(Condition);
		}

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
				BotMain.Stop();
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

		[XmlAttribute("condition")]
		public string Condition { get; set; }

		public Func<bool> Conditional { get; set; }

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
					bComplexDoneCheck=GetConditionExec();
				}
				if (bComplexDoneCheck==false)
				{
					return true;
				}
				// Ok we've already checked that and it was false FIRST check, so now go purely on behavior-done flag
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