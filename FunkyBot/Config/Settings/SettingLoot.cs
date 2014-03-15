using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Settings
{
	public class SettingLoot
	{
		//Plugin Item Default Settings
		public int GilesMinimumWeaponScore { get; set; }
		public int GilesMinimumArmorScore { get; set; }
		public int GilesMinimumJeweleryScore { get; set; }

		public int[] MinimumWeaponItemLevel { get; set; }
		public int[] MinimumArmorItemLevel { get; set; }
		public int[] MinimumJeweleryItemLevel { get; set; }
		public int MinimumLegendaryItemLevel { get; set; }

		public int MaximumHealthPotions { get; set; }
		public int MinimumGoldPile { get; set; }

		//red, green, purple, yellow, white
		public bool[] PickupGems { get; set; }
		public bool PickupGemDiamond { get; set; }
		public int MinimumGemItemLevel { get; set; }
		public bool PickupCraftTomes { get; set; }
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
		public bool PickupFollowerItems { get; set; }
		public bool PickupInfernalKeys { get; set; }
		public bool PickupDemonicEssence { get; set; }
		public int MiscItemLevel { get; set; }

		public SettingLoot()
		{
			GilesMinimumWeaponScore=75000;
			GilesMinimumArmorScore=30000;
			GilesMinimumJeweleryScore=30000;
			MinimumWeaponItemLevel=new int[] { 0, 59 };
			MinimumArmorItemLevel=new int[] { 0, 59 };
			MinimumJeweleryItemLevel=new int[] { 0, 55 };
			MinimumLegendaryItemLevel=59;
			MaximumHealthPotions=100;
			MinimumGoldPile=425;
			PickupCraftTomes=true;
			PickupCraftPlans=true;
			PickupBlacksmithPlanSix=false;
			PickupBlacksmithPlanFive=false;
			PickupBlacksmithPlanFour=false;
			PickupBlacksmithPlanRazorspikes=false;
			PickupBlacksmithPlanArchonGauntlets=false;
			PickupBlacksmithPlanArchonSpaulders=false;
			PickupJewelerDesignFlawlessStar=false;
			PickupJewelerDesignPerfectStar=false;
			PickupJewelerDesignRadiantStar=false;
			PickupJewelerDesignMarquise=false;
			PickupJewelerDesignAmulet=false;
			PickupInfernalKeys=true;
			PickupDemonicEssence=true;
			PickupFollowerItems=true;
			MiscItemLevel=59;
			MinimumGemItemLevel=60;
			PickupGems=new bool[] { true, true, false, false, true };
			PickupGemDiamond = true;
		}

		private static string DefaultFilePath=Path.Combine(FolderPaths.SettingsDefaultPath, "Specific", "Loot_Default.xml");
		public static SettingLoot DeserializeFromXML()
		{
			 XmlSerializer deserializer=new XmlSerializer(typeof(SettingLoot));
			 TextReader textReader=new StreamReader(DefaultFilePath);
			 SettingLoot settings;
			 settings=(SettingLoot)deserializer.Deserialize(textReader);
			 textReader.Close();
			 return settings;
		}
		public static SettingLoot DeserializeFromXML(string Path)
		{
			 XmlSerializer deserializer=new XmlSerializer(typeof(SettingLoot));
			 TextReader textReader=new StreamReader(Path);
			 SettingLoot settings;
			 settings=(SettingLoot)deserializer.Deserialize(textReader);
			 textReader.Close();
			 return settings;
		}
	}
}