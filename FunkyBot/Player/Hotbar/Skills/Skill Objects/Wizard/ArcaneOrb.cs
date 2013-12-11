using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class ArcaneOrb : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=500;
				ExecutionType=AbilityExecuteFlags.ClusterTarget|AbilityExecuteFlags.Target;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=35;
				Range=UsingCriticalMass()?20:40;
				IsRanged=true;
				IsProjectile=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Low;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer|
				                          AbilityPreCastFlags.CheckEnergy));
				ClusterConditions=new SkillClusterConditions(4d, UsingCriticalMass()?20:40, 3, true);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial, 25, 0.5d, TargetProperties.Fast);
				FcriteriaCombat=() => !Bot.Character.Class.bWaitingForSpecial;
		  }

		  private bool UsingCriticalMass()
		  {
				return Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.Wizard_Passive_CriticalMass); ;
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
				get { return SNOPower.Wizard_ArcaneOrb; }
		  }
	 }
}
