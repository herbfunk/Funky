using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Crusader
{
	public class SteedCharge : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_SteedCharge; }
		}


		public override double Cooldown { get { return 15000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 5;
			Cost = 10;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast));

			//We want to preform zig-zag movements when using rune Ramming Speed or Nightmare
			if (RuneIndex==0||RuneIndex==4)
				IsSpecialMovementSkill = true;

			//return location (no prediction required!)
			FOutOfCombatMovement = (v) => v;

			FCombatMovement = (v) =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (!FunkyGame.Hero.Class.bWaitingForSpecial && FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 5f)
				{
					if (fDistanceFromTarget > 45f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 45f);
					else
						return v;
				}

				return Vector3.Zero;
			};

			ClusterConditions.Add(new SkillClusterConditions(5d, 50, 7, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 35, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			
			//Reduced cool down.. lets use it more!
			if (Hotbar.PassivePowers.Contains(SNOPower.X1_Crusader_Passive_LordCommander))
			{
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 50, MinimumHealthPercent: 0d, falseConditionalFlags: TargetProperties.Weak));
			}
		}
	}
}
