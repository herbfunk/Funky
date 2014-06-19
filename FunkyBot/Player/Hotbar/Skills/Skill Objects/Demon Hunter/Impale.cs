using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class Impale : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=SkillExecutionFlags.Target;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=25;
				Range=12;
				UseageType=SkillUseage.Combat;
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 12));


				FcriteriaCombat=() => (!Bot.Character.Class.bWaitingForSpecial && Bot.Character.Data.dCurrentEnergy >= Bot.Character.Class.iWaitingReservedAmount);
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
				get { return SNOPower.DemonHunter_Impale; }
		  }
	 }
}
