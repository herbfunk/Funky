using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.XML
{
	[XmlElement("AcceptTieredRiftReward")]
	public class AcceptTieredRiftRewardTag : ProfileBehavior
	{
		public enum RewardType
		{
			Key,
			Gem
		}

		[XmlAttribute("Reward")]
		public RewardType Reward { get; set; }

		protected override Composite CreateBehavior()
		{
			return new ActionRunCoroutine(ret => BehaviorRoutine());
		}

		private Delayer delayer = new Delayer();

		private List<CacheACDItem> SelectableGems = new List<CacheACDItem>();
		private List<int> SelectableUIGemElementIndexs = new List<int>();
		private List<UIElement> SelectableUIGemElements = new List<UIElement>();
		private int GRiftLevel = 0;
		private async Task<bool> BehaviorRoutine()
		{
			//Refresh Active Quests and Check that we are still in the reward step..
			FunkyGame.Bounty.RefreshActiveQuests();
			if (!FunkyGame.Bounty.ActiveQuests.ContainsKey(337492) || FunkyGame.Bounty.ActiveQuests[337492].Step != 34)
			{
				Logger.DBLog.Info("Active Quest no longer valid for reward!");
				m_IsDone = true;
				return true;
			}

			//Check if the rift key upgrade continue button is visible
			if (UI.ValidateUIElement(UI.Game.RiftReward_UpgradeKey_ContinueButton))
			{
				UI.Game.RiftReward_UpgradeKey_ContinueButton.Click();
				await Coroutine.Sleep(250);
				await Coroutine.Yield();
			}

			if (!delayer.Test(5))
			{
				await Coroutine.Sleep(250);
				await Coroutine.Yield();
			}

			//Check if the NPC dialog is still valid
			if (!UI.ValidateUIElement(UI.Game.SalvageMainPage))
			{
				Logger.DBLog.Info("Rift Reward Dialog not valid.");
				m_IsDone = true;
				return true;
			}

			//Upgrading Gems?
			if (UI.Game.RiftReward_gemUpgradePane_UpgradeButton.IsVisible)
			{
				if (!delayer.Test(10))
				{
					await Coroutine.Sleep(250);
					await Coroutine.Yield();
				}

				Logger.DBLog.Info("Upgrading Gems..");

				//Update our variables (UIElements of gems, number of gems, and the gem ACDItems)
				if (SelectableGems.Count==0)
				{
					GRiftLevel = GetTieredLootLevel();
					SelectableUIGemElements = GetGemUIElements();
					int totalGemUIElements = SelectableUIGemElements.Count;
					SelectableUIGemElementIndexs = GetGemACDGuids(totalGemUIElements);
					SelectableGems = GetGemCacheACDItems(SelectableUIGemElementIndexs).OrderByDescending(i => i.LegendaryGemRank).ToList();
				}

				//Check if the upgrade button is enabled.. if not we select our gem!
				if (!UI.Game.RiftReward_gemUpgradePane_UpgradeButton.IsEnabled)
				{
					//Set our default upgrade to our highest ranked (thats not 0%)
					var upgradingGem = SelectableGems.FirstOrDefault();
					int selectIndex = SelectableUIGemElementIndexs.IndexOf(upgradingGem.ACDGUID);
					string GemName = upgradingGem.ThisRealName;

					//Check if any of the gems are ranked and are greater than 8% chance..
					var rankedGems = SelectableGems.Where(g => g.LegendaryGemRank > 0 && GRiftLevel-g.LegendaryGemRank>-3).ToList();
					if (rankedGems.Count>0)
					{
						selectIndex = SelectableUIGemElementIndexs.IndexOf(rankedGems[0].ACDGUID);
						GemName = rankedGems[0].ThisRealName;
					}

					Logger.DBLog.InfoFormat("Upgrading Gem {0}", GemName);
					
					//Select the Gem UIElement
					UI.Game.RiftReward_gemUpgradePane_List.ItemsListSetSelectedItemByIndex(selectIndex);
					await Coroutine.Sleep(250);
					await Coroutine.Yield();
				}
				else
				{
					UI.Game.RiftReward_gemUpgradePane_UpgradeButton.Click();
					await Coroutine.Sleep(250);
					await Coroutine.Yield();
				}

			}
			else
			{
				Logger.DBLog.Info("Reward Selection..");

				//Click continue after selecting a reward (not used currently)
				if (UI.ValidateUIElement(UI.Game.RiftReward_UpgradeContinue))
				{
					UI.Game.RiftReward_UpgradeContinue.Click();
					await Coroutine.Sleep(250);
					await Coroutine.Yield();
				}

				//Validate that upgrade key is enabled.. then attempt to upgrade!
				if (Reward == RewardType.Key && UI.Game.RiftReward_Choice_UpgradeKey.IsEnabled && ZetaDia.Me.AttemptUpgradeKeystone())
				{
					Logger.DBLog.Info("Keystone Upgraded");
					UIManager.CloseAllOpenFrames();
					await Coroutine.Sleep(250);
					await Coroutine.Yield();
				}
				else 
				{
					//Doing Gem Upgrade..
					GRiftLevel = GetTieredLootLevel();
					var Gems = GetGemCacheACDItems().OrderByDescending(i => i.LegendaryGemRank).ToList();
					if (Gems.Count>0)
					{
						//Logger.DBLog.InfoFormat("Upgrading Gem {0}", Gems[0].ThisRealName);
						await CommonCoroutines.AttemptUpgradeGem(Gems[0].ACDItem);
						await Coroutine.Sleep(250);
						await Coroutine.Yield();
					}
					else
					{
						Reward= RewardType.Key;
						await Coroutine.Sleep(250);
						await Coroutine.Yield();
					}
				}
			}

			

			return true;
		}

		private List<UIElement> GetGemUIElements()
		{
			List<UIElement> list = new List<UIElement>();
			foreach (var uie in UI.GetChildren(UI.Game.RiftReward_gemUpgradePane_List_Content_StackPanel))
			{
				foreach (var b in UI.GetChildren(uie))
				{
					list.Add(b);
				}
			}

			return list;
		}
		private int GetTotalGemUIElements()
		{
			int totalGemUIElements = 0;
			//Get Total Gems Selectable
			foreach (var uie in UI.GetChildren(UI.Game.RiftReward_gemUpgradePane_List_Content_StackPanel))
			{
				foreach (var b in UI.GetChildren(uie))
				{
					totalGemUIElements++;
				}
			}

			return totalGemUIElements;
		}

		private List<int> GetGemACDGuids(int totalGems)
		{
			List<int> GemACDGUIDs = new List<int>();
			for (int i = 0; i < totalGems; i++)
			{
				GemACDGUIDs.Add(UI.Game.RiftReward_gemUpgradePane_List.GetItemsListItemACDGuidByIndex(i));
				//LBDebug.Controls.Add(new UserControlDebugEntry(UI.Game.RiftReward_gemUpgradePane_List.GetItemsListItemACDGuidByIndex(i).ToString()));
			}

			return GemACDGUIDs;
		}

		private List<CacheACDItem> GetGemCacheACDItems(List<int> Acdguids)
		{
			

			List<CacheACDItem> GemList = new List<CacheACDItem>();
			foreach (var item in ZetaDia.Actors.GetActorsOfType<ACDItem>().Where(item => Acdguids.Contains(item.ACDGuid)))
			{
				CacheACDItem cItem = new CacheACDItem(item);
				if (cItem.LegendaryGemRank == 50) continue;
				if (GRiftLevel - cItem.LegendaryGemRank == -7) continue;
				GemList.Add(cItem);
			}

			return GemList;
		}

		private List<CacheACDItem> GetGemCacheACDItems()
		{
			List<CacheACDItem> Gems=new List<CacheACDItem>();
			foreach (var gem in ZetaDia.Actors.GetActorsOfType<ACDItem>().Where(item => item.ItemType == ItemType.LegendaryGem))
			{
				var cItem = new CacheACDItem(gem);
				if (cItem.LegendaryGemRank == 50) continue;
				if (GRiftLevel - cItem.LegendaryGemRank == -7) continue;
				Gems.Add(new CacheACDItem(gem));
			}

			return Gems;
		}

		private int GetTieredLootLevel()
		{
			return ZetaDia.Me.CommonData.GetAttribute<int>(ActorAttributeType.InTieredLootRunLevel);
		}

		public override void ResetCachedDone()
		{
			m_IsDone = false;
			base.ResetCachedDone();
		}

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}
	}
}
