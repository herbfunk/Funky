using System;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Movement;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;

namespace FunkyBot.AbilityFunky.Abilities.Monk
{
	 public class TempestRush : Ability, IAbility
	 {
		  public TempestRush()
				: base()
		  {
		  }


		  private bool IsHobbling
		  {
				get
				{
					 return Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run|SNOAnim.Monk_Male_HTH_Hobble_Run);
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
				Priority=AbilityPriority.Low;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated);
				UseageType=AbilityUseage.Anywhere;
				
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
				TargetUnitConditionFlags=new UnitTargetConditions
				{
					 TrueConditionFlags=TargetProperties.IsSpecial,
					 Distance=30,

				};

				FcriteriaCombat=new Func<bool>(() =>
				{
					 bool isChanneling=(this.IsHobbling||this.LastUsedMilliseconds<150);
					 int channelingCost=Bot.Class.HotBar.RuneIndexCache[Power]==3?8:10;

					 //If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
					 return (isChanneling&&Bot.Character.dCurrentEnergy>channelingCost)||(Bot.Character.dCurrentEnergy>40)
								  &&(!Bot.Class.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount);
				});

				FCombatMovement=new Func<Vector3, Vector3>((v) =>
				{
					 bool isChanneling=(this.IsHobbling||this.LastUsedMilliseconds<150);
					 int channelingCost=Bot.Class.HotBar.RuneIndexCache[Power]==3?8:10;

					 //If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
					 if ((isChanneling&&Bot.Character.dCurrentEnergy>channelingCost)||(Bot.Character.dCurrentEnergy>15)&&!Bot.Class.bWaitingForSpecial)
					 {
						  if (v.Distance(Bot.Character.Position)>10f)
								return MathEx.CalculatePointFrom(v, Bot.Character.Position, 10f);
						  else
								return v;
					 }


					 return Vector3.Zero;
				});

				FOutOfCombatMovement=new Func<Vector3, Vector3>((v) =>
				{
					 Vector3 vTargetAimPoint=MathEx.CalculatePointFrom(v, Bot.Character.Position, 10f);
					 bool isChanneling=(this.IsHobbling||this.LastUsedMilliseconds<150);
					 int channelingCost=Bot.Class.HotBar.RuneIndexCache[Power]==3?8:10;

					 //If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
					 if ((isChanneling&&Bot.Character.dCurrentEnergy>channelingCost)||Bot.Character.dCurrentEnergyPct>0.50d)
						  return vTargetAimPoint;

					 return Vector3.Zero;
				});
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; }
		  }

		  public override int GetHashCode()
		  {
				return (int)this.Power;
		  }

		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||this.GetType()!=obj.GetType())
				{
					 return false;
				}
				else
				{
					 Ability p=(Ability)obj;
					 return this.Power==p.Power;
				}
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.Monk_TempestRush; }
		  }
	 }
}
