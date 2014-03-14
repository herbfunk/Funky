using System.Globalization;
using System.Runtime.InteropServices;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityMaxDeaths")]
	public class TrinityMaxDeathsTag : ProfileBehavior
	{
        public static int MaxDeathsAllowed = 0;
		private bool m_IsDone;

		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
                if (MaxDeaths != MaxDeathsAllowed)
					Logger.DBLog.InfoFormat("[Funky] Max deaths set by profile. Trinity now handling deaths, and will restart the game after "+MaxDeaths.ToString(CultureInfo.InvariantCulture));

                MaxDeathsAllowed = MaxDeaths;
                //if (Reset!=null&&Reset.ToLower()=="true")
                //    Bot.Stats.iDeathsThisRun=0;
				m_IsDone=true;
			});
		}


		[XmlAttribute("reset")]
		public string Reset { get; set; }

		[XmlAttribute("max")]
		public int MaxDeaths { get; set; }

		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}