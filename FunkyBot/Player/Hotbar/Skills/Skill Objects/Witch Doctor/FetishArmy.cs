using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
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
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 16, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			//Zunimassa Set we have perma pets!
			if (Bot.Settings.WitchDoctor.ZunimassaFullSet)
			{
				IsBuff = true;
				FcriteriaBuff = () => Bot.Character.Data.PetData.WitchdoctorFetish < GetTotalFetishCount();
				FcriteriaCombat = () => Bot.Character.Data.PetData.WitchdoctorFetish < GetTotalFetishCount();
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
