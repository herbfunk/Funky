using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class Sprint : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_Sprint; } }


		public override double Cooldown { get { return 2900; } }

		private readonly WaitLoops _waitVars = new WaitLoops(1, 1, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override bool IsMovementSkill { get { return true; } }
		public override bool IsBuff { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }
		
		public override SkillUseage UseageType { get { return SkillUseage.Anywhere; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;

			Cost = 20;

			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckCanCast |
									  SkillPrecastFlags.CheckPlayerIncapacitated));


			FcriteriaBuff = () => Bot.Settings.General.OutOfCombatMovement && !Bot.Character.Class.HotBar.HasBuff(Power);

			FcriteriaCombat = () => (!Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_Sprint) && Bot.Settings.General.OutOfCombatMovement) ||
								  (((Bot.Settings.Barbarian.bFuryDumpWrath && Bot.Character.Data.dCurrentEnergyPct >= 0.95 &&
									 Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker)) ||
									(Bot.Settings.Barbarian.bFuryDumpAlways && Bot.Character.Data.dCurrentEnergyPct >= 0.95) ||
									((Bot.Character.Class.Abilities[SNOPower.Barbarian_Sprint].AbilityUseTimer() && !Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_Sprint)) &&
				// Always keep up if we are whirlwinding, or if the target is a goblin
									 (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_Whirlwind) ||
									  Bot.Targeting.Cache.CurrentTarget.IsTreasureGoblin))) &&
								   (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage) ||
									(Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage) && Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_BattleRage))));
		}

	}
}
