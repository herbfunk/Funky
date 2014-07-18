using fBaseXtensions.Game.Hero;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Monk
{
	 public class MantraOfHealing : Skill
	 {
		 public override bool IsBuff { get { return true; } }
		 public override bool IsSpecialAbility { get { return true; } }
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=3300;
			
				WaitVars=new WaitLoops(0, 1, true);
				Cost = Hotbar.PassivePowers.Contains(SNOPower.Monk_Passive_ChantOfResonance) ? 25 : 50;
				
				
				Priority=SkillPriority.High;
				PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckRecastTimer));
				
				FcriteriaBuff=() => !Hotbar.HasBuff(SNOPower.X1_Monk_MantraOfHealing_v2_Passive);

				FcriteriaCombat = () => !Hotbar.HasBuff(SNOPower.X1_Monk_MantraOfHealing_v2_Passive)
				                      ||
				                      Bot.Settings.Monk.bMonkSpamMantra&&Bot.Targeting.Cache.CurrentTarget!=null&&
				                      (Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25]>0||
				                       Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=2||
				                       (Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=1&&Bot.Settings.Monk.bMonkInnaSet)||
									   (Bot.Targeting.Cache.CurrentUnitTarget.IsEliteRareUnique || Bot.Targeting.Cache.CurrentTarget.IsBoss) &&
				                       Bot.Targeting.Cache.CurrentTarget.RadiusDistance<=25f)&&
				                      // Check if either we don't have blinding flash, or we do and it's been cast in the last 6000ms
				                      //DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Monk_BlindingFlash]).TotalMilliseconds <= 6000)) &&
				                      (!Hotbar.HasPower(SNOPower.Monk_BlindingFlash)||
				                       (Hotbar.HasPower(SNOPower.Monk_BlindingFlash)&&(Hotbar.HasBuff(SNOPower.Monk_BlindingFlash))));
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.X1_Monk_MantraOfHealing_v2; }
		  }
	 }
}
