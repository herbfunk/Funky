using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter
{
	public class MarkedForDeath : Skill
	{
		public override double Cooldown { get { return 10000; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 3;
			SecondaryEnergy = true;
			Range = 40;

			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckEnergy |
									  SkillPrecastFlags.CheckRecastTimer));
			ClusterConditions.Add(new SkillClusterConditions(4d, 35f, 2, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.RareElite, maxdistance: 40));

		}


		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_MarkedForDeath; }
		}
	}
}
