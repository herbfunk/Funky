using System;
using System.Globalization;
using System.Linq;
using FunkyBot.Config.Settings;
using FunkyBot.Game;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;

namespace FunkyBot.DBHandlers
{
	public static class OutOfGame
	{

		internal static bool MuleBehavior = false;
		private static bool InitMuleBehavior;
		private static bool CreatedCharacter;
		private static bool RanProfile;
		internal static bool TransferedGear = false;
		internal static bool Finished = false;

		public static bool OutOfGameOverlord(object ret)
		{
			if (MuleBehavior)
			{
				Logger.DBLog.Warn("Cannot Stash anymore Items and Mule behavior is disabled!");
				//if (!Bot.Settings.Plugin.CreateMuleOnStashFull)
				//{
					BotMain.Stop(true, "Cannot stash anymore items!");
					return false;
				//}

				//Skip this until we create our new A1 game..
				if (RanProfile && !TransferedGear)
					return false;

				//Now we finish up..
				if (RanProfile && TransferedGear && !Finished) return true;


				Logger.Write(LogLevel.OutOfGame, "Starting Mule Behavior");
				CreatedCharacter = false;
				RanProfile = false;
				TransferedGear = false;

				if (ZetaDia.Service.GameAccount.NumEmptyHeroSlots == 0)
				{
					Logger.Write(LogLevel.OutOfGame, "No Empty Hero Slots Remain, and our stash if full.. stopping the bot!");
					BotMain.Stop(true, "Cannot stash anymore items!");
				}
				else
					return true;
			}

			return false;
		}

		public static RunStatus OutOfGameBehavior(object ret)
		{
			if (MuleBehavior)
			{
				if (!InitMuleBehavior)
				{
					Finished = false;
					InitMuleBehavior = true;
					NewMuleGame.BotHeroName = ZetaDia.Service.Hero.Name;
					NewMuleGame.BotHeroIndex = 0;
					NewMuleGame.LastProfile = ProfileManager.CurrentProfile.Path;
					NewMuleGame.LastHandicap = CharacterSettings.Instance.MonsterPowerLevel;
				}

				if (!CreatedCharacter)
				{
					RunStatus NewHeroStatus = CreateNewHero();

					if (NewHeroStatus == RunStatus.Success)
					{
						CreatedCharacter = true;
						//Setup Settings
						Bot.Character.Account.UpdateCurrentAccountDetails();
						Settings_Funky.LoadFunkyConfiguration();
					}
					return RunStatus.Running;
				}

				if (!RanProfile)
				{
					RunStatus NewGameStatus = NewMuleGame.BeginNewGameProfile();
					if (NewGameStatus == RunStatus.Success)
					{
						RanProfile = true;
						return RunStatus.Success;
					}
					return RunStatus.Running;
				}

				RunStatus FinishStatus = NewMuleGame.FinishMuleBehavior();
				if (FinishStatus == RunStatus.Success)
				{
					Finished = true;
					RanProfile = false;
					CreatedCharacter = false;
					InitMuleBehavior = false;
					MuleBehavior = false;
					//Load Settings
					Bot.Character.Account.UpdateCurrentAccountDetails();
					Settings_Funky.LoadFunkyConfiguration();

					return RunStatus.Success;
				}
				return RunStatus.Running;

			}

			return RunStatus.Success;
		}

		public static RunStatus CreateNewHero()
		{
			if (DateTime.Now.Subtract(LastActionTaken).TotalMilliseconds > 1000)
			{
				if (UI.ValidateUIElement(UI.GameMenu.SwitchHeroButton))
				{
					if (NewCharacterName == null)
					{
						UI.GameMenu.SwitchHeroButton.Click();
					}
					else if (ZetaDia.Service.Hero.Name == NewCharacterName)
					{
						Logger.Write(LogLevel.OutOfGame, "Successfully Created New Character");
						return RunStatus.Success;
					}
				}
				else if (UI.ValidateUIElement(UI.GameMenu.CreateHeroButton))
				{
					UI.GameMenu.CreateHeroButton.Click();
				}
				else if (UI.ValidateUIElement(UI.GameMenu.HeroNameText))
				{
					if (!SelectedClass)
					{
						UIElement thisClassButton = UI.GameMenu.SelectHeroType(ActorClass.DemonHunter);
						if (thisClassButton != null && thisClassButton.IsValid && thisClassButton.IsEnabled && thisClassButton.IsVisible)
						{
							thisClassButton.Click();
							SelectedClass = true;
						}
					}
					else
					{
						if (NewCharacterName == null)
							NewCharacterName = GenerateRandomText();

						if (UI.GameMenu.HeroNameText.IsValid)
						{
							Logger.Write(LogLevel.OutOfGame, "Valid TextObject for character name UI");
						}

						if (!UI.GameMenu.HeroNameText.HasText)
						{
							UI.GameMenu.HeroNameText.SetText(NewCharacterName.Substring(0, 1));
						}
						else
						{
							if (UI.GameMenu.HeroNameText.Text != NewCharacterName)
							{
								UI.GameMenu.HeroNameText.SetText(NewCharacterName.Substring(0, UI.GameMenu.HeroNameText.Text.Length + 1));
							}
							else if (UI.ValidateUIElement(UI.GameMenu.CreateNewHeroButton))
							{
								UI.GameMenu.CreateNewHeroButton.Click();
							}
						}
					}
				}

				LastActionTaken = DateTime.Now;
			}
			return RunStatus.Running;
		}
		private static DateTime LastActionTaken = DateTime.Today;
		private static bool SelectedClass = false;

		internal static string NewCharacterName = null;

		private static string GenerateRandomText()
		{
			var chars = "abcdefghijklmnopqrstuvwxyz";
			var random = new Random();
			var result = new string(
				 Enumerable.Repeat(chars, random.Next(5, 8))
				 .Select(s => s[random.Next(s.Length)])
				 .ToArray());

			Logger.DBLog.InfoFormat("Generated Name " + result);
			return result;
		}


	}
}