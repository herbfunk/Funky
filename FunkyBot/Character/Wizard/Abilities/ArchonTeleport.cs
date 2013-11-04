using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Wizard
{
	 public class ArchonTeleport : Ability, IAbility
	 {
		  public ArchonTeleport()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=10000;
				ExecutionType=AbilityExecuteFlags.ClusterLocation|AbilityExecuteFlags.ZigZagPathing;
				WaitVars=new WaitLoops(1, 1, true);
				Range=48;
				UseageType=AbilityUseage.Anywhere;
				//IsNavigationSpecial = true;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast);

				ClusterConditions=new ClusterConditions(5d, 48f, 2, false, minDistance: 15f);

				FcriteriaCombat=new Func<bool>(() =>
				{
					 return ((Bot.Settings.Class.bTeleportFleeWhenLowHP&&
									(Bot.Character.dCurrentHealthPct<0.5d)||
									(Bot.Targeting.RequiresAvoidance))
							  ||
							  (Bot.Settings.Class.bTeleportIntoGrouping&&this.LastConditionPassed==ConditionCriteraTypes.Cluster)
							  ||
							  (!Bot.Settings.Class.bTeleportFleeWhenLowHP&&!Bot.Settings.Class.bTeleportIntoGrouping));

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
				get { return SNOPower.Wizard_Archon_Teleport; }
		  }
	 }
}
