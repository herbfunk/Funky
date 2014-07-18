using FunkyBot.Movement.Clustering;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Barb
{
	public class Avalanche : Skill
	{
		public override SNOPower Power { get { return SNOPower.X1_Barbarian_Avalanche_v2; } }


		public override double Cooldown { get { return 30000; } }


		private readonly WaitLoops _waitVars = new WaitLoops(4, 4, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location|SkillExecutionFlags.ClusterLocation; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }
		
		public override void Initialize()
		{
			Priority = SkillPriority.High;
			Range = 50;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));

			//SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.IsSpecial, 50, 0.50d, falseConditionalFlags: TargetProperties.Fast));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, maxdistance: 40, MinimumHealthPercent: 0.75d));

			ClusterConditions.Add(new SkillClusterConditions(6d, 40, 8, false));
			ClusterConditions.Add(new SkillClusterConditions(6d, 40, 2, false, clusterflags: ClusterProperties.Elites));
		}

	}
}
