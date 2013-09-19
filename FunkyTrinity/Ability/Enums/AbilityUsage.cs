using System;

namespace FunkyTrinity.Ability
{
	 [Flags]
	 public enum AbilityUseage
	 {
			Anywhere=1,
			OutOfCombat=2,
			Combat=4,
	 }
}