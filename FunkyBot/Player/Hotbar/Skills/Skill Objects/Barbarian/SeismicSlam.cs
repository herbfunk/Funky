using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class SeismicSlam : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_SeismicSlam; } }


		public override double Cooldown { get { return 200; } }

		private readonly WaitLoops _waitVars = new WaitLoops(2, 2, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;
			Cost = RuneIndex == 3 ? 15 : 30;

			Range = 40;


			PreCast = new SkillPreCast((SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckEnergy |
									  SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));
			ClusterConditions.Add(new SkillClusterConditions(Bot.Character.Class.HotBar.RuneIndexCache[Power] == 4 ? 4d : 6d, 40f, 2, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.LowHealth, falseConditionalFlags: TargetProperties.Normal));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss));
			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}

	}
}
