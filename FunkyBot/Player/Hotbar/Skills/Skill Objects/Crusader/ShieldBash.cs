﻿using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class ShieldBash : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_ShieldBash2; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 25;
			Cost = 30;
			Priority = AbilityPriority.Low;
			ExecutionType = AbilityExecuteFlags.Target|AbilityExecuteFlags.ClusterTarget;
			IsRanged = true;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.CheckEnergy);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 25, 0.75d));
			ClusterConditions.Add(new SkillClusterConditions(5d, 25f, 2, true));
			UseageType = AbilityUseage.Combat;
		}
	}
}
