using System;
using System.Collections.Generic;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot.Profile;
using Zeta.Internals;
using Zeta.Internals.SNO;
using Zeta.Navigation;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action=Zeta.TreeSharp.Action;
namespace FunkyBot.XMLTags
{


	 [XmlElement("TrinityMoveToScene")]
	 public class TrinityMoveToScene : ProfileBehavior
	 {
		  /// <summary>
		  /// The Scene SNOId
		  /// </summary>
		  [XmlAttribute("sceneId")]
		  public int SceneId { get; set; }

		  /// <summary>
		  /// The Scene Name, will match a sub-string
		  /// </summary>
		  [XmlAttribute("sceneName")]
		  public string SceneName { get; set; }

		  /// <summary>
		  /// The distance the bot will mark the position as visited
		  /// </summary>
		  [XmlAttribute("pathPrecision")]
		  public float PathPrecision { get; set; }

		  /// <summary>
		  /// The current player position
		  /// </summary>
		  private Vector3 myPos { get { return Bot.Character.Data.Position; } }

		  /// <summary>
		  /// The last scene SNOId we entered
		  /// </summary>
		  private int mySceneId=-1;
		  /// <summary>
		  /// The last position we updated the ISearchGridProvider at
		  /// </summary>
		  private Vector3 GPUpdatePosition=Vector3.Zero;

		  /// <summary>
		  /// Called when the profile behavior starts
		  /// </summary>
		  public override void OnStart()
		  {
				Logging.WriteDiagnostic("TrinityMoveToScene OnStart() called");

				if (PathPrecision==0)
					 PathPrecision=15f;

				if (SceneId==0&&String.IsNullOrEmpty(SceneName.Trim()))
				{
					 Logging.WriteDiagnostic("TrinityMoveToScene: No sceneId or sceneName specified!");
					 isDone=true;
				}

		  }

		  protected override Composite CreateBehavior()
		  {
				return
				new Sequence(
					 UpdateSearchGridProvider(),
					 new PrioritySelector(
						  PrioritySceneCheck()
					 )
				);
		  }

		  /// <summary>
		  /// Will find and move to Prioritized Scene's based on Scene SNOId or Name
		  /// </summary>
		  /// <returns></returns>
		  private Composite PrioritySceneCheck()
		  {
				return
				new Decorator(ret => !(SceneId==0&&String.IsNullOrEmpty(SceneName.Trim())),
					 new Sequence(
						  new DecoratorContinue(ret => DateTime.Now.Subtract(lastCheckedScenes).TotalMilliseconds>1000,
								new Sequence(
									 new Action(ret => lastCheckedScenes=DateTime.Now),
									 new Action(ret => FindPrioritySceneTarget())
								)
						  ),
						  new Decorator(ret => PrioritySceneTarget!=Vector3.Zero,
								new PrioritySelector(
									 new Decorator(ret => PrioritySceneTarget.Distance2D(myPos)<=PathPrecision,
										  new Sequence(
												new Action(ret => Logging.WriteDiagnostic("Successfully navigated to priority scene {0} {1} center {2} Distance {3:0}",
													 CurrentPriorityScene.Name, CurrentPriorityScene.SceneInfo.SNOId, PrioritySceneTarget, PrioritySceneTarget.Distance2D(myPos))),
												new Action(ret => isDone=true)
										  )
									 ),
									 new Action(ret => MoveToPriorityScene())
								)
						  )
					 )
				);
		  }
		  /// <summary>
		  /// Handles actual movement to the Priority Scene
		  /// </summary>
		  private void MoveToPriorityScene()
		  {
				Logging.WriteDiagnostic("Moving to Priority Scene {0} - {1} Center {2} Distance {3:0}",
					 CurrentPriorityScene.Name, CurrentPriorityScene.SceneInfo.SNOId, PrioritySceneTarget, PrioritySceneTarget.Distance2D(myPos));

				var moveResult=Funky.PlayerMover.NavigateTo(PrioritySceneTarget);

				if (moveResult==MoveResult.PathGenerationFailed)
				{
					 Logging.WriteDiagnostic("Unable to navigate to Scene {0} - {1} Center {2} Distance {3:0}, cancelling!",
						  CurrentPriorityScene.Name, CurrentPriorityScene.SceneInfo.SNOId, PrioritySceneTarget, PrioritySceneTarget.Distance2D(myPos));
					 PrioritySceneMoveToFinished();
				}
		  }

		  /// <summary>
		  /// Sets a priority scene as finished
		  /// </summary>
		  private void PrioritySceneMoveToFinished()
		  {
				PriorityScenesInvestigated.Add(PrioritySceneSNOId);
				PrioritySceneSNOId=-1;
				PrioritySceneTarget=Vector3.Zero;
		  }


		  /// <summary>
		  /// Updates the search grid provider as needed
		  /// </summary>
		  /// <returns></returns>
		  private Composite UpdateSearchGridProvider()
		  {
				return
				new DecoratorContinue(ret => mySceneId!=Bot.Character.Data.iSceneID||Vector3.Distance(myPos, GPUpdatePosition)>150,
					 new Sequence(
						  new Action(ret => mySceneId=Bot.Character.Data.iSceneID),
						  new Action(ret => GPUpdatePosition=myPos),
						  new Action(ret => MiniMapMarker.UpdateFailedMarkers())
					 )
				);
		  }
		  private Vector3 PrioritySceneTarget=Vector3.Zero;
		  private int PrioritySceneSNOId=-1;
		  private Scene CurrentPriorityScene;
		  /// <summary>
		  /// A list of Scene SNOId's that have already been investigated
		  /// </summary>
		  private List<int> PriorityScenesInvestigated=new List<int>();

