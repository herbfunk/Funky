using System;
using System.Collections.Generic;
using Zeta.Game.Internals;

namespace fItemPlugin.Items
{
	public class ItemProperties
	{
		public float Dexterity { get; set; }
		public float Intelligence { get; set; }
		public float Strength { get; set; }
		public float Vitality { get; set; }

		public float ResistAll { get; set; }
		public float ResistArcane { get; set; }
		public float ResistCold { get; set; }
		public float ResistFire { get; set; }
		public float ResistHoly { get; set; }
		public float ResistLightning { get; set; }
		public float ResistPhysical { get; set; }
		public float ResistPoison { get; set; }

		public float LifePercent { get; set; }

		public float LifeOnHit { get; set; }
		public float LifeSteal { get; set; }
		public float LifeOnKill { get; set; }
		public float HealthPerSpiritSpent { get; set; }
		public float HealthPerSecond { get; set; }

		public float MagicFind { get; set; }
		public float GoldFind { get; set; }
		public float MovementSpeed { get; set; }
		public float PickUpRadius { get; set; }

		public float Sockets { get; set; }

		public float CritPercent { get; set; }
		public float CritDamagePercent { get; set; }
		
		public float MinDamage { get; set; }
		public float MaxDamage { get; set; }

		public float AttackSpeedPercent { get; set; }
		public float AttackSpeedPercentBonus { get; set; }


		public float BlockChance { get; set; }
		public float BlockChanceBonus { get; set; }

		public float Thorns { get; set; }

		
		
		public float MaxDiscipline { get; set; }
		public float MaxMana { get; set; }
		public float MaxArcanePower { get; set; }
		public float MaxFury { get; set; }
		public float MaxSpirit { get; set; }

		public float ArcaneOnCrit { get; set; }
		public float ManaRegen { get; set; }
		public float SpiritRegen { get; set; }
		public float HatredRegen { get; set; }

		public float ExperienceBonus { get; set; }
		public float GlobeBonus { get; set; }

		public float ArmorBonus { get; set; }
		public float Armor { get; set; }
		public float ArmorTotal { get; set; }

		public float Level { get; set; }
		public float ItemLevelRequirementReduction { get; set; }

		public float WeaponDamagePerSecond { get; set; }
		public float WeaponAttacksPerSecond { get; set; }
		public float WeaponDamagePercent { get; set; }
		public float WeaponMaxDamage { get; set; }
		public float WeaponMinDamage { get; set; }
		public float MinDamageElemental { get; set; }
		public float MaxDamageElemental { get; set; }

		public float PowerCooldownReductionPercent { get; set; }
		public float ResourceCostReductionPercent { get; set; }
		public float SkillDamagePercentBonus { get; set; }
		public float OnHitAreaDamageProcChance { get; set; }

		public float DamagePercentReductionFromElites { get; set; }
		public float DamageReductionPhysicalPercent { get; set; }
		public float DamagePercentBonusVsElites { get; set; }


		public float WeaponOnHitFearProcChance { get; set; }
		public float WeaponOnHitBlindProcChance { get; set; }
		public float WeaponOnHitFreezeProcChance { get; set; }
		public float WeaponOnHitChillProcChance { get; set; }
		public float WeaponOnHitImmobilizeProcChance { get; set; }
		public float WeaponOnHitKnockbackProcChance { get; set; }
		public float WeaponOnHitSlowProcChance { get; set; }
		public float WeaponOnHitBleedProcChance { get; set; }


		public float MaxDamageFire { get; set; }
		public float MinDamageFire { get; set; }
		public float FireSkillDamagePercentBonus { get; set; }

		public float MaxDamageLightning { get; set; }
		public float MinDamageLightning { get; set; }
		public float LightningSkillDamagePercentBonus { get; set; }

		public float MaxDamageCold { get; set; }
		public float MinDamageCold { get; set; }
		public float ColdSkillDamagePercentBonus { get; set; }

