using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter
{
	public class RapidFire : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }


		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }


		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target | SkillExecutionFlags.ClusterTargetNearest; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 1, true);
			Cost = RuneIndex == 3 ? 10 : 20;
			Range = 50;

			IsChanneling = true;
			IsDestructiblePower = true;
			
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			ClusterConditions.Add(new SkillClusterConditions(10d, 45f, 2, true));

			FcriteriaCombat = (u) =>
			{
				if (FunkyGame.Hero.Class.bWaitingForSpecial) return false;

				var isChanneling = (IsFiring || LastUsedMilliseconds < 450);
				//If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
				return (isChanneling && FunkyGame.Hero.dCurrentEnergy > 6) || (FunkyGame.Hero.dCurrentEnergy > 40)
					   && (!FunkyGame.Hero.Class.bWaitingForSpecial || FunkyGame.Hero.dCurrentEnergy >= FunkyGame.Hero.Class.iWaitingReservedAmount);
			};
		}

		private bool IsFiring
		{
			get
			{
				return
					 FunkyGame.Hero.CurrentSNOAnim.HasFlag(
						SNOAnim.Demonhunter_Female_1HXBow_RapidFire_01 |
						SNOAnim.Demonhunter_Female_Bow_RapidFire_01 |
						SNOAnim.Demonhunter_Female_DW_XBow_RapidFire_01 |
						SNOAnim.Demonhunter_Female_XBow_RapidFire_01 |
						SNOAnim.Demonhunter_Male_1HXBow_RapidFire_01 |
						SNOAnim.Demonhunter_Male_Bow_RapidFire_01 |
						SNOAnim.Demonhunter_Male_DW_XBow_RapidFire_01 |
						SNOAnim.Demonhunter_Male_XBow_RapidFire_01);
			}
		}

		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_RapidFire; }
		}
	}
}
