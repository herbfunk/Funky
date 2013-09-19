using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.WitchDoctor
{
	public class LocustSwarm : ability, IAbility
	{
		public LocustSwarm() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.ClusterLocation | AbilityExecuteFlags.Location;
			ClusterConditions = new ClusterConditions(5d, 20f, 1, true, 0.25d);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.None, 21, 0.5d, TargetProperties.DOTDPS);
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 196;
			Range = 21;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.High;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated | AbilityPreCastFlags.CheckCanCast |
			                     AbilityPreCastFlags.CheckEnergy | AbilityPreCastFlags.CheckRecastTimer);

			Fprecast=new Func<bool>(() => { return !Bot.Class.HasDebuff(SNOPower.Succubus_BloodStar); });

			 IsSpecialAbility = true;
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
			get { return SNOPower.Witchdoctor_Locust_Swarm; }
		}
	}
}
