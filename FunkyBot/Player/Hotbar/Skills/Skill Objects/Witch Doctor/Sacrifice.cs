using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	 public class Sacrifice : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=1000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 0, true);
				Cost=10;
				Range=48;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast));
				//ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.None, falseConditionalFlags: TargetProperties.FullHealth);
				FcriteriaCombat = () =>
				{
					if (LastUsedMilliseconds > 27000 || Bot.Character.Class.Abilities[SNOPower.Witchdoctor_SummonZombieDog].CheckPreCastConditionMethod())
						return true;

					return false;
				};
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
				get { return SNOPower.Witchdoctor_Sacrifice; }
		  }
	 }
}
