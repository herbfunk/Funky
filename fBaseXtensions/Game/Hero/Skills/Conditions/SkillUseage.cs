using System;

namespace fBaseXtensions.Game.Hero.Skills.Conditions
{
	 [Flags]
	 public enum SkillUseage
	 {
			Anywhere=1,
			OutOfCombat=2,
			Combat=4,
	 }
}