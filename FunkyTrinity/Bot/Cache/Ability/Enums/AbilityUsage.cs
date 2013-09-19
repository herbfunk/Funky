using System;

namespace FunkyTrinity.ability
{
	 [Flags]
	 public enum AbilityUseage
	 {
			Anywhere=1,
			OutOfCombat=2,
			Combat=4,
	 }
}