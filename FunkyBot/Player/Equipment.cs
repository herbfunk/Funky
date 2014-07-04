using System;
using System.Collections.Generic;
using System.Linq;
using FunkyBot.Cache.Objects;
using FunkyBot.Misc;
using FunkyBot.Player.Class;
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
		Akkhan,
		StarmetalKukri,
		RaimentofaThousandStorms,
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

		private DateTime _lastCheckedItemIDs;
		private const int _itemIDRecheckSeconds = 10;
		private List<int> _equippedItemIDs;
		private bool ShouldRecheckItemIDs
		{
			get
			{
				return DateTime.Now.Subtract(_lastCheckedItemIDs).TotalSeconds >= _itemIDRecheckSeconds;
			}
		}
		public void CheckEquippment()
		{
			if (!PlayerClass.ShouldRecreatePlayerClass && ShouldRecheckItemIDs)
			{
				_lastCheckedItemIDs = DateTime.Now;
				List<int> compareList = ReturnEquippedItemIDs();

				//Check for any new IDs..
				if (compareList.Except(_equippedItemIDs).Any())
				{
					_equippedItemIDs = new List<int>(compareList);
					RefreshEquippedItemsList();
					PlayerClass.ShouldRecreatePlayerClass = true;
				}
			}
		}
		private List<int> ReturnEquippedItemIDs()
		{
			var returnItems = new List<int>();

			try
			{
				using (ZetaDia.Memory.AcquireFrame())
				{
					foreach (ACDItem item in ZetaDia.Me.Inventory.Equipped)
					{
						try
						{
							var thisItemID = item.GameBalanceId;
							returnItems.Add(thisItemID);
						}
						catch{}
					}
				}

			}
			catch
			{

			}

			return returnItems;
		}

		public Equipment()
		{
			EquippedItems = new List<CacheACDItem>();
			GlobesRestoreResource = false;
			ImmuneToDescratorMoltenPlaguedAvoidances = false;
			RingOfGrandeur = false;
			NoMonsterCollision = false;
			ImmuneToArcane = false;
			ImmuneToPoison = false;
			_equippedItemIDs = new List<int>();
			_lastCheckedItemIDs = DateTime.MinValue;
		}

		public void RefreshEquippedItemsList()
		{
			if (Bot.GameIsInvalid())
				return;

			Logger.DBLog.DebugFormat("Refreshing Item List..");

			EquippedItems.Clear();
			EquippedItems = ReturnCurrentEquippedItems();

			var ids = EquippedItems.Select(i => i.ThisBalanceID).ToList();
			_equippedItemIDs = new List<int>(ids);

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

		private List<CacheACDItem> ReturnCurrentEquippedItems()
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
			{1823578424, EquippedItemType.StarmetalKukri},

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
			{-1187722720, EquippedItemType.Zunimassas}, //Pox
			

			//Akkhan's
			{-1992164625, EquippedItemType.Akkhan},//Helm
			{-800755056, EquippedItemType.Akkhan},//Sabatons
			{-1980761457, EquippedItemType.Akkhan},//Pauldrons
			{827015887, EquippedItemType.Akkhan},//Cuisses
			{259933632, EquippedItemType.Akkhan},//Breastplate
			{2059399737, EquippedItemType.Akkhan},//Gauntlets

			//Raiment of a ThousandStorms
			{826117462, EquippedItemType.RaimentofaThousandStorms},//Pants
			{-801653481, EquippedItemType.RaimentofaThousandStorms},//Boots
			{-1981659882, EquippedItemType.RaimentofaThousandStorms},//Shoulders
			{-1993063050, EquippedItemType.RaimentofaThousandStorms},//Helm
			{259035207, EquippedItemType.RaimentofaThousandStorms},//Chest
			//TODO:: Add Gloves
		};


		#endregion

		/*

		 Item - Name: Zunimassa's Pox (InternalName: Ring_norm_unique_012-214) BalanceID: -1187722720
		Item - Name: Scales of the Dancing Serpent (InternalName: x1_pants_norm_set_08-148) BalanceID: 826117462
		 * Item - Name: Eight-Demon Boots (InternalName: x1_Boots_norm_set_08-151) BalanceID: -801653481
		 * Item - Name: Mantle of the Upside-Down Sinners (InternalName: x1_shoulderPads_norm_set_08-182) BalanceID: -1981659882
		 * Item - Name: Mask of the Searing Sky (InternalName: x1_Helm_norm_set_08-194) BalanceID: -1993063050
		 * Item - Name: Heart of the Crashing Wave (InternalName: x1_chestArmor_norm_set_08-187) BalanceID: 259035207
		 * 
	 */
	}
}
