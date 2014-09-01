using System;
using System.Linq;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using Zeta.Bot.Navigation;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.XML
{
	[XmlElement("FunkyMoveToSno")]
    internal class FunkyMoveToSnoTag : ProfileBehavior
    {
		public FunkyMoveToSnoTag() { }

        private const double MaxNavPointAgeMs = 15000;
       // private readonly QTNavigator _qtNavigator = new QTNavigator();
        private bool _isDone;
        private DateTime _lastGeneratedNavPoint = DateTime.MinValue;
        private MoveResult _lastMoveResult = default(MoveResult);
        private Vector3 _navTarget;
        private DateTime _tagStartTime;

        public override bool IsDone
        {
            get { return !IsActiveQuestStep || _isDone; }
        }

        [XmlAttribute("pathPrecision")]
        public int PathPrecision { get; set; }

		[XmlAttribute("Sno")]
		public int SNO { get; set; }

        

        /// <summary>
        ///     This will set a time in seconds that this tag is allowed to run for
        /// </summary>
        [XmlAttribute("timeout")]
        public int Timeout { get; set; }

		private CacheObject CurrentObject = null;
		private void UpdateObject()
		{
			if (ObjectCache.ShouldUpdateObjectCollection)
				ObjectCache.UpdateCacheObjectCollection();

			if (CurrentObject==null)
			{
				var objs = ObjectCache.Objects.Values.Where(o => o.SNOID == SNO).ToList();
				if (objs.Count == 0)
				{
					return;
					//if (FunkyGame.Targeting.Cache.Environment.LoSMovementObjects.Count > 0)
					//{
					//	objs = FunkyGame.Targeting.Cache.Environment.LoSMovementObjects.Where(o => o.SNOID == SNO).ToList();
					//	if (objs.Count == 0) return;
					//}
				}

				CurrentObject = objs.FirstOrDefault();
			}
		}
        /// <summary>
        ///     Main SafeMoveTo behavior
        /// </summary>
        /// <returns></returns>
        protected override Composite CreateBehavior()
        {
            return new Sequence(
                new DecoratorContinue(ret =>FunkyGame.GameIsInvalid,
                    new Action(ret => RunStatus.Failure)
                    ),

				new Action(ret => UpdateObject()),

				new Decorator(ret => ZetaDia.Me.IsDead,
					new Action(ret => RunStatus.Failure)),

				new Decorator(ret => CurrentObject==null,
					new Action(ret => _isDone=true)),

                //new Action(ret => GameUI.SafeClickUIButtons()),
                new PrioritySelector(
                    new Decorator(ctx => _tagStartTime != DateTime.UtcNow && DateTime.UtcNow.Subtract(_tagStartTime).TotalSeconds > Timeout,
                        new Sequence(
                            new Action(ret => Logger.DBLog.DebugFormat("Timeout of {0} seconds exceeded for Profile Behavior (start: {1} now: {2}) {3}", Timeout, _tagStartTime.ToLocalTime(), DateTime.Now, Status())),
                            new Action(ret => _isDone = true)
                            )
                        ),
                    new Switch<MoveResult>(ret => Move(),
                        new SwitchArgument<MoveResult>(MoveResult.ReachedDestination,
                            new Sequence(
								new Action(ret => Logger.DBLog.DebugFormat("ReachedDestination! {0}", Status())),
                                new Action(ret => _isDone = true)
                                )
                            ),
                        new SwitchArgument<MoveResult>(MoveResult.PathGenerationFailed,
                            new Sequence(
								new Action(ret => Logger.DBLog.DebugFormat("Move Failed: {0}! {1}", ret, Status())),
                                new Action(ret => _isDone = true)
                                )
                            )
                        )
                    )
                );
        }

        private MoveResult Move()
        {
			if (CurrentObject.Position.Distance2D(ZetaDia.Me.Position) <= PathPrecision)
                return MoveResult.ReachedDestination;

			_navTarget = CurrentObject.Position;

            MoveResult moveResult;
             // Use the Navigator or PathFinder
			moveResult = Navigation.Navigation.NP.MoveTo(_navTarget, Status());
            
            LogStatus();

            return moveResult;
        }

        public override void OnStart()
        {
            if (PathPrecision == 0)
                PathPrecision = 15;
            if (Timeout == 0)
                Timeout = 180;

            _lastGeneratedNavPoint = DateTime.MinValue;
            _lastMoveResult = MoveResult.Moved;
            _tagStartTime = DateTime.UtcNow;
			UpdateObject();
            //PositionCache.Cache.Clear();

            Navigator.Clear();
            Logger.DBLog.DebugFormat("Initialized {0}", Status());
        }

        private void LogStatus()
        {
            if (true)
            {
				double distance = Math.Round(CurrentObject.Position.Distance2D(ZetaDia.Me.Position) / 10.0, 0) * 10;

                Logger.DBLog.DebugFormat("Distance to target: {0:0} {1}", distance, Status());
            }
        }

        /// <summary>
        ///     Returns a friendly string of variables for logging purposes
        /// </summary>
        /// <returns></returns>
        private String Status()
        {
            //if (QuestToolsSettings.Instance.DebugEnabled)
                return String.Format("SNO=\"{0}\" Position=\"{1}\" pathPrecision={2} MoveResult={3} statusText={4}",
					SNO, CurrentObject!=null?CurrentObject.Position.ToString():"NULL", PathPrecision, _lastMoveResult, StatusText);

            return string.Empty;
        }

        public override void ResetCachedDone()
        {
            _isDone = false;
            _lastGeneratedNavPoint = DateTime.MinValue;
            _lastMoveResult = MoveResult.Moved;
            _tagStartTime = DateTime.MinValue;
			CurrentObject = null;
        }
    }
}