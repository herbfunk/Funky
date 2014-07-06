using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class WrathOfTheBerserker : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_WrathOfTheBerserker; }
		}

		
		public override double Cooldown { get { return 120500; } }

		private readonly WaitLoops _waitVars = new WaitLoops(4, 4, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override bool IsSpecialAbility { get { return Bot.Settings.Barbarian.bWaitForWrath; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }
		
		public override SkillUseage UseageType { get { return SkillUseage.Anywhere; } }

		public override void Initialize()
		{
			Priority = SkillPriority.High;
			Cost = 50;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckExisitingBuff | SkillPrecastFlags.CheckCanCast);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 40, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			if (Bot.Settings.Barbarian.bBarbUseWOTBAlways)
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));

			FcriteriaCombat = () =>
			{
				//Anytime?
				if (Bot.Settings.Barbarian.bBarbUseWOTBAlways)
					return true;

				//Treasure Goblins?
				if (Bot.Targeting.Cache.CurrentTarget.IsTreasureGoblin)
					return Bot.Settings.Barbarian.bGoblinWrath;

				//Normal Targets?
				if (Bot.Targeting.Cache.CurrentTarget.Properties.HasFlag(TargetProperties.Normal))
					return false;


				return true;
			};
		}
	}
}
