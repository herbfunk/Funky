using System;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
{
	public class Haunt : Skill
	{
		public override double Cooldown { get { return 2000; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		private int recastTime = 11;

		public override void Initialize()
		{
			if (Equipment.CheckLegendaryItemCount(LegendaryItemTypes.Quetzalcoatl))
				recastTime = 5;

			bool hotbarContainsLoctusSwarm = Hotbar.HasPower(SNOPower.Witchdoctor_Locust_Swarm);

			//since we can only track one DOTDPS, we track locus swarm and cast this 
			if (hotbarContainsLoctusSwarm)
			{
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: -1, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			}
			else
			{
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45, falseConditionalFlags: TargetProperties.DOTDPS));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: -1, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal | TargetProperties.DOTDPS));
			}

			WaitVars = new WaitLoops(0, 0, false);
			Cost = 50;
			Range = 45;

		
			Priority = SkillPriority.High;
			ShouldTrack = true;

			var precastflags = SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast;
			if (hotbarContainsLoctusSwarm) precastflags |= SkillPrecastFlags.CheckRecastTimer;

			PreCast = new SkillPreCast(precastflags);

			PreCast.Criteria += (s) => !Hotbar.HasDebuff(SNOPower.Succubus_BloodStar);

			FcriteriaCombat = () =>
			{
				if (FunkyGame.Targeting.Cache.CurrentTarget.SkillsUsedOnObject.ContainsKey(Power))
				{
					//If we have Creeping Death, then we ignore any units that we already cast upon.
					if (Hotbar.PassivePowers.Contains(SNOPower.Witchdoctor_Passive_CreepingDeath)) return false;

					return DateTime.Now.Subtract(FunkyGame.Targeting.Cache.CurrentTarget.SkillsUsedOnObject[Power]).TotalSeconds > recastTime;
				}

				return true;
			};
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Haunt; }
		}
	}
}
