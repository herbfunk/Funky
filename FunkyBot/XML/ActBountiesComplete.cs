using System;
using System.Linq;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[XmlElement("ActBountiesComplete")]
	public class ActBountiesComplete : BaseComplexNodeTag
	{

		protected override Composite CreateBehavior()
		{
			PrioritySelector decorated = new PrioritySelector(new Composite[0]);
			foreach (ProfileBehavior behavior in base.GetNodes())
			{
				decorated.AddChild(behavior.Behavior);
			}
			return new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(CheckNotAlreadyDone), decorated);
		}

		public override bool GetConditionExec()
		{
			Logger.DBLog.Debug("Checking Act " + _Act.ToString());

			var a = Bot.Game.Bounty.CurrentBounties.Values.Count(bounty => bounty.Act == _Act && bounty.State == QuestState.Completed);
			Logger.DBLog.DebugFormat("Count for Act {0} == {1}", _Act.ToString(), a);

			return a==5;
		}

		private bool CheckNotAlreadyDone(object obj)
		{
			return !IsDone;
		}

		private Act _Act=Zeta.Game.Act.Invalid;

		private string _act;
		[XmlAttribute("act", true)]
		public string Act
		{
			get { return _act; }
			set 
			{ 
				_act = value;
				_Act = (Act)Enum.Parse(typeof(Act), value);
			}
		}
	}
}
