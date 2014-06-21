using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class DiamondSkin : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=15000;
				
				WaitVars=new WaitLoops(0, 1, true);
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckCanCast));

				FcriteriaBuff = () => Bot.Character.Data.dCurrentHealthPct <= 0.45d && Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_40] > 0;
				FcriteriaCombat=() => Bot.Character.Data.dCurrentHealthPct<=0.45d && Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_40]>0;
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_DiamondSkin; }
		  }
	 }
}
