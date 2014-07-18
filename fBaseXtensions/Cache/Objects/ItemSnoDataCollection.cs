using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items.Enums;

namespace fBaseXtensions.Cache.Objects
{
	public class ItemSnoDataCollection
	{
		public HashSet<ItemEntry> DroppedItemCache { get; set; } 


		public ItemSnoDataCollection()
		{
			DroppedItemCache = new HashSet<ItemEntry>
			{
				//============================= ARMOR =============================
				//
				new ItemEntry(3353, PluginDroppedItemTypes.Belt,"Belt_norm_base_04"),
				new ItemEntry(3352, PluginDroppedItemTypes.Belt,"Belt_norm_base_03"),
				new ItemEntry(3358, PluginDroppedItemTypes.Belt,"Belt_norm_base_flippy"),
				new ItemEntry(3351, PluginDroppedItemTypes.Belt),
				new ItemEntry(3354, PluginDroppedItemTypes.Belt, "Belt_norm_base_05-10802"),
				
				//
				new ItemEntry(205620, PluginDroppedItemTypes.Boots,"Boots_norm_unique_075"),
				new ItemEntry(3433, PluginDroppedItemTypes.Boots),
				new ItemEntry(3434, PluginDroppedItemTypes.Boots,"Boots_norm_base_03"),
				new ItemEntry(3439, PluginDroppedItemTypes.Boots),
				new ItemEntry(330, PluginDroppedItemTypes.Boots, "Boots_norm_base_04-11248"),

				//
				new ItemEntry(56323, PluginDroppedItemTypes.Bracers,"Bracers_norm_base_01"),
				new ItemEntry(56324, PluginDroppedItemTypes.Bracers),
				new ItemEntry(56325, PluginDroppedItemTypes.Bracers,"Bracers_norm_base_03"),
				new ItemEntry(56327, PluginDroppedItemTypes.Bracers, "Bracers_norm_base_05-15573"),
				new ItemEntry(56326, PluginDroppedItemTypes.Bracers, "Bracers_norm_base_04-6794"),

				//
				new ItemEntry(3802, PluginDroppedItemTypes.Chest,"chestArmor_norm_base_04"),
				new ItemEntry(3799, PluginDroppedItemTypes.Chest,"chestArmor_norm_base_01"),
				new ItemEntry(3800, PluginDroppedItemTypes.Chest,"chestArmor_norm_base_02"),
				new ItemEntry(3801, PluginDroppedItemTypes.Chest),
				new ItemEntry(3813, PluginDroppedItemTypes.Chest,"chestArmor_norm_base_flippy"),
				new ItemEntry(3803, PluginDroppedItemTypes.Chest, "chestArmor_norm_base_05-7757"),

				//
				new ItemEntry(4264, PluginDroppedItemTypes.Gloves,"Gloves_norm_base_flippy"),

				//
				new ItemEntry(205482, PluginDroppedItemTypes.Helm,"HelmCloth_norm_base_flippy"),
				new ItemEntry(4463, PluginDroppedItemTypes.Helm,"Helm_norm_base_flippy"),
				
				//
				new ItemEntry(4833, PluginDroppedItemTypes.Pants,"pants_norm_base_flippy"),

				//
				new ItemEntry(5288, PluginDroppedItemTypes.Shoulders,"shoulderPads_norm_base_flippy"),

				//============================= WEAPONS =============================
				//
				new ItemEntry(3254, PluginDroppedItemTypes.Axe,"Axe_norm_base_flippy_02"),
				new ItemEntry(3255, PluginDroppedItemTypes.Axe),
				new ItemEntry(367144, PluginDroppedItemTypes.Axe),
				new ItemEntry(3258, PluginDroppedItemTypes.Axe, "Axe_norm_base_flippy_06-18875"),
				new ItemEntry(3257, PluginDroppedItemTypes.Axe, "Axe_norm_base_flippy_05-9883"),

				//
				new ItemEntry(482, PluginDroppedItemTypes.AxeTwoHanded,"twoHandedAxe_norm_base_flippy_02"),
				new ItemEntry(6325, PluginDroppedItemTypes.AxeTwoHanded,"twoHandedAxe_norm_base_flippy_01"),
				new ItemEntry(186497, PluginDroppedItemTypes.AxeTwoHanded,"twoHandedAxe_norm_unique_flippy_03"),
				new ItemEntry(367146, PluginDroppedItemTypes.AxeTwoHanded,"x1_twoHandedAxe_norm_base_flippy_02"),
			
				//
				new ItemEntry(331, PluginDroppedItemTypes.Bow,"Bow_norm_base_flippy_01"),
				new ItemEntry(367158, PluginDroppedItemTypes.Bow,"x1_bow_norm_base_flippy_02"),
				new ItemEntry(3456, PluginDroppedItemTypes.Bow, "Bow_norm_base_flippy_04-11164"),

				//
				new ItemEntry(367199, PluginDroppedItemTypes.CeremonialKnife),

				//
				new ItemEntry(6618, PluginDroppedItemTypes.Crossbow, "XBow_norm_base_flippy_02"),
				new ItemEntry(6619, PluginDroppedItemTypes.Crossbow,"XBow_norm_base_flippy_03"),
				new ItemEntry(6620, PluginDroppedItemTypes.Crossbow),
				new ItemEntry(367161, PluginDroppedItemTypes.Crossbow,"x1_xbow_norm_base_flippy_02"),
				new ItemEntry(6621, PluginDroppedItemTypes.Crossbow, "XBow_norm_base_flippy_05-10406"),

				//
				new ItemEntry(3843, PluginDroppedItemTypes.Daibo, "combatStaff_norm_base_flippy_03-15187"),

				//
				new ItemEntry(3911, PluginDroppedItemTypes.Dagger),
				new ItemEntry(367137, PluginDroppedItemTypes.Dagger),
				new ItemEntry(3914, PluginDroppedItemTypes.Dagger, "Dagger_norm_base_flippy_05-11593"),

				//
				//new ItemEntry(000000, PluginDroppedItemTypes.FistWeapon),

				//
				new ItemEntry(247389, PluginDroppedItemTypes.Flail,"x1_flail1H_norm_base_flippy_02"),
				new ItemEntry(247392, PluginDroppedItemTypes.Flail,"x1_flail1H_norm_base_flippy_05"),
				new ItemEntry(247390, PluginDroppedItemTypes.Flail, "x1_flail1H_norm_base_flippy_03-8053"),

				//
				new ItemEntry(247357, PluginDroppedItemTypes.FlailTwoHanded,"x1_flail2H_norm_base_flippy_01"),
				new ItemEntry(247395, PluginDroppedItemTypes.FlailTwoHanded,"x1_flail2H_norm_base_flippy_02"),
				new ItemEntry(247397, PluginDroppedItemTypes.FlailTwoHanded, "x1_flail2H_norm_base_flippy_04-17083"),

				//
				new ItemEntry(145119, PluginDroppedItemTypes.HandCrossbow,"handXBow_norm_base_flippy_05"),
				new ItemEntry(145093, PluginDroppedItemTypes.HandCrossbow),
				new ItemEntry(367186, PluginDroppedItemTypes.HandCrossbow),

				//
				new ItemEntry(4653, PluginDroppedItemTypes.Mace,"Mace_norm_base_flippy_01"),
				new ItemEntry(367148, PluginDroppedItemTypes.Mace,"x1_Mace_norm_base_flippy_02"),
				new ItemEntry(4657, PluginDroppedItemTypes.Mace, "Mace_norm_base_flippy_05-14517"),
				new ItemEntry(4656, PluginDroppedItemTypes.Mace, "Mace_norm_base_flippy_04-11695"),

				//
				new ItemEntry(6337, PluginDroppedItemTypes.MaceTwoHanded),
				new ItemEntry(367152, PluginDroppedItemTypes.MaceTwoHanded),
				new ItemEntry(6338, PluginDroppedItemTypes.MaceTwoHanded, "twoHandedMace_norm_base_flippy_03-3310"),

				//
				//new ItemEntry(000000, PluginDroppedItemTypes.MightyWeapon),

				//
				new ItemEntry(367171, PluginDroppedItemTypes.MightyWeaponTwoHanded,"x1_mightyWeapon_2H_norm_base_flippy_02"),

				//
				new ItemEntry(4874, PluginDroppedItemTypes.Polearm,"Polearm_norm_base_flippy_02"),
				new ItemEntry(4875, PluginDroppedItemTypes.Polearm, "Polearm_norm_base_flippy_03-970"),

				//
				new ItemEntry(5457, PluginDroppedItemTypes.Spear,"Spear_norm_base_flippy_01"),
				new ItemEntry(5458, PluginDroppedItemTypes.Spear),
				new ItemEntry(367156, PluginDroppedItemTypes.Spear),
				new ItemEntry(5460, PluginDroppedItemTypes.Spear, "Spear_norm_base_flippy_04-19280"),
				new ItemEntry(5459, PluginDroppedItemTypes.Spear, "Spear_norm_base_flippy_03-2545"),

				//
				new ItemEntry(5491, PluginDroppedItemTypes.Staff, "Staff_norm_base_flippy_02"),
				new ItemEntry(5493, PluginDroppedItemTypes.Staff, "Staff_norm_base_flippy_04-16574"),

				//
				new ItemEntry(5529, PluginDroppedItemTypes.Sword, "Sword_norm_base_flippy_03"),
				new ItemEntry(367140, PluginDroppedItemTypes.Sword),

				//
				new ItemEntry(6348, PluginDroppedItemTypes.SwordTwoHanded,"twoHandedSword_norm_base_flippy_02"),
				new ItemEntry(6349, PluginDroppedItemTypes.SwordTwoHanded,"twoHandedSword_norm_base_flippy_03"),
				new ItemEntry(367142, PluginDroppedItemTypes.SwordTwoHanded,"x1_twoHandedSword_norm_base_flippy_02"),
				new ItemEntry(6351, PluginDroppedItemTypes.SwordTwoHanded, "twoHandedSword_norm_base_flippy_05-13187"),

				//
				new ItemEntry(367203, PluginDroppedItemTypes.Wand,"x1_Wand_norm_base_flippy_02"),
				new ItemEntry(6430, PluginDroppedItemTypes.Wand, "Wand_norm_base_flippy_04-12251"),

				//============================= JEWELERY =============================
				new ItemEntry(3188, PluginDroppedItemTypes.Amulet),
				new ItemEntry(63985, PluginDroppedItemTypes.Ring),
				new ItemEntry(193056, PluginDroppedItemTypes.FollowerTrinket),

				//============================= OFFHAND =============================

				//
				new ItemEntry(335033, PluginDroppedItemTypes.CrusaderShield,"x1_CruShield_norm_base_flippy_03"),
				new ItemEntry(335038, PluginDroppedItemTypes.CrusaderShield),
				new ItemEntry(367176, PluginDroppedItemTypes.CrusaderShield,"x1_CruShield_norm_base_flippy_08"),
				new ItemEntry(335039, PluginDroppedItemTypes.CrusaderShield, "x1_CruShield_norm_base_flippy_05-3185"),

				//
				new ItemEntry(146942, PluginDroppedItemTypes.Mojo,"Mojo_norm_base_flippy_02"),
				new ItemEntry(216525, PluginDroppedItemTypes.Mojo),
				new ItemEntry(367196, PluginDroppedItemTypes.Mojo),

				//
				new ItemEntry(218695, PluginDroppedItemTypes.Quiver),

				//
				new ItemEntry(5268, PluginDroppedItemTypes.Shield,"Shield_norm_base_flippy_02"),
				new ItemEntry(5269, PluginDroppedItemTypes.Shield,"Shield_norm_base_flippy_03"),
				new ItemEntry(367165, PluginDroppedItemTypes.Shield,"x1_Shield_norm_base_flippy_02"),
				new ItemEntry(152660, PluginDroppedItemTypes.Shield, "Shield_norm_unique_flippy_03-18333"),

				//
				//new ItemEntry(000000, PluginDroppedItemTypes.Source),


				//============================= CRAFTING =============================
				new ItemEntry(137958, PluginDroppedItemTypes.CraftingMaterial,"CraftingMaterials_Flippy_Global"),

				new ItemEntry(364695,PluginDroppedItemTypes.InfernalKey), 
				new ItemEntry(364697, PluginDroppedItemTypes.InfernalKey),
				new ItemEntry(364694,PluginDroppedItemTypes.InfernalKey),
				new ItemEntry(364696, PluginDroppedItemTypes.InfernalKey),
				new ItemEntry(255880, PluginDroppedItemTypes.InfernalKey),
				new ItemEntry(255881, PluginDroppedItemTypes.InfernalKey),
				new ItemEntry(255882,PluginDroppedItemTypes.InfernalKey),

				new ItemEntry(323722, PluginDroppedItemTypes.KeyFragment,"LootRunKey"),

				//============================= MISC =============================
				new ItemEntry(301283, PluginDroppedItemTypes.PowerGlobe,"Console_PowerGlobe"),
				new ItemEntry(4267, PluginDroppedItemTypes.HealthGlobe,"HealthGlobe"),
				new ItemEntry(85798, PluginDroppedItemTypes.HealthGlobe,"HealthGlobe_02"),

				//
				new ItemEntry(376, PluginDroppedItemTypes.Gold,"GoldCoin"),
				new ItemEntry(209200, PluginDroppedItemTypes.Gold),
				new ItemEntry(4311, PluginDroppedItemTypes.Gold,"GoldLarge"),
				new ItemEntry(4312, PluginDroppedItemTypes.Gold,"GoldMedium"),
				new ItemEntry(4313, PluginDroppedItemTypes.Gold,"GoldSmall"),

				new ItemEntry(304319, PluginDroppedItemTypes.Potion,"healthPotion_Console"),

				new ItemEntry(192866, PluginDroppedItemTypes.LoreBook),
				new ItemEntry(218853, PluginDroppedItemTypes.LoreBook, "Lore_Book_Flippy-10504"),

				new ItemEntry(359504, PluginDroppedItemTypes.BloodShard),
				//============================= GEMS =============================

				new ItemEntry(56860, PluginDroppedItemTypes.Amethyst, "Amethyst_01-7233"),
				new ItemEntry(56863, PluginDroppedItemTypes.Amethyst), 
				new ItemEntry(56864, PluginDroppedItemTypes.Amethyst), 
				new ItemEntry(56865, PluginDroppedItemTypes.Amethyst), 
				new ItemEntry(56866,PluginDroppedItemTypes.Amethyst), 
				new ItemEntry(56867,PluginDroppedItemTypes.Amethyst), 
				new ItemEntry(283116,PluginDroppedItemTypes.Amethyst), 
				new ItemEntry(361564,PluginDroppedItemTypes.Amethyst), 
				new ItemEntry(361565,PluginDroppedItemTypes.Amethyst), 
				new ItemEntry(361566,PluginDroppedItemTypes.Amethyst), 
				new ItemEntry(361567,PluginDroppedItemTypes.Amethyst), 

				new ItemEntry(56874, PluginDroppedItemTypes.Diamond, "Diamond_01-7228"),
				new ItemEntry(56877, PluginDroppedItemTypes.Diamond), 
				new ItemEntry(56878,PluginDroppedItemTypes.Diamond),  
				new ItemEntry(56879, PluginDroppedItemTypes.Diamond), 
				new ItemEntry(56880,PluginDroppedItemTypes.Diamond), 
				new ItemEntry(56881, PluginDroppedItemTypes.Diamond), 
				new ItemEntry(361559,PluginDroppedItemTypes.Diamond), 
				new ItemEntry(361560,PluginDroppedItemTypes.Diamond), 
				new ItemEntry(361561, PluginDroppedItemTypes.Diamond), 
				new ItemEntry(361562, PluginDroppedItemTypes.Diamond), 
				new ItemEntry(361563,PluginDroppedItemTypes.Diamond),

				new ItemEntry(56888, PluginDroppedItemTypes.Emerald, "Emerald_01-9486"),
				new ItemEntry(56889, PluginDroppedItemTypes.Emerald, "Emerald_02-15787"),
				new ItemEntry(56891,PluginDroppedItemTypes.Emerald), 
				new ItemEntry(56892, PluginDroppedItemTypes.Emerald),
				new ItemEntry(56893, PluginDroppedItemTypes.Emerald),
				new ItemEntry(56894, PluginDroppedItemTypes.Emerald),
				new ItemEntry(56895, PluginDroppedItemTypes.Emerald),
				new ItemEntry(283117,PluginDroppedItemTypes.Emerald), 
				new ItemEntry(361492, PluginDroppedItemTypes.Emerald),
				new ItemEntry(361493, PluginDroppedItemTypes.Emerald),
				new ItemEntry(361494, PluginDroppedItemTypes.Emerald),
				new ItemEntry(361495,PluginDroppedItemTypes.Emerald),

				new ItemEntry(56846, PluginDroppedItemTypes.Ruby, "Ruby_01-11255"),
				new ItemEntry(56849, PluginDroppedItemTypes.Ruby), 
				new ItemEntry(56850,  PluginDroppedItemTypes.Ruby),
				new ItemEntry(56851,  PluginDroppedItemTypes.Ruby),
				new ItemEntry(56852,  PluginDroppedItemTypes.Ruby),
				new ItemEntry(56853,  PluginDroppedItemTypes.Ruby),
				new ItemEntry(283118,  PluginDroppedItemTypes.Ruby),
				new ItemEntry(361568,  PluginDroppedItemTypes.Ruby),
				new ItemEntry(361569,  PluginDroppedItemTypes.Ruby),
				new ItemEntry(361570,  PluginDroppedItemTypes.Ruby),
				new ItemEntry(361571, PluginDroppedItemTypes.Ruby),

				new ItemEntry(5625, PluginDroppedItemTypes.Topaz),
				new ItemEntry(56919, PluginDroppedItemTypes.Topaz),
				new ItemEntry(56920, PluginDroppedItemTypes.Topaz),
				new ItemEntry(56921, PluginDroppedItemTypes.Topaz),
				new ItemEntry(56922,PluginDroppedItemTypes.Topaz),
				new ItemEntry(56923, PluginDroppedItemTypes.Topaz),
				new ItemEntry(283119, PluginDroppedItemTypes.Topaz),
				new ItemEntry(361572, PluginDroppedItemTypes.Topaz),
				new ItemEntry(361573, PluginDroppedItemTypes.Topaz),
				new ItemEntry(361574, PluginDroppedItemTypes.Topaz),
				new ItemEntry(361575,PluginDroppedItemTypes.Topaz)
			};

			
		}

