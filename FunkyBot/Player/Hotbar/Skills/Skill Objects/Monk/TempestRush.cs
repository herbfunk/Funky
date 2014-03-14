using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class TempestRush : Skill
	 {
		 private bool IsHobbling
		  {
				get
				{
					 return Bot.Character.Data.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run|SNOAnim.Monk_Male_HTH_Hobble_Run);
				}
		  }

		  public override void Initialize()
		  {
				Cooldown=150;
				ExecutionType=AbilityExecuteFlags.ZigZagPathing;
				WaitVars=new WaitLoops(0, 0, true);
				Cost=15;
				IsChanneling=true;
				Range=23;
				Priority=AbilityPriority.Medium;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated));
				UseageType=AbilityUseage.Anywhere;
				
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
				SingleUnitCondition=new UnitTargetConditions
				{
					 TrueConditionFlags=TargetProperties.IsSpecial,
					 Distance=30,

				};

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

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; }
		  }

		  public override int GetHashCode()
		  {
				return (int)Power;
		  }

		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||GetType()!=obj.GetType())
				{
					 return false;
				}
			  Skill p=(Skill)obj;
			  return Power==p.Power;
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.Monk_TempestRush; }
		  }
	 }
}
