﻿using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	public class AkaratsChampion : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.None; }
		}

		public override void Initialize()
		{
			Cooldown = 120000;
			Range = 8;
			Priority = AbilityPriority.None;
			ExecutionType = AbilityExecuteFlags.Target;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.None);
			UseageType = AbilityUseage.Combat;
		}
	}
}
