using fBaseXtensions.Cache.Internal.Enums;

namespace fBaseXtensions.Settings
{
	public class SettingCombat
	{
		 public double GlobeHealthPercent { get; set; }
		 public double PotionHealthPercent { get; set; }
		 public double HealthWellHealthPercent { get; set; }
		 public int GoblinMinimumRange { get; set; }
		 public bool AllowDefaultAttackAlways { get; set; }

		 public TargetType CombatMovementTargetTypes { get; set; }

		 public SettingCombat()
		 {
			  GlobeHealthPercent=0.6d;
			  PotionHealthPercent=0.5d;
			  HealthWellHealthPercent=0.75d;
			  CombatMovementTargetTypes = TargetType.Avoidance | TargetType.Fleeing;
			  GoblinMinimumRange = 40;
			  AllowDefaultAttackAlways = false;
		 }
	}
}
