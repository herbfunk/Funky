using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class Warcry : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_WarCry; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=20500;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=0;
				Range=0;
				IsBuff=true;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckPlayerIncapacitated));
				FcriteriaBuff=() => !Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_WarCry);
				FcriteriaCombat=() => (!Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_WarCry)
				                       ||
				                       (Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.Barbarian_Passive_InspiringPresence)&&
				                        LastUsedMilliseconds>59
				                        ||Bot.Character.Data.dCurrentEnergyPct<0.10));

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
