using System;
using System.Threading.Tasks;
using fBaseXtensions.Cache.Internal.Blacklist;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero.Skills;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;

namespace fBaseXtensions.Behaviors
{
	public class CombatHandler
	{
		public static bool GlobalOverlord(object ret)
		{
			// If we aren't in the game of a world is loading, don't do anything yet
			if (FunkyGame.GameIsInvalid)
			{
				FunkyGame.Navigation.lastChangedZigZag = DateTime.Today;
				FunkyGame.Navigation.vPositionLastZigZagCheck = Vector3.Zero;
				return false;
			}

			// Clear target current and reset key variables used during the target-handling function
			FunkyGame.Targeting.ResetTargetHandling();
			FunkyGame.Targeting.Cache.DontMove = false;

			// Should we refresh target list?
			if (FunkyGame.Targeting.Cache.ShouldRefreshObjectList)
			{
				FunkyGame.Targeting.Cache.Refresh();

				// We have a target, start the target handler!
				if (FunkyGame.Targeting.Cache.CurrentTarget != null)
				{
					FunkyGame.Targeting.cMovement.RestartTracking();
					FunkyGame.Targeting.Cache.bWholeNewTarget = true;
					FunkyGame.Targeting.Cache.DontMove = true;
					FunkyGame.Targeting.Cache.bPickNewAbilities = true;
					FunkyGame.RunningTargetingBehavior = true;
					//Bot.Targeting.Cache.StartingLocation = Bot.Character_.Data.Position;
					return true;
				}
			}

			
			// Pop a potion when necessary
			if (FunkyGame.Hero.Class.HealthPotionAbility.CheckPreCastConditionMethod())
			{
				if (FunkyGame.Hero.Class.HealthPotionAbility.CheckCustomCombatMethod())
				{
					FunkyGame.Hero.Class.HealthPotionAbility.AttemptToUseHealthPotion();
				}
			}

			BlacklistCache.CheckRefreshBlacklists();


			if (FunkyBaseExtension.Settings.Debugging.DebugStatusBar && FunkyGame.bResetStatusText)
			{
				FunkyGame.bResetStatusText = false;
				BotMain.StatusText = "[Funky] No more targets - DemonBuddy/profile management is now in control";
			}

			FunkyGame.RunningTargetingBehavior = false;
			// Nothing to do... do we have some maintenance we can do instead, like out of combat buffing?
			FunkyGame.Navigation.lastChangedZigZag = DateTime.Today;
			FunkyGame.Navigation.vPositionLastZigZagCheck = Vector3.Zero;

			// Out of combat buffing etc. but only if we don't want to return to town etc.
			AnimationState myAnimationState = FunkyGame.Hero.CurrentAnimationState;
			if ((!FunkyGame.Hero.bIsInTown || FunkyBaseExtension.Settings.General.AllowBuffingInTown) &&
				 !FunkyGame.IsInNonCombatBehavior &&
				 myAnimationState != AnimationState.Attacking && myAnimationState != AnimationState.Casting && myAnimationState != AnimationState.Channeling)
			{
				Skill Buff;
				if (FunkyGame.Hero.Class.FindBuffPower(out Buff))
				{
					Skill.SetupAbilityForUse(ref Buff);
					FunkyGame.Hero.WaitWhileAnimating(4, true);
					Skill.UsePower(ref Buff);
					Buff.OnSuccessfullyUsed();
					FunkyGame.Hero.WaitWhileAnimating(3, true);
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

			if (FunkyGame.Targeting.Cache.CurrentTarget != null)
				return FunkyGame.Targeting.HandleThis();  //Default Behavior: Current Target

			return RunStatus.Success;
		}
	}
}