using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.DemonHunter
{
	public class RapidFire : ability, IAbility
	{
		public RapidFire() : base()
		{
		}

		private bool IsFiring
		{
			 get
			 {
				  Bot.Character.UpdateAnimationState(false);
				  return 
						Bot.Character.CurrentSNOAnim.HasFlag(
						  SNOAnim.Demonhunter_Female_1HXBow_RapidFire_01|
						  SNOAnim.Demonhunter_Female_Bow_RapidFire_01|
						  SNOAnim.Demonhunter_Female_DW_XBow_RapidFire_01|
						  SNOAnim.Demonhunter_Female_XBow_RapidFire_01|
						  SNOAnim.Demonhunter_Male_1HXBow_RapidFire_01|
						  SNOAnim.Demonhunter_Male_Bow_RapidFire_01|
						  SNOAnim.Demonhunter_Male_DW_XBow_RapidFire_01|
						  SNOAnim.Demonhunter_Male_XBow_RapidFire_01);
			 }
		}

		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Target|AbilityExecuteFlags.ClusterTargetNearest;
			WaitVars = new WaitLoops(0, 1, true);
			Cost=this.RuneIndex==3?10:20;
			Range = 50;
			IsRanged = true;
			IsProjectile=true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated);

			TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 45);
			ClusterConditions=new ClusterConditions(10d, 45f, 2, true);

			Fcriteria=new Func<bool>(() =>
			{
				 bool isChanneling=(this.IsFiring||this.LastUsedMilliseconds<450);
				 //If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
				 return (isChanneling&&Bot.Character.dCurrentEnergy>6)||(Bot.Character.dCurrentEnergy>40)
							  &&(!Bot.Class.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount);
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
			get { return SNOPower.DemonHunter_RapidFire; }
		}
	}
}
