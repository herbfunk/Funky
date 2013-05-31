using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using Zeta.CommonBot;

namespace FunkyTrinity
{
    public partial class Funky
    {

        public static Ability GilesAbilitySelector(bool bCurrentlyAvoiding = false, bool bOOCBuff = false, bool bDestructiblePower = false)
        {


            // Switch based on the cached character class
            switch (Bot.Class.AC)
            {
                case ActorClass.Barbarian:
                    return BarbAbility(bCurrentlyAvoiding, bOOCBuff, bDestructiblePower);
                case ActorClass.Monk:
                    return MonkAbility(bCurrentlyAvoiding, bOOCBuff, bDestructiblePower);
                case ActorClass.Wizard:
                    return WizardAbility(bCurrentlyAvoiding, bOOCBuff, bDestructiblePower);
                case ActorClass.WitchDoctor:
                    return WitchDoctorAbility(bCurrentlyAvoiding, bOOCBuff, bDestructiblePower);
                case ActorClass.DemonHunter:
                    return DemonHunterAbility(bCurrentlyAvoiding, bOOCBuff, bDestructiblePower);
            }

            return new Ability(SNOPower.None, 0, vNullLocation, -1, -1, 0, 0, false);
        }

        // **********************************************************************************************
        // *****      Quick and Dirty routine just to force a wait until the character is "free"    *****
        // **********************************************************************************************
        public static void WaitWhileAnimating(int iMaxSafetyLoops = 10, bool bWaitForAttacking = false)
        {
            bool bKeepLooping = true;
            int iSafetyLoops = 0;
            while (bKeepLooping)
            {
                iSafetyLoops++;
                if (iSafetyLoops > iMaxSafetyLoops)
                    bKeepLooping = false;
                bool bIsAnimating = false;
                try
                {
						  Bot.Character.UpdateAnimationState();
						  if (Bot.Character.CurrentAnimationState==AnimationState.Casting||Bot.Character.CurrentAnimationState==AnimationState.Channeling)
                        bIsAnimating = true;
						  if (bWaitForAttacking&&(Bot.Character.CurrentAnimationState==AnimationState.Attacking))
                        bIsAnimating = true;
                } catch (NullReferenceException)
                {
                    bIsAnimating = true;
                }
                if (!bIsAnimating)
                    bKeepLooping = false;
            }
        }

        // **********************************************************************************************
        // *****                         Check re-use timers on skills                             *****
        // **********************************************************************************************

        // Returns whether or not we can use a skill, or if it's on our own internal Trinity cooldown timer
        private static bool AbilityUseTimer(SNOPower thispower, bool bReCheck = false)
        {
            if (DateTime.Now.Subtract(dictAbilityLastUse[thispower]).TotalMilliseconds >= Bot.Class.AbilityCooldowns[thispower])
                return true;
            if (bReCheck && DateTime.Now.Subtract(dictAbilityLastUse[thispower]).TotalMilliseconds >= 150 && DateTime.Now.Subtract(dictAbilityLastUse[thispower]).TotalMilliseconds <= 600)
                return true;
            return false;
        }
		  private static double AbilityLastUseMS(SNOPower P)
		  {
				return DateTime.Now.Subtract(dictAbilityLastUse[P]).TotalMilliseconds;
		  }
		  public static bool HotbarAbilitiesContainsPower(SNOPower P)
		  {
				return Bot.Class.HotbarAbilities.Contains(P);
		  }
		  public static bool PassiveSkillsContainsPower(SNOPower P)
		  {
				return Bot.Class.PassiveAbilities.Contains(P);
		  }
        // This function checks when the spell last failed (according to D3 memory, which isn't always reliable)
        // To prevent Trinity getting stuck re-trying the same spell over and over and doing nothing else
        // No longer used but keeping this here incase I re-use it
        private static bool GilesCanRecastAfterFailure(SNOPower thispower, int iMaxRecheckTime = 250)
        {
            if (DateTime.Now.Subtract(dictAbilityLastFailed[thispower]).TotalMilliseconds <= iMaxRecheckTime)
                return false;
            return true;
        }

        // When last hit the power-manager for this - not currently used, saved here incase I use it again in the future!
        // This is a safety function to prevent spam of the CPU and time-intensive "PowerManager.CanCast" function in DB
        // No longer used but keeping this here incase I re-use it
        private static bool GilesPowerManager(SNOPower thispower, int iMaxRecheckTime)
        {
            if (DateTime.Now.Subtract(dictAbilityLastPowerChecked[thispower]).TotalMilliseconds <= iMaxRecheckTime)
                return false;
            dictAbilityLastPowerChecked[thispower] = DateTime.Now;
            if (PowerManager.CanCast(thispower))
                return true;
            return false;
        }

        // **********************************************************************************************
        // *****                    Checking for buffs and caching the buff list                    *****
        // **********************************************************************************************

        // Check if a particular buff is present
        public static bool HasBuff(SNOPower power)
        {
            int id = (int)power;
            return Bot.Class.CurrentBuffs.Keys.Any(u => u == id);
        }

        // Returns how many stacks of a particular buff there are
        public static int GetBuffStacks(SNOPower thispower)
        {
            int iStacks;
            if (Bot.Class.CurrentBuffs.TryGetValue((int)thispower, out iStacks))
            {
                return iStacks;
            }
            return 0;
        }
    }
}