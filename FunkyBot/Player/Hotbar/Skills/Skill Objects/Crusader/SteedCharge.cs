using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class SteedCharge : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_SteedCharge; }
		}

		public override void Initialize()
		{
			Cooldown = 15000;
			Range = 5;
			Priority = SkillPriority.High;
			ExecutionType = SkillExecutionFlags.Buff;
			WaitVars = new WaitLoops(0, 0, true);
			UseageType = SkillUseage.Combat;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast));

			//We want to preform zig-zag movements when using rune Ramming Speed or Nightmare
			if (RuneIndex==0||RuneIndex==4)
				IsSpecialMovementSkill = true;

			//return location (no prediction required!)
			FOutOfCombatMovement = (v) => v;

			FCombatMovement = (v) =>
			{
				float fDistanceFromTarget = Bot.Character.Data.Position.Distance(v);
				if (!Bot.Character.Class.bWaitingForSpecial && Funky.Difference(Bot.Character.Data.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 5f)
				{
					if (fDistanceFromTarget > 45f)
						return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 45f);
					else
						return v;
				}

				return Vector3.Zero;
			};

			ClusterConditions.Add(new SkillClusterConditions(5d, 50, 7, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 35, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			
			//Reduced cool down.. lets use it more!
			if (Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.X1_Crusader_Passive_LordCommander))
			{
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 50, MinimumHealthPercent: 0d, falseConditionalFlags: TargetProperties.Weak));
			}
		}
	}
}
