using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class WrathOfTheBerserker : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_WrathOfTheBerserker; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=120500;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(4, 4, true);
				Cost=50;
				UseageType=AbilityUseage.Anywhere;
				IsSpecialAbility=Bot.Settings.Barbarian.bWaitForWrath;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckExisitingBuff|
				                          AbilityPreCastFlags.CheckCanCast));
				FcriteriaCombat=() => Bot.Targeting.Cache.Environment.bAnyChampionsPresent
				                      ||(Bot.Settings.Barbarian.bBarbUseWOTBAlways&&Bot.Targeting.Cache.Environment.SurroundingUnits>1)
									  || (Bot.Settings.Barbarian.bGoblinWrath && Bot.Targeting.Cache.CurrentTarget.IsTreasureGoblin);
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
