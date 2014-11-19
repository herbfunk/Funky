using System;
using Zeta.Bot;

namespace fBaseXtensions.Game.Hero.Skills.Conditions
{
	///<summary>
	///Skill Precast Conditions
	///</summary>
	public class SkillPreCast
	{
		public SkillPreCast(SkillPrecastFlags flags)
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

		public SkillPrecastFlags Flags { get; set; }
		public Func<Skill,bool> Criteria { get; set; } 

		internal void CreatePrecastCriteria()
		{
			SkillPrecastFlags precastconditions_ = Flags;
			if (precastconditions_.Equals(SkillPrecastFlags.None))
			{
				Criteria += ((s) => true);
				return;
			}
			if (precastconditions_.HasFlag(SkillPrecastFlags.CheckPlayerIncapacitated))
				Criteria += ((s) => !FunkyGame.Hero.bIsIncapacitated);

			if (precastconditions_.HasFlag(SkillPrecastFlags.CheckPlayerRooted))
				Criteria += ((s) => !FunkyGame.Hero.bIsRooted);

			if (precastconditions_.HasFlag(SkillPrecastFlags.CheckExisitingBuff))
				Criteria += ((s) => !Hotbar.HasBuff(s.Power));

			if (precastconditions_.HasFlag(SkillPrecastFlags.CheckPetCount))
				Criteria += ((s) => FunkyGame.Hero.Class.MainPetCount < s.Counter);

			if (precastconditions_.HasFlag(SkillPrecastFlags.CheckRecastTimer))
				Criteria += ((s) => s.LastUsedMilliseconds > s.Cooldown);

			if (precastconditions_.HasFlag(SkillPrecastFlags.CheckEnergy))
			{
				Criteria += ((s) =>
				{
					bool energyCheck = !s.SecondaryEnergy ? FunkyGame.Hero.dCurrentEnergy >= s.Cost : FunkyGame.Hero.dDiscipline >= s.Cost;
					if (s.IsSpecialAbility && !energyCheck) //we trigger waiting for special here.
						FunkyGame.Hero.Class.bWaitingForSpecial = true;
					if (!energyCheck && (s.IsRanged || s.Range > 0))
						FunkyGame.Hero.Class.CanUseDefaultAttack = true;

					return energyCheck;
				});
			}

			if (precastconditions_.HasFlag(SkillPrecastFlags.CheckCanCast))
			{
				Criteria += ((s) =>
				{
					bool cancast = PowerManager.CanCast(s.Power, out s.CanCastFlags);

					//PowerManager.CanCastFlags.Flag80; (Not enough Resource?)
					//PowerManager.CanCastFlags.Flag8; (On Cooldown?)
					if (!cancast && s.CanCastFlags.HasFlag(PowerManager.CanCastFlags.Flag80))
					{
						if (s.IsSpecialAbility)
							FunkyGame.Hero.Class.bWaitingForSpecial = true;

						if (s.IsRanged || s.Range > 0)
							FunkyGame.Hero.Class.CanUseDefaultAttack = true;
					}
					//else if (s.IsSpecialAbility)
						//FunkyGame.Hero.Class.bWaitingForSpecial = false;

					return cancast;
				});
			}


		}
	}
}
