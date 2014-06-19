using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	public class ExplodingPalm : Skill
	{
		public override void Initialize()
		{
			Cooldown = 5000;
			ExecutionType = SkillExecutionFlags.Target;
			WaitVars = new WaitLoops(0, 0, false);
			Cost = 40;
			Range = 14;
			ShouldTrack = true;
			UseageType = SkillUseage.Combat;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated);

			SingleUnitCondition.Add(
				new UnitTargetConditions 
				{ 
				  TrueConditionFlags = TargetProperties.None,
				  MaximumDistance=25,
				  Criteria = () => !Bot.Targeting.Cache.CurrentTarget.SkillsUsedOnObject.ContainsKey(Power) || DateTime.Now.Subtract(Bot.Targeting.Cache.CurrentTarget.SkillsUsedOnObject[Power]).TotalSeconds>(RuneIndex==0?12:9)
				});

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
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
			get { return SNOPower.Monk_ExplodingPalm; }
		}
	}
}
