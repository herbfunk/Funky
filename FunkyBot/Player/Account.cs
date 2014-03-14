using System;
using Zeta.Game;

namespace FunkyBot.Player
{
	class Account
	{
		public Account()
		{
			ActorClass = ActorClass.Invalid;
			CurrentAccountName = String.Empty;
			CurrentHeroName = String.Empty;
			CurrentLevel = 0;
		}

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
					
					ActorClass = ZetaDia.Service.Hero.Class;
					CurrentAccountName = ZetaDia.Service.Hero.BattleTagName;
					CurrentHeroName = ZetaDia.Service.Hero.Name;
					CurrentLevel = ZetaDia.Service.Hero.Level;
				}
			}
			catch (Exception)
			{
				Logger.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Account Details.");
			}
		}
	}
}
