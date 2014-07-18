using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Wizard
{
	public class Meteor : Skill
	{
		public override double Cooldown { get { return 1000; } }

		public override bool IsRanged { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 2, true);
			Cost = 50;
			Range = 50;

			
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);
			ClusterConditions.Add(new SkillClusterConditions(4d, 50f, 2, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal | TargetProperties.Fast));
			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Meteor; }
		}
	}
}
