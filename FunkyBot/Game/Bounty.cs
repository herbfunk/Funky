using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zeta.Game;
using Zeta.Game.Internals;

namespace FunkyBot.Game
{
	public class BountyCache
	{
		public Dictionary<int, QuestState> BountyQuestStates = new Dictionary<int, QuestState>();
		public Dictionary<int, BountyInfo> CurrentBounties = new Dictionary<int, BountyInfo>();


		private BountyInfo currentBountyInfo = null;

		private int currentBountyID = 0;
		///<summary>
		///Current SNO ID (set by BountyLoad tag)
		///</summary>
		public int CurrentBountyID
		{
			get { return currentBountyID; }
			set 
			{ 
				currentBountyID = value;
				if (CurrentBounties.ContainsKey(value)) currentBountyInfo = CurrentBounties[value];
				Logger.DBLog.DebugFormat("Setting Current Bounty ID to {0}", value);
			}
		}


		///<summary>
		///Refreshes Bounty Info Cache
		///</summary>
		public void UpdateBounties()
		{
			CurrentBounties.Clear();
			foreach (var bounty in ZetaDia.ActInfo.Bounties)
			{
				CurrentBounties.Add(bounty.Info.QuestSNO, bounty);
			}
		}
		///<summary>
		///Refreshes Current Bounty Quest States
		///</summary>
		public void RefreshBountyInfo()
		{
			BountyQuestStates.Clear();

			foreach (var bounty in CurrentBounties.Values)
			{
				BountyQuestStates.Add(bounty.Info.QuestSNO, bounty.Info.State);
			}
		}
	}
}
