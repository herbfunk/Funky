using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Barb
{
	public class FuriousCharge : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_FuriousCharge; } }


		public override double Cooldown { get { return 15000; } }


		public override bool IsMovementSkill { get { return true; } }

		private readonly WaitLoops _waitVars = new WaitLoops(1, 2, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;
			Range = 35;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated);

			ClusterConditions.Add(new SkillClusterConditions(7d, 35f, 4, false, minDistance: 15f, useRadiusDistance: true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 30, falseConditionalFlags: TargetProperties.Normal));

			FCombatMovement = v =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (Bot.Settings.General.OutOfCombatMovement && !Bot.Character.Class.bWaitingForSpecial && Funky.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
					if (fDistanceFromTarget > 35f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 35f);
					return v;
				}

				return Vector3.Zero;
			};
			FOutOfCombatMovement = v =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (Funky.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
					if (fDistanceFromTarget > 35f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 35f);
					return v;
				}

				return Vector3.Zero;
			};
		}

	}
}
