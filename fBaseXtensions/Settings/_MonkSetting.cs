namespace fBaseXtensions.Settings
{
	public class SettingMonk
	{
		public bool bMonkSpamMantra { get; set; }
		public bool bMonkMaintainSweepingWind { get; set; }

		public bool bMonkComboStrike { get; set; }
		public int iMonkComboStrikeAbilities { get; set; }

		public SettingMonk()
		{
			bMonkComboStrike = false;
			iMonkComboStrikeAbilities = 0;
			bMonkSpamMantra = false;
			bMonkMaintainSweepingWind = false;
		}
	}
}
