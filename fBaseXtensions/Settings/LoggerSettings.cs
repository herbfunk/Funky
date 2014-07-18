using fBaseXtensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
