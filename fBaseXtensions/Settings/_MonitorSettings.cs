namespace fBaseXtensions.Settings
{
	public class MonitorSettings
	{
		public int GoldInactivityTimeoutSeconds { get; set; }

		public MonitorSettings()
		{
			GoldInactivityTimeoutSeconds = 0; //default disabled
		}
	}
}
