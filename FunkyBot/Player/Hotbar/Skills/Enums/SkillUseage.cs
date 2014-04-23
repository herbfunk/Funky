using System;

namespace FunkyBot.Player.HotBar.Skills
{
	 [Flags]
	 public enum SkillUseage
	 {
			Anywhere=1,
			OutOfCombat=2,
			Combat=4,
	 }
}