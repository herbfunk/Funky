namespace FunkyBot.Movement
{
	public class AreaBoundary
	{
		public AreaBoundary(GridPoint TopLeft, GridPoint BottomRight)
		{
			tl=TopLeft;
			br=BottomRight;
		}
		public AreaBoundary(GridPoint Center)
		{
			tl=Center;
			br=Center;
		}

		private readonly GridPoint tl, br;

		public LocationFlags ComputeOutCode(double x, double y)
		{
			LocationFlags code=LocationFlags.None;
			if (x<tl.X)           // to the left of center
				code|=LocationFlags.Left;
			else if (x>br.X)      // to the right of center
				code|=LocationFlags.Right;
			if (y>br.Y)           // below the center
				code|=LocationFlags.Bottom;
			else if (y<tl.Y)      // above the center
				code|=LocationFlags.Top;
			return code;
		}
	}
}