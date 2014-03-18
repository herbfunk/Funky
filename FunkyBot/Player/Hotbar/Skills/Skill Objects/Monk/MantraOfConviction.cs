using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class MantraOfConviction : Skill
	 {
		 public override void Initialize()
		  {
			 
				Cooldown=3300;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.Monk_Passive_ChantOfResonance)?25:50;
				IsBuff=true;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCast = new SkillPreCast((AbilityPreCastFlags.CheckEnergy | AbilityPreCastFlags.CheckRecastTimer));
				IsSpecialAbility=true;
				FcriteriaBuff=() => !Bot.Character.Class.HotBar.HasBuff(SNOPower.X1_Monk_MantraOfConviction_v2_Passive);

				FcriteriaCombat = () => !Bot.Character.Class.HotBar.HasBuff(SNOPower.X1_Monk_MantraOfConviction_v2_Passive)
				                      ||
				                      Bot.Settings.Class.bMonkSpamMantra&&Bot.Targeting.Cache.CurrentTarget!=null&&
				                      (Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25]>0||
				                       Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=2||
				                       (Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=1&&Bot.Settings.Class.bMonkInnaSet)||
									   (Bot.Targeting.Cache.CurrentUnitTarget.IsEliteRareUnique || Bot.Targeting.Cache.CurrentTarget.IsBoss) &&
				                       Bot.Targeting.Cache.CurrentTarget.RadiusDistance<=25f)&&
				                      // Check if either we don't have blinding flash, or we do and it's been cast in the last 6000ms
				                      //DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Monk_BlindingFlash]).TotalMilliseconds <= 6000)) &&
				                      (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_BlindingFlash)||
				                       (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_BlindingFlash)&&(Bot.Character.Class.HotBar.HasBuff(SNOPower.Monk_BlindingFlash))));
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
				get { return SNOPower.X1_Monk_MantraOfConviction_v2; }
		  }
	 }
}
