using System;
using System.Collections.Generic;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	public class Archon : Skill
	{
		//RuneIndex == 4 //Combustion
		//RuneIndex == 2 //Teleport
		//RuneIndex == 3 //FirePower
		//RuneIndex == 1 //Slow Time
		//RuneIndex == 0 //Improved

		public override void Initialize()
		{
			Cooldown = 100000;
			ExecutionType = SkillExecutionFlags.Buff;
			WaitVars = new WaitLoops(4, 5, true);
			Cost = 25;
			UseageType = SkillUseage.Combat;
			IsSpecialAbility = true;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			if (RuneIndex==4)
			{
				Range = 10;
				ClusterConditions.Add(new SkillClusterConditions(8d, 30, 10, true));
			}

			//Any non-normal unit within 30 yards that is 95% HP or less!
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 30, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		}


		#region IAbility

		public override int RuneIndex
		{
			get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; }
		}

		public override int GetHashCode()
		{
			return (int)Power;
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Skill p = (Skill)obj;
			return Power == p.Power;
		}

		#endregion

		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon; }
		}
	}
}
