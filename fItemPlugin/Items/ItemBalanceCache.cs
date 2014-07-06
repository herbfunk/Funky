using System.Collections.Generic;

namespace fItemPlugin.Items
{
	public class ItemBalanceCache
	{
		public const int Potion_Regular_BalanceID = -2142362846;


		public static readonly Dictionary<int, LegendaryItemTypes> LegendaryItems = new Dictionary<int, LegendaryItemTypes>
		{
			//Single Items
			{1898798298, LegendaryItemTypes.SerpentSparker},
			{-1149809185, LegendaryItemTypes.TallManFinger},
			{-1149593563, LegendaryItemTypes.RoyalGrandeur},
			{1979309080, LegendaryItemTypes.IllusionaryBoots},
			{820499474, LegendaryItemTypes.PiroMarella},
			{1566368217, LegendaryItemTypes.CountessJuliasCameo},
			{1528490619, LegendaryItemTypes.MarasKaleidoscope},
			{1823578424, LegendaryItemTypes.StarmetalKukri},

			//BLACKTHORNE'S
			{-773231465, LegendaryItemTypes.Blackthornes}, //Jousting Mail
			{1772078106, LegendaryItemTypes.Blackthornes}, //Notched Belt
			{-115330289, LegendaryItemTypes.Blackthornes}, //Surcoat
			{1941575230, LegendaryItemTypes.Blackthornes}, //Spurs
			{1528526556, LegendaryItemTypes.Blackthornes}, //Duncraig Cross

			//Zunimassa's
			{1316917835, LegendaryItemTypes.Zunimassas}, //Marrow
			{-960430780, LegendaryItemTypes.Zunimassas}, //String of Skulls
			{-840125482, LegendaryItemTypes.Zunimassas}, //Vision
			{1941359608, LegendaryItemTypes.Zunimassas}, //Trail
			{-1187722720, LegendaryItemTypes.Zunimassas}, //Pox
			

			//Akkhan's
			{-1992164625, LegendaryItemTypes.Akkhan},//Helm
			{-800755056, LegendaryItemTypes.Akkhan},//Sabatons
			{-1980761457, LegendaryItemTypes.Akkhan},//Pauldrons
			{827015887, LegendaryItemTypes.Akkhan},//Cuisses
			{259933632, LegendaryItemTypes.Akkhan},//Breastplate
			{2059399737, LegendaryItemTypes.Akkhan},//Gauntlets

			//Raiment of a ThousandStorms
			{826117462, LegendaryItemTypes.RaimentofaThousandStorms},//Pants
			{-801653481, LegendaryItemTypes.RaimentofaThousandStorms},//Boots
			{-1981659882, LegendaryItemTypes.RaimentofaThousandStorms},//Shoulders
			{-1993063050, LegendaryItemTypes.RaimentofaThousandStorms},//Helm
			{259035207, LegendaryItemTypes.RaimentofaThousandStorms},//Chest
			{2058501312, LegendaryItemTypes.RaimentofaThousandStorms},//Gloves
		

			//Raiment of the Jade Harvester
			//{000000, LegendaryItemTypes.JadeHarvester},//Pants
			{-801617544, LegendaryItemTypes.JadeHarvester},//Boots
			//{000000, LegendaryItemTypes.JadeHarvester},//Shoulders
			{-1993027113, LegendaryItemTypes.JadeHarvester},//Helm
			{259071144, LegendaryItemTypes.JadeHarvester},//Chest
			{2058537249, LegendaryItemTypes.JadeHarvester},//Gloves

			//Inna's Mantra
			{-774237701, LegendaryItemTypes.Inna},//Pants
			{1770964059, LegendaryItemTypes.Inna},//Belt
			//{000000, LegendaryItemTypes.Inna},//Helm
			//{000000, LegendaryItemTypes.Inna},//Chest
			//{000000, LegendaryItemTypes.Inna},//Daibo

			//Might of the Earth
			//{000000, LegendaryItemTypes.MightOfTheEarth},//Pants
			//{000000, LegendaryItemTypes.MightOfTheEarth},//Gloves
			//{000000, LegendaryItemTypes.MightOfTheEarth},//Helm
			{-1980581772, LegendaryItemTypes.MightOfTheEarth},//Shoulders


			//Embodiment of the Marauder
			//{000000, LegendaryItemTypes.Marauder},//Pants
			{-801689418, LegendaryItemTypes.Marauder},//Boots
			//{000000, LegendaryItemTypes.Marauder},//Shoulders
			//{000000, LegendaryItemTypes.Marauder},//Helm
			//{000000, LegendaryItemTypes.Marauder},//Chest
			//{000000, LegendaryItemTypes.Marauder},//Gloves
		};
	}
}
