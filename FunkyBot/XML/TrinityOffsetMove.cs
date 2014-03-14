using System.Runtime.InteropServices;
using FunkyBot.Movement;
using Zeta.Bot.Navigation;
using Zeta.Bot.Pathfinding;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	/// <summary>
	/// This profile tag will move the player a a direction given by the offsets x, y. Examples:
	///       <TrinityOffsetMove questId="101758" stepId="1" offsetX="-1000" offsetY="1000" />
	///       <TrinityOffsetMove questId="101758" stepId="1" offsetX="1000" offsetY="-1000" />
	///       <TrinityOffsetMove questId="101758" stepId="1" offsetX="-1000" offsetY="-1000" />
	///       <TrinityOffsetMove questId="101758" stepId="1" offsetX="1000" offsetY="1000" />
	/// </summary>
	[ComVisible(false)]
	[XmlElement("TrinityOffsetMove")]
	public class TrinityOffsetMove : ProfileBehavior
	{
		private bool isDone;
		public override bool IsDone
		{
			get { return !IsActiveQuestStep||isDone; }
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
		private static MoveResult lastMoveResult=MoveResult.Moved;

		protected override Composite CreateBehavior()
		{
			return
				new PrioritySelector(
					new Decorator(ret => Position.Distance2D(MyPos)<=PathPrecision||lastMoveResult==MoveResult.ReachedDestination,
						new Action(ret => isDone=true)

						),
					new Action(ret => MoveToPostion())
					);
		}

		private void MoveToPostion()
		{
			lastMoveResult=Funky.PlayerMover.NavigateTo(Position);

			if (lastMoveResult==MoveResult.PathGenerationFailed)
			{
				isDone=true;
			}
		}


		public Vector3 MyPos { get { return ZetaDia.Me.Position; } }
		private ISearchAreaProvider gp { get { return Navigation.MGP; } }
		//private PathFinder pf { get { return GilesTrinity.pf; } }

		public override void OnStart()
		{
			var x=MyPos.X+OffsetX;
			var y=MyPos.Y+OffsetY;

			Position=new Vector3(x, y, gp.GetHeight(new Vector2(x, y)));

			if (PathPrecision==0)
				PathPrecision=10f;

		}
		public override void OnDone()
		{

		}
	}
}