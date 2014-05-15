using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class DashingStrike : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=1000;
				ExecutionType=SkillExecutionFlags.Target;
				UseageType=SkillUseage.Combat;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=25;
				Range=30;
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckEnergy|SkillPrecastFlags.CheckCanCast|
				                          SkillPrecastFlags.CheckRecastTimer|SkillPrecastFlags.CheckPlayerIncapacitated));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Ranged, 20));


				FcriteriaCombat=() => (!Bot.Character.Class.bWaitingForSpecial||Bot.Character.Data.dCurrentEnergy>=Bot.Character.Class.iWaitingReservedAmount);
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
				get { return SNOPower.X1_Monk_DashingStrike; }
		  }
	 }
}
