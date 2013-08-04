using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace FunkyTrinity.XMLTags
{
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
}