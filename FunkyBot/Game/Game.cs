using System;
using FunkyBot.Cache;
using FunkyBot.XMLTags;
using Zeta;
using Zeta.Internals.Service;

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

		internal GoldInactivity GoldTimeoutChecker=new GoldInactivity();


		internal ProfileCache Profile = new ProfileCache();

		private GameId _currentGameId = new GameId();
		internal bool RefreshGameId()
		{
			GameId curgameID = _currentGameId;
			using (ZetaDia.Memory.AcquireFrame())
			{
				curgameID = ZetaDia.Service.CurrentGameId;
			}

			if (!curgameID.Equals(_currentGameId))
			{
				if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.OutOfCombat))
				{
					Logger.Write(LogLevel.OutOfCombat, "New Game Started");
				}

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
				GoldTimeoutChecker.LastCoinageUpdate=DateTime.Now;

				_currentGameId = curgameID;
				return true;
			}

			return false;
		}
	}
}
