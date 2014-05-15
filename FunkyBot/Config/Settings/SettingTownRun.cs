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

		 public SettingTownRun()
		 {
			 StashHoradricCache = false;
			 EnableBloodShardGambling = false;
			 MinimumBloodShards = 100;
			 BloodShardGambleItems = BloodShardGambleItems.All;
			 BuyPotionsDuringTownRun = false;
         }
	}
}
