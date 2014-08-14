using System;

namespace fBaseXtensions.Cache.External.Objects
{
	[Flags]
	public enum UnitFlags
	{
		None = 0,
		Normal = 1,
		Rare = 2,
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
		AdventureModeBoss = 16348,
	}
}
