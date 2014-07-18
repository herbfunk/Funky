using System.Collections.Generic;
using fBaseXtensions.Items.Enums;

namespace fBaseXtensions.Items
{
	public static class ItemSnoCache
	{
		/*
		 * Item ActorSNO: 301283 Name: Console_PowerGlobe-3171 
		 * Item ActorSNO: 4267 Name: HealthGlobe-3396 
		 * 
		 * Item ActorSNO: 4311 Name: GoldLarge-2701 
		 * Item ActorSNO: 4312 Name: GoldMedium-3188 
		 * Item ActorSNO: 4313 Name: GoldSmall-2696 
		 * 
		 * Item ActorSNO: 323722 Name: LootRunKey-580 (Keyfragment)
		 * Item ActorSNO: 137958 Name: CraftingMaterials_Flippy_Global-2511 (Arcane Dust)
		 * 
		 * 
		 * Item ActorSNO: 4833 Name: pants_norm_base_flippy-3170 
		 * Item Name: shoulderPads_norm_base_flippy-2081 ActorSNO: 5288
		 * Item ActorSNO: 4264 Name: Gloves_norm_base_flippy-3576 
		 * Item ActorSNO: 367158 Name: x1_bow_norm_base_flippy_02-4565 
		 * Item ActorSNO: 3358 Name: Belt_norm_base_flippy-4934 
		 * Item ActorSNO: 4463 Name: Helm_norm_base_flippy-5277 
		 * Item ActorSNO: 367203 Name: x1_Wand_norm_base_flippy_02-4032 
		 * Item ActorSNO: 367146 Name: x1_twoHandedAxe_norm_base_flippy_02-4047 
		 * Item ActorSNO: 3813 Name: chestArmor_norm_base_flippy-4380 
		 * Item ActorSNO: 367165 Name: x1_Shield_norm_base_flippy_02-4398 
		 * 
		 */

		public static readonly Dictionary<int, PluginItemTypes> ReaperOfSoulItemTypes = new Dictionary<int, PluginItemTypes>
		{
			{325062, PluginItemTypes.Amulet},
			{5044, PluginItemTypes.Ring},{5043, PluginItemTypes.Ring},

			{253996, PluginItemTypes.Belt},{253987, PluginItemTypes.Belt},
			{253997, PluginItemTypes.Boots},{253986, PluginItemTypes.Boots},
			{253988, PluginItemTypes.Bracers},{253995, PluginItemTypes.Bracers},

			{253994, PluginItemTypes.Chest},{253983, PluginItemTypes.Chest},
			{367188, PluginItemTypes.Cloak},{335376, PluginItemTypes.Cloak},
			{253993, PluginItemTypes.Gloves},{253985, PluginItemTypes.Gloves},
			{253992, PluginItemTypes.Helm},{239290, PluginItemTypes.Helm},
			{253984, PluginItemTypes.Pants},{253991, PluginItemTypes.Pants},
			{367173, PluginItemTypes.MightyBelt},{367172, PluginItemTypes.MightyBelt},
			{253990, PluginItemTypes.Shoulders},{239289, PluginItemTypes.Shoulders},
			{335392, PluginItemTypes.SpiritStone},{364156, PluginItemTypes.SpiritStone},
			{367197, PluginItemTypes.VoodooMask},{335387, PluginItemTypes.VoodooMask},
			{367201, PluginItemTypes.WizardHat},{335378, PluginItemTypes.WizardHat},


			{367143, PluginItemTypes.Axe},{335155, PluginItemTypes.Axe},
			{335159, PluginItemTypes.TwoHandAxe},{367145, PluginItemTypes.TwoHandAxe},
			{367157, PluginItemTypes.TwoHandBow},{335186, PluginItemTypes.TwoHandBow},
			{326811, PluginItemTypes.CeremonialKnife},{367198, PluginItemTypes.CeremonialKnife},
			{335189, PluginItemTypes.TwoHandCrossbow},{367159, PluginItemTypes.TwoHandCrossbow},
			{367136, PluginItemTypes.Dagger},{335128, PluginItemTypes.Dagger},
			{327966, PluginItemTypes.TwoHandDaibo},{367191, PluginItemTypes.TwoHandDaibo},
			{247381, PluginItemTypes.Flail},{247380, PluginItemTypes.Flail},
			{247387, PluginItemTypes.TwoHandFlail},{247386, PluginItemTypes.TwoHandFlail},
			{367193, PluginItemTypes.FistWeapon},{328572, PluginItemTypes.FistWeapon},
			{335369, PluginItemTypes.HandCrossbow},{367185, PluginItemTypes.HandCrossbow},
			{335169, PluginItemTypes.TwoHandMace},{367151, PluginItemTypes.TwoHandMace},
			{367147, PluginItemTypes.Mace},{335166, PluginItemTypes.Mace},
			{367168, PluginItemTypes.MightyWeapon},{335340, PluginItemTypes.MightyWeapon},
			{367170, PluginItemTypes.TwoHandMighty},{221451, PluginItemTypes.TwoHandMighty},
			{335176, PluginItemTypes.TwoHandPolearm},{367153, PluginItemTypes.TwoHandPolearm},
			{367155, PluginItemTypes.Spear},{335179, PluginItemTypes.Spear},
			{367162, PluginItemTypes.TwoHandStaff},{328169, PluginItemTypes.TwoHandStaff},
			{335139, PluginItemTypes.TwoHandSword},{367141, PluginItemTypes.TwoHandSword},
			{335133, PluginItemTypes.Sword},{367139, PluginItemTypes.Sword},
			{367202, PluginItemTypes.Wand},{335373, PluginItemTypes.Wand},

			{367175, PluginItemTypes.CrusaderShield},{335037, PluginItemTypes.CrusaderShield},
			{335259, PluginItemTypes.Mojo},{367195, PluginItemTypes.Mojo},
			{367204, PluginItemTypes.Source},{327063, PluginItemTypes.Source},
			{367184, PluginItemTypes.Quiver},{367183, PluginItemTypes.Quiver},
			{367164, PluginItemTypes.Shield},{335208, PluginItemTypes.Shield},

			{190635, PluginItemTypes.FollowerEnchantress},{190632, PluginItemTypes.FollowerEnchantress},
			{190639, PluginItemTypes.FollowerScoundrel},{190638, PluginItemTypes.FollowerScoundrel},
			{190628, PluginItemTypes.FollowerTemplar},{190629, PluginItemTypes.FollowerTemplar},

		};

