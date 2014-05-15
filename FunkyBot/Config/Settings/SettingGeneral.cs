namespace FunkyBot.Config.Settings
{
	 //To hold all plugin internal related variables (for advance tweaking!)
	public class SettingGeneral
	{
		public int AfterCombatDelay { get; set; }
		public bool EnableWaitAfterContainers { get; set; }
		public bool OutOfCombatMovement { get; set; }
		public bool AllowBuffingInTown { get; set; }

		 public SettingGeneral()
		 {
			 EnableWaitAfterContainers = false;
			 AfterCombatDelay = 500;
			 OutOfCombatMovement = false;
			 AllowBuffingInTown = false;
         }
	}
}
