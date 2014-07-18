using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Crusader
{
	public class Phalanx : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.x1_Crusader_Phalanx3; }
		}

		
		public override double Cooldown { get { return 1000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location; } }
		
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 49;
			Cost = 30;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45, MinimumHealthPercent: 0.95d));
		}
	}
}
