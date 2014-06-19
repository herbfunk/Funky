using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class AncientSpear : Skill
	{

		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Barbarian_AncientSpear; }
		}

		public override void Initialize()
		{
			Cooldown=300;
			ExecutionType = SkillExecutionFlags.Target;
			WaitVars = new WaitLoops(2, 2, true);
			Range = 35;
			IsRanged = true;
			IsProjectile=true;
			UseageType=SkillUseage.Combat;
			Priority = SkillPriority.Medium;
			PreCast=new SkillPreCast((SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckCanCast |
			                          SkillPrecastFlags.CheckPlayerIncapacitated));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Ranged, maxdistance: 25, MinimumHealthPercent: 0.50d));
								
								//TestCustomCombatConditionAlways=true,
			FcriteriaCombat = () => Bot.Targeting.Cache.CurrentUnitTarget.IsRanged ||
			                        Bot.Character.Data.dCurrentEnergyPct < 0.5d;
		}

		#region IAbility
		public override int GetHashCode()
		{
			 return (int)Power;
		}
		public override bool Equals(object obj)
		{
			 //Check for null and compare run-time types. 
			 if (obj==null||GetType()!=obj.GetType())
			 {
				  return false;
			 }
			Skill p=(Skill)obj;
			return Power==p.Power;
		}

		#endregion
	}
}
