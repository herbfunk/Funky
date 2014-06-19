using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class WrathOfTheBerserker : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_WrathOfTheBerserker; }
		}

		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override void Initialize()
		{
			Cooldown = 120500;
			ExecutionType = SkillExecutionFlags.Buff;
			WaitVars = new WaitLoops(4, 4, true);
			Cost = 50;
			UseageType = SkillUseage.Anywhere;
			IsSpecialAbility = Bot.Settings.Barbarian.bWaitForWrath;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckExisitingBuff | SkillPrecastFlags.CheckCanCast);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 40, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			
			if (Bot.Settings.Barbarian.bBarbUseWOTBAlways)
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));

			FcriteriaCombat = () =>
			{
				//Anytime?
				if (Bot.Settings.Barbarian.bBarbUseWOTBAlways)
					return true;

				//Treasure Goblins?
				if (Bot.Targeting.Cache.CurrentTarget.IsTreasureGoblin)
					return Bot.Settings.Barbarian.bGoblinWrath;

				//Normal Targets?
				if (Bot.Targeting.Cache.CurrentTarget.Properties.HasFlag(TargetProperties.Normal)) 
					return false;


				return true;
			};
		}

		#region IAbility
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
			else
			{
				Skill p = (Skill)obj;
				return Power == p.Power;
			}
		}


		#endregion
	}
}
