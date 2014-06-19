using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class Overpower : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_Overpower; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=200;
				ExecutionType=SkillExecutionFlags.Self;
				WaitVars=new WaitLoops(4, 4, true);
				Cost=0;
				UseageType=SkillUseage.Anywhere;
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckRecastTimer|SkillPrecastFlags.CheckEnergy|
				                          SkillPrecastFlags.CheckCanCast|SkillPrecastFlags.CheckPlayerIncapacitated));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 10,
					falseConditionalFlags: TargetProperties.Fast));
				ClusterConditions.Add(new SkillClusterConditions(5d, 7, 2, false));
				FcriteriaCombat=() => true;
		  }

		  #region IAbility
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
	 }
}
