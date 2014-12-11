using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
{
	public class Horrify : Skill
	{
		public override double Cooldown { get { return 16200; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Self; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 37;

			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast |
									  SkillPrecastFlags.CheckEnergy));

			FcriteriaCombat = (u) =>
			{
				//maintain armor buff..
				if (RuneIndex == 0 && LastUsedMilliseconds > 7800)
					return true;

				return FunkyGame.Hero.dCurrentHealthPct <= 0.60;
			};
		}

		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Horrify; }
		}
	}
}
