using fBaseXtensions.Helpers;

namespace fBaseXtensions.Settings
{
	public class LoggerSettings
	{
		public LogLevel LogFlags { get; set; }

		public LoggerSettings()
		{
			LogFlags = LogLevel.None;
		}
	}
}
