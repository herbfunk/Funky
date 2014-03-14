using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class Preparation : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=5000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer|
				                          AbilityPreCastFlags.CheckCanCast));
				Cost=Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.DemonHunter_Preparation]==0?25:0;
				FcriteriaCombat=() => Bot.Character.Data.dDisciplinePct<0.25d
					//Rune: Punishment (Restores all Hatered for 25 disc)
				                      ||(Bot.Character.Class.HotBar.RuneIndexCache[Power]==0&&Bot.Character.Data.dCurrentEnergyPct<0.20d);

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
				else
				{
					 Skill p=(Skill)obj;
					 return Power==p.Power;
				}
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_Preparation; }
		  }
	 }
}
