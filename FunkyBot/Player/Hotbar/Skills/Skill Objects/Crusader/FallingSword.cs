﻿using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class FallingSword : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_FallingSword; }
		}

		public override void Initialize()
		{
			Cooldown = 30000;
			Cost = 25;
			Range = 35;
			Priority = SkillPriority.Medium;
			ExecutionType = SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			IsASpecialMovementPower = true;

			ClusterConditions.Add(new SkillClusterConditions(5d, 35, 6, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 35, 0.95d, TargetProperties.Normal));

			FCombatMovement = (v) =>
			{
				float fDistanceFromTarget = Bot.Character.Data.Position.Distance(v);
				if (!Bot.Character.Class.bWaitingForSpecial && Funky.Difference(Bot.Character.Data.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
					if (fDistanceFromTarget > 35f)
						return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 35f);
					else
						return v;
				}

				return Vector3.Zero;
			};
			FOutOfCombatMovement = (v) =>
			{
				float fDistanceFromTarget = Bot.Character.Data.Position.Distance(v);
				if (Funky.Difference(Bot.Character.Data.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
					if (fDistanceFromTarget > 35f)
						return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 35f);
					else
						return v;
				}

				return Vector3.Zero;
			};
		}
	}
}