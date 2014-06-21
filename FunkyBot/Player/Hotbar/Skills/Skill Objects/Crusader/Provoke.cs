using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Provoke : Skill
	{

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Provoke; }
		}


		public override double Cooldown { get { return 20000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override bool IsBuff { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		/*
		 * Rune Indexs
		 * 0 == Cleanse (Life on Hit increased)
		 * 1 == Flee Fool (Fear Enemies)
		 * 2 == Too Scared to Run (Reduced Attack Speed and Movement)
		 * 3 == Charged Up (Lightning Damage)
		 * 4 == Hit Me (Increased Block Chance)
		*/
		public override void Initialize()
		{
			Range = 20;
			Cost = 30;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);


			//Low On Wrath Buff Option (less than 10%)
			FcriteriaBuff = () => Bot.Character.Data.dCurrentEnergyPct < 0.10d;

			ClusterConditions.Add(new SkillClusterConditions(10d, 15f, 3, false));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 20, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			//Minimum Wrath Missing 10%
			FcriteriaCombat = () => Bot.Character.Data.dCurrentEnergyPct < 0.90d;
		}
	}
}
