using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class BlessedShield : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_BlessedShield; }
		}

	
		public override double Cooldown { get { return 5; } }

		private readonly WaitLoops _waitVars = new WaitLoops(0, 4, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target | SkillExecutionFlags.ClusterTargetNearest; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 49;
			Cost = 20;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			ClusterConditions.Add(new SkillClusterConditions(10d, 35f, 3, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 20, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			
		}
	}
}