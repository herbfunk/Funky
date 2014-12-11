using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
{
	public class FetishArmy : Skill
	{
		public override double Cooldown { get { return 90000; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Self; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 1, true);
			
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 25, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			//Zunimassa Set we have perma pets!
			if (Equipment.CheckLegendaryItemCount(LegendaryItemTypes.ZunimassasHaunt, 4))
			{
				IsBuff = true;
				FcriteriaBuff = () => FunkyGame.Targeting.Cache.Environment.HeroPets.WitchdoctorFetish < GetTotalFetishCount();
				FcriteriaCombat = (u) => FunkyGame.Targeting.Cache.Environment.HeroPets.WitchdoctorFetish < GetTotalFetishCount();
			}
		}

		private int GetTotalFetishCount()
		{
			int count = 5;

			if (RuneIndex == 4 || RuneIndex == 2) //Head Hunters and Tiki Torchers
				count = 7;
			else if (RuneIndex == 1) //Legion of Daggers
				count = 8;

			return count;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_FetishArmy; }
		}
	}
}