		public static HashSet<int> MiscItemSNOIds = new HashSet<int>();
 		public static void LoadItemIds()
		{
			MiscItemSNOIds.Clear();
			MiscItemSNOIds.Add(210787); //Angelic Wings
			MiscItemSNOIds.UnionWith(InfernalKeySNOIds);
			MiscItemSNOIds.UnionWith(InfernalMachineMaterialSNOIds);
			MiscItemSNOIds.UnionWith(InfernalMachineSNOIds);
			MiscItemSNOIds.UnionWith(StaffOfHerdingMaterialSNOIds);

		}

		/*
		 * Gems
		 * Health Potions
		 * Crafting Materials
		 * Crafting Plans
		 * Uber Keys
		 * Hoardric Cache
		 * Dyes
		 */

		/* Health Potions
		 * Regular - 304319
		 * Kulle-Aid - 344093
		 * Mutalation - 342824
		 * Regeneration - 341343
		 * Diamond - 341342
		 * Leech - 342823
		 * Tower - 341333
		 */

		public static readonly HashSet<int> HealthPotionSNOIds = new HashSet<int>
		{
			304319,344093,342824,341343,341342,342823,341333
		};

		/* Crafting Materials
		 * 
		 * Common Debris - 189860
		 * Reusable Parts - 361984
		 * 
		 * Exquisite Essence - 189861
		 * Arcane Dust - 361985
		 * 
		 * Iridescent Tear - 189862
		 * Veiled Crystal - 361986
		 * 
		 * Fiery Brimstone - 189863
		 * Forgotten Soul - 361988
		 * 
		 * Demonic Essence - 283101
		 * Deaths Breath - 361989
		 * 
		 */

		public static readonly HashSet<int> CraftingMaterialSNOIds = new HashSet<int>
		{
			189860,361984,189861,361985,189862,361986,189863,361988,283101,361989
		};


		#region GEMS

