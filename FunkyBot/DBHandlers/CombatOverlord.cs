using System;
using System.Linq;
using FunkyBot.Cache;
using FunkyBot.DBHandlers.CharacterMule;
using FunkyBot.Player.Class;
using FunkyBot.Player.HotBar.Skills;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.TreeSharp;

namespace FunkyBot.DBHandlers
{
	public class CombatHandler
	{
		internal static bool ShouldRecreateBotClass = false;

		public static bool GlobalOverlord(object ret)
		{
			// If we aren't in the game of a world is loading, don't do anything yet
			if (Bot.GameIsInvalid())
			{
				Bot.NavigationCache.lastChangedZigZag = DateTime.Today;
				Bot.NavigationCache.vPositionLastZigZagCheck = Vector3.Zero;
				return false;
			}

			//Equipment Change Check
			Bot.Character.Data.equipment.CheckEquippment();


			if (Bot.Character.Class == null)
			{//Null?
				PlayerClass.ShouldRecreatePlayerClass = true;
			}
			else
			{//Skill Change Check
				Bot.Character.Class.HotBar.CheckSkills();
			}

			//Should we recreate class?
			if (PlayerClass.ShouldRecreatePlayerClass)
				PlayerClass.CreateBotClass();
			

			//Seconday Hotbar Check
			Bot.Character.Class.SecondaryHotbarBuffPresent();

			

			// Clear target current and reset key variables used during the target-handling function
			Bot.Targeting.ResetTargetHandling();
			Bot.Targeting.Cache.DontMove = false;

			//update current profile behavior.
			Bot.Game.Profile.CheckCurrentProfileBehavior();

			// Should we refresh target list?
			if (Bot.Targeting.Cache.ShouldRefreshObjectList)
			{
				Bot.Targeting.Cache.Refresh();

				// We have a target, start the target handler!
				if (Bot.Targeting.Cache.CurrentTarget != null)
				{
					Bot.Targeting.Movement.RestartTracking();
					Bot.Targeting.Cache.bWholeNewTarget = true;
					Bot.Targeting.Cache.DontMove = true;
					Bot.Targeting.Cache.bPickNewAbilities = true;
					Bot.RunningTargetingBehavior = true;
					//Bot.Targeting.Cache.StartingLocation = Bot.Character_.Data.Position;
					return true;
				}
			}
			else
			{
				if (OutOfGame.MuleBehavior)
				{
					if (BotMain.StatusText.Contains("Game Finished"))
					{
						if (ZetaDia.Actors.GetActorsOfType<GizmoPlayerSharedStash>(true, true).Any())
						{
							return true;
						}
					}
				}
				else if (ExitGame.ShouldExitGame)
				{
					ExitGame.BehaviorEngaged = true;
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


			if (Bot.Settings.Debug.DebugStatusBar && Funky.bResetStatusText)
			{
				Funky.bResetStatusText = false;
				BotMain.StatusText = "[Funky] No more targets - DemonBuddy/profile management is now in control";
			}

			Bot.RunningTargetingBehavior = false;
			// Nothing to do... do we have some maintenance we can do instead, like out of combat buffing?
			Bot.NavigationCache.lastChangedZigZag = DateTime.Today;
			Bot.NavigationCache.vPositionLastZigZagCheck = Vector3.Zero;

			// Out of combat buffing etc. but only if we don't want to return to town etc.
			AnimationState myAnimationState = Bot.Character.Data.CurrentAnimationState;
			if ((!Bot.Character.Data.bIsInTown || Bot.Settings.General.AllowBuffingInTown) &&
				 !Bot.IsInNonCombatBehavior &&
				 myAnimationState != AnimationState.Attacking && myAnimationState != AnimationState.Casting && myAnimationState != AnimationState.Channeling)
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

			if (Bot.Targeting.Cache.CurrentTarget != null)
				return Bot.Targeting.Handler.HandleThis();  //Default Behavior: Current Target

			if (OutOfGame.MuleBehavior)
			{
				if (!OutOfGame.TransferedGear)
				{
					return NewMuleGame.StashTransfer();
				}

				return NewMuleGame.FinishMuleBehavior();
			}

			//Exit Game!!
			if (ExitGame.BehaviorEngaged)
			{
				return ExitGame.Behavior();
			}

			return RunStatus.Success;
		}
	}
}