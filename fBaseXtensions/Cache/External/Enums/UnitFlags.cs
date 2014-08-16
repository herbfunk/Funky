using System;

namespace fBaseXtensions.Cache.External.Enums
{
	[Flags]
	public enum UnitFlags
	{
		None = 0,
		Normal = 1,
		CorruptGrowth = 2,
		Unique = 4,
		Boss = 8,
		TreasureGoblin = 16,
		Ranged = 32,
		Fast = 64,
		Stealthable = 128,
		Burrowing = 256,
		Summoner = 512,
		SucideBomber = 1024,
		Grotesque = 2048,
		ReflectiveMissle = 4096,
		Flying = 8192,
		AdventureModeBoss = 16384,
		AvoidanceSummoner = 32768,
		MalletLord=65536,
		Debuffing=131072,
		Worm=262144,
		Revivable=524288,

	}
}
