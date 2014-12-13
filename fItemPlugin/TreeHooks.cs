using System;
using fBaseXtensions;
using fBaseXtensions.Behaviors;
using fBaseXtensions.Helpers;
using fItemPlugin.Townrun;
using Zeta.TreeSharp;

namespace fItemPlugin
{
	internal static class TreehookHandling
	{
		internal static bool initTreeHooks;
		internal static PrioritySelector VendorPrioritySelector;
		internal static CanRunDecoratorDelegate VendorCanRunDelegate;
		internal static Guid VendorGUID = Guid.Empty;

		internal static bool CheckVendorHooked()
		{
			var vendorValue = HookHandler.ReturnHookValue(HookHandler.HookType.VendorRun);
			return vendorValue[0].Guid == VendorGUID;
		}

		internal static void HookBehaviorTree()
		{
			
            

			Logger.DBLog.InfoFormat("[FunkyTownRun] Treehooks..");
			#region VendorRun
			// Wipe the vendorrun and loot behavior trees, since we no longer want them

			Logger.DBLog.DebugFormat("[FunkyTownRun] VendorRun...");


            Decorator vendorRunDecorator = HookHandler.ReturnHookValue(HookHandler.HookType.VendorRun)[0] as Decorator;
            if (vendorRunDecorator != null && !(vendorRunDecorator.Children[0] is PrioritySelector))
            {
                Logger.DBLog.DebugFormat("[FunkyTownRun] VendorRun Child mismatched -- restoring original.");
                HookHandler.RestoreHook(HookHandler.HookType.VendorRun);
            }

            HookHandler.StoreHook(HookHandler.HookType.VendorRun);

			Decorator GilesDecorator = HookHandler.ReturnHookValue(HookHandler.HookType.VendorRun)[0] as Decorator;
			//PrioritySelector GilesReplacement = GilesDecorator.Children[0] as PrioritySelector;
			PrioritySelector GilesReplacement = GilesDecorator.Children[0] as PrioritySelector;
			//HookHandler.PrintChildrenTypes(GilesReplacement.Children);

			CanRunDecoratorDelegate canRunDelegateReturnToTown = TownPortalBehavior.FunkyTPOverlord;
			ActionDelegate actionDelegateReturnTown = TownPortalBehavior.FunkyTPBehavior;
			ActionDelegate actionDelegateTownPortalFinish = TownPortalBehavior.FunkyTownPortalTownRun;
			Sequence sequenceReturnTown = new Sequence(
				new Zeta.TreeSharp.Action(actionDelegateReturnTown),
				new Zeta.TreeSharp.Action(actionDelegateTownPortalFinish)
				);
			GilesReplacement.Children[1] = new Decorator(canRunDelegateReturnToTown, sequenceReturnTown);
			Logger.DBLog.DebugFormat("[FunkyTownRun] Town Portal - hooked...");


			ActionDelegate actionDelegatePrePause = TownRunManager.GilesStashPrePause;
			ActionDelegate actionDelegatePause = TownRunManager.GilesStashPause;

			CanRunDecoratorDelegate canRunDelegateEvaluateAction = TownRunManager.ActionsEvaluatedOverlord;
			ActionDelegate actionDelegateEvaluateAction = TownRunManager.ActionsEvaluatedBehavior;

			Sequence sequenceEvaluate = new Sequence(
						new Zeta.TreeSharp.Action(actionDelegatePrePause),
						new Zeta.TreeSharp.Action(actionDelegatePause),
						new Zeta.TreeSharp.Action(actionDelegateEvaluateAction)
					);

			GilesReplacement.Children[2] = new Decorator(canRunDelegateEvaluateAction, sequenceEvaluate);


			#region Idenify



			CanRunDecoratorDelegate canRunDelegateFunkyIDManual = TownRunManager.IdenifyItemManualOverlord;
			ActionDelegate actionDelegateIDManual = TownRunManager.IdenifyItemManualBehavior;
			ActionDelegate actionDelegateIDFinish = TownRunManager.IdenifyItemManualFinishBehavior;
			Sequence sequenceIDManual = new Sequence(
				new Zeta.TreeSharp.Action(actionDelegateIDManual),
				new Zeta.TreeSharp.Action(actionDelegateIDFinish)
			);

			CanRunDecoratorDelegate canRunDelegateFunkyIDBookOfCain = TownRunManager.IdenifyItemBookOfCainOverlord;
			ActionDelegate actionDelegateIDBookOfCainMovement = TownRunManager.IdenifyItemBookOfCainMovementBehavior;
			ActionDelegate actionDelegateIDBookOfCainInteraction = TownRunManager.IdenifyItemBookOfCainInteractionBehavior;
			Sequence sequenceIDBookOfCain = new Sequence(
				new Zeta.TreeSharp.Action(actionDelegateIDBookOfCainMovement),
				new Zeta.TreeSharp.Action(actionDelegateIDBookOfCainInteraction),
				new Zeta.TreeSharp.Action(actionDelegateIDFinish)
			);


			PrioritySelector priorityIDItems = new PrioritySelector(
				new Decorator(canRunDelegateFunkyIDManual, sequenceIDManual),
				new Decorator(canRunDelegateFunkyIDBookOfCain, sequenceIDBookOfCain)
			);

			CanRunDecoratorDelegate canRunDelegateFunkyIDOverlord = TownRunManager.IdenifyItemOverlord;
			GilesReplacement.Children[3] = new Decorator(canRunDelegateFunkyIDOverlord, priorityIDItems);

			Logger.DBLog.DebugFormat("[FunkyTownRun] Idenify Items - hooked...");



			#endregion

			#region Salvage

			// Replace DB salvaging behavior tree with my optimized & "one-at-a-time" version
			CanRunDecoratorDelegate canRunDelegateSalvageGilesOverlord = TownRunManager.SalvageOverlord;
			ActionDelegate actionDelegatePreSalvage = TownRunManager.PreSalvage;
			ActionDelegate actionDelegateSalvage = TownRunManager.GilesOptimisedSalvage;
			ActionDelegate actionDelegatePostSalvage = TownRunManager.PostSalvage;
			Sequence sequenceSalvage = new Sequence(
					new Zeta.TreeSharp.Action(actionDelegatePreSalvage),
					new Zeta.TreeSharp.Action(actionDelegateSalvage),
					new Zeta.TreeSharp.Action(actionDelegatePostSalvage),
					new Sequence(
					new Zeta.TreeSharp.Action(actionDelegatePrePause),
					new Zeta.TreeSharp.Action(actionDelegatePause)
					)
					);
			GilesReplacement.Children[4] = new Decorator(canRunDelegateSalvageGilesOverlord, sequenceSalvage);
			Logger.DBLog.DebugFormat("[FunkyTownRun] Salvage - hooked...");

			#endregion

			#region Stash

			// Replace DB stashing behavior tree with my optimized version with loot rule replacement
			CanRunDecoratorDelegate canRunDelegateStashGilesOverlord = TownRunManager.StashOverlord;
			ActionDelegate actionDelegatePreStash = TownRunManager.PreStash;
			ActionDelegate actionDelegatePostStash = TownRunManager.PostStash;

			ActionDelegate actionDelegateStashMovement = TownRunManager.StashMovement;
			ActionDelegate actionDelegateStashUpdate = TownRunManager.StashUpdate;
			ActionDelegate actionDelegateStashItems = TownRunManager.StashItems;

			Sequence sequencestash = new Sequence(
					new Zeta.TreeSharp.Action(actionDelegatePreStash),
					new Zeta.TreeSharp.Action(actionDelegateStashMovement),
					new Zeta.TreeSharp.Action(actionDelegateStashUpdate),
					new Zeta.TreeSharp.Action(actionDelegateStashItems),
					new Zeta.TreeSharp.Action(actionDelegatePostStash),
					new Sequence(
					new Zeta.TreeSharp.Action(actionDelegatePrePause),
					new Zeta.TreeSharp.Action(actionDelegatePause)
					)
					);
			GilesReplacement.Children[5] = new Decorator(canRunDelegateStashGilesOverlord, sequencestash);
			Logger.DBLog.DebugFormat("[FunkyTownRun] Stash - hooked...");

			#endregion

			#region Vendor

			// Replace DB vendoring behavior tree with my optimized & "one-at-a-time" version
			CanRunDecoratorDelegate canRunDelegateSellGilesOverlord = TownRunManager.SellOverlord;
			ActionDelegate actionDelegatePreSell = TownRunManager.PreSell;
			ActionDelegate actionDelegateMovement = TownRunManager.SellMovement;
			ActionDelegate actionDelegateSell = TownRunManager.SellInteraction;
			ActionDelegate actionDelegatePostSell = TownRunManager.PostSell;
			Sequence sequenceSell = new Sequence(
					new Zeta.TreeSharp.Action(actionDelegatePreSell),
					new Zeta.TreeSharp.Action(actionDelegateMovement),
					new Zeta.TreeSharp.Action(actionDelegateSell),
					new Zeta.TreeSharp.Action(actionDelegatePostSell),
					new Sequence(
					new Zeta.TreeSharp.Action(actionDelegatePrePause),
					new Zeta.TreeSharp.Action(actionDelegatePause)
					)
					);
			GilesReplacement.Children[6] = new Decorator(canRunDelegateSellGilesOverlord, sequenceSell);
			Logger.DBLog.DebugFormat("[FunkyTownRun] Vendor - hooked...");


			#endregion

			#region Interaction Behavior
			CanRunDecoratorDelegate canRunDelegateInteraction = TownRunManager.InteractionOverlord;
			ActionDelegate actionDelegateInteractionMovementhBehavior = TownRunManager.InteractionMovement;
			ActionDelegate actionDelegateInteractionClickBehaviorBehavior = TownRunManager.InteractionClickBehavior;
			ActionDelegate actionDelegateInteractionLootingBehaviorBehavior = TownRunManager.InteractionLootingBehavior;
			ActionDelegate actionDelegateInteractionFinishBehaviorBehavior = TownRunManager.InteractionFinishBehavior;

			Sequence sequenceInteraction = new Sequence(
					new Zeta.TreeSharp.Action(actionDelegateInteractionFinishBehaviorBehavior),
					new Zeta.TreeSharp.Action(actionDelegateInteractionMovementhBehavior),
					new Zeta.TreeSharp.Action(actionDelegateInteractionClickBehaviorBehavior),
					new Zeta.TreeSharp.Action(actionDelegateInteractionLootingBehaviorBehavior),
					new Zeta.TreeSharp.Action(actionDelegateInteractionFinishBehaviorBehavior)
				);
			GilesReplacement.InsertChild(7, new Decorator(canRunDelegateInteraction, sequenceInteraction));
			Logger.DBLog.DebugFormat("[FunkyTownRun] Interaction Behavior - Inserted...");

			#endregion

			#region Gambling Behavior

			CanRunDecoratorDelegate canRunDelegateGambling = TownRunManager.GamblingRunOverlord;
			ActionDelegate actionDelegateGamblingMovementBehavior = TownRunManager.GamblingMovement;
			ActionDelegate actionDelegateGamblingInteractionBehavior = TownRunManager.GamblingInteraction;
			ActionDelegate actionDelegateGamblingStartBehavior = TownRunManager.GamblingStart;
			ActionDelegate actionDelegateGamblingFinishBehavior = TownRunManager.GamblingFinish;

			Sequence sequenceGambling = new Sequence(
					new Zeta.TreeSharp.Action(actionDelegateGamblingStartBehavior),
					new Zeta.TreeSharp.Action(actionDelegateGamblingMovementBehavior),
					new Zeta.TreeSharp.Action(actionDelegateGamblingInteractionBehavior),
					new Zeta.TreeSharp.Action(actionDelegateGamblingFinishBehavior)
				);
			GilesReplacement.InsertChild(8, new Decorator(canRunDelegateGambling, sequenceGambling));
			Logger.DBLog.DebugFormat("[FunkyTownRun] Gambling Behavior - Inserted...");

			#endregion

			#region Finish Behavior

			int childrenCount = GilesReplacement.Children.Count;
			ActionDelegate actionFinishReset = TownRunManager.ActionsEvaluatedEndingBehavior;
			ActionDelegate actionDelegateFinishBehavior = TownRunManager.FinishBehavior;
			var finish = GilesReplacement.Children[childrenCount - 1];

			Sequence FinishSequence = new Sequence
			(
				finish,
				new Zeta.TreeSharp.Action(actionFinishReset),
				new Zeta.TreeSharp.Action(actionDelegateFinishBehavior)
			);
			GilesReplacement.Children[childrenCount - 1] = FinishSequence;
			Logger.DBLog.DebugFormat("[FunkyTownRun] Created Sequence Finish Behavior.");

			#endregion


			CanRunDecoratorDelegate canRunDelegateGilesTownRunCheck = TownRunManager.TownRunCheckOverlord;
			VendorCanRunDelegate = canRunDelegateGilesTownRunCheck;
			VendorPrioritySelector = GilesReplacement;
			var VendorComposite = new Decorator(canRunDelegateGilesTownRunCheck, GilesReplacement);
			VendorGUID = VendorComposite.Guid;

			HookHandler.SetHookValue(HookHandler.HookType.VendorRun, 0, VendorComposite);
			Logger.DBLog.DebugFormat("[FunkyTownRun] Vendor Run tree hooking finished.");
			#endregion


			initTreeHooks = true;
		}
	}
}
