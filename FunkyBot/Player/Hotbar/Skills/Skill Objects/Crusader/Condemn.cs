﻿using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Condemn : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Condemn; }
		}

		public override void Initialize()
		{
			Cooldown = 15000;
			Range = 15;
			Priority = AbilityPriority.High;
			ExecutionType = AbilityExecuteFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.None);
			UseageType = AbilityUseage.Combat;
		}
	}
}
