using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class HammeroftheAncients : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_HammerOfTheAncients; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=0;
				ExecutionType=SkillExecutionFlags.ClusterTarget|SkillExecutionFlags.Target;
				WaitVars=new WaitLoops(0, 0, true);
				Cost=20;
				Range=Bot.Character.Class.HotBar.RuneIndexCache[Power]==0?13:Bot.Character.Class.HotBar.RuneIndexCache[Power]==1?20:16;
				UseageType=SkillUseage.Combat;
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast(SkillPrecastFlags.CheckCanCast|SkillPrecastFlags.CheckPlayerIncapacitated);
				ClusterConditions.Add(new SkillClusterConditions(6d, 30f, 3, true));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: Range, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
				SingleUnitCondition.Add(new UnitTargetConditions
				{
					Criteria = () => Bot.Character.Data.dCurrentEnergyPct > 0.80d,
					MaximumDistance = Range,
					FalseConditionFlags = TargetProperties.LowHealth,
				});

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