		public float MaxDamagePoison { get; set; }
		public float MinDamagePoison { get; set; }
		public float PosionSkillDamagePercentBonus { get; set; }

		public float MaxDamageArcane { get; set; }
		public float MinDamageArcane { get; set; }
		public float ArcaneSkillDamagePercentBonus { get; set; }

		public float MaxDamageHoly { get; set; }
		public float MinDamageHoly { get; set; }
		public float HolySkillDamagePercentBonus { get; set; }
		

		public ItemProperties() { }
		public ItemProperties(ItemStats thesestats)
		{
			MaxDamageHoly = thesestats.MaxDamageHoly;
			MinDamageHoly = thesestats.MinDamageHoly;
			HolySkillDamagePercentBonus = thesestats.HolySkillDamagePercentBonus;

			MaxDamageArcane = thesestats.MaxDamageArcane;
			MinDamageArcane = thesestats.MinDamageArcane;
			ArcaneSkillDamagePercentBonus = thesestats.ArcaneSkillDamagePercentBonus;

			MaxDamagePoison = thesestats.MaxDamagePoison;
			MinDamagePoison = thesestats.MinDamagePoison;
			PosionSkillDamagePercentBonus = thesestats.PosionSkillDamagePercentBonus;

			MaxDamageCold = thesestats.MaxDamageCold;
			MinDamageCold = thesestats.MinDamageCold;
			ColdSkillDamagePercentBonus = thesestats.ColdSkillDamagePercentBonus;

			MaxDamageLightning = thesestats.MaxDamageLightning;
			MinDamageLightning = thesestats.MinDamageLightning;
			LightningSkillDamagePercentBonus = thesestats.LightningSkillDamagePercentBonus;

			MaxDamageFire = thesestats.MaxDamageFire;
			MinDamageFire = thesestats.MinDamageFire;
			FireSkillDamagePercentBonus = thesestats.FireSkillDamagePercentBonus;


			WeaponOnHitFearProcChance = thesestats.WeaponOnHitFearProcChance;
			WeaponOnHitBlindProcChance = thesestats.WeaponOnHitBlindProcChance;
			WeaponOnHitFreezeProcChance = thesestats.WeaponOnHitFreezeProcChance;
			WeaponOnHitChillProcChance = thesestats.WeaponOnHitChillProcChance;
			WeaponOnHitImmobilizeProcChance = thesestats.WeaponOnHitImmobilizeProcChance;
			WeaponOnHitKnockbackProcChance = thesestats.WeaponOnHitKnockbackProcChance;
			WeaponOnHitSlowProcChance = thesestats.WeaponOnHitSlowProcChance;
			WeaponOnHitBleedProcChance = thesestats.WeaponOnHitBleedProcChance;

			DamagePercentBonusVsElites = thesestats.DamagePercentBonusVsElites;
			DamagePercentReductionFromElites = thesestats.DamagePercentReductionFromElites;
			DamageReductionPhysicalPercent = thesestats.DamageReductionPhysicalPercent;

			PowerCooldownReductionPercent=thesestats.PowerCooldownReductionPercent;
			ResourceCostReductionPercent = thesestats.ResourceCostReductionPercent;
			SkillDamagePercentBonus=thesestats.SkillDamagePercentBonus;
			OnHitAreaDamageProcChance = thesestats.OnHitAreaDamageProcChance;

			ArmorBonus = thesestats.ArmorBonus;
			Armor=thesestats.Armor;
			ArmorTotal=thesestats.ArmorTotal;

			Level = thesestats.Level;
			ItemLevelRequirementReduction = thesestats.ItemLevelRequirementReduction;

			ArcaneOnCrit = thesestats.ArcaneOnCrit;

			AttackSpeedPercent = thesestats.AttackSpeedPercent;
			AttackSpeedPercentBonus = thesestats.AttackSpeedPercentBonus;

			BlockChance = thesestats.BlockChance;
			BlockChanceBonus = thesestats.BlockChanceBonus;

			CritPercent = thesestats.CritPercent;
			CritDamagePercent = thesestats.CritDamagePercent;
			Dexterity = thesestats.Dexterity;
			ExperienceBonus = thesestats.ExperienceBonus;
			Intelligence = thesestats.Intelligence;

			LifePercent = thesestats.LifePercent;
			LifeOnHit = thesestats.LifeOnHit;
			LifeSteal = thesestats.LifeSteal;
			LifeOnKill=thesestats.LifeOnKill;
			HealthPerSpiritSpent = thesestats.HealthPerSpiritSpent;

			HealthPerSecond = thesestats.HealthPerSecond;
			MagicFind = thesestats.MagicFind;
			GoldFind = thesestats.GoldFind;
			GlobeBonus = thesestats.HealthGlobeBonus;
			MovementSpeed = thesestats.MovementSpeed;
			PickUpRadius = thesestats.PickUpRadius;

			ResistAll = thesestats.ResistAll;
			ResistArcane = thesestats.ResistArcane;
			ResistCold = thesestats.ResistCold;
			ResistFire = thesestats.ResistFire;
			ResistHoly = thesestats.ResistHoly;
			ResistLightning = thesestats.ResistLightning;
			ResistPhysical = thesestats.ResistPhysical;
			ResistPoison = thesestats.ResistPoison;


			MinDamage = thesestats.MinDamage;
			MaxDamage = thesestats.MaxDamage;
			MinDamageElemental = thesestats.MinDamageElemental;
			MaxDamageElemental = thesestats.MaxDamageElemental;

			MaxDiscipline = thesestats.MaxDiscipline;
			MaxMana = thesestats.MaxMana;
			MaxArcanePower = thesestats.MaxArcanePower;
			MaxFury = thesestats.MaxFury;
			MaxSpirit = thesestats.MaxSpirit;

			ManaRegen = thesestats.ManaRegen;
			HatredRegen = thesestats.HatredRegen;

			Thorns = thesestats.Thorns;

			Sockets = thesestats.Sockets;
			SpiritRegen = thesestats.SpiritRegen;
			Strength = thesestats.Strength;
			Vitality = thesestats.Vitality;
			WeaponDamagePerSecond = thesestats.WeaponDamagePerSecond;
			WeaponAttacksPerSecond = thesestats.WeaponAttacksPerSecond;
			WeaponMaxDamage=thesestats.WeaponMaxDamage;
			WeaponMinDamage = thesestats.WeaponMinDamage;
			WeaponDamagePercent = thesestats.WeaponDamagePercent;
		}

