namespace FunkyBot.Settings
{
	public class SettingBacktrack
	{
		public bool TrackStartOfCombatEngagment { get; set; }
		public int MinimumDistanceFromStart { get; set; }
		public bool EnableBacktracking { get; set; }
		public bool TrackLootableItems { get; set; }

		public SettingBacktrack()
		{
			MinimumDistanceFromStart = 20;
			EnableBacktracking = false;
			TrackLootableItems = true;
			TrackStartOfCombatEngagment = false;
		}
	}
}