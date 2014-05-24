using System;
using FunkyBot.Cache.Enums;
using FunkyBot.Player;
using Zeta.Bot;
using Zeta.Game.Internals.Actors;
using System.Linq;

namespace FunkyBot.DBHandlers
{

	internal static partial class TownRunManager
	{

		internal static bool UnidItemOverlord(object ret)
		{
			bool foundStashableItem = false;

			if (bCheckUnidItems)
			{
				townRunItemCache.KeepItems.Clear();
				Bot.Character.Data.BackPack.Update();
				bCheckUnidItems = false;

				foreach (var thisitem in Bot.Character.Data.BackPack.CacheItemList.Values)
				{
					if (thisitem.ACDItem.BaseAddress != IntPtr.Zero)
					{
						// Find out if this item's in a protected bag slot
						if (!ItemManager.Current.ItemIsProtected(thisitem.ACDItem))
						{
							if (thisitem.ThisDBItemType == ItemType.Potion) continue;

							if (thisitem.IsUnidentified && thisitem.BaseItemType!=GilesBaseItemType.Misc && thisitem.BaseItemType!=GilesBaseItemType.Gem)
							{
								if (Bot.Character.ItemRulesEval.checkUnidStashItem(thisitem.ACDItem) == Interpreter.InterpreterAction.KEEP)
								{
									townRunItemCache.KeepItems.Add(thisitem);
									foundStashableItem = true;
								}
							}
							else
							{//Incase we do find an unidentified item.. we should also stash any additional items while we are there!
								if (Bot.Settings.ItemRules.UseItemRules)
								{
									Interpreter.InterpreterAction action = Bot.Character.ItemRulesEval.checkItem(thisitem.ACDItem, ItemEvaluationType.Keep);
									switch (action)
									{
										case Interpreter.InterpreterAction.KEEP:
											townRunItemCache.KeepItems.Add(thisitem);
											continue;
										case Interpreter.InterpreterAction.TRASH:
											continue;
									}
								}

								bool bShouldStashThis = (Bot.Settings.ItemRules.ItemRuleGilesScoring ? ItemFunc.StashValidation(thisitem)
									: ItemManager.Current.ShouldStashItem(thisitem.ACDItem));

								if (bShouldStashThis)
								{
									townRunItemCache.KeepItems.Add(thisitem);
								}
							}
						}
					}
					else
					{
						Logger.DBLog.InfoFormat("GSError: Diablo 3 memory read error, or item became invalid [StashOver-1]");
					}
				}
			}

			return foundStashableItem;
		}


	}

}