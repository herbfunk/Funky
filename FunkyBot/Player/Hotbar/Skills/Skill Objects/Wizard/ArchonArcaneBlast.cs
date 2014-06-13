using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class ArchonArcaneBlast : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=1000;
				ExecutionType=SkillExecutionFlags.Buff;
				WaitVars=new WaitLoops(0, 0, false);
				UseageType=SkillUseage.Anywhere;
				Priority=SkillPriority.High;
				IsBuff = true;
				FcriteriaBuff = () => false;
				PreCast=new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast);
				//SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 10));
				FcriteriaCombat = () => Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_12]>0;
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
				get { return SNOPower.Wizard_Archon_ArcaneBlast; }
		  }
	 }

	public class ArchonArcaneBlastCold : ArchonArcaneBlast
	{
		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon_ArcaneBlast_Cold; }
		}
	}

	public class ArchonArcaneBlastFire : ArchonArcaneBlast
	{
		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon_ArcaneBlast_Fire; }
		}
	}

	public class ArchonArcaneBlastLightning : ArchonArcaneBlast
	{
		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon_ArcaneBlast_Lightning; }
		}
	}


}
