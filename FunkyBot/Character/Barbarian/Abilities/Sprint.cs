using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Barb
{
	 public class Sprint : Ability, IAbility
	 {
		  public Sprint()
				: base()
		  {
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_Sprint; }
		  }

		  public override int RuneIndex { get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=2900;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=20;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.Low;
				PreCastFlags=(AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast|
											AbilityPreCastFlags.CheckPlayerIncapacitated);

				IsBuff=true;
				FcriteriaBuff=new Func<bool>(() =>
					 {
						  return Bot.Settings.OutOfCombatMovement&&!Bot.Class.HotBar.HasBuff(this.Power);
					 }
				);
				FcriteriaCombat=new Func<bool>(() =>
				{
					 return (!Bot.Class.HotBar.HasBuff(SNOPower.Barbarian_Sprint)&&Bot.Settings.OutOfCombatMovement)||
							  (((Bot.Settings.Class.bFuryDumpWrath&&Bot.Character.dCurrentEnergyPct>=0.95&&
										 Bot.Class.HotBar.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker))||
								 (Bot.Settings.Class.bFuryDumpAlways&&Bot.Character.dCurrentEnergyPct>=0.95)||
									  ((Bot.Class.Abilities[SNOPower.Barbarian_Sprint].AbilityUseTimer()&&!Bot.Class.HotBar.HasBuff(SNOPower.Barbarian_Sprint))&&
						  // Always keep up if we are whirlwinding, or if the target is a goblin
								  (Bot.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_Whirlwind)||
									Bot.Targeting.CurrentTarget.IsTreasureGoblin)))&&
								(!Bot.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)||
									  (Bot.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_BattleRage)&&Bot.Class.HotBar.HasBuff(SNOPower.Barbarian_BattleRage))));
				});
		  }

		  #region IAbility
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
	 }
}
