using System;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Barb
{
	 public class Earthquake : Skill
	 {
		 public override SNOPower Power { get { return SNOPower.Barbarian_Earthquake; } }

		
		 public override double Cooldown { get { return 120500; } }

		 public override bool IsSpecialAbility { get { return true; } }

		 private readonly WaitLoops _waitVars = new WaitLoops(4, 4, true);
		 public override WaitLoops WaitVars { get { return _waitVars; } }

		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }
	
		 public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		  public override void Initialize()
		  {

				Cooldown=120500;
				
				WaitVars=new WaitLoops(4, 4, true);
				
			
				Priority=SkillPriority.High;

				PreCast=new SkillPreCast((SkillPrecastFlags.CheckRecastTimer|SkillPrecastFlags.CheckEnergy|
				                          SkillPrecastFlags.CheckExisitingBuff|SkillPrecastFlags.CheckCanCast|
				                          SkillPrecastFlags.CheckPlayerIncapacitated));
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1);

				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 13, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		  }

	 }
}
