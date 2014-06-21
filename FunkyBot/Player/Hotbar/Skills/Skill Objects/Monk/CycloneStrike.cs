using System;
using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	public class CycloneStrike : Skill
	{
		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Cooldown = 1000;


			WaitVars = new WaitLoops(2, 2, true);
			Cost = 50;
			Priority = SkillPriority.High;

			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckCanCast |
									  SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckPlayerIncapacitated));

			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 7);
			//ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 3);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, 23, 7, 0.95d));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 23, 7, 0.95d, TargetProperties.Normal | TargetProperties.Boss));

			ClusterConditions.Add(new SkillClusterConditions(10d, 24f, 4, false, 0, ClusterProperties.Large, 10f, false));
			FcriteriaCombat = () =>
			{
				if (LastConditionPassed == ConditionCriteraTypes.SingleTarget) return true; //special and fast..

				//Use every 5.5s when 7+ units are within 25f.
				if (LastConditionPassed == ConditionCriteraTypes.UnitsInRange && LastUsedMilliseconds > 5500 && !Bot.Character.Class.bWaitingForSpecial)
					return true;

				if (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_ExplodingPalm) || !Bot.Targeting.Cache.CurrentUnitTarget.SkillsUsedOnObject.ContainsKey(SNOPower.Monk_ExplodingPalm)) return true; //Non Exploding Palm Check

				if (DateTime.Now.Subtract(Bot.Targeting.Cache.CurrentUnitTarget.SkillsUsedOnObject[SNOPower.Monk_ExplodingPalm]).TotalSeconds < 9
					&& Bot.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct < 0.10d)
				{
					return true;
				}

				return false;
			};
		}

		public override SNOPower Power
		{
			get { return SNOPower.Monk_CycloneStrike; }
		}
	}
}
