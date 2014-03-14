using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class Teleport : Skill
	 {
		 private readonly ClusterConditions combatClusterCondition=new ClusterConditions(5d,48f,2,false);

		 public override void Initialize()
		  {
				Cooldown=16000;
				ExecutionType=AbilityExecuteFlags.ClusterLocation|AbilityExecuteFlags.ZigZagPathing;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=15;
				Range=35;
				UseageType=AbilityUseage.Combat;
				IsASpecialMovementPower = true;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast|
				                          AbilityPreCastFlags.CheckEnergy));
				ClusterConditions=new SkillClusterConditions(5d, 48f, 2, false);
				//TestCustomCombatConditionAlways=true,
				FcriteriaCombat=() => ((Bot.Settings.Class.bTeleportFleeWhenLowHP&&Bot.Character.Data.dCurrentHealthPct<0.5d)
				                       ||
				                       (Bot.Settings.Class.bTeleportIntoGrouping&&
										Bot.Targeting.Clusters.AbilityClusterCache(combatClusterCondition).Count > 0 &&
										Bot.Targeting.Clusters.AbilityClusterCache(combatClusterCondition)[0].Midpoint.Distance(
					                        Bot.Character.Data.PointPosition)>15f)
				                       ||(!Bot.Settings.Class.bTeleportFleeWhenLowHP&&!Bot.Settings.Class.bTeleportIntoGrouping));
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
				get { return SNOPower.Wizard_Teleport; }
		  }
	 }
}
