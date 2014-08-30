using System;

namespace fBaseXtensions.Cache.Internal.Enums
{
	[Flags]
	public enum DebugDataTypes
	{
		None=0,
		Items=1,
		Doors=2,
		Containers=4,
		Destructibles=8,
		Barricades=16,
		Units=32,
	}
}
