using System;
using System.Collections.Generic;
using System.Linq;
using FunkyBot.Cache.Objects;
using FunkyBot.Misc;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player
{
	public enum EquippedItemType
	{
		None,
		Blackthornes,
		Zunimassas,
		SerpentSparker,
		TallManFinger,
		RoyalGrandeur,
		IllusionaryBoots,
		PiroMarella,
		CountessJuliasCameo,
		MarasKaleidoscope,
	}

	internal class Equipment
	{


		public bool GlobesRestoreResource { get; set; }
		public bool ImmuneToDescratorMoltenPlaguedAvoidances { get; set; }
		public bool ImmuneToArcane { get; set; }
		public bool ImmuneToPoison { get; set; }
		public bool RingOfGrandeur { get; set; }
		public bool NoMonsterCollision { get; set; }

		public List<CacheACDItem> EquippedItems { get; set; }

		public Equipment()
		{
			EquippedItems = new List<CacheACDItem>();
			GlobesRestoreResource = false;
			ImmuneToDescratorMoltenPlaguedAvoidances = false;
			RingOfGrandeur = false;
			NoMonsterCollision = false;
			ImmuneToArcane = false;
			ImmuneToPoison = false;
		}

		public void RefreshEquippedItemsList()
		{
			EquippedItems.Clear();
			EquippedItems = ReturnCurrentEquippedItems();

			//Countess Julias Cameo
			ImmuneToArcane = EquippedItems.Any(i => i.EquippedType == EquippedItemType.CountessJuliasCameo);
			ImmuneToPoison = EquippedItems.Any(i => i.EquippedType == EquippedItemType.MarasKaleidoscope);

			//Repear Wraps
			if (EquippedItems.Any(i => i.ThisRealName.Contains("Reaper's Wraps")))
			{
				GlobesRestoreResource = true;
				Logger.DBLog.DebugFormat("Reaper's Wraps is equipped.. globes will be targeted on low resource!");
			}
			else
				GlobesRestoreResource = false;


			//Illusionary Boots
			if (EquippedItems.Any(i => i.EquippedType==EquippedItemType.IllusionaryBoots))
			{
				NoMonsterCollision = true;
				Logger.DBLog.DebugFormat("Illusionary Boots Found -- No monster collision!");
			}
			else
				NoMonsterCollision = false;


			//Ring of Royal Grandeur
			RingOfGrandeur = EquippedItems.Any(i => i.EquippedType==EquippedItemType.RoyalGrandeur);


			//Blackthorns Set
			int BlackThornSetCount = EquippedItems.Count(i => i.EquippedType==EquippedItemType.Blackthornes);
			if (BlackThornSetCount == 4 || BlackThornSetCount == 3 && RingOfGrandeur)
			{
				ImmuneToDescratorMoltenPlaguedAvoidances = true;
				Logger.DBLog.DebugFormat("Blackthorne's Avoidance Immune is equipped");
			}
			else
				ImmuneToDescratorMoltenPlaguedAvoidances = false;
			
		}

		public List<CacheACDItem> ReturnCurrentEquippedItems()
		{
			var returnItems = new List<CacheACDItem>();
			try
			{
				using (ZetaDia.Memory.AcquireFrame())
				{
					foreach (ACDItem item in ZetaDia.Me.Inventory.Equipped)
					{
						var thiscacheditem = new CacheACDItem(item);
						returnItems.Add(thiscacheditem);
					}
				}

			}
			catch (Exception)
			{

			}
			return returnItems;
		}


		#region Static Item Balance IDs
		public static readonly Dictionary<int, EquippedItemType> ImportantItems = new Dictionary<int, EquippedItemType>
		{

			//Single Items
			{1898798298, EquippedItemType.SerpentSparker},
			{-1149809185, EquippedItemType.TallManFinger},
			{-1149593563, EquippedItemType.RoyalGrandeur},
			{1979309080, EquippedItemType.IllusionaryBoots},
			{820499474, EquippedItemType.PiroMarella},
			{1566368217, EquippedItemType.CountessJuliasCameo},
			{1528490619, EquippedItemType.MarasKaleidoscope},

			//BLACKTHORNE'S
			{-773231465, EquippedItemType.Blackthornes}, //Jousting Mail
			{1772078106, EquippedItemType.Blackthornes}, //Notched Belt
			{-115330289, EquippedItemType.Blackthornes}, //Surcoat
			{1941575230, EquippedItemType.Blackthornes}, //Spurs
			{1528526556, EquippedItemType.Blackthornes}, //Duncraig Cross

			//Zunimassa's
			{1316917835, EquippedItemType.Zunimassas}, //Marrow
			{-960430780, EquippedItemType.Zunimassas}, //String of Skulls
			{-840125482, EquippedItemType.Zunimassas}, //Vision
			{1941359608, EquippedItemType.Zunimassas}, //Trail
			//TODO:: Add POX




		};


		#endregion

		/*
	 * Item - Name: Illusory Boots BalanceID: 1979309080

		Item - Name: Piro Marella BalanceID: 820499474

		Item - Name: Countess Julia's Cameo (InternalName: x1_Amulet_norm_unique_19-211) BalanceID: 1566368217

		Item - Name: Mara's Kaleidoscope BalanceID: 1528490619



	 */
	}
}
