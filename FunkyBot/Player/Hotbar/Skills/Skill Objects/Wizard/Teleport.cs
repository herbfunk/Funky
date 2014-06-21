using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	public class Teleport : Skill
	{
		public override double Cooldown { get { return 16000; } }

		public override bool IsMovementSkill { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		private readonly ClusterConditions combatClusterCondition = new ClusterConditions(5d, 48f, 2, false);

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.ZigZagPathing; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 1, true);
			Cost = 15;
			Range = 50;
		

			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast |
									  SkillPrecastFlags.CheckEnergy));
			ClusterConditions.Add(new SkillClusterConditions(5d, 48f, 2, false));
			//TestCustomCombatConditionAlways=true,
			FcriteriaCombat = () => ((Bot.Settings.Wizard.bTeleportFleeWhenLowHP && Bot.Character.Data.dCurrentHealthPct < 0.5d)
								   ||
								   (Bot.Settings.Wizard.bTeleportIntoGrouping &&
									Bot.Targeting.Cache.Clusters.AbilityClusterCache(combatClusterCondition).Count > 0 &&
									Bot.Targeting.Cache.Clusters.AbilityClusterCache(combatClusterCondition)[0].Midpoint.Distance(
										Bot.Character.Data.PointPosition) > 15f)
								   || (!Bot.Settings.Wizard.bTeleportFleeWhenLowHP && !Bot.Settings.Wizard.bTeleportIntoGrouping));
			FCombatMovement = v =>
			{
				float fDistanceFromTarget = Bot.Character.Data.Position.Distance(v);
				if (!Bot.Character.Class.bWaitingForSpecial && Funky.Difference(Bot.Character.Data.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 25f)
				{
					if (fDistanceFromTarget > 50f)
						return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 50f);
					return v;
				}

				return Vector3.Zero;
			};
			FOutOfCombatMovement = v =>
			{
				float fDistanceFromTarget = Bot.Character.Data.Position.Distance(v);
				if (Funky.Difference(Bot.Character.Data.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 10f)
				{
					if (fDistanceFromTarget > 50f)
						return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 50f);
					return v;
				}

				return Vector3.Zero;
			};
		}

		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Teleport; }
		}
	}
}
