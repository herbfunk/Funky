using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Config.Settings.Class
{
	public class SettingMonk
	{
		public bool RainmentsOfThousandStormsFiveBonus { get; set; }
		public bool bMonkInnaSet { get; set; }
		public bool bMonkSpamMantra { get; set; }
		public bool bMonkComboStrike { get; set; }
		public int iMonkComboStrikeAbilities { get; set; }
		public bool bMonkMaintainSweepingWind { get; set; }

		public SettingMonk()
		{
			RainmentsOfThousandStormsFiveBonus = false;
			bMonkInnaSet = false;
			bMonkSpamMantra = false;
			bMonkComboStrike = false;
			bMonkMaintainSweepingWind = false;
			iMonkComboStrikeAbilities = 0;
		}
	}
}
