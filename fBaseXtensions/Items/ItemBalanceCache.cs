using System.Collections.Generic;
using fBaseXtensions.Items.Enums;

namespace fBaseXtensions.Items
{
	public class ItemBalanceCache
	{
		public const int Potion_Regular_BalanceID = -2142362846;


		public static readonly Dictionary<int, PluginItemTypes> ReaperOfSoulItemTypes = new Dictionary<int, PluginItemTypes>
		{
			{1682228656, PluginItemTypes.Amulet},
			{1146967350, PluginItemTypes.Ring},{1146967349, PluginItemTypes.Ring},

			{-733829184, PluginItemTypes.Belt},{-733829185, PluginItemTypes.Belt},
			{2140882335, PluginItemTypes.Boots},{2140882336, PluginItemTypes.Boots},
			{-875942693, PluginItemTypes.Bracers},{-875942694, PluginItemTypes.Bracers},
			{1612259888, PluginItemTypes.Chest},{1612259889, PluginItemTypes.Chest},
			{40857601, PluginItemTypes.Cloak},{40857600, PluginItemTypes.Cloak},
			{-1533912119, PluginItemTypes.Gloves},{-1533912120, PluginItemTypes.Gloves},
			{1565456767, PluginItemTypes.Helm},{1565456766, PluginItemTypes.Helm},
			{-1512729954, PluginItemTypes.Pants},{-1512729953, PluginItemTypes.Pants},
			{2112157589, PluginItemTypes.MightyBelt},{2112157588, PluginItemTypes.MightyBelt},
			{365492434, PluginItemTypes.Shoulders},{365492433, PluginItemTypes.Shoulders},
			{-242893286, PluginItemTypes.SpiritStone},{-242893285, PluginItemTypes.SpiritStone},
			{620036249, PluginItemTypes.VoodooMask},{620036248, PluginItemTypes.VoodooMask},
			{1755623813, PluginItemTypes.WizardHat},{1755623812, PluginItemTypes.WizardHat},


			{1661415658, PluginItemTypes.Axe},{1661415657, PluginItemTypes.Axe},
			{1700551050, PluginItemTypes.TwoHandAxe},{1700551051, PluginItemTypes.TwoHandAxe},
			{-2091500804, PluginItemTypes.TwoHandBow},{-2091500805, PluginItemTypes.TwoHandBow},
			{-635266316, PluginItemTypes.CeremonialKnife},{-635266315, PluginItemTypes.CeremonialKnife},
			{181035077, PluginItemTypes.TwoHandCrossbow},{181035078, PluginItemTypes.TwoHandCrossbow},
			{-1303412034, PluginItemTypes.Dagger},{-1303412035, PluginItemTypes.Dagger},
			{1771752118, PluginItemTypes.TwoHandDaibo},{1771752119, PluginItemTypes.TwoHandDaibo},
			{-873319305, PluginItemTypes.Flail},{-873319306, PluginItemTypes.Flail},
			{-912454698, PluginItemTypes.TwoHandFlail},{-912454699, PluginItemTypes.TwoHandFlail},
			{1236608236, PluginItemTypes.FistWeapon},{1236608235, PluginItemTypes.FistWeapon},
			{-363388402, PluginItemTypes.HandCrossbow},{-363388401, PluginItemTypes.HandCrossbow},
			{-1616887518, PluginItemTypes.TwoHandMace},{-1616887517, PluginItemTypes.TwoHandMace},
			{-1656022910, PluginItemTypes.Mace},{-1656022911, PluginItemTypes.Mace},
			{290069769, PluginItemTypes.MightyWeapon},{290069768, PluginItemTypes.MightyWeapon},
			{329205162, PluginItemTypes.TwoHandMighty},{329205161, PluginItemTypes.TwoHandMighty},
			{-1337760253, PluginItemTypes.TwoHandPolearm},{-1337760252, PluginItemTypes.TwoHandPolearm},
			{-101309489, PluginItemTypes.Spear},{-101309490, PluginItemTypes.Spear},
			{-2115688088, PluginItemTypes.TwoHandStaff},{-2115688089, PluginItemTypes.TwoHandStaff},
			{-231800261, PluginItemTypes.TwoHandSword},{-231800260, PluginItemTypes.TwoHandSword},
			{-270935654, PluginItemTypes.Sword},{-270935653, PluginItemTypes.Sword},
			{88668318, PluginItemTypes.Wand},{88668317, PluginItemTypes.Wand},

			{-118252694, PluginItemTypes.CrusaderShield},{-118252695, PluginItemTypes.CrusaderShield},
			{-136814293, PluginItemTypes.Mojo},{-136814292, PluginItemTypes.Mojo},
			{1905181658, PluginItemTypes.Source},{1905181657, PluginItemTypes.Source},
			{1539238484, PluginItemTypes.Quiver},{1539238483, PluginItemTypes.Quiver},
			{1815809043, PluginItemTypes.Shield},{1815809042, PluginItemTypes.Shield},

			{761439029, PluginItemTypes.FollowerEnchantress},{761439028, PluginItemTypes.FollowerEnchantress},
			{-229899866, PluginItemTypes.FollowerScoundrel},{-229899867, PluginItemTypes.FollowerScoundrel},
			{1147341803, PluginItemTypes.FollowerTemplar},{1147341804, PluginItemTypes.FollowerTemplar},

		};

