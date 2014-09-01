using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fBaseXtensions.Helpers;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;

namespace fBaseXtensions.Game
{
	public class UI
	{
		/// <summary>
		/// Checks if UIElement is not null and is valid and visible.
		/// </summary>
		public static bool ValidateUIElement(UIElement uie)
		{
			return uie != null && uie.IsValid && uie.IsVisible;
		}

		public static List<UIElement> GetChildren(UIElement uie)
		{
			List<UIElement> retList = new List<UIElement>();

			try
			{
				foreach (var u in UIElement.GetChildren(uie))
				{
					retList.Add(u);
				}
			}
			catch
			{

			}

			return retList;
		}
		public static List<UIElement> GetUIMap()
		{
			List<UIElement> retList = new List<UIElement>();

			try
			{
				foreach (var u in UIElement.UIMap)
				{
					retList.Add(u);
				}
			}
			catch
			{

			}

			return retList;
		}

		public static string UIElementString(UIElement uie)
		{
			try
			{

				return String.Format("Name {0} Hash {1} HasText {2} Text {3} Visible {4} Enabled {5}",
					uie.Name, string.Format("0x{0:X}", uie.Hash), uie.HasText, uie.Text, uie.IsVisible, uie.IsEnabled);
			}
			catch (Exception ex)
			{
				return String.Format("Exception Occured!\r\n{0}", ex.Message);
			}
		}

