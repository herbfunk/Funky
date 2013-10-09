using System;

namespace FunkyBot.AbilityFunky
{
	 [Flags]
	 public enum AbilityUseage
	 {
			Anywhere=1,
			OutOfCombat=2,
			Combat=4,
	 }
}