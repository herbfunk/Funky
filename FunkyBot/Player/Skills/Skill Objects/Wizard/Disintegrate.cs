using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Wizard
{
	public class Disintegrate : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 20;
			Range = 35;

			
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);
			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Disintegrate; }
		}
	}
}
