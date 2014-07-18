using fBaseXtensions.Game.Hero;
using FunkyBot.Cache;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Barb
{
	public class Whirlwind : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_Whirlwind; }
		}


		public override double Cooldown { get { return 5; } }


		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override bool IsMovementSkill { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ZigZagPathing; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;
			Cost = 10;
			Range = 15;

			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckPlayerIncapacitated));
			ClusterConditions.Add(new SkillClusterConditions(10d, 30f, 2, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 20, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial &&
								  (!Bot.Settings.Barbarian.bSelectiveWhirlwind || Bot.Targeting.Cache.Environment.bAnyNonWWIgnoreMobsInRange ||
								   !CacheIDLookup.hashActorSNOWhirlwindIgnore.Contains(Bot.Targeting.Cache.CurrentTarget.SNOID)) &&
				// If they have battle-rage, make sure it's up
								  (!Hotbar.HasPower(SNOPower.Barbarian_BattleRage) ||
								   (Hotbar.HasPower(SNOPower.Barbarian_BattleRage) && Hotbar.HasBuff(SNOPower.Barbarian_BattleRage)));


		}
	}
}
