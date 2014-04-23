using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

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
				ExecutionType=SkillExecutionFlags.Self;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=0;
				UseageType=SkillUseage.Anywhere;
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckRecastTimer|SkillPrecastFlags.CheckCanCast|SkillPrecastFlags.CheckPlayerIncapacitated));
				FcriteriaCombat=() => (
					Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_20]>1||
					(Bot.Targeting.Cache.CurrentTarget.IsBoss && Bot.Targeting.Cache.CurrentTarget.RadiusDistance <= 20) ||
					(Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]>2&&!Bot.Targeting.Cache.Environment.bAnyBossesInRange&&
					 (Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_50]==0||
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
