using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Barb
{
	public class Overpower : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_Overpower; } }


		public override double Cooldown { get { return 200; } }

		private readonly WaitLoops _waitVars = new WaitLoops(4, 4, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Self; } }

		public override SkillUseage UseageType { get { return SkillUseage.Anywhere; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckEnergy |
									  SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 10,
				falseConditionalFlags: TargetProperties.Fast));
			ClusterConditions.Add(new SkillClusterConditions(5d, 7, 2, false));
			FcriteriaCombat = () => true;
		}

	}
}
