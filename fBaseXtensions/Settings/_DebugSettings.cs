using fBaseXtensions.Cache.Internal.Enums;

namespace fBaseXtensions.Settings
{
	public class DebugSettings
	{
		 public bool DebugStatusBar { get; set; }
		 public bool LogGroupingOutput { get; set; }
		 public bool SkipAhead { get; set; }
		 public bool LogStuckLocations { get; set; }
		 public bool EnableUnstucker { get; set; }
		 public bool RestartGameOnLongStucks { get; set; }
		 public bool DebuggingData { get; set; }
		 public DebugDataTypes DebuggingDataTypes { get; set; }
		 public DebugSettings()
		 {
			  DebuggingData = false;
			  DebugStatusBar=true;
			  EnableUnstucker=true;
			  RestartGameOnLongStucks=true;
			  LogStuckLocations=true;
			  SkipAhead=true;
			  DebuggingDataTypes = DebugDataTypes.None;
		 }
	}
}