		/*
		 * Chipped Emerald - 56888 BalanceID: -1733388806
		 * Chipped Amethyst 56860 
		 * Chipped Diamond 56874 
		 * 
		 * 
		 * 
		 * 
		 * Flawed Ruby - 56847 BalanceID: 1603007811
		 * Flawed Emerald - 56889 BalanceID: -1733388805
		 * Flawed Topaz - 56917 BalanceID: 2058771887
		 * 
		 * 
		 * 
		 * Ruby - 56848 BalanceID: 1603007812
		 * Diamond - 56876 BalanceID: -1315690658
		 * Amethyst - 56862 BalanceID: -1411866895
		 * Emerald - 56890 BalanceID: -1733388804
		 * 
		 * 
		 * Flawless Amethyst - 56863 BalanceID: -1411866894
		 * Flawless Diamond - 56877 BalanceID: -1315690657
		 * Flawless Ruby - 56849 BalanceID: 1603007813
		 * Flawless Topaz - 56919 BalanceID: 2058771889
		 * Flawless Emerald - 56891 BalanceID: -1733388803
		 * 
		 * Perfect Amethyst - 56864 BalanceID: -1411866893
		 * Perfect Topaz - 56920 BalanceID: 2058771890
		 * Perfect Ruby - 56850 BalanceID: 1603007814
		 * Perfect Emerald - 56892 BalanceID: -1733388802
		 * Perfect Diamond - 56878 BalanceID: -1315690656
		 * 
		 * Radiant Diamond - 56879 BalanceID: -1315690655
		 * Radiant Amethyst - 56865 BalanceID: -1411866892
		 * Radiant Ruby - 56851 BalanceID: 1603007815
		 * Radiant Topaz - 56921 BalanceID: 2058771891
		 * Radiant Emerald - 56893 BalanceID: -1733388801
		 * 
		 * Square Amethyst - 56866 BalanceID: -1411866891
		 * Square Diamond - 56880 BalanceID: -1315690654
		 * Square Emerald - 56894 BalanceID: -1733388800
		 * Square Topaz - 56922 BalanceID: 2058771892
		 * Square Ruby - 56852 BalanceID: 1603007816
		 * 
		 * Flawless Square Topaz - 56923 BalanceID: 2058771893
		 * Flawless Square Ruby - 56853 BalanceID: 1603007817
		 * Flawless Square Emerald - 56895 BalanceID: -1733388799
		 * Flawless Square Amethyst - 56867 BalanceID: -1411866890
		 * Flawless Square Diamond - 56881 BalanceID: -1315690653
		 * 
		 * Perfect Square Topaz - 56924 BalanceID: 2058771894
		 * Perfect Square Ruby - 56854 BalanceID: 1603007818
		 * Perfect Square Diamond - 56882 BalanceID: -1315690652
		 * Perfect Square Emerald - 56896 BalanceID: -1733388798
		 * Perfect Square Amethyst - 56868 BalanceID: -1411866889
		 * 
		 * Radiant Square Diamond - 56883 BalanceID: -1315690628
		 * Radiant Square Topaz - 56925 BalanceID: 2058771918
		 * Radiant Square Ruby - 56855 BalanceID: 1603007842
		 * Radiant Square Emerald - 56897 BalanceID: -1733388774
		 * Radiant Square Amethyst - 56869 BalanceID: -1411866865
		 * 
		 * Star Topaz - 56926 BalanceID: 2058771919
		 * Star Diamond - 56884 BalanceID: -1315690627
		 * Star Ruby - 56856 BalanceID: 1603007843
		 * Star Emerald - 56898 BalanceID: -1733388773
		 * Star Amethyst - 56870 BalanceID: -1411866864
		 * 
		 * 
		 * Flawless Star Amethyst - 56871 BalanceID: -1411866863
		 * 
		 */

