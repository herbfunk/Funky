namespace fBaseXtensions.Settings
{
	public class MonitorSettings
	{
		public int GoldInactivityTimeoutSeconds { get; set; }

		public MonitorSettings()
		{
			GoldInactivityTimeoutSeconds = 180; //3 minutes
		}
	}
}
