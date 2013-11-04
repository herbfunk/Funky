using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.DemonHunter
{
	 public class SpikeTrap : Ability, IAbility
	 {
		  public SpikeTrap()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=1000;

                //Runeindex 2 == Sticky Trap!
                if (this.RuneIndex != 2)
                    ExecutionType = AbilityExecuteFlags.Location | AbilityExecuteFlags.ClusterTargetNearest;
                else
                    ExecutionType = AbilityExecuteFlags.Target | AbilityExecuteFlags.ClusterTarget;

				WaitVars=new WaitLoops(1, 1, true);
				Cost=30;
				Range=40;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.Low;

				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckEnergy);

                if (this.RuneIndex==2) //sticky trap on weak non-full HP units!
				    TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.Weak, falseConditionalFlags: TargetProperties.FullHealth);
                else
                    TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.RareElite);


				ClusterConditions=new AbilityFunky.ClusterConditions(6d, 45f, 2, true);

				FcriteriaCombat=new Func<bool>(() =>
				{
                    
					 return 
                         Bot.Character.PetData.DemonHunterSpikeTraps< 
                         (Bot.Class.HotBar.PassivePowers.Contains(SNOPower.DemonHunter_Passive_CustomEngineering)?6:3); //Passive increases traps to 6
				});
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; }
		  }

		  public override int GetHashCode()
		  {
				return (int)this.Power;
		  }

		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||this.GetType()!=obj.GetType())
				{
					 return false;
				}
				else
				{
					 Ability p=(Ability)obj;
					 return this.Power==p.Power;
				}
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_SpikeTrap; }
		  }
	 }
}
