using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Markup;
using Zeta;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.CommonBot;
using Zeta.CommonBot.Profile;
using Zeta.CommonBot.Profile.Composites;
using Zeta.Internals;
using Zeta.Internals.Actors;
using Zeta.Internals.Actors.Gizmos;
using Zeta.Internals.SNO;
using Zeta.Navigation;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot
{
	 public partial class Funky
	 {
		  internal static DateTime FunkyTP_LastCastAttempt=DateTime.MinValue;
		  internal static Vector3 StartingPosition=Vector3.Zero;
		  internal static bool MovementOccured=false;

		 public static bool FunkyTPBehaviorFlag=false;

		  public static bool FunkyTPOverlord(object ret)
		  {
				//Ingame and not dead?
				if (TPActionIsValid())
				{
					 //If not already in town, check if we can cast..
					 if (!ZetaDia.Me.IsInTown)
					 {
						  return CanCastTP();
					 }
				}

				//No reason to run behavior..
				return false;
		  }

		  internal static bool TPActionIsValid()
		  {
				try
				{
					 if (!ZetaDia.IsInGame||ZetaDia.Me.IsDead)
						  return false;
					 else
						  return true;
				} catch (Exception)
				{
					 return false;
				}

		  }

		  internal static bool CanCastTP()
		  {
				string TPcastTest;
				bool cancast;
				try
				{
					 cancast=ZetaDia.Me.CanUseTownPortal(out TPcastTest);
				} catch (NullReferenceException)
				{
					 TPcastTest="Exception during CanUseTownPortal";
					 cancast=false;
				}
				
				if (!cancast)
				{
					 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.OutOfCombat))
						  Logger.Write(LogLevel.OutOfCombat,"Cannot cast TP: "+TPcastTest);
					 return false;
				}
				return true;
		  }

		  public static void ResetTPBehavior()
		  {
				worldtransferStarted=false;
				FunkyTPBehaviorFlag=false;
				worldChanged=false;
				initizedTPBehavior=false;
				FunkyTP_LastCastAttempt=DateTime.MinValue;
				//GameEvents.OnWorldChanged-=OnWorldChanged;
				//GameEvents.OnWorldTransferStart-=OnWorldChangeStart;
				StartingPosition=Vector3.Zero;
		  }

		  internal static bool initizedTPBehavior=false;
		  internal static void InitTPBehavior()
		  {
				Logger.Write(LogLevel.OutOfCombat,"Starting TP Behavior",true);
				worldtransferStarted=false;
				worldChanged=false;
				//GameEvents.OnWorldChanged+=OnWorldChanged;
				//GameEvents.OnWorldTransferStart+=OnWorldChangeStart;
				initizedTPBehavior=true;
				StartingPosition=Bot.Character.Position;
				MovementOccured=false;
		  }

		  //Events below are not used ATM.
		  internal static bool worldtransferStarted=false;
		  internal static bool worldChanged=false;

		  internal static bool CastingRecall()
		  {
				if (Bot.Character.CurrentAnimationState!=AnimationState.Idle)
				{
					 switch (Bot.Character.CurrentSNOAnim)
					 {
						  case SNOAnim.barbarian_male_HTH_Recall_Channel_01:
						  case SNOAnim.Barbarian_Female_HTH_Recall_Channel_01:
						  case SNOAnim.Monk_Male_recall_channel:
						  case SNOAnim.Monk_Female_recall_channel:
						  case SNOAnim.WitchDoctor_Male_recall_channel:
						  case SNOAnim.WitchDoctor_Female_recall_channel:
						  case SNOAnim.Wizard_Male_HTH_recall_channel:
						  case SNOAnim.Wizard_Female_HTH_recall_channel:
						  case SNOAnim.Demonhunter_Male_HTH_recall_channel:
						  case SNOAnim.Demonhunter_Female_HTH_recall_channel:
								return true;
					 }
				}
				return false;
		  }

		  public static Composite FunkyTownPortal()
		  {
				CanRunDecoratorDelegate canRunDelegateReturnToTown=new CanRunDecoratorDelegate(FunkyTPOverlord);
				ActionDelegate actionDelegateReturnTown=new ActionDelegate(FunkyTPBehavior);
				Sequence sequenceReturnTown=new Sequence(
					new Zeta.TreeSharp.Action(actionDelegateReturnTown)
					);
				return new Zeta.TreeSharp.Decorator(canRunDelegateReturnToTown, sequenceReturnTown);
		  }
		  internal static bool CastAttempted=false;
		  public static RunStatus FunkyTPBehavior(object ret)
		  {
				//Init
				if (!initizedTPBehavior)
				{
					 InitTPBehavior();
					 return RunStatus.Running;
				}

				double ElapsedTime=DateTime.Now.Subtract(FunkyTP_LastCastAttempt).TotalSeconds;

				//Check world transfer start
				if (worldtransferStarted)
				{
					 if (ElapsedTime<10||worldChanged)
					 {
						 //Logger.Write(LogLevel.OutOfCombat,"Waiting for world change!");

						  if (!ZetaDia.Me.IsInTown)
								return RunStatus.Running;
						  else
						  {
								Logger.Write(LogLevel.OutOfCombat,"Casting Behavior Finished, we are in town!", true);
								ResetTPBehavior();
								//UpdateSearchGridProvider(true);
								return RunStatus.Success;
						  }
					 }
					 else if (ElapsedTime>=10&&!ZetaDia.Me.IsInTown)
					 {
						  //Retry?
						  worldtransferStarted=false;
						  CastAttempted=false;
						  Vector3 UnstuckPos;
                          if (Bot.NavigationCache.AttemptFindSafeSpot(out UnstuckPos, Vector3.Zero, Bot.Settings.Plugin.AvoidanceFlags))
						  {
								Logger.Write(LogLevel.OutOfCombat, "Generated Unstuck Position at {0}", UnstuckPos.ToString());
								ZetaDia.Me.UsePower(SNOPower.Walk, UnstuckPos, Bot.Character.iCurrentWorldID, -1);
						  }

					 }

					 return RunStatus.Running;
				}

				//Precheck - Ingame, not dead..
				if (!TPActionIsValid())
				{
					 ResetTPBehavior();
					 return RunStatus.Success;
				}
				else if (ZetaDia.IsLoadingWorld) //Loading.. we just wait!
					 return RunStatus.Running;
				else if (!CanCastTP()) //Not loading but is valid.. see if we can cast?
				{
					 ResetTPBehavior();
					 return RunStatus.Success;
				}

				//Set our flag which is used to setup the refreshing specific for this/similar behaviors.
				FunkyTPBehaviorFlag=true;

				//Refresh?
				if (Bot.Targeting.ShouldRefreshObjectList)
				{
					 Bot.Targeting.RefreshDiaObjects();
				}

				//Check if we have any NEW targets to deal with.. 
				//Note: Refresh will filter targets to units and avoidance ONLY.
				if (Bot.Targeting.CurrentTarget!=null)
				{
					 //Directly Handle Target..
					 RunStatus targetHandler=Bot.Targeting.HandleThis();

					 //Only return failure if handling failed..
					 if (targetHandler==RunStatus.Failure)
					 {
						  ResetTPBehavior();
						  return RunStatus.Success;
					 }
					 else if (targetHandler==RunStatus.Success)
						  Bot.Targeting.ResetTargetHandling();

					 return RunStatus.Running;
				}
				else if (MovementOccured)
				{
					 //Backtrack to orginal location...

					 bool isMoving=false;
					 try
					 {
						  isMoving=ZetaDia.Me.Movement.IsMoving;
					 } catch (NullReferenceException) { }

					 //Use simple checking of movement, with UsePower on our last location.
					 if (!isMoving)
					 {
						  double DistanceFromStart=StartingPosition.Distance(Bot.Character.Position);

						  if (DistanceFromStart>15f&&DistanceFromStart<50f)
						  {
								//Logging.WriteVerbose("[FunkyTP] Backtracking!");
								//Move back to starting position..
								//ZetaDia.Me.UsePower(SNOPower.Walk, StartingPosition);
								//return RunStatus.Running;
						  }
						  else if (DistanceFromStart>=50f)
						  {
								//Logging.WriteVerbose("[FunkyTP] Range from our starting position is {0}. Now using Navigator to move.", DistanceFromStart);
								//Navigator.MoveTo(StartingPosition, "Backtracking to Orginal Position", true);
						  }
					 }
					 else
						  return RunStatus.Running;

					 MovementOccured=false;
				}

				//Update Movement Data
				Bot.NavigationCache.RefreshMovementCache();

				//Make sure we are not moving..
				if (Bot.NavigationCache.IsMoving)
					 return RunStatus.Running;

				//Check if we are casting, if not cast, else if casting but time has elapsed then cancel cast.

				if (!CastingRecall())
				{
					 //Check last time cast..
					 if (ElapsedTime>5&&CastAttempted)
					 {
						  worldtransferStarted=true;
						  return RunStatus.Running;
					 }
					 else if (ElapsedTime>8||!CastAttempted)
					 {
						  //Recast
						  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.OutOfCombat))
								Logger.Write(LogLevel.OutOfCombat,"Casting TP..");
						  ZetaDia.Me.UseTownPortal();
						  CastAttempted=true;
						  FunkyTP_LastCastAttempt=DateTime.Now;
					 }

					 return RunStatus.Running;
				}
				else
				{
					 if (ElapsedTime>8)
					 {
						  //Void Cast?
						  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.OutOfCombat))
								Logger.Write(LogLevel.OutOfCombat,"Attempting to void cast with movement..");
						  Vector3 V3loc;
                          bool success = Bot.NavigationCache.AttemptFindSafeSpot(out V3loc, Vector3.Zero, Bot.Settings.Plugin.AvoidanceFlags);
						  if (success)
						  {
								Zeta.Navigation.Navigator.MoveTo(V3loc, "Void Cast Movement", false);
						  }
						  
						  return RunStatus.Running;
					 }

					 return RunStatus.Running;
				}
		  }

		  internal static RunStatus FunkyTownPortalTownRun(object ret)
		  {
				TownRunManager.TownrunStartedInTown=false;
				return RunStatus.Success;
		  }
	 }
}