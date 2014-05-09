using FunkyBot.Movement;
namespace FunkyBot.Settings
{
	 //To hold all plugin internal related variables (for advance tweaking!)
	public class SettingAdventureMode
	{
		 public bool NavigatePointsOfInterest { get; set; }


		 public SettingAdventureMode()
		 {
			 NavigatePointsOfInterest = false;
         }
	}
}
