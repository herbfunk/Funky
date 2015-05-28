using System;

namespace fBaseXtensions.Cache.Internal.Enums
{
	[Flags]
	public enum GizmoTargetTypes
	{
		None=0,

		Chest=1,
		Corpse=2,
		Resplendant=4,
		SpecialDestructible=8,
		MiscContainer=16,

		Shrine=32,
		Healthwell=64,
		PoolOfReflection=128,

		Cursed=256,

		FloorContainer=512,

		ArmorRack=1024,
		WeaponRack=2048,
        PylonShrine = 4096,

        Obstacle = 8192,

        Bounty = 16384,

        Containers=Chest|Corpse|Resplendant|MiscContainer|FloorContainer|ArmorRack|WeaponRack,


	}
}
