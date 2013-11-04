using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Barb
{
	 public class Leap : Ability, IAbility
	 {
		  public Leap()
				: base()
		  {
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_Leap; }
		  }

		  public override int RuneIndex { get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=10200;
				WaitVars=new WaitLoops(2, 2, true);
				ExecutionType=AbilityExecuteFlags.ClusterLocation|AbilityExecuteFlags.Location;
				Range=35;
				Priority=AbilityPriority.Low;
				UseageType=AbilityUseage.Combat;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer|
											AbilityPreCastFlags.CheckCanCast);
				ClusterConditions=new ClusterConditions(5d, 30, 2, true);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial,
					falseConditionalFlags: TargetProperties.Fast, MinimumDistance: 30);

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
