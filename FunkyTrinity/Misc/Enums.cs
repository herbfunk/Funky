using System;

namespace FunkyTrinity
{
    public partial class Funky
    {
		  internal enum ShrineTypes
		  {
				Fleeting=0,
				Enlightenment=1,
				Frenzy=2,
				Fortune=3,
				Protection=4,
				Empowered=5,
		  }
		  internal static ShrineTypes FindShrineType(int SNOID)
		  {
				switch (SNOID)
				{
					 case 176075:
						  return ShrineTypes.Enlightenment;
					 case 176077:
						  return ShrineTypes.Frenzy;
					 case 176074:
						  return ShrineTypes.Protection;
					 case 176076:
						  return ShrineTypes.Fortune;
					 case 260331:
						  return ShrineTypes.Fleeting;
					 default:
						  return ShrineTypes.Empowered; //260330
				}
		  }
		  internal enum GemQuality
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
		  internal enum GilesBaseItemType
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

		  //Used for pet counter in character cache.
		  public enum PetTypes
		  {
				MONK_MysticAlly=1,
				WITCHDOCTOR_Gargantuan=2,
				WITCHDOCTOR_ZombieDogs=4,
				DEMONHUNTER_Pet=8,
				WIZARD_Hydra=16,
		  }

		  public enum AvoidanceType
		  {
				ArcaneSentry,
				AzmodanBodies,
				AzmodanFireball,
				AzmodanPool,
				AzmodanOrb,
				BeeProjectile,
				BelialGround,
				Dececrator,
				DiabloMetor,
				DiabloPrison,
				Frozen,
				GrotesqueExplosion,
				LacuniBomb,
				MageFirePool,
				MoltenCore,
				MoltenTrail,
				PlagueCloud,
				PlagueHand,
				PoisonGas,
				ShamanFireBall,
				SuccubusProjectile,
				TreeSpore,
				Unknown,
				Wall,
		  }

		  ///<summary>
		  ///Determines the type of blacklist an object should recieve. Permanent is entire game, Temporary is 60 seconds long.
		  ///</summary>
		  public enum BlacklistType
		  {
				None,
				Temporary,
				Permanent
		  }

		  //Range Values used for monster range counters
		  public enum RangeIntervals
		  {
				Range_50=0,
				Range_40=1,
				Range_30=2,
				Range_25=3,
				Range_20=4,
				Range_15=5,
				Range_12=6,
				Range_6=7,
		  }

		  ///<summary>
		  ///Used to describe the object type for target handling.
		  ///</summary>
		  [Flags]
		  public enum TargetType
		  {

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
				Door=1024,
				None=2048,

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
				Monster=1,
				StaticAvoidance=2,
				MovingAvoidance=4,
				ServerObject=8,
				Destructable=16,
				None=32,

				All=Monster|MovingAvoidance|StaticAvoidance|ServerObject,
				Avoidance=StaticAvoidance|MovingAvoidance,
				Navigation=Monster|ServerObject|Destructable,
		  }

    }
}