using System;
using Zeta;
using Zeta.Common;
using Zeta.TreeSharp;
using Action=Zeta.TreeSharp.Action;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  private static DateTime LastBreak=DateTime.Now;
		  private static bool AFKBreak=false;
		  private static double BreakMinutes=0;
		  private static DateTime BreakStart=DateTime.Today;

		  private static bool MuleBehavior=false;
		  private static bool InitMuleBehavior=false;
		  private static bool CreatedCharacter=false;
		  private static bool RanProfile=false;
		  private static bool TransferedGear=false;
		  private static bool Finished=false;

		  public static bool OutOfGameOverlord(object ret)
		  {
				//Herbfunk
				if (SettingsFunky.EnableCoffeeBreaks&&
					 DateTime.Now.Subtract(LastBreak).TotalHours>=SettingsFunky.breakTimeHour)
				{
					 Logging.Write("Going AFK for a break..");
					 AFKBreak=true;
					 BreakStart=DateTime.Now;
					 BreakMinutes=MathEx.Random(SettingsFunky.MinBreakTime, SettingsFunky.MinBreakTime+SettingsFunky.MaxBreakTime);
					 return true;
				}
				else if (MuleBehavior)
				{
					 //Skip this until we create our new A1 game..
					 if (RanProfile&&!TransferedGear)
						  return false;

					 //Now we finish up..
					 if (RanProfile&&TransferedGear&&!Finished)
						  return true;

					 Log("Starting Mule Behavior");
					 CreatedCharacter=false;
					 RanProfile=false;
					 TransferedGear=false;

					 if (ZetaDia.Service.GameAccount.NumEmptyHeroSlots==0)
					 {
						  Log("No Empty Hero Slots Remain, and our stash if full.. stopping the bot!");
						  Zeta.CommonBot.BotMain.Stop(true, "Cannot stash anymore items!");
					 }
					 else
						  return true;
				}

				return false;
		  }

		  public static RunStatus OutOfGameBehavior(object ret)
		  {
				if (SettingsFunky.EnableCoffeeBreaks&&AFKBreak)
				{
					 if (DateTime.Now.Subtract(BreakStart).TotalMinutes>=BreakMinutes)
					 {
						  //Finished.
						  Logging.Write("Afk Break Finished..");
						  AFKBreak=false;
						  LastBreak=DateTime.Now;
						  return RunStatus.Success;
					 }

					 return RunStatus.Running;
				}
				else if (MuleBehavior)
				{
					 if (!InitMuleBehavior)
					 {
						  InitMuleBehavior=true;
						  NewMuleGame.BotHeroName=ZetaDia.Service.CurrentHero.Name;
						  NewMuleGame.BotHeroIndex=0;
						  NewMuleGame.LastProfile=Zeta.CommonBot.ProfileManager.CurrentProfile.Path;
					 }

					 if (!CreatedCharacter)
					 {
						  RunStatus NewHeroStatus=D3Character.CreateNewHero();

						  if (NewHeroStatus==RunStatus.Success)
						  {
								CreatedCharacter=true;
								//Setup Settings
								UpdateCurrentAccountDetails();
								LoadFunkyConfiguration();
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
					 else if(!Finished)
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
								UpdateCurrentAccountDetails();
								LoadFunkyConfiguration();

								return RunStatus.Success;
						  }
						  return RunStatus.Running;
					 }
				}

				return RunStatus.Success;
		  }
	 }
}