using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Wizard
{
	public class Teleport : ability, IAbility
	{
		public Teleport() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.ClusterLocation | AbilityExecuteFlags.ZigZagPathing;
			WaitVars = new WaitLoops(0, 1, true);
			Cost = 15;
			Range = 35;
			UseageType=AbilityUseage.Combat;
			//IsNavigationSpecial = true;
			Priority = AbilityPriority.High;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated | AbilityPreCastFlags.CheckCanCast |
			                     AbilityPreCastFlags.CheckEnergy);
			ClusterConditions = new ClusterConditions(5d, 48f, 2, false);
								//TestCustomCombatConditionAlways=true,
			Fcriteria = new Func<bool>(() =>
			{
				return ((Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP && Bot.Character.dCurrentHealthPct < 0.5d)
				        ||
				        (Bot.SettingsFunky.Class.bTeleportIntoGrouping &&
				         Bot.Combat.Clusters(new ClusterConditions(5d, 48f, 2, false)).Count > 0 &&
				         Bot.Combat.Clusters(new ClusterConditions(5d, 48f, 2, false))[0].Midpoint.Distance(
					         Bot.Character.PointPosition) > 15f)
				        || (!Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP && !Bot.SettingsFunky.Class.bTeleportIntoGrouping));
			});

			FOutOfCombatMovement=new Func<Vector3, Vector3>((v) =>
			{
				 float fDistanceFromTarget=Bot.Character.Position.Distance(v);
				 if (Funky.Difference(Bot.Character.Position.Z, v.Z)<=4&&fDistanceFromTarget>=20f)
				 {
					  if (fDistanceFromTarget>35f)
							return MathEx.CalculatePointFrom(v, Bot.Character.Position, 35f);
					  else
							return v;
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
			get { return SNOPower.Wizard_Teleport; }
		}
	}
}
