using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
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
						Criteria = () => Bot.Character.Data.dCurrentEnergyPct>0.9d || Bot.Character.Data.dCurrentHealthPct<0.5d,
						MaximumDistance = 15,
						FalseConditionFlags = TargetProperties.Normal,
						TrueConditionFlags = TargetProperties.None,
						HealthPercent = 0.95d,
					}
				);

				ClusterConditions.Add(new SkillClusterConditions(5d, 15, 4, true));

				//FcriteriaCombat=() => Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15]>=1||Bot.Character.Data.dCurrentHealthPct<=0.4||
				//					  (Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=5&&
				//					   Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_50]==0)||
				//					  (Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3&&Bot.Character.Data.dCurrentEnergyPct<=0.5)||
				//					  (Bot.Targeting.Cache.CurrentTarget.IsBoss && Bot.Targeting.Cache.CurrentTarget.RadiusDistance <= 15f) ||
				//					  (Bot.Settings.Monk.bMonkInnaSet && Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15] >= 1 &&
				//					   Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Monk_SweepingWind))
				//					  &&
				//					  // Check if we don't have breath of heaven
				//					  (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_BreathOfHeaven)||
				//					   (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_BreathOfHeaven) && (!Bot.Settings.Monk.bMonkInnaSet ||
				//																										 Bot.Character.Class.HotBar.HasBuff(SNOPower.Monk_BreathOfHeaven))))&&
				//					  // Check if either we don't have sweeping winds, or we do and it's ready to cast in a moment
				//					  (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)||
				//					   (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&(Bot.Character.Data.dCurrentEnergy>=95||
				//																									   (Bot.Settings.Monk.bMonkInnaSet &&
				//																										Bot.Character.Data.dCurrentEnergy>=25)||
				//																									   Bot.Character.Class.HotBar.HasBuff(SNOPower.Monk_SweepingWind)))||
				//					   Bot.Character.Data.dCurrentHealthPct<=0.4);
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.Monk_BlindingFlash; }
		  }
	 }
}
