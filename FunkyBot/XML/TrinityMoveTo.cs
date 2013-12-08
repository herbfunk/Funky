using System;
using System.Runtime.InteropServices;
using FunkyBot.Movement;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot.Profile;
using Zeta.Navigation;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityMoveTo")]
	public class TrinityMoveTo : ProfileBehavior
	{
		private bool m_IsDone;
		private string useNavigator;
		private Vector3? vMainVector;
		private bool skippingAhead;

		protected override Composite CreateBehavior()
		{
			//Funky.hashSkipAheadAreaCache=new HashSet<Funky.SkipAheadNavigation>();
			var children = new Composite[2];
			var compositeArray = new Composite[2];
			compositeArray[0] = new Action(new ActionSucceedDelegate(FlagTagAsCompleted));
			children[0] = new Decorator(CheckDistanceWithinPathPrecision, new Sequence(compositeArray));
			ActionDelegate actionDelegateMove = GilesMoveToLocation;
			var sequenceblank = new Sequence(
				new Action(actionDelegateMove)
				);
			children[1] = sequenceblank;
			return new PrioritySelector(children);
		}

		private RunStatus GilesMoveToLocation(object ret)
		{

			// First check if we can skip ahead because we recently moved here
			if (NoSkip == null || NoSkip.ToLower() != "true")
			{
				if (SkipAheadCache.CheckPositionForSkipping(Position))
				{
					Logging.WriteDiagnostic("Finished Path {0} earlier due to SkipAreaCache find!", Position.ToString());
					skippingAhead = true;
				}

				if (skippingAhead)
				{
					m_IsDone = true;
					return RunStatus.Success;
				}
			}

			// Now use Trinity movement to try a direct movement towards that location
			var NavTarget = Position;
			var MyPos = Bot.Character.Data.Position;
			if (!ZetaDia.WorldInfo.IsGenerated && Vector3.Distance(MyPos, NavTarget) > 250)
			{
				NavTarget = MathEx.CalculatePointFrom(MyPos, NavTarget, Vector3.Distance(MyPos, NavTarget) - 250);
			}

			if (useNavigator != null && useNavigator.ToLower() == "false")
			{
				Navigator.PlayerMover.MoveTowards(NavTarget);
			}
			else
			{
				//Special cache for skipping locations visited.
				if (Bot.Settings.Debug.SkipAhead)
					SkipAheadCache.RecordSkipAheadCachePoint(PathPrecision);

				Navigator.MoveTo(NavTarget);
			}

			return RunStatus.Success;
		}

		private bool CheckDistanceWithinPathPrecision(object object_0)
		{

			// First see if we should skip ahead one move because we were already at that location
			if (skippingAhead)
			{
				skippingAhead = false;
				return true;
			}

			// Ok not skipping, now see if we are already within pathprecision range of that location
			return (ZetaDia.Me.Position.Distance(Position) <= Math.Max(PathPrecision, Navigator.PathPrecision));
		}

		private void FlagTagAsCompleted(object object_0)
		{
			m_IsDone = true;
		}

		public override void ResetCachedDone()
		{
			m_IsDone = false;
			base.ResetCachedDone();
		}

		public override bool IsDone
		{
			get
			{
				if (IsActiveQuestStep)
				{
					return m_IsDone;
				}
				return true;
			}
		}

		[XmlAttribute("navigation")]
		public string UseNavigator
		{
			get
			{
				return useNavigator;
			}
			set
			{
				useNavigator = value;
			}
		}

		[XmlAttribute("noskip")]
		public string NoSkip { get; set; }

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("pathPrecision")]
		public float PathPrecision { get; set; }

		public Vector3 Position
		{
			get
			{
				if (!vMainVector.HasValue)
				{
					if (UnsafeRandomDistance == 0f)
					{
						vMainVector = new Vector3(X, Y, Z);
					}
					else
					{
						float degrees = new Random().Next(0, 360);
						vMainVector = MathEx.GetPointAt(new Vector3(X, Y, Z), (float)(new Random().NextDouble() * UnsafeRandomDistance), MathEx.ToRadians(degrees));
					}
				}
				return vMainVector.Value;
			}
		}

		[XmlAttribute("unsafeRandomDistance")]
		public float UnsafeRandomDistance { get; set; }

		[XmlAttribute("x")]
		public float X { get; set; }

		[XmlAttribute("y")]
		public float Y { get; set; }

		[XmlAttribute("z")]
		public float Z { get; set; }
	}
}