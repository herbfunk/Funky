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

				// World ID safety caching incase it's ever unavailable
				if (ZetaDia.CurrentWorldDynamicId!=-1) Bot.Character.iCurrentWorldID=ZetaDia.CurrentWorldDynamicId;


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
					 if (MuleBehavior)
					 {
						  if (BotMain.StatusText.Contains("Game Finished"))
						  {
								if (ZetaDia.Actors.GetActorsOfType<Zeta.Internals.Actors.Gizmos.GizmoPlayerSharedStash>(true, true).Any())
								{
									 return true;
								}
						  }
					 }

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


				if (Bot.SettingsFunky.Debug.DebugStatusBar&&Bot.bResetStatusText)
				{
					 Bot.bResetStatusText=false;
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
					 FunkyTrinity.AbilityFunky.Ability Buff;
					 if (Bot.Class.FindBuffPower(out Buff))
					 {
						  FunkyTrinity.AbilityFunky.Ability.SetupAbilityForUse(ref Buff);
						  Bot.Character.WaitWhileAnimating(4, true);
						  AbilityFunky.Ability.UsePower(ref Buff);
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