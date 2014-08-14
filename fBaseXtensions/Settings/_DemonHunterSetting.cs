namespace fBaseXtensions.Settings
{
	public class SettingDemonHunter
	{
		public int iDHVaultMovementDelay { get; set; }
		public bool BombadiersRucksack { get; set; }
		public bool FullMarauderSet { get; set; }

		public SettingDemonHunter()
		{
			iDHVaultMovementDelay = 400;
			BombadiersRucksack = false;
			FullMarauderSet = false;
		}
	}
}
