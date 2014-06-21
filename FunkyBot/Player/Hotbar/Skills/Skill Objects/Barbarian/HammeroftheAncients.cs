using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class HammeroftheAncients : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_HammerOfTheAncients; } }


		public override double Cooldown { get { return 0; } }


		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = Bot.Character.Class.HotBar.RuneIndexCache[Power] == 0 ? 13 : Bot.Character.Class.HotBar.RuneIndexCache[Power] == 1 ? 20 : 16;
			Cost = 20;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated);
			ClusterConditions.Add(new SkillClusterConditions(6d, 30f, 3, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: Range, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				Criteria = () => Bot.Character.Data.dCurrentEnergyPct > 0.80d,
				MaximumDistance = Range,
				FalseConditionFlags = TargetProperties.LowHealth,
			});

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}

	}
}
