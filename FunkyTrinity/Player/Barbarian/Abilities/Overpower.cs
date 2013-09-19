using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Barb
{
	public class Overpower : ability, IAbility
	{
		public Overpower() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_Overpower; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Self;
			WaitVars = new WaitLoops(4, 4, true);
			Cost = 0;
			UseageType=AbilityUseage.Anywhere;
			Priority = AbilityPriority.Low;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckEnergy |
			                     AbilityPreCastFlags.CheckCanCast | AbilityPreCastFlags.CheckPlayerIncapacitated);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.None, 10,
				falseConditionalFlags: TargetProperties.Fast);
			ClusterConditions = new ClusterConditions(5d, 7, 2, false);
			Fcriteria = new Func<bool>(() =>
			{
				// Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_6]>=2||(Bot.Character.dCurrentHealthPct<=0.85&&Bot.Target.CurrentTarget.RadiusDistance<=5f)||
				return true;
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
