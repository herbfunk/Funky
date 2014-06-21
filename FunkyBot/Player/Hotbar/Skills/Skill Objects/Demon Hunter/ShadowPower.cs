using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class ShadowPower : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=5000;
			
				WaitVars=new WaitLoops(1, 1, true);
				Cost=14;
				SecondaryEnergy=true;
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckRecastTimer|
				                          SkillPrecastFlags.CheckEnergy));

				FcriteriaCombat=() => (Bot.Character.Data.dCurrentHealthPct<=0.99d||Bot.Character.Data.bIsRooted||
				                       Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25]>=1||
				                       Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3);
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_ShadowPower; }
		  }
	 }
}
