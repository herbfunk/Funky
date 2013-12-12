using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	 public class SpiritWalk : Skill
	 {
		 public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }


		  public override void Initialize()
		  {
				Cooldown=15200;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 0, true);
				Cost=49;
				UseageType=AbilityUseage.Anywhere;
				IsSpecialAbility=true;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast));

				IsBuff=true;
				FcriteriaBuff=() => Bot.Settings.OutOfCombatMovement;

				FcriteriaCombat=() => (    (Bot.Character.Data.dCurrentHealthPct <= 0.65) ||
				                           (RuneIndex==3&&Bot.Character.Data.dCurrentEnergy<=150)||
				                           (Bot.Targeting.Environment.FleeTriggeringUnits.Count > 0) ||
				                           (Bot.Targeting.Environment.TriggeringAvoidances.Count > 0) ||
				                           (Bot.Character.Data.bIsIncapacitated || Bot.Character.Data.bIsRooted));
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
			  Skill p=(Skill)obj;
			  return Power==p.Power;
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.Witchdoctor_SpiritWalk; }
		  }
	 }
}
