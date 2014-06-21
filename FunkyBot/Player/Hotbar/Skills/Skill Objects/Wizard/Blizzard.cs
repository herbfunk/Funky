using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	public class Blizzard : Skill
	{
		public override double Cooldown { get { return 2500; } }

		public override bool IsRanged { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 2, true);
			Cost = 40;
			Range = 50;

		
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			ClusterConditions.Add(new SkillClusterConditions(5d, 50f, 2, true));
			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}

		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Blizzard; }
		}
	}
}
