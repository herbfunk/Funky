using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	public class BlackHole : Skill
	{
		public override double Cooldown { get { return 20000; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 2, true);
			Cost = 20;
			Range = 50;

			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);

			ClusterConditions.Add(new SkillClusterConditions(7d, 50f, 5, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, maxdistance: 40, MinimumHealthPercent: 0.95d));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 40, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal | TargetProperties.LowHealth));
		}

		public override SNOPower Power
		{
			get { return SNOPower.X1_Wizard_Wormhole; }
		}
	}
}
