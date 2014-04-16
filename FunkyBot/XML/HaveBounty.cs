using System.Linq;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[XmlElement("HaveBounty")]
	public class HaveBounty : BaseComplexNodeTag
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
			if (Bot.Game.Bounty.CurrentBounties.ContainsKey(QuestId)) 
				return Bot.Game.Bounty.CurrentBounties[QuestId].State != QuestState.Completed;

			return ZetaDia.ActInfo.Bounties.Where(bounty => bounty.Info.QuestSNO == QuestId && bounty.Info.State != QuestState.Completed).FirstOrDefault() != null;
		}

		private bool CheckNotAlreadyDone(object obj)
		{
			return !IsDone;
		}
	}
}
