using FunkyBot.DBHandlers;
using FunkyBot.Movement;
namespace FunkyBot.Settings
{
	 //To hold all plugin internal related variables (for advance tweaking!)
	public class SettingTownRun
	{
		 public bool StashHoradricCache { get; set; }

		 public bool EnableBloodShardGambling { get; set; }
		 public int MinimumBloodShards { get; set; }
		 public BloodShardGambleItems BloodShardGambleItems { get; set; }
		 public SettingTownRun()
		 {
			 StashHoradricCache = false;
			 MinimumBloodShards = 100;
			 BloodShardGambleItems = BloodShardGambleItems.All;
         }
	}
}
