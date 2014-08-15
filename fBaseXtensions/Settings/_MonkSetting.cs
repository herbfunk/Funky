namespace fBaseXtensions.Settings
{
	public class SettingMonk
	{
		public bool bMonkSpamMantra { get; set; }
		public bool bMonkComboStrike { get; set; }
		public int iMonkComboStrikeAbilities { get; set; }
		public bool bMonkMaintainSweepingWind { get; set; }

		public SettingMonk()
		{
			bMonkSpamMantra = false;
			bMonkComboStrike = false;
			bMonkMaintainSweepingWind = false;
			iMonkComboStrikeAbilities = 0;
		}
	}
}
