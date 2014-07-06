using System;

namespace fItemPlugin.Townrun
{
	[Flags]
	public enum BloodShardGambleItems
	{
		None = 0,
		OneHandItem = 1,
		TwoHandItem = 2,
		Quiver = 4,
		Orb = 8,
		Mojo = 16,
		Helm = 32,
		Gloves = 64,
		Boots = 128,
		Chest = 256,
		Belt = 512,
		Shoulders = 1024,
		Pants = 2048,
		Bracers = 4096,
		Shield = 8192,
		Ring = 16384,
		Amulet = 32768,

		All = OneHandItem | TwoHandItem | Quiver | Orb | Mojo | Helm | Gloves | Boots | Chest | Belt | Shoulders | Pants | Bracers | Shield | Ring | Amulet
	}
}