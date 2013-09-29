using System;

namespace FunkyTrinity.AbilityFunky
{
	 [Flags]
	 public enum AbilityUseage
	 {
			Anywhere=1,
			OutOfCombat=2,
			Combat=4,
	 }
}