using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class RapidFire : Skill
	 {
		 private bool IsFiring
		  {
				get
				{
					 return
						  Bot.Character.Data.CurrentSNOAnim.HasFlag(
							 SNOAnim.Demonhunter_Female_1HXBow_RapidFire_01|
							 SNOAnim.Demonhunter_Female_Bow_RapidFire_01|
							 SNOAnim.Demonhunter_Female_DW_XBow_RapidFire_01|
							 SNOAnim.Demonhunter_Female_XBow_RapidFire_01|
							 SNOAnim.Demonhunter_Male_1HXBow_RapidFire_01|
							 SNOAnim.Demonhunter_Male_Bow_RapidFire_01|
							 SNOAnim.Demonhunter_Male_DW_XBow_RapidFire_01|
							 SNOAnim.Demonhunter_Male_XBow_RapidFire_01);
				}
		  }

		  public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=SkillExecutionFlags.Target|SkillExecutionFlags.ClusterTargetNearest;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=RuneIndex==3?10:20;
				Range=50;
				IsRanged=true;
				IsProjectile=true;
				IsChanneling=true;
				UseageType=SkillUseage.Combat;
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 45, 0.95d, TargetProperties.Normal));

				ClusterConditions.Add(new SkillClusterConditions(10d, 45f, 2, true));

				FcriteriaCombat=() =>
				{
					if (Bot.Character.Class.bWaitingForSpecial) return false;

					var isChanneling=(IsFiring||LastUsedMilliseconds<450);
					//If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
					return (isChanneling&&Bot.Character.Data.dCurrentEnergy>6)||(Bot.Character.Data.dCurrentEnergy>40)
					       &&(!Bot.Character.Class.bWaitingForSpecial||Bot.Character.Data.dCurrentEnergy>=Bot.Character.Class.iWaitingReservedAmount);
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
				get { return SNOPower.DemonHunter_RapidFire; }
		  }
	 }
}
