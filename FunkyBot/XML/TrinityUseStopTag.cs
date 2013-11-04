using System.Runtime.InteropServices;
using FunkyBot.Cache;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using FunkyBot.Game;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityUseStop")]
	public class TrinityUseStopTag : ProfileBehavior
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