		private static readonly string DefaultFilePath = Path.Combine(FolderPaths.PluginPath, "Cache", "Dictionaries", "SNOId_Cache_Items.xml");
		internal static ItemSnoDataCollection DeserializeFromXML()
		{
			var deserializer = new XmlSerializer(typeof(ItemSnoDataCollection));
			TextReader textReader = new StreamReader(DefaultFilePath);
			var settings = (ItemSnoDataCollection)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
		internal static void SerializeToXML(ItemSnoDataCollection settings)
		{
			var serializer = new XmlSerializer(typeof(ItemSnoDataCollection));
			var textWriter = new StreamWriter(DefaultFilePath);
			serializer.Serialize(textWriter, settings);
			textWriter.Close();
		}

		public class ItemSnoEntry
		{
			public int Sno { get; set; }
			public PluginDroppedItemTypes Type { get; set; }

			public ItemSnoEntry()
			{
				Sno = -1;
				Type = PluginDroppedItemTypes.Unknown;
			}
			public ItemSnoEntry(int sno, PluginDroppedItemTypes type)
			{
				Sno = sno;
				Type = type;
			}

			public override int GetHashCode()
			{
				return Sno;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				var p = obj as ItemSnoEntry;
				if (p == null)
					return false;
				return (Sno == p.Sno);
			}
		}

	}
}
