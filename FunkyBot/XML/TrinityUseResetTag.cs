using System.Runtime.InteropServices;
using FunkyBot.Cache;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityUseReset")]
	public class TrinityUseResetTag : ProfileBehavior
	{
		private bool m_IsDone=false;
		private int iID;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Zeta.TreeSharp.Action(ret =>
			{
				// See if we've EVER hit this ID before
				// If so, delete it, if not, do nothing
				 if (ProfileCache.hashUseOnceID.Contains(ID))
				{
					 ProfileCache.hashUseOnceID.Remove(ID);
					 ProfileCache.dictUseOnceID.Remove(ID);
				}
				m_IsDone=true;
			});
		}


		[XmlAttribute("id")]
		public int ID
		{
			get
			{
				return iID;
			}
			set
			{
				iID=value;
			}
		}

		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}