using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	 public class BlindingFlash : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=15200;
				
				WaitVars=new WaitLoops(0, 1, true);
				Cost=10;
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast);

				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, maxdistance: 15, MinimumHealthPercent: 0.95d));
				SingleUnitCondition.Add(
					new UnitTargetConditions
					{
						Criteria = (unit) => FunkyGame.Hero.dCurrentEnergyPct>0.9d || FunkyGame.Hero.dCurrentHealthPct<0.5d,
						MaximumDistance = 15,
						FalseConditionFlags = TargetProperties.Normal,
						TrueConditionFlags = TargetProperties.None,
						HealthPercent = 0.95d,
					}
				);

				ClusterConditions.Add(new SkillClusterConditions(5d, 15, 4, true));

				//FcriteriaCombat=() => FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15]>=1||FunkyGame.Hero.dCurrentHealthPct<=0.4||
				//					  (FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=5&&
				//					   FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_50]==0)||
				//					  (FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3&&FunkyGame.Hero.dCurrentEnergyPct<=0.5)||
				//					  (FunkyGame.Targeting.Cache.CurrentTarget.IsBoss && FunkyGame.Targeting.Cache.CurrentTarget.RadiusDistance <= 15f) ||
				//					  (FunkyBaseExtension.Settings.Monk.bMonkInnaSet && FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15] >= 1 &&
				//					   Hotbar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&!Hotbar.HasBuff(SNOPower.Monk_SweepingWind))
				//					  &&
				//					  // Check if we don't have breath of heaven
				//					  (!Hotbar.HotbarPowers.Contains(SNOPower.Monk_BreathOfHeaven)||
				//					   (Hotbar.HotbarPowers.Contains(SNOPower.Monk_BreathOfHeaven) && (!FunkyBaseExtension.Settings.Monk.bMonkInnaSet ||
				//																										 Hotbar.HasBuff(SNOPower.Monk_BreathOfHeaven))))&&
				//					  // Check if either we don't have sweeping winds, or we do and it's ready to cast in a moment
				//					  (!Hotbar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)||
				//					   (Hotbar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&(FunkyGame.Hero.dCurrentEnergy>=95||
				//																									   (FunkyBaseExtension.Settings.Monk.bMonkInnaSet &&
				//																										FunkyGame.Hero.dCurrentEnergy>=25)||
				//																									   Hotbar.HasBuff(SNOPower.Monk_SweepingWind)))||
				//					   FunkyGame.Hero.dCurrentHealthPct<=0.4);
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.Monk_BlindingFlash; }
		  }
	 }
}
