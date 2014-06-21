using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
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


			FcriteriaCombat = () => (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind) ||
								   (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind) && Bot.Character.Class.HotBar.HasBuff(SNOPower.Monk_SweepingWind))) &&
								  (!Bot.Character.Class.bWaitingForSpecial || Bot.Character.Data.dCurrentEnergy >= Bot.Character.Class.iWaitingReservedAmount);
		}

		public override SNOPower Power
		{
			get { return SNOPower.Monk_LashingTailKick; }
		}
	}
}
