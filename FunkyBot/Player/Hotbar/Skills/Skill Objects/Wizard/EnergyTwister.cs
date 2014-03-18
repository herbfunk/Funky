using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class EnergyTwister : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.Location;
				WaitVars=new WaitLoops(0, 0, true);
				Cost=35;
				Range=28;
				IsRanged=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Medium;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckCanCast));

				FcriteriaCombat=() => (!HasSignatureAbility()||Bot.Character.Class.HotBar.GetBuffStacks(SNOPower.Wizard_EnergyTwister)<1)&&
				                      (Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_30]>=1||
				                       Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_25]>=1||
				                       Bot.Targeting.Cache.CurrentTarget.RadiusDistance<=12f)&&
				                      (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_Electrocute)||
				                       !Bot.Targeting.Cache.CurrentUnitTarget.IsFast)&&
				                      (Bot.Character.Data.dCurrentEnergy>=35);
		  }

		  private bool HasSignatureAbility()
		  {
				return (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_MagicMissile)||Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_ShockPulse)||
										Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_SpectralBlade)||Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_Electrocute));
		  }


		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; }
		  }

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
			  Skill p=(Skill)obj;
			  return Power==p.Power;
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_EnergyTwister; }
		  }
	 }
}
