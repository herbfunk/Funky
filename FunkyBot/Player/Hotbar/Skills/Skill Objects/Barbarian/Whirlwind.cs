using FunkyBot.Cache;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class Whirlwind : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_Whirlwind; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.ZigZagPathing;
				WaitVars=new WaitLoops(0, 0, true);
				Cost=10;
				Range=15;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Medium;

				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckPlayerIncapacitated));
				ClusterConditions=new SkillClusterConditions(10d, 30f, 2, true);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial, 20);
				FcriteriaCombat=() => !Bot.Character.Class.bWaitingForSpecial&&
				                      (!Bot.Settings.Class.bSelectiveWhirlwind||Bot.Targeting.Environment.bAnyNonWWIgnoreMobsInRange||
				                       !CacheIDLookup.hashActorSNOWhirlwindIgnore.Contains(Bot.Targeting.CurrentTarget.SNOID))&&
				                      // If they have battle-rage, make sure it's up
				                      (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)||
				                       (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)&&Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_BattleRage)));


		  }
		  #region IAbility
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
				else
				{
					 Skill p=(Skill)obj;
					 return Power==p.Power;
				}
		  }


		  #endregion
	 }
}
