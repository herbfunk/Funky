using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class Companion : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=30000;
				ExecutionType=SkillExecutionFlags.Buff;
				WaitVars=new WaitLoops(2, 1, true);
				Cost=10;
				SecondaryEnergy=true;
				Counter=1;
				Range=0;
				UseageType=SkillUseage.Anywhere;
				IsBuff=true;
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPetCount|SkillPrecastFlags.CheckEnergy|
				                          SkillPrecastFlags.CheckRecastTimer|SkillPrecastFlags.CheckPlayerIncapacitated));
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
				get { return SNOPower.X1_DemonHunter_Companion; }
		  }
	 }
}