		public static class WaypointMap
		{
			public static UIElement WaypointMap_Container
			{
				get
				{
					try { return UIElement.FromHash(0xCFFB84D91466AFCC); }
					catch {return null;}
				}
			}
			public static UIElement WaypointMap_Local
			{
				get
				{
					try { return UIElement.FromHash(0xD5B19145DA950BB0); }
					catch { return null; }
				}
			}
			public static UIElement WaypointMap_CloseButton
			{
				get
				{
					try { return UIElement.FromHash(0xDA148F18B99E607); }
					catch { return null; }
				}
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

		public static class GameMenu
		{
			public static UIElement SwitchHeroButton
			{
				get
				{
					try { return UIElement.FromHash(0xBE4E61ABD1DCDC79); }
					catch { return null; }
				}
			}
			public static UIElement CreateHeroButton
			{
				get
				{
					try { return UIElement.FromHash(0x744BC83D82918CE2); }
					catch { return null; }
				}
			}
			public static UIElement SelectHeroButton
			{
				get
				{
					try { return UIElement.FromHash(0x5D73E830BC87CE66); }
					catch { return null; }
				}
			}
			public static UIElement HeroNameText
			{
				get
				{
					try { return UIElement.FromHash(0x8D2C771F09BC037F); }
					catch { return null; }
				}
			}
			public static UIElement CreateNewHeroButton
			{
				get
				{
					try { return UIElement.FromHash(0x28578F7B0F6384C6); }
					catch { return null; }
				}
			}
			public static UIElement SelectHeroType(ActorClass type)
			{
				UIElement thisClassButton = null;
				switch (type)
				{
					case ActorClass.Barbarian:
						thisClassButton = UIElement.FromHash(0x98976D3F43BBF74);
						break;
					case ActorClass.DemonHunter:
						thisClassButton = UIElement.FromHash(0x98976D3F43BBF74);
						break;
					case ActorClass.Monk:
						thisClassButton = UIElement.FromHash(0x7733072C07DABF11);
						break;
					case ActorClass.Witchdoctor:
						thisClassButton = UIElement.FromHash(0x1A2DB1F47C26A8C2);
						break;
					case ActorClass.Wizard:
						thisClassButton = UIElement.FromHash(0xBC3AA6A915972065);
						break;
					case ActorClass.Crusader:
						thisClassButton = UIElement.FromHash(0x99AF146AC9D24C99);
						break;
				}

				return thisClassButton;
			}

			
			public static UIElement SelectHeroByIndex(int index)
			{
				try
				{
					switch (index)
					{
						case 0:
							return UIElement.FromHash(0x744AB2C58DBCA9FB);
						case 1:
							return UIElement.FromHash(0x744AB1C58DBCA848);
						case 2:
							return UIElement.FromHash(0x744AB4C58DBCAD61);
						case 3:
							return UIElement.FromHash(0x744AB3C58DBCABAE);
						case 4:
							return UIElement.FromHash(0x744AB6C58DBCB0C7);
						case 5:
							return UIElement.FromHash(0x744AB5C58DBCAF14);
						case 6:
							return UIElement.FromHash(0x744AB8C58DBCB42D);
						case 7:
							return UIElement.FromHash(0x744AB7C58DBCB27A);
						case 8:
							return UIElement.FromHash(0x744AAAC58DBC9C63);
						case 9:
							return UIElement.FromHash(0x744AA9C58DBC9AB0);
						case 10:
							return UIElement.FromHash(0x57948AAFD79243E8);
						case 11:
							return UIElement.FromHash(0x57948BAFD792459B);
					}

				}
				catch 
				{

				}
				return null;
			}
			public static UIElement SelectHeroMain
			{
				get
				{
					try { return UIElement.FromHash(0xE93F10A86BBE8010); }
					catch { return null; }
				}
			}
			public static UIElement HeroSelectList
			{
				get
				{
					try { return UIElement.FromHash(0x47D0B765B5139B32); }
					catch { return null; }
				}
			}
			public static UIElement HeroSelectListStackPanel
			{
				get
				{
					try { return UIElement.FromHash(0xCF94B42C4CC5C80D); }
					catch { return null; }
				}
			}

			//Name Root.NormalLayer.BattleNetHeroSelect_main.LayoutRoot.HeroSelectList.HeroList._content._stackpanel._item0.Portrait Hash 0xDF71FA14F653F0C8 HasText False Text  Visible True Enabled True

		}

		public static class GameMenuEscape
		{
			public static UIElement Background
			{
				get
				{
					try { return UIElement.FromHash(0xC7E4FEF85B9EC8B8); }
					catch { return null; }
				}
			}
			public static UIElement LowerDifficulty
			{
				get
				{
					try { return UIElement.FromHash(0xF90FDFF83B63DA65); }
					catch { return null; }
				}
			}
			public static UIElement ResumeGame
			{
				get
				{
					try { return UIElement.FromHash(0x47A64A5AF2758329); }
					catch { return null; }
				}
			}
			//
		}

		public static class Game
		{

			//
			public static UIElement Conversation_Dialog_Main
			{
				get
				{
					try { return UIElement.FromHash(0x9738C281AF3C6E2B); }
					catch { return null; }
				}
			}

			//Name Root.NormalLayer.vendor_dialog_mainPage.riftReward_dialog.LayoutRoot.gemUpgradePane.items_list Hash 0xF5010CDEA6F362EE HasText True Text qު�q߮�q߲�qߵ�q߸�q��q���q���q���q���q���q���q���q���q���q���V�V�V�V�V�V �V"�V#�V#�V%�V&�V)�U*�U+�U-�U/�U1�T2�T3�T5�T8�T9�T<�T<�T?�TA�SC�SF�SG�SJ�SM�SO�SS�SV�TZ�T[�U`�Uc�Vg�X k�X!o�X!q�X!u�Y"y�Z#|�Z#��[#��\$��\$��\%��^&��^&��`'��`'��`'��b)��a)��b)��c)��d*��d*��d+��f,��f,��X�X�X�X�X�X!�X#�X#�X$�X&�X'�X)�X+�X+�X-�X/�X1�X3�X5�X6�W8�W:�W<�W>�W@�WC�WD�VG�VH�VK�VM�VO�VS�VV�WZ�V[�X`�Xc�Xg�Z k�Z!o�Z!q�[!u�\"y�\#|�^#��^#��_$�� Visible True Enabled False
			public static UIElement RiftReward_gemUpgradePane_List
			{
				get
				{
					try { return UIElement.FromHash(0xF5010CDEA6F362EE); }
					catch { return null; }
				}
			}
			public static UIElement RiftReward_gemUpgradePane
			{
				get
				{
					try { return UIElement.FromHash(0x8210044D8CC4C43); }
					catch { return null; }
				}
			}
			public static UIElement RiftReward_MainDialog
			{
				get
				{
					try { return UIElement.FromHash(0xE528DB177E3CB4AA); }
					catch { return null; }
				}
			}
			public static UIElement RiftReward_UpgradeGem
			{
				get
				{
					try { return UIElement.FromHash(0x826E5716E8D4DD05); }
					catch { return null; }
				}
			}
			public static UIElement RiftReward_UpgradeKey
			{
				get
				{
					try { return UIElement.FromHash(0x4BDE2D63B5C36134); }
					catch { return null; }
				}
			}
			public static UIElement RiftReward_UpgradeContinue
			{
				get
				{
					try { return UIElement.FromHash(0x1A089FAFF3CB6576); }
					catch { return null; }
				}
			}

			public static UIElement BloodShardVendorMainDialog
			{
				get
				{
					try { return UIElement.FromHash(0xA83F2BC15AC524D7); }
					catch { return null; }
				}
			}

			//
			public static UIElement NeaphlemObeliskDialog
			{
				get
				{
					try { return UIElement.FromHash(0x3182F223039F15F0); }
					catch { return null; }
				}
			}

			public static UIElement RiftDialog_Item
			{
				get
				{
					try { return UIElement.FromHash(0x4C2246960A3EC9F6); }
					catch { return null; }
				}
			}
			//0x18CCECE7D2C0A87B
			public static UIElement Inventory_ButtonBackpack
			{
				get
				{
					try { return UIElement.FromHash(0xC261F82ABCC57ABF); }
					catch { return null; }
				}
			}

			public static UIElement SalvageMainPage
			{
				get
				{
					try { return UIElement.FromHash(0x244BD04C84DF92F1); }
					catch { return null; }
				}
			}
			public static UIElement SalvageMainPagTab2
			{
				get
				{
					try { return UIElement.FromHash(0xE062F9B5040F3229); }
					catch { return null; }
				}
			}
			public static UIElement SalvageAllNormal
			{
				get
				{
					try { return UIElement.FromHash(0xCE31A05539BE5710); }
					catch { return null; }
				}
			}
			public static UIElement SalvageAllMagical
			{
				get
				{
					try { return UIElement.FromHash(0xD58A34C0A51E3A60); }
					catch { return null; }
				}
			}
			public static UIElement SalvageAllRare
			{
				get
				{
					try { return UIElement.FromHash(0x9AA6E1AD644CF239); }
					catch { return null; }
				}
			}

			public static UIElement AchievementsContainer
			{
				get
				{
					try { return UIElement.FromHash(0xCD1649818D5141C1); }
					catch { return null; }
				}
			}
			public static UIElement AchievementsExitButton
			{
				get
				{
					try { return UIElement.FromHash(0x7FA919A6E2B7E32D); }
					catch { return null; }
				}
			}
			public static UIElement FriendsList
			{
				get
				{
					try { return UIElement.FromHash(0xDED5B2ABED3A471A); }
					catch { return null; }
				}
			}
			public static UIElement FriendsListExitButton
			{
				get
				{
					try { return UIElement.FromHash(0x7B1FD584DA74FA94); }
					catch { return null; }
				}
			}
			public static UIElement GroupsList
			{
				get
				{
					try { return UIElement.FromHash(0x5DBCECC3C3511278); }
					catch { return null; }
				}
			}
			public static UIElement GroupsListExitButton
			{
				get
				{
					try { return UIElement.FromHash(0x160F060502E012DE); }
					catch { return null; }
				}
			}

			public static UIElement Inventory
			{
				get
				{
					try { return UIElement.FromHash(0x3622D03B2C9B8E6D); }
					catch { return null; }
				}
			}
			public static UIElement Skills_Main
			{
				get
				{
					try { return UIElement.FromHash(0x90C698C4BC11D872); }
					catch { return null; }
				}
			}
			public static UIElement Skills_MainCloseButton
			{
				get
				{
					try { return UIElement.FromHash(0xEA2AA1332BD49910); }
					catch { return null; }
				}
			}
			public static UIElement Skills_MainExitButton
			{
				get
				{
					try { return UIElement.FromHash(0xEDE91C4942014134); }
					catch { return null; }
				}
			}
			public static UIElement Skills_Select
			{
				get
				{
					try { return UIElement.FromHash(0x6B74DAD9B0A77AD7); }
					catch { return null; }
				}
			}
			public static UIElement Skills_PassiveSelect
			{
				get
				{
					try { return UIElement.FromHash(0x55DDCD228312723A); }
					catch { return null; }
				}
			}

			//
		}

		internal static UIElement FindGameProhibitingElements()
		{
			if (Game.Skills_MainExitButton != null && ValidateUIElement(Game.Skills_MainExitButton))
				return Game.Skills_MainExitButton;
			if (Game.AchievementsExitButton != null && ValidateUIElement(Game.AchievementsExitButton))
				return Game.AchievementsExitButton;
			if (GameMenuEscape.ResumeGame != null && ValidateUIElement(GameMenuEscape.ResumeGame))
				return GameMenuEscape.ResumeGame;
			FunkyGame.Profile.CheckCurrentProfileBehavior(true);
			if (WaypointMap.WaypointMap_CloseButton != null && !FunkyGame.Profile.CurrentProfileBehaviorType.HasFlag(Profile.ProfileBehaviorTypes.UseWaypoint) &&  ValidateUIElement(WaypointMap.WaypointMap_CloseButton))
				return WaypointMap.WaypointMap_CloseButton;
			if (WaypointMap.WaypointMap_Local != null && ValidateUIElement(WaypointMap.WaypointMap_Local))
				return WaypointMap.WaypointMap_Local;
			return null;
		}

		internal static bool ClosingUIElements = false;
		private static ulong LastClickedElement = 0;
		public static async Task<bool> _CloseGameProhibitingElements()
		{
			var uie = FindGameProhibitingElements();
			if (uie != null)
			{
				if (LastClickedElement == uie.Hash)
				{
					Logger.DBLog.InfoFormat("Failed to close Game Prohibiting Element {0}", uie.Name);
					UIManager.CloseAllOpenFrames();
					return true;
				}

				Logger.DBLog.InfoFormat("Closing Game Prohibiting Element {0}", uie.Name);

				LastClickedElement = uie.Hash;
				uie.Click();

				return true;
			}

			LastClickedElement = 0;
			ClosingUIElements = false;
			return false;
		}
		internal static RunStatus CloseGameProhibitingElements()
		{
			var uie = FindGameProhibitingElements();
			if (uie!=null)
			{
				if (LastClickedElement==uie.Hash)
				{
					Logger.DBLog.InfoFormat("Failed to close Game Prohibiting Element {0}", uie.Name);
					UIManager.CloseAllOpenFrames();
					return RunStatus.Running;
				}
				
				Logger.DBLog.InfoFormat("Closing Game Prohibiting Element {0}", uie.Name);

				LastClickedElement = uie.Hash;
				uie.Click();

				return RunStatus.Running;
			}

			LastClickedElement = 0;
			ClosingUIElements = false;
			return RunStatus.Success;
		}

		//public static UIElement BountyCompleteContinue
		//{
		//	get
		//	{
		//		try { return UIElement.FromHash(0x278249110947CA00); }
		//		catch { return null; }
		//	}
		//}
	}
}
