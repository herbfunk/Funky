using System.Linq;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	public class Companion : Skill
	{
		public override void Initialize()
		{
			//rune index 2: Wolf (increased attack damage)
			//rune index 4: Ferret (insantly pickup all nearby globes 60 yards)
			//rune index 1: Boar (Taunt nearby enemies 20 yards)
			//rune index 3: Bat (Gain 50 Hatred)
			//rune index 0: Spider (Web nearby enemies 25 yards)

			Cooldown = 30000;
			ExecutionType = SkillExecutionFlags.Buff;
			WaitVars = new WaitLoops(2, 1, true);
			UseageType = SkillUseage.Combat;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);

			//Web / Taunt
			if (RuneIndex == 0 || RuneIndex == 1)
			{
				//Cluster of 5 or more
				ClusterConditions.Add(new SkillClusterConditions(6d, RuneIndex==1?20:25,5,true));
				//Non-Normal Unit
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, RuneIndex == 1 ? 20 : 25, 0.95d, TargetProperties.Normal));
			}

			//Increased Damage
			if (RuneIndex==2)
			{
				//Non-Normal Unit
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, RuneIndex == 1 ? 20 : 25, 0.95d, TargetProperties.Normal));
			}

			//Hatred
			if (RuneIndex==3)
			{
				//Any Unit with 95% or less HP within 50 yards when energy is less than 50%
				SingleUnitCondition.Add(
					new UnitTargetConditions
					{
						Criteria = () => Bot.Character.Data.dCurrentEnergyPct<0.50d,
						Distance=50,
						HealthPercent=0.95d,
						TrueConditionFlags=TargetProperties.None
					}
				);
			}

			//Globe Pickup
			if (RuneIndex==4)
			{
				//Any Unit with 95% or less HP within 50 yards when health is less than 50%
				SingleUnitCondition.Add(
					new UnitTargetConditions
					{
						Criteria = () => Bot.Character.Data.dCurrentHealthPct < 0.50d,
						Distance = 50,
						HealthPercent = 0.95d,
						TrueConditionFlags = TargetProperties.None
					}
				);

				//When any health globes are within 60 yards!
				FcriteriaCombat = () => ObjectCache.Objects.Values.Any(o => o.targetType.HasValue && o.targetType.Value == TargetType.Globe && o.CentreDistance < 60);
			}

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
			get { return SNOPower.X1_DemonHunter_Companion; }
		}
	}
}
