using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fBaseXtensions.Settings
{
	public class MonitorSettings
	{
		public int GoldInactivityTimeoutSeconds { get; set; }

		public MonitorSettings()
		{
			GoldInactivityTimeoutSeconds = 180; //default of 3 minutes
		}
	}
}
