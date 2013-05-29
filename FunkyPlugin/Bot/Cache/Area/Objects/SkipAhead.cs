﻿using System;
using Zeta;
using Zeta.Common;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  // A list of small areas covering zones we move through while fighting to help our custom move-handler skip ahead waypoints
		  public class SkipAheadNavigation
		  {
				public Vector3 Position { get; set; }
				public float Radius { get; set; }

				public SkipAheadNavigation(Vector3 pos, float radius)
				{
					 this.Position=pos;
					 this.Radius=radius;
				}
		  }
    }
}