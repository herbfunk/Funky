using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class Gargantuan : Skill
	{
		public override double Cooldown { get { return 25000; } }

		public override bool IsBuff { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(2, 1, true);
			Counter = 1;

			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			FcriteriaBuff = () => RuneIndex != 0 && Bot.Character.Data.PetData.Gargantuan == 0;

			FcriteriaCombat = () => (RuneIndex == 0 &&
								   (Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15] >= 1 ||
									(Bot.Targeting.Cache.CurrentUnitTarget.IsEliteRareUnique && Bot.Targeting.Cache.CurrentTarget.RadiusDistance <= 15f))
								   || RuneIndex != 0 && Bot.Character.Data.PetData.Gargantuan == 0);
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Gargantuan; }
		}
	}
}
