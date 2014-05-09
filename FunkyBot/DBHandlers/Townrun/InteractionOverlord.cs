using System;
using System.Linq;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Movement;
using FunkyBot.Player;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;


namespace FunkyBot.DBHandlers
{

	internal static partial class TownRunManager
	{
		internal static bool IgnoreVendoring = false;
		private static bool InteractionMovementMade = false;

		internal static bool InteractionOverlord(object ret)
		{//Should we preform town run finishing behavior?

			townRunItemCache.InteractItems.Clear();
			//Get new list of current backpack
			Bot.Character.Data.BackPack.Update();

			foreach (var thisitem in Bot.Character.Data.BackPack.CacheItemList.Values)
			{
				if (thisitem.ACDItem.BaseAddress != IntPtr.Zero)
				{
					// Find out if this item's in a protected bag slot
					if (!ItemManager.Current.ItemIsProtected(thisitem.ACDItem))
					{
						if (thisitem.ThisDBItemType == ItemType.Potion) continue;

						if (thisitem.IsHoradricCache)
						{
							townRunItemCache.InteractItems.Add(thisitem);
						}
						else
						{
							return false;
						}
					}
				}
				else
				{
					Logger.DBLog.DebugFormat("GSError: Diablo 3 memory read error, or item became invalid [FinishOver-1]");
				}
			}

			return townRunItemCache.InteractItems.Count>0;
		}

		internal static RunStatus InteractionMovement(object ret)
		{//Move to nice open space!
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null)
			{
				Logger.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			Act curAct = ZetaDia.CurrentAct;
			if (curAct == Act.Invalid || curAct == Act.OpenWorld || curAct == Act.Test)
				curAct = Character.FindActByLevelID(Bot.Character.Data.iCurrentLevelID);
			var vectorLocation = ReturnMovementVector(TownRunBehavior.Interaction, curAct);

			Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
			float fDistance = Vector3.Distance(vectorPlayerPosition, vectorLocation);
			if (fDistance>7.5f)
			{
				Bot.NavigationCache.RefreshMovementCache();

				//Wait until we are not moving to send click again..
				if (Bot.NavigationCache.IsMoving)
					return RunStatus.Running;

				Navigator.PlayerMover.MoveTowards(vectorLocation);
				return RunStatus.Running;
			}

			return RunStatus.Success;
		}

		internal static RunStatus InteractionClickBehavior(object ret)
		{
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null)
			{
				Logger.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			if (!UIElements.InventoryWindow.IsVisible)
			{
				Bot.Character.Data.BackPack.InventoryBackPackToggle(true);
				return RunStatus.Running;
			}

			//Interact with items..
			if (townRunItemCache.InteractItems.Count > 0)
			{
				if (!TownRunItemLoopsTest(2.5)) return RunStatus.Running;

				CacheACDItem thisitem = townRunItemCache.InteractItems.FirstOrDefault();

				if (thisitem != null)
				{
					ZetaDia.Me.Inventory.UseItem(thisitem.ThisDynamicID);
					if (thisitem.IsHoradricCache) Bot.Game.CurrentGameStats.CurrentProfile.HoradricCacheOpened++;
				}
				if (thisitem != null)
					townRunItemCache.InteractItems.Remove(thisitem);
				if (townRunItemCache.InteractItems.Count > 0)
					return RunStatus.Running;
			}

			//Add a long wait after interaction!
			if (!TownRunItemLoopsTest(10)) return RunStatus.Running;

			return RunStatus.Success;
		}

		internal static RunStatus InteractionLootingBehavior(object ret)
		{
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null)
			{
				Logger.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			//Move!!
			if (!InteractionMovementMade)
			{
				var loc = Navigator.StuckHandler.GetUnstuckPos();
				Navigator.MoveTo(loc);
				InteractionMovementMade = true;
				return RunStatus.Running;
			}

			IgnoreVendoring = true;

			//Refresh?
			if (Bot.Targeting.Cache.ShouldRefreshObjectList)
				Bot.Targeting.Cache.Refresh();

			//Check if we have any NEW targets to deal with.. 
			if (Bot.Targeting.Cache.CurrentTarget != null)
			{
				

				//Directly Handle Target..
				RunStatus targetHandler = Bot.Targeting.Handler.HandleThis();

				//Only return failure if handling failed..
				if (targetHandler == RunStatus.Failure)
				{
					return RunStatus.Success;
				}
				if (targetHandler == RunStatus.Success)
				{
					Bot.Targeting.ResetTargetHandling();
				}

				return RunStatus.Running;
			}

			IgnoreVendoring = false;


			return RunStatus.Success;
		}

		internal static RunStatus InteractionFinishBehavior(object ret)
		{
			InteractionMovementMade = false;
			IgnoreVendoring = false;
			return RunStatus.Success;
		}

	}

}