		/* Gems
		 * 
		 * Marquise Emerald - 283117 BalanceID: -1733388769
		 * Imperial Emerald - 361492 BalanceID: -1733388768
		 * Flawless Imperial Emerald - 361493 BalanceID: -1733388767
		 * Royal Emerald - 361494 BalanceID: -1733388766
		 * Flawless Royal Emerald - 361495 BalanceID: -1733388765
		 * 
		 * Marquise Diamond - 361559 BalanceID: -1315690623
		 * Imperial Diamond - 361560 BalanceID: -1315690622
		 * Flawless Imperial Diamond - 361561 BalanceID: -1315690621
		 * Royal Diamond - 361562 BalanceID: -1315690620
		 * Flawless Royal Diamond - 361563 BalanceID: -1315690619
		 * 
		 * Marquise Topaz - 283119 BalanceID: 2058771923
		 * Imperial Topaz - 361572 BalanceID: 2058771924
		 * Flawless Imperial Topaz - 361573 BalanceID: 2058771925
		 * Royal Topaz - 361574 BalanceID: 2058771926
		 * Flawless Royal Topaz - 361575 BalanceID: 2058771927
		 * 
		 * Marquise Ruby - 283118 BalanceID: 1603007847
		 * Imperial Ruby - 361568 BalanceID: 1603007848
		 * Flawless Imperial Ruby - 361569 BalanceID: 1603007849
		 * Royal Ruby - 361570 BalanceID: 1603007850
		 * Flawless Royal Ruby - 361571 BalanceID: 1603007851
		 * 
		 * Marquise Amethyst - 283116 BalanceID: -1411866860
		 * Imperial Amethyst - 361564 BalanceID: -1411866859
		 * Flawless Imperial Amethyst - 361565 BalanceID: -1411866858
		 * Royal Amethyst - 361566 BalanceID: -1411866857
		 * Flawless Royal Amethyst - 361567 BalanceID: -1411866856
		 * 
		 */

		public static readonly HashSet<int> GemsSNOIds = new HashSet<int>
		{
			56863,56877,56849,56919,56891, //Flawless
			56864,56920,56850,56892,56878, //Perfect
			56879,56865,56851,56921,56893, //Radiant
			56866,56880,56894,56922,56852, //Square
			56923,56853,56895,56867,56881, //Flawless Square
			283117,361559,283119,283118,283116, //Marquise
			361568,361572,361564,361560,361492, //Imperial
			361493,361569,361561,361565,361573, //Flawless Imperial
			361566,361562,361570,361494,361574, //Royal
			361575,361571,361567,361563,361495 //Flawless Royal
		};
		public static readonly HashSet<int> GEMS_EmeraldSNOIds = new HashSet<int>
		{
			56891,56892,56893,56894,56895,283117,361492,361493,361494,361495
		};
		public static readonly HashSet<int> GEMS_DiamondSNOIds = new HashSet<int>
		{
			56877,56878,56879,56880,56881,361559,361560,361561,361562,361563
		};
		public static readonly HashSet<int> GEMS_TopazSNOIds = new HashSet<int>
		{
			56919,56920,56921,56922,56923,283119,361572,361573,361574,361575
		};
		public static readonly HashSet<int> GEMS_RubySNOIds = new HashSet<int>
		{
			56849,56850,56851,56852,56853,283118,361568,361569,361570,361571
		};
		public static readonly HashSet<int> GEMS_AmethystSNOIds = new HashSet<int>
		{
			56863,56864,56865,56866,56867,283116,361564,361565,361566,361567
		};
		public static readonly HashSet<int> GEMS_FlawlessSNOIds = new HashSet<int>
		{
			56863,56877,56849,56919,56891
		};
		public static readonly HashSet<int> GEMS_PerfectSNOIds = new HashSet<int>
		{
			56864,56920,56850,56892,56878
		};
		public static readonly HashSet<int> GEMS_RadiantSNOIds = new HashSet<int>
		{
			56879,56865,56851,56921,56893
		};
		public static readonly HashSet<int> GEMS_SquareSNOIds = new HashSet<int>
		{
			56866,56880,56894,56922,56852
		};
		public static readonly HashSet<int> GEMS_FlawlessSquareSNOIds = new HashSet<int>
		{
			56923,56853,56895,56867,56881
		};
		public static readonly HashSet<int> GEMS_MarquiseSNOIds = new HashSet<int>
		{
			283117,361559,283119,283118,283116
		};
		public static readonly HashSet<int> GEMS_ImperialSNOIds = new HashSet<int>
		{
			361568,361572,361564,361560,361492
		};
		public static readonly HashSet<int> GEMS_FlawlessImperialSNOIds = new HashSet<int>
		{
			361493,361569,361561,361565,361573
		};
		public static readonly HashSet<int> GEMS_RoyalSNOIds = new HashSet<int>
		{
			361566,361562,361570,361494,361574
		};
		public static readonly HashSet<int> GEMS_FlawlessRoyalSNOIds = new HashSet<int>
		{
			361575,361571,361567,361563,361495
		};

