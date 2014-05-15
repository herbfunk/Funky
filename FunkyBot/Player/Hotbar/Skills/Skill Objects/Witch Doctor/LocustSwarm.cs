using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class LocustSwarm : Skill
	{
		public override void Initialize()
		{
			bool hotbarContainsHaunt = Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Witchdoctor_Haunt);

			Cooldown = 8000;
			ExecutionType = SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location;
			Range = 14;
			Cost = 196;

			var precastflags = SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast;
			if (!hotbarContainsHaunt)
			{
				ClusterConditions.Add(new SkillClusterConditions(5d, Range, 4, true, 0.25d));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, 0.99d, TargetProperties.DOTDPS));
			}
			else
			{
				ClusterConditions.Add(new SkillClusterConditions(5d, Range, 4, true));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, 0.99d, TargetProperties.Weak));
				precastflags |= SkillPrecastFlags.CheckRecastTimer;
			}

			WaitVars = new WaitLoops(1, 1, true);
			
			
			UseageType = SkillUseage.Combat;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(precastflags);
			ShouldTrack = true;

			PreCast.Criteria += (s) => !Bot.Character.Class.HotBar.HasDebuff(SNOPower.Succubus_BloodStar);

			IsSpecialAbility = true;

			FcriteriaCombat = () =>
			{
				if (Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.Witchdoctor_Passive_CreepingDeath))
				{
					return !Bot.Targeting.Cache.CurrentTarget.SkillsUsedOnObject.ContainsKey(Power);
				}
				return true;
			};
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
			Skill p = (Skill)obj;
			return Power == p.Power;
		}

		#endregion

		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Locust_Swarm; }
		}
	}
}
