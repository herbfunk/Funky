using System.Runtime.InteropServices;
using fBaseXtensions.Helpers;
using fBaseXtensions.Settings;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace fBaseXtensions.XML
{
	[ComVisible(false)]
	[XmlElement("FunkySetAdventureMode")]
	public class FunkyAdventureMode : ProfileBehavior
	{
		[XmlAttribute("enabled")]
		public bool Enabled { get; set; }


		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
				SettingAdventureMode.AdventureModeSettingsTag.AllowCombatModifications = Enabled;
				Logger.DBLog.InfoFormat("[Funky] Adventureing Mode has been set to {0}", Enabled);
				m_IsDone=true;
			});
		}

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}
		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}