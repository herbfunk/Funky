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
using Zeta.Internals.Service;
using FunkyBot.Character;
using FunkyBot.Game;

namespace FunkyBot
{

		  //This class is used to hold the data

		  public static class Bot
		  {
				public static Settings_Funky Settings=new Settings_Funky();

				///<summary>
				///Skills, Hotbar, Buffs and Combat Stats of Current Character
				///</summary>
				public static Player Class { get; set; }
				///<summary>
				///Values of the Current Character
				///</summary>
				public static CharacterCache Character = new CharacterCache();
				//TODO:: Create base Class to contain Player and CharacterCache classes

				public static TargetingHandler Targeting { get; set; }

				///<summary>
				///Game Stats and Values of Current Character
				///</summary>
				public static GameCache Game = new GameCache();
				//Initalized once for total stats tracking

				///<summary>
				///Contains movement related properties and methods pretaining to the Bot itself.
				///</summary>
				public static Navigation NavigationCache { get; set; }


				// Darkfriend's Looting Rule
				internal static Interpreter ItemRulesEval;
				
	

				///<summary>
				///Checks behavioral flags that are considered OOC/Non-Combat
				///</summary>
				internal static bool IsInNonCombatBehavior
				{
					 get
					 {
						  //OOC IDing, Town Portal Casting, Town Run
						 return (Bot.Game.Profile.IsRunningOOCBehavior || Funky.FunkyTPBehaviorFlag || Funky.TownRunManager.bWantToTownRun);
					 }
				}

				//Recreate Bot Classes
				internal static void Reset()
				{
					
					Character = new CharacterCache();
					Targeting = new TargetingHandler();
					NavigationCache = new Navigation();

					//Nullify to be updated inside  GlobalOverlord method
					Class = null;
				}
		  }
	 
}