using System;
using FunkyTrinity.Cache;
using FunkyTrinity.Cache.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.DemonHunter
{
	public class Vault : ability, IAbility
	{
		public Vault() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Location;
			WaitVars = new WaitLoops(1, 2, true);
			Cost = 8;
			SecondaryEnergy = true;
			Range = 20;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;

			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated | AbilityPreCastFlags.CheckCanCast |
			                     AbilityPreCastFlags.CheckEnergy | AbilityPreCastFlags.CheckRecastTimer);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.None, 10);
			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_6, 1);

			FOutOfCombatMovement=new Func<Vector3, Vector3>((v) =>
			{
				 float fDistanceFromTarget=Bot.Character.Position.Distance(v);
				 if(Funky.Difference(Bot.Character.Position.Z,v.Z)<=4&&fDistanceFromTarget>=18f&&
																					(this.LastUsedMilliseconds>=Bot.SettingsFunky.Class.iDHVaultMovementDelay))
				 {
					  return MathEx.CalculatePointFrom(v, Bot.Character.Position, Math.Max(fDistanceFromTarget,35f));
				 }

				 return Vector3.Zero;
			});
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
			get { return SNOPower.DemonHunter_Vault; }
		}
	}
}
