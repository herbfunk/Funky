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
			
				public static TargetingHandler Targeting { get; set; }

                ///<summary>
                ///
                ///</summary>
				public static GameCache Game = new GameCache();

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






				internal static void Reset()
				{
					 Class=null;
					 character=new CharacterCache();
					 Targeting=new TargetingHandler();
					 NavigationCache=new Navigation();
				}
		  }
	 
}