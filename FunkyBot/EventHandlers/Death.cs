using System;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Monitor;
using FunkyBot.Skills;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;

namespace FunkyBot.EventHandlers
{
	public partial class EventHandlers
	{
		internal static bool DeathShouldWait(object ret)
		{
			if (Bot.Settings.Death.WaitForPotionCooldown)
			{
				//Check Potion Cast Flags..
				PowerManager.CanCastFlags potionCastFlags;
				if (!PowerManager.CanCast(SNOPower.DrinkHealthPotion, out potionCastFlags))
				{
					if (potionCastFlags.HasFlag(PowerManager.CanCastFlags.Flag8))
					{
						BotMain.StatusText = "[Funky] Death: Waiting For Cooldowns!";
						return true;
					}
				}
			}

			if (Bot.Settings.Death.WaitForAllSkillsCooldown)
			{
				//Check Archon?
				if (Bot.Character.Class.AC==ActorClass.Wizard && Hotbar.HasBuff(SNOPower.Wizard_Archon))
				{
					Skill cancelSkill=Bot.Character.Class.Abilities[SNOPower.Wizard_Archon_Cancel];
					Skill.UsePower(ref cancelSkill);
					Hotbar.RefreshHotbar();
					BotMain.StatusText = "[Funky] Death: Waiting For Cooldowns!";
					return true;
				}

				foreach (var skill in Hotbar.HotbarSkills)
				{
					PowerManager.CanCastFlags skillCastFlags;
					if (!PowerManager.CanCast(skill.Power, out skillCastFlags))
					{
						if (skillCastFlags.HasFlag(PowerManager.CanCastFlags.Flag8))
						{
							BotMain.StatusText = "[Funky] Death: Waiting For Cooldowns!";
							return true;
						}
					}
				}
			}

			return false;
		}

		internal static RunStatus DeathWaitAction(object ret)
		{

			if (Bot.Settings.Death.WaitForPotionCooldown)
			{
				//Check Potion Cast Flags..
				PowerManager.CanCastFlags potionCastFlags;
				if (!PowerManager.CanCast(SNOPower.DrinkHealthPotion, out potionCastFlags))
				{
					if (potionCastFlags.HasFlag(PowerManager.CanCastFlags.Flag8))
					{
						InactivityDetector.Reset();
						return RunStatus.Running;
					}
				}
			}

			if (Bot.Settings.Death.WaitForAllSkillsCooldown)
			{
				if (Bot.Character.Class.AC == ActorClass.Wizard && Hotbar.HasBuff(SNOPower.Wizard_Archon))
				{
					Hotbar.RefreshHotbar();
					InactivityDetector.Reset();
					return RunStatus.Running;
				}

				foreach (var skill in Hotbar.HotbarSkills)
				{
					PowerManager.CanCastFlags skillCastFlags;
					if (!PowerManager.CanCast(skill.Power, out skillCastFlags))
					{
						if (skillCastFlags.HasFlag(PowerManager.CanCastFlags.Flag8))
						{
							InactivityDetector.Reset();
							return RunStatus.Running;
						}
					}
				}
			}

			GoldInactivity.LastCoinageUpdate = DateTime.Now;
			return RunStatus.Success;
		}


		internal static bool TallyedDeathCounter = false;
		internal static bool TallyDeathCanRunDecorator(object ret)
		{
			return !TallyedDeathCounter;
		}
		internal static RunStatus TallyDeathAction(object ret)
		{
			FunkyGame.CurrentGameStats.CurrentProfile.DeathCount++;
			//Bot.Game.CurrentGameStats.CurrentProfile.DeathCount++;
			TallyedDeathCounter = true;
			return RunStatus.Success;
		}
	}
}
