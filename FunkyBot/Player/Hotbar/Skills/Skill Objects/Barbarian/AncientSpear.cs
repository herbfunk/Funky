using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class AncientSpear : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_AncientSpear; }
		}

		public override void Initialize()
		{
			Cooldown=300;
			ExecutionType = AbilityExecuteFlags.Target;
			WaitVars = new WaitLoops(2, 2, true);
			Range = 35;
			IsRanged = true;
			IsProjectile=true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastFlags = (AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckCanCast |
			                     AbilityPreCastFlags.CheckPlayerIncapacitated);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.Ranged, 25, 0.50d);
								
								//TestCustomCombatConditionAlways=true,
			FcriteriaCombat = () => Bot.Targeting.CurrentUnitTarget.IsRanged ||
			                        Bot.Character.Data.dCurrentEnergyPct < 0.5d;
		}

		#region IAbility
		public override int GetHashCode()
		{
			 return (int)Power;
		}
		public override bool Equals(object obj)
		{
			 //Check for null and compare run-time types. 
			 if (obj==null||GetType()!=obj.GetType())
			 {
				  return false;
			 }
			 else
			 {
				  Skill p=(Skill)obj;
				  return Power==p.Power;
			 }
		}

		#endregion
	}
}
