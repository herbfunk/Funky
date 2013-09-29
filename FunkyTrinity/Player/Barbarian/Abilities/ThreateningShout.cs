using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.AbilityFunky.Abilities.Barb
{
	public class ThreateningShout : Ability, IAbility
	{
		public ThreateningShout() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_ThreateningShout; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Self;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 0;
			UseageType=AbilityUseage.Anywhere;
			Priority = AbilityPriority.Low;
			PreCastFlags = (AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckCanCast | AbilityPreCastFlags.CheckPlayerIncapacitated);
			FcriteriaCombat = new Func<bool>(() =>
			{
				return (
					Bot.Combat.iElitesWithinRange[(int) RangeIntervals.Range_20] > 1 ||
					(Bot.Target.CurrentTarget.IsBoss && Bot.Target.CurrentTarget.RadiusDistance <= 20) ||
					(Bot.Combat.iAnythingWithinRange[(int) RangeIntervals.Range_20] > 2 && !Bot.Combat.bAnyBossesInRange &&
					 (Bot.Combat.iElitesWithinRange[(int) RangeIntervals.Range_50] == 0 ||
					  Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_SeismicSlam))) ||
					Bot.Character.dCurrentHealthPct <= 0.75
					);
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
