using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Monk
{
	public class WaveOfLight : Ability, IAbility
	{
		public WaveOfLight() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = Bot.Class.RuneIndexCache[SNOPower.Monk_WaveOfLight] == 1
				? AbilityUseType.Self
				: AbilityUseType.ClusterLocation | AbilityUseType.Location;
			WaitVars = new WaitLoops(2, 2, true);
			Cost=Bot.Class.RuneIndexCache[SNOPower.Monk_WaveOfLight]==3?40:75;
			Range = 16;
			Priority = AbilityPriority.Low;
			UseageType=AbilityUseage.Combat;

			PreCastConditions = (AbilityConditions.CheckEnergy | AbilityConditions.CheckCanCast |
			                     AbilityConditions.CheckRecastTimer | AbilityConditions.CheckPlayerIncapacitated);
			ClusterConditions = new ClusterConditions(6d, 35f, 3, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 20);

			Fcriteria=new Func<bool>(() => { return !Bot.Class.bWaitingForSpecial; });

			Fbuff=new Func<bool>(() => { return Bot.Character.dCurrentHealthPct<0.25d; });
			IsBuff=true;

		}

		public override void InitCriteria()
		{
			base.AbilityTestConditions = new AbilityUsablityTests(this);
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
				Ability p = (Ability) obj;
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
