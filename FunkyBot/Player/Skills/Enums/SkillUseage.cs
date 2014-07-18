using System;

namespace FunkyBot.Skills
{
	 [Flags]
	 public enum SkillUseage
	 {
			Anywhere=1,
			OutOfCombat=2,
			Combat=4,
	 }
}