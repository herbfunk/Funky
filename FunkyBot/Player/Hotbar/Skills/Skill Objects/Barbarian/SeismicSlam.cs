using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class SeismicSlam : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_SeismicSlam; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=200;
				ExecutionType=AbilityExecuteFlags.ClusterLocation|AbilityExecuteFlags.Location;
				WaitVars=new WaitLoops(2, 2, true);
				Cost=Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.Barbarian_SeismicSlam]==3?15:30;
				Range=40;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Medium;

				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckPlayerIncapacitated));
				ClusterConditions=new SkillClusterConditions(Bot.Character.Class.HotBar.RuneIndexCache[Power]==4?4d:6d, 40f, 2, true);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial,
					falseConditionalFlags: TargetProperties.TreasureGoblin|TargetProperties.Fast);

				FcriteriaCombat=() => !Bot.Character.Class.bWaitingForSpecial;
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
