using System;
using FunkyBot.Cache;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Barb
{
	 public class Whirlwind : Ability, IAbility
	 {
		  public Whirlwind()
				: base()
		  {

		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_Whirlwind; }
		  }

		  public override int RuneIndex { get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.ZigZagPathing;
				WaitVars=new WaitLoops(0, 0, true);
				Cost=10;
				Range=15;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Low;

				PreCastFlags=(AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckPlayerIncapacitated);
				ClusterConditions=new ClusterConditions(10d, 30f, 2, true);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 20);
				FcriteriaCombat=new Func<bool>(() =>
				{
					 return !Bot.Class.bWaitingForSpecial&&
							  (!Bot.Settings.Class.bSelectiveWhirlwind||Bot.Targeting.Environment.bAnyNonWWIgnoreMobsInRange||
								!CacheIDLookup.hashActorSNOWhirlwindIgnore.Contains(Bot.Targeting.CurrentTarget.SNOID))&&
						  // If they have battle-rage, make sure it's up
							  (!Bot.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)||
								(Bot.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)&&Bot.Class.HotBar.HasBuff(SNOPower.Barbarian_BattleRage)));
				});


		  }
		  #region IAbility
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
	 }
}
