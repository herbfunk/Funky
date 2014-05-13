using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class DeadlyReach : Skill
	 {
		 public override void Initialize()
		  {
				if (!Bot.Settings.Monk.bMonkComboStrike)
					 Cooldown=5;
				else
					 Cooldown=250+(250*Bot.Settings.Monk.iMonkComboStrikeAbilities);

				ExecutionType=SkillExecutionFlags.Target;
				UseageType=SkillUseage.Combat;
				WaitVars=new WaitLoops(0, 0, false);
				Priority=Bot.Settings.Monk.bMonkComboStrike?SkillPriority.Medium:SkillPriority.Low;
				Range=16;
				var precastflags = SkillPrecastFlags.CheckPlayerIncapacitated;
				//Combot Strike? lets enforce recast timer and cast check
				if (Bot.Settings.Monk.bMonkComboStrike)
					precastflags |= SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckCanCast;

				PreCast = new SkillPreCast(precastflags);
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
				get { return SNOPower.Monk_DeadlyReach; }
		  }
	 }
}
