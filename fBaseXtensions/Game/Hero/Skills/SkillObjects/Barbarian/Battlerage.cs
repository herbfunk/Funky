using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class Battlerage : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_BattleRage; } }


		public override double Cooldown { get { return 120000; } }


		public override bool IsBuff { get { return true; } }

		private readonly WaitLoops _waitVars = new WaitLoops(1, 1, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override SkillUseage UseageType { get { return SkillUseage.Anywhere; } }

		public override void Initialize()
		{
			Priority = SkillPriority.High;
			Cost = 20;
			Range = 35;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast));
			FcriteriaBuff = () => !Hotbar.HasBuff(SNOPower.Barbarian_BattleRage);
			FcriteriaCombat = () => !Hotbar.HasBuff(SNOPower.Barbarian_BattleRage) ||
				//Only if we cannot spam sprint..
								  (!Hotbar.HasPower(SNOPower.Barbarian_Sprint) &&
								   ((FunkyBaseExtension.Settings.Barbarian.bFuryDumpWrath && FunkyGame.Hero.dCurrentEnergyPct >= 0.98 &&
									 Hotbar.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker)
									 && FunkyGame.Hero.dCurrentHealthPct > 0.50d) ||
									(FunkyBaseExtension.Settings.Barbarian.bFuryDumpAlways && FunkyGame.Hero.dCurrentEnergyPct >= 0.98 && FunkyGame.Hero.dCurrentHealthPct > 0.50d)));
		}

	}
}
