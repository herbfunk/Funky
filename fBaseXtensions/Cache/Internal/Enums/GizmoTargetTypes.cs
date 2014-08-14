using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fBaseXtensions.Cache.Internal.Enums
{
	[Flags]
	public enum GizmoTargetTypes
	{
		None=0,

		Chest=1,
		Corpse=2,
		Resplendant=4,
		ItemRack=8,
		MiscContainer=16,

		Shrine=32,
		Healthwell=64,
		PoolOfReflection=128,

		Cursed=256,

	}
}
