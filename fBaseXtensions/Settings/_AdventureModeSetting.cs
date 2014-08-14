namespace fBaseXtensions.Settings
{
	 //To hold all plugin internal related variables (for advance tweaking!)
	public class SettingAdventureMode
	{
		 public bool EnableAdventuringMode { get; set; }
		 public bool NavigatePointsOfInterest { get; set; }


		 public SettingAdventureMode()
		 {
			 EnableAdventuringMode = true;
			 NavigatePointsOfInterest = false;
         }

		 private static SettingAdventureMode adventureModeSettingsTag = new SettingAdventureMode();
		 internal static SettingAdventureMode AdventureModeSettingsTag
		 {
			 get { return adventureModeSettingsTag; }
			 set { adventureModeSettingsTag = value; }
		 }
	}
}
