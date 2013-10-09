using System.Runtime.InteropServices;
using Zeta.Common;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityMaxDeaths")]
	public class TrinityMaxDeathsTag : ProfileBehavior
	{
		private bool m_IsDone=false;
		private int iMaxDeaths;
		private string sReset;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Zeta.TreeSharp.Action(ret =>
			{
				if (MaxDeaths!=Bot.Stats.iMaxDeathsAllowed)
					Logging.Write("[Funky] Max deaths set by profile. Trinity now handling deaths, and will restart the game after "+MaxDeaths.ToString());

				Bot.Stats.iMaxDeathsAllowed=MaxDeaths;
				if (Reset!=null&&Reset.ToLower()=="true")
					Bot.Stats.iDeathsThisRun=0;
				m_IsDone=true;
			});
		}


		[XmlAttribute("reset")]
		public string Reset
		{
			get
			{
				return sReset;
			}
			set
			{
				sReset=value;
			}
		}

		[XmlAttribute("max")]
		public int MaxDeaths
		{
			get
			{
				return iMaxDeaths;
			}
			set
			{
				iMaxDeaths=value;
			}
		}

		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}