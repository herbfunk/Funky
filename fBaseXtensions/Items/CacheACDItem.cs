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

			bool foundItemTypeUsingCache = false;
			PluginItemTypes _itemtype;
			if (!ItemFunc.DetermineItemTypeUsingSNO(SNO, out _itemtype))
			{
				if (ItemSnoCache.ReaperOfSoulItemTypes.TryGetValue(SNO, out _itemtype))
				{
					ItemType = _itemtype;
					foundItemTypeUsingCache = true;
				}
			}
			else
			{
				ItemType=_itemtype;
				foundItemTypeUsingCache = true;
			}

			if (!foundItemTypeUsingCache)
			{
				ThisFollowerType = item.FollowerSpecialType;
				ThisDBItemType = item.ItemType;
				ItemType = ItemFunc.DetermineItemType(ThisInternalName, ThisDBItemType, ThisFollowerType, SNO);
			}
			else
			{
				ThisDBItemType = ThisDBItemType = ItemFunc.PluginItemTypeToDBItemType(ItemType);
				ThisFollowerType = ItemFunc.ReturnFollowerType(ItemType);
			}

			BaseItemType = ItemFunc.DetermineBaseType(ItemType);
			IsStackableItem = ItemFunc.DetermineIsStackable(ItemType);
			IsTwoSlot = ItemFunc.DetermineIsTwoSlot(ItemType);

			if (BaseItemType== PluginBaseItemTypes.Armor || BaseItemType== PluginBaseItemTypes.Jewelry || BaseItemType== PluginBaseItemTypes.Offhand || BaseItemType == PluginBaseItemTypes.WeaponOneHand || BaseItemType == PluginBaseItemTypes.WeaponRange || BaseItemType == PluginBaseItemTypes.WeaponTwoHand)
			{
				if (!foundItemTypeUsingCache)
				{
					LegendaryItemTypes _legendarytype;
					if (ItemBalanceCache.LegendaryItems.TryGetValue(ThisBalanceID, out _legendarytype))
						LegendaryItemType = _legendarytype;
					else if (ItemSnoCache.LegendaryItems.TryGetValue(SNO, out _legendarytype))
						LegendaryItemType = _legendarytype;
				}

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

			IsPotion = ItemSnoCache.HealthPotionSNOIds.Contains(SNO);
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
		}


		public bool IsRegularPotion
		{
			get
			{
				return ThisBalanceID == -2142362846;
			}
		}
		public bool IsHoradricCache
		{
			get { return ThisInternalName.StartsWith("HoradricCache"); }
		}



	}



}