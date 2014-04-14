using FunkyBot.Game;
using FunkyBot.Settings;
using Zeta.Bot.Profile;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace FunkyBot.XMLTags
{
	[XmlElement("FunkyBountyRefresh")]
	class FunkyBountyRefresh : ProfileBehavior
	{

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
				Bot.Game.UpdateBountyInfo();
				m_IsDone = true;
			});
		}


		public override void ResetCachedDone()
		{
			m_IsDone = false;
			base.ResetCachedDone();
		}
	}
}
