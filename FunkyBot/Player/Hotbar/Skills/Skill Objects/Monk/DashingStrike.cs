using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class DashingStrike : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=1000;
				ExecutionType=SkillExecutionFlags.Location;
				UseageType=SkillUseage.Combat;
				WaitVars=new WaitLoops(0, 0, false);
				Range=40;
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast(SkillPrecastFlags.CheckCanCast|SkillPrecastFlags.CheckPlayerIncapacitated);
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Ranged, mindistance: 30));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, mindistance: 30, falseConditionalFlags: TargetProperties.Normal));

				FcriteriaCombat=() => !Bot.Character.Class.bWaitingForSpecial;

				IsMovementSkill = true;
				FCombatMovement = (v) =>
				{
					float fDistanceFromTarget = Bot.Character.Data.Position.Distance(v);
					if (!Bot.Character.Class.bWaitingForSpecial && Funky.Difference(Bot.Character.Data.Position.Z, v.Z) <= 4)
					{
						if (fDistanceFromTarget > 50f)
							return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 50f);
						
						return v;
					}

					return Vector3.Zero;
				};
				FOutOfCombatMovement = (v) =>
				{
					float fDistanceFromTarget = Bot.Character.Data.Position.Distance(v);
					if (Funky.Difference(Bot.Character.Data.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
					{
						if (fDistanceFromTarget > 50f)
							return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 50f);
						else
							return v;
					}

					return Vector3.Zero;
				};
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; }
		  }

		  public override int GetHashCode()
		  {
				return (int)Power;
		  }

		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||GetType()!=obj.GetType())
				{
					 return false;
				}
			  Skill p=(Skill)obj;
			  return Power==p.Power;
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.X1_Monk_DashingStrike; }
		  }
	 }
}
