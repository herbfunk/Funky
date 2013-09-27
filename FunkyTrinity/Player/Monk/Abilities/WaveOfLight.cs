using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Monk
{
	public class WaveOfLight : ability, IAbility
	{
		public WaveOfLight() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = Bot.Class.RuneIndexCache[SNOPower.Monk_WaveOfLight] == 1
				? AbilityExecuteFlags.Self
				: AbilityExecuteFlags.ClusterLocation | AbilityExecuteFlags.Location;
			WaitVars = new WaitLoops(2, 2, true);
			Cost=Bot.Class.RuneIndexCache[SNOPower.Monk_WaveOfLight]==3?40:75;
			Range = 16;
			Priority = AbilityPriority.Low;
			UseageType=AbilityUseage.Combat;

			PreCastPreCastFlags = (AbilityPreCastFlags.CheckEnergy | AbilityPreCastFlags.CheckCanCast |
			                     AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckPlayerIncapacitated);
			ClusterConditions = new ClusterConditions(6d, 35f, 3, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 20);

			FcriteriaCombat=new Func<bool>(() => { return !Bot.Class.bWaitingForSpecial; });

			FcriteriaBuff=new Func<bool>(() => { return Bot.Character.dCurrentHealthPct<0.25d; });
			IsBuff=true;

		}

		#region IAbility

		public override int RuneIndex
		{
			get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power) ? Bot.Class.RuneIndexCache[this.Power] : -1; }
		}

		public override int GetHashCode()
		{
			return (int) this.Power;
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			else
			{
				ability p = (ability) obj;
				return this.Power == p.Power;
			}
		}

		#endregion

		public override SNOPower Power
		{
			get { return SNOPower.Monk_WaveOfLight; }
		}
	}
}
