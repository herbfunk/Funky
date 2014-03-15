using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using FunkyBot;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace Trinity.XmlTags
{
	 // TrinityLoadProfile will load the specified XML profile up
	 [XmlElement("TrinityLoadProfile")]
	 public class TrinityLoadProfile : ProfileBehavior
	 {
		  private bool m_IsDone;

		 public override bool IsDone
		  {
				get { return m_IsDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				return new Action(ret =>
				{
					 var bExitGame=Exit!=null&&Exit.ToLower()=="true";
					 var sThisProfileString=File;

					 // See if there are multiple profile choices, if so split them up and pick a random one
					 if (sThisProfileString.Contains("!"))
					 {
						  string[] sProfileChoices;
						  sProfileChoices=sThisProfileString.Split(new[] { "!" }, StringSplitOptions.None);
						  var rndNum=new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
						  var iChooseProfile=rndNum.Next(sProfileChoices.Count());
						  sThisProfileString=sProfileChoices[iChooseProfile];
					 }

					 // Now calculate our current path by checking the currently loaded profile
					 var sCurrentProfilePath=Path.GetDirectoryName(GlobalSettings.Instance.LastProfile);

					 // And prepare a full string of the path, and the new .xml file name
					 var sNextProfile=sCurrentProfilePath+@"\"+sThisProfileString;
					 Logger.DBLog.InfoFormat("Loading new profile.");
					 ProfileManager.Load(sNextProfile);

					 // A quick nap-time helps prevent some funny issues
					 if (NoDelay==null||NoDelay.ToLower()!="true")
						  Thread.Sleep(3000);
					 else
						  Thread.Sleep(300);

					 // See if the XML tag requested we exit the game after loading this profile or not
					 if (bExitGame)
					 {
						  Logger.DBLog.InfoFormat("Exiting game to continue with next profile.");

						  // Attempt to teleport to town first for a quicker exit
						  var iSafetyLoops=0;
						  while (!ZetaDia.IsInTown)
						  {
								iSafetyLoops++;
								Bot.Character.Data.WaitWhileAnimating(5, true);
								ZetaDia.Me.UsePower(SNOPower.UseStoneOfRecall, Bot.Character.Data.Position, ZetaDia.Me.WorldDynamicId);
								Thread.Sleep(1000);
								Bot.Character.Data.WaitWhileAnimating(1000, true);
								if (iSafetyLoops>5)
									 break;
						  }
						  Thread.Sleep(1000);
						  ZetaDia.Service.Party.LeaveGame();
						  EventHandlers.FunkyOnLeaveGame(null, null);

						  // Wait for 10 second log out timer if not in town, else wait for 3 seconds instead
						  Thread.Sleep(!Bot.Character.Data.bIsInTown?10000:3000);
					 }

					 // Check if we want to restart the game
					 m_IsDone=true;
				});
		  }

		 [XmlAttribute("exit")]
		 public string Exit { get; set; }

		 [XmlAttribute("nodelay")]
		 public string NoDelay { get; set; }

		 [XmlAttribute("file")]
		 public string File { get; set; }

		 public override void ResetCachedDone()
		  {
				m_IsDone=false;
				base.ResetCachedDone();
		  }
	 }
}