		#endregion


		/* Infernal Keys
		 * 
		 * Key of Gluttony - 364695 BalanceID: -1207737543
		 * Key of Evil - 364697 BalanceID: 113551449
		 * Key of Bones - 364694 BalanceID: 483403932
		 * Key of War - 364696 BalanceID: 2050794135
		 * 
		 * Key of Destruction - 255880 BalanceID: -1088058180
		 * Key of Hate - 255881 BalanceID: 1064300938
		 * Key of Terror - 255882 BalanceID: -143133978
		 */

		public static readonly HashSet<int> InfernalKeySNOIds = new HashSet<int>
		{
			364695,364697,364694,364696,
			255880,255881,255882
		};

		/* Hellfire Ring Materials
		 * 
		 * Leroric's Regret - 364722
		 * Vial of Putridness - 364723
		 * Idol of Terror - 364724
		 * Heart of Evil - 364725
		 * 
		 * 
		 * Vengeful Eye - 257738
		 * Devil's Fang - 257736 BalanceID: -645052297
		 * Writhing Spine - 257739 BalanceID: -1824155952
		 */

		private static readonly HashSet<int> InfernalMachineMaterialSNOIds = new HashSet<int>
		{
			364722,364723,364724,364725,
			257736,257739,257738
		};

		/* Infernal Machines
		 * 
		 * Level 60 - 257737
		 * Bones - 366946
		 * Evil - 366949
		 * Gluttony - 366947
		 * War - 366948
		 */

		private static readonly HashSet<int> InfernalMachineSNOIds = new HashSet<int>
		{
			257737,366946,366949,366947,366948
		};


		/* Dyes
		 * 
		 * Summer Dye - 148299 BalanceID: -234196797
		 * All-Soap's Miraculous Dye Remover - 148311 BalanceID: -575184703
		 * Tanner's Dye - 54505 BalanceID: -234196830
		 * Mariner's Dye - 148304 BalanceID: -234196793
		 * Lovely Dye - 148298 BalanceID: -234196798
		 * Forester's Dye - 148303 BalanceID: -234196794
		 * Autumn Dye - 148296 BalanceID: -234196822
		 * 
		 * Abyssal - 148309
		 * Bottled Cloud - 212182
		 * Bottled Smoke - 212183
		 * Golden - 148305
		 * Infernal - 148307
		 * Purity - 148308
		 * Vanishing - 148310
		 * Winter - 148288
		 * 
		 * TODO
		 * Aquatic
		 * Spring
		 * Cardinal
		 * Desert
		 * Rangers
		 * Royal
		 * Elegant
		 * 
		 */

		public static readonly HashSet<int> DyesSNOIds = new HashSet<int>
		{
			148299,148311,54505,148304,148298,148303,148296,
			148309,212182,212183,148305,148307,148308,148310,
			148288
		};

		/* Staff of Herding Crafting Materials
		 * 
		 * Gibbering Gemstone - 214604 BalanceID: -799974399
		 * Liquid Rainbow - 214603
		 * Wirt's Bell - 180697 BalanceID: -799868536
		 * Leoric's Shinbone - 214605 BalanceID: -629520052
		 * 
		 */

		private static readonly HashSet<int> StaffOfHerdingMaterialSNOIds = new HashSet<int>
		{
			214604,214603,180697,214605
		};

		public static readonly HashSet<int> HoradricCacheSNOIds = new HashSet<int>
		{
			364329,360166
		};

		//Rift Keystone Fragment - 323722 BalanceID: -1698136228
		//Horadric Cache - 364329 BalanceID: 198273233
		//Horadric Cache - 360166 BalanceID: -1575654859
		//Angelic Wings - 210787
		
