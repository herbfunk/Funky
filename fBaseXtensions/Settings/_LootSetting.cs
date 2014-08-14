namespace fBaseXtensions.Settings
{
	public class SettingLoot
	{
		//0 == None, 1 == All, 61 == ROS Only
		public int PickupLegendaryItems { get; set; }
		public int PickupRareItems { get; set; }
		public int PickupMagicItems { get; set; }
		public int PickupWhiteItems { get; set; }

		public int MaximumHealthPotions { get; set; }
		public int MinimumGoldPile { get; set; }

		//red, green, purple, yellow, white
		public bool[] PickupGems { get; set; }
		public bool PickupGemDiamond { get; set; }
		public int MinimumGemItemLevel { get; set; }
		
		public bool PickupCraftPlans { get; set; }
		public bool PickupBlacksmithPlanSix { get; set; }
		public bool PickupBlacksmithPlanFive { get; set; }
		public bool PickupBlacksmithPlanFour { get; set; }
		public bool PickupBlacksmithPlanArchonGauntlets { get; set; }
		public bool PickupBlacksmithPlanArchonSpaulders { get; set; }
		public bool PickupBlacksmithPlanRazorspikes { get; set; }
		public bool PickupJewelerDesignFlawlessStar { get; set; }
		public bool PickupJewelerDesignPerfectStar { get; set; }
		public bool PickupJewelerDesignRadiantStar { get; set; }
		public bool PickupJewelerDesignMarquise { get; set; }
		public bool PickupJewelerDesignAmulet { get; set; }


		
		public bool PickupInfernalKeys { get; set; }
		public bool PickupKeystoneFragments { get; set; }
		public bool PickupCraftMaterials { get; set; }

		public bool ExpBooks { get; set; }

		public SettingLoot()
		{
			PickupWhiteItems = 1;
			PickupMagicItems = 1;
			PickupRareItems = 1;
			PickupLegendaryItems = 1;

			MaximumHealthPotions=100;
			MinimumGoldPile=1000;
			PickupCraftPlans=true;
			PickupBlacksmithPlanSix=true;
			PickupBlacksmithPlanFive=true;
			PickupBlacksmithPlanFour=true;
			PickupBlacksmithPlanRazorspikes=true;
			PickupBlacksmithPlanArchonGauntlets=true;
			PickupBlacksmithPlanArchonSpaulders=true;
			PickupJewelerDesignFlawlessStar=true;
			PickupJewelerDesignPerfectStar=true;
			PickupJewelerDesignRadiantStar=true;
			PickupJewelerDesignMarquise=true;
			PickupJewelerDesignAmulet=true;
			PickupInfernalKeys=true;

			
			PickupCraftMaterials=true;
			PickupKeystoneFragments = true;

			MinimumGemItemLevel=67;
			PickupGems = new bool[] { true, true, true, true, true };
			PickupGemDiamond = true;
			ExpBooks = true;
		}
	}
}