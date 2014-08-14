using fBaseXtensions.Game.Hero;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
{
	public class SummonZombieDogs : Skill
	{
		public override double Cooldown { get { return 25000; } }

		public override bool IsBuff { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }


		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 0, true);

			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			FcriteriaBuff = () => FunkyGame.Targeting.Cache.Environment.HeroPets.ZombieDogs < GetTotalZombieDogsSummonable();
			FcriteriaCombat = () => FunkyGame.Targeting.Cache.Environment.HeroPets.ZombieDogs < GetTotalZombieDogsSummonable();
		}

		private int GetTotalZombieDogsSummonable()
		{
			int total = 3;

			if (Hotbar.PassivePowers.Contains(SNOPower.Witchdoctor_Passive_ZombieHandler))
				total++;
			if (Hotbar.PassivePowers.Contains(SNOPower.Witchdoctor_Passive_MidnightFeast))
				total++;
			if (Hotbar.PassivePowers.Contains(SNOPower.Witchdoctor_Passive_FierceLoyalty))
				total++;

			//Tall Man Finger - Only 1
			if (FunkyBaseExtension.Settings.WitchDoctor.TallManFinger)
				total = 1;

			return total;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_SummonZombieDog; }
		}
	}
}
