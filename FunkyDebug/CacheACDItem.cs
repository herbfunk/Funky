using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace FunkyDebug
{

	public class CacheACDItem
	{
		public string InternalName { get; set; }
		public string Name { get; set; }
		public int Level { get; set; }
		public ItemQuality ItemQualityLevel { get; set; }
		public int ThisGoldAmount { get; set; }
		public int GameBalanceId { get; set; }
		public int ThisDynamicID { get; set; }
		public bool IsOneHand { get; set; }
		public bool IsTwoHand { get; set; }
		public DyeType ThisDyeType { get; set; }
		public ItemType ItemType { get; set; }
		public ItemBaseType ItemBaseType { get; set; }
		public FollowerType ThisFollowerType { get; set; }
		public bool IsUnidentified { get; set; }
		public bool IsPotion { get; set; }
		public int ThisItemStackQuantity { get; set; }
		public float Dexterity { get; set; }
		public float Intelligence { get; set; }
		public float Strength { get; set; }
		public float Vitality { get; set; }
		public float LifePercent { get; set; }
		public float LifeOnHit { get; set; }
		public float LifeSteal { get; set; }
		public float HealthPerSecond { get; set; }
		public float MagicFind { get; set; }
		public float GoldFind { get; set; }
		public float MovementSpeed { get; set; }
		public float PickUpRadius { get; set; }
		public float Sockets { get; set; }
		public float CritPercent { get; set; }
		public float CritDamagePercent { get; set; }
		public float AttackSpeedPercent { get; set; }
		public float MinDamage { get; set; }
		public float MaxDamage { get; set; }
		public float BlockChance { get; set; }
		public float Thorns { get; set; }
		public float ResistAll { get; set; }
		public float ResistArcane { get; set; }
		public float ResistCold { get; set; }
		public float ResistFire { get; set; }
		public float ResistHoly { get; set; }
		public float ResistLightning { get; set; }
		public float ResistPhysical { get; set; }
		public float ResistPoison { get; set; }
		public float WeaponDamagePerSecond { get; set; }
		public float ArmorBonus { get; set; }
		public float MaxDiscipline { get; set; }
		public float MaxMana { get; set; }
		public float ArcaneOnCrit { get; set; }
		public float ManaRegen { get; set; }
		public float SpiritRegen { get; set; }
		public float ExperienceBonus { get; set; }

		public float GlobeBonus { get; set; }
		public ACDItem ACDItem { get; set; }
		public int ACDGUID { get; set; }

		//inventory positions
		public int invRow { get; set; }
		public int invCol { get; set; }

		public string ItemStatString { get; set; }
		public bool IsStackableItem { get; set; }
		public ItemStats Stats { get; set; }

		public CacheACDItem(ACDItem item)
		{
			ACDItem = item;

			ACDGUID = item.ACDGuid;
			GameBalanceId = item.GameBalanceId;
			ThisDynamicID = item.DynamicId;
			InternalName = item.InternalName;
			Name = item.Name;
			ThisGoldAmount = item.Gold;
			Level = item.Level;
			ThisItemStackQuantity = item.ItemStackQuantity;
			ThisFollowerType = item.FollowerSpecialType;
			//ItemQualityLevel = item.ItemQualityLevel;
			IsOneHand = item.IsOneHand;
			IsTwoHand=item.IsTwoHand;
			IsUnidentified = item.IsUnidentified;
			ItemType = item.ItemType;
			ItemBaseType = item.ItemBaseType;
			ThisDyeType = item.DyeType;
			IsPotion = item.IsPotion;
			invRow = item.InventoryRow;
			invCol = item.InventoryColumn;


			Stats = item.Stats;

			ItemQualityLevel = Stats.Quality;
			ItemStatString = Stats.ToString();
			ArmorBonus = Stats.ArmorBonus;
			ArcaneOnCrit = Stats.ArcaneOnCrit;
			AttackSpeedPercent = Stats.AttackSpeedPercent;
			BlockChance = Stats.BlockChance;
			CritPercent = Stats.CritPercent;
			CritDamagePercent = Stats.CritDamagePercent;
			Dexterity = Stats.Dexterity;
			ExperienceBonus = Stats.ExperienceBonus;
			Intelligence = Stats.Intelligence;

			LifePercent = Stats.LifePercent;
			LifeOnHit = Stats.LifeOnHit;
			LifeSteal = Stats.LifeSteal;
			HealthPerSecond = Stats.HealthPerSecond;
			MagicFind = Stats.MagicFind;
			GoldFind = Stats.GoldFind;
			GlobeBonus = Stats.HealthGlobeBonus;
			MovementSpeed = Stats.MovementSpeed;
			PickUpRadius = Stats.PickUpRadius;

			ResistAll = Stats.ResistAll;
			ResistArcane = Stats.ResistArcane;
			ResistCold = Stats.ResistCold;
			ResistFire = Stats.ResistFire;
			ResistHoly = Stats.ResistHoly;
			ResistLightning = Stats.ResistLightning;
			ResistPhysical = Stats.ResistPhysical;
			ResistPoison = Stats.ResistPoison;

			MinDamage = Stats.MinDamage;
			MaxDamage = Stats.MaxDamage;
			MaxDiscipline = Stats.MaxDiscipline;
			MaxMana = Stats.MaxMana;
			ManaRegen = Stats.ManaRegen;


			Thorns = Stats.Thorns;

			Sockets = Stats.Sockets;
			SpiritRegen = Stats.SpiritRegen;
			Strength = Stats.Strength;
			Vitality = Stats.Vitality;
			WeaponDamagePerSecond = Stats.WeaponDamagePerSecond;
			Stats = Stats;

		}
	}



}