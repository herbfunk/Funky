using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class Warcry : Skill
	{
		public override SNOPower Power { get { return SNOPower.X1_Barbarian_WarCry_v2; } }

	
		public override double Cooldown { get { return 20500; } }
		

		private readonly WaitLoops _waitVars = new WaitLoops(1, 1, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override bool IsBuff { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }
		public override SkillUseage UseageType { get { return SkillUseage.Anywhere; } }

		public override void Initialize()
		{
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));
			FcriteriaBuff = () => !Bot.Character.Class.HotBar.HasBuff(SNOPower.X1_Barbarian_WarCry_v2);
			FcriteriaCombat = () => (!Bot.Character.Class.HotBar.HasBuff(SNOPower.X1_Barbarian_WarCry_v2) ||
								   (Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.Barbarian_Passive_InspiringPresence) && LastUsedMilliseconds > 59) ||
									Bot.Character.Data.dCurrentEnergyPct < 0.10);
		}

	}
}
