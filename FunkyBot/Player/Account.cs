using System;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;

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
	}
}
