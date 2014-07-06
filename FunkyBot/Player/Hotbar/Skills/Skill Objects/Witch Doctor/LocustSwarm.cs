using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class LocustSwarm : Skill
	{
		public override double Cooldown { get { return 8000; } }

		public override bool IsSpecialAbility { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location; } }

		public override void Initialize()
		{
			bool hotbarContainsDOT = Bot.Character.Class.HotBar.HasPower(SNOPower.Witchdoctor_Haunt) || (Bot.Character.Class.HotBar.HasPower(SNOPower.Witchdoctor_Piranhas));

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
			
			
		
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(precastflags);
			ShouldTrack = true;

			PreCast.Criteria += (s) => !Bot.Character.Class.HotBar.HasDebuff(SNOPower.Succubus_BloodStar);


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

		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Locust_Swarm; }
		}
	}
}
