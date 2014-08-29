using System;
using System.Collections.Generic;
using System.Linq;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;

namespace fItemPlugin.Townrun
{

	internal static partial class TownRunManager
	{

		internal static bool IdenifyItemOverlord(object ret)
		{
			if (!FunkyTownRunPlugin.PluginSettings.IdentifyLegendaries) return false;

			RefreshUnidList(true);
			return UnidentifiedItems.Count > 0;
		}

		/// <summary>
		/// Determine if we should ID manually!
		/// </summary>
		internal static bool IdenifyItemManualOverlord(object ret)
		{
			return UnidentifiedItems.Count == 1;
		}

		private static bool ItemIDRefreshDone = false;
		private static Queue<ACDItem> UnidentifiedItems = new Queue<ACDItem>();
		private static int totalFailures = 0;
		private static ACDItem currentItem;
		private static int castAttempts = 0;

		private static void RefreshUnidList(bool behaviorRefresh = false)
		{
			//Clear any previous items.
			UnidentifiedItems.Clear();

			try
			{
				if (!behaviorRefresh)
					UnidentifiedItems = Backpack.ReturnUnidenifiedItems();
				else
					//Used during IDing
					UnidentifiedItems = Backpack.ReturnUnidenifiedItemsSorted();
			}
			catch
			{
				FunkyTownRunPlugin.DBLog.DebugFormat("Exception occured during refresh of inventory");
			}
		}


