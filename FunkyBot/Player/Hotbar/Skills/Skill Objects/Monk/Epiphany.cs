using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	public class Epiphany : Skill
	 {
		 public override void Initialize()
		  {
				//Only check for buff when correct rune is set! rune==2
				Cooldown=60000;
				ExecutionType=SkillExecutionFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				UseageType=SkillUseage.Combat;

				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckCanCast|SkillPrecastFlags.CheckRecastTimer));
				
				ClusterConditions.Add(new SkillClusterConditions(10d, 50f, 13, false));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, maxdistance: 45, MinimumHealthPercent: 0.75d));

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
				get { return SNOPower.X1_Monk_Epiphany; }
		  }
	 }
}
