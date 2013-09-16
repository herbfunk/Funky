using System;

namespace FunkyTrinity.ability
{
	 [Flags]
	 public enum AbilityUseFlags
	 {
			Anywhere=1,
			OutOfCombat=2,
			Combat=4,
	 }
}