using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
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
			FcriteriaBuff = () => !Hotbar.HasBuff(SNOPower.X1_Barbarian_WarCry_v2);

		    SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 50));
			FcriteriaCombat = () => (!Hotbar.HasBuff(SNOPower.X1_Barbarian_WarCry_v2) ||
								   (Hotbar.PassivePowers.Contains(SNOPower.Barbarian_Passive_InspiringPresence) && LastUsedMilliseconds > 59) ||
									FunkyGame.Hero.dCurrentEnergyPct < (FunkyGame.Hero.Class.ContainsAnyPrimarySkill?0.10:0.50)); //10% with a primary skill, 50% without one!
		}

	}
}
