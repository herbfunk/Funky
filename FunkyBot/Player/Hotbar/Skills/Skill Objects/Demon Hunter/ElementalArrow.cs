using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class ElementalArrow : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.ClusterTarget|AbilityExecuteFlags.Target;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=10;
				Range=48;
				IsRanged=true;
				IsProjectile=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Medium;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy));
				ClusterConditions=new SkillClusterConditions(4d, 40, 2, true);
				FcriteriaCombat = () => (!Bot.Targeting.Cache.CurrentTarget.IsTreasureGoblin &&
				                       Bot.Targeting.Cache.CurrentTarget.SNOID!=5208&&Bot.Targeting.Cache.CurrentTarget.SNOID!=5209&&
				                       Bot.Targeting.Cache.CurrentTarget.SNOID!=5210);
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
				get { return SNOPower.DemonHunter_ElementalArrow; }
		  }
	 }
}
