using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class ClusterArrow : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=150;
				ExecutionType=SkillExecutionFlags.Location|SkillExecutionFlags.ClusterLocation;
				WaitVars=new WaitLoops(0, 0, false);
				Cost=50;
				Range=50;
				IsRanged=true;
				IsProjectile=true;
				UseageType=SkillUseage.Combat;
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast);


				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, 0.95d, TargetProperties.Normal));
				ClusterConditions.Add(new SkillClusterConditions(7d, Range, 3, true));
				
			    //Any unit when our energy is greater than 90%!
				SingleUnitCondition.Add(new UnitTargetConditions
				{
					TrueConditionFlags = TargetProperties.None,
					Criteria = () => Bot.Character.Data.dCurrentEnergyPct > 0.9d,
					Distance = Range,
					FalseConditionFlags = TargetProperties.LowHealth,
				});

				FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
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
				else
				{
					 Skill p=(Skill)obj;
					 return Power==p.Power;
				}
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_ClusterArrow; }
		  }
	 }
}
