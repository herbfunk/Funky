using System;
using System.Linq;
using Zeta;
using Zeta.CommonBot.Logic;
using Zeta.Internals;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using Zeta.TreeSharp;


namespace FunkyBot.DBHandlers
{
	public static class ItemIdentifyBehavior
	{
		internal static RunStatus FunkyIDBehavior(object ret)
		{
			return HandleIDBehavior();
		}

		internal static Queue<ACDItem> UnidentifiedItems = new Queue<ACDItem>();
		internal static ACDItem currentItem;
		internal static bool RefreshDone = false;
		internal static DateTime lastActionPreformed = DateTime.MinValue;
		internal static List<ACDItem> IdentifiedItems = new List<ACDItem>();
		public static bool shouldPreformOOCItemIDing = false;

		internal static void RefreshUnidList(bool behaviorRefresh = false)
		{
			//Clear any previous items.
			UnidentifiedItems.Clear();
			IdentifiedItems.Clear();

			try
			{
				if (!behaviorRefresh)
					UnidentifiedItems = Bot.Character.Data.BackPack.ReturnUnidenifiedItems();
				else
					//Used during IDing
					UnidentifiedItems = Bot.Character.Data.BackPack.ReturnUnidifiedItemsRandomizedSorted();
			}
			catch
			{
				Logger.Write(LogLevel.Items, "Exception occured during refresh of inventory");
			}
		}

		//Implementation into Town Run behavior!
		internal static bool FunkyIDOverlord(object ret)
		{
			//Question: Do we want to run ID Behavior?

			//ONLY RUN IF VENDOR BEHAVIOR...
			if (!BrainBehavior.ShouldVendor)
				return false;

			RefreshUnidList();
			//ONLY RUN ID BEHAVIOR IF UNID ITEMS EXIST!
			return UnidentifiedItems.Count() > 0;
		}


		internal static RunStatus HandleIDBehavior()
		{
			//Exit this?
			if (ZetaDia.Me.IsDead || !ZetaDia.IsInGame)
				return RunStatus.Success;


			//Update List Once..
			if (!RefreshDone)
			{
				RefreshUnidList(true);
				RefreshDone = true;
			}

			//Finishing?
			if (isFinishingBahavior)
			{
				FinishBehavior();

				if (finishPauseDone)
				{
					Bot.Character.Data.lastPreformedNonCombatAction = DateTime.Now;
					finishPauseDone = false;
					return RunStatus.Success;
				}
				return RunStatus.Running;
			}


			//Refresh Combat if NOT VENDOR RUN..
			if (!Bot.Character.Data.bIsInTown)
			{
				//Refresh?
				if (Bot.Targeting.ShouldRefreshObjectList)
				{
					Bot.Targeting.RefreshDiaObjects();
				}

				//Check if we have any NEW targets to deal with.. 
				//Note: Refresh will filter targets to units and avoidance ONLY.
				if (Bot.Targeting.CurrentTarget != null)
				{
					//Check if we have not made a ID cast in awhile..
					if (DateTime.Now.Subtract(lastActionPreformed).TotalSeconds > 20)
						Bot.Character.Data.BackPack.InventoryBackPackToggle(false);

					//Directly Handle Target..
					RunStatus targetHandler = Bot.Targeting.HandleThis();

					//Only return failure if handling failed..
					if (targetHandler == RunStatus.Failure)
						return RunStatus.Success;
					if (targetHandler == RunStatus.Success)
						Bot.Targeting.ResetTargetHandling();

					return RunStatus.Running;
				}
			}


			//Continue if items remain.
			if (UnidentifiedItems.Count > 0)
			{


				//3 successive failures.. so lets just finish this behavior until next time.
				if (totalFailures > 2)
				{
					Logger.Write(LogLevel.Items, "Quiting OOC Behavior due to too many failures");
					FinishBehavior();
					return RunStatus.Success;
				}

				//Reset our current Item to next in our unid queue.
				if (currentItem == null)
				{
					currentItem = UnidentifiedItems.Peek();
					castAttempts = 0;
				}
				else if (!preIdentifyCastChecks())
					return RunStatus.Running;


				//Attempt live update of current item only if vendoring
				//if (Bot.Character_.Data.bIsInTown)
				//UpdateCurrentItem();

				//Get Unid, handle exception if any.. exit if failure to update.
				bool unid;
				if (currentItem.BaseAddress != IntPtr.Zero)
				{

					unid = currentItem.IsUnidentified;

				}
				else
				{
					currentItem = null;
					RefreshDone = false;
					return RunStatus.Running;
				}




				//Check if already identified. Or if we have tried to many times already.
				if (!unid || castAttempts > 2)
				{
					castAttempt = true;
					lastCastTime = DateTime.Now; //Just to prevent another cast from occuring..
					waitTime = RandomWaitTime();
					currentItem = null;

					//Update ID items only if we are not vendoring..
					if (!Bot.Character.Data.bIsInTown)
						IdentifiedItems.Add(UnidentifiedItems.Dequeue());
					else
					{
						UnidentifiedItems.Dequeue();
						//Update Items during vendor runs
						RefreshDone = false;
					}

					//count failures
					if (castAttempts > 2)
						totalFailures++;
					else if (totalFailures > 0)
						totalFailures--;

					return RunStatus.Running;
				}

				//Checks for pre-casting.
				if (preIdentifyCastChecks())
				{
					//Cast, Update Vars
					ZetaDia.Me.Inventory.IdentifyItem(currentItem.DynamicId);
					castAttempt = true;
					castAttempts++; //counter for attempts..
					lastCastTime = DateTime.Now;
					waitTime = RandomWaitTime(true);

					//Update so we can close inv if combat takes awhile..
					lastActionPreformed = DateTime.Now;
				}

				return RunStatus.Running;
			}
			//No Items Remain.. Start finshing behavior.
			isFinishingBahavior = true;
			return RunStatus.Running;
		}