		internal static RunStatus IdenifyItemManualBehavior(object ret)
		{
			if (FunkyGame.GameIsInvalid)
			{
				ActionsChecked = false;
				FunkyTownRunPlugin.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			//Update List Once..
			if (!ItemIDRefreshDone)
			{
				RefreshUnidList(true);
				ItemIDRefreshDone = true;
			}

			//Finishing?
			//if (isFinishingBahavior)
			//{
			//	FinishBehavior();

			//	if (finishPauseDone)
			//	{
			//		Bot.Character.Data.lastPreformedNonCombatAction = DateTime.Now;
			//		finishPauseDone = false;
			//		return RunStatus.Success;
			//	}
			//	return RunStatus.Running;
			//}


			//Continue if items remain.
			if (UnidentifiedItems.Count > 0)
			{

				//3 successive failures.. so lets just finish this behavior until next time.
				if (totalFailures > 2)
				{
					FunkyTownRunPlugin.DBLog.DebugFormat("Quiting Idenify Behavior due to too many failures");
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
					ItemIDRefreshDone = false;
					return RunStatus.Running;
				}




				//Check if already identified. Or if we have tried to many times already.
				if (!unid || castAttempts > 2)
				{
					castAttempt = true;
					lastCastTime = DateTime.Now; //Just to prevent another cast from occuring..
					waitTime = RandomWaitTime();
					currentItem = null;


					UnidentifiedItems.Dequeue();
					//Update Items during vendor runs
					ItemIDRefreshDone = false;


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
					//lastActionPreformed = DateTime.Now;
				}

				return RunStatus.Running;
			}

			FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Idenify Behavior Finishing!");

			return RunStatus.Success;
		}


		internal static RunStatus IdenifyItemManualFinishBehavior(object ret)
		{
			//reset vars
			ItemIDRefreshDone = false;
			castAttempt = false;
			currentItem = null;
			totalFailures = 0;
			castAttempts = 0;
			preWaitBeforeOpenInv = false;


			FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Idenify Behavior Finished!");
			return RunStatus.Success;
		}

		private static DateTime lastCastTime = DateTime.MinValue;
		private static bool preWaitBeforeOpenInv = false;
		private static int waitTime = 3;
		private static bool castAttempt = false;

		private static int RandomWaitTime(bool cast = false)
		{
			Random R = new Random(DateTime.Now.Millisecond);
			if (!cast) return R.Next(550, 909);

			return R.Next(5005, 6100);
		}
		private static bool preIdentifyCastChecks()
		{
			double totalMSlastAction = DateTime.Now.Subtract(lastCastTime).TotalMilliseconds;


			//Wait until we are not moving
			if (FunkyGame.Hero.IsMoving)
			{
				return false;
			}



			//Check for inventory window visibility first.
			try
			{
				if (!Backpack.InventoryBackpackVisible())
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
						Backpack.InventoryBackPackToggle(true);
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


		/// <summary>
		/// Determine if we should use Book of Cain to ID.
		/// </summary>
		internal static bool IdenifyItemBookOfCainOverlord(object ret)
		{
			return UnidentifiedItems.Count > 1;
		}
		//SNO: 297813, 297814, 295415, 342675 
		internal static RunStatus IdenifyItemBookOfCainMovementBehavior(object ret)
		{
			if (FunkyGame.GameIsInvalid)
			{
				ActionsChecked = false;
				FunkyTownRunPlugin.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			DiaGizmo objBookOfCain = UpdateBookOfCainObject();

			Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
			Vector3 vectorBookOfCainLocation = Vector3.Zero;


			//Normal distance we use to move to specific location before moving to NPC
			float _distanceRequired = CurrentAct != Act.A5 ? 50f : 14f; //Act 5 we want short range only!

			if (objBookOfCain == null || objBookOfCain.Distance > _distanceRequired)
			{
				vectorBookOfCainLocation = SafetyIdenifyLocation;
			}
			else
				vectorBookOfCainLocation = objBookOfCain.Position;

			//if (vectorBookOfCainLocation == Vector3.Zero)
			//Character.FindActByLevelID(Bot.Character.Data.CurrentWorldDynamicID);

			//Wait until we are not moving
			if (FunkyGame.Hero.IsMoving) return RunStatus.Running;


			float iDistanceFromSell = Vector3.Distance(vectorPlayerPosition, vectorBookOfCainLocation);
			//Out-Of-Range...
			if (objBookOfCain == null || iDistanceFromSell > 4.3f)
			{
				Navigator.PlayerMover.MoveTowards(vectorBookOfCainLocation);
				return RunStatus.Running;
			}

			FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Idenify Behavior Book of Cain Movement Finished!");
			return RunStatus.Success;
		}

		internal static RunStatus IdenifyItemBookOfCainInteractionBehavior(object ret)
		{
			if (FunkyGame.GameIsInvalid)
			{
				ActionsChecked = false;
				FunkyTownRunPlugin.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			//Check if we still have items to ID..
			RefreshUnidList();
			if (UnidentifiedItems.Count == 0)
			{
				FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Idenify Behavior Book of Cain Interaction Finished!");
				return RunStatus.Success;
			}

			//Make sure we have the object!
			DiaGizmo objBookOfCain = UpdateBookOfCainObject();

			//Verify the object!
			if (objBookOfCain == null) return RunStatus.Failure;

			//Precheck?
			if (!PreChecksBookOfCain()) return RunStatus.Running;


			//Attempt Interaction!
			objBookOfCain.Interact();
			castAttempt = true;
			castAttempts++; //counter for attempts..
			lastCastTime = DateTime.Now;
			waitTime = RandomWaitTime(true);
			return RunStatus.Running;
		}

		private static bool PreChecksBookOfCain()
		{
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

				double totalMSlastAction = DateTime.Now.Subtract(lastCastTime).TotalMilliseconds;

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

		private static DiaGizmo UpdateBookOfCainObject()
		{
			try
			{
				var bookofcain = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).FirstOrDefault<DiaGizmo>(u => u.ActorInfo.GizmoType == GizmoType.IdentifyAll);
				if (bookofcain != null) return bookofcain;
			}
			catch (Exception)
			{

			}
			return null;
		}

		/* Book of Cain:
		 * Movement
		 * Interaction / Wait / Confirm
		 * Finish
		 */


	}

}