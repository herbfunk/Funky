using System;
using System.Linq;
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
using Zeta.CommonBot.Dungeons;
using Zeta.CommonBot.Logic;
using Zeta.Pathfinding;
using Action=Zeta.TreeSharp.Action;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;


namespace FunkyTrinity
{
	 // **********************************************************************************************
	 // **********************************************************************************************
	 // **********************************************************************************************
	 // **********************************************************************************************
	 // **********************************************************************************************
	 // *****                             Giles Custom XML Codes                                 *****
	 // **********************************************************************************************
	 // **********************************************************************************************
	 // **********************************************************************************************
	 // **********************************************************************************************
	 // **********************************************************************************************

	 // Note to self: No more sandwich :(

	 // **********************************************************************************************
	 // *****                       TrinityTownRun forces a town-run request                     *****
	 // **********************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityTownRun")]
	 public class TrinityTownRunTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.Action(ret =>
				{
					 Zeta.CommonBot.Logic.BrainBehavior.ForceTownrun("Town-run request received, will town-run at next possible moment.");
					 m_IsDone=true;
				});
		  }

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }

	 // **********************************************************************************************
	 // *****             TrinityLoadProfile will load the specified XML profile up              *****
	 // **********************************************************************************************

	 [ComVisible(false)]
	 [XmlElement("TrinityLoadProfile")]
	 public class TrinityLoadProfile : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  private string sFileName;
		  private string sExitString;
		  private string sNoDelay;
		  // private string sStarterProfile;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.Action(ret =>
				{
					 string sThisProfileString=File;
					 // See if there are multiple profile choices, if so split them up and pick a random one
					 if (sThisProfileString.Contains("!"))
					 {
						  string[] sProfileChoices;
						  sProfileChoices=sThisProfileString.Split(new string[] { "!" }, StringSplitOptions.None);
						  Random rndNum=new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
						  int iChooseProfile=(rndNum.Next(sProfileChoices.Count()))-1;
						  sThisProfileString=sProfileChoices[iChooseProfile];
					 }
					 // Now calculate our current path by checking the currently loaded profile
					 string sCurrentProfilePath=Path.GetDirectoryName(Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile);

					 /* Used for disonnect bug when Iding Items
					 if (sStarterProfile != null)
					 {
						  Logging.Write("[GilesTrinity] Starting Profile Set: " + sStarterProfile);
						  GilesTrinity.StarterProfile = sCurrentProfilePath + @"\" + sStarterProfile;
					 }
					 */

					 bool bExitGame=Exit!=null&&Exit.ToLower()=="true";


					 // And prepare a full string of the path, and the new .xml file name
					 string sNextProfile=sCurrentProfilePath+@"\"+sThisProfileString;
					 Logging.Write("[Funky] Loading new profile.");
					 ProfileManager.Load(sNextProfile);
					 // A quick nap-time helps prevent some funny issues
					 if (NoDelay==null||NoDelay.ToLower()!="true")
						  Thread.Sleep(3000);
					 else
						  Thread.Sleep(300);
					 // See if the XML tag requested we exit the game after loading this profile or not
					 if (bExitGame)
					 {
						  Logging.Write("[Funky] Exiting game to continue with next profile.");
						  // Attempt to teleport to town first for a quicker exit
						  int iSafetyLoops=0;
						  while (!ZetaDia.Me.IsInTown)
						  {
								iSafetyLoops++;
								Funky.WaitWhileAnimating(5, true);
								ZetaDia.Me.UsePower(SNOPower.UseStoneOfRecall, ZetaDia.Me.Position, ZetaDia.Me.WorldDynamicId, -1);
								Thread.Sleep(1000);
								Funky.WaitWhileAnimating(1000, true);
								if (iSafetyLoops>5)
									 break;
						  }
						  Thread.Sleep(1000);
						  ZetaDia.Service.Party.LeaveGame();
						  Funky.ResetGame();
						  // Wait for 10 second log out timer if not in town, else wait for 3 seconds instead
						  Thread.Sleep(!ZetaDia.Me.IsInTown?10000:3000);
					 } // Check if we want to restart the game
					 m_IsDone=true;
				});
		  }

		  [XmlAttribute("exit")]
		  public string Exit
		  {
				get
				{
					 return sExitString;
				}
				set
				{
					 sExitString=value;
				}
		  }

		  [XmlAttribute("nodelay")]
		  public string NoDelay
		  {
				get
				{
					 return sNoDelay;
				}
				set
				{
					 sNoDelay=value;
				}
		  }

		  [XmlAttribute("file")]
		  public string File
		  {
				get
				{
					 return sFileName;
				}
				set
				{
					 sFileName=value;
				}
		  }

		  /* Was used due to the frequent disconnect from IDing items..
		  [XmlAttribute("starterprofile")]
		  public string StarterProfile
		  {
				get
				{
					 return sStarterProfile;
				}
				set
				{
					 sStarterProfile = value;
				}
		  }
		  */

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }

	 // **********************************************************************************************
	 // *****    TrinityMaxDeaths tells Trinity to handle deaths and exit game after X deaths    *****
	 // **********************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityMaxDeaths")]
	 public class TrinityMaxDeathsTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  private int iMaxDeaths;
		  private string sReset;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.Action(ret =>
				{
					 if (MaxDeaths!=Funky.Bot.iMaxDeathsAllowed)
						  Logging.Write("[Funky] Max deaths set by profile. Trinity now handling deaths, and will restart the game after "+MaxDeaths.ToString());

					 Funky.Bot.iMaxDeathsAllowed=MaxDeaths;
					 if (Reset!=null&&Reset.ToLower()=="true")
						  Funky.Bot.iDeathsThisRun=0;
					 m_IsDone=true;
				});
		  }


		  [XmlAttribute("reset")]
		  public string Reset
		  {
				get
				{
					 return sReset;
				}
				set
				{
					 sReset=value;
				}
		  }

		  [XmlAttribute("max")]
		  public int MaxDeaths
		  {
				get
				{
					 return iMaxDeaths;
				}
				set
				{
					 iMaxDeaths=value;
				}
		  }

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }

	 // **********************************************************************************************
	 // *****      TrinityInteract attempts a blind object-use of an SNO without movement        *****
	 // **********************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityInteract")]
	 public class TrinityInteractTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  private int iSNOID;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.Action(ret =>
				{
					 float fClosestRange=-1;
					 int iACDGuid=-1;
					 Vector3 vMyLocation=ZetaDia.Me.Position;
					 foreach (DiaObject thisobject in ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).Where<DiaObject>(a => a.ActorSNO==SNOID))
					 {
						  if (fClosestRange==-1||thisobject.Position.Distance(vMyLocation)<=fClosestRange)
						  {
								fClosestRange=thisobject.Position.Distance(vMyLocation);
								iACDGuid=thisobject.ACDGuid;
						  }
					 }

					 if (iACDGuid!=-1)
					 {
						  try
						  {
								ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, iACDGuid);
						  } catch
						  {
								Logging.WriteDiagnostic("[Funky] There was a memory/DB failure trying to follow the TrinityInteract XML tag on SNO "+SNOID.ToString());
						  }
					 }
					 m_IsDone=true;
				});
		  }


		  [XmlAttribute("snoid")]
		  public int SNOID
		  {
				get
				{
					 return iSNOID;
				}
				set
				{
					 iSNOID=value;
				}
		  }

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }

	 // **********************************************************************************************
	 // *****                  Trinity Log lets profiles send log messages to DB                 *****
	 // **********************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityLog")]
	 public class TrinityLogTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  private string sLogOutput;
		  private string sLogLevel;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.Action(ret =>
				{
					 if (Level!=null&&Level.ToLower()=="diagnostic")
						  Logging.WriteDiagnostic(Output);
					 else
						  Logging.Write(Output);
					 m_IsDone=true;
				});
		  }

		  [XmlAttribute("level", true)]
		  public string Level
		  {
				get
				{
					 return sLogLevel;
				}
				set
				{
					 sLogLevel=value;
				}
		  }

		  [XmlAttribute("output", true)]
		  public string Output
		  {
				get
				{
					 return sLogOutput;
				}
				set
				{
					 sLogOutput=value;
				}
		  }

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }


	 // * TrinityMoveTo moves in a straight line without any navigation hits, and allows tag-skips
	 [ComVisible(false)]
	 [XmlElement("TrinityMoveTo")]
	 public class TrinityMoveTo : ProfileBehavior
	 {
		  private bool m_IsDone;
		  private float fPosX;
		  private float fPosY;
		  private float fPosZ;
		  private float fPathPrecision;
		  private float fRandomizedDistance;
		  private string sDestinationName;
		  private string sNoSkip;
		  private string useNavigator;
		  private Vector3? vMainVector;
		  private bool skippingAhead=false;

		  protected override Composite CreateBehavior()
		  {
				Funky.CacheMovementTracking.bSkipAheadAGo=true;
				//Funky.hashSkipAheadAreaCache=new HashSet<Funky.SkipAheadNavigation>();
				Composite[] children=new Composite[2];
				Composite[] compositeArray=new Composite[2];
				compositeArray[0]=new Zeta.TreeSharp.Action(new ActionSucceedDelegate(FlagTagAsCompleted));
				children[0]=new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(CheckDistanceWithinPathPrecision), new Sequence(compositeArray));
				ActionDelegate actionDelegateMove=new ActionDelegate(GilesMoveToLocation);
				Sequence sequenceblank=new Sequence(
					 new Zeta.TreeSharp.Action(actionDelegateMove)
					 );
				children[1]=sequenceblank;
				return new PrioritySelector(children);
		  }

		  private RunStatus GilesMoveToLocation(object ret)
		  {

				// First check if we can skip ahead because we recently moved here
				if (NoSkip==null||NoSkip.ToLower()!="true")
				{
					 if (Funky.CacheMovementTracking.CheckPositionForSkipping(Position))
					 {
						  Logging.WriteDiagnostic("Finished Path {0} earlier due to SkipAreaCache find!", Position.ToString());
						  skippingAhead=true;
					 }

					 if (skippingAhead)
					 {
						  Funky.CacheMovementTracking.bSkipAheadAGo=false;
						  return RunStatus.Success;
					 }
				}

				// Now use Trinity movement to try a direct movement towards that location
				Vector3 NavTarget=Position;
				Vector3 MyPos=Funky.Bot.Character.Position;
				if (!ZetaDia.WorldInfo.IsGenerated&&Vector3.Distance(MyPos, NavTarget)>250)
				{
					 NavTarget=MathEx.CalculatePointFrom(MyPos, NavTarget, Vector3.Distance(MyPos, NavTarget)-250);
				}

				if (useNavigator!=null&&useNavigator.ToLower()=="false")
				{
					 Navigator.PlayerMover.MoveTowards(NavTarget);
				}
				else
				{
					 Navigator.MoveTo(NavTarget);
				}

				return RunStatus.Success;
		  }

		  private bool CheckDistanceWithinPathPrecision(object object_0)
		  {

				// First see if we should skip ahead one move because we were already at that location
				if (skippingAhead)
				{
					 skippingAhead=false;
					 return true;
				}

				// Ok not skipping, now see if we are already within pathprecision range of that location
				return (ZetaDia.Me.Position.Distance(Position)<=Math.Max(PathPrecision, Navigator.PathPrecision));
		  }

		  private void FlagTagAsCompleted(object object_0)
		  {
				m_IsDone=true;
		  }

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
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
					 useNavigator=value;
				}
		  }

		  [XmlAttribute("noskip")]
		  public string NoSkip
		  {
				get
				{
					 return sNoSkip;
				}
				set
				{
					 sNoSkip=value;
				}
		  }

		  [XmlAttribute("name")]
		  public string Name
		  {
				get
				{
					 return sDestinationName;
				}
				set
				{
					 sDestinationName=value;
				}
		  }

		  [XmlAttribute("pathPrecision")]
		  public float PathPrecision
		  {
				get
				{
					 return fPathPrecision;
				}
				set
				{
					 fPathPrecision=value;
				}
		  }

		  public Vector3 Position
		  {
				get
				{
					 if (!vMainVector.HasValue)
					 {
						  if (UnsafeRandomDistance==0f)
						  {
								vMainVector=new Vector3(X, Y, Z);
						  }
						  else
						  {
								float degrees=new Random().Next(0, 360);
								vMainVector=new Vector3?(MathEx.GetPointAt(new Vector3(X, Y, Z), (float)(new Random().NextDouble()*UnsafeRandomDistance), MathEx.ToRadians(degrees)));
						  }
					 }
					 return vMainVector.Value;
				}
		  }

		  [XmlAttribute("unsafeRandomDistance")]
		  public float UnsafeRandomDistance
		  {
				get
				{
					 return fRandomizedDistance;
				}
				set
				{
					 fRandomizedDistance=value;
				}
		  }

		  [XmlAttribute("x")]
		  public float X
		  {
				get
				{
					 return fPosX;
				}
				set
				{
					 fPosX=value;
				}
		  }

		  [XmlAttribute("y")]
		  public float Y
		  {
				get
				{
					 return fPosY;
				}
				set
				{
					 fPosY=value;
				}
		  }

		  [XmlAttribute("z")]
		  public float Z
		  {
				get
				{
					 return fPosZ;
				}
				set
				{
					 fPosZ=value;
				}
		  }


	 }



	 // ****************************************************************************************************
	 // ***** TrinityMoveTo moves in a straight line without any navigation hits, and allows tag-skips *****
	 // ****************************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityMoveToSNO")]
	 public class TrinityMoveToSNOTag : ProfileBehavior
	 {
		  private bool m_IsDone;
		  private float fPathPrecision;
		  private int iSNOID;
		  private string sDestinationName;

		  protected override Composite CreateBehavior()
		  {
				Composite[] children=new Composite[2];
				Composite[] compositeArray=new Composite[2];
				compositeArray[0]=new Zeta.TreeSharp.Action(new ActionSucceedDelegate(FlagTagAsCompleted));
				children[0]=new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(CheckDistanceWithinPathPrecision), new Sequence(compositeArray));
				ActionDelegate actionDelegateMove=new ActionDelegate(GilesMoveToLocation);
				Sequence sequenceblank=new Sequence(
					 new Zeta.TreeSharp.Action(actionDelegateMove)
					 );
				children[1]=sequenceblank;
				return new PrioritySelector(children);
		  }

		  private RunStatus GilesMoveToLocation(object ret)
		  {
				DiaObject tempObject=ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID);
				if (tempObject!=null)
				{
					 Navigator.PlayerMover.MoveTowards(tempObject.Position);
					 return RunStatus.Success;
				}
				return RunStatus.Success;
		  }

		  private bool CheckDistanceWithinPathPrecision(object object_0)
		  {
				DiaObject tempObject=ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID);
				if (tempObject!=null)
				{
					 return (ZetaDia.Me.Position.Distance(tempObject.Position)<=Math.Max(PathPrecision, Navigator.PathPrecision));
				}
				return false;
		  }

		  private void FlagTagAsCompleted(object object_0)
		  {
				m_IsDone=true;
		  }

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
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

		  [XmlAttribute("name")]
		  public string Name
		  {
				get
				{
					 return sDestinationName;
				}
				set
				{
					 sDestinationName=value;
				}
		  }

		  [XmlAttribute("pathPrecision")]
		  public float PathPrecision
		  {
				get
				{
					 return fPathPrecision;
				}
				set
				{
					 fPathPrecision=value;
				}
		  }

		  [XmlAttribute("snoid")]
		  public int SNOID
		  {
				get
				{
					 return iSNOID;
				}
				set
				{
					 iSNOID=value;
				}
		  }
	 }
	 // Note to self: Could manually start a new game with;
	 //ZetaDia.Service.Games.CreateGame(createGameParams.Act, createGameParams.Difficulty, createGameParams.Quest, createGameParams.Step, createGameParams.ResumeFromSave, createGameParams.IsPrivate);

	 // **********************************************************************************************
	 // *****         Trinity If perfectly mimics DB If - this is just for experimenting         *****
	 // **********************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityIf")]
	 public class TrinityIfTag : ComplexNodeTag, IPythonExecutable
	 {
		  private bool? bComplexDoneCheck;
		  private bool? bAlreadyCompleted;
		  private Func<bool> funcConditionalProcess;
		  private static Func<ProfileBehavior, bool> funcBehaviorProcess;
		  private string sConditionString;

		  public void CompilePython()
		  {

				ScriptManager.GetCondition(Condition);
		  }

		  protected override Composite CreateBehavior()
		  {
				PrioritySelector decorated=new PrioritySelector(new Composite[0]);
				foreach (ProfileBehavior behavior in base.GetNodes())
				{
					 decorated.AddChild(behavior.Behavior);
				}
				return new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(CheckNotAlreadyDone), decorated);
		  }

		  public bool GetConditionExec()
		  {
				bool flag;
				try
				{
					 if (Conditional==null)
					 {
						  Conditional=ScriptManager.GetCondition(Condition);
					 }
					 flag=Conditional();
				} catch (Exception exception)
				{
					 Logging.WriteDiagnostic(ScriptManager.FormatSyntaxErrorException(exception));
					 BotMain.Stop(false, "");
					 throw;
				}
				return flag;
		  }

		  private bool CheckNotAlreadyDone(object object_0)
		  {
				return !IsDone;
		  }

		  public override void ResetCachedDone()
		  {
				foreach (ProfileBehavior behavior in Body)
				{
					 behavior.ResetCachedDone();
				}
				bComplexDoneCheck=null;
		  }

		  private static bool CheckBehaviorIsDone(ProfileBehavior profileBehavior)
		  {
				return profileBehavior.IsDone;
		  }

		  [XmlAttribute("condition", true)]
		  public string Condition
		  {
				get
				{
					 return sConditionString;
				}
				set
				{
					 sConditionString=value;
				}
		  }

		  public Func<bool> Conditional
		  {
				get
				{
					 return funcConditionalProcess;
				}
				set
				{
					 funcConditionalProcess=value;
				}
		  }

		  public override bool IsDone
		  {
				get
				{
					 // Make sure we've not already completed this if
					 if (bAlreadyCompleted.HasValue&&bAlreadyCompleted==true)
					 {
						  return true;
					 }
					 // First check the actual "if" conditions if we haven't already
					 if (!bComplexDoneCheck.HasValue)
					 {
						  bComplexDoneCheck=new bool?(GetConditionExec());
					 }
					 if (bComplexDoneCheck==false)
					 {
						  return true;
					 }
					 // Ok we've already checked that and it was false FIRST check, so now go purely on behavior-done flag
					 if (funcBehaviorProcess==null)
					 {
						  funcBehaviorProcess=new Func<ProfileBehavior, bool>(CheckBehaviorIsDone);
					 }
					 bool bAllChildrenDone=Body.All<ProfileBehavior>(funcBehaviorProcess);
					 if (bAllChildrenDone)
					 {
						  bAlreadyCompleted=true;
					 }
					 return bAllChildrenDone;
				}
		  }
	 }

	 // **********************************************************************************************
	 // *****     TrinityIfWithinRange checks an SNO is in range and processes child nodes       *****
	 // **********************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityIfSNOInRange")]
	 public class IfSNOInRangeTag : ComplexNodeTag
	 {
		  private bool? bComplexDoneCheck;
		  private bool? bAlreadyCompleted;
		  private Func<bool> funcConditionalProcess;
		  private static Func<ProfileBehavior, bool> funcBehaviorProcess;
		  private int iSNOID;
		  private float fRadius;
		  private string sType;

		  protected override Composite CreateBehavior()
		  {
				PrioritySelector decorated=new PrioritySelector(new Composite[0]);
				foreach (ProfileBehavior behavior in base.GetNodes())
				{
					 decorated.AddChild(behavior.Behavior);
				}
				return new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(CheckNotAlreadyDone), decorated);
		  }

		  public bool GetConditionExec()
		  {
				bool flag;
				Vector3 vMyLocation=ZetaDia.Me.Position;
				if (sType!=null&&sType=="reverse")
					 flag=ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID&&a.Position.Distance(vMyLocation)<=Range)==null;
				else
					 flag=(ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault<DiaObject>(a => a.ActorSNO==SNOID&&a.Position.Distance(vMyLocation)<=Range)!=null);
				return flag;
		  }

		  private bool CheckNotAlreadyDone(object object_0)
		  {
				return !IsDone;
		  }

		  public override void ResetCachedDone()
		  {
				foreach (ProfileBehavior behavior in Body)
				{
					 behavior.ResetCachedDone();
				}
				bComplexDoneCheck=null;
		  }

		  private static bool CheckBehaviorIsDone(ProfileBehavior profileBehavior)
		  {
				return profileBehavior.IsDone;
		  }

		  [XmlAttribute("snoid")]
		  public int SNOID
		  {
				get
				{
					 return iSNOID;
				}
				set
				{
					 iSNOID=value;
				}
		  }

		  [XmlAttribute("range")]
		  public float Range
		  {
				get
				{
					 return fRadius;
				}
				set
				{
					 fRadius=value;
				}
		  }

		  [XmlAttribute("type")]
		  public string Type
		  {
				get
				{
					 return sType;
				}
				set
				{
					 sType=value;
				}
		  }

		  public Func<bool> Conditional
		  {
				get
				{
					 return funcConditionalProcess;
				}
				set
				{
					 funcConditionalProcess=value;
				}
		  }

		  public override bool IsDone
		  {
				get
				{
					 // Make sure we've not already completed this tag
					 if (bAlreadyCompleted.HasValue&&bAlreadyCompleted==true)
					 {
						  return true;
					 }
					 if (!bComplexDoneCheck.HasValue)
					 {
						  bComplexDoneCheck=new bool?(GetConditionExec());
					 }
					 if (bComplexDoneCheck==false)
					 {
						  return true;
					 }
					 if (funcBehaviorProcess==null)
					 {
						  funcBehaviorProcess=new Func<ProfileBehavior, bool>(CheckBehaviorIsDone);
					 }
					 bool bAllChildrenDone=Body.All<ProfileBehavior>(funcBehaviorProcess);
					 if (bAllChildrenDone)
					 {
						  bAlreadyCompleted=true;
					 }
					 return bAllChildrenDone;
				}
		  }
	 }


	 // **********************************************************************************************
	 // *****     TrinityRandom assigns a random value between min and max to a specified id     *****
	 // **********************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityRandomRoll")]
	 public class TrinityRandomRollTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  private int iID;
		  private int iMin;
		  private int iMax;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.Action(ret =>
				{
					 // Generate a random value between the selected min-max range, and assign it to our dictionary of random values
					 int iOldValue;
					 Random rndNum=new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
					 int iNewRandomValue=(rndNum.Next((Max-Min)+1))+Min;
					 Logging.Write("[Funky] Generating RNG for profile between "+Min.ToString()+" and "+Max.ToString()+", result="+iNewRandomValue.ToString());
					 if (!Funky.dictRandomID.TryGetValue(ID, out iOldValue))
					 {
						  Funky.dictRandomID.Add(ID, iNewRandomValue);
					 }
					 else
					 {
						  Funky.dictRandomID[ID]=iNewRandomValue;
					 }
					 m_IsDone=true;
				});
		  }


		  [XmlAttribute("id")]
		  public int ID
		  {
				get
				{
					 return iID;
				}
				set
				{
					 iID=value;
				}
		  }
		  [XmlAttribute("min")]
		  public int Min
		  {
				get
				{
					 return iMin;
				}
				set
				{
					 iMin=value;
				}
		  }
		  [XmlAttribute("max")]
		  public int Max
		  {
				get
				{
					 return iMax;
				}
				set
				{
					 iMax=value;
				}
		  }

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }

	 // **********************************************************************************************
	 // *****  TrinityIfRandom only runs the container stuff if the given id is the given value  *****
	 // **********************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityIfRandom")]
	 public class TrinityIfRandomTag : ComplexNodeTag
	 {
		  private bool? bComplexDoneCheck;
		  private bool? bAlreadyCompleted;
		  private Func<bool> funcConditionalProcess;
		  private static Func<ProfileBehavior, bool> funcBehaviorProcess;
		  private int iID;
		  private int iResult;

		  protected override Composite CreateBehavior()
		  {
				PrioritySelector decorated=new PrioritySelector(new Composite[0]);
				foreach (ProfileBehavior behavior in base.GetNodes())
				{
					 decorated.AddChild(behavior.Behavior);
				}
				return new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(CheckNotAlreadyDone), decorated);
		  }

		  public bool GetConditionExec()
		  {
				int iOldValue;
				// If the dictionary value doesn't even exist, FAIL!
				if (!Funky.dictRandomID.TryGetValue(ID, out iOldValue)&&Result!=-1)
					 return false;
				// Ok, do the results match up what we want? then SUCCESS!
				if (iOldValue==Result||Result==-1)
					 return true;
				// No? Fail!
				return false;
		  }

		  private bool CheckNotAlreadyDone(object object_0)
		  {
				return !IsDone;
		  }

		  public override void ResetCachedDone()
		  {
				foreach (ProfileBehavior behavior in Body)
				{
					 behavior.ResetCachedDone();
				}
				bComplexDoneCheck=null;
		  }

		  private static bool CheckBehaviorIsDone(ProfileBehavior profileBehavior)
		  {
				return profileBehavior.IsDone;
		  }

		  [XmlAttribute("id")]
		  public int ID
		  {
				get
				{
					 return iID;
				}
				set
				{
					 iID=value;
				}
		  }

		  [XmlAttribute("result")]
		  public int Result
		  {
				get
				{
					 return iResult;
				}
				set
				{
					 iResult=value;
				}
		  }

		  public Func<bool> Conditional
		  {
				get
				{
					 return funcConditionalProcess;
				}
				set
				{
					 funcConditionalProcess=value;
				}
		  }

		  public override bool IsDone
		  {
				get
				{
					 // Make sure we've not already completed this tag
					 if (bAlreadyCompleted.HasValue&&bAlreadyCompleted==true)
					 {
						  return true;
					 }
					 if (!bComplexDoneCheck.HasValue)
					 {
						  bComplexDoneCheck=new bool?(GetConditionExec());
					 }
					 if (bComplexDoneCheck==false)
					 {
						  return true;
					 }
					 if (funcBehaviorProcess==null)
					 {
						  funcBehaviorProcess=new Func<ProfileBehavior, bool>(CheckBehaviorIsDone);
					 }
					 bool bAllChildrenDone=Body.All<ProfileBehavior>(funcBehaviorProcess);
					 if (bAllChildrenDone)
					 {
						  bAlreadyCompleted=true;
					 }
					 return bAllChildrenDone;
				}
		  }
	 }

	 // ************************************************************************************************
	 // ***** TrinityUseReset - Resets a UseOnce tag as if it has never been used                  *****
	 // ************************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityUseReset")]
	 public class TrinityUseResetTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  private int iID;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.Action(ret =>
				{
					 // See if we've EVER hit this ID before
					 // If so, delete it, if not, do nothing
					 if (Funky.hashUseOnceID.Contains(ID))
					 {
						  Funky.hashUseOnceID.Remove(ID);
						  Funky.dictUseOnceID.Remove(ID);
					 }
					 m_IsDone=true;
				});
		  }


		  [XmlAttribute("id")]
		  public int ID
		  {
				get
				{
					 return iID;
				}
				set
				{
					 iID=value;
				}
		  }

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }

	 // ************************************************************************************************
	 // ***** TrinityUseStop - prevents a useonce tag ID ever being used again                     *****
	 // ************************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityUseStop")]
	 public class TrinityUseStopTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  private int iID;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.Action(ret =>
				{
					 // See if we've EVER hit this ID before
					 // If so, set it disabled - if not, add it and prevent it
					 if (Funky.hashUseOnceID.Contains(ID))
					 {
						  Funky.dictUseOnceID[ID]=-1;
					 }
					 else
					 {
						  Funky.hashUseOnceID.Add(ID);
						  Funky.dictUseOnceID.Add(ID, -1);
					 }
					 m_IsDone=true;
				});
		  }


		  [XmlAttribute("id")]
		  public int ID
		  {
				get
				{
					 return iID;
				}
				set
				{
					 iID=value;
				}
		  }

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }

	 // ************************************************************************************************
	 // ***** TrinityUseOnce ensures a sequence of tags is only ever used once during this profile *****
	 // ************************************************************************************************
	 [ComVisible(false)]
	 [XmlElement("TrinityUseOnce")]
	 public class TrinityUseOnceTag : ComplexNodeTag
	 {
		  private bool? bComplexDoneCheck;
		  private bool? bAlreadyCompleted;
		  private Func<bool> funcConditionalProcess;
		  private static Func<ProfileBehavior, bool> funcBehaviorProcess;
		  private int iUniqueID;
		  private int iMaxRedo;
		  private string sDisablePrevious;

		  protected override Composite CreateBehavior()
		  {
				PrioritySelector decorated=new PrioritySelector(new Composite[0]);
				foreach (ProfileBehavior behavior in base.GetNodes())
				{
					 decorated.AddChild(behavior.Behavior);
				}
				return new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(CheckNotAlreadyDone), decorated);
		  }

		  public bool GetConditionExec()
		  {
				// See if we've EVER hit this ID before
				if (Funky.hashUseOnceID.Contains(ID))
				{
					 // See if we've hit it more than or equal to the max times before
					 if (Funky.dictUseOnceID[ID]>=Max||Funky.dictUseOnceID[ID]<0)
						  return false;
					 // Add 1 to our hit count, and let it run this time
					 Funky.dictUseOnceID[ID]++;
					 return true;
				}
				// Never hit this before, so create the entry and let it run
				// First see if we should disable all other ID's currently hit to prevent them ever being run again this run
				if (DisablePrevious!=null&&DisablePrevious.ToLower()=="true")
				{
					 foreach (int thisid in Funky.hashUseOnceID)
					 {
						  if (thisid!=ID)
						  {
								Funky.dictUseOnceID[thisid]=-1;
						  }
					 }
				}
				// Now store the fact we have hit this ID and set up the dictionary entry for it
				Funky.hashUseOnceID.Add(ID);
				Funky.dictUseOnceID.Add(ID, 1);
				return true;
		  }

		  private bool CheckNotAlreadyDone(object object_0)
		  {
				return !IsDone;
		  }

		  public override void ResetCachedDone()
		  {
				foreach (ProfileBehavior behavior in Body)
				{
					 behavior.ResetCachedDone();
				}
				bComplexDoneCheck=null;
		  }

		  private static bool CheckBehaviorIsDone(ProfileBehavior profileBehavior)
		  {
				return profileBehavior.IsDone;
		  }

		  [XmlAttribute("id")]
		  public int ID
		  {
				get
				{
					 return iUniqueID;
				}
				set
				{
					 iUniqueID=value;
				}
		  }

		  [XmlAttribute("disableprevious")]
		  public string DisablePrevious
		  {
				get
				{
					 return sDisablePrevious;
				}
				set
				{
					 sDisablePrevious=value;
				}
		  }

		  [XmlAttribute("max")]
		  public int Max
		  {
				get
				{
					 return iMaxRedo;
				}
				set
				{
					 iMaxRedo=value;
				}
		  }

		  public Func<bool> Conditional
		  {
				get
				{
					 return funcConditionalProcess;
				}
				set
				{
					 funcConditionalProcess=value;
				}
		  }

		  public override bool IsDone
		  {
				get
				{
					 // Make sure we've not already completed this tag
					 if (bAlreadyCompleted.HasValue&&bAlreadyCompleted==true)
					 {
						  return true;
					 }
					 if (!bComplexDoneCheck.HasValue)
					 {
						  bComplexDoneCheck=new bool?(GetConditionExec());
					 }
					 if (bComplexDoneCheck==false)
					 {
						  return true;
					 }
					 if (funcBehaviorProcess==null)
					 {
						  funcBehaviorProcess=new Func<ProfileBehavior, bool>(CheckBehaviorIsDone);
					 }
					 bool bAllChildrenDone=Body.All<ProfileBehavior>(funcBehaviorProcess);
					 if (bAllChildrenDone)
					 {
						  bAlreadyCompleted=true;
					 }
					 return bAllChildrenDone;
				}
		  }
	 }
	 [ComVisible(false)]
	 [XmlElement("TrinityResetAll")]
	 public class TrinityResetAllTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.Action(ret =>
				{
					 Funky.ResetGame();

					 m_IsDone=true;
				});
		  }


		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }
	 [ComVisible(false)]
	 [XmlElement("FunkyCastTP")]
	 public class FunkyCastTPTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				Composite[] children=new Composite[2];

				ActionDelegate actionDelegateCast=new ActionDelegate(Funky.FunkyTPBehavior);
				Sequence sequenceblank=new Sequence(
					 new Zeta.TreeSharp.Action(actionDelegateCast)
					 );

				children[0]=new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(Funky.FunkyTPOverlord), sequenceblank);
				children[1]=new Zeta.TreeSharp.Action(new ActionSucceedDelegate(FlagTagAsCompleted));

				return new PrioritySelector(children);
		  }

		  private void FlagTagAsCompleted(object object_0)
		  {
				m_IsDone=true;
		  }

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }

	 }

	 /// <summary>
	 /// TrinityExploreDungeon is fuly backwards compatible with the built-in Demonbuddy ExploreArea tag. It provides additional features such as:
	 /// Moving to investigate MiniMapMarker pings and the current ExitNameHash if provided and visible (mini map marker 0 and the current exitNameHash)
	 /// Moving to investigate Priority Scenes if provided (PrioritizeScenes subtags)
	 /// Ignoring DungeonExplorer nodes in certain scenes if provided (IgnoreScenes subtags)
	 /// Reduced backtracking (via pathPrecision attribute and combat skip ahead cache)
	 /// Multiple ActorId's for the ObjectFound end type (AlternateActors sub-tags)
	 /// </summary>
	 [ComVisible(false)]
	 [XmlElement("TrinityExploreDungeon")]
	 public class TrinityExploreDungeon : ProfileBehavior
	 {
		  /// <summary>
		  /// The SNOId of the Actor that we're looking for, used with until="ObjectFound"
		  /// </summary>
		  [XmlAttribute("actorId", true)]
		  public int ActorId { get; set; }

		  /// <summary>
		  /// Sets a custom grid segmentation Box Size (default 15)
		  /// </summary>
		  [XmlAttribute("boxSize", true)]
		  public int BoxSize { get; set; }

		  /// <summary>
		  /// Sets a custom grid segmentation Box Tolerance (default 0.55)
		  /// </summary>
		  [XmlAttribute("boxTolerance", true)]
		  public float BoxTolerance { get; set; }

		  /// <summary>
		  /// The nameHash of the exit the bot will move to and finish the tag when found
		  /// </summary>
		  [XmlAttribute("exitNameHash", true)]
		  public int ExitNameHash { get; set; }

		  [XmlAttribute("ignoreGridReset", true)]
		  public bool IgnoreGridReset { get; set; }

		  /// <summary>
		  /// Not currently implimented
		  /// </summary>
		  [XmlAttribute("leaveWhenFinished", true)]
		  public bool LeaveWhenExplored { get; set; }

		  /// <summary>
		  /// The distance the bot must be from an actor before marking the tag as complete, when used with until="ObjectFound"
		  /// </summary>
		  [XmlAttribute("objectDistance", true)]
		  public float ObjectDistance { get; set; }

		  /// <summary>
		  /// The until="" atribute must match one of these
		  /// </summary>
		  public enum TrinityExploreEndType
		  {
				FullyExplored=0,
				ObjectFound,
				ExitFound,
				SceneFound
		  }

		  [XmlAttribute("endType", true)]
		  [XmlAttribute("until", true)]
		  public TrinityExploreEndType EndType { get; set; }

		  /// <summary>
		  /// The list of Scene SNOId's or Scene Names that the bot will ignore dungeon nodes in
		  /// </summary>
		  [XmlElement("IgnoreScenes")]
		  public List<IgnoreScene> IgnoreScenes { get; set; }

		  /// <summary>
		  /// The list of Scene SNOId's or Scene Names that the bot will prioritize (only works when the scene is "loaded")
		  /// </summary>
		  [XmlElement("PriorityScenes")]
		  [XmlElement("PrioritizeScenes")]
		  public List<PrioritizeScene> PriorityScenes { get; set; }

		  /// <summary>
		  /// The Ignore Scene class, used as IgnoreScenes child elements
		  /// </summary>
		  [XmlElement("IgnoreScene")]
		  public class IgnoreScene
		  {
				[XmlAttribute("sceneName")]
				public string SceneName { get; set; }
				[XmlAttribute("sceneId")]
				public int SceneId { get; set; }

				public IgnoreScene()
				{
					 SceneId=-1;
					 SceneName=String.Empty;
				}

				public IgnoreScene(string name)
				{
					 this.SceneName=name;
				}
				public IgnoreScene(int id)
				{
					 this.SceneId=id;
				}
		  }

		  /// <summary>
		  /// The Priority Scene class, used as PrioritizeScenes child elements
		  /// </summary>
		  [XmlElement("PrioritizeScene")]
		  public class PrioritizeScene
		  {
				[XmlAttribute("sceneName")]
				public string SceneName { get; set; }
				[XmlAttribute("sceneId")]
				public int SceneId { get; set; }
				[XmlAttribute("pathPrecision")]
				public float PathPrecision { get; set; }

				public PrioritizeScene()
				{
					 PathPrecision=15f;
					 SceneName=String.Empty;
					 SceneId=-1;
				}

				public PrioritizeScene(string name)
				{
					 this.SceneName=name;
				}
				public PrioritizeScene(int id)
				{
					 this.SceneId=id;
				}
		  }

		  [XmlElement("AlternateActors")]
		  public List<AlternateActor> AlternateActors { get; set; }

		  [XmlElement("AlternateActor")]
		  public class AlternateActor
		  {
				[XmlAttribute("actorId")]
				public int ActorId { get; set; }
				[XmlAttribute("objectDistance")]
				public float ObjectDistance { get; set; }

				public AlternateActor()
				{
					 ActorId=-1;
					 ObjectDistance=60f;
				}
		  }

		  /// <summary>
		  /// The Scene SNOId, used with ExploreUntil="SceneFound"
		  /// </summary>
		  [XmlAttribute("sceneId")]
		  public int SceneId { get; set; }

		  /// <summary>
		  /// The Scene Name, used with ExploreUntil="SceneFound", a sub-string match will work
		  /// </summary>
		  [XmlAttribute("sceneName")]
		  public string SceneName { get; set; }

		  /// <summary>
		  /// The distance the bot will mark dungeon nodes as "visited" (default is 1/2 of box size, minimum 10)
		  /// </summary>
		  [XmlAttribute("pathPrecision")]
		  public float PathPrecision { get; set; }

		  /// <summary>
		  /// The distance before reaching a MiniMapMarker before marking it as visited
		  /// </summary>
		  [XmlAttribute("markerDistance")]
		  public float MarkerDistance { get; set; }

		  /// <summary>
		  /// Disable Mini Map Marker Scouting
		  /// </summary>
		  [XmlAttribute("ignoreMarkers")]
		  public bool IgnoreMarkers { get; set; }

		  public enum TimeoutType
		  {
				Timer,
				GoldInactivity,
				None,
		  }

		  /// <summary>
		  /// The TimeoutType to use (default None, no timeout)
		  /// </summary>
		  [XmlAttribute("timeoutType")]
		  public TimeoutType ExploreTimeoutType { get; set; }

		  /// <summary>
		  /// Value in Seconds. 
		  /// The timeout value to use, when used with Timer will force-end the tag after a certain time. When used with GoldInactivity will end the tag after coinages doesn't change for the given period
		  /// </summary>
		  [XmlAttribute("timeoutValue")]
		  public int TimeoutValue { get; set; }

		  /// <summary>
		  /// If we want to use a townportal before ending the tag when a timeout happens
		  /// </summary>
		  [XmlAttribute("townPortalOnTimeout")]
		  public bool TownPortalOnTimeout { get; set; }

		  /// <summary>
		  /// The Position of the CurrentNode NavigableCenter
		  /// </summary>
		  private Vector3 CurrentNavTarget
		  {
				get
				{
					 if (GetRouteUnvisitedNodeCount()>0)
					 {
						  return BrainBehavior.DungeonExplorer.CurrentNode.NavigableCenter;
					 }
					 else
					 {
						  return Vector3.Zero;
					 }
				}
		  }
		  private bool InitDone=false;
		  private DungeonNode NextNode;

		  /// <summary>
		  /// The current player position
		  /// </summary>
		  private Vector3 myPos { get { return ZetaDia.Me.Position; } }
		  private static ISearchAreaProvider gp
		  {
				get
				{
					 return FunkyTrinity.Funky.mgp;
				}
		  }

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
				//DbHelper.Log(TrinityLogLevel.Normal, LogCategory.ProfileTag, "TrinityExploreDungeon OnStart() called");

				UpdateSearchGridProvider();

				CheckResetDungeonExplorer();

				if (!InitDone)
				{
					 Init();
				}
				TagTimer.Reset();
				timesForcedReset=0;

				PrintNodeCounts("PostInit");
		  }

		  /// <summary>
		  /// Re-sets the DungeonExplorer, BoxSize, BoxTolerance, and Updates the current route
		  /// </summary>
		  private void CheckResetDungeonExplorer()
		  {
				// I added this because GridSegmentation may (rarely) reset itself without us doing it to 15/.55.
				if ((BoxSize!=0&&BoxTolerance!=0)&&(GridSegmentation.BoxSize!=BoxSize||GridSegmentation.BoxTolerance!=BoxTolerance)||(GetGridSegmentationNodeCount()==0))
				{
					 //DbHelper.Log(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Box Size or Tolerance has been changed! {0}/{1}", GridSegmentation.BoxSize, GridSegmentation.BoxTolerance);

					 BrainBehavior.DungeonExplorer.Reset();
					 PrintNodeCounts("BrainBehavior.DungeonExplorer.Reset");

					 GridSegmentation.BoxSize=BoxSize;
					 GridSegmentation.BoxTolerance=BoxTolerance;
					 PrintNodeCounts("SetBoxSize+Tolerance");

					 BrainBehavior.DungeonExplorer.Update();
					 PrintNodeCounts("BrainBehavior.DungeonExplorer.Update");
				}
		  }

		  /// <summary>
		  /// The main profile behavior
		  /// </summary>
		  /// <returns></returns>
		  protected override Composite CreateBehavior()
		  {
				return
				new Sequence(
					 new DecoratorContinue(ret => !IgnoreMarkers,
						  FunkyTrinity.MiniMapMarker.DetectMiniMapMarkers(ExitNameHash)
					 ),
					 UpdateSearchGridProvider(),
					 new Action(ret => CheckResetDungeonExplorer()),
					 new PrioritySelector(
						  CheckIsObjectiveFinished(),
						  PrioritySceneCheck(),
						  new Decorator(ret => !IgnoreMarkers,
								FunkyTrinity.MiniMapMarker.VisitMiniMapMarkers(myPos, MarkerDistance)
						  ),
						  new Sequence(
								new DecoratorContinue(ret => !BrainBehavior.DungeonExplorer.CurrentRoute.Any(),
									 new Action(ret => UpdateRoute())
								),
								CheckIsExplorerFinished()
						  ),
						  new DecoratorContinue(ret => BrainBehavior.DungeonExplorer!=null&&BrainBehavior.DungeonExplorer.CurrentRoute.Any(),
								new PrioritySelector(
									 CheckNodeFinished(),
									 new Sequence(
										  new Action(ret => PrintNodeCounts("MainBehavior")),
										  new Action(ret => MoveToNextNode())
									 )
								)
						  )
					 )
				);
		  }


		  /// <summary>
		  /// Updates the search grid provider as needed
		  /// </summary>
		  /// <returns></returns>
		  private Composite UpdateSearchGridProvider()
		  {
				return
				new DecoratorContinue(ret => mySceneId!=ZetaDia.Me.SceneId||Vector3.Distance(myPos, GPUpdatePosition)>150,
					 new Sequence(
						  new Action(ret => mySceneId=ZetaDia.Me.SceneId),
						  new Action(ret => GPUpdatePosition=myPos),
					 //new Action(ret => FunkyTrinity.Funky.UpdateSearchGridProvider(true)),
						  new Action(ret => FunkyTrinity.MiniMapMarker.UpdateFailedMarkers())
					 )
				);
		  }

		  /// <summary>
		  /// Checks if we are using a timeout and will end the tag if the timer has breached the given value
		  /// </summary>
		  /// <returns></returns>
		  private Composite TimeoutCheck()
		  {
				return
				new PrioritySelector(
					 new Decorator(ret => timeoutBreached,
						  new Sequence(
								new DecoratorContinue(ret => TownPortalOnTimeout&&!ZetaDia.Me.IsInTown,
									 new Action(ret => isDone=true)
								),
								new DecoratorContinue(ret => !TownPortalOnTimeout,
									 new Action(ret => isDone=true)
								)
						  )
					 ),
					 new Decorator(ret => ExploreTimeoutType==TimeoutType.Timer,
						  new Action(ret => CheckSetTimer(ret))
					 ),
					 new Decorator(ret => ExploreTimeoutType==TimeoutType.GoldInactivity,
						  new Action(ret => CheckSetGoldInactive(ret))
					 )
				);
		  }

		  bool timeoutBreached=false;
		  Stopwatch TagTimer=new Stopwatch();
		  /// <summary>
		  /// Will start the timer if needed, and end the tag if the timer has exceeded the TimeoutValue
		  /// </summary>
		  /// <param name="ctx"></param>
		  /// <returns></returns>
		  private RunStatus CheckSetTimer(object ctx)
		  {
				if (!TagTimer.IsRunning)
				{
					 TagTimer.Start();
					 return RunStatus.Failure;
				}
				if (ExploreTimeoutType==TimeoutType.Timer&&TagTimer.Elapsed.TotalSeconds>TimeoutValue)
				{
					 //DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "TrinityExploreDungeon timer ended ({0}), tag finished!", TimeoutValue);
					 timeoutBreached=true;
					 return RunStatus.Success;
				}
				return RunStatus.Failure;
		  }

		  private int lastCoinage=-1;
		  /// <summary>
		  /// Will check if the bot has not picked up any gold within the allocated TimeoutValue
		  /// </summary>
		  /// <param name="ctx"></param>
		  /// <returns></returns>
		  private RunStatus CheckSetGoldInactive(object ctx)
		  {
				CheckSetTimer(ctx);
				if (lastCoinage==-1)
				{
					 lastCoinage=ZetaDia.Me.Inventory.Coinage;
					 return RunStatus.Failure;
				}
				else if (lastCoinage!=ZetaDia.Me.Inventory.Coinage)
				{
					 TagTimer.Restart();
					 return RunStatus.Failure;
				}
				else if (lastCoinage==ZetaDia.Me.Inventory.Coinage&&TagTimer.Elapsed.TotalSeconds>TimeoutValue)
				{
					 //DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "TrinityExploreDungeon gold inactivity timer tripped ({0}), tag finished!", TimeoutValue);
					 timeoutBreached=true;
					 return RunStatus.Success;
				}

				return RunStatus.Failure;
		  }

		  private int timesForcedReset=0;
		  private int timesForceResetMax=5;

		  /// <summary>
		  /// Checks to see if the tag is finished as needed
		  /// </summary>
		  /// <returns></returns>
		  private Composite CheckIsExplorerFinished()
		  {
				return
				new PrioritySelector(
					 CheckIsObjectiveFinished(),
					 new Decorator(ret => GetRouteUnvisitedNodeCount()==0&&timesForcedReset>timesForceResetMax,
						  new Sequence(
								new Action(ret => FunkyTrinity.Funky.CacheMovementTracking.bSkipAheadAGo=false),
								new Action(ret => isDone=true)
						  )
					 ),
					 new Decorator(ret => GetRouteUnvisitedNodeCount()==0,
						  new Sequence(
					 //new Action(ret => DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "Visited all nodes but objective not complete, forcing grid reset!")),
								new Action(ret => timesForcedReset++),
					 //new Action(ret => FunkyTrinity.Funky.hashSkipAheadAreaCache.Clear()),
								new Action(ret => FunkyTrinity.MiniMapMarker.KnownMarkers.Clear()),
								new Action(ret => ForceUpdateScenes()),
								new Action(ret => GridSegmentation.Reset()),
								new Action(ret => GridSegmentation.Update()),
								new Action(ret => BrainBehavior.DungeonExplorer.Reset()),
								new Action(ret => PriorityScenesInvestigated.Clear()),
								new Action(ret => UpdateRoute())
						  )
					 )
			  );
		  }

		  private void ForceUpdateScenes()
		  {
				foreach (Scene scene in ZetaDia.Scenes.GetScenes().ToList())
				{
					 scene.UpdatePointer(scene.BaseAddress);
				}
		  }

		  /// <summary>
		  /// Checks to see if the tag is finished as needed
		  /// </summary>
		  /// <returns></returns>
		  private Composite CheckIsObjectiveFinished()
		  {
				return
				new PrioritySelector(
					 TimeoutCheck(),
					 new Decorator(ret => EndType==TrinityExploreEndType.FullyExplored&&GetRouteUnvisitedNodeCount()==0,
						  new Sequence(
					 //new Action(ret => DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "Fully explored area! Tag Finished.", ExitNameHash)),
								new Action(ret => isDone=true)
						  )
					 ),
					 new Decorator(ret => EndType==TrinityExploreEndType.ExitFound&&ExitNameHash!=0&&IsExitNameHashVisible(),
						  new Sequence(
					 //new Action(ret => DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "Found exitNameHash {0}!", ExitNameHash)),
								new Action(ret => isDone=true)
						  )
					 ),
					 new Decorator(ret => EndType==TrinityExploreEndType.ObjectFound&&ActorId!=0&&ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false)
						  .Any(a => a.ActorSNO==ActorId&&a.Distance<=ObjectDistance),
						  new Sequence(
					 //new Action(ret => DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "Found Object {0}!", ActorId)),
								new Action(ret => isDone=true)
						  )
					 ),
					 new Decorator(ret => EndType==TrinityExploreEndType.ObjectFound&&AlternateActorsFound(),
						  new Sequence(
					 //new Action(ret => DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "Found Alternate Object {0}!", GetAlternateActor().ActorSNO)),
								new Action(ret => isDone=true)
						  )
					 ),
					 new Decorator(ret => EndType==TrinityExploreEndType.SceneFound&&ZetaDia.Me.SceneId==SceneId,
						  new Sequence(
					 //new Action(ret => DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "Found SceneId {0}!", SceneId)),
								new Action(ret => isDone=true)
						  )
					 ),
					 new Decorator(ret => EndType==TrinityExploreEndType.SceneFound&&ZetaDia.Me.CurrentScene.Name.ToLower().Contains(SceneName.ToLower()),
						  new Sequence(
					 //new Action(ret => DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "Found SceneName {0}!", SceneName)),
								new Action(ret => isDone=true)
						  )
					 ),
					 new Decorator(ret => ZetaDia.Me.IsInTown,
						  new Sequence(
					 //new Action(ret => DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "Cannot use TrinityExploreDungeon in town - tag finished!", SceneName)),
								new Action(ret => isDone=true)
						  )
					 )
				);
		  }

		  private bool AlternateActorsFound()
		  {
				return AlternateActors.Any()&&ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false)
						  .Where(o => AlternateActors.Any(a => a.ActorId==o.ActorSNO&&o.Distance<=a.ObjectDistance)).Any();
		  }

		  private DiaObject GetAlternateActor()
		  {
				return ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false)
						  .Where(o => AlternateActors.Any(a => a.ActorId==o.ActorSNO&&o.Distance<=a.ObjectDistance)).OrderBy(o => o.Distance).FirstOrDefault();
		  }

		  /// <summary>
		  /// Determine if the tag ExitNameHash is visible in the list of Current World Markers
		  /// </summary>
		  /// <returns></returns>
		  private bool IsExitNameHashVisible()
		  {
				return ZetaDia.Minimap.Markers.CurrentWorldMarkers.Any(m => m.NameHash==ExitNameHash&&m.Position.Distance2D(myPos)<=MarkerDistance+10f);
		  }

		  private Vector3 PrioritySceneTarget=Vector3.Zero;
		  private int PrioritySceneSNOId=-1;
		  private Scene CurrentPriorityScene=null;
		  private float PriorityScenePathPrecision=-1f;
		  /// <summary>
		  /// A list of Scene SNOId's that have already been investigated
		  /// </summary>
		  private List<int> PriorityScenesInvestigated=new List<int>();

		  private DateTime lastCheckedScenes=DateTime.MinValue;
		  /// <summary>
		  /// Will find and move to Prioritized Scene's based on Scene SNOId or Name
		  /// </summary>
		  /// <returns></returns>
		  private Composite PrioritySceneCheck()
		  {
				return
				new Decorator(ret => PriorityScenes!=null&&PriorityScenes.Any(),
					 new Sequence(
						  new DecoratorContinue(ret => DateTime.Now.Subtract(lastCheckedScenes).TotalMilliseconds>1000,
								new Sequence(
									 new Action(ret => lastCheckedScenes=DateTime.Now),
									 new Action(ret => FindPrioritySceneTarget())
								)
						  ),
						  new Decorator(ret => PrioritySceneTarget!=Vector3.Zero,
								new PrioritySelector(
									 new Decorator(ret => PrioritySceneTarget.Distance2D(myPos)<=PriorityScenePathPrecision,
										  new Action(ret => PrioritySceneMoveToFinished())
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
				//DbHelper.Log(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Moving to Priority Scene {0} - {1} Center {2} Distance {3:0}",
				//CurrentPriorityScene.Name, CurrentPriorityScene.SceneInfo.SNOId, PrioritySceneTarget, PrioritySceneTarget.Distance2D(myPos));

				MoveResult moveResult=FunkyTrinity.Funky.PlayerMover.NavigateTo(PrioritySceneTarget);

				if (moveResult==MoveResult.PathGenerationFailed||moveResult==MoveResult.ReachedDestination)
				{
					 // DbHelper.Log(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Unable to navigate to Scene {0} - {1} Center {2} Distance {3:0}, cancelling!",
					 //  CurrentPriorityScene.Name, CurrentPriorityScene.SceneInfo.SNOId, PrioritySceneTarget, PrioritySceneTarget.Distance2D(myPos));
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
				UpdateRoute();
		  }

		  /// <summary>
		  /// Finds a navigable point in a priority scene
		  /// </summary>
		  private void FindPrioritySceneTarget()
		  {
				if (!PriorityScenes.Any())
					 return;

				gp.Update();

				if (PrioritySceneTarget!=Vector3.Zero)
					 return;

				bool foundPriorityScene=false;

				// find any matching priority scenes in scene manager - match by name or SNOId

				List<Scene> PScenes=ZetaDia.Scenes.GetScenes()
					 .Where(s => PriorityScenes.Any(ps => ps.SceneId!=-1&&s.SceneInfo.SNOId==ps.SceneId)).ToList();

				PScenes.AddRange(ZetaDia.Scenes.GetScenes()
					  .Where(s => PriorityScenes.Any(ps => !String.IsNullOrEmpty(ps.SceneName.Trim())&&ps.SceneId==-1&&s.Name.ToLower().Contains(ps.SceneName.ToLower()))).ToList());

				List<Scene> foundPriorityScenes=new List<Scene>();
				Dictionary<int, Vector3> foundPrioritySceneIndex=new Dictionary<int, Vector3>();

				foreach (Scene scene in PScenes)
				{
					 if (PriorityScenesInvestigated.Contains(scene.SceneInfo.SNOId))
						  continue;

					 foundPriorityScene=true;

					 NavZone navZone=scene.Mesh.Zone;
					 NavZoneDef zoneDef=navZone.NavZoneDef;

					 Vector2 zoneMin=navZone.ZoneMin;
					 Vector2 zoneMax=navZone.ZoneMax;

					 Vector3 zoneCenter=GetNavZoneCenter(navZone);

					 List<NavCell> NavCells=zoneDef.NavCells.Where(c => c.Flags.HasFlag(NavCellFlags.AllowWalk)).ToList();

					 if (!NavCells.Any())
						  continue;

					 NavCell bestCell=NavCells.OrderBy(c => GetNavCellCenter(c.Min, c.Max, navZone).Distance2D(zoneCenter)).FirstOrDefault();

					 if (bestCell!=null)
					 {
						  foundPrioritySceneIndex.Add(scene.SceneInfo.SNOId, GetNavCellCenter(bestCell, navZone));
						  foundPriorityScenes.Add(scene);
					 }
					 else
					 {
						  //DbHelper.Log(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Found Priority Scene but could not find a navigable point!", true);
					 }
				}

				if (foundPrioritySceneIndex.Any())
				{
					 KeyValuePair<int, Vector3> nearestPriorityScene=foundPrioritySceneIndex.OrderBy(s => s.Value.Distance2D(myPos)).FirstOrDefault();

					 PrioritySceneSNOId=nearestPriorityScene.Key;
					 PrioritySceneTarget=nearestPriorityScene.Value;
					 CurrentPriorityScene=foundPriorityScenes.FirstOrDefault(s => s.SceneInfo.SNOId==PrioritySceneSNOId);
					 PriorityScenePathPrecision=GetPriorityScenePathPrecision(PScenes.FirstOrDefault(s => s.SceneInfo.SNOId==nearestPriorityScene.Key));

					 //DbHelper.Log(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Found Priority Scene {0} - {1} Center {2} Distance {3:0}",
					 //CurrentPriorityScene.Name, CurrentPriorityScene.SceneInfo.SNOId, PrioritySceneTarget, PrioritySceneTarget.Distance2D(myPos));
				}

				if (!foundPriorityScene)
				{
					 PrioritySceneTarget=Vector3.Zero;
				}
		  }

		  private float GetPriorityScenePathPrecision(Scene scene)
		  {
				return PriorityScenes.FirstOrDefault(ps => ps.SceneId!=0&&ps.SceneId==scene.SceneInfo.SNOId||scene.Name.ToLower().Contains(ps.SceneName.ToLower())).PathPrecision;
		  }

		  /// <summary>
		  /// Gets the center of a given Navigation Zone
		  /// </summary>
		  /// <param name="zone"></param>
		  /// <returns></returns>
		  private Vector3 GetNavZoneCenter(NavZone zone)
		  {
				float X=zone.ZoneMin.X+((zone.ZoneMax.X-zone.ZoneMin.X)/2);
				float Y=zone.ZoneMin.Y+((zone.ZoneMax.Y-zone.ZoneMin.Y)/2);

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
				float X=zone.ZoneMin.X+min.X+((max.X-min.X)/2);
				float Y=zone.ZoneMin.Y+min.Y+((max.Y-min.Y)/2);
				float Z=min.Z+((max.Z-min.Z)/2);

				return new Vector3(X, Y, Z);
		  }

		  /// <summary>
		  /// Checks to see if the current DungeonExplorer node is in an Ignored scene, and marks the node immediately visited if so
		  /// </summary>
		  /// <returns></returns>
		  private Composite CheckIgnoredScenes()
		  {
				return
				new Decorator(ret => IgnoreScenes!=null&&IgnoreScenes.Any(),
					 new PrioritySelector(
						  new Decorator(ret => PositionInsideIgnoredScene(CurrentNavTarget),
								new Sequence(
									 new Action(ret => SetNodeVisited("Node is in Ignored Scene"))
								)
						  )
					 )
				);
		  }

		  /// <summary>
		  /// Determines if a given Vector3 is in a provided IgnoreScene (if the scene is loaded)
		  /// </summary>
		  /// <param name="position"></param>
		  /// <returns></returns>
		  private bool PositionInsideIgnoredScene(Vector3 position)
		  {
				List<Scene> ignoredScenes=ZetaDia.Scenes.GetScenes()
					 .Where(scn => IgnoreScenes.Any(igscn => !String.IsNullOrEmpty(igscn.SceneName)&&scn.Name.ToLower().Contains(igscn.SceneName.ToLower()))||
						  IgnoreScenes.Any(igscn => scn.SceneInfo.SNOId==igscn.SceneId)&&
						  !PriorityScenes.Any(psc => !String.IsNullOrEmpty(psc.SceneName.Trim())&&scn.Name.ToLower().Contains(psc.SceneName))&&
						  !PriorityScenes.Any(psc => psc.SceneId!=-1&&scn.SceneInfo.SNOId!=psc.SceneId)).ToList();

				foreach (Scene scene in ignoredScenes)
				{
					 if (scene.Mesh.Zone==null)
						  return true;

					 Vector2 pos=position.ToVector2();
					 Vector2 min=scene.Mesh.Zone.ZoneMin;
					 Vector2 max=scene.Mesh.Zone.ZoneMax;

					 if (pos.X>=min.X&&pos.X<=max.X&&pos.Y>=min.Y&&pos.Y<=max.Y)
						  return true;
				}
				return false;
		  }

		  /// <summary>
		  /// Determines if the current node can be marked as Visited, and does so if needed
		  /// </summary>
		  /// <returns></returns>
		  private Composite CheckNodeFinished()
		  {
				return
				new PrioritySelector(
					 new Decorator(ret => LastMoveResult==MoveResult.ReachedDestination,
						  new Sequence(
								new Action(ret => SetNodeVisited("Reached Destination")),
								new Action(ret => LastMoveResult=MoveResult.Moved),
								new Action(ret => UpdateRoute())
						  )
					 ),
					 new Decorator(ret => GetRouteUnvisitedNodeCount()==0||!BrainBehavior.DungeonExplorer.CurrentRoute.Any(),
					 //new Action(ret => DbHelper.Log(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Error - CheckIsNodeFinished() called while Route is empty!")),
								new Action(ret => UpdateRoute())

					 ),
					 new Decorator(ret => CurrentNavTarget.Distance2D(myPos)<=PathPrecision,
						  new Sequence(
								new Action(ret => SetNodeVisited(String.Format("Node {0} is within PathPrecision ({1:0}/{2:0})",
									 CurrentNavTarget, CurrentNavTarget.Distance2D(myPos), PathPrecision))),
								new Action(ret => UpdateRoute())
						  )
					 ),
					 new Decorator(ret => CurrentNavTarget.Distance2D(myPos)<=90f&&!gp.CanStandAt(gp.WorldToGrid(CurrentNavTarget.ToVector2())),
						  new Sequence(
								new Action(ret => SetNodeVisited("Center Not Navigable")),
								new Action(ret => UpdateRoute())
						  )
					 ),
					 new Decorator(ret => FunkyTrinity.Funky.ObjectCache.Obstacles.IsPositionWithinAny(CurrentNavTarget),
						  new Sequence(
								new Action(ret => SetNodeVisited("Navigation obstacle detected at node point")),
								new Action(ret => UpdateRoute())
						  )
					 ),
					 new Decorator(ret => !FunkyTrinity.Funky.Bot.Character.currentMovementState.HasFlag(MovementState.WalkingInPlace|MovementState.None)&&myPos.Distance2D(CurrentNavTarget)<=50f&&!Navigator.Raycast(myPos, CurrentNavTarget),
						  new Sequence(
								new Action(ret => SetNodeVisited("Stuck moving to node point, marking done (in LoS and nearby!)")),
								new Action(ret => UpdateRoute())
						  )
					 ),
					 new Decorator(ret => FunkyTrinity.Funky.CacheMovementTracking.CheckPositionForSkipping(CurrentNavTarget),
						  new Sequence(
								new Action(ret => SetNodeVisited("Found node to be in skip ahead cache, marking done")),
								new Action(ret => UpdateRoute())
						  )
					 ),
					 CheckIgnoredScenes()
				);
		  }

		  /// <summary>
		  /// Updates the DungeonExplorer Route
		  /// </summary>
		  private void UpdateRoute()
		  {
				CheckResetDungeonExplorer();

				BrainBehavior.DungeonExplorer.Update();
				PrintNodeCounts("BrainBehavior.DungeonExplorer.Update");

				// Throw an exception if this shiz don't work
				ValidateCurrentRoute();
		  }

		  /// <summary>
		  /// Marks the current dungeon Explorer as Visited and dequeues it from the route
		  /// </summary>
		  /// <param name="reason"></param>
		  private void SetNodeVisited(string reason="")
		  {
				//DbHelper.Log(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Dequeueing current node {0} - {1}", BrainBehavior.DungeonExplorer.CurrentNode.NavigableCenter, reason);
				BrainBehavior.DungeonExplorer.CurrentNode.Visited=true;
				BrainBehavior.DungeonExplorer.CurrentRoute.Dequeue();
				PrintNodeCounts("SetNodeVisited");
		  }

		  /// <summary>
		  /// Makes sure the current route is not null! Bad stuff happens if it's null...
		  /// </summary>
		  private static void ValidateCurrentRoute()
		  {
				if (BrainBehavior.DungeonExplorer.CurrentRoute==null)
				{
					 throw new ApplicationException("DungeonExplorer CurrentRoute is null");
				}
		  }

		  /// <summary>
		  /// Prints a plethora of useful information about the Dungeon Exploration process
		  /// </summary>
		  /// <param name="step"></param>
		  private void PrintNodeCounts(string step="")
		  {
				/*
				if (GilesTrinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.ProfileTag))
				{
					 string nodeDistance=String.Empty;
					 if (GetRouteUnvisitedNodeCount()>0)
					 {
						  try
						  {
								float distance=BrainBehavior.DungeonExplorer.CurrentNode.NavigableCenter.Distance(myPos);

								if (distance>0)
									 nodeDistance=String.Format("Dist:{0:0}", Math.Round(distance/10f, 2)*10f);
						  } catch { }
					 }

					 var log=String.Format("Nodes [Unvisited: Route:{1} Grid:{3} | Grid-Visited: {2}] Box:{4}/{5} Step:{6} {7} Nav:{8} RayCast:{9} PP:{10:0} Dir: {11} ZDiff:{12:0}",
						  GetRouteVisistedNodeCount(),                                 // 0
						  GetRouteUnvisitedNodeCount(),                                // 1
						  GetGridSegmentationVisistedNodeCount(),                      // 2
						  GetGridSegmentationUnvisitedNodeCount(),                     // 3
						  GridSegmentation.BoxSize,                                    // 4
						  GridSegmentation.BoxTolerance,                               // 5
						  step,                                                        // 6
						  nodeDistance,                                                // 7
						  gp.CanStandAt(gp.WorldToGrid(CurrentNavTarget.ToVector2())), // 8
						  !Navigator.Raycast(myPos, CurrentNavTarget),
						  PathPrecision,
						  GilesTrinity.GetHeadingToPoint(CurrentNavTarget),
						  Math.Abs(myPos.Z-CurrentNavTarget.Z)
						  );

					 DbHelper.Log(TrinityLogLevel.Normal, LogCategory.ProfileTag, log);
				}
				*/
		  }

		  /*
			* Dungeon Explorer Nodes
			*/
		  /// <summary>
		  /// Gets the number of unvisited nodes in the DungeonExplorer Route
		  /// </summary>
		  /// <returns></returns>
		  private int GetRouteUnvisitedNodeCount()
		  {
				if (GetCurrentRouteNodeCount()>0)
					 return BrainBehavior.DungeonExplorer.CurrentRoute.Count(n => !n.Visited);
				else
					 return 0;
		  }

		  /// <summary>
		  /// Gets the number of visisted nodes in the DungeonExplorer Route
		  /// </summary>
		  /// <returns></returns>
		  private int GetRouteVisistedNodeCount()
		  {
				if (GetCurrentRouteNodeCount()>0)
					 return BrainBehavior.DungeonExplorer.CurrentRoute.Count(n => n.Visited);
				else
					 return 0;
		  }

		  /// <summary>
		  /// Gets the number of nodes in the DungeonExplorer Route
		  /// </summary>
		  /// <returns></returns>
		  private int GetCurrentRouteNodeCount()
		  {
				if (BrainBehavior.DungeonExplorer.CurrentRoute!=null)
					 return BrainBehavior.DungeonExplorer.CurrentRoute.Count();
				else
					 return 0;
		  }
		  /*
			*  Grid Segmentation Nodes
			*/
		  /// <summary>
		  /// Gets the number of Unvisited nodes as reported by the Grid Segmentation provider
		  /// </summary>
		  /// <returns></returns>
		  private int GetGridSegmentationUnvisitedNodeCount()
		  {
				if (GetGridSegmentationNodeCount()>0)
					 return GridSegmentation.Nodes.Count(n => !n.Visited);
				else
					 return 0;
		  }

		  /// <summary>
		  /// Gets the number of Visited nodes as reported by the Grid Segmentation provider
		  /// </summary>
		  /// <returns></returns>
		  private int GetGridSegmentationVisistedNodeCount()
		  {
				if (GetCurrentRouteNodeCount()>0)
					 return GridSegmentation.Nodes.Count(n => n.Visited);
				else
					 return 0;
		  }

		  /// <summary>
		  /// Gets the total number of nodes with the current BoxSize/Tolerance as reported by the Grid Segmentation Provider
		  /// </summary>
		  /// <returns></returns>
		  private int GetGridSegmentationNodeCount()
		  {
				if (GridSegmentation.Nodes!=null)
					 return GridSegmentation.Nodes.Count();
				else
					 return 0;
		  }

		  private MoveResult LastMoveResult=MoveResult.Moved;
		  private DateTime lastGeneratedPath=DateTime.MinValue;
		  /// <summary>
		  /// Moves the bot to the next DungeonExplorer node
		  /// </summary>
		  private void MoveToNextNode()
		  {
				NextNode=BrainBehavior.DungeonExplorer.CurrentNode;
				Vector3 moveTarget=NextNode.NavigableCenter;

				string nodeName=String.Format("{0} Distance: {1:0} Direction: {2}",
					 NextNode.NavigableCenter, NextNode.NavigableCenter.Distance(FunkyTrinity.Funky.Bot.Character.Position), FunkyTrinity.Funky.GetHeadingToPoint(NextNode.NavigableCenter));

				//Funky.RecordSkipAheadCachePoint();

				LastMoveResult=Navigator.MoveTo(CurrentNavTarget);
		  }
		  /// <summary>
		  /// Initizializes the profile tag and sets defaults as needed
		  /// </summary>
		  private void Init(bool forced=false)
		  {
				if (BoxSize==0)
					 BoxSize=15;

				if (BoxTolerance==0)
					 BoxTolerance=0.55f;

				if (PathPrecision==0)
					 PathPrecision=BoxSize/2f;

				float minPathPrecision=15f;

				if (PathPrecision<minPathPrecision)
					 PathPrecision=minPathPrecision;

				if (ObjectDistance==0)
					 ObjectDistance=40f;

				if (MarkerDistance==0)
					 MarkerDistance=25f;

				if (TimeoutValue==0)
					 TimeoutValue=900;

				//FunkyTrinity.Funky.hashSkipAheadAreaCache.Clear();
				FunkyTrinity.Funky.CacheMovementTracking.bSkipAheadAGo=true;
				PriorityScenesInvestigated.Clear();
				FunkyTrinity.MiniMapMarker.KnownMarkers.Clear();


				if (PriorityScenes==null)
					 PriorityScenes=new List<PrioritizeScene>();

				if (IgnoreScenes==null)
					 IgnoreScenes=new List<IgnoreScene>();

				if (AlternateActors==null)
					 AlternateActors=new List<AlternateActor>();

				if (!forced)
				{
					 /*
					 DbHelper.Log(TrinityLogLevel.Normal, LogCategory.ProfileTag,
						  "Initialized TrinityExploreDungeon: boxSize={0} boxTolerance={1:0.00} endType={2} timeoutType={3} timeoutValue={4} pathPrecision={5:0} sceneId={6} actorId={7} objectDistance={8} markerDistance={9} exitNameHash={10}",
						  GridSegmentation.BoxSize, GridSegmentation.BoxTolerance, EndType, ExploreTimeoutType, TimeoutValue, PathPrecision, SceneId, ActorId, ObjectDistance, MarkerDistance, ExitNameHash);
				*/
				}
				InitDone=true;
		  }


		  private bool isDone=false;
		  /// <summary>
		  /// When true, the next profile tag is used
		  /// </summary>
		  public override bool IsDone
		  {
				get
				{
					 bool done=(!IsActiveQuestStep||isDone);
					 if (done)
					 {
						  FunkyTrinity.Funky.CacheMovementTracking.bSkipAheadAGo=false;
					 }
					 return done;
				}
		  }

		  /// <summary>
		  /// Resets this profile tag to defaults
		  /// </summary>
		  public override void ResetCachedDone()
		  {
				isDone=false;
				InitDone=false;
				GridSegmentation.Reset();
				BrainBehavior.DungeonExplorer.Reset();
				FunkyTrinity.MiniMapMarker.KnownMarkers.Clear();
		  }
	 }



	 // Thanks Nesox for the XML loading tricks!
	 [ComVisible(false)]
	 [XmlElement("TrinityLoadOnce")]
	 public class TrinityLoadOnce : ProfileBehavior
	 {
		  internal static List<string> UsedProfiles=new List<string>();
		  string[] AvailableProfiles= { };
		  string NextProfileName=String.Empty;
		  string NextProfilePath=String.Empty;
		  string CurrentProfilePath=String.Empty;
		  string CurrentProfileName=String.Empty;
		  Random rand=new Random();
		  bool initialized=false;
		  private bool isDone=false;

		  public override bool IsDone
		  {
				get { return !IsActiveQuestStep||isDone; }
		  }

		  [XmlElement("ProfileList")]
		  public List<LoadProfileOnce> Profiles { get; set; }

		  public TrinityLoadOnce()
		  {
				if (Profiles==null)
					 Profiles=new List<LoadProfileOnce>();
				else if (Profiles.Count()==0)
					 Profiles=new List<LoadProfileOnce>();

				GameEvents.OnGameJoined+=TrinityLoadOnce_OnGameJoined;

		  }

		  ~TrinityLoadOnce()
		  {
				GameEvents.OnGameJoined-=TrinityLoadOnce_OnGameJoined;
		  }

		  void TrinityLoadOnce_OnGameJoined(object sender, EventArgs e)
		  {
				UsedProfiles=new List<string>();


		  }


		  private void Initialize()
		  {
				if (initialized)
					 return;

				if (Profiles==null)
					 Profiles=new List<LoadProfileOnce>();

				CurrentProfilePath=Path.GetDirectoryName(ProfileManager.CurrentProfile.Path);
				CurrentProfileName=Path.GetFileName(ProfileManager.CurrentProfile.Path);

				initialized=true;
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Sequence(
					 new Action(ret => Initialize()),
					 new Action(ret => Logging.WriteDiagnostic("TrinityLoadOnce: Found {0} Total Profiles, {1} Used Profiles, {2} Unused Profiles",
						  Profiles.Count(), UsedProfiles.Count(), Profiles.Where(p => !UsedProfiles.Contains(p.FileName)).Count())),
					 new Action(ret => AvailableProfiles=(from p in Profiles where !UsedProfiles.Contains(p.FileName)&&p.FileName!=CurrentProfileName select p.FileName).ToArray()),
					 new PrioritySelector(
						  new Decorator(ret => AvailableProfiles.Length==0,
								new Sequence(
									 new Action(ret => Logging.WriteDiagnostic("TrinityLoadOnce: All available profiles have been used!", true)),
									 new Action(ret => isDone=true)
								)
						  ),
						  new Decorator(ret => AvailableProfiles.Length>0,
								new Sequence(
									 new Action(ret => NextProfileName=AvailableProfiles[rand.Next(0, AvailableProfiles.Length-1)]),
									 new Action(ret => NextProfilePath=Path.Combine(CurrentProfilePath, NextProfileName)),
									 new PrioritySelector(
										  new Decorator(ret => File.Exists(NextProfilePath),
												new Sequence(
													 new Action(ret => Logging.WriteDiagnostic("TrinityLoadOnce: Loading next random profile: {0}", NextProfileName)),
													 new Action(ret => UsedProfiles.Add(NextProfileName)),
													 new Action(ret => ProfileManager.Load(NextProfilePath))
												)
										  ),
										  new Action(ret => Logging.WriteDiagnostic("TrinityLoadOnce: ERROR: Profile {0} does not exist!", NextProfilePath))
									 )
								)
						  ),
						  new Action(ret => Logging.WriteDiagnostic("TrinityLoadOnce: Unkown error", true))
					 )
			  );
		  }
		  public override void ResetCachedDone()
		  {
				initialized=false;
				isDone=false;
				base.ResetCachedDone();
		  }
	 }
	 [ComVisible(false)]
	 [XmlElement("LoadProfileOnce")]
	 public class LoadProfileOnce
	 {
		  [XmlAttribute("filename")]
		  [XmlAttribute("Filename")]
		  [XmlAttribute("FileName")]
		  [XmlAttribute("fileName")]
		  [XmlAttribute("profile")]
		  public string FileName { get; set; }

		  public LoadProfileOnce(string filename)
		  {
				this.FileName=filename;
		  }

		  public LoadProfileOnce()
		  {

		  }

		  public override string ToString()
		  {
				return FileName;
		  }
	 }


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
				lastMoveResult=FunkyTrinity.Funky.PlayerMover.NavigateTo(Position);

				if (lastMoveResult==MoveResult.PathGenerationFailed)
				{
					 isDone=true;
				}
		  }


		  public Vector3 MyPos { get { return ZetaDia.Me.Position; } }
		  private ISearchAreaProvider gp { get { return FunkyTrinity.Funky.mgp; } }
		  //private PathFinder pf { get { return GilesTrinity.pf; } }

		  public override void OnStart()
		  {
				float x=MyPos.X+OffsetX;
				float y=MyPos.Y+OffsetY;

				Position=new Vector3(x, y, gp.GetHeight(new Vector2(x, y)));

				if (PathPrecision==0)
					 PathPrecision=10f;

		  }
		  public override void OnDone()
		  {

		  }
	 }


	 // CheckScenes takes the value Scenes as string, and splits up IDs using " , " as delimiter
	 // If current scenes contains one of the given IDs, it will set the Scene value to the ID.
	 ///<summary>
	 ///Takes a given set of IDs which are seperated by ","
	 ///If any of these IDs are currently present in the scence cache then Scence will be set to this ID.
	 ///To check Scence, use <Scene>
	 ///</summary>
	 [ComVisible(false)]
	 [XmlElement("CheckScenes")]
	 public class CheckScenesTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  private string sIDs;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				string[] Values=sIDs.ToString().Split(",".ToCharArray());
				List<int> IDs=new List<int>();
				//Logging.Write("Checking: " + IDs.Count.ToString());

				for (int i=0; i<Values.Count(); i++)
				{
					 IDs.Add(Convert.ToInt32(Values[i]));
				}

				FunkyTrinity.Funky.ScenceCheck=0;

				foreach (Scene item in ZetaDia.Scenes.GetScenes())
				{
					 //Logging.Write("SID: " + item.SceneInfo.SNOId.ToString());
					 if (IDs.Contains(item.SceneInfo.SNOId))
					 {
						  //Logging.Write("Found Match");
						  FunkyTrinity.Funky.ScenceCheck=item.SceneInfo.SNOId;
					 }
				}

				return new Zeta.TreeSharp.Action(ret =>
				{
					 m_IsDone=true;
				});
		  }


		  [XmlAttribute("Scenes")]
		  public string iScenes
		  {
				get
				{
					 return sIDs;
				}
				set
				{
					 sIDs=value;
				}
		  }
	 }

	 [ComVisible(false)]
	 [XmlElement("RestartProfile")]
	 public class RestartProfileTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				string ReloadPath=null;

				return
					 new Zeta.TreeSharp.PrioritySelector(
					 new Zeta.TreeSharp.Decorator(ret => (DateTime.Now.Subtract(FunkyTrinity.Funky.LastProfileReload).TotalSeconds<5)==true,
						  new Zeta.TreeSharp.Action(ret => m_IsDone=true)
					  ),
					 new Zeta.TreeSharp.Decorator(ret => ZetaDia.IsInGame&&ZetaDia.Me.IsValid,
						  new Zeta.TreeSharp.Sequence(
								new Zeta.TreeSharp.Action(ret => ReloadPath=Zeta.CommonBot.ProfileManager.CurrentProfile.Path
								),
								new Zeta.TreeSharp.Action(ret => ProfileManager.Load(ReloadPath)
								),
								new Zeta.TreeSharp.Action(ret => m_IsDone=true)
						  )
					 ));
		  }
	 }

	 //Scene returns the value set if any by checkscenes.
	 ///<summary>
	 ///Is set to an ID when using CheckScences and a valid scence was found.
	 ///</summary>
	 [ComVisible(false)]
	 [XmlElement("Scene")]
	 public class SceneTag : ComplexNodeTag
	 {
		  private bool? bComplexDoneCheck;
		  private bool? bAlreadyCompleted;
		  private Func<bool> funcConditionalProcess;
		  private static Func<ProfileBehavior, bool> funcBehaviorProcess;
		  private int iUniqueID;

		  protected override Composite CreateBehavior()
		  {
				PrioritySelector decorated=new PrioritySelector(new Composite[0]);

				foreach (ProfileBehavior behavior in base.GetNodes())
				{
					 decorated.AddChild(behavior.Behavior);
				}
				// Logging.Write("Count: ");

				return new Zeta.TreeSharp.Decorator(new CanRunDecoratorDelegate(CheckNotAlreadyDone), decorated);
		  }

		  public bool GetConditionExec()
		  {
				if (FunkyTrinity.Funky.ScenceCheck==iUniqueID)
				{
					 return true;
				}
				return false;
		  }

		  private bool CheckNotAlreadyDone(object object_0)
		  {
				return !IsDone;
		  }

		  public override void ResetCachedDone()
		  {
				foreach (ProfileBehavior behavior in Body)
				{
					 behavior.ResetCachedDone();
				}
				bComplexDoneCheck=null;
		  }

		  private static bool CheckBehaviorIsDone(ProfileBehavior profileBehavior)
		  {
				return profileBehavior.IsDone;
		  }


		  [XmlAttribute("id")]
		  public int ID
		  {
				get
				{
					 return iUniqueID;
				}
				set
				{
					 iUniqueID=value;
				}
		  }


		  public Func<bool> Conditional
		  {
				get
				{
					 return funcConditionalProcess;
				}
				set
				{
					 funcConditionalProcess=value;
				}
		  }

		  public override bool IsDone
		  {
				get
				{
					 // Make sure we've not already completed this tag
					 if (bAlreadyCompleted.HasValue&&bAlreadyCompleted==true)
					 {
						  return true;
					 }
					 if (!bComplexDoneCheck.HasValue)
					 {
						  bComplexDoneCheck=new bool?(GetConditionExec());
					 }
					 if (bComplexDoneCheck==false)
					 {
						  return true;
					 }
					 if (funcBehaviorProcess==null)
					 {
						  funcBehaviorProcess=new Func<ProfileBehavior, bool>(CheckBehaviorIsDone);
					 }
					 bool bAllChildrenDone=Body.All<ProfileBehavior>(funcBehaviorProcess);
					 if (bAllChildrenDone)
					 {
						  bAlreadyCompleted=true;
					 }
					 return bAllChildrenDone;
				}
		  }
	 }

	 [ComVisible(false)]
	 [XmlElement("UseWaypointFunk")]
	 public class UseWaypointFunkTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.PrioritySelector(
					 //Init: Current Position / Current Level ID / Navigator Cleared
					 new Zeta.TreeSharp.Decorator(ret => !InitDone,
						  new Zeta.TreeSharp.Action(ret => Init())
					 ),
					 //Attempt to Update Actor, Failure == DONE! -- Check if level area has changed already!
					 new Zeta.TreeSharp.Decorator(ret => !SafeActorUpdate()||LevelAreaChanged(),
						  new Zeta.TreeSharp.PrioritySelector(
								new Zeta.TreeSharp.Decorator(ret => ZetaDia.IsLoadingWorld,
									 new Zeta.TreeSharp.Sleep(1000)),
								new Zeta.TreeSharp.Decorator(ret => !ZetaDia.IsLoadingWorld,
									 new Zeta.TreeSharp.Action(ret => m_IsDone=true)))
					 ),
					 //Check if we have not already made our movements to get into interact range. (Prevents moving during transition)
					 new Zeta.TreeSharp.Decorator(ret => !MovementToWaypointCompleted,
						  new Zeta.TreeSharp.PrioritySelector(
					 //Check if out of range and not moving
								new Zeta.TreeSharp.Decorator(ret => !WithinRangeOfInteraction()&&ShouldMove(),
									 new Zeta.TreeSharp.Action(ret => MoveToActor())
								),
					 //Check if in range but still moving
								new Zeta.TreeSharp.Decorator(ret => WithinRangeOfInteraction()&&!ShouldMove(),
									 new Zeta.TreeSharp.Action(ret => Navigator.PlayerMover.MoveStop())
								),
					 //Within Range and Not Moving, update our movement var so we don't move anymore again.
								new Zeta.TreeSharp.Decorator(ret => WithinRangeOfInteraction()&&ShouldMove(),
									 new Zeta.TreeSharp.Action(ret => MovementToWaypointCompleted=true))
								)
					  ),
					 //Check if in range and interaction should occur again!
					 new Zeta.TreeSharp.Decorator(ret => WithinRangeOfInteraction()&&ShouldAttemptInteraction()&&!LevelAreaChanged(),
						  new Zeta.TreeSharp.Action(ret => DoInteraction())
					 )
				);
		  }

		  private Vector3 CurrentPosition=Vector3.Zero;
		  private int CurrentLevelID;
		  private int CurrentWorldID;

		  private bool InitDone=false;
		  private void Init()
		  {
				Navigator.Clear();

				try
				{
					 CurrentLevelID=ZetaDia.CurrentLevelAreaId;
					 CurrentWorldID=ZetaDia.CurrentWorldId;
					 CurrentPosition=ZetaDia.Me.Position;
					 InitDone=true;
				} catch (Exception)
				{

				}
		  }

		  private GizmoWaypoint WaypointGizmo;
		  private bool SafeActorUpdate()
		  {
				var units=ZetaDia.Actors.GetActorsOfType<GizmoWaypoint>(false, false)
.Where(o => o.IsValid&&o.InLineOfSight)
.OrderBy(o => o.Distance);

				if (units.Count()==0)
					 return false;

				try
				{
					 WaypointGizmo=units.First();
					 return true;
				} catch (Exception)
				{

					 return false;
				}

		  }

		  private bool MovementToWaypointCompleted=false;


		  private bool WithinRangeOfInteraction()
		  {
				try
				{
					 if (WaypointGizmo.Distance>5f)
						  return false;
					 else
						  return true;
				} catch (Exception)
				{
					 return false;
				}
		  }

		  private bool LevelAreaChanged()
		  {
				int CurrentID=-1;
				int curWorldID=-1;
				try
				{
					 CurrentID=ZetaDia.CurrentLevelAreaId;
					 curWorldID=ZetaDia.CurrentWorldId;
				} catch (Exception)
				{
					 // return false;


				}

				if ((curWorldID==-1&&CurrentID==-1)||(CurrentID==CurrentLevelID&&CurrentWorldID==curWorldID))
					 return false;
				if (!(CurrentID==-1&&curWorldID==-1)&&(CurrentLevelID!=CurrentID||curWorldID!=CurrentWorldID))
					 return true;

				return true;
		  }

		  private bool ShouldMove()
		  {
				try
				{
					 return !ZetaDia.Me.Movement.IsMoving;
				} catch (Exception)
				{

					 return false;
				}
		  }

		  private MoveResult LastMoveResult;
		  private void MoveToActor()
		  {

				LastMoveResult=Navigator.MoveTo(WaypointGizmo.Position);

		  }

		  private DateTime LastInteraction=DateTime.MinValue;
		  private bool ShouldAttemptInteraction()
		  {
				if (DateTime.Now.Subtract(LastInteraction).TotalSeconds>5)
					 return true;
				else
					 return false;
		  }

		  private void DoInteraction()
		  {
				try
				{
					 Logging.Write("Using Waypoint -- WaypointID: "+waypointID);
					 WaypointGizmo.UseWaypoint(waypointID);
					 LastInteraction=DateTime.Now;

				} catch (Exception)
				{

				}

		  }


		  [XmlAttribute("waypointNumber")]
		  [XmlAttribute("waypoint")]
		  public int waypointID
		  {
				get;
				set;
		  }
	 }

	 [ComVisible(false)]
	 [XmlElement("SetProfileVar")]
	 public class SetProfileVarTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  private string sStartProfile;
		  private string sLastProfile;

		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  [XmlAttribute("startProfile")]
		  public string StartProfile
		  {
				get
				{
					 return sStartProfile;
				}
				set
				{
					 sStartProfile=value;
				}
		  }

		  [XmlAttribute("lastProfile")]
		  public string lastProfile
		  {
				get
				{
					 return sLastProfile;
				}
				set
				{
					 sLastProfile=value;
				}
		  }


		  protected override Composite CreateBehavior()
		  {
				string LogStatusUpdate="[Herbfunk] Setting Profile Vars";

				if (!String.IsNullOrEmpty(sStartProfile))
				{
					 FunkyTrinity.Funky.StartProfile=sStartProfile;
					 LogStatusUpdate+="\r\n\t"+"Starting Profile: "+sStartProfile;
				}

				if (!String.IsNullOrEmpty(sLastProfile))
				{
					 FunkyTrinity.Funky.LastProfile=sLastProfile;
					 LogStatusUpdate+="\r\n\t"+"Ending Profile: "+sLastProfile;
				}

				if (FunkyTrinity.Funky.ProfilesSets!=null&&FunkyTrinity.Funky.ProfilesSets.Count>0)
				{
					 FunkyTrinity.Funky.ProfilesSets.Clear();
					 LogStatusUpdate+="\r\n\t"+"Previous Profile Sets have been cleared.";
				}

				LogStatusUpdate+="\r\n"+"============FuNkY============";

				Logging.Write(LogStatusUpdate);

				return new Zeta.TreeSharp.Action(ret => m_IsDone=true);
		  }
	 }

	 [ComVisible(false)]
	 [XmlElement("NextProfile")]
	 public class NextProfileTag : ProfileBehavior
	 {
		  private bool town_=false;
		  [XmlAttribute("returntotown")]
		  [XmlAttribute("usetp")]
		  public bool town
		  {
				get
				{
					 return town_;
				}
				set
				{
					 town_=value;
				}
		  }

		  private bool m_IsDone=false;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  private string nextProfile=null;
		  private bool StartProfile=false;

		  private bool CheckedProfile=false;
		  private bool CheckProfile()
		  {
				Random R=new Random(DateTime.Now.Millisecond);


				string sCurrentProfilePath=Path.GetDirectoryName(Zeta.CommonBot.ProfileManager.CurrentProfile.Path);
				FunkyTrinity.Funky.ProfileSet tmp_ProfileSet=null;

				//Check if all random profiles in first element are null and remove and update the list.
				if (FunkyTrinity.Funky.ProfilesSets!=null&&FunkyTrinity.Funky.ProfilesSets.Count>0)
				{
					 tmp_ProfileSet=FunkyTrinity.Funky.ProfilesSets.First();
					 var ProfileTest=tmp_ProfileSet.Profiles.Where(s => !string.IsNullOrEmpty(s));
					 if (ProfileTest.Count()==0)
					 {
						  FunkyTrinity.Funky.ProfilesSets.RemoveAt(0);
						  FunkyTrinity.Funky.ProfilesSets.TrimExcess();

						  tmp_ProfileSet=null;
						  if (FunkyTrinity.Funky.ProfilesSets.Count()>0)
						  {
								tmp_ProfileSet=FunkyTrinity.Funky.ProfilesSets.FirstOrDefault();
								Logging.Write("[HerbFunk] Profile set completed. Next set is ready.");
						  }
						  else
						  {
								Logging.Write("[HerbFunk] Profile set completed.. could not find another set to run!");
						  }
					 }
				}

				if (tmp_ProfileSet!=null)
				{
					 if (tmp_ProfileSet.random)
					 {//Random
						  int TotalProfileCount=tmp_ProfileSet.Profiles.Count();
						  do
						  {
								int nextInt=R.Next(0, TotalProfileCount);
								if (!string.IsNullOrEmpty(tmp_ProfileSet.Profiles[nextInt]))
								{
									 nextProfile=tmp_ProfileSet.Profiles[nextInt];
									 tmp_ProfileSet.Profiles[nextInt]=null;
									 break;
								}
						  } while (true);
					 }
					 else
					 {//Normal
						  for (int i=0; i<tmp_ProfileSet.Profiles.Count(); i++)
						  {
								if (!string.IsNullOrEmpty(tmp_ProfileSet.Profiles[i]))
								{
									 nextProfile=tmp_ProfileSet.Profiles[i];
									 tmp_ProfileSet.Profiles[i]=null;
									 break;
								}
						  }
					 }
				}

				//Check if there is any random profile sets left.
				if (FunkyTrinity.Funky.ProfilesSets!=null&&FunkyTrinity.Funky.ProfilesSets.Count>0)
				{
					 // And prepare a full string of the path, and the new .xml file name
					 nextProfile=sCurrentProfilePath+@"\"+nextProfile;
				}
				else
				{
					 //No random profile sets left, so now we check for a last profile if vaild or a starting profile if valid.
					 if (String.IsNullOrEmpty(FunkyTrinity.Funky.LastProfile)&&!String.IsNullOrEmpty(FunkyTrinity.Funky.StartProfile))
					 {
						  nextProfile=sCurrentProfilePath+@"\"+FunkyTrinity.Funky.StartProfile;
						  StartProfile=true;
					 }
					 else if (!String.IsNullOrEmpty(FunkyTrinity.Funky.LastProfile))
					 {
						  nextProfile=sCurrentProfilePath+@"\"+FunkyTrinity.Funky.LastProfile;
						  FunkyTrinity.Funky.LastProfile=null;
					 }
				}


				if (String.IsNullOrEmpty(nextProfile))
				{
					 Logging.Write("Failed to load next profile, attempting to find Starting Profile..");
					 if (!String.IsNullOrEmpty(sCurrentProfilePath))
					 {
						  string profileSearch=null;
						  foreach (string item in System.IO.Directory.GetFiles(sCurrentProfilePath))
						  {
								if (item.ToLower().Contains("start"))
								{
									 profileSearch=item;
									 break;
								}
						  }
						  if (!String.IsNullOrEmpty(profileSearch))
						  {
								nextProfile=profileSearch;
								StartProfile=true;
								Logging.Write("Found a starting profile "+nextProfile+"\r\n"+" Will now restart game..");
						  }

					 }
				}
				CheckedProfile=true;
				return !String.IsNullOrEmpty(nextProfile);
		  }

		  private DateTime TimeSinceLoadedProfile=DateTime.Today;
		  private RunStatus FinishBehavior()
		  {
				if (DateTime.Now.Subtract(TimeSinceLoadedProfile).TotalSeconds<3)
					 return RunStatus.Running;

				ProfileManager.Load(nextProfile);

				m_IsDone=true;

				if (StartProfile)
					 ZetaDia.Service.Party.LeaveGame();

				return RunStatus.Success;
		  }


		  protected override Composite CreateBehavior()
		  {


				return
					 new Zeta.TreeSharp.PrioritySelector(
						  new Zeta.TreeSharp.Decorator(ret => !CheckedProfile,
								new Zeta.TreeSharp.Action(ret => CheckProfile())),
						  new Zeta.TreeSharp.Decorator(ret => ((StartProfile||town)&&Funky.FunkyTPOverlord(ret)),
								new Zeta.TreeSharp.Sequence(
								new Zeta.TreeSharp.Action(ret => Funky.FunkyTPBehavior(ret)),
								new Zeta.TreeSharp.Action(ret => Funky.ResetTPBehavior()))),
						  new Zeta.TreeSharp.Sequence(
								new Zeta.TreeSharp.Action(ret => Logging.Write("[Funky] Loading next profile {0}", nextProfile.ToString())),
								new Zeta.TreeSharp.Action(ret => TimeSinceLoadedProfile=DateTime.Now),
								new Zeta.TreeSharp.Action(ret => FinishBehavior())));
		  }

		  public override void ResetCachedDone()
		  {
				TimeSinceLoadedProfile=DateTime.Today;
				CheckedProfile=false;
				StartProfile=false;
				nextProfile=null;
				town_=false;
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }

	 [ComVisible(false)]
	 [XmlElement("AddProfileSet")]
	 public class AddRandomProfileSetTag : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.Action(ret => AddNewProfileEntry());
		  }

		  private void AddNewProfileEntry()
		  {

				FunkyTrinity.Funky.ProfilesSets.Add(new FunkyTrinity.Funky.ProfileSet(bRandom, sProfiles));
				Logging.Write("Added profile set. Is using Random: "+bRandom);

				m_IsDone=true;
		  }

		  private bool randomB=false;
		  [XmlAttribute("random")]
		  [XmlAttribute("randomize")]
		  public bool bRandom
		  {
				get
				{
					 return randomB;
				}
				set
				{
					 randomB=value;
				}
		  }

		  [XmlAttribute("Profiles")]
		  [XmlAttribute("profiles")]
		  public string sProfiles { get; set; }
	 }

	 [XmlElement("TrinityRandomWait")]
	 public class TrinityRandomWait : ProfileBehavior
	 {
		  private bool isDone=false;
		  private int minDelay;
		  private int maxDelay;
		  private int delay;
		  private string statusText;
		  private Stopwatch timer=new Stopwatch();

		  public override bool IsDone
		  {
				get { return isDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				Sequence RandomWaitSequence=new Sequence(
					 new Action(ret => delay=new Random().Next(minDelay, maxDelay)),
					 new Action(ret => statusText=String.Format("[XML Tag] Trinity Random Wait - Taking a break for {0:3} seconds.", delay)),
					 new Action(ret => BotMain.StatusText=statusText),
					 new Action(ctx => DoRandomWait(ctx)),
					 new Action(ret => isDone=true)
				);

				return RandomWaitSequence;
		  }

		  private RunStatus DoRandomWait(object ctx)
		  {
				if (!timer.IsRunning)
				{
					 timer.Start();
					 return RunStatus.Running;
				}
				else if (timer.IsRunning&&timer.ElapsedMilliseconds<delay)
				{
					 return RunStatus.Running;
				}
				else
				{
					 timer.Reset();
					 return RunStatus.Success;
				}
		  }


		  [XmlAttribute("min")]
		  public int min
		  {
				get
				{
					 return minDelay;
				}
				set
				{
					 minDelay=value;
				}
		  }
		  [XmlAttribute("max")]
		  public int max
		  {
				get
				{
					 return maxDelay;
				}
				set
				{
					 maxDelay=value;
				}
		  }

		  public override void ResetCachedDone()
		  {
				isDone=false;
				base.ResetCachedDone();
		  }
	 }
}
