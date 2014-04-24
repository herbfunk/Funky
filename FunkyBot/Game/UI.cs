using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zeta.Game;
using Zeta.Game.Internals;

namespace FunkyBot.Game
{
	public class UI
	{
		public static bool ValidateUIElement(UIElement uie)
		{
			return uie != null && uie.IsValid && uie.IsVisible;
		}

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

		//Waypoint 17 or less equals Act One
		//Waypoint 25 or less equals Act Two
		//Waypoint 26 to 39 Act Three
		//Waypoint 40-45 Act Four
		
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
