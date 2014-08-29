using System;
using System.Globalization;

namespace fItemPlugin.Townrun
{
	public class Delayer
	{
		public Delayer()
		{
			Reset();
		}

		public bool Test(double multiplier = 1)
		{
			iCurrentLoops++;
			if (iCurrentLoops < DelayLoopLimit * multiplier) return false;

			Reset();
			return true;
		}
		public void Reset()
		{
			iCurrentLoops = 0;
			RandomizeTheTimer();
		}

		private int DelayLoopLimit;
		private int iCurrentLoops;

		private void RandomizeTheTimer()
		{
			Random rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
			int rnd = rndNum.Next(5);
			DelayLoopLimit = 2 + rnd;
		}
	}
}