		public static readonly Dictionary<int, LegendaryItemTypes> LegendaryItems = new Dictionary<int, LegendaryItemTypes>
		{
			//Single Items
			{1898798298, LegendaryItemTypes.SerpentSparker},
			{-1149809185, LegendaryItemTypes.TheTallMansFinger},
			{-1149593563, LegendaryItemTypes.RingofRoyalGrandeur},
			{1979309080, LegendaryItemTypes.IllusionaryBoots},
			{820499474, LegendaryItemTypes.PiroMarella},
			{1566368217, LegendaryItemTypes.CountessJuliasCameo},
			{1528490619, LegendaryItemTypes.MarasKaleidoscope},
			{1823578424, LegendaryItemTypes.StarmetalKukri},

			//BLACKTHORNE'S
			{-773231465, LegendaryItemTypes.BlackthornesBattlegear}, //Jousting Mail
			{1772078106, LegendaryItemTypes.BlackthornesBattlegear}, //Notched Belt
			{-115330289, LegendaryItemTypes.BlackthornesBattlegear}, //Surcoat
			{1941575230, LegendaryItemTypes.BlackthornesBattlegear}, //Spurs
			{1528526556, LegendaryItemTypes.BlackthornesBattlegear}, //Duncraig Cross

			//Zunimassa's
			{1316917835, LegendaryItemTypes.ZunimassasHaunt}, //Marrow
			{-960430780, LegendaryItemTypes.ZunimassasHaunt}, //String of Skulls
			{-840125482, LegendaryItemTypes.ZunimassasHaunt}, //Vision
			{1941359608, LegendaryItemTypes.ZunimassasHaunt}, //Trail
			{-1187722720, LegendaryItemTypes.ZunimassasHaunt}, //Pox
			

			//Akkhan's
			{-1992164625, LegendaryItemTypes.ArmorofAkkhan},//Helm
			{-800755056, LegendaryItemTypes.ArmorofAkkhan},//Sabatons
			{-1980761457, LegendaryItemTypes.ArmorofAkkhan},//Pauldrons
			{827015887, LegendaryItemTypes.ArmorofAkkhan},//Cuisses
			{259933632, LegendaryItemTypes.ArmorofAkkhan},//Breastplate
			{2059399737, LegendaryItemTypes.ArmorofAkkhan},//Gauntlets

			//Raiment of a ThousandStorms
			{826117462, LegendaryItemTypes.RaimentofaThousandStorms},//Pants
			{-801653481, LegendaryItemTypes.RaimentofaThousandStorms},//Boots
			{-1981659882, LegendaryItemTypes.RaimentofaThousandStorms},//Shoulders
			{-1993063050, LegendaryItemTypes.RaimentofaThousandStorms},//Helm
			{259035207, LegendaryItemTypes.RaimentofaThousandStorms},//Chest
			{2058501312, LegendaryItemTypes.RaimentofaThousandStorms},//Gloves
		
			
			//Raiment of the Jade Harvester
			{826153399, LegendaryItemTypes.RaimentoftheJadeHarvester},//Pants
			{-801617544, LegendaryItemTypes.RaimentoftheJadeHarvester},//Boots
			//{000000, LegendaryItemTypes.JadeHarvester},//Shoulders
			{-1993027113, LegendaryItemTypes.RaimentoftheJadeHarvester},//Helm
			{259071144, LegendaryItemTypes.RaimentoftheJadeHarvester},//Chest
			{2058537249, LegendaryItemTypes.RaimentoftheJadeHarvester},//Gloves

			//Inna's Mantra
			{-774237701, LegendaryItemTypes.InnasMantra},//Pants
			{1770964059, LegendaryItemTypes.InnasMantra},//Belt
			//{000000, LegendaryItemTypes.Inna},//Helm
			//{000000, LegendaryItemTypes.Inna},//Chest
			//{000000, LegendaryItemTypes.Inna},//Daibo

			//Might of the Earth
			//{000000, LegendaryItemTypes.MightOfTheEarth},//Pants
			//{000000, LegendaryItemTypes.MightOfTheEarth},//Gloves
			//{000000, LegendaryItemTypes.MightOfTheEarth},//Helm
			{-1980581772, LegendaryItemTypes.MightOfTheEarth},//Shoulders


			//Embodiment of the Marauder
			{826081525, LegendaryItemTypes.EmbodimentoftheMarauder},//Pants
			{-801689418, LegendaryItemTypes.EmbodimentoftheMarauder},//Boots
			{-1981695819, LegendaryItemTypes.EmbodimentoftheMarauder},//Shoulders
			{-1993098987, LegendaryItemTypes.EmbodimentoftheMarauder},//Helm
			{258999270, LegendaryItemTypes.EmbodimentoftheMarauder},//Chest
			{2058465375, LegendaryItemTypes.EmbodimentoftheMarauder},//Gloves
		};

	}
}
