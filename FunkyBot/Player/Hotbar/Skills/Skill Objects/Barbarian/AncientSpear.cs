using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class AncientSpear : Skill
	{
		public override SNOPower Power { get { return SNOPower.X1_Barbarian_AncientSpear; } }

		public override double Cooldown { get { return 300; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		private readonly WaitLoops _waitVars = new WaitLoops(2, 2, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;
			Range = 35;
			PreCast=new SkillPreCast((SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckCanCast |
			                          SkillPrecastFlags.CheckPlayerIncapacitated));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Ranged, maxdistance: 25, MinimumHealthPercent: 0.50d));
								
								//TestCustomCombatConditionAlways=true,
			FcriteriaCombat = () => Bot.Targeting.Cache.CurrentUnitTarget.IsRanged ||
			                        Bot.Character.Data.dCurrentEnergyPct < 0.5d;
		}

	}
}
