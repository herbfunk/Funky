using System;
using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class BigBadVoodoo : Skill
	{
		public override double Cooldown { get { return 120000; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Self; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 0, true);
			
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast));

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, MinimumHealthPercent: 0.95d));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 25, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			//Reduced CD Dagger!
			if (Bot.Settings.WitchDoctor.StarmetalKukri)
			{
				ClusterConditions.Add(new SkillClusterConditions(9d, 50f, 5, true, clusterflags: ClusterProperties.Strong));
				ClusterConditions.Add(new SkillClusterConditions(9d, 50f, 10, false));
			}
		}

		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_BigBadVoodoo; }
		}
	}
}
