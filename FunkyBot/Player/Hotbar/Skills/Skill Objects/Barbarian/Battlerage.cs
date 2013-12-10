using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class Battlerage : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_BattleRage; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=118000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=20;
				IsBuff=true;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast));
				FcriteriaBuff=() => !Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_BattleRage);
				FcriteriaCombat=() => !Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_BattleRage)||
				                      //Only if we cannot spam sprint..
				                      (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_Sprint)&&
				                       ((Bot.Settings.Class.bFuryDumpWrath&&Bot.Character.Data.dCurrentEnergyPct>=0.98&&
				                         Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker)
				                         &&Bot.Character.Data.dCurrentHealthPct>0.50d)||
				                        (Bot.Settings.Class.bFuryDumpAlways&&Bot.Character.Data.dCurrentEnergyPct>=0.98&&Bot.Character.Data.dCurrentHealthPct>0.50d)));
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
	 }
}
