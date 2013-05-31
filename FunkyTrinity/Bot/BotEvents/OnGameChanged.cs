using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;
using System.Windows;
using System.Windows.Controls;

namespace FunkyTrinity
{
    public partial class Funky
    {
		  private static void FunkyOnGameChanged(object sender, EventArgs e)
		  {
				ResetGame();
		  }
    }
}