using System;
using FunkyBot.Misc;
using FunkyBot.Player.HotBar.Skills;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals;
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
				if (Bot.Character.Class.AC==ActorClass.Wizard && Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_Archon))
				{
					Skill cancelSkill=Bot.Character.Class.Abilities[SNOPower.Wizard_Archon_Cancel];
					Skill.UsePower(ref cancelSkill);
					Bot.Character.Class.HotBar.RefreshHotbar();
					BotMain.StatusText = "[Funky] Death: Waiting For Cooldowns!";
					return true;
				}

				foreach (var skill in Bot.Character.Class.HotBar.HotbarPowers)
				{
					PowerManager.CanCastFlags skillCastFlags;
					if (!PowerManager.CanCast(skill, out skillCastFlags))
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
				if (Bot.Character.Class.AC == ActorClass.Wizard && Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_Archon))
				{
					Bot.Character.Class.HotBar.RefreshHotbar();
					InactivityDetector.Reset();
					return RunStatus.Running;
				}

				foreach (var skill in Bot.Character.Class.HotBar.HotbarPowers)
				{
					PowerManager.CanCastFlags skillCastFlags;
					if (!PowerManager.CanCast(skill, out skillCastFlags))
					{
						if (skillCastFlags.HasFlag(PowerManager.CanCastFlags.Flag8))
						{
							InactivityDetector.Reset();
							return RunStatus.Running;
						}
					}
				}
			}

			Bot.Game.GoldTimeoutChecker.LastCoinageUpdate = DateTime.Now;
			return RunStatus.Success;
		}


		internal static bool TallyedDeathCounter = false;
		internal static bool TallyDeathCanRunDecorator(object ret)
		{
			return !TallyedDeathCounter;
		}
		internal static RunStatus TallyDeathAction(object ret)
		{
			Bot.Game.CurrentGameStats.CurrentProfile.DeathCount++;
			TallyedDeathCounter = true;
			return RunStatus.Success;
		}
	}
}
