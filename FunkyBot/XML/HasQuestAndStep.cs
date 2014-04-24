using System.Linq;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[XmlElement("HasQuestAndStep")]
	public class HasQuestAndStep : BaseComplexNodeTag
	{
		protected override Composite CreateBehavior()
		{
			return
			new Decorator(ret => !IsDone,
				new PrioritySelector(
					base.GetNodes().Select(b => b.Behavior).ToArray()
				)
			);
		}

		public override bool GetConditionExec()
		{
			return ZetaDia.ActInfo.AllQuests
				.Where(quest => quest.QuestSNO == QuestId && quest.State != QuestState.Completed && quest.QuestStep == StepId).FirstOrDefault() != null;
		}

		private bool CheckNotAlreadyDone(object obj)
		{
			return !IsDone;
		}
	}
}
