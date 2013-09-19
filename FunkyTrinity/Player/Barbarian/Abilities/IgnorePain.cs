using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;
namespace FunkyTrinity.Ability.Abilities.Barb
{
	public class IgnorePain : ability, IAbility
	{
		public IgnorePain() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_IgnorePain; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Buff;
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 0;
			UseageType=AbilityUseage.Anywhere;
			IsSpecialAbility = true;
			Priority = AbilityPriority.High;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckCanCast);

			Fcriteria = new Func<bool>(() => { return Bot.Character.dCurrentHealthPct <= 0.45; });
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
