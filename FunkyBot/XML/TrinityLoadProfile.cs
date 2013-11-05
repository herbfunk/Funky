using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using FunkyBot;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Profile;
using Zeta.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.XmlTags
{
	 // TrinityLoadProfile will load the specified XML profile up
	 [XmlElement("TrinityLoadProfile")]
	 public class TrinityLoadProfile : ProfileBehavior
	 {
		  private bool m_IsDone=false;
		  private string sFileName;
		  private string sExitString;
		  private string sNoDelay;

		  public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Zeta.TreeSharp.Action(ret =>
				{
					 bool bExitGame=Exit!=null&&Exit.ToLower()=="true";
					 string sThisProfileString=File;

					 // See if there are multiple profile choices, if so split them up and pick a random one
					 if (sThisProfileString.Contains("!"))
					 {
						  string[] sProfileChoices;
						  sProfileChoices=sThisProfileString.Split(new string[] { "!" }, StringSplitOptions.None);
						  Random rndNum=new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
						  int iChooseProfile=rndNum.Next(sProfileChoices.Count());
						  sThisProfileString=sProfileChoices[iChooseProfile];
					 }

					 // Now calculate our current path by checking the currently loaded profile
					 string sCurrentProfilePath=Path.GetDirectoryName(Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile);

					 // And prepare a full string of the path, and the new .xml file name
					 string sNextProfile=sCurrentProfilePath+@"\"+sThisProfileString;
					 Logging.Write("Loading new profile.");
					 ProfileManager.Load(sNextProfile);

					 // A quick nap-time helps prevent some funny issues
					 if (NoDelay==null||NoDelay.ToLower()!="true")
						  Thread.Sleep(3000);
					 else
						  Thread.Sleep(300);

					 // See if the XML tag requested we exit the game after loading this profile or not
					 if (bExitGame)
					 {
						  Logging.Write("Exiting game to continue with next profile.");

						  // Attempt to teleport to town first for a quicker exit
						  int iSafetyLoops=0;
						  while (!ZetaDia.Me.IsInTown)
						  {
								iSafetyLoops++;
								Bot.Character.WaitWhileAnimating(5, true);
								ZetaDia.Me.UsePower(SNOPower.UseStoneOfRecall, Bot.Character.Position, ZetaDia.Me.WorldDynamicId, -1);
								Thread.Sleep(1000);
								FunkyBot.Bot.Character.WaitWhileAnimating(1000, true);
								if (iSafetyLoops>5)
									 break;
						  }
						  Thread.Sleep(1000);
						  ZetaDia.Service.Party.LeaveGame();
						  Funky.FunkyOnLeaveGame(null, null);

						  // Wait for 10 second log out timer if not in town, else wait for 3 seconds instead
						  Thread.Sleep(!Bot.Character.bIsInTown?10000:3000);
					 }

					 // Check if we want to restart the game
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

		  public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }
}