using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	public class Sentry : Skill
	{
		public override void Initialize()
		{
			Cooldown = 6000;
			ExecutionType = SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation;
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 30;
			Range = 55;
			UseageType = SkillUseage.Anywhere;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);
			
			//Force wait on other skills when out of energy!
			IsSpecialAbility = true;

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, 0.95d, TargetProperties.Normal | TargetProperties.Fast));
			//Any unit when our energy is greater than 90%!
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
				Criteria = () => Bot.Character.Data.dCurrentEnergyPct > 0.9d,
				Distance = Range,
				FalseConditionFlags = TargetProperties.LowHealth,
			});

			ClusterConditions.Add(new SkillClusterConditions(4d, Range, 3, true));


			FcriteriaCombat = () => Bot.Character.Data.PetData.DemonHunterSentry < SentryMax();
		}

		private int SentryMax()
		{
			int n=2;

			//Custom Engineering - Max 3
			if (Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.DemonHunter_Passive_CustomEngineering))
				n++;
			
			//Bombardier's Rucksack - Additional 2
			if (Bot.Settings.DemonHunter.BombadiersRucksack)
				n += 2;

			return n;
		}

		#region IAbility

		public override int RuneIndex
		{
			get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; }
		}

		public override int GetHashCode()
		{
			return (int)Power;
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			else
			{
				Skill p = (Skill)obj;
				return Power == p.Power;
			}
		}

		#endregion

		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_Sentry; }
		}
	}
}
