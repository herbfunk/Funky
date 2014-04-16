using System;
using System.Collections.Generic;
using System.Windows.Documents;
using FunkyBot.Cache;
using FunkyBot.XMLTags;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Service;

namespace FunkyBot.Game
{
	//Tracks Stats, Profile related properties, and general in-game info
	public class GameCache
	{
		///<summary>
		///Tracking of All Game Stats 
		///</summary>
		internal TotalStats TrackingStats = new TotalStats();

		///<summary>
		///Tracking of current Game Stats
		///</summary>
		internal GameStats CurrentGameStats = new GameStats();

		internal GoldInactivity GoldTimeoutChecker = new GoldInactivity();

		internal ProfileCache Profile = new ProfileCache();

		internal BountyCache Bounty = new BountyCache();

		public bool AdventureMode { get { return _adventureMode; } }
		private bool _adventureMode = false;

		private GameId _currentGameId = new GameId();
		internal bool RefreshGameId()
		{
			GameId curgameID = _currentGameId;
			int questId = 0;
			using (ZetaDia.Memory.AcquireFrame())
			{
				curgameID = ZetaDia.Service.CurrentGameId;
				questId = ZetaDia.CurrentQuest.QuestSNO;
			}

			if (!curgameID.Equals(_currentGameId))
			{

				Logger.Write(LogLevel.OutOfCombat, "New Game Started");

				//Adventure Mode (QuestID == 312429)
				_adventureMode = questId == 312429;
				if (AdventureMode) Bounty.UpdateBounties();

				//Merge last GameStats with the Total
				TrackingStats.GameChanged(ref CurrentGameStats);

				//Create new GameStats
				CurrentGameStats = new GameStats();

				//Update Account Details
				Bot.Character.Account.UpdateCurrentAccountDetails();

				//Clear TrinityLoadOnce Used Profiles!
				TrinityLoadOnce.UsedProfiles.Clear();

				//Clear Interactable Cache
				Profile.InteractableObjectCache.Clear();

				//Clear Health Average
				ObjectCache.Objects.ClearHealthAverageStats();

				//Renew bot
				Funky.ResetBot();

				//Gold Inactivity
				GoldTimeoutChecker.LastCoinageUpdate = DateTime.Now;

				_currentGameId = curgameID;
				return true;
			}

			return false;
		}
		public static UIElement BountyCompleteContinue
		{
			get
			{
				try { return UIElement.FromHash(0x278249110947CA00); }
				catch { return null; }
			}
		}
		//
		internal void CheckUI()
		{
			if (BountyCompleteContinue != null && BountyCompleteContinue.IsValid && BountyCompleteContinue.IsVisible)
			{
				Logger.DBLog.Info("Funky Clicking Bounty Dialog!");
				BountyCompleteContinue.Click();
			}
		}
	}
}
