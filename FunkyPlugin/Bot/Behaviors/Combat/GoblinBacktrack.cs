using System;
using System.Linq;
using Zeta;
using Zeta.TreeSharp;
using Zeta.Navigation;
using Zeta.Common;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  public class GoblinRewind
		  {
				public static CacheObject Goblin=null;
				public static bool ShouldGoblinRewind=false;

				private static MoveResult LastMoveResult;
				private static bool Initilized=false;
				private static bool clientNavFailed=false;
				private static int RayCastDistance=200;
				private static Vector3 tmpDestination=new Vector3();

				public static RunStatus PreformGoblinRewindBehavior()
				{
					 Zeta.CommonBot.BotMain.StatusText="Goblin backtrack behavior";

					 //Check if we should continue preforming..
					 if (!ZetaDia.IsInGame || ZetaDia.Me.IsDead ||Zeta.CommonBot.Logic.BrainBehavior.ShouldVendor)
						  return RunStatus.Success;

					 if (!Initilized)
					 {
						  Initilized=true;
						  tmpDestination=Goblin.Position;
						  Navigator.Clear();
					 }

					 //Backtrack to the position of the goblin, checking and preforming combat/loot related actions along the way.
					 if (dbRefresh.ShouldRefreshObjectList)
						  dbRefresh.RefreshDiaObjects();

					 //Check if we have any NEW targets to deal with.. 
					 //Note: Refresh will filter targets to units and avoidance ONLY.
					 if (Bot.Target.ObjectData!=null)
					 {
						  //Directly Handle Target..
						  RunStatus targetHandler=Bot.Target.HandleThis();

						  //Only return failure if handling failed..
						  if (targetHandler==RunStatus.Failure)
								return RunStatus.Success;
						  else if (targetHandler==RunStatus.Success)
								Bot.Combat.ResetTargetHandling();

						  return RunStatus.Running;
					 }



					 if (Goblin.CentreDistance>20f)
					 {//Preform movement navigation

						  //Navigator.NavigationProvider.MoveTo(Goblin.Position);

						  
						  if (clientNavFailed&&RayCastDistance>20)
						  {
								RayCastDistance=RayCastDistance-10;
						  }
						  else if (clientNavFailed&&RayCastDistance<=20)
						  {
								RayCastDistance=200;
						  }

						  tmpDestination=Goblin.Position;

						  if (Goblin.CentreDistance>RayCastDistance)
						  {
								float MaxReduction=Math.Min(Goblin.CentreDistance, RayCastDistance);
								tmpDestination=MathEx.CalculatePointFrom(ZetaDia.Me.Position, tmpDestination, Goblin.CentreDistance-MaxReduction);
						  }

						  LastMoveResult=Navigator.MoveTo(tmpDestination, "goblinbacktrack", true);


						  switch (LastMoveResult)
						  {
								case MoveResult.Moved:
								case MoveResult.ReachedDestination:
									 clientNavFailed=false;
									 break;
								case MoveResult.UnstuckAttempt:
								case MoveResult.PathGenerated:
								case MoveResult.PathGenerating:
								case MoveResult.PathGenerationFailed:
								case MoveResult.Failed:
									 clientNavFailed=true;
									 break;
						  }

						  Zeta.CommonBot.BotMain.StatusText="Goblin Backtrack Moveresult: "+LastMoveResult.ToString() + " Goblins Position: " + Goblin.Position.ToString();
						  

						  return RunStatus.Running;

					 }
					 else
					 {//We are in range but refreshing didn't set target data..

						  //See if we killed the target?
					 }
					 Initilized=false;
					 ShouldGoblinRewind=false;
					 Goblin=null;

					 return RunStatus.Success;
				}
		  }
	 }
}