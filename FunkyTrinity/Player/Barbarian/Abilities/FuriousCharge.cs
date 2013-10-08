using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Barb
{
	 public class FuriousCharge : Ability, IAbility
	 {
		  public FuriousCharge()
				: base()
		  {
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_FuriousCharge; }
		  }

		  public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=500;
				ExecutionType=AbilityExecuteFlags.Target;
				WaitVars=new WaitLoops(1, 2, true);
				Cost=20;
				Range=35;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Low;
				PreCastFlags=(AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckEnergy|
											AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckPlayerIncapacitated);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 3);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 15);

				FCombatMovement=new Func<Vector3, Vector3>((v) =>
				{
					 float fDistanceFromTarget=Bot.Character.Position.Distance(v);
					 if (!Bot.Class.bWaitingForSpecial&&Funky.Difference(Bot.Character.Position.Z, v.Z)<=4&&fDistanceFromTarget>=20f)
					 {
						  if (fDistanceFromTarget>35f)
								return MathEx.CalculatePointFrom(v, Bot.Character.Position, 35f);
						  else
								return v;
					 }

					 return Vector3.Zero;
				});
				FOutOfCombatMovement=new Func<Vector3, Vector3>((v) =>
				{
					 float fDistanceFromTarget=Bot.Character.Position.Distance(v);
					 if (Funky.Difference(Bot.Character.Position.Z, v.Z)<=4&&fDistanceFromTarget>=20f)
					 {
						  if (fDistanceFromTarget>35f)
								return MathEx.CalculatePointFrom(v, Bot.Character.Position, 35f);
						  else
								return v;
					 }

					 return Vector3.Zero;
				});
		  }

		  #region IAbility
		  public override int GetHashCode()
		  {
				return (int)this.Power;
		  }
		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||this.GetType()!=obj.GetType())
				{
					 return false;
				}
				else
				{
					 Ability p=(Ability)obj;
					 return this.Power==p.Power;
				}
		  }


		  #endregion
	 }
}
