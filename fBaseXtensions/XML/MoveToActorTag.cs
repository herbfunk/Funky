using System.Linq;
using System.Runtime.InteropServices;
using fBaseXtensions.Behaviors;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using fBaseXtensions.Settings;
using Zeta.Bot.Navigation;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace fBaseXtensions.XML
{
	[ComVisible(false)]
	[XmlElement("FunkyMoveToActor")]
	public class FunkyMoveToActorTag : ProfileBehavior
	{

		[XmlAttribute("pathPrecision")]
		public float PathPrecision { get; set; }

		[XmlAttribute("sno", true)]
		public int Sno { get; set; }

		protected override Composite CreateBehavior()
		{
			return new PrioritySelector
			(
				new Decorator(ret => FunkyGame.GameIsInvalid,
					new Action(ret => m_IsDone = true)),


				//Setup our Vector and SNO
				new Decorator(ret => !Initalized,
					new Action(ret => Init())),

				new Decorator(ret => !UpdateObject() || MovementVector == Vector3.Zero || ZetaDia.Me.Position.Distance(MovementVector) <= PathPrecision,
					new Action(ret => m_IsDone = true)),

				new Decorator(ret => lastMoveResult== MoveResult.ReachedDestination || lastMoveResult == MoveResult.Failed,
					new Action(ret => m_IsDone=true)),

				//Movement
				new Decorator(ret => ZetaDia.Me.Position.Distance(MovementVector) > PathPrecision,
					new Action(ret => Movement(MovementVector)))
			);
		}
		private bool Initalized = false;
		private void Init()
		{
			if (UpdateObject())
			{
				MovementVector = Object.Position;
			}
			Initalized = true;
		}
		private bool UpdateObject()
		{
			if (ObjectCache.ShouldUpdateObjectCollection)
				ObjectCache.UpdateCacheObjectCollection();

			Object = ObjectCache.Objects.Values.FirstOrDefault(o => o.SNOID == Sno);

			if (Object != null)
				MovementVector = Object.Position;

			return Object != null;
		}

		private MoveResult lastMoveResult=MoveResult.Moved;
		private bool Movement(Vector3 pos)
		{
			lastMoveResult=Navigator.MoveTo(pos);
			return true;
		}

		private Vector3 MovementVector = Vector3.Zero;
		private CacheObject Object = null;

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}
		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}