using System.Runtime.InteropServices;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using FunkyBot.Game;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityUseReset")]
	public class TrinityUseResetTag : ProfileBehavior
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
				if (All == true)
				{
					ProfileCache.hashUseOnceID.Clear();
					ProfileCache.dictUseOnceID.Clear();
				}
				else
				{
					// See if we've EVER hit this ID before
					// If so, delete it, if not, do nothing
					if (ProfileCache.hashUseOnceID.Contains(ID))
					{
						ProfileCache.hashUseOnceID.Remove(ID);
						ProfileCache.dictUseOnceID.Remove(ID);
					}
				}
				m_IsDone=true;
			});
		}

		[XmlAttribute("All")]
		[XmlAttribute("all")]
		public bool All { get; set; }

		[XmlAttribute("ID")]
		[XmlAttribute("id")]
		public int ID { get; set; }

		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}