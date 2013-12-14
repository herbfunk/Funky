using System.Diagnostics;
using FunkyBot.DBHandlers;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action=Zeta.TreeSharp.Action;

namespace FunkyBot.XMLTags
{
	 // TrinityTownRun forces a town-run request
	 [XmlElement("TrinityTownPortal")]
	 public class TrinityTownPortal : ProfileBehavior
	 {
		  public static int DefaultWaitTime=2000;

		  [XmlAttribute("waitTime")]
		  [XmlAttribute("wait")]
		  public static int WaitTime { get; set; }

		  public static Stopwatch AreaClearTimer=null;
		  public static bool ForceClearArea=false;

		  private double _StartHealth=-1;

		  private bool _IsDone;

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
				if (WaitTime<=0)
				{
					 WaitTime=DefaultWaitTime;
				}
				_StartHealth=Bot.Character.Data.dCurrentHealthPct;
		  }

		  protected override Composite CreateBehavior()
		  {
				return new
				PrioritySelector(
					 new Decorator(ret => ZetaDia.IsLoadingWorld,
						  new Action()
					 ),
					 new Decorator(ret => Bot.Character.Data.bIsInTown&&ZetaDia.CurrentLevelAreaId!=55313,
						  new Action(ret =>
						  {
								ForceClearArea=false;
								AreaClearTimer.Reset();
								_IsDone=true;
								//Logger.Logging.Write("[TrinityTownPortal] In Town");
						  })
					 ),
					 new Decorator(ret => !Bot.Character.Data.bIsInTown && !TownPortalBehavior.CanCastTP(),
						  new Action(ret =>
						  {
								ForceClearArea=false;
								AreaClearTimer.Reset();
								_IsDone=true;
								//Logger.Logging.Write("[TrinityTownPortal] Unable to use TownPortal!");
						  })
					 ),
					 new Decorator(ret => Bot.Character.Data.dCurrentHealthPct<_StartHealth,
						  new Action(ret =>
						  {
								_StartHealth=Bot.Character.Data.dCurrentHealthPct;
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
								new Decorator(ret => Bot.NavigationCache.IsMoving,
									 new Sequence(
										  CommonBehaviors.MoveStop(),
										  new Sleep(1000)
									 )
								),
								new Sequence(
					 // Already casting, just wait
									 new DecoratorContinue(ret => TownPortalBehavior.CastingRecall(),
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