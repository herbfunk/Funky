namespace fBaseXtensions.Settings
{
	public class MonitorSettings
	{
		public int GoldInactivityTimeoutSeconds { get; set; }

		public MonitorSettings()
		{
			GoldInactivityTimeoutSeconds = 180; //3 minutes
		}

		private static MonitorSettings _monitorSettingsTag = new MonitorSettings();
		internal static MonitorSettings MonitorSettingsTag
		{
			get { return _monitorSettingsTag; }
			set { _monitorSettingsTag = value; }
		}
	}
}
