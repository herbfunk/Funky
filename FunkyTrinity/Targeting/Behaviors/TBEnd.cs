﻿using System;
using FunkyTrinity.Cache;
using FunkyTrinity.Cache.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Targeting.Behaviors
{
	 public class TBEnd : TargetBehavior
	 {
		  public TBEnd() : base() { }
		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Target; } }


		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				{
					 if (obj==null)
					 {
						  // See if we should wait for milliseconds for possible loot drops before continuing run
						  if (DateTime.Now.Subtract(FunkyTrinity.Bot.Targeting.lastHadUnitInSights).TotalMilliseconds<=FunkyTrinity.Bot.Settings.AfterCombatDelay&&DateTime.Now.Subtract(Bot.Targeting.lastHadEliteUnitInSights).TotalMilliseconds<=10000||
								//Cut the delay time in half for non-elite monsters!
							  DateTime.Now.Subtract(FunkyTrinity.Bot.Targeting.lastHadUnitInSights).TotalMilliseconds<=FunkyTrinity.Bot.Settings.AfterCombatDelay)
						  {
								obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.NoMovement, 20000, "WaitForLootDrops", 2f, -1);
								return true;

						  }
						  //Herbfunks wait after loot containers are opened. 3s for rare chests, half the settings delay for everything else.
						  if ((DateTime.Now.Subtract(FunkyTrinity.Bot.Targeting.lastHadRareChestAsTarget).TotalMilliseconds<=3750)||
							  (DateTime.Now.Subtract(FunkyTrinity.Bot.Targeting.lastHadContainerAsTarget).TotalMilliseconds<=(FunkyTrinity.Bot.Settings.AfterCombatDelay*1.25)))
						  {
								obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.NoMovement, 20000, "ContainerLootDropsWait", 2f, -1);
								return true;
						  }

						  // Finally, a special check for waiting for wrath of the berserker cooldown before engaging Azmodan
						  if (FunkyTrinity.Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_WrathOfTheBerserker)&&FunkyTrinity.Bot.Settings.Class.bWaitForWrath&&!FunkyTrinity.Bot.Class.Abilities[SNOPower.Barbarian_WrathOfTheBerserker].AbilityUseTimer()&&
							  ZetaDia.CurrentWorldId==121214&&
							  (Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for Wrath Of The Berserker cooldown before continuing to Azmodan.");
								obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.NoMovement, 20000, "GilesWaitForWrath", 0f, -1);
								InactivityDetector.Reset();
								return true;
						  }
						  // And a special check for wizard archon
						  if (FunkyTrinity.Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_Archon)&&!Bot.Class.Abilities[SNOPower.Wizard_Archon].AbilityUseTimer()&&FunkyTrinity.Bot.Settings.Class.bWaitForArchon&&ZetaDia.CurrentWorldId==121214&&
							  (Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for Wizard Archon cooldown before continuing to Azmodan.");
								obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.NoMovement, 20000, "GilesWaitForArchon", 0f, -1);
								InactivityDetector.Reset();
								return true;
						  }
						  // And a very sexy special check for WD BigBadVoodoo
						  if (FunkyTrinity.Bot.Class.HotbarPowers.Contains(SNOPower.Witchdoctor_BigBadVoodoo)&&!PowerManager.CanCast(SNOPower.Witchdoctor_BigBadVoodoo)&&ZetaDia.CurrentWorldId==121214&&
							  (Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(FunkyTrinity.Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
						  {
								Logging.Write("[Funky] Waiting for WD BigBadVoodoo cooldown before continuing to Azmodan.");
								obj=new CacheObject(FunkyTrinity.Bot.Character.Position, TargetType.NoMovement, 20000, "GilesWaitForVoodooo", 0f, -1);
								InactivityDetector.Reset();
								return true;
						  }
					 }

					 return obj!=null;
				};
		  }
	 }
}
