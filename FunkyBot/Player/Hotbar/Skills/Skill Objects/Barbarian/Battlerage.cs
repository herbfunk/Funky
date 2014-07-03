using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class Battlerage : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_BattleRage; } }


		public override double Cooldown { get { return 118000; } }


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
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast));
			FcriteriaBuff = () => !Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_BattleRage);
			FcriteriaCombat = () => !Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_BattleRage) ||
				//Only if we cannot spam sprint..
								  (!Bot.Character.Class.HotBar.HasPower(SNOPower.Barbarian_Sprint) &&
								   ((Bot.Settings.Barbarian.bFuryDumpWrath && Bot.Character.Data.dCurrentEnergyPct >= 0.98 &&
									 Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker)
									 && Bot.Character.Data.dCurrentHealthPct > 0.50d) ||
									(Bot.Settings.Barbarian.bFuryDumpAlways && Bot.Character.Data.dCurrentEnergyPct >= 0.98 && Bot.Character.Data.dCurrentHealthPct > 0.50d)));
		}

	}
}
