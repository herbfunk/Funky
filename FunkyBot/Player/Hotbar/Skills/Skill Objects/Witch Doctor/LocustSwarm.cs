using System;
using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class LocustSwarm : Skill
	{
		public override void Initialize()
		{
			bool hotbarContainsDOT = Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Witchdoctor_Haunt) || (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Witchdoctor_Piranhas));

			Cooldown = 8000;
			ExecutionType = SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location;
			Range = 14;
			Cost = 196;

			var precastflags = SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast;
			if (!hotbarContainsDOT)
			{
				ClusterConditions.Add(new SkillClusterConditions(5d, 35, 4, true, 0.25d));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 20, MinimumHealthPercent: 0.99d, falseConditionalFlags: TargetProperties.DOTDPS));

				//Any non-normal unit (Any Range!)
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: -1, MinimumHealthPercent: 0.99d, falseConditionalFlags: TargetProperties.Normal|TargetProperties.DOTDPS));
			}
			else
			{
				ClusterConditions.Add(new SkillClusterConditions(5d, 35, 4, true));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 20, MinimumHealthPercent: 0.99d, falseConditionalFlags: TargetProperties.Weak));

				//Any non-normal unit (Any Range!)
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: -1, MinimumHealthPercent: 0.99d, falseConditionalFlags: TargetProperties.Normal));
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
				if (Bot.Targeting.Cache.CurrentTarget.SkillsUsedOnObject.ContainsKey(Power))
				{
					//If we have Creeping Death, then we ignore any units that we already cast upon.
					if (Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.Witchdoctor_Passive_CreepingDeath)) return false;

					//Runeindex 2 has duration of 16s instead of 7s
					return DateTime.Now.Subtract(Bot.Targeting.Cache.CurrentTarget.SkillsUsedOnObject[Power]).TotalSeconds > (RuneIndex==2?15:7);
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
