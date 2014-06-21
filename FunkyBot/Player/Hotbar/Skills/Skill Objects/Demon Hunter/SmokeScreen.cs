using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class SmokeScreen : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=3000;
			
				WaitVars=new WaitLoops(0, 1, true);
				Cost=28;
				SecondaryEnergy=true;
				Range=0;
				
				Priority=SkillPriority.High;
				//PreCastFlags=,

				FcriteriaCombat=() => (!Bot.Character.Class.HotBar.HasBuff(SNOPower.DemonHunter_ShadowPower)||Bot.Character.Data.bIsIncapacitated)
				                      &&(Bot.Character.Data.dDiscipline>=28)
				                      &&
				                      (Bot.Character.Data.dCurrentHealthPct<=0.90||Bot.Character.Data.bIsRooted||
				                       Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_20]>=1||
				                       Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3||Bot.Character.Data.bIsIncapacitated);
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_SmokeScreen; }
		  }
	 }
}
