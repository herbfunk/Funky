using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class GraspOfTheDead : Skill
	{
		public override double Cooldown { get { return 8000; } }

		public override bool IsRanged { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 3, true);
			Cost = 122;
			Range = 45;

			
			Priority = SkillPriority.High;

			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);

			PreCast.Criteria += (s) => !Bot.Character.Class.HotBar.HasDebuff(SNOPower.Succubus_BloodStar);

			ClusterConditions.Add(new SkillClusterConditions(4d, 45f, 4, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: Range, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal | TargetProperties.Fast));
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
				Criteria = () => Bot.Character.Data.dCurrentEnergyPct > 0.9d,
				//Distance = Range,
				FalseConditionFlags = TargetProperties.LowHealth,
			});

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_GraspOfTheDead; }
		}
	}
}
