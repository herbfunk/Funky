using System.Diagnostics;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action=Zeta.TreeSharp.Action;

namespace FunkyTrinity.XMLTags
{
	 // TrinityTownRun forces a town-run request
	 [XmlElement("TrinityTownPortal")]
	 public class TrinityTownPortal : ProfileBehavior
	 {
		  public static int DefaultWaitTime=-1;

		  [XmlAttribute("waitTime")]
		  [XmlAttribute("wait")]
		  public static int WaitTime { get; set; }

		  public static Stopwatch AreaClearTimer=null;
		  public static bool ForceClearArea=false;

		  private double _StartHealth=-1;

		  private bool _IsDone=false;

		  public override bool IsDone
		  {
				get { return _IsDone||!IsActiveQuestStep; }
		  }

		  public TrinityTownPortal()
		  {
				AreaClearTimer=new Stopwatch();
		  }

		  public override void OnStart()
		  {
				if (ZetaDia.Me.IsInTown)
				{
					 _IsDone=true;
					 return;
				}

				Logging.WriteDiagnostic("TrinityTownPortal started - clearing area");
				ForceClearArea=true;
				AreaClearTimer.Start();
				//DefaultWaitTime=V.I("XmlTag.TrinityTownPortal.DefaultWaitTime");
				//int forceWaitTime=V.I("XmlTag.TrinityTownPortal.ForceWaitTime");
				//if (WaitTime<=0&&forceWaitTime==-1)
				//{
				//	 WaitTime=DefaultWaitTime;
				//}
				//else
				//{
				//	 WaitTime=forceWaitTime;
				//}
				_StartHealth=ZetaDia.Me.HitpointsCurrent;
		  }

		  protected override Composite CreateBehavior()
		  {
				return new
				PrioritySelector(
					 new Decorator(ret => ZetaDia.IsLoadingWorld,
						  new Action()
					 ),
					 new Decorator(ret => ZetaDia.Me.IsInTown&&ZetaDia.CurrentLevelAreaId!=55313,
						  new Action(ret =>
						  {
								ForceClearArea=false;
								AreaClearTimer.Reset();
								_IsDone=true;
								//Logger.Log("[TrinityTownPortal] In Town");
						  })
					 ),
					 new Decorator(ret => !ZetaDia.Me.IsInTown&&!ZetaDia.Me.CanUseTownPortal(),
						  new Action(ret =>
						  {
								ForceClearArea=false;
								AreaClearTimer.Reset();
								_IsDone=true;
								//Logger.Log("[TrinityTownPortal] Unable to use TownPortal!");
						  })
					 ),
					 new Decorator(ret => ZetaDia.Me.HitpointsCurrent<_StartHealth,
						  new Action(ret =>
						  {
								_StartHealth=ZetaDia.Me.HitpointsCurrent;
								AreaClearTimer.Restart();
								ForceClearArea=true;
						  })
					 ),
					 new Decorator(ret => AreaClearTimer.IsRunning,
						  new PrioritySelector(
								new Decorator(ret => AreaClearTimer.ElapsedMilliseconds<=WaitTime,
									 new Action(ret => ForceClearArea=true) // returns RunStatus.Success
								),
								new Decorator(ret => AreaClearTimer.ElapsedMilliseconds>WaitTime,
									 new Action(ret =>
									 {
										  Logging.WriteDiagnostic("TownRun timer finished");
										  ForceClearArea=false;
										  AreaClearTimer.Reset();
									 })
								)
						  )
					 ),
					 new Decorator(ret => !ForceClearArea,
						  new PrioritySelector(
								new Decorator(ret => ZetaDia.Me.Movement.IsMoving,
									 new Sequence(
										  Zeta.CommonBot.CommonBehaviors.MoveStop(),
										  new Sleep(1000)
									 )
								),
								new Sequence(
					 // Already casting, just wait
									 new DecoratorContinue(ret => ZetaDia.Me.LoopingAnimationEndTime>0,
										  new Action()
									 ),
									 new Action(ret =>
									 {
										  GameEvents.FireWorldTransferStart();
										  ZetaDia.Me.UseTownPortal();
									 })
								)
						  )
					 )
				);
		  }

		  public override void ResetCachedDone()
		  {
				_IsDone=false;
				base.ResetCachedDone();
		  }
	 }
}