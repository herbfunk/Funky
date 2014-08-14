using System;

namespace fBaseXtensions.Navigation.Gridpoint
{
	[Flags]
	public enum LocationFlags
	{
		None = 0,
		Right = 1,
		Left = 2,
		Bottom = 3,
		Top = 4,

		BottomLeft = Bottom | Left,
		BottomRight = Bottom | Right,

		TopLeft = Top | Left,
		TopRight = Top | Right,
	}
}