using System;
using System.Collections.Generic;
using Zeta;
using Zeta.Internals.Actors;

namespace FunkyBot.Cache
{

		  public class CacheBalance
		  {
			    public int iThisBalanceID { get; set; }
				public int iThisItemLevel { get; set; }
				public ItemType thisItemType { get; set; }
				public ItemBaseType thisItemBaseType { get; set; }
				public bool bThisTwoHand { get; set; }
				public bool bThisOneHand { get; set; }
				public FollowerType thisFollowerType { get; set; }

				public bool bNeedsUpdated { get; set; }
				public CacheBalance(int balanceID, int itemlevel, ItemType itemtype, ItemBaseType itembasetype, bool onehand, bool twohand, FollowerType followertype)
				{
					 iThisBalanceID = balanceID;
					 iThisItemLevel=itemlevel;
					 thisItemType=itemtype;
					 bThisOneHand=onehand;
					 bThisTwoHand=twohand;
					 thisItemBaseType=itembasetype;
					 thisFollowerType=followertype;
					 bNeedsUpdated=false;
				}

				public CacheBalance(int itemlevel, ItemType itemtype, bool onehand, FollowerType followertype)
				{
					 iThisItemLevel=itemlevel;
					 thisItemType=itemtype;
					 bThisOneHand=onehand;
					 thisFollowerType=followertype;
					 bNeedsUpdated=true;
				}

				public CacheBalance()
				{
					 bNeedsUpdated=true;
				}


			  //Property -- Craft Plans
				public bool IsBlacksmithPlanSixProperties
				{
					get
					{
						return HashPlansPropertiesSix.Contains(this.iThisBalanceID);
					}
				}
				public bool IsBlacksmithPlanFiveProperties
				{
					get
					{
						return HashPlansPropertiesFive.Contains(this.iThisBalanceID);
					}
				}
				public bool IsBlacksmithPlanFourProperties
				{
					get
					{
						return HashPlansPropertiesFour.Contains(this.iThisBalanceID);
					}
				}
				public bool IsBlacksmithPlanArchonGauntlets
				{
					get
					{
						return HashPlansArchonGauntlets.Contains(this.iThisBalanceID);
					}
				}
				public bool IsBlacksmithPlanArchonSpaulders
				{
					get
					{
						return HashPlansArchonSpaulders.Contains(this.iThisBalanceID);
					}
				}
				public bool IsBlacksmithPlanRazorspikes
				{
					get
					{
						return HashPlansRazorspikes.Contains(this.iThisBalanceID);
					}
				}
				public bool IsJewelcraftDesignFlawlessStarGem
				{
					get
					{
						return HashDesignFlawlessStarGem.Contains(this.iThisBalanceID);
					}
				}
				public bool IsJewelcraftDesignPerfectStarGem
				{
					get
					{
						return HashDesignPerfectStarGem.Contains(this.iThisBalanceID);
					}
				}
				public bool IsJewelcraftDesignRadiantStarGem
				{
					get
					{
						return HashDesignRadiantStarGem.Contains(this.iThisBalanceID);
					}
				}
				public bool IsJewelcraftDesignMarquiseGem
				{
					get
					{
						return HashDesignMarquiseGem.Contains(this.iThisBalanceID);
					}
				}
				public bool IsJewelcraftDesignAmulet
				{
					get
					{
						return HashDesignAmulet.Contains(this.iThisBalanceID);
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

		  }
    
}