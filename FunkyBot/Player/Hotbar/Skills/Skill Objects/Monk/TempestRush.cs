using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
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
					int channelingCost=Bot.Character.Class.HotBar.RuneIndexCache[Power]==3?8:10;

					//If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
					return (isChanneling&&Bot.Character.Data.dCurrentEnergy>channelingCost)||(Bot.Character.Data.dCurrentEnergy>40)
					       &&(!Bot.Character.Class.bWaitingForSpecial||Bot.Character.Data.dCurrentEnergy>=Bot.Character.Class.iWaitingReservedAmount);
				};

				FCombatMovement=v =>
				{
					bool isChanneling=(IsHobbling||LastUsedMilliseconds<150);
					int channelingCost=Bot.Character.Class.HotBar.RuneIndexCache[Power]==3?8:10;

					//If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
					if ((isChanneling&&Bot.Character.Data.dCurrentEnergy>channelingCost)||(Bot.Character.Data.dCurrentEnergy>15)&&!Bot.Character.Class.bWaitingForSpecial)
					{
						if (v.Distance(Bot.Character.Data.Position)>10f)
							return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 10f);
						return v;
					}


					return Vector3.Zero;
				};

				FOutOfCombatMovement=v =>
				{
					Vector3 vTargetAimPoint=MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 10f);
					bool isChanneling=(IsHobbling||LastUsedMilliseconds<150);
					int channelingCost=Bot.Character.Class.HotBar.RuneIndexCache[Power]==3?8:10;

					//If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
					if ((isChanneling&&Bot.Character.Data.dCurrentEnergy>channelingCost)||Bot.Character.Data.dCurrentEnergyPct>0.50d)
						return vTargetAimPoint;

					return Vector3.Zero;
				};
		  }
		  private bool IsHobbling
		  {
			  get
			  {
				  return Bot.Character.Data.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run | SNOAnim.Monk_Male_HTH_Hobble_Run);
			  }
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Monk_TempestRush; }
		  }
	 }
}