		internal static DateTime lastCastTime = DateTime.MinValue;
		internal static int waitTime = 3;
		internal static int castAttempts = 0;
		internal static int totalFailures = 0;
		internal static bool shouldUpdateTime = true;
		internal static bool castAttempt = false;
		internal static bool isFinishingBahavior = false;
		internal static bool finishPauseDone = false;
		internal static bool preWaitBeforeOpenInv = false;

		internal static void FinishBehavior()
		{

			//We don't want to close our inventory immeditaly after our last cast, so this prevents that by making it wait around 3-4s
			if (shouldUpdateTime)
			{
				//Update time to wait before actual finish
				waitTime = RandomWaitTime();
				shouldUpdateTime = false;
				lastCastTime = DateTime.Now;
			}

			//Wait..
			if (!finishPauseDone && DateTime.Now.Subtract(lastCastTime).TotalMilliseconds > (waitTime * 1.35))
				finishPauseDone = true;
			else
				return;

			Logger.Write(LogLevel.Items, "Finishing OOC ID Behavior");

			//reset vars
			RefreshDone = false;
			castAttempt = false;
			shouldUpdateTime = true;
			currentItem = null;
			totalFailures = 0;
			castAttempts = 0;
			shouldPreformOOCItemIDing = false;
			preWaitBeforeOpenInv = false;
			isFinishingBahavior = false;

			//close inv only if we are not wanting to town run!
			if (Bot.Character.Data.BackPack.InventoryBackpackVisible() && !BrainBehavior.ShouldVendor)
				Bot.Character.Data.BackPack.InventoryBackPackToggle(false);
		}


		internal static int RandomWaitTime(bool cast = false)
		{
			Random R = new Random(DateTime.Now.Millisecond);
			if (!cast)
				return R.Next(550, 909);
			return R.Next(999, 2550);
		}

		internal static bool preIdentifyCastChecks()
		{
			double totalMSlastAction = DateTime.Now.Subtract(lastCastTime).TotalMilliseconds;

			//Wait until we are not moving!
			try
			{
				if (ZetaDia.Me.Movement.IsMoving)
				{
					return false;
				}
			}
			catch
			{
			}


			//Check for inventory window visibility first.
			try
			{
				if (!Bot.Character.Data.BackPack.InventoryBackpackVisible())
				{
					//Init a wait before opening..
					if (!preWaitBeforeOpenInv)
					{
						preWaitBeforeOpenInv = true;
						waitTime = RandomWaitTime(true);
						lastCastTime = DateTime.Now;
					}
					else if (totalMSlastAction > waitTime)
					{
						//Open inventory and wait..
						Bot.Character.Data.BackPack.InventoryBackPackToggle(true);
						preWaitBeforeOpenInv = false;
						waitTime = RandomWaitTime();
						lastCastTime = DateTime.Now;
					}
					return false;
				}//Wait after opening..
				if (totalMSlastAction < waitTime)
					return false;
			}
			catch
			{
				return false;
			}

			//Cannot ID when stash window is visible.
			try
			{
				if (UIElements.StashWindow.IsVisible)
				{
					UIElement.FromHash(0x5CF7230E045FF4DF).Click();
					return false;
				}
			}
			catch
			{
			}

			//Attempted a cast?
			if (castAttempt)
			{
				//Get AnimState
				bool bCasting;
				try
				{
					bCasting = (ZetaDia.Me.CommonData.AnimationState == AnimationState.Casting);
				}
				catch
				{
					RandomWaitTime();
					return false;
				}

				//Cast attempted, not casting, and wait time is passed.
				if (!bCasting && totalMSlastAction > waitTime)
				{
					castAttempt = false;
					return true;
				}
				return false;
			}
			return true;
		}


	}
}