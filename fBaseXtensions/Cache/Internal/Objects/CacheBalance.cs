using System;
using System.Collections.Generic;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Cache.Internal.Objects
{

	public class CacheBalance
	{
        public int BalanceID
        {
            get { return _balanceId; }
            set { _balanceId = value; }
        }
        private int _balanceId=-1;

        public int ItemLevel
        {
            get { return _itemLevel; }
            set { _itemLevel = value; }
        }
        private int _itemLevel=-1;

        public ItemType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        private ItemType _type= ItemType.Unknown;

        public ItemBaseType Base
        {
            get { return _base; }
            set { _base = value; }
        }
        private ItemBaseType _base= ItemBaseType.None;

        public FollowerType FollowerType
        {
            get { return _followerType; }
            set { _followerType = value; }
        }
        private FollowerType _followerType= FollowerType.None;

        public PluginItemTypes PluginType
        {
            get { return _pluginType; }
            set { _pluginType = value; }
        }
        private PluginItemTypes _pluginType= PluginItemTypes.Unknown;

        public PluginBaseItemTypes PluginBase
        {
            get { return _pluginBase; }
            set { _pluginBase = value; }
        }
        private PluginBaseItemTypes _pluginBase= PluginBaseItemTypes.Unknown;

        public bool IsStackable
        {
            get { return _isStackable; }
            set { _isStackable = value; }
        }
        private bool _isStackable = false;

        public bool IsTwoSlot
        {
            get { return _isTwoSlot; }
            set { _isTwoSlot = value; }
        }
        private bool _isTwoSlot=false;
	    
	    
	    


		public bool bNeedsUpdated { get; set; }
		public bool IsRegularPotion
		{
			get
			{
				return BalanceID == -2142362846;
			}
		}

		public PluginItemTypes GetGItemType(string internalName)
		{
			return ItemFunc.DetermineItemType(internalName, _type, _followerType);
		}

	    public CacheBalance(CacheItem item)
	    {
	        try
	        {
                BalanceID = item.ref_DiaItem.CommonData.GameBalanceId;
                _itemLevel = item.ref_DiaItem.CommonData.Level;
                _type = item.ref_DiaItem.CommonData.ItemType;
                _base = item.ref_DiaItem.CommonData.ItemBaseType;
                _followerType = item.ref_DiaItem.CommonData.FollowerSpecialType;

                _pluginType = ItemFunc.DetermineItemType(item.InternalName, _type, _followerType, item.SNOID);
                if (item.ItemDropType.HasValue)
                    _pluginBase = ItemFunc.DetermineBaseItemType(item.ItemDropType.Value);
                else
                    _pluginBase = ItemFunc.DetermineBaseItemType(item.InternalName, item.SNOID);

                _isStackable = ItemFunc.DetermineIsStackable(_pluginType, item.SNOID);

                if (!_isStackable)
                    _isTwoSlot = ItemFunc.DetermineIsTwoSlot(_pluginType);
	        }
	        catch (Exception ex)
	        {
	            Logger.Write(LogLevel.Items,
	                String.Format("Failed to create balance data for item {0}", item.DebugStringSimple));

	        }
	        
	    }


		public CacheBalance()
		{
			bNeedsUpdated = true;
		}

	    public override string ToString()
	    {
	        return String.Format("BalanceID {0} Level {1}\r\n" +
	                             "ItemType {2} BaseType {3} FollowerType {4}\r\n" +
	                             "PluginType {5} PluginBase {6}\r\n" +
	                             "IsStackable {7} IsTwoSlot {8}",
                                 _balanceId,_itemLevel, _type, _base, _followerType,
                                 _pluginType,_pluginBase,
                                 _isStackable,_isTwoSlot);
	    }


	    //Property -- Craft Plans
		public bool IsBlacksmithPlanSixProperties
		{
			get
			{
				return HashPlansPropertiesSix.Contains(this.BalanceID);
			}
		}
		public bool IsBlacksmithPlanFiveProperties
		{
			get
			{
				return HashPlansPropertiesFive.Contains(this.BalanceID);
			}
		}
		public bool IsBlacksmithPlanFourProperties
		{
			get
			{
				return HashPlansPropertiesFour.Contains(this.BalanceID);
			}
		}
		public bool IsBlacksmithPlanArchonGauntlets
		{
			get
			{
				return HashPlansArchonGauntlets.Contains(this.BalanceID);
			}
		}
		public bool IsBlacksmithPlanArchonSpaulders
		{
			get
			{
				return HashPlansArchonSpaulders.Contains(this.BalanceID);
			}
		}
		public bool IsBlacksmithPlanRazorspikes
		{
			get
			{
				return HashPlansRazorspikes.Contains(this.BalanceID);
			}
		}
		public bool IsJewelcraftDesignFlawlessStarGem
		{
			get
			{
				return HashDesignFlawlessStarGem.Contains(this.BalanceID);
			}
		}
		public bool IsJewelcraftDesignPerfectStarGem
		{
			get
			{
				return HashDesignPerfectStarGem.Contains(this.BalanceID);
			}
		}
		public bool IsJewelcraftDesignRadiantStarGem
		{
			get
			{
				return HashDesignRadiantStarGem.Contains(this.BalanceID);
			}
		}
		public bool IsJewelcraftDesignMarquiseGem
		{
			get
			{
				return HashDesignMarquiseGem.Contains(this.BalanceID);
			}
		}
		public bool IsJewelcraftDesignAmulet
		{
			get
			{
				return HashDesignAmulet.Contains(this.BalanceID);
			}
		}

	    

	    #region Craft Plans - Game Balance IDs

		private static readonly HashSet<int> HashPlansPropertiesSix = new HashSet<int>
		  {
				-1051150313,-93021373,-1660666893,-638006107,-1690232948,-2015049106,
				-364008976, -576445430, 972140825, 1108898772, -275669100, 531375006,
				2129978460, 364927531, -1661852814, -1162323497, 82340210, 1134806017,
				-636820186, 255305006, 368302888, 623242822, 844895418, -1689047027,
				1288600123, 435695963, 1110084693, 543691116, -1205502139, 1717766204,
				-115137848, 1784279615, 398631475, -807237752
		  };
		private static readonly HashSet<int> HashPlansPropertiesFive = new HashSet<int>
		  {
				82340209, -1661852815, 364927530, 2129978459, -275669101, 1108898771,
				-576445431, -2015049107, -364008977, -1690232949, -638006108, -1660666894,
				-1051150314, -1205502140, 435695962, 543691115, 1288600122, 1717766203,
 				-1689047028, 1110084692, 844895417, 368302887, 623242821, 255305005,
				1134806016, -636820187, -807237753, 398631474, -115137849
		  };
		private static readonly HashSet<int> HashPlansPropertiesFour = new HashSet<int>
		  {
				-364008978, -1690232950, -638006109, -1660666895, -275669102, 1108898770,
				-576445432, -2015049108, 255305004, 1134806015, -636820188, 82340208,
				-1661852816, 364927529, -807237754, 398631473, -115137850, 543691114,
				1288600121, -1689047029, 1110084691, 844895416, 368302886, 623242820
		  };

		private static readonly HashSet<int> HashPlansArchonGauntlets = new HashSet<int>
		  {
				-1427340245,-1427345983,-1427326253,-1427329159
		  };


		private static readonly HashSet<int> HashPlansArchonSpaulders = new HashSet<int>
		  {
				1194999700,1195013692,1194993962,1195010786
		  };


		private static readonly HashSet<int> HashPlansRazorspikes = new HashSet<int>
		  {
				-1656008187,-1656011093,-1656022179,-1656027917
		  };


		private static readonly HashSet<int> HashDesignFlawlessStarGem = new HashSet<int>
		  {
				-1171649812,872611723,521743063,2147129183
		  };


		private static readonly HashSet<int> HashDesignPerfectStarGem = new HashSet<int>
		  {
				-1171613874, 872647661, 521779001, 2147165121
		  };

		private static readonly HashSet<int> HashDesignRadiantStarGem = new HashSet<int>
		  {
				-1171577936, 872683599, 521814939, 2147201059
		  };


		private static readonly HashSet<int> HashDesignMarquiseGem = new HashSet<int>
		  {
				-1171541998, 872719537, 521850877, 2147236997
		  };

		private static readonly HashSet<int> HashDesignAmulet = new HashSet<int>
		  {
				2110615435,2110601443,2110612529,2110595705
		  };

		#endregion

		//TODO:: Add Tower, Diamond, and Mutilation IDs
		private static readonly HashSet<int> HashBottomlessPotions = new HashSet<int>
		{
			-2018707796, //Regeneration
			-2018707795, //Leech
			-2018707793, //Kulle-Aid

		};

	}

}