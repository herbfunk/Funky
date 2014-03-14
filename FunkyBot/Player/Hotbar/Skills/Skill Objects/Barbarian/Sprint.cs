using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class Sprint : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_Sprint; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=2900;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=20;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.Medium;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast|
				                          AbilityPreCastFlags.CheckPlayerIncapacitated));

				IsBuff=true;
				FcriteriaBuff=() => Bot.Settings.OutOfCombatMovement&&!Bot.Character.Class.HotBar.HasBuff(Power);

				FcriteriaCombat=() => (!Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_Sprint)&&Bot.Settings.OutOfCombatMovement)||
				                      (((Bot.Settings.Class.bFuryDumpWrath&&Bot.Character.Data.dCurrentEnergyPct>=0.95&&
				                         Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker))||
				                        (Bot.Settings.Class.bFuryDumpAlways&&Bot.Character.Data.dCurrentEnergyPct>=0.95)||
				                        ((Bot.Character.Class.Abilities[SNOPower.Barbarian_Sprint].AbilityUseTimer()&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_Sprint))&&
				                         // Always keep up if we are whirlwinding, or if the target is a goblin
				                         (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_Whirlwind)||
										  Bot.Targeting.CurrentTarget.IsTreasureGoblin))) &&
				                       (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)||
				                        (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)&&Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_BattleRage))));
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
