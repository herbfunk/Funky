using System;
using FunkyTrinity.Cache;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Barb
{
	public class Whirlwind : ability, IAbility
	{
		public Whirlwind() : base()
		{
			 
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_Whirlwind; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.ZigZagPathing;
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 10;
			Range = 15;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;

			PreCastPreCastFlags=(AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckPlayerIncapacitated);
			ClusterConditions = new ClusterConditions(10d, 30f, 2, true);
			TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 20);
			Fcriteria = new Func<bool>(() =>
			{
				return !Bot.Class.bWaitingForSpecial &&
				       (!Bot.SettingsFunky.Class.bSelectiveWhirlwind || Bot.Combat.bAnyNonWWIgnoreMobsInRange ||
				        !CacheIDLookup.hashActorSNOWhirlwindIgnore.Contains(Bot.Target.CurrentTarget.SNOID)) &&
				       // If they have battle-rage, make sure it's up
				       (!Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage) ||
				        (Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage) && Bot.Class.HasBuff(SNOPower.Barbarian_BattleRage)));
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
					ability p=(ability)obj;
					return this.Power==p.Power;
			 }
		}
	
	
		#endregion
	}
}