		public static Dictionary<int, LegendaryItemTypes> LegendaryItems = new Dictionary<int, LegendaryItemTypes>
		{
			//Single Items
			{272084, LegendaryItemTypes.SerpentSparker},
			{298088, LegendaryItemTypes.TallManFinger},
			{298094, LegendaryItemTypes.RoyalGrandeur},
			{332342, LegendaryItemTypes.IllusionaryBoots},
			{299411, LegendaryItemTypes.PiroMarella},
			{298050, LegendaryItemTypes.CountessJuliasCameo},
			{197824, LegendaryItemTypes.MarasKaleidoscope},
			{271738, LegendaryItemTypes.StarmetalKukri},
			{222464, LegendaryItemTypes.IceClimbers},
			{298118, LegendaryItemTypes.ReapersWraps},

			//BLACKTHORNE'S
			{222477, LegendaryItemTypes.Blackthornes}, //Jousting Mail
			{224191, LegendaryItemTypes.Blackthornes}, //Notched Belt
			{222456, LegendaryItemTypes.Blackthornes}, //Surcoat
			{222463, LegendaryItemTypes.Blackthornes}, //Spurs
			{224189, LegendaryItemTypes.Blackthornes}, //Duncraig Cross

			//Zunimassa's
			{205615, LegendaryItemTypes.Zunimassas}, //Marrow
			{216525, LegendaryItemTypes.Zunimassas}, //String of Skulls
			{221202, LegendaryItemTypes.Zunimassas}, //Vision
			{205627, LegendaryItemTypes.Zunimassas}, //Trail
			{212579, LegendaryItemTypes.Zunimassas}, //Pox
			

			//Akkhan's
			{358799, LegendaryItemTypes.Akkhan},//Helm
			{358795, LegendaryItemTypes.Akkhan},//Sabatons
			{358801, LegendaryItemTypes.Akkhan},//Pauldrons
			{358800, LegendaryItemTypes.Akkhan},//Cuisses
			{358796, LegendaryItemTypes.Akkhan},//Breastplate
			{358798, LegendaryItemTypes.Akkhan},//Gauntlets

			//Raiment of a ThousandStorms
			{338035, LegendaryItemTypes.RaimentofaThousandStorms},//Pants
			{338031, LegendaryItemTypes.RaimentofaThousandStorms},//Boots
			{338036, LegendaryItemTypes.RaimentofaThousandStorms},//Shoulders
			{338034, LegendaryItemTypes.RaimentofaThousandStorms},//Helm
			{338032, LegendaryItemTypes.RaimentofaThousandStorms},//Chest
			{338033, LegendaryItemTypes.RaimentofaThousandStorms},//Gloves

			//Inna's Mantra
			{205646, LegendaryItemTypes.Inna},//Pants
			{222487, LegendaryItemTypes.Inna},//Belt
			{222307, LegendaryItemTypes.Inna},//Helm
			{205614, LegendaryItemTypes.Inna},//Chest
			{212208, LegendaryItemTypes.Inna},//Daibo


			//Raiment of the Jade Harvester
			{338041, LegendaryItemTypes.JadeHarvester},//Pants
			{338037, LegendaryItemTypes.JadeHarvester},//Boots
			{338042, LegendaryItemTypes.JadeHarvester},//Shoulders
			{338040, LegendaryItemTypes.JadeHarvester},//Helm
			{338038, LegendaryItemTypes.JadeHarvester},//Chest
			{338039, LegendaryItemTypes.JadeHarvester},//Gloves

			//Vyr's Amazing Arcana
			{332360, LegendaryItemTypes.Vyrs},//Pants
			{346210, LegendaryItemTypes.Vyrs},//Gloves
			{332363, LegendaryItemTypes.Vyrs},//Boots
			{332357, LegendaryItemTypes.Vyrs},//Chest
			
			//Might of the Earth
			{340521, LegendaryItemTypes.MightOfTheEarth},//Pants
			{340523, LegendaryItemTypes.MightOfTheEarth},//Gloves
			{340528, LegendaryItemTypes.MightOfTheEarth},//Helm
			{340526, LegendaryItemTypes.MightOfTheEarth},//Shoulders

			//Embodiment of the Marauder
			{336993, LegendaryItemTypes.Marauder},//Pants
			{336995, LegendaryItemTypes.Marauder},//Boots
			{336996, LegendaryItemTypes.Marauder},//Shoulders
			{336994, LegendaryItemTypes.Marauder},//Helm
			//{000000, LegendaryItemTypes.Marauder},//Chest
			{336992, LegendaryItemTypes.Marauder},//Gloves
		};

	}
}
