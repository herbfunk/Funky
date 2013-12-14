using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class FuriousCharge : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_FuriousCharge; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=500;
				ExecutionType=AbilityExecuteFlags.Target;
				WaitVars=new WaitLoops(1, 2, true);
				Cost=20;
				Range=35;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Medium;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckPlayerIncapacitated));
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 3);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial, 15);

				FCombatMovement=v =>
				{
					float fDistanceFromTarget=Bot.Character.Data.Position.Distance(v);
					if (!Bot.Character.Class.bWaitingForSpecial&&Funky.Difference(Bot.Character.Data.Position.Z, v.Z)<=4&&fDistanceFromTarget>=20f)
					{
						if (fDistanceFromTarget>35f)
							return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 35f);
						return v;
					}

					return Vector3.Zero;
				};
				FOutOfCombatMovement=v =>
				{
					float fDistanceFromTarget=Bot.Character.Data.Position.Distance(v);
					if (Funky.Difference(Bot.Character.Data.Position.Z, v.Z)<=4&&fDistanceFromTarget>=20f)
					{
						if (fDistanceFromTarget>35f)
							return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 35f);
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
