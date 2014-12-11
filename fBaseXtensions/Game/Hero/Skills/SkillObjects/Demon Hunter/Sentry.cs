using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter
{
	public class Sentry : Skill
	{
		public override double Cooldown { get { return 6000; } }
        public override bool IsRanged { get { return true; } }
		public override bool IsSpecialAbility { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation; } }

		private bool FullMarauderSetBonus = false;
		private bool BombadiersRucksack = false;
		public override void Initialize()
		{
			FullMarauderSetBonus = Equipment.CheckLegendaryItemCount(LegendaryItemTypes.EmbodimentoftheMarauder, 6);
			BombadiersRucksack=Equipment.CheckLegendaryItemCount(LegendaryItemTypes.BombadiersRucksack);

			WaitVars = new WaitLoops(0, 0, true);
			Cost = 30;
			Range = 55;
			
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast
			{
				Flags = SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast,
			};
			PreCast.Criteria += skill => FunkyGame.Targeting.Cache.Environment.HeroPets.DemonHunterSentry < SentryMax();
			PreCast.CreatePrecastCriteria();

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			//Any unit when our energy is greater than 90%!
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
                Criteria = (unit) => FunkyGame.Hero.dCurrentEnergyPct > (FullMarauderSetBonus ? 0.5d : 0.9d),
				MaximumDistance = Range,
				FalseConditionFlags = TargetProperties.LowHealth,
			});

			ClusterConditions.Add(new SkillClusterConditions(6d, Range, 3, true));


			FcriteriaCombat = (u) => FunkyGame.Targeting.Cache.Environment.HeroPets.DemonHunterSentry < SentryMax();
		}

		private int SentryMax()
		{
			int n=2;

			//Custom Engineering - Max 3
			if (Hotbar.PassivePowers.Contains(SNOPower.DemonHunter_Passive_CustomEngineering))
				n++;
			
			//Bombardier's Rucksack - Additional 2
			if (BombadiersRucksack)
				n += 2;

			return n;
		}


		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_Sentry; }
		}
	}
}
