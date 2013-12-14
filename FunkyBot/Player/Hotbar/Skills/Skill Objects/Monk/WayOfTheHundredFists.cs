using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class WayOfTheHundredFists : Skill
	 {
		 public override void Initialize()
		  {
				if (!Bot.Settings.Class.bMonkComboStrike)
					 Cooldown=5;
				else
					 Cooldown=250+(250*Bot.Settings.Class.iMonkComboStrikeAbilities);

				ExecutionType=AbilityExecuteFlags.Target;
				WaitVars=new WaitLoops(0, 1, false);
				Priority=Bot.Settings.Class.bMonkComboStrike?AbilityPriority.Medium:AbilityPriority.Low;
				Range=14;
				UseageType=AbilityUseage.Combat;
				var precastflags = AbilityPreCastFlags.CheckPlayerIncapacitated;
				//Combot Strike? lets enforce recast timer and cast check
				if (Bot.Settings.Class.bMonkComboStrike)
					precastflags |= AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckCanCast;

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
				get { return SNOPower.Monk_WayOfTheHundredFists; }
		  }
	 }
}
