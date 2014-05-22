using FunkyBot.DBHandlers;

namespace FunkyBot.Config.Settings
{
	 //To hold all plugin internal related variables (for advance tweaking!)
	public class SettingTownRun
	{
		 public bool StashHoradricCache { get; set; }

		 public bool EnableBloodShardGambling { get; set; }
		 public int MinimumBloodShards { get; set; }
		 public BloodShardGambleItems BloodShardGambleItems { get; set; }
		 public bool BuyPotionsDuringTownRun { get; set; }

		 //0 == Ignore, 1 == All, 61 == ROS Only
		 public int SalvageWhiteItemLevel { get; set; }
		 public int SalvageMagicItemLevel { get; set; }
		 public int SalvageRareItemLevel { get; set; }
		 public int SalvageLegendaryItemLevel { get; set; }

		 public SettingTownRun()
		 {
			 StashHoradricCache = false;
			 EnableBloodShardGambling = false;
			 MinimumBloodShards = 100;
			 BloodShardGambleItems = BloodShardGambleItems.All;
			 BuyPotionsDuringTownRun = false;

			 SalvageWhiteItemLevel = 0;
			 SalvageMagicItemLevel = 0;
			 SalvageRareItemLevel = 0;
			 SalvageLegendaryItemLevel = 0;
         }
	}
}