		  private DateTime lastCheckedScenes=DateTime.MinValue;
		  /// <summary>
		  /// Finds a navigable point in a priority scene
		  /// </summary>
		  private void FindPrioritySceneTarget()
		  {
				if (SceneId==0&&String.IsNullOrEmpty(SceneName))
					 return;

				//gp.Update();

				if (PrioritySceneTarget!=Vector3.Zero)
					 return;

				var foundPriorityScene=false;

				// find any matching priority scenes in scene manager - match by name or SNOId

				var PScenes=ZetaDia.Scenes.GetScenes()
					 .Where(s => s.SceneInfo.SNOId==SceneId).ToList();

				PScenes.AddRange(ZetaDia.Scenes.GetScenes()
					  .Where(s => !String.IsNullOrEmpty(SceneName.Trim())&&s.Name.ToLower().Contains(SceneName.ToLower())).ToList());

				var foundPriorityScenes=new List<Scene>();
				var foundPrioritySceneIndex=new Dictionary<int, Vector3>();

				foreach (var scene in PScenes)
				{
					 if (PriorityScenesInvestigated.Contains(scene.SceneInfo.SNOId))
						  continue;

					 foundPriorityScene=true;

					 var navZone=scene.Mesh.Zone;
					 var zoneDef=navZone.NavZoneDef;

					var zoneCenter=GetNavZoneCenter(navZone);

					 var NavCells=zoneDef.NavCells.Where(c => c.Flags.HasFlag(NavCellFlags.AllowWalk)).ToList();

					 if (!NavCells.Any())
						  continue;

					 var bestCell=NavCells.OrderBy(c => GetNavCellCenter(c.Min, c.Max, navZone).Distance2D(zoneCenter)).FirstOrDefault();

					 if (bestCell!=null)
					 {
						  foundPrioritySceneIndex.Add(scene.SceneInfo.SNOId, GetNavCellCenter(bestCell, navZone));
						  foundPriorityScenes.Add(scene);
					 }
					 else
					 {
						  Logging.WriteDiagnostic("Found Priority Scene but could not find a navigable point!");
					 }
				}

				if (foundPrioritySceneIndex.Any())
				{
					 var nearestPriorityScene=foundPrioritySceneIndex.OrderBy(s => s.Value.Distance2D(myPos)).FirstOrDefault();

					 PrioritySceneSNOId=nearestPriorityScene.Key;
					 PrioritySceneTarget=nearestPriorityScene.Value;
					 CurrentPriorityScene=foundPriorityScenes.FirstOrDefault(s => s.SceneInfo.SNOId==PrioritySceneSNOId);

					 Logging.WriteDiagnostic("Found Priority Scene {0} - {1} Center {2} Distance {3:0}",
						  CurrentPriorityScene.Name, CurrentPriorityScene.SceneInfo.SNOId, PrioritySceneTarget, PrioritySceneTarget.Distance2D(myPos));
				}

				if (!foundPriorityScene)
				{
					 PrioritySceneTarget=Vector3.Zero;
				}
		  }

		  /// <summary>
		  /// Gets the center of a given Navigation Zone
		  /// </summary>
		  /// <param name="zone"></param>
		  /// <returns></returns>
		  private Vector3 GetNavZoneCenter(NavZone zone)
		  {
				var X=zone.ZoneMin.X+((zone.ZoneMax.X-zone.ZoneMin.X)/2);
				var Y=zone.ZoneMin.Y+((zone.ZoneMax.Y-zone.ZoneMin.Y)/2);

				return new Vector3(X, Y, 0);
		  }

		  /// <summary>
		  /// Gets the center of a given Navigation Cell
		  /// </summary>
		  /// <param name="cell"></param>
		  /// <param name="zone"></param>
		  /// <returns></returns>
		  private Vector3 GetNavCellCenter(NavCell cell, NavZone zone)
		  {
				return GetNavCellCenter(cell.Min, cell.Max, zone);
		  }

		  /// <summary>
		  /// Gets the center of a given box with min/max, adjusted for the Navigation Zone
		  /// </summary>
		  /// <param name="min"></param>
		  /// <param name="max"></param>
		  /// <param name="zone"></param>
		  /// <returns></returns>
		  private Vector3 GetNavCellCenter(Vector3 min, Vector3 max, NavZone zone)
		  {
				var X=zone.ZoneMin.X+min.X+((max.X-min.X)/2);
				var Y=zone.ZoneMin.Y+min.Y+((max.Y-min.Y)/2);
				var Z=min.Z+((max.Z-min.Z)/2);

				return new Vector3(X, Y, Z);
		  }

		  private bool isDone;
		  /// <summary>
		  /// When true, the next profile tag is used
		  /// </summary>
		  public override bool IsDone
		  {
				get { return !IsActiveQuestStep||isDone; }
		  }



	 }
}
