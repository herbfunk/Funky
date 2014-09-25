using System;
using System.Collections.Generic;
using System.Linq;
using fBaseXtensions.Items.Enums;

namespace fBaseXtensions.Settings
{
	//To hold all plugin internal related variables (for advance tweaking!)
	public class SettingAdventureMode
	{
		public bool EnableAdventuringMode { get; set; }
		public bool NavigatePointsOfInterest { get; set; }
		public int MaximumTieredRiftKeyAllowed { get; set; }
		public double GemUpgradingMinimumSuccessRate { get; set; }
		public List<LegendaryGemTypes> GemUpgradePriorityList { get; set; }

		public enum GemUpgradingType
		{
			None,
			HighestRank,
			LowestRank,
			Priority
		}
		public GemUpgradingType GemUpgradeType { get; set; }

		public SettingAdventureMode()
		{
			EnableAdventuringMode = true;
			NavigatePointsOfInterest = false;
			GemUpgradeType = GemUpgradingType.None;
			GemUpgradePriorityList = new List<LegendaryGemTypes>();
			GemUpgradingMinimumSuccessRate = 0.08;
			MaximumTieredRiftKeyAllowed = 100;
		}

		private static SettingAdventureMode adventureModeSettingsTag = new SettingAdventureMode();
		internal static SettingAdventureMode AdventureModeSettingsTag
		{
			get { return adventureModeSettingsTag; }
			set { adventureModeSettingsTag = value; }
		}
	}
}
