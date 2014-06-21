using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	public class ArcaneOrb : Skill
	{
		public override double Cooldown { get { return 100; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 35;
			Range = 45;

		
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);
			ClusterConditions.Add(new SkillClusterConditions(5d, 40, 3, true));

			//Any unit when our energy is greater than 90%!
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
				Criteria = () => Bot.Character.Data.dCurrentEnergyPct > 0.9d,
				MaximumDistance = 40,

			});

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 40, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal | TargetProperties.Fast | TargetProperties.MissileDampening));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, maxdistance: 40));
			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}

		public override SNOPower Power
		{
			get { return SNOPower.Wizard_ArcaneOrb; }
		}
	}
}
