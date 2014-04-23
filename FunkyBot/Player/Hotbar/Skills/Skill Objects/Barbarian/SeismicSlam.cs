using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class SeismicSlam : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_SeismicSlam; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=200;
				ExecutionType=SkillExecutionFlags.ClusterLocation|SkillExecutionFlags.Location;
				WaitVars=new WaitLoops(2, 2, true);
				Cost=Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.Barbarian_SeismicSlam]==3?15:30;
				Range=40;
				UseageType=SkillUseage.Combat;
				Priority=SkillPriority.Medium;

				PreCast=new SkillPreCast((SkillPrecastFlags.CheckRecastTimer|SkillPrecastFlags.CheckEnergy|
				                          SkillPrecastFlags.CheckCanCast|SkillPrecastFlags.CheckPlayerIncapacitated));
				ClusterConditions.Add(new SkillClusterConditions(Bot.Character.Class.HotBar.RuneIndexCache[Power]==4?4d:6d, 40f, 2, true));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.LowHealth,falseConditionalFlags: TargetProperties.Normal));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss));
				FcriteriaCombat=() => !Bot.Character.Class.bWaitingForSpecial;
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
