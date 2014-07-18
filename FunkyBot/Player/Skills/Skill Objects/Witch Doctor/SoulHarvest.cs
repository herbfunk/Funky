using fBaseXtensions.Game.Hero;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.WitchDoctor
{
	 public class SoulHarvest : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override void Initialize()
		  {
				Cooldown=15000;
				
				WaitVars=new WaitLoops(0, 1, true);
				Cost=59;
				Counter=5;
				
				Priority=SkillPriority.High;
				Range = 10;

				PreCast=new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast);
				ClusterConditions.Add(new SkillClusterConditions(6d, 10f, 4, false, useRadiusDistance: true));
				SingleUnitCondition.Add(new UnitTargetConditions
				{
					Criteria = () => Hotbar.GetBuffStacks(SNOPower.Witchdoctor_SoulHarvest) == 0,
					MaximumDistance = 9,
					FalseConditionFlags = TargetProperties.Normal,
					HealthPercent = 0.95d,
					TrueConditionFlags = TargetProperties.None,
				});

				FcriteriaCombat=() =>
				{

					double lastCast=LastUsedMilliseconds;
					int RecastMS=RuneIndex==1?45000:20000;
					bool recast=lastCast>RecastMS; //if using soul to waste -- 45ms, else 20ms.
					int stackCount=Hotbar.GetBuffStacks(SNOPower.Witchdoctor_SoulHarvest);
					if (stackCount<5)
						return true;
					if (recast)
						return true;
					return false;
				};
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Witchdoctor_SoulHarvest; }
		  }
	 }
}
