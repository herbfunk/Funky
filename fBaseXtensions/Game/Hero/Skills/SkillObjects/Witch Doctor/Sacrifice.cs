using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
{
	 public class Sacrifice : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=1000;
				
				WaitVars=new WaitLoops(1, 0, true);
				Cost=10;
				Range=48;
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast));
				//ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1);
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, falseConditionalFlags: TargetProperties.FullHealth));
				FcriteriaCombat = (u) =>
				{
					if (LastUsedMilliseconds > 27000 || FunkyGame.Hero.Class.Abilities[SNOPower.Witchdoctor_SummonZombieDog].CheckPreCastConditionMethod())
						return true;

					return false;
				};
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.Witchdoctor_Sacrifice; }
		  }
	 }
}
