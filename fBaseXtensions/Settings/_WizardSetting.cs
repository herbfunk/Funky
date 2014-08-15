namespace fBaseXtensions.Settings
{
	public class SettingWizard
	{
		public bool bWaitForArchon { get; set; }
		public bool bKiteOnlyArchon { get; set; }
		public bool bCancelArchonRebuff { get; set; }
		public bool bTeleportIntoGrouping { get; set; }
		public bool bTeleportFleeWhenLowHP { get; set; }


		public SettingWizard()
		{
			bTeleportIntoGrouping = false;
			bTeleportFleeWhenLowHP = true;
			bCancelArchonRebuff = false;
			bWaitForArchon = false;
			bKiteOnlyArchon = true;
		}
	}
}
