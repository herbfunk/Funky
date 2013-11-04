using System;
using FunkyBot.Settings;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.TreeSharp;
using Action=Zeta.TreeSharp.Action;

namespace FunkyBot
{
	 public partial class Funky
	 {

		  private static bool MuleBehavior=false;
		  private static bool InitMuleBehavior=false;
		  private static bool CreatedCharacter=false;
		  private static bool RanProfile=false;
		  private static bool TransferedGear=false;
		  private static bool Finished=false;

		  public static bool OutOfGameOverlord(object ret)
		  {
				if (MuleBehavior)
				{
					 if (!Bot.Settings.Plugin.CreateMuleOnStashFull)
					 {
						  Zeta.CommonBot.BotMain.Stop(true, "Cannot stash anymore items!");
						  return false;
					 }

					 //Skip this until we create our new A1 game..
					 if (RanProfile&&!TransferedGear)
						  return false;

					 //Now we finish up..
					 if (RanProfile&&TransferedGear&&!Finished)
						  return true;
					 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.OutOfGame))
						  Logger.Write(LogLevel.OutOfGame, "Starting Mule Behavior");
					 CreatedCharacter=false;
					 RanProfile=false;
					 TransferedGear=false;

					 if (ZetaDia.Service.GameAccount.NumEmptyHeroSlots==0)
					 {
						  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.OutOfGame))
								Logger.Write(LogLevel.OutOfGame, "No Empty Hero Slots Remain, and our stash if full.. stopping the bot!");
						  Zeta.CommonBot.BotMain.Stop(true, "Cannot stash anymore items!");
					 }
					 else
						  return true;
				}

				//Change the Monster Power!
				if (Bot.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings)
				{
                    int overridePowerLevel=Bot.Settings.Demonbuddy.MonsterPower;
                    Logging.Write("[Funky] Overriding Monster Power Level to {0}", overridePowerLevel.ToString());
                    Zeta.CommonBot.Settings.CharacterSettings.Instance.MonsterPowerLevel = overridePowerLevel;
				}

				////Disconnect -- Starting Profile Setup.
				//if (FunkyErrorClicker.FunkyErrorClicker.HadDisconnectError)
				//{
				//	 Logging.Write("[Funky] Disconnected Last Game.. Reloading Current Profile.");
				//	 //ReloadStartingProfile();
				//	 ProfileManager.Load(Zeta.CommonBot.ProfileManager.CurrentProfile.Path);
				//	 FunkyErrorClicker.FunkyErrorClicker.HadDisconnectError=false;
				//}


				return false;
		  }

		  public static RunStatus OutOfGameBehavior(object ret)
		  {
				if (MuleBehavior)
				{
					 if (!InitMuleBehavior)
					 {
						  InitMuleBehavior=true;
						  NewMuleGame.BotHeroName=ZetaDia.Service.CurrentHero.Name;
						  NewMuleGame.BotHeroIndex=0;
						  NewMuleGame.LastProfile=Zeta.CommonBot.ProfileManager.CurrentProfile.Path;
						  NewMuleGame.LastHandicap=Zeta.CommonBot.Settings.CharacterSettings.Instance.MonsterPowerLevel;
					 }

					 if (!CreatedCharacter)
					 {
						  RunStatus NewHeroStatus=D3Character.CreateNewHero();

						  if (NewHeroStatus==RunStatus.Success)
						  {
								CreatedCharacter=true;
								//Setup Settings
								Bot.Game.UpdateCurrentAccountDetails();
								Settings_Funky.LoadFunkyConfiguration();
						  }
						  return RunStatus.Running;
					 }
					 else if (!RanProfile)
					 {
						  RunStatus NewGameStatus=NewMuleGame.BeginNewGameProfile();
						  if (NewGameStatus==RunStatus.Success)
						  {
								RanProfile=true;
								return RunStatus.Success;
						  }
						  return RunStatus.Running;
					 }
					 else if (!Finished)
					 {
						  RunStatus FinishStatus=NewMuleGame.FinishMuleBehavior();
						  if (FinishStatus==RunStatus.Success)
						  {
								Finished=true;
								RanProfile=false;
								CreatedCharacter=false;
								InitMuleBehavior=false;
								MuleBehavior=false;
								//Load Settings
								Bot.Game.UpdateCurrentAccountDetails();
								Settings_Funky.LoadFunkyConfiguration();

								return RunStatus.Success;
						  }
						  return RunStatus.Running;
					 }
				}

				return RunStatus.Success;
		  }
	 }
}