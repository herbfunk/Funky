using System;
using fBaseXtensions.Cache;
using fBaseXtensions.Cache.External.Objects;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Items
{
	

	public class CacheACDItem
	{
		public string ThisInternalName { get; set; }
		public string ThisRealName { get; set; }
		public int ThisLevel { get; set; }
		public ItemQuality ThisQuality { get; set; }
		public int ThisGoldAmount { get; set; }
		public int LegendaryGemRank { get; set; }
		public LegendaryGemTypes LegendaryGemType { get; set; }
		public int ThisBalanceID { get; set; }
		public int ThisDynamicID { get; set; }
		public bool ThisOneHanded { get; set; }
		public bool TwoHanded { get; set; }
		public DyeType ThisDyeType { get; set; }
		public ItemType ThisDBItemType { get; set; }
		public FollowerType ThisFollowerType { get; set; }
		public bool IsUnidentified { get; set; }
		
		public bool IsPotion { get; set; }
		public PotionTypes PotionType=PotionTypes.None;
		public LegendaryItemTypes LegendaryItemType=LegendaryItemTypes.None;


		public int ThisItemStackQuantity { get; set; }


		public ACDItem ACDItem { get; set; }
		public int ACDGUID { get; set; }
		public int SNO { get; set; }

		//inventory positions
		public int invRow { get; set; }
		public int invCol { get; set; }

		public string ItemStatString = "";
		public ItemProperties ItemStatProperties { get; set; }
		public bool IsStackableItem { get; set; }
		public bool IsTwoSlot { get; set; }
		public bool IsVendorBought { get; set; }

		//Plugin Item Properties
		public PluginBaseItemTypes BaseItemType { get; set; }
		public PluginItemTypes ItemType { get; set; }


		public CacheACDItem(ACDItem item)
		{
			
			SNO=item.ActorSNO;
			ThisBalanceID = item.GameBalanceId;
			ThisInternalName = item.InternalName;
			ThisLevel = item.Level;

			ItemDataEntry itemEntry;
			if (TheCache.ObjectIDCache.ItemDataEntries.TryGetValue(SNO, out itemEntry))
			{
				ItemType=itemEntry.ItemType;
				ThisDBItemType = ItemFunc.PluginItemTypeToDBItemType(ItemType);
				ThisFollowerType = ItemFunc.ReturnFollowerType(ItemType);
				LegendaryItemType = itemEntry.LegendaryType;
			}
			else
			{
				ThisFollowerType = item.FollowerSpecialType;
				ThisDBItemType = item.ItemType;
				ItemType = ItemFunc.DetermineItemType(ThisInternalName, ThisDBItemType, ThisFollowerType);
			}

			BaseItemType = ItemFunc.DetermineBaseType(ItemType);
			IsStackableItem = ItemFunc.DetermineIsStackable(ItemType);
			IsTwoSlot = ItemFunc.DetermineIsTwoSlot(ItemType);

			if (BaseItemType== PluginBaseItemTypes.Armor || BaseItemType== PluginBaseItemTypes.Jewelry || BaseItemType== PluginBaseItemTypes.Offhand || BaseItemType == PluginBaseItemTypes.WeaponOneHand || BaseItemType == PluginBaseItemTypes.WeaponRange || BaseItemType == PluginBaseItemTypes.WeaponTwoHand)
			{
				if (BaseItemType == PluginBaseItemTypes.WeaponOneHand)
					ThisOneHanded = true;
				else if (BaseItemType == PluginBaseItemTypes.WeaponTwoHand)
					TwoHanded = true;
			}


			
			ACDItem = item;
			ACDGUID = item.ACDGuid;
			ThisDynamicID = item.DynamicId;
			ThisRealName = item.Name;
			ThisGoldAmount = item.Gold;
			ThisLevel = item.Level;
			ThisItemStackQuantity = item.ItemStackQuantity;
			
			ThisQuality = item.ItemQualityLevel;
			ThisOneHanded = item.IsOneHand;
			TwoHanded = item.IsTwoHand;
			IsUnidentified = item.IsUnidentified;
			
			ThisDyeType = item.DyeType;

			IsPotion = ItemType == PluginItemTypes.HealthPotion;
			if (IsPotion)
				PotionType = ItemFunc.ReturnPotionType(SNO);
			

			invRow = item.InventoryRow;
			invCol = item.InventoryColumn;

			IsVendorBought = item.IsVendorBought;

			if (BaseItemType== PluginBaseItemTypes.Armor || BaseItemType== PluginBaseItemTypes.Jewelry || BaseItemType== PluginBaseItemTypes.Offhand || BaseItemType == PluginBaseItemTypes.WeaponOneHand || BaseItemType == PluginBaseItemTypes.WeaponRange || BaseItemType == PluginBaseItemTypes.WeaponTwoHand|| BaseItemType== PluginBaseItemTypes.FollowerItem)
			{
				ItemStats thesestats = item.Stats;
				ItemStatString = thesestats.ToString();
				ItemStatProperties = new ItemProperties(thesestats);
			}

			if (BaseItemType == PluginBaseItemTypes.Gem && ItemType == PluginItemTypes.LegendaryGem)
			{
				try
				{
					LegendaryGemRank = item.GetAttribute<int>(ActorAttributeType.JewelRank);
				}
				catch (Exception)
				{
					Logger.DBLog.Debug("Failed to get Jewel Rank for Legendary Gem!");
				}

				try
				{
					LegendaryGemType = (LegendaryGemTypes)Enum.Parse(typeof(LegendaryGemTypes), SNO.ToString());
				}
				catch (Exception)
				{
					LegendaryGemType= LegendaryGemTypes.None;
				}
					
			}

			if (itemEntry==null && !IsUnidentified)
			{
				if (FunkyBaseExtension.Settings.Debugging.DebuggingData && FunkyBaseExtension.Settings.Debugging.DebuggingDataTypes.HasFlag(DebugDataTypes.Items))
				{
					ObjectCache.DebuggingData.CheckEntry(this);
				}
			}
		}


		public bool IsRegularPotion
		{
			get
			{
				return SNO == 304319;
			}
		}
		public bool IsHoradricCache
		{
			get { return ThisInternalName.StartsWith("HoradricCache"); }
		}
		public bool IsSalvagable
		{
			get
			{
				return  (!IsVendorBought && ThisLevel != 1) &&
						(BaseItemType == PluginBaseItemTypes.Armor || BaseItemType == PluginBaseItemTypes.FollowerItem ||
				        BaseItemType == PluginBaseItemTypes.Jewelry || BaseItemType == PluginBaseItemTypes.Offhand ||
				        BaseItemType == PluginBaseItemTypes.WeaponOneHand || BaseItemType == PluginBaseItemTypes.WeaponRange || 
						BaseItemType == PluginBaseItemTypes.WeaponTwoHand);
			}
		}


	}



}