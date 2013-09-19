using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Wizard
{
	public class ArchonTeleport : ability, IAbility
	{
		public ArchonTeleport() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.ClusterLocation | AbilityExecuteFlags.ZigZagPathing;
			WaitVars = new WaitLoops(1, 1, true);
			Range = 48;
			UseageType=AbilityUseage.Anywhere;
			//IsNavigationSpecial = true;
			Priority = AbilityPriority.High;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated | AbilityPreCastFlags.CheckCanCast);

			ClusterConditions = new ClusterConditions(5d, 48f, 2, false, minDistance: 15f);

			Fcriteria = new Func<bool>(() =>
			{
				 return ((Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP&&
								(Bot.Character.dCurrentHealthPct<0.5d)||
								(Bot.Combat.RequiresAvoidance)||
								(Bot.Combat.IsFleeing))
				        ||
				        (Bot.SettingsFunky.Class.bTeleportIntoGrouping && this.LastConditionPassed==ConditionCriteraTypes.Cluster)
				        ||
						  (!Bot.SettingsFunky.Class.bTeleportFleeWhenLowHP && !Bot.SettingsFunky.Class.bTeleportIntoGrouping));

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
			get { return SNOPower.Wizard_Archon_Teleport; }
		}
	}
}
