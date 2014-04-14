using FunkyBot;
using FunkyBot.Movement;
using Zeta.Bot.Navigation;
using Zeta.Bot.Pathfinding;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;
using Logger = FunkyBot.Logger;

namespace FunkyBot.XMLTags
{
	/// <summary>
	/// This profile tag will move the player a a direction given by the offsets x, y. Examples:
	///       <TrinityOffsetMove questId="101758" stepId="1" offsetX="-1000" offsetY="1000" /> 
	///       <TrinityOffsetMove questId="101758" stepId="1" offsetX="1000" offsetY="-1000" />
	///       <TrinityOffsetMove questId="101758" stepId="1" offsetX="-1000" offsetY="-1000" />
	///       <TrinityOffsetMove questId="101758" stepId="1" offsetX="1000" offsetY="1000" />
	/// </summary>
	[XmlElement("TrinityOffsetMove")]
	public class TrinityOffsetMove : ProfileBehavior
	{
		private bool isDone;
		public override bool IsDone
		{
			get { return !IsActiveQuestStep || isDone; }
		}

		/// <summary>
		/// The distance on the X axis to move
		/// </summary>
		[XmlAttribute("x")]
		[XmlAttribute("offsetX")]
		[XmlAttribute("offsetx")]
		public float OffsetX { get; set; }

		/// <summary>
		/// The distance on the Y axis to move
		/// </summary>
		[XmlAttribute("y")]
		[XmlAttribute("offsetY")]
		[XmlAttribute("offsety")]
		public float OffsetY { get; set; }

		/// <summary>
		/// The distance before we've "reached" the destination
		/// </summary>
		[XmlAttribute("pathPrecision")]
		public float PathPrecision { get; set; }

		public Vector3 Position { get; set; }
		private static MoveResult lastMoveResult = MoveResult.Moved;

		protected override Composite CreateBehavior()
		{
			return
			new PrioritySelector(
				new Decorator(ret => IsFinished(),
					new Sequence(
						new Action(ret => Logger.DBLog.InfoFormat("Finished Offset Move x={0} y={1} position={3}",
							OffsetX, OffsetY, Position.Distance2D(MyPos), Position)),
						new Action(ret => isDone = true)
					)
				),
				new Action(ret => MoveToPostion())
			);
		}

		private bool IsFinished()
		{
			return Position.Distance2D(MyPos) <= PathPrecision || lastMoveResult == MoveResult.ReachedDestination;
		}

		private void MoveToPostion()
		{
			Logger.DBLog.DebugFormat("Moving to offset x={0} y={1} distance={2:0} position={3}",
						OffsetX, OffsetY, Position.Distance2D(MyPos), Position);

			lastMoveResult = Funky.PlayerMover.NavigateTo(Position);

			SkipAheadCache.RecordSkipAheadCachePoint(PathPrecision);

			if (lastMoveResult == MoveResult.PathGenerationFailed)
			{
				Logger.DBLog.InfoFormat("Error moving to offset x={0} y={1} distance={2:0} position={3}",
						   OffsetX, OffsetY, Position.Distance2D(MyPos), Position);
				isDone = true;
			}
		}

		public Vector3 MyPos { get { return Bot.Character.Data.Position; } }
		private ISearchAreaProvider MainGridProvider { get { return Navigation.MGP; } }

		public override void OnStart()
		{
			lastMoveResult = MoveResult.Moved;

			float x = MyPos.X + OffsetX;
			float y = MyPos.Y + OffsetY;

			MainGridProvider.Update();

			Position = new Vector3(x, y, MainGridProvider.GetHeight(new Vector2(x, y)));

			if (PathPrecision == 0)
				PathPrecision = 10f;
			Logger.DBLog.InfoFormat("OffsetMove Initialized offset x={0} y={1} distance={2:0} position={3}",
					   OffsetX, OffsetY, Position.Distance2D(MyPos), Position);

		}
		public override void OnDone()
		{

		}
	}
}