namespace fBaseXtensions.Settings
{
	 //To hold all plugin internal related variables (for advance tweaking!)
	public class GeneralSettings
	{
		public int AfterCombatDelay { get; set; }
		public bool EnableWaitAfterContainers { get; set; }
		public bool OutOfCombatMovement { get; set; }
		public bool AllowBuffingInTown { get; set; }
        public int AltHeroIndex { get; set; }
		 public GeneralSettings()
		 {
			 EnableWaitAfterContainers = false;
			 AfterCombatDelay = 500;
			 OutOfCombatMovement = false;
			 AllowBuffingInTown = false;
		     AltHeroIndex = -1;
		 }
	}
}
