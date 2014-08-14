namespace fBaseXtensions.Settings
{
	//To hold all plugin internal related variables (for advance tweaking!)
	public class SettingDeath
	{
		public bool WaitForPotionCooldown { get; set; }
		public bool WaitForAllSkillsCooldown { get; set; }

		public SettingDeath()
		{
			WaitForPotionCooldown = false;
			WaitForAllSkillsCooldown = false;
		}
	}
}
