using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class ThreateningShout : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_ThreateningShout; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=10200;
				ExecutionType=AbilityExecuteFlags.Self;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=0;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.Low;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckPlayerIncapacitated));
				FcriteriaCombat=() => (
					Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_20]>1||
					(Bot.Targeting.CurrentTarget.IsBoss&&Bot.Targeting.CurrentTarget.RadiusDistance<=20)||
					(Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]>2&&!Bot.Targeting.Environment.bAnyBossesInRange&&
					 (Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_50]==0||
					  Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_SeismicSlam)))||
					Bot.Character.Data.dCurrentHealthPct<=0.75
					);
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
