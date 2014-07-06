namespace FunkyBot.Config.Settings
{
	public class SettingTargeting
	{
		 public bool IgnoreAboveAverageMobs { get; set; }
		 public bool IgnoreCorpses { get; set; }
		 public bool MissleDampeningEnforceCloseRange { get; set; }
		 public int GoblinPriority { get; set; }
		 public bool[] UseShrineTypes { get; set; }
		 public bool UseExtendedRangeRepChest { get; set; }

		 public bool UnitExceptionLowHP { get; set; }
         public int UnitExceptionLowHPMaximumDistance { get; set; }
		 public bool UnitExceptionRangedUnits { get; set; }
		 public bool UnitExceptionSpawnerUnits { get; set; }
		 public bool UnitExceptionSucideBombers { get; set; }

		 public bool PrioritizeCloseRangeUnits { get; set; }
		 public int PrioritizeCloseRangeMinimumUnits { get; set; }

		 public SettingTargeting()
		 {
			  GoblinPriority=2;
			  UseShrineTypes=new [] { true, true, true, true, true, true };
			  IgnoreAboveAverageMobs=false;
			  PrioritizeCloseRangeUnits=true;
			  PrioritizeCloseRangeMinimumUnits=3;
			  IgnoreCorpses=true;
			  UseExtendedRangeRepChest=false;
			  MissleDampeningEnforceCloseRange=true;
			  UnitExceptionLowHP = false;
              UnitExceptionLowHPMaximumDistance = 20;
			  UnitExceptionRangedUnits=false;
			  UnitExceptionSpawnerUnits = false;
			  UnitExceptionSucideBombers=true;
		 }

	}
}
