using System;
using FunkyBot.AbilityFunky;
using FunkyBot.Cache;
using Zeta;
using System.Linq;
using Zeta.Common;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using Zeta.CommonBot;
using Zeta.TreeSharp;
using FunkyBot.Character;

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
				if (Bot.Class == null)
					Player.CreateBotClass();

				//Seconday Hotbar Check
				Bot.Class.SecondaryHotbarBuffPresent();

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
						  Bot.Targeting.bWholeNewTarget=true;
						  Bot.Targeting.DontMove=true;
						  Bot.Targeting.bPickNewAbilities=true;
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


				if (Bot.Settings.Debug.DebugStatusBar&&Funky.bResetStatusText)
				{
					 Funky.bResetStatusText=false;
					 BotMain.StatusText="[Funky] No more targets - DemonBuddy/profile management is now in control";
				}

				// Nothing to do... do we have some maintenance we can do instead, like out of combat buffing?
				Bot.NavigationCache.lastChangedZigZag=DateTime.Today;
				Bot.NavigationCache.vPositionLastZigZagCheck=Vector3.Zero;

				// Out of combat buffing etc. but only if we don't want to return to town etc.
				AnimationState myAnimationState=Bot.Character.CurrentAnimationState;
				if ((!Bot.Character.bIsInTown||Bot.Settings.AllowBuffingInTown)&&
					 !TownRunManager.bWantToTownRun&&
					 myAnimationState!=AnimationState.Attacking&&myAnimationState!=AnimationState.Casting&&myAnimationState!=AnimationState.Channeling)
				{
					 Ability Buff;
					 if (Bot.Class.FindBuffPower(out Buff))
					 {
						  Ability.SetupAbilityForUse(ref Buff);
						  Bot.Character.WaitWhileAnimating(4, true);
						  AbilityFunky.Ability.UsePower(ref Buff);
						  Buff.OnSuccessfullyUsed();
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
				else if (Bot.Targeting.CurrentTarget!=null)
					 return Bot.Targeting.HandleThis();  //Default Behavior: Current Target
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