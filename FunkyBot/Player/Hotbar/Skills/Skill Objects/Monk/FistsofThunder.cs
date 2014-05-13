using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class FistsofThunder : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=SkillExecutionFlags.ClusterTarget|SkillExecutionFlags.Target;
				WaitVars=new WaitLoops(0, 0, false);
				UseageType=SkillUseage.Combat;
				Priority=SkillPriority.Low;
				Range=RuneIndex==0?25:12;

				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast));

				ClusterConditions.Add(new SkillClusterConditions(5d, 20f, 1, true));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));

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
				get { return SNOPower.Monk_FistsofThunder; }
		  }
	 }
}
