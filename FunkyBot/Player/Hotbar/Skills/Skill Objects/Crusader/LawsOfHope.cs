﻿using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class LawsOfHope : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_LawsOfHope2; }
		}

		public override void Initialize()
		{
			Cooldown = 45000;
			Range = 8;
			Priority = SkillPriority.High;
			ExecutionType = SkillExecutionFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 20, 0.95d, TargetProperties.Normal));
			ClusterConditions.Add(new SkillClusterConditions(10d, 25, 10, false));
			FcriteriaCombat = () => Bot.Character.Data.dCurrentHealthPct < 0.75d;
		}
	}
}