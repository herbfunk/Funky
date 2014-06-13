using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class ArchonSlowTime : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=16000;
				ExecutionType=SkillExecutionFlags.Buff;
				IsBuff = true;
				Priority = SkillPriority.High;
				WaitVars=new WaitLoops(1, 1, true);
				//Range=48;
				UseageType=SkillUseage.Anywhere;
				PreCast = new SkillPreCast
				{
					Criteria = skill => skill.LastUsed == DateTime.MinValue
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
				get { return SNOPower.Wizard_Archon_SlowTime; }
		  }
	 }
}
