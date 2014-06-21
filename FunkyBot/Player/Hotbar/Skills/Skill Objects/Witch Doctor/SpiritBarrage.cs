using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class SpiritBarrage : Skill
	{
		public override double Cooldown { get { return 15000; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			//ClusterConditions.Add(new SkillClusterConditions(5d, 20f, 1, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45, falseConditionalFlags: TargetProperties.DOTDPS));

			Cost = 100;
			Range = 45;
			
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);

			PreCast.Criteria += (s) => !Bot.Character.Class.HotBar.HasDebuff(SNOPower.Succubus_BloodStar);

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_SpiritBarrage; }
		}
	}
}
