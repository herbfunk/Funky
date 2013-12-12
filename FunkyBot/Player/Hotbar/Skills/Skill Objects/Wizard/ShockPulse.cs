using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class ShockPulse : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.Target;
				WaitVars=new WaitLoops(0, 1, true);
				Range=Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.Wizard_ShockPulse]==2?40
					:Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.Wizard_ShockPulse]==1?26:15;

				IsRanged=true;
				IsProjectile=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.None;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated));
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
				get { return SNOPower.Wizard_ShockPulse; }
		  }
	 }
}
