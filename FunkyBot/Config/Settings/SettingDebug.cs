using FunkyBot.Misc;

namespace FunkyBot.Config.Settings
{
	public class SettingDebug
	{
		 public bool DebugStatusBar { get; set; }
		 public bool LogGroupingOutput { get; set; }
		 public LogLevel FunkyLogFlags { get; set; }
		 public bool SkipAhead { get; set; }
		 public bool LogStuckLocations { get; set; }
		 public bool EnableUnstucker { get; set; }
		 public bool RestartGameOnLongStucks { get; set; }
		 public bool DebuggingData { get; set; }

		 public SettingDebug()
		 {
			  DebuggingData = false;
			  DebugStatusBar=true;
			  FunkyLogFlags=LogLevel.None;
			  EnableUnstucker=true;
			  RestartGameOnLongStucks=true;
			  LogStuckLocations=true;
			  SkipAhead=true;
		 }
	}
}
