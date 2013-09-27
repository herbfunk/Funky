using System;
using Zeta;
using System.Linq;
using Zeta.Common;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using Zeta.CommonBot;
using Zeta.TreeSharp;
using FunkyTrinity.Cache;

namespace FunkyTrinity
{
	 public partial class Funky
	 {


		  // Total main loops so we can update things every XX loops
		  private static int iCombatLoops=0;


		  private static bool GlobalOverlord(object ret)
		  {
				// If we aren't in the game of a world is loading, don't do anything yet
				if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld)
				{
					 Bot.Combat.lastChangedZigZag=DateTime.Today;
					 Bot.Combat.vPositionLastZigZagCheck=Vector3.Zero;
					 return false;
				}

				if (Bot.ShuttingDownBot)
				{
					 Logging.Write("Stopping Bot due to low health!");
					 Bot.ShutDownBot();
					 return false;
				}

				// World ID safety caching incase it's ever unavailable
				if (ZetaDia.CurrentWorldDynamicId!=-1) Bot.Character.iCurrentWorldID=ZetaDia.CurrentWorldDynamicId;

				//Check Low Level Logic Setting
				if (Bot.SettingsFunky.UseLevelingLogic) LowLevelLogicPulse();
				

				// Store all of the player's abilities every now and then, to keep it cached and handy, also check for critical-mass timer changes etc.
				iCombatLoops++;
				if (Bot.Class==null||iCombatLoops>=50)
				{
					 // Update the cached player's cache
					 ActorClass tempClass=ActorClass.Invalid;
					 try
					 {
						  tempClass=ZetaDia.Actors.Me.ActorClass;
					 } catch (NullReferenceException)
					 {
						  Logging.WriteDiagnostic("[Funky] Safely handled exception trying to get character class.");
					 }


					 if (tempClass!=ActorClass.Invalid&&Bot.Class==null)
					 {
						  //Create Specific Player Class
						  switch (tempClass)
						  {
								case ActorClass.Barbarian:
									 Bot.Class=new Barbarian(tempClass);
									 break;
								case ActorClass.DemonHunter:
									 Bot.Class=new DemonHunter(tempClass);
									 break;
								case ActorClass.Monk:
									 Bot.Class=new Monk(tempClass);
									 break;
								case ActorClass.WitchDoctor:
									 Bot.Class=new WitchDoctor(tempClass);
									 break;
								case ActorClass.Wizard:
									 Bot.Class=new Wizard(tempClass);
									 break;
						  }

						  Bot.Class.RecreateAbilities();

					 }

					 iCombatLoops=0;

					 //Set Character Radius?
					 if (Bot.Character.fCharacterRadius==0f)
					 {
						  Bot.Character.fCharacterRadius=ZetaDia.Me.ActorInfo.Sphere.Radius;

						  //Wizards are short -- causing issues (At least Male Wizard is!)
						  if (Bot.ActorClass==ActorClass.Wizard) Bot.Character.fCharacterRadius+=1f;
					 }
					 if (Bot.Character.iTeamID==0)
					 {
						  try
						  {
								Bot.Character.iTeamID=ZetaDia.Me.CommonData.GetAttribute<int>(ActorAttributeType.TeamID);
						  } catch
						  {

						  }
					 }

					 // Game difficulty, used really for vault on DH's
					 //if (ZetaDia.Service.CurrentHero.CurrentDifficulty!=GameDifficulty.Invalid)
					 //    Bot.Character.iCurrentGameDifficulty=ZetaDia.Service.CurrentHero.CurrentDifficulty;
				}

				// Recording of all the XML's in use this run
				Bot.Profile.CheckProfile();


				//Seconday Hotbar Check
				Bot.Class.SecondaryHotbarBuffPresent();
				

				// Clear target current and reset key variables used during the target-handling function
				Bot.Combat.ResetTargetHandling();
				Bot.Combat.DontMove=false;

				//update current profile behavior.
				Bot.Profile.CheckCurrentProfileBehavior();


