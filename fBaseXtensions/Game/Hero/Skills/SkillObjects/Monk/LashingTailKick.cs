using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class LashingTailKick : Skill
	{
		public override double Cooldown { get { return 250; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 30;
			Range = 10;
			Priority = SkillPriority.Medium;

			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckCanCast |
									  SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckPlayerIncapacitated));
			ClusterConditions.Add(new SkillClusterConditions(4d, 18f, 3, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 10, falseConditionalFlags: TargetProperties.Normal));


			FcriteriaCombat = (u) => (!Hotbar.HasPower(SNOPower.Monk_SweepingWind) ||
								   (Hotbar.HasPower(SNOPower.Monk_SweepingWind) && Hotbar.HasBuff(SNOPower.Monk_SweepingWind))) &&
								  (!FunkyGame.Hero.Class.bWaitingForSpecial || FunkyGame.Hero.dCurrentEnergy >= FunkyGame.Hero.Class.iWaitingReservedAmount);
		}

		public override SNOPower Power
		{
			get { return SNOPower.Monk_LashingTailKick; }
		}
	}
}
