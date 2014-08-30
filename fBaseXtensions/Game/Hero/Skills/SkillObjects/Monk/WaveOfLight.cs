using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class WaveOfLight : Skill
	{
		public override double Cooldown { get { return 250; } }

		public override bool IsBuff { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return _executiontype; } set { _executiontype = value; } }
		private SkillExecutionFlags _executiontype = SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location;

		public override void Initialize()
		{

			if (RuneIndex == 1)
				ExecutionType = SkillExecutionFlags.Self;

			WaitVars = new WaitLoops(2, 4, true);
			Cost = RuneIndex == 3 ? 40 : 75;
			Range = 16;
			Priority = SkillPriority.Medium;


			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckCanCast |
									  SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckPlayerIncapacitated));
			ClusterConditions.Add(new SkillClusterConditions(6d, 35f, 3, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 30, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal | TargetProperties.MissileReflecting));
			FcriteriaCombat = () => !FunkyGame.Hero.Class.bWaitingForSpecial;

			FcriteriaBuff = () => FunkyGame.Hero.dCurrentHealthPct < 0.25d;


		}


		public override SNOPower Power
		{
			get { return SNOPower.Monk_WaveOfLight; }
		}
	}
}
