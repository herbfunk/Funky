using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class Vault : Skill
	 {
		 public override bool IsMovementSkill { get { return true; } }

		 public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ZigZagPathing; } }

		 public override void Initialize()
		  {
				Cooldown=400;
			
				WaitVars=new WaitLoops(1, 2, true);
				Cost=8;
				SecondaryEnergy=true;
				Range=20;
				
				Priority=SkillPriority.Medium;
				
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast|
				                          SkillPrecastFlags.CheckEnergy|SkillPrecastFlags.CheckRecastTimer));
				//SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 10);
				//UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_6, 1);

				FCombatMovement=v =>
				{
					float fDistanceFromTarget=Bot.Character.Data.Position.Distance(v);
					if (!Bot.Character.Class.bWaitingForSpecial&&Funky.Difference(Bot.Character.Data.Position.Z, v.Z)<=4&&fDistanceFromTarget>=18f&&
					    (LastUsedMilliseconds>=Bot.Settings.DemonHunter.iDHVaultMovementDelay))
					{
						return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, Math.Max(fDistanceFromTarget, 35f));
					}

					return Vector3.Zero;
				};
				FOutOfCombatMovement=v =>
				{
					float fDistanceFromTarget=Bot.Character.Data.Position.Distance(v);
					if (Funky.Difference(Bot.Character.Data.Position.Z, v.Z)<=4&&fDistanceFromTarget>=18f&&
					    (LastUsedMilliseconds>=Bot.Settings.DemonHunter.iDHVaultMovementDelay))
					{
						return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, Math.Max(fDistanceFromTarget, 35f));
					}

					return Vector3.Zero;
				};
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_Vault; }
		  }
	 }
}
