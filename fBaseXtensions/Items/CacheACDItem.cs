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
	   
        public string ThisRealName
        {
            get { return _thisRealName; }
            set { _thisRealName = value; }
        }
        private string _thisRealName="";

        public int ThisLevel
        {
            get { return _thisLevel; }
            set { _thisLevel = value; }
        }
        private int _thisLevel=0;

        public int ThisGoldAmount
        {
            get { return _thisGoldAmount; }
            set { _thisGoldAmount = value; }
        }
        private int _thisGoldAmount=0;

        public ItemQuality ThisQuality
        {
            get { return _thisQuality; }
            set { _thisQuality = value; }
        }
	    private ItemQuality _thisQuality=ItemQuality.Invalid;

        public long ThisItemStackQuantity
        {
            get { return _thisItemStackQuantity; }
            set { _thisItemStackQuantity = value; }
        }
        private long _thisItemStackQuantity = 0;

	    public int MaxStackQuanity
	    {
            get { return _maxstackquanity; }
            set { _maxstackquanity = value; }
	    }
        private int _maxstackquanity = 0;

        public DyeType ThisDyeType
        {
            get { return _thisDyeType; }
            set { _thisDyeType = value; }
        }
        private DyeType _thisDyeType= DyeType.None;
	   
		public int LegendaryGemRank { get; set; }
        public int KeystoneRank { get; set; }
		public LegendaryGemTypes LegendaryGemType { get; set; }
		public int ThisBalanceID { get; set; }
		public int ThisDynamicID { get; set; }
		public bool ThisOneHanded { get; set; }
		public bool TwoHanded { get; set; }
        
		public ItemType ThisDBItemType { get; set; }
		public FollowerType ThisFollowerType { get; set; }

        public bool IsUnidentified
        {
            get { return _isUnidentified; }
            set { _isUnidentified = value; }
        }
	    private bool _isUnidentified=false;

        public bool IsVendorBought
        {
            get { return _isVendorBought; }
            set { _isVendorBought = value; }
        }
        private bool _isVendorBought=false;

        public int SocketsFilled { get; set; }
		
		public bool IsPotion { get; set; }
		public PotionTypes PotionType=PotionTypes.None;
		public LegendaryItemTypes LegendaryItemType=LegendaryItemTypes.None;

        


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

        

		public int DurabilityCurrent { get; set; }
		public int DurabilityMax { get; set; }
		public float DurabilityPercent { get; set; }

		//Plugin Item Properties
		public PluginBaseItemTypes BaseItemType { get; set; }
		public PluginItemTypes ItemType { get; set; }


	    public string SimpleDebugString
	    {
	        get { return String.Format("Name {0} Sno {1} BalanceID {2}", ThisInternalName, SNO, ThisBalanceID); }
	    }

		public CacheACDItem(ACDItem item)
		{
            ACDItem = item;
			SNO=item.ActorSNO;
			ThisInternalName = item.InternalName;

		    try
		    {
                _thisLevel = item.Level;
		    }
		    catch (Exception ex)
		    {
                Logger.Write(LogLevel.Items, "Failed to retrieve item level for {0} \r\n {1}", SimpleDebugString, ex.Message);
		    }
			

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
			IsStackableItem = ItemFunc.DetermineIsStackable(ItemType, SNO);
            IsTwoSlot = !IsStackableItem && ItemFunc.DetermineIsTwoSlot(ItemType);

            //Armor / Jewelery / Weapons / Offhand / Follower Items
			if (BaseItemType== PluginBaseItemTypes.Armor || BaseItemType== PluginBaseItemTypes.Jewelry || BaseItemType== PluginBaseItemTypes.Offhand || BaseItemType == PluginBaseItemTypes.WeaponOneHand || BaseItemType == PluginBaseItemTypes.WeaponRange || BaseItemType == PluginBaseItemTypes.WeaponTwoHand || BaseItemType == PluginBaseItemTypes.FollowerItem)
			{
				if (BaseItemType == PluginBaseItemTypes.WeaponOneHand)
					ThisOneHanded = true;
				else if (BaseItemType == PluginBaseItemTypes.WeaponTwoHand)
					TwoHanded = true;


                try
                {
                    ItemStats thesestats = item.Stats;
                    ItemStatString = thesestats.ToString();
                    ItemStatProperties = new ItemProperties(thesestats);

                    if (ItemStatProperties.Sockets > 0)
                    {
                        SocketsFilled = item.NumSocketsFilled;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(LogLevel.Items, "Failed to retrieve item stats {0} \r\n {1}", SimpleDebugString, ex.Message);
                }


                #region Durability
                try
                {
                    //Durability
                    DurabilityCurrent = item.CurrentDurability;
                    DurabilityMax = item.MaxDurability;
                    DurabilityPercent = item.DurabilityPercent;
                }
                catch (Exception ex)
                {
                    Logger.Write(LogLevel.Items, "Failed to retrieve item durability {0} \r\n {1}", SimpleDebugString, ex.Message);
                } 
                #endregion

                #region ItemQualityLevel
                try
                {
                    _thisQuality = item.ItemQualityLevel;
                }
                catch (Exception ex)
                {
                    Logger.Write(LogLevel.Items, "Failed to retrieve item quality {0} \r\n {1}", SimpleDebugString, ex.Message);
                }
                #endregion

			}
			else
			{//Gem, Misc

                if (ItemType == PluginItemTypes.KeyStone)
                {
                    KeystoneRank = ItemFunc.GetGreaterRiftKeystoneRank(SNO);

                    if (KeystoneRank == -1)
                    {
                        try
                        {
                            KeystoneRank = item.TieredLootRunKeyLevel;
                        }
                        catch (Exception ex)
                        {
                            Logger.DBLog.DebugFormat("Failed to get TieredLootRunKeyLevel for Keystone!{0} \r\n {1}", SimpleDebugString, ex.Message);
                        }

                    }
                }
                else if (BaseItemType == PluginBaseItemTypes.Gem && ItemType == PluginItemTypes.LegendaryGem)
                {
                    try
                    {
                        LegendaryGemRank = item.JewelRank;
                    }
                    catch (Exception ex)
                    {
                        Logger.DBLog.DebugFormat("Failed to get Jewel Rank for Legendary Gem!{0} \r\n {1}", SimpleDebugString, ex.Message);
                    }

                    try
                    {
                        LegendaryGemType = (LegendaryGemTypes)Enum.Parse(typeof(LegendaryGemTypes), SNO.ToString());
                    }
                    catch (Exception)
                    {
                        LegendaryGemType = LegendaryGemTypes.None;
                    }

                }
                else
                {
                    IsPotion = ItemType == PluginItemTypes.HealthPotion || ItemType == PluginItemTypes.LegendaryHealthPotion;
                    if (IsPotion)
                        PotionType = ItemFunc.ReturnPotionType(SNO);
                }

                #region DyeType
                try
                {
                    _thisDyeType = item.DyeType;
                }
                catch (Exception ex)
                {
                    Logger.Write(LogLevel.Items, "Failed to retrieve item dye type {0} \r\n {1}", SimpleDebugString, ex.Message);
                }
                #endregion

			    if (IsStackableItem)
			    {
                    #region ItemStackQuantity
                    try
                    {
                        _thisItemStackQuantity = item.ItemStackQuantity;
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(LogLevel.Items, "Failed to retrieve item stack quanity {0} \r\n {1}", SimpleDebugString, ex.Message);
                    }
                    #endregion

                    #region ItemStackMaxQuantity
                    try
                    {
                        _maxstackquanity = item.MaxStackCount;
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(LogLevel.Items, "Failed to retrieve item max stack quanity {0} \r\n {1}", SimpleDebugString, ex.Message);
                    }
                    #endregion
			    }
			}

            
            #region GameBalanceId
            try
            {
                ThisBalanceID = item.GameBalanceId;
            }
            catch (Exception ex)
            {
                Logger.Write(LogLevel.Items, "Failed to retrieve item GameBalanceId {0} \r\n {1}", SimpleDebugString, ex.Message);
            }
            #endregion

            #region ACDGuid
            try
            {
                ACDGUID = item.ACDGuid;
            }
            catch (Exception ex)
            {
                Logger.Write(LogLevel.Items, "Failed to retrieve item ACDGUID {0} \r\n {1}", SimpleDebugString, ex.Message);
            }
            #endregion

            #region DynamicId
            try
            {
                ThisDynamicID = item.DynamicId;
            }
            catch (Exception ex)
            {
                Logger.Write(LogLevel.Items, "Failed to retrieve item DynamicId {0} \r\n {1}", SimpleDebugString, ex.Message);
            }
            
            #endregion

            #region Name
            try
            {
                _thisRealName = item.Name;
            }
            catch (Exception ex)
            {
                Logger.Write(LogLevel.Items, "Failed to retrieve item name {0} \r\n {1}", SimpleDebugString, ex.Message);
            } 
            #endregion

            #region Gold
            try
            {
                _thisGoldAmount = item.Gold;
            }
            catch (Exception ex)
            {
                Logger.Write(LogLevel.Items, "Failed to retrieve item gold amount {0} \r\n {1}", SimpleDebugString, ex.Message);
            } 
            #endregion


		    if (!IsStackableItem)
		    {
                #region IsUnidentified
                try
                {
                    _isUnidentified = item.IsUnidentified;
                }
                catch (Exception ex)
                {
                    Logger.Write(LogLevel.Items, "Failed to retrieve item is identified {0} \r\n {1}", SimpleDebugString, ex.Message);
                }
                #endregion
		    }

            #region IsVendorBought
            try
            {
                _isVendorBought = item.IsVendorBought;
            }
            catch (Exception ex)
            {
                Logger.Write(LogLevel.Items, "Failed to retrieve item is vendor bought {0} \r\n {1}", SimpleDebugString, ex.Message);
            } 
            #endregion

            #region InventoryRow/Column
            try
            {
               invRow = item.InventoryRow;
               invCol = item.InventoryColumn;
            }
            catch (Exception ex)
            {
                Logger.Write(LogLevel.Items, "Failed to retrieve item inventory row/column {0} \r\n {1}", SimpleDebugString, ex.Message);
            } 
            #endregion
			

			if (itemEntry==null && !_isUnidentified && ItemType!= PluginItemTypes.Unknown && _thisRealName!=String.Empty)
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
				return  (!_isVendorBought && _thisLevel != 1 && !_isUnidentified) &&
						(BaseItemType == PluginBaseItemTypes.Armor || BaseItemType == PluginBaseItemTypes.FollowerItem ||
				        BaseItemType == PluginBaseItemTypes.Jewelry || BaseItemType == PluginBaseItemTypes.Offhand ||
				        BaseItemType == PluginBaseItemTypes.WeaponOneHand || BaseItemType == PluginBaseItemTypes.WeaponRange || 
						BaseItemType == PluginBaseItemTypes.WeaponTwoHand);



			}
		}

	    


	    public override int GetHashCode()
		{
			return ThisDynamicID;
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			else
			{
				CacheACDItem p = (CacheACDItem)obj;
				return (ThisDynamicID == p.ThisDynamicID);
			}
		}

		public override string ToString()
		{
			string specificType = 
				LegendaryGemType != LegendaryGemTypes.None ? String.Format("Legendary Gem Type {0} Rank {1}", LegendaryGemType, LegendaryGemRank) :
				_thisDyeType != DyeType.None ? String.Format("Dye Type {0}", _thisDyeType) :
				LegendaryItemType != LegendaryItemTypes.None ? String.Format("Legendary Item Type {0}", LegendaryItemType) :
				ThisFollowerType != FollowerType.None ? String.Format("Follower Type {0}", ThisFollowerType) :
				"";

			return String.Format("Item {0} ({1}) [Sno: {2} Balance: {3}]\r\n" +
			                     "[DynamicID: {4} ACDGuid: {5}]\r\n" +
			                     "BaseType: {6} ItemType: {7} ({8})\r\n" +
								 "Quality: {9} Level: {10} IsUnidentified: {11} \r\n" +
			                     "Durability: {12} / {13} ({14})\r\n" +
			                     "InvRow {15} InvCol {16} StackQuanity {17}\r\n" +
								 "IsVendorBought {18}\r\n" +
			                     "{19}\r\n" +
			                     "{20}",
									_thisRealName, ThisInternalName, SNO, ThisBalanceID,
									ThisDynamicID, ACDGUID,
									BaseItemType, ItemType, ThisDBItemType,
									_thisQuality, _thisLevel, _isUnidentified,
									DurabilityCurrent,DurabilityMax,DurabilityPercent,
									invRow,invCol,_thisItemStackQuantity,
									_isVendorBought,
									specificType,
									ItemStatString);

			/*
		public int ThisGoldAmount { get; set; }
		public bool ThisOneHanded { get; set; }
		public bool TwoHanded { get; set; }



		public string ItemStatString = "";
		public ItemProperties ItemStatProperties { get; set; }
		public bool IsStackableItem { get; set; }
		public bool IsTwoSlot { get; set; }

			 */
		}
	}



}