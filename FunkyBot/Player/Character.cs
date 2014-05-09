using System;
using FunkyBot.Cache;
using FunkyBot.Cache.Objects;
using FunkyBot.DBHandlers;
using FunkyBot.Movement;
using FunkyBot.Player.Class;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Game;

namespace FunkyBot.Player
{
	public class Character
	{
		public Character()
		{
			Account = new Account();
			Data = new CharacterCache();
			Class = null;
		}
		public void RefreshHotBar()
		{
			Class = null;
		}
		///<summary>
		///Values of the Current Character
		///</summary>
		internal CharacterCache Data { get; set; }
		public PlayerClass Class { get; set; }
		internal Account Account { get; set; }

		///<summary>
		///Item Rules
		///</summary>
		internal Interpreter ItemRulesEval { get; set; }


		public delegate void ItemPickupEvaluation(CacheItem item);
		public static event ItemPickupEvaluation OnItemPickupEvaluation;
		internal void ItemPickupEval(CacheItem item)
		{
			if (OnItemPickupEvaluation == null)
			{//If no event hooked then use default evaluation

				if (Bot.Settings.ItemRules.UseItemRules)
				{
					Interpreter.InterpreterAction action = ItemRulesEval.checkPickUpItem(item, ItemEvaluationType.PickUp);
					switch (action)
					{
						case Interpreter.InterpreterAction.PICKUP:
							item.ShouldPickup = true;
							break;
						case Interpreter.InterpreterAction.IGNORE:
							item.ShouldPickup = false;
							break;
					}
				}

				if (!item.ShouldPickup.HasValue)
				{
					//Use Giles Scoring or DB Weighting..
					item.ShouldPickup =
						   Bot.Settings.ItemRules.ItemRuleGilesScoring ? Backpack.GilesPickupItemValidation(item)
						 : ItemManager.Current.EvaluateItem(item.ref_DiaItem.CommonData, ItemEvaluationType.PickUp); ;
				}
			}
			else
			{
				OnItemPickupEvaluation(item);
			}
		}

		private int LastWorldID = -1;
		private bool LastLevelIDChangeWasTownRun;
		private void LevelAreaIDChangeHandler(int ID)
		{
			Logger.Write(LogLevel.Event, "Level Area ID has Changed");



			if (!BrainBehavior.IsVendoring)
			{
				//Check for World ID change!
				if (Bot.Character.Data.iCurrentWorldID != LastWorldID)
				{
					Logger.Write(LogLevel.Event, "World ID changed.. clearing Profile Interactable Cache.");
					LastWorldID = Bot.Character.Data.iCurrentWorldID;
					Bot.Game.Profile.InteractableObjectCache.Clear();
					Navigator.SearchGridProvider.Update();

					//Gold Inactivity
					Bot.Game.GoldTimeoutChecker.LastCoinageUpdate = DateTime.Now;
				}

				if (!LastLevelIDChangeWasTownRun)
				{//Do full clear

					BackTrackCache.cacheMovementGPRs.Clear();
					Bot.NavigationCache.LOSBlacklistedRAGUIDs.Clear();
					Bot.Game.Profile.InteractableCachedObject = null;
				}
				else
				{
					//Gold Inactivity
					Bot.Game.GoldTimeoutChecker.LastCoinageUpdate = DateTime.Now;
					TownRunManager.TalliedTownRun = false;
				}

				//Clear the object cache!
				ObjectCache.Objects.Clear();
				//ObjectCache.cacheSnoCollection.ClearDictionaryCacheEntries();
				Bot.Targeting.Cache.RemovalCheck = false;

				//Reset Skip Ahead Cache
				SkipAheadCache.ClearCache();

				Bot.Character.Data.UpdateCoinage = true;

				//ZetaDia.ActInfo.ActiveBounty.Info.QuestSNO
				//Adventure Mode?
				//if (Bot.Game.AdventureMode)
				//{
				//	Bot.Game.Bounty.RefreshBountyLevelChange();
				//}

				LastLevelIDChangeWasTownRun = false;
			}
			else if (Bot.Character.Data.bIsInTown)
			{
				LastLevelIDChangeWasTownRun = true;
			}
		}

		internal void Reset()
		{
			Data = new CharacterCache();
			Data.OnLevelAreaIDChanged += LevelAreaIDChangeHandler;
			Class = null;
			ItemRulesEval = new Interpreter();
			Account.UpdateCurrentAccountDetails();
		}

		///<summary>
		///To Find Town Areas
		///</summary>
		public static Act FindActByLevelID(int ID)
		{
			switch (ID)
			{
				case 332339:
					return Act.A1;
				case 168314:
					return Act.A2;
				case 92945:
					return Act.A3;
				case 270011:
					return Act.A5;
			}

			return Act.Invalid;
		}

	}
}
