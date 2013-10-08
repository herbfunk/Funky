using System;
using System.Drawing.Imaging;
using FunkyBot.Cache;
using FunkyBot.Movement;
using FunkyBot.Settings;
using FunkyBot.Targeting;
using Zeta;
using Zeta.CommonBot;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.CommonBot.Settings;
using Zeta.Internals;
using Zeta.Internals.Actors;
using System.Threading;
using FunkyBot.Avoidances;

namespace FunkyBot
{

		  //This class is used to hold the data

		  public static partial class Bot
		  {
				public static Settings_Funky Settings=new Settings_Funky();
				public static Player Class { get; set; }

				private static CharacterCache character=new CharacterCache();
				public static CharacterCache Character
				{
					 get { return character; }
					 set { character=value; }
				}
				public static CombatCache Combat { get; set; }
				public static TargetingHandler Targeting { get; set; }

				private static BotStatistics Stats_=new BotStatistics();
				internal static BotStatistics Stats
				{
					 get { return Stats_; }
					 set { Stats_=value; }
				}
				private static ProfileCache profile=new ProfileCache();
				public static ProfileCache Profile
				{
					 get { return profile; }
					 set { profile=value; }
				}

				public static Navigation NavigationCache { get; set; }


				// Darkfriend's Looting Rule
				internal static Interpreter ItemRulesEval;



				
				internal static ActorClass ActorClass=ActorClass.Invalid;
				internal static string CurrentAccountName;
				internal static string CurrentHeroName;
				internal static int CurrentLevel;

				///<summary>
				///Updates Account Name, Current Hero Name and Class Variables
				///</summary>
				internal static void UpdateCurrentAccountDetails()
				{

					 try
					 {
						  ActorClass=ZetaDia.Service.CurrentHero.Class;
						  CurrentAccountName=ZetaDia.Service.CurrentHero.BattleTagName;
						  CurrentHeroName=ZetaDia.Service.CurrentHero.Name;
						  CurrentLevel=ZetaDia.Service.CurrentHero.Level;
					 } catch (Exception)
					 {
						  Logging.WriteDiagnostic("[Funky] Exception Attempting to Update Current Account Details.");
					 }
				}

				///<summary>
				///Checks behavioral flags that are considered OOC/Non-Combat
				///</summary>
				internal static bool IsInNonCombatBehavior
				{
					 get
					 {
						  //OOC IDing, Town Portal Casting, Town Run
						  return (Bot.Profile.IsRunningOOCBehavior||Funky.FunkyTPBehaviorFlag||Funky.TownRunManager.bWantToTownRun);
					 }
				}






				internal static void Reset()
				{
					 Class=null;
					 character=new CharacterCache();
					 Combat=new CombatCache();
					 Targeting=new TargetingHandler();
					 NavigationCache=new Navigation();
					 Stats_=new BotStatistics();
				}
		  }
	 
}