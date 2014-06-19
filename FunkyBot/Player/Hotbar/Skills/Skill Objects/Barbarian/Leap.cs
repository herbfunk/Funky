using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class Leap : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_Leap; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=10200;
				WaitVars=new WaitLoops(0, 0, true);
				ExecutionType=SkillExecutionFlags.ClusterLocation|SkillExecutionFlags.Location;
				Range=35;
				Priority=SkillPriority.Medium;
				UseageType=SkillUseage.Combat;
				IsMovementSkill = true;
				PreCast=new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast);
				
				ClusterConditions.Add(new SkillClusterConditions(18d, 30, 2, true));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 30));

				FCombatMovement=(v) =>
				{
					float fDistanceFromTarget=Bot.Character.Data.Position.Distance(v);
					if (!Bot.Character.Class.bWaitingForSpecial&&Funky.Difference(Bot.Character.Data.Position.Z, v.Z)<=4&&fDistanceFromTarget>=20f)
					{
						if (fDistanceFromTarget>35f)
							return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 35f);
						else
							return v;
					}

					return Vector3.Zero;
				};
				FOutOfCombatMovement=(v) =>
				{
					float fDistanceFromTarget=Bot.Character.Data.Position.Distance(v);
					if (Funky.Difference(Bot.Character.Data.Position.Z, v.Z)<=4&&fDistanceFromTarget>=20f)
					{
						if (fDistanceFromTarget>35f)
							return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 35f);
						else
							return v;
					}

					return Vector3.Zero;
				};

		  }

		  #region IAbility
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
				else
				{
					 Skill p=(Skill)obj;
					 return Power==p.Power;
				}
		  }


		  #endregion
	 }
}
