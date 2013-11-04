using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Wizard
{
	 public class Teleport : Ability, IAbility
	 {
		  public Teleport()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=16000;
				ExecutionType=AbilityExecuteFlags.ClusterLocation|AbilityExecuteFlags.ZigZagPathing;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=15;
				Range=35;
				UseageType=AbilityUseage.Combat;
				//IsNavigationSpecial = true;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast|
											AbilityPreCastFlags.CheckEnergy);
				ClusterConditions=new ClusterConditions(5d, 48f, 2, false);
				//TestCustomCombatConditionAlways=true,
				FcriteriaCombat=new Func<bool>(() =>
				{
					 return ((Bot.Settings.Class.bTeleportFleeWhenLowHP&&Bot.Character.dCurrentHealthPct<0.5d)
								||
								(Bot.Settings.Class.bTeleportIntoGrouping&&
								 Bot.Targeting.Clusters.AbilityClusterCache(new ClusterConditions(5d, 48f, 2, false)).Count>0&&
								 Bot.Targeting.Clusters.AbilityClusterCache(new ClusterConditions(5d, 48f, 2, false))[0].Midpoint.Distance(
									 Bot.Character.PointPosition)>15f)
								||(!Bot.Settings.Class.bTeleportFleeWhenLowHP&&!Bot.Settings.Class.bTeleportIntoGrouping));
				});
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

		  public override int RuneIndex
		  {
				get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; }
		  }

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

		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_Teleport; }
		  }
	 }
}
