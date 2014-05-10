using FunkyBot.Movement;
namespace FunkyBot.Settings
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
	}
}
