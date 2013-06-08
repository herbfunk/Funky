using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunkyTrinity
{
	 internal partial class PointComparer : IEqualityComparer<Funky.GridPoint>
	 {

		  public bool Equals(Funky.GridPoint x, Funky.GridPoint y)
		  {
				double l_d=Funky.GridPoint.GetDistanceBetweenPoints(x, y);

				if (l_d==0&&(x.X==y.X)&&(x.Y==y.Y))
					 return true;
				else
					 return false;

		  }  // of Equals

		  public int GetHashCode(Funky.GridPoint obj)
		  {
				return 0;
		  }  // of GetHashCode()

	 }  // of partial class PointComparer
}