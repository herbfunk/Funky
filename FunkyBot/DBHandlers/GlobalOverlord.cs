using System;
using FunkyBot.DBHandlers;
using FunkyBot.Player.HotBar.Skills;
using FunkyBot.Cache;
using Zeta;
using System.Linq;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.CommonBot;
using Zeta.TreeSharp;
using FunkyBot.Player.Class;

namespace FunkyBot
{
	 public partial class Funky
	 {
		  private static bool GlobalOverlord(object ret)
		  {
				// If we aren't in the game of a world is loading, don't do anything yet
				if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld||ZetaDia.Me==null||ZetaDia.Me.CommonData==null)
				{
					 Bot.NavigationCache.lastChangedZigZag=DateTime.Today;
					 Bot.NavigationCache.vPositionLastZigZagCheck=Vector3.Zero;
					 return false;
				}

			  //check if we initialized the bot..
				if (Bot.Character.Class == null)
					PlayerClass.CreateBotClass();

				//Seconday Hotbar Check
				Bot.Character.Class.SecondaryHotbarBuffPresent();

				// Clear target current and reset key variables used during the target-handling function
				Bot.Targeting.ResetTargetHandling();
				Bot.Targeting.DontMove=false;

				//update current profile behavior.
				Bot.Game.Profile.CheckCurrentProfileBehavior();


				// Should we refresh target list?
				if (Bot.Targeting.ShouldRefreshObjectList)
				{
					 Bot.Targeting.RefreshDiaObjects();

					 // We have a target, start the target handler!
					 if (Bot.Targeting.CurrentTarget!=null)
					 {
						  Bot.Targeting.TargetMover.RestartTracking();
						  Bot.Targeting.bWholeNewTarget=true;
						  Bot.Targeting.DontMove=true;
						  Bot.Targeting.bPickNewAbilities=true;
						  Bot.RunningTargetingBehavior = true;
						  //Bot.Targeting.StartingLocation = Bot.Character_.Data.Position;
						  return true;
					 }
				}
				else
				{
					if (OutOfGame.MuleBehavior)
					 {
						  if (BotMain.StatusText.Contains("Game Finished"))
						  {
								if (ZetaDia.Actors.GetActorsOfType<Zeta.Internals.Actors.Gizmos.GizmoPlayerSharedStash>(true, true).Any())
								{
									 return true;
								}
						  }
					 }
					else if(Bot.Game.GoldTimeoutChecker.TimeoutTripped)
					{
						Bot.Game.GoldTimeoutChecker.BehaviorEngaged = true;
						return true;
					}

					 return false;
				}

				// Pop a potion when necessary
				if (Bot.Character.Class.HealthPotionAbility.CheckPreCastConditionMethod())
				{
					 if (Bot.Character.Class.HealthPotionAbility.CheckCustomCombatMethod())
					 {
						  Bot.Character.Class.HealthPotionAbility.AttemptToUseHealthPotion();
					 }
				}

				BlacklistCache.CheckRefreshBlacklists();


				if (Bot.Settings.Debug.DebugStatusBar&&bResetStatusText)
				{
					 bResetStatusText=false;
					 BotMain.StatusText="[Funky] No more targets - DemonBuddy/profile management is now in control";
				}

				Bot.RunningTargetingBehavior = false;
				// Nothing to do... do we have some maintenance we can do instead, like out of combat buffing?
				Bot.NavigationCache.lastChangedZigZag=DateTime.Today;
				Bot.NavigationCache.vPositionLastZigZagCheck=Vector3.Zero;

				// Out of combat buffing etc. but only if we don't want to return to town etc.
				AnimationState myAnimationState=Bot.Character.Data.CurrentAnimationState;
				if ((!Bot.Character.Data.bIsInTown||Bot.Settings.AllowBuffingInTown)&&
					 !TownRunManager.bWantToTownRun&&
					 myAnimationState!=AnimationState.Attacking&&myAnimationState!=AnimationState.Casting&&myAnimationState!=AnimationState.Channeling)
				{
					 Skill Buff;
					 if (Bot.Character.Class.FindBuffPower(out Buff))
					 {
						  Skill.SetupAbilityForUse(ref Buff);
						  Bot.Character.Data.WaitWhileAnimating(4, true);
						  Skill.UsePower(ref Buff);
						  Buff.OnSuccessfullyUsed();
						  Bot.Character.Data.WaitWhileAnimating(3, true);
					 }
				}




				// Ok let DemonBuddy do stuff this loop, since we're done for the moment
				return false;
		  }

		  //Used when we actually want to handle a target!
		  public static RunStatus HandleTarget(object ret)
		  {
				//if (ItemIdentifyBehavior.shouldPreformOOCItemIDing)
					//return ItemIdentifyBehavior.HandleIDBehavior(); //Check if we are doing OOC ID behavior..

				if (Bot.Targeting.CurrentTarget!=null)
					 return Bot.Targeting.HandleThis();  //Default Behavior: Current Target

				if (OutOfGame.MuleBehavior)
				{
					if (!OutOfGame.TransferedGear)
					 {
						  return NewMuleGame.StashTransfer();
					 }

					return NewMuleGame.FinishMuleBehavior();
				}

			    //Exit Game!!
				if (Bot.Game.GoldTimeoutChecker.BehaviorEngaged)
				{
					return Bot.Game.GoldTimeoutChecker.ExitGame();
				}

				return RunStatus.Success;
		  }
	 }
}