using System;
using Zeta.Bot;

namespace FunkyBot.Player.HotBar.Skills.Conditions
{
	///<summary>
	///Skill Precast Conditions
	///</summary>
	public class SkillPreCast
	{
		public SkillPreCast(AbilityPreCastFlags flags)
		{
			Flags = flags;
			CreatePrecastCriteria();
		}
		public SkillPreCast(Func<Skill,bool> criteria )
		{
			Criteria = criteria;
		}
		public SkillPreCast()
		{
			Criteria += ((s) => true);
		}

		public AbilityPreCastFlags Flags { get; set; }
		public Func<Skill,bool> Criteria { get; set; } 

		private void CreatePrecastCriteria()
		{
			AbilityPreCastFlags precastconditions_ = Flags;
			if (precastconditions_.Equals(AbilityPreCastFlags.None))
			{
				Criteria += ((s) => true);
				return;
			}
			if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckPlayerIncapacitated))
				Criteria += ((s) => !Bot.Character.Data.bIsIncapacitated);

			if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckPlayerRooted))
				Criteria += ((s) => !Bot.Character.Data.bIsRooted);

			if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckExisitingBuff))
				Criteria += ((s) => !Bot.Character.Class.HotBar.HasBuff(s.Power));

			if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckPetCount))
				Criteria += ((s) => Bot.Character.Class.MainPetCount < s.Counter);

			if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckRecastTimer))
				Criteria += ((s) => s.LastUsedMilliseconds > s.Cooldown);

			if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckCanCast))
			{
				Criteria += ((s) =>
				{
					bool cancast = PowerManager.CanCast(s.Power, out s.CanCastFlags);


					if (!cancast && s.CanCastFlags.HasFlag(PowerManager.CanCastFlags.PowerNotEnoughResource))
					{
						if (s.IsSpecialAbility)
							Bot.Character.Class.bWaitingForSpecial = true;

						if (s.IsRanged || s.Range > 0)
							Bot.Character.Class.CanUseDefaultAttack = true;
					}
					//else if (IsSpecialAbility)
					//	 Bot.Character_.Class.bWaitingForSpecial=false;

					return cancast;
				});
			}

			if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckEnergy))
			{
				Criteria += ((s) =>
				{
					bool energyCheck = !s.SecondaryEnergy ? Bot.Character.Data.dCurrentEnergy >= s.Cost : Bot.Character.Data.dDiscipline >= s.Cost;
					if (s.IsSpecialAbility && !energyCheck) //we trigger waiting for special here.
						Bot.Character.Class.bWaitingForSpecial = true;
					if (!energyCheck && (s.IsRanged || s.Range > 0))
						Bot.Character.Class.CanUseDefaultAttack = true;

					return energyCheck;
				});
			}
		}
	}
}
