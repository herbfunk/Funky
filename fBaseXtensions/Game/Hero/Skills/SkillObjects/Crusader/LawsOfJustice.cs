using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Crusader
{
	public class LawsOfJustice : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_LawsOfJustice2; }
		}

		public override double Cooldown { get { return 45000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			
			Range = 8;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 20, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			ClusterConditions.Add(new SkillClusterConditions(10d, 25, 10, false));
			FcriteriaCombat = (u) => FunkyGame.Hero.dCurrentHealthPct < 0.75d;
		}
	}
}
