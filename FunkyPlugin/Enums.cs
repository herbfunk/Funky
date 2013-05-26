using System;

namespace FunkyTrinity
{
    public partial class Funky
    {
		  public enum GemQuality
		  {
				Chipped=14,
				Flawed=22,
				Normal=30,
				Flawless=36,
				Perfect=42,
				Radiant=48,
				Square=54,
				FlawlessSquare=60,
		  }
		  // Primary "lowest level" item type (eg EXACTLY what kind of item it is)
		  public enum GilesItemType
		  {
				Unknown,
				Axe,
				CeremonialKnife,
				HandCrossbow,
				Dagger,
				FistWeapon,
				Mace,
				MightyWeapon,
				Spear,
				Sword,
				Wand,
				TwoHandAxe,
				TwoHandBow,
				TwoHandDaibo,
				TwoHandCrossbow,
				TwoHandMace,
				TwoHandMighty,
				TwoHandPolearm,
				TwoHandStaff,
				TwoHandSword,
				StaffOfHerding,
				Mojo,
				Source,
				Quiver,
				Shield,
				Amulet,
				Ring,
				Belt,
				Boots,
				Bracers,
				Chest,
				Cloak,
				Gloves,
				Helm,
				Pants,
				MightyBelt,
				Shoulders,
				SpiritStone,
				VoodooMask,
				WizardHat,
				FollowerEnchantress,
				FollowerScoundrel,
				FollowerTemplar,
				CraftingMaterial,
				CraftTome,
				Ruby,
				Emerald,
				Topaz,
				Amethyst,
				SpecialItem,
				CraftingPlan,
				HealthPotion,
				Dye,
				HealthGlobe,
				InfernalKey,
		  }
		  // Base types, eg "one handed weapons" "armors" etc.
		  public enum GilesBaseItemType
		  {
				Unknown,
				WeaponOneHand,
				WeaponTwoHand,
				WeaponRange,
				Offhand,
				Armor,
				Jewelry,
				FollowerItem,
				Misc,
				Gem,
				HealthGlobe
		  }
        

		  /*
        public enum PotionType
        {
            Unknown = -1,
            HealthPotionMinor = 0, // 4440,
            HealthPotionLesser = 1, // 4439,

            //unconfirmed order!
            HealthPotion = 2,
            HealthPotionMajor = 3,
            HealthPotionGreater = 4,
            HealthPotionSuper = 5,
            HealthPotionHeroic = 6,
            HealthPotionRunic = 7,
            HealthPotionResplendent = 8,
            HealthPotionMythic = 9
        }
		  */

		  ///<summary>
		  ///Used to describe the object type for target handling.
		  ///</summary>
		  [Flags]
		  public enum TargetType
		  {
				None=0,
				Unit=1,
				Shrine=2,
				Interactable=4,
				Destructible=8,
				Barricade=16,
				Container=32,
				Item=64,
				Gold=128,
				Globe=256,
				Avoidance=512,
				Door=1028,

				All=Unit|Shrine|Interactable|Destructible|Barricade|Container|Item|Gold|Globe|Avoidance|Door,
				Gizmo=Shrine|Interactable|Destructible|Barricade|Container|Door,
				Interactables=Shrine|Interactable|Door|Container,
				ServerObjects=Unit|Interactables|Destructible|Barricade,
		  }

		  ///<summary>
		  ///Used to describe the objects obstacle type.
		  ///</summary>
		  [Flags]
		  public enum ObstacleType
		  {
				None=0,
				Monster=1,
				StaticAvoidance=2,
				MovingAvoidance=4,
				ServerObject=8,
				Destructable=16,
				All=Monster|MovingAvoidance|StaticAvoidance|ServerObject,
				Avoidance=StaticAvoidance|MovingAvoidance,
				Navigation=Monster|ServerObject|Destructable,
		  }

    }
}