using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Barb
{
	public class Leap : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_Leap; } }

		public override double Cooldown { get { return 10200; } }

		public override bool IsMovementSkill { get { return true; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 35;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);

			ClusterConditions.Add(new SkillClusterConditions(18d, 30, 2, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 30));

			FCombatMovement = (v) =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (!Bot.Character.Class.bWaitingForSpecial && Funky.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
					if (fDistanceFromTarget > 35f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 35f);
					else
						return v;
				}

				return Vector3.Zero;
			};
			FOutOfCombatMovement = (v) =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (Funky.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
					if (fDistanceFromTarget > 35f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 35f);
					else
						return v;
				}

				return Vector3.Zero;
			};

		}

	}
}