				// Should we refresh target list?
				if (Bot.Target.ShouldRefreshObjectList)
				{
					 Bot.Target.RefreshDiaObjects();

					 // We have a target, start the target handler!
					 if (Bot.Target.CurrentTarget!=null)
					 {
						  //Backtracking?
						  //if (Bot.Character.IsRunningInteractiveBehavior&&!Bot.Character.ShouldBackTrack)
						  //{
						  //    Bot.Character.BackTrackVector=Bot.Character.Position;
						  //    Bot.Character.ShouldBackTrack=true;
						  //}

						  Bot.Combat.bWholeNewTarget=true;
						  Bot.Combat.DontMove=true;
						  Bot.Combat.bPickNewAbilities=true;
						  return true;
					 }
				}
				else
				{
					 //Check OOC ID Behavior..
					 if (Bot.SettingsFunky.OOCIdentifyItems&&ShouldRunIDBehavior())
					 {
						  Logging.WriteDiagnostic("[Funky] Starting OOC ID Behavior");
						  Bot.Combat.DontMove=true;
						  return true;
					 }
					 else if (MuleBehavior)
					 {
						  if (BotMain.StatusText.Contains("Game Finished"))
						  {
								if (ZetaDia.Actors.GetActorsOfType<Zeta.Internals.Actors.Gizmos.GizmoPlayerSharedStash>(true, true).Any())
								{
									 //Zeta.CommonBot.BotMain.CurrentBot.Logic.Stop(null);
									 return true;
								}
						  }
					 }
					 // Only do something when pulsing if it's been at least 5 seconds since last pulse, to prevent spam
					 else if (Bot.SettingsFunky.UseLevelingLogic&&Bot.Character.iMyLevel<60&&DateTime.Now.Subtract(_lastLooked).TotalSeconds>5)
					 {
						  // Every 5 minutes, re-check all equipped items and clear stored blacklist
						  if (DateTime.Now.Subtract(_lastFullEvaluation).TotalSeconds>300)
						  {
								bNeedFullItemUpdate=true;
						  }
						  // Now check the backpack
						  CheckBackpack();
					 }
					 //else if (Bot.Character.ShouldBackTrack)
					 //{
					 //    if (Bot.Character.Position.Distance(Bot.Character.BackTrackVector)>7.5f)
					 //    {
					 //        Logging.WriteVerbose("BackTracking back to orginal location");
					 //        Logging.WriteVerbose("Current Vector used {0}", Bot.Character.BackTrackVector.ToString());
					 //        //Return to the vector set.
					 //        Bot.Target.CurrentTarget=new CacheObject(Bot.Character.BackTrackVector, Enums.TargetType.Avoidance, 20000, "BackTrack", 5f);
					 //        Bot.Combat.DontMove=true;
					 //        return true;
					 //    }
					 //}

					 // Return false here means we only do all of the below OOC stuff at max once every 150ms
					 return false;
				}

				// Pop a potion when necessary
				if (Bot.Class.HealthPotionAbility.CheckPreCastConditionMethod())
				{
					 if (Bot.Class.HealthPotionAbility.CheckCustomCombatMethod())
					 {
						  Bot.Class.HealthPotionAbility.AttemptToUseHealthPotion();
					 }
				}

			  BlacklistCache.CheckRefreshBlacklists();


				if (Bot.SettingsFunky.Debug.DebugStatusBar&&bResetStatusText)
				{
					 bResetStatusText=false;
					 BotMain.StatusText="[Funky] No more targets - DemonBuddy/profile management is now in control";
				}

				// Nothing to do... do we have some maintenance we can do instead, like out of combat buffing?
				Bot.Combat.lastChangedZigZag=DateTime.Today;
				Bot.Combat.vPositionLastZigZagCheck=Vector3.Zero;

				// Out of combat buffing etc. but only if we don't want to return to town etc.
				Bot.Character.UpdateAnimationState(true, false);
				AnimationState myAnimationState=Bot.Character.CurrentAnimationState;
				if ((!Bot.Character.bIsInTown||Bot.SettingsFunky.AllowBuffingInTown)&&
					 !TownRunManager.bWantToTownRun&&
					 myAnimationState!=AnimationState.Attacking&&myAnimationState!=AnimationState.Casting&&myAnimationState!=AnimationState.Channeling)
				{
					 FunkyTrinity.Ability.ability Buff;
					 if (Bot.Class.FindBuffPower(out Buff))
					 {
						  FunkyTrinity.Ability.ability.SetupAbilityForUse(ref Buff);
						  Bot.Character.WaitWhileAnimating(4, true);
						  Ability.ability.UsePower(ref Buff);
						  Buff.SuccessfullyUsed();
						  Bot.Character.WaitWhileAnimating(3, true);
					 }
				}




				// Ok let DemonBuddy do stuff this loop, since we're done for the moment
				return false;
		  }

		  //Used when we actually want to handle a target!
		  public static RunStatus HandleTarget(object ret)
		  {
				if (shouldPreformOOCItemIDing)
					 return HandleIDBehavior(); //Check if we are doing OOC ID behavior..
				else if (Bot.Target.CurrentTarget!=null)
					 return Bot.Target.HandleThis();  //Default Behavior: Current Target
				else if (MuleBehavior)
				{
					 if (!TransferedGear)
					 {
						  return NewMuleGame.StashTransfer();
					 }
					 else if (!Finished)
					 {
						  return NewMuleGame.FinishMuleBehavior();
					 }
				}

				return RunStatus.Success;
		  }
	 }
}