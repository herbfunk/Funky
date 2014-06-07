using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class EvasiveFire : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=1500;
				Range = RuneIndex==4?35:45;
				ExecutionType=SkillExecutionFlags.Target;
				WaitVars=new WaitLoops(0, 0, false);
				Cost=0;
				UseageType=SkillUseage.Anywhere;
				Priority=SkillPriority.Low;

				PreCast=new SkillPreCast((SkillPrecastFlags.CheckCanCast));
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
				else
				{
					 Skill p=(Skill)obj;
					 return Power==p.Power;
				}
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.X1_DemonHunter_EvasiveFire; }
		  }
	 }
}
