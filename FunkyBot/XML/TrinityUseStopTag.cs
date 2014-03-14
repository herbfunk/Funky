using System.Runtime.InteropServices;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using FunkyBot.Game;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityUseStop")]
	public class TrinityUseStopTag : ProfileBehavior
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
				// See if we've EVER hit this ID before
				// If so, set it disabled - if not, add it and prevent it
				if (ProfileCache.hashUseOnceID.Contains(ID))
				{
					 ProfileCache.dictUseOnceID[ID]=-1;
				}
				else
				{
					 ProfileCache.hashUseOnceID.Add(ID);
					 ProfileCache.dictUseOnceID.Add(ID, -1);
				}
				m_IsDone=true;
			});
		}


		[XmlAttribute("id")]
		public int ID { get; set; }

		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}