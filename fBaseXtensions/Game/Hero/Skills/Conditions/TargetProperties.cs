using System;

namespace fBaseXtensions.Game.Hero.Skills.Conditions
{
	 ///<summary>
	 ///Flags that are used to test TargetConditions.
	 ///</summary>
	 [Flags]
	 public enum TargetProperties
	 {
			None=0,
			MissileReflecting=1,
			MissileDampening=2,
			Shielding=4,
			Boss=8,
			RareElite=16,
			Unique=32,
			TreasureGoblin=64,
			Stealthable=128,
			Burrowing=256,
			SucideBomber=512,
			Weak=1024,
			FullHealth=2048,
            Flying = 4096,
			Ranged=8192,
			TargetableAndAttackable=16384,
			Fast=32768,
			DOTDPS=65536,
		    CloseDistance=131072,
			ReflectsDamage=262144,
            Electrified = 524288,
		    Normal = 1048576,
			LowHealth=2097152,

            Summoner = 4194304,
            AvoidanceSummoner = 8388608,
            Debuffing = 16777216,

	 }

     
}