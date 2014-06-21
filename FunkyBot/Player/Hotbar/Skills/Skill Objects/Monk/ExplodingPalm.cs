using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	public class ExplodingPalm : Skill
	{
		public override double Cooldown { get { return 5000; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{

			WaitVars = new WaitLoops(0, 0, false);
			Cost = 40;
			Range = 14;
			ShouldTrack = true;
			
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


		public override SNOPower Power
		{
			get { return SNOPower.Monk_ExplodingPalm; }
		}
	}
}
