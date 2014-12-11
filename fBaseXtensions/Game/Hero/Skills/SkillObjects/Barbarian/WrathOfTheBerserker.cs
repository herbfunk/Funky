using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
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

		public override bool IsSpecialAbility { get { return FunkyBaseExtension.Settings.Barbarian.bWaitForWrath; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }
		
		public override SkillUseage UseageType { get { return SkillUseage.Anywhere; } }

		public override void Initialize()
		{
			Priority = SkillPriority.High;
			Cost = 50;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckExisitingBuff | SkillPrecastFlags.CheckCanCast);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 40, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			if (FunkyBaseExtension.Settings.Barbarian.bBarbUseWOTBAlways)
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));

			FcriteriaCombat = (u) =>
			{
				//Anytime?
				if (FunkyBaseExtension.Settings.Barbarian.bBarbUseWOTBAlways)
					return true;

				//Treasure Goblins?
				if (u.IsTreasureGoblin)
					return FunkyBaseExtension.Settings.Barbarian.bGoblinWrath;

				//Normal Targets?
				if (u.Properties.HasFlag(TargetProperties.Normal))
					return false;


				return true;
			};
		}
	}
}
