using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class WaveOfLight : Skill
	{
		public override double Cooldown { get { return 250; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return _executiontype; } set { _executiontype = value; } }
		private SkillExecutionFlags _executiontype = SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location;

		public override void Initialize()
		{

			//if (RuneIndex == 1)
			//	ExecutionType = SkillExecutionFlags.Self;

			WaitVars = new WaitLoops(2, 4, true);
			Cost = RuneIndex == 3 ? 40 : 75;
			Range = 16;
			Priority = SkillPriority.Medium;

			if (Equipment.CheckLegendaryItemCount(LegendaryItemTypes.TzoKrinsGaze))
			{
				_executiontype = SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target;
				Range = 49;
			}


			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckCanCast |
									  SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckPlayerIncapacitated));
			ClusterConditions.Add(new SkillClusterConditions(6d, Range, 3, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal | TargetProperties.MissileReflecting));
			FcriteriaCombat = () => !FunkyGame.Hero.Class.bWaitingForSpecial;


		}


		public override SNOPower Power
		{
			get { return SNOPower.Monk_WaveOfLight; }
		}
	}
}