		public List<string> GetPrimaryStatStrings()
		{
			List<string> retList = new List<string>();
			if (Dexterity > 0) retList.Add(String.Format("+{0} Dexterity", Dexterity));
			if (Intelligence > 0) retList.Add(String.Format("+{0} Intelligence", Intelligence));
			if (Strength > 0) retList.Add(String.Format("+{0} Strength", Strength));
			if (Vitality > 0) retList.Add(String.Format("+{0} Vitality", Vitality));
			if (ResistAll > 0) retList.Add(String.Format("+{0} Resistance to All Elements", ResistAll));
			if (LifePercent > 0) retList.Add(String.Format("+{0}% Life", LifePercent));
			if (LifeOnHit > 0) retList.Add(String.Format("+{0} Life per Hit", LifeOnHit));
			if (LifeSteal > 0) retList.Add(String.Format("Life Steal {0}%", LifeSteal));
			if (HealthPerSpiritSpent > 0) retList.Add(String.Format("Health Per Spirit Spent {0}", HealthPerSpiritSpent));
			if (HealthPerSecond > 0) retList.Add(String.Format("Regenerates {0} Life per Second", HealthPerSecond));
			if (MovementSpeed > 0) retList.Add(String.Format("+{0}% Movement Speed", MovementSpeed));
			if (Sockets > 0) retList.Add(String.Format("{0} Sockets", Sockets));
			if (CritPercent > 0) retList.Add(String.Format("Critical Hit Chance Increased by {0}%", CritPercent));
			if (CritDamagePercent > 0) retList.Add(String.Format("Critical Hit Damage Increased by {0}%", CritDamagePercent));

			if (MinDamage > 0)
			{
				//if (MaxDamage > 0) retList.Add(String.Format("MaxDamage {0}", MaxDamage));
				retList.Add(String.Format("+{0}-{1} Damage", MinDamage, MaxDamage));
			}
			else if (MinDamageFire > 0)
			{
				retList.Add(String.Format("+{0}-{1} Fire Damage", MinDamageFire, MaxDamageFire));
			}
			else if (MinDamageLightning > 0)
			{
				retList.Add(String.Format("+{0}-{1} Lightning Damage", MinDamageLightning, MaxDamageLightning));
			}
			else if (MinDamageCold > 0)
			{
				retList.Add(String.Format("+{0}-{1} Cold Damage", MinDamageCold, MaxDamageCold));
			}
			else if (MinDamagePoison > 0)
			{
				retList.Add(String.Format("+{0}-{1} Poison Damage", MinDamagePoison, MaxDamagePoison));
			}
			else if (MinDamageArcane > 0)
			{
				retList.Add(String.Format("+{0}-{1} Arcane Damage", MinDamageArcane, MaxDamageArcane));
			}
			else if (MinDamageHoly > 0)
			{
				retList.Add(String.Format("+{0}-{1} Holy Damage", MinDamageHoly, MaxDamageHoly));
			}


			if (AttackSpeedPercent > 0) retList.Add(String.Format("Attack Speed Increased by {0}%", AttackSpeedPercent));
			if (AttackSpeedPercentBonus > 0) retList.Add(String.Format("Increases Attack Speed by {0}%", AttackSpeedPercentBonus));
			if (BlockChanceBonus > 0) retList.Add(String.Format("+{0}% Chance to Block", BlockChanceBonus));
			if (OnHitAreaDamageProcChance > 0) retList.Add(String.Format("Chance to Deal {0}% Area Damage on Hit", OnHitAreaDamageProcChance));
			if (PowerCooldownReductionPercent > 0) retList.Add(String.Format("Reduces cooldown of all skills by {0}%", PowerCooldownReductionPercent));
			if (ResourceCostReductionPercent > 0) retList.Add(String.Format("ResourceCostReductionPercent {0}", ResourceCostReductionPercent));
			//


			if (ArcaneOnCrit > 0) retList.Add(String.Format("Critical hits grant {0} Arcane Power (Wizard Only)", ArcaneOnCrit));
			if (ManaRegen > 0) retList.Add(String.Format("Increases Mana Regeneration by {0} per Second", ManaRegen));
			if (SpiritRegen > 0) retList.Add(String.Format("Increases Spirit Regeneration by {0} per Second", SpiritRegen));
			if (HatredRegen > 0) retList.Add(String.Format("Increases Hatred Regeneration by {0} per second", HatredRegen));
			if (ArmorBonus > 0) retList.Add(String.Format("+{0} Armor", ArmorBonus));
			if (WeaponDamagePercent > 0) retList.Add(String.Format("WeaponDamagePercent {0}", WeaponDamagePercent));


			return retList;
		}
		public List<string> GetSecondaryStatStrings()
		{
			List<string> retList = new List<string>();
			if (MaxDiscipline > 0) retList.Add(String.Format("+{0} Maximum Discipline", MaxDiscipline));
			if (MaxMana > 0) retList.Add(String.Format("+{0} Maximum Mana", MaxMana));
			if (MaxArcanePower > 0) retList.Add(String.Format("+{0} Maximum Arcane Power", MaxArcanePower));
			if (MaxFury > 0) retList.Add(String.Format("+{0} Maximum Fury", MaxFury));
			if (MaxSpirit > 0) retList.Add(String.Format("+{0} Maximum Spirit", MaxSpirit));

			if (ResistArcane > 0) retList.Add(String.Format("+{0} Arcane Resistance", ResistArcane));
			if (ResistCold > 0) retList.Add(String.Format("+{0} Cold Resistance", ResistCold));
			if (ResistFire > 0) retList.Add(String.Format("+{0} Fire Resistance", ResistFire));
			if (ResistHoly > 0) retList.Add(String.Format("+{0} Holy Resistance", ResistHoly));
			if (ResistLightning > 0) retList.Add(String.Format("+{0} Lightning Resistance", ResistLightning));
			if (ResistPhysical > 0) retList.Add(String.Format("+{0} Physical Resistance", ResistPhysical));
			if (ResistPoison > 0) retList.Add(String.Format("+{0} Poison Resistance", ResistPoison));

			if (ExperienceBonus > 0) retList.Add(String.Format("Monster kills grant +{0} Experience", ExperienceBonus));
			if (GlobeBonus > 0) retList.Add(String.Format("Health Globes and Potions Grant +{0} Life", GlobeBonus));
			if (ItemLevelRequirementReduction > 0) retList.Add(String.Format("Level Requirment Reduced by {0}", ItemLevelRequirementReduction));
			if (MagicFind > 0) retList.Add(String.Format("MagicFind {0}", MagicFind));
			if (GoldFind > 0) retList.Add(String.Format("+{0}% Extra Gold from Monsters", GoldFind));
			if (Thorns > 0) retList.Add(String.Format("Ranged and Melee Attackers take {0} damage per hit.", Thorns));
			if (PickUpRadius > 0) retList.Add(String.Format("Increases Gold and Health Pickup by {0} Yards", PickUpRadius));
			if (LifeOnKill > 0) retList.Add(String.Format("+{0} Life after Each Kill", LifeOnKill));

			if (WeaponOnHitFearProcChance > 0) retList.Add(String.Format("{0}% Chance to Fear on Hit", WeaponOnHitFearProcChance));
			if (WeaponOnHitBlindProcChance > 0) retList.Add(String.Format("{0}% Chance to Blind on Hit", WeaponOnHitBlindProcChance));
			if (WeaponOnHitFreezeProcChance > 0) retList.Add(String.Format("{0}% Chance to Freeze on Hit", WeaponOnHitFreezeProcChance));
			if (WeaponOnHitChillProcChance > 0) retList.Add(String.Format("{0}% Chance to Chill on Hit", WeaponOnHitChillProcChance));
			if (WeaponOnHitImmobilizeProcChance > 0) retList.Add(String.Format("{0}% Chance to Immobilize on Hit", WeaponOnHitImmobilizeProcChance));
			if (WeaponOnHitKnockbackProcChance > 0) retList.Add(String.Format("{0}% Chance to Knockback on Hit", WeaponOnHitKnockbackProcChance));
			if (WeaponOnHitSlowProcChance > 0) retList.Add(String.Format("{0}% Chance to Slow on Hit", WeaponOnHitSlowProcChance));
			if (WeaponOnHitBleedProcChance > 0) retList.Add(String.Format("WeaponOnHitBleedProcChance", WeaponOnHitBleedProcChance));

			return retList;
		}
		public string ReturnItemMainStatString()
		{
			string retString = String.Empty;
			if (Armor > 0)
			{
				retString = String.Format("Armor {0}", ArmorTotal);

				if (BlockChance > 0)
					retString = retString + String.Format("   Block Chance {0}", BlockChance);

			}
			else if (WeaponDamagePerSecond > 0)
			{
				retString = String.Format("DPS {0}", WeaponDamagePerSecond);
				if (WeaponAttacksPerSecond > 0)
					retString = retString + String.Format("   Attack Speed {0}", WeaponAttacksPerSecond);
			}

			return retString;
		}
	}
}
