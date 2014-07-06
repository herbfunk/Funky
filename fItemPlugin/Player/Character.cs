using System;
using System.Linq;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace fItemPlugin.Player
{
	public static class Character
	{
		public static bool GameIsInvalid()
		{
			return !ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null;
		}

		private static string _CurrentAccountName = String.Empty;
		public static string CurrentAccountName
		{
			get { return _CurrentAccountName; }
			set { _CurrentAccountName = value; }
		}


		private static string _CurrentHeroName = String.Empty;
		public static string CurrentHeroName
		{
			get { return _CurrentHeroName; }
			set { _CurrentHeroName = value; }
		}

		public static bool UpdateAccoutDetails()
		{
			//Check game is valid!
			//if (GameIsInvalid())
			//return false;

			//Update!
			if (!BotMain.IsRunning)
			{
				ZetaDia.Memory.ClearCache();
			}

			string tmpHeroName = String.Empty;
			string tmpAccountName = String.Empty;

			try
			{
				using (ZetaDia.Memory.AcquireFrame())
				{
					//Get current Names!
					tmpHeroName = ZetaDia.Service.Hero.Name;
					tmpAccountName = ZetaDia.Service.Hero.BattleTagName;
				}
			}
			catch (Exception)
			{
				FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Account Details.");
				return false;
			}

			//Check the values..
			if (tmpHeroName.ToCharArray().Any(c => !Char.IsLetter(c)))
			{
				FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Hero Name Invalid (Contains Non-Letter Character) - {0}", tmpHeroName);
				return false;
			}
			if (!tmpAccountName.Contains("#"))
			{
				FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Account Name Invalid (Missing #) - {0}", tmpAccountName);
				return false;
			}
			if (tmpHeroName.Trim() == String.Empty || tmpAccountName.Trim() == String.Empty)
			{
				FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Account Details are Invalid");
				return false;
			}

			CurrentAccountName = tmpAccountName;
			CurrentHeroName = tmpHeroName;
			return true;
		}


		private static bool isMoving = false;
		public static bool IsMoving
		{
			get
			{
				if (ShouldRefreshMovementProperty)
					RefreshMovementCache();

				return isMoving;
			}
		}
		private static DateTime LastUpdatedMovementData = DateTime.MinValue;
		private static bool ShouldRefreshMovementProperty
		{
			get { return DateTime.Now.Subtract(LastUpdatedMovementData).TotalMilliseconds < 150; }
		}
		private static void RefreshMovementCache()
		{
			LastUpdatedMovementData = DateTime.Now;

			//These vars are used to accuratly predict what the bot is doing (Melee Movement/Combat)
			using (ZetaDia.Memory.AcquireFrame())
			{
				// If we aren't in the game of a world is loading, don't do anything yet
				if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld)
					return;
				try
				{
					ActorMovement botMovement = ZetaDia.Me.Movement;
					isMoving = botMovement.IsMoving;
				}
				catch
				{

				}
			}
		}

	}
}
