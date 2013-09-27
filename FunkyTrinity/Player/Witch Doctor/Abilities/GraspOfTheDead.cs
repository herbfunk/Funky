using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.WitchDoctor
{
	public class GraspOfTheDead : ability, IAbility
	{
		public GraspOfTheDead() : base()
		{
		}



		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.ClusterTarget | AbilityExecuteFlags.Target;

			WaitVars = new WaitLoops(0, 3, true);
			Cost = 122;
			Range = 45;
			IsRanged = true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;

			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated | AbilityPreCastFlags.CheckCanCast |
			                     AbilityPreCastFlags.CheckEnergy);

			FcriteriaPreCast=new Func<bool>(() => { return !Bot.Class.HasDebuff(SNOPower.Succubus_BloodStar); });

			ClusterConditions = new ClusterConditions(4d, 45f, 2, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 45,
				falseConditionalFlags: TargetProperties.Fast | TargetProperties.Weak);
			FcriteriaCombat = new Func<bool>(() =>
			{
				return !Bot.Class.bWaitingForSpecial;
			});
		}

		#region IAbility

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
			get { return SNOPower.Witchdoctor_GraspOfTheDead; }
		}
	}
}
