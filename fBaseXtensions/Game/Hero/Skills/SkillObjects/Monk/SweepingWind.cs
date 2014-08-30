using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class SweepingWind : Skill
	{
		public override double Cooldown { get { return 6000; } }

		public override bool IsBuff { get { return true; } }
		public override bool IsSpecialAbility { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 1, true);
			Cost = Equipment.CheckLegendaryItemCount(LegendaryItemTypes.InnasMantra, 3) ? 5 : 75;
			Priority = SkillPriority.High;
			
		
			
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy));

			ClusterConditions.Add(new SkillClusterConditions(7d, 35f, 2, false));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 25, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			FcriteriaBuff = () =>
			{
				//Rune index of 4 increases duration of buff to 20 seconds..
				int buffDuration = RuneIndex == 4 ? 17500 : 4500;

				if (FunkyBaseExtension.Settings.Monk.bMonkMaintainSweepingWind &&  //Maintaining Sweeping Wind (Must already have buff.. and has not used combat ability within 2000ms!)
					DateTime.Now.Subtract(FunkyGame.Hero.Class.LastUsedACombatAbility).TotalMilliseconds > 2000 &&
					LastUsedMilliseconds > buffDuration)
				{
					return true;
				}

				return false;
			};
			FcriteriaCombat = () =>
			{
				if (!Hotbar.CurrentBuffs.ContainsKey((int)SNOPower.Monk_SweepingWind))
					return true;

				return false;
			};
		}


		public override SNOPower Power
		{
			get { return SNOPower.Monk_SweepingWind; }
		}
	}
}
