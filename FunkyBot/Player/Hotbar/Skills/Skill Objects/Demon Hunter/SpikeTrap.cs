using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class SpikeTrap : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=1000;

                //Runeindex 2 == Sticky Trap!
                if (RuneIndex != 2)
                    ExecutionType = AbilityExecuteFlags.Location | AbilityExecuteFlags.ClusterTargetNearest;
                else
                    ExecutionType = AbilityExecuteFlags.Target | AbilityExecuteFlags.ClusterTarget;

				WaitVars=new WaitLoops(1, 1, true);
				Cost=30;
				Range=40;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.Medium;

				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckEnergy));

                if (RuneIndex==2) //sticky trap on weak non-full HP units!
				    SingleUnitCondition=new UnitTargetConditions(TargetProperties.Weak, falseConditionalFlags: TargetProperties.FullHealth);
                else
                    SingleUnitCondition = new UnitTargetConditions(TargetProperties.RareElite);


				ClusterConditions=new SkillClusterConditions(6d, 45f, 2, true);

				FcriteriaCombat=() => Bot.Character.Data.PetData.DemonHunterSpikeTraps< 
				                      (Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.DemonHunter_Passive_CustomEngineering)?6:3);
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
				get { return SNOPower.DemonHunter_SpikeTrap; }
		  }
	 }
}
