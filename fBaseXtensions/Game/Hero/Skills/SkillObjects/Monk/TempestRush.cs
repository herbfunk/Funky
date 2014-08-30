using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	 public class TempestRush : Skill
	 {

		 public override bool IsMovementSkill { get { return true; } }


		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.ZigZagPathing; } }


		  public override void Initialize()
		  {
				Cooldown=150;
			
				WaitVars=new WaitLoops(0, 0, true);
				Cost=15;
				IsChanneling=true;
			
				Range=23;
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated));
				
				
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 30, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

				FcriteriaCombat=() =>
				{
					bool isChanneling=(IsHobbling||LastUsedMilliseconds<150);
					int channelingCost = RuneIndex == 3 ? 8 : 10;

					//If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
					return (isChanneling&&FunkyGame.Hero.dCurrentEnergy>channelingCost)||(FunkyGame.Hero.dCurrentEnergy>40)
					       &&(!FunkyGame.Hero.Class.bWaitingForSpecial||FunkyGame.Hero.dCurrentEnergy>=FunkyGame.Hero.Class.iWaitingReservedAmount);
				};

				FCombatMovement=v =>
				{
					bool isChanneling=(IsHobbling||LastUsedMilliseconds<150);
					int channelingCost = RuneIndex == 3 ? 8 : 10;

					//If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
					if ((isChanneling&&FunkyGame.Hero.dCurrentEnergy>channelingCost)||(FunkyGame.Hero.dCurrentEnergy>15)&&!FunkyGame.Hero.Class.bWaitingForSpecial)
					{
						if (v.Distance(FunkyGame.Hero.Position)>10f)
							return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 10f);
						return v;
					}


					return Vector3.Zero;
				};

				FOutOfCombatMovement=v =>
				{
					Vector3 vTargetAimPoint=MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 10f);
					bool isChanneling=(IsHobbling||LastUsedMilliseconds<150);
					int channelingCost = RuneIndex == 3 ? 8 : 10;

					//If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
					if ((isChanneling&&FunkyGame.Hero.dCurrentEnergy>channelingCost)||FunkyGame.Hero.dCurrentEnergyPct>0.50d)
						return vTargetAimPoint;

					return Vector3.Zero;
				};
		  }
		  private bool IsHobbling
		  {
			  get
			  {
				  return FunkyGame.Hero.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run | SNOAnim.Monk_Male_HTH_Hobble_Run);
			  }
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Monk_TempestRush; }
		  }
	 }
}
