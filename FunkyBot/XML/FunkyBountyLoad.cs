using System.IO;
using System.Web.Profile;
using FunkyBot.Game;
using Zeta.Bot.Profile;
using Zeta.Bot.Settings;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;
using ProfileManager = Zeta.Bot.ProfileManager;

namespace FunkyBot.XMLTags
{
	[XmlElement("FunkyBountyLoad")]
	class FunkyBountyLoad : ProfileBehavior
	{
		//

		[XmlAttribute("ID")]
		[XmlAttribute("SNO")]
		public int SNO { get; set; }

		[XmlAttribute("profile")]
		[XmlAttribute("Profile")]
		public string Profile { get; set; }

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
				if (Bot.Game.Bounty.BountyQuestStates.ContainsKey(SNO) && Bot.Game.Bounty.BountyQuestStates[SNO] != QuestState.Completed)
				{
					//Set Current Bounty ID
					Bot.Game.Bounty.CurrentBountyID = SNO;

					if (!Profile.ToLower().EndsWith(".xml"))
						Profile = Profile + ".xml";

					Logger.DBLog.InfoFormat("Loading Bounty Profile {0}", Profile);
					// Now calculate our current path by checking the currently loaded profile
					var sCurrentProfilePath = Path.GetDirectoryName(GlobalSettings.Instance.LastProfile);

					// And prepare a full string of the path, and the new .xml file name
					var sNextProfile = sCurrentProfilePath + @"\" + Profile;

					ProfileManager.Load(sNextProfile);
				}
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
