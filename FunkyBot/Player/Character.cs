using FunkyBot.Cache.Objects;
using FunkyBot.Player.Class;
using Zeta.Bot;

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

		internal void Reset()
		{
			Data = new CharacterCache();
			Class = null;
		}

	}
}
