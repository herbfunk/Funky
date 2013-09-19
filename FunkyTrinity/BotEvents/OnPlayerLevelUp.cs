using System;
using Zeta;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal static DateTime LastLevelUp=DateTime.MinValue;

		  public void OnPlayerLevelUp(object sender, EventArgs e)
		  {
				LeveledUpEventFired=true;
		  }
    }
}