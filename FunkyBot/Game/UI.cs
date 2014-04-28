using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Game
{
	public class UI
	{
		public static bool ValidateUIElement(UIElement uie)
		{
			return uie != null && uie.IsValid && uie.IsVisible;
		}
		public static UIElement Inventory_Dialog_MainPage
		{
			get
			{
				try { return UIElement.FromHash(0x3622D03B2C9B8E6D); }
				catch { return null; }
			}
		}

		public static UIElement BloodShardVendor_GoldText
		{
			get
			{
				try { return UIElement.FromHash(0x8470DAE1C16136B8); }
				catch { return null; }
			}
		}
		// 

		public static class WaypointMap
		{
			public static UIElement WaypointMap_ZoomOut
			{
				get
				{
					try { return UIElement.FromHash(0xCB314C484693A30F); }
					catch { return null; }
				}
			}
			public static UIElement WaypointMap_ActOne
			{
				get
				{
					try { return UIElement.FromHash(0x45B83395BC996ADB); }
					catch { return null; }
				}
			}
			public static UIElement WaypointMap_ActTwo
			{
				get
				{
					try { return UIElement.FromHash(0x854BFC0273981332); }
					catch { return null; }
				}
			}
			public static UIElement WaypointMap_ActThree
			{
				get
				{
					try { return UIElement.FromHash(0xF860C7A78EE7F749); }
					catch { return null; }
				}
			}
			public static UIElement WaypointMap_ActFour
			{
				get
				{
					try { return UIElement.FromHash(0x370EB298B3D336B0); }
					catch { return null; }
				}
			}
			public static UIElement WaypointMap_ActFive
			{
				get
				{
					try { return UIElement.FromHash(0x68BF74C564275057); }
					catch { return null; }
				}
			}
			//
			public static UIElement WaypointMap_ActOneTown
			{
				get
				{
					try { return UIElement.FromHash(0xCC1886B0F5996A09); }
					catch { return null; }
				}
			}
			public static UIElement WaypointMap_ActTwoTown
			{
				get
				{
					try { return UIElement.FromHash(0x95ACFD674568C08E); }
					catch { return null; }
				}
			}
			public static UIElement WaypointMap_ActThreeTown
			{
				get
				{
					try { return UIElement.FromHash(0x9D81B3A0A605115F); }
					catch { return null; }
				}
			}
			public static UIElement WaypointMap_ActFourDiamondGates
			{
				get
				{
					try { return UIElement.FromHash(0xD30F71DD3651AA58); }
					catch { return null; }
				}
			}
			public static UIElement WaypointMap_ActFiveTown
			{
				get
				{
					try { return UIElement.FromHash(0x890DE9F8105A05F1); }
					catch { return null; }
				}
			}

			public static UIElement GetWaypointUIByWaypointID(int id)
			{
				if (id <= 17) return WaypointMap_ActOneTown;
				if (id <= 25) return WaypointMap_ActTwoTown;
				if (id <= 39) return WaypointMap_ActThreeTown;
				if (id <= 45) return WaypointMap_ActFourDiamondGates;
				return WaypointMap_ActFiveTown;
			}
			public static UIElement GetWaypointActUIByWaypointID(int id)
			{

				if (id <= 17) return WaypointMap_ActOne;
				if (id <= 25) return WaypointMap_ActTwo;
				if (id <= 39) return WaypointMap_ActThree;
				if (id <= 45) return WaypointMap_ActFour;
				return WaypointMap_ActFive;
			}
		}

		public static class BloodShardVendor
		{
			public static UIElement BloodShardVendorMainDialog
			{
				get
				{
					try { return UIElement.FromHash(0xA83F2BC15AC524D7); }
					catch { return null; }
				}
			}

			public static UIElement BloodShardVendorWeaponTab
			{
				get
				{
					try { return UIElement.FromHash(0x95EFA6BFC7BD2AD5); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorArmorTab
			{
				get
				{
					try { return UIElement.FromHash(0x95EFA5BFC7BD2922); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorTrinketsTab
			{
				get
				{
					try { return UIElement.FromHash(0x95EFA4BFC7BD276F); }
					catch { return null; }
				}
			}
			//
			public static UIElement BloodShardVendorOneHandItem
			{
				get
				{
					try { return UIElement.FromHash(0xB9491978054D9A55); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorTwoHandItem
			{
				get
				{
					try { return UIElement.FromHash(0xAFC5FE77FFAC9B0A); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorQuiver
			{
				get
				{
					try { return UIElement.FromHash(0xB9491878054D98A2); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorOrb
			{
				get
				{
					try { return UIElement.FromHash(0xAFC5FF77FFAC9CBD); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorMojo
			{
				get
				{
					try { return UIElement.FromHash(0xB9491778054D96EF); }
					catch { return null; }
				}
			}
			//
			public static UIElement BloodShardVendorHelm
			{
				get
				{
					try { return UIElement.FromHash(0xB9491978054D9A55); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorGloves
			{
				get
				{
					try { return UIElement.FromHash(0xAFC5FE77FFAC9B0A); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorBoots
			{
				get
				{
					try { return UIElement.FromHash(0xB9491878054D98A2); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorChestArmor
			{
				get
				{
					try { return UIElement.FromHash(0xAFC5FF77FFAC9CBD); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorBelt
			{
				get
				{
					try { return UIElement.FromHash(0xB9491778054D96EF); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorShoulders
			{
				get
				{
					try { return UIElement.FromHash(0xAFC5FC77FFAC97A4); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorPants
			{
				get
				{
					try { return UIElement.FromHash(0xB9491678054D953C); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorBracers
			{
				get
				{
					try { return UIElement.FromHash(0xAFC5FD77FFAC9957); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorShield
			{
				get
				{
					try { return UIElement.FromHash(0xB9491578054D9389); }
					catch { return null; }
				}
			}
			//
			public static UIElement BloodShardVendorRing
			{
				get
				{
					try { return UIElement.FromHash(0xB9491978054D9A55); }
					catch { return null; }
				}
			}
			public static UIElement BloodShardVendorAmulet
			{
				get
				{
					try { return UIElement.FromHash(0xAFC5FE77FFAC9B0A); }
					catch { return null; }
				}
			}

		}

		
		public static UIElement BountyCompleteContinue
		{
			get
			{
				try { return UIElement.FromHash(0x278249110947CA00); }
				catch { return null; }
			}
		}
	}
}
