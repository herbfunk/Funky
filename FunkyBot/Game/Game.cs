using FunkyBot.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Internals.Service;

namespace FunkyBot.Game
{
	//Tracks Stats, Profile related properties, and general in-game info
	public class GameCache
	{
		///<summary>
		///Tracks the total stats while bot is running. (totals of each game)
		///</summary>
		internal TotalStats TrackingStats = new TotalStats();
		internal ProfileCache Profile = new ProfileCache();

		internal ActorClass ActorClass = ActorClass.Invalid;
		internal string CurrentAccountName;
		internal string CurrentHeroName;
		internal int CurrentLevel;
		///<summary>
		///Updates Account Name, Current Hero Name and Class Variables
		///</summary>
		internal void UpdateCurrentAccountDetails()
		{
			//Clear Cache -- (DB reuses values, even if it is incorrect!)
			ZetaDia.Memory.ClearCache();


			try
			{
				using (ZetaDia.Memory.AcquireFrame())
				{
					ActorClass = ZetaDia.Service.CurrentHero.Class;
					CurrentAccountName = ZetaDia.Service.CurrentHero.BattleTagName;
					CurrentHeroName = ZetaDia.Service.CurrentHero.Name;
					CurrentLevel = ZetaDia.Service.CurrentHero.Level;
				}
			}
			catch (Exception)
			{
				Logging.WriteDiagnostic("[Funky] Exception Attempting to Update Current Account Details.");
			}
		}

		internal GameId currentGameID = new GameId();
		internal bool RefreshGameID()
		{
			GameId curgameID = currentGameID;
			using (ZetaDia.Memory.AcquireFrame())
			{
				curgameID = ZetaDia.Service.CurrentGameId;
			}

			if (!curgameID.Equals(currentGameID))
			{
				if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.OutOfCombat))
				{
					Logger.Write(LogLevel.OutOfCombat, "New Game Started");
				}

				//Update our Profile Tracking Stats (records last game)
				this.TrackingStats.GameChanged();

				//Update Account Details
				this.UpdateCurrentAccountDetails();

				//Clear TrinityLoadOnce Used Profiles!
				FunkyBot.XMLTags.TrinityLoadOnce.UsedProfiles.Clear();

				//Clear Interactable Cache
				Bot.Game.Profile.InteractableObjectCache.Clear();

				//Clear Health Average
				ObjectCache.Objects.ClearHealthAverageStats();

				//Renew bot
				Funky.ResetBot();

				currentGameID = curgameID;
				return true;
			}

			return false;
		}
	}
}
