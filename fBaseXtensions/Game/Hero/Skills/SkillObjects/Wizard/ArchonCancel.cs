using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
{
	public class CancelArchonBuff : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon; }
		}

		public override double Cooldown { get { return 5; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.RemoveBuff; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(3, 3, true);
			Priority = SkillPriority.None;
			UseageType = SkillUseage.OutOfCombat;
			PreCast = new SkillPreCast(SkillPrecastFlags.None);

			//Important!! We have to override the default return of true.. we dont want this to fire as a combat Ability.
			FcriteriaCombat = () => false;
		}
	}
}
