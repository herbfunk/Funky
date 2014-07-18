using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Crusader
{
	public class FallingSword : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_FallingSword; }
		}

		
		public override double Cooldown { get { return 30000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override bool IsMovementSkill { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 45;
			Cost = 25;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			ClusterConditions.Add(new SkillClusterConditions(5d, 45, 6, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			FCombatMovement = (v) =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (!Bot.Character.Class.bWaitingForSpecial && Funky.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
					if (fDistanceFromTarget > 45f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 45f);
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
					if (fDistanceFromTarget > 45f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 45f);
					else
						return v;
				}

				return Vector3.Zero;
			};
		}
	}
